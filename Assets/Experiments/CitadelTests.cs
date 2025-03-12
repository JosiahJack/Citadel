using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class CitadelTests : MonoBehaviour {
	public Camera probeCam;
	public Light cookieLight;
	public GameObject[] geometryContainters;
	public GameObject[] staticObjectContainters;
	public GameObject[] lightContainers; // Can't use LevelManager's since
										 // there is no instance unless in Play
										 // mode.
	public GameObject[] dynamicObjectContainers;
// 	public GameObject gameObjectToSave;
	public int levelToOutputFrom = 0;
	public Const ct;
	public LevelManager lm;
	public PauseScript ps;
	
	private bool getValparsed;
	private bool[] levelDataLoaded;
	private int getValreadInt;
	private float getValreadFloat;

	public List<GameObject> allGOs;
	public List<GameObject> allParents;

	[HideInInspector] public string buttonLabel = "Run Tests";

	public void SaveReflectionProbeImage() {
        // Check if the probe has a baked texture
        if (probeCam != null) {
            // Define the path where we want to save the image
            string path = Path.Combine(Application.dataPath, "StreamingAssets", "testprobe.png");
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            // Read the pixels from the realtimeTexture
            Cubemap cube = new Cubemap(512,TextureFormat.ARGB32,false);
            probeCam.RenderToCubemap(cube);
            cookieLight.cookie = cube;
//             CubemapFace[] faces = (CubemapFace[])System.Enum.GetValues(typeof(CubemapFace));
//             foreach (CubemapFace face in faces) {
// 				if (face == CubemapFace.Unknown) continue;
//
//                 // Create a new Texture2D for each face
//                 Texture2D tex = new Texture2D(cube.width, cube.height, TextureFormat.RGB24, false);
//                 tex.SetPixels(cube.GetPixels(face,0)); // Get pixels for specific face
//                 tex.Apply();
//
//                 // Convert texture to PNG format and save it
//                 byte[] pngBytes = tex.EncodeToPNG();
//                 string pathfinal = Path.Combine(directory, $"cubemap_{face}.png");
//                 File.WriteAllBytes(pathfinal, pngBytes);
//                 UnityEngine.Debug.Log("Cubemap face " + face + " saved to: " + pathfinal);
//
//                 // Cleanup
//                 UnityEngine.Object.DestroyImmediate(tex);
//             }
        } else {
            UnityEngine.Debug.LogWarning("No cubemap camera for rendering light's view");
        }
    }

	public void RunUnits() {
		Stopwatch testTimer = new Stopwatch();
		testTimer.Start();
		GameObject go = new GameObject();
		ActivateButton ab = go.AddComponent<ActivateButton>();
		UnityEngine.Object.DestroyImmediate(ab);
		UnityEngine.Object.DestroyImmediate(go);
		testTimer.Stop();
		UnityEngine.Debug.Log("All unit tests completed in "
							  + testTimer.Elapsed.ToString());
	}

	public void SetupLists() {
		int i=0;
		int k=0;
		List<GameObject> allGOs = new List<GameObject>();
		List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
		for (i=0;i<allParents.Count;i++) {
			Component[] compArray = allParents[i].GetComponentsInChildren(typeof(Transform),true);
			for (k=0;k<compArray.Length;k++) {
				allGOs.Add(compArray[k].gameObject); // Add to full list, separate so we don't infinite loop
			}
		}
	}

// 	public void LoadLevelLights() {
// 		lm.LoadLevelLights(levelToOutputFrom);
// 	}
// 
// 	public void UnloadLevelLights() {
// 		lm.UnloadLevelLights(levelToOutputFrom);
// 	}

// 	public void CheckUseHandlers() {
// 		List<GameObject> useHandlerHolders = new List<GameObject>();
// 		FindAllGOWithComponent(ref useHandlerHolders);
// 		int num = 0;
// 		for (int i=0;i<useHandlerHolders.Count;i++) {
// 			UseHandler uh = useHandlerHolders[i].GetComponent<UseHandler>();
// 			if (uh == null) continue;
// 			
// 			num++;
// 			GameObject parent = useHandlerHolders[i].transform.parent.gameObject;
// 			string parentString = "-";
// 			if (parent == null) parentString = "self";
// 			else parentString = parent.name;
// 			if (!uh.useButtonSwitch) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with useButtonSwitch set to false! Parent: " + parentString);
// 			if (!uh.useChargeStation) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with useChargeStation set to false! Parent: " + parentString);
// 			if (!uh.useDoor) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with useDoor set to false! Parent: " + parentString);
// 			if (!uh.useHealingBed) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with useHealingBed set to false! Parent: " + parentString);
// 			if (!uh.useKeypadElevator) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with useKeypadElevator set to false! Parent: " + parentString);
// 			if (!uh.useKeypadKeycode) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with useKeypadKeycode set to false! Parent: " + parentString);
// 			if (!uh.usePaperLog) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with usePaperLog set to false! Parent: " + parentString);
// 			if (!uh.usePuzzleGridPuzzle) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with usePuzzleGridPuzzle set to false! Parent: " + parentString);
// 			if (!uh.usePuzzleWirePuzzle) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with usePuzzleWirePuzzle set to false! Parent: " + parentString);
// 			if (!uh.useUseableObjectUse) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with useUseableObjectUse set to false! Parent: " + parentString);
// 			if (!uh.useUseableAttachment) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with useUseableAttachment set to false! Parent: " + parentString);
// 			if (!uh.useCyberAccess) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with useCyberAccess set to false! Parent: " + parentString);
// 			if (!uh.useInteractablePanel) UnityEngine.Debug.Log(useHandlerHolders[i].name + " has UseHandler with useInteractablePanel set to false! Parent: " + parentString);
// 		}
// 		
// 		UnityEngine.Debug.Log(num.ToString() + " UseHandlers checked!");
// 	}
	
	// Change to get the needed component.
	void FindAllGOWithComponent(ref List<GameObject> gos) {
		List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
		for (int i=0;i<allParents.Count;i++) {
			Component[] compArray = allParents[i].GetComponentsInChildren(typeof(UseHandler),true); // find all SaveObject components, including inactive (hence the true here at the end)
			for (int k=0;k<compArray.Length;k++) {
				gos.Add(compArray[k].gameObject); //add the gameObject associated with all SaveObject components in the scene
			}
		}
	}
	
	public void GenerateStaticObjectsImmutableDataFile() {
		#if UNITY_EDITOR
		UnityEngine.Debug.Log("Outputting all static objects to "
				  + "StreamingAssets/CitadelScene_staticobjectsimmutable_level"
				  + levelToOutputFrom.ToString() + ".txt");

		List<GameObject> allStaticObjects = new List<GameObject>();
		Transform tr = staticObjectContainters[levelToOutputFrom].transform;
		Transform child = null;
		for (int i=0;i<tr.childCount;i++) {
			child = tr.GetChild(i);
			if (child == null) continue;
			
			allStaticObjects.Add(child.gameObject);
		}

		UnityEngine.Debug.Log("Found " + allStaticObjects.Count
							  + " static objects chunks in level "
							  + levelToOutputFrom.ToString());

		string lName = "CitadelScene_staticobjects_level"
					   + levelToOutputFrom.ToString() + ".txt";

		string lP = Utils.SafePathCombine(Application.streamingAssetsPath,
										  lName);

		StreamWriter sw = new StreamWriter(lP,false,Encoding.ASCII);
		if (sw == null) {
			UnityEngine.Debug.Log("Static objects output file path invalid");
			return;
		}

		using (sw) {
			for (int i=0;i<allStaticObjects.Count;i++) {				
				sw.Write(SaveLoad.SavePrefab(allStaticObjects[i]));
				sw.Write(Environment.NewLine);
			}
			sw.Close();
		}
		#endif
	}

/*	
	public void GenerateGeometryDataFile() {
		#if UNITY_EDITOR
		UnityEngine.Debug.Log("Outputting all chunks to StreamingAssets/CitadelScene_geometry_level" + levelToOutputFrom.ToString() + ".dat");
		List<GameObject> allGeometry = new List<GameObject>();
		Transform tr = geometryContainters[levelToOutputFrom].transform;
		Transform child = null;
		for (int i=0;i<tr.childCount;i++) {
			child = tr.GetChild(i);
			if (child == null) continue;
			
			allGeometry.Add(child.gameObject);
		}

		UnityEngine.Debug.Log("Found " + allGeometry.Count + " geometry chunks in level " + levelToOutputFrom.ToString());

		string lName = "CitadelScene_geometry_level"
					   + levelToOutputFrom.ToString() + ".dat";

		string lP = Utils.SafePathCombine(Application.streamingAssetsPath,
										  lName);

		StreamWriter sw = new StreamWriter(lP,false,Encoding.ASCII);
		if (sw == null) {
			UnityEngine.Debug.Log("Geometry output file path invalid");
			return;
		}
		
		StringBuilder s1 = new StringBuilder();
		using (sw) {
			PrefabIdentifier pid = null;
			for (int i=0;i<allGeometry.Count;i++) {
				pid = allGeometry[i].GetComponent<PrefabIdentifier>();
				if (pid == null) continue;
				
				bool hasBoxColliderOverride = false;
				bool hasTextOverride = false;
				string materialOverride = "";
				List<ObjectOverride> ovides = PrefabUtility.GetObjectOverrides(allGeometry[i],false);
				for (int j=0; j < ovides.Count; j++) {
					UnityEngine.Object ob = ovides[j].instanceObject;
					SerializedObject sob = new UnityEditor.SerializedObject(ob);

					hasBoxColliderOverride = false;
					// List overridden properties
					SerializedProperty prop = sob.GetIterator();
					while (prop.NextVisible(true)) {
						if (prop.propertyPath == "m_Name" ||
							prop.propertyPath == "m_LocalPosition" ||
							prop.propertyPath == "m_LocalPosition.x" ||
							prop.propertyPath == "m_LocalPosition.y" ||
							prop.propertyPath == "m_LocalPosition.z" ||
							prop.propertyPath == "m_LocalRotation" ||
							prop.propertyPath == "m_LocalRotation.x" ||
							prop.propertyPath == "m_LocalRotation.y" ||
							prop.propertyPath == "m_LocalRotation.z" ||
							prop.propertyPath == "m_LocalRotation.w" ||
							prop.propertyPath == "m_LocalScale" ||
							prop.propertyPath == "m_LocalScale.x" ||
							prop.propertyPath == "m_LocalScale.y" ||
							prop.propertyPath == "m_LocalScale.z")  {
							
							continue; // Skip transform overrides
						}

						if (prop.prefabOverride) {
							if (prop.propertyPath == "m_Size" ||
								prop.propertyPath == "m_Size.x" ||
								prop.propertyPath == "m_Size.y" ||
								prop.propertyPath == "m_Size.z" ||
								prop.propertyPath == "m_Center" ||
								prop.propertyPath == "m_Center.x" ||
								prop.propertyPath == "m_Center.y" ||
								prop.propertyPath == "m_Center.z") {
							
								hasBoxColliderOverride = true;
							}
							
							if (prop.propertyPath == "lingdex") {
								hasTextOverride = true;
							}
							
							string value = GetPropertyValue(prop);
							if (prop.propertyPath == "m_Materials.Array.data[0]") {
								materialOverride = value;
							}
							
							UnityEngine.Debug.Log(allGeometry[i].name
												  + ":: Found Override: "
												  + prop.propertyPath
												  + ", Value: " + value);
						}
					}
				}
				
				s1.Clear();
				s1.Append(Utils.UintToString(pid.constIndex,"constIndex"));
				s1.Append(Utils.splitChar);
				s1.Append(allGeometry[i].name);
				s1.Append(Utils.splitChar);
				s1.Append(Utils.SaveTransform(allGeometry[i].transform));
				if (hasBoxColliderOverride) {
					BoxCollider bcol = allGeometry[i].GetComponent<BoxCollider>();
					s1.Append(Utils.splitChar);
					s1.Append(Utils.BoolToString(bcol.enabled,"BoxCollider.enabled"));
					s1.Append(Utils.splitChar);
					s1.Append(Utils.FloatToString(bcol.size.x,"size.x"));
					s1.Append(Utils.splitChar);
					s1.Append(Utils.FloatToString(bcol.size.y,"size.y"));
					s1.Append(Utils.splitChar);
					s1.Append(Utils.FloatToString(bcol.size.z,"size.z"));
					s1.Append(Utils.splitChar);
					s1.Append(Utils.FloatToString(bcol.center.x,"center.x"));
					s1.Append(Utils.splitChar);
					s1.Append(Utils.FloatToString(bcol.center.y,"center.y"));
					s1.Append(Utils.splitChar);
					s1.Append(Utils.FloatToString(bcol.center.z,"center.z"));
					if (allGeometry[i].transform.childCount >= 1) {
						// Get collision aid.
						Transform subtr = allGeometry[i].transform.GetChild(0);
						if (subtr != null) {
							s1.Append(Utils.splitChar);
							s1.Append(Utils.BoolToString(
								subtr.gameObject.activeSelf,
								"collisionAid.activeSelf"));
						}
					}
				}
				
				if (!string.IsNullOrWhiteSpace(materialOverride)) {
					s1.Append(Utils.splitChar);
					s1.Append(Utils.SaveString(materialOverride,"material"));
				}
				
				if (pid.constIndex == 218) { // chunk_reac2_4 has text on it.
					if (hasTextOverride) {
						s1.Append(Utils.splitChar);
						Transform textr1 = allGeometry[i].transform.GetChild(1); // text_decalStopDSS1
						Transform textr2 = allGeometry[i].transform.GetChild(2); // text_decalStopDSS1 (1)
						TextLocalization tex1 = textr1.gameObject.GetComponent<TextLocalization>();
						TextLocalization tex2 = textr2.gameObject.GetComponent<TextLocalization>();
						s1.Append(Utils.UintToString(tex1.lingdex,"lingdex"));
						s1.Append(Utils.splitChar);
						s1.Append(Utils.UintToString(tex2.lingdex,"lingdex"));
					}
				}
				sw.Write(s1.ToString());
				sw.Write(Environment.NewLine);
			}
			sw.Close();
		}
		#endif
	}*/
	
	#if UNITY_EDITOR
	// Helper method to get the correct value from a SerializedProperty
    private static string GetPropertyValue(SerializedProperty prop) {
        switch (prop.propertyType) {
            case SerializedPropertyType.Integer:         return prop.intValue.ToString();
            case SerializedPropertyType.Boolean:         return prop.boolValue.ToString();
            case SerializedPropertyType.Float:           return prop.floatValue.ToString();
            case SerializedPropertyType.String:          return prop.stringValue;
            case SerializedPropertyType.Color:           return prop.colorValue.ToString();
            case SerializedPropertyType.ObjectReference: return prop.objectReferenceValue ? prop.objectReferenceValue.name : "null";
            case SerializedPropertyType.LayerMask:       return prop.intValue.ToString();
            case SerializedPropertyType.Enum:            return prop.enumDisplayNames[prop.enumValueIndex];
			case SerializedPropertyType.Vector2:         return prop.vector2Value.ToString();
            case SerializedPropertyType.Vector3:         return prop.vector3Value.ToString();
            case SerializedPropertyType.Vector4:         return prop.vector4Value.ToString();
            case SerializedPropertyType.Rect:            return prop.rectValue.ToString();
            case SerializedPropertyType.Bounds:          return prop.boundsValue.ToString();
            case SerializedPropertyType.Quaternion:      return prop.quaternionValue.ToString();
            default:                                     return "Unsupported property type";
        }
    }
    #endif

//     public void CorrectDynamicDataFiles() {
// 		// Iterate over all levels, load their dynamic objects, fix each keyvalue pair to reorder constIndex from slot 19 to 0
// 		for (int i=0;i<14;i++) {
// 			string lName = "CitadelScene_dynamics_level" + i.ToString() + ".dat";
// 			string lP = Utils.SafePathCombine(Application.streamingAssetsPath,lName);
// 			StreamReader sf = Utils.ReadStreamingAsset(lName);
// 			if (sf == null) {
// 				UnityEngine.Debug.Log("Dynamics input file path invalid");
// 				return;
// 			}
// 
// 			string readline;
// 			List<string> readFileList = new List<string>();
// 			using (sf) {
// 				do {
// 					readline = sf.ReadLine();
// 					if (readline == null) break;
// 					
// 					readFileList.Add(readline);
// 				} while (!sf.EndOfStream);
// 				sf.Close();
// 			}
// 
// 			char splitter = Convert.ToChar(SaveLoad.splitChar);
// 			List<string> writeFileList = new List<string>();
// 			for (int j=0;j<readFileList.Count;j++) {
// 				string[] initialEntries = readFileList[j].Split(splitter);
// 				int max = initialEntries.Length;
// 				string[] rewritten = new string[max];
// 				rewritten[0] = initialEntries[19];
// 				// 0|....|17|18|constdex|20|21
// 				// constdex|0|1|2|3|4|...|17|18|20|21
// 				// 0       |1|2|3|4|5|...|18|19|20|21
// 				for (int k=1;k<20;k++) rewritten[k] = initialEntries[k-1];
// 				for (int k=20;k<max;k++) rewritten[k] = initialEntries[k];
// 				StringBuilder rewrite = new StringBuilder();
// 				rewrite.Clear();
// 				for (int k=0;k<(rewritten.Length - 1);k++) {
// 					rewrite.Append(rewritten[k]);
// 					rewrite.Append(SaveLoad.splitChar);
// 				}
// 
// 				rewrite.Append(rewritten[rewritten.Length - 1]);
// 				writeFileList.Add(rewrite.ToString());
// 			}
// 			
// 			string lName2 = "CitadelScene_dynamics_level" + i.ToString() + ".txt";
// 			string lP2 = Utils.SafePathCombine(Application.streamingAssetsPath,lName2);
// 			StreamWriter sw = new StreamWriter(lP2,false,Encoding.ASCII);
// 			if (sw == null) {
// 				UnityEngine.Debug.Log("Dynamics output file path invalid");
// 				return;
// 			}
// 
// 			using (sw) {
// 				for (int j=0;j<writeFileList.Count;j++) {
// 					sw.Write(writeFileList[j]);
// 					sw.Write(Environment.NewLine);
// 				}
// 				sw.Close();
// 			}
// 		}
// 	}

//     public void CorrectLightDataFiles() {
// 		// Iterate over all levels, load their lights, fix each keyvalue pair to add the key plus :
// 		for (int i=0;i<14;i++) {
// 			string lName = "CitadelScene_lights_level" + i.ToString() + ".dat";
// 			string lP = Utils.SafePathCombine(Application.streamingAssetsPath,lName);
// 			StreamReader sf = Utils.ReadStreamingAsset(lName);
// 			if (sf == null) {
// 				UnityEngine.Debug.Log("Lights input file path invalid");
// 				return;
// 			}
// 
// 			string readline;
// 			List<string> readFileList = new List<string>();
// 			using (sf) {
// 				do {
// 					readline = sf.ReadLine();
// 					if (readline == null) break;
// 					
// 					readFileList.Add(readline);
// 				} while (!sf.EndOfStream);
// 				sf.Close();
// 			}
// 
// 			char splitter = Convert.ToChar(SaveLoad.splitChar);
// 			string[] entries = new string[24];
// 			List<string> writeFileList = new List<string>();
// 			for (int j=0;j<readFileList.Count;j++) {
// 				entries = readFileList[j].Split(splitter);
// 				string[] rewritten = new string[24];
// 				rewritten[0] = "localPosition.x:" + entries[0];
// 				rewritten[1] = "localPosition.y:" + entries[1];
// 				rewritten[2] = "localPosition.z:" + entries[2];
// 				rewritten[3] = "localRotation.x:" + entries[3];
// 				rewritten[4] = "localRotation.y:" + entries[4];
// 				rewritten[5] = "localRotation.z:" + entries[5];
// 				rewritten[6] = "localRotation.w:" + entries[6];
// 				rewritten[7] = "localScale.x:" + entries[7];
// 				rewritten[8] = "localScale.y:" + entries[8];
// 				rewritten[9] = "localScale.z:" + entries[9];
// 				rewritten[10] = "intensity:" + entries[10];
// 				rewritten[11] = "range:" + entries[11];
// 				rewritten[12] = "type:" + entries[12];
// 				rewritten[13] = "color.r:" + entries[13];
// 				rewritten[14] = "color.g:" + entries[14];
// 				rewritten[15] = "color.b:" + entries[15];
// 				rewritten[16] = "color.a:" + entries[16];
// 				rewritten[17] = "spotAngle:" + entries[17];
// 				rewritten[18] = "shadows:" + entries[18];
// 				rewritten[19] = "shadowStrength:" + entries[19];
// 				rewritten[20] = "shadowResolution:" + entries[20];
// 				rewritten[21] = "shadowBias:" + entries[21];
// 				rewritten[22] = "shadowNormalBias:" + entries[22];
// 				rewritten[23] = "shadowNearPlane:" + entries[23];
// 
// 				string rewrite = rewritten[0] + SaveLoad.splitChar
// 								 + rewritten[1] + SaveLoad.splitChar
// 								 + rewritten[2] + SaveLoad.splitChar
// 								 + rewritten[3] + SaveLoad.splitChar
// 								 + rewritten[4] + SaveLoad.splitChar
// 								 + rewritten[5] + SaveLoad.splitChar
// 								 + rewritten[6] + SaveLoad.splitChar
// 								 + rewritten[7] + SaveLoad.splitChar
// 								 + rewritten[8] + SaveLoad.splitChar
// 								 + rewritten[9] + SaveLoad.splitChar
// 								 + rewritten[10] + SaveLoad.splitChar
// 								 + rewritten[11] + SaveLoad.splitChar
// 								 + rewritten[12] + SaveLoad.splitChar
// 								 + rewritten[13] + SaveLoad.splitChar
// 								 + rewritten[14] + SaveLoad.splitChar
// 								 + rewritten[15] + SaveLoad.splitChar
// 								 + rewritten[16] + SaveLoad.splitChar
// 								 + rewritten[17] + SaveLoad.splitChar
// 								 + rewritten[18] + SaveLoad.splitChar
// 								 + rewritten[19] + SaveLoad.splitChar
// 								 + rewritten[20] + SaveLoad.splitChar
// 								 + rewritten[21] + SaveLoad.splitChar
// 								 + rewritten[22] + SaveLoad.splitChar
// 								 + rewritten[23] + SaveLoad.splitChar;
// 
// 				writeFileList.Add(rewrite);
// 			}
// 			
// 			string lName2 = "CitadelScene_lights_level" + i.ToString() + ".txt";
// 			string lP2 = Utils.SafePathCombine(Application.streamingAssetsPath,lName2);
// 			StreamWriter sw = new StreamWriter(lP2,false,Encoding.ASCII);
// 			if (sw == null) {
// 				UnityEngine.Debug.Log("Lights output file path invalid");
// 				return;
// 			}
// 
// 			using (sw) {
// 				for (int j=0;j<writeFileList.Count;j++) {
// 					sw.Write(writeFileList[j]);
// 					sw.Write(Environment.NewLine);
// 				}
// 				sw.Close();
// 			}
// 		}
// 	}
    
    // Commented out, all lights already generated.
	public void GenerateLightsDataFile() {
// 		UnityEngine.Debug.Log("Outputting all lights to StreamingAssets/CitadelScene_lights_level" + levelToOutputFrom.ToString() + ".dat");
// 		StringBuilder s1 = new StringBuilder();
// 		List<GameObject> allLights = new List<GameObject>();
// 		Component[] compArray = lightContainers[levelToOutputFrom].GetComponentsInChildren(typeof(Light),true);
// 		for (int i=0;i<compArray.Length;i++) {
// 			if (compArray[i].gameObject.GetComponent<LightAnimation>() != null) {
// 				UnityEngine.Debug.Log("Skipping light with LightAnimation");
// 				continue;
// 			}
// 			if (compArray[i].gameObject.GetComponent<TargetIO>() != null) {
// 				UnityEngine.Debug.Log("Skipping light with TargetIO");
// 				continue;
// 			}
// 			allLights.Add(compArray[i].gameObject);
// 		}
// 
// 		UnityEngine.Debug.Log("Found " + allLights.Count + " lights in level " + levelToOutputFrom.ToString());
// 
// 		string lName = "CitadelScene_lights_level"
// 					   + levelToOutputFrom.ToString() + ".dat";
// 
// 		string lP = Utils.SafePathCombine(Application.streamingAssetsPath,
// 										  lName);
// 
// 		StreamWriter sw = new StreamWriter(lP,false,Encoding.ASCII);
// 		if (sw == null) {
// 			UnityEngine.Debug.Log("Lights output file path invalid");
// 			return;
// 		}
// 
// 		using (sw) {
// 			for (int i=0;i<allLights.Count;i++) {
// 				s1.Clear();
// 				Transform tr = allLights[i].transform;
// 				s1.Append(Utils.SaveTransform(allLights[i].transform));
// 				s1.Append(Utils.splitChar);
// 				Light lit = allLights[i].GetComponent<Light>();
// 				s1.Append(Utils.FloatToString(lit.intensity));
// 				s1.Append(Utils.splitChar);
// 				s1.Append(Utils.FloatToString(lit.range));
// 				s1.Append(Utils.splitChar);
// 				s1.Append(lit.type.ToString());
// 				s1.Append(Utils.splitChar);
// 				s1.Append(Utils.FloatToString(lit.color.r));
// 				s1.Append(Utils.splitChar);
// 				s1.Append(Utils.FloatToString(lit.color.g));
// 				s1.Append(Utils.splitChar);
// 				s1.Append(Utils.FloatToString(lit.color.b));
// 				s1.Append(Utils.splitChar);
// 				s1.Append(Utils.FloatToString(lit.color.a));
// 				s1.Append(Utils.splitChar);
// 				s1.Append(Utils.FloatToString(lit.spotAngle));
// 				s1.Append(Utils.splitChar);
// 				s1.Append(lit.shadows.ToString());
// 				s1.Append(Utils.splitChar);
// 				s1.Append(Utils.FloatToString(lit.shadowStrength));
// 				s1.Append(Utils.splitChar);
// 				s1.Append(lit.shadowResolution);
// 				s1.Append(Utils.splitChar);
// 				s1.Append(Utils.FloatToString(lit.shadowBias));
// 				s1.Append(Utils.splitChar);
// 				s1.Append(Utils.FloatToString(lit.shadowNormalBias));
// 				s1.Append(Utils.splitChar);
// 				s1.Append(Utils.FloatToString(lit.shadowNearPlane));
// 				s1.Append(Utils.splitChar);
// 				s1.Append(lit.cullingMask.ToString());
// 				//UnityEngine.Debug.Log(s1.ToString());
// 				sw.Write(s1.ToString());
// 				sw.Write(Environment.NewLine);
// 			}
// 			sw.Close();
// 		}
	}
/*
	public void LoadLevelDynamicObjects() {
		lm.LoadLevelDynamicObjects(levelToOutputFrom);
	}

	public void UnloadLevelDynamicObjects() {
		lm.UnloadLevelDynamicObjects(levelToOutputFrom,false);
	}

	public void GenerateDynamicObjectsDataFile() {
		UnityEngine.Debug.Log("Outputting all dynamic objects to StreamingAssets/CitadelScene_dynamics_level" + levelToOutputFrom.ToString() + ".dat");
		StringBuilder s1 = new StringBuilder();
		List<GameObject> allDynamicObjects = new List<GameObject>();
		Component[] compArray = dynamicObjectContainers[levelToOutputFrom].GetComponentsInChildren(typeof(SaveObject),true);
		for (int i=0;i<compArray.Length;i++) {
			allDynamicObjects.Add(compArray[i].gameObject);
		}

		UnityEngine.Debug.Log("Found " + allDynamicObjects.Count
							  + " dynamic objects in level "
							  + levelToOutputFrom.ToString());

		string dynName = "CitadelScene_dynamics_level"
					     + levelToOutputFrom.ToString() + ".dat";

		string dynP = Utils.SafePathCombine(Application.streamingAssetsPath,
										    dynName);

		StreamWriter sw = new StreamWriter(dynP,false,Encoding.ASCII);
		if (sw == null) {
			UnityEngine.Debug.Log("Dynamic objects output file path invalid");
			return;
		}

		using (sw) {
			for (int i=0;i<allDynamicObjects.Count;i++) {
				sw.Write(SaveObject.Save(allDynamicObjects[i]));
				sw.Write(Environment.NewLine);
			}
			sw.Close();
		}
	}*/
/*
	public void SaveSelectedObject() {
		string line = SaveObject.Save(gameObjectToSave);
		string sName = "saving_unit_test.dat";
		string sP = Utils.SafePathCombine(Application.streamingAssetsPath,
										  sName);
		StreamWriter sw = new StreamWriter(sP,false,Encoding.ASCII);
		if (sw == null) {
			UnityEngine.Debug.Log("Save unit test output file path invalid");
			return;
		}

		using (sw) {;
			sw.Write(line);
			sw.Write(Environment.NewLine);
			sw.Close();
		}
		UnityEngine.Debug.Log("Saved data for " + gameObjectToSave.name);
	}

	public void LoadSelectedObject() {
		string lName = "saving_unit_test.dat";
		string lP = Utils.SafePathCombine(Application.streamingAssetsPath,
										  lName);

		Utils.ConfirmExistsInStreamingAssetsMakeIfNot(lName);
		StreamReader sf = new StreamReader(lP);
		if (sf == null) {
			UnityEngine.Debug.Log("Save unit test input file path invalid");
			return;
		}

		string readline;
		List<string> readFileList = new List<string>();
		using (sf) {
			do {
				readline = sf.ReadLine();
				if (readline != null) {
					readFileList.Add(readline);
				}
			} while (!sf.EndOfStream);
			sf.Close();
		}

		string[] entries = readFileList[0].Split('|');
		SaveObject.Load(gameObjectToSave,ref entries,-1);
		UnityEngine.Debug.Log("Loaded data for " + gameObjectToSave.name);
	}*/
/*
	public void TEMP_SetFunc_WallChunkIDs() {
		List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
		for (int i=0;i<allParents.Count;i++) {
			Component[] compArray = allParents[i].GetComponentsInChildren(typeof(FuncWall),true); // Find all FuncWall components, including inactive (hence the true here at the end)
			for (int k=0;k<compArray.Length;k++) {
				TEMP_Func_Wall_SetChunkIDs(compArray[k].gameObject);
			}
		}
	}

	public void SetStaticSaveableIDs() {
		#if UNITY_EDITOR
			int idInc = 1000000;
			SaveObject so;
			List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
			for (int i=0;i<allParents.Count;i++) {
				Component[] compArray = allParents[i].GetComponentsInChildren(typeof(SaveObject),true); // find all SaveObject components, including inactive (hence the true here at the end)
				for (int k=0;k<compArray.Length;k++) {
					so = compArray[k].gameObject.GetComponent<SaveObject>();
					so.SaveID = idInc; //add the gameObject associated with all SaveObject components in the scene
					//EditorUtility.SetDirty(so as Object);
					PrefabUtility.RecordPrefabInstancePropertyModifications(so);
					idInc++;
				}
			}
			Scene sc = SceneManager.GetActiveScene();
			//EditorSceneManager.MarkSceneDirty(sc);
			EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
		#endif
	}

	public void ReportMaxSaveableID() {
		#if UNITY_EDITOR
			int idInc = 0;
			SaveObject so;
			List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
			for (int i=0;i<allParents.Count;i++) {
				Component[] compArray = allParents[i].GetComponentsInChildren(typeof(SaveObject),true); // find all SaveObject components, including inactive (hence the true here at the end)
				for (int k=0;k<compArray.Length;k++) {
					so = compArray[k].gameObject.GetComponent<SaveObject>();
					if (so.SaveID > idInc) idInc = so.SaveID;
				}
			}
			UnityEngine.Debug.Log("Largest SaveID: " + idInc.ToString());
		#endif
	}*/

	// Before: 15006, After 8000, for some reason it didn't work for all of them.
	public void RevertAll_m_CastShadows() {
		//List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
		//for (int i=0;i<allParents.Count;i++) {
		//	Component[] compArray = allParents[i].GetComponentsInChildren(typeof(Transform),true);
		//	for (int k=0;k<compArray.Length;k++) {
		//		GameObject go = compArray[k].gameObject;
		//		if (!PrefabUtility.IsPartOfPrefabInstance(go)) continue;
		//		if (!PrefabUtility.HasPrefabInstanceAnyOverrides(go, false)) continue;

		//		List<ObjectOverride> ovides = PrefabUtility.GetObjectOverrides(go,false);
		//		for (int j=0; j < ovides.Count; j++) {
		//			UnityEngine.Object ob = ovides[j].instanceObject;
		//			SerializedObject sob = new UnityEditor.SerializedObject(ob);
		//			SerializedProperty prop = sob.FindProperty("m_CastShadows");
		//			if (prop == null) continue;
		//			if (!prop.prefabOverride) continue;

		//			UnityEngine.Debug.Log("Reverting m_CastShadows");
		//			PrefabUtility.RevertPropertyOverride(prop,InteractionMode.AutomatedAction);
		//		}


		//	}
		//}
	}
	// m_CastShadows
	// m_MotionVectors Before 19057
	// m_LightProbeUsage

// 	void TEMP_Func_Wall_SetChunkIDs(GameObject go) {
// 		UnityEngine.Debug.Log("go.name: " + go.name);
// 		FuncWall fw = go.GetComponent<FuncWall>();
// 		fw.chunkIDs = new int[go.transform.childCount];
// 		GameObject childGO;
// 		for (int i = 0; i < go.transform.childCount; i++) {
// 			childGO = go.transform.GetChild(i).gameObject;
// 			UnityEngine.Debug.Log("childGO.name: " + childGO.name);
// 			PrefabIdentifier pid = childGO.GetComponent<PrefabIdentifier>();
// 			if (pid == null) {
// 				UnityEngine.Debug.Log("ERROR: FuncWall child missing PrefabIdentifier");
// 				continue;
// 			}
// 			fw.chunkIDs[i] = pid.constIndex;
// 		}
// 	}
}
