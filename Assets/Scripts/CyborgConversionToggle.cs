using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyborgConversionToggle : MonoBehaviour {
	public AudioClip SFXEnabled;
	public AudioClip SFXDisabled;
	private AudioSource SFX;

    void Awake() {
		SFX = GetComponent<AudioSource>();
	}

	public void PlayVoxMessage() {
		if (LevelManager.a.IsCurrentLevelCyborgConversionEnabled()) {
			if (SFX != null && SFX.enabled == true && SFXEnabled != null) SFX.PlayOneShot(SFXEnabled);
		} else {
			if (SFX != null && SFX.enabled == true && SFXDisabled != null) SFX.PlayOneShot(SFXDisabled);
		}
	}
}
