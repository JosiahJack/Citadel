using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectImpact : MonoBehaviour {
	// External values set per prefab instance, optional.
	public float minVolumeSpeed = 2f;
	public float maxVolumeSpeed = 10f;
	public AudioClip ImpactSFX;

	// Internal references, required
	[HideInInspector] public AudioSource SFXSource;
	[HideInInspector] public Rigidbody rbody;
	[HideInInspector] public Vector3 oldVelocity;

	void Start () {
		rbody = GetComponent<Rigidbody>();
		if (rbody == null) this.enabled = false;
		SFXSource = GetComponent<AudioSource>();
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.relativeVelocity.sqrMagnitude > (minVolumeSpeed * minVolumeSpeed)) {
			if (SFXSource != null) {
				SFXSource.pitch = (UnityEngine.Random.Range(0.8f,1.2f));
				Utils.PlayOneShotSavable(SFXSource,ImpactSFX,(collision.relativeVelocity.magnitude/maxVolumeSpeed) * 0.3f); // Play sound when object changes velocity significantly enough that it must have hit something
			}
		}
	}
}
