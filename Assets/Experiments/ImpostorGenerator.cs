using UnityEngine;

public class ImpostorGenerator : MonoBehaviour
{
//     public GameObject target; // The 3D model to capture
//     public int resolution = 128; // Resolution per slice
//     public int angularSteps = 16; // Number of steps in the sphere
//     private Camera captureCamera;
// 
//     void Start()
//     {
//         SetupCaptureCamera();
//         GenerateImpostorTexture();
//     }
// 
//     void SetupCaptureCamera()
//     {
//         // Create a new camera for rendering
//         GameObject camObj = new GameObject("CaptureCamera");
//         captureCamera = camObj.AddComponent<Camera>();
//         captureCamera.backgroundColor = Color.clear;
//         captureCamera.orthographic = false;
//         captureCamera.enabled = false;
//     }
// 
//     void GenerateImpostorTexture()
//     {
//         // Create the 3D texture
//         Texture3D impostorTexture = new Texture3D(resolution, resolution, angularSteps, TextureFormat.RGBA32, false);
// 
//         // Array to hold the pixel data
//         Color[] textureData = new Color[resolution * resolution * angularSteps];
// 
//         // Render each slice
//         for (int i = 0; i < angularSteps; i++)
//         {
//             float phi = Mathf.PI * i / angularSteps; // Elevation angle
//             for (int j = 0; j < angularSteps; j++)
//             {
//                 float theta = 2 * Mathf.PI * j / angularSteps; // Azimuth angle
//                 Texture2D sliceTex = RenderSlice(phi, theta);
// 
//                 // Copy slice data into the 3D texture
//                 int sliceIndex = i * angularSteps + j;
//                 Color[] slicePixels = sliceTex.GetPixels();
//                 for (int y = 0; y < resolution; y++)
//                 {
//                     for (int x = 0; x < resolution; x++)
//                     {
//                         int flatIndex = sliceIndex * resolution * resolution + y * resolution + x;
//                         textureData[flatIndex] = slicePixels[y * resolution + x];
//                     }
//                 }
// 
//                 Destroy(sliceTex); // Clean up the temporary slice texture
//             }
//         }
// 
//         impostorTexture.SetPixels(textureData);
//         impostorTexture.Apply();
// 
//         // Assign the texture to the material
//         Material impostorMaterial = target.GetComponent<Renderer>().sharedMaterial;
//         impostorMaterial.SetTexture("_VolumeTex", impostorTexture);
//     }
// 
//     Texture2D RenderSlice(float phi, float theta)
//     {
//         // Position the camera
//         captureCamera.transform.position = target.transform.position +
//                                            new Vector3(
//                                                Mathf.Sin(phi) * Mathf.Cos(theta),
//                                                Mathf.Cos(phi),
//                                                Mathf.Sin(phi) * Mathf.Sin(theta)
//                                            ) * 5; // Distance from model
//         captureCamera.transform.LookAt(target.transform);
// 
//         // Render to a temporary RenderTexture
//         RenderTexture tempRT = new RenderTexture(resolution, resolution, 0);
//         captureCamera.targetTexture = tempRT;
//         captureCamera.Render();
// 
//         // Copy RenderTexture to Texture2D
//         RenderTexture.active = tempRT;
//         Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
//         tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
//         tex.Apply();
// 
//         // Clean up
//         RenderTexture.active = null;
//         captureCamera.targetTexture = null;
//         tempRT.Release();
// 
//         return tex;
//     }
}

