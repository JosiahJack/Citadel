using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectImpact : MonoBehaviour {
	[HideInInspector]
	public Rigidbody rbody;
	[HideInInspector]
	public Vector3 oldVelocity;
	public float impactSoundSpeed = 11.72f;
	[HideInInspector]
	public AudioSource SFXSource;
	public AudioClip SFX;

	// Use this for initialization
	void Awake () {
		rbody = GetComponent<Rigidbody> ();
		if (rbody == null) {
			Const.sprint ("ERROR: No rigidbody found on object with ObjectImpact script!", Const.a.allPlayers);
			transform.gameObject.SetActive (false);
		}

		SFXSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Handle impact sound
		if (Mathf.Abs ((oldVelocity.y - rbody.velocity.y)) > impactSoundSpeed) {
			if (SFXSource != null) {
				SFXSource.PlayOneShot (SFX); // Play sound when object changes velocity significantly enough that it must have hit something
			}
		}
		oldVelocity = rbody.velocity;
	}
}
