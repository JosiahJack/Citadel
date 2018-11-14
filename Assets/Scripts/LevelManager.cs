using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {
	public int currentLevel;
	public static LevelManager a;
	public GameObject[] levels;
	public int[] levelSecurity;
	//public GameObject currentPlayer;
	//public GameObject elevatorControl;
	public GameObject sky;

	void Awake () {
		a = this;
		//print("LevelManager Awake(): Current level: " + currentLevel);
		SetAllPlayersLevelsToCurrent();
		DisableAllNonOccupiedLevels();
		if (sky != null)
			sky.SetActive(true);

		Time.timeScale = Const.a.defaultTimeScale;
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
			if (p1level != i && p2level != i && p3level != i && p4level != i)
				levels[i].SetActive(false);
			else
				levels[i].SetActive(true);
		}
	}

	public GameObject GetCurrentLevelDynamicContainer() {
        GameObject retval = null;
        if (currentLevel < levels.Length)
            retval = levels[currentLevel].GetComponent<Level>().dynamicObjectsContainer;

        return retval;
	}

	public int GetCurrentLevelSecurity() {
		return levelSecurity [currentLevel];
	}

	public void ReduceCurrentLevelSecurity(int secDrop) {
		levelSecurity [currentLevel] -= secDrop;
		if (levelSecurity [currentLevel] < 0)
			levelSecurity [currentLevel] = 0; // limit reduction in case of setup errors.  Calculate your sec levels carefully!!
	}
}
