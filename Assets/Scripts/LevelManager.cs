using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

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
	public bool superoverride = false;
	public GameObject saturn;
	public GameObject exterior;
	public MeshRenderer skyMR;
	public bool[] showSkyForLevel;
	public bool[] showExteriorForLevel;
	public bool[] showSaturnForLevel;
	public NPCSubManager[] npcsm;
	public enum SecurityType : byte {None,Camera,NodeSmall,NodeLarge};
	public Level[] levelScripts;
	public GameObject[] lightContainers;

	private bool getValparsed;
	private bool[] levelDataLoaded;
	private int getValreadInt;
	private float getValreadFloat;
	private CultureInfo en_US_Culture = new CultureInfo("en-US");

	// Singleton instance
	public static LevelManager a;

	void Awake () {
		a = this;
		if (currentLevel < 0 || currentLevel > 12) return; // 12 because I don't think I support starting in cyberspace, 13, for testing.

		//PlayerReferenceManager.a.playerCurrentLevel = currentLevel;
		if (sky == null) Debug.Log("BUG: LevelManager missing manually assigned reference for sky.");
		else sky.SetActive(true);
		if (showSkyForLevel[currentLevel]) skyMR.enabled = true; else skyMR.enabled = false;
		if (showExteriorForLevel[currentLevel]) exterior.SetActive(true); else exterior.SetActive(false);
		if (showSaturnForLevel[currentLevel]) saturn.SetActive(true); else saturn.SetActive(false);
		if (ressurectionBayDoor.Length != 8) Debug.Log("BUG: LevelManager ressurectionBayDoor array length not equal to 8.");
		Time.timeScale = Const.a.defaultTimeScale;
		levelDataLoaded = new bool[14];
		for (int i=0;i<14;i++) levelDataLoaded[i] = false;
		LoadLevelData(currentLevel);
	}

	public void CyborgConversionToggleForCurrentLevel() {
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

	public bool RessurectPlayer (GameObject currentPlayer) {
		// Note:  currentPlayer is the main Player gameObject that contains PlayerReferenceManager.
		if (currentPlayer == null) { Const.sprint("BUG: LevelManager cannot find current player for RessurectPlayer."); return false; } // Prevent possible error if wrong player is passed.

		if (ressurectionActive[currentLevel]) {
			if (currentLevel == 10 ||currentLevel == 11 ||currentLevel == 12) {
				LoadLevel(6,ressurectionLocation[currentLevel].gameObject,ressurectionLocation[currentLevel].position);
				ressurectionBayDoor[6].ForceClose();
			} else {
				if (currentLevel <= 7 && currentLevel >= 0) ressurectionBayDoor[currentLevel].ForceClose();
				if (currentLevel != 13) PlayerReferenceManager.a.playerCapsule.transform.position = transform.TransformPoint(ressurectionLocation[currentLevel].position); //teleport to ressurection chamber
			}
			PlayerReferenceManager.a.playerDeathRessurectEffect.SetActive(true); // activate death screen and readouts for "BRAIN ACTIVITY SATISFACTORY"            ya debatable right
			Music.a.PlayTrack(currentLevel,Music.TrackType.Revive,Music.MusicType.Override);
			PlayerMovement.a.ressurectingFinished = PauseScript.a.relativeTime + 3f;
			return true;
		}
		return false;
	}

	// Make sure that unneeded objects are unloaded
	public void UnloadLevelData (int levnum) {
		if (levnum < 0 || levnum > 12) return; // Not in a level, in a test or editor space.
		if (!levelDataLoaded[levnum]) return; // Already cleared.

		UnloadLevelLights(levnum);
		levelDataLoaded[levnum] = false;
	}

	// Make sure relevant data and objects are loaded in and present for the level.
	public void LoadLevelData (int levnum) {
		if (levnum < 0 || levnum > 12) return; // Not in a level, in a test or editor space.
		if (levelDataLoaded[levnum]) return; // Already loaded.

		LoadLevelLights(levnum);
		if (QuestLogNotesManager.a != null) QuestLogNotesManager.a.NotifyLevelChange(currentLevel);
		levelDataLoaded[levnum] = true;
	}

	public void LoadLevel (int levnum, GameObject targetDestination, Vector3 targetPosition) {
		// NOTE: Check this first since the button for the current level has a null destination.  This is fine and expected.
		if (currentLevel == levnum) { Const.sprint(Const.a.stringTable[9]); return; } //Already there

		int lastlev = currentLevel;
		MFDManager.a.TurnOffElevatorPad();
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
		if (targetDestination != null) PlayerReferenceManager.a.playerCapsule.transform.position = targetDestination.transform.position; // Put player in the new level
		else if (targetPosition != null) PlayerReferenceManager.a.playerCapsule.transform.position = targetPosition; // Return to level from cyberspace.

		PlayerMovement.a.SetAutomapExploredReference(levnum);
		PlayerMovement.a.automapBaseImage.overrideSprite = PlayerMovement.a.automapsBaseImages[levnum];
		Music.a.inCombat = false;
		Music.a.SFXMain.Stop();
		Music.a.SFXOverlay.Stop();
		Music.a.levelEntry = true;
		levels[levnum].SetActive(true); // enable new level
		PlayerReferenceManager.a.playerCurrentLevel = levnum;
		currentLevel = levnum; // Set current level to be the new level

		LoadLevelData(currentLevel);
		DisableAllNonOccupiedLevels(currentLevel);
		if (showSkyForLevel[currentLevel]) skyMR.enabled = true; else skyMR.enabled = false;
		if (showSaturnForLevel[currentLevel]) saturn.SetActive(true); else saturn.SetActive(false);
		if (showExteriorForLevel[currentLevel]) exterior.SetActive(true); else exterior.SetActive(false);
		if (currentLevel == 2 && AutoSplitterData.missionSplitID == 0) AutoSplitterData.missionSplitID++; // 1 - Medical split - we are now on level 2
		System.GC.Collect();
	}

	public void LoadLevelFromSave (int levnum) {
		LoadLevelData(levnum); // Let this function check and load data if it isn't yet.
		if (currentLevel == levnum) return;

		int lastlev = currentLevel;
		Music.a.inCombat = false;
		Music.a.SFXMain.Stop();
		Music.a.SFXOverlay.Stop();
		levels[currentLevel].SetActive(false); // Unload current level	
		levels[levnum].SetActive(true); // Load new level
		currentLevel = levnum; // Set current level to be the new level
		System.GC.Collect();
	}

	public void DisableAllNonOccupiedLevels(int occupiedLevel) {
		for (int i=0;i<levels.Length;i++) {
			if (i == occupiedLevel) continue;

			UnloadLevelData(i);
			if (levels[i] != null) levels[i].SetActive(false);
		}
	}

	public GameObject GetCurrentLevelDynamicContainer() {
		if (currentLevel == -1) return null;
        if (currentLevel < levels.Length) return levelScripts[currentLevel].dynamicObjectsContainer;
        return null;
	}

	public GameObject GetRequestedLevelDynamicContainer(int index) {
		if (index == -1) return null;
        if (currentLevel < levels.Length) return levelScripts[index].dynamicObjectsContainer;
        return null;
	}

	public int GetCurrentLevelSecurity() {
		if (currentLevel == -1) return 0;
		if (superoverride) return 0; // tee hee we are SHODAN, no security blocks in place
		return levelSecurity [currentLevel];
	}

	public void RegisterSecurityObject(int lev,SecurityType stype) {
		if (lev > 14 || lev < 0) return;
		if (currentLevel > 14 || currentLevel < 0) return;
		switch (stype) {
			case SecurityType.None: return;
			case SecurityType.Camera: levelCameraCount[lev]++; break;
			case SecurityType.NodeSmall: levelSmallNodeCount[lev]++; break;
			case SecurityType.NodeLarge: levelLargeNodeCount[lev]++; break;
		}
	}

	// Typical level
	// 4 CPU nodes
	// 20 cameras
	// 100% = 4x + 20y
	// Assuming that a good camera percentage is 2-3%, CPU % would be about 10-15 each
	public void ReduceCurrentLevelSecurity(SecurityType stype) {
		if (currentLevel < 0 || currentLevel > 12) return;

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

	bool PosWithinLeafBounds(Vector3 pos, BoxCollider b) {
		if (b == null) { Debug.Log("BUG: null BoxCollider passed to PosWithinLeafBounds!"); return false; }

		float xMax = b.bounds.max.x;
		float xMin = b.bounds.min.x;
		float yMax = b.bounds.max.z; // Yes Unity you stupid engine...z is y people!  
		float yMin = b.bounds.min.z; // NO I'm not making a 2D game....it's y and I'm sticking to it
		if ((pos.x < xMax && pos.x > xMin) && (pos.z < yMax && pos.z > yMin)) return true;
		return false;
	}

	public void LoadLevelLights(int curlevel) {
		if (curlevel > (lightContainers.Length - 1)) return;
		if (curlevel < 0) return;

		StreamReader sf = new StreamReader(Application.dataPath + "/StreamingAssets/CitadelScene_lights_level" + curlevel.ToString() + ".dat");
		if (sf == null) { UnityEngine.Debug.Log("Lights input file path invalid"); return; }

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

		string[] entries = new string[27];
		char delimiter = '|';
		int index = 0;
		float readFloatx;
		float readFloaty;
		float readFloatz;
		float readFloatw;
		Vector3 tempvec;
		Quaternion tempquat;
		for (int i=0;i<readFileList.Count;i++) {
			entries = readFileList[i].Split(delimiter);
			if (entries.Length <= 1) continue;

			index = 0;
			GameObject newLight = new GameObject("PointLight" + i.ToString());
			Light lit = newLight.AddComponent<Light>();
			Transform tr = newLight.transform;
			tr.SetParent(lightContainers[curlevel].transform);

			// Get transform
			readFloatx = GetFloatFromString(entries[index]); index++;
			readFloaty = GetFloatFromString(entries[index]); index++;
			readFloatz = GetFloatFromString(entries[index]); index++;
			tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
			tr.localPosition = tempvec;

			// Get rotation
			readFloatx = GetFloatFromString(entries[index]); index++;
			readFloaty = GetFloatFromString(entries[index]); index++;
			readFloatz = GetFloatFromString(entries[index]); index++;
			readFloatw = GetFloatFromString(entries[index]); index++;
			tempquat = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
			tr.localRotation = tempquat;

			// Get scale
			readFloatx = GetFloatFromString(entries[index]); index++;
			readFloaty = GetFloatFromString(entries[index]); index++;
			readFloatz = GetFloatFromString(entries[index]); index++;
			tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
			tr.localScale = tempvec;

			lit.intensity = GetFloatFromString(entries[index]); index++;
			lit.range = GetFloatFromString(entries[index]); index++;
			lit.type = GetLightTypeFromString(entries[index]); index++;
			readFloatx = GetFloatFromString(entries[index]); index++;
			readFloaty = GetFloatFromString(entries[index]); index++;
			readFloatz = GetFloatFromString(entries[index]); index++;
			readFloatw = GetFloatFromString(entries[index]); index++;
			lit.color = new Color(readFloatx, readFloaty, readFloatz, readFloatw);
			lit.spotAngle = GetFloatFromString(entries[index]); index++;
			lit.shadows = GetLightShadowsFromString(entries[index]); index++;
			lit.shadowStrength = GetFloatFromString(entries[index]); index++;
			lit.shadowResolution = GetShadowResFromString(entries[index]); index++;
			lit.shadowBias = GetFloatFromString(entries[index]); index++;
			lit.shadowNormalBias = GetFloatFromString(entries[index]); index++;
			lit.shadowNearPlane = GetFloatFromString(entries[index]); index++;
			lit.cullingMask = GetIntFromString(entries[index]); index++;
		}
	}

	public void UnloadLevelLights(int curlevel) {
		if (curlevel > (lightContainers.Length - 1)) return;
		if (curlevel < 0) return;

		Component[] compArray = lightContainers[curlevel].GetComponentsInChildren(typeof(Light),true);
		for (int i=0;i<compArray.Length;i++) {
			if (compArray[i].gameObject.GetComponent<LightAnimation>() != null) continue;
			if (compArray[i].gameObject.GetComponent<TargetIO>() != null) continue;

			DestroyImmediate(compArray[i].gameObject);
		}
		compArray = null;
	}

	private int GetIntFromString(string val) {
		if (val == "0") return 0;

		getValparsed = Int32.TryParse(val, NumberStyles.Integer, en_US_Culture, out getValreadInt);
		if (!getValparsed) { UnityEngine.Debug.Log("BUG: Could not parse int from `" + val + "`"); return 0; }
		return getValreadInt;
	}

	private float GetFloatFromString(string val) {
		getValparsed = Single.TryParse(val, NumberStyles.Float, en_US_Culture, out getValreadFloat);
		if (!getValparsed) {
			UnityEngine.Debug.Log("BUG: Could not parse float from `" + val + "`");
			return 0.0f;
		}
		return getValreadFloat;
	}


	private LightType GetLightTypeFromString(string type) {
		if (type == "Spot") return LightType.Spot;
		else if (type == "Directional") return LightType.Directional;
		else if (type == "Rectangle") return LightType.Rectangle;
		else if (type == "Disc") return LightType.Disc;
		return LightType.Point;	
	}

	private LightShadows GetLightShadowsFromString(string shadows) {
		if (shadows == "None") return LightShadows.None;
		else if (shadows == "Hard") return LightShadows.Hard;
		return LightShadows.Soft;	
	}

	private LightShadowResolution GetShadowResFromString(string res) {
		if (res == "Low") return LightShadowResolution.Low;
		else if (res == "Medium") return LightShadowResolution.Medium;
		else if (res == "High") return LightShadowResolution.High;
		else if (res == "VeryHigh") return LightShadowResolution.VeryHigh;
		return LightShadowResolution.FromQualitySettings;	
	}
}
