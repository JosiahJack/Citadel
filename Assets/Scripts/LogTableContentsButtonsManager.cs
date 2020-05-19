using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogTableContentsButtonsManager : MonoBehaviour {
	public GameObject[] LogButtons;

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			for (int i=0; i<10; i++) {
				// Only show category buttons for levels we have logs from
				if (LogInventory.a.numLogsFromLevel[i] > 0) {
					LogButtons[i].SetActive(true);
				} else {
					LogButtons[i].SetActive(false);
				}
			}
		}
	}
}
