using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class PatchCountsText : MonoBehaviour {
	public Text text;
	public int countsSlotnum = 0;
	private int lastCount = 0;
	
	void Start () {
		text = GetComponent<Text>();
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (lastCount != PatchInventory.PatchInvInstance.patchCounts[countsSlotnum]) {
				text.text = PatchInventory.PatchInvInstance.patchCounts[countsSlotnum].ToString();
				lastCount = PatchInventory.PatchInvInstance.patchCounts[countsSlotnum];
			}

			if (countsSlotnum == PatchCurrent.PatchInstance.patchCurrent) {
				text.color = Const.a.ssYellowText; // Yellow
			} else {
				text.color = Const.a.ssGreenText; // Green
			}
		}
	}
}