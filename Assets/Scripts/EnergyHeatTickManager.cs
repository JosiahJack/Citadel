using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyHeatTickManager : MonoBehaviour {
    public Image[] ticks;
    private float tempFloat;

	void Awake () {
        // Note that we only ever affect the last 9 ticks.  Tick 1 of 10 stays on so we ignore it.
		for (int i=0;i<9;i++) {
            ticks[i].enabled = false;
        }
	}

    // Note that WeaponFire calls this every tick seconds
	public void HeatBleed (float currentHeat) {
        tempFloat = 10f;
		for (int i=0;i<9;i++) {
            if (currentHeat >= tempFloat)
                ticks[i].enabled = true;
            else
                ticks[i].enabled = false;
            tempFloat += 10f;
        }
	}
}
