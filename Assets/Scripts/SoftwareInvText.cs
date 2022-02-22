using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class SoftwareInvText : MonoBehaviour {
	Text text;
	public int slotnum = 0;
	public Text versionText;
	
	void Start () {
		text = GetComponent<Text>();
		if (text == null) UnityEngine.Debug.Log("BUG: Missing Text component for SoftwareInvText");
	}

	public void Select(bool thisOne) {
		if (thisOne) {
			if (text != null) text.color = Const.a.ssYellowText; // Yellow
			if (versionText != null) versionText.color = Const.a.ssYellowText; // Yellow
		} else {
			if (text != null) text.color = Const.a.ssGreenText; // Green
			if (versionText != null) versionText.color = Const.a.ssGreenText; // Green
		}
	}
}