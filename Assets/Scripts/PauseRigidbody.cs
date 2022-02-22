using UnityEngine;
using System.Collections;

public class PauseRigidbody : MonoBehaviour {
	private Rigidbody rbody;
	private Vector3 previousVelocity;
	private bool previousUseGravity;
	private bool previousKinematic;
	private CollisionDetectionMode previouscolDetMode;

	void Awake () {
		rbody = GetComponent<Rigidbody>();
	}
		
	public void Pause () {
		if (rbody != null) {
			previousVelocity = rbody.velocity;
			previousUseGravity = rbody.useGravity;
			previousKinematic = rbody.isKinematic;
			previouscolDetMode = rbody.collisionDetectionMode;
			rbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			rbody.useGravity = false;
			rbody.isKinematic = true;
		}
	}

	public void UnPause () {
		if (rbody != null) {
			rbody.isKinematic = previousKinematic;
			rbody.useGravity = previousUseGravity;
			rbody.collisionDetectionMode = previouscolDetMode;
			rbody.velocity = previousVelocity;
		}
	}
}
