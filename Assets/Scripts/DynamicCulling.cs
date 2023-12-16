using System;
using System.IO;
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
    public bool[,] worldCellCheckedYet = new bool [WORLDX,WORLDX];
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

    private byte[] bytes;
    private static string openDebugImagePath;
    private static string visDebugImagePath;
    private Color32[] pixels;
    private Texture2D debugTex;

    public static DynamicCulling a;

    // Standard culling: 2.21ms to 2.44ms at game start, no camera motion.
    // Plus CullResultsCreateShared: 0.20ms to 0.34ms

    // Plain radius based culling (this first iteration):  97ms !!
    // Fixed: ___ms

    void Awake() {
        a = this;
        a.Cull_Init();
        openDebugImagePath = Utils.SafePathCombine(
                                 Application.streamingAssetsPath,
                                 "worldcellopen.png");
        visDebugImagePath = Utils.SafePathCombine(
                                Application.streamingAssetsPath,
                                "worldcellvis.png");
        a.pixels = new Color32[WORLDX * WORLDX];
    }

    public bool EulerAnglesWithin90(Vector3 angs) {
        if (angs.x % 90 >= 0.05f) return false;
        if (angs.z % 90 >= 0.05f) return false;
        if (angs.y % 90 >= 0.05f) return false;
        return true; 
    }

    void ClearCellList() {
        worldCellVisible = new bool [WORLDX,WORLDX];
        worldCellCheckedYet = new bool [WORLDX,WORLDX];
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
        debugTex = new Texture2D(64,64);
        pixels = new Color32[WORLDX * WORLDX];
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {
                pixels[x + (y * 64)] = Color.black;
            }
        }

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
                        SetOpenPixel(x,y);
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

        openDebugImagePath = Utils.SafePathCombine(
            Application.streamingAssetsPath,
            "worldcellopen.png");
        visDebugImagePath = Utils.SafePathCombine(
            Application.streamingAssetsPath,
            "worldcellvis.png");

         // Output Debug image of the open
        debugTex.SetPixels32(pixels);
        debugTex.Apply();
        bytes = debugTex.EncodeToPNG();
        File.WriteAllBytes(openDebugImagePath,bytes);
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

        if (!cullEnabled) return;

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

    Vector2Int PosToCellCoords(Vector3 pos) {
        Vector2Int retval = new Vector2Int(0,0);
        retval.x = (int)((pos.x - worldMin.x + 1.28f) / 2.56f);
        if (retval.x > 63) retval.x = 63;
        if (retval.x < 0) retval.x = 0;
        retval.y = (int)((pos.z - worldMin.z + 1.28f) / 2.56f);
        if (retval.y > 63) retval.y = 63;
        if (retval.y < 0) retval.y = 0;
        return retval;
    }

    bool UpdatedPlayerCell() {
        int lastX = playerCellX;
        int lastY = playerCellY;
        Vector2Int spot = PosToCellCoords(PlayerMovement.a.transform.position);
        playerCellX = spot.x;
        playerCellY = spot.y;
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

    void SetVisible(int x, int y, Color col) {
        if (!XYPairInBounds(x,y)) return;
        if (worldCellCheckedYet[x,y]) return;

        worldCellVisible[x,y] = IsOpen(x,y);
        worldCellCheckedYet[x,y] = true;
        SetVisPixel(x,y,col);
    }

    void DetermineVisibleCells() {
        debugTex = new Texture2D(64,64);
        int x,y;
        for (x=0;x<64;x++) {
            for (y=0;y<64;y++) {
                worldCellVisible[x,y] = false;
                pixels[x + (y * 64)] = worldCellOpen[x,y] ? Color.white : Color.black;
            }
        }

        x = playerCellX; y = playerCellY;
        worldCellVisible[x,y] = true;
        worldCellCheckedYet[x,y] = true;
        SetVisPixel(x,y,Color.blue);

        // Set all neighboring cells visible if open in 3x3 square.
        Color c = Color.cyan;
        SetVisible(x - 1,y + 1,c); SetVisible(x,y + 1,c); SetVisible(x + 1,y + 1,c);

        SetVisible(x - 1,y,c);     /* Player Position*/   SetVisible(x + 1,y,c);

        SetVisible(x - 1,y - 1,c); SetVisible(x,y - 1,c); SetVisible(x + 1,y - 1,c);

        // [ ] = cell, empty means not checked
        // [1] = starting point or last loop's current, assumed visible.
        // [2] = current
        // [3] = neighbors we should be able to see if [2] could be.

        //CastStraightX(1);  // [ ][3]
                           // [1][2]
                           // [ ][3]

        //CastStraightX(-1); // [3][ ]
                           // [2][1]
                           // [3][ ]

        //CastStraightY(1);  // [3][2][3]
                           // [ ][1][ ]

        //CastStraightY(-1); // [ ][1][ ]
                           // [3][2][3]

        //Cast45(1,1);       // [3][2]
                           // [1][3]

        //Cast45(-1,1);      // [2][3]
                           // [3][1]

        //Cast45(-1,-1);     // [3][1]
                           // [2][3]

        //Cast45(1,-1);      // [1][3]
                           // [3][2]

        for (x=1;x<63;x++) CastRay(playerCellX,playerCellY,x,0);
        for (x=1;x<63;x++) CastRay(playerCellX,playerCellY,x,63);
        for (y=1;y<63;y++) CastRay(playerCellX,playerCellY,0,y);
        for (y=1;y<63;y++) CastRay(playerCellX,playerCellY,63,y);

        if (XYPairInBounds(playerCellX + 1,playerCellY)) {
            for (x=1;x<63;x++) CastRay(playerCellX + 1,playerCellY,x,0);
            for (x=1;x<63;x++) CastRay(playerCellX + 1,playerCellY,x,63);
            for (y=1;y<63;y++) CastRay(playerCellX + 1,playerCellY,0,y);
            for (y=1;y<63;y++) CastRay(playerCellX + 1,playerCellY,63,y);
        }

        if (XYPairInBounds(playerCellX + 1,playerCellY + 1)) {
            for (x=1;x<63;x++) CastRay(playerCellX + 1,playerCellY + 1,x,0);
            for (x=1;x<63;x++) CastRay(playerCellX + 1,playerCellY + 1,x,63);
            for (y=1;y<63;y++) CastRay(playerCellX + 1,playerCellY + 1,0,y);
            for (y=1;y<63;y++) CastRay(playerCellX + 1,playerCellY + 1,63,y);
        }

        if (XYPairInBounds(playerCellX,playerCellY + 1)) {
            for (x=1;x<63;x++) CastRay(playerCellX,playerCellY + 1,x,0);
            for (x=1;x<63;x++) CastRay(playerCellX,playerCellY + 1,x,63);
            for (y=1;y<63;y++) CastRay(playerCellX,playerCellY + 1,0,y);
            for (y=1;y<63;y++) CastRay(playerCellX,playerCellY + 1,63,y);
        }

        if (XYPairInBounds(playerCellX - 1,playerCellY + 1)) {
            for (x=1;x<63;x++) CastRay(playerCellX - 1,playerCellY + 1,x,0);
            for (x=1;x<63;x++) CastRay(playerCellX - 1,playerCellY + 1,x,63);
            for (y=1;y<63;y++) CastRay(playerCellX - 1,playerCellY + 1,0,y);
            for (y=1;y<63;y++) CastRay(playerCellX - 1,playerCellY + 1,63,y);
        }

        if (XYPairInBounds(playerCellX - 1,playerCellY)) {
            for (x=1;x<63;x++) CastRay(playerCellX - 1,playerCellY,x,0);
            for (x=1;x<63;x++) CastRay(playerCellX - 1,playerCellY,x,63);
            for (y=1;y<63;y++) CastRay(playerCellX - 1,playerCellY,0,y);
            for (y=1;y<63;y++) CastRay(playerCellX - 1,playerCellY,63,y);
        }

        if (XYPairInBounds(playerCellX - 1,playerCellY - 1)) {
            for (x=1;x<63;x++) CastRay(playerCellX - 1,playerCellY - 1,x,0);
            for (x=1;x<63;x++) CastRay(playerCellX - 1,playerCellY - 1,x,63);
            for (y=1;y<63;y++) CastRay(playerCellX - 1,playerCellY - 1,0,y);
            for (y=1;y<63;y++) CastRay(playerCellX - 1,playerCellY - 1,63,y);
        }

        if (XYPairInBounds(playerCellX,playerCellY - 1)) {
            for (x=1;x<63;x++) CastRay(playerCellX,playerCellY - 1,x,0);
            for (x=1;x<63;x++) CastRay(playerCellX,playerCellY - 1,x,63);
            for (y=1;y<63;y++) CastRay(playerCellX,playerCellY - 1,0,y);
            for (y=1;y<63;y++) CastRay(playerCellX,playerCellY - 1,63,y);
        }

        if (XYPairInBounds(playerCellX + 1,playerCellY - 1)) {
            for (x=1;x<63;x++) CastRay(playerCellX + 1,playerCellY - 1,x,0);
            for (x=1;x<63;x++) CastRay(playerCellX + 1,playerCellY - 1,x,63);
            for (y=1;y<63;y++) CastRay(playerCellX + 1,playerCellY - 1,0,y);
            for (y=1;y<63;y++) CastRay(playerCellX + 1,playerCellY - 1,63,y);
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

        // Output Debug image of the open
        debugTex.SetPixels32(pixels);
        debugTex.Apply();
        bytes = debugTex.EncodeToPNG();
        File.WriteAllBytes(visDebugImagePath,bytes);
    }

    void SetOpenPixel(int x, int y){
        if (!XYPairInBounds(x,y)) return;

        pixels[x + (y * 64)] = worldCellOpen[x,y] ? Color.white : Color.black;
    }

    void SetVisPixel(int x, int y, Color col){
        if (!XYPairInBounds(x,y)) return;

        pixels[x + (y * 64)] = worldCellVisible[x,y]
                               ? col
                               : (worldCellOpen[x,y] ? Color.white
                                                     : Color.black);
    }

    private void CastStraightY(int signy) {
        if (signy > 0 && playerCellY >= 63) return;
        if (signy < 0 && playerCellY <= 0) return;

        int x = playerCellX;
        int y = playerCellY + signy;
        bool currentVisible = false;
        bool leftVisible = false;
        bool rightVisible = false;
        for (;y<64;y+=signy) { // Up
            currentVisible = false;
            if (XYPairInBounds(x,y - signy)
                && XYPairInBounds(x,y)) {

                if (!worldCellCheckedYet[x,y] || !worldCellOpen[x,y]) {
                    if (worldCellVisible[x,y - signy]) {
                        worldCellVisible[x,y] = worldCellOpen[x,y];
                        worldCellCheckedYet[x,y] = true;
                        currentVisible = true; // Would be if twas open.
                        SetVisPixel(x,y,Color.green);
                    }
                } else {
                    currentVisible = worldCellOpen[x,y]; // Keep going.
                }
            }

            if (XYPairInBounds(x + 1,y)) {
                if (!worldCellCheckedYet[x + 1,y] || !worldCellOpen[x + 1,y]) {
                    worldCellVisible[x + 1,y] = IsOpen(x + 1,y);
                    worldCellCheckedYet[x + 1,y] = true;
                    leftVisible = false; // Would be if twas open.
                    SetVisPixel(x + 1,y,Color.green);
                }
            }

            if (XYPairInBounds(x - 1,y)) {
                if (!worldCellCheckedYet[x - 1,y] || !worldCellOpen[x - 1,y]) {
                    worldCellVisible[x - 1,y] = IsOpen(x - 1,y);
                    worldCellCheckedYet[x - 1,y] = true;
                    rightVisible = false; // Would be if twas open.
                    SetVisPixel(x - 1,y,Color.green);
                }
            }

            if (!(currentVisible || leftVisible || rightVisible)) {
                pixels[x + (y * 64)] = new Color(0.5f,0f,0f,1f);
                break; // Hit wall!
            }
        }
    }

    private void CastStraightX(int signx) {
        if (signx > 0 && playerCellX >= 63) return;
        if (signx < 0 && playerCellX <= 0) return;

        int x = playerCellX + signx;
        int y = playerCellY;
        bool currentVisible = false;
        bool leftVisible = false;
        bool rightVisible = false;
        for (;x<64;x+=signx) {
            currentVisible = false;
            if (XYPairInBounds(x - signx,y)
                && XYPairInBounds(x,y)) {

                if (!worldCellCheckedYet[x,y] || !worldCellOpen[x,y]) {
                    if (worldCellVisible[x - signx,y]) {
                        worldCellVisible[x,y] = worldCellOpen[x,y];
                        worldCellCheckedYet[x,y] = true;
                        currentVisible = true; // Would be if twas open.
                        SetVisPixel(x,y,Color.green);
                    }
                } else {
                    currentVisible = worldCellOpen[x,y]; // Keep going.
                }
            }

            if (XYPairInBounds(x,y + 1)) {
                if (!worldCellCheckedYet[x,y + 1] || !worldCellOpen[x,y + 1]) {
                    worldCellVisible[x,y + 1] = IsOpen(x,y + 1);
                    worldCellCheckedYet[x,y + 1] = true;
                    leftVisible = false; // Would be if twas open.
                    SetVisPixel(x,y + 1,Color.green);
                }
            }

            if (XYPairInBounds(x,y - 1)) {
                if (!worldCellCheckedYet[x,y - 1] || !worldCellOpen[x,y - 1]) {
                    worldCellVisible[x,y - 1] = IsOpen(x,y - 1);
                    worldCellCheckedYet[x,y - 1] = true;
                    rightVisible = false; // Would be if twas open.
                    SetVisPixel(x,y - 1,Color.green);
                }
            }

            if (!(currentVisible || leftVisible || rightVisible)) {
                pixels[x + (y * 64)] = new Color(0.5f,0f,0f,1f);
                break; // Hit wall!
            }
        }
    }

    private void Cast45(int signx, int signy) {
        int x = playerCellX;
        int y = playerCellY;
        bool neighbor1,neighbor2;
        bool currentVisible = true;
        int xofs,yofs;
        for (int iter=0;iter<64;iter++) {
            currentVisible = false;
            x+=signx; y+=signy; // Always increment else continue stops early.
            if (!XYPairInBounds(x,y)) break;

            xofs = x - signx;
            yofs = y - signy;
            neighbor1 = IsOpen(xofs,y);
            neighbor2 = IsOpen(x,yofs);
            if (!neighbor1 && !neighbor2) break; // Don't see thru closed corner

            if (!worldCellCheckedYet[x,y] || !worldCellOpen[x,y]) {
                worldCellVisible[x,y] = currentVisible = IsOpen(x,y);
                worldCellCheckedYet[x,y] = true;
                SetVisPixel(x,y,Color.magenta);
            } else {
                currentVisible = worldCellOpen[x,y]; // Keep going.
            }

            if (XYPairInBounds(xofs,y)) {
                if (!worldCellCheckedYet[xofs,y] || !worldCellOpen[xofs,y]) {
                    worldCellVisible[xofs,y] = neighbor1;
                    worldCellCheckedYet[xofs,y] = true;
                    SetVisPixel(xofs,y,Color.magenta);
                }
            }

            if (XYPairInBounds(x,yofs)) {
                if (!worldCellCheckedYet[x,yofs] || !worldCellOpen[x,yofs]) {
                    worldCellVisible[x,yofs] = neighbor2;
                    worldCellCheckedYet[x,yofs] = true;
                    SetVisPixel(x,yofs,Color.magenta);
                }
            }

            if (!currentVisible) {
                pixels[x + (y * 64)] = new Color(0.5f,0f,0f,1f);
                break; // Hit wall!  Break after neighbors.
            }
        }
    }

    public void CastRay(int x0, int y0, int x1, int y1) {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;
        int x = x0;
        int y = y0;
        int iter = Mathf.Max(dx,dy);
        while (iter > 0) {
            if (CastRayCellCheck(x,y) == -1) return;
            if (x == x1 && y == y1) return;

            int e2 = 2 * err;
            if (e2 > -dy) {
                err -= dy;
                x += sx;
            }

            if (e2 < dx) {
                err += dx;
                y += sy;
            }

            iter--;
        }
    }

    int CastRayCellCheck(int x, int y) {
        if (XYPairInBounds(x,y)) {
           if (!worldCellCheckedYet[x,y] || !worldCellOpen[x,y]) {
                worldCellVisible[x,y] = worldCellOpen[x,y];
                worldCellCheckedYet[x,y] = true;
                if (!worldCellVisible[x,y]) {
                    pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                    return -1;
                }

                SetVisPixel(x,y,new Color(0f,0f,0.5f,1f));
                return 1;
            }
        }

        return 0;
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
        label_iterate_mesh_renderers:
        int count = dynamicMeshes.Count;
        for (int i=0;i < count; i++) {
            if (dynamicMeshes[i] == null) {
                dynamicMeshes.RemoveAt(i);
                goto label_iterate_mesh_renderers; // Start over
            }
        }

        for (int i=0;i < count; i++) {
            dynamicMeshCoords[i] = PosToCellCoords(dynamicMeshes[i].transform.position);
        }
    }

    public void UpdateNPCPVS() {
        label_iterate_aics:
        int count = npcAICs.Count;
        for (int i=0;i < count; i++) {
            if (npcAICs[i] == null) {
                npcAICs.RemoveAt(i);
                goto label_iterate_aics; // Start over
            }
        }

        for (int i=0;i < count; i++) {
            npcCoords[i] = PosToCellCoords(npcTransforms[i].position);
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
                for (y=0;y<64;y++) {
                    worldCellLastVisible[x,y] = worldCellVisible[x,y];
                    worldCellCheckedYet[x,y] = false;
                }
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

