using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectImpact : MonoBehaviour {
	[HideInInspector]
	public Rigidbody rbody;
	[HideInInspector]
	public Vector3 oldVelocity;
	public float minVolumeSpeed = 2f;
	public float maxVolumeSpeed = 10f;
	[HideInInspector]
	public AudioSource SFXSource;
	public AudioClip ImpactSFX;


	void Start () {
		rbody = GetComponent<Rigidbody> ();
		if (rbody == null) {
			Const.sprint ("ERROR: No rigidbody found on object with ObjectImpact script!", Const.a.allPlayers);
			//transform.gameObject.SetActive (false);
		}
		SFXSource = GetComponent<AudioSource> ();
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.relativeVelocity.magnitude > minVolumeSpeed) {
			if (SFXSource != null) {
				SFXSource.pitch = (UnityEngine.Random.Range(0.8f,1.2f));
				SFXSource.PlayOneShot (ImpactSFX,(collision.relativeVelocity.magnitude/maxVolumeSpeed) * 0.3f); // Play sound when object changes velocity significantly enough that it must have hit something
			}
		}
	}
}
