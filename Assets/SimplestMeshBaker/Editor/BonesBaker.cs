using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SimplestMeshBaker
{
    [CanEditMultipleObjects]
    public class BonesBaker : EditorWindow
    {
        private const int YES = 0;
        private const int NO = 2;
        private const int CANCEL = 1;

        enum QuestionResult
        {
            Yes = 0,
            No = 2,
            Cancel = 1,
        }

        private static List<Transform> bonesToRemove;
        
        [MenuItem("GameObject/Bake Bones", false, 0)]
        static void BakeBones(MenuCommand menuCommand)
        {
            //Prevent executing multiple times
            if (Selection.objects.Length > 1)
            {
                if (menuCommand.context != Selection.objects[0])
                {
                    return;
                }
            }
            
            QuestionResult questionResult = (QuestionResult) EditorUtility.DisplayDialogComplex("Simplest Mesh Baker", "Do you want to remove bones after backing?", "Yes", "Cancel", "No");

            if (questionResult == QuestionResult.Cancel)
            {
                return;
            }
            
            int count = 0;

            bonesToRemove = new List<Transform>();
            foreach (GameObject selected in Selection.gameObjects)
            {
                count += Bake(selected);
            }
            if (questionResult == QuestionResult.Yes)
            {
                foreach (var transform in bonesToRemove)
                {
                    if (transform != null)
                    {
                        Undo.DestroyObjectImmediate(transform.gameObject);
                    }
                }
            }
            EditorUtility.DisplayDialog("Simplest Mesh Baker", "Baked " + count + " objects.",
                count == 0 ? "Hm, Ok." : "Great, thanks!");
        }

        private static int Bake(GameObject gameObject)
        {
            int count = 0;
            SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                var newMesh = GetMeshFromSkinnedMeshRenderer(skinnedMeshRenderer);
                //get needed data from skinnedMeshRenderer
                Material material = skinnedMeshRenderer.sharedMaterial;
                Transform rootBone = skinnedMeshRenderer.rootBone;
                if (rootBone != null && rootBone.parent != null)
                {
                    bonesToRemove.Add(rootBone.parent);
                }
                //and remove it
                Undo.DestroyObjectImmediate(skinnedMeshRenderer);
                //add and setup meshFilter and meshRenderer
                MeshFilter meshFilter = Undo.AddComponent<MeshFilter>(gameObject);
                meshFilter.mesh = newMesh;
                MeshRenderer meshRenderer = Undo.AddComponent<MeshRenderer>(gameObject);
                meshRenderer.sharedMaterial = material;
                count++;
            }
            foreach (Transform tr in gameObject.transform)
            {
                count += Bake(tr.gameObject);
            }
            return count;
        }

        public static Mesh GetMeshFromSkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            Mesh newMesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(newMesh);
            //update with scaling scale
            Vector3[] verts = newMesh.vertices;
            float scaleX = skinnedMeshRenderer.transform.lossyScale.x;
            float scaleY = skinnedMeshRenderer.transform.lossyScale.y;
            float scaleZ = skinnedMeshRenderer.transform.lossyScale.z;
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = new Vector3(verts[i].x / scaleX, verts[i].y / scaleY, verts[i].z / scaleZ);
            }
            newMesh.vertices = verts;
            newMesh.RecalculateBounds();
            return newMesh;
        }
    }
}