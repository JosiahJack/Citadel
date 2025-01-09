using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class CitadelLightMapper : MonoBehaviour {
    public MeshRenderer[] targetRenderers; // Array of MeshRenderers to apply lightmaps to
    public Mesh[] meshes; // Each kind of mesh
    public Light[] lights;
    private LightmapData[] newLightmaps;
    private bool bakeDone = false;
    public Texture2D[] results;
    private float lightFac = 0.2f;
    private float lightPow = 1.2f;
    private static uint state = 0;

    private NativeArray<Color> bakedTextureColors;
//     private NativeArray<Vector3> cardPositions;
//     private NativeArray<Vector3> cardForwards;
//     private NativeArray<Vector3> cardUps;
//     private NativeArray<Vector3> cardRights;
    private NativeArray<Vector3> lightPositions;
    private NativeArray<float> lightRanges;
    private NativeArray<Color> lightColors;
    private NativeArray<float> lightIntensities;
    private NativeArray<Triangle> triangles;

    private JobHandle currentJobHandle;
    private Stopwatch bakeTimer;

    public static CitadelLightMapper a;

    void Start() {
        a = this;

        // Create new buffers
//         cardPositions = new NativeArray<Vector3>(targetRenderers.Length, Allocator.Persistent);
//         cardForwards = new NativeArray<Vector3>(targetRenderers.Length, Allocator.Persistent);
//         cardUps = new NativeArray<Vector3>(targetRenderers.Length, Allocator.Persistent);
//         cardRights = new NativeArray<Vector3>(targetRenderers.Length, Allocator.Persistent);
        bakedTextureColors = new NativeArray<Color>(targetRenderers.Length * 32 * 32, Allocator.Persistent);
        lightPositions = new NativeArray<Vector3>(lights.Length, Allocator.Persistent);
        lightRanges = new NativeArray<float>(lights.Length, Allocator.Persistent);
        lightColors = new NativeArray<Color>(lights.Length, Allocator.Persistent);
        lightIntensities = new NativeArray<float>(lights.Length, Allocator.Persistent);
        results = new Texture2D[targetRenderers.Length];
        List<Triangle> gotTris = new List<Triangle>();
        Mesh msh;
        PrefabIdentifier pid;
        Matrix4x4 localToWorld;
        Vector3[] vertices;
        Vector3[] normals;
        Color[] colors;
        Vector2[] uvs;
        Vector4[] tangents;
        int[] tris;
        Vector3 posA,posB,posC,normA,normB,normC;

        // Populate buffers
        for (int i=0;i<targetRenderers.Length;i++) {
//             cardPositions[i] = targetRenderers[i].transform.position;
//             cardForwards[i] = targetRenderers[i].transform.forward;
//             cardUps[i] = targetRenderers[i].transform.up;
//             cardRights[i] = targetRenderers[i].transform.right;
            pid = targetRenderers[i].gameObject.GetComponent<PrefabIdentifier>();
            if (pid == null) continue;

            msh = meshes[pid.constIndex];

            // Get mesh data
            vertices = msh.vertices;
            normals = msh.normals;
            colors = msh.colors;
            uvs = msh.uv;
            tangents = msh.tangents;
            tris = msh.triangles;

            // Transform data into world space and create Triangle structs
            localToWorld = targetRenderers[i].transform.localToWorldMatrix;

            for (int j = 0; j < tris.Length; j += 3) {
                // Get indices of the vertices of the triangle
                int idxA = tris[j];
                int idxB = tris[j + 1];
                int idxC = tris[j + 2];

                // Transform positions to world space
                posA = localToWorld.MultiplyPoint3x4(vertices[idxA]);
                posB = localToWorld.MultiplyPoint3x4(vertices[idxB]);
                posC = localToWorld.MultiplyPoint3x4(vertices[idxC]);

                // Transform normals to world space
                normA = localToWorld.MultiplyVector(normals[idxA]);
                normB = localToWorld.MultiplyVector(normals[idxB]);
                normC = localToWorld.MultiplyVector(normals[idxC]);

                // Create Triangle struct
                Triangle tri = new Triangle {
                    posA = posA,
                    posB = posB,
                    posC = posC,
                    normA = normA.normalized,
                    normB = normB.normalized,
                    normC = normC.normalized,
                    color = colors.Length > 0 ? colors[idxA] : Color.white,
                    uvA = uvs.Length > 0 ? uvs[idxA] : Vector2.zero,
                    uvB = uvs.Length > 0 ? uvs[idxB] : Vector2.zero,
                    uvC = uvs.Length > 0 ? uvs[idxC] : Vector2.zero,
                    tanA = tangents.Length > 0 ? tangents[idxA] : Vector4.zero,
                    tanB = tangents.Length > 0 ? tangents[idxB] : Vector4.zero,
                    tanC = tangents.Length > 0 ? tangents[idxC] : Vector4.zero,
                    meshRendererIndex = i
                };

                // Add to list
                gotTris.Add(tri);
            }
        }

        triangles = new NativeArray<Triangle>(gotTris.Count, Allocator.Persistent);
        for (int i=0;i<triangles.Length;i++) triangles[i] = gotTris[i];
        for (int i=0;i<(targetRenderers.Length * 32 * 32);i++) bakedTextureColors[i] = Color.black;
        for (int i=0;i<lights.Length;i++) {
            lightPositions[i] = lights[i].transform.position;
            lightRanges[i] = lights[i].range;
            lightColors[i] = lights[i].color;
            lightIntensities[i] = lights[i].intensity;
        }

        LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        newLightmaps = new LightmapData[targetRenderers.Length];

        bakeTimer = new Stopwatch();
        BakeLightJob bakeJob = new BakeLightJob {
            lightPositions = lightPositions,
            lightRanges = lightRanges,
//             cardPositions = cardPositions,
//             cardForwards = cardForwards,
//             cardUps = cardUps,
//             cardRights = cardRights,
            triangles = triangles,
            bakedTextureColors = bakedTextureColors,
            lightColors = lightColors,
            lightIntensities = lightIntensities
        };

        bakeTimer.Start();
        currentJobHandle = bakeJob.Schedule(lightPositions.Length,Mathf.Max(lightPositions.Length,64));
    }

    void Update() {
        if (!bakeDone) {
            if (currentJobHandle.IsCompleted) {
                currentJobHandle.Complete();
                bakeTimer.Stop();
                UnityEngine.Debug.Log("Parallel bake completed in: " + bakeTimer.Elapsed.ToString());
                bakeTimer.Restart();
                ApplyFinishedJobResults();
                bakeDone = true;
                bakeTimer.Stop();
                UnityEngine.Debug.Log("Applied bake in: " + bakeTimer.Elapsed.ToString());
            }
        }
    }

    private void ApplyUniqueLightmap(int index) {
        Texture2D tex = new Texture2D(32,32);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Trilinear;
        Color[] cols = new Color[32 * 32];
        for (int i=0;i<cols.Length;i++) cols[i] = bakedTextureColors[(index * 32 * 32) + i];
        tex.SetPixels(cols);
        tex.Apply();
        results[index] = tex;
        newLightmaps[index] = new LightmapData();
        newLightmaps[index].lightmapColor = tex;
        targetRenderers[index].lightmapIndex = index;
        targetRenderers[index].lightmapScaleOffset = new Vector4(1, 1, 0, 0); // Full coverage
    }

    private void ApplyFinishedJobResults() {
        for (int k=0;k<targetRenderers.Length;k++) ApplyUniqueLightmap(k);
        for (int k=0;k<lights.Length;k++) {
            lights[k].cullingMask &= ~(1 << 9); // Remove Geometry layer so light doesn't affect baked meshRenderers.
        }

        LightmapSettings.lightmaps = newLightmaps; // Finally put actual lightmaps into use.
    }

    private struct BakeLightJob : IJobParallelFor {
        [ReadOnly] public NativeArray<Vector3> lightPositions;
        [ReadOnly] public NativeArray<float> lightRanges;
        [ReadOnly] public NativeArray<Color> lightColors;
        [ReadOnly] public NativeArray<float> lightIntensities;
//         [ReadOnly] public NativeArray<Vector3> cardPositions;
//         [ReadOnly] public NativeArray<Vector3> cardForwards;
//         [ReadOnly] public NativeArray<Vector3> cardUps;
//         [ReadOnly] public NativeArray<Vector3> cardRights;
        [ReadOnly] public NativeArray<Triangle> triangles;
        [NativeDisableParallelForRestriction] public NativeArray<Color> bakedTextureColors;

        public void Execute(int index) {
//             if (index > cardPositions.Length) return; // Nothing more to do
            if (index > lightPositions.Length) return; // Nothing more to do

//             int baseIndex = index * 32 * 32;
            float atten = 0f;
//             float distanceSquared = 0f;
//             int colorsIndex = 0;
//             bool breakout = false;
            Color pencil;
//             float lambert;
//             Vector3 dir;
            int numrays = 10;
            float DivergeStrength = 1f;
            for (int lightIndex=0;lightIndex<lightPositions.Length;lightIndex++) {
                TriangleHitInfo triHitInfo;
                for (int tridex=0;tridex<triangles.Length;tridex++) {
                    for (int i=0;i<numrays;i++) {
                        Vector2 jitter = RandomPointInCircle() * DivergeStrength / 31f;
                        Vector3 jitteredFocusPoint = lightPositions[lightIndex] + Vector3.right * jitter.x + Vector3.up * jitter.y;
                        Vector3 rayDir = Vector3.Normalize(jitteredFocusPoint - lightPositions[lightIndex]);
                        triHitInfo = RayTriangle(lightPositions[lightIndex],rayDir,triangles[tridex]);
                        atten = GetLighting(lightPositions[lightIndex],triHitInfo.hitPoint,triangles[tridex].normA,lightRanges[lightIndex],lightIntensities[lightIndex]);
                        pencil = new Color(atten, atten, atten,1f);
                        bakedTextureColors[triangles[tridex].meshRendererIndex] += pencil;
                    }
                }
                // Old per texel method
//                 breakout = false;
//                 for (int y=0;y<32;y++) {
//                     for (int x=0;x<32;x++) {
//                         hitPos = GetTexelWorldPosition(cardPositions[index],x,y,cardForwards[index],cardRights[index],cardUps[index]);
//                         atten = GetLighting(lightPositions[lightIndex],hitPos,-cardUps[index],lightRanges[lightIndex],lightIntensities[lightIndex]);
//                         dir = lightPositions[lightIndex] - hitPos;
//                         distanceSquared = dir.sqrMagnitude;
//                         colorsIndex = (index * 32 * 32) + ((y * 32) + x);
//                         if (colorsIndex >= bakedTextureColors.Length) { breakout = true; break; }
//                         if (colorsIndex >= bakedTextureColors.Length) {
//                             UnityEngine.Debug.LogError($"Out of bounds: colorsIndex={colorsIndex}, bakedTextureColors.Length={bakedTextureColors.Length}");
//                             breakout = true;
//                             break;
//                         }
//
//                         Triangle tri;
//                         TriangleHitInfo triHitInfo;
//                         for (int i = 0; i < cardPositions.Length; i++) {
//                             if (i == index) continue;
//
//                             // Check first tri of the card
//                             tri.posA = GetTexelWorldPosition(cardPositions[i],0,31,cardForwards[i],cardRights[i],cardUps[i]);
//                             tri.posB = GetTexelWorldPosition(cardPositions[i],0,0,cardForwards[i],cardRights[i],cardUps[i]);
//                             tri.posC = GetTexelWorldPosition(cardPositions[i],31,0,cardForwards[i],cardRights[i],cardUps[i]);
//                             triHitInfo = RayTriangle(hitPos,dir,tri);
//                             if (triHitInfo.didHit && (lightPositions[lightIndex] - triHitInfo.hitPoint).sqrMagnitude < distanceSquared) {
//                                 atten = 0f;
//                                 break;
//                             }
//
//                             // Check 2nd tri of the card
//                             tri.posA = GetTexelWorldPosition(cardPositions[i],0,31,cardForwards[i],cardRights[i],cardUps[i]);
//                             tri.posB = GetTexelWorldPosition(cardPositions[i],31,0,cardForwards[i],cardRights[i],cardUps[i]);
//                             tri.posC = GetTexelWorldPosition(cardPositions[i],31,31,cardForwards[i],cardRights[i],cardUps[i]);
//                             triHitInfo = RayTriangle(hitPos,dir,tri);
//                             if (triHitInfo.didHit && (lightPositions[lightIndex] - triHitInfo.hitPoint).sqrMagnitude < distanceSquared) {
//                                 atten = 0f;
//                                 break;
//                             }
//                         }
//
//                         pencil = new Color(atten * lightColors[lightIndex].r,atten * lightColors[lightIndex].g,atten * lightColors[lightIndex].b,1f);
//                         bakedTextureColors[colorsIndex] += pencil;
//                     }
//
//                     if (breakout) break;
//                 }
            }
        }
    }

    private static float GetLighting(Vector3 lightPos, Vector3 hitPos, Vector3 triNorm, float range, float intensity) {
        float atten = 0f;
        float lambert = 1f;
        Vector3 dir = lightPos - hitPos;
        float distanceSquared = dir.sqrMagnitude;
        if (distanceSquared <= (range * range)) atten = 1f - (distanceSquared / (range * range));

        lambert = Mathf.Max(0.0f,Vector3.Dot(triNorm,Vector3.Normalize(dir)));
        atten *= lambert;
        atten *= intensity * a.lightFac;
        atten = Mathf.Max(0.0f,Mathf.Pow(atten,a.lightPow));
        return atten;
    }

    void OnDestroy() {
        if (currentJobHandle.IsCompleted == false) {
            currentJobHandle.Complete();
        }

        if (lightPositions != null) { if (lightPositions.IsCreated) lightPositions.Dispose(); }
        if (lightRanges != null) { if (lightRanges.IsCreated) lightRanges.Dispose(); }
//         if (cardPositions != null) { if (cardPositions.IsCreated) cardPositions.Dispose(); }
//         if (cardForwards != null) { if (cardForwards.IsCreated) cardForwards.Dispose(); }
//         if (cardUps != null) { if (cardUps.IsCreated) cardUps.Dispose(); }
//         if (cardRights != null) { if (cardRights.IsCreated) cardRights.Dispose(); }
        if (bakedTextureColors != null) { if (bakedTextureColors.IsCreated) bakedTextureColors.Dispose(); }
        if (lightColors != null) { if (lightColors.IsCreated) lightColors.Dispose(); }
        if (lightIntensities != null) { if (lightIntensities.IsCreated) lightIntensities.Dispose(); }
        if (triangles != null) { if (triangles.IsCreated) triangles.Dispose(); }
    }

    private static Vector3 GetTexelWorldPosition(Vector3 cardCenter, int x, int y, Vector3 forward, Vector3 right, Vector3 up) {
        // Map x: 0 -> cardWidth/2, 31 -> -cardWidth/2
        float localX = Mathf.Lerp(1.26f, -1.26f, (float)x / 31f);
        // Map y: 0 -> cardHeight/2, 31 -> -cardHeight/2
        float localY = Mathf.Lerp(-1.26f, 1.26f, (float)y / 31f);

        // Calculate world position using cardCenter and orientation vectors
        Vector3 worldPosition = cardCenter + (right * localX) + (up * 1.28f) + (forward * localY); // Front surface
        return worldPosition;
    }

    private struct Triangle {
        public Vector3 posA, posB, posC;
        public Vector3 normA, normB, normC;
        public Color color;
        public Vector2 uvA, uvB, uvC;
        public Vector4 tanA, tanB, tanC;
        public int meshRendererIndex;
    };

    private struct TriangleHitInfo {
        public bool didHit;
        public Vector3 hitPoint;
    };

    // Calculate the intersection of a ray with a triangle using MöllerTrumbore algorithm
    // Thanks to https://stackoverflow.com/a/42752998
    private static TriangleHitInfo RayTriangle(Vector3 origin, Vector3 dir, Triangle tri) {
        TriangleHitInfo hitInfo;
        hitInfo.didHit = false;
        hitInfo.hitPoint = Vector3.zero;
        Vector3 edgeAB = tri.posB - tri.posA;
        Vector3 edgeAC = tri.posC - tri.posA;
        Vector3 normalVector = Vector3.Cross(edgeAB, edgeAC);
        Vector3 ao = origin - tri.posA;
        Vector3 dao = Vector3.Cross(ao,dir);

        float determinant = -Vector3.Dot(dir, normalVector);
        float invDet = 1 / determinant;
        if (!(determinant >= 1E-4)) return hitInfo;

        // Calculate dst to triangle & barycentric coordinates of intersection point
        float dst = Vector3.Dot(ao, normalVector) * invDet;
        float u = Vector3.Dot(edgeAC, dao) * invDet;
        float v = -Vector3.Dot(edgeAB, dao) * invDet;
        float w = 1 - u - v;

        // Initialize hit info
        hitInfo.didHit = dst >= 0 && u >= 0 && v >= 0 && w >= 0;
        hitInfo.hitPoint = origin + dir * dst;
        return hitInfo;
    }

    // ---- RNG Functions ----

    // PCG (permuted congruential generator). Thanks to:
    // www.pcg-random.org and www.shadertoy.com/view/XlGcRh
//     uint NextRandom() {
//         state = state * 747796405 + 2891336453;
//         uint result = ((state >> ((state >> 28) + 4)) ^ state) * 277803737;
//         result = (result >> 22) ^ result;
//         return result;
//     }
    private static uint NextRandom() {
        state = state * 747796405 + 2891336453;
        uint result = ((state >> ((int)(state >> 28) + 4)) ^ state) * 277803737;
        result = (result >> 22) ^ result;
        return result;
    }

    private static float RandomValue() {
        return NextRandom() / 4294967295.0f; // 2^32 - 1
    }

    // Random value in normal distribution (with mean=0 and sd=1)
    private static float RandomValueNormalDistribution() {
        // Thanks to https://stackoverflow.com/a/6178290
        float theta = 2f * 3.1415926f * RandomValue();
        float rho = Mathf.Sqrt(-2f * Mathf.Log(RandomValue()));
        return rho * Mathf.Cos(theta);
    }

    // Calculate a random direction
    private static Vector3 RandomDirection() {
        // Thanks to https://math.stackexchange.com/a/1585996
        float x = RandomValueNormalDistribution();
        float y = RandomValueNormalDistribution();
        float z = RandomValueNormalDistribution();
        return Vector3.Normalize(new Vector3(x, y, z));
    }

    private static Vector2 RandomPointInCircle() {
        float angle = RandomValue() * 2f * 3.1415926f;
        Vector2 pointOnCircle = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        return pointOnCircle * Mathf.Sqrt(RandomValue());
    }
}
