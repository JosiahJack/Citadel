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
			HealthManager hm = Utils.GetMainHealthManager(col.gameObject);
			if (hm != null) {
				if (hm.health > 0f && justUsed < PauseScript.a.relativeTime) {
					if (teleportFX != null) teleportFX.SetActive(true);
					col.transform.position = targetDestination.position;
					TeleportTouch tt = targetDestination.transform.gameObject.GetComponent<TeleportTouch>();
					if (tt != null) tt.justUsed = PauseScript.a.relativeTime + 1.0f;
					if (playSound) Utils.PlayOneShotSavable(SoundFXSource,SoundFX);
				}
			}
		}
	}

	public static string Save(GameObject go) {
		TeleportTouch tt = go.GetComponent<TeleportTouch>();
		if (tt == null) {
			Debug.Log("TeleportTouch missing on savetype of TeleportTouch!  GameObject.name: " + go.name);
			return "0000.00000";
		}

		string line = System.String.Empty;
		line = Utils.SaveRelativeTimeDifferential(tt.justUsed); // float - is the player still touching it?
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		TeleportTouch tt = go.GetComponent<TeleportTouch>();
		if (tt == null) {
			Debug.Log("TeleportTouch.Load failure, tt == null");
			return index + 1;
		}

		if (index < 0) {
			Debug.Log("TeleportTouch.Load failure, index < 0");
			return index + 1;
		}

		if (entries == null) {
			Debug.Log("TeleportTouch.Load failure, entries == null");
			return index + 1;
		}

		tt.justUsed = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - is the player still touching it?
		return index;
	}
}