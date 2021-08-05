using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScroll : MonoBehaviour {
	public Text creditsText;
	private bool bottom = false;
	public int pagenum = 0;
	public GameObject exitVideo;

    void OnEnable() {
		bottom = false;
		pagenum = 0;
		creditsText.text = Const.a.creditsText[pagenum];
		exitVideo.SetActive(true);
		if (Const.a.gameFinished) {
			// Get player stats for finishing the game
			Const.a.creditsText[1] = Const.a.CreditsStats();
		}
    }

	void Update () {
		// Escape/back button listener
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (exitVideo.activeSelf) {
				exitVideo.SetActive(false);
				return;
			}
			MainMenuHandler.a.GoBack();
		}

		if (Input.GetMouseButtonUp(0)) {
			if (exitVideo.activeSelf) {
				return;
			}

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
				MainMenuHandler.a.GoBack();
			}
		}

		if (Input.GetMouseButtonUp(1)) {
			if (exitVideo.activeSelf) {
				return;
			}

			pagenum--;
			if (pagenum <0) pagenum = 0;
			creditsText.text = Const.a.creditsText[pagenum];
		}
	}
}
