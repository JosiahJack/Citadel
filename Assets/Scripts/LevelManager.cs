using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {
	public int currentLevel;
	public static LevelManager a;
	public GameObject[] levels;
	public GameObject currentPlayer;
	public GameObject elevatorControl;
	private int oldLevel;

	void Awake () {
		a = this;
		print("LevelManager Awake(): Current level: " + currentLevel);
		oldLevel = (currentLevel - 1); // set initially to a value not equal to currentLevel so Update can set correctly active levels
	}

	public void LoadLevel (int levnum, GameObject targetDestination) {
		// NOTE: Check this first since the button for the current level has a null destination
		if (currentLevel == levnum) {
			Const.sprint("Already there"); // Do nothing
			return;
		}
			
		if (currentPlayer == null) {
			Const.sprint("BUG: LevelManager cannot find current player.");
			return; // prevent possible error if keypad does not have player to move
		}

		if (targetDestination == null) {
			Const.sprint("BUG: LevelManager cannot find destination.");
			return; // prevent possible error if keypad does not have destination set
		}
			
		levels[levnum].SetActive(true); // Load new level
		elevatorControl.SetActive(false);
		GUIState.a.isBlocking = false;
		currentPlayer.GetComponentInChildren<MouseLookScript>().overButton = false;
		currentPlayer.GetComponentInChildren<MouseLookScript>().overButtonType = -1;
		currentPlayer.transform.position = targetDestination.transform.position; // Put player in the new level
		// TODO: Check if any other player is in the level before deactivating
		levels[currentLevel].SetActive(false); // Unload current level
		currentLevel = levnum; // Set current level to be the new level
	}

	void Update () {
		if (currentLevel != oldLevel) {
			for (int i=0;i<levels.Length;i++) {
				if (i != currentLevel)
					levels[i].SetActive(false);
				else
					levels[i].SetActive(true);
			}
			oldLevel = currentLevel; // only perform pass to disable levels without players once per change in currentLevel
		}
	}
}
