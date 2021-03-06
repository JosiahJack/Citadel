﻿using UnityEngine;
using System.Collections;

public class TeleportTouch : MonoBehaviour {
	public Transform targetDestination; // assign in the editor
	public bool playSound;
	public AudioClip SoundFX;
	public GameObject teleportFX;
	public float justUsed = 0f; // save
	private AudioSource SoundFXSource;

	void Awake () {
		SoundFXSource = GetComponent<AudioSource>();
		if (SoundFXSource != null) SoundFXSource.playOnAwake = false;
	}

	void  OnTriggerEnter ( Collider col  ) {
		if (targetDestination == null || col == null) return;
		if (col.gameObject == null) return;

		if (col.gameObject.CompareTag("Player")) {
			HealthManager hm = col.gameObject.GetComponent<HealthManager>();
			if (hm != null) {
				if (hm.health > 0f && justUsed < PauseScript.a.relativeTime) {
					if (teleportFX != null) teleportFX.SetActive(true);
					col.transform.position = targetDestination.position;
					TeleportTouch tt = targetDestination.transform.gameObject.GetComponent<TeleportTouch>();
					if (tt != null) tt.justUsed = PauseScript.a.relativeTime + 1.0f;
					if (playSound && SoundFXSource != null && SoundFX != null) SoundFXSource.PlayOneShot(SoundFX);
				}
			}
		}
	}
}