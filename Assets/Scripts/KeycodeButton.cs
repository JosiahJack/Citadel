using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeycodeButton : MonoBehaviour {
	public GameObject playerCamera;
	public GameObject keycodeController;
	public int index;

	public void PtrEnter () {
		GUIState.a.PtrHandler(true,true,ButtonType.Generic,gameObject);
	}

	public void PtrExit () {
		GUIState.a.PtrHandler(false,false,ButtonType.None,null);
	}

	void KeycodeButtonClick () {
		keycodeController.GetComponent<KeypadKeycodeButtons>().Keypress(index);
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { KeycodeButtonClick(); });
	}
}
