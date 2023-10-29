using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCulling : MonoBehaviour {
    const int WORLDX = 64;
    const int ARRSIZE = WORLDX * WORLDX;
    const float CELLX = 2.56f;
    const float CELLXHALF = 1.28f;
    public bool[] worldCellOpen = new bool [ARRSIZE];
    public Vector3[] worldCells = new Vector3 [ARRSIZE];
    public List<GameObject>[] cells = new List<GameObject>[ARRSIZE];
    public bool[] isDirty = new bool[ARRSIZE];
    public int lastPlayerCell = 0;
    public List<MeshRenderer> meshes;
    public int playerCell = 0;
    public float deltaX = 0.0f;
    public float deltaY = 0.0f;
    public Vector3 worldMax;
    public Vector3 worldMin;

    public static DynamicCulling a;

    // Standard culling: 2.21ms to 2.44ms at game start, no camera motion.
    // Plus CullResultsCreateShared: 0.20ms to 0.34ms

    // Plain radius based culling (this first iteration):  97ms !!

    // These two were measured using Deep Profile on:
    // RenderDeferred.Lighting Built-in: 2.8ms to 4.7ms, ~3ms average
    // RednerDeferred.Lighting Maxewell: 

    void Awake() {
        a = this;
        a.Cull_Init();
    }

    public bool EulerAnglesWithin90(Vector3 angs) {
        if (angs.x % 90 >= 0.05f) return false;
        if (angs.z % 90 >= 0.05f) return false;
        if (angs.y % 90 >= 0.05f) return false;
        return true; 
    }

    public void Cull_Init() {
        List<GameObject> orthogonalChunks = new List<GameObject>();
        orthogonalChunks.Clear();

        for (int i=0;i<ARRSIZE;i++) {
            cells[i] = new List<GameObject>();
            cells[i].Clear();
        }

        // Determine open world cells at game start.
		GameObject dCont = LevelManager.a.GetCurrentGeometryContainer();
        if (dCont == null) return;

        GameObject childGO = null;
        Transform container = dCont.transform;
        int chunkCount = container.childCount;
        Vector3 mod = new Vector3(0f,0f,0f);
        for (int i=0; i < chunkCount; i++) {
			childGO = container.GetChild(i).gameObject;
            if (EulerAnglesWithin90(childGO.transform.localEulerAngles)) {
                mod = childGO.transform.localPosition;
                mod.x = mod.x % CELLX;
                mod.y = mod.y % CELLX;
                mod.z = mod.z % CELLX;

                // Only accept found chunks that are grid aligned
                if (mod.x < 0.04f && mod.y < 0.04f && mod.z < 0.04f) {
                    orthogonalChunks.Add(childGO);   
                }
            }

            Component[] compArray = childGO.GetComponentsInChildren(
								    typeof(MeshRenderer),true);

			foreach (MeshRenderer mr in compArray) meshes.Add(mr);
        }

        Debug.Log("Found " + orthogonalChunks.Count.ToString() + " chunks");

        // Find a reference point from which to build the grid.  Any orthogonal
        // chunk will do.
        Vector3 pos = orthogonalChunks[0].transform.localPosition;
        Vector2 reference = new Vector2(pos.x,pos.z);

        worldMax = new Vector3(0f,0f,0f);
        worldMin = new Vector3(0f,0f,0f);
        // Find the grid extents by pretending reference is 0,0,0
        for (int i=0; i < orthogonalChunks.Count; i++) {
			childGO = orthogonalChunks[i].gameObject;
            pos = childGO.transform.localPosition;
            if (pos.x > (worldMax.x - reference.x)) worldMax.x = pos.x; 
            if (pos.z > (worldMax.z - reference.y)) worldMax.z = pos.z;
            if (pos.x < (worldMin.x - reference.x)) worldMin.x = pos.x; 
            if (pos.z < (worldMin.z - reference.y)) worldMin.z = pos.z;            
        }

        bool breakx = false;
        bool breaky = false;
        Vector2 pos2d = new Vector2(0f,0f);
        Vector2 pos2dcurrent = new Vector2(0f,0f);
        for (int x=0; x < WORLDX; x++) {
            breakx = false;
            pos2dcurrent.x = worldMin.x + (CELLX * (float)x);
            for (int z=0; z < WORLDX; z++) {
                breaky = false;
                pos2dcurrent.y = worldMin.z + (CELLX * (float)z);
                for (int i=0; i < orthogonalChunks.Count; i++) {
                    childGO = orthogonalChunks[i].gameObject;
                    pos = childGO.transform.localPosition;
                    pos2d.x = pos.x;
                    pos2d.y = pos.z;
                    if (Vector2.Distance(pos2d,pos2dcurrent) < 0.64f) {
                        worldCellOpen[(z * 64) + x] = true;
                        worldCells[(z * 64) + x] = pos;
                        Debug.Log("Found open cell");
                        breakx = breaky = true; break;
                    }
                }
                if (breaky) break;
            }
            if (breakx) break;
        }

        // Now go through every chunk and assign to a particular cell list.
        bool[] alreadyInAtLeastOneList = new bool[chunkCount];
        for (int i=0;i<ARRSIZE;i++) {
            isDirty[i] = true;
            for (int c=0;c<chunkCount;c++) {
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
                    //mr.enabled = false;
                }
            }
        }

        playerCell = 0;
        // Find player cell
        pos = PlayerMovement.a.transform.position;
        for (int c=0;c<4096;c++) {
            if (Vector3.Distance(pos,worldCells[c]) < 1.28f) {
                playerCell = c;
            }
        }
        lastPlayerCell = playerCell;
    }

    public void Cull() {
        Vector3 pos = PlayerMovement.a.transform.position;
        deltaX = pos.x - worldCells[lastPlayerCell].x;
        deltaY = pos.z - worldCells[lastPlayerCell].z;
        if      (pos.x > worldMax.x || pos.x < worldMin.x) playerCell = 0;
        else if (pos.y > worldMax.y || pos.y < worldMin.y) playerCell = 0;
        else if (deltaX > CELLXHALF) { // Move current right

            playerCell = lastPlayerCell + 1;

            if      (deltaY > CELLXHALF) playerCell -= WORLDX; // Move up
            else if (deltaY < -CELLXHALF) playerCell += WORLDX; // Move down
        } else if (deltaX < -CELLXHALF) { // Move current left

            playerCell = lastPlayerCell - 1;

            if     (deltaY > CELLXHALF) playerCell -= WORLDX; // Move up
            else if (deltaY < -CELLXHALF) playerCell += WORLDX; // Move down
        }
        
        if (playerCell == lastPlayerCell) return; // No cell status updates.

        //DetermineVisibleCells(); // Reevaluate visible cells from new pos.
        //ToggleVisibility(); // Update all cells marked as dirty.
        //return;





        //if (lastPlayerCell != playerCell) {
        //    for (int i=0;i<4096;i++) isDirty[i] = true; // TODO use vis!
        //}

        // Determine visibility of world cells by casting up to 252 rays along
        // fixed integer angles based on going to center of each cell along
        // world bounds, using open worldCellOpen status found previously.
        // Then hide/unhide MeshRenderers accordingly.

        // Iterate over all level chunks and adjust their visibility based on
        // afore raycasting results.
        //int count = meshes.Count;
        //Vector2 pos2d = new Vector2(0f,0f);
        //bool visible = false;
        //GameObject childGO = null;
        //for (int i=0; i < count; i++) {
        //    visible = false;
        //    pos = meshes[i].transform.localPosition;
        //    for (int c=0;c<4096;c++) {
        //        if (!isDirty[c]) continue;
        //        if (Vector3.Distance(pos,worldCells[c]) >= 0.16f) continue;

        //        if (Vector3.Distance(worldCells[c],worldCells[playerCell]) < 10.86f) { // 3 cell radius
        //            visible = true;
        //        }
        //    }

        //    meshes[i].enabled = visible;
        //}
    }
}
