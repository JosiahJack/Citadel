using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {
	public int currentLevel;
	public static LevelManager a;
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
	//public GameObject currentPlayer;
	//public GameObject elevatorControl;
	public GameObject sky;
	public bool superoverride = false;
	public GameObject saturn;
	public GameObject exterior;
	public MeshRenderer skyMR;
	public bool[] showSkyForLevel;
	public bool[] showExteriorForLevel;
	public bool[] showSaturnForLevel;
	public NPCSubManager[] npcsm;
	public enum SecurityType{None,Camera,NodeSmall,NodeLarge};
	public Level[] levelScripts;
	public int currentLeaf;
	public Transform[] levelGeometryContainers;

	void Awake () {
		a = this;
		if (currentLevel == -1) return;
		//StartCoroutine(InitializeAllLevels());
		SetAllPlayersLevelsToCurrent();
		if (sky != null) sky.SetActive(true);
		if (showSkyForLevel[currentLevel]) skyMR.enabled = true; else skyMR.enabled = false;
		if (showExteriorForLevel[currentLevel]) exterior.SetActive(true); else exterior.SetActive(false);
		if (showSaturnForLevel[currentLevel]) saturn.SetActive(true); else saturn.SetActive(false);
		Time.timeScale = Const.a.defaultTimeScale;
		currentLeaf = -1;
	}

	public void Start() {
		PullOutCurrentLevelNPCs();
	}

	public void PullOutCurrentLevelNPCs() {
		if (npcsm[currentLevel] != null) npcsm[currentLevel].PullOutNPCs();
	}

	public void PutBackCurrentLevelNPCs() {
		if (npcsm[currentLevel] != null) npcsm[currentLevel].PutBackNPCs();
	}
	/*public void InitializeAllLevels() {
		PauseScript.a.paused = true;
		for (int i=0;i<levels.Length;i++) {
			if (levels[i] != null) levels[i].SetActive(true);
			//yield return null); // should be 0.14s total
		}
		yield return new WaitForSeconds(0.5f); // 0.14+0.5 = 0.6s total for level initialization
		DisableAllNonOccupiedLevels(); // sure, this probably takes another second at least :)
		PauseScript.a.paused = false;
	}*/

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
			ressurectionActive[currentLevel] = !ressurectionActive[currentLevel]; // toggle current level
		}
	}

	public bool IsCurrentLevelCyborgConversionEnabled() {
		return ressurectionActive[currentLevel];
	}

	public bool RessurectPlayer (GameObject currentPlayer) {
		// Note: currentPlayer is the main Player gameObject that contains PlayerReferenceManager
		if (currentPlayer == null) {
			Const.sprint("BUG: LevelManager cannot find current player for RessurectPlayer.",Const.a.allPlayers);
			return false; // prevent possible error if wrong player is passed
		}

		if (ressurectionActive[currentLevel]) {
			if (currentLevel == 10 ||currentLevel == 11 ||currentLevel == 12) {
				LoadLevel(6,ressurectionLocation[currentLevel].gameObject,currentPlayer,ressurectionLocation[currentLevel].position);
				ressurectionBayDoor[6].ForceClose();
			} else {
				ressurectionBayDoor[currentLevel].ForceClose();
				currentPlayer.GetComponent<PlayerReferenceManager>().playerCapsule.transform.position = transform.TransformPoint(ressurectionLocation[currentLevel].position); //teleport to ressurection chamber
			}
			currentPlayer.GetComponent<PlayerReferenceManager>().playerDeathRessurectEffect.SetActive(true); // activate death screen and readouts for "BRAIN ACTIVITY SATISFACTORY"            ya debatable right
			Music.a.PlayTrack(currentLevel,Music.TrackType.Revive,Music.MusicType.Override);
			currentPlayer.GetComponent<PlayerReferenceManager>().playerCapsule.GetComponent<PlayerMovement>().ressurectingFinished = PauseScript.a.relativeTime + 3f;

			return true;
		}

		return false;
	}

	public void LoadLevel (int levnum, GameObject targetDestination, GameObject currentPlayer, Vector3 targetPosition) {
		// NOTE: Check this first since the button for the current level has a null destination
		if (currentLevel == levnum) {
			Const.sprint(Const.a.stringTable[9],currentPlayer); //Already there
			return;
		}
			
		if (currentPlayer == null) {
			Const.sprint("BUG: LevelManager cannot find current player.",Const.a.allPlayers);
			return; // prevent possible error if keypad does not have player to move
		}

		if (targetDestination == null && targetPosition == null) {
			Const.sprint("BUG: LevelManager cannot find destination.",Const.a.allPlayers);
			return; // prevent possible error if keypad does not have destination set
		}
			
		MFDManager.a.TurnOffElevatorPad();
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
		Vector3 pos = new Vector3();
		if (targetDestination == null) {
			pos = targetPosition;
		} else {
			pos = targetDestination.transform.position;
		}
		PlayerReferenceManager prm = currentPlayer.GetComponent<PlayerReferenceManager>();
		prm.playerCapsule.transform.position = pos; // Put player in the new level
		PlayerMovement pm = prm.playerCapsule.GetComponent<PlayerMovement>();
		pm.SetAutomapExploredReference(levnum);
		pm.automapBaseImage.overrideSprite = pm.automapsBaseImages[levnum];

		levels[levnum].SetActive(true); // enable new level
		prm.playerCurrentLevel = levnum;
		PutBackCurrentLevelNPCs();
		currentLevel = levnum; // Set current level to be the new level
		DisableAllNonOccupiedLevels();
		if (showSkyForLevel[currentLevel]) skyMR.enabled = true; else skyMR.enabled = false;
		if (showSaturnForLevel[currentLevel]) saturn.SetActive(true); else saturn.SetActive(false);
		if (showExteriorForLevel[currentLevel]) exterior.SetActive(true); else exterior.SetActive(false);
		PullOutCurrentLevelNPCs(); // unparent from level transforms to reduce physics work
		if (currentLevel == 2 && AutoSplitterData.missionSplitID == 0) AutoSplitterData.missionSplitID++; // 1 - Medical split - we are now on level 2
	}

	public void LoadLevelFromSave (int levnum) {
		// NOTE: Check this first since the button for the current level has a null destination
		if (currentLevel == levnum) {
			return;
		}
		
		levels[currentLevel].SetActive(false); // Unload current level	
		levels[levnum].SetActive(true); // Load new level
		currentLevel = levnum; // Set current level to be the new level
	}

	void SetAllPlayersLevelsToCurrent () {
		if (Const.a.player1 != null) {
			Const.a.player1.GetComponent<PlayerReferenceManager>().playerCurrentLevel = currentLevel;
		}
		if (Const.a.player2 != null) {
			Const.a.player2.GetComponent<PlayerReferenceManager>().playerCurrentLevel = currentLevel;
		}
		if (Const.a.player3 != null) {
			Const.a.player3.GetComponent<PlayerReferenceManager>().playerCurrentLevel = currentLevel;
		}
		if (Const.a.player4 != null) {
			Const.a.player4.GetComponent<PlayerReferenceManager>().playerCurrentLevel = currentLevel;
		}
	}

	public void DisableAllNonOccupiedLevels() {
		int p1level = -1;
		int p2level = -1;
		int p3level = -1;
		int p4level = -1;;
		if (Const.a.player1 != null) p1level = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCurrentLevel;
		if (Const.a.player2 != null) p2level = Const.a.player2.GetComponent<PlayerReferenceManager>().playerCurrentLevel;
		if (Const.a.player3 != null) p3level = Const.a.player3.GetComponent<PlayerReferenceManager>().playerCurrentLevel;
		if (Const.a.player4 != null) p4level = Const.a.player4.GetComponent<PlayerReferenceManager>().playerCurrentLevel;

		for (int i=0;i<levels.Length;i++) {
			if (p1level != i && p2level != i && p3level != i && p4level != i) {
				if (levels[i] != null) levels[i].SetActive(false);
			} else {
				if (levels[i] != null) levels[i].SetActive(true);
			}
		}
	}

	public void EnableAllLevels() {
		for (int i=0;i<levels.Length;i++) {
			if (levels[i] != null) levels[i].SetActive(true);
		}
	}

	public GameObject GetCurrentLevelDynamicContainer() {
        GameObject retval = null;
		if (currentLevel == -1) return retval;
        if (currentLevel < levels.Length)
            retval = levelScripts[currentLevel].dynamicObjectsContainer;

        return retval;
	}

	public GameObject GetRequestedLevelDynamicContainer(int index) {
        GameObject retval = null;
		if (index == -1) return retval;
        if (currentLevel < levels.Length)
            retval = levelScripts[index].dynamicObjectsContainer;

        return retval;
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
		if (currentLevel == -1) return;

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
		//Debug.Log("secDrop: " + secDrop.ToString());
		//Debug.Log("Camera calc: " + (((camScore/secscoreTotal) * 100)).ToString());
		//Debug.Log("Node small calc: " + (((nodeSmallScore/secscoreTotal) * 100)).ToString());
		//Debug.Log("Node large calc: " + (((nodeLargeScore/secscoreTotal) * 100)).ToString());
		levelSecurity [currentLevel] -= (int)secDrop;
		if (levelSecurity [currentLevel] < 0) levelSecurity [currentLevel] = 0;
		if ((levelLargeNodeDestroyedCount[currentLevel] == levelLargeNodeCount[currentLevel]) && (levelSmallNodeDestroyedCount[currentLevel] == levelSmallNodeCount[currentLevel]) && (levelCameraDestroyedCount[currentLevel] == levelCameraCount[currentLevel])) {
			levelSecurity[currentLevel] = 0;
		}
		Const.sprint(Const.a.stringTable[306] + levelSecurity[currentLevel].ToString() + Const.a.stringTable[307],Const.a.allPlayers);
	}

	bool PosWithinLeafBounds(Vector3 pos, BoxCollider b) {
		if (b == null) { Debug.Log("BUG: null BoxCollider passed to PosWithinLeafBounds!"); return false; }

		float xMax = b.bounds.max.x;
		float xMin = b.bounds.min.x;
		float yMax = b.bounds.max.z; // Yes Unity you stupid engine...z is y people!  
		float yMin = b.bounds.min.z; // NO I'm not making a 2D game....it's y and I'm sticking to it
		if (pos.x < xMax && pos.x > xMin) {
			if (pos.z < yMax && pos.z > yMin) {
				return true;
			}
		}
		return false;
	}
}
