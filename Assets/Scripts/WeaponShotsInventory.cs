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
		if (shotSlotnum == WeaponCurrent.Instance.weaponCurrent) {
			text.color = new Color(0.8902f, 0.8745f, 0f); // Yellow
		} else {
			text.color = new Color(0.3725f, 0.6549f, 0.1686f); // Green
		}
	}
}