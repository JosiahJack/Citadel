using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeTimerSlider : MonoBehaviour {
    Slider slideS;
	public Text valueText;

	void Awake () {
        slideS = GetComponent<Slider>();
	}

	void Update () {
		if (Inventory.a.grenadeCurrent != -1) {
			if (Inventory.a.grenadeCurrent == 5) {
				slideS.value = Inventory.a.nitroTimeSetting;
			} else if (Inventory.a.grenadeCurrent == 6) {
				slideS.value = Inventory.a.earthShakerTimeSetting;
			} else {
				slideS.value = 0;
			}
		} else {
			slideS.value = 0;
		}

		valueText.text = slideS.value.ToString("0.0");
	}

    public void SetValue(float val) {
		if (Inventory.a.grenadeCurrent != 5
			|| Inventory.a.grenadeCurrent != 6) {

			return;
		}

		MFDManager.a.mouseClickHeldOverGUI = true;
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
    }
}
