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
			Utils.PlayOneShotSavable(SFX,SFXEnabled);
			Const.sprint(Const.a.stringTable[591]); // "Cyborg conversion cancelled.  Healing normal."
		} else {
			Utils.PlayOneShotSavable(SFX,SFXDisabled);
			Const.sprint(Const.a.stringTable[592]); // "Cyborg conversion reactivated."
		}
	}
}
