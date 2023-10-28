using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DynamicCulling {
    static int raymax = 252; // 64x64 = 63 * 4 = 252 possible directions
    static int raynum; // 0 to 251 for which ray at the moment.
    static bool[] worldCellOpen = new bool [64 * 64];
    static Vector3[] worldCells = new Vector3 [64 * 64];
    static List<GameObject>[] cells = new List<GameObject>[64 * 64];
    static bool[] isDirty = new bool[64 * 64];
    static int lastPlayerCell = 0;
    static List<MeshRenderer> meshes;

    // Standard culling: 2.21ms to 2.44ms at game start, no camera motion.
    // Plus CullResultsCreateShared: 0.20ms to 0.34ms

    // Plain radius based culling (this first iteration): 

    public static bool EulerAnglesWithin90(Vector3 angs) {
        if (angs.x % 90 >= 0.05f) return false;
        if (angs.z % 90 >= 0.05f) return false;
        if (angs.y % 90 >= 0.05f) return false;
        return true;
    }

    public static void Cull_Init() {
        return;
        List<GameObject> orthogonalChunks = new List<GameObject>();
        orthogonalChunks.Clear();

        for (int i=0;i<4096;i++) {
            cells[i] = new List<GameObject>();
            cells[i].Clear();
        }

        // Determine open world cells at game start.
		GameObject dCont = LevelManager.a.GetCurrentGeometryContainer();
        if (dCont == null) return;

        GameObject childGO = null;
        Transform container = dCont.transform;
        int count = container.childCount;
        Vector3 mod = new Vector3(0f,0f,0f);
        for (int i=0; i < count; i++) {
			childGO = container.GetChild(i).gameObject;
            if (EulerAnglesWithin90(childGO.transform.localEulerAngles)) {
                mod = childGO.transform.localPosition;
                mod.x = mod.x % 2.56f;
                mod.y = mod.y % 2.56f;
                mod.z = mod.z % 2.56f;

                // Only accept found chunks that are grid aligned
                if (mod.x < 0.04f && mod.y < 0.04f && mod.z < 0.04f) {
                    orthogonalChunks.Add(childGO);   
                }
            }

            Component[] compArray = childGO.GetComponentsInChildren(
								    typeof(MeshRenderer),true);

			foreach (MeshRenderer mr in compArray) meshes.Add(mr);
        }

        // Find a reference point from which to build the grid.  Any orthogonal
        // chunk will do.
        Vector3 pos = orthogonalChunks[0].transform.localPosition;
        Vector2 reference = new Vector2(pos.x,pos.z);

        Vector3 max = new Vector3(0f,0f,0f);
        Vector3 min = new Vector3(0f,0f,0f);
        // Find the grid extents by pretending reference is 0,0,0
        for (int i=0; i < orthogonalChunks.Count; i++) {
			childGO = orthogonalChunks[i].gameObject;
            pos = childGO.transform.localPosition;
            if (pos.x > (max.x - reference.x)) max.x = pos.x; 
            if (pos.z > (max.z - reference.y)) max.z = pos.z;
            if (pos.x < (min.x - reference.x)) min.x = pos.x; 
            if (pos.z < (min.z - reference.y)) min.z = pos.z;            
        }

        bool breakx = false;
        bool breaky = false;
        Vector2 pos2d = new Vector2(0f,0f);
        Vector2 pos2dcurrent = new Vector2(0f,0f);
        for (int x=0; x < 64; x++) {
            breakx = false;
            pos2dcurrent.x = min.x + (2.56f * (float)x);
            for (int z=0; z < 64; z++) {
                breaky = false;
                pos2dcurrent.y = min.z + (2.56f * (float)z);
                for (int i=0; i < orthogonalChunks.Count; i++) {
                    childGO = orthogonalChunks[i].gameObject;
                    pos = childGO.transform.localPosition;
                    pos2d.x = pos.x;
                    pos2d.y = pos.z;
                    if (Vector2.Distance(pos2d,pos2dcurrent) < 0.16f) {
                        worldCellOpen[(z * 64) + x] = true;
                        worldCells[(z * 64) + x] = pos;
                        breakx = breaky = true; break;
                    }
                }
                if (breaky) break;
            }
            if (breakx) break;
        }

        // Now go through every chunk and assign to a particular cell list.
        bool[] alreadyInAtLeastOneList = new bool[count];
        for (int i=0;i<4096;i++) {
            isDirty[i] = true;
            for (int c=0;c<count;c++) {
                if (alreadyInAtLeastOneList[c]) continue;

                childGO = container.GetChild(c).gameObject;
                pos = childGO.transform.localPosition;
                pos2d.x = pos.x;
                pos2d.y = pos.z;
                pos2dcurrent.x = worldCells[i].x;
                pos2dcurrent.y = worldCells[i].z;
                if (Vector2.Distance(pos2d,pos2dcurrent) >= 1.28f) continue;

                cells[i].Add(childGO);
                alreadyInAtLeastOneList[c] = true;
                Component[] compArray = childGO.GetComponentsInChildren(
                                        typeof(MeshRenderer),true);

                foreach (MeshRenderer mr in compArray) {
                    meshes.Add(mr);
                    mr.enabled = false;
                }
            }
        }
    }
    
    public static void Cull() {
        return;

        //DetermineVisibleCells();
        //ToggleVisibility();

        Vector3 pos = PlayerMovement.a.transform.position;
        int playerCell = 0;
        // Find player cell
        for (int c=0;c<4096;c++) {
            if (Vector3.Distance(pos,worldCells[c]) < 1.28f) {
                playerCell = c;
            }
        }

        if (lastPlayerCell != playerCell) {
            for (int i=0;i<4096;i++) isDirty[i] = true; // TODO use vis!
        }

        // Determine visibility of world cells by casting up to 252 rays along
        // fixed integer angles based on going to center of each cell along
        // world bounds, using open worldCellOpen status found previously.
        // Then hide/unhide MeshRenderers accordingly.

        // Iterate over all level chunks and adjust their visibility based on
        // afore raycasting results.
        int count = meshes.Count;
        Vector2 pos2d = new Vector2(0f,0f);
        bool visible = false;
        GameObject childGO = null;
        for (int i=0; i < count; i++) {
            visible = false;
            pos = meshes[i].transform.localPosition;
            for (int c=0;c<4096;c++) {
                if (!isDirty[c]) continue;
                if (Vector3.Distance(pos,worldCells[c]) >= 0.16f) continue;

                if (Vector3.Distance(worldCells[c],worldCells[playerCell]) < 10.86f) { // 3 cell radius
                    visible = true;
                }
            }

            meshes[i].enabled = visible;
        }
    }
}
