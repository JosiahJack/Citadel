using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class GrenadeCountsText : MonoBehaviour {
	Text text;
	public int countsSlotnum = 0;
	private int lastCount = 0;

	void Start () {
		text = GetComponent<Text>();
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (lastCount != GrenadeInventory.GrenadeInvInstance.grenAmmo[countsSlotnum]) {
				text.text = GrenadeInventory.GrenadeInvInstance.grenAmmo[countsSlotnum].ToString();
				lastCount = GrenadeInventory.GrenadeInvInstance.grenAmmo[countsSlotnum];
			}

			if (countsSlotnum == GrenadeCurrent.GrenadeInstance.grenadeCurrent) {
				text.color = Const.a.ssYellowText; // Yellow
			} else {
				text.color = Const.a.ssGreenText; // Green
			}
		}
	}
}