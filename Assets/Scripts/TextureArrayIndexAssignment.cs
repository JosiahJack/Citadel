using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
[ExecuteInEditMode]
#endif
public class TextureArrayIndexAssignment : MonoBehaviour {
    public float index = 0;
    private MaterialPropertyBlock propBlock;
    private MeshRenderer mr;

    void OnEnable() {
        SetIndex();
    }

    void Awake() {
        #if UNITY_EDITOR
        this.enabled = true;
        #endif
        SetIndex();
    }

    public static void RevertPrefabPropertyOverrideWithMatchingName(Object ob, string name) {
        SerializedObject serializedObject = new SerializedObject(ob);
        SerializedProperty serializedProperty = serializedObject.FindProperty(name);
        PrefabUtility.RevertPropertyOverride(serializedProperty,InteractionMode.AutomatedAction);
    }

    void SetIndex() {
        if (propBlock == null) propBlock = new MaterialPropertyBlock();
        if (mr == null) mr = GetComponent<MeshRenderer>();
        if (mr == null) return;

        mr.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Slice",index);
        mr.SetPropertyBlock(propBlock);
        #if UNITY_EDITOR
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null) {
            if (PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected) {
                foreach (ObjectOverride over in PrefabUtility.GetObjectOverrides(gameObject)) {
                    Object obj = over.instanceObject;
                    if (obj.GetType() == typeof(MeshFilter)) {
                        string mname = mf.sharedMesh.name;
                        if (mname.Contains("_assigned")) {
                            RevertPrefabPropertyOverrideWithMatchingName(obj,"m_Mesh");
                        }
                    } else if (obj.GetType() == typeof(MeshRenderer)) {
                        string mname = mr.sharedMaterial.name;
                        if (mname.Contains("(Instance)")) {
                            RevertPrefabPropertyOverrideWithMatchingName(obj,"m_Materials");
                        }
                        if (mname.Contains("chunk")) {
                            RevertPrefabPropertyOverrideWithMatchingName(obj,"m_CastShadows");
                            RevertPrefabPropertyOverrideWithMatchingName(obj,"m_LightProbeUsage");
                            RevertPrefabPropertyOverrideWithMatchingName(obj,"m_MotionVectors");
                            RevertPrefabPropertyOverrideWithMatchingName(obj,"m_CastShadows");
                        }
                    }
                }
            }
        }
        #endif
    }

}
