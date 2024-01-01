using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MultiMediaLogTableButton : MonoBehaviour {
	public int logTableButtonIndex;

	void LogTableButtonClick() {
		Inventory.a.hardwareIsActive[2] = true;
		MFDManager.a.OpenEReaderInItemsTab();
		MFDManager.a.mouseClickHeldOverGUI = true;
		MFDManager.a.OpenLogsLevelFolder(logTableButtonIndex);
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { LogTableButtonClick(); });
	}
}
