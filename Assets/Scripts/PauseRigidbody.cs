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
		if (rbody.isKinematic && rbody.collisionDetectionMode != CollisionDetectionMode.ContinuousSpeculative) Debug.Log(gameObject.name + " has isKinematic true on initialize when not using ContinuousSpeculative!");
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
			PrefabIdentifier pid = GetComponent<PrefabIdentifier>();
			if (pid != null && GetComponent<AIController>() == null) {
				if (ConsoleEmulator.ConstIndexIsDynamicObject(pid.constIndex)) {
					rbody.isKinematic = false;
					rbody.useGravity = true;
				}
			} else {
				rbody.isKinematic = previousKinematic;
				rbody.useGravity = previousUseGravity;
			}
			
			if (previouscolDetMode == CollisionDetectionMode.ContinuousSpeculative
				&& !rbody.isKinematic && GetComponent<AIController>() == null) {
				
				previouscolDetMode = CollisionDetectionMode.ContinuousDynamic;
			}
			
			if (rbody.isKinematic && previouscolDetMode != CollisionDetectionMode.ContinuousSpeculative) {
				rbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			} else {
				rbody.collisionDetectionMode = previouscolDetMode;
			}
			
			rbody.velocity = previousVelocity;
		}
	}
}
