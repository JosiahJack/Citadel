using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
	// DT optional since not all levels have all music types
	[DTValidator.Optional] public AudioClip levelRMusicNormal;
	[DTValidator.Optional] public AudioClip levelRMusicSuspense;
	[DTValidator.Optional] public AudioClip levelRMusicAction;
	[DTValidator.Optional] public AudioClip levelRMusicDistorted;
	[DTValidator.Optional] public AudioClip level1MusicNormal;
	[DTValidator.Optional] public AudioClip level1MusicSuspense;
	[DTValidator.Optional] public AudioClip level1MusicAction;
	[DTValidator.Optional] public AudioClip level1MusicDistorted;
	[DTValidator.Optional] public AudioClip level2MusicNormal;
	[DTValidator.Optional] public AudioClip level2MusicSuspense;
	[DTValidator.Optional] public AudioClip level2MusicAction;
	[DTValidator.Optional] public AudioClip level2MusicDistorted;
	[DTValidator.Optional] public AudioClip level3MusicNormal;
	[DTValidator.Optional] public AudioClip level3MusicSuspense;
	[DTValidator.Optional] public AudioClip level3MusicAction;
	[DTValidator.Optional] public AudioClip level3MusicDistorted;
	[DTValidator.Optional] public AudioClip level4MusicNormal;
	[DTValidator.Optional] public AudioClip level4MusicSuspense;
	[DTValidator.Optional] public AudioClip level4MusicAction;
	[DTValidator.Optional] public AudioClip level4MusicDistorted;
	[DTValidator.Optional] public AudioClip level5MusicNormal;
	[DTValidator.Optional] public AudioClip level5MusicSuspense;
	[DTValidator.Optional] public AudioClip level5MusicAction;
	[DTValidator.Optional] public AudioClip level5MusicDistorted;
	[DTValidator.Optional] public AudioClip level6MusicNormal;
	[DTValidator.Optional] public AudioClip level6MusicSuspense;
	[DTValidator.Optional] public AudioClip level6MusicAction;
	[DTValidator.Optional] public AudioClip level6MusicDistorted;
	[DTValidator.Optional] public AudioClip level7MusicNormal;
	[DTValidator.Optional] public AudioClip level7MusicSuspense;
	[DTValidator.Optional] public AudioClip level7MusicAction;
	[DTValidator.Optional] public AudioClip level7MusicDistorted;
	[DTValidator.Optional] public AudioClip level8MusicNormal;
	[DTValidator.Optional] public AudioClip level8MusicSuspense;
	[DTValidator.Optional] public AudioClip level8MusicAction;
	[DTValidator.Optional] public AudioClip level8MusicDistorted;
	[DTValidator.Optional] public AudioClip level9MusicNormal;
	[DTValidator.Optional] public AudioClip level9MusicSuspense;
	[DTValidator.Optional] public AudioClip level9MusicAction;
	[DTValidator.Optional] public AudioClip level9MusicDistorted;
	[DTValidator.Optional] public AudioClip levelGMusicNormal;
	[DTValidator.Optional] public AudioClip levelGMusicSuspense;
	[DTValidator.Optional] public AudioClip levelGMusicAction;
	[DTValidator.Optional] public AudioClip levelGMusicDistorted;
	[DTValidator.Optional] public AudioClip levelBetaGMusicNormal;
	[DTValidator.Optional] public AudioClip cyber1Music;
	[DTValidator.Optional] public AudioClip cyber2Music;
	private AudioSource SFX;
	private int actionState = 0;
	public GameObject mainMenu;
	public WeaponFire wf;
	public HealthManager hm;

	void Awake() {
		SFX = GetComponent<AudioSource>();
	}

	void CheckActionState() {
		// 0 = normal, 1 = suspense, 2 = action, 3 = distorted
		// Normal = usual low-key gentle music
		// Suspense = critter nearby, play some faster paced bits, play creepy overtones, and other bonus bits n bobs
		// Action = fast paced full out, we are fighting or being attacked!
		// Distorted = for those damaged and rearranged areas of the station TODO: Add distortion triggers for these areas and check for justDistorted
		actionState = 0; // normal

		// Did we fire a weapon sometime in the past 30s or less
		if ((Time.time - wf.justFired) < 30f) {
			actionState = 2; // action - we are fighting!
			return;
		}

		// Did we get hurt sometime in the past 30s or less
		if ((Time.time - hm.justHurtByEnemy) < 30f) {
			actionState = 2; // action - we are being fought!
			return;
		}

		// Is an enemy close by? TODO: should I loop through all enemies and check there distance or is this kind of a cheat?
		// for(int i=0;i<????.Length;i++) {if (Vector3.Distance(????[i].position,playerCapsuleTransform.position) < 10.24f) actionState = 1; // critter nearby, freak out!}
	}

// BUGS!!  Apparently it keeps start start start start start start start starting the same clip over and over
/*
    void Update() {
		// Check if main menu is active and disable playing background music
		if (mainMenu.activeSelf == true) {
			SFX.Stop();
			return;
		}

		CheckActionState();

        switch(LevelManager.a.currentLevel) {
			case 0: SetClip(levelRMusicNormal,levelRMusicSuspense,levelRMusicAction,levelRMusicDistorted,1f);
					break;
			case 1: SetClip(level1MusicNormal,level1MusicSuspense,level1MusicAction,level1MusicDistorted,1f);
					break;
			case 2: SetClip(level2MusicNormal,level2MusicSuspense,level2MusicAction,level2MusicDistorted,1f);
					break;
			case 3: SetClip(level3MusicNormal,level3MusicSuspense,level3MusicAction,level3MusicDistorted,1f);
					break;
			case 4: SetClip(level4MusicNormal,level4MusicSuspense,level4MusicAction,level4MusicDistorted,1f);
					break;
			case 5: SetClip(level5MusicNormal,level5MusicSuspense,level5MusicAction,level5MusicDistorted,1f);
					break;
			case 6: SetClip(level6MusicNormal,level6MusicSuspense,level6MusicAction,level6MusicDistorted,1f);
					break;
			case 7: SetClip(level7MusicNormal,level7MusicSuspense,level7MusicAction,level7MusicDistorted,1f);
					break;
			case 8: SetClip(level8MusicNormal,level8MusicSuspense,level8MusicAction,level8MusicDistorted,1f);
					break;
			case 9: SetClip(level9MusicNormal,level9MusicSuspense,level9MusicAction,level9MusicDistorted,1f);
					break;
			case 10: SetClip(levelGMusicNormal,levelRMusicSuspense,levelRMusicAction,levelRMusicDistorted,1f);
					break;
			case 11: SetClip(levelBetaGMusicNormal,levelBetaGMusicNormal,levelBetaGMusicNormal,levelBetaGMusicNormal,1f);
					break;
			case 12: SetClip(levelGMusicNormal,levelGMusicSuspense,levelGMusicAction,levelGMusicDistorted,1f);
					break;
			case 13: SetClip(cyber1Music,cyber1Music,cyber2Music,cyber2Music,1f);
					break;
		}
    }
*/

	void SetClip (AudioClip acNormal, AudioClip acSuspense, AudioClip acAction, AudioClip acDistorted, float vol) {
		switch(actionState) {
			case 0: SFX.clip = acNormal; break;
			case 1: SFX.clip = acSuspense; break;
			case 2: SFX.clip = acAction; break;
			case 3: SFX.clip = acDistorted; break;
			default: SFX.clip = acNormal; break;
		}
		SFX.loop = true;
		SFX.volume = vol;
		if (SFX.clip != null)
			SFX.Play();
		else
			SFX.Stop();
	}
}
