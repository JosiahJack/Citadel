using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCulling : MonoBehaviour {
    const int WORLDX = 64;
    const int ARRSIZE = WORLDX * WORLDX;
    const float CELLX = 2.56f;
    const float CELLXHALF = 1.28f;
    public bool[,] worldCellOpen = new bool [WORLDX,WORLDX];
    public bool[,] worldCellVisible = new bool [WORLDX,WORLDX];
    public Vector3[,] worldCellPositions = new Vector3 [WORLDX,WORLDX];
    public List<GameObject>[,] cellLists = new List<GameObject>[WORLDX,WORLDX];
    public List<MeshRenderer>[,] cellListsMR = new List<MeshRenderer>[WORLDX,WORLDX];
    public bool[,] worldCellDirty = new bool[WORLDX,WORLDX];
    public int lastPlayerCellX = 0;
    public int lastPlayerCellY = 0;
    public int playerCellX = 0;
    public int playerCellY = 0;
    public float deltaX = 0.0f;
    public float deltaY = 0.0f;
    public Vector3 worldMax;
    public Vector3 worldMin;
    public List<GameObject> orthogonalChunks;

    public static DynamicCulling a;

    // Standard culling: 2.21ms to 2.44ms at game start, no camera motion.
    // Plus CullResultsCreateShared: 0.20ms to 0.34ms

    // Plain radius based culling (this first iteration):  97ms !!
    // Fixed: ___ms

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

    void ClearCellList() {
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                cellLists[x,y] = new List<GameObject>();
                cellLists[x,y].Clear();
            }
        }
    }

    void FindWorldExtents(List<GameObject> chunks) {
        worldMax = new Vector3(0f,0f,0f);
        worldMin = new Vector3(0f,0f,0f);
        Transform tr;
        Vector3 pos;
        for (int i=0; i < chunks.Count; i++) {
			tr = chunks[i].transform;
            pos = tr.position;
            if (pos.x > worldMax.x) worldMax.x = pos.x;
            if (pos.z > worldMax.z) worldMax.z = pos.z;
            if (pos.x < worldMin.x) worldMin.x = pos.x;
            if (pos.z < worldMin.z) worldMin.z = pos.z;
        }
    }

    void FindOpenCellsAndPositions(List<GameObject> chunks) {
        bool breakx = false;
        bool breaky = false;
        Vector2 pos2d = new Vector2(0f,0f);
        Vector2 pos2dcurrent = new Vector2(0f,0f);
        GameObject childGO = null;
        Vector3 pos;
        for (int x=0; x<64; x++) {
            breakx = false;
            pos2dcurrent.x = worldMin.x + (CELLX * (float)x);
            for (int y=0; y<64; y++) {
                breaky = false;
                pos2dcurrent.y = worldMin.z + (CELLX * (float)y);
                for (int i=0; i < orthogonalChunks.Count; i++) {
                    childGO = orthogonalChunks[i].gameObject;
                    pos = childGO.transform.position;
                    pos2d.x = pos.x;
                    pos2d.y = pos.z;
                    if (Vector2.Distance(pos2d,pos2dcurrent) < 0.64f) {
                        worldCellOpen[x,y] = true;
                        worldCellPositions[x,y] = pos;
                        breakx = breaky = true; break;
                    }
                }
                if (breaky) continue;
            }
            if (breakx) continue;
        }
    }

    void FindOrthogonalChunks(GameObject chunkContainer) {
        orthogonalChunks = new List<GameObject>();
        orthogonalChunks.Clear();
        GameObject childGO = null;
        Transform container = chunkContainer.transform;
        int chunkCount = container.childCount;
        for (int i=0; i < chunkCount; i++) {
			childGO = container.GetChild(i).gameObject;
            if (EulerAnglesWithin90(childGO.transform.localEulerAngles)) {
                orthogonalChunks.Add(childGO);
            }
        }
    }

    void PutChunksInCells() {
        Transform container = LevelManager.a.GetCurrentGeometryContainer().transform;
        int chunkCount = container.childCount;
        bool[] alreadyInAtLeastOneList = new bool[chunkCount];
        for (int c=0;c<chunkCount;c++) alreadyInAtLeastOneList[c] = false;
        GameObject childGO = null;
        Vector3 pos;
        Vector2 pos2d = new Vector2(0f,0f);
        Vector2 pos2dcurrent = new Vector2(0f,0f);
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                for (int c=0;c<chunkCount;c++) {
                    if (alreadyInAtLeastOneList[c]) continue;

                    childGO = container.GetChild(c).gameObject;
                    pos = childGO.transform.position;
                    pos2d.x = pos.x;
                    pos2d.y = pos.z;
                    pos2dcurrent.x = worldCellPositions[x,y].x;
                    pos2dcurrent.y = worldCellPositions[x,y].z;
                    if (Vector2.Distance(pos2d,pos2dcurrent) >= 1.28f) continue;

                    cellLists[x,y].Add(childGO);
                    alreadyInAtLeastOneList[c] = true;
                    Component[] compArray = childGO.GetComponentsInChildren(
                                              typeof(MeshRenderer),true);

                    foreach (MeshRenderer mr in compArray) cellListsMR[x,y].Add(mr);
                }
            }
        }
    }

    void FindPlayerCell() {
        playerCellX = 0;
        playerCellY = 0;
        Vector3 pos = PlayerMovement.a.transform.position;
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                if (Vector3.Distance(pos,worldCellPositions[x,y]) < 1.28f) {
                    playerCellX = x;
                    playerCellY = y;
                    lastPlayerCellX = playerCellX;
                    lastPlayerCellY = playerCellY;
                    return;
                }
            }
        }
    }

    public void Cull_Init() {
        ClearCellList();
        FindOrthogonalChunks(LevelManager.a.GetCurrentGeometryContainer());
        FindWorldExtents(orthogonalChunks);
        FindOpenCellsAndPositions(orthogonalChunks);
        PutChunksInCells();
        FindPlayerCell();
    }

    bool UpdatedPlayerCell() {
        Vector3 pos = PlayerMovement.a.transform.position;
        deltaX = pos.x - worldCellPositions[playerCellX,playerCellY].x;
        deltaY = pos.z - worldCellPositions[playerCellX,playerCellY].z;

        if (deltaX > CELLXHALF) playerCellX++;
        else if (deltaX < -CELLXHALF) playerCellX--;
        if (playerCellX < 0) playerCellX = 0;
        if (playerCellX > 63) playerCellX = 63;

        if (deltaY > CELLXHALF) playerCellY++;
        else if (deltaY < -CELLXHALF) playerCellY--;
        if (playerCellY < 0) playerCellY = 0;
        if (playerCellY > 63) playerCellY = 63;

        if (   playerCellX == lastPlayerCellX
            && playerCellY == lastPlayerCellY) {

            return false; // No cell status updates.
        }

        lastPlayerCellX = playerCellX;
        lastPlayerCellY = playerCellY;
        return true;
    }

    void DetermineVisibleCells() {
        for (int i=0;i<64;i++) {

        }
    }

    void ToggleVisibility() {
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                if (!worldCellVisible[x,y] && !worldCellDirty[x,y]) return;

                worldCellDirty[x,y] = false;
                List<MeshRenderer> cellContents = cellListsMR[x,y];
                for (int i=0;i<cellContents.Count;i++) {
                    if (worldCellVisible[x,y]) {
                        cellContents[i].enabled = true;
                    } else {
                        cellContents[i].enabled = false;
                    }
                }
            }
        }
    }

    public void Cull() {
        if (!UpdatedPlayerCell()) return;

        DetermineVisibleCells(); // Reevaluate visible cells from new pos.
        ToggleVisibility(); // Update all cells marked as dirty.

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
