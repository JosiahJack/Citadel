using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundTriggered : MonoBehaviour {
	public int SFXClip = -1;
	public bool loopingAmbient = false;
	public bool playEverywhere = false;
	public bool playSoundOnParticleEmit = false;
	
	private AudioSource SFX;
	[HideInInspector] public bool currentlyPlaying = false;
	[HideInInspector] public int numparticles = 0;
	[HideInInspector] public int burstemittcnt1 = 15;
	[HideInInspector] public int burstemittcnt2 = 30;
	private bool justPaused;
	private ParticleSystem psys;


    void Start() {
		if (SFX == null) SFX = GetComponent<AudioSource>();
		SFX.playOnAwake = false;
		SFX.loop = false;
		SFX.clip = Const.a.sounds[SFXClip];
		if (playEverywhere) {
			SFX.spatialBlend = 0.0f;
		} else {
			SFX.spatialBlend = 1.0f;
		}
		
		if (loopingAmbient) {
			if (SFX != null) SFX.loop = true;
			currentlyPlaying = true;
			if (SFX != null) SFX.Play();
		}

		if (playSoundOnParticleEmit) {
			Debug.Log("PlaySoundOnParticleEmit true on " + gameObject.name);
			psys = GetComponent<ParticleSystem>();
			if (psys == null) Debug.Log("ERROR: missing ParticleSystem for PlaySoundTriggered");
			loopingAmbient = false; //only play when triggered by the psys emission
			numparticles = 0;
		}
    }

	// For ambient noises
	void OnEnable() {
		if (SFX == null) SFX = GetComponent<AudioSource>();
		if (loopingAmbient) {
			if (SFX != null) SFX.loop = true;
			if (SFX != null) SFX.clip = Const.a.sounds[SFXClip];
			if (SFX != null) SFX.Play();
		}
	}

	void Update() {
		if (currentlyPlaying) {
			if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) {
				if (SFX != null) SFX.Pause();
				justPaused = true;
			} else {
				if (justPaused) {
					if (SFX != null) SFX.UnPause();
					justPaused = false;
				}
			}
		}

		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (playSoundOnParticleEmit){
				int count = psys.particleCount;
				if (count > numparticles && (count == burstemittcnt1 || count == burstemittcnt2)) {
					Utils.PlayOneShotSavable(SFX,Const.a.sounds[SFXClip]);
				}
				numparticles = count;
			}
		}
	}

    public void PlaySoundEffect() {
		if (SFX != null) SFX.loop = false;
		Utils.PlayOneShotSavable(SFX,Const.a.sounds[SFXClip]);
	}
	
	public void StopSoundEffect() {
		if (SFX != null) SFX.Stop();
		currentlyPlaying = false;
	}
}
