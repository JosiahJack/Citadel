using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
	// DT optional since not all levels have all music types
	public AudioClip[] levelMusicWalking;
	public AudioClip[] levelMusicCombat;
	public AudioClip[] levelMusicMutantNear;
	public AudioClip[] levelMusicCyborgNear;
	public AudioClip[] levelMusicCyborgDroneNear;
	public AudioClip[] levelMusicRobotNear;
	public AudioClip[] levelMusicTransition;
	public AudioClip[] levelMusicRevive;
	public AudioClip[] levelMusicDeath;
	public AudioClip[] levelMusicCybertube;
	public AudioClip[] levelMusicElevator;
	public AudioClip[] levelMusicDistortion;
	public AudioSource SFXMain;
	public AudioSource SFXOverlay;
	private float clipFinished;
	private float clipLength;
	private float clipOverlayLength;
	private float clipOverlayFinished;
	//private int actionState;
	public GameObject mainMenu;
	public WeaponFire wf;
	public HealthManager hm;
	public enum MusicType{None,Walking,Combat,Overlay,Override};
	public enum TrackType{None,Walking,Combat,MutantNear,CyborgNear,CyborgDroneNear,RobotNear,Transition,Revive,Death,Cybertube,Elevator,Distortion};
	private AudioClip tempC;
	private AudioClip curC;
	private AudioClip curOverlayC;

	public static Music a;

	void Start() {
		a = this;
		a.clipFinished = Time.time;
		a.clipOverlayFinished = Time.time;
		a.tempC = null;
	}

	public void PlayTrack(int levnum, TrackType ttype, MusicType mtype) {
		tempC = GetCorrespondingLevelClip(levnum,ttype);
		switch(mtype) {
			case MusicType.Walking:
				if (tempC == curC && SFXMain.isPlaying) break; // no need, already playing

				if (SFXMain != null) SFXMain.Stop(); // stop playing normal
				if (tempC != null) {
					SFXMain.clip = tempC;
					curC = tempC;
					SFXMain.Play();
				} else {
					curC = null;
				}
				break;
			case MusicType.Combat:
				if (tempC == curC && SFXMain.isPlaying) break; // no need, already playing

				if (SFXMain != null) SFXMain.Stop(); // stop playing normal
				if (tempC != null) {
					SFXMain.clip = tempC;
					curC = tempC;
					SFXMain.Play();
				} else {
					curC = null;
				}
				break;
			case MusicType.Overlay:
				if (tempC == curOverlayC && SFXOverlay.isPlaying) break; // no need, already playing
				if (SFXOverlay != null) SFXOverlay.Stop(); // stop any overlays
				if (tempC != null) {
					SFXOverlay.clip = tempC;
					curOverlayC = tempC;
					SFXOverlay.PlayOneShot(tempC);
				} else {
					curOverlayC = null;
				}
				break;
			case MusicType.Override:
				if (tempC == curC && SFXMain.isPlaying) break; // no need, already playing

				// stop both
				if (SFXMain != null) SFXMain.Stop();
				if (SFXOverlay != null) SFXOverlay.Stop();
				if (tempC != null) {
					SFXMain.clip = tempC;
					curC = tempC;
					SFXMain.Play();
				} else {
					curC = null;
				}
				break;
		}
	}

	AudioClip GetCorrespondingLevelClip(int levnum, TrackType ttype) {
		if (levnum < 0 || levnum > 13) return null; // out of bounds

		switch(ttype) {
			case TrackType.Walking: return levelMusicWalking[levnum];
			case TrackType.Combat: return levelMusicCombat[levnum];
			case TrackType.MutantNear: return levelMusicMutantNear[levnum];
			case TrackType.CyborgNear: return levelMusicCyborgNear[levnum];
			case TrackType.CyborgDroneNear: return levelMusicCyborgDroneNear[levnum];
			case TrackType.RobotNear: return levelMusicRobotNear[levnum];
			case TrackType.Transition: return levelMusicTransition[levnum];
			case TrackType.Revive: return levelMusicRevive[levnum];
			case TrackType.Death: return levelMusicDeath[levnum];
			case TrackType.Cybertube: return levelMusicCybertube[levnum];
			case TrackType.Elevator: return levelMusicElevator[levnum];
			case TrackType.Distortion: return levelMusicDistortion[levnum];
		}

		return null; // Tracktype none
	}

    void Update() {
		// Check if main menu is active and disable playing background music
		if (mainMenu.activeSelf == true) {
			if (SFXMain != null) SFXMain.Stop();
			if (SFXOverlay != null) SFXOverlay.Stop();
			return;
		}

		//CheckActionState();
    }

	// void SetClip (AudioClip acNormal, AudioClip acSuspense, AudioClip acAction, AudioClip acDistorted, float vol) {
		// switch(actionState) {
			// case 0: if (SFXMain.clip != acNormal) SFX.clip = acNormal; break;
			// case 1: if (SFX.clip != acSuspense) SFX.clip = acSuspense; break;
			// case 2: if (SFX.clip != acAction) SFX.clip = acAction; break;
			// case 3: if (SFX.clip != acDistorted) SFX.clip = acDistorted; break;
			// default: if (SFX.clip != acNormal) SFX.clip = acNormal; break;
		// }

		// if (SFX.clip != null && SFX != null && clipFinished < Time.time) {
			// if (SFX.clip != null) {
				// clipLength = SFX.clip.length;
			// } else {
				// clipLength = 0;
			// }
			// SFX.loop = true;
			// SFX.volume = vol;
			// clipFinished = Time.time + clipLength;
			// if (SFX != null) SFX.PlayOneShot(SFX.clip);
		// } else {
			// if (SFX != null) SFX.Stop();
		// }
	// }
}
