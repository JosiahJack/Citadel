using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponMagazineCounter : MonoBehaviour {
	public Sprite[] indicatorSprites;
	public Image onesIndicator;
	public Image tensIndicator;
	public Image hunsIndicator;
	private int tempi;
	private int checkcount;
	private int[] tempis = new int[] {0,0,0};

	public void UpdateDigits (int newamount) {
		tempi = newamount;
		tempis[0] = 10;
		tempis[1] = 10;
		tempis[2] = 10;

		//if (tempi > 99) {
			tempis[2] = tempi % 10; // ones
		if (newamount > 9) {
			tempi /= 10;
			tempis[1] = tempi % 10; // tens
		}
		if (newamount> 99) {
			tempi /= 10;
			tempis[0] = tempi % 10; // huns
		}

		onesIndicator.overrideSprite = indicatorSprites[tempis[2]];
		tensIndicator.overrideSprite = indicatorSprites[tempis[1]];
		hunsIndicator.overrideSprite = indicatorSprites[tempis[0]];
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			int index = WeaponCurrent.WepInstance.weaponCurrent; // 0 to 6, Changed from this: WeaponFire.Get16WeaponIndexFromConstIndex(WeaponCurrent.WepInstance.weaponIndex); 0 to 15
			if (index == -1) return;
			if (WeaponAmmo.a.wepLoadedWithAlternate[WeaponCurrent.WepInstance.weaponCurrent]) {
				UpdateDigits(WeaponCurrent.WepInstance.currentMagazineAmount2[index]);
			} else {
				UpdateDigits(WeaponCurrent.WepInstance.currentMagazineAmount[index]);
			}
		}
	}
}
