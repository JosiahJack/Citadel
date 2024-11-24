using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyborgConversionToggle : MonoBehaviour {
	private AudioSource SFX;

    void Awake() {
		SFX = GetComponent<AudioSource>();
	}

	public void PlayVoxMessage() {
		SFX.Stop();
		int lindex = LevelManager.a.currentLevel != -1 ? LevelManager.a.currentLevel : 0;
		if (LevelManager.a.ressurectionActive[lindex]) {
			Utils.PlayOneShotSavable(SFX,Const.a.sounds[183]); // "vox_cybconvcancelled"
			Const.sprint(Const.a.stringTable[591]); // "Cyborg conversion cancelled.  Healing normal."
		} else {
			Utils.PlayOneShotSavable(SFX,Const.a.sounds[184]); // "vox_cybconvenabled"
			Const.sprint(Const.a.stringTable[592]); // "Cyborg conversion reactivated."
		}
	}
}
