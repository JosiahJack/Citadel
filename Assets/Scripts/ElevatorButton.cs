using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ElevatorButton : MonoBehaviour {
	public bool floorAccessible = false;
	public bool tooFarAway = false;
	public GameObject playerCamera;
	private GameObject levelManager;
	[HideInInspector]
	public GameObject targetDestination;
	private Text childText;
	private int levelIndex;


	void Awake() {
		levelManager = GameObject.Find("LevelManager");
		if (levelManager == null)
			Const.sprint("Warning: Could Not Find object 'LevelManager' in scene\n");
		childText = GetComponentInChildren<Text>();
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { ElevButtonClick(); });
	}

	public void PtrEnter () {
		GUIState.isBlocking = true;
		playerCamera.GetComponent<MouseLookScript>().overButton = true;
		playerCamera.GetComponent<MouseLookScript>().overButtonType = 77;
	}

	public void PtrExit () {
		GUIState.isBlocking = false;
		playerCamera.GetComponent<MouseLookScript>().overButton = false;
		playerCamera.GetComponent<MouseLookScript>().overButtonType = -1;
	}

	void ElevButtonClick () {
		if (tooFarAway) {
			Const.sprint("You are too far away from that");
		} else {
			if (floorAccessible && levelManager != null) {
				levelManager.GetComponent<LevelManager>().LoadLevel(levelIndex,targetDestination);
			} else {
				Const.sprint("Shaft Damage -- Unable to go there.");
			}
		}
	}

	public void SetTooFarTrue() { tooFarAway = true; }
	public void SetTooFarFalse() { tooFarAway = false; }

	void Update() {
		if (childText.text == "R") {
			levelIndex = 0;
		}
		if (childText.text == "1") {
			levelIndex = 1;
		}
		if (childText.text == "2") {
			levelIndex = 2;
		}
		if (childText.text == "3") {
			levelIndex = 3;
		}
		if (childText.text == "4") {
			levelIndex = 4;
		}
		if (childText.text == "5") {
			levelIndex = 5;
		}
		if (childText.text == "6") {
			levelIndex = 6;
		}
		if (childText.text == "7") {
			levelIndex = 7;
		}
		if (childText.text == "8") {
			levelIndex = 8;
		}
		if (childText.text == "9") {
			levelIndex = 9;
		}
		if (childText.text == "G1") {
			levelIndex = 10;
		}
		if (childText.text == "G2") {
			levelIndex = 11;
		}
		if (childText.text == "G4") {
			levelIndex = 12;
		}
		if (childText.text == "C") {
			levelIndex = 13;
		}
	}
}
