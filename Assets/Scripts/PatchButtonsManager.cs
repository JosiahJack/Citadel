using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PatchButtonsManager : MonoBehaviour {
	public GameObject[] patchButtons;
	[SerializeField] private GameObject[] patchCountsText;
	private int index;

	void LateUpdate() {
		if (PatchInventory.PatchInvInstance == null) {
			Const.sprint("ERROR->PatchButtonsManager: PatchInventory is null",Const.a.allPlayers);
			return;
		}

		for (int i=0; i<7; i++) {
			if (PatchInventory.PatchInvInstance.patchCounts[i] > 0) {
				patchButtons[i].SetActive(true);
				patchCountsText[i].SetActive(true);
			} else {
				patchButtons[i].SetActive(false);
				patchCountsText[i].SetActive(false);
			}
		}
	}
}
