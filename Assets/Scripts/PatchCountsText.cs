using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class PatchCountsText : MonoBehaviour {
	public Text text;
	public int countsSlotnum = 0;
	
	void Start () {
		text = GetComponent<Text>();
	}

	void Update () {
		text.text = PatchInventory.PatchInvInstance.patchCounts[countsSlotnum].ToString();
		if (countsSlotnum == PatchCurrent.PatchInstance.patchCurrent) {
			text.color = Const.a.ssYellowText; // Yellow
		} else {
			text.color = Const.a.ssGreenText; // Green
		}
	}
}