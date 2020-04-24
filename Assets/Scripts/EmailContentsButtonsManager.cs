using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailContentsButtonsManager : MonoBehaviour {
	public GameObject[] EmailButtons;
	public MultiMediaLogButton[] mmLBs;

	void Update() {
		for (int i=0; i<EmailButtons.Length; i++) {
			// Only show category buttons for levels we have logs from
			if (mmLBs[i].logReferenceIndex == -1) continue;
			if (LogInventory.a.hasLog[mmLBs[i].logReferenceIndex]) {
				EmailButtons[i].SetActive(true);
			} else {
				EmailButtons[i].SetActive(false);
			}
		}
	}
}
