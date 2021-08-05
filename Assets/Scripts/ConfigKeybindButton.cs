using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigKeybindButton : MonoBehaviour {
	private Button self;
	private bool enterMode;
	private bool firstFrame;
	private Text selfText;
	public int index;

	void Awake () {
		self = GetComponent<Button>();
		selfText = GetComponentInChildren<Text>();
		enterMode = false;
		firstFrame = true;
	}

	void Start() {
		UpdateText();
	}

	public void UpdateText() {
		if (selfText != null) {
			selfText.text = Const.a.InputConfigNames[Const.a.InputCodeSettings[index]];
			if (Const.a.InputCodeSettings[index] == 109) selfText.color = Const.a.ssRedText;
			else selfText.color = Const.a.ssGreenText;
		}
	}

	public void KeybindButtonClick() {
		if (MainMenuHandler.a.PresetConfirmDialog.activeSelf) return;

		if (!enterMode) {
			self.GetComponentInChildren<Text>().text = "...";
			enterMode = true;
		}
	}

	void CheckAndHandleConflicts(int checkVal) {
		// Debug.Log("Checking value of for "+ checkVal.ToString() + " keybind " + Const.a.InputConfigNames[Const.a.InputCodeSettings[checkVal]] + " doesn't match another keybind");
		for (int i=0;i<Const.a.InputCodeSettings.Length;i++) {
			if (i == index) continue; // We already know this one is us.

			if (Const.a.InputCodeSettings[i] == checkVal) {
				Const.a.InputCodeSettings[i] = 109; Const.sprint("Found and unbound conflict with " + Const.a.InputCodes[i],Const.a.player1);
				// cbm.configButtons[i].index = 109;
				// cbm.configButtons[i].UpdateText();
				break;
			}
		}
	}

	void Update () {
		if (enterMode) {
			if (firstFrame) {
				firstFrame = false;
				return; // Prevent capturing the click in input check when first clicking on the button to enter the entry mode.
			}

			bool goodkey = false;
			for (int i=0;i<159;i++) {
				if (i == 139) {if (Input.GetKeyDown(KeyCode.CapsLock)) goodkey = true; } // elseif block to prevent bad check for keys that Unity doesn't recognize in GetKeyUp/GetKey
				else if (i == 153) { if (GetInput.a.MouseWheelUp()) goodkey = true; }
				else if (i == 154) { if (GetInput.a.MouseWheelDn()) goodkey = true; }
				else if (Input.GetKeyUp(Const.a.InputValues[i])) goodkey = true;

				if (goodkey) {
					selfText.text = Const.a.InputConfigNames[i];
					Const.a.InputCodeSettings[index] = i;
					Const.a.WriteConfig();
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
}
