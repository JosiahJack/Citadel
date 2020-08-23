using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergySlider : MonoBehaviour {
    public WeaponCurrent currentWeapon;
	public WeaponFire wf;
	public MouseCursor mc;
    Slider slideS;

	void Awake () {
        slideS = GetComponent<Slider>();
	}

	void OnEnable () {
		if (WeaponCurrent.WepInstance.weaponCurrent != -1)
			slideS.value = currentWeapon.weaponEnergySetting[WeaponCurrent.WepInstance.weaponCurrent];
		else
			slideS.value = 0;
	}

    public void SetValue(float val) {
		slideS.value = val*100f;
        currentWeapon.weaponEnergySetting[WeaponCurrent.WepInstance.weaponCurrent] = slideS.value;
    }
}
