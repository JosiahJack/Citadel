using UnityEngine;

[ExecuteInEditMode] // Allows running in Editor without Play mode
public class DumpLayerMatrix : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Dumping Layer Collision Matrix:");
        for (int i = 0; i < 32; i++) // Layers 0-31
        {
            string layerNameI = LayerMask.LayerToName(i);
            if (string.IsNullOrEmpty(layerNameI)) continue; // Skip unnamed layers

            for (int j = i; j < 32; j++) // Symmetric, so start from i to avoid duplicates
            {
                string layerNameJ = LayerMask.LayerToName(j);
                if (string.IsNullOrEmpty(layerNameJ)) continue;

                bool ignores = Physics.GetIgnoreLayerCollision(i, j);
                if (!ignores)
                {
                    Debug.Log($"Layer {i} ({layerNameI}) COLLIDES with Layer {j} ({layerNameJ})");
                }
                else
                {
                    Debug.Log($"Layer {i} ({layerNameI}) IGNORES (no collision) Layer {j} ({layerNameJ})");
                }
            }
        }
        Debug.Log("Dump complete.");
    }
}
