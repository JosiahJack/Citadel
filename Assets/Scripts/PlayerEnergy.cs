using UnityEngine;
using System.Collections;

public class PlayerEnergy : MonoBehaviour {
	public float energy = 54f; //max is 255
	public float resetAfterDeathTime = 0.5f;
	public float timer;
    private AudioSource SFX;
    public AudioClip SFXBatteryUse;
    public AudioClip SFXChargeStationUse;
	public WeaponCurrent wepCur;
	public HardwareInvCurrent hwc;
	public HardwareInventory hwi;
	private float tick = 0.1f;
	private float tickFinished;

    public void Awake() {
        SFX = GetComponent<AudioSource>();
		tickFinished = Time.time + tick + Random.value; // random offset seed to prevent ticks lining up and causing frame hiccups
    }

	void Update () {
		if (tickFinished < Time.time) {
			if (hwc.hardwareIsActive[3]) {
				float take = 1f;
				switch (hwi.hardwareVersionSetting[3]) {
					case 0: take = 0.21f; break; // takes about 120s to drain full energy
					case 1: take = 0.42f; break; // takes about 60s to drain full energy
					case 2: take = 0.28f; break; // takes about 90s to drain full energy
				}
				TakeEnergy(take);
			}
			tickFinished = Time.time + tick;
		}
	}

    public void TakeEnergy ( float take  ){
		float was = energy;
		if (energy == 0) return;
		if (wepCur != null) {
			if (wepCur.redbull) return; // no energy drain!
		}

		energy -= take;
		if (energy <= 0f)
			energy = 0f;
		//print("Player Energy: " + energy.ToString());
		Debug.Log("Player Energy taken.  Was: " + was.ToString() +", Now: " + energy.ToString());
	}

	public void GiveEnergy (float give, int type) {
		energy += give;
		if (energy > 255f) {
			energy = 255f;
		}

        if (type == 0) {
            if (SFX != null) SFX.PlayOneShot(SFXBatteryUse); // battery sound
        }
        if (type == 1) {
            if (SFX != null) SFX.PlayOneShot(SFXChargeStationUse); // charging station sound
        }
    }
}