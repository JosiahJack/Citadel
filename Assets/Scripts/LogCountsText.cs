using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class LogCountsText : MonoBehaviour {
	Text text;
	int tempint;
	public int countsSlotnum = 0;
	
	void Start () {
		text = GetComponent<Text>();
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			tempint = Inventory.a.numLogsFromLevel[countsSlotnum];
			if (tempint > 0)text.text = Inventory.a.numLogsFromLevel[countsSlotnum].ToString();
			else 			text.text = " ";  // Blank out the text
		}
	}
}