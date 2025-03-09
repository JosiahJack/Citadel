using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyHeatTickManager : MonoBehaviour {
    public Image[] ticks;
    private float tempFloat;

	void Awake() {
        // Only ever affect last 9 ticks.  Tick 1 of 10 stays on so ignore it.
		for (int i=0;i<9;i++) {
            Utils.DisableImage(ticks[i]);
        }
	}

    // Note that WeaponFire calls this every tick seconds
	public void HeatBleed (float currentHeat) {
        tempFloat = 10f;
		for (int i=0;i<9;i++) {
            if (currentHeat >= tempFloat) Utils.EnableImage(ticks[i]);
            else Utils.DisableImage(ticks[i]);

            tempFloat += 10f;
        }
	}
}
