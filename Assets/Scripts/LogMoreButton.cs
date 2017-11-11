using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class LogMoreButton : MonoBehaviour {
	public GameObject logTextOutput;
	public GameObject multiMediaTab;
	public GameObject dataTabAudLogContainer;
	private string remainder = System.String.Empty;

	void LogMoreButtonClick() {
		remainder = logTextOutput.GetComponent<Text>().text;
		if (remainder.Length>568) {
			remainder = remainder.Remove(0,568);
			logTextOutput.GetComponent<Text>().text = remainder;
		} else {
			dataTabAudLogContainer.SetActive(false);
			multiMediaTab.GetComponent<MultiMediaTabManager>().ResetTabs();
		}
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { LogMoreButtonClick(); });
	}
}
