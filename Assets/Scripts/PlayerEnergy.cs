using UnityEngine;
using System.Collections;

public class PlayerEnergy : MonoBehaviour {
	public float energy = 54f; //max is 255
	public float resetAfterDeathTime = 0.5f;
	public float timer;
    private AudioSource SFX;
    public AudioClip SFXBatteryUse;
    public AudioClip SFXChargeStationUse;
    public AudioClip SFXPowerExhausted;
	public WeaponCurrent wepCur;
	public HardwareInvCurrent hwc;
	public HardwareInventory hwi;
	private float tick = 0.1f;
	private float tickFinished;
	private float tempF;
	[HideInInspector]
	public float maxenergy = 255f;

    public void Awake() {
        SFX = GetComponent<AudioSource>();
		tempF = 0;
		tickFinished = Time.time + tick + Random.value; // random offset seed to prevent ticks lining up and causing frame hiccups
    }

	void Update () {
		tempF = 1f;
		bool activeEnergyDrainers = false;
		if (tickFinished < Time.time) {
			// 0 System Analyzer doesn't take energy

			// 1 = Navigation Unit doesn't take energy

			// 2 = Datareader doesn't take energy

			// 3 Drain sensaround
			if (hwc.hardwareIsActive[3]) {
				switch (hwi.hardwareVersionSetting[3]) {
					case 0: tempF = 0.08533f; break; // takes about 300s to drain full energy
					case 1: tempF = 0.08533f; break; // takes about 300s to drain full energy
					case 2: tempF = 0.10666f; break; // takes about 240s to drain full energy
				}
				activeEnergyDrainers = true;
				TakeEnergy(tempF);
			}

			// 4 = Target Identifier doesn't take energy

			// 5 = Energy Shield - handled by HealthManager

			// 6 = Biomonitor
			if (hwc.hardwareIsActive[6]) {
				switch (hwi.hardwareVersionSetting[6]) {
					case 0: tempF = 0.08533f; activeEnergyDrainers = true; break; // takes about 300s to drain full energy
					case 1: tempF = 0; break; // doesn't take energy
				}
				if (tempF > 0) TakeEnergy(tempF);
			}

			// 7 = Head Mounted Lantern
			if (hwc.hardwareIsActive[7]) {
				switch (hwi.hardwareVersionSetting[7]) {
					case 0: tempF = 0.1422f; break; // takes about 180s to drain full energy
					case 1: tempF = 0.21f; break; // takes about 120s to drain full energy
					case 2: tempF = 0.28f; break; // takes about 90s to drain full energy
				}
				activeEnergyDrainers = true;
				TakeEnergy(tempF);
			}

			// 8 Envirosuit - handled by HealthManager for radiation checks

			// 9 = Turbo Motion Booster - done in PlayerMovement since we only use energy on boost, no drain with skates

			// 10 Drain jump jet boots - done in PlayerMovement since we only drain while jumping

			// 11 Drain nightsight
			if (hwc.hardwareIsActive [11]) {
				tempF = 0.21f; // takes about 120s to drain full energy
				activeEnergyDrainers = true;
				TakeEnergy(tempF);
			}
			tickFinished = Time.time + tick;
		}

		// Turn everything off when we are out of energy
		if (activeEnergyDrainers && energy == 0) {
			hwc.hardwareIsActive [3] = false;
			if (hwc.hardwareIsActive [3]) hwc.hardwareButtons[1].SensaroundOff(); //sensaround
			if (hwc.hardwareIsActive [6] && hwi.hardwareVersionSetting[6] == 0) hwc.hardwareButtons[0].BioOff(); // biomonitor, but only on v1, v2 doesn't use power
			if (hwc.hardwareIsActive [5]) hwc.hardwareButtons[3].ShieldOff(); // shield
			if (hwc.hardwareIsActive [7]) hwc.hardwareButtons[2].LanternOff(); // lantern
			if (hwc.hardwareIsActive [11]) hwc.hardwareButtons[4].InfraredOff(); // infrared
			activeEnergyDrainers = false;
		}
	}

    public void TakeEnergy ( float take  ){
		float was = energy;
		if (energy == 0) return;
		if (wepCur != null) {
			if (wepCur.redbull) return; // no energy drain!
		}

		energy -= take;
		if (energy <= 0f) {
			energy = 0f;
			if (SFX != null && SFXPowerExhausted != null) SFX.PlayOneShot(SFXPowerExhausted); // battery sound
			Const.sprint(Const.a.stringTable[314],wepCur.owner); //Power supply exhausted.
		}
		//print("Player Energy: " + energy.ToString());
		//Debug.Log(take.ToString("F4") +" player energy taken.  Was: " + was.ToString() +", Now: " + energy.ToString());
	}

	public void GiveEnergy (float give, int type) {
		energy += give;
		if (energy > maxenergy) {
			energy = maxenergy;
		}

        if (type == 0) {
            if (SFX != null && SFXBatteryUse != null) SFX.PlayOneShot(SFXBatteryUse); // battery sound
        }
        if (type == 1) {
            if (SFX != null && SFXChargeStationUse != null) SFX.PlayOneShot(SFXChargeStationUse); // charging station sound
        }
    }
}