using UnityEngine;
using System.Collections;

public class PlayerEnergy : MonoBehaviour {
	public float energy = 54f; //max is 255
	public float resetAfterDeathTime = 0.5f;
	public float timer;
    private AudioSource SFX;
    public AudioClip SFXBatteryUse;
    public AudioClip SFXChargeStationUse;

    public void Awake() {
        SFX = GetComponent<AudioSource>();
    }

    public void TakeEnergy ( float take  ){
		energy -= take;
		if (energy <= 0f)
			energy = 0f;
		//print("Player Energy: " + energy.ToString());
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