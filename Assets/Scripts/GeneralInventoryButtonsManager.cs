using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GeneralInventoryButtonsManager : MonoBehaviour {
	public GameObject[] genButtons;

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			for (int i=0; i<14; i++) {
				if (GeneralInventory.GeneralInventoryInstance.generalInventoryIndexRef[i] > -1) {
					if (!genButtons[i].activeInHierarchy) genButtons[i].SetActive(true);
				} else {
					if (genButtons[i].activeInHierarchy) genButtons[i].SetActive(false);
				}
			}
		}
	}
}
