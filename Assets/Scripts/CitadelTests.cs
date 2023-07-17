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
	public GameObject[] lightContainers; // Can't use LevelManager's since
										 // there is no instance unless in Play
										 // mode.
	public GameObject[] dynamicObjectContainers;
	public GameObject gameObjectToSave;
	public int levelToOutputFrom = 0;
	public LevelManager lm;

	private bool getValparsed;
	private bool[] levelDataLoaded;
	private int getValreadInt;
	private float getValreadFloat;

	public List<GameObject> allGOs;
	public List<GameObject> allParents;


	[HideInInspector] public string buttonLabel = "Run Tests";

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

	public void Run() {
		#if UNITY_EDITOR
		Stopwatch testTimer = new Stopwatch();
		testTimer.Start();

		// Run through all GameObjects and perform all tests
		/*for (i=0;i<allGOs.Count;i++) {
			script = "LogicRelay";
			LogicRelay leray = allGOs[i].GetComponent<LogicRelay>();
			if (leray != null) {
				if (string.IsNullOrWhiteSpace(leray.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == leray.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + leray.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "LogicTimer";
			LogicTimer limer = allGOs[i].GetComponent<LogicTimer>();
			if (limer != null) {
				if (string.IsNullOrWhiteSpace(limer.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == limer.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + limer.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "PuzzleGridPuzzle";
			PuzzleGridPuzzle puzg = allGOs[i].GetComponent<PuzzleGridPuzzle>();
			if (puzg != null) {
				if (string.IsNullOrWhiteSpace(puzg.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == puzg.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + puzg.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "PuzzleWirePuzzle";
			PuzzleWirePuzzle puzw = allGOs[i].GetComponent<PuzzleWirePuzzle>();
			if (puzw != null) {
				if (string.IsNullOrWhiteSpace(puzw.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == puzw.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + puzw.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "QuestBitRelay";
			QuestBitRelay qbr = allGOs[i].GetComponent<QuestBitRelay>();
			if (qbr != null) {
				if (string.IsNullOrWhiteSpace(qbr.target)) { 
					//UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == qbr.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + qbr.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "Trigger";
			Trigger trig = allGOs[i].GetComponent<Trigger>();
			if (trig != null) {
				if (string.IsNullOrWhiteSpace(trig.target)) {
					UnityEngine.Debug.Log(script + " has no target on "
										  + allGOs[i].name + " with parent of "
										  + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == trig.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) {
						UnityEngine.Debug.Log(script
											  + " has no matching targets for "
											  + trig.target + " on "
											  + allGOs[i].name
											  + " with parent of "
											  + allGOs[i].transform.parent.name);
					}
				}
			}

			script = "TriggerCounter";
			TriggerCounter trigc = allGOs[i].GetComponent<TriggerCounter>();
			if (trigc != null) {
				if (string.IsNullOrWhiteSpace(trigc.target)) { 
					UnityEngine.Debug.Log(script + " has no target on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name);
				} else {
					float numtargetsfound = 0;
					for (int m=0;m<allGOs.Count;m++) {
						TargetIO tioTemp = allGOs[m].GetComponent<TargetIO>();
						if (tioTemp != null) {
							if (tioTemp.targetname == trigc.target) numtargetsfound++;
						}
					}
					if (numtargetsfound < 1) { UnityEngine.Debug.Log(script + " has no matching targets for " + trigc.target + " on " + allGOs[i].name + " with parent of " + allGOs[i].transform.parent.name); }
				}
			}

			script = "MeshRenderer";
			MeshRenderer mrend = allGOs[i].GetComponent<MeshRenderer>();
			if (mrend != null) {
				num_MeshRenderer++;
				if (mrend.sharedMaterials.Length < 1) { UnityEngine.Debug.Log(script + " on " + allGOs[i].name + " has 0 materials with parent of " + allGOs[i].transform.parent.name); issueCount_MeshRenderer++; }
				else {
					for (int j=0; j < mrend.sharedMaterials.Length;j++) {
						if (mrend.sharedMaterials[j] == null) { UnityEngine.Debug.Log(script + " on " + allGOs[i].name + " has missing material " + j.ToString() + " with parent of " + allGOs[i].transform.parent.name); issueCount_MeshRenderer++; }
					}
				}
			}

			script = "BioMonitor";
			BioMonitor bio = allGOs[i].GetComponent<BioMonitor>();
			if (bio != null) {
				if (bio.heartRate == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for heartRate");
				if (bio.patchEffects == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for patchEffects");
				if (bio.heartRateText == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for heartRateText");
				if (bio.header == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for header");
				if (bio.patchesActiveText == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for patchesActiveText");
				if (bio.bpmText == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for bpmText");
				if (bio.fatigueDetailText == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for fatigueDetailText");
				if (bio.fatigue == null) UnityEngine.Debug.Log("BUG: BioMonitor missing manually assigned reference for fatigue");
			}

			//script = "Transform";
			//Transform tfm = allGOs[i].GetComponent<Transform>();
			//if (tfm.localScale.x < Mathf.Epsilon) { UnityEngine.Debug.Log(script + " on " + allGOs[i].name + " has negative x scale " + tfm.localScale.x.ToString() + " with parent of " + allGOs[i].transform.parent.name); }
			//if (tfm.localScale.y < Mathf.Epsilon) { UnityEngine.Debug.Log(script + " on " + allGOs[i].name + " has negative y scale " + tfm.localScale.y.ToString() + " with parent of " + allGOs[i].transform.parent.name); }
			//if (tfm.localScale.z < Mathf.Epsilon) { UnityEngine.Debug.Log(script + " on " + allGOs[i].name + " has negative z scale " + tfm.localScale.z.ToString() + " with parent of " + allGOs[i].transform.parent.name); }
		}

		// Ok now print result tallies
		PrintTally("AIController",				issueCount_AIController,				num_AIController);
		PrintTally("AIAnimationController",		issueCount_AIAnimationController,		num_AIAnimationController);
		PrintTally("HealthManager",				issueCount_HealthManager,				num_HealthManagerController);
		PrintTally("ElevatorButton",			issueCount_ElevatorButton,				num_ElevatorButton);
		PrintTally("SearchableItem",			issueCount_SearchableItem,				num_SearchableItem);
		PrintTally("MeshRenderer",				issueCount_MeshRenderer,				num_MeshRenderer);
		PrintTally("TargetIO",					issueCount_TargetIO,					num_TargetIO);
*/
		testTimer.Stop();
		UnityEngine.Debug.Log("All tests completed in " + testTimer.Elapsed.ToString());
		buttonLabel = "Run Tests (Last was: " + testTimer.Elapsed.ToString() + ")";
		#endif
	}

	public struct LightGOData {
		public Vector3 position;
		public Vector3 rotation;
		public Color color;
		public float intensity;
		public bool isSpotlight;
	}

    //private List<GameObject> GetAllObjectsOnlyInScene() {
    //    List<GameObject> objectsInScene = new List<GameObject>();
    //    foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
    //        if (!EditorUtility.IsPersistent(go.transform.root.gameObject)
	//			&& !(go.hideFlags == HideFlags.NotEditable
	//			     || go.hideFlags == HideFlags.HideAndDontSave)) objectsInScene.Add(go);
    //    }

    //    return objectsInScene;
    //}

	public void LoadLevelLights() {
		lm.LoadLevelLights(levelToOutputFrom);
	}

	public void UnloadLevelLights() {
		lm.UnloadLevelLights(levelToOutputFrom);
	}

	public void GenerateLightsDataFile() {
		UnityEngine.Debug.Log("Outputting all lights to StreamingAssets/CitadelScene_lights_level" + levelToOutputFrom.ToString() + ".dat");
		StringBuilder s1 = new StringBuilder();
		List<GameObject> allLights = new List<GameObject>();
		Component[] compArray = lightContainers[levelToOutputFrom].GetComponentsInChildren(typeof(Light),true);
		for (int i=0;i<compArray.Length;i++) {
			if (compArray[i].gameObject.GetComponent<LightAnimation>() != null) {
				UnityEngine.Debug.Log("Skipping light with LightAnimation");
				continue;
			}
			if (compArray[i].gameObject.GetComponent<TargetIO>() != null) {
				UnityEngine.Debug.Log("Skipping light with TargetIO");
				continue;
			}
			allLights.Add(compArray[i].gameObject);
		}

		UnityEngine.Debug.Log("Found " + allLights.Count + " lights in level " + levelToOutputFrom.ToString());

		string lName = "CitadelScene_lights_level"
					   + levelToOutputFrom.ToString() + ".dat";

		string lP = Utils.SafePathCombine(Application.streamingAssetsPath,
										  lName);

		StreamWriter sw = new StreamWriter(lP,false,Encoding.ASCII);
		if (sw == null) {
			UnityEngine.Debug.Log("Lights output file path invalid");
			return;
		}

		using (sw) {
			for (int i=0;i<allLights.Count;i++) {
				s1.Clear();
				Transform tr = allLights[i].transform;
				s1.Append(Utils.SaveTransform(allLights[i].transform));
				s1.Append(Utils.splitChar);
				Light lit = allLights[i].GetComponent<Light>();
				s1.Append(Utils.FloatToString(lit.intensity));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.range));
				s1.Append(Utils.splitChar);
				s1.Append(lit.type.ToString());
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.color.r));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.color.g));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.color.b));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.color.a));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.spotAngle));
				s1.Append(Utils.splitChar);
				s1.Append(lit.shadows.ToString());
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.shadowStrength));
				s1.Append(Utils.splitChar);
				s1.Append(lit.shadowResolution);
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.shadowBias));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.shadowNormalBias));
				s1.Append(Utils.splitChar);
				s1.Append(Utils.FloatToString(lit.shadowNearPlane));
				s1.Append(Utils.splitChar);
				s1.Append(lit.cullingMask.ToString());
				//UnityEngine.Debug.Log(s1.ToString());
				sw.Write(s1.ToString());
				sw.Write(Environment.NewLine);
			}
			sw.Close();
		}
	}

	public void LoadLevelDynamicObjects() {
		lm.LoadLevelDynamicObjects(levelToOutputFrom,dynamicObjectContainers[levelToOutputFrom]);
	}

	public void UnloadLevelDynamicObjects() {
		lm.UnloadLevelDynamicObjects(levelToOutputFrom);
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
			UnityEngine.Debug.Log("Lights output file path invalid");
			return;
		}

		using (sw) {
			for (int i=0;i<allDynamicObjects.Count;i++) {
				sw.Write(SaveObject.Save(allDynamicObjects[i]));
				sw.Write(Environment.NewLine);
			}
			sw.Close();
		}
	}

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
		int index = 5;
		index = SaveObject.Load(gameObjectToSave,ref entries,-1);
		UnityEngine.Debug.Log("Loaded data for " + gameObjectToSave.name + ", contained " + index.ToString() + " entries on the line.");
	}

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
	}

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

	void TEMP_Func_Wall_SetChunkIDs(GameObject go) {
		UnityEngine.Debug.Log("go.name: " + go.name);
		FuncWall fw = go.GetComponent<FuncWall>();
		fw.chunkIDs = new int[go.transform.childCount];
		GameObject childGO;
		for (int i = 0; i < go.transform.childCount; i++) {
			childGO = go.transform.GetChild(i).gameObject;
			UnityEngine.Debug.Log("childGO.name: " + childGO.name);
			PrefabIdentifier pid = childGO.GetComponent<PrefabIdentifier>();
			if (pid == null) {
				UnityEngine.Debug.Log("ERROR: FuncWall child missing PrefabIdentifier");
				continue;
			}
			fw.chunkIDs[i] = pid.constIndex;
		}
	}

	private void PrintTally(string className, int issueCount, int objCount) {
		UnityEngine.Debug.Log(issueCount.ToString() + " " + className + " issues found on " + objCount.ToString() + " gameobjects");
	}

	private bool MissingReference(string className, GameObject go, Component comp, string variableName) {
		string self = (go != null) ? go.name : "errname";
		string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
		if (comp == null) {
			UnityEngine.Debug.Log("BUG: " + className + " is missing reference " + comp.ToString() + "(" + variableName + ") on GameObject " + self + " with parent of " + parent);
			return true;
		} else return false;
	}

	private bool MissingComponent(string className, GameObject go, System.Type type) {
		string self = (go != null) ? go.name : "errname";
		string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
		if (go.GetComponent(type) == null) {
			UnityEngine.Debug.Log("BUG: " + className + " is missing component " + type.ToString() + " on GameObject " + self + " with parent of " + parent);
			return true;
		} else return false;
	}

	private bool BoundsError(string className, GameObject go, int expectedMin, int expectedMax, int foundVal, string valueName) {
		string self = (go != null) ? go.name : "errname";
		string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
		if (foundVal > expectedMax || foundVal < expectedMin) {
			UnityEngine.Debug.Log("BUG: " + className + " has invalid value for "+ valueName +" on GameObject " + self + " with parent of " + parent + ". Expected value in range: "
			+ expectedMin.ToString() + " to " + expectedMax.ToString() + "; Found value: " + foundVal.ToString());
			return true;
		} else return false;
	}

	private bool BoundsError(string className, GameObject go, float expectedMin, float expectedMax, float foundVal, string valueName) {
		string self = (go != null) ? go.name : "errname";
		string parent = (go.transform.parent != null) ? go.transform.parent.name : "none";
		if (foundVal > expectedMax || foundVal < expectedMin) {
			UnityEngine.Debug.Log("BUG: " + className + " has invalid value for " + valueName + " on GameObject " + self + " with parent of " + parent + ". Expected value in range: "
			+ expectedMin.ToString() + " to " + expectedMax.ToString() + "; Found value: " + foundVal.ToString());
			return true;
		} else return false;
	}
}
