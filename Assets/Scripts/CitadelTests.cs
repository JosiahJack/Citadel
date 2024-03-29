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
