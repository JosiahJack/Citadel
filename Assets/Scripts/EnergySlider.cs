using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergySlider : MonoBehaviour {
    Slider slideS;

	void Awake () {
        slideS = GetComponent<Slider>();
	}

	void Update () {
		if (WeaponCurrent.a.weaponCurrent != -1) {
			slideS.value =
			  WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent];
		} else {
			slideS.value = 0;
		}
	}

    public void SetValue(float val) {
		if (WeaponCurrent.a.weaponCurrent < 0
			|| WeaponCurrent.a.weaponCurrent > 6) {
			return;
		}

		MFDManager.a.mouseClickHeldOverGUI = true;
		if (val < 1.0f) val = val * 100f;
		if (val < 0) val = 0f;
		if (val >= 98f) val = 100f;
		slideS.value = val;
		Debug.Log("Set energy slider value to " + slideS.value.ToString()
				  + ", from " + val.ToString());
        WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent] =
			slideS.value;
    }
}
