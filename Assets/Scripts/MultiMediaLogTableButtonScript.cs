using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MultiMediaLogTableButtonScript : MonoBehaviour {
	public int logTableButtonIndex;
	public GameObject multiMediaTab;

	void LogTableButtonClick() {
		multiMediaTab.GetComponent<MultiMediaTabManager>().OpenLogsLevelFolder(logTableButtonIndex);
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { LogTableButtonClick(); });
	}
}
