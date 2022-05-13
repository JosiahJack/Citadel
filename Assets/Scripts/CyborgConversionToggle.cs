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
		if (SFX != null) SFX.Stop();
		int lindex = LevelManager.a.currentLevel != -1 ? LevelManager.a.currentLevel : 0;
		if (LevelManager.a.ressurectionActive[lindex]) {
			if (SFX != null && SFX.enabled == true && SFXEnabled != null) SFX.PlayOneShot(SFXEnabled);
			Const.sprint(Const.a.stringTable[591]); // "Cyborg conversion cancelled.  Healing normal."
		} else {
			if (SFX != null && SFX.enabled == true && SFXDisabled != null) SFX.PlayOneShot(SFXDisabled);
			Const.sprint(Const.a.stringTable[592]); // "Cyborg conversion reactivated."
		}
	}
}
