using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class GrenadeCountsText : MonoBehaviour {
	Text text;
	public int countsSlotnum = 0;
	
	void Start () {
		text = GetComponent<Text>();
	}

	void Update () {
		text.text = GrenadeInventory.GrenadeInvInstance.grenAmmo[countsSlotnum].ToString();
		if (countsSlotnum == GrenadeCurrent.GrenadeInstance.grenadeCurrent) {
			text.color = Const.a.ssYellowText; // Yellow
		} else {
			text.color = Const.a.ssGreenText; // Green
		}
	}
}