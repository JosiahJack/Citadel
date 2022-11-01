using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour {
	public float tick = 0.1f;
	private float tickFinished;
	public TrackType trackType;
	public MusicType musicType;

	void Awake() {
		tickFinished = PauseScript.a.relativeTime + 2f;
	}

	void OnTriggerEnter(Collider other) {
		if (tickFinished < PauseScript.a.relativeTime) {
			if (other.gameObject.CompareTag("Player")) {
				Music.a.PlayTrack(LevelManager.a.currentLevel,trackType,musicType);
				Music.a.NotifyZone(trackType);
			}
			tickFinished = PauseScript.a.relativeTime + tick;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.CompareTag("Player")) {
			Music.a.Stop(); // return to normal upon leaving the trigger
		}
	}
}
