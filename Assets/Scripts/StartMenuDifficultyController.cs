using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuDifficultyController : MonoBehaviour {
	public StartMenuButtonHighlight button0;
	public StartMenuButtonHighlight button1;
	public StartMenuButtonHighlight button2;
	public StartMenuButtonHighlight button3;
	public int difficultySetting;
	public string[] highlightString;
	public Text externalTextObject;

	public void SetDifficulty(int value) {
		difficultySetting = value;
		externalTextObject.text = highlightString[difficultySetting];
		HighlightUpdate();
	}

	void ResetHighlights() {
		button0.DeHighlight();
		button1.DeHighlight();
		button2.DeHighlight();
		button3.DeHighlight();
		externalTextObject.text = highlightString[difficultySetting];
	}

	public void ClickViaKeyboard() {
		difficultySetting++;
		if (difficultySetting >= 4) difficultySetting = 0;
		externalTextObject.text = highlightString[difficultySetting];
		HighlightUpdate();
	}

	void OnEnable() {
		HighlightUpdate();
	}

	public void HighlightUpdate() {
		//Debug.Log(gameObject.name + " difficultySetting: "
		//		  + difficultySetting.ToString());

		switch (difficultySetting) {
			case 0:
				ResetHighlights();
				button0.Highlight();
				break;
			case 1:
				ResetHighlights();
				button1.Highlight();
				break;
			case 2:
				ResetHighlights();
				button2.Highlight();
				break;
			case 3:
				ResetHighlights();
				button3.Highlight();
				break;
		}
	}
}