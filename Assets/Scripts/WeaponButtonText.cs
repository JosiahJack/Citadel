using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class WeaponButtonText : MonoBehaviour {
	Text text;
	public int slotnum = 0;
	
	void Start () {
		text = GetComponent<Text>();
	}

	void Update () {
		text.text = WeaponInventory.WepInventoryInstance.weaponInventoryText[slotnum];
		if (slotnum == WeaponCurrent.WepInstance.weaponCurrent) {
			text.color = Const.a.ssYellowText; // Yellow
		} else {
			text.color = Const.a.ssGreenText; // Green
		}
	}
}