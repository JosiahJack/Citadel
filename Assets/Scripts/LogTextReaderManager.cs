using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogTextReaderManager : MonoBehaviour {
	public GameObject moreButton;
	public GameObject logTextOutput;
	public Text moreButtonText;
	public GameObject backButton;
	public LogBackButton logBackButton;
	public int refIndex = -1;
	
	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (logTextOutput.GetComponent<Text>().text.Length > 568) {
				moreButtonText.text = Const.a.stringTable[26];
				if (backButton.activeSelf) backButton.SetActive(false);
			} else {
				moreButtonText.text = Const.a.stringTable[27];
				if (!backButton.activeSelf && Const.a.audioLogSpeech2Text[refIndex].Length > 568) {
					backButton.SetActive(true);
					logBackButton.refIndex = refIndex;
				}
			}
		}
	}

	public void SendTextToReader(int referenceIndex) {
		if (referenceIndex < 0) {
			Debug.Log("BUG: Audiolog index was less than 0. Report from "
					  + "LogTextReaderManager.");
			return;
		}

		logTextOutput.GetComponent<Text>().text = Const.a.audioLogSpeech2Text[referenceIndex];
		refIndex = referenceIndex;
		if (Const.a.audioLogSpeech2Text[referenceIndex].Length > 568) {
			logBackButton.refIndex = referenceIndex;
		}
	}
}
