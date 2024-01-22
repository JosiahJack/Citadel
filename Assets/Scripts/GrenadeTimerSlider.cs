using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeTimerSlider : MonoBehaviour {
    Slider slideS;
	public Slider actualSlider;
	public Text valueText;

	void Awake () {
        slideS = GetComponent<Slider>();
	}

	void Update () {
		if (Inventory.a.grenadeCurrent != -1) {
			if (Inventory.a.grenadeCurrent == 5) {
				valueText.text = Inventory.a.nitroTimeSetting.ToString("0.0");
			} else if (Inventory.a.grenadeCurrent == 6) {
				valueText.text = Inventory.a.earthShakerTimeSetting.ToString("0.0");
			}
		}

	}

    public void SetValue() {
		if (Inventory.a == null) return;
		if (Inventory.a.grenadeCurrent != 5
			&& Inventory.a.grenadeCurrent != 6) {

			return;
		}

		MFDManager.a.mouseClickHeldOverGUI = true;
		float val = actualSlider.value;
		if (val >= 60f) val = 60f;
		if (Inventory.a.grenadeCurrent == 5) {
			if (val < 2f) val = 2f;
			Inventory.a.nitroTimeSetting = val;
		} else if (Inventory.a.grenadeCurrent == 6) {
			if (val < 4f) val = 4f;
			Inventory.a.earthShakerTimeSetting = val;
		}

		slideS.value = val;
		valueText.text = slideS.value.ToString("0.0");
		Slider slidLH =
	 	  MFDManager.a.itemTabLH.grenadeTimerSliderSlider.GetComponent<Slider>();

		Slider slidRH =
		  MFDManager.a.itemTabRH.grenadeTimerSliderSlider.GetComponent<Slider>();

		if (Inventory.a.grenadeCurrent == 5) {
			if (slidLH != actualSlider) slidLH.value = Inventory.a.nitroTimeSetting;
			if (slidRH != actualSlider) slidRH.value = Inventory.a.nitroTimeSetting;
		} else if (Inventory.a.grenadeCurrent == 6) {
			if (slidLH != actualSlider) slidLH.value = Inventory.a.earthShakerTimeSetting;
			if (slidRH != actualSlider) slidRH.value = Inventory.a.earthShakerTimeSetting;
		}
    }
}
