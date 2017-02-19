using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour {
	public int[] wepAmmo;
	public static WeaponAmmo a;

	void Awake () {
		int i;
		a = this;
		for (i= 0; i<16; i++) {
			a.wepAmmo[i] = 0;
		}
	}
}
