using CSM.Runtime;
using CSM.Runtime.MonoBehaviours;
using UnityEditor;
using UnityEngine;

namespace CSM.Editor {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [CanEditMultipleObjects]
  [CustomEditor(inspectedType : typeof(CachedShadowMapLight))]
  public class CachedShadowMapEditor : UnityEditor.Editor {
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override void OnInspectorGUI() {
      this.DrawDefaultInspector();

      var csm = (CachedShadowMapLight)this.target;

      if (Application.isPlaying) {
        if (GUILayout.Button("Update cache")) {
          if (csm != null) {
            ShadowMapCacheSystem.Instance.EnqueueDirtyCandidate(a_light : csm);
          }
        }
      }
    }
  }
}