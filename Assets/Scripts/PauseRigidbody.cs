using UnityEngine;
using System.Collections;

public class PauseRigidbody : MonoBehaviour {
	private Rigidbody rbody;
	private Vector3 previousVelocity;
	private bool justUnPaused = false;
	private bool justPaused = false;

	void Awake () {
		rbody = GetComponent<Rigidbody>();
		justPaused = true;
	}

	void Update () {
		if (PauseScript.a != null && PauseScript.a.paused) {
			if (rbody != null) {
				if (justPaused) {
					justPaused = false;
					justUnPaused = true;
					previousVelocity = rbody.velocity;
					rbody.isKinematic = true;
				}
			}
		} else {
			if (rbody != null) {
				if (justUnPaused) {
					justUnPaused = false;
					justPaused = true;
					rbody.isKinematic = false;
					rbody.velocity = previousVelocity;
				}
			}
		}
	}
}
