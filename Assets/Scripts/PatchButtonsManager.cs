using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PatchButtonsManager : MonoBehaviour {
	public GameObject[] patchButtons;
	public GameObject[] patchCountsText;
	private int index;

	void LateUpdate() {
		if (PatchInventory.PatchInvInstance == null) {
			Const.sprint("ERROR->PatchButtonsManager: PatchInventory is null",Const.a.allPlayers);
			return;
		}

		for (int i=0; i<7; i++) {
			if (PatchInventory.PatchInvInstance.patchCounts[i] > 0) {
				if (!patchButtons[i].activeInHierarchy) patchButtons[i].SetActive(true);
				if (!patchCountsText[i].activeInHierarchy) patchCountsText[i].SetActive(true);
			} else {
				if (patchButtons[i].activeInHierarchy) patchButtons[i].SetActive(false);
				if (patchCountsText[i].activeInHierarchy) patchCountsText[i].SetActive(false);
			}
		}
	}
}
