using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScroll : MonoBehaviour {
	public Text creditsText;
	private bool bottom = false;
	public int pagenum = 0;
	public GameObject exitVideo;
	private float vidFinished;
	public GameObject endVideoTextGO1;
	public GameObject endVideoTextGO2;
	public GameObject endVideoTextGO3;
	public Text endVideoText1;
	public Text endVideoText2;
	public Text endVideoText3;
	private float vidLength = 37.2f;
	private float vidStartTime;

    void OnEnable() {
		bottom = false;
		pagenum = 0;
		creditsText.text = Const.a.creditsText[pagenum];
		Utils.Activate(exitVideo);
		Utils.Activate(endVideoTextGO1);
		endVideoText1.text = Const.a.stringTable[610];
		endVideoText2.text = Const.a.stringTable[611];
		endVideoText3.text = Const.a.stringTable[612];
		Utils.Deactivate(endVideoTextGO2);
		Utils.Deactivate(endVideoTextGO3);
		vidFinished = Time.time + vidLength;
		vidStartTime = Time.time;
		if (Const.a.gameFinished) {
			// Get player stats for finishing the game!
			Const.a.creditsText[1] = Const.a.CreditsStats();
		}
    }

	void Update() {
		if (vidFinished > 0 && (Time.time - vidStartTime) > 7f
			&& endVideoTextGO1.activeSelf && !endVideoTextGO2.activeSelf) {

			Utils.Activate(endVideoTextGO2);
			Utils.Deactivate(endVideoTextGO1);
		}

		if (vidFinished > 0 && (Time.time - vidStartTime) > 11f
			&& endVideoTextGO2.activeSelf && !endVideoTextGO3.activeSelf) {

			Utils.Activate(endVideoTextGO3);
			Utils.Deactivate(endVideoTextGO2);
		}

		if (vidFinished > 0 && (Time.time - vidStartTime) > 14f
			&& endVideoTextGO3.activeSelf) {

			Utils.Deactivate(endVideoTextGO3);
		}

		if (vidFinished < Time.time && exitVideo.activeSelf
			&& vidFinished > 0) {

			vidFinished = 0;
			Utils.Deactivate(exitVideo);
			Utils.Deactivate(endVideoTextGO1);
			Utils.Deactivate(endVideoTextGO2);
			Utils.Deactivate(endVideoTextGO3);
		}

		if (Input.GetKeyUp(KeyCode.Escape)) {
			if (exitVideo.activeSelf) {
				Utils.Deactivate(exitVideo);
				return;
			}
			
			MainMenuHandler.a.GoBack();
		}

		if (Input.GetMouseButtonUp(0)) {
			if (exitVideo.activeSelf) return;

			if (!bottom) {
				pagenum++;
				if (!Const.a.gameFinished) {
					if (pagenum == 1) pagenum++; // Skip stats when playing
					                             // from main menu.
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
			if (exitVideo.activeSelf) return;

			pagenum--;
			if (pagenum <0) pagenum = 0;
			creditsText.text = Const.a.creditsText[pagenum];
		}
	}
}
