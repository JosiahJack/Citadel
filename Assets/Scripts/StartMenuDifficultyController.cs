using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuDifficultyController : MonoBehaviour {
	public GameObject button0;
	public GameObject button1;
	public GameObject button2;
	public GameObject button3;
	public int difficultySetting;
	public string[] highlightString;
	public Text externalTextObject;

	public void SetDifficulty (int value) {
		difficultySetting = value;
		externalTextObject.text = highlightString[difficultySetting];
	}

	void ResetHighlights () {
		button0.GetComponent<StartMenuButtonHighlight>().SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);
		button1.GetComponent<StartMenuButtonHighlight>().SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);
		button2.GetComponent<StartMenuButtonHighlight>().SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);
		button3.GetComponent<StartMenuButtonHighlight>().SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);
		externalTextObject.text = highlightString[difficultySetting];
	}

	void ClickViaKeyboard () {
		difficultySetting++;
		if (difficultySetting >= 4)
			difficultySetting = 0;

		externalTextObject.text = highlightString[difficultySetting];
	}

	void Update () {
		switch (difficultySetting) {
		case 0:
			ResetHighlights();
			button0.GetComponent<StartMenuButtonHighlight>().SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
			break;
		case 1:
			ResetHighlights();
			button1.GetComponent<StartMenuButtonHighlight>().SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
			break;
		case 2:
			ResetHighlights();
			button2.GetComponent<StartMenuButtonHighlight>().SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
			break;
		case 3:
			ResetHighlights();
			button3.GetComponent<StartMenuButtonHighlight>().SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
			break;
		}
	}
}
