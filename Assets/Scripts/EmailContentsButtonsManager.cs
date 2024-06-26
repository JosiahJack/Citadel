﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailContentsButtonsManager : MonoBehaviour {
	public GameObject[] EmailButtons;
	public MultiMediaLogButton[] mmLBs;

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			for (int i=0; i<EmailButtons.Length; i++) {
				// Only show category buttons for levels we have logs from
				if (mmLBs[i].logReferenceIndex == -1) continue;
				if (Inventory.a.hasLog[mmLBs[i].logReferenceIndex]) {
					if (!EmailButtons[i].activeSelf) EmailButtons[i].SetActive(true);
				} else {
					if (EmailButtons[i].activeSelf) EmailButtons[i].SetActive(false);
				}
			}
		}
	}
}
