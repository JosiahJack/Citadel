using UnityEngine;

[ExecuteAlways] // Allow updates in the Editor
public class ImposterRenderer : MonoBehaviour {
    public Texture2D[] slices; // Array of pre-rendered slices
    public int yawSteps = 8;  // Number of slices per horizontal ring (e.g., 8 for 45° increments)
    public int pitchSteps = 7; // Number of vertical rings (e.g., -60° to 60°)
    private Renderer objRenderer;

    void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer == null)
            Debug.LogError("No Renderer component found on this GameObject!");
    }

    void Update()
    {
        if (slices == null || slices.Length == 0)
            return;

        // Determine view direction
        Vector3 toCamera = (Camera.main.transform.position - transform.position).normalized;

        // Convert to spherical coordinates
        float azimuth = Mathf.Atan2(toCamera.z, toCamera.x) * Mathf.Rad2Deg; // Yaw
        float elevation = Mathf.Asin(toCamera.y) * Mathf.Rad2Deg;           // Pitch

        // Normalize azimuth to 0-360 degrees
        azimuth = (azimuth + 360) % 360;

        // Find the closest slice indices
        int yawIndex = Mathf.RoundToInt(azimuth / 360f * yawSteps) % yawSteps;
        int pitchIndex = Mathf.Clamp(Mathf.RoundToInt((elevation + 180f) / 360f * pitchSteps), 0, pitchSteps - 1);

        // Calculate slice index
        int sliceIndex = pitchIndex * yawSteps + yawIndex;

        if (sliceIndex >= slices.Length)
        {
            Debug.LogWarning($"Slice index {sliceIndex} out of range! Check slices array.");
            return;
        }

        // Assign the selected texture to the material
        if (objRenderer != null)
        {
            objRenderer.material.mainTexture = slices[sliceIndex];
        }
    }
}
