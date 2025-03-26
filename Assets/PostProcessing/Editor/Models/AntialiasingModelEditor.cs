#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing {
    using Settings = AntialiasingModel.Settings;

    [PostProcessingModelEditor(typeof(AntialiasingModel))]
    public class AntialiasingModelEditor : PostProcessingModelEditor {
        SerializedProperty m_FxaaPreset;

        public override void OnEnable() {
            m_FxaaPreset = FindSetting((Settings x) => x.fxaaSettings.preset);
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(m_FxaaPreset);
        }
    }
}

#endif
