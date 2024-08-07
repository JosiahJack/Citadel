using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuDifficultyController : MonoBehaviour {
	public StartMenuButtonHighlight button0;
	public StartMenuButtonHighlight button1;
	public StartMenuButtonHighlight button2;
	public StartMenuButtonHighlight button3;
	public int difficultyType = 0; // 0 = Combat, 1 = Mission, 2 = Puzzle, 3 = Cyberspace
	public int difficultySetting;
	public Text externalTextObject;

	public void SetText() {
		switch(difficultyType) {
			case 0: // Combat
				switch(difficultySetting) {
					case 0: externalTextObject.text = Const.a.stringTable[752]; break;
					case 1: externalTextObject.text = Const.a.stringTable[753]; break;
					case 2: externalTextObject.text = Const.a.stringTable[754]; break;
					case 3: externalTextObject.text = Const.a.stringTable[755]; break;
				}
				
				break;
			case 1: // Mission
				switch(difficultySetting) {
					case 0: externalTextObject.text = Const.a.stringTable[756]; break;
					case 1: externalTextObject.text = Const.a.stringTable[757]; break;
					case 2: externalTextObject.text = Const.a.stringTable[758]; break;
					case 3: externalTextObject.text = Const.a.stringTable[759]; break;
				}
				
				break;
			case 2: // Puzzle
				switch(difficultySetting) {
					case 0: externalTextObject.text = Const.a.stringTable[760]; break;
					case 1: externalTextObject.text = Const.a.stringTable[761]; break;
					case 2: externalTextObject.text = Const.a.stringTable[762]; break;
					case 3: externalTextObject.text = Const.a.stringTable[763]; break;
				}
				
				break;
			case 3: // Cyberspace
				switch(difficultySetting) {
					case 0: externalTextObject.text = Const.a.stringTable[764]; break;
					case 1: externalTextObject.text = Const.a.stringTable[765]; break;
					case 2: externalTextObject.text = Const.a.stringTable[766]; break;
					case 3: externalTextObject.text = Const.a.stringTable[767]; break;
				}
				
				break;
		}
	}
	
	public void SetDifficulty(int value) {
		difficultySetting = value;
		SetText();
		HighlightUpdate();
	}

	void ResetHighlights() {
		button0.DeHighlight();
		button1.DeHighlight();
		button2.DeHighlight();
		button3.DeHighlight();
		SetText();
	}

	public void ClickViaKeyboard() {
		difficultySetting++;
		if (difficultySetting >= 4) difficultySetting = 0;
		SetText();
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
