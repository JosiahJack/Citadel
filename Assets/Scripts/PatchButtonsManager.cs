using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PatchButtonsManager : MonoBehaviour {
	[SerializeField] private GameObject[] patchButtons;
	[SerializeField] private GameObject[] patchCountsText;
	private int index;

	void LateUpdate() {
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
