using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class DebugGUIGraphAttribute : Attribute
{
    public float min { get; private set; }
    public float max { get; private set; }
    public Color color { get; private set; }
    public int group { get; private set; }
    public bool autoScale { get; private set; }

    public DebugGUIGraphAttribute(
        // Line color
        float r = 1,
        float g = 1,
        float b = 1,
        // Values at top/bottom of graph
        float min = 0,
        float max = 1,
        // Offset position on screen
        int group = 0,
        // Auto-adjust min/max to fit the values
        bool autoScale = true
    )
    {
        color = new Color(r, g, b, 0.9f);
        this.min = min;
        this.max = max;
        this.group = group;
        this.autoScale = autoScale;
    }
}