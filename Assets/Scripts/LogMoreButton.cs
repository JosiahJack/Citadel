using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class LogMoreButton : MonoBehaviour {
	public GameObject logTextOutput;
	public GameObject multiMediaTab;
	private string remainder = System.String.Empty;

	void LogMoreButtonClick() {
		remainder = logTextOutput.GetComponent<Text>().text;
		if (remainder.Length>568) {
			// MORE BUTTON
			remainder = remainder.Remove(0,568);
			logTextOutput.GetComponent<Text>().text = remainder;
		} else {
			// CLOSE BUTTON
			multiMediaTab.GetComponent<MultiMediaTabManager>().ResetTabs();
			MFDManager.a.leftTC.ReturnToLastTab();
			MFDManager.a.rightTC.ReturnToLastTab();
			MFDManager.a.ClearDataTab();
			MFDManager.a.ctb.TabButtonClickSilent(0,true);
			GetComponent<UIButtonMask>().PtrExit(); // force mouse cursor out of UI
		}
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { LogMoreButtonClick(); });
	}
}
