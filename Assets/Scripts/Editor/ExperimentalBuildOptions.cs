using UnityEditor;

public static class ExperimentalBuildOptions {
    [MenuItem("Custom/Set Texture Resolution Half")]
    public static void SetTextureResolutionHalf() {
        TextureImporter[] importers = Resources.FindObjectsOfTypeAll<TextureImporter>();
        
        foreach (TextureImporter importer in importers) {
            int originalMaxSize = importer.maxTextureSize;
            int newMaxTextureSize = Mathf.Max(64, Mathf.ClosestPowerOfTwo(Mathf.Max(1, originalMaxSize / 2)));
            
            importer.maxTextureSize = newMaxTextureSize;
            importer.SaveAndReimport();
        }
        
        //EditorUtility.DisplayDialog("Texture Resolution Set", "Texture resolution has been set to 64.", "OK");
    }
}
