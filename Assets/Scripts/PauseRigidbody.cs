using UnityEngine;
using System.Collections;

public class PauseRigidbody : MonoBehaviour {
	private Rigidbody rbody;
	private Vector3 previousVelocity;
	private bool previousUseGravity;
	private bool previousKinematic;
	private CollisionDetectionMode previouscolDetMode;

	void Awake () {
		Initialize();
	}

	void Initialize() {
		rbody = GetComponent<Rigidbody>();
		if (!Const.a.prb.Contains(this)) Const.a.prb.Add(this);
		SetPreviousValuse();
	}

	void SetPreviousValuse() {
		previousVelocity = rbody.velocity;
		previousUseGravity = rbody.useGravity;
		previousKinematic = rbody.isKinematic;
		previouscolDetMode = rbody.collisionDetectionMode;
	}

	void OnEnable () {
		if (rbody == null) Initialize();
	}
		
	public void Pause () {
		if (rbody != null) {
			SetPreviousValuse();
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
