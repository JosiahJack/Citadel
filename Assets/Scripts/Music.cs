using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
	// DT optional since not all levels have all music types
	public AudioClip levelRMusicNormal;
	public AudioClip levelRMusicSuspense;
	public AudioClip levelRMusicAction;
	public AudioClip levelRMusicDistorted;
	public AudioClip level1MusicNormal;
	public AudioClip level1MusicSuspense;
	public AudioClip level1MusicAction;
	public AudioClip level1MusicDistorted;
	public AudioClip level2MusicNormal;
	public AudioClip level2MusicSuspense;
	public AudioClip level2MusicAction;
	public AudioClip level2MusicDistorted;
	public AudioClip level3MusicNormal;
	public AudioClip level3MusicSuspense;
	public AudioClip level3MusicAction;
	public AudioClip level3MusicDistorted;
	public AudioClip level4MusicNormal;
	public AudioClip level4MusicSuspense;
	public AudioClip level4MusicAction;
	public AudioClip level4MusicDistorted;
	public AudioClip level5MusicNormal;
	public AudioClip level5MusicSuspense;
	public AudioClip level5MusicAction;
	public AudioClip level5MusicDistorted;
	public AudioClip level6MusicNormal;
	public AudioClip level6MusicSuspense;
	public AudioClip level6MusicAction;
	public AudioClip level6MusicDistorted;
	public AudioClip level7MusicNormal;
	public AudioClip level7MusicSuspense;
	public AudioClip level7MusicAction;
	public AudioClip level7MusicDistorted;
	public AudioClip level8MusicNormal;
	public AudioClip level8MusicSuspense;
	public AudioClip level8MusicAction;
	public AudioClip level8MusicDistorted;
	public AudioClip level9MusicNormal;
	public AudioClip level9MusicSuspense;
	public AudioClip level9MusicAction;
	public AudioClip level9MusicDistorted;
	public AudioClip levelGMusicNormal;
	public AudioClip levelGMusicSuspense;
	public AudioClip levelGMusicAction;
	public AudioClip levelGMusicDistorted;
	public AudioClip levelBetaGMusicNormal;
	public AudioClip cyber1Music;
	public AudioClip cyber2Music;
	private AudioSource SFX;
	private float clipFinished;
	private float clipLength;
	private int actionState = 0;
	public GameObject mainMenu;
	public WeaponFire wf;
	public HealthManager hm;

	void Awake() {
		SFX = GetComponent<AudioSource>();
		clipFinished = Time.time;
	}

	void CheckActionState() {
		// 0 = normal, 1 = suspense, 2 = action, 3 = distorted
		// Normal = usual low-key gentle music
		// Suspense = critter nearby, play some faster paced bits, play creepy overtones, and other bonus bits n bobs
		// Action = fast paced full out, we are fighting or being attacked!
		// Distorted = for those damaged and rearranged areas of the station UPDATE: Add distortion triggers for these areas and check for justDistorted
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

		// Is an enemy close by? I loop through all enemies and check there distance
		// for(int i=0;i<????.Length;i++) {if (Vector3.Distance(????[i].position,playerCapsuleTransform.position) < 10.24f) actionState = 1; // critter nearby, freak out!}
	}

    void Update() {
		// Check if main menu is active and disable playing background music
		if (mainMenu.activeSelf == true) {
			if (SFX != null) SFX.Stop();
			return;
		}

		CheckActionState();

		/*
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
		*/
    }

	void SetClip (AudioClip acNormal, AudioClip acSuspense, AudioClip acAction, AudioClip acDistorted, float vol) {
		switch(actionState) {
			case 0: if (SFX.clip != acNormal) SFX.clip = acNormal; break;
			case 1: if (SFX.clip != acSuspense) SFX.clip = acSuspense; break;
			case 2: if (SFX.clip != acAction) SFX.clip = acAction; break;
			case 3: if (SFX.clip != acDistorted) SFX.clip = acDistorted; break;
			default: if (SFX.clip != acNormal) SFX.clip = acNormal; break;
		}

		if (SFX.clip != null && SFX != null && clipFinished < Time.time) {
			if (SFX.clip != null) {
				clipLength = SFX.clip.length;
			} else {
				clipLength = 0;
			}
			SFX.loop = true;
			SFX.volume = vol;
			clipFinished = Time.time + clipLength;
			if (SFX != null) SFX.PlayOneShot(SFX.clip);
		} else {
			if (SFX != null) SFX.Stop();
		}
	}
}
