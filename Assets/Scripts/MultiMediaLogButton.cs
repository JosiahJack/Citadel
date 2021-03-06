using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MultiMediaLogButton : MonoBehaviour {
	public int logButtonIndex;
	public GameObject textReader;
	public GameObject multiMediaTab;
	public GameObject dataTabAudioLogContainerLH;
	public GameObject dataTabAudioLogContainerRH;
	[HideInInspector]
	public LogDataTabContainerManager logContainerManagerLH;
	[HideInInspector]
	public LogDataTabContainerManager logContainerManagerRH;	
	public int logReferenceIndex;
	public LogInventory logInventory;
	public GameObject logContentsTable;
	public bool isEmailButton = false;
	public Color readColor;
	public Color unreadColor;

	void LogButtonClick() {
		//Debug.Log("LogButtonClick()ed, logButtonIndex = " + logButtonIndex.ToString() + ", logReferenceIndex = " + logReferenceIndex.ToString());
		textReader.SetActive(true);
		logInventory.PlayLog(logReferenceIndex);
		multiMediaTab.GetComponent<MultiMediaTabManager>().OpenLogTextReader();
		dataTabAudioLogContainerLH.GetComponent<LogDataTabContainerManager>().SendLogData(logReferenceIndex, false);
		dataTabAudioLogContainerRH.GetComponent<LogDataTabContainerManager>().SendLogData(logReferenceIndex, true);
		textReader.GetComponent<LogTextReaderManager>().SendTextToReader(logReferenceIndex);
		if (logContentsTable != null) logContentsTable.SetActive(false);
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { LogButtonClick(); });
	}

	void OnEnable() {
		Text tt = GetComponentInChildren<Text>();
		if (isEmailButton) {
			tt.text = Const.a.audiologNames[logReferenceIndex];
		}

		if (LogInventory.a.readLog[logReferenceIndex]) {
			tt.color = readColor;
		} else {
			tt.color = unreadColor;
		}
	}
}
