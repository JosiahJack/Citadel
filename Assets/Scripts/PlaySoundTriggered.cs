using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundTriggered : MonoBehaviour {
	public AudioClip SFXClipToPlay;
	public bool loopingAmbient = false;
	public bool playEverywhere = false;
	[HideInInspector]
    public AudioSource SFX;
	private bool justPaused;
	[HideInInspector]
	public bool currentlyPlaying = false;
	public bool playSoundOnParticleEmit = false;
	private ParticleSystem psys;
	public int numparticles;
	public int burstemittcnt1 = 15;
	public int burstemittcnt2 = 30;

    void Start() {
		if (SFX == null) SFX = GetComponent<AudioSource>();
		SFX.playOnAwake = false;
		SFX.loop = false;
		SFX.clip = SFXClipToPlay;
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
			if (SFX != null) SFX.clip = SFXClipToPlay;
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
					if (SFX != null) SFX.PlayOneShot(SFXClipToPlay);
				}
				numparticles = count;
			}
		}
	}

    public void PlaySoundEffect() {
		if (SFX != null) SFX.loop = false;
		if (SFX != null) SFX.PlayOneShot(SFXClipToPlay);
	}
}
