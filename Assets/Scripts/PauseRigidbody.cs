using UnityEngine;
using System.Collections;

public class PauseRigidbody : MonoBehaviour {
	[HideInInspector] public Rigidbody rbody;
	private Vector3 previousVelocity;
	private bool previousUseGravity;
	private bool previousKinematic;
	private CollisionDetectionMode previouscolDetMode;

	void Awake() {
		Initialize();
	}

	void Initialize() {
		if (rbody == null) rbody = GetComponent<Rigidbody>();
		if (rbody == null) rbody = gameObject.AddComponent<Rigidbody>();
		if (!Const.a.prb.Contains(this)) Const.a.prb.Add(this);
		SetPreviousValues();
	}

	void SetPreviousValues() {
		previousVelocity = rbody.velocity;
		previousUseGravity = rbody.useGravity;
		previousKinematic = rbody.isKinematic;
		previouscolDetMode = rbody.collisionDetectionMode;
	}

	void OnEnable() {
		if (rbody == null) Initialize();
		if (PauseScript.a.MenuActive()) Pause();
	}
		
	public void Pause() {
		if (rbody != null) {
			SetPreviousValues();
			rbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			rbody.useGravity = false;
			rbody.isKinematic = true;
		}
	}

	public void UnPause() {
		if (rbody != null) {
			rbody.isKinematic = previousKinematic;
			rbody.useGravity = previousUseGravity;
			rbody.collisionDetectionMode = previouscolDetMode;
			rbody.velocity = previousVelocity;
		}
	}
}
