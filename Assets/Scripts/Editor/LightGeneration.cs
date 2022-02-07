using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// [ExecuteInEditMode]
public class LightGeneration : MonoBehaviour {
	public GameObject selected;
	public Light singleLight;
	public string directoryPath = "C:/Users/Josiah/Dropbox/GitHub/Citadel/Assets/Models/Materials/GeneratedMaterials/";
	public GameObject[] allGameObjectsInLevel;
	public Light[] allLightsInLevel;
	public float shiftJitter = 0.02f;
	//public float bleedValueTreshold = 0.01f;
	//public float bleedRGBTreshold = 0.03921f;
	//public int neighborCountThreshold = 3;
	private float boundsBuffer = 0.03f;
	private float boost = 0.65f;
	private float multiplier = 0.07f;
	private float distBoost = 0.1f;
	private int layerMaskGeoLight;
	private RaycastHit tempHit;
	private bool[] lightWasForGeometry;
	private int[] bleedPixel;
	private int LAYER_GEOMETRY = 512;
	private string generatedFolderPath = "Assets/Models/Materials/GeneratedMaterials/";
	private string[] filenamelessextension;
	private string[] filename;
	private string[] fullPath;
	private bool[] goLitSuccess;
	private bool[] lightSuccess;
	private Texture2D[] emissiveNew;
	private Texture2D[] refAlbedo;
	private Color[] pixEmission;
	private Color[] albedoColors;
	private int[] resolution;
	private float xMin = 0f;
	private float xMax = 0f;
	private float yMin = 0f;
	private float yMax = 0f;
	private float zMin = 0f;
	private float zMax = 0f;
	private Vector3 boundsMin;
	private Vector3 boundsMax;
	private Vector3 dir;
	private float value;
	private float rgbReduceColoredIntensiy;
	private tridata[] triangleData;
	private Color[,] pixelColorAdd;
	private float[,] pixelValueAdd;
	private float angredux;
	private float rangePercent;
	private float diffuse;

	public void Reset_Selected() {
		#if UNITY_EDITOR
		if (lightWasForGeometry != null) {
			if (lightWasForGeometry.Length > 0) {
				for (int i =0; i< allLightsInLevel.Length;i++) {
					if (allLightsInLevel[i] != null) { // Reset Light.
						if ((allLightsInLevel[i].cullingMask & LAYER_GEOMETRY) < 1) {
							if (lightWasForGeometry[i]) allLightsInLevel[i].cullingMask = allLightsInLevel[i].cullingMask | LAYER_GEOMETRY; // Reset light.
						}
					}
				}
			}
		}

		for (int j=0; j<allGameObjectsInLevel.Length;j++) {
			if (allGameObjectsInLevel[j] != null) { // Reset GameObject.
				MeshRenderer mr = allGameObjectsInLevel[j].GetComponent<MeshRenderer>();
				SerializedProperty sp = new SerializedObject(mr).FindProperty("m_Materials.Array.data[0]");
				if (sp != null) PrefabUtility.RevertPropertyOverride(sp,InteractionMode.UserAction);
				else UnityEngine.Debug.Log("Failed to reset material on mr " + mr.ToString());
			}
		}
		#endif
	}

	public void GenerateLighting_MultipleSelectionHandler() {
		#if UNITY_EDITOR
		GenerateLighting_MultipleSelection();
		#endif
	}

	bool PositionValid(Vector3 pos) {
		return (pos.x < 999999f && pos.y < 999999f && pos.z < 999999f);
	}

	void GenerateLighting_MultipleSelection() {
		Stopwatch testTimer = new Stopwatch();
		testTimer.Start();
		
		// Prepare the geometry.  First we need to find everything that could block light rays.
		// Only considering Geometry layer to save computation.  Need to assign MeshColliders to 
		// anything that receives baked light or shadowing during bake.
		Mesh[] meshList = new Mesh[allGameObjectsInLevel.Length];
		MeshCollider[] meshCollidersList = new MeshCollider[allGameObjectsInLevel.Length];
		bool[] mcolPreExisted = new bool[allGameObjectsInLevel.Length];
		MeshRenderer[] meshRendererList = new MeshRenderer[allGameObjectsInLevel.Length];
		emissiveNew = new Texture2D[allGameObjectsInLevel.Length];
		refAlbedo = new Texture2D[allGameObjectsInLevel.Length];
		resolution = new int[allGameObjectsInLevel.Length];
		for (int j=0; j<allGameObjectsInLevel.Length;j++) {
			if (allGameObjectsInLevel[j] != null) {
				if (allGameObjectsInLevel[j].layer == 9) {
					meshList[j] = allGameObjectsInLevel[j].GetComponent<MeshFilter>().sharedMesh;
					meshCollidersList[j] = allGameObjectsInLevel[j].GetComponent<MeshCollider>();
					meshRendererList[j] = allGameObjectsInLevel[j].GetComponent<MeshRenderer>();
					mcolPreExisted[j] = false;
					if (meshCollidersList[j] == null) {
						meshCollidersList[j] = allGameObjectsInLevel[j].AddComponent(typeof(MeshCollider)) as MeshCollider;
						meshCollidersList[j].sharedMesh = meshList[j];
						mcolPreExisted[j] = false;
					} else mcolPreExisted[j] = true;
					refAlbedo[j] = (Texture2D)meshRendererList[j].sharedMaterial.mainTexture as Texture2D;
					resolution[j] = refAlbedo[j].width;
					emissiveNew[j] = new Texture2D(resolution[j], resolution[j], TextureFormat.RGB24, true);
					emissiveNew[j].filterMode = FilterMode.Point;
					Texture2D tempEmiss = (Texture2D)meshRendererList[j].sharedMaterial.GetTexture("_EmissionMap") as Texture2D;
					if (tempEmiss == null) {
						Color[] tempPix = new Color[resolution[j]*resolution[j]];
						for (int h=0; h<tempPix.Length;h++) tempPix[h] = Color.black;
						tempEmiss = new Texture2D(resolution[j],resolution[j], TextureFormat.RGB24, true);
						tempEmiss.SetPixels(tempPix);
					}
					emissiveNew[j].SetPixels(tempEmiss.GetPixels());
				}
			}
		}

		// Go through each light, find all GameObjects it affects, apply light to each, then remove Geometry layer from the light and move on to next light.
		layerMaskGeoLight = LayerMask.GetMask("Geometry"); // Not doing Default layer since I'd have to generate even moar MeshColliders for them.
		lightWasForGeometry = new bool[allLightsInLevel.Length];
		//bleedPixel = new int[2]; bleedPixel[0] = -1; bleedPixel[1] = -1;
		tempHit = new RaycastHit();
		filenamelessextension = new string[allGameObjectsInLevel.Length];
		filename = new string[allGameObjectsInLevel.Length];
		fullPath = new string[allGameObjectsInLevel.Length];
		goLitSuccess = new bool[allGameObjectsInLevel.Length];
		lightSuccess = new bool[allLightsInLevel.Length];
		for (int i =0; i< allLightsInLevel.Length;i++) {
			if (allLightsInLevel[i] != null) {
				if ((allLightsInLevel[i].cullingMask & LAYER_GEOMETRY) > 0) { // Valid light that affects geometry
					lightWasForGeometry[i] = true;
					for (int j=0; j<allGameObjectsInLevel.Length;j++) {
						if (allGameObjectsInLevel[j] != null) {
							if (allGameObjectsInLevel[j].layer == 9) {
								if (meshRendererList[j] != null) {
									Texture2D refEmission = (Texture2D)meshRendererList[j].sharedMaterial.GetTexture("_EmissionMap") as Texture2D;
									if (refEmission == null) {pixEmission = new Color[resolution[j]*resolution[j]]; for (int h=0; h<pixEmission.Length;h++) pixEmission[h] = Color.black;}
									else pixEmission = refEmission.GetPixels();
									albedoColors = refAlbedo[j].GetPixels();
									Vector3[] vertSet = meshList[j].vertices; // Referencing this directly causes boxing and is slower.
									xMin = 0f;
									xMax = 0f;
									yMin = 0f;
									yMax = 0f;
									zMin = 0f;
									zMax = 0f;
									// pixelColorAdd = new Color[resolution,resolution];
									// pixelValueAdd = new float[resolution,resolution];
									Vector3[] vertSetWorldspace = new Vector3[vertSet.Length];
									for (int k=0;k<vertSet.Length;k++) {
										vertSetWorldspace[k] = allGameObjectsInLevel[j].transform.TransformPoint(vertSet[k]);
										xMin = Mathf.Min(xMin,vertSetWorldspace[k].x);
										xMax = Mathf.Max(xMax,vertSetWorldspace[k].x);
										yMin = Mathf.Min(yMin,vertSetWorldspace[k].y);
										yMax = Mathf.Max(yMax,vertSetWorldspace[k].y);
										zMin = Mathf.Min(zMin,vertSetWorldspace[k].z);
										zMax = Mathf.Max(zMax,vertSetWorldspace[k].z);
									}
									boundsMin.x = xMin - boundsBuffer;
									boundsMin.y = yMin - boundsBuffer;
									boundsMin.z = zMin - boundsBuffer;
									boundsMax.x = xMax + boundsBuffer;
									boundsMax.y = yMax + boundsBuffer;
									boundsMax.z = zMax + boundsBuffer;
									triangleData = GetTriangleData(vertSetWorldspace,resolution[j],meshList[j].uv);
									//GenerateRaymarchedLightingOnObject(allLightsInLevel[i],pixEmission,albedoColors,ref emissiveNew[j],triangleData,resolution[j]);
									GenerateLightingOnObject(allGameObjectsInLevel[j],allLightsInLevel[i],meshList[j],boundsMin,boundsMax,pixEmission,albedoColors,ref emissiveNew[j],vertSetWorldspace,j,resolution[j]);
									goLitSuccess[j] = lightSuccess[i] = true;
									filenamelessextension[j] = (allGameObjectsInLevel[j].name + allGameObjectsInLevel[j].GetInstanceID().ToString());
									filename[j] = filenamelessextension[j] + ".png";
									fullPath[j] = (directoryPath + filename[j]);
								}
							}
						}
					}
					if (lightSuccess[i]) allLightsInLevel[i].cullingMask -= LAYER_GEOMETRY;// UnityEngine.Debug.Log("Light success for index " + i.ToString()); } // Remove Geometry layer now that this light is done affecting GameObjects.
				} else lightWasForGeometry[i] = false;
			} else lightWasForGeometry[i] = false;
		}

		// Apply lighting to file for each.  Doing this last since multiple lights could affect same object and this way we only save the file once.
		// Clean ups.  Need to remove auto-generated MeshCollider components that were needed for the raycasts to get texcoords from and to be accurate.
		for (int j=0; j<allGameObjectsInLevel.Length;j++) {
			if (allGameObjectsInLevel[j] != null) {
				if (allGameObjectsInLevel[j].layer == 9) {
					if (goLitSuccess[j]) {
						byte[] bytes = emissiveNew[j].EncodeToPNG();
						System.IO.File.WriteAllBytes(fullPath[j], bytes);
						// UnityEngine.Debug.Log("Saved bake as: " + filename[j]);
						AssetDatabase.ImportAsset(generatedFolderPath + filename[j],ImportAssetOptions.ForceUpdate);
						TextureImporter timpo = AssetImporter.GetAtPath(generatedFolderPath + filename[j]) as TextureImporter;
						timpo.filterMode = FilterMode.Point;
						timpo.textureCompression = TextureImporterCompression.CompressedHQ;
						timpo.compressionQuality = 85;
						timpo.crunchedCompression = true;
						timpo.mipmapEnabled = false;
						timpo.isReadable = true;
						timpo.SaveAndReimport();
						Material mat = new Material(meshRendererList[j].sharedMaterial);//new Material(Shader.Find("Standard (Specular setup)"));
						AssetDatabase.CreateAsset(mat,generatedFolderPath + filenamelessextension[j] + ".mat");
						mat.SetTexture("_MainTex",(Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(meshRendererList[j].sharedMaterial.mainTexture),typeof(Texture2D)));
						mat.SetColor("_EmissionColor",Color.white);
						mat.EnableKeyword("_EMISSION");
						mat.SetTexture("_EmissionMap",(Texture2D)AssetDatabase.LoadAssetAtPath(generatedFolderPath + filename[j],typeof(Texture2D)));
						meshRendererList[j].material = mat;
					}

					if (!mcolPreExisted[j] && meshCollidersList[j] != null) DestroyImmediate(meshCollidersList[j]);
				}
			}
		}
		testTimer.Stop();
		UnityEngine.Debug.Log("Light generated for all lights on selection in " + testTimer.Elapsed.ToString());
	}

	

	bool GenerateRaymarchedLightingOnObject(Light testLight, Color[] pixEmission, Color[] pixRef,ref Texture2D emissiveNewRef, tridata[] triangleData, int resolution) {
		Color[] pix = pixEmission;
		Color[] pixPrevious = emissiveNewRef.GetPixels();
		int uvx, uvy, pixIndexFlat;
		float lightsum = 0f;
		Color litCol = Color.black;
		bool[,] pixelPositionFound = new bool[resolution,resolution];
		uvx = uvy = 0;
		for (int i=0;i<triangleData.Length;i++) {
			for (int x=0;x< triangleData[i].uPixels;x++) {
				for (int y=0;y< triangleData[i].vPixels;y++) {
					// if (i != 18) continue;
					uvx = (int)Mathf.Clamp((triangleData[i].uvSet[x,y].x * resolution),0f,resolution-1);
					uvy = (int)Mathf.Clamp((triangleData[i].uvSet[x,y].y * resolution),0f,resolution-1);
					if (!pixelPositionFound[uvx,uvy]) {
						pixIndexFlat = PixFlat(uvx, uvy, resolution);
						lightsum = testLight.color.r + testLight.color.g + testLight.color.b;
						value = LightFalloff(testLight.intensity, Vector3.Distance(triangleData[i].pixelPositions[x,y],testLight.transform.position), testLight.range, tempHit.normal,Vector3.Normalize(testLight.transform.position - triangleData[i].pixelPositions[x,y]), testLight.color);
						rgbReduceColoredIntensiy = (lightsum + pixRef[pixIndexFlat].r + pixRef[pixIndexFlat].g + pixRef[pixIndexFlat].b) / 6f; // Average the light plus albedo.
						litCol = new Color((pixRef[pixIndexFlat].r * value * rgbReduceColoredIntensiy * testLight.color.r) + pixEmission[pixIndexFlat].r,
										 (pixRef[pixIndexFlat].g * value * rgbReduceColoredIntensiy * testLight.color.g) + pixEmission[pixIndexFlat].g,
										 (pixRef[pixIndexFlat].b * value * rgbReduceColoredIntensiy * testLight.color.b) + pixEmission[pixIndexFlat].b);
						//UnityEngine.Debug.Log("Lighting pixel with color: " + litCol.ToString());
						pix[pixIndexFlat] = litCol;
						pixelPositionFound[uvx,uvy] = true;
						// UnityEngine.Debug.DrawLine(testLight.transform.position,triangleData[i].pixelPositions[x,y],Color.green,2f);
					}
					//if (i == 33) {
						//UnityEngine.Debug.Log("Ray");
						UnityEngine.Debug.DrawLine(testLight.transform.position,triangleData[i].pixelPositions[x,y],Color.green,2f);
					//}
				}
			}
		}

		for (int i=0;i<pixPrevious.Length;i++) {
			pix[i].r = pix[i].r + pixPrevious[i].r;
			pix[i].g = pix[i].g + pixPrevious[i].g;
			pix[i].b = pix[i].b + pixPrevious[i].b;
		}
		emissiveNewRef.SetPixels(pix);
		return true;
	}

	bool GenerateLightingOnObject(GameObject go, Light testLight, Mesh md, Vector3 boundsMin, Vector3 boundsMax,
	Color[] pixEmission, Color[] pixRef,ref Texture2D emissiveNewRef, Vector3[] vertSetWorldspace, int objectIndex, int resolution) {
		Vector3[] normalSet = md.normals;
		Color[] pix = pixEmission;
		Color[] pixPrevious = emissiveNewRef.GetPixels();
		Color litCol = Color.black;
		int uvx,uvy;
		int pixIndexFlat = 0;
		value = 0;
		float lightsum = testLight.color.r + testLight.color.g + testLight.color.b;
		// Setup the raycasts for this object
		List<Vector3> potentialCastRegister = new List<Vector3>();
		List<Vector3> castTargets = new List<Vector3>();
		// The slow, very-effective, brute-force method
		for (float x=boundsMin.x;x<=boundsMax.x;x = x+shiftJitter) {
			for (float y=boundsMin.y;y<=boundsMax.y;y=y+shiftJitter) {
				for (float z=boundsMin.z;z<=boundsMax.z;z=z+shiftJitter) {
					castTargets.Add(new Vector3(x,y,z));
				}
			}
		}
		for (int n=0;n<vertSetWorldspace.Length;n++) {
			castTargets.Add(vertSetWorldspace[n]);
		}
		// DON'T DELETE, this is what worked best!

		bool[,] pixelPositionFound = new bool[resolution,resolution];
		for (int m=0;m < castTargets.Count; m++) {
			dir = castTargets[m] - testLight.transform.position;
			potentialCastRegister.Add(Vector3.Normalize(dir));
		}
		// UnityEngine.Debug.Log("Casts: " + potentialCastRegister.Count.ToString());
		for (int i=0;i<potentialCastRegister.Count;i++) {
			if (Physics.Raycast(testLight.transform.position, potentialCastRegister[i], out tempHit, testLight.range + Mathf.Epsilon,layerMaskGeoLight)) {
				// Raycasts are 42% of the performance cost.

				// This inner bit is 13% of the performance cost.
				if (tempHit.transform == go.transform) {
					uvx = (int)Mathf.Clamp((tempHit.textureCoord.x * resolution),0f,resolution-1);
					uvy = (int)Mathf.Clamp((tempHit.textureCoord.y * resolution),0f,resolution-1);
					pixIndexFlat = PixFlat(uvx, uvy, resolution);
					value = LightFalloff(testLight.intensity, tempHit.distance, testLight.range, tempHit.normal, potentialCastRegister[i], testLight.color);
					if (!pixelPositionFound[uvx,uvy]) {
						rgbReduceColoredIntensiy = (lightsum + pixRef[pixIndexFlat].r + pixRef[pixIndexFlat].g + pixRef[pixIndexFlat].b) / 6f;
						litCol = new Color((pixRef[pixIndexFlat].r * value * rgbReduceColoredIntensiy * testLight.color.r) + pixEmission[pixIndexFlat].r,
										   (pixRef[pixIndexFlat].g * value * rgbReduceColoredIntensiy * testLight.color.g) + pixEmission[pixIndexFlat].g,
										   (pixRef[pixIndexFlat].b * value * rgbReduceColoredIntensiy * testLight.color.b) + pixEmission[pixIndexFlat].b);
						pix[pixIndexFlat] = litCol;
						pixelPositionFound[uvx,uvy] = true;
					}
					// UnityEngine.Debug.DrawRay(testLight.transform.position,potentialCastRegister[i] * testLight.range,Color.green,2f);
				}
			}
			
		}
		for (int i=0;i<pixPrevious.Length;i++) {
			pix[i].r = pix[i].r + pixPrevious[i].r;
			pix[i].g = pix[i].g + pixPrevious[i].g;
			pix[i].b = pix[i].b + pixPrevious[i].b;
		}
		emissiveNewRef.SetPixels(pix);
		return true;
	}

	public struct tridata {
		public Vector3 p1;
		public Vector3 p2;
		public Vector3 p3;
		public Vector3 normal;
		public Vector2 p1_uv;
		public Vector2 p2_uv;
		public Vector2 p3_uv;
		public Vector2[,] uvSet;
		public Vector3[,] pixelPositions;
		public float[,] pixelOnTriangle; // Represents the percentage on tri. 1 = fully within the tri, 0 = off, partial = partial and thus gets lit partially.
		public int uPixels;
		public int vPixels;
	}

	void SetPixelsPositions(ref tridata tri, int resolution, int debugIndex) {
		tri.uvSet = new Vector2[resolution,resolution]; // This is overkill but our best bet assuming no extraneous tris outside of uv 0 to 1 position and no tris larger than 0 to 1.
		float pixelWidth = (1f / resolution);
		int stepX = 0;
		int stepY = 0;
		// Create a rectangle of the pixels that the tri is on top of.
		float uvxMin = Mathf.Min(tri.p1_uv.x,tri.p2_uv.x,tri.p3_uv.x);
		float uvyMin = Mathf.Min(tri.p1_uv.y,tri.p2_uv.y,tri.p3_uv.y);
		float uvxMax = Mathf.Max(tri.p1_uv.x,tri.p2_uv.x,tri.p3_uv.x);
		float uvyMax = Mathf.Max(tri.p1_uv.y,tri.p2_uv.y,tri.p3_uv.y);
		tri.uvSet[0,0].x = uvxMin;
		tri.uvSet[0,0].y = uvyMin;
		for (float u=uvxMin; u<uvxMax; u+=pixelWidth) {
			stepX++;
			for (float v=uvyMin; v<uvyMax; v+=pixelWidth) {
				stepY++;
				if (stepX < resolution && stepY < resolution) {
					tri.uvSet[stepX,stepY].x = u;
					tri.uvSet[stepX,stepY].y = v;
					tri.uPixels = stepX + 1;
					tri.vPixels = stepY + 1;
				} else break;
			}
		}
		if (tri.uPixels < 1) tri.uPixels = 1;
		if (tri.vPixels < 1) tri.vPixels = 1;
		stepX = 0;
		stepY = 0;
		 // These are the direction in worldspace that traverses along the tri such that it is parallel to the uv texture space in u or v direction.
		Vector3 uvXDirection = Vector3.Normalize(Quaternion.AngleAxis(Mathf.Atan2(tri.p1_uv.y,tri.p1_uv.x)*(180f/(float)Math.PI),tri.normal) * Vector3.Normalize(tri.p2 - tri.p1));
		Vector3 uvYDirection = Vector3.Normalize(Vector3.Cross(uvXDirection,tri.normal));
		float pixel3DWidth = (Vector2.Distance(tri.p1_uv,tri.p2_uv) * resolution)/tri.uPixels * 0.02f;// Worldspace assumes 0.02f = normal pixel width (this is Citadel's common texel size).
		tri.pixelPositions = new Vector3[tri.uPixels,tri.vPixels];
		// pixelPositions[0,0] needs to be the worldspace coordinate of the top left pixel of the
		// bounding box of pixels on a tri...this might not actually be on the tri!  That's fine.
		// Find it anyways to be overzealous.  Rely on float list pixelOnTriangle[,] later for
		// whether a pixel actually gets lit and how much (see note above).
		Vector3 startOfUvBounds = new Vector3(-9999f,-9999f,-9999f);
		Vector2 uvBounds = new Vector2(1f,1f);
		Vector2 uvMin = new Vector2(uvxMin,uvyMin);
		if (tri.p1_uv.y <= uvyMin) { startOfUvBounds = tri.p1; uvBounds = tri.p1_uv; }
		else if (tri.p2_uv.y <= uvyMin) { startOfUvBounds = tri.p2; uvBounds = tri.p2_uv; }
		else { startOfUvBounds = tri.p3; uvBounds = tri.p3_uv; }

		startOfUvBounds = startOfUvBounds + (uvXDirection * Vector2.Distance(uvBounds,uvMin));
		for (stepX=0; stepX<tri.uPixels;stepX++) {
			for (stepY=0; stepY<tri.vPixels;stepY++) {
				if (stepX < resolution && stepY < resolution) {
					tri.pixelPositions[stepX,stepY] = startOfUvBounds + (uvXDirection * pixel3DWidth * stepX);
					tri.pixelPositions[stepX,stepY] = tri.pixelPositions[stepX,stepY] + (uvYDirection * pixel3DWidth * stepY);
				} else break;
			}
		}
	}

	tridata[] GetTriangleData(Vector3[] vertPositions, int resolution, Vector2[] uvs) {
		tridata[] retval = new tridata[vertPositions.Length / 3];
		int triangleIndex = 0;
		for (int i=0;i<vertPositions.Length - 3;i += 3) { // Iterate through all triangles.
			retval[triangleIndex].p1 = vertPositions[i]; // Get triangle corners
			retval[triangleIndex].p2 = vertPositions[i+1];
			retval[triangleIndex].p3 = vertPositions[i+2];
			retval[triangleIndex].normal = Vector3.Cross(retval[triangleIndex].p2 - retval[triangleIndex].p1, retval[triangleIndex].p3 - retval[triangleIndex].p1);
			retval[triangleIndex].p1_uv = uvs[i];
			retval[triangleIndex].p2_uv = uvs[i+1];
			retval[triangleIndex].p3_uv = uvs[i+2];
			SetPixelsPositions(ref retval[triangleIndex],resolution,i);
			triangleIndex++;
		}
		return retval;
	}

	int PixFlat(int x, int y, int resolution) { return (int)Mathf.Clamp((y * resolution + x),0,resolution * resolution); }

	float LightFalloff(float intensity, float dist, float radius, Vector3 surfnorm, Vector3 raydir, Color lightColor) {
		if (dist > radius) return 0f;
		angredux = Mathf.Clamp(Vector3.Dot(-surfnorm,raydir),0f,1f); // Apply Lambertian reflectance.
		rangePercent = Mathf.Clamp((radius - dist) / radius,0f,1f); // Clamp falloff to the lights set range value as a radius.
		diffuse = ((Mathf.Pow(rangePercent,0.5f) * intensity * angredux * 0.5f) / 2.2f); // Apply base light intensity.
		return Mathf.Max(0f,diffuse + ((1f/Mathf.Max(0f,(Mathf.Pow(Mathf.Max(0f,(dist/radius)) * 5f,2f)))) * intensity * angredux) * multiplier * rangePercent + (boost * rangePercent) + (distBoost * (1f/(dist*dist))));
	}
}