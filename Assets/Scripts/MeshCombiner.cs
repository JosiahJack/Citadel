using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Original implementation from: https://github.com/dawid-t/Mesh-Combiner
// Heavily modified for mesh colliders and vertex color splitting by: Chris Kurhan @ LlamAcademy 2024
// Heavily modified for Citadel by JosiahJack, find at https://github.com/JosiahJack/Citadel
// I removed any handling of colliders as I want them preserved.  This is purely visual combination
// for rendering performance benefit. -Josiah

[DisallowMultipleComponent]
public class MeshCombiner : MonoBehaviour {
    private GameObject lastCombineResult;
    private Meshenderer[] sourceMeshenderers;
    private bool[] sourceMeshendererStates;

    // Called before Culling so we don't screw up resultant cull enable states.
    public void UncombineMeshes() {
        if (lastCombineResult != null) {
            DestroyImmediate(lastCombineResult); // No longer display combined result
            for (int i=0;i<sourceMeshenderers.Length;i++) {
                sourceMeshenderers[i].meshRenderer.enabled = sourceMeshendererStates[i]; // Reset sources
            }
        }
    }

    // Called after Culling so we only combine the visible meshes for this frame.
    // Combines all child meshes and stores the result into NEW SUB GAMEOBJECT.
    // There will be a new child gameObjects created with MeshFilter and .
    public void CombineMeshes() {
        // Temporarily unparent organizational folder GameObject so mesh origin
        // is correct prior to offsetting back into the "level" parent "folder"
        // GameObject.
        Vector3 oldScaleAsChild = transform.localScale;
        int positionInParentHierarchy = transform.GetSiblingIndex();
        Transform parent = transform.parent;
        transform.parent = null;
        Quaternion oldRotation = transform.rotation;
        Vector3 oldPosition = transform.position;
        Vector3 oldScale = transform.localScale;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;

        // Do the combine
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(false); // Disregard inactive children that were culled, hence false here
        sourceMeshenderers = new Meshenderer[meshFilters.Length];
        sourceMeshendererStates = new bool[meshFilters.Length];
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        PrefabIdentifier[] pids = new PrefabIdentifier[meshFilters.Length];
        bool isChunk = false;
        for(int i=0;i<meshFilters.Length;i++) {
            pids[i] = meshFilters[i].gameObject.GetComponent<PrefabIdentifier>();
            if (pids[i] != null) {
                if (ConsoleEmulator.ConstIndexIsGeometry(pids[i].constIndex)) isChunk = true;
            }

            sourceMeshenderers[i] = DynamicCulling.GetMeshAndItsRenderer(meshFilters[i].gameObject,pids[i] != null ? pids[i].constIndex : 307); // Paper wad as fallback to force just getting mesh and not LOD
            sourceMeshendererStates[i] = sourceMeshenderers[i].meshRenderer.enabled;
            sourceMeshenderers[i].meshRenderer.enabled = false; // Hide it; it's been subsumed into the collective.
            combineInstances[i].subMeshIndex = 0;
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh {
            name = ("combined_mesh_" + gameObject.name),
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 // SUPER IMPORTANT!  Allows for greater tha 65535 verts!
        };

        combinedMesh.CombineMeshes(combineInstances,true,true,false); // Combine all meshes.
        lastCombineResult = new GameObject("CombinedMeshObject_" + gameObject.name);
        MeshFilter filter = lastCombineResult.AddComponent<MeshFilter>();
        filter.sharedMesh = combinedMesh;
        MeshRenderer renderer = lastCombineResult.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = isChunk ? DynamicCulling.a.chunkMaterial : LevelManager.a.dynamicObjectsMaterial;
        lastCombineResult.transform.SetParent(transform, false);
        lastCombineResult.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        // Restore organizational folder GameObject into its parent and position.
        transform.rotation = oldRotation;
        transform.position = oldPosition;
        transform.localScale = oldScale;
        transform.parent = parent;
        transform.SetSiblingIndex(positionInParentHierarchy);
        transform.localScale = oldScaleAsChild;
    }
}
