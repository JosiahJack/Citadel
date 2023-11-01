using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCulling : MonoBehaviour {
    const int WORLDX = 64;
    const int ARRSIZE = WORLDX * WORLDX;
    const float CELLXHALF = 1.28f;
    public bool[,] worldCellOpen = new bool [WORLDX,WORLDX];
    public bool[,] worldCellVisible = new bool [WORLDX,WORLDX];
    public Vector3[,] worldCellPositions = new Vector3 [WORLDX,WORLDX];
    public List<GameObject>[,] cellLists = new List<GameObject>[WORLDX,WORLDX];
    public GameObject[,] debugCubes = new GameObject[WORLDX,WORLDX];
    public List<MeshRenderer>[,] cellListsMR = new List<MeshRenderer>[WORLDX,WORLDX];
    public bool[,] worldCellDirty = new bool[WORLDX,WORLDX];
    public bool playerCellChanged;
    public int playerCellX = 0;
    public int playerCellY = 0;
    public float deltaX = 0.0f;
    public float deltaY = 0.0f;
    public Vector3 worldMax;
    public Vector3 worldMin;
    public List<GameObject> orthogonalChunks;
    public bool started = false;

    public static DynamicCulling a;

    // Standard culling: 2.21ms to 2.44ms at game start, no camera motion.
    // Plus CullResultsCreateShared: 0.20ms to 0.34ms

    // Plain radius based culling (this first iteration):  97ms !!
    // Fixed: ___ms

    void Awake() {
        a = this;
        a.Cull_Init();
        a.started = false;
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
                cellListsMR[x,y] = new List<MeshRenderer>();
                cellListsMR[x,y].Clear();
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
            pos2dcurrent.x = worldMin.x + (2.56f * (float)x);
            for (int y=0; y<64; y++) {
                breaky = false;
                pos2dcurrent.y = worldMin.z + (2.56f * (float)y);
                for (int i=0; i < chunks.Count; i++) {
                    childGO = chunks[i].gameObject;
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

        for (int x=0; x<64; x++) {
            for (int y=0; y<64; y++) {
                if (worldCellOpen[x,y]) continue;

                worldCellPositions[x,y] = new Vector3(
                    worldMin.x + ((float)x * 2.56f)/* - (15f * 2.56f)*/,
                    -43.52f,
                    worldMin.z + ((float)y * 2.56f)
                );
            }
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
                if (worldCellOpen[x,y]) {
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

//         for (int x=0; x<64; x++) {
//             for (int y=0; y<64; y++) {
//                 if (worldCellOpen[x,y]) {
//                     debugCubes[x,y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                     debugCubes[x,y].transform.position = worldCellPositions[x,y];
//                     MeshRenderer mr = debugCubes[x,y].GetComponent<MeshRenderer>();
//                     mr.material = Const.a.genericMaterials[9]; // Green forcefield
//                 } else {
//                     Debug.Log("Placing red cube at " + worldCellPositions[x,y].ToString());
//                     debugCubes[x,y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                     debugCubes[x,y].transform.position = worldCellPositions[x,y];
//                     MeshRenderer mr = debugCubes[x,y].GetComponent<MeshRenderer>();
//                     mr.material = Const.a.genericMaterials[5]; // Red forcefield
//                 }
//             }
//         }
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
                    return;
                }
            }
        }

        playerCellChanged = true;
    }

    void MarkAllNonVisible() {
//         bool last = false;
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                worldCellVisible[x,y] = false;
//                 if (last != worldCellVisible[x,y]) {
//                     worldCellDirty[playerCellX,playerCellY] = true;
//                 }
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
        int lastX = playerCellX;
        int lastY = playerCellY;
        if (deltaX > CELLXHALF) playerCellX++;
        else if (deltaX < -CELLXHALF) playerCellX--;
        if (playerCellX < 0) playerCellX = 0;
        if (playerCellX > 63) playerCellX = 63;
        if (deltaY > CELLXHALF) playerCellY++;
        else if (deltaY < -CELLXHALF) playerCellY--;
        if (playerCellY < 0) playerCellY = 0;
        if (playerCellY > 63) playerCellY = 63;
        if (playerCellX == lastX && playerCellY == lastY) return false;
        return true;
    }

    // From Bob Nystrom, https://github.com/munificent/fov, converted to C#.
    // Aaand de-OOP'ed of course for performance.
    // https://journal.stuffwithstuff.com/2015/09/07/what-the-hero-sees/
    // =====================================================================

    public Vector2Int TransformOctant(int row, int col, int octant) {
        switch (octant) {
            case 0: return new Vector2Int( col, -row);
            case 1: return new Vector2Int( row, -col);
            case 2: return new Vector2Int( row,  col);
            case 3: return new Vector2Int( col,  row);
            case 4: return new Vector2Int(-col,  row);
            case 5: return new Vector2Int(-row,  col);
            case 6: return new Vector2Int(-row, -col);
            case 7: return new Vector2Int(-col, -row);
        }

        return new Vector2Int(col,row);
    }

    public void RefreshOctant(int octant) {
        Vector2Int start = new Vector2Int(playerCellX,playerCellY);
        List<Shadow> shadows = new List<Shadow>();
        bool fullShadow = false;
        bool visible = true;
        Vector2Int pos = new Vector2Int(0,0);
        for (int row = 1; row < 64; row++) {
            pos = (start + TransformOctant(row,0,octant));
            if (pos.x >= 64 || pos.y >= 64 || pos.x < 0 || pos.y < 0) break;

            for (int col = 0; col <= row; col++) {
                pos = start + TransformOctant(row,col,octant);
                if (pos.x >= 64 || pos.y >= 64 || pos.x < 0 || pos.y < 0) break;

                if (fullShadow) {
                    worldCellVisible[pos.x,pos.y] = false;
                } else {
                    Shadow projection = ProjectTile(row, col);
                    visible = true;
                    for (int i=0;i< shadows.Count;i++) {
                        if (shadows[i].start <= projection.start
                            && shadows[i].end >= projection.end) {

                            visible = false;
                        }
                    }

                    worldCellVisible[pos.x,pos.y] = visible;
                    if (visible && !worldCellOpen[pos.x,pos.y]) {
                        shadows.Add(projection);
                        fullShadow = (shadows.Count == 1
                                      && shadows[0].start == 0
                                      && shadows[0].end == 1);
                    }
                }
            }
        }
    }

    public Shadow ProjectTile(int row, int col) {
        int topLeft = col / (row + 2);
        int bottomRight = (col + 1) / (row + 1);
        Shadow newshad = new Shadow();
        newshad.start = topLeft;
        newshad.end = bottomRight;
        return newshad;
    }

    public struct Shadow {
        public int start;
        public int end;
    }

    // =====================================================================

    void DetermineVisibleCells() {
        MarkAllNonVisible();
        for (int octant=0;octant<8;octant++) RefreshOctant(octant);
        worldCellVisible[playerCellX,playerCellY] = true;
        worldCellDirty[playerCellX,playerCellY] = true;
    }

    void ToggleVisibility() {
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                //if (!worldCellDirty[x,y]) continue;

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

//         for (int x=0; x<64; x++) {
//             for (int y=0; y<64; y++) {
//                 if (worldCellVisible[x,y]) {
//                     MeshRenderer mr = debugCubes[x,y].GetComponent<MeshRenderer>();
//                     mr.material = Const.a.genericMaterials[8]; // Blue forcefield
//                 } else {
//                     MeshRenderer mr = debugCubes[x,y].GetComponent<MeshRenderer>();
//                     mr.material = Const.a.genericMaterials[9]; // Green forcefield
//                 }
//             }
//         }
    }

    public void Cull() {
        if (!UpdatedPlayerCell() && started) return;

        DetermineVisibleCells(); // Reevaluate visible cells from new pos.
        ToggleVisibility(); // Update all cells marked as dirty.
        started = true;
    }
}
