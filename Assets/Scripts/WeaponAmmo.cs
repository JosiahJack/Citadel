using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour {
	public int[] wepAmmo;
	public int[] wepAmmoSecondary;
	public enum energyWeaponStates {Ready,Overheated};
	public energyWeaponStates[] currentEnergyWeaponState;
	public static WeaponAmmo a;

	void Awake () {
		int i;
		a = this;
		for (i= 0; i<16; i++) {
			a.wepAmmo[i] = 0;
			a.wepAmmoSecondary[i] = 0;
			a.currentEnergyWeaponState[i] = energyWeaponStates.Ready;
		}
	}
}
