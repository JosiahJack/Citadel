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
    public List<MeshRenderer> staticMeshesSaveable = new List<MeshRenderer>();
    public List<Vector2Int> staticMeshSaveableCoords = new List<Vector2Int>();
    public List<Light> lights = new List<Light>();
    public List<Vector2Int> lightCoords = new List<Vector2Int>();
    public List<MeshRenderer> doors = new List<MeshRenderer>();
    public List<Vector2Int> doorsCoords = new List<Vector2Int>();

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

    void ClearCellList() {
        worldCellVisible = new bool [WORLDX,WORLDX];
        worldCellCheckedYet = new bool [WORLDX,WORLDX];
        worldCellOpen = new bool [WORLDX,WORLDX];
        worldCellPositions = new Vector3 [WORLDX,WORLDX];
        staticMeshesImmutable = new List<MeshRenderer>();
        staticMeshesImmutable.Clear();
        staticMeshImmutableCoords = new List<Vector2Int>();
        staticMeshImmutableCoords.Clear();
        staticMeshesSaveable = new List<MeshRenderer>();
        staticMeshesSaveable.Clear();
        staticMeshSaveableCoords = new List<Vector2Int>();
        staticMeshSaveableCoords.Clear();
        doors = new List<MeshRenderer>();
        doors.Clear();
        doorsCoords = new List<Vector2Int>();
        doorsCoords.Clear();
        lights = new List<Light>();
        lights.Clear();
        lightCoords = new List<Vector2Int>();
        lightCoords.Clear();
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

    // ========================================================================
    // Handle Occluders (well, just determining visible cells and their chunks)

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
//         debugTex = new Texture2D(64,64);
//         pixels = new Color32[WORLDX * WORLDX];
//         for (int x=0;x<64;x++) {
//             for (int y=0;y<64;y++) {
//                 pixels[x + (y * 64)] = Color.black;
//             }
//         }

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
//                         pixels[x + (y * 64)] = worldCellOpen[x,y]
//                                                ? Color.white
//                                                : Color.black;

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

//         openDebugImagePath = Utils.SafePathCombine(
//             Application.streamingAssetsPath,
//             "worldcellopen.png");
//         visDebugImagePath = Utils.SafePathCombine(
//             Application.streamingAssetsPath,
//             "worldcellvis.png");

         // Output Debug image of the open
//         debugTex.SetPixels32(pixels);
//         debugTex.Apply();
//         bytes = debugTex.EncodeToPNG();
//         File.WriteAllBytes(openDebugImagePath,bytes);
    }

    void FindOrthogonalChunks(GameObject chunkContainer) {
        orthogonalChunks = new List<GameObject>();
        orthogonalChunks.Clear();
        GameObject childGO = null;
        Transform container = chunkContainer.transform;
        int chunkCount = container.childCount;
        for (int i=0; i < chunkCount; i++) {
			childGO = container.GetChild(i).gameObject;
            orthogonalChunks.Add(childGO);
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
    }

    // ========================================================================
    // Handle Occludees

    private void AddMeshRenderer(int type, MeshRenderer mr) {
        if (mr == null) return;

        switch(type) {
            case 1:
                dynamicMeshes.Add(mr);
                dynamicMeshCoords.Add(Vector2Int.zero);
                break;
            case 2:
                doors.Add(mr);
                doorsCoords.Add(Vector2Int.zero);
                break;
            case 3: break; // NPCs done different due to SkinnedMeshRenderer's.
            case 4:
                staticMeshesSaveable.Add(mr);
                staticMeshSaveableCoords.Add(Vector2Int.zero);
                break;
            case 5: break; // Lights done differently due to Light (what, it makes sense).
            default:
                staticMeshesImmutable.Add(mr);
                staticMeshImmutableCoords.Add(Vector2Int.zero);
                break;
        }
    }

    public void FindMeshRenderers(int type) {
        GameObject container = null;
        switch(type) {
            case 1: container = LevelManager.a.GetCurrentDynamicContainer(); break;
            case 2: container = LevelManager.a.GetCurrentDoorsContainer(); break;
            case 3: container = LevelManager.a.GetRequestedLevelNPCContainer(LevelManager.a.currentLevel); break;
            case 4: container = LevelManager.a.GetCurrentStaticSaveableContainer(); break;
            case 5: container = LevelManager.a.GetCurrentLightsContainer(); break;
            default: container = LevelManager.a.GetCurrentStaticImmutableContainer(); break;
        }

        Component[] compArray = container.GetComponentsInChildren(typeof(MeshRenderer),true);
        int count = container.transform.childCount;
        Transform parent = null;
        Transform child = null;
        AIController aic = null;
        Light lit = null;
        for (int i=0;i<count;i++) {
            if (type == 3) { // NPC
                aic = container.transform.GetChild(i).GetComponent<AIController>();
                if (aic == null) continue;

                npcAICs.Add(aic);
                npcTransforms.Add(container.transform.GetChild(i));
                npcCoords.Add(Vector2Int.zero);
            } else if (type == 5) { // Lights
                lit = container.transform.GetChild(i).GetComponent<Light>();
                if (lit == null) continue;

                lights.Add(lit);
                lightCoords.Add(Vector2Int.zero);
            } else {
                parent = container.transform.GetChild(i);
                AddMeshRenderer(type,parent.GetComponent<MeshRenderer>());
                for (int j=0;j<parent.childCount;j++) {
                    child = parent.GetChild(j);
                    AddMeshRenderer(type,child.GetComponent<MeshRenderer>());
                }
            }
        }
    }

    public void PutMeshesInCells(int type) {
        int count = 0;
        switch(type) {
            case 1: count = dynamicMeshes.Count; break;
            case 2: count = doors.Count; break;
            case 3: count = npcTransforms.Count; break;
            case 4: count = staticMeshesSaveable.Count; break;
            case 5: count = lights.Count; break;
            default: count = staticMeshesImmutable.Count; break;
        }

        for (int index=0;index<count;index++) {
            switch(type) {
                case 1: dynamicMeshCoords[index] = PosToCellCoords(dynamicMeshes[index].transform.position); break;
                case 2: doorsCoords[index] = PosToCellCoords(doors[index].transform.position); break;
                case 3: npcCoords[index] = PosToCellCoords(npcTransforms[index].position); break;
                case 4: staticMeshSaveableCoords[index] = PosToCellCoords(staticMeshesSaveable[index].transform.position); break;
                case 5: lightCoords[index] = PosToCellCoords(lights[index].transform.position); break;
                default: staticMeshImmutableCoords[index] = PosToCellCoords(staticMeshesImmutable[index].transform.position); break;
            }
        }
    }

    public void Cull_Init() {
        // Setup and find all cullables and associate them with x,y coords.
        ClearCellList();
        FindOrthogonalChunks(LevelManager.a.GetCurrentGeometryContainer());
        FindWorldExtents(orthogonalChunks);
        FindOpenCellsAndPositions(orthogonalChunks);
        PutChunksInCells();
        FindMeshRenderers(0); // Static Immutable
        FindMeshRenderers(1); // Dynamic
        FindMeshRenderers(2); // Doors
        FindMeshRenderers(3); // NPCs
        FindMeshRenderers(4); // Static Saveable
        FindMeshRenderers(5); // Lights
        PutMeshesInCells(0); // Static Immutable
        PutMeshesInCells(1); // Dynamic
        PutMeshesInCells(2); // Doors
        PutMeshesInCells(3); // NPCs
        PutMeshesInCells(4); // Static Saveable
        PutMeshesInCells(5); // Lights

        // --------------------------------------------------------------------
        // Do first Cull pass
        FindPlayerCell();
        DetermineVisibleCells(); // Reevaluate visible cells from new pos.
        if (!cullEnabled) return;

        // Force all cells dirty at start so the visibility is toggled for all.
        int x,y;
        for (x=0;x<64;x++) {
            for (y=0;y<64;y++) worldCellLastVisible[x,y] = !worldCellVisible[x,y];
        }

        ToggleVisibility(); // Update all cells marked as dirty.
        ToggleStaticMeshesImmutableVisibility();
        ToggleStaticMeshesSaveableVisibility();
        ToggleDoorsVisibility();
        ToggleLightsVisibility();
        UpdateNPCPVS();
        ToggleNPCPVS();
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
        int x,y;
        x = (int)((pos.x - worldMin.x + 1.28f) / 2.56f);
        if (x > 63) x = 63;
        else if (x < 0) x = 0;
        y = (int)((pos.z - worldMin.z + 1.28f) / 2.56f);
        if (y > 63) y = 63;
        else if (y < 0) y = 0;

        return new Vector2Int(x,y);
    }

    int WorldXToCellX(float playerX) {
        int x;
        x = (int)((playerX - worldMin.x + 1.28f) / 2.56f);
        if (x > 63) x = 63;
        else if (x < 0) x = 0;
        return x;
    }

    int WorldZToCellY(float playerZ) {
        int y;
        y = (int)((playerZ - worldMin.z + 1.28f) / 2.56f);
        if (y > 63) y = 63;
        else if (y < 0) y = 0;
        return y;
    }

    bool UpdatedPlayerCell() {
        int lastX = playerCellX;
        int lastY = playerCellY;
        playerCellX = WorldXToCellX(PlayerMovement.a.transform.position.x);
        playerCellY = WorldZToCellY(PlayerMovement.a.transform.position.z);
        if (playerCellX == lastX && playerCellY == lastY) return false;
        return true;
    }

    bool XYPairInBounds(int x, int y) {
        return (x < 64 && y < 64 && x >= 0 && y >= 0);
    }

    void SetVisible(int x, int y) {
        if (!XYPairInBounds(x,y)) return;
        if (worldCellCheckedYet[x,y]) return;

        worldCellVisible[x,y] = worldCellOpen[x,y];
        worldCellCheckedYet[x,y] = true;
        //SetVisPixel(x,y,Color.cyan);
    }

    void DetermineVisibleCells() {
        //debugTex = new Texture2D(64,64);
        int x,y;
        for (x=0;x<64;x++) {
            for (y=0;y<64;y++) {
                worldCellVisible[x,y] = false;
//                 pixels[x + (y * 64)] = worldCellOpen[x,y]
//                                        ? Color.white
//                                        : Color.black;
            }
        }

        x = playerCellX; y = playerCellY;
        worldCellVisible[x,y] = true;
        worldCellCheckedYet[x,y] = true;
//         SetVisPixel(x,y,Color.blue);

        // Set all neighboring cells visible if open in 3x3 square.
        SetVisible(x - 1,y + 1); SetVisible(x,y + 1); SetVisible(x + 1,y + 1);

        SetVisible(x - 1,y);     /* Player Position*/   SetVisible(x + 1,y);

        SetVisible(x - 1,y - 1); SetVisible(x,y - 1); SetVisible(x + 1,y - 1);

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

        // Output Debug image of the open
//         debugTex.SetPixels32(pixels);
//         debugTex.Apply();
//         bytes = debugTex.EncodeToPNG();
//         File.WriteAllBytes(visDebugImagePath,bytes);
    }

//     void SetVisPixel(int x, int y, Color col){
//         pixels[x + (y * 64)] = worldCellVisible[x,y]
//                                ? col
//                                : (worldCellOpen[x,y] ? Color.white
//                                                      : Color.black);
//     }

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
//                     pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                    return -1;
                }

//                 SetVisPixel(x,y,new Color(0f,0f,0.5f,1f));
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
        HealthManager hm = null;
        for (int i=0;i<npcAICs.Count;i++) {
            if (worldCellVisible[npcCoords[i].x,npcCoords[i].y]) {
                npcAICs[i].withinPVS = true;
                hm = npcAICs[i].healthManager;
                if (npcAICs[i].currentState == AIState.Dead) {

                    if (npcAICs[i].index == 0 || npcAICs[i].index == 14 // Autobomb, hopper
                        || hm.teleportOnDeath || hm.gibOnDeath) {

                        Utils.Deactivate(npcAICs[i].visibleMeshEntity);
                    }
                } else if (npcAICs[i].currentState == AIState.Dying) {
                    if (npcAICs[i].index == 0 || npcAICs[i].index == 14 // Autobomb, hopper
                        || hm.teleportOnDeath) {
                            Utils.Deactivate(npcAICs[i].visibleMeshEntity);
                        } else {
                            Utils.Activate(npcAICs[i].visibleMeshEntity);
                        }
                } else {
                    Utils.Activate(npcAICs[i].visibleMeshEntity);
                }
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
            if (worldCellVisible[staticMeshImmutableCoords[i].x,staticMeshImmutableCoords[i].y]) {
                staticMeshesImmutable[i].enabled = true;
            } else {
                staticMeshesImmutable[i].enabled = false;
            }
        }
    }

    public void ToggleStaticMeshesSaveableVisibility() {
        for (int i=0;i<staticMeshesSaveable.Count;i++) {
            if (worldCellVisible[staticMeshSaveableCoords[i].x,staticMeshSaveableCoords[i].y]) {
                staticMeshesSaveable[i].enabled = true;
            } else {
                staticMeshesSaveable[i].enabled = false;
            }
        }
    }

    public void ToggleDoorsVisibility() {
        for (int i=0;i<doors.Count;i++) {
            if (worldCellVisible[doorsCoords[i].x,doorsCoords[i].y]) {
                doors[i].enabled = true;
            } else {
                doors[i].enabled = false;
            }
        }
    }

    public void ToggleLightsVisibility() {
        for (int i=0;i<lights.Count;i++) {
            if (worldCellVisible[lightCoords[i].x,lightCoords[i].y]) {
                lights[i].enabled = true;
            } else {
                lights[i].enabled = false;
            }
        }
    }

    public void Cull() {
        if (!cullEnabled || LevelManager.a.currentLevel == 13) return;

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
            worldCellVisible[0,0] = true; // Errors default here so draw them anyways.
            ToggleVisibility(); // Update all cells marked as dirty.
            ToggleStaticMeshesImmutableVisibility();
            ToggleStaticMeshesSaveableVisibility();
            ToggleDoorsVisibility();
            ToggleLightsVisibility();
            UpdateNPCPVS();
            ToggleNPCPVS();
        }

        // Update dynamic meshes after PVS has been updated, if player moved.
        //UpdateDynamicMeshes(); // Always check all of them because any can move.
        //ToggleDynamicMeshesVisibility(); // Now turn them on or off.
    }
}

