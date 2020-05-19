using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUIExamples : MonoBehaviour
{
    /* * * *
     * 
     *   [DebugGUIGraph]
     *   Renders the variable in a graph on-screen. Attribute based graphs will updates every Update.
     *    Lets you optionally define:
     *        max, min  - The range of displayed values
     *        r, g, b   - The RGB color of the graph (0~1)
     *        group     - The order of the graph on screen. Graphs may be overlapped!
     *        autoScale - If true the graph will readjust min/max to fit the data
     *   
     *   [DebugGUIPrint]
     *    Draws the current variable continuously on-screen as 
     *    $"{GameObject name} {variable name}: {value}"
     *   
     *   For more control, these features can be accessed manually.
     *    DebugGUI.SetGraphProperties(key, ...) - Set the properties of the graph with the provided key
     *    DebugGUI.Graph(key, value)            - Push a value to the graph
     *    DebugGUI.LogPersistent(key, value)    - Print a persistent log entry on screen
     *    DebugGUI.Log(value)                   - Print a temporary log entry on screen
     *    
     *   See DebugGUI.cs for more info
     * 
     * * * */

    // Disable Field Unused warning
#pragma warning disable 0414

    // Works with regular fields
    [DebugGUIGraph(min: -1, max: 1, r: 0, g: 1, b: 0, autoScale: true)]
    float SinField;

    // As well as properties
    [DebugGUIGraph(min: -1, max: 1, r: 0, g: 1, b: 1, autoScale: true)]
    float CosProperty { get { return Mathf.Cos(Time.time * 6); } }

#if NET_4_6
    // Also works for expression-bodied properties in .Net 4.6+
    [DebugGUIGraph(min: -1, max: 1, r: 1, g: 0.3f, b: 1)]
    float SinProperty => Mathf.Sin((Time.time + Mathf.PI / 2) * 6);
#else
    [DebugGUIGraph(min: -1, max: 1, r: 1, g: 0.5f, b: 1)]
    float SinProperty { get { return Mathf.Sin((Time.time + Mathf.PI / 2) * 6); } }
#endif

    // User inputs, print and graph in one!
    [DebugGUIPrint, DebugGUIGraph(group: 1, r: 1, g: 0.3f, b: 0.3f)]
    float mouseX;
    [DebugGUIPrint, DebugGUIGraph(group: 1, r: 0, g: 1, b: 0)]
    float mouseY;

    void Awake()
    {
        // Log (as opposed to LogPersistent) will disappear automatically after some time.
        DebugGUI.Log("Hello! I will disappear in five seconds!");

        // Set up graph properties using our graph keys
        DebugGUI.SetGraphProperties("smoothFrameRate", "SmoothFPS", 0, 200, 2, new Color(0, 1, 1), false);
        DebugGUI.SetGraphProperties("frameRate", "FPS", 0, 200, 2, new Color(1, 0.5f, 1), false);
        DebugGUI.SetGraphProperties("fixedFrameRateSin", "FixedSin", -1, 1, 3, new Color(1, 1, 0), true);
    }

    void Update()
    {
        // Update the fields our attributes are graphing
        SinField = Mathf.Sin(Time.time * 6);
        mouseX = Input.mousePosition.x / Screen.width;
        mouseY = Input.mousePosition.y / Screen.height;

        // Manual logging
        if (Input.GetMouseButtonDown(0))
        {
            DebugGUI.Log(string.Format(
                "Mouse clicked! ({0}, {1})",
                mouseX.ToString("F3"),
                mouseY.ToString("F3")
            ));
        }

        // Manual persistent logging
        DebugGUI.LogPersistent("smoothFrameRate", "SmoothFPS: " + (1 / Time.deltaTime).ToString("F3"));
        DebugGUI.LogPersistent("frameRate", "FPS: " + (1 / Time.smoothDeltaTime).ToString("F3"));

        if (Time.smoothDeltaTime != 0)
            DebugGUI.Graph("smoothFrameRate", 1 / Time.smoothDeltaTime);
        if (Time.deltaTime != 0)
            DebugGUI.Graph("frameRate", 1 / Time.deltaTime);
    }

    void FixedUpdate()
    {
        // Manual graphing
        DebugGUI.Graph("fixedFrameRateSin", Mathf.Sin(Time.time * 6));
    }

    void OnDestroy()
    {
        // Clean up our logs and graphs when this object is destroyed
        DebugGUI.RemoveGraph("frameRate");
        DebugGUI.RemoveGraph("fixedFrameRateSin");

        DebugGUI.RemovePersistent("frameRate");
    }
}
