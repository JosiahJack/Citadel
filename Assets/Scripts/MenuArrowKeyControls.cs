using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuArrowKeyControls : MonoBehaviour {
	public int currentIndex;
	public GameObject[] menuItems;
	/*[DTValidator.Optional] */public GameObject[] menuSubItems;

	void Awake () {
		currentIndex = 0;
	}

	void OnEnable() {
		SetIndex(0);
	}

	void  Update() {
		if (PlayerMovement.a.consoleActivated) return;

		if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter)) {
			if (menuItems[currentIndex].GetComponent<Button>() != null) {
				menuItems[currentIndex].GetComponent<Button>().onClick.Invoke();
			} else {
				if (menuItems[currentIndex].GetComponent<StartMenuNameAKeyEnter>() != null) menuItems[currentIndex].GetComponent<StartMenuNameAKeyEnter>().ClickViaKeyboard();
				if (menuItems[currentIndex].GetComponent<StartMenuDifficultyController>() != null) menuItems[currentIndex].GetComponent<StartMenuDifficultyController>().ClickViaKeyboard();
			}
			return;
		}

		if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.LeftArrow)) ShiftMenuItem(false);
		if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.RightArrow)) ShiftMenuItem(true);
	}

	void ShiftMenuItem (bool isDownKey) {
		if (menuItems[currentIndex] != null) {
			menuItems[currentIndex].SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);
			menuItems[currentIndex].SendMessage("InputFieldCancelFocus",SendMessageOptions.DontRequireReceiver);
		}
		if (menuSubItems[currentIndex] != null) menuSubItems[currentIndex].SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);

		if (isDownKey) {
			currentIndex++;
			if (currentIndex >= menuItems.Length) currentIndex = 0; // Wrap around :)
		} else {
			currentIndex--;
			if (currentIndex < 0) currentIndex = (menuItems.Length - 1); // Wrap around :)
		}

		if (menuItems[currentIndex] != null) menuItems[currentIndex].SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
		if (menuItems[currentIndex] != null) menuItems[currentIndex].SendMessage("InputFieldFocus",SendMessageOptions.DontRequireReceiver);
		if (menuSubItems[currentIndex] != null) menuSubItems[currentIndex].SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
	}

	public void SetIndex(int index) {
		currentIndex = index;
		for (int i=0;i<menuItems.Length;i++) {
			menuItems[i].SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);
			menuItems[i].SendMessage("InputFieldCancelFocus",SendMessageOptions.DontRequireReceiver);
		}

		if (menuItems[currentIndex] != null) menuItems[currentIndex].SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
		if (menuItems[currentIndex] != null) menuItems[currentIndex].SendMessage("InputFieldFocus",SendMessageOptions.DontRequireReceiver);
		if (menuSubItems[currentIndex] != null) menuSubItems[currentIndex].SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
	}
}
