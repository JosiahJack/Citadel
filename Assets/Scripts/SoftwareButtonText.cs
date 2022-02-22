using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class SoftwareButtonText : MonoBehaviour {
	Text text;
	public int slotnum = 0;
	
	void Start () {
		text = GetComponent<Text>();
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (slotnum == Inventory.a.currentCyberItem) {
				text.color = Const.a.ssYellowText; // Yellow
			} else {
				text.color = Const.a.ssGreenText; // Green
			}
		}
	}
}