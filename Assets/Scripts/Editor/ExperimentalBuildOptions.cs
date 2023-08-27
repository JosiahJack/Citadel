using UnityEditor;
using UnityEngine;

public static class ExperimentalBuildOptions {
    public static void SetTextureResolutionHalf() {
        TextureImporter[] importers = Resources.FindObjectsOfTypeAll<TextureImporter>();
        foreach (TextureImporter importer in importers) {
            int originalMaxSize = importer.maxTextureSize;
            int newMaxTextureSize = Mathf.Max(64, Mathf.ClosestPowerOfTwo(Mathf.Max(1, originalMaxSize / 2)));
            importer.maxTextureSize = newMaxTextureSize;
            importer.SaveAndReimport();
        }
    }
}
