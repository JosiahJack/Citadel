using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCulling : MonoBehaviour {
    public bool cullEnabled = false;
    public bool outputDebugImages = false;
    const int WORLDX = 64;
    const int ARRSIZE = WORLDX * WORLDX;
    const float CELLXHALF = 1.28f;
    public bool[,] worldCellOpen = new bool [WORLDX,WORLDX];
    public bool[,] worldCellVisible = new bool [WORLDX,WORLDX];
    public bool[,] worldCellCheckedYet = new bool [WORLDX,WORLDX];
    //public Texture2D[] precalculatedPVS = new Texture2D[ARRSIZE];
    public List<GameObject>[,] cellLists = new List<GameObject>[WORLDX,WORLDX];
    public List<MeshRenderer>[,] cellListsMR = new List<MeshRenderer>[WORLDX,WORLDX];
    public List<MeshRenderer> dynamicMeshes = new List<MeshRenderer>();
    public List<Vector2Int> dynamicMeshCoords = new List<Vector2Int>();
    public List<MeshRenderer> staticMeshesImmutable = new List<MeshRenderer>();
    public List<MeshFilter>   staticMeshesImmutableMFs = new List<MeshFilter>();
    public List<Mesh>         staticMeshesImmutableUsualMeshes = new List<Mesh>();
    public List<Mesh>         staticMeshesImmutableLODMeshes = new List<Mesh>();
    public List<Vector2Int>   staticMeshImmutableCoords = new List<Vector2Int>();
    public List<int>          staticMeshConstIndex = new List<int>();
    public List<MeshRenderer> staticMeshesSaveable = new List<MeshRenderer>();
    public List<Vector2Int>   staticMeshSaveableCoords = new List<Vector2Int>();
    public List<Light> lights = new List<Light>();
    public List<Light> lightsInPVS = new List<Light>();
    public List<Vector2Int> lightCoords = new List<Vector2Int>();
    public List<Vector2Int> lightsInPVSCoords = new List<Vector2Int>();
    public List<MeshRenderer> doors = new List<MeshRenderer>();
    public List<Vector2Int> doorsCoords = new List<Vector2Int>();
    public bool[,] worldCellLastVisible = new bool[WORLDX,WORLDX];
    public int playerCellX = 0;
    public int playerCellY = 0;
    public float deltaX = 0.0f;
    public float deltaY = 0.0f;
    public Vector3 worldMin;
    public List<Transform> npcTransforms;
    public List<AIController> npcAICs = new List<AIController>();
    public List<Vector2Int> npcCoords = new List<Vector2Int>();
    public Mesh[] lodMeshes;
    public Mesh lodMeshTemplate;

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
                                 "worldcellopen_" + LevelManager.a.currentLevel.ToString() + ".png");
        visDebugImagePath = Utils.SafePathCombine(
                                Application.streamingAssetsPath,
                                "worldcellvis_" + LevelManager.a.currentLevel.ToString() + ".png");
        a.pixels = new Color32[WORLDX * WORLDX];
    }

    float GetVertexColorForChunk(int constdex) {
        switch(constdex) {
            case 1:   return 0f;
            case 2:   return 1f / 255f;
            case 3:   return 1f / 255f;
            case 4:   return 1f / 255f;
            case 5:   return 2f / 255f;
            case 6:   return 2f / 255f;
            case 7:   return 2f / 255f;
            case 8:   return 3f / 255f;
            case 9:   return 3f / 255f;
            case 10:  return 3f / 255f;
            case 11:  return 4f / 255f;
            case 12:  return 6f / 255f;
            case 13:  return 7f / 255f;
            case 14:  return 8f / 255f;
            case 15:  return 9f / 255f;
            case 16:  return 10f / 255f;
            case 17:  return 11f / 255f;
            case 18:  return 12f / 255f;
            case 19:  return 13f / 255f;
            case 23:  return 14f / 255f;
            case 24:  return 15f / 255f;
            case 25:  return 16f / 255f;
            case 26:  return 17f / 255f;
            case 27:  return 18f / 255f;
            case 28:  return 19f / 255f;
            case 29:  return 20f / 255f;
            case 30:  return 21f / 255f;
            case 31:  return 21f / 255f;
            case 32:  return 21f / 255f;
            case 33:  return 22f / 255f;
            case 34:  return 23f / 255f;
            case 35:  return 24f / 255f;
            case 36:  return 25f / 255f;
            case 37:  return 26f / 255f;
            case 38:  return 26f / 255f;
            case 39:  return 26f / 255f;
            case 40:  return 26f / 255f;
            case 41:  return 26f / 255f;
            case 42:  return 26f / 255f;
            case 43:  return 26f / 255f;
            case 44:  return 26f / 255f;
            case 45:  return 26f / 255f;
            case 46:  return 26f / 255f;
            case 47:  return 26f / 255f;
            case 48:  return 26f / 255f;
            case 49:  return 26f / 255f;
            case 50:  return 26f / 255f;
            case 51:  return 26f / 255f;
            case 52:  return 26f / 255f;
            case 53:  return 26f / 255f;
            case 54:  return 26f / 255f;
            case 55:  return 26f / 255f;
            case 56:  return 26f / 255f;
            case 57:  return 26f / 255f;
            case 58:  return 26f / 255f;
            case 59:  return 26f / 255f;
            case 60:  return 26f / 255f;
            case 61:  return 26f / 255f;
            case 62:  return 26f / 255f;
            case 63:  return 26f / 255f;
            case 64:  return 26f / 255f;
            case 65:  return 26f / 255f;
            case 66:  return 26f / 255f;
            case 67:  return 26f / 255f;
            case 68:  return 26f / 255f;
            case 69:  return 26f / 255f;
            case 70:  return 26f / 255f;
            case 71:  return 26f / 255f;
            case 72:  return 26f / 255f;
            case 73:  return 26f / 255f;
            case 74:  return 26f / 255f;
            case 75:  return 26f / 255f;
            case 76:  return 26f / 255f;
            case 77:  return 26f / 255f;
            case 78:  return 26f / 255f;
            case 79:  return 26f / 255f;
            case 80:  return 26f / 255f;
            case 81:  return 26f / 255f;
            case 82:  return 26f / 255f;
            case 83:  return 26f / 255f;
            case 84:  return 26f / 255f;
            case 85:  return 26f / 255f;
            case 86:  return 26f / 255f;
            case 87:  return 26f / 255f;
            case 88:  return 26f / 255f;
            case 89:  return 26f / 255f;
            case 90:  return 26f / 255f;
            case 91:  return 26f / 255f;
            case 92:  return 26f / 255f;
            case 93:  return 26f / 255f;
            case 94:  return 26f / 255f;
            case 95:  return 26f / 255f;
            case 96:  return 26f / 255f;
            case 97:  return 26f / 255f;
            case 98:  return 26f / 255f;
            case 99:  return 26f / 255f;
            case 100: return 26f / 255f;
            case 101: return 26f / 255f;
            case 102: return 26f / 255f;
            case 103: return 26f / 255f;
            case 104: return 26f / 255f;
            case 105: return 26f / 255f;
            case 106: return 26f / 255f;
            case 107: return 26f / 255f;
            case 108: return 26f / 255f;
            case 109: return 26f / 255f;
            case 110: return 26f / 255f;
            case 111: return 26f / 255f;
            case 112: return 26f / 255f;
            case 113: return 26f / 255f;
            case 114: return 26f / 255f;
            case 115: return 26f / 255f;
            case 116: return 26f / 255f;
            case 117: return 26f / 255f;
            case 118: return 26f / 255f;
            case 119: return 26f / 255f;
            case 120: return 26f / 255f;
            case 121: return 26f / 255f;
            case 122: return 26f / 255f;
            case 123: return 26f / 255f;
            case 124: return 26f / 255f;
            case 125: return 26f / 255f;
            case 126: return 26f / 255f;
            case 127: return 26f / 255f;
            case 128: return 26f / 255f;
            case 129: return 26f / 255f;
            case 130: return 26f / 255f;
            case 131: return 26f / 255f;
            case 132: return 26f / 255f;
            case 133: return 26f / 255f;
            case 134: return 26f / 255f;
            case 135: return 26f / 255f;
            case 136: return 26f / 255f;
            case 137: return 26f / 255f;
            case 138: return 26f / 255f;
            case 139: return 26f / 255f;
            case 140: return 26f / 255f;
            case 141: return 26f / 255f;
            case 142: return 26f / 255f;
            case 143: return 26f / 255f;
            case 144: return 26f / 255f;
            case 145: return 26f / 255f;
            case 146: return 26f / 255f;
            case 147: return 26f / 255f;
            case 148: return 26f / 255f;
            case 149: return 118f / 255f;
            case 150: return 118f / 255f;
            case 151: return 118f / 255f;
            case 152: return 118f / 255f;
            case 153: return 118f / 255f;
            case 154: return 119f / 255f;
            case 155: return 120f / 255f;
            case 156: return 121f / 255f;
            case 157: return 122f / 255f;
            case 158: return 123f / 255f;
            case 159: return 124f / 255f;
            case 160: return 125f / 255f;
            case 161: return 126f / 255f;
            case 162: return 127f / 255f;
            case 163: return 128f / 255f;
            case 164: return 129f / 255f;
            case 165: return 130f / 255f;
            case 166: return 131f / 255f;
            case 167: return 132f / 255f;
            case 168: return 133f / 255f;
            case 169: return 134f / 255f;
            case 170: return 135f / 255f;
            case 171: return 136f / 255f;
            case 172: return 137f / 255f;
            case 173: return 138f / 255f;
            case 174: return 139f / 255f;
            case 175: return 140f / 255f;
            case 176: return 141f / 255f;
            case 177: return 142f / 255f;
            case 178: return 143f / 255f;
            case 179: return 144f / 255f;
            case 180: return 145f / 255f;
            case 181: return 146f / 255f;
            case 182: return 147f / 255f;
            case 183: return 148f / 255f;
            case 184: return 149f / 255f;
            case 185: return 150f / 255f;
            case 186: return 151f / 255f;
            case 187: return 152f / 255f;
            case 188: return 153f / 255f;
            case 189: return 154f / 255f;
            case 190: return 155f / 255f;
            case 191: return 156f / 255f;
            case 192: return 157f / 255f;
            case 193: return 158f / 255f;
            case 194: return 159f / 255f;
            case 195: return 160f / 255f;
            case 196: return 161f / 255f;
            case 197: return 162f / 255f;
            case 198: return 163f / 255f;
            case 199: return 164f / 255f;
            case 200: return 165f / 255f;
            case 201: return 166f / 255f;
            case 202: return 167f / 255f;
            case 203: return 168f / 255f;
            case 204: return 169f / 255f;
            case 205: return 170f / 255f;
            case 206: return 171f / 255f;
            case 207: return 172f / 255f;
            case 208: return 173f / 255f;
            case 209: return 174f / 255f;
            case 210: return 175f / 255f;
            case 211: return 176f / 255f;
            case 212: return 177f / 255f;
            case 213: return 178f / 255f;
            case 214: return 179f / 255f;
            case 215: return 180f / 255f;
            case 216: return 181f / 255f;
            case 217: return 182f / 255f;
            case 218: return 183f / 255f;
            case 219: return 184f / 255f;
            case 220: return 185f / 255f;
            case 221: return 186f / 255f;
            case 222: return 187f / 255f;
            case 223: return 188f / 255f;
            case 224: return 189f / 255f;
            case 225: return 190f / 255f;
            case 226: return 191f / 255f;
            case 227: return 192f / 255f;
            case 228: return 193f / 255f;
            case 229: return 194f / 255f;
            case 230: return 195f / 255f;
            case 231: return 196f / 255f;
            case 232: return 197f / 255f;
            case 233: return 198f / 255f;
            case 234: return 199f / 255f;
            case 235: return 200f / 255f;
            case 236: return 201f / 255f;
            case 237: return 202f / 255f;
            case 238: return 203f / 255f;
            case 239: return 204f / 255f;
            case 240: return 205f / 255f;
            case 241: return 206f / 255f;
            case 242: return 207f / 255f;
            case 243: return 208f / 255f;
            case 244: return 209f / 255f;
            case 245: return 210f / 255f;
            case 246: return 211f / 255f;
            case 247: return 212f / 255f;
            case 248: return 213f / 255f;
            case 249: return 214f / 255f;
            case 250: return 215f / 255f;
            case 251: return 216f / 255f;
            case 252: return 217f / 255f;
            case 253: return 218f / 255f;
            case 254: return 219f / 255f;
            case 255: return 220f / 255f;
            case 256: return 221f / 255f;
            case 257: return 222f / 255f;
            case 258: return 223f / 255f;
            case 259: return 224f / 255f;
            case 260: return 225f / 255f;
            case 261: return 226f / 255f;
            case 262: return 227f / 255f;
            case 263: return 228f / 255f;
            case 264: return 229f / 255f;
            case 265: return 230f / 255f;
            case 266: return 231f / 255f;
            case 267: return 232f / 255f;
            case 268: return 233f / 255f;
            case 269: return 234f / 255f;
            case 270: return 235f / 255f;
            case 271: return 236f / 255f;
            case 272: return 237f / 255f;
            case 273: return 238f / 255f;
            case 274: return 239f / 255f;
            case 275: return 240f / 255f;
            case 276: return 241f / 255f;
            case 277: return 242f / 255f;
            case 278: return 243f / 255f;
            case 279: return 244f / 255f;
            case 280: return 245f / 255f;
            case 281: return 246f / 255f;
            case 282: return 247f / 255f;
            case 283: return 248f / 255f;
            case 284: return 249f / 255f;
            case 285: return 250f / 255f;
            case 286: return 251f / 255f;
            case 287: return 252f / 255f;
            case 288: return 253f / 255f;
            case 289: return 254f / 255f;
            case 290: return 255f / 255f;
            case 291: return 255f / 255f;
            case 292: return 255f / 255f;
            case 293: return 255f / 255f;
            case 294: return 255f / 255f;
            case 295: return 255f / 255f;
            case 296: return 255f / 255f;
            case 297: return 255f / 255f;
            case 298: return 255f / 255f;
            case 299: return 255f / 255f;
            case 300: return 255f / 255f;
            case 301: return 255f / 255f;
            case 302: return 255f / 255f;
            case 303: return 255f / 255f;
            case 304: return 255f / 255f;
            
            // And so on
            default: return 0f;
        }
    }
    
    void ClearCellList() {
        worldCellVisible = new bool [WORLDX,WORLDX];
        worldCellCheckedYet = new bool [WORLDX,WORLDX];
        worldCellOpen = new bool [WORLDX,WORLDX];
        staticMeshesImmutable = new List<MeshRenderer>();
        staticMeshesImmutable.Clear();
        staticMeshesImmutableMFs = new List<MeshFilter>();
        staticMeshesImmutableMFs.Clear();
        staticMeshesImmutableUsualMeshes = new List<Mesh>();
        staticMeshesImmutableUsualMeshes.Clear();
        staticMeshesImmutableLODMeshes = new List<Mesh>();
        staticMeshesImmutableLODMeshes.Clear();
        staticMeshConstIndex = new List<int>();
        staticMeshConstIndex.Clear();
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
        lightsInPVS = new List<Light>();
        lightsInPVS.Clear();
        lightCoords = new List<Vector2Int>();
        lightCoords.Clear();
        lightsInPVSCoords = new List<Vector2Int>();
        lightsInPVSCoords.Clear();
        dynamicMeshes = new List<MeshRenderer>();
        dynamicMeshes.Clear();
        dynamicMeshCoords = new List<Vector2Int>();
        dynamicMeshCoords.Clear();
        npcAICs = new List<AIController>();
        npcAICs.Clear();
        npcTransforms = new List<Transform>();
        npcTransforms.Clear();
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

    void PutChunksInCells() {
        Transform container =
            LevelManager.a.GetCurrentGeometryContainer().transform;

        int chunkCount = container.childCount;
        bool[] alreadyInAtLeastOneList = new bool[chunkCount];
        for (int c=0;c<chunkCount;c++) alreadyInAtLeastOneList[c] = false;
        GameObject childGO = null;
        Vector3 pos = new Vector3(0f,0f,0f);
        Vector2 pos2d = new Vector2(0f,0f);
        Vector2 pos2dcurrent = new Vector2(0f,0f);
        for (int c=0;c<chunkCount;c++) {
            childGO = container.GetChild(c).gameObject;
            Vector2Int posint = PosToCellCoordsChunks(childGO.transform.position);
            cellLists[posint.x,posint.y].Add(childGO);
            worldCellOpen[posint.x,posint.y] = true;
//             Note posN = childGO.AddComponent<Note>();
//             posN.note = "Cell coords: " + posint.x.ToString() + ", " + posint.y.ToString();
            MeshRenderer mr = childGO.GetComponent<MeshRenderer>();
            if (mr != null) cellListsMR[posint.x,posint.y].Add(mr);

            Component[] compArray = childGO.GetComponentsInChildren(
                typeof(MeshRenderer),true);

            foreach (MeshRenderer mrc in compArray) {
                if (mrc != null) cellListsMR[posint.x,posint.y].Add(mrc);
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
                if (mr.transform.parent != null) {
                    if (mr.transform.parent.gameObject.name == "flightpods_exterior") break;
                    if (mr.transform.parent.parent != null) {
                        if (mr.transform.parent.parent.gameObject.name == "flightpods_exterior") break;
                    }
                }

                staticMeshesImmutable.Add(mr);
                MeshFilter mf = mr.gameObject.GetComponent<MeshFilter>();
                if (mf != null) Debug.Log("no mesh filter on cullable object " + mr.gameObject.name);
                staticMeshesImmutableMFs.Add(mf);
                staticMeshesImmutableUsualMeshes.Add(mr.GetComponent<MeshFilter>().mesh);
                PrefabIdentifier pid = mr.GetComponent<PrefabIdentifier>();
                if (pid == null) pid = mr.transform.parent.GetComponent<PrefabIdentifier>();
                if (pid == null) pid = mr.transform.parent.parent.GetComponent<PrefabIdentifier>();
                if (pid == null) Debug.Log("no prefab identifier found for " + mr.gameObject.name);
                staticMeshConstIndex.Add(pid.constIndex);
                if (pid.constIndex > 304) {
                    // Always add something so it's same length as all other
                    // static mesh immutable lists.
                    staticMeshesImmutableLODMeshes.Add(mr.GetComponent<MeshFilter>().mesh);
                    staticMeshImmutableCoords.Add(Vector2Int.zero);
                } else {
                    staticMeshesImmutableLODMeshes.Add(lodMeshes[pid.constIndex]);
                    staticMeshImmutableCoords.Add(Vector2Int.zero);
                }
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

        Transform ctr = container.transform;
        Component[] compArray = container.GetComponentsInChildren(typeof(MeshRenderer),true);
        int count = ctr.childCount;
        if (type == 3) {        // NPC
            AIController aic = null;
            for (int i=0;i<count;i++) {
                aic = ctr.GetChild(i).GetComponent<AIController>();
                if (aic == null) continue;

                npcAICs.Add(aic);
                npcTransforms.Add(ctr.GetChild(i));
                npcCoords.Add(Vector2Int.zero);
            }
        } else if (type == 5) { // Lights
            Light lit = null;
            for (int i=0;i<count;i++) {
                lit = ctr.GetChild(i).GetComponent<Light>();
                if (lit == null) continue;

                lights.Add(lit);
                lightCoords.Add(Vector2Int.zero);
            }
        } else {
            Transform parent = null;
            Transform child = null;
            for (int i=0;i<count;i++) {
                parent = ctr.GetChild(i);
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
                case 1: dynamicMeshCoords[index]          = PosToCellCoords(dynamicMeshes[index].transform.position); break;
                case 2: doorsCoords[index]                = PosToCellCoords(doors[index].transform.position); break;
                case 3: npcCoords[index]                  = PosToCellCoords(npcTransforms[index].position); break;
                case 4: staticMeshSaveableCoords[index]   = PosToCellCoords(staticMeshesSaveable[index].transform.position); break;
                case 5: lightCoords[index]                = PosToCellCoords(lights[index].transform.position); break;
                default: staticMeshImmutableCoords[index] = PosToCellCoords(staticMeshesImmutable[index].transform.position); break;
            }
        }
    }

    public void Cull_Init() {
        // Populate LOD (Level of Detail) Meshes for farther chunks.
        // This creates a prepopulated list of quads (2 tris each) with vertex
        // color Red channel set to the index into the Texture2DArray for the
        // chunk shader.  These get swapped out in place of the
        // staticMeshes_ arrays above based on player's distance.  This list is
        // just the representation for each chunk prefab, excepting slices.
        a.lodMeshes = new Mesh[305]; // Constindexes 0 through 304.
        Color[] vertColors = new Color[lodMeshTemplate.vertexCount];
        for (int i=0;i<vertColors.Length;i++) {
            vertColors[i] = new Color(0f,0f,0f,1f);
        }
        
        float red = 0f;
        for (int i=0;i<305;i++) {
            a.lodMeshes[i] = new Mesh();
            a.lodMeshes[i].name = "lodMesh" + i.ToString();
            a.lodMeshes[i].vertices = lodMeshTemplate.vertices;
            a.lodMeshes[i].triangles = lodMeshTemplate.triangles;
            a.lodMeshes[i].normals = lodMeshTemplate.normals;
            a.lodMeshes[i].uv = lodMeshTemplate.uv;
            red = GetVertexColorForChunk(i);
            for (int j=0;j<vertColors.Length;j++) {
                vertColors[j].r = red; // All matching.
            }
            a.lodMeshes[i].colors = vertColors;
        }
        
        // Setup and find all cullables and associate them with x,y coords.
        ClearCellList();
        switch(LevelManager.a.currentLevel) { // PosToCellCoords -1 on just x
            // chunk.x + (Geometry.x + Level.x),0,chunk.z + (Geometry.z + Level.z)
            case 0: worldMin =  new Vector3( -38.40f + ( 0.0000f +   3.6000f),0f,-51.20f + (0f + 1f)); break;
            case 1: worldMin =  new Vector3( -76.80f + ( 0.0000f +  25.5600f),0f,-56.32f + (0f + -5.2f)); break;
            case 2: worldMin =  new Vector3( -40.96f + ( 0.0000f +  -2.6000f),0f,-46.08f + (0f + -7.7f)); break;
            case 3: worldMin =  new Vector3( -53.76f + (50.1740f + -45.1200f),0f,-46.08f + (13.714f + -16.32f)); break;
            case 4: worldMin =  new Vector3(  -7.68f + ( 1.1780f + -20.4000f),0f,-64.00f + (1.292799f + 11.48f)); break;
            case 5: worldMin =  new Vector3( -35.84f + ( 1.1778f + -10.1400f),0f,-51.20f + (-1.2417f + -0.0383f)); break;
            case 6: worldMin =  new Vector3( -64.00f + ( 1.2928f +  -0.6728f),0f,-71.68f + (-1.2033f + 3.76f)); break;
            case 7: worldMin =  new Vector3( -58.88f + ( 1.2411f +  -6.7000f),0f,-79.36f + (-1.2544f + 1.16f)); break;
            case 8: worldMin =  new Vector3( -40.96f + (-1.3056f + 1.08f),0f,-43.52f + (1.2928f + 0.8f)); break;
            case 9: worldMin =  new Vector3( -51.20f + (-1.3439f + 3.6f),0f,-64f + (-1.1906f + -1.28f)); break;
            case 10: worldMin = new Vector3(-128.00f + (-0.90945f + 107.37f) - 2.56f,0f,-71.68f + (-1.0372f + 35.48f)); break;
            case 11: worldMin = new Vector3( -38.40f + (-1.2672f + 15.05f) - 2.56f,0f,51.2f + (0.96056f + -77.94f)); break;
            case 12: worldMin = new Vector3( -34.53f + (0f + 19.04f),0f,-123.74f + (0f + 95.8f)); break;
        }

        worldMin.x -= 2.56f; // Add one cell gap around edges
        worldMin.z -= 2.56f;

        if (outputDebugImages) {
            debugTex = new Texture2D(64,64);
            pixels = new Color32[WORLDX * WORLDX];
            openDebugImagePath = Utils.SafePathCombine(
                Application.streamingAssetsPath,
                "worldcellopen_" + LevelManager.a.currentLevel.ToString()
                + ".png");

            visDebugImagePath = Utils.SafePathCombine(
                Application.streamingAssetsPath,
                "worldcellvis_" + LevelManager.a.currentLevel.ToString()
                + ".png");
        }

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
        int x,y;
        //Color col = new Color(0f,0f,0f,1f);
        //for (int i=0;i<128;i++) {
        //    precalculatedPVS[i] = new Texture2D(WORLDX,WORLDX);
        //    for (x=0;x<WORLDX;x++) {
        //        for (y=0;y<WORLDX;y++) {
        //            playerCellX = x;
        //            playerCellY = y;
        //            for (int x2=0;x2<64;x2++) {
        //                for (int y2=0;y2<64;y2++) {
        //                    worldCellVisible[x2,y2] = false;
        //                    worldCellCheckedYet[x2,y2] = false;
        //                }
        //            }

        //            DetermineVisibleCells();
        //            if (worldCellVisible[x,y]) col.r = 1.0f;
        //            else col.r = 0.0f;

        //            precalculatedPVS[i].SetPixel(x,y,col,0);
        //        }
        //    }
        //}

        if (!cullEnabled) return;

        // Force all cells dirty at start so the visibility is toggled for all.
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

    Vector2Int PosToCellCoordsChunks(Vector3 pos) {
        int x,y;
        x = (int)((pos.x - worldMin.x + 1.28f) / 2.56f);
        if (x > 63) x = 63;
        else if (x < 0) x = 0;
        y = (int)((pos.z - worldMin.z + 1.28f) / 2.56f);
        if (y > 63) y = 63;
        else if (y < 0) y = 0;

        return new Vector2Int(x,y);
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

    void FindPlayerCell() {
        Vector2Int pxy = PosToCellCoords(PlayerMovement.a.transform.position);
        playerCellX = pxy.x;
        playerCellY = pxy.y;
    }

    bool UpdatedPlayerCell() {
        int lastX = playerCellX;
        int lastY = playerCellY;
        FindPlayerCell();
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
        SetVisPixel(x,y,Color.cyan);
    }

    //void DetermineVisibleCellsPrecalculated() {
    //    int playerCell4096 = playerCellX + (playerCellY * 64);
    //    for (int x=0;x<64;x++) {
    //        for (int y=0;y<64;y++) {
    //            worldCellVisible[x,y] = 
    //                (precalculatedPVS[playerCell4096].GetPixel(x,y,0).r > 0.99f);
    //        }
    //    }
    //}

    void DetermineVisibleCells() {
        if (outputDebugImages) debugTex = new Texture2D(64,64);
        int x,y;
        for (x=0;x<64;x++) {
            for (y=0;y<64;y++) {
                worldCellVisible[x,y] = false;
                if (outputDebugImages) {
                    pixels[x + (y * 64)] = worldCellOpen[x,y]
                                           ? Color.white : Color.black;
                }
            }
        }

        x = playerCellX; y = playerCellY;
        worldCellVisible[x,y] = true;
        worldCellCheckedYet[x,y] = true;
        SetVisPixel(x,y,Color.blue);

        // Set all neighboring cells visible if open in 3x3 square.
        SetVisible(x - 1,y + 1); SetVisible(x,y + 1); SetVisible(x + 1,y + 1);

        SetVisible(x - 1,y);     /* Player Position*/   SetVisible(x + 1,y);

        SetVisible(x - 1,y - 1); SetVisible(x,y - 1); SetVisible(x + 1,y - 1);

        CastStraightX(playerCellX,playerCellY,1);  // [ ][3]
                                                   // [1][2]
                                                   // [ ][3]
        if (XYPairInBounds(playerCellX,playerCellY + 1)) {
            CastStraightX(playerCellX,playerCellY + 1,1);
        }

        if (XYPairInBounds(playerCellX,playerCellY - 1)) {
            CastStraightX(playerCellX,playerCellY - 1,1);
        }

        CastStraightX(playerCellX,playerCellY,-1); // [3][ ]
                                                   // [2][1]
                                                   // [3][ ]
        if (XYPairInBounds(playerCellX,playerCellY + 1)) {
            CastStraightX(playerCellX,playerCellY + 1,-1);
        }

        if (XYPairInBounds(playerCellX,playerCellY - 1)) {
            CastStraightX(playerCellX,playerCellY - 1,-1);
        }

        CastStraightY(playerCellX,playerCellY,1);  // [3][2][3]
                                                   // [ ][1][ ]
        if (XYPairInBounds(playerCellX + 1,playerCellY)) {
            CastStraightX(playerCellX + 1,playerCellY,1);
        }

        if (XYPairInBounds(playerCellX - 1,playerCellY)) {
            CastStraightX(playerCellX - 1,playerCellY,1);
        }

        CastStraightY(playerCellX,playerCellY,-1); // [ ][1][ ]
                                                   // [3][2][3]
        if (XYPairInBounds(playerCellX + 1,playerCellY)) {
            CastStraightX(playerCellX + 1,playerCellY,-1);
        }

        if (XYPairInBounds(playerCellX - 1,playerCellY)) {
            CastStraightX(playerCellX - 1,playerCellY,-1);
        }

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
        if (outputDebugImages) {
            debugTex.SetPixels32(pixels);
            debugTex.Apply();
            bytes = debugTex.EncodeToPNG();
            File.WriteAllBytes(visDebugImagePath,bytes);
        }
    }

    void SetVisPixel(int x, int y, Color col){
        if (!outputDebugImages) return;

        pixels[x + (y * 64)] = worldCellVisible[x,y]
                               ? col
                               : (worldCellOpen[x,y] ? Color.white
                                                     : Color.black);
    }

    private void CastStraightY(int px, int py, int signy) {
        if (signy > 0 && py >= 63) return;
        if (signy < 0 && py <= 0) return;

        int x = px;
        int y = py + signy;
        bool currentVisible = true;
        for (;y<64;y+=signy) { // Up
            currentVisible = false;
            if (XYPairInBounds(x,y - signy)
                && XYPairInBounds(x,y)) {

                if (!worldCellCheckedYet[x,y]) {
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

            if (!currentVisible) break; // Hit wall!

            if (XYPairInBounds(x + 1,y)) {
                if (!worldCellCheckedYet[x + 1,y]) {
                    worldCellVisible[x + 1,y] = worldCellOpen[x + 1,y];
                    worldCellCheckedYet[x + 1,y] = true;
                    SetVisPixel(x + 1,y,Color.green);
                }
            }

            if (XYPairInBounds(x - 1,y)) {
                if (!worldCellCheckedYet[x - 1,y]) {
                    worldCellVisible[x - 1,y] = worldCellOpen[x - 1,y];
                    worldCellCheckedYet[x - 1,y] = true;
                    SetVisPixel(x - 1,y,Color.green);
                }
            }
        }
    }

    private void CastStraightX(int px, int py, int signx) {
        if (signx > 0 && px >= 63) return;
        if (signx < 0 && px <= 0) return;

        int x = px + signx;
        int y = py;
        bool currentVisible = true;
        for (;x<64;x+=signx) { // Right
            currentVisible = false;
            if (XYPairInBounds(x - signx,y)
                && XYPairInBounds(x,y)) {

                if (!worldCellCheckedYet[x,y]) {
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

            if (!currentVisible) break; // Hit wall!

            if (XYPairInBounds(x,y + 1)) {
                if (!worldCellCheckedYet[x,y + 1]) {
                    worldCellVisible[x,y + 1] = worldCellOpen[x,y + 1];
                    worldCellCheckedYet[x,y + 1] = true;
                    SetVisPixel(x,y + 1,Color.green);
                }
            }

            if (XYPairInBounds(x,y - 1)) {
                if (!worldCellCheckedYet[x,y - 1]) {
                    worldCellVisible[x,y - 1] = worldCellOpen[x,y - 1];
                    worldCellCheckedYet[x,y - 1] = true;
                    SetVisPixel(x,y - 1,Color.green);
                }
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
        while (iter >= 0) {
            if (CastRayCellCheck(x,y) == -1) return;

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
                    if (outputDebugImages) {
                        pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                    }

                    return -1;
                }

                SetVisPixel(x,y,new Color(0f,0f,0.5f,1f));
                return 1;
            }
        }

        return 0;
    }

    void ToggleVisibility() {
        worldCellLastVisible[playerCellX,playerCellY] = false;
        worldCellVisible[playerCellX,playerCellY] = true; // Guarantee enable.
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
            if (worldCellVisible[npcCoords[i].x,npcCoords[i].y]
                || !worldCellOpen[npcCoords[i].x,npcCoords[i].y]
                || npcAICs[i].enemy != null || npcAICs[i].ai_dying
                || npcAICs[i].ai_dead) {
                npcAICs[i].withinPVS = true;
                hm = npcAICs[i].healthManager;
                if (npcAICs[i].visibleMeshVisible) {
                    Utils.Activate(npcAICs[i].visibleMeshEntity);
                } else {
                    Utils.Deactivate(npcAICs[i].visibleMeshEntity);
                }
//                 if (npcAICs[i].currentState == AIState.Dead
//                     || (!npcAICs[i].HasHealth(hm) && hm.gibOnDeath)) {
//                     if (npcAICs[i].DeactivatesVisibleMeshWhileDying()
//                         || (hm.gibOnDeath && npcAICs[i].ai_dead)) {
//                         Utils.Deactivate(npcAICs[i].visibleMeshEntity);
//                     } else {
//                         Utils.Activate(npcAICs[i].visibleMeshEntity);
//                     }
//                 } else if (npcAICs[i].currentState == AIState.Dying) {
//                     if (npcAICs[i].DeactivatesVisibleMeshWhileDying()) {
//                             Utils.Deactivate(npcAICs[i].visibleMeshEntity);
//                         } else {
//                             Utils.Activate(npcAICs[i].visibleMeshEntity);
//                         }
//                 } else {
//                     Utils.Activate(npcAICs[i].visibleMeshEntity);
//                 }
            } else {
                npcAICs[i].withinPVS = false;
                Utils.Deactivate(npcAICs[i].visibleMeshEntity);
            }
        }
    }

    public void ToggleDynamicMeshesVisibility() {
        for (int i=0;i<dynamicMeshes.Count;i++) {
            if (worldCellVisible[dynamicMeshCoords[i].x,dynamicMeshCoords[i].y]
                || !worldCellOpen[dynamicMeshCoords[i].x,dynamicMeshCoords[i].y]) {
                dynamicMeshes[i].enabled = true;
            } else {
                dynamicMeshes[i].enabled = false;
            }
        }
    }

    public float SqrDist(int x0, int y0, int x1, int y1) {
        float deltaX = x1 - x0;
        float deltaY = y1 - y0;
        return (deltaX * deltaX) + (deltaY * deltaY);
    }

    public void ToggleStaticMeshesImmutableVisibility() {
        for (int i=0;i<staticMeshesImmutable.Count;i++) {
            int x = staticMeshImmutableCoords[i].x;
            int y = staticMeshImmutableCoords[i].y;
            if (worldCellVisible[x,y] || !worldCellOpen[x,y]) {
                staticMeshesImmutable[i].enabled = true;

                if (staticMeshConstIndex[i] > 304) return; // No LOD for non-chunks
                
                Mesh targetMesh = SqrDist(x,y,playerCellX,playerCellY) > 36f
                    ? staticMeshesImmutableLODMeshes[i]
                    : staticMeshesImmutableUsualMeshes[i];

                if (staticMeshesImmutableMFs[i].sharedMesh != targetMesh) {
                    staticMeshesImmutableMFs[i].sharedMesh = targetMesh;
                }
            } else {
                staticMeshesImmutable[i].enabled = false;
            }
        }
    }

    public void ToggleStaticMeshesSaveableVisibility() {
        for (int i=0;i<staticMeshesSaveable.Count;i++) {
            if (worldCellVisible[staticMeshSaveableCoords[i].x,staticMeshSaveableCoords[i].y]
                || !worldCellOpen[staticMeshSaveableCoords[i].x,staticMeshSaveableCoords[i].y]) {
                HealthManager hm = staticMeshesSaveable[i].GetComponent<HealthManager>();
                if (hm != null) {
                    if (hm.health > 0 || !hm.gibOnDeath || hm.isScreen) {
                        staticMeshesSaveable[i].enabled = true;
                    } else {
                        staticMeshesSaveable[i].enabled = false;
                    }
                } else {
                    staticMeshesSaveable[i].enabled = true;
                }
            } else {
                staticMeshesSaveable[i].enabled = false;
            }
        }
    }

    public void ToggleDoorsVisibility() {
        for (int i=0;i<doors.Count;i++) {
            if (worldCellVisible[doorsCoords[i].x,doorsCoords[i].y]
                || !worldCellOpen[doorsCoords[i].x,doorsCoords[i].y]) {
                doors[i].enabled = true;
            } else {
                doors[i].enabled = false;
            }
        }
    }

    public void ToggleLightsVisibility() {
        lightsInPVS.Clear();
        for (int i=0;i<lights.Count;i++) {
            if (worldCellVisible[lightCoords[i].x,lightCoords[i].y]
                || !worldCellOpen[lightCoords[i].x,lightCoords[i].y]) {
                lights[i].enabled = true;
                lightsInPVS.Add(lights[i]);
                lightsInPVSCoords.Add(lightCoords[i]);
            } else {
                lights[i].enabled = false;
            }
        }
    }

    public void ToggleLightsFrustumVisibility() {
        Vector3 lightPos;
        Vector3 lightDir;
        for (int i=0;i<lightsInPVS.Count;i++) {
            lightPos = lightsInPVS[i].transform.position;
            lightDir = PlayerMovement.a.transform.position - lightPos;
            if (Vector3.Dot(lightDir,MouseLookScript.a.transform.forward) < 0.5f) {
                lightsInPVS[i].enabled = true;
                Debug.Log("Light at "
                + lightsInPVS[i].transform.position.ToString()
                + " VISIBLE, check was: "
                + Vector3.Dot(lightDir,MouseLookScript.a.transform.forward).ToString());
            } else {
                lightsInPVS[i].enabled = false;
                Debug.Log("Light at "
                + lightsInPVS[i].transform.position.ToString()
                + " NOT VISIBLE, check was: "
                + Vector3.Dot(lightDir,MouseLookScript.a.transform.forward).ToString());
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
            //DetermineVisibleCellsPrecalculated(); // Reevaluate visible cells from new pos.
            worldCellVisible[0,0] = true; // Errors default here so draw them anyways.
            ToggleVisibility(); // Update all cells marked as dirty.
            ToggleStaticMeshesImmutableVisibility();
            ToggleStaticMeshesSaveableVisibility();
            ToggleDoorsVisibility();
            ToggleLightsVisibility();
            UpdateNPCPVS();
            ToggleNPCPVS();
        }

        if (MouseLookScript.a.lightCulling) ToggleLightsFrustumVisibility();

        // Update dynamic meshes after PVS has been updated, if player moved.
        //UpdateDynamicMeshes(); // Always check all of them because any can move.
        //ToggleDynamicMeshesVisibility(); // Now turn them on or off.
    }
}

