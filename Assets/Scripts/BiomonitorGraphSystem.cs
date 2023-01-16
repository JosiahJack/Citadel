using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

// Heavily stripped down version of WeaverDev's DebugGUI Graph available on the Unity Asset Store for Free.
public class BiomonitorGraphSystem : MonoBehaviour {
	// Internal refernces
    private int graphWidth = 480;
    private int graphHeight = 80;
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.7f);
    [Header("Runtime Debugging Only")]
    private GraphContainer[] graphs;
    private GUIStyle minMaxTextStyle;
    private GUIStyle boxStyle;
    private Texture2D boxTexture;
    private HashSet<int> graphGroupBoxesDrawn = new HashSet<int>();
    private Dictionary<Type, HashSet<FieldInfo>> debugGUIGraphFields = new Dictionary<Type, HashSet<FieldInfo>>();
    private Dictionary<Type, HashSet<PropertyInfo>> debugGUIGraphProperties = new Dictionary<Type, HashSet<PropertyInfo>>();

	// Singleton instance
    public static BiomonitorGraphSystem a; // Ensure an instance is present

    void Awake() {
		a = this;
        a.InitializeGUIStyles();
		a.graphs = new GraphContainer[3];
		a.graphs[0] = new GraphContainer();
		a.graphs[1] = new GraphContainer();
		a.graphs[2] = new GraphContainer();
        a.SetGraphProperties(0, "EnergyLevel", -1, 1, 0, new Color(0, 0.5f, 1), false); // Turquoise energy usage indicator
        a.SetGraphProperties(1, "Chi", -2, 2, 0, new Color(0.7f, 0, 1), false); // Purple sine wave graph
        a.SetGraphProperties(2, "Heartbeat", -1, 1, 0, new Color(1, 0, 0), false); // Red heartbeat graph
    }

    void OnGUI() {
        GUI.color = Color.white;
        DrawGraphs();
    }

    // Set the properties of a graph.
    public void SetGraphProperties(int index, string label, float min, float max, int group, Color color, bool autoScale) {
		if (index < 0 || index > 2) return;

		if (graphs[index] == null) { Debug.Log("graphs["+index.ToString()+"] was null"); return; }
        graphs[index].name = label;
        graphs[index].SetMinMax(min, max);
        graphs[index].group = Mathf.Max(0, group);
        graphs[index].color = color;
        graphs[index].autoScale = autoScale;
    }

    // Add a data point to a graph.
    public void Graph(int index, float val) {
        //if (!gameObject.activeSelf) return; // Commented out to try to have values update so toggling will look correct.

        graphs[index].Push(val,true); // The true here was gameObject.activeSelf
    }

    // Resets graph data.
    public void ClearGraphs() {
		for (int i=0; i<graphs.Length; ++i) {
            graphs[i].Clear();
        }
	}

    void InitializeGUIStyles() {
        minMaxTextStyle = new GUIStyle();
        minMaxTextStyle.fontSize = 10;
        minMaxTextStyle.fontStyle = FontStyle.Bold;
        Color[] pix = new Color[4];
        for (int i = 0; i < pix.Length; ++i) {
            pix[i] = Color.white;
        }
        boxTexture = new Texture2D(2, 2);
        boxTexture.SetPixels(pix);
        boxTexture.Apply();
        boxStyle = new GUIStyle();
        boxStyle.normal.background = boxTexture;
    }

    public void DrawGraphs() {
        float graphBlockHeight = (graphHeight + 3);
        GUI.backgroundColor = backgroundColor;

        // Boxes for the graphs themselves
        for (int i=0;i<graphs.Length;i++) {
            if (graphGroupBoxesDrawn.Add(graphs[i].group)) GUI.Box(new Rect(0, 0 + graphBlockHeight * graphs[i].group, graphWidth, graphHeight), "", boxStyle);
            graphs[i].Draw(new Rect(0, 0 + graphBlockHeight * graphs[i].group, graphWidth, graphHeight));
        }
    }

    [Serializable]
    public class GraphContainer {
        public string name;
        public float max = 1; // Value at the top of the graph
        public float min = 0; // Value at the bottom of the graph
        public bool autoScale; // Should min/max scale to values outside of min/max?
        public Color color;
        public int group; // Graph order on screen
        public Color32[] clearColorArray = new Color32[BiomonitorGraphSystem.a.graphWidth * BiomonitorGraphSystem.a.graphHeight];
        public Texture2D tex0;
        public Texture2D tex1;
        public bool texFlipFlop;
        public int currentIndex;
		public float[] values;

        public void SetMinMax(float min, float max) {
            if (this.min == min && this.max == max) return;

            RegenerateGraph();
            this.min = min;
            this.max = max;
        }

        public GraphContainer() {
            values = new float[BiomonitorGraphSystem.a.graphWidth];
            tex0 = new Texture2D(BiomonitorGraphSystem.a.graphWidth, BiomonitorGraphSystem.a.graphHeight);
            tex0.SetPixels32(clearColorArray);
            tex1 = new Texture2D(BiomonitorGraphSystem.a.graphWidth, BiomonitorGraphSystem.a.graphHeight);
            tex1.SetPixels32(clearColorArray);
        }

        // Add a data point to the beginning of the graph
        public void Push(float val, bool doDraw) {
            if (autoScale && (val > max || val < min)) SetMinMax(Mathf.Min(val, min), Mathf.Max(val, max));
            currentIndex = (currentIndex + 1) % values.Length;
            values[currentIndex] = val;
            Texture2D source = texFlipFlop ? tex0 : tex1;
            Texture2D target = texFlipFlop ? tex1 : tex0;
            texFlipFlop = !texFlipFlop;
            Graphics.CopyTexture(source, 0, 0, 0, 0, source.width - 1, source.height, target, 0, 0, 1, 0);

            // Clear column
			int h = target.height;
			Color clr = Color.clear;
            for (int i = 0; i < h; i++) {
                target.SetPixel(0, i,clr);
            }

            var value = values[Mod(currentIndex, values.Length)]; // Read from index backwards
            var nextVal = values[Mod(currentIndex - 1, values.Length)]; // Read from index backwards
            int y0 = (int)(Mathf.InverseLerp(min, max, value) * BiomonitorGraphSystem.a.graphHeight); // Flip the y coordinate to start at the bottom
            int y1 = (int)(Mathf.InverseLerp(min, max, nextVal) * BiomonitorGraphSystem.a.graphHeight); // Flip the y coordinate to start at the bottom
            y0 = y0 >= BiomonitorGraphSystem.a.graphHeight ? BiomonitorGraphSystem.a.graphHeight - 1 : y0; // Prevent wraparound to zero
            y1 = y1 >= BiomonitorGraphSystem.a.graphHeight ? BiomonitorGraphSystem.a.graphHeight - 1 : y1; // Prevent wraparound to zero
            if (doDraw) DrawLine(target, 0, y0, 1, y1, color);
        }

        public void Clear() {
            for (int i = 0; i < values.Length; i++) {
                values[i] = 0;
            }
			tex0.SetPixels32(clearColorArray);
			tex1.SetPixels32(clearColorArray);
        }

        // Draw this graph on the given texture
        public void Draw(Rect rect) {
            Texture2D target = texFlipFlop ? tex1 : tex0;
            target.Apply();
            GUI.DrawTexture(rect, target);
        }

        public float GetValue(int index) { return values[Mod(currentIndex + index, values.Length)]; }

        // Redraw graph using data points
        public void RegenerateGraph() {
            Texture2D source = texFlipFlop ? tex0 : tex1;
            tex0.SetPixels32(clearColorArray);
            tex1.SetPixels32(clearColorArray);

            for (int i = 0; i < values.Length - 1; i++) {
                DrawLine(source,i,(int)(Mathf.InverseLerp(min, max, values[Mod(currentIndex - i, values.Length)]) * BiomonitorGraphSystem.a.graphHeight), i + 1, (int)(Mathf.InverseLerp(min, max, values[Mod(currentIndex - i - 1, values.Length)]) * BiomonitorGraphSystem.a.graphHeight), color);
            }
        }

        public static int Mod(int n, int m) {
            return ((n % m) + m) % m;
        }

        // Modified version of:
        // Method Author: Eric Haines (Eric5h5) 
        // Creative Common's Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)
        // http://wiki.unity3d.com/index.php?title=TextureDrawLine
        public void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color col) {
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
            if (dx > dy) {
                fraction = dy - (dx >> 1);
                while ((x0 > x1 ? x0 - x1 : x1 - x0) > 1) {
                    if (fraction >= 0) {
                        y0 += stepy;
                        fraction -= dx;
                    }
                    x0 += stepx;
                    fraction += dy;
                    tex.SetPixel(x0, y0, col);
                }
            } else {
                fraction = dx - (dy >> 1);
                while ((y0 > y1 ? y0 - y1 : y1 - y0) > 1) {
                    if (fraction >= 0) {
                        x0 += stepx;
                        fraction -= dy;
                    }
                    y0 += stepy;
                    fraction += dx;
                    tex.SetPixel(x0, y0, col);
                }
            }
        }
    }
}