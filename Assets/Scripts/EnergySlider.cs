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
		if (WeaponCurrent.WepInstance.weaponCurrent != -1)
			slideS.value = WeaponCurrent.WepInstance.weaponEnergySetting[WeaponCurrent.WepInstance.weaponCurrent];
		else
			slideS.value = 0;
	}

    public void SetValue(float val) {
		slideS.value = val*100f;
        WeaponCurrent.WepInstance.weaponEnergySetting[WeaponCurrent.WepInstance.weaponCurrent] = slideS.value;
    }
}
