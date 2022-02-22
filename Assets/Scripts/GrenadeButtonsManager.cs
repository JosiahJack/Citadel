using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrenadeButtonsManager : MonoBehaviour {
	public GameObject[] grenButtons;
	public GameObject[] grenCountsText;

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			for (int i=0; i<7; i++) {
				if (Inventory.a.grenAmmo[i] > 0) {
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