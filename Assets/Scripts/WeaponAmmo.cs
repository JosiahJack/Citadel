using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour {
	public int[] wepAmmo; // save
	public int[] wepAmmoSecondary; // save
	public float[] currentEnergyWeaponHeat; // save
	public bool[] wepLoadedWithAlternate; // save
	public static WeaponAmmo a;

	void Awake () {
		a = this;
		for (int i=0;i<16;i++) {
			a.wepAmmo[i] = 0;
			a.wepAmmoSecondary[i] = 0;
		}

		for (int j=0;j<7;j++) {
			a.wepLoadedWithAlternate[j] = false;
			a.currentEnergyWeaponHeat[j] = 0f;
		}
	}
}
