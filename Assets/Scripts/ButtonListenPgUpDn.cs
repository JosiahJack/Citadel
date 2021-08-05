using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonListenPgUpDn : MonoBehaviour {
	public Button[] button;
	private int curTab = 0;

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (Input.GetKeyDown(KeyCode.PageUp)) {
				curTab--;
				if (curTab < 0)
					curTab = 3; // Wrap around
				button[curTab].onClick.Invoke();
			}
			if (Input.GetKeyDown(KeyCode.PageDown)) {
				curTab++;
				if (curTab > 3)
					curTab = 0; // Wrap around
				button[curTab].onClick.Invoke();
			}
		}
	}
}
