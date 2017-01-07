using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuArrowKeyControls : MonoBehaviour {
	private int currentIndex;
	public GameObject[] menuItems;
	public GameObject[] menuSubItems;

	void Awake () {
		currentIndex = 0;
	}

	void  Update () {
		if (Input.GetKeyUp(KeyCode.Return)) {
			//menuItems[currentIndex].SendMessage("Click",SendMessageOptions.DontRequireReceiver);
			if (menuItems[currentIndex].GetComponent<Button>() != null) {
				menuItems[currentIndex].GetComponent<Button>().onClick.Invoke();
			} else {
				menuItems[currentIndex].SendMessage("ClickViaKeyboard",SendMessageOptions.DontRequireReceiver);
			}
			return;
		}

		if (Input.GetKeyUp(KeyCode.UpArrow)) {
			if (menuItems[currentIndex] != null) menuItems[currentIndex].SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);
			if (menuSubItems[currentIndex] != null) menuSubItems[currentIndex].SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);
				
			currentIndex--;
			if (currentIndex < 0) currentIndex = (menuItems.Length - 1); // Wrap around :)

			if (menuItems[currentIndex] != null) menuItems[currentIndex].SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
			if (menuSubItems[currentIndex] != null) menuSubItems[currentIndex].SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
		}

		if (Input.GetKey(KeyCode.DownArrow)) {
			if (menuItems[currentIndex] != null) menuItems[currentIndex].SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);
			if (menuSubItems[currentIndex] != null) menuSubItems[currentIndex].SendMessage("DeHighlight",SendMessageOptions.DontRequireReceiver);

			currentIndex++;
			if (currentIndex >= menuItems.Length) currentIndex = 0; // Wrap around :)

			if (menuItems[currentIndex] != null) menuItems[currentIndex].SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
			if (menuSubItems[currentIndex] != null) menuSubItems[currentIndex].SendMessage("Highlight",SendMessageOptions.DontRequireReceiver);
		}
	}
}
