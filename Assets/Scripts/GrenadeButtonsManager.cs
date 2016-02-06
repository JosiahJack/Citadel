using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrenadeButtonsManager : MonoBehaviour {
	[SerializeField] private GameObject[] grenButtons;
	[SerializeField] private GameObject[] grenCountsText;

	void Update() {
		for (int i=0; i<7; i++) {
			if (GrenadeInventory.GrenadeInvInstance.grenAmmo[i] > 0) {
				grenButtons[i].SetActive(true);
				grenCountsText[i].SetActive(true);
            } else {
				grenButtons[i].SetActive(false);
				grenCountsText[i].SetActive(false);
			}
		}
	}
}
