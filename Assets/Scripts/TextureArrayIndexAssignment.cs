using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
[ExecuteInEditMode]
#endif
public class TextureArrayIndexAssignment : MonoBehaviour {
    public float index = 0;
    public static int numberOfIndices = 225;
    public static MaterialPropertyBlock[] propertyBlocks;
    public static MaterialPropertyBlock propertyBlock;
    private MeshRenderer mr;

//     void OnEnable() {
//         if (propertyBlocks.Length != numberOfIndices) InitializePropBlocks();
//         SetIndex();
//     }

    void Awake() {
//         #if UNITY_EDITOR
//         this.enabled = true;
//         #endif
//         SetIndex();
    }

//     void InitializePropBlocks() {
//         propertyBlocks = new MaterialPropertyBlock[numberOfIndices];
//         for (int i = 0; i < numberOfIndices; i++) {
//             propertyBlocks[i] = new MaterialPropertyBlock();
//             propertyBlocks[i].SetFloat("_Slice",(float)i);
//         }
//     }

    public static void RevertPrefabPropertyOverrideWithMatchingName(Object ob, string name) {
//         #if UNITY_EDITOR
//         SerializedObject serializedObject = new SerializedObject(ob);
//         SerializedProperty serializedProperty = serializedObject.FindProperty(name);
//         PrefabUtility.RevertPropertyOverride(serializedProperty,InteractionMode.AutomatedAction);
//         #endif
    }

    void SetIndex() {
//         if (propertyBlocks == null) InitializePropBlocks();
        //if (propertyBlocks.Length != numberOfIndices) InitializePropBlocks();
        //if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();
//         if (index < 0) return;
//         if (index >= numberOfIndices) return;
//
//         if (mr == null) mr = GetComponent<MeshRenderer>();
//         if (mr == null) return;

        //mr.SetPropertyBlock(propertyBlocks[(int)index]);
        #if UNITY_EDITOR
//         MeshFilter mf = GetComponent<MeshFilter>();
//         if (mf != null) {
//             if (PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected) {
//                 foreach (ObjectOverride over in PrefabUtility.GetObjectOverrides(gameObject)) {
//                     Object obj = over.instanceObject;
//                     if (obj.GetType() == typeof(MeshFilter)) {
//                         if (mf.sharedMesh != null) {
//                             string mname = mf.sharedMesh.name;
//                             if (mname.Contains("_assigned")) {
//                                 RevertPrefabPropertyOverrideWithMatchingName(obj,"m_Mesh");
//                             }
//                         }
//                     } else if (obj.GetType() == typeof(MeshRenderer)) {
//                         if (mr.sharedMaterial != null) {
//                             string mname = mr.sharedMaterial.name;
//                             if (mname.Contains("(Instance)")) {
//                                 RevertPrefabPropertyOverrideWithMatchingName(obj,"m_Materials");
//                             }
//                             if (mname.Contains("chunk")) {
//                                 RevertPrefabPropertyOverrideWithMatchingName(obj,"m_CastShadows");
//                                 RevertPrefabPropertyOverrideWithMatchingName(obj,"m_LightProbeUsage");
//                                 RevertPrefabPropertyOverrideWithMatchingName(obj,"m_MotionVectors");
//                                 RevertPrefabPropertyOverrideWithMatchingName(obj,"m_CastShadows");
//                             }
//                         }
//                     }
//                 }
//             }
//         }
        #endif
    }
}
