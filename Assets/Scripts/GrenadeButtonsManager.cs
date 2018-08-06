using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrenadeButtonsManager : MonoBehaviour {
	[SerializeField] private GameObject[] grenButtons;
	[SerializeField] private GameObject[] grenCountsText;

	void Update() {
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