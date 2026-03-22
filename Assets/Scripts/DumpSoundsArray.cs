using UnityEngine;
using System.Text;

[ExecuteInEditMode]
public class DumpSoundsArray : MonoBehaviour
{
    [Header("Reference to your Const singleton or object")]
    private Const constReference; // ← Drag your Const object here in Inspector

    void OnEnable()
    {
        // Optional: auto-get if on same object
        if (constReference == null)
        {
            constReference = GetComponent<Const>();
        }

        if (constReference == null)
        {
            Debug.LogError("No Const reference assigned and none found on this GameObject!");
            return;
        }

        if (constReference.sounds == null || constReference.sounds.Length == 0)
        {
            Debug.LogError("Const.sounds array is empty or null!");
            return;
        }

        Debug.Log("<color=cyan>=== Sounds Path Dump (ready to copy-paste) ===</color>");

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("const string[] soundPaths = new string[]");
        sb.AppendLine("{");

        int count = 0;
        foreach (AudioClip clip in constReference.sounds)
        {
            if (clip == null)
            {
                sb.AppendLine("    null,  // ← missing clip");
                count++;
                continue;
            }

            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(clip);

            if (string.IsNullOrEmpty(assetPath))
            {
                sb.AppendLine($"    // Warning: could not find path for {clip.name}");
                continue;
            }

            // Remove "Assets/" prefix + remove "Sound/" (only once, usually right after Assets/)
            string cleanPath = assetPath
                .Replace("Assets/", "")
                .Replace("Sound/", ""); // ← key change: remove first "Sound/" only


            // Output in requested format
            sb.AppendLine($"    \"./Audio/{cleanPath}\",");
            count++;
        }

        sb.AppendLine("};");
        sb.AppendLine($"// Total items processed: {count} (including nulls)");

        Debug.Log(sb.ToString());

        Debug.Log("<color=green>Copy the block above! (including commas and quotes)</color>");
    }
}
