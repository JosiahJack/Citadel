using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class DebugGUI : MonoBehaviour
{
    // Ensure an instance is present
    private static DebugGUI _instance;
    private static DebugGUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DebugGUI>();

                if (_instance == null && Application.isPlaying)
                {
                    _instance = new GameObject("DebugGUI").AddComponent<DebugGUI>();
                }
            }
            return _instance;
        }
    }

    const int graphWidth = 300;
    const int graphHeight = 100;
    const float temporaryLogLifetime = 5f;

    // Show logs and graphs in build?
    [SerializeField] bool drawInBuild = false;

    [SerializeField] bool displayGraphs = true;
    [SerializeField] bool displayLogs = true;

    [SerializeField] Color backgroundColor = new Color(0f, 0f, 0f, 0.7f);

    [Header("Runtime Debugging Only")]
    [SerializeField] List<GraphContainer> graphs = new List<GraphContainer>();

    Dictionary<object, string> persistentLogs = new Dictionary<object, string>();
    Queue<TransientLog> transientLogs = new Queue<TransientLog>();
    Dictionary<object, GraphContainer> graphDictionary = new Dictionary<object, GraphContainer>();

    GUIStyle minMaxTextStyle;
    GUIStyle boxStyle;

    // On mouse down on graph, stop moving it
    bool freezeGraphs;

    static bool LogsEnabled { get { return Instance.displayLogs && (Instance.drawInBuild || Application.isEditor); } }
    static bool GraphsEnabled { get { return Instance.displayGraphs && (Instance.drawInBuild || Application.isEditor); } }

    /// <summary>
    /// Create or update an existing message with the same key.
    /// </summary>
    public static void LogPersistent(object key, object message)
    {
        if (LogsEnabled)
            Instance.InstanceLogPersistent(key, message);
    }

    /// <summary>
    /// Remove an existing persistent message.
    /// </summary>
    public static void RemovePersistent(object key)
    {
        if (LogsEnabled)
            Instance.InstanceRemovePersistent(key);
    }

    /// <summary>
    /// Clears all persistent logs.
    /// </summary>
    public static void ClearPersistent()
    {
        if (LogsEnabled)
            Instance.InstanceClearPersistent();
    }

    /// <summary>
    /// Print a temporary message.
    /// </summary>
    public static void Log(object message)
    {
        if (LogsEnabled)
            Instance.InstanceLog(message.ToString());
    }

    /// <summary>
    /// Set the properties of a graph.
    /// </summary>
    /// <param name="key">The graph's key</param>
    /// <param name="label">The graph's label</param>
    /// <param name="min">Value at the bottom of the graph box</param>
    /// <param name="max">Value at the top of the graph box</param>
    /// <param name="group">The graph's ordinal position on screen</param>
    /// <param name="color">The graph's color</param>
    public static void SetGraphProperties(object key, string label, float min, float max, int group, Color color, bool autoScale)
    {
        if (GraphsEnabled)
            Instance.InstanceSetGraphProperties(key, label, min, max, group, color, autoScale);
    }

    /// <summary>
    /// Add a data point to a graph.
    /// </summary>
    /// <param name="key">The graph's key</param>
    /// <param name="val">Value to be added</param>
    public static void Graph(object key, float val)
    {
        if (GraphsEnabled)
            Instance.InstanceGraph(key, val);
    }

    /// <summary>
    /// Remove an existing graph.
    /// </summary>
    /// <param name="key">The graph's key</param>
    public static void RemoveGraph(object key)
    {
        if (GraphsEnabled)
            Instance.InstanceRemoveGraph(key);
    }

    /// <summary>
    /// Resets a graph's data.
    /// </summary>
    /// <param name="key">The graph's key</param>
    public static void ClearGraph(object key)
    {
        if (GraphsEnabled)
            Instance.InstanceClearGraph(key);
    }

    private void InstanceLogPersistent(object key, object message)
    {
        if (persistentLogs.ContainsKey(key))
            persistentLogs[key] = message.ToString();
        else
            persistentLogs.Add(key, message.ToString());
    }

    private void InstanceRemovePersistent(object key)
    {
        if (persistentLogs.ContainsKey(key))
            persistentLogs.Remove(key);
    }

    private void InstanceClearPersistent()
    {
        persistentLogs.Clear();
    }

    private void InstanceRemoveGraph(object key)
    {
        if (graphDictionary.ContainsKey(key))
        {
            var graph = graphDictionary[key];
            graph.DestroyTextures();
            graphs.Remove(graph);
            graphDictionary.Remove(key);
        }
    }

    private void InstanceClearGraph(object key)
    {
        if (graphDictionary.ContainsKey(key))
            graphDictionary[key].Clear();
    }

    private void InstanceLog(string str)
    {
        transientLogs.Enqueue(new TransientLog(str, temporaryLogLifetime));
    }

    private void InstanceGraph(object key, float val)
    {
        if (!graphDictionary.ContainsKey(key))
        {
            InstanceCreateGraph(key);
        }

        if (freezeGraphs) return;

        graphDictionary[key].Push(val);
    }

    private void InstanceSetGraphProperties(object key, string label, float min, float max, int group, Color color, bool autoScale)
    {
        if (!graphDictionary.ContainsKey(key))
            InstanceCreateGraph(key);

        var graph = graphDictionary[key];
        graph.name = label;
        graph.SetMinMax(min, max);
        graph.group = Mathf.Max(0, group);
        graph.color = color;
        graph.autoScale = autoScale;
    }

    private void InstanceCreateGraph(object key)
    {
        graphDictionary.Add(key, new GraphContainer(graphWidth, graphHeight));
        graphs.Add(graphDictionary[key]);
    }

    void Awake()
    {
        if (!drawInBuild && !Application.isEditor) return;

        InitializeGUIStyles();
        RegisterAttributes();
    }

    void Update()
    {
        // Clean up attributes of deleted monobehaviours
        if (LogsEnabled || GraphsEnabled)
        {
            CleanUpDeletedAtributes();
        }

        if (LogsEnabled)
        {
            // Clean up expired logs
            while (transientLogs.Count > 0 && transientLogs.Peek().expiryTime <= Time.realtimeSinceStartup)
            {
                transientLogs.Dequeue();
            }
        }
        if (GraphsEnabled)
        {
            if (freezeGraphs) return;

            // Poll graph attributes
            for (int i = 0; i < attributeContainers.Count; i++)
            {
                MonoBehaviour mb = attributeContainers[i];
                if (mb != null && attributeKeys.ContainsKey(mb))
                {
                    if (!mb.enabled) continue;

                    foreach (var key in attributeKeys[mb])
                    {
                        if (key.memberInfo is FieldInfo)
                        {
                            FieldInfo fieldInfo = key.memberInfo as FieldInfo;
                            float? val = fieldInfo.GetValue(mb) as float?;
                            if (val != null)
                                graphDictionary[key].Push(val.Value);
                        }
                        else if (key.memberInfo is PropertyInfo)
                        {
                            PropertyInfo propertyInfo = key.memberInfo as PropertyInfo;
                            float? val = propertyInfo.GetValue(mb, null) as float?;
                            if (val != null)
                                graphDictionary[key].Push(val.Value);
                        }
                    }
                }
            }
        }
    }

    void OnGUI()
    {
        GUI.color = Color.white;

        if (LogsEnabled)
            DrawLogs();

        if (GraphsEnabled)
            DrawGraphs();
    }

    Texture2D boxTexture;
    void InitializeGUIStyles()
    {
        minMaxTextStyle = new GUIStyle();
        minMaxTextStyle.fontSize = 10;
        minMaxTextStyle.fontStyle = FontStyle.Bold;

        Color[] pix = new Color[4];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = Color.white;
        }
        boxTexture = new Texture2D(2, 2);
        boxTexture.SetPixels(pix);
        boxTexture.Apply();

        boxStyle = new GUIStyle();
        boxStyle.normal.background = boxTexture;
    }

    const float minMaxTextHeight = 8f;
    const float nextLineHeight = 15f;
    GUIContent labelGuiContent = new GUIContent();
    float textWidth;
    Rect textRect;
    void DrawLogs()
    {
        GUI.backgroundColor = backgroundColor;
        GUI.Box(new Rect(0, 0, textWidth + 10, textRect.y + 5), "", boxStyle);

        textRect = new Rect(5, 0, Screen.width, Screen.height);
        textWidth = 0;

        // Attributes
        for (int i = 0; i < attributeContainers.Count; i++)
        {
            MonoBehaviour mb = attributeContainers[i];
            if (mb != null)
            {
                if (!mb.enabled) continue;

                Type type = mb.GetType();
                if (debugGUIPrintFields.ContainsKey(type))
                {
                    foreach (var field in debugGUIPrintFields[type])
                    {
                        DrawLabel(string.Format("{0} {1}: {2}", mb.name, field.Name, field.GetValue(mb)));
                    }
                }
                if (debugGUIPrintProperties.ContainsKey(type))
                {
                    foreach (var property in debugGUIPrintProperties[type])
                    {
                        DrawLabel(string.Format("{0} {1}: {2}", mb.name, property.Name, property.GetValue(mb, null)));
                    }
                }
            }
        }

        foreach (var log in persistentLogs.Values)
        {
            DrawLabel(log);
        }

        if (textRect.y != 0 && transientLogs.Count != 0)
        {
            DrawLabel("-------------------");
        }

        foreach (var log in transientLogs)
        {
            DrawLabel(log.text);
        }

        // Clear up transient logs going off screen
        while (textRect.y > Screen.height && transientLogs.Count > 0)
        {
            transientLogs.Dequeue();
            textRect.y -= nextLineHeight;
        }
    }

    void DrawLabel(string label)
    {
        labelGuiContent.text = label;
        GUI.Label(textRect, labelGuiContent);
        textRect.y += nextLineHeight;
        textWidth = Mathf.Max(textWidth, GUIStyle.none.CalcSize(labelGuiContent).x);
    }

    HashSet<int> graphGroupBoxesDrawn = new HashSet<int>();
    float graphLabelWidth;
    StringBuilder stringBuilder = new StringBuilder();
    void DrawGraphs()
    {
        float graphBlockHeight = (graphHeight + 3);
        float graphBlockWidth = (graphWidth + 3);
        GUI.backgroundColor = backgroundColor;

        // Boxes for the graph labels
        foreach (var group in graphGroupBoxesDrawn)
        {
            GUI.Box(new Rect(
                Screen.width - graphBlockWidth - graphLabelWidth - 5,
                group * graphBlockHeight,
                graphLabelWidth + 5,
                graphHeight
            ), "", boxStyle);
        }

        graphLabelWidth = 0;
        graphGroupBoxesDrawn.Clear();

        // Boxes for the graphs themselves
        foreach (GraphContainer graph in graphDictionary.Values)
        {
            if (graphGroupBoxesDrawn.Add(graph.group))
            {
                GUI.Box(new Rect(Screen.width - graphWidth, 0 + graphBlockHeight * graph.group, graphWidth, graphHeight), "", boxStyle);
            }

            graph.Draw(new Rect(Screen.width - graphWidth, 0 + graphBlockHeight * graph.group, graphWidth, graphHeight));
        }

        foreach (int group in graphGroupBoxesDrawn)
        {
            float groupOrigin = group * graphBlockHeight;
            float yOffset = groupOrigin + minMaxTextHeight;
            float minMaxXOffset = 0;
            foreach (GraphContainer graph in graphDictionary.Values)
            {
                labelGuiContent.text = "";
                if (graph.group == group)
                {
                    minMaxTextStyle.normal.textColor = graph.color;
                    GUI.color = Color.white;

                    string minText = graph.min.ToString("F2");
                    string maxText = graph.max.ToString("F2");
                    Vector2 textSize = minMaxTextStyle.CalcSize(labelGuiContent);
                    labelGuiContent.text = minText;
                    float width = textSize.x;
                    labelGuiContent.text = maxText;
                    width = Mathf.Max(width, minMaxTextStyle.CalcSize(labelGuiContent).x);

                    // Max
                    labelGuiContent.text = graph.max.ToString("F2");
                    minMaxXOffset += width + 5;
                    GUI.Label(new Rect(Screen.width - graphWidth - minMaxXOffset, groupOrigin, minMaxXOffset, graphHeight), labelGuiContent, minMaxTextStyle);
                    // Min
                    labelGuiContent.text = graph.min.ToString("F2");
                    GUI.Label(new Rect(Screen.width - graphWidth - minMaxXOffset, groupOrigin + graphHeight - textSize.y, minMaxXOffset, graphHeight), labelGuiContent, minMaxTextStyle);

                    GUI.color = graph.color;
                    // Name
                    labelGuiContent.text = graph.name;
                    float xOffset = GUIStyle.none.CalcSize(labelGuiContent).x + 5;

                    graphLabelWidth = Mathf.Max(xOffset, graphLabelWidth, minMaxXOffset);

                    GUI.Label(new Rect(Screen.width - graphWidth - xOffset, yOffset, xOffset, graphHeight), labelGuiContent);
                    yOffset += nextLineHeight;
                }
            }
        }

        // Draw value at mouse position
        var mousePos = Input.mousePosition;
        mousePos.y = Screen.height - mousePos.y;

        if (freezeGraphs && !Input.GetMouseButton(0))
            freezeGraphs = false;

        foreach (var group in graphGroupBoxesDrawn)
        {
            // aabb check for mouse
            if (
                mousePos.x < Screen.width && mousePos.x > Screen.width - graphWidth &&
                mousePos.y > group * graphBlockHeight && mousePos.y < group * graphBlockHeight + graphHeight
            )
            {
                if (Input.GetMouseButtonDown(0))
                    freezeGraphs = true;

                // Line
                GUI.backgroundColor = new Color(1, 1, 0, 0.75f);
                GUI.color = new Color(1, 1, 0, 0.75f);
                GUI.Box(new Rect(mousePos.x, group * graphBlockHeight, 1, graphHeight), "", boxStyle);

                // Background box
                GUI.backgroundColor = new Color(0, 0, 0, 0.5f);
                GUI.color = Color.white;
                GUI.Box(new Rect(mousePos.x - 60, group * graphBlockHeight, 55, 55), "", boxStyle);

                int graphMousePos = (int)(Screen.width - mousePos.x);
                
                float yOffset = 0;
                foreach (GraphContainer graph in graphDictionary.Values)
                {
                    if (graph.group == group)
                    {
                        minMaxTextStyle.normal.textColor = graph.color;
                        GUI.color = Color.white;
                        labelGuiContent.text = graph.GetValue(graphMousePos).ToString("F3");
                        GUI.Label(new Rect(mousePos.x + -55, group * graphBlockHeight + yOffset, 45, 50), labelGuiContent, minMaxTextStyle);
                        yOffset += 8f;
                    }
                }
                break;
            }
        }
    }

    // Instances with debugGUIPrint fields
    List<MonoBehaviour> attributeContainers = new List<MonoBehaviour>();
    Dictionary<Type, HashSet<FieldInfo>> debugGUIPrintFields = new Dictionary<Type, HashSet<FieldInfo>>();
    Dictionary<Type, HashSet<PropertyInfo>> debugGUIPrintProperties = new Dictionary<Type, HashSet<PropertyInfo>>();
    // Instances with debugGUIGraph fields
    Dictionary<Type, HashSet<FieldInfo>> debugGUIGraphFields = new Dictionary<Type, HashSet<FieldInfo>>();
    Dictionary<Type, HashSet<PropertyInfo>> debugGUIGraphProperties = new Dictionary<Type, HashSet<PropertyInfo>>();

    Dictionary<Type, int> typeInstanceCounts = new Dictionary<Type, int>();

    Dictionary<MonoBehaviour, List<AttributeKey>> attributeKeys = new Dictionary<MonoBehaviour, List<AttributeKey>>();

    public static void ForceReinitializeAttributes()
    {
        // Clean up graphs
        List<object> toRemove = new List<object>();
        foreach (var key in Instance.graphDictionary.Keys)
        {
            if (key is AttributeKey)
                toRemove.Add(key);
        }
        foreach (var key in toRemove)
        {
            Instance.InstanceRemoveGraph(key);
        }
        toRemove.Clear();
        foreach (var key in Instance.persistentLogs.Keys)
        {
            if (key is AttributeKey)
                toRemove.Add(key);
        }
        foreach (var key in toRemove)
        {
            Instance.persistentLogs.Remove(key);
        }

        Instance.attributeContainers = new List<MonoBehaviour>();
        Instance.debugGUIPrintFields = new Dictionary<Type, HashSet<FieldInfo>>();
        Instance.debugGUIPrintProperties = new Dictionary<Type, HashSet<PropertyInfo>>();
        Instance.debugGUIGraphFields = new Dictionary<Type, HashSet<FieldInfo>>();
        Instance.debugGUIGraphProperties = new Dictionary<Type, HashSet<PropertyInfo>>();
        Instance.typeInstanceCounts = new Dictionary<Type, int>();

        Instance.attributeKeys = new Dictionary<MonoBehaviour, List<AttributeKey>>();

        Instance.RegisterAttributes();
    }

    // Populate attributes
    void RegisterAttributes()
    {
        MonoBehaviour[] sceneObjects = FindObjectsOfType<MonoBehaviour>();
        HashSet<MonoBehaviour> uniqueAttributeContainers = new HashSet<MonoBehaviour>();

        foreach (MonoBehaviour mb in sceneObjects)
        {
            Type monoType = mb.GetType();

            // Fields
            {
                // Retreive the fields from the mono instance
                FieldInfo[] objectFields = monoType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // search all fields/properties for the [DebugGUIVar] attribute
                for (int i = 0; i < objectFields.Length; i++)
                {
                    DebugGUIPrintAttribute printAttribute = Attribute.GetCustomAttribute(objectFields[i], typeof(DebugGUIPrintAttribute)) as DebugGUIPrintAttribute;

                    if (printAttribute != null)
                    {

                        uniqueAttributeContainers.Add(mb);
                        if (!debugGUIPrintFields.ContainsKey(monoType))
                            debugGUIPrintFields.Add(monoType, new HashSet<FieldInfo>());
                        if (!debugGUIPrintProperties.ContainsKey(monoType))
                            debugGUIPrintProperties.Add(monoType, new HashSet<PropertyInfo>());

                        debugGUIPrintFields[monoType].Add(objectFields[i]);
                    }

                    DebugGUIGraphAttribute graphAttribute = Attribute.GetCustomAttribute(objectFields[i], typeof(DebugGUIGraphAttribute)) as DebugGUIGraphAttribute;

                    if (graphAttribute != null)
                    {
                        // Can't cast to float so we don't bother registering it
                        if (objectFields[i].GetValue(mb) as float? == null)
                        {
                            Debug.LogError(string.Format("Cannot cast {0}.{1} to float. This member will be ignored.", monoType.Name, objectFields[i].Name));
                            continue;
                        }

                        uniqueAttributeContainers.Add(mb);
                        if (!debugGUIGraphFields.ContainsKey(monoType))
                            debugGUIGraphFields.Add(monoType, new HashSet<FieldInfo>());
                        if (!debugGUIGraphProperties.ContainsKey(monoType))
                            debugGUIGraphProperties.Add(monoType, new HashSet<PropertyInfo>());


                        debugGUIGraphFields[monoType].Add(objectFields[i]);
                        GraphContainer graph =
                            new GraphContainer(graphWidth, graphHeight)
                            {
                                name = objectFields[i].Name,
                                max = graphAttribute.max,
                                min = graphAttribute.min,
                                group = graphAttribute.group,
                                autoScale = graphAttribute.autoScale
                            };
                        if (!graphAttribute.color.Equals(default(Color)))
                            graph.color = graphAttribute.color;

                        var key = new AttributeKey(objectFields[i]);
                        if (!attributeKeys.ContainsKey(mb))
                            attributeKeys.Add(mb, new List<AttributeKey>());
                        attributeKeys[mb].Add(key);

                        graphDictionary.Add(key, graph);
                        graphs.Add(graph);
                    }
                }
            }

            // Properties
            {
                PropertyInfo[] objectProperties = monoType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                for (int i = 0; i < objectProperties.Length; i++)
                {
                    DebugGUIPrintAttribute printAttribute = Attribute.GetCustomAttribute(objectProperties[i], typeof(DebugGUIPrintAttribute)) as DebugGUIPrintAttribute;

                    if (printAttribute != null)
                    {
                        uniqueAttributeContainers.Add(mb);

                        if (!debugGUIPrintFields.ContainsKey(monoType))
                            debugGUIPrintFields.Add(monoType, new HashSet<FieldInfo>());
                        if (!debugGUIPrintProperties.ContainsKey(monoType))
                            debugGUIPrintProperties.Add(monoType, new HashSet<PropertyInfo>());

                        debugGUIPrintProperties[monoType].Add(objectProperties[i]);
                    }

                    DebugGUIGraphAttribute graphAttribute = Attribute.GetCustomAttribute(objectProperties[i], typeof(DebugGUIGraphAttribute)) as DebugGUIGraphAttribute;

                    if (graphAttribute != null)
                    {
                        // Can't cast to float so we don't bother registering it
                        if (objectProperties[i].GetValue(mb, null) as float? == null)
                        {
                            Debug.LogError("Cannot cast " + objectProperties[i].Name + " to float. This member will be ignored.");
                            continue;
                        }

                        uniqueAttributeContainers.Add(mb);

                        if (!debugGUIGraphFields.ContainsKey(monoType))
                            debugGUIGraphFields.Add(monoType, new HashSet<FieldInfo>());
                        if (!debugGUIGraphProperties.ContainsKey(monoType))
                            debugGUIGraphProperties.Add(monoType, new HashSet<PropertyInfo>());

                        debugGUIGraphProperties[monoType].Add(objectProperties[i]);
                        GraphContainer graph =
                            new GraphContainer(graphWidth, graphHeight)
                            {
                                name = objectProperties[i].Name,
                                max = graphAttribute.max,
                                min = graphAttribute.min,
                                group = graphAttribute.group,
                                autoScale = graphAttribute.autoScale
                            };
                        if (!graphAttribute.color.Equals(default(Color)))
                            graph.color = graphAttribute.color;

                        var key = new AttributeKey(objectProperties[i]);
                        if (!attributeKeys.ContainsKey(mb))
                            attributeKeys.Add(mb, new List<AttributeKey>());
                        attributeKeys[mb].Add(key);

                        graphDictionary.Add(key, graph);
                        graphs.Add(graph);
                    }
                }
            }
        }

        foreach (var mb in uniqueAttributeContainers)
        {
            attributeContainers.Add(mb);
            Type type = mb.GetType();
            if (!typeInstanceCounts.ContainsKey(type))
                typeInstanceCounts.Add(type, 0);
            typeInstanceCounts[type]++;
        }
    }

    void CleanUpDeletedAtributes()
    {
        for (int i = 0; i < attributeContainers.Count; i++)
        {
            if (attributeContainers[i] == null)
            {
                MonoBehaviour mb = attributeContainers[i];
                attributeContainers.RemoveAt(i);
                var keys = attributeKeys[mb];
                foreach (var key in keys)
                {
                    InstanceRemoveGraph(key);
                }
                attributeKeys.Remove(mb);

                Type type = mb.GetType();
                typeInstanceCounts[type]--;
                if (typeInstanceCounts[type] == 0)
                {
                    if (debugGUIPrintFields.ContainsKey(type))
                        debugGUIPrintFields.Remove(type);
                    if (debugGUIPrintProperties.ContainsKey(type))
                        debugGUIPrintProperties.Remove(type);
                    if (debugGUIGraphFields.ContainsKey(type))
                        debugGUIGraphFields.Remove(type);
                    if (debugGUIGraphProperties.ContainsKey(type))
                        debugGUIGraphProperties.Remove(type);
                }

                i--;
            }
        }
    }

    // Wrapper for unique keys
    class AttributeKey
    {
        public MemberInfo memberInfo;
        public AttributeKey(MemberInfo memberInfo)
        {
            this.memberInfo = memberInfo;
        }
    }

    struct TransientLog
    {
        public string text;
        public float expiryTime;

        public TransientLog(string text, float duration)
        {
            this.text = text;
            expiryTime = Time.realtimeSinceStartup + duration;
        }
    }

    [Serializable]
    class GraphContainer
    {
        public string name;

        // Value at the top of the graph
        public float max = 1;
        // Value at the bottom of the graph
        public float min = 0;
        // Should min/max scale to values outside of min/max?
        public bool autoScale;
        public Color color;
        // Graph order on screen
        public int group;

        Texture2D tex0;
        Texture2D tex1;
        bool texFlipFlop;

        int currentIndex;
        float[] values;

        public void SetMinMax(float min, float max)
        {
            if (this.min == min && this.max == max) return;

            RegenerateGraph();

            this.min = min;
            this.max = max;
        }

        static Color32[] clearColorArray = new Color32[graphWidth * graphHeight];

        public GraphContainer(int width, int height)
        {
            values = new float[width];

            tex0 = new Texture2D(width, height);
            tex0.SetPixels32(clearColorArray);
            tex1 = new Texture2D(width, height);
            tex1.SetPixels32(clearColorArray);
        }

        // Add a data point to the beginning of the graph
        public void Push(float val)
        {
            if (autoScale && (val > max || val < min))
            {
                SetMinMax(Mathf.Min(val, min), Mathf.Max(val, max));
            }

            currentIndex = (currentIndex + 1) % values.Length;
            values[currentIndex] = val;

            Texture2D source = texFlipFlop ? tex0 : tex1;
            Texture2D target = texFlipFlop ? tex1 : tex0;
            texFlipFlop = !texFlipFlop;

            Graphics.CopyTexture(
                source, 0, 0, 0, 0, source.width - 1, source.height,
                target, 0, 0, 1, 0
            );

            // Clear column
            for (int i = 0; i < target.height; i++)
            {
                target.SetPixel(0, i, Color.clear);
            }

            // Read from index backwards
            var value = values[Mod(currentIndex, values.Length)];
            var nextVal = values[Mod(currentIndex - 1, values.Length)];

            // Flip the y coordinate to start at the bottom
            int y0 = (int)(Mathf.InverseLerp(min, max, value) * graphHeight);
            int y1 = (int)(Mathf.InverseLerp(min, max, nextVal) * graphHeight);

            // Prevent wraparound to zero
            y0 = y0 >= graphHeight ? graphHeight - 1 : y0;
            y1 = y1 >= graphHeight ? graphHeight - 1 : y1;

            DrawLine(target, 0, y0, 1, y1, color);
        }

        public void Clear()
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = 0;
            }
        }

        // Draw this graph on the given texture
        public void Draw(Rect rect)
        {
            Texture2D target = texFlipFlop ? tex1 : tex0;

            target.Apply();

            GUI.DrawTexture(rect, target);
        }

        public float GetValue(int index)
        {
            return values[Mod(currentIndex + index, values.Length)];
        }

        // Redraw graph using data points
        private void RegenerateGraph()
        {
            Texture2D source = texFlipFlop ? tex0 : tex1;
            tex0.SetPixels32(clearColorArray);
            tex1.SetPixels32(clearColorArray);

            for (int i = 0; i < values.Length - 1; i++)
            {
                DrawLine(
                    source,
                    i,
                    (int)(Mathf.InverseLerp(min, max, values[Mod(currentIndex - i, values.Length)]) * graphHeight),
                    i + 1,
                    (int)(Mathf.InverseLerp(min, max, values[Mod(currentIndex - i - 1, values.Length)]) * graphHeight),
                    color
                );
            }
        }

        private static int Mod(int n, int m)
        {
            return ((n % m) + m) % m;
        }

        // Modified version of:
        // Method Author: Eric Haines (Eric5h5) 
        // Creative Common's Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)
        // http://wiki.unity3d.com/index.php?title=TextureDrawLine
        private void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color col)
        {
            int dy = y1 - y0;
            int dx = x1 - x0;
            int stepx, stepy;

            if (dy < 0) { dy = -dy; stepy = -1; }
            else { stepy = 1; }
            if (dx < 0) { dx = -dx; stepx = -1; }
            else { stepx = 1; }
            dy <<= 1;
            dx <<= 1;

            float fraction = 0;

            tex.SetPixel(x0, y0, col);
            if (dx > dy)
            {
                fraction = dy - (dx >> 1);
                while ((x0 > x1 ? x0 - x1 : x1 - x0) > 1)
                {
                    if (fraction >= 0)
                    {
                        y0 += stepy;
                        fraction -= dx;
                    }
                    x0 += stepx;
                    fraction += dy;
                    tex.SetPixel(x0, y0, col);
                }
            }
            else
            {
                fraction = dx - (dy >> 1);
                while ((y0 > y1 ? y0 - y1 : y1 - y0) > 1)
                {
                    if (fraction >= 0)
                    {
                        x0 += stepx;
                        fraction -= dy;
                    }
                    y0 += stepy;
                    fraction += dx;
                    tex.SetPixel(x0, y0, col);
                }
            }
        }

        public void DestroyTextures()
        {
            Destroy(tex0);
            Destroy(tex1);
        }
    }

    void OnDestroy()
    {
        if (Application.isPlaying)
            Destroy(boxTexture);
    }
}
