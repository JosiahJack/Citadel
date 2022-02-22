using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncWall : MonoBehaviour {
	// Externally set in inspector per instance
	public float speed = 0.64f;
	public GameObject targetPosition;
	public FuncStates startState;
	public float percentAjar = 0;
	public AudioClip SFXMoving;
	public AudioClip SFXStop;
	public AudioSource SFXSource;
	public FuncStates currentState; // save

	// Enumerations
	public enum FuncStates : byte {Start, Target, MovingStart, MovingTarget,AjarMovingStart,AjarMovingTarget};

	private Vector3 startPosition;
	private Vector3 goalPosition;
	private Vector3 tempVec;
	private Rigidbody rbody;
	private bool stopSoundPlayed;
	private float dist;
	private float startTime;

	void Awake () {
		currentState = startState; // set door position to picked state
		startPosition = transform.position;
		if (currentState == FuncStates.AjarMovingStart || currentState == FuncStates.AjarMovingTarget) {
			tempVec = ((transform.position - targetPosition.transform.position).normalized * (Vector3.Distance(transform.position,targetPosition.transform.position) * percentAjar * -1)) + transform.position;
			transform.position = tempVec;
		}
		rbody = GetComponent<Rigidbody>();
		rbody.isKinematic = true;
		rbody.useGravity = false;
		rbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		if (SFXSource == null) SFXSource = GetComponent<AudioSource>();
		stopSoundPlayed = false;
		dist = 0;
		startTime = PauseScript.a.relativeTime;
	}
		
	public void Targetted (UseData ud) {
		switch (currentState) {
			case FuncStates.Start:
				MoveTarget();
				break;
			case FuncStates.Target:
				MoveStart();
				break;
			case FuncStates.MovingStart:
				MoveTarget ();
				break;
			case FuncStates.MovingTarget:
				MoveStart ();
				break;
			case FuncStates.AjarMovingStart:
				MoveStart();
				break;
			case FuncStates.AjarMovingTarget:
				MoveTarget();
				break;
		}
		if (SFXSource != null) SFXSource.clip = SFXMoving;
		if (SFXSource != null) SFXSource.loop = true;
		if (SFXSource != null) SFXSource.Play();
		stopSoundPlayed = false;
	}

	void MoveStart() {
		currentState = FuncStates.MovingStart;
		startTime = PauseScript.a.relativeTime + 10f;
	}

	void MoveTarget() {
		currentState = FuncStates.MovingTarget;
		startTime = PauseScript.a.relativeTime + 10f;
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeSelf) {
			switch (currentState) {
				case FuncStates.Start:
					transform.position = startPosition;
					if (rbody.velocity.sqrMagnitude > 0) rbody.velocity = Const.a.vectorZero;
					break;
				case FuncStates.Target:
					transform.position = targetPosition.transform.position;
					if (rbody.velocity.sqrMagnitude > 0) rbody.velocity = Const.a.vectorZero;
					break;
				case FuncStates.MovingStart:
					goalPosition = startPosition;
					rbody.WakeUp();
					dist = speed * Time.deltaTime;
					tempVec = ((transform.position - goalPosition).normalized * dist * -1) + transform.position;
					rbody.MovePosition(tempVec);
					if (Vector3.Distance(transform.position,goalPosition) <= 0.04f || startTime < PauseScript.a.relativeTime) {
						currentState = FuncStates.Start;
						if (SFXSource != null) {
							SFXSource.Stop ();
							SFXSource.loop = false;
							if (!stopSoundPlayed && SFXSource != null) {
								SFXSource.PlayOneShot (SFXStop);
								stopSoundPlayed = true;
							}
						}
					}
					break;
				case FuncStates.MovingTarget:
					goalPosition = targetPosition.transform.position;
					rbody.WakeUp();
					dist = speed * Time.deltaTime;
					tempVec = ((transform.position - goalPosition).normalized * dist * -1) + transform.position;
					rbody.MovePosition(tempVec);
					if (Vector3.Distance(transform.position,goalPosition) <= 0.04f || startTime < PauseScript.a.relativeTime) {
						currentState = FuncStates.Target;
						if (SFXSource != null) {
							SFXSource.Stop ();
							SFXSource.loop = false;
							if (!stopSoundPlayed && SFXSource != null) {
								SFXSource.PlayOneShot (SFXStop);
								stopSoundPlayed = true;
							}
						}
					}
					break;
			}
		}
	}
}
