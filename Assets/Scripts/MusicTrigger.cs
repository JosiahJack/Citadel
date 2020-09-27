using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour {
	public float tick = 0.1f;
	private float tickFinished;
	public Music.TrackType trackType;
	public Music.MusicType musicType;

	void Awake() {
		tickFinished = PauseScript.a.relativeTime + 2f;
	}

	void OnTriggerEnter(Collider other) {
		if (tickFinished < PauseScript.a.relativeTime) {
			if (other.gameObject.CompareTag("Player")) {
				Music.a.PlayTrack(LevelManager.a.currentLevel,Music.TrackType.Elevator,Music.MusicType.Override);
			}
			tickFinished = PauseScript.a.relativeTime + tick;
		}
	}
}
