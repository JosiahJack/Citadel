using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponIconManager : MonoBehaviour {
	public Sprite[] wepIcons;
	private Image wepIcon;

	void Awake() {
		wepIcon = GetComponent<Image>();
	}

	public void SetWepIcon (int index) {
		if (index >= 0)	GetComponent<Image>().overrideSprite = Const.a.useableItemsIcons[index];
	}

	void Update() {
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(WeaponCurrent.WepInstance.weaponIndex);
		if (wep16index >=0 && wep16index < 16) {
			wepIcon.overrideSprite = wepIcons[wep16index];
		}

		if (WeaponInventory.WepInventoryInstance.numweapons <= 0) {
			if (wepIcon.enabled) wepIcon.enabled = false;
		} else {
			if (!wepIcon.enabled) wepIcon.enabled = true;
		}
	}
}
