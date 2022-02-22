using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ElevatorButton : MonoBehaviour {
	public bool floorAccessible = false;
	public bool doorOpen = false;
	public GameObject currentPlayer;

	// Externally modified
	[HideInInspector] public GameObject targetDestination;

	// Internal references
	private Text childText;
	private int levelIndex;
	private string levR = "R";
	private string lev1 = "1";
	private string lev2 = "2";
	private string lev3 = "3";
	private string lev4 = "4";
	private string lev5 = "5";
	private string lev6 = "6";
	private string lev7 = "7";
	private string lev8 = "8";
	private string lev9 = "9";
	private string levG1 = "G1";
	private string levG2 = "G2";
	private string levG4 = "G4";
	private string levC = "C";

	void Awake() {
		childText = GetComponentInChildren<Text>();
		doorOpen = false;
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { ElevButtonClick(); });
	}

	void ElevButtonClick () {
		if (Vector3.Distance(MFDManager.a.objectInUsePos,MFDManager.a.playerCapsuleTransform.position) > Const.a.elevatorPadUseDistance || MFDManager.a.linkedElevatorDoor == null) {
			Const.sprint(Const.a.stringTable[6],currentPlayer);
		} else {
			if (MFDManager.a.linkedElevatorDoor.doorOpen != Door.doorState.Closed) {
				Const.sprint(Const.a.stringTable[7],currentPlayer);
			} else {
				if (floorAccessible) {
					LevelManager.a.LoadLevel(levelIndex,targetDestination,currentPlayer,targetDestination.transform.position);
				} else {
					Const.sprint(Const.a.stringTable[8],currentPlayer);
				}
			}
		}
	}

	void OnEnable() {
		if (childText.text == levR) {
			levelIndex = 0;
		} else if (childText.text == lev1) {
			levelIndex = 1;
		} else if (childText.text == lev2) {
			levelIndex = 2;
		} else if (childText.text == lev3) {
			levelIndex = 3;
		} else if (childText.text == lev4) {
			levelIndex = 4;
		} else if (childText.text == lev5) {
			levelIndex = 5;
		} else if (childText.text == lev6) {
			levelIndex = 6;
		} else if (childText.text == lev7) {
			levelIndex = 7;
		} else if (childText.text == lev8) {
			levelIndex = 8;
		} else if (childText.text == lev9) {
			levelIndex = 9;
		} else if (childText.text == levG1) {
			levelIndex = 10;
		} else if (childText.text == levG2) {
			levelIndex = 11;
		} else if (childText.text == levG4) {
			levelIndex = 12;
		} else if (childText.text == levC) {
			levelIndex = 13;
		}
	}
}
