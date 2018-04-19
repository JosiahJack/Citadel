using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class WeaponShotsInventory : MonoBehaviour {
	Text text;
	public int shotSlotnum = 0;
	
	void Start () {
		text = GetComponent<Text>();
	}

	void Update () {
		text.text = WeaponShotsText.weaponShotsInventoryText[shotSlotnum];
		if (shotSlotnum == WeaponCurrent.WepInstance.weaponCurrent) {
			text.color = Const.a.ssYellowText; // Yellow
		} else {
			text.color = Const.a.ssGreenText; // Green
		}
	}
}