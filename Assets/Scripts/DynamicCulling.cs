using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCulling : MonoBehaviour {
    public bool cullEnabled = false;
    const int WORLDX = 64;
    const int ARRSIZE = WORLDX * WORLDX;
    const float CELLXHALF = 1.28f;
    public bool[,] worldCellOpen = new bool [WORLDX,WORLDX];
    public bool[,] worldCellVisible = new bool [WORLDX,WORLDX];
    public Vector3[,] worldCellPositions = new Vector3 [WORLDX,WORLDX];
    public List<GameObject>[,] cellLists = new List<GameObject>[WORLDX,WORLDX];
    public GameObject[,] debugCubes = new GameObject[WORLDX,WORLDX];
    public List<MeshRenderer>[,] cellListsMR = new List<MeshRenderer>[WORLDX,WORLDX];
    public List<MeshRenderer> dynamicMeshes = new List<MeshRenderer>();
    public List<Vector2Int> dynamicMeshCoords = new List<Vector2Int>();
    public bool[,] worldCellDirty = new bool[WORLDX,WORLDX];
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
        worldCellVisible = new bool [WORLDX,WORLDX];
        worldCellOpen = new bool [WORLDX,WORLDX];
        worldCellPositions = new Vector3 [WORLDX,WORLDX];
        dynamicMeshes = new List<MeshRenderer>();
        dynamicMeshes.Clear();
        dynamicMeshCoords = new List<Vector2Int>();
        dynamicMeshCoords.Clear();
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                cellLists[x,y] = new List<GameObject>();
                cellLists[x,y].Clear();
                cellListsMR[x,y] = new List<MeshRenderer>();
                cellListsMR[x,y].Clear();
                worldCellVisible[x,y] = false;
                worldCellOpen[x,y] = false;
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

         // for (int x=0; x<64; x++) {
         //     for (int y=0; y<64; y++) {
         //         if (worldCellOpen[x,y]) {
         //             debugCubes[x,y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
         //             debugCubes[x,y].transform.position = worldCellPositions[x,y];
         //             MeshRenderer mr = debugCubes[x,y].GetComponent<MeshRenderer>();
         //             mr.material = Const.a.genericMaterials[9]; // Green
         //         } else {
         //             debugCubes[x,y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
         //             debugCubes[x,y].transform.position = worldCellPositions[x,y];
         //             MeshRenderer mr = debugCubes[x,y].GetComponent<MeshRenderer>();
         //             mr.material = Const.a.genericMaterials[5]; // Red
         //         }
         //     }
         // }
    }

    void MarkAllNonVisible() {
        bool last = false;
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                last = worldCellVisible[x,y];
                worldCellVisible[x,y] = false;
                if (last != worldCellVisible[x,y]) {
                    worldCellDirty[playerCellX,playerCellY] = true;
                }
            }
        }
    }

    public void FindDynamicMeshes() {
        GameObject container = LevelManager.a.GetCurrentDynamicContainer();
        Component[] compArray = childGO.GetComponentsInChildren(typeof(MeshRenderer),true);
        foreach (MeshRenderer mr in compArray) {
            dynamicMeshes.Add(mr);
            dynamicMeshCoords.Add(Vector2Int.zero);
        }
    }

    public void PutDynamicMeshInCell(int index) {
        Vector3 pos = dynamicMeshes[index].transform.position;
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                if (Vector3.Distance(pos,worldCellPositions[x,y]) < 1.28f) {
                    dynamicMeshCoords[index] = new Vector2Int(x,y);
                    return;
                }
            }
        }
    }

    public void PutDynamicMeshesInCells() {
        for (int i=0;i<dynamicMeshes.Count;i++) {
            PutDynamicMeshInCell(i);
        }
    }

    public void UpdateDynamicMeshes() {
        int x,y,lastX,lastY;
        Vector3 pos;
        float deltaX,deltaY;

        for (int i=0;i<dynamicMeshes.Count;i++) {
            x = dynamicMeshCoords[i].x;
            y = dynamicMeshCoords[i].y;
            Vector3 pos = dynamicMeshes[i].transform.position;
            deltaX = pos.x -
            worldCellPositions[x,y].x;
            deltaY = pos.z -
            worldCellPositions[x,y].z;
            lastX = x;
            lastY = y;
            if (deltaX > 2.56f || deltaY > 2.56f
                || deltaX < -2.56f || deltaY < -2.56f) {
                PutDynamicMeshInCell(i);
                return;
            }

            if (deltaX > CELLXHALF) x++;
            else if (deltaX < -CELLXHALF) x--;

            if (x < 0) x = 0;
            if (x > 63) x = 63;
            if (deltaY > CELLXHALF) y++;
            else if (deltaY < -CELLXHALF) y--;

            if (y < 0) y = 0;
            if (y > 63) y = 63;
            return;
        }
    }

    public void Cull_Init() {
        ClearCellList();
        FindOrthogonalChunks(LevelManager.a.GetCurrentGeometryContainer());
        FindWorldExtents(orthogonalChunks);
        FindOpenCellsAndPositions(orthogonalChunks);
        PutChunksInCells();
        FindDynamicMeshes();
        PutDynamicMeshesInCells();
        FindPlayerCell();
        DetermineVisibleCells(); // Reevaluate visible cells from new pos.
        ToggleVisibility(); // Update all cells marked as dirty.
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
    }

    bool UpdatedPlayerCell() {
        Vector3 pos = PlayerMovement.a.transform.position;
        deltaX = pos.x - worldCellPositions[playerCellX,playerCellY].x;
        deltaY = pos.z - worldCellPositions[playerCellX,playerCellY].z;
        int lastX = playerCellX;
        int lastY = playerCellY;
        if (deltaX > 2.56f || deltaY > 2.56f
            || deltaX < -2.56f || deltaY < -2.56f) {
            FindPlayerCell();
            Debug.Log("Innaccuracy, correcting player location via brute "
                      + "force.");

            if (playerCellX == lastX && playerCellY == lastY) return false;
            return true;
        }


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

    bool MarkVisible(int x, int y) {
        if (worldCellOpen[x,y]) {
            worldCellVisible[x,y] = true;
            return true;
        } else return false;
    }

    void DetermineVisibleCells() {
        MarkAllNonVisible();
        worldCellVisible[playerCellX,playerCellY] = true;
        worldCellDirty[playerCellX,playerCellY] = true;
        int x,y;
        bool currentVisible = false; // Mark if twere open if visible.

        // Skip 0 and 63 corners since 45deg rays below get them.
        for (x=1;x<63;x++) CastRay(x,0);
        for (x=1;x<63;x++) CastRay(x,63);
        for (y=1;y<63;y++) CastRay(0,y);
        for (y=1;y<63;y++) CastRay(63,y);

        // [ ] = cell, empty means not checked
        // [1] = starting point or last loop's current, assumed visible.
        // [2] = current
        // [3] = neighbors we should be able to see if [2] could be.

        // [ ][3]
        // [1][2]
        // [ ][3]
        if (playerCellX < 63) {
            for (x=playerCellX + 1;x<64;x++) { // Right
                currentVisible = false;
                if (worldCellVisible[x - 1,playerCellY]) {
                    MarkVisible(x,playerCellY);
                    currentVisible = true;
                }

                if (currentVisible) {
                    MarkVisible(x,playerCellY + 1);
                    MarkVisible(x,playerCellY - 1);
                } else break;
            }

            if (playerCellY > 0) {
                for (x=playerCellX + 1;x<64;x++) { // Right, South neighbor
                    currentVisible = false;
                    if (worldCellVisible[x - 1,playerCellY - 1]) {
                        MarkVisible(x,playerCellY - 1);
                        currentVisible = true;
                    }
                }
            }

            if (playerCellY < 63) {
                for (x=playerCellX + 1;x<64;x++) { // Right, North neighbor
                    currentVisible = false;
                    if (worldCellVisible[x - 1,playerCellY + 1]) {
                        MarkVisible(x,playerCellY + 1);
                        currentVisible = true;
                    }
                }
            }
        }

        // [3][ ]
        // [2][1]
        // [3][ ]
        if (playerCellX > 0) {
            for (x=playerCellX - 1;x>=0;x--) { // Left
                currentVisible = false;
                if (worldCellVisible[x + 1,playerCellY]) {
                    MarkVisible(x,playerCellY);
                    currentVisible = true;
                }

                if (currentVisible) {
                    MarkVisible(x,playerCellY + 1);
                    MarkVisible(x,playerCellY - 1);
                } else break;
            }

            if (playerCellY > 0) {
                for (x=playerCellX - 1;x>=0;x--) { // Left, South neighbor
                    currentVisible = false;
                    if (worldCellVisible[x + 1,playerCellY - 1]) {
                        MarkVisible(x,playerCellY - 1);
                        currentVisible = true;
                    }
                }
            }

            if (playerCellY < 63) {
                for (x=playerCellX - 1;x>=0;x--) { // Left, North neighbor
                    currentVisible = false;
                    if (worldCellVisible[x + 1,playerCellY + 1]) {
                        MarkVisible(x,playerCellY + 1);
                        currentVisible = true;
                    }
                }
            }
        }

        // [3][2][3]
        // [ ][1][ ]
        if (playerCellY < 63) {
            for (y=playerCellY + 1;y<64;y++) { // Up
                currentVisible = false;
                if (worldCellVisible[playerCellX,y - 1]) {
                    MarkVisible(playerCellX,y);
                    currentVisible = true;
                }

                if (currentVisible) {
                    MarkVisible(playerCellX + 1,y);
                    MarkVisible(playerCellX - 1,y);
                } else break;
            }

            if (playerCellX < 63) {
                for (y=playerCellY + 1;y<63;y++) { // Up, right neighbor
                    currentVisible = false;
                    if (worldCellVisible[playerCellX + 1,y - 1]) {
                        MarkVisible(playerCellX + 1,y);
                        currentVisible = true;
                    }
                }
            }

            if (playerCellX > 0) {
                for (y=playerCellY + 1;y<63;y++) { // Up, left neighbor
                    currentVisible = false;
                    if (worldCellVisible[playerCellX - 1,y - 1]) {
                        MarkVisible(playerCellX - 1,y);
                        currentVisible = true;
                    }
                }
            }
        }

        // [ ][1][ ]
        // [3][2][3]
        if (playerCellY > 0) {
            for (y=playerCellY - 1;y>=0;y--) { // Down
                currentVisible = false;
                if (worldCellVisible[playerCellX,y + 1]) {
                    MarkVisible(playerCellX,y);
                    currentVisible = true;
                }

                if (currentVisible) {
                    MarkVisible(playerCellX + 1,y);
                    MarkVisible(playerCellX - 1,y);
                } else break;
            }

            if (playerCellX > 0) {
                for (y=playerCellY - 1;y>=0;y--) { // Down, left neighbor
                    currentVisible = false;
                    if (worldCellVisible[playerCellX - 1,y + 1]) {
                        MarkVisible(playerCellX - 1,y);
                        currentVisible = true;
                    }
                }
            }

            if (playerCellX < 63) {
                for (y=playerCellY - 1;y>=0;y--) { // Down, right neighbor
                    currentVisible = false;
                    if (worldCellVisible[playerCellX + 1,y + 1]) {
                        MarkVisible(playerCellX + 1,y);
                        currentVisible = true;
                    }
                }
            }
        }

        bool diagonal = false;

        // [3][2]
        // [1][3]
        x = playerCellX + 1;
        y = playerCellY + 1;
        for (int iter=0;iter<64;iter++) { // Up to Right
            currentVisible = false;
            diagonal = worldCellVisible[x - 1,y -1] &&
                (worldCellVisible[x - 1,y] || worldCellVisible[x,y - 1]);

            if (   worldCellVisible[x - 1,y]        /* {current} */
                || diagonal                 || worldCellVisible[x,y - 1]) {

                MarkVisible(x,y);
                currentVisible = true;
            }

            x++;
            y++;
            if (x > 63 || y > 63) break;
            if (currentVisible) {
                MarkVisible(x - 1,y);
                MarkVisible(x,y - 1);
            } else break;
        }

        // [2][3]
        // [3][1]
        x = playerCellX - 1;
        y = playerCellY + 1;
        for (int iter=0;iter<64;iter++) { // Up to Left
            currentVisible = false;
            diagonal = worldCellVisible[x + 1,y - 1] &&
                (worldCellVisible[x,y - 1] || worldCellVisible[x + 1,y]);

            if (/* {current} */                 worldCellVisible[x + 1,y]
                || worldCellVisible[x,y - 1] || diagonal                  ) {

                MarkVisible(x,y);
                currentVisible = true;
            }

            x--;
            y++;
            if (x < 0 || y > 63) break;
            if (currentVisible) {
                MarkVisible(x + 1,y);
                MarkVisible(x,y - 1);
            } else break;
        }

        // [3][1]
        // [2][3]
        x = playerCellX - 1;
        y = playerCellY - 1;
        for (int iter=0;iter<64;iter++) { // Down to Left
            currentVisible = false;
            diagonal = worldCellVisible[x + 1,y + 1] &&
                (worldCellVisible[x,y + 1] || worldCellVisible[x + 1,y]);

                if (worldCellVisible[x,y + 1] || diagonal
                /* {current} */               || worldCellVisible[x + 1,y]) {

                MarkVisible(x,y);
                currentVisible = true;
            }

            x--;
            y--;
            if (x < 0 || y < 0) break;
            if (currentVisible) {
                MarkVisible(x,y + 1);
                MarkVisible(x + 1,y);
            } else break;
        }

        // [1][3]
        // [3][2]
        x = playerCellX + 1;
        y = playerCellY - 1;
        for (int iter=0;iter<64;iter++) { // Down to Right
            currentVisible = false;
            diagonal = worldCellVisible[x - 1,y - 1] &&
                (worldCellVisible[x - 1,y] || worldCellVisible[x,y + 1]);

            if (   worldCellVisible[x - 1,y + 1] || worldCellVisible[x,y + 1]
                || worldCellVisible[x - 1,y]          /* {current} */        ) {

                MarkVisible(x,y);
                currentVisible = true;
            }

            x++;
            y--;
            if (x > 63 || y < 0) break;
            if (currentVisible) {
                MarkVisible(x - 1,y);
                MarkVisible(x,y + 1);
            } else break;
        }

        // [ ][ ][ ][ ][ ][ ][ ][4][ ][ ][ ][ ][ ][ ][ ][6]
        // [7][ ][ ][ ][ ][ ][ ][4][ ][ ][ ][ ][ ][ ][6][ ]
        // [ ][7][ ][ ][ ][ ][ ][4][ ][ ][ ][ ][ ][6][ ][ ]
        // [ ][ ][7][ ][ ][ ][ ][4][ ][ ][ ][ ][6][ ][ ][ ]
        // [ ][ ][ ][7][ ][ ][ ][4][ ][ ][ ][6][ ][ ][ ][ ]
        // [ ][ ][ ][ ][7][ ][ ][4][ ][ ][6][ ][ ][ ][ ][ ]
        // [ ][ ][ ][ ][ ][7][ ][4][ ][6][ ][ ][ ][ ][ ][ ]
        // [ ][ ][ ][ ][ ][ ][7][4][6][ ][ ][ ][ ][ ][ ][ ]
        // [3][3][3][3][3][3][3][1][2][2][2][2][2][2][2][2]
        // [ ][ ][ ][ ][ ][ ][8][5][9][ ][ ][ ][ ][ ][ ][ ]
        // [ ][ ][ ][ ][ ][8][ ][5][ ][9][ ][ ][ ][ ][ ][ ]
        // [ ][ ][ ][ ][8][ ][ ][5][ ][ ][9][ ][ ][ ][ ][ ]
        // [ ][ ][ ][8][ ][ ][ ][5][ ][ ][ ][9][ ][ ][ ][ ]
        // [ ][ ][8][ ][ ][ ][ ][5][ ][ ][ ][ ][9][ ][ ][ ]
        // [ ][8][ ][ ][ ][ ][ ][5][ ][ ][ ][ ][ ][9][ ][ ]
        // [8][ ][ ][ ][ ][ ][ ][5][ ][ ][ ][ ][ ][ ][9][ ]
    }

    // x1,y1 = playerCellX,playerCellY
    private void CastRay(int x2, int y2) {
        int deltaX = x2 - playerCellX;
        int deltaY = y2 - playerCellY;
        float absdx = Mathf.Abs(deltaX);
        float absdy = Mathf.Abs(deltaY);
        float majorAxisSteps = absdx > absdy ? absdx : absdy;
        float xIncrement = (float)deltaX / majorAxisSteps;
        float yIncrement = (float)deltaY / majorAxisSteps;
        float x = (float)playerCellX;
        float y = (float)playerCellY;
        bool visibleLast = true; // Assume starting point is player's cell.
        for (float step = 0; step <= majorAxisSteps; step+=majorAxisSteps) {
            int xint = (int)x;
            int yint = (int)y;
            if (visibleLast) visibleLast = MarkVisible(xint,yint);
            else break;

            x += xIncrement;
            if (x < 0f || x > 63f) break;
            y += yIncrement;
            if (y < 0f || y > 63f) break;
        }
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

        // MeshRenderer mr = null;
        // for (int x=0; x<64; x++) {
        //     for (int y=0; y<64; y++) {
        //         if (worldCellVisible[x,y]) {
        //             mr = debugCubes[x,y].GetComponent<MeshRenderer>();
        //             mr.material = Const.a.genericMaterials[8]; // Blue forcefield
        //         } else {
        //             mr = debugCubes[x,y].GetComponent<MeshRenderer>();
        //             mr.material = Const.a.genericMaterials[9]; // Green forcefield
        //         }
        //     }
        // }
        //
        // mr = debugCubes[playerCellX,playerCellY].GetComponent<MeshRenderer>();
        // mr.material = Const.a.genericMaterials[12]; // Indigo forcefield
    }

    public void Cull() {
        UpdateDynamicMeshes();

        // Now handle player position updating PVS
        if (!UpdatedPlayerCell()) return;
        if (!cullEnabled) return;

        DetermineVisibleCells(); // Reevaluate visible cells from new pos.
        ToggleVisibility(); // Update all cells marked as dirty.
    }
}

