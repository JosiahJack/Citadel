using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigKeybindButton : MonoBehaviour {
	// Externally set per instance
	public int index;

	// Internal references
	private Button self;
	private bool enterMode;
	private bool firstFrame;
	private Text selfText;

	void Start() { // Wait for Const.a. to initialize.
		Initialize();
	}

	void OnEnable() {
		Initialize();
	}

	void Initialize() {
		if (index < 0 || index >= 40) {
			Debug.Log("BUG: ConfigKeybindButton index out of range [0,40]");
			index = 0;
		}

		enterMode = false;
		firstFrame = true;
		if (self == null) self = GetComponent<Button>();
		if (self == null) {
			Debug.Log("BUG: ConfigKeybindButton missing component for self.");
		}

		if (selfText == null) selfText = GetComponentInChildren<Text>(true);

		UpdateText();
	}

	public void KeybindButtonClick() {
		if (MainMenuHandler.a.PresetConfirmDialog.activeSelf) return;

		if (!enterMode) {
			self.GetComponentInChildren<Text>(true).text = "...";
			enterMode = true;
		}
	}

	void CheckAndHandleConflicts(int checkVal) {
		for (int i=0;i<Const.a.InputCodeSettings.Length;i++) {
			if (i == index) continue; // We already know this one is us.

			if (Const.a.InputCodeSettings[i] == checkVal) {
				Const.a.InputCodeSettings[i] = 109;
				Const.sprint("Found and unbound conflict with "
							 + Const.a.InputCodes[i],Const.a.player1);
				break;
			}
		}
	}

	void Update() {
		if (enterMode) {
			// Prevent capturing the click in input check when first clicking
			// on the button to enter the entry mode.
			if (firstFrame) {firstFrame = false; return; } 

			bool goodkey = false;
			// Prevent checking keys Unity doesn't recognize in GetKeyUp/GetKey
			for (int i=0;i<159;i++) {
				if (i == 139) {
					if (Input.GetKeyDown(KeyCode.CapsLock)) goodkey = true;
				} else if (i == 153) {
					if (GetInput.a.MouseWheelUp()) goodkey = true;
				} else if (i == 154) {
					if (GetInput.a.MouseWheelDn()) goodkey = true;
				} else {
					if (Input.GetKeyUp(Const.a.InputValues[i])) goodkey = true;
				}

				if (goodkey) {
					selfText.text = Const.a.InputConfigNames[i];
					Const.a.InputCodeSettings[index] = i;
					Config.WriteConfig();
					enterMode = false;
					firstFrame = true;
					CheckAndHandleConflicts(i);
					return;
				}
			}
		} else {
			UpdateText();
		}
	}

	public void UpdateText() { // Called by Start also.
		if (selfText != null) {
			selfText.text = Const.a.InputConfigNames[Const.a.InputCodeSettings[index]];
			if (Const.a.InputCodeSettings[index] == 109) selfText.color = Const.a.ssRedText;
			else selfText.color = Const.a.ssGreenText;
		}
	}
}
