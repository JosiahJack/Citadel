using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogContentsButtonsManager : MonoBehaviour {
	public GameObject[] LogButtons;
	public Text[] LogButtonsText;
	public int currentLevelFolder;
	public string[] logNames;
	public int[] retrievedIndices;
	/*[DTValidator.Optional] */public MultiMediaLogButton[] logRefButtons; //DT optional because it's empty until initialized below during Start

	void Start() {
		InitializeLogsFromLevelIntoFolder();
	}

	public void InitializeLogsFromLevelIntoFolder() {
		retrievedIndices = new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
		logNames = GetLogNamesFromLevel(currentLevelFolder);
		for (int i=0; i<15; i++) {
			logRefButtons[i] = LogButtons[i].GetComponent<MultiMediaLogButton>();
		}
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			for (int i=0; i<15; i++) {
				LogButtonsText[i].text = logNames[i];
				logRefButtons[i].logReferenceIndex = retrievedIndices[i];
				if (Inventory.a.hasLog[retrievedIndices[i]]) {
					LogButtons[i].SetActive(true);
				} else {
					LogButtons[i].SetActive(false);
				}
			}
		}
	}

	string[] GetLogNamesFromLevel (int index) {
		string[] retval = {"","","","","","","","","","","","","","",""};
		int indexingVal = 0;
		for (int i=0;i<134;i++) {
			if ((Const.a.audioLogLevelFound[i] == index)) {
				retval[indexingVal] = Const.a.audiologNames[i];
				retrievedIndices[indexingVal] = i;
				indexingVal++;
				if (indexingVal > 14)
					break; // No need to iterate through remaining list if we already have 14 values
			}
		}
		return retval;
	}
}
