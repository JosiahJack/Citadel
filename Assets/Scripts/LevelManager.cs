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
	public GameObject[] lightContainers;
	public GameObject[] npcContainers;
	public GameObject[] elevatorTargetDestinations;
	public Material rtxEmissive;
	public Mesh sphereMesh;

	private bool getValparsed;
	private bool[] levelDataLoaded;
	private int getValreadInt;
	private float getValreadFloat;

	// Singleton instance
	public static LevelManager a;

	void Awake () {
		a = this;
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
		levelDataLoaded[levnum] = false;
	}

	// Make sure relevant data and objects are loaded in and present for the level.
	public void LoadLevelData(int levnum) {
		if (QuestLogNotesManager.a != null) QuestLogNotesManager.a.NotifyLevelChange(currentLevel);

		if (levnum < 0 || levnum > 12) { // Not in a level, in a test or editor space.
			levelDataLoaded[levnum] = true;
			return;
		}
		if (levelDataLoaded[levnum]) return; // Already loaded.

		LoadLevelLights(levnum);
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
		DisableAllNonOccupiedLevelsExcept(currentLevel);
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
		if (showSkyForLevel[currentLevel]) skyMR.enabled = true; else skyMR.enabled = false;
		if (showSaturnForLevel[currentLevel]) saturn.SetActive(true); else saturn.SetActive(false);
		if (showExteriorForLevel[currentLevel]) exterior.SetActive(true); else exterior.SetActive(false);
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

	public void UnloadLevelLights(int curlevel) {
		if (curlevel > (lightContainers.Length - 1)) return;
		if (curlevel < 0) return;

		Component[] compArray = 
		  lightContainers[curlevel].GetComponentsInChildren(typeof(Light),true);

		GameObject go = null;
		for (int i=0;i<compArray.Length;i++) {
			go = compArray[i].gameObject;
			if (go.GetComponent<LightAnimation>() != null) continue;
			if (go.GetComponent<TargetIO>() != null) continue;

			DestroyImmediate(go); // Dangerous isn't it :D
		}
		compArray = null;
	}

	public void LoadLevelLights(int curlevel) {
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
			lit.shadowStrength = Utils.GetFloatFromString(entries[index]); index++;
			lit.shadowResolution = Utils.GetShadowResFromString(entries[index]); index++;
			lit.shadowBias = Utils.GetFloatFromString(entries[index]); index++;
			lit.shadowNormalBias = Utils.GetFloatFromString(entries[index]); index++;
			lit.shadowNearPlane = Utils.GetFloatFromString(entries[index]); index++;
			lit.cullingMask = Utils.GetIntFromString(entries[index]); index++;

			//GameObject rtxSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			//rtxSphere.name = "Sphere" + i.ToString();
			//Transform sprTr = rtxSphere.transform;
			//sprTr.SetParent(tr);
			//sprTr.localPosition = Vector3.zero;
			//sprTr.localRotation = Const.a.quaternionIdentity;
			//float sprScale = 0.1f * (lit.intensity / 5.5f);
			//sprTr.localScale = new Vector3(readFloatx,readFloaty,readFloatz);
			//rtxSphere.layer = 1; // TransparentFX
			//MeshRenderer mr = rtxSphere.GetComponent<MeshRenderer>();
			//if (mr == null) mr = rtxSphere.AddComponent<MeshRenderer>();
			//mr.sharedMaterial = rtxEmissive;
			//MeshFilter mf = rtxSphere.GetComponent<MeshFilter>();
			//if (mf == null) mf = rtxSphere.AddComponent<MeshFilter>();
			//mf.mesh = sphereMesh;
			//SphereCollider sprCol = rtxSphere.GetComponent<SphereCollider>();
			//if (sprCol != null) sprCol.enabled = false;
		}
	}

	public void UnloadLevelDynamicObjects(int curlevel) {
		GameObject go = GetRequestedLevelDynamicContainer(curlevel);
		Component[] compArray = go.GetComponentsInChildren(typeof(SaveObject),true);
		for (int i=0;i<compArray.Length;i++) {
			PrefabIdentifier pid = compArray[i].gameObject.GetComponent<PrefabIdentifier>();
			if (pid == null) {
				if (gameObject != LevelManager.a.gameObject) {
					Debug.Log("SaveObject on a GameObject " + gameObject.name
							  + " missing companion PrefabIdentifier component");
				}
			} else {
				if (pid.constIndex == 517) { // func_wall has SaveObject on first child mover_target so destroy the parent instead
					DestroyImmediate(compArray[i].gameObject.transform.parent.gameObject);
				} else {
					if (pid.constIndex == 477) {
						GameObject cgo =
							compArray[i].transform.GetChild(0).gameObject;

						HealthManager hm = Utils.GetMainHealthManager(cgo);
						if (hm == null) {
							Debug.Log("Missing HealthManager on security "
									  + "camera "
									  + compArray[i].gameObject.name);
						} else {
							UnityEngine.UI.Image overlay = hm.linkedOverlay;
							if (overlay != null) {
								GameObject hmGO = overlay.gameObject;
								hmGO.SetActive(false);
							}
						}
					}
					DestroyImmediate(compArray[i].gameObject);
				}
			}
		}
		compArray = null;
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
		for (int i=0;i<readFileList.Count;i++) {
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
