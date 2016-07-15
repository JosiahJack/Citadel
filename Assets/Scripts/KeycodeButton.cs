using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeycodeButton : MonoBehaviour {
	public GameObject playerCamera;
	public GameObject keycodeController;
	public int index;

	public void PtrEnter () {
		GUIState.a.isBlocking = true;
		playerCamera.GetComponent<MouseLookScript>().overButton = true;
		playerCamera.GetComponent<MouseLookScript>().overButtonType = 77;
	}

	public void PtrExit () {
		GUIState.a.isBlocking = false;
		playerCamera.GetComponent<MouseLookScript>().overButton = false;
		playerCamera.GetComponent<MouseLookScript>().overButtonType = -1;
	}

	void KeycodeButtonClick () {
		keycodeController.GetComponent<KeypadKeycodeButtons>().Keypress(index);
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { KeycodeButtonClick(); });
	}
}
