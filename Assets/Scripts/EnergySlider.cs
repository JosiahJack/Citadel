using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergySlider : MonoBehaviour {
    Slider slideS;

	void Awake () {
        slideS = GetComponent<Slider>();
	}

	void OnEnable () {
		if (WeaponCurrent.a.weaponCurrent != -1)
			slideS.value = WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent];
		else
			slideS.value = 0;
	}

    public void SetValue(float val) {
		MFDManager.a.mouseClickHeldOverGUI = true;

		slideS.value = val*100f;
        WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent] = slideS.value;
    }
}
