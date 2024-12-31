using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;

public class VoxelLightingManager : MonoBehaviour {
    public MeshRenderer mr;
    private Stopwatch bakeTimer;
    public Texture2DArray chunkAlbedo;
    public GameObject[] chunksToVoxelize;
    public int width = 4;
    public int voxelRenderResolution = 16;
    public int averageDivisor = 64;
    public int debugVoxelIndex = 130;
    public float lightVolumeMultiplier = 2f;
    public float voxelFOV = 150f;
    public float voxelNearZ = 0.02f;
    public float voxelFarZ = 70f;
    public float voxelVolume = 2f;
    public float voxelVolumeFOVFac = 12f;
    public List<Loxel> Voxels;
    private Dictionary<Vector3Int, Loxel> VoxelMap1 = new Dictionary<Vector3Int, Loxel>();
    private Dictionary<Vector3Int, Loxel> VoxelMap2 = new Dictionary<Vector3Int, Loxel>();
    private Dictionary<Vector3Int, Loxel> VoxelMap3 = new Dictionary<Vector3Int, Loxel>(); // Outside corners of 3 overlapping chunks can have 3 voxels in same position.
    public Light[] lights;
    public bool ambientPass1 = true;
    public bool ambientPass2 = true;
    public bool ambientPass3 = true;
    private int iterMax;
    private int lightIndex = 0;
    private float voxelRenderResolutionHalf;
    private float maxDot;
    private float voxelSize;
    private float voxelSizeSquared;
    private int voxelWorldMax;
    private Texture2D debugTex;
    private Vector3[,] colorBuffer;
    private Material matDebug;

    // Light Voxel
    public class Loxel {
        public Vector3 globalPos;
        public Vector3 normal;
        public Vector3 up;
        public Vector3 right;
        public Color diffuse;
        public Color directLighting;
        public Color ambientPass1;
        public Color ambientPass2;
        public Color ambientPass3;
        public GameObject debugCube;
        public int chunkObjectIndex;
        public int chunkIndex;
        public int voxelIndex;
        public float spotAngle;
        public float range;
    }

    void Start() {
        voxelRenderResolutionHalf = voxelRenderResolution / 2f;
        colorBuffer = new Vector3[voxelRenderResolution,voxelRenderResolution];
        float halfAngRadians = Mathf.Deg2Rad * (voxelFOV * 0.5f);
        maxDot = Mathf.Cos(halfAngRadians);
        voxelSize = 2.56f/width;
        voxelSizeSquared = voxelSize * voxelSize;
        voxelWorldMax = 64 * width;
        debugTex = new Texture2D(voxelRenderResolution,voxelRenderResolution);
        debugTex.filterMode = FilterMode.Point;
        iterMax = width * 4; // After this many chunks, stop checking so precisely to prevent rays slipping through gaps.
        CreateVoxels();
        for (int i=0;i<lights.Length;i++) {
            Loxel lit = new Loxel();
            lit.debugCube = null;//MakeDebugCube(lights[i].transform,0f,0f,0f,-1,Voxels.Count);
            lit.globalPos = lights[i].transform.position;
            lit.normal = Vector3.zero;
            lit.diffuse = lights[i].color;
            lit.directLighting = new Color(lights[i].intensity,lights[i].intensity,lights[i].intensity,1f);
            lit.ambientPass1 = lit.ambientPass2 = lit.ambientPass3 = Color.black;
            //SetDebugCubeColor(lit.debugCube,lights[i].color);
            lit.chunkIndex = -1;
            lit.chunkObjectIndex = -1;
            lit.voxelIndex = Voxels.Count;
            lit.up = Vector3.zero;
            lit.right = Vector3.zero;
            lit.spotAngle = lights[i].spotAngle;
            lit.range = lights[i].range;
            Voxels.Add(lit); // Add light as a voxel
            AddVoxel(lit);
            lightIndex = Voxels.Count - 1;
        }
        
        UnityEngine.Debug.Log("Total voxels: " + Voxels.Count.ToString());
        bakeTimer = new Stopwatch();
        bakeTimer.Start();
        DirectLightVoxels();
//         if (ambientPass1) AmbientPass(1);
//         if (ambientPass2) AmbientPass(2);
//         if (ambientPass3) AmbientPass(3);
        bakeTimer.Stop();
        UnityEngine.Debug.Log("Lighted voxels in: " + bakeTimer.Elapsed.ToString());
    }
    
    private Loxel TraceToGetVoxel(Vector3Int start, Vector3Int end, Vector3 raydir) {
        Loxel vox = null;
        int[] steps = new int[] { Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y), Mathf.Abs(end.z - start.z) };
        int[] signs = new int[] { 
            end.x > start.x ? 1 : -1, 
            end.y > start.y ? 1 : -1, 
            end.z > start.z ? 1 : -1 
        };

        // Find the axis with the greatest movement
        int dominantAxis = 0;
        for (int i = 1; i < 3; i++) if (steps[i] > steps[dominantAxis]) dominantAxis = i;

        int[] p = new int[2]; // Error accumulators for the other two axes
        for (int i = 0, j = 0; i < 3; i++) {
            if (i != dominantAxis) {
                p[j++] = 2 * steps[i] - steps[dominantAxis];
            }
        }

        int[] pos = new int[] { start.x, start.y, start.z };
        int iterations = 0; // To count iterations
        while (pos[dominantAxis] != end[dominantAxis]) {
            pos[dominantAxis] += signs[dominantAxis];
            for (int i = 0, j = 0; i < 3; i++) {
                if (i != dominantAxis) {
                    if (p[j] >= 0) {
                        pos[i] += signs[i];
                        p[j] -= 2 * steps[dominantAxis];
                    }
                    p[j] += 2 * steps[i];
                    j++;
                }
            }

            Vector3Int spot = new Vector3Int(pos[0], pos[1], pos[2]);
            
            // Check the current spot
            vox = GetVoxel(spot, raydir);
            if (vox != null) return vox;

            // Only check neighbors after the first two iterations
            iterations++;
            if (iterations < iterMax && steps[(dominantAxis + 1) % 3] != 0 && steps[(dominantAxis + 2) % 3] != 0) { // Moving diagonally in two dimensions
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        Vector3Int neighbor = new Vector3Int(
                            spot.x + (x * (dominantAxis == 0 ? 0 : 1)),
                            spot.y + (y * (dominantAxis == 1 ? 0 : 1)),
                            spot.z + ((x + y == 0 ? 0 : 1) * (dominantAxis == 2 ? 0 : 1))
                        );
                        if (neighbor != spot) {
                            vox = GetVoxel(neighbor, raydir);
                            if (vox != null) return vox;
                        }
                    }
                }
            }
        }

        return vox;
    }

    private void DirectLightVoxels() {
        Color lighting, final;
        lighting = final = Color.black;
        Vector3Int rayend;
        Vector3Int start;
        Vector3 startPos;
        Vector3 rayDirection;
        Vector3 rayendF;
        Vector3 endPos;
        Loxel vox = null;
        for (int i=0;i<Voxels.Count;i++) {
            if (Voxels[i].chunkObjectIndex < 0) continue; // Don't light lights
            //if (i != debugVoxelIndex) continue;
            
            lighting = Color.black;
            startPos = Voxels[i].globalPos + (Voxels[i].normal * voxelSize);
            start = GetVec3IntSpot(startPos);
            float angDeg = 180f/(voxelRenderResolution + 1); // Angle distance between rays, or between end rays and perpendicular vector to Voxels[i].normal
            float offset = (-90f) + angDeg;
            for (int y=0;y<voxelRenderResolution;y++) {
                Vector3 rowDirection = Quaternion.AngleAxis((-angDeg * y) - offset, Voxels[i].up) * Voxels[i].normal;
                for (int x=0;x<voxelRenderResolution;x++) {
                    rayDirection = Quaternion.AngleAxis((-angDeg * x) - offset, Voxels[i].right) * rowDirection;
                    rayendF = rayDirection.normalized;
                    endPos = Voxels[i].globalPos + rayendF * 20f;
                    rayend = GetVec3IntSpot(endPos);
                    vox = TraceToGetVoxel(start,rayend,(endPos - startPos).normalized);
                    if (vox == null) colorBuffer[x,y] = Vector3.zero;
                    else {
                        colorBuffer[x,y].x = vox.directLighting.r;
                        colorBuffer[x,y].y = vox.directLighting.g;
                        colorBuffer[x,y].z = vox.directLighting.b;
                    }

                    if (i == debugVoxelIndex) debugTex.SetPixel(x,y,new Color(colorBuffer[x,y].x,colorBuffer[x,y].y,colorBuffer[x,y].z,1f),0);
                }
            }

            if (i == debugVoxelIndex) {
                UnityEngine.Debug.Log("Painting debugTex for i " + debugVoxelIndex.ToString());
                debugTex.Apply();
                matDebug = new Material(Shader.Find("Unlit/Texture"));
                matDebug.mainTexture = debugTex;
                mr.material = matDebug;
            }

            for (int y=0;y<voxelRenderResolution;y++) {
                for (int x=0;x<voxelRenderResolution;x++) {
                    lighting.r += colorBuffer[x,y].x;
                    lighting.g += colorBuffer[x,y].y;
                    lighting.b += colorBuffer[x,y].z;
                }
            }

            // Get average color of resultant 16x16 render
            int numPixels = averageDivisor;
            lighting.r /= numPixels; // Get the average visible color
            lighting.g /= numPixels; // Get the average visible color
            lighting.b /= numPixels; // Get the average visible color
            lighting.r = Mathf.Min(1f,lighting.r);
            lighting.g = Mathf.Min(1f,lighting.g);
            lighting.b = Mathf.Min(1f,lighting.b);
            final.r = lighting.r;// * Voxels[i].diffuse.r;
            final.b = lighting.g;// * Voxels[i].diffuse.g;
            final.g = lighting.b;// * Voxels[i].diffuse.b;
            if (Voxels[i].chunkObjectIndex >= 0) SetDebugCubeColor(Voxels[i].debugCube,final);
            Voxels[i].directLighting = final; // Apply final ambient lighting.
        }

        for (int i=0;i<Voxels.Count;i++) {
            if (Voxels[i].chunkObjectIndex >= 0) continue; // Not a light

            Voxels[i].directLighting = Color.black; // Exclude lights from next pass.
        }
    }
    
    private void SetDebugCubeColor(GameObject cube, Color col) {
        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = col;
        cube.GetComponent<Renderer>().material = mat;
    }
    
    private void CreateVoxels() {
        Voxels = new List<Loxel>();
        Transform tr;
        Color selected;
        Vector3 sumColor;
        for (int c=0;c<chunksToVoxelize.Length;c++) {
            tr = chunksToVoxelize[c].transform;
            Mesh msh = chunksToVoxelize[c].GetComponent<MeshFilter>().sharedMesh;
            float red = msh.colors[0].r;
            int chunkIndex = (int)(red * 255.0f);
            Color[] textureCols = chunkAlbedo.GetPixels(chunkIndex,0);
            int numPixelsInVoxel = (int)(128f / (float)width); // E.g. if width is 8, numPixelsInVoxel = 128 / 8 = 16;
            sumColor = Vector3.zero;
            Color[] voxelColors = new Color[width * width];
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < width; j++) {
                    sumColor = Vector3.zero;
                    int startX = i * numPixelsInVoxel;
                    int startY = j * numPixelsInVoxel;

                    for (int pixX = startX; pixX < startX + numPixelsInVoxel; pixX++) {
                        for (int pixY = startY; pixY < startY + numPixelsInVoxel; pixY++) {
                            selected = textureCols[pixY * 128 + (127 - pixX)];
                            sumColor.x += selected.r;
                            sumColor.y += selected.g;
                            sumColor.z += selected.b;
                        }
                    }

                    int totalPixels = numPixelsInVoxel * numPixelsInVoxel;
                    Color vol = new Color(
                        sumColor.x / totalPixels, // Do the average
                        sumColor.y / totalPixels, // Do the average
                        sumColor.z / totalPixels, // Do the average
                        1f
                    );

                    voxelColors[i * width + j] = vol;
                }
            }

            for (int x=0;x<width;x++) {
                for (int y=0;y<width;y++) {
                    float offsetX = (x * voxelSize) - 1.28f + (voxelSize * 0.5f);
                    float offsetY = 1.28f + (voxelSize * 0.5f);
                    float offsetZ = (y * voxelSize) - 1.28f + (voxelSize * 0.5f);
                    GameObject cube = MakeDebugCube(tr,offsetX,offsetY,offsetZ,c,Voxels.Count);
                    Loxel lox = new Loxel();
                    lox.globalPos = cube.transform.position;
                    lox.normal = -tr.up; // Green axis, card faces down on rotation 0,0,0
                    lox.diffuse = voxelColors[x * width + y];
                    lox.directLighting = lox.ambientPass1 = lox.ambientPass2 = lox.ambientPass3 = Color.black;
                    lox.debugCube = cube;
                    lox.chunkObjectIndex = c;
                    lox.chunkIndex = chunkIndex;
                    lox.voxelIndex = Voxels.Count;
                    lox.up = tr.right; // Red axis
                    lox.right = -tr.forward; // Blue axis, flipped due to card facing down in rotation 0,0,0
                    lox.spotAngle = 0f;
                    lox.range = 0f;
                    Voxels.Add(lox);
                    AddVoxel(lox);
                }
            }
        }
    }
    
    public bool CheckForCellAtLocation(Vector3Int pos) {
        return    VoxelMap1.ContainsKey(pos)
               || VoxelMap2.ContainsKey(pos)
               || VoxelMap3.ContainsKey(pos);
    }

    private Vector3Int GetVec3IntSpot(Vector3 pos) {
        return new Vector3Int(
            Mathf.RoundToInt(pos.x / voxelSize),
            Mathf.RoundToInt(pos.y / voxelSize),
            Mathf.RoundToInt(pos.z / voxelSize)
        );
    }
    
    private Vector3 GetVec3Spot(Vector3Int posInt) {
        return new Vector3(
            (float)posInt.x * voxelSize,
            (float)posInt.y * voxelSize,
            (float)posInt.z * voxelSize
        );
    }

    // Add to 1 first, then 2, then 3.  VoxelMap2 voxels _always_ have a mate
    // in VoxelMap1, VoxelMap3 voxels _always_ have a mate in both VoxelMap1 & 2.
    public void AddVoxel(Loxel voxel) {
        Vector3Int posInt = GetVec3IntSpot(voxel.globalPos);
        //UnityEngine.Debug.Log($"Voxel at {voxel.globalPos} -> Grid position: {posInt}");
        if (!VoxelMap1.ContainsKey(posInt)) VoxelMap1[posInt] = voxel;
        else if (!VoxelMap2.ContainsKey(posInt)) VoxelMap2[posInt] = voxel;
        else if (!VoxelMap3.ContainsKey(posInt)) VoxelMap3[posInt] = voxel;
        //else UnityEngine.Debug.Log("ERROR: Could not add voxel at " + voxel.globalPos.ToString() + "(" + posInt.ToString() + ")");
    }

    public Loxel GetVoxel(Vector3Int position, Vector3 rayDir) {
        List<Loxel> voxels = new List<Loxel>();
        if (VoxelMap1.TryGetValue(position, out Loxel voxel1)) voxels.Add(voxel1);
        if (VoxelMap2.TryGetValue(position, out Loxel voxel2)) voxels.Add(voxel2);
        if (VoxelMap3.TryGetValue(position, out Loxel voxel3)) voxels.Add(voxel3);
        if (voxels.Count == 0) return null; // No voxels at this position.

        Loxel bestVoxel = null;
        float bestDot = float.MinValue;
        for (int i = 0; i < voxels.Count; i++) {
            float dot = Vector3.Dot(rayDir, voxels[i].normal);
            if ((dot <= 0 && dot > bestDot) || voxels[i].chunkObjectIndex < 0) {
                // Only consider voxels whose normals face the ray and are closer to alignment
                bestDot = dot;
                bestVoxel = voxels[i];
            }
        }

        return bestVoxel; // If bestVoxel is null, no valid (front-facing) voxel was found
    }
    
    // This actually sets the voxel's grid position as well.
    private GameObject MakeDebugCube(Transform parent,float offsetX, float offsetY, float offsetZ, int chunkObjectIndex, int voxIndex) {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = parent; // Put on parent
        cube.transform.localScale = new Vector3(voxelSize,voxelSize,voxelSize);
        cube.transform.localPosition = new Vector3(offsetX,offsetY,offsetZ); // Apply offset
        cube.transform.parent = null; // Make it separate from parent to get true position
        Vector3 posInt = cube.transform.position;
        posInt.x = Mathf.Floor(posInt.x / voxelSize) * voxelSize;
        posInt.y = Mathf.Floor(posInt.y / voxelSize) * voxelSize;
        posInt.z = Mathf.Floor(posInt.z / voxelSize) * voxelSize;
        cube.transform.position = posInt; // Take from float position and put into grid position
        Note nt = cube.AddComponent<Note>();
        nt.note = "Chunk index: " + chunkObjectIndex.ToString() + ", Voxel index: " + voxIndex.ToString();
        return cube;
    }
    
    void OnDestroy() {
        // Clear the List
        if (Voxels != null) {
            Voxels.Clear();
            Voxels = null; // This step is optional, but can help with garbage collection
        }

        // Clear the Dictionaries
        if (VoxelMap1 != null) {
            VoxelMap1.Clear();
            VoxelMap1 = null;
        }
        if (VoxelMap2 != null) {
            VoxelMap2.Clear();
            VoxelMap2 = null;
        }
        if (VoxelMap3 != null) {
            VoxelMap3.Clear();
            VoxelMap3 = null;
        }
    }
}
