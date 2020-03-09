using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergySlider : MonoBehaviour {
    public WeaponCurrent currentWeapon;
	public WeaponFire wf;

    Slider slideS;
    private int index;
    private float ener_min;
    private float ener_max;
    private float val;

	void Awake () {
        slideS = GetComponent<Slider>();
        index = -1;
        ener_min = 2f;
        ener_max = 130f;
        val = 0;
	}

	void OnEnable () {
		index = WeaponFire.Get16WeaponIndexFromConstIndex(currentWeapon.weaponIndex);
		slideS.value = currentWeapon.weaponEnergySetting[index];
	}

    public void SetValue() {
        index = WeaponFire.Get16WeaponIndexFromConstIndex(currentWeapon.weaponIndex);
        ener_min = Const.a.energyDrainLowForWeapon[index];
        ener_max = Const.a.energyDrainHiForWeapon[index];
        val = slideS.value / 100f;
        val = (val*(ener_max - ener_min)) + ener_min;
        currentWeapon.weaponEnergySetting[index] = val;
		wf.energySliderClickedTime = Time.time + 0.1f;
    }
}
