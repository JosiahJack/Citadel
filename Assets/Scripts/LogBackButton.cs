using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class LogBackButton : MonoBehaviour {
	public GameObject logTextOutput;
	private string remainder = System.String.Empty;
	public int refIndex = -1;

	void LogBackButtonClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		if (refIndex < 0) return;

		logTextOutput.GetComponent<Text>().text = Const.a.audioLogSpeech2Text[refIndex];
		refIndex = -1;
	}

	void Start() {
		refIndex = -1;
		GetComponent<Button>().onClick.AddListener(() => { LogBackButtonClick(); });
	}
}
