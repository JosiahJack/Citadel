﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectImpact : MonoBehaviour {
	[HideInInspector]
	public Rigidbody rbody;
	[HideInInspector]
	public Vector3 oldVelocity;
	private float impactSoundSpeed = 3f;
	[HideInInspector]
	public AudioSource SFXSource;
	public AudioClip SFX;

	void Awake () {
		rbody = GetComponent<Rigidbody> ();
		if (rbody == null) {
			Const.sprint ("ERROR: No rigidbody found on object with ObjectImpact script!", Const.a.allPlayers);
			transform.gameObject.SetActive (false);
		}

		SFXSource = GetComponent<AudioSource> ();
	}
	
	void FixedUpdate () {
		// Handle impact sound
		if (SFX == null) return;
		if (Mathf.Abs ((oldVelocity.y - rbody.velocity.y)) > impactSoundSpeed) {
			if (SFXSource != null) {
				SFXSource.PlayOneShot (SFX); // Play sound when object changes velocity significantly enough that it must have hit something
			}
		}
		oldVelocity = rbody.velocity;
	}
}
