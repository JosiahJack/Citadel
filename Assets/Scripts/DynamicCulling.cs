using System;
using System.IO;
using Unity.Jobs;
using Unity.Burst;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine;
using System.Runtime.CompilerServices;

public class DynamicCulling : MonoBehaviour {
    [HideInInspector] public const int WORLDX = 64;
    [HideInInspector] public const int ARRSIZE = WORLDX * WORLDX;
    [HideInInspector] public const float CELLXHALF = 1.28f;
    
    public bool cullEnabled = true;
    public bool debugHelpers = false;
    public bool dynamicObjectCull = true;
    public bool lightCulling = true;
    public bool lightsFrustumCull = false;
    public bool outputDebugImages = false;
    public bool forceRecull = false;
    public bool mergeVisibleMeshes = false;
    public bool useLODMeshes = true;
    public bool overrideWindowsToBlack = false; // Used by unit tests.
    public Material chunkMaterial;
    public Material genericMaterial;
    public Texture2DArray chunkAlbedo;
    [HideInInspector] public GridCell[,] gridCells;
    [HideInInspector] public List<MeshRenderer> dynamicMeshes = new List<MeshRenderer>();
    [HideInInspector] public List<PrefabIdentifier> dynamicMeshesPIDs = new List<PrefabIdentifier>();
    [HideInInspector] public List<HealthManager> dynamicMeshesHMs = new List<HealthManager>();
    [HideInInspector] public List<Transform> dynamicMeshesTransforms = new List<Transform>();
    [HideInInspector] public List<ForceBridge> dynamicMeshesFBs = new List<ForceBridge>();
    [HideInInspector] public List<Vector2Int> dynamicMeshCoords = new List<Vector2Int>();
    [HideInInspector] public List<MeshRenderer> staticMeshesImmutable = new List<MeshRenderer>();
    [HideInInspector] public List<PrefabIdentifier> staticMeshesImmutablePIDs = new List<PrefabIdentifier>();
    [HideInInspector] public List<Vector2Int>   staticMeshImmutableCoords = new List<Vector2Int>();
    [HideInInspector] public List<Vector2Int>   staticImmutableParticleCoords = new List<Vector2Int>();
    [HideInInspector] public List<PauseParticleSystem> staticImmutableParticleSystems = new List<PauseParticleSystem>();
    [HideInInspector] public List<MeshRenderer> staticMeshesSaveable = new List<MeshRenderer>();
    [HideInInspector] public List<HealthManager> staticMeshesSaveableHMs = new List<HealthManager>();
    [HideInInspector] public List<PrefabIdentifier> staticMeshesSaveablePIDs = new List<PrefabIdentifier>();
    [HideInInspector] public List<Vector2Int>   staticMeshSaveableCoords = new List<Vector2Int>();
    [HideInInspector] public List<Light> lights = new List<Light>();
    [HideInInspector] public List<Light> lightsInPVS = new List<Light>();
    [HideInInspector] public List<Vector2Int> lightCoords = new List<Vector2Int>();
    [HideInInspector] public List<Vector2Int> lightsInPVSCoords = new List<Vector2Int>();
    [HideInInspector] public List<MeshRenderer> doors = new List<MeshRenderer>();
    [HideInInspector] public List<Vector2Int> doorsCoords = new List<Vector2Int>();
    public int playerCellX = 0;
    public int playerCellY = 0;
    [HideInInspector] public float deltaX = 0.0f;
    [HideInInspector] public float deltaY = 0.0f;
    [HideInInspector] public float lodSqrDist = 2621.44f; // (20 * 2.56f)^2
    public Vector3 worldMin;
    [HideInInspector] public List<Transform> npcTransforms;
    [HideInInspector] public List<AIController> npcAICs = new List<AIController>();
    [HideInInspector] public List<Vector2Int> npcCoords = new List<Vector2Int>();
    [HideInInspector] public bool lodMeshesInitialized = false;
    [HideInInspector] public Mesh[] lodMeshes;
    public Mesh lodMeshTemplate;
    public bool skyVisibleToPlayer;
    private byte[] bytes;
    private static string visDebugImagePath;
    private Color32[] pixels;
    private Texture2D debugTex;
    [HideInInspector] public static Dictionary<GameObject, Vector3> camPositions = new Dictionary<GameObject, Vector3>();
    public CameraView[] cameraViews;
    private bool[,] worldCellsOpen = new bool[WORLDX,WORLDX];
    
    // Mesh combining
    private GameObject lastCombineResult;
    private List<Meshenderer> sourceMeshenderers;

    // Called before Culling so we don't screw up resultant cull enable states.
    public void UncombineMeshes() {
        if (lastCombineResult != null) {
            DestroyImmediate(lastCombineResult); // No longer display combined result
            for (int i=0;i<sourceMeshenderers.Count;i++) {
                sourceMeshenderers[i].meshRenderer.enabled = true; // Reset sources
            }
            
            sourceMeshenderers.Clear();
        }
    }

    // Called after Culling so we only combine the visible meshes for this frame.
    // Combines all child meshes and stores the result into NEW SUB GAMEOBJECT.
    // There will be a new child gameObjects created with MeshFilter and .
    public void CombineMeshes(bool isChunk) {
        CombineInstance[] combineInstances = new CombineInstance[sourceMeshenderers.Count];
        for(int i=0;i<sourceMeshenderers.Count;i++) {
            if (sourceMeshenderers[i].meshRenderer == null) continue;
            if (sourceMeshenderers[i].meshFilter == null) continue;
            
            sourceMeshenderers[i].meshRenderer.enabled = false; // Hide it; it's been subsumed into the collective.
            combineInstances[i].subMeshIndex = 0;
            combineInstances[i].mesh = sourceMeshenderers[i].meshFilter.sharedMesh;
            combineInstances[i].transform = sourceMeshenderers[i].meshFilter.transform.localToWorldMatrix;
        }

        Mesh combinedMesh = new Mesh {
            name = ("combined_mesh_" + gameObject.name),
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 // SUPER IMPORTANT!  Allows for greater tha 65535 verts!
        };

        combinedMesh.CombineMeshes(combineInstances,true,true,false); // Combine all meshes.
        lastCombineResult = new GameObject("CombinedMeshObject_" + gameObject.name);
        MeshFilter filter = lastCombineResult.AddComponent<MeshFilter>();
        filter.sharedMesh = combinedMesh;
        MeshRenderer renderer = lastCombineResult.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = genericMaterial;//isChunk ? chunkMaterial : genericMaterial;
    }
    
    public static DynamicCulling a;

    void Awake() {
        a = this;
        a.Cull_Init();
        a.pixels = new Color32[WORLDX * WORLDX];
        a.worldCellsOpen = new bool[WORLDX,WORLDX];
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

            case 94:  return  71f / 255f;
//             case 95:  return 255f; // Slice, but would be 71f / 255f;
            case 96:  return  72f / 255f;
            case 97:  return  73f / 255f;
            case 98:  return  74f / 255f;
            case 99:  return  75f / 255f;
            case 100: return  76f / 255f;
            case 101: return  77f / 255f;
            case 102: return  78f / 255f;
            case 103: return  79f / 255f;
            case 104: return  80f / 255f;
            case 105: return  81f / 255f;
            case 106: return  82f / 255f;
            case 107: return  83f / 255f;
            case 108: return  84f / 255f;
            case 109: return  85f / 255f;
            case 110: return  86f / 255f;
            case 111: return  87f / 255f;

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
        // Initialize on first call, reuse thereafter
        if (gridCells == null) {
            gridCells = new GridCell[WORLDX, WORLDX];
            for (int x = 0; x < WORLDX; x++) {
                for (int y = 0; y < WORLDX; y++) {
                    gridCells[x, y] = new GridCell();
                    gridCells[x, y].chunkPrefabs = new List<ChunkPrefab>();
                    gridCells[x, y].dynamicObjects = new List<DynamicObject>();
                    gridCells[x, y].visibleCellsFromHere = new bool[WORLDX, WORLDX];
                }
            }
        }
        if (staticMeshesImmutable == null) staticMeshesImmutable = new List<MeshRenderer>();
        if (staticMeshesImmutablePIDs == null) staticMeshesImmutablePIDs = new List<PrefabIdentifier>();
        if (staticMeshImmutableCoords == null) staticMeshImmutableCoords = new List<Vector2Int>();
        if (staticImmutableParticleCoords == null) staticImmutableParticleCoords = new List<Vector2Int>();
        if (staticImmutableParticleSystems == null) staticImmutableParticleSystems = new List<PauseParticleSystem>();
        if (staticMeshesSaveable == null) staticMeshesSaveable = new List<MeshRenderer>();
        if (staticMeshesSaveableHMs == null) staticMeshesSaveableHMs = new List<HealthManager>();
        if (staticMeshesSaveablePIDs == null) staticMeshesSaveablePIDs = new List<PrefabIdentifier>();
        if (staticMeshSaveableCoords == null) staticMeshSaveableCoords = new List<Vector2Int>();
        if (doors == null) doors = new List<MeshRenderer>();
        if (doorsCoords == null) doorsCoords = new List<Vector2Int>();
        if (lights == null) lights = new List<Light>();
        if (lightsInPVS == null) lightsInPVS = new List<Light>();
        if (lightCoords == null) lightCoords = new List<Vector2Int>();
        if (lightsInPVSCoords == null) lightsInPVSCoords = new List<Vector2Int>();
        if (dynamicMeshes == null) dynamicMeshes = new List<MeshRenderer>();
        if (dynamicMeshesPIDs == null) dynamicMeshesPIDs = new List<PrefabIdentifier>();
        if (dynamicMeshesHMs == null) dynamicMeshesHMs = new List<HealthManager>();
        if (dynamicMeshesTransforms == null) dynamicMeshesTransforms = new List<Transform>();
        if (dynamicMeshesFBs == null) dynamicMeshesFBs = new List<ForceBridge>();
        if (dynamicMeshCoords == null) dynamicMeshCoords = new List<Vector2Int>();
        if (npcAICs == null) npcAICs = new List<AIController>();
        if (npcTransforms == null) npcTransforms = new List<Transform>();
        if (sourceMeshenderers == null) sourceMeshenderers = new List<Meshenderer>();

        // Clear existing lists
        staticMeshesImmutable.Clear();
        staticMeshesImmutablePIDs.Clear();
        staticMeshImmutableCoords.Clear();
        staticImmutableParticleCoords.Clear();
        staticImmutableParticleSystems.Clear();
        staticMeshesSaveable.Clear();
        staticMeshesSaveableHMs.Clear();
        staticMeshesSaveablePIDs.Clear();
        staticMeshSaveableCoords.Clear();
        doors.Clear();
        doorsCoords.Clear();
        lights.Clear();
        lightsInPVS.Clear();
        lightCoords.Clear();
        lightsInPVSCoords.Clear();
        dynamicMeshes.Clear();
        dynamicMeshesPIDs.Clear();
        dynamicMeshesHMs.Clear();
        dynamicMeshesTransforms.Clear();
        dynamicMeshesFBs.Clear();
        dynamicMeshCoords.Clear();
        npcAICs.Clear();
        npcTransforms.Clear();
        sourceMeshenderers.Clear();

        // Reset gridCells
        for (int x = 0; x < WORLDX; x++) {
            for (int y = 0; y < WORLDX; y++) {
                gridCells[x, y].x = x;
                gridCells[x, y].y = y;
                gridCells[x, y].open = false;
                gridCells[x, y].visible = false;
                gridCells[x, y].closedNorth = false;
                gridCells[x, y].closedEast = false;
                gridCells[x, y].closedSouth = false;
                gridCells[x, y].closedWest = false;
                gridCells[x, y].chunkPrefabs.Clear();
                gridCells[x, y].dynamicObjects.Clear();
                Array.Clear(gridCells[x, y].visibleCellsFromHere, 0, WORLDX * WORLDX);
            }
        }
    }

    // ========================================================================
    // Handle Occluders (well, just determining visible cells and their chunks)

    public static Meshenderer GetMeshAndItsRenderer(GameObject go,int constIndex) {
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
        mrr.materialUsual = mr.sharedMaterial;
        if (DynamicCulling.a.overrideWindowsToBlack) {
            if (constIndex == 1 || constIndex == 123 || constIndex == 93) {
                mrr.materialUsual = Const.a.genericMaterials[100]; // Set windows to black so sky is blocked, no pink unless there's a legit leak.
            }
        }
        
        PrefabIdentifier pid = go.GetComponent<PrefabIdentifier>();
        if (pid != null) mrr.constIndex = pid.constIndex;
        else mrr.constIndex = -1; // Child meshes preserved for LOD swap out of level chunks with flat cards.
        
        if (ConsoleEmulator.ConstIndexIsGeometry(constIndex) && constIndex >= 0) {
            if (DynamicCulling.a != null) {
                if (DynamicCulling.a.lodMeshes.Length > 0 && constIndex < DynamicCulling.a.lodMeshes.Length) {
                    if (DynamicCulling.a.lodMeshes[constIndex] != null) {
                        mrr.meshLOD = DynamicCulling.a.lodMeshes[constIndex];
                        mrr.materialLOD = DynamicCulling.a.chunkMaterial;
                    } else {
                        mrr.meshLOD = msh;
                        mrr.materialLOD = mrr.materialUsual;
                    }
                }
            } 
        } else {
            mrr.meshLOD = msh;
            mrr.materialLOD = mrr.materialUsual;
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
            
            cr = new ChunkPrefab();
            cr.x = x;
            cr.y = y;
            cr.constIndex = pid.constIndex;
            cr.go = childGO;
            cr.meshenderers = new List<Meshenderer>();

            // Add top level GameObject's mesh renderer and filter.
            Meshenderer mrr = GetMeshAndItsRenderer(cr.go,pid.constIndex);
            if (mrr != null) cr.meshenderers.Add(mrr);
            
            // Add children GameObjects' mesh renderers and filters.
            Component[] compArray = cr.go.GetComponentsInChildren(
                                                    typeof(MeshFilter),true);
            foreach (MeshFilter mfc in compArray) {
                mrr = GetMeshAndItsRenderer(mfc.gameObject,pid.constIndex);
                if (mrr != null) cr.meshenderers.Add(mrr);
            }

            gridCells[x,y].chunkPrefabs.Add(cr);
            MarkAsFloor maf = cr.go.GetComponent<MarkAsFloor>();
            if (maf != null) gridCells[x,y].floorHeight = maf.floorHeight; // Height of the chunk origin, so 1.28f above the actual floor collider.
            
            #if UNITY_EDITOR
            if (debugHelpers) {
                Note nt = childGO.gameObject.AddComponent<Note>();
                nt.note = "Cullable Geometry Chunk, grid cell: " + x.ToString() + "," + y.ToString();
            }
            #endif
//             gridCells[x,y].open = true;
        }
    }

    void DetermineClosedEdges() {
        // The first indices in Const.a.textures are the world closed edges.
        // Priorities priorities after all.  Gotta figure out if we should draw
        // anything else first before it matters what texture it has.
        Color32[] edgePixels = Const.a.textures[LevelManager.a.currentLevel].GetPixels32();
        Color32 closedData;
        for (int x=0;x<WORLDX;x++) {
            for (int y=0;y<WORLDX;y++) {
                gridCells[x,y].closedNorth = false;
                gridCells[x,y].closedEast = false;
                gridCells[x,y].closedSouth = false;
                gridCells[x,y].closedWest = false;
                 closedData = edgePixels[x + y * WORLDX];
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
                    = gridCells[x,y].closedSouth = gridCells[x,y].closedWest = true;
                }
            }
        }
        
        Color32[] openPixels = Const.a.textures[LevelManager.a.currentLevel + 13].GetPixels32();
        Color32 openData;
        for (int x=0;x<WORLDX;x++) {
            for (int y=0;y<WORLDX;y++) {
                gridCells[x,y].open = false;
                openData = openPixels[x + y * WORLDX];
                if (openData.r > 0 || openData.g > 0 || openData.b > 0) {
                    gridCells[x,y].open = true;
                } else {
                    gridCells[x,y].closedNorth = gridCells[x,y].closedSouth = gridCells[x,y].closedEast = gridCells[x,y].closedWest = true;
                }
            }
        }
        
        Color32[] skyPixels = Const.a.textures[LevelManager.a.currentLevel + 26].GetPixels32();
        Color32 skyData;
        for (int x=0;x<WORLDX;x++) {
            for (int y=0;y<WORLDX;y++) {
                gridCells[x,y].skyVisible = false;
                skyData = skyPixels[x + y * WORLDX];
                if (skyData.r <= 0.5f && skyData.g <= 0.5f && skyData.b > 0.5f) gridCells[x,y].skyVisible = true;
            }
        }
    }

    // ========================================================================
    // Handle Occludees

    private bool AddMeshRenderer(int type, MeshRenderer mr) {
        if (mr == null) return false;

        PrefabIdentifier pid = mr.GetComponent<PrefabIdentifier>();
        if (pid == null) pid = mr.transform.parent.GetComponent<PrefabIdentifier>();
        if (pid == null) pid = mr.transform.parent.parent.GetComponent<PrefabIdentifier>();
        if (pid == null) Debug.LogError("No PrefabIdentifier on mesh for culling gameObject named " + mr.gameObject.name);

        #if UNITY_EDITOR
            Note nt = null;
        #endif
        
        switch(type) {
            case 1:
                dynamicMeshes.Add(mr);
                dynamicMeshCoords.Add(Vector2Int.zero);
                dynamicMeshesPIDs.Add(pid);
                dynamicMeshesHMs.Add(mr.GetComponent<HealthManager>());
                dynamicMeshesFBs.Add(mr.GetComponent<ForceBridge>());
                dynamicMeshesTransforms.Add(mr.transform);
                #if UNITY_EDITOR     
                if (debugHelpers) {
                    nt = mr.gameObject.AddComponent<Note>();
                    nt.note = "Cullable Dynamic Object";
                }
                #endif
                break;
            case 2:
                doors.Add(mr);
                doorsCoords.Add(Vector2Int.zero);
                #if UNITY_EDITOR
                if (debugHelpers) {
                    nt = mr.gameObject.AddComponent<Note>();
                    nt.note = "Cullable Door Object";
                }
                #endif
                break;
            case 3: break; // NPCs done different due to SkinnedMeshRenderer's.
            case 4:
                staticMeshesSaveable.Add(mr);
                staticMeshSaveableCoords.Add(Vector2Int.zero);
                staticMeshesSaveablePIDs.Add(pid);
                staticMeshesSaveableHMs.Add(mr.gameObject.GetComponent<HealthManager>());
                #if UNITY_EDITOR     
                if (debugHelpers) {
                    nt = mr.gameObject.AddComponent<Note>();
                    nt.note = "Cullable Static Saveable Object";
                }
                #endif
                break;
            case 5: break; // Lights done differently due to Light (what, it makes sense).
            default:
//                 if (pid.constIndex == 592 || pid.constIndex == 593) return false; // TODO: handle text_decal and text_decalStopDSS1

                staticMeshesImmutable.Add(mr);
                staticMeshImmutableCoords.Add(Vector2Int.zero);
                staticMeshesImmutablePIDs.Add(pid);
                #if UNITY_EDITOR     
                if (debugHelpers) {
                    nt = mr.gameObject.AddComponent<Note>();
                    nt.note = "Cullable Static Immutable Object";
                }
                #endif
                break;
        }
        
        return true;
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
                #if UNITY_EDITOR     
                if (debugHelpers) {
                    Note nt = aic.gameObject.AddComponent<Note>();
                    nt.note = "Cullable NPC";
                }
                #endif
            }
        } else if (type == 5) { // Lights
            Light lit = null;
            for (int i=0;i<count;i++) {
                lit = ctr.GetChild(i).GetComponent<Light>();
                if (lit == null) continue;

                lights.Add(lit);
                lightCoords.Add(Vector2Int.zero);
                #if UNITY_EDITOR     
                if (debugHelpers) {
                    Note nt = lit.gameObject.AddComponent<Note>();
                    nt.note = "Cullable Light";
                }
                #endif
            }
        } else if (type == 0) { // Static Immutable
            for (int k=0;k<compArray.Length;k++) {
                AddMeshRenderer(type,compArray[k].gameObject.GetComponent<MeshRenderer>());
            }
            
            Transform parent = null;
//             Transform child = null;
            for (int i=0;i<count;i++) {
                parent = ctr.GetChild(i);
//                 AddMeshRenderer(type,parent.GetComponent<MeshRenderer>());
                AddStaticImmutableParticleSystem(parent);
//                 for (int j=0;j<parent.childCount;j++) {
//                     child = parent.GetChild(j);
//                     AddMeshRenderer(type,child.GetComponent<MeshRenderer>());
//                 }
            }
        } else { // Dynamic, Doors, Static Saveable
            for (int k=0;k<compArray.Length;k++) {
                AddMeshRenderer(type,compArray[k].gameObject.GetComponent<MeshRenderer>());
            }
//             Transform parent = null;
//             Transform child = null;
//             for (int i=0;i<count;i++) {
//                 parent = ctr.GetChild(i);
//                 AddMeshRenderer(type,parent.GetComponent<MeshRenderer>());
//                 for (int j=0;j<parent.childCount;j++) {
//                     child = parent.GetChild(j);
//                     AddMeshRenderer(type,child.GetComponent<MeshRenderer>());
//                 }
//             }
        }
    }
    
    private void AddStaticImmutableParticleSystem(Transform tr) {
        PauseParticleSystem pps = tr.GetComponent<PauseParticleSystem>();
        if (pps != null) {
            staticImmutableParticleSystems.Add(pps);
            staticImmutableParticleCoords.Add(PosToCellCoords(tr.position));
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

        Vector3[] nudges = new Vector3[]{
            new Vector3(0.32f, 0f, 0f), new Vector3(-0.32f, 0f, 0f),
            new Vector3(0f, 0f, 0.32f), new Vector3(0f, 0f, -0.32f),          
            new Vector3(0.64f, 0f, 0f), new Vector3(-0.64f, 0f, 0f),
            new Vector3(0f, 0f, 0.64f), new Vector3(0f, 0f, -0.64f),          
        };
        int iter;
        
        #if UNITY_EDITOR     
            Note nt = null;
        #endif
            
        for (int index=0;index<count;index++) {
            switch(type) {
                case 1: dynamicMeshCoords[index]          = PosToCellCoords(dynamicMeshes[index].transform.position);
                        #if UNITY_EDITOR
                        if (debugHelpers) {
                            nt = dynamicMeshes[index].gameObject.GetComponent<Note>();
                            nt.note = "Cullable Dynamic Object, grid cell: " + dynamicMeshCoords[index].x.ToString() + "," + dynamicMeshCoords[index].y.ToString();
                        }
                        #endif
                            
                        if (dynamicMeshCoords[index].x == 0 && dynamicMeshCoords[index].y == 0) {
                            UnityEngine.Debug.Log("dynamic mesh misplaced for " + dynamicMeshes[index].gameObject.name + " at " + dynamicMeshes[index].transform.position.ToString());
                        }
                        break;
                case 2: doorsCoords[index]                = PosToCellCoords(doors[index].transform.position);
                        #if UNITY_EDITOR
                        if (debugHelpers) {
                            nt = doors[index].gameObject.GetComponent<Note>();
                            nt.note = "Cullable Door Object, grid cell: " + doorsCoords[index].x.ToString() + "," + doorsCoords[index].y.ToString();
                        }
                        #endif
                        if (doorsCoords[index].x == 0 && doorsCoords[index].y == 0) {
                            UnityEngine.Debug.Log("door misplaced for " + doors[index].gameObject.name + " at " + doors[index].transform.position.ToString());
                        }
                        break;
                case 3: npcCoords[index]                  = PosToCellCoords(npcTransforms[index].position);
                        #if UNITY_EDITOR
                        if (debugHelpers) {
                            nt = npcTransforms[index].gameObject.GetComponent<Note>();
                            nt.note = "Cullable NPC, grid cell: " + npcCoords[index].x.ToString() + "," + npcCoords[index].y.ToString();
                        }
                        #endif
                        if (npcCoords[index].x == 0 && npcCoords[index].y == 0) {
                            UnityEngine.Debug.Log("npc misplaced for " + npcTransforms[index].gameObject.name + " at " + npcTransforms[index].position.ToString());
                        }
                        break;
                case 4: staticMeshSaveableCoords[index]   = PosToCellCoords(staticMeshesSaveable[index].transform.position);
                        iter = 0;
                        while(!gridCells[staticMeshSaveableCoords[index].x,staticMeshSaveableCoords[index].y].open) {
//                             UnityEngine.Debug.Log("Nudging staticMeshesSaveable " + staticMeshesSaveable[index].gameObject.name);
                            staticMeshSaveableCoords[index] = PosToCellCoords(staticMeshesSaveable[index].transform.position + nudges[iter]);
                            iter++;
                            if (iter > (nudges.Length - 1)) break;
                        }
                        #if UNITY_EDITOR
                        if (debugHelpers) {
                            nt = staticMeshesSaveable[index].gameObject.GetComponent<Note>();
                            nt.note = "Cullable Static Saveable Object, grid cell: " + staticMeshSaveableCoords[index].x.ToString() + "," + staticMeshSaveableCoords[index].y.ToString();
                        }
                        #endif
                        if (staticMeshSaveableCoords[index].x == 0 && staticMeshSaveableCoords[index].y == 0) {
                            UnityEngine.Debug.Log("static mesh savable misplaced for " + staticMeshesSaveable[index].gameObject.name + " at " + staticMeshesSaveable[index].transform.position.ToString());
                        }
                        break;
                case 5: lightCoords[index]                = PosToCellCoords(lights[index].transform.position);
                        #if UNITY_EDITOR
                        if (debugHelpers) {
                            nt = lights[index].gameObject.GetComponent<Note>();
                            nt.note = "Cullable Light, grid cell: " + lightCoords[index].x.ToString() + "," + lightCoords[index].y.ToString();
                        }
                        #endif
                        if (lightCoords[index].x == 0 && lightCoords[index].y == 0) {
                            lights[index].shadows = LightShadows.None;
                        }
                        break;
                default: staticMeshImmutableCoords[index] = PosToCellCoords(staticMeshesImmutable[index].transform.position);
                        iter = 0;
                        while(!gridCells[staticMeshImmutableCoords[index].x,staticMeshImmutableCoords[index].y].open) {
//                             UnityEngine.Debug.Log("Nudging staticMeshImmutableCoords " + staticMeshesImmutable[index].gameObject.name);
                            staticMeshImmutableCoords[index] = PosToCellCoords(staticMeshesImmutable[index].transform.position + nudges[iter]);
                            iter++;
                            if (iter > (nudges.Length - 1)) break;
                        }
                        #if UNITY_EDITOR
                        if (debugHelpers) {
                            nt = staticMeshesImmutable[index].gameObject.GetComponent<Note>();
                            nt.note = "Cullable Static Immutable Object, grid cell: " + staticMeshImmutableCoords[index].x.ToString() + "," + staticMeshImmutableCoords[index].y.ToString();
                        }
                        #endif
                        if (staticMeshImmutableCoords[index].x == 0 && staticMeshImmutableCoords[index].y == 0) {
                            UnityEngine.Debug.Log("static mesh immutable misplaced for " + staticMeshesImmutable[index].gameObject.name + " at " + staticMeshesImmutable[index].transform.position.ToString());
                        }
                        break;
            }
        }
    }
    
    public static void UpdatePubliclyVisibleCameraViewsList() {
        if (camPositions == null) return;
        if (DynamicCulling.a == null) return; // Not awakened yet.
        
        DynamicCulling.a.cameraViews = new CameraView[camPositions.Count];
        for (int i=0;i<camPositions.Count;i++) {
            KeyValuePair<GameObject, Vector3> entry = camPositions.ElementAt(i);
            if (entry.Key == null) continue; // GameObject is null, skip
            CameraView camV = entry.Key.GetComponent<CameraView>();
            DynamicCulling.a.cameraViews[i] = camV;
            if (camV == null) UnityEngine.Debug.LogWarning("Missing CameraView on " + entry.Key.name);
        }
    }

    public static void AddCameraPosition(CameraView cam) {
        if (camPositions == null) camPositions = new Dictionary<GameObject, Vector3>();
        
        if (!camPositions.ContainsKey(cam.gameObject)) {
            camPositions[cam.gameObject] = cam.transform.position;
//             UnityEngine.Debug.Log("Added screen cam " + cam.gameObject.name);
            UpdatePubliclyVisibleCameraViewsList();
        }
    }

    public static void RemoveCameraPosition(CameraView cam) {
        if (camPositions == null) return;
        if (cam == null) { UnityEngine.Debug.LogWarning("Null CameraView passed to RemoveCameraPosition!"); return; }
        
//         UnityEngine.Debug.Log("Removed screen cam " + cam.gameObject.name);
        camPositions.Remove(cam.gameObject);
        UpdatePubliclyVisibleCameraViewsList();
    }
    
    private void SetupDebugImageWorkingVariables() {
        debugTex = new Texture2D(WORLDX,WORLDX);
        pixels = new Color32[WORLDX * WORLDX];
//             openDebugImagePath = Utils.SafePathCombine(
//                 Application.streamingAssetsPath,
//                 "gridcellsopen_" + LevelManager.a.currentLevel.ToString()
//                 + ".png");

        visDebugImagePath = Utils.SafePathCombine(
            Application.streamingAssetsPath,
            "worldcellvis_" + LevelManager.a.currentLevel.ToString()
            + ".png");        
    }

    public void Cull_Init() {
//         UnityEngine.Debug.Log("Cull_Init: Start");
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
            case 0: worldMin =  new Vector3( -38.40f + ( 0.00000f +    3.6000f),0f, -51.20f + (0f + 1f)); break;
            case 1: worldMin =  new Vector3( -76.80f + ( 0.00000f +   25.5600f),0f, -56.32f + (0f + -5.2f)); break;
            case 2: worldMin =  new Vector3( -40.96f + ( 0.00000f +   -2.6000f),0f, -46.08f + (0f + -7.7f)); break;
            case 3: worldMin =  new Vector3( -53.76f + (50.17400f +  -45.1200f),0f, -46.08f + (13.714f + -16.32f)); break;
            case 4: worldMin =  new Vector3(  -7.68f + ( 1.17800f +  -20.4000f),0f, -64.00f + (1.292799f + 11.48f)); break;
            case 5: worldMin =  new Vector3( -35.84f + ( 1.17780f +  -10.1400f),0f, -51.20f + (-1.2417f + -0.0383f)); break;
            case 6: worldMin =  new Vector3( -64.00f + ( 1.29280f +   -0.6728f),0f, -71.68f + (-1.2033f + 3.76f)); break;
            case 7: worldMin =  new Vector3( -58.88f + ( 1.24110f +   -6.7000f),0f, -79.36f + (-1.2544f + 1.16f)); break;
            case 8: worldMin =  new Vector3( -40.96f + (-1.30560f +    1.0800f),0f, -43.52f + (1.2928f + 0.8f)); break;
            case 9: worldMin =  new Vector3( -51.20f + (-1.34390f +    3.6000f),0f, -64f + (-1.1906f + -1.28f)); break;
            case 10: worldMin = new Vector3(-128.00f + (-0.90945f +  107.3700f),0f, -71.68f + (-1.0372f + 35.48f)); break;
            case 11: worldMin = new Vector3( -38.40f + (-1.26720f +   15.0500f),0f,  51.2f + (0.96056f + -77.94f)); break;
            case 12: worldMin = new Vector3( -34.53f + ( 0.00000f +   19.0400f),0f, -123.74f + (0f + 95.8f)); break;
        }

        worldMin.x -= 2.56f; // Add one cell gap around edges
        worldMin.z -= 2.56f;
        
        if (outputDebugImages) SetupDebugImageWorkingVariables();
        PutChunksInCells();
        DetermineClosedEdges();
        for (int y=0;y<WORLDX;y++) {
            for (int x=0;x<WORLDX;x++) {
                playerCellX = x;
                playerCellY = y;
                DetermineVisibleCells(x,y);
                for (int y2=0;y2<WORLDX;y2++) {
                    for (int x2=0;x2<WORLDX;x2++) {
                        gridCells[x,y].visibleCellsFromHere[x2,y2] = gridCells[x2,y2].visible;
                    }
                }
                
                if (LevelManager.a.currentLevel == 10) {
                    if ((x == 15 || x == 16) && y == 23) { // Fix up problem cells at odd angle where ddx doesn't work.
                        gridCells[x,y].visibleCellsFromHere[12,11] = true;
                    }
                }
            }
        }

        FindMeshRenderers(0); // Static Immutable
        FindMeshRenderers(1); // Dynamic
        FindMeshRenderers(2); // Doors
        FindMeshRenderers(3); // NPCs
        FindMeshRenderers(4); // Static Saveable
        FindMeshRenderers(5); // Lights
        UpdatedPlayerCell();
        for (int y=0;y<WORLDX;y++) {
            for (int x=0;x<WORLDX;x++) {
                gridCells[x,y].visible = gridCells[playerCellX,playerCellY].visibleCellsFromHere[x,y]; // Get visible before putting meshes into their cells so we can nudge them a little.
                worldCellsOpen[x,y] = gridCells[x,y].open || gridCells[x,y].visible;
            }
        }
//         DetermineVisibleCells(playerCellX,playerCellY); // Get visible before putting meshes into their cells so we can nudge them a little.
        gridCells[0,0].visible = true;
        PutMeshesInCells(0); // Static Immutable
        PutMeshesInCells(1); // Dynamic
        PutMeshesInCells(2); // Doors
        PutMeshesInCells(3); // NPCs
        PutMeshesInCells(4); // Static Saveable
        PutMeshesInCells(5); // Lights
        CullCore(); // Do first Cull pass, forcing as player moved to new cell.
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2Int PosToCellCoordsChunks(Vector3 pos) {
        int x,y;
        int max = WORLDX - 1; // 63
        x = (int)((pos.x - worldMin.x + 1.28f) / 2.56f);
        if (x > max) x = max;
        else if (x < 0) x = 0;

        y = (int)((pos.z - worldMin.z + 1.28f) / 2.56f);
        if (y > max) y = max;
        else if (y < 0) y = 0;

        return new Vector2Int(x,y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2Int PosToCellCoords(Vector3 pos) {
        int x,y;
        int max = WORLDX - 1; // 63
        x = (int)((pos.x - worldMin.x + 1.28f) / 2.56f);
        if (x > max) x = max;
        else if (x < 0) x = 0;

        y = (int)((pos.z - worldMin.z + 1.28f) / 2.56f);
        if (y > max) y = max;
        else if (y < 0) y = 0;

        return new Vector2Int(x,y);
    }

    void FindPlayerCell() {
        Vector2Int pxy = PosToCellCoords(MouseLookScript.a.transform.position);
        playerCellX = pxy.x;
        playerCellY = pxy.y;
    }

    bool UpdatedPlayerCell() {
        if (forceRecull) { forceRecull = false; return true; }

        int lastX = playerCellX;
        int lastY = playerCellY;
        FindPlayerCell();
        if (playerCellX == lastX && playerCellY == lastY) return false;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool XYPairInBounds(int x, int y) {
        return (x < WORLDX && y < WORLDX && x >= 0 && y >= 0);
    }

    private void DetermineVisibleCells(int startX, int startY) {
        if (!XYPairInBounds(startX,startY)) return;

        for (int x=0;x<WORLDX;x++) {
            for (int y=0;y<WORLDX;y++) {
                gridCells[x,y].visible = false;
                worldCellsOpen[x,y] = gridCells[x,y].open;
            }
        }

        gridCells[startX,startY].visible = true;
        // Cast to the right (East)             [ ][3]
        CastStraightX(startX,startY,1);      // [1][2]
                                             // [ ][3]
        int iter = 0;
        for (int march=startX;march<(WORLDX - 1);march++) {
            iter++;
            if (iter > WORLDX) break;
            
            if (XYPairInBounds(march,startY + 1)) {
                if (gridCells[march,startY + 1].visible) {
                    march = CastStraightX(march,startY + 1,1);  // Above [1]
                }
            }
        }
        
        iter = 0;
        for (int march=startX;march<(WORLDX - 1);march++) {
            iter++;
            if (iter > WORLDX) break;

            if (XYPairInBounds(march,startY - 1)) {
                if (gridCells[march,startY - 1].visible) {
                    march = CastStraightX(march,startY - 1,1);  // Below [1]
                }
            }
        }
        
        // Cast to the left (West)              [3][ ]
        CastStraightX(startX,startY,-1);     // [2][1]
                                             // [3][ ]
        iter = 0;
        for (int march=startX;march>=1;march--) {
            iter++;
            if (iter > WORLDX) break;
            
            if (XYPairInBounds(march,startY + 1)) {
                if (gridCells[march,startY + 1].visible) {
                    march = CastStraightX(march,startY + 1,-1); // Above [1]
                }
            }
        }

        iter = 0;
        for (int march=startX;march>=1;march--) {
            iter++;
            if (iter > WORLDX) break;

            if (XYPairInBounds(march,startY - 1)) {
                if (gridCells[march,startY - 1].visible) {
                    march = CastStraightX(march,startY - 1,-1); // Below [1]
                }
            }
        }

        // Cast down (South)                     [ ][1][ ]
        CastStraightY(startX,startY,-1);      // [3][2][3]
        iter = 0;
        for (int march=startY;march>=1;march--) {
            iter++;
            if (iter > WORLDX) break;

            if (XYPairInBounds(startX + 1,march)) {
                if (gridCells[startX + 1,march].visible) {
                    march = CastStraightY(startX + 1,march,-1);
                }
            }
        }
        
        iter = 0;
        for (int march=startY;march>=1;march--) {
            iter++;
            if (iter > WORLDX) break;

            if (XYPairInBounds(startX - 1,march)) {
                if (gridCells[startX - 1,march].visible) {
                    march = CastStraightY(startX - 1,march,-1);
                }
            }
        }

        // Cast up (North)                     [3][2][3]
        CastStraightY(startX,startY,1);     // [ ][1][ ]
        iter = 0;
        for (int march=startY;march<(WORLDX - 1);march++) {
            iter++;
            if (iter > WORLDX) break;

            if (XYPairInBounds(startX + 1,march)) {
                if (gridCells[startX + 1,march].visible) {
                    march = CastStraightY(startX + 1,march,1);
                }
            }
        }
        
        iter = 0;
        for (int march=startY;march<(WORLDX - 1);march++) {
            iter++;
            if (iter > WORLDX) break;

            if (XYPairInBounds(startX - 1,march)) {
                if (gridCells[startX - 1,march].visible) {
                    march = CastStraightY(startX - 1,march,1);
                }
            }
        }

        CircleFanRays(startX,startY);
        CircleFanRays(startX + 1,startY);
        CircleFanRays(startX + 1,startY + 1);
        CircleFanRays(startX,startY + 1);
        CircleFanRays(startX - 1,startY + 1);
        CircleFanRays(startX - 1,startY);
        CircleFanRays(startX - 1,startY - 1);
        CircleFanRays(startX,startY - 1);
        CircleFanRays(startX + 1,startY - 1);
        
        for (int x=0;x<WORLDX;x++) {
            for (int y=0;y<WORLDX;y++) {
                if (LevelManager.a.currentLevel == 5) {
                    if ((x <= 15 && startX <= 15) || (y <= 9 && startY <= 9)
                        || (x >= 32 && startX >=32)
                        || (y == 31 && startY == 31 && x >= 27 && startX >= 27)
                        || x >= 34) {
                        gridCells[x,y].visible = true;
                    }
                    
                    if (startX <=12 && x == 14 && y == 31 && startY >= 24) gridCells[x,y].visible = true;
                    if (startX <=12 && x == 14 && y == 30 && startY >= 24) gridCells[x,y].visible = true;
                    if (startX <=12 && x == 13 && y == 30 && startY >= 24) gridCells[x,y].visible = true;
                }
            }
        }
    }

    private int CastStraightY(int px, int py, int signy) {
        if (signy > 0 && py >= (WORLDX - 1)) return py; // Nowwhere to step to if right by edge, hence WORLDX - 1 here.
        if (signy < 0 && py <= 0) return py;
        if (!XYPairInBounds(px,py)) return py;
        if (!gridCells[px,py].visible) return py;

        int x = px;
        int y = py + signy;
        bool currentVisible = true;
        for (;Mathf.Abs(y)<WORLDX;y+=signy) { // Up/Down
            currentVisible = false;
            if (XYPairInBounds(x,y - signy) && XYPairInBounds(x,y)) {
                if (gridCells[x,y - signy].visible) {
                    if (signy > 0) {
                        if (gridCells[x,y - 1].closedNorth && gridCells[x,y - 1].open) return y;
                    } else if (signy < 0) {
                        if (gridCells[x,y + 1].closedSouth && gridCells[x,y + 1].open) return y;
                    }

                    gridCells[x,y].visible = worldCellsOpen[x,y];
                    currentVisible = true; // Would be if twas open.
                }
            }

            if (!currentVisible) break; // Hit wall!

            if (XYPairInBounds(x + 1,y)) {
                if (CastRayCellCheck(x,y,x + 1,y) > 0) {
//                     if (gridCells[x + 1,y].open) gridCells[x + 1,y].visible = true;
                    gridCells[x + 1,y].visible = gridCells[x + 1,y].open;
                } else {
                    gridCells[x + 1,y].visible = false;
                }
            }

            if (XYPairInBounds(x - 1,y)) {
                if (CastRayCellCheck(x,y,x - 1,y) > 0) {
//                     if (gridCells[x - 1,y].open) gridCells[x - 1,y].visible = true;
                    gridCells[x - 1,y].visible = gridCells[x - 1,y].open;
                } else {
                    gridCells[x - 1,y].visible = false;
                }
            }
        }
        
        return WORLDX * signy;
    }

    private int CastStraightX(int px, int py, int signx) {
        if (signx > 0 && px >= (WORLDX - 1)) return px; // Nowwhere to step to if right by edge, hence WORLDX - 1 here.
        if (signx < 0 && px <= 0) return px;
        if (!XYPairInBounds(px,py)) return px;
        if (!gridCells[px,py].visible) return px;

        int x = px + signx;
        int y = py;
        bool currentVisible = true;
        for (;Mathf.Abs(x)<WORLDX;x+=signx) { // Right/Left
            currentVisible = false;
            if (XYPairInBounds(x - signx,y) && XYPairInBounds(x,y)) {
                if (gridCells[x - signx,y].visible) {
                    if (signx > 0) {
                        if (gridCells[x - 1,y].closedEast && gridCells[x - 1,y].open) return x;
                    } else if (signx < 0) {
                        if (gridCells[x + 1,y].closedWest && gridCells[x + 1,y].open) return x;
                    }

                    gridCells[x,y].visible = worldCellsOpen[x,y];
                    currentVisible = true; // Would be if twas open.
                }
            }

            if (!currentVisible) break; // Hit wall!

            if (XYPairInBounds(x,y + 1)) {
                if (CastRayCellCheck(x,y,x,y + 1) > 0) {
//                     if (gridCells[x,y + 1].open) gridCells[x,y + 1].visible = true;
                    gridCells[x,y + 1].visible = gridCells[x,y + 1].open;
                } else {
                    gridCells[x,y + 1].visible = false;
                }
            }

            if (XYPairInBounds(x,y - 1)) {
                if (CastRayCellCheck(x,y,x,y - 1) > 0) {
//                     if (gridCells[x,y - 1].open) gridCells[x,y - 1].visible = true;
                    gridCells[x,y - 1].visible = gridCells[x,y - 1].open;
                } else {
                    gridCells[x,y - 1].visible = false;
                }
            }
        }
        
        return WORLDX * signx;
    }

    // CastRay()'s in fan from x0,y0 out to every cell around map perimeter.
    private void CircleFanRays(int x0, int y0) {
        if (!XYPairInBounds(x0,y0)) return;
        if (!gridCells[x0,y0].visible) return;

        int x,y;     
        int max = WORLDX; // Reduce work slightly by not casting towards 
        int min = 0;      // edges but 1 less = [1,63].
        for (x=min;x<max;x++) CastRay(x0,y0,x,min);
        for (x=min;x<max;x++) CastRay(x0,y0,x,max);
        for (y=min;y<max;y++) CastRay(x0,y0,min,y);
        for (y=min;y<max;y++) CastRay(x0,y0,max,y);
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
            if (!XYPairInBounds(x,y)) continue;
            if (!XYPairInBounds(lastX,lastY)) continue;
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

    int CastRayCellCheck(int x, int y, int lastX, int lastY) {
        if (!(lastX == x && lastY == y)) {
            if (XYPairInBounds(lastX,lastY)) {
                if (lastY == y) {
                    if (lastX > x) { // [  x  ][lastX]
                        if (gridCells[lastX,lastY].closedWest) return -1;
                    } else { // Less than x since == x was already checked.
                        if (gridCells[lastX,lastY].closedEast) return -1;
                    }
                }

                if (lastX == x) {
                    if (lastY > y) { // [lastY]
                                     // [  y  ]
                        if (gridCells[lastX,lastY].closedSouth) return -1;
                    } else { // Less than y since == y was already checked.
                        if (gridCells[lastX,lastY].closedNorth) return -1;
                    }
                }

                // Diagonals
                if (lastY != y && lastX != x) {
                    if (lastY > y && lastX > x) { // [Nb][ 1]
                                                  // [ 2][Na]
                        bool neighborClosedWest = false;
                        bool neighborClosedSouth = false;
                        if (XYPairInBounds(lastX,lastY - 1)) neighborClosedWest = gridCells[lastX,lastY - 1].closedWest && gridCells[lastX,lastY - 1].open;
                        if (XYPairInBounds(lastX - 1,lastY)) neighborClosedSouth = gridCells[lastX - 1,lastY].closedSouth && gridCells[lastX - 1,lastY].open;
                        if (gridCells[lastX,lastY].closedSouth && gridCells[lastX,lastY].closedWest) return -1;// Check cell 1 only
                        if (gridCells[lastX,lastY].closedWest && neighborClosedWest) return -1; // Check cell 1 and Neighbor a (Na)
                        if (gridCells[lastX,lastY].closedSouth && neighborClosedSouth) return -1; // Check cell 1 and Neighbor b (Nb)
                        if (neighborClosedWest && neighborClosedSouth) return -1; // Check Neighbor a (Na) and Neighbor b (Nb)
                    } else if (lastY < y && lastX < x) { // [ ][2]
                                                         // [1][ ]return
                        bool neighborClosedEast = false;
                        bool neighborClosedNorth = false;
                        if (XYPairInBounds(lastX,lastY + 1)) neighborClosedEast = gridCells[lastX,lastY + 1].closedEast && gridCells[lastX,lastY + 1].open;
                        if (XYPairInBounds(lastX + 1,lastY)) neighborClosedNorth = gridCells[lastX + 1,lastY].closedNorth && gridCells[lastX + 1,lastY].open;
                        if (gridCells[lastX,lastY].closedNorth && gridCells[lastX,lastY].closedEast) return -1;
                        if (gridCells[lastX,lastY].closedEast && neighborClosedEast) return -1;
                        if (gridCells[lastX,lastY].closedNorth && neighborClosedNorth) return -1;
                        if (neighborClosedEast && neighborClosedNorth) return -1;
                    } else if (lastY > y && lastX < x) { // [1][ ]
                                                         // [ ][2]
                        bool neighborClosedEast = false;
                        bool neighborClosedSouth = false;
                        if (XYPairInBounds(lastX,lastY - 1)) neighborClosedEast = gridCells[lastX,lastY - 1].closedEast && gridCells[lastX,lastY - 1].open;
                        if (XYPairInBounds(lastX + 1,lastY)) neighborClosedSouth = gridCells[lastX + 1,lastY].closedSouth && gridCells[lastX + 1,lastY].open;
                        if (gridCells[lastX,lastY].closedSouth && gridCells[lastX,lastY].closedEast) return -1;
                        if (gridCells[lastX,lastY].closedEast && neighborClosedEast) return -1;
                        if (gridCells[lastX,lastY].closedSouth && neighborClosedSouth) return -1;
                        if (neighborClosedEast && neighborClosedSouth) return -1;
                    } else if (lastY < y && lastX > x) { // [2][ ]
                                                         // [ ][1]
                        bool neighborClosedWest = false;
                        bool neighborClosedNorth = false;
                        if (XYPairInBounds(lastX,lastY + 1)) neighborClosedWest = gridCells[lastX,lastY + 1].closedWest && gridCells[lastX,lastY + 1].open;
                        if (XYPairInBounds(lastX - 1,lastY)) neighborClosedNorth = gridCells[lastX - 1,lastY].closedNorth && gridCells[lastX - 1,lastY].open;
                        if (gridCells[lastX,lastY].closedNorth && gridCells[lastX,lastY].closedWest) return -1;
                        if (gridCells[lastX,lastY].closedWest && neighborClosedWest) return -1;
                        if (gridCells[lastX,lastY].closedNorth && neighborClosedNorth) return -1;
                        if (neighborClosedWest && neighborClosedNorth) return -1;
                    }
                }
            }
        }
        
        if (XYPairInBounds(x,y)) {
//             if (worldCellsOpen[x,y]) gridCells[x,y].visible = true; // Switched from using assignment in order to preserve existing true vis state.
            gridCells[x,y].visible = worldCellsOpen[x,y];
            if (!gridCells[x,y].visible) return -1;
            return 1;
        }

        return 0;
    }
    
    void CameraViewUnculling(int startX, int startY) {
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
            for (int x=0;x<WORLDX;x++) {
                for (int y=0;y<WORLDX;y++) {
                    if (gridCells[pnt.x,pnt.y].visibleCellsFromHere[x,y]) gridCells[x,y].visible = true;
                        if (gridCells[x,y].visible && gridCells[x,y].skyVisible) skyVisibleToPlayer = true;

                }
            }
        }
    }
 
    void ToggleVisibility() {
        gridCells[playerCellX,playerCellY].visible = true; // Guarantee enable.
        ChunkPrefab chp = null;
        float distSqrCheck = lodSqrDist;
        bool pidGood = false;
        if (LevelManager.a.currentLevel > 9) distSqrCheck = 419.4304f; // (8 * 2.56f)^2, lower than normal due to foliage tanking performance
        else if (LevelManager.a.currentLevel == 0 || LevelManager.a.currentLevel == 9) distSqrCheck = 1474.56f; // (15*2.56)^2, lower than normal due to high poly angled ceilings and pipe walls
        for (int x=0;x<WORLDX;x++) {
            for (int y=0;y<WORLDX;y++) {
                float sqrdist = 0f;
                for (int i=0;i<gridCells[x,y].chunkPrefabs.Count;i++) {
                    chp = gridCells[x,y].chunkPrefabs[i];
                    if (chp == null) continue;
                    
                    for (int k=0;k<chp.meshenderers.Count;k++) {
                        if (chp.meshenderers[k].meshRenderer == null) continue;
                        
                        chp.meshenderers[k].meshRenderer.enabled = gridCells[x,y].visible;
                        if (!gridCells[x,y].visible) continue;
                        if (chp.constIndex > 304 || chp.constIndex < 0) continue;

                        pidGood = ConsoleEmulator.ConstIndexIsGeometry(chp.meshenderers[k].constIndex);
                        if (useLODMeshes && pidGood) {
                            sqrdist = (MouseLookScript.a.transform.position - chp.meshenderers[k].meshRenderer.transform.position).sqrMagnitude;
                            chp.meshenderers[k].SetMesh(sqrdist >= distSqrCheck);
                        }
                        
                        if (mergeVisibleMeshes) {
                            sourceMeshenderers.Add(chp.meshenderers[k]);
                        }
                    }
                }
            }
        }
    }

    public void UpdateDynamicMeshes() {
        int count = 0;
        label_iterate_mesh_renderers:
        count = dynamicMeshes.Count;
        for (int i=0;i < count; i++) {
            if (dynamicMeshes[i] == null) {
                dynamicMeshes.RemoveAt(i);
                dynamicMeshesPIDs.RemoveAt(i);
                dynamicMeshCoords.RemoveAt(i);
                dynamicMeshesHMs.RemoveAt(i);
                dynamicMeshesFBs.RemoveAt(i);
                dynamicMeshesTransforms.RemoveAt(i);
                goto label_iterate_mesh_renderers; // Start over
            }
        }
        
        count = dynamicMeshes.Count;
        for (int i=0;i < count; i++) {
            dynamicMeshCoords[i] = PosToCellCoords(dynamicMeshesTransforms[i].position);
        }
    }

    public void UpdateNPCPVS() {
        int count = 0;
        label_iterate_aics:
        count = npcAICs.Count;
        for (int i=0;i < count; i++) {
            if (npcAICs[i] == null) {
                npcAICs.RemoveAt(i);
                npcTransforms.RemoveAt(i);
                npcCoords.RemoveAt(i);
                goto label_iterate_aics; // Start over
            }
        }
        
        count = npcAICs.Count;
        for (int i=0;i < count; i++) {
            npcCoords[i] = PosToCellCoords(npcTransforms[i].position);
        }
    }

     // Avoid NPC doing raycasts when not in player's PVS.  Symmetrical vision.
    public void ToggleNPCPVS() {
        HealthManager hm = null;
        int x,y;
        for (int i=0;i<npcAICs.Count;i++) {
            if (npcAICs[i] == null) continue;
            
            x = npcCoords[i].x;
            y = npcCoords[i].y;
            if (gridCells[x,y].visible || !worldCellsOpen[x,y]
                || npcAICs[i].enemy != null || npcAICs[i].ai_dying
                || npcAICs[i].ai_dead) {
                npcAICs[i].withinPVS = true;
                hm = npcAICs[i].healthManager;
                if (npcAICs[i].visibleMeshVisible) {
                    npcAICs[i].visibleMeshEntity.SetActive(true);
                } else {
                    npcAICs[i].visibleMeshEntity.SetActive(false);
                }
            } else {
                npcAICs[i].withinPVS = false;
                npcAICs[i].visibleMeshEntity.SetActive(false);
            }
        }
    }

    public void ToggleDynamicMeshesVisibility() {
        int x,y;
        for (int i=0;i<dynamicMeshes.Count;i++) {
            if (dynamicMeshes[i] == null) continue;
            
            x = dynamicMeshCoords[i].x;
            y = dynamicMeshCoords[i].y;
            bool inPVS = false;
            if (gridCells[x,y].visible || !worldCellsOpen[x,y]) {
                if (dynamicMeshesPIDs[i].constIndex == 515) { // func_forcebridge
                    if (dynamicMeshesFBs[i] != null) { // The SEGI Emitters are null because the PID GetComponent gets its parent but it has no ForceBridge on it itself.
                        if (dynamicMeshesFBs[i].activated) {
                            inPVS = true;
                        }
                    }
                } else if (dynamicMeshesHMs[i] != null) {
                    if (dynamicMeshesHMs[i].health > 0 || !dynamicMeshesHMs[i].gibOnDeath
                        || dynamicMeshesPIDs[i].constIndex == 279) { // chunk_screen stays on when destroyed
                        
                        inPVS = true;
                    }
                    
                    if (dynamicMeshesHMs[i].health <= 0 && dynamicMeshesHMs[i].deathFX != PoolType.None) inPVS = false;
                } else {
                    inPVS = true;
                }
            }
            
            dynamicMeshes[i].enabled = inPVS;
            if (dynamicMeshes[i].gameObject.layer == 2) { // SEGIEmitter
                GameObject parent = dynamicMeshes[i].transform.parent.gameObject;
                if (parent != null) {
                    PrefabIdentifier pidPar = parent.GetComponent<PrefabIdentifier>();
                    if (pidPar != null) {
                        if (pidPar.constIndex == 515) {
                            ForceBridge fb = parent.GetComponent<ForceBridge>();
                            if (fb != null) { // The SEGI Emitters are null because the PID GetComponent gets its parent but it has no ForceBridge on it itself.
                                if (fb.activated) {
                                    dynamicMeshes[i].enabled = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void ToggleStaticMeshesImmutableVisibility() {
        for (int i=0;i<staticMeshesImmutable.Count;i++) {
            if (staticMeshesImmutable[i] == null) continue;
            
            int x = staticMeshImmutableCoords[i].x;
            int y = staticMeshImmutableCoords[i].y;
            if (gridCells[x,y].visible || (!worldCellsOpen[x,y] && skyVisibleToPlayer)) {
                staticMeshesImmutable[i].enabled = true;
                if (mergeVisibleMeshes) {
                    Meshenderer mrsh = GetMeshAndItsRenderer(staticMeshesImmutable[i].gameObject,-1);
                    if (mrsh != null) sourceMeshenderers.Add(mrsh);
                }
            } else {
                staticMeshesImmutable[i].enabled = false;
            }
        }
    }
    
    public void ToggleStaticImmutableParticlesVisibility() {
        for (int i=0;i<staticImmutableParticleSystems.Count;i++) {
            if (staticImmutableParticleSystems[i] == null) continue;
            
            int x = staticImmutableParticleCoords[i].x;
            int y = staticImmutableParticleCoords[i].y;
            if (gridCells[x,y].visible || !worldCellsOpen[x,y]) {
                staticImmutableParticleSystems[i].gameObject.SetActive(true);//Unhide();
            } else {
                staticImmutableParticleSystems[i].gameObject.SetActive(false);//Hide();
            }
        }
    }

    public void ToggleStaticMeshesSaveableVisibility() {
        int x,y;
        for (int i=0;i<staticMeshesSaveable.Count;i++) {
            if (staticMeshesSaveable[i] == null) continue;
            
            x = staticMeshSaveableCoords[i].x;
            y = staticMeshSaveableCoords[i].y;
            if (gridCells[x,y].visible || !worldCellsOpen[x,y]) {
                if (staticMeshesSaveableHMs[i] != null) {
                    if (staticMeshesSaveableHMs[i].health > 0
                        || !staticMeshesSaveableHMs[i].gibOnDeath
                        || staticMeshesSaveablePIDs[i].constIndex == 279) {
                        
                        staticMeshesSaveable[i].enabled = true;
                        if (mergeVisibleMeshes) {
                            Meshenderer mrsh = GetMeshAndItsRenderer(staticMeshesSaveablePIDs[i].gameObject,staticMeshesSaveablePIDs[i].constIndex);
                            if (mrsh != null) sourceMeshenderers.Add(mrsh);
                        }
                    } else {
                        staticMeshesSaveable[i].enabled = false;
                    }
                } else {
                    staticMeshesSaveable[i].enabled = true;
                    if (mergeVisibleMeshes) {
                        Meshenderer mrsh = GetMeshAndItsRenderer(staticMeshesSaveablePIDs[i].gameObject,staticMeshesSaveablePIDs[i].constIndex);
                        if (mrsh != null) sourceMeshenderers.Add(mrsh);
                    }
                }
            } else {
                staticMeshesSaveable[i].enabled = false;
            }
        }
    }

    public void ToggleDoorsVisibility() {
        int x,y;
        for (int i=0;i<doors.Count;i++) {
            if (doors[i] == null) continue;
            
            x = doorsCoords[i].x;
            y = doorsCoords[i].y;
            if (gridCells[x,y].visible || !worldCellsOpen[x,y]) {
                doors[i].enabled = true;
            } else {
                doors[i].enabled = false;
            }
        }
    }

    public float lightDot = 0f; // Extra padding to account for near objects
    public int lightNearCellCount = 5;
    public void LightsFrustumCull(int startX, int startY) {
        if (!lightCulling || !lightsFrustumCull) return;

        Vector3 dir;
        Camera cam = MouseLookScript.a.playerCamera;
        int dx,dy;
        for (int i = 0; i < lightsInPVS.Count; i++) {
            if (lightsInPVS[i] == null) continue;
            
            dir = lightsInPVS[i].transform.position - cam.transform.position;
            if (Vector3.Dot(dir.normalized,cam.transform.forward) > lightDot) {
                lightsInPVS[i].enabled = true;
                EnableSEGIEmitter(lightsInPVS[i]);
            } else {
                dx = Mathf.Abs(startX - lightsInPVSCoords[i].x);
                dy = Mathf.Abs(startY - lightsInPVSCoords[i].y);
                if (dx < lightNearCellCount && dy < lightNearCellCount) {
                    lightsInPVS[i].enabled = true;
                    EnableSEGIEmitter(lightsInPVS[i]);
                } else {
                    lightsInPVS[i].enabled = false;
                    DisableSEGIEmitter(lightsInPVS[i]);
                }
            }
        }
    }
    
    private void EnableSEGIEmitter(Light lit) {
        if (lit == null) return;
        if (lit.transform.childCount < 1) return;
        
        Transform child = lit.transform.GetChild(0);        
        if (child.gameObject.layer == 2) child.gameObject.SetActive(true);
    }
    private void DisableSEGIEmitter(Light lit) {
        if (lit == null) return;
        if (lit.transform.childCount < 1) return;
        
        Transform child = lit.transform.GetChild(0);
        if (child.gameObject.layer == 2) child.gameObject.SetActive(false);
    }
    
    public void ToggleLightsVisibility() {
        if (!lightCulling) return;
        
        lightsInPVS.Clear();
        lightsInPVSCoords.Clear();
        int x,y;
        bool inPVS = false;
        Camera cam = MouseLookScript.a.playerCamera;
        for (int i=0;i<lights.Count;i++) {
            if (lights[i] == null) continue;
            
            x = lightCoords[i].x;
            y = lightCoords[i].y;
            if (!XYPairInBounds(x,y)) {
                lights[i].enabled = true;
                continue;
            }
            
            int range = (int)Mathf.Floor(lights[i].range / 2.56f);
            int xMin = x - range;
            int xMax = x + range;
            int yMin = y - range;
            int yMax = y + range;
            inPVS = false;
            if (gridCells[x,y].visible || !gridCells[x,y].open) {
                inPVS = true;
                lights[i].enabled = true;
                EnableSEGIEmitter(lights[i]);
                lightsInPVS.Add(lights[i]);
                lightsInPVSCoords.Add(lightCoords[i]);
            } else {
                for (int ix = xMin;ix <= xMax; ix++) {
                    for (int iy = yMin;iy <= yMax; iy++) {
                        if (!XYPairInBounds(ix,iy)) continue;

                        if (gridCells[ix,iy].visible // Player can see it cell in light's range.
                            && gridCells[x,y].visibleCellsFromHere[ix,iy]) { // Light's cell can see the cell in light's range.
                            
                            inPVS = true;
                            goto LightContinue; // Cheap hack to avoid checking any more.  One is enough to justify turning on light.
                        }
                    }
                }
            }

            LightContinue:
            if (inPVS) {
                lights[i].enabled = true;
                lightsInPVS.Add(lights[i]);
                lightsInPVSCoords.Add(lightCoords[i]);
            } else {
                lights[i].enabled = false;
                DisableSEGIEmitter(lights[i]);
            }
            continue;
        }
    }
    
    public void CullCore() {
        if (mergeVisibleMeshes) UncombineMeshes(); // In lieu of the fact that this skyrockets the lighting calculations, not doing!
        if (LevelManager.a != null) {
            if (LevelManager.a.currentLevel >= 13) return;
        }

        skyVisibleToPlayer = false;
        for (int y=0;y<WORLDX;y++) {
            for (int x=0;x<WORLDX;x++) {
                gridCells[x,y].visible = gridCells[playerCellX,playerCellY].visibleCellsFromHere[x,y];
                if (gridCells[x,y].visible && gridCells[x,y].skyVisible) skyVisibleToPlayer = true;

                worldCellsOpen[x,y] = gridCells[x,y].open || gridCells[x,y].visible;
                if (outputDebugImages) {
                    pixels[x + (y * WORLDX)] = gridCells[x,y].open ? Color.white : Color.black;
                }
            }
        }

        gridCells[0,0].visible = true; // Errors default here so draw them anyways.
        gridCells[playerCellX,playerCellY].visible = true;
        CameraViewUnculling(playerCellX,playerCellY);
        ToggleVisibility(); // Update all cells marked as dirty.
        ToggleStaticMeshesImmutableVisibility();
        ToggleStaticImmutableParticlesVisibility();
        ToggleStaticMeshesSaveableVisibility();
        ToggleDoorsVisibility();
        ToggleLightsVisibility();
        UpdateNPCPVS();
        ToggleNPCPVS();
        if (LevelManager.a != null) LevelManager.a.SetSkyVisible(skyVisibleToPlayer);
        if (mergeVisibleMeshes) CombineMeshes(true);

        // Output Debug image of the open
        if (outputDebugImages) {
            if (debugTex == null) SetupDebugImageWorkingVariables();
            Vector2 ply = new Vector2((float)playerCellX,(float)playerCellY);
            for (int x=0;x<WORLDX;x++) {
                for (int y=0;y<WORLDX;y++) {
                    if (!gridCells[x,y].open) {
                        pixels[x + (y * WORLDX)] = Color.black;
                    } else {
                        if (gridCells[x,y].visible) {
                            Vector2 pos = new Vector2((float)x,(float)y);
                            float distToPlayer = Vector2.Distance(pos,ply);
                            float green = Mathf.Max((20f - distToPlayer)/20f,0.2f);
                            float red = gridCells[x,y].closedNorth && gridCells[x,y].visible ? 0.75f : 0f;
                            green += gridCells[x,y].closedEast && gridCells[x,y].visible ? 0.5f : 0f;
                            green += gridCells[x,y].closedWest && gridCells[x,y].visible ? 0.5f : 0f;
                            float blue = gridCells[x,y].closedSouth && gridCells[x,y].visible ? 0.75f : 0f;
                            if (x == playerCellX && y == playerCellY) { red = 1f; blue = 0.8f; green = 0.25f; }
                            pixels[x + (y * WORLDX)] = new Color(red,green,blue,1f);
                        } else {
                            pixels[x + (y * WORLDX)] = Color.white;
                        }
                    }
                }
            }

            debugTex = new Texture2D(WORLDX,WORLDX);
            debugTex.SetPixels32(pixels);
            debugTex.Apply();
            bytes = debugTex.EncodeToPNG();
            File.WriteAllBytes(visDebugImagePath,bytes);
        }
    }

    public void Cull(bool force) {
        int lev = LevelManager.a.currentLevel;
        if (PauseScript.a.MenuActive()) return;
        if (PauseScript.a.Paused()) return;
        if (!cullEnabled || lev == 13) return;

        // Now handle player position updating PVS. Always do UpdatedPlayerCell
        // to set playerCellX and playerCellY.
        if (UpdatedPlayerCell() || force) CullCore();
        if (lightsFrustumCull) LightsFrustumCull(playerCellX,playerCellY);
        

        // Update dynamic meshes after PVS has been updated, if player moved.
        if (dynamicObjectCull) {
            UpdateDynamicMeshes(); // Always check all because any can move.
            ToggleDynamicMeshesVisibility(); // Now turn them on or off.
        }
    }
    
    public GridCell GetPlayerCell() {
        return gridCells[playerCellX,playerCellY];
    }
    
    void OnDestroy() {
        chunkMaterial = null;
        genericMaterial = null;
        chunkAlbedo = null;
        gridCells = null;
        dynamicMeshes = null;
        dynamicMeshesPIDs = null;
        dynamicMeshesHMs = null;
        dynamicMeshesTransforms = null;
        dynamicMeshesFBs = null;
        dynamicMeshCoords = null;
        staticMeshesImmutable = null;
        staticMeshesImmutablePIDs = null;
        staticMeshImmutableCoords = null;
        staticImmutableParticleCoords = null;
        staticImmutableParticleSystems = null;
        staticMeshesSaveable = null;
        staticMeshesSaveableHMs = null;
        staticMeshesSaveablePIDs = null;
        staticMeshSaveableCoords = null;
        lights = null;
        lightsInPVS = null;
        lightCoords = null;
        lightsInPVSCoords = null;
        doors = null;
        doorsCoords = null;
        npcTransforms = null;
        npcAICs = null;
        npcCoords = null;
        lodMeshes = null;
        lodMeshTemplate = null;
        bytes = null;
        pixels = null;
        debugTex = null;
        cameraViews = null;
        worldCellsOpen = null;
        lastCombineResult = null;
        sourceMeshenderers = null;
        if (a == this) {
            camPositions.Clear();
            a = null;
        }
    }
}

public class Meshenderer {
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public Mesh meshUsual;
    public Mesh meshLOD;
    public Material materialUsual;
    public Material materialLOD;
    public int constIndex;
    public void SetMesh(bool useLOD) {
        meshFilter.sharedMesh = useLOD ? meshLOD : meshUsual;
        meshRenderer.sharedMaterial = useLOD ? materialLOD : materialUsual;
        meshRenderer.receiveShadows = true;
        meshRenderer.shadowCastingMode = ShadowCastingMode.TwoSided;
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
    public bool skyVisible;
    public bool closedNorth; // For when chunk configurations are such that
    public bool closedEast;  // the immediately adjacent cell at this edge
    public bool closedSouth; // is not visible, consider edge as closed to
    public bool closedWest;  // be able to further reduce visible cells.
    public float floorHeight;
    public bool[,] visibleCellsFromHere;
    public List<ChunkPrefab> chunkPrefabs;
    public List<DynamicObject> dynamicObjects;
}
