using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScroll : MonoBehaviour {
	public Text creditsText;
	private bool bottom = false;
	public MainMenuHandler mainMenuHandler;
	public int pagenum = 0;

    void OnEnable() {
		bottom = false;
		pagenum = 0;
		creditsText.text = Const.a.creditsText[pagenum];
		if (Const.a.gameFinished) {
			// Get player stats for finishing the game

		}
    }

	void Update () {
		// Escape/back button listener
		if (Input.GetKeyDown(KeyCode.Escape)) {
			mainMenuHandler.GoBack();
		}

		if (Input.GetMouseButtonUp(0)) {
			if (!bottom) {
				pagenum++;
				if (!Const.a.gameFinished) {
					if (pagenum == 1) pagenum++; // skip stats when playing from main menu
				}
				if (pagenum >= Const.a.creditsLength) {
					bottom = true;
				} else {
					creditsText.text = Const.a.creditsText[pagenum];
				}
			} else {
				mainMenuHandler.GoBack();
			}
		}

		if (Input.GetMouseButtonUp(1)) {
			pagenum--;
			if (pagenum <0) pagenum = 0;
			creditsText.text = Const.a.creditsText[pagenum];
		}
	}
}
