using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrenadeButtonsManager : MonoBehaviour {
	public GameObject[] grenButtons;
	public GameObject[] grenCountsText;

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			for (int i=0; i<7; i++) {
				if (GrenadeInventory.GrenadeInvInstance.grenAmmo[i] > 0) {
					if (!grenButtons[i].activeInHierarchy) grenButtons[i].SetActive(true);
					if (!grenCountsText[i].activeInHierarchy) grenCountsText[i].SetActive(true);
				} else {
					if (grenButtons[i].activeInHierarchy) grenButtons[i].SetActive(false);
					if (grenCountsText[i].activeInHierarchy) grenCountsText[i].SetActive(false);
				}
			}
		}
	}
}