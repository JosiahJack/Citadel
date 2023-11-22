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
    public List<MeshRenderer> staticMeshesImmutable = new List<MeshRenderer>();
    public List<Vector2Int> staticMeshImmutableCoords = new List<Vector2Int>();
    public bool[,] worldCellLastVisible = new bool[WORLDX,WORLDX];
    public int playerCellX = 0;
    public int playerCellY = 0;
    public float deltaX = 0.0f;
    public float deltaY = 0.0f;
    public Vector3 worldMax;
    public Vector3 worldMin;
    public List<GameObject> orthogonalChunks;
    public List<Transform> npcTransforms;
    public List<AIController> npcAICs = new List<AIController>();
    public List<Vector2Int> npcCoords = new List<Vector2Int>();

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
        staticMeshesImmutable = new List<MeshRenderer>();
        staticMeshesImmutable.Clear();
        staticMeshImmutableCoords = new List<Vector2Int>();
        staticMeshImmutableCoords.Clear();
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

    public void FindDynamicMeshes() {
        GameObject container = LevelManager.a.GetCurrentDynamicContainer();
        Component[] compArray = container.GetComponentsInChildren(typeof(MeshRenderer),true);
        int count = container.transform.childCount;
        MeshRenderer mr = null;
        for (int i=0;i<count;i++) {
            mr = container.transform.GetChild(i).GetComponent<MeshRenderer>();
            if (mr == null) continue;

            dynamicMeshes.Add(mr);
            dynamicMeshCoords.Add(Vector2Int.zero);
        }
    }

    public void FindStaticMeshesImmutable() {
        GameObject container = LevelManager.a.GetCurrentStaticImmutableContainer();
        Component[] compArray = container.GetComponentsInChildren(typeof(MeshRenderer),true);
        int count = container.transform.childCount;
        MeshRenderer mr = null;
        for (int i=0;i<count;i++) {
            mr = container.transform.GetChild(i).GetComponent<MeshRenderer>();
            if (mr == null) continue;

            staticMeshesImmutable.Add(mr);
            staticMeshImmutableCoords.Add(Vector2Int.zero);
        }
    }

    public void FindAllNPCsForLevel() {
        GameObject container = LevelManager.a.GetRequestedLevelNPCContainer(LevelManager.a.currentLevel);
        Component[] compArray = container.GetComponentsInChildren(typeof(AIController),true);
        int count = container.transform.childCount;
        AIController aic = null;
        for (int i=0;i<count;i++) {
            aic = container.transform.GetChild(i).GetComponent<AIController>();
            if (aic == null) continue;

            npcAICs.Add(aic);
            npcTransforms.Add(container.transform.GetChild(i));
            npcCoords.Add(Vector2Int.zero);
        }
    }

    public void PutNPCInCell(int index) {
        Vector3 pos = npcTransforms[index].position;
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                if (Vector3.Distance(pos,worldCellPositions[x,y]) < 1.28f) {
                    npcCoords[index] = new Vector2Int(x,y);
                    return;
                }
            }
        }
    }

    public void PutNPCsInCells() {
        int count = npcTransforms.Count;
        for (int i=0;i<count;i++) {
            PutNPCInCell(i);
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
        int count = dynamicMeshes.Count;
        for (int i=0;i<count;i++) {
            PutDynamicMeshInCell(i);
        }
    }

    public void PutStaticMeshImmutableInCell(int index) {
        Vector3 pos = staticMeshesImmutable[index].transform.position;
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                if (Vector3.Distance(pos,worldCellPositions[x,y]) < 1.28f) {
                    staticMeshImmutableCoords[index] = new Vector2Int(x,y);
                    return;
                }
            }
        }
    }

    public void PutStaticMeshesImmutableInCells() {
        int count = staticMeshesImmutable.Count;
        for (int i=0;i<count;i++) {
            PutStaticMeshImmutableInCell(i);
        }
    }

    public void Cull_Init() {
        // Setup and find all cullables and associate them with x,y coords.
        ClearCellList();
        FindOrthogonalChunks(LevelManager.a.GetCurrentGeometryContainer());
        FindWorldExtents(orthogonalChunks);
        FindOpenCellsAndPositions(orthogonalChunks);
        PutChunksInCells();

        FindDynamicMeshes();
        PutDynamicMeshesInCells();

        FindStaticMeshesImmutable();
        PutStaticMeshesImmutableInCells();

        FindAllNPCsForLevel();
        PutNPCsInCells();

        // Do first Cull pass
        FindPlayerCell();
        DetermineVisibleCells(); // Reevaluate visible cells from new pos.

        // Force all cells dirty at start so the visibility is toggled for all.
        int x,y;
        for (x=0;x<64;x++) {
            for (y=0;y<64;y++) worldCellLastVisible[x,y] = !worldCellVisible[x,y];
        }

        // Update all cells marked as dirty.
        ToggleVisibility();
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

    bool XYPairInBounds(int x, int y) {
        return (x < 64 && y < 64 && x >= 0 && y >= 0);
    }

    bool IsOpen(int x, int y) {
        if (!XYPairInBounds(x,y)) return false;
        return worldCellOpen[x,y];
    }

    void DetermineVisibleCells() {
        bool[,] vis = new bool[64,64];
        int x,y,xofs,yofs;
        for (x=0;x<64;x++) {
            for (y=0;y<64;y++) vis[x,y] = false;
        }

        worldCellVisible[playerCellX,playerCellY] = true;
        vis[playerCellX,playerCellY] = true;
        bool currentVisible = true; // Mark if twere open if visible.

        x = playerCellX + 1;
        y = playerCellY;
        worldCellVisible[x,y] = vis[x,y] = IsOpen(x,y);

        x = playerCellX + 1;
        y = playerCellY + 1;
        worldCellVisible[x,y] = vis[x,y] = IsOpen(x,y);

        x = playerCellX;
        y = playerCellY + 1;
        worldCellVisible[x,y] = vis[x,y] = IsOpen(x,y);

        x = playerCellX - 1;
        y = playerCellY + 1;
        worldCellVisible[x,y] = vis[x,y] = IsOpen(x,y);

        x = playerCellX - 1;
        y = playerCellY;
        worldCellVisible[x,y] = vis[x,y] = IsOpen(x,y);

        x = playerCellX - 1;
        y = playerCellY - 1;
        worldCellVisible[x,y] = vis[x,y] = IsOpen(x,y);

        x = playerCellX;
        y = playerCellY - 1;
        worldCellVisible[x,y] = vis[x,y] = IsOpen(x,y);

        x = playerCellX + 1;
        y = playerCellY - 1;
        worldCellVisible[x,y] = vis[x,y] = IsOpen(x,y);

        // Skip 0 and 63 corners since 45deg rays below get them.
//         for (x=1;x<63;x++) CastRay(x,0);
//         for (x=1;x<63;x++) CastRay(x,63);
//         for (y=1;y<63;y++) CastRay(0,y);
//         for (y=1;y<63;y++) CastRay(63,y);

        for (x=0;x<64;x++) CastRay(playerCellX,playerCellY,x,0);
        for (x=0;x<64;x++) CastRay(playerCellX + 1,playerCellY,x,0);
        for (x=0;x<64;x++) CastRay(playerCellX - 1,playerCellY,x,0);
        for (x=0;x<64;x++) CastRay(playerCellX + 1,playerCellY + 1,x,0);
        for (x=0;x<64;x++) CastRay(playerCellX - 1,playerCellY + 1,x,0);
        for (x=0;x<64;x++) CastRay(playerCellX - 1,playerCellY - 1,x,0);
        for (x=0;x<64;x++) CastRay(playerCellX + 1,playerCellY - 1,x,0);
        for (x=0;x<64;x++) CastRay(playerCellX,playerCellY + 1,x,0);
        for (x=0;x<64;x++) CastRay(playerCellX,playerCellY - 1,x,0);

        for (x=0;x<64;x++) CastRay(playerCellX,playerCellY,x,63);
        for (x=0;x<64;x++) CastRay(playerCellX,playerCellY + 1,x,63);
        for (x=0;x<64;x++) CastRay(playerCellX,playerCellY - 1,x,63);
        for (x=0;x<64;x++) CastRay(playerCellX + 1,playerCellY + 1,x,63);
        for (x=0;x<64;x++) CastRay(playerCellX - 1,playerCellY + 1,x,63);
        for (x=0;x<64;x++) CastRay(playerCellX - 1,playerCellY - 1,x,63);
        for (x=0;x<64;x++) CastRay(playerCellX + 1,playerCellY - 1,x,63);
        for (x=0;x<64;x++) CastRay(playerCellX,playerCellY + 1,x,63);
        for (x=0;x<64;x++) CastRay(playerCellX,playerCellY - 1,x,63);

        for (y=0;y<64;y++) CastRay(playerCellX,playerCellY,0,y);
        for (y=0;y<64;y++) CastRay(playerCellX,playerCellY + 1,0,y);
        for (y=0;y<64;y++) CastRay(playerCellX,playerCellY - 1,0,y);
        for (y=0;y<64;y++) CastRay(playerCellX + 1,playerCellY + 1,0,y);
        for (y=0;y<64;y++) CastRay(playerCellX - 1,playerCellY + 1,0,y);
        for (y=0;y<64;y++) CastRay(playerCellX - 1,playerCellY - 1,0,y);
        for (y=0;y<64;y++) CastRay(playerCellX + 1,playerCellY - 1,0,y);
        for (y=0;y<64;y++) CastRay(playerCellX,playerCellY + 1,0,y);
        for (y=0;y<64;y++) CastRay(playerCellX,playerCellY - 1,0,y);

        for (y=0;y<64;y++) CastRay(playerCellX,playerCellY,63,y);
        for (y=0;y<64;y++) CastRay(playerCellX,playerCellY + 1,63,y);
        for (y=0;y<64;y++) CastRay(playerCellX,playerCellY - 1,63,y);
        for (y=0;y<64;y++) CastRay(playerCellX + 1,playerCellY + 1,63,y);
        for (y=0;y<64;y++) CastRay(playerCellX - 1,playerCellY + 1,63,y);
        for (y=0;y<64;y++) CastRay(playerCellX - 1,playerCellY - 1,63,y);
        for (y=0;y<64;y++) CastRay(playerCellX + 1,playerCellY - 1,63,y);
        for (y=0;y<64;y++) CastRay(playerCellX,playerCellY + 1,63,y);
        for (y=0;y<64;y++) CastRay(playerCellX,playerCellY - 1,63,y);

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
                if (vis[x - 1,playerCellY]) {
                    if (worldCellOpen[x,playerCellY]) vis[x,playerCellY] = true;
                    currentVisible = true; // Would be if twas open.
                }

                if (!currentVisible) break; // Hit wall!

                vis[x,playerCellY + 1] = IsOpen(x,playerCellY + 1);
                vis[x,playerCellY - 1] = IsOpen(x,playerCellY - 1);
            }

            if (playerCellY > 0) {
                for (x=playerCellX + 1;x<64;x++) { // Right, South neighbor
                    currentVisible = false;
                    xofs = x - 1;
                    yofs = playerCellY - 1;
                    if (XYPairInBounds(xofs,yofs)) {
                        if (vis[xofs,yofs]) {
                            vis[x,yofs] = IsOpen(x,yofs);
                            currentVisible = true; // Would be if twas open.
                        }
                    }
                }
            }

            if (playerCellY < 63) {
                for (x=playerCellX + 1;x<64;x++) { // Right, North neighbor
                    currentVisible = false;
                    xofs = x - 1;
                    yofs = playerCellY + 1;
                    if (XYPairInBounds(xofs,yofs)) {
                        if (vis[xofs,yofs]) {
                            vis[x,yofs] = IsOpen(x,yofs);
                            currentVisible = true; // Would be if twas open.
                        }
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
                if (vis[x + 1,playerCellY]) {
                    if (worldCellOpen[x,playerCellY]) vis[x,playerCellY] = true;
                    currentVisible = true; // Would be if twas open.
                }

                if (!currentVisible) break; // Hit wall!

                vis[x,playerCellY + 1] = IsOpen(x,playerCellY + 1);
                vis[x,playerCellY - 1] = IsOpen(x,playerCellY - 1);
            }

            if (playerCellY > 0) {
                for (x=playerCellX - 1;x>=0;x--) { // Left, South neighbor
                    currentVisible = false;
                    xofs = x + 1;
                    yofs = playerCellY - 1;
                    if (XYPairInBounds(xofs,yofs)) {
                        if (vis[xofs,yofs]) {
                            vis[x,yofs] = IsOpen(x,yofs);
                            currentVisible = true; // Would be if twas open.
                        }
                    }
                }
            }

            if (playerCellY < 63) {
                for (x=playerCellX - 1;x>=0;x--) { // Left, North neighbor
                    currentVisible = false;
                    xofs = x + 1;
                    yofs = playerCellY + 1;
                    if (XYPairInBounds(xofs,yofs)) {
                        if (vis[xofs,yofs]) {
                            vis[x,yofs] = IsOpen(x,yofs);
                            currentVisible = true; // Would be if twas open.
                        }
                    }
                }
            }
        }

        // [3][2][3]
        // [ ][1][ ]
        if (playerCellY < 63) {
            for (y=playerCellY + 1;y<64;y++) { // Up
                currentVisible = false;
                if (vis[playerCellX,y - 1]) {
                    if (worldCellOpen[playerCellX,y]) vis[playerCellX,y] = true;
                    currentVisible = true; // Would be if twas open.
                }

                if (!currentVisible) break; // Hit wall!

                vis[playerCellX + 1,y] = IsOpen(playerCellX + 1,y);
                vis[playerCellX - 1,y] = IsOpen(playerCellX - 1,y);
            }

            if (playerCellX < 63) {
                for (y=playerCellY + 1;y<63;y++) { // Up, right neighbor
                    currentVisible = false;
                    xofs = playerCellX + 1;
                    yofs = y - 1;
                    if (XYPairInBounds(xofs,yofs)) {
                        if (vis[xofs,yofs]) {
                            vis[xofs,y] = IsOpen(xofs,y);
                            currentVisible = true; // Would be if twas open.
                        }
                    }
                }
            }

            if (playerCellX > 0) {
                for (y=playerCellY + 1;y<63;y++) { // Up, left neighbor
                    currentVisible = false;
                    xofs = playerCellX - 1;
                    yofs = y - 1;
                    if (XYPairInBounds(xofs,yofs)) {
                        if (vis[xofs,yofs]) {
                            vis[xofs,y] = IsOpen(xofs,y);
                            currentVisible = true; // Would be if twas open.
                        }
                    }
                }
            }
        }

        // [ ][1][ ]
        // [3][2][3]
        if (playerCellY > 0) {
            for (y=playerCellY - 1;y>=0;y--) { // Down
                currentVisible = false;
                if (vis[playerCellX,y + 1]) {
                    if (worldCellOpen[playerCellX,y]) vis[playerCellX,y] = true;
                    currentVisible = true; // Would be if twas open.
                }

                if (!currentVisible) break; // Hit wall!

                vis[playerCellX + 1,y] = IsOpen(playerCellX + 1,y);
                vis[playerCellX - 1,y] = IsOpen(playerCellX - 1,y);
            }

            if (playerCellX > 0) {
                for (y=playerCellY - 1;y>=0;y--) { // Down, left neighbor
                    currentVisible = false;
                    xofs = playerCellX - 1;
                    yofs = y + 1;
                    if (XYPairInBounds(xofs,yofs)) {
                        if (vis[xofs,yofs]) {
                            vis[xofs,y] = IsOpen(xofs,y);
                            currentVisible = true; // Would be if twas open.
                        }
                    }
                }
            }

            if (playerCellX < 63) {
                for (y=playerCellY - 1;y>=0;y--) { // Down, right neighbor
                    currentVisible = false;
                    xofs = playerCellX + 1;
                    yofs = y + 1;
                    if (XYPairInBounds(xofs,yofs)) {
                        if (vis[xofs,yofs]) {
                            vis[xofs,y] = IsOpen(xofs,y);
                            currentVisible = true; // Would be if twas open.
                        }
                    }
                }
            }
        }

        bool north = false;
        bool west = false;
        bool south = false;
        bool east = false;

        // [3][2]
        // [1][3]
        x = playerCellX + 1;
        y = playerCellY + 1;
        for (int iter=0;iter<64;iter++) { // Up to Right
            xofs = x - 1;
            yofs = y - 1;
            west = IsOpen(xofs,y); // The two [3] cells.
            south = IsOpen(x,yofs);
            if (!west && !south) break; // Don't see through closed [3]s corner.

            vis[x,y] = currentVisible = IsOpen(x,y);
            vis[xofs,y] = west;  // [west][2    ]  Check the two [3] cells
            vis[x,yofs] = south; // [1   ][south]
            if (!currentVisible) break; // Hit wall!  Break after opening [3]s.

            x++; y++;
            if (!XYPairInBounds(x,y)) break;
        }

        // [2][3]
        // [3][1]
        x = playerCellX - 1;
        y = playerCellY + 1;
        for (int iter=0;iter<64;iter++) { // Up to Right
            xofs = x + 1;
            yofs = y - 1;
            east = IsOpen(xofs,y); // The two [3] cells.
            south = IsOpen(x,yofs);
            if (!east && !south) break; // Don't see through closed [3]s corner.

            vis[x,y] = currentVisible = IsOpen(x,y);
            vis[xofs,y] = east;  // [2    ][east]  Check the two [3] cells
            vis[x,yofs] = south; // [south][1   ]
            if (!currentVisible) break; // Hit wall!  Break after opening [3]s.

            x--; y++;
            if (!XYPairInBounds(x,y)) break;
        }

        // [3][1]
        // [2][3]
        x = playerCellX - 1;
        y = playerCellY - 1;
        for (int iter=0;iter<64;iter++) { // Up to Right
            xofs = x + 1;
            yofs = y + 1;
            north = IsOpen(x,yofs); // The two [3] cells.
            east = IsOpen(xofs,y);
            if (!east && !south) break; // Don't see through closed [3]s corner.

            vis[x,y] = currentVisible = IsOpen(x,y);
            vis[x,yofs] = north;// [north][1   ]  Check the two [3] cells
            vis[xofs,y] = east; // [2    ][east]
            if (!currentVisible) break; // Hit wall!  Break after opening [3]s.

            x--; y--;
            if (!XYPairInBounds(x,y)) break;
        }

        // [1][3]
        // [3][2]
        x = playerCellX + 1;
        y = playerCellY - 1;
        for (int iter=0;iter<64;iter++) { // Up to Right
            xofs = x - 1;
            yofs = y + 1;
            north = IsOpen(x,yofs); // The two [3] cells.
            west = IsOpen(xofs,y);
            if (!east && !south) break; // Don't see through closed [3]s corner.

            vis[x,y] = currentVisible = IsOpen(x,y);
            vis[x,yofs] = north;  // [1   ][north]  Check the two [3] cells
            vis[xofs,y] = west;   // [west][2    ]
            if (!currentVisible) break; // Hit wall!  Break after opening [3]s.

            x++; y--;
            if (!XYPairInBounds(x,y)) break;
        }

        for (x=0;x<64;x++) {
            for (y=0;y<64;y++) {
                worldCellVisible[x,y] = vis[x,y];
            }
        }

        // Pattern of custom axial rays, 3 wide each (center only shown).
        // [ ][ ][ ][ ][ ][ ][ ][4][ ][ ][ ][ ][ ][ ][ ][6]
        // [7][ ][ ][ ][ ][ ][ ][4][ ][ ][ ][ ][ ][ ][6][ ]
        // [ ][7][ ][ ][ ][ ][ ][4][ ][ ][ ][ ][ ][6][ ][ ]
        // [ ][ ][7][ ][ ][ ][ ][4][ ][ ][ ][ ][6][ ][ ][ ]
        // [ ][ ][ ][7][ ][ ][ ][4][ ][ ][ ][6][ ][ ][ ][ ]
        // [ ][ ][ ][ ][7][ ][ ][4][ ][ ][6][ ][ ][ ][ ][ ]
        // [ ][ ][ ][ ][ ][7][ ][4][ ][6][ ][ ][ ][ ][ ][ ]
        // [ ][ ][ ][ ][ ][ ][1][1][1][ ][ ][ ][ ][ ][ ][ ]
        // [3][3][3][3][3][3][1][0][1][2][2][2][2][2][2][2]
        // [ ][ ][ ][ ][ ][ ][1][1][1][ ][ ][ ][ ][ ][ ][ ]
        // [ ][ ][ ][ ][ ][8][ ][5][ ][9][ ][ ][ ][ ][ ][ ]
        // [ ][ ][ ][ ][8][ ][ ][5][ ][ ][9][ ][ ][ ][ ][ ]
        // [ ][ ][ ][8][ ][ ][ ][5][ ][ ][ ][9][ ][ ][ ][ ]
        // [ ][ ][8][ ][ ][ ][ ][5][ ][ ][ ][ ][9][ ][ ][ ]
        // [ ][8][ ][ ][ ][ ][ ][5][ ][ ][ ][ ][ ][9][ ][ ]
        // [8][ ][ ][ ][ ][ ][ ][5][ ][ ][ ][ ][ ][ ][9][ ]
    }

    /*
    private void CastRay(int x1, int y1, int x2, int y2) {
        if (!XYPairInBounds(x1,y1) || !XYPairInBounds(x2,y2)) return;

        int deltaX = x2 - x1;//playerCellX;
        int deltaY = y2 - y1;//playerCellY;
        float absdx = Mathf.Abs(deltaX);
        float absdy = Mathf.Abs(deltaY);
        float majorAxisSteps = absdx > absdy ? absdx : absdy;
        float xIncrement = (float)deltaX / majorAxisSteps;
        float yIncrement = (float)deltaY / majorAxisSteps;
        float x = (float)x1;//playerCellX;
        float y = (float)y1;//playerCellY;
        bool visibleLast = worldCellOpen[x1,y1];
        for (float step = 0; step <= majorAxisSteps; step+=1f) {
            int xint = (int)x;
            int yint = (int)y;
            int xrint = (int)Mathf.Ceil(x);
            int yrint = (int)Mathf.Ceil(y);
            if (visibleLast) {
                if (worldCellOpen[xint,yint]) {
                    worldCellVisible[xint,yint] = true;
                    visibleLast = true;
                }

                bool xroundThreshMet = (xrint != xint);
                bool yroundThreshMet = (yrint != yint);
                if (xroundThreshMet) {
                    if (worldCellOpen[xrint,yint]) {
                        worldCellVisible[xrint,yint] = true;
                        visibleLast = true;
                    }
                }

                if (yroundThreshMet) {
                    if (worldCellOpen[xint,yrint]) {
                        worldCellVisible[xint,yrint] = true;
                        visibleLast = true;
                    }
                }

                if (xroundThreshMet && yroundThreshMet) {
                    if (worldCellOpen[xrint,yrint]) {
                        worldCellVisible[xrint,yrint] = true;
                        visibleLast = true;
                    }
                }
            } else break;

            x += xIncrement;
            if (x < 0f || x > 63f) break;
            y += yIncrement;
            if (y < 0f || y > 63f) break;
        }
    }*/

    private void CastRay(int x1, int y1, int x2, int y2) {
        int deltaX = x2 - x1;
        int deltaY = y2 - y1;
        int majorAxisSteps = Mathf.Abs(deltaX) > Mathf.Abs(deltaY) ?
        Mathf.Abs(deltaX) : Mathf.Abs(deltaY);

        float xIncrement = (float)deltaX / majorAxisSteps;
        float yIncrement = (float)deltaY / majorAxisSteps;
        int x = x1;
        int y = y1;
        bool visibleLast = true; // Assume starting point is player's cell.
        for (int step = 0; step <= majorAxisSteps; step++) {

            if (x >= 0 && x < 64 && y >= 0 && y < 64) {
                if (visibleLast) {
                    worldCellVisible[x,y] = IsOpen(x,y);
                    visibleLast = worldCellVisible[x,y];
                } else break;
            }
            x += Mathf.RoundToInt(xIncrement);
            y += Mathf.RoundToInt(yIncrement);
        }
    }

    void ToggleVisibility() {
        worldCellLastVisible[playerCellX,playerCellY] = worldCellVisible[playerCellX,playerCellY] = true;
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                if (worldCellLastVisible[x,y] == worldCellVisible[x,y]) continue;

                List<MeshRenderer> cellContents = cellListsMR[x,y];
                int count = cellContents.Count;
                for (int i=0;i<count;i++) {
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

    public void UpdateDynamicMeshes() {
        int x,y,lastX,lastY;
        Vector3 pos;
        float deltaX,deltaY;

        label_iterate_mesh_renderers:
        int count = dynamicMeshes.Count;
        for (int i=0;i < count; i++) {
            if (dynamicMeshes[i] == null) {
                dynamicMeshes.RemoveAt(i);
                goto label_iterate_mesh_renderers; // Start over
            }
        }

        for (int i=0;i < count; i++) {
            x = dynamicMeshCoords[i].x;
            y = dynamicMeshCoords[i].y;
            pos = dynamicMeshes[i].transform.position;
            deltaX = pos.x - worldCellPositions[x,y].x;
            deltaY = pos.z - worldCellPositions[x,y].z;
            lastX = x;
            lastY = y;
            if (deltaX > CELLXHALF) x++;
            else if (deltaX < -CELLXHALF) x--;

            if (x < 0) x = 0;
            if (x > 63) x = 63;
            if (deltaY > CELLXHALF) y++;
            else if (deltaY < -CELLXHALF) y--;

            if (y < 0) y = 0;
            if (y > 63) y = 63;
            dynamicMeshCoords[i] = new Vector2Int(x,y);
        }
    }

    public void UpdateNPCPVS() {
        int x,y,lastX,lastY;
        Vector3 pos;
        float deltaX,deltaY;

        label_iterate_aics:
        int count = npcAICs.Count;
        for (int i=0;i < count; i++) {
            if (npcAICs[i] == null) {
                npcAICs.RemoveAt(i);
                goto label_iterate_aics; // Start over
            }

            x = npcCoords[i].x;
            y = npcCoords[i].y;
            pos = npcTransforms[i].position;
            deltaX = pos.x - worldCellPositions[x,y].x;
            deltaY = pos.z - worldCellPositions[x,y].z;
            lastX = x;
            lastY = y;
            // if (deltaX > 2.56f || deltaY > 2.56f
            //     || deltaX < -2.56f || deltaY < -2.56f) {
            //     PutNPCInCell(i);
            //     return;
            // }

            if (deltaX > CELLXHALF) x++;
            else if (deltaX < -CELLXHALF) x--;

            if (x < 0) x = 0;
            if (x > 63) x = 63;
            if (deltaY > CELLXHALF) y++;
            else if (deltaY < -CELLXHALF) y--;

            if (y < 0) y = 0;
            if (y > 63) y = 63;
            npcCoords[i] = new Vector2Int(x,y);
        }
    }

     // Avoid NPC doing raycasts when not in player's PVS.  Symmetrical vision.
    public void ToggleNPCPVS() {
        for (int i=0;i<npcAICs.Count;i++) {
            if (worldCellVisible[npcCoords[i].x,npcCoords[i].y]) {
                npcAICs[i].withinPVS = true;
                Utils.Activate(npcAICs[i].visibleMeshEntity);
            } else {
                npcAICs[i].withinPVS = false;
                Utils.Deactivate(npcAICs[i].visibleMeshEntity);
            }
        }
    }

    public void ToggleDynamicMeshesVisibility() {
        for (int i=0;i<dynamicMeshes.Count;i++) {
            if (worldCellVisible[dynamicMeshCoords[i].x,dynamicMeshCoords[i].y]) {
                dynamicMeshes[i].enabled = true;
            } else {
                dynamicMeshes[i].enabled = false;
            }
        }
    }

    public void ToggleStaticMeshesImmutableVisibility() {
        for (int i=0;i<staticMeshesImmutable.Count;i++) {
            if (worldCellVisible[staticMeshImmutableCoords[i].x,
                                 staticMeshImmutableCoords[i].y]) {

                staticMeshesImmutable[i].enabled = true;
            } else {
                staticMeshesImmutable[i].enabled = false;
            }
        }
    }

    public void Cull() {
        if (!cullEnabled) return;

        // Now handle player position updating PVS
        if (UpdatedPlayerCell()) {
            int x,y;
            for (x=0;x<64;x++) {
                for (y=0;y<64;y++) worldCellLastVisible[x,y] = worldCellVisible[x,y];
            }

            DetermineVisibleCells(); // Reevaluate visible cells from new pos.
            ToggleVisibility(); // Update all cells marked as dirty.
            UpdateNPCPVS();
            ToggleNPCPVS();
        }

        // Update dynamic meshes after PVS has been updated, if player moved.
        UpdateDynamicMeshes(); // Always check all of them because any can move.
        ToggleDynamicMeshesVisibility(); // Now turn them on or off.
        ToggleStaticMeshesImmutableVisibility();
    }
}

