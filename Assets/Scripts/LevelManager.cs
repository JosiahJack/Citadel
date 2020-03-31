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
	public Transform[] ressurectionLocation;
	public bool[] ressurectionActive;
	//public GameObject currentPlayer;
	//public GameObject elevatorControl;
	public GameObject sky;
	public bool superoverride = false;
	public GameObject saturn;
	public MeshRenderer skyMR;
	public bool[] showSkyForLevel;
	public bool[] showSaturnForLevel;
	public enum SecurityType{None,Camera,NodeSmall,NodeLarge};

	void Awake () {
		a = this;
		if (currentLevel == -1) return;
		SetAllPlayersLevelsToCurrent();
		DisableAllNonOccupiedLevels();
		if (sky != null)
			sky.SetActive(true);

		skyMR.enabled = false;
		saturn.SetActive(false); //using GO because of ring children
		if (showSkyForLevel[currentLevel]) skyMR.enabled = true;
		if (showSaturnForLevel[currentLevel]) saturn.SetActive(true);
		Time.timeScale = Const.a.defaultTimeScale;
	}

	public void CyborgConversionToggleForCurrentLevel() {
		ressurectionActive[currentLevel] = !ressurectionActive[currentLevel]; // toggle current level
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
			currentPlayer.GetComponent<PlayerReferenceManager>().playerCapsule.transform.position = ressurectionLocation[currentLevel].position; //teleport to ressurection chamber
			currentPlayer.GetComponent<PlayerReferenceManager>().playerDeathRessurectEffect.SetActive(true); // activate death screen and readouts for "BRAIN ACTIVITY SATISFACTORY"            ya debatable right
			currentPlayer.GetComponent<PlayerReferenceManager>().playerCapsule.GetComponent<PlayerMovement>().ressurectingFinished = Time.time + 3f;
			return true;
		}

		return false;
	}

	public void LoadLevel (int levnum, GameObject targetDestination, GameObject currentPlayer) {
		// NOTE: Check this first since the button for the current level has a null destination
		if (currentLevel == levnum) {
			Const.sprint("Already there",currentPlayer); // Do nothing
			return;
		}
			
		if (currentPlayer == null) {
			Const.sprint("BUG: LevelManager cannot find current player.",Const.a.allPlayers);
			return; // prevent possible error if keypad does not have player to move
		}

		if (targetDestination == null) {
			Const.sprint("BUG: LevelManager cannot find destination.",Const.a.allPlayers);
			return; // prevent possible error if keypad does not have destination set
		}
			
		MFDManager.a.TurnOffElevatorPad();
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
		currentPlayer.GetComponent<PlayerReferenceManager>().playerCapsule.transform.position = targetDestination.transform.position; // Put player in the new level

		levels[levnum].SetActive(true); // enable new level
		currentPlayer.GetComponent<PlayerReferenceManager>().playerCurrentLevel = levnum;
		if (levnum == 13) currentPlayer.GetComponent<PlayerReferenceManager> ().playerCapsule.GetComponent<PlayerMovement> ().inCyberSpace = true;
		currentLevel = levnum; // Set current level to be the new level
		DisableAllNonOccupiedLevels();
		if (showSkyForLevel[currentLevel]) skyMR.enabled = true; else skyMR.enabled = false;
		if (showSaturnForLevel[currentLevel]) saturn.SetActive(true); else saturn.SetActive(false);
	}

	public void LoadLevelFromSave (int levnum) {
		// NOTE: Check this first since the button for the current level has a null destination
		if (currentLevel == levnum) {
			return;
		}
			
		levels[levnum].SetActive(true); // Load new level
		levels[currentLevel].SetActive(false); // Unload current level
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

	void DisableAllNonOccupiedLevels() {
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

	public GameObject GetCurrentLevelDynamicContainer() {
        GameObject retval = null;
		if (currentLevel == -1) return retval;
        if (currentLevel < levels.Length)
            retval = levels[currentLevel].GetComponent<Level>().dynamicObjectsContainer;

        return retval;
	}

	public int GetCurrentLevelSecurity() {
		if (currentLevel == -1) return 0;
		if (superoverride) return 0; // tee hee we are SHODAN, no security blocks in place
		return levelSecurity [currentLevel];
	}

	public void RegisterSecurityObject(int lev,SecurityType stype) {
		if (lev > 14 || lev < 0) return;
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

		int camScore = 2;
		int nodeSmallScore = 8;
		int nodeLargeScore = 15;
		int secscoreTotal = (levelCameraCount[currentLevel] * camScore) + (levelSmallNodeCount[currentLevel] * nodeSmallScore) + (levelLargeNodeCount[currentLevel] * nodeLargeScore);
		int secDrop = camScore; // default to camScore
		switch (stype) {
			case SecurityType.None: return;
			case SecurityType.Camera: secDrop = (int)Mathf.Ceil((camScore/secscoreTotal) * 100); break; // 1 camera divided by the total, so 2/ say (40+60) = 2/100 = 0.02, or 2% using the example numbers above
			case SecurityType.NodeSmall: secDrop = (int)Mathf.Ceil((nodeSmallScore/secscoreTotal) * 100); break;
			case SecurityType.NodeLarge: secDrop = (int)Mathf.Ceil((nodeLargeScore/secscoreTotal) * 100); break;
		}

		levelSecurity [currentLevel] -= secDrop;
		if (levelSecurity [currentLevel] < 0) levelSecurity [currentLevel] = 0;
		Const.sprint("Level security now " + levelSecurity[currentLevel].ToString() + "%",Const.a.allPlayers);
	}
}
