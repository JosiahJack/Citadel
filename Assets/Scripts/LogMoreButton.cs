using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class LogMoreButton : MonoBehaviour {
	public GameObject logTextOutput;
	public GameObject multiMediaTab;
	public MFDManager mfdManager;
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
			if (mfdManager.leftTC != null) mfdManager.leftTC.ReturnToLastTab();
			if (mfdManager.rightTC != null) mfdManager.rightTC.ReturnToLastTab();
			mfdManager.ClearDataTab();
			GetComponent<UIButtonMask>().PtrExit(); // force mouse cursor out of UI
		}
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { LogMoreButtonClick(); });
	}
}
