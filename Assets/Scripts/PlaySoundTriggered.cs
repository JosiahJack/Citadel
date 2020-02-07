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

    void Start() {
		SFX = GetComponent<AudioSource>();
		SFX.loop = false;
		SFX.clip = SFXClipToPlay;
		if (playEverywhere) {
			SFX.spatialBlend = 0.0f;
		} else {
			SFX.spatialBlend = 1.0f;
		}
		if (loopingAmbient) {
			SFX.loop = true;
			currentlyPlaying = true;
			SFX.Play();
		}
    }

	// For ambient noises
	void OnEnable() {
		if (loopingAmbient) {
			SFX.loop = true;
			SFX.clip = SFXClipToPlay;
			SFX.Play();
		}
	}

	void Update() {
		if (currentlyPlaying) {
			if (PauseScript.a != null && PauseScript.a.Paused()) {
				SFX.Pause();
				justPaused = true;
			} else {
				if (justPaused) {
					SFX.UnPause();
					justPaused = false;
				}
			}
		}
	}

    public void PlaySoundEffect() {
		SFX.loop = false;
		SFX.PlayOneShot(SFXClipToPlay);
	}
}
