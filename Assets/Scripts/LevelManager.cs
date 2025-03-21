using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {
	public int currentLevel;
	public GameObject[] levels;
	public int[] levelSecurity;
	public int[] levelCameraCount;
	public int[] levelSmallNodeCount;
	public int[] levelLargeNodeCount;
	public int[] levelCameraDestroyedCount;
	public int[] levelSmallNodeDestroyedCount;
	public int[] levelLargeNodeDestroyedCount;
	public Transform[] ressurectionLocation;
	public bool[] ressurectionActive;
	public Door[] ressurectionBayDoor;
	public GameObject sky;
	public GameObject sun;
	public GameObject sunSprite;
	public bool superoverride = false;
	public GameObject saturn;
	public GameObject exterior;
	public GameObject exterior_shield;
	public MeshRenderer skyMR;
	public bool[] showSkyForLevel;
	public bool[] showExteriorForLevel;
	public bool[] showSaturnForLevel;
	public NPCSubManager[] npcsm;
	public Level[] levelScripts;
	public GameObject[] geometryContainers;
	public GameObject[] lightContainers;
	public GameObject[] npcContainers;
	public GameObject[] elevatorTargetDestinations;
	public Material rtxEmissive;
	public Mesh sphereMesh;
	public SkyRotate skyRotate;
	public Material pipe_maint2_3_coolant;
// 	public Material dynamicObjectsMaterial;
// 	public Texture2D dynamicObjectsAlbedo;
// 	public Texture2D dynamicObjectsGlow;
// 	public Texture2D dynamicObjectsSpecular;
// 	public Rect[] dynamicObjectsUvs;
// 	public bool changeDynamicMaterial = true;
	[HideInInspector] public List<string>[] DynamicObjectsSavestrings = new List<string>[14];
	
	private bool getValparsed;
	private bool[] levelDataLoaded;
	private int getValreadInt;
	private float getValreadFloat;

	// Singleton instance
	public static LevelManager a;

	public void SetA() {
		if (a == null) a = this;
	}
	
	void Awake () {
		SetA();
		if (currentLevel < 0) {
			if (Const.a == null) return;
			if (Const.a.player1CapsuleMainCameragGO == null) return;

			Camera cam = Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>();
			if (cam == null) return;

			cam.useOcclusionCulling = false; // For debug whiteroom
			return;
		}
		if (currentLevel < 0 || currentLevel > 12) return; // 12 because I don't think I support starting in cyberspace, 13, for testing.

		if (sky == null) Debug.Log("BUG: LevelManager missing manually assigned reference for sky.");
		else sky.SetActive(true);

		SetSkyVisible(true);
		if (ressurectionBayDoor.Length != 8) Debug.Log("BUG: LevelManager ressurectionBayDoor array length not equal to 8.");
		Time.timeScale = Const.defaultTimeScale;
		levelDataLoaded = new bool[14];
		for (int i=0;i<14;i++) levelDataLoaded[i] = false;
		ResetSaveStrings();
		LoadDynamicObjectsSavestrings(true);
		LoadLevelData(currentLevel);
	}
	
	// As the UV's weren't aligning, I elected to do this by hand for max control.
/*	
	private void InitializeDynamicObjectsMaterial() {
		dynamicObjectsAlbedo = new Texture2D(4096,4096);
		int numDynamicObjectTypes = 152;
		Texture2D[] texArray = new Texture2D[numDynamicObjectTypes]; // Keep in sync!!
		dynamicObjectsUvs = new Rect[numDynamicObjectTypes];
		texArray[0] = Const.a.textures[13]; // paper_wad.png
		texArray[1] = Const.a.textures[14]; // beaker.png
		dynamicObjectsUvs = dynamicObjectsAlbedo.PackTextures(texArray,0,4096,false);
		dynamicObjectsMaterial.SetTexture("_MainTex",dynamicObjectsAlbedo);
	}*/
	
	public static bool LevNumInBounds(int levnum) {
		return (levnum >=0 && levnum < 14); // 14 levels
	}

	public static bool LevNumIsNonCyber(int levnum) {
		return (levnum >=0 && levnum < 13); // 13 non-cyber levels
	}

	public void ResetSaveStrings() {
		DynamicObjectsSavestrings = new List<string>[14];
		for (int i=0;i<14;i++) {
			DynamicObjectsSavestrings[i] = new List<string>();
			DynamicObjectsSavestrings[i].Clear();
		}		
	}
	
	// Used in a couple places, bit slow to return list but it's only part of
	// loads and transitions between levels.
	public List<string> ReadDynamicObjectFileList(int lev) {
		List<string> readFileList = new List<string>();
		if (lev > (levelScripts.Length - 1)) return readFileList;
		if (!LevNumInBounds(lev)) return readFileList;

		string dynName = "CitadelScene_dynamics_level"+lev.ToString()+".txt";
// 		Debug.Log("Loading dynamic objects from " + dynName);
		StreamReader sf = Utils.ReadStreamingAsset(dynName);
		if (sf == null) { UnityEngine.Debug.Log("Dynamic objects filepath invalid"); return readFileList; }

		string readline;
		using (sf) {
			do {
				readline = sf.ReadLine();
				if (readline != null) {
					readFileList.Add(readline);
				}
			} while (!sf.EndOfStream);
			sf.Close();
		}
		
		return readFileList;
	}
	
	public void LoadDynamicObjectsSavestrings(bool skipCurrent) {
		ResetSaveStrings();
// 		Debug.Log("Loading dynamic objects save strings into LevelManager "
// 				  + "array from the .dat files in StreamingAssets");
		
		for (int i=0;i<14;i++) {			
			List<string> readFileList = ReadDynamicObjectFileList(i);
			for (int j=0;j<readFileList.Count;j++) {
				DynamicObjectsSavestrings[i].Add(readFileList[j]);
			}
		}
	}
	
	public void SetSkyVisible(bool on) {
		skyMR.enabled = (on && showSkyForLevel[currentLevel]);
		saturn.SetActive(on && showSaturnForLevel[currentLevel]);
		exterior.SetActive(on && showExteriorForLevel[currentLevel]);
		sun.SetActive(Const.a.GraphicsShadowMode >= 1 && on);
		sunSprite.SetActive(on && showSaturnForLevel[currentLevel]);
		if (Const.a == null) return;
		if (Const.a.questData == null) return;
		
		exterior_shield.SetActive(on && showExteriorForLevel[currentLevel]
								  && Const.a.questData.ShieldActivated);
	}

	public void CyborgConversionToggleForCurrentLevel() {
		if (!LevNumInBounds(currentLevel)) return;
	    
		if (currentLevel == 6) {
			if (ressurectionActive[currentLevel]) {
				ressurectionActive[currentLevel] = false;
				ressurectionActive[10] = false;
				ressurectionActive[11] = false;
				ressurectionActive[12] = false;
			} else {
				ressurectionActive[currentLevel] = true;
				ressurectionActive[10] = true;
				ressurectionActive[11] = true;
				ressurectionActive[12] = true;
			}
		} else {
			ressurectionActive[currentLevel] = !ressurectionActive[currentLevel]; // Toggle current level.
		}
	}

	public bool RessurectPlayer() {
		if (!ressurectionActive[currentLevel]) return false;

		if (currentLevel == 10 ||currentLevel == 11 ||currentLevel == 12) {
			LoadLevel(6,ressurectionLocation[currentLevel].position);
			ressurectionBayDoor[6].ForceClose();
		} else {
			if (currentLevel <= 7 && currentLevel >= 0) {
				ressurectionBayDoor[currentLevel].ForceClose();
			}

			if (currentLevel >= 0 || currentLevel < 13) {
				Transform plyr = PlayerReferenceManager.a.playerCapsule.transform;
				Vector3 spot = ressurectionLocation[currentLevel].position;
				plyr.position = transform.TransformPoint(spot);
			}
		}

		// Activate death screen and readouts for
		// "BRAIN ACTIVITY SATISFACTORY..."            ya debatable right
		// etc. etc.
		PlayerReferenceManager.a.playerDeathRessurectEffect.SetActive(true);
		Music.a.PlayTrack(currentLevel,TrackType.Revive,MusicType.Override);
		PlayerMovement.a.ressurectingFinished = PauseScript.a.relativeTime + 3f;
		return true;
	}

	// Make sure that unneeded objects are unloaded
	public void UnloadLevelData(int levnum) {
		if (!LevNumIsNonCyber(levnum)) return; // In a test or editor space.
		if (!levelDataLoaded[levnum]) return; // Already cleared.

		UnloadLevelLights(levnum);
		UnloadLevelGeometry(levnum);
 		UnloadLevelDynamicObjects(levnum,true);
		levelDataLoaded[levnum] = false;
	}

	// Make sure relevant data and objects are loaded in and present for the level.
	public void LoadLevelData(int levnum) {
		if (QuestLogNotesManager.a != null) QuestLogNotesManager.a.NotifyLevelChange(currentLevel);

		if (!LevNumInBounds(currentLevel)) { // In a test or editor space.
			levelDataLoaded[levnum] = true;
			return;
		}
		if (levelDataLoaded[levnum]) return; // Already loaded.

// 		Debug.Log("Loading level data for " + levnum.ToString());
		LoadLevelLights(levnum);
		LoadLevelGeometry(levnum);
		LoadLevelDynamicObjects(levnum);
		Music.a.LoadLevelMusic(levnum);
		levelDataLoaded[levnum] = true;
	}

	public void LoadLevel(int levnum, Vector3 targetPosition) {
		if (!LevNumInBounds(levnum)) { Debug.LogWarning("levnum out of bounds"); return; }

		// NOTE: Check this first since the button for the current level has a null destination.  This is fine and expected.
		if (currentLevel == levnum) { Const.sprint(Const.a.stringTable[9]); return; } //Already there

		MFDManager.a.TurnOffElevatorPad();
// 		Debug.Log("Cleared GUI Over Button state from clicking on elevator button in MFD side pane");
		GUIState.a.ClearOverButton();
		if (targetPosition.x == 0 && targetPosition.y == 0 && targetPosition.z == 0) {
			switch(levnum) {
				case 0:  targetPosition = elevatorTargetDestinations[25].transform.position; break;
				case 1:  targetPosition =  elevatorTargetDestinations[0].transform.position; break;
				case 2:  targetPosition =  elevatorTargetDestinations[1].transform.position; break;
				case 3:  targetPosition =  elevatorTargetDestinations[3].transform.position; break;
				case 4:  targetPosition =  elevatorTargetDestinations[6].transform.position; break;
				case 5:  targetPosition =  elevatorTargetDestinations[7].transform.position; break;
				case 6:  targetPosition =  elevatorTargetDestinations[9].transform.position; break;
				case 7:  targetPosition = elevatorTargetDestinations[17].transform.position; break;
				case 8:  targetPosition = elevatorTargetDestinations[19].transform.position; break;
				case 9:  targetPosition = elevatorTargetDestinations[21].transform.position; break;
				case 10: targetPosition = elevatorTargetDestinations[22].transform.position; break;
				case 11: targetPosition = elevatorTargetDestinations[23].transform.position; break;
				case 12: targetPosition = elevatorTargetDestinations[24].transform.position; break;
			}
		}

				 
		// Return to level from cyberspace.
		PlayerReferenceManager.a.playerCapsule.transform.position = targetPosition;
		currentLevel = levnum; // Set current level to be the new level
		DisableAllNonOccupiedLevelsExcept(currentLevel);
		System.GC.Collect();
		System.GC.WaitForPendingFinalizers();
		DynamicCulling.camPositions = new Dictionary<GameObject, Vector3>();
		levels[levnum].SetActive(true); // enable new level
		PlayerReferenceManager.a.playerCurrentLevel = levnum;
		if (currentLevel == 2 && AutoSplitterData.missionSplitID == 0) {
			AutoSplitterData.missionSplitID++; // 1 - Medical split - we are now on level 2
			Debug.Log("AutoSplitterData missionSplitID incremented: " + AutoSplitterData.missionSplitID.ToString());
		}
		
		PostLoadLevelSetupSystems();
		if (currentLevel != 13) {
			DynamicCulling.a.Cull_Init();
			StartCoroutine(DelayedCull());
		}
	}
	
	public IEnumerator DelayedCull() {
		yield return new WaitForSeconds(0.5f);
		DynamicCulling.a.CullCore(); // For Level 10, visible screen with camera view can't update until cams awake.
	}

	public void LoadLevelFromSave(int levnum) {
		if (!LevNumInBounds(levnum)) return;

// 		Debug.Log("LevelManager LoadLevelFromSave()");
		LoadLevelData(levnum); // Let this function check and load data if it isn't yet.
		currentLevel = levnum; // Set current level to be the new level
		DisableAllNonOccupiedLevelsExcept(currentLevel); // Unload last level.
		levels[currentLevel].SetActive(true); // Load new level
		PostLoadLevelSetupSystems();
	}

	private void PostLoadLevelSetupSystems() {
		Music.a.inCombat = false;
		Music.a.SFXMain.Stop();
		Music.a.SFXOverlay.Stop();
		Music.a.levelEntry = true;
		PlayerHealth.a.radiationArea = false;
		LoadLevelData(currentLevel);
		Automap.a.SetAutomapExploredReference(currentLevel);
		Automap.a.automapBaseImage.overrideSprite = Automap.a.automapsBaseImages[currentLevel];
		Const.a.ClearActiveAutomapOverlays(); // After other levels turned off.
		Const.a.ResetPauseLists();
		SetSkyVisible(true);
		Config.SetLanguage(); // Update all translatable text.
// 		Const.a.ClearPrefabs();
		System.GC.Collect();
		System.GC.WaitForPendingFinalizers();
		Resources.UnloadUnusedAssets();
	}

	public void DisableAllNonOccupiedLevelsExcept(int occupiedLevel) {
		for (int i=0;i<levels.Length;i++) {
			if (i == occupiedLevel) continue;

			UnloadLevelData(i);
			if (levels[i] != null) levels[i].SetActive(false);
		}
	}

	public GameObject GetCurrentDynamicContainer() { // Does not return null
		if (!LevNumInBounds(currentLevel)) {
			return levelScripts[1].dynamicObjectsContainer;
		}

        return levelScripts[currentLevel].dynamicObjectsContainer;
	}

	public GameObject GetCurrentGeometryContainer() { // Does not return null
		if (!LevNumInBounds(currentLevel)) {
			return levelScripts[1].geometryContainer;
		}

        return levelScripts[currentLevel].geometryContainer;
	}

	public GameObject GetCurrentLightsStaticImmutableContainer() {
		if (!LevNumInBounds(currentLevel)) {
			return levelScripts[1].lightsStaticImmutable;
		}
		
		return levelScripts[currentLevel].lightsStaticImmutable;
	}
	
	public GameObject GetCurrentStaticImmutableContainer() { // Does not return null
		if (!LevNumInBounds(currentLevel)) {
			return levelScripts[1].staticObjectsImmutable;
		}

		return levelScripts[currentLevel].staticObjectsImmutable;
	}

	public GameObject GetCurrentStaticSaveableContainer() { // Does not return null
		if (!LevNumInBounds(currentLevel)) {
			return levelScripts[1].staticObjectsSaveable;
		}

		return levelScripts[currentLevel].staticObjectsSaveable;
	}

	public GameObject GetCurrentDoorsContainer() { // Does not return null
		if (!LevNumInBounds(currentLevel)) {
			return levelScripts[1].doorsStaticSaveable;
		}

		return levelScripts[currentLevel].doorsStaticSaveable;
	}

	public GameObject GetCurrentLightsContainer() { // Does not return null
		if (!LevNumInBounds(currentLevel)) {
			return levelScripts[1].lightsStaticImmutable;
		}

		return levelScripts[currentLevel].lightsStaticImmutable;
	}
	
	public GameObject GetRequestedLightsStaticImmutableContainer(int index) {
		if (!LevNumInBounds(index)) {
			return levelScripts[1].lightsStaticImmutable;
		}
		
		return levelScripts[index].lightsStaticImmutable;
	}

	public GameObject GetRequestedLevelDynamicContainer(int index) {
		if (!LevNumInBounds(currentLevel)) {
			return levelScripts[1].dynamicObjectsContainer; // Default to Medical level
		}
		
        return levelScripts[index].dynamicObjectsContainer;
	}

	public GameObject GetRequestedLevelNPCContainer(int index) {
		if (!LevNumInBounds(currentLevel)) {
			return npcContainers[1]; // Default to Medical level
		}

        return npcContainers[index];
	}

	public int GetInstantiateParent(GameObject go, bool isNPC,
									PrefabIdentifier prefID) {

		Transform parTr = go.transform.parent;
		if (parTr == null) return -1;

		GameObject par = parTr.gameObject;
		// func_wall exception.
		if (prefID.constIndex == 517) {
			Transform parpar = par.transform.parent;
			if (parpar != null) par = par.transform.parent.gameObject;
		}

		if (par == null) return -1;

		for (int i=0; i < 14; i++) {
			if (isNPC && par == npcContainers[i]) return i;
			else if (par == levelScripts[i].dynamicObjectsContainer) return i;
		}

		return -1;
	}

	public void SetInstantiateParent(int lev, GameObject go, bool isNPC) {
		if (!LevNumInBounds(lev)) return;

		Transform par = null;
		if (isNPC) {
			GameObject parNPC = GetRequestedLevelNPCContainer(lev);
			if (parNPC != null) par = parNPC.transform;
		} else {
			GameObject parDyn = GetRequestedLevelDynamicContainer(lev);
			if (parDyn != null) par = parDyn.transform;
		}

		if (par == null) return;
		if (go.transform.parent == par) return;

		go.transform.SetParent(par);
	}

	public int GetCurrentLevelSecurity() {
		if (Const.a.difficultyMission < 1) return 0;
		if (!LevNumInBounds(currentLevel)) return 0;
		if (superoverride) return 0; // tee hee we are SHODAN, no security blocks in place
		return levelSecurity[currentLevel];
	}

	// Typical level
	// 4 CPU nodes
	// 20 cameras
	// 100% = 4x + 20y
	// Assuming that a good camera percentage is 2-3%, CPU % would be about 10-15 each
	public void ReduceCurrentLevelSecurity(SecurityType stype) {
		if (!LevNumIsNonCyber(currentLevel)) return;

		float camScore = 4;
		float nodeSmallScore = 10;
		float nodeLargeScore = 27;
		float secscoreTotal = (levelCameraCount[currentLevel] * camScore) + (levelSmallNodeCount[currentLevel] * nodeSmallScore) + (levelLargeNodeCount[currentLevel] * nodeLargeScore);
		//secscoreTotal = 106 for medical level
		float secDrop = camScore; // default to camScore
		switch (stype) {
			case SecurityType.None: return;
			case SecurityType.Camera: secDrop = ((camScore/secscoreTotal) * 100); levelCameraDestroyedCount[currentLevel]++; break; // 1 camera divided by the total, so 2/ say (40+60) = 2/100 = 0.02, or 2% using the example numbers above
			case SecurityType.NodeSmall: secDrop = ((nodeSmallScore/secscoreTotal) * 100); levelSmallNodeDestroyedCount[currentLevel]++; break;
			case SecurityType.NodeLarge: secDrop = ((nodeLargeScore/secscoreTotal) * 100); levelLargeNodeDestroyedCount[currentLevel]++; break;
		}
		levelSecurity [currentLevel] -= (int)secDrop;
		if (levelSecurity [currentLevel] < 0) levelSecurity [currentLevel] = 0;
		if ((levelLargeNodeDestroyedCount[currentLevel] == levelLargeNodeCount[currentLevel]) && (levelSmallNodeDestroyedCount[currentLevel] == levelSmallNodeCount[currentLevel]) && (levelCameraDestroyedCount[currentLevel] == levelCameraCount[currentLevel])) {
			levelSecurity[currentLevel] = 0;
		}
		Const.sprint(Const.a.stringTable[306] + levelSecurity[currentLevel].ToString() + Const.a.stringTable[307]);

		// Notify quest log if all nodes were destroyed
		if (levelLargeNodeDestroyedCount[currentLevel] == levelLargeNodeCount[currentLevel]) {
			if (QuestLogNotesManager.a != null) QuestLogNotesManager.a.NodesDestroyed(currentLevel);
		}
	}

	public bool PosWithinLeafBounds(Vector3 pos, BoxCollider b) {
		if (b == null) { Debug.Log("BUG: null BoxCollider passed to PosWithinLeafBounds!"); return false; }

		float xMax = b.bounds.max.x;
		float xMin = b.bounds.min.x;
		float yMax = b.bounds.max.z; // Yes Unity you stupid engine...z is y people!  
		float yMin = b.bounds.min.z; // NO I'm not making a 2D game....it's y and I'm sticking to it
		if ((pos.x < xMax && pos.x > xMin) && (pos.z < yMax && pos.z > yMin)) {
			return true;
		}

		return false;
	}
	
	public void UnloadLevelGeometry(int curlevel) {
		if (curlevel > (geometryContainers.Length - 1)) return;
		if (curlevel < 0) return;

		List<GameObject> deleteMes = new List<GameObject>();
		Transform parent = geometryContainers[curlevel].transform;
		int children = parent.childCount;
		for (int i=0;i<children;i++) deleteMes.Add(parent.GetChild(i).gameObject);
		for (int i=0;i<deleteMes.Count;i++) {
			if (deleteMes[i] != null) DestroyImmediate(deleteMes[i]);
		}
	}
	
	public void LoadLevelGeometry(int curlevel) {
		if (curlevel > (geometryContainers.Length - 1)) return;
		if (curlevel < 0) return;
		
		string gName = "CitadelScene_geometry_level"+curlevel.ToString()+".txt";
		StreamReader sf = Utils.ReadStreamingAsset(gName);
		if (sf == null) {
			UnityEngine.Debug.Log("Geometry input file path invalid");
			return;
		}

		string readline;
		List<string> readFileList = new List<string>();
		int lineNum = 0;
		char splitter = Convert.ToChar(SaveLoad.splitChar);
		Transform parent,child;
		int count = 0;
		Light lit;
		List<Light> chunkLights = new List<Light>();
		GameObject go;
		using (sf) {
			do {
				readline = sf.ReadLine();
				if (readline == null) break;

				string[] entries = readline.Split(splitter);
				
				go = SaveLoad.LoadPrefab(ref entries,lineNum,curlevel);
				if (go != null) parent = go.transform;
				else parent = null;
				
				if (parent != null) {
					// Move all lights off of the prefab and into the cullable lights container.
					child = null;
					count = parent.childCount;
					for (int i=0;i<count;i++) {
						child = parent.GetChild(i);
						lit = child.GetComponent<Light>();
						if (lit != null) chunkLights.Add(lit);
					}
				}
				lineNum++;
			} while (!sf.EndOfStream);
			
			for (int i=0;i<chunkLights.Count;i++) {
				lit = chunkLights[i];
				lit.gameObject.name = "ChunkLight_" + lit.gameObject.name;
				lit.transform.parent = lightContainers[curlevel].transform;
// 				UnityEngine.Debug.Log("Moved light off of " + lit.gameObject.name);
			}
			
			sf.Close();
		}
	}

	public void UnloadLevelLights(int curlevel) {
		if (curlevel > 12) return;
		if (curlevel > (lightContainers.Length - 1)) return;
		if (curlevel < 0) return;

		Component[] compArray = 
		  lightContainers[curlevel].GetComponentsInChildren(typeof(Light),true);

		GameObject go = null;
		int litCount = compArray.Length;
		for (int i=0;i<litCount;i++) {
			go = compArray[i].gameObject;
			if (go.GetComponent<LightAnimation>() != null) continue;
			if (go.GetComponent<TargetIO>() != null) continue;

			int childCount = go.transform.childCount;
			for (int j = 0; j < childCount; j++) {
				Transform child = go.transform.GetChild(j);
				MeshRenderer mr = child.GetComponent<MeshRenderer>();
				if (mr != null && mr.material != null) {
					Material mat = mr.material;
					mr.material = null; // Clear reference
					DestroyImmediate(mat); // Destroy the material instance
				}
			}

			DestroyImmediate(go); // Dangerous isn't it :D
		}
		compArray = null;
	}

	public void LoadLevelLights(int curlevel) {
		if (curlevel > 12) return;
		if (curlevel > (lightContainers.Length - 1)) return;
		if (curlevel < 0) return;

		string lName = "CitadelScene_lights_level"+curlevel.ToString()+".txt";
		StreamReader sf = Utils.ReadStreamingAsset(lName);
		if (sf == null) {
			UnityEngine.Debug.Log("Lights input file path invalid");
			return;
		}

		string readline;
		List<string> readFileList = new List<string>();
		int lineNum = 0;
		char splitter = Convert.ToChar(SaveLoad.splitChar);
		using (sf) {
			do {
				readline = sf.ReadLine();
				if (readline == null) break;
				
				string[] entries = readline.Split(splitter);
				SaveLoad.LoadPrefab(ref entries,lineNum,curlevel);
				lineNum++;
			} while (!sf.EndOfStream);
			sf.Close();
		}
	}

	public void UnloadLevelDynamicObjects(int curlevel, bool saveExisting) {
		Transform tr = GetRequestedLevelDynamicContainer(curlevel).transform;
		if (saveExisting) {
			List<GameObject> allDynamicObjects = new List<GameObject>();
			Component[] compArray = tr.gameObject.GetComponentsInChildren(typeof(SaveObject),true);
			for (int i=0;i<compArray.Length;i++) {
				allDynamicObjects.Add(compArray[i].gameObject);
			}

			DynamicObjectsSavestrings[curlevel].Clear(); // Empty list.
			for (int i=0;i<allDynamicObjects.Count;i++) {
				DynamicObjectsSavestrings[curlevel].Add(SaveObject.Save(allDynamicObjects[i]));
			}
		}
		
		// Iterate over all gameobjects at first level within.
		for (int i=(tr.childCount-1);i>=0;i--) {
			DestroyImmediate(tr.GetChild(i).gameObject); // Go going, gone!
		}
	}

	public void UnloadLevelNPCs(int curlevel) {
		GameObject go = npcContainers[curlevel];
		Component[] compArray = go.GetComponentsInChildren(typeof(SaveObject),
														   true);

		for (int i=0;i<compArray.Length;i++) {
			AIController aic = compArray[i].gameObject.GetComponent<AIController>();
			if (aic == null) {
				UnityEngine.Debug.Log("AIController missing on "
									  + "child " + compArray[i].gameObject.name
									  + " of NPC container " + go.name);
			} else {
				if (aic != null) {
					if (aic.healthManager != null) {
						Image over = aic.healthManager.linkedOverlay;
						if (over != null) {
							Utils.DisableImage(over);
							Utils.Deactivate(over.gameObject);
						}
					}
				}
			}

			DestroyImmediate(compArray[i].gameObject);
		}
		compArray = null;
	}

	public void LoadLevelDynamicObjects(int curlevel) {
		if (curlevel > (levelScripts.Length - 1)) return;
		if (curlevel < 0) return;

		string[] entries;
// 		MeshRenderer mr;
		GameObject dynGO;
		char splitter = Convert.ToChar(SaveLoad.splitChar);
		for (int i=0;i<DynamicObjectsSavestrings[curlevel].Count;i++) {
			entries = DynamicObjectsSavestrings[curlevel][i].Split(splitter);
			if (entries.Length <= 1) continue;
			
			dynGO = SaveLoad.LoadPrefab(ref entries,0,curlevel);
			if (dynGO == null) continue;

			int constIndex = Utils.GetIntFromString(entries[0],"constIndex");
			
// 			if (changeDynamicMaterial) {
// 				mr = dynGO.GetComponent<MeshRenderer>();
// 				if (mr == null) continue;
// 				
// 				mr.sharedMaterial = dynamicObjectsMaterial;
// 				MeshFilter mf = dynGO.GetComponent<MeshFilter>();
// 				switch(constIndex) {
// 					case 307: mf.sharedMesh.uv = GetUVMappedToSubspace(mf.sharedMesh,dynamicObjectsUvs[0]); break;
// 					case 309: mf.sharedMesh.uv = GetUVMappedToSubspace(mf.sharedMesh,dynamicObjectsUvs[1]); break;
// 				}
// 			}
		}

		DynamicObjectsSavestrings[curlevel].Clear();
	}
	
// 	private Vector2[] GetUVMappedToSubspace(Mesh mesh, Rect uvSpace) {
// 		UnityEngine.Debug.Log("uvSpace: " + uvSpace.ToString());
// 		Vector2[] uvsIn = mesh.uv;
// 		Vector2[] newUVs = new Vector2[uvsIn.Length];			
// 		for (int u=0;u<uvsIn.Length;u++) {
// 			newUVs[u].x = (uvsIn[u].x * uvSpace.width) + uvSpace.xMin;
// 			newUVs[u].y = (uvsIn[u].y * uvSpace.height) + uvSpace.yMin;
// 		}
// 		
// 		return newUVs;
// 	}

	public void CheatLoadLevel(int ind) {
		if (ind == 10) {
			LoadLevel(10,PlayerMovement.a.cheatG1Spawn.position);
		} else if (ind == 11) {
			LoadLevel(11,PlayerMovement.a.cheatG2Spawn.position);
		} else if (ind == 12) {
			LoadLevel(12,PlayerMovement.a.cheatG4Spawn.position);
		} else {
			LoadLevel(ind,ressurectionLocation[ind].position);
		}
	}

	public static string Save(GameObject go) {
		int i=0;
		LevelManager lvm = go.GetComponent<LevelManager>();
		StringBuilder s1 = new StringBuilder();
		s1.Clear(); // keep reusing s1
		s1.Append(Utils.UintToString(LevelManager.a.currentLevel,"currentLevel"));
		s1.Append(Utils.splitChar);
		for (i=0;i<14;i++) { s1.Append(Utils.UintToString(LevelManager.a.levelSecurity[i],"levelSecurity["+i.ToString()+"]")); s1.Append(Utils.splitChar); }
		for (i=0;i<14;i++) { s1.Append(Utils.UintToString(LevelManager.a.levelCameraDestroyedCount[i],"levelCameraDestroyedCount["+i.ToString()+"]")); s1.Append(Utils.splitChar); }
		for (i=0;i<14;i++) { s1.Append(Utils.UintToString(LevelManager.a.levelSmallNodeDestroyedCount[i],"levelSmallNodeDestroyedCount["+i.ToString()+"]")); s1.Append(Utils.splitChar); }
		for (i=0;i<14;i++) { s1.Append(Utils.UintToString(LevelManager.a.levelLargeNodeDestroyedCount[i],"levelLargeNodeDestroyedCount["+i.ToString()+"]")); s1.Append(Utils.splitChar); }
		for (i=0;i<13;i++) { s1.Append(Utils.BoolToString(LevelManager.a.ressurectionActive[i],"ressurectionActive["+i.ToString()+"]")); s1.Append(Utils.splitChar); }
		s1.Append(Utils.BoolToString(LevelManager.a.ressurectionActive[13],"ressurectionActive[13]"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		LevelManager lvm = go.GetComponent<LevelManager>();
		int i = 0;
		int levelNum = Utils.GetIntFromString(entries[index],"currentLevel"); index++;
		LevelManager.a.LoadLevelFromSave(levelNum);
		for (i=0;i<14;i++) { LevelManager.a.levelSecurity[i] = Utils.GetIntFromString(entries[index],"levelSecurity[" + i.ToString() + "]"); index++; }
		for (i=0;i<14;i++) { LevelManager.a.levelCameraDestroyedCount[i] = Utils.GetIntFromString(entries[index],"levelCameraDestroyedCount[" + i.ToString() + "]"); index++; }
		for (i=0;i<14;i++) { LevelManager.a.levelSmallNodeDestroyedCount[i] = Utils.GetIntFromString(entries[index],"levelSmallNodeDestroyedCount[" + i.ToString() + "]"); index++; }
		for (i=0;i<14;i++) { LevelManager.a.levelLargeNodeDestroyedCount[i] = Utils.GetIntFromString(entries[index],"levelLargeNodeDestroyedCount[" + i.ToString() + "]"); index++; }
		for (i=0;i<14;i++) { LevelManager.a.ressurectionActive[i] = Utils.GetBoolFromString(entries[index],"ressurectionActive[" + i.ToString() + "]"); index++; }
		return index;
	}
	
	void OnDestroy() {
		levels = null;
		ressurectionLocation = null;
		ressurectionBayDoor = null;
		sky = null;
		sun = null;
		sunSprite = null;
		saturn = null;
		exterior = null;
		exterior_shield = null;
		skyMR = null;
		npcsm = null;
		levelScripts = null;
		geometryContainers = null;
		lightContainers = null;
		npcContainers = null;
		elevatorTargetDestinations = null;
		rtxEmissive = null;
		sphereMesh = null;
		pipe_maint2_3_coolant = null;
// 		dynamicObjectsMaterial = null;
// 		dynamicObjectsAlbedo = null;
// 		dynamicObjectsGlow = null;
// 		dynamicObjectsSpecular = null;
// 		dynamicObjectsUvs = null;
		DynamicObjectsSavestrings = null;
		if (a == this) a = null;
	}
}
