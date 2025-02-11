using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MultiMediaLogButton : MonoBehaviour {
	public int logButtonIndex;
	public GameObject textReader;
	[HideInInspector]
	public LogDataTabContainerManager logContainerManagerLH;
	[HideInInspector]
	public LogDataTabContainerManager logContainerManagerRH;	
	public int logReferenceIndex;
	public GameObject logContentsTable;
	public bool isEmailButton = false;
	public Color readColor;
	public Color unreadColor;

	void LogButtonClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		textReader.SetActive(true);
		Inventory.a.PlayLog(logReferenceIndex);
		MFDManager.a.OpenLogTextReader();
		MFDManager.a.logDataTabInfoLH.SendLogData(logReferenceIndex, false);
		MFDManager.a.logDataTabInfoRH.SendLogData(logReferenceIndex, true);
		textReader.GetComponent<LogTextReaderManager>().SendTextToReader(logReferenceIndex);
		if (logContentsTable != null) logContentsTable.SetActive(false);
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { LogButtonClick(); });
	}

	void OnEnable() {
		Text tt = GetComponentInChildren<Text>(true);
		if (isEmailButton) tt.text = Const.a.audiologNames[logReferenceIndex];
		if (Inventory.a.readLog[logReferenceIndex]) {
			tt.color = readColor;
		} else {
			tt.color = unreadColor;
		}
	}
}
