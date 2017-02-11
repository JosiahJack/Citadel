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

	void Start () {
		selfText.text = Const.a.InputConfigNames[Const.a.InputCodeSettings[index]];
	}

	public void KeybindButtonClick() {
		if (!enterMode) {
			self.GetComponentInChildren<Text>().text = "...";
			enterMode = true;
		}
	}

	void Update () {
		if (enterMode) {
			if (firstFrame) {
				firstFrame = false;
				return; // prevent capturing the click in input check
			}

			for (int i=0;i<159;i++) {
				if (Input.GetKeyUp(Const.a.InputValues[i])) {
					selfText.text = Const.a.InputConfigNames[i];
					Const.a.InputCodeSettings[index] = i;
					Const.a.WriteConfig();
					enterMode = false;
					firstFrame = true;
					return;
				}
			}
		}

	}
}
