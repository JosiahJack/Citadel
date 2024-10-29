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
	public Material pipe_maint2_3_coolant;
	[HideInInspector] public List<string>[] DynamicObjectsSavestrings = new List<string>[13];
	
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

		for (int i=0;i<levelScripts.Length;i++) levelScripts[i].Awake();
		//PlayerReferenceManager.a.playerCurrentLevel = currentLevel;
		if (sky == null) Debug.Log("BUG: LevelManager missing manually assigned reference for sky.");
		else sky.SetActive(true);

		SetSkyVisible(true);
		if (ressurectionBayDoor.Length != 8) Debug.Log("BUG: LevelManager ressurectionBayDoor array length not equal to 8.");
		Time.timeScale = Const.a.defaultTimeScale;
		levelDataLoaded = new bool[14];
		for (int i=0;i<14;i++) {
			levelDataLoaded[i] = false;
			DynamicObjectsSavestrings[i] = new List<string>();
		}
		LoadLevelData(currentLevel);
	}
	
	public void SetSkyVisible(bool on) {
		skyMR.enabled = (on && showSkyForLevel[currentLevel]);
		saturn.SetActive(on && showSaturnForLevel[currentLevel]);
		exterior.SetActive(on && showExteriorForLevel[currentLevel]);
		if (Const.a == null) return;
		if (Const.a.questData == null) return;
		
		exterior_shield.SetActive(on && showExteriorForLevel[currentLevel]
								  && Const.a.questData.ShieldActivated);
	}

	public void CyborgConversionToggleForCurrentLevel() {
	    if (currentLevel < 0 || currentLevel >= 13) return;
	    
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
			LoadLevel(6,ressurectionLocation[currentLevel].gameObject,
						ressurectionLocation[currentLevel].position);

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
		if (levnum < 0 || levnum > 12) return; // Not in a level, in a test or editor space.
		if (!levelDataLoaded[levnum]) return; // Already cleared.

		UnloadLevelLights(levnum);
		UnloadLevelGeometry(levnum);
 		UnloadLevelDynamicObjects(levnum,true);
		levelDataLoaded[levnum] = false;
	}

	// Make sure relevant data and objects are loaded in and present for the level.
	public void LoadLevelData(int levnum) {
		if (QuestLogNotesManager.a != null) QuestLogNotesManager.a.NotifyLevelChange(currentLevel);

		if (levnum < 0 || levnum > 13) { // Not in a level, in a test or editor space.
			levelDataLoaded[levnum] = true;
			return;
		}
		if (levelDataLoaded[levnum]) return; // Already loaded.

		LoadLevelLights(levnum);
		LoadLevelGeometry(levnum);
		levelDataLoaded[levnum] = true;
	}

	public void LoadLevel(int levnum, GameObject targetDestination, Vector3 targetPosition) {
		// NOTE: Check this first since the button for the current level has a null destination.  This is fine and expected.
		if (currentLevel == levnum) { Const.sprint(Const.a.stringTable[9]); return; } //Already there

		MFDManager.a.TurnOffElevatorPad();
		GUIState.a.ClearOverButton();
		if (targetDestination == null && targetPosition.x == 0 && targetPosition.y == 0 && targetPosition.z == 0) {
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

		if (targetDestination != null) PlayerReferenceManager.a.playerCapsule.transform.position = targetDestination.transform.position; // Put player in the new level
		else if (targetPosition != null) PlayerReferenceManager.a.playerCapsule.transform.position = targetPosition; // Return to level from cyberspace.

		currentLevel = levnum; // Set current level to be the new level
		DisableAllNonOccupiedLevelsExcept(currentLevel);
		levels[levnum].SetActive(true); // enable new level
		PlayerReferenceManager.a.playerCurrentLevel = levnum;
		if (currentLevel == 2 && AutoSplitterData.missionSplitID == 0) AutoSplitterData.missionSplitID++; // 1 - Medical split - we are now on level 2
		PostLoadLevelSetupSystems();
		if (currentLevel != 13) DynamicCulling.a.Cull_Init();
	}

	public void LoadLevelFromSave (int levnum) {
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
		LoadLevelData(currentLevel);
		Automap.a.SetAutomapExploredReference(currentLevel);
		Automap.a.automapBaseImage.overrideSprite = Automap.a.automapsBaseImages[currentLevel];
		Const.a.ClearActiveAutomapOverlays(); // After other levels turned off.
		ObjectContainmentSystem.UpdateActiveFlooring(); // Update list to only include active.
		SetSkyVisible(true);
		System.GC.Collect();
	}

	public void DisableAllNonOccupiedLevelsExcept(int occupiedLevel) {
		for (int i=0;i<levels.Length;i++) {
			if (i == occupiedLevel) continue;

			UnloadLevelData(i);
			if (levels[i] != null) levels[i].SetActive(false);
		}
	}

	public GameObject GetCurrentDynamicContainer() { // Does not return null
		if (currentLevel < 0 || currentLevel > 13) {
			return levelScripts[1].dynamicObjectsContainer;
		}

        return levelScripts[currentLevel].dynamicObjectsContainer;
	}

	public GameObject GetCurrentGeometryContainer() { // Does not return null
		if (currentLevel < 0 || currentLevel > 13) {
			return levelScripts[1].geometryContainer;
		}

        return levelScripts[currentLevel].geometryContainer;
	}

	public GameObject GetCurrentStaticImmutableContainer() { // Does not return null
		if (currentLevel < 0 || currentLevel > 13) {
			return levelScripts[1].staticObjectsImmutable;
		}

		return levelScripts[currentLevel].staticObjectsImmutable;
	}

	public GameObject GetCurrentStaticSaveableContainer() { // Does not return null
		if (currentLevel < 0 || currentLevel > 13) {
			return levelScripts[1].staticObjectsSaveable;
		}

		return levelScripts[currentLevel].staticObjectsSaveable;
	}

	public GameObject GetCurrentDoorsContainer() { // Does not return null
		if (currentLevel < 0 || currentLevel > 13) {
			return levelScripts[1].doorsStaticSaveable;
		}

		return levelScripts[currentLevel].doorsStaticSaveable;
	}

	public GameObject GetCurrentLightsContainer() { // Does not return null
		if (currentLevel < 0 || currentLevel > 13) {
			return levelScripts[1].lightsStaticImmutable;
		}

		return levelScripts[currentLevel].lightsStaticImmutable;
	}

	public GameObject GetRequestedLevelDynamicContainer(int index) {
		if (index < 0 || index > 13) return levelScripts[1].dynamicObjectsContainer; // Default to Medical level

        return levelScripts[index].dynamicObjectsContainer;
	}

	public GameObject GetRequestedLevelNPCContainer(int index) {
		if (index < 0 || index > 13) return npcContainers[1]; // Default to Medical level

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
		if (lev < 0) return;
		if (lev > 13) return;

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
		
		string gName = "CitadelScene_geometry_level"+curlevel.ToString()+".dat";
		StreamReader sf = Utils.ReadStreamingAsset(gName);
		if (sf == null) {
			UnityEngine.Debug.Log("Geometry input file path invalid");
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

		string[] entries = new string[27];
		int index = 0;
		Vector3 vec;
		Transform childChunk;
		for (int i=0;i<readFileList.Count;i++) {
			entries = readFileList[i].Split(Convert.ToChar(Utils.splitChar));
			if (entries.Length <= 1) continue;

			index = 0;
			int constdex = Utils.GetIntFromString(entries[index],"constIndex"); index++;
			GameObject chunk = ConsoleEmulator.SpawnDynamicObject(constdex,
																  curlevel,
																  false,null,0);
			if (chunk == null) continue;
			
			chunk.name = entries[index]; index++;
			index = Utils.LoadTransform(chunk.transform,ref entries,index);
			if ((entries.Length - 1) >= index) {
				string[] splits = entries[index].Split(':');
				string variableName = splits[0];
				string variableValue = splits[1];
				if (variableName == "BoxCollider.enabled") {
					BoxCollider bcol = chunk.GetComponent<BoxCollider>();
					bcol.enabled = Utils.GetBoolFromString(entries[index],"BoxCollider.enabled"); index++;
					vec = new Vector3(1f,1f,1f);
					vec.x = Utils.GetFloatFromString(entries[index],"size.x"); index++;
					vec.y = Utils.GetFloatFromString(entries[index],"size.y"); index++;
					vec.z = Utils.GetFloatFromString(entries[index],"size.z"); index++;
					bcol.size = vec;
					vec.x = Utils.GetFloatFromString(entries[index],"center.x"); index++;
					vec.y = Utils.GetFloatFromString(entries[index],"center.y"); index++;
					vec.z = Utils.GetFloatFromString(entries[index],"center.z"); index++;
					bcol.center = vec;
					if (chunk.transform.childCount >= 1) {
						// Get collisionAid
						Transform subtr = chunk.transform.GetChild(0);
						if (subtr != null) {
							subtr.gameObject.SetActive(Utils.GetBoolFromString(entries[index],"collisionAid.activeSelf")); index++;
						}
					}
					
					if (index < entries.Length) {
						splits = entries[index].Split(':');
						variableName = splits[0];
					}
				} else if (variableName == "material") {
					MeshRenderer mr = chunk.GetComponent<MeshRenderer>();
					int matVal = GetMaterialByName(variableValue);
					//Debug.Log("found geometry with material override of "
					//		  + variableValue + " giving index "
					//		  + matVal.ToString());

					if (matVal == 86 && constdex == 130) {
						childChunk = chunk.transform.GetChild(0);
						if (childChunk != null) {
							mr = childChunk.gameObject.GetComponent<MeshRenderer>();
							if (mr != null) {
								mr.sharedMaterial = Const.a.genericMaterials[matVal];
							}
						}
					}
					
					if (mr != null) {
						mr.sharedMaterial = Const.a.genericMaterials[matVal];
					} else if (chunk.transform.childCount > 0) {
						childChunk = chunk.transform.GetChild(0);
						if (childChunk != null) {
							mr = childChunk.gameObject.GetComponent<MeshRenderer>();
							if (mr != null) {
								mr.sharedMaterial = Const.a.genericMaterials[matVal];
							}
						}
					}
				}
				
				if (constdex == 218) { // chunk_reac2_4 has text on it.
					if (entries.Length - 1 >= index) {
						Transform textr1 = chunk.transform.GetChild(1); // text_decalStopDSS1
						Transform textr2 = chunk.transform.GetChild(2); // text_decalStopDSS1 (1)
						TextLocalization tex1 = textr1.gameObject.GetComponent<TextLocalization>();
						TextLocalization tex2 = textr2.gameObject.GetComponent<TextLocalization>();
						tex1.lingdex = Utils.GetIntFromString(entries[index],"lingdex"); index++;
						tex2.lingdex = Utils.GetIntFromString(entries[index],"lingdex"); index++;
					}
				}
			}
		}
	}
	
	int GetMaterialByName(string name) {
		switch (name) {
			case "cyberpanel_black":              return 49;
 			case "cyberpanel_blue":               return 50;
			case "cyberpanel_bluegray":           return 51;
			case "cyberpanel_cyan":               return 52;
			case "cyberpanel_cyandark":           return 53;
			case "cyberpanel_gray":               return 54;
			case "cyberpanel_green":              return 55;
			case "cyberpanel_greendark":          return 56;
			case "cyberpanel_orange":             return 57;
			case "cyberpanel_orangedark":         return 58;
			case "cyberpanel_paleorange":         return 59;
			case "cyberpanel_palepurple":         return 60;
			case "cyberpanel_palered":            return 61;
			case "cyberpanel_paleyellow":         return 62;
			case "cyberpanel_purple":             return 63;
			case "cyberpanel_red":                return 64;
			case "cyberpanel_slice45":            return 65;
			case "cyberpanel_slice45_blue":       return 66;
			case "cyberpanel_slice45_bluegray":   return 67;
			case "cyberpanel_slice45_cyan":       return 68;
			case "cyberpanel_slice45_cyandark":   return 69;
			case "cyberpanel_slice45_gray":       return 70;
			case "cyberpanel_slice45_green":      return 71;
			case "cyberpanel_slice45_greendark":  return 72;
			case "cyberpanel_slice45_orange":     return 73;
			case "cyberpanel_slice45_orangedark": return 74;
			case "cyberpanel_slice45_paleorange": return 75;
			case "cyberpanel_slice45_palepurple": return 76;
			case "cyberpanel_slice45_palered":    return 77;
			case "cyberpanel_slice45_paleyellow": return 78;
			case "cyberpanel_slice45_purple":     return 79;
			case "cyberpanel_slice45_red":        return 80;
			case "cyberpanel_slice45_yellow":     return 81;
			case "cyberpanel_touching":           return 82;
			case "cyberpanel_white":              return 83;
			case "cyberpanel_yellow":             return 84;
			case "cyberpanel_yellowdark":         return 85;
			case "pipe_maint2_3_coolant":         return 86;
		}
		
		return 0;
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

			DestroyImmediate(go); // Dangerous isn't it :D
		}
		compArray = null;
	}

	public void LoadLevelLights(int curlevel) {
		if (curlevel > 12) return;
		if (curlevel > (lightContainers.Length - 1)) return;
		if (curlevel < 0) return;

		string lName = "CitadelScene_lights_level"+curlevel.ToString()+".dat";
		StreamReader sf = Utils.ReadStreamingAsset(lName);
		if (sf == null) {
			UnityEngine.Debug.Log("Lights input file path invalid");
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

		string[] entries = new string[27];
		int index = 0;
		float readFloatx;
		float readFloaty;
		float readFloatz;
		float readFloatw;
		Vector3 tempvec;
		Quaternion tempquat;
		float[] shadCullArray = new float[32];
		// Shadow distance is 35.0f so anything non-zero less than that helps.
		shadCullArray[0] = 15f; // Default
		shadCullArray[1] = 0.0f; // TransparentFX
		shadCullArray[2] = 0.0f; // Ignore Raycast
		shadCullArray[3] = 0.0f; // 
		shadCullArray[4] = 0.0f; // Water
		shadCullArray[5] = 0.0f; // UI
		shadCullArray[6] = 0.0f; // 
		shadCullArray[7] = 0.0f; // 
		shadCullArray[8] = 0.0f; // GunViewModel
		shadCullArray[9] = 20.0f; // Geometry
		shadCullArray[10] = 15f; // NPC
		shadCullArray[11] = 5f; // Bullets
		shadCullArray[12] = 7f; // Player
		shadCullArray[13] = 15f; // Corpse
		shadCullArray[14] = 20f; // PhysObjects
		shadCullArray[15] = 0.0f; // Sky
		shadCullArray[16] = 0.0f; // PlayerTriggerOnly
		shadCullArray[17] = 0.0f; // Trigger
		shadCullArray[18] = 20f; // Door
		shadCullArray[19] = 15f; // InterDebris
		shadCullArray[20] = 0.0f; // Player2
		shadCullArray[21] = 0.0f; // Player3
		shadCullArray[22] = 0.0f; // Player4
		shadCullArray[23] = 0.0f; // NPCTrigger
		shadCullArray[24] = 5f; // NPCBullet
		shadCullArray[25] = 0.0f; // NPCClip
		shadCullArray[26] = 0.0f; // Clip
		shadCullArray[27] = 0.0f; // Automap
		shadCullArray[28] = 0.0f; // Culling
		shadCullArray[29] = 15f; // CorpseSearchable
		shadCullArray[30] = 0.0f; // 
		shadCullArray[31] = 0.0f; // 
		for (int i=0;i<readFileList.Count;i++) {
			entries = readFileList[i].Split(Convert.ToChar(Utils.splitChar));
			if (entries.Length <= 1) continue;

			index = 0;
			GameObject newLight = new GameObject("PointLight" + i.ToString());
			Light lit = newLight.AddComponent<Light>();
			Transform tr = newLight.transform;
			tr.SetParent(lightContainers[curlevel].transform);

			// Get transform
			readFloatx = Utils.GetFloatFromString(entries[index]); index++;
			readFloaty = Utils.GetFloatFromString(entries[index]); index++;
			readFloatz = Utils.GetFloatFromString(entries[index]); index++;
			tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
			tr.localPosition = tempvec;

			// Get rotation
			readFloatx = Utils.GetFloatFromString(entries[index]); index++;
			readFloaty = Utils.GetFloatFromString(entries[index]); index++;
			readFloatz = Utils.GetFloatFromString(entries[index]); index++;
			readFloatw = Utils.GetFloatFromString(entries[index]); index++;
			tempquat = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
			tr.localRotation = tempquat;

			// Get scale
			readFloatx = Utils.GetFloatFromString(entries[index]); index++;
			readFloaty = Utils.GetFloatFromString(entries[index]); index++;
			readFloatz = Utils.GetFloatFromString(entries[index]); index++;
			tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
			tr.localScale = tempvec;

			lit.intensity = Utils.GetFloatFromString(entries[index]); index++;
			lit.range = Utils.GetFloatFromString(entries[index]); index++;
			lit.type = Utils.GetLightTypeFromString(entries[index]); index++;
			readFloatx = Utils.GetFloatFromString(entries[index]); index++;
			readFloaty = Utils.GetFloatFromString(entries[index]); index++;
			readFloatz = Utils.GetFloatFromString(entries[index]); index++;
			readFloatw = Utils.GetFloatFromString(entries[index]); index++;
			lit.color = new Color(readFloatx, readFloaty, readFloatz, readFloatw);
			lit.spotAngle = Utils.GetFloatFromString(entries[index]); index++;
			lit.shadows = Utils.GetLightShadowsFromString(entries[index]); index++;
			if (lit.intensity < 0.3f || (lit.range > 7f && lit.intensity < 2f)) lit.shadows = LightShadows.None;
			lit.shadowStrength = Utils.GetFloatFromString(entries[index]); index++;
			lit.shadowResolution = Utils.GetShadowResFromString(entries[index]); index++;
			lit.shadowBias = Utils.GetFloatFromString(entries[index]); index++;
			lit.shadowNormalBias = Utils.GetFloatFromString(entries[index]); index++;
			lit.shadowNearPlane = Utils.GetFloatFromString(entries[index]); index++;
			lit.layerShadowCullDistances = shadCullArray;
			lit.cullingMask = Utils.GetIntFromString(entries[index]); index++;
			lit.cullingMask = LayerMask.GetMask("Default","TransparentFX",
												"Water","GunViewModel",
												"Geometry","NPC","Bullets",
												"Player","Corpse",
												"PhysObjects","Door",
												"InterDebris","Player2",
												"Player3","Player4",
												"NPCBulllet",
												"CorpseSearchable");
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

	public void LoadLevelDynamicObjects(int curlevel, GameObject container) {
		if (curlevel > (levelScripts.Length - 1)) return;
		if (curlevel < 0) return;

		string dynName = "CitadelScene_dynamics_level" + curlevel.ToString()
						 + ".dat";

		//string dynPath = Utils.SafePathCombine(Application.streamingAssetsPath,
		//									   dynName);

		//Utils.ConfirmExistsInStreamingAssetsMakeIfNot(dynName);
		//StreamReader sf = new StreamReader(dynPath);
		StreamReader sf = Utils.ReadStreamingAsset(dynName);
		if (sf == null) {
			UnityEngine.Debug.Log("Dynamic objects input file path invalid");
			return;
		}

		string readline;
		List<string> readFileList = new List<string>();
		using (sf) {
			do {
				readline = sf.ReadLine();
				if (readline != null) readFileList.Add(readline);
			} while (!sf.EndOfStream);
			sf.Close();
		}

		int val = 307; // First useable object (paper wad) just in case.
					   // Don't want to spawn 0 as a fallback which is a wall.

		string[] entries;
		DynamicObjectsSavestrings[curlevel].Clear(); // Empty list.
		for (int i=0;i<readFileList.Count;i++) {
			DynamicObjectsSavestrings[curlevel].Add(readFileList[i]);
			entries = readFileList[i].Split(Convert.ToChar(Utils.splitChar));
			if (entries.Length <= 1) continue;

			val = Utils.GetIntFromString(entries[21]); // Master Index
			GameObject newGO = ConsoleEmulator.SpawnDynamicObject(val,curlevel,
																  false,
																  container,
																  -1);

			if (newGO != null) SaveObject.Load(newGO,ref entries,-2);
		}
	}

	public void LoadLevelDynamicObjects(int curlevel) {
		LoadLevelDynamicObjects(curlevel,null);
	}

	public void CheatLoadLevel(int ind) {
		if (ind == 10) {
			LoadLevel(10,PlayerMovement.a.cheatG1Spawn.gameObject,
					  ressurectionLocation[10].position);
			return;
		} else if (ind == 11) {
			LoadLevel(11,PlayerMovement.a.cheatG2Spawn.gameObject,
					  ressurectionLocation[11].position);
			return;
		} else if (ind == 12) {
			LoadLevel(12,PlayerMovement.a.cheatG4Spawn.gameObject,
					  ressurectionLocation[12].position);
			return;
		}

		LoadLevel(ind,ressurectionLocation[ind].gameObject,
				  ressurectionLocation[ind].position);
	}

	public static string Save(GameObject go) {
		int i=0;
		LevelManager lvm = go.GetComponent<LevelManager>();
		if (lvm == null) {
			Debug.Log("LevelManager missing!  GameObject.name: " + go.name);
			string line = "i";
			for (i=0;i<(14 * 4);i++) line += "i";
			for (i=0;i<14;i++) line += "b";
			return Utils.DTypeWordToSaveString(line);
		}

		StringBuilder s1 = new StringBuilder();
		s1.Clear(); // keep reusing s1
		// Global states and Difficulties
		s1.Append(Utils.UintToString(LevelManager.a.currentLevel,
								     "currentLevel"));
		s1.Append(Utils.splitChar);
		for (i=0;i<14;i++) {
			s1.Append(Utils.UintToString(LevelManager.a.levelSecurity[i],
										 "levelSecurity["+i.ToString()+"]"));
			s1.Append(Utils.splitChar);
		}

		for (i=0;i<14;i++) {
			s1.Append(Utils.UintToString(
				LevelManager.a.levelCameraDestroyedCount[i],
				"levelCameraDestroyedCount["+i.ToString()+"]"
			));
			s1.Append(Utils.splitChar);
		}

		for (i=0;i<14;i++) {
			s1.Append(Utils.UintToString(
				LevelManager.a.levelSmallNodeDestroyedCount[i],
				"levelSmallNodeDestroyedCount["+i.ToString()+"]"
			));
			s1.Append(Utils.splitChar);
		}

		for (i=0;i<14;i++) {
			s1.Append(Utils.UintToString(
				LevelManager.a.levelLargeNodeDestroyedCount[i],
				"levelLargeNodeDestroyedCount["+i.ToString()+"]"
			));
			s1.Append(Utils.splitChar);
		}

		for (i=0;i<13;i++) {
			s1.Append(Utils.BoolToString(LevelManager.a.ressurectionActive[i],
									  "ressurectionActive["+i.ToString()+"]"));

			s1.Append(Utils.splitChar);
		}
		s1.Append(Utils.BoolToString(LevelManager.a.ressurectionActive[13]));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		LevelManager lvm = go.GetComponent<LevelManager>();
		if (lvm == null) {
			Debug.Log("LevelManager.Load failure, lvm == null");
			return index + (14 * 5) + 1;
		}

		if (index < 0) {
			Debug.Log("LevelManager.Load failure, index < 0");
			return index + (14 * 5) + 1;
		}

		if (entries == null) {
			Debug.Log("LevelManager.Load failure, entries == null");
			return index + (14 * 5) + 1;
		}

		int i = 0;
		int levelNum = Utils.GetIntFromString(entries[index],"currentLevel");
		index++;

		LevelManager.a.LoadLevelFromSave(levelNum);
		for (i=0;i<14;i++) {
			LevelManager.a.levelSecurity[i] = 
				Utils.GetIntFromString(entries[index],
									   "levelSecurity[" + i.ToString() + "]");

			index++;
		}
		for (i=0;i<14;i++) {
			LevelManager.a.levelCameraDestroyedCount[i] =
				Utils.GetIntFromString(entries[index],
									   "levelCameraDestroyedCount["
									   + i.ToString() + "]");
			index++;
		}
		for (i=0;i<14;i++) {
			LevelManager.a.levelSmallNodeDestroyedCount[i] =
				Utils.GetIntFromString(entries[index],
									   "levelSmallNodeDestroyedCount["
									   + i.ToString() + "]");
			index++;
		}
		for (i=0;i<14;i++) {
			LevelManager.a.levelLargeNodeDestroyedCount[i] =
				Utils.GetIntFromString(entries[index],
									   "levelLargeNodeDestroyedCount["
									   + i.ToString() + "]");
			index++;
		}
		for (i=0;i<14;i++) {
			LevelManager.a.ressurectionActive[i] =
				Utils.GetBoolFromString(entries[index],"ressurectionActive["
													   + i.ToString() + "]");
			index++;
		}
		return index;
	}
}
