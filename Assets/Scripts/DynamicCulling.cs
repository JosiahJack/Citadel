using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DynamicCulling : MonoBehaviour {
    [HideInInspector] public const int WORLDX = 64;
    [HideInInspector] public const int ARRSIZE = WORLDX * WORLDX;
    [HideInInspector] public const float CELLXHALF = 1.28f;
    
    public bool cullEnabled = false;
    public bool dynamicObjectCull = false;
    public bool lightCulling = true;
    public bool outputDebugImages = false;
    public bool forceRecull = false;
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    public Texture2D[] worldClosedEdges;
    public bool[,] worldCellCheckedYet = new bool [WORLDX,WORLDX];
    //public Texture2D[] precalculatedPVS = new Texture2D[ARRSIZE];
    public GridCell[,] gridCells = new GridCell[WORLDX,WORLDX];
    public List<MeshRenderer> dynamicMeshes = new List<MeshRenderer>();
    public List<Vector2Int> dynamicMeshCoords = new List<Vector2Int>();
    public List<MeshRenderer> staticMeshesImmutable = new List<MeshRenderer>();
    public List<Vector2Int>   staticMeshImmutableCoords = new List<Vector2Int>();
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
    public float lodSqrDist = 36f;
    public Vector3 worldMin;
    public List<Transform> npcTransforms;
    public List<AIController> npcAICs = new List<AIController>();
    public List<Vector2Int> npcCoords = new List<Vector2Int>();
    public bool lodMeshesInitialized = false;
    public Mesh[] lodMeshes;
    public Mesh lodMeshTemplate;

    private byte[] bytes;
    private static string openDebugImagePath;
    private static string visDebugImagePath;
    private Color32[] pixels;
    private Texture2D debugTex;
    private Dictionary<GameObject, Vector3> camPositions = new Dictionary<GameObject, Vector3>();
    private bool[,] worldCellsOpen = new bool[64,64];
    
    [HideInInspector] public ComputeBuffer triangleBuffer;
    [HideInInspector] public ComputeBuffer modelBuffer;
    
    public static DynamicCulling a;
    
    public class Meshenderer {
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public Mesh meshUsual;
        public Mesh meshLOD;
        
        public void SetMesh(bool useLOD) {
            meshFilter.sharedMesh = useLOD ? meshLOD : meshUsual;
        }
    }
    
    public class ChunkPrefab {
        public int x;
        public int y;
        public int constIndex;
        public GameObject go;
        public List<Meshenderer> meshenderers;
    }
    
    public class DynamicObject {
        public int x;
        public int y;
        public int constIndex;
        public GameObject go;
        public List<Meshenderer> meshenderers;
    }
    
    public class GridCell {
        public int x;
        public int y;
        public bool open;
        public bool visible;
        public bool closedNorth; // For when chunk configurations are such that
        public bool closedEast;  // the immediately adjacent cell at this edge
        public bool closedSouth; // is not visible, consider edge as closed to
        public bool closedWest;  // be able to further reduce visible cells.
        public bool[,] visibleCellsFromHere;
        public List<ChunkPrefab> chunkPrefabs;
        public List<DynamicObject> dynamicObjects;
    }

    void Awake() {
        a = this;
        a.Cull_Init();
        openDebugImagePath = Utils.SafePathCombine(
                                 Application.streamingAssetsPath,
                                 "gridcellsopen_"
                                 + LevelManager.a.currentLevel.ToString()
                                 + ".png");
        
        visDebugImagePath = Utils.SafePathCombine(
                                Application.streamingAssetsPath,
                                "worldcellvis_"
                                + LevelManager.a.currentLevel.ToString()
                                + ".png");
        
        a.pixels = new Color32[WORLDX * WORLDX];
        a.camPositions = new Dictionary<GameObject, Vector3>();
        a.renderTexture = new RenderTexture(256,256,24);
        a.renderTexture.enableRandomWrite = true;
        a.renderTexture.Create();
        computeShader.SetTexture(0,"Result",renderTexture);
        computeShader.Dispatch(0,renderTexture.width / 8, renderTexture.height / 8, 1);
        a.worldCellsOpen = new bool[64,64];
    }

    float GetVertexColorForChunk(int constdex) {
        switch(constdex) {



            
            case 4:   return   1f / 255f;
            case 5:   return   2f / 255f;
            case 6:   return 255f; // Slice, but would be 2f / 255f;.
            case 7:   return   2f / 255f;
            case 8:   return   3f / 255f;
            case 9:   return 255f; // Slice, but would be 3f / 255f;
            case 10:  return 255f; // Slice, but would be 3f / 255f;
            case 11:  return   4f / 255f;
            case 12:  return   6f / 255f;
            case 13:  return   7f / 255f;
            case 14:  return   8f / 255f;
            case 15:  return   9f / 255f;
            case 16:  return  10f / 255f;
            case 17:  return  11f / 255f;
            case 18:  return  12f / 255f;
            case 19:  return  13f / 255f;



            case 23:  return  14f / 255f;
            case 24:  return  15f / 255f;
            case 25:  return  16f / 255f;
            case 26:  return  17f / 255f;
            case 27:  return  18f / 255f;
            case 28:  return  19f / 255f;
            case 29:  return  20f / 255f;
            case 30:  return  21f / 255f;
            case 31:  return 255f; // Slice, but would be 21f / 255f;
            case 32:  return 255f; // Slice, but would be 21f / 255f;
            case 33:  return  22f / 255f;
            case 34:  return  23f / 255f;
            case 35:  return  24f / 255f;
            case 36:  return  25f / 255f;
            case 37:  return  26f / 255f;
            case 38:  return  27f / 255f;
            case 39:  return  28f / 255f;
            case 40:  return  29f / 255f;
            case 41:  return  30f / 255f;
            case 42:  return 255f; // Slice, but would be 30f / 255f;
            case 43:  return 255f; // Slice, but would be 30f / 255f;
            case 44:  return 255f; // Slice, but would be 30f / 255f;
            case 45:  return  31f / 255f;
            case 46:  return  32f / 255f;
            case 47:  return  33f / 255f;
            case 48:  return  34f / 255f;
            case 49:  return  35f / 255f;
            case 50:  return  36f / 255f;
            case 51:  return  37f / 255f;
            case 52:  return 255f; // Slice, but would be 37f / 255f;

            case 54:  return  38f / 255f;
            case 55:  return  39f / 255f;
            case 56:  return  40f / 255f;
            case 57:  return  41f / 255f;
            case 58:  return  42f / 255f;
            case 59:  return  43f / 255f;
            case 60:  return  44f / 255f;
            case 61:  return  45f / 255f;
            case 62:  return  46f / 255f;
            case 63:  return 255f; // Slice, but would be 46f / 255f;
            case 64:  return  47f / 255f;
            case 65:  return  48f / 255f;
            case 66:  return  49f / 255f;
            case 67:  return  50f / 255f;
            case 68:  return  51f / 255f;
            case 69:  return  52f / 255f;
            case 70:  return  53f / 255f;
            case 71:  return  54f / 255f;
            case 72:  return  55f / 255f;
            case 73:  return  56f / 255f;
            case 74:  return  57f / 255f;
            case 75:  return  58f / 255f;
            case 76:  return  59f / 255f;
            case 77:  return  60f / 255f;


            case 80:  return  61f / 255f;
            case 81:  return  62f / 255f;
            case 82:  return  63f / 255f;
            case 83:  return 255f; // Slice, but would be 63f / 255f;
            case 84:  return  64f / 255f;
            case 85:  return  65f / 255f;
            case 86:  return  66f / 255f;
            case 87:  return 255f; // Slice, but would be 66f / 255f;
            case 88:  return  67f / 255f;
            case 89:  return  68f / 255f;
            case 90:  return  69f / 255f;
            case 91:  return 255f; // Slice, but would be 69f / 255f;
            case 92:  return  70f / 255f;
// Need to prevent swapping out all the grass and leaves with cards!
//             case 94:  return  71f / 255f;
//             case 95:  return 255f; // Slice, but would be 71f / 255f;
//             case 96:  return  72f / 255f;
//             case 97:  return  73f / 255f;
//             case 98:  return  74f / 255f;
//             case 99:  return  75f / 255f;
//             case 100: return  76f / 255f;
//             case 101: return  77f / 255f;
//             case 102: return  78f / 255f;
//             case 103: return  79f / 255f;
//             case 104: return  80f / 255f;
//             case 105: return  81f / 255f;
//             case 106: return  82f / 255f;
//             case 107: return  83f / 255f;
//             case 108: return  84f / 255f;
//             case 109: return  85f / 255f;
//             case 110: return  86f / 255f;
//             case 111: return  87f / 255f;

            case 113: return  88f / 255f;
            case 114: return  89f / 255f;
            case 115: return  90f / 255f;
            case 116: return  91f / 255f;
            case 117: return  92f / 255f;
            case 118: return  93f / 255f;
            case 119: return  94f / 255f;
            case 120: return  95f / 255f;
            case 121: return  96f / 255f;
            case 122: return  97f / 255f;

            case 124: return  98f / 255f;
            case 125: return  99f / 255f;
            case 126: return 100f / 255f;
            case 127: return 101f / 255f;
            case 128: return 102f / 255f;
            case 129: return 103f / 255f;
            case 130: return 104f / 255f;
            case 131: return 105f / 255f;
            case 132: return 106f / 255f;
            case 133: return 107f / 255f;
            case 134: return 108f / 255f;
            case 135: return 109f / 255f;
            case 136: return 110f / 255f;
            case 137: return 111f / 255f;
            case 138: return 112f / 255f;
            case 139: return 113f / 255f;
            case 140: return 114f / 255f;
            case 141: return 115f / 255f;
            case 142: return 255f; // Slice, but would be 115f / 255f;
            case 143: return 255f; // Slice, but would be 115f / 255f;
            case 144: return 116f / 255f;
            case 145: return 255f; // Slice, but would be 116f / 255f;
            case 146: return 255f; // Slice, but would be 116f / 255f;
            case 147: return 255f; // Slice, but would be 116f / 255f;
            case 148: return 117f / 255f;
            case 149: return 118f / 255f;
            case 150: return 255f; // Slice, but would be 118f / 255f;
            case 151: return 255f; // Slice, but would be 118f / 255f;
            case 152: return 255f; // Slice, but would be 118f / 255f;
            case 153: return 255f; // Slice, but would be 118f / 255f;
            case 154: return 119f / 255f;
            case 155: return 120f / 255f;
            case 156: return 121f / 255f;
            case 157: return 122f / 255f;
            case 158: return 123f / 255f;
            case 159: return 124f / 255f;
            case 160: return 125f / 255f;
            case 161: return 126f / 255f;
            case 162: return 127f / 255f;
            case 163: return 255f; // Slice, but would be 127f / 255f;
            case 164: return 255f; // Slice, but would be 127f / 255f;
            case 165: return 255f; // Slice, but would be 127f / 255f;
            case 166: return 255f; // Slice, but would be 127f / 255f;
            case 167: return 128f / 255f;
            case 168: return 255f; // Slice, but would be 128f / 255f;
            case 169: return 129f / 255f;
            case 170: return 130f / 255f;
            case 171: return 131f / 255f;
            case 172: return 255f; // Slice, but would be 131f / 255f;
            case 173: return 255f; // Slice, but would be 131f / 255f;
            case 174: return 132f / 255f;
            case 175: return 255f; // Slice, but would be 132f / 255f;
            case 176: return 255f; // Slice, but would be 132f / 255f;
            case 177: return 255f; // Slice, but would be 132f / 255f;
            case 178: return 133f / 255f;
            case 179: return 255f; // Slice, but would be 133f / 255f;
            case 180: return 134f / 255f;
            case 181: return 135f / 255f;
            case 182: return 255f; // Slice, but would be 135f / 255f;
            case 183: return 136f / 255f;
            case 184: return 137f / 255f;
            case 185: return 138f / 255f;
            case 186: return 139f / 255f;
            case 187: return 140f / 255f;
            case 188: return 141f / 255f;

            case 190: return 143f / 255f;
            case 191: return 255f; // Slice, but would be 143f / 255f;
            case 192: return 255f; // Slice, but would be 143f / 255f;
            case 193: return 255f; // Slice, but would be 143f / 255f;
            case 194: return 144f / 255f;
            case 195: return 145f / 255f;
            case 196: return 146f / 255f;
            case 197: return 147f / 255f;
            case 198: return 148f / 255f;
            case 199: return 149f / 255f;
            case 200: return 255f; // Slice, but would be 149f / 255f;
            case 201: return 150f / 255f;
            case 202: return 151f / 255f;
            case 203: return 152f / 255f;
            case 204: return 153f / 255f;
            case 205: return 154f / 255f;
            case 206: return 155f / 255f;
            case 207: return 156f / 255f;
            case 208: return 157f / 255f;
            case 209: return 158f / 255f;
            case 210: return 255f; // Slice, but would be 158f / 255f;
            case 211: return 255f; // Slice, but would be 158f / 255f;
            case 212: return 255f; // Slice, but would be 158f / 255f;
            case 213: return 255f; // Slice, but would be 158f / 255f;
            case 214: return 159f / 255f;
            case 215: return 255f; // Mirror, but would be 159f / 255f;
            case 216: return 255f; // Mirror, but would be 158f / 255f;
            case 217: return 160f / 255f;
            case 218: return 161f / 255f;
            case 219: return 255f; // Slice, but would be 161f / 255f;
            case 220: return 162f / 255f;
            case 221: return 163f / 255f;
            case 222: return 164f / 255f;
            case 223: return 165f / 255f;
            case 224: return 166f / 255f;
            case 225: return 167f / 255f;
            case 226: return 168f / 255f;
            case 227: return 169f / 255f;
            case 228: return 170f / 255f;
            case 229: return 171f / 255f;
            case 230: return 172f / 255f;
            case 231: return 173f / 255f;
            case 232: return 174f / 255f;
            case 233: return 255f; // Slice, but would be 174f / 255f;
            case 234: return 175f / 255f;
            case 235: return 176f / 255f;
            case 236: return 177f / 255f;
            case 237: return 178f / 255f;
            case 238: return 179f / 255f;
            case 239: return 180f / 255f;
            case 240: return 181f / 255f;
            case 241: return 182f / 255f;
            case 242: return 255f; // Slice, but would be 182f / 255f;
            case 243: return 255f; // Slice, but would be 182f / 255f;
            case 244: return 183f / 255f;
            case 245: return 184f / 255f;
            case 246: return 255f; // Slice, but would be 184f / 255f;
            case 247: return 255f; // Slice, but would be 184f / 255f;
            case 248: return 255f; // Slice, but would be 184f / 255f;
            case 249: return 255f; // Slice, but would be 184f / 255f;
            case 250: return 185f / 255f;
            case 251: return 186f / 255f;
            case 252: return 187f / 255f;
            case 253: return 188f / 255f;
            case 254: return 189f / 255f;
            case 255: return 255f; // Slice, but would be 189f / 255f;
            case 256: return 190f / 255f;
            case 257: return 191f / 255f;
            case 258: return 192f / 255f;
            case 259: return 193f / 255f;
            case 260: return 194f / 255f;
            case 261: return 195f / 255f;
            case 262: return 196f / 255f;
            case 263: return 255f; // Slice, but would be 196f / 255f;
            case 264: return 255f; // Slice, but would be 196f / 255f;
            case 265: return 197f / 255f;
            case 266: return 198f / 255f;
            case 267: return 199f / 255f;
            case 268: return 200f / 255f;
            case 269: return 201f / 255f;
            case 270: return 202f / 255f;
            case 271: return 203f / 255f;
            case 272: return 204f / 255f;
            case 273: return 205f / 255f;
            case 274: return 206f / 255f;
            case 275: return 207f / 255f;
            case 276: return 208f / 255f;
            case 277: return 209f / 255f;
            case 278: return 210f / 255f;

            case 280: return 211f / 255f;
            case 281: return 212f / 255f;
            case 282: return 213f / 255f;
            case 283: return 255f; // Slice, but would be 213f / 255f;
            case 284: return 255f; // Slice, but would be 213f / 255f;
            case 285: return 255f; // Slice, but would be 213f / 255f;
            case 286: return 255f; // Slice, but would be 213f / 255f;
            case 287: return 255f; // Slice, but would be 213f / 255f;
            case 288: return 214f / 255f;
            case 289: return 215f / 255f;
            case 290: return 216f / 255f;
            case 291: return 255f; // Slice, but would be 216f / 255f;
            case 292: return 217f / 255f;
            case 293: return 218f / 255f;
            case 294: return 219f / 255f;
            case 295: return 220f / 255f;
            case 296: return 221f / 255f;
            case 297: return 222f / 255f;
            case 298: return 255f; // Slice, but would be 222f / 255f;
            case 299: return 255f; // Slice, but would be 222f / 255f;
            case 300: return 255f; // Slice, but would be 222f / 255f;
            case 301: return 255f; // Slice, but would be 222f / 255f;
            case 302: return 223f / 255f;
            case 303: return 255f; // Slice, but would be 223f / 255f;
            case 304: return 224f / 255f;
            
            // And so on
            default: return 255f; // Null value
        }
    }
    
    void ClearCellList() {
        gridCells = new GridCell[WORLDX,WORLDX];
        worldCellCheckedYet = new bool [WORLDX,WORLDX];
        staticMeshesImmutable = new List<MeshRenderer>();
        staticMeshImmutableCoords = new List<Vector2Int>();
        staticMeshesSaveable = new List<MeshRenderer>();
        staticMeshSaveableCoords = new List<Vector2Int>();
        doors = new List<MeshRenderer>();
        doorsCoords = new List<Vector2Int>();
        lights = new List<Light>();
        lightsInPVS = new List<Light>();
        lightCoords = new List<Vector2Int>();
        lightsInPVSCoords = new List<Vector2Int>();
        dynamicMeshes = new List<MeshRenderer>();
        dynamicMeshCoords = new List<Vector2Int>();
        npcAICs = new List<AIController>();
        npcTransforms = new List<Transform>();
        for (int x=0;x<WORLDX;x++) {
            for (int y=0;y<WORLDX;y++) {
                gridCells[x,y] = new GridCell();
                gridCells[x,y].x = x;
                gridCells[x,y].y = y;
                gridCells[x,y].open = false;
                gridCells[x,y].visible = false;
                gridCells[x,y].closedNorth = false;
                gridCells[x,y].closedEast = false;
                gridCells[x,y].closedSouth = false;
                gridCells[x,y].closedWest = false;
                gridCells[x,y].chunkPrefabs = new List<ChunkPrefab>();
                gridCells[x,y].dynamicObjects = new List<DynamicObject>();
                gridCells[x,y].visibleCellsFromHere = new bool[WORLDX,WORLDX];
            }
        }
    }

    // ========================================================================
    // Handle Occluders (well, just determining visible cells and their chunks)
    
    Meshenderer GetMeshAndItsRenderer(GameObject go, PrefabIdentifier pid) {
        // Add top level GameObject's mesh renderer and filter.
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        Mesh msh = null;
        if (mr == null) return null;
        
        MeshFilter mf = go.GetComponent<MeshFilter>();
        Meshenderer mrr = new Meshenderer();
        mrr.meshRenderer = mr;
        mrr.meshFilter = mf;
        msh = mf.sharedMesh;
        mrr.meshUsual = msh;
        if (pid == null) {
            mrr.meshLOD = msh;
        } else {
            if (pid.constIndex <= 304 && pid.constIndex > 0) {
                if (lodMeshes[pid.constIndex] != null) {
                    mrr.meshLOD = lodMeshes[pid.constIndex];
                } else mrr.meshLOD = msh;
            } else mrr.meshLOD = msh;
        }
        return mrr;
    }
    
    void PutChunksInCells() {
        Transform ctn = LevelManager.a.GetCurrentGeometryContainer().transform;
        int chunkCount = ctn.childCount;
        GameObject childGO = null;
        int x,y;
        ChunkPrefab cr = null;
        Vector2Int posint = new Vector2Int(0,0);
        PrefabIdentifier pid = null;
        for (int c=0;c<chunkCount;c++) {
            childGO = ctn.GetChild(c).gameObject;
            posint = PosToCellCoordsChunks(childGO.transform.position);
            x = posint.x;
            y = posint.y;
            pid = childGO.GetComponent<PrefabIdentifier>();
            Transform ctr = null;
            if (pid == null) {
                ctr = childGO.transform.GetChild(0);
                if (ctr != null) {
                    pid = ctr.gameObject.GetComponent<PrefabIdentifier>();
                }
            }
            
            if (pid == null) Debug.Log(childGO.name + " no PrefabIdentifier!");
            cr = new ChunkPrefab();
            cr.x = x;
            cr.y = y;
            cr.constIndex = pid.constIndex;
            cr.go = childGO;
            cr.meshenderers = new List<Meshenderer>();

            // Add top level GameObject's mesh renderer and filter.
            Meshenderer mrr = GetMeshAndItsRenderer(cr.go,pid);
            if (mrr != null) cr.meshenderers.Add(mrr);
            
            // Add children GameObjects' mesh renderers and filters.
            Component[] compArray = cr.go.GetComponentsInChildren(
                                                    typeof(MeshFilter),true);
            foreach (MeshFilter mfc in compArray) {
                mrr = GetMeshAndItsRenderer(mfc.gameObject,pid);
                if (mrr != null) cr.meshenderers.Add(mrr);
            }

            gridCells[x,y].chunkPrefabs.Add(cr);
            gridCells[x,y].open = true;
        }
    }

    void MakeDebugCube(Transform tr, Vector3 ofs, Material mat) {
//         GameObject debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube); // Automatically adds MeshFilter and MeshRenderer
//         debugCube.transform.parent = tr;
//         debugCube.transform.localPosition = ofs;
//         MeshRenderer mr = debugCube.GetComponent<MeshRenderer>();
//         mr.material = mat;
    }

    void DetermineClosedEdges() {
        Color32[] edgePixels = worldClosedEdges[LevelManager.a.currentLevel].GetPixels32();
        for (int x=0;x<WORLDX;x++) {
            for (int y=0;y<WORLDX;y++) {
                gridCells[x,y].closedNorth = false;
                gridCells[x,y].closedEast = false;
                gridCells[x,y].closedSouth = false;
                gridCells[x,y].closedWest = false;
                Color32 closedData = edgePixels[x + y * 64];
                if (closedData.r > 127) gridCells[x,y].closedNorth = true;
                if (closedData.g > 127) gridCells[x,y].closedEast = true;
                if (closedData.b > 127) gridCells[x,y].closedSouth = true;
                if (   (closedData.r < 255 && closedData.r > 0)
                    || (closedData.g < 255 && closedData.g > 0)
                    || (closedData.b < 255 && closedData.b > 0)) {
                    
                    gridCells[x,y].closedWest = true;
                }
                
                if (closedData.a > 0 && closedData.a < 255) {
                    gridCells[x,y].closedNorth = gridCells[x,y].closedEast
                    = gridCells[x,y].closedSouth = gridCells[x,y].closedWest
                    = true;
                }

//                 Debug.Log("gridCells[" + x.ToString() + "," + y.ToString()
//                           + "] has NESW:"
//                           + (gridCells[x,y].closedNorth ? "1" : "0")
//                           + (gridCells[x,y].closedEast ? "1" : "0")
//                           + (gridCells[x,y].closedSouth ? "1" : "0")
//                           + (gridCells[x,y].closedWest ? "1" : "0")
//                           + " from color: " + closedData.ToString());
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
                PrefabIdentifier pid = mr.GetComponent<PrefabIdentifier>();
                if (pid == null) pid = mr.transform.parent.GetComponent<PrefabIdentifier>();
                if (pid == null) pid = mr.transform.parent.parent.GetComponent<PrefabIdentifier>();
                if (pid == null) return;

                if (pid.constIndex == 592 || pid.constIndex == 593) return;

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

    public void AddCameraPosition(CameraView cam) {
        if (!camPositions.ContainsKey(cam.gameObject)) {
            camPositions[cam.gameObject] = cam.transform.position;
        }
    }

    public void RemoveCameraPosition(CameraView cam) {
        camPositions.Remove(cam.gameObject);
    }

    public void Cull_Init() {
        // Populate LOD (Level of Detail) Meshes for farther chunks.
        // This creates a prepopulated list of quads (2 tris each) with vertex
        // color Red channel set to the index into the Texture2DArray for the
        // chunk shader.  These get swapped out in place of the
        // staticMeshes_ arrays above based on player's distance.  This list is
        // just the representation for each chunk prefab, excepting slices.
        if (!lodMeshesInitialized) {
            a.lodMeshes = new Mesh[305]; // Constindexes 0 through 304.
            Color[] vertColors = new Color[lodMeshTemplate.vertexCount];
            for (int i=0;i<vertColors.Length;i++) {
                vertColors[i] = new Color(0f,0f,0f,1f);
            }

            float red = 0f;
            for (int i=0;i<305;i++) {
                red = GetVertexColorForChunk(i);
                if (red == 255f) { a.lodMeshes[i] = null; continue; }
                
                a.lodMeshes[i] = new Mesh();
                a.lodMeshes[i].name = "lodMesh" + i.ToString();
                a.lodMeshes[i].vertices = lodMeshTemplate.vertices;
                a.lodMeshes[i].triangles = lodMeshTemplate.triangles;
                a.lodMeshes[i].normals = lodMeshTemplate.normals;
                a.lodMeshes[i].uv = lodMeshTemplate.uv;
                for (int j=0;j<vertColors.Length;j++) {
                    vertColors[j].r = red; // All matching.
                }
                a.lodMeshes[i].colors = vertColors;
            }
            
            lodMeshesInitialized = true;
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
                "gridcellsopen_" + LevelManager.a.currentLevel.ToString()
                + ".png");

            visDebugImagePath = Utils.SafePathCombine(
                Application.streamingAssetsPath,
                "worldcellvis_" + LevelManager.a.currentLevel.ToString()
                + ".png");
        }

        PutChunksInCells();
        DetermineClosedEdges();
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
        if (!cullEnabled) return;

        // Force all cells dirty at start so the visibility is toggled for all.
        for (x=0;x<64;x++) {
            for (y=0;y<64;y++) worldCellLastVisible[x,y] = !gridCells[x,y].visible;
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
        Vector2Int pxy = PosToCellCoords(MouseLookScript.a.transform.position);
        playerCellX = pxy.x;
        playerCellY = pxy.y;
    }

    bool UpdatedPlayerCell() {
        if (forceRecull) {
            for (int x=0;x<64;x++) {
                for (int y=0;y<64;y++) {
                    worldCellLastVisible[x,y] = false;
                    worldCellCheckedYet[x,y] = false;
                }
            }
            forceRecull = false;
            return true;
        }
        
        int lastX = playerCellX;
        int lastY = playerCellY;
        FindPlayerCell();
        if (playerCellX == lastX && playerCellY == lastY) return false;
        return true;
    }

    bool XYPairInBounds(int x, int y) {
        return (x < 64 && y < 64 && x >= 0 && y >= 0);
    }

    // Accessible meaning that there aren't closed edges blocking the view to
    // that cell at x,y (checked in the calls below).
    void SetVisible(bool isAccessible, int x, int y) {
        if (!isAccessible) {
            gridCells[x,y].visible = false;
        } else {
            gridCells[x,y].visible = worldCellsOpen[x,y];
        }
        
        worldCellCheckedYet[x,y] = true;
        if (gridCells[x,y].visible) SetVisPixel(x,y,Color.cyan);
        else SetVisPixel(x,y,Color.black);
    }

    //void DetermineVisibleCellsPrecalculated() {
    //    int playerCell4096 = playerCellX + (playerCellY * 64);
    //    for (int x=0;x<64;x++) {
    //        for (int y=0;y<64;y++) {
    //            gridCells[x,y].visible = 
    //                (precalculatedPVS[playerCell4096].GetPixel(x,y,0).r > 0.99f);
    //        }
    //    }
    //}
    
    bool IsAccessible(GridCell start, GridCell end) {
        bool accessibleCC; // Caddy Corner, if cell closed on adjacent edges.
        bool accessibleAA; // Axis Aligned X, check if neighbor and cell closed.
        bool accessibleBB; // Axis Aligned Y, check if neighbor and cell closed.
        // The way neighbor checks work is that, if both current cell and the
        // neighbor share a connected closed edge, e.g. North and North, then
        // the caddy corner cell just to the North of the neighbor is also
        // inaccessible and not visible.

        if (start.x > end.x && start.y < end.y) {
            // NW
            // [ 2][BB][  ] // CC is at playerCellX,playerCellY.
            // [AA][CC][  ]

            // [ 2][ W][  ] Have to check both cells facing north aren't closed
            // [ N][NW][  ] and both cells facing west aren't closed as well as
            //              the current cell.  Same idea for all of them.
            accessibleCC = !(start.closedNorth && start.closedWest);
            accessibleAA = !(start.closedNorth && end.closedNorth);
            accessibleBB = !(start.closedWest && end.closedWest);
            return accessibleCC && accessibleAA && accessibleBB;
        } else if (start.x < end.x && start.y < end.y) {
            // NE
            // [  ][BB][ 2] // CC is at playerCellX,playerCellY.
            // [  ][CC][AA]

            // [  ][ E][ 2]
            // [  ][NE][ N]
            accessibleCC = !(start.closedNorth && start.closedEast);
            accessibleAA = !(start.closedNorth && end.closedNorth);
            accessibleBB = !(start.closedEast && end.closedEast);
            return accessibleCC && accessibleAA && accessibleBB;
        } else if (start.x > end.x && start.y > end.y) {
            // SW
            // [AA][CC][  ] // CC is at playerCellX,playerCellY.
            // [ 2][BB][  ]

            // [ S][SW][  ]
            // [ 2][ W][  ]
            accessibleCC = !(start.closedSouth && start.closedWest);
            accessibleAA = !(start.closedSouth && end.closedSouth);
            accessibleBB = !(start.closedWest && end.closedWest);
            return accessibleCC && accessibleAA && accessibleBB;
        } else if (start.x < end.x && start.y > end.y) {
            // SE
            // [  ][CC][AA] // CC is at playerCellX,playerCellY.
            // [  ][BB][ 2]

            // [  ][SE][ S]
            // [  ][ E][ 2]
            accessibleCC = !(start.closedSouth && start.closedEast);
            accessibleAA = !(start.closedSouth && end.closedSouth);
            accessibleBB = !(start.closedEast && end.closedEast);   
            return accessibleCC && accessibleAA && accessibleBB;
        } else if (start.x > end.x && start.y == end.y) {
            // W
            // [ 2][ 1]
            return !start.closedWest;
        } else if (start.x < end.x && start.y == end.y) {
            // E
            // [ 1][ 2]
            return !start.closedEast;
        } else if (start.x == end.x && start.y < end.y) {
            // N
            // [ 2]
            // [ 1]
            return !start.closedNorth;
        } else if (start.x == end.x && start.y > end.y) {
            // S
            // [ 1]
            // [ 2]
            return !start.closedSouth;
        }

        return false;
    }

    void DetermineVisibleCells() {
        int x,y;
        for (x=0;x<64;x++) {
            for (y=0;y<64;y++) {
                gridCells[x,y].visible = false;
                worldCellsOpen[x,y] = gridCells[x,y].open;
                worldCellCheckedYet[x,y] = false;
                if (outputDebugImages) {
                    pixels[x + (y * 64)] = gridCells[x,y].open
                                           ? Color.white : Color.black;
                }
            }
        }

        x = playerCellX; y = playerCellY;
        gridCells[x,y].visible = true;
        worldCellCheckedYet[x,y] = true;
        SetVisPixel(x,y,Color.blue);

        // Set all neighboring cells visible if open in 3x3 square.
        // Ordinals
        if (XYPairInBounds(x,y+1)) SetVisible(IsAccessible(gridCells[x,y],gridCells[x,y + 1]),x,y + 1); // North
        if (XYPairInBounds(x+1,y)) SetVisible(IsAccessible(gridCells[x,y],gridCells[x + 1,y]),x + 1,y); // East
        if (XYPairInBounds(x,y-1)) SetVisible(IsAccessible(gridCells[x,y],gridCells[x,y - 1]),x,y - 1); // South
        if (XYPairInBounds(x-1,y)) SetVisible(IsAccessible(gridCells[x,y],gridCells[x - 1,y]),x - 1,y); // Weast

        // Diagonals
        if (XYPairInBounds(x-1,y+1)) SetVisible(IsAccessible(gridCells[x,y],gridCells[x-1,y+1]),x - 1,y + 1); // NW
        if (XYPairInBounds(x+1,y+1)) SetVisible(IsAccessible(gridCells[x,y],gridCells[x+1,y+1]),x + 1,y + 1); // NE
        if (XYPairInBounds(x-1,y-1)) SetVisible(IsAccessible(gridCells[x,y],gridCells[x-1,y-1]),x - 1,y - 1); // SW
        if (XYPairInBounds(x+1,y-1)) SetVisible(IsAccessible(gridCells[x,y],gridCells[x+1,y-1]),x + 1,y - 1); // SE

//         CastStraightX(playerCellX,playerCellY,1);  // [ ][3]
//                                                    // [1][2]
//                                                    // [ ][3]
// 
//         CastStraightX(playerCellX,playerCellY,-1); // [3][ ]
//                                                    // [2][1]
//                                                    // [3][ ]
// 
//         CastStraightY(playerCellX,playerCellY,1);  // [3][2][3]
//                                                    // [ ][1][ ]
// 
//         CastStraightY(playerCellX,playerCellY,-1); // [ ][1][ ]
//                                                    // [3][2][3]
        CircleFanRays(playerCellX,playerCellY);

        Vector2Int pnt = new Vector2Int();
        // Use a for loop to iterate over the positions
        foreach (KeyValuePair<GameObject, Vector3> entry in camPositions) {
            GameObject camGO = entry.Key; // The GameObject key
            CameraView camV = camGO.GetComponent<CameraView>();
            if (camV == null) continue;
            if (!camV.IsVisible()) continue;

            Vector3 position = entry.Value; // The position value
            pnt = PosToCellCoords(position);
            gridCells[pnt.x,pnt.y].visible = true;
            worldCellCheckedYet[pnt.x,pnt.y] = true;
            SetVisPixel(pnt.x,pnt.y,Color.blue);
            
//             CastStraightX(pnt.x,pnt.y,1);      // [ ][3]
//             CastStraightX(pnt.x,pnt.y + 1,1);  // [1][2]
//             CastStraightX(pnt.x,pnt.y - 1,1);  // [ ][3]
// 
//             CastStraightX(pnt.x,pnt.y,-1);     // [3][ ]
//             CastStraightX(pnt.x,pnt.y + 1,-1); // [2][1]
//             CastStraightX(pnt.x,pnt.y - 1,-1); // [3][ ]
// 
//             CastStraightY(pnt.x,pnt.y,1);      // [3][2][3]
//             CastStraightY(pnt.x + 1,pnt.y,1);  // [ ][1][ ]
//             CastStraightY(pnt.x - 1,pnt.y,1);
// 
//             CastStraightY(pnt.x,pnt.y,-1);     // [ ][1][ ]
//             CastStraightY(pnt.x + 1,pnt.y,-1); // [3][2][3]
//             CastStraightY(pnt.x - 1,pnt.y,-1);
            
            CircleFanRays(pnt.x,pnt.y);
        }

        // Output Debug image of the open
        if (outputDebugImages) {
            debugTex = new Texture2D(64,64);
            debugTex.SetPixels32(pixels);
            debugTex.Apply();
            bytes = debugTex.EncodeToPNG();
            File.WriteAllBytes(visDebugImagePath,bytes);
        }
    }

    void SetVisPixel(int x, int y, Color col){
        if (!outputDebugImages) return;

        pixels[x + (y * 64)] = gridCells[x,y].visible
                               ? col
                               : (worldCellsOpen[x,y] ? Color.white
                                                      : Color.black);
    }

    private void CastStraightY(int px, int py, int signy) {
        if (signy > 0 && py >= 63) return;
        if (signy < 0 && py <= 0) return;
        if (!XYPairInBounds(px,py)) return;
        if (!gridCells[px,py].visible) return;

        int x = px;
        int y = py + signy;
        bool currentVisible = true;
        for (;y<64;y+=signy) { // Up
            currentVisible = false;
            if (XYPairInBounds(x,y - signy) && XYPairInBounds(x,y)) {
                if (!worldCellCheckedYet[x,y]) {
                    if (gridCells[x,y - signy].visible) {
                        if (signy > 0) {
                            if (gridCells[x,y - 1].closedNorth) return;
                        } else if (signy < 0) {
                            if (gridCells[x,y + 1].closedSouth) return;
                        }
                        
                        gridCells[x,y].visible = worldCellsOpen[x,y];
                        worldCellCheckedYet[x,y] = true;
                        currentVisible = true; // Would be if twas open.
                        SetVisPixel(x,y,Color.green);
                    }
                } else {
                    currentVisible = worldCellsOpen[x,y]; // Keep going.
                }
            }

            if (!currentVisible) break; // Hit wall!

            if (XYPairInBounds(x + 1,y)) {
                if (!worldCellCheckedYet[x + 1,y]) {
                    if (IsAccessible(gridCells[x,y],gridCells[x + 1,y])) {
                        gridCells[x + 1,y].visible = gridCells[x + 1,y].open;
                    } else {
                        gridCells[x + 1,y].visible = false;
                    }
                    
                    worldCellCheckedYet[x + 1,y] = true;
                    if (gridCells[x+1,y].visible) SetVisPixel(x + 1,y,Color.green);
                    else SetVisPixel(x+1,y,Color.black);
                }
            }

            if (XYPairInBounds(x - 1,y)) {
                if (!worldCellCheckedYet[x - 1,y]) {
                    if (IsAccessible(gridCells[x,y],gridCells[x - 1,y])) {
                        gridCells[x - 1,y].visible = gridCells[x - 1,y].open;
                    } else {
                        gridCells[x - 1,y].visible = false;
                    }
                    
                    worldCellCheckedYet[x - 1,y] = true;
                    if (gridCells[x-1,y].visible) SetVisPixel(x - 1,y,Color.green);
                    else SetVisPixel(x-1,y,Color.black);
                }
            }
        }
    }

    private void CastStraightX(int px, int py, int signx) {
        if (signx > 0 && px >= 63) return;
        if (signx < 0 && px <= 0) return;
        if (!XYPairInBounds(px,py)) return;
        if (!gridCells[px,py].visible) return;
        
        int x = px + signx;
        int y = py;
        bool currentVisible = true;
        for (;x<64;x+=signx) { // Right
            currentVisible = false;
            if (XYPairInBounds(x - signx,y) && XYPairInBounds(x,y)) {
                if (!worldCellCheckedYet[x,y]) {
                    if (gridCells[x - signx,y].visible) {
                        if (signx > 0) {
                            if (gridCells[x - 1,y].closedEast) return;
                        } else if (signx < 0) {
                            if (gridCells[x + 1,y].closedWest) return;
                        }
                        
                        gridCells[x,y].visible = worldCellsOpen[x,y];
                        worldCellCheckedYet[x,y] = true;
                        currentVisible = true; // Would be if twas open.
                        SetVisPixel(x,y,Color.green);
                    }
                } else {
                    currentVisible = worldCellsOpen[x,y]; // Keep going.
                }
            }

            if (!currentVisible) break; // Hit wall!

            if (XYPairInBounds(x,y + 1)) {
                if (!worldCellCheckedYet[x,y + 1]) {
                    if (IsAccessible(gridCells[x,y],gridCells[x,y + 1])) {
                        gridCells[x,y + 1].visible = gridCells[x,y + 1].open;
                    } else {
                        gridCells[x,y + 1].visible = false;
                    }
                    
                    worldCellCheckedYet[x,y + 1] = true;
                    if (gridCells[x,y + 1].visible) SetVisPixel(x,y + 1,Color.green);
                    else SetVisPixel(x,y + 1,Color.black);
                }
            }

            if (XYPairInBounds(x,y - 1)) {
                if (!worldCellCheckedYet[x,y - 1]) {
                    if (IsAccessible(gridCells[x,y],gridCells[x,y - 1])) {
                        gridCells[x,y - 1].visible = gridCells[x,y - 1].open;
                    } else {
                        gridCells[x,y - 1].visible = false;
                    }
                    
                    worldCellCheckedYet[x,y - 1] = true;
                    if (gridCells[x,y - 1].visible) SetVisPixel(x,y - 1,Color.green);
                    else SetVisPixel(x,y - 1,Color.black);
                }
            }
        }
    }
    
    // CastRay()'s in fan from x0,y0 out to every cell around map perimeter.
    private void CircleFanRays(int x0, int y0) {
        int x,y;
        if (XYPairInBounds(x0,y0)) {
            if (gridCells[x0,y0].visible) {
                for (x=1;x<63;x++) CastRay(x0,y0,x,0);
                for (x=1;x<63;x++) CastRay(x0,y0,x,63);
                for (y=1;y<63;y++) CastRay(x0,y0,0,y);
                for (y=1;y<63;y++) CastRay(x0,y0,63,y);
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
        int lastX = x;
        int lastY = y;
        while (iter >= 0) {
            if (!XYPairInBounds(x,y)) return;
            if (!XYPairInBounds(lastX,lastY)) return;
            if (CastRayCellCheck(x,y,lastX,lastY) == -1) return;

            lastX = x;
            lastY = y;
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

//     int CastRayCellCheck(GridCell start, GridCell end) {
//         if (!IsAccessible(start,end)) return -1; // End ray for closed edge.
// 
//         end.visible = worldCellsOpen[end.x,end.y];
//         if (end.visible) {
//             SetVisPixel(end.x,end.y,new Color(0f,0f,0.5f,1f));
//         } else {
//             SetVisPixel(end.x,end.y,new Color(1f,0f,0f,1f));
//             return -1; // End ray for solid space.
//         }
// 
//         return 1;
//     }

    int CastRayCellCheck(int x, int y, int lastX, int lastY) {
        bool edgesOpen = true;
        if (!(lastX == x && lastY == y)) {
            if (XYPairInBounds(lastX,lastY)) {
                if (lastY == y) {
                    if (lastX > x) { // [  x  ][lastX]
                        if (gridCells[lastX,lastY].closedWest) {
                            if (outputDebugImages) {
                                pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                            }
                            return -1;
                        }
                    } else { // Less than x since == x was already checked.
                        if (gridCells[lastX,lastY].closedEast) {
                            if (outputDebugImages) {
                                pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                            }
                            return -1;
                        }
                    }
                }

                if (lastX == x) {
                    if (lastY > y) { // [lastY]
                                     // [  y  ]
                        if (gridCells[lastX,lastY].closedSouth) {
                             if (outputDebugImages) {
                                pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                            }
                            return -1;
                        }
                    } else { // Less than y since == y was already checked.
                        if (gridCells[lastX,lastY].closedNorth) {
                            if (outputDebugImages) {
                                pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                            }
                            return -1;
                        }
                    }
                }

                // Diagonals
                if (lastY != y && lastX != x) {
                    if (lastY > y && lastX > x) { // [ ][1]
                                                  // [2][N]
                        bool neighborClosedWest = false;
                        if (XYPairInBounds(lastX,lastY - 1)) neighborClosedWest = gridCells[lastX,lastY - 1].closedWest;
                        if ((gridCells[lastX,lastY].closedSouth && gridCells[lastX,lastY].closedWest)
                            || (gridCells[lastX,lastY].closedWest && neighborClosedWest)) {
                            
                            if (outputDebugImages) {
                                pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                            }
                            return -1;
                        }
                    } else if (lastY < y && lastX < x) { // [ ][2]
                                                         // [1][ ]
                        bool neighborClosedEast = false;
                        if (XYPairInBounds(lastX,lastY + 1)) neighborClosedEast = gridCells[lastX,lastY + 1].closedEast;
                        if ((gridCells[lastX,lastY].closedNorth && gridCells[lastX,lastY].closedEast)
                            || (gridCells[lastX,lastY].closedEast && neighborClosedEast)) {
                            
                            if (outputDebugImages) {
                                pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                            }
                            return -1;
                        }
                    } else if (lastY > y && lastX < x) { // [1][ ]
                                                         // [ ][2]
                        bool neighborClosedEast = false;
                        if (XYPairInBounds(lastX,lastY - 1)) neighborClosedEast = gridCells[lastX,lastY - 1].closedEast;
                        if ((gridCells[lastX,lastY].closedSouth && gridCells[lastX,lastY].closedEast)
                            || (gridCells[lastX,lastY].closedEast && neighborClosedEast)) {
                            
                            if (outputDebugImages) {
                                pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                            }
                            return -1;
                        }
                    } else if (lastY < y && lastX > x) { // [2][ ]
                                                         // [ ][1]
                        bool neighborClosedWest = false;
                        if (XYPairInBounds(lastX,lastY + 1)) neighborClosedWest = gridCells[lastX,lastY + 1].closedWest;
                        if ((gridCells[lastX,lastY].closedNorth && gridCells[lastX,lastY].closedWest)
                            || (gridCells[lastX,lastY].closedWest && neighborClosedWest)) {
                            
                            if (outputDebugImages) {
                                pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                            }
                            return -1;
                        }
                    }
                }
            }
        }
        
        if (XYPairInBounds(x,y)) {
//            if (!worldCellCheckedYet[x,y] || !worldCellsOpen[x,y]) {
                gridCells[x,y].visible = (edgesOpen && worldCellsOpen[x,y]);
                if (!worldCellCheckedYet[x,y]) {
                    SetVisPixel(x,y,new Color(0f,0f,0.5f,1f));
                    worldCellCheckedYet[x,y] = true;
                }
                
                if (!gridCells[x,y].visible) {
                    if (outputDebugImages) {
                        pixels[x + (y * 64)] = new Color(1f,0f,0f,1f);
                    }

                    return -1;
                }


                return 1;
        }

        return 0;
    }
 
    void ToggleVisibility() {
        worldCellLastVisible[playerCellX,playerCellY] = false;
        gridCells[playerCellX,playerCellY].visible = true; // Guarantee enable.
        bool skyVisible = false;
        for (int x=0;x<64;x++) {
            for (int y=0;y<64;y++) {                
                float sqrdist = 0f;
                ChunkPrefab chp = null;
                for (int i=0;i<gridCells[x,y].chunkPrefabs.Count;i++) {
                    chp = gridCells[x,y].chunkPrefabs[i];
                    if (chp.constIndex == 1) {
                        if (gridCells[x,y].visible) {
//                             Debug.Log("Sky set visible by chunk at "
//                                       + x.ToString() + "," + y.ToString());
                            
                            skyVisible = true;
                        }
                    }
                    if (chp == null) continue;

                    for (int k=0;k<chp.meshenderers.Count;k++) {
                        chp.meshenderers[k].meshRenderer.enabled =
                                                        gridCells[x,y].visible;

                        if (!gridCells[x,y].visible) continue;
                        if (chp.constIndex > 304 || chp.constIndex < 0) continue;

                        sqrdist = SqrDist(x,y,playerCellX,playerCellY);
                        chp.meshenderers[k].SetMesh(sqrdist >= lodSqrDist);
                    }
                }
            }
        }
        
        if (LevelManager.a != null) {
            if (skyVisible) LevelManager.a.SetSkyVisible(true);
            else LevelManager.a.SetSkyVisible(false);
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
        int x,y;
        for (int i=0;i<npcAICs.Count;i++) {
            x = npcCoords[i].x;
            y = npcCoords[i].y;
            if (gridCells[x,y].visible || !worldCellsOpen[x,y]
                || npcAICs[i].enemy != null || npcAICs[i].ai_dying
                || npcAICs[i].ai_dead) {
                npcAICs[i].withinPVS = true;
                hm = npcAICs[i].healthManager;
                if (npcAICs[i].visibleMeshVisible) {
                    Utils.Activate(npcAICs[i].visibleMeshEntity);
                } else {
                    Utils.Deactivate(npcAICs[i].visibleMeshEntity);
                }
            } else {
                npcAICs[i].withinPVS = false;
                Utils.Deactivate(npcAICs[i].visibleMeshEntity);
            }
        }
    }

    public void ToggleDynamicMeshesVisibility() {
        int x,y;
        for (int i=0;i<dynamicMeshes.Count;i++) {
            x = dynamicMeshCoords[i].x;
            y = dynamicMeshCoords[i].y;
            if (gridCells[x,y].visible || !worldCellsOpen[x,y]) {
                Utils.EnableMeshRenderer(dynamicMeshes[i]);
            } else {
                Utils.DisableMeshRenderer(dynamicMeshes[i]);
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
            if (gridCells[x,y].visible || !worldCellsOpen[x,y]) {
                staticMeshesImmutable[i].enabled = true;
            } else {
                staticMeshesImmutable[i].enabled = false;
            }
        }
    }

    public void ToggleStaticMeshesSaveableVisibility() {
        int x,y;
        for (int i=0;i<staticMeshesSaveable.Count;i++) {
            x = staticMeshSaveableCoords[i].x;
            y = staticMeshSaveableCoords[i].y;
            if (gridCells[x,y].visible || !worldCellsOpen[x,y]) {
                HealthManager hm =
                    staticMeshesSaveable[i].GetComponent<HealthManager>();
                    
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
        int x,y;
        for (int i=0;i<doors.Count;i++) {
            x = doorsCoords[i].x;
            y = doorsCoords[i].y;
            if (gridCells[x,y].visible || !worldCellsOpen[x,y]) {
                doors[i].enabled = true;
            } else {
                doors[i].enabled = false;
            }
        }
    }

    public void ToggleLightsVisibility() {
        if (!lightCulling) return;
        
        lightsInPVS.Clear();
        int x,y;
        for (int i=0;i<lights.Count;i++) {
            x = lightCoords[i].x;
            y = lightCoords[i].y;
            if (gridCells[x,y].visible || !worldCellsOpen[x,y]) {
                lights[i].enabled = true;
                lightsInPVS.Add(lights[i]);
                lightsInPVSCoords.Add(lightCoords[i]);
            } else {
                lights[i].enabled = false;
            }
        }
    }

    public void ToggleLightsFrustumVisibility() {
        if (!lightCulling) return;

        Vector3 lightPos;
        Vector3 lightDir;
        for (int i=0;i<lightsInPVS.Count;i++) {
            lightPos = lightsInPVS[i].transform.position;
            lightDir = PlayerMovement.a.transform.position - lightPos;
            if (Vector3.Dot(lightDir,MouseLookScript.a.transform.forward) < 0.5f) {
                lightsInPVS[i].enabled = true;
//                 Debug.Log("Light at "
//                 + lightsInPVS[i].transform.position.ToString()
//                 + " VISIBLE, check was: "
//                 + Vector3.Dot(lightDir,MouseLookScript.a.transform.forward).ToString());
            } else {
                if (lightDir.magnitude > 10.24f) lightsInPVS[i].enabled = true;
                else lightsInPVS[i].enabled = false;
//                 Debug.Log("Light at "
//                 + lightsInPVS[i].transform.position.ToString()
//                 + " NOT VISIBLE, check was: "
//                 + Vector3.Dot(lightDir,MouseLookScript.a.transform.forward).ToString());
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
                    worldCellLastVisible[x,y] = gridCells[x,y].visible;
                    worldCellCheckedYet[x,y] = false;
                }
            }

            DetermineVisibleCells(); // Reevaluate visible cells from new pos.
            //DetermineVisibleCellsPrecalculated(); // Reevaluate visible cells from new pos.
            gridCells[0,0].visible = true; // Errors default here so draw them anyways.
            ToggleVisibility(); // Update all cells marked as dirty.
            ToggleStaticMeshesImmutableVisibility();
            ToggleStaticMeshesSaveableVisibility();
            ToggleDoorsVisibility();
            ToggleLightsVisibility();
            UpdateNPCPVS();
            ToggleNPCPVS();
        }

        ToggleLightsFrustumVisibility();

        // Update dynamic meshes after PVS has been updated, if player moved.
        if (dynamicObjectCull) {
            UpdateDynamicMeshes(); // Always check all because any can move.
            ToggleDynamicMeshesVisibility(); // Now turn them on or off.
        }
    }
}

