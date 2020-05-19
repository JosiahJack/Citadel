using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncWall : MonoBehaviour {
	public float speed = 0.64f;
	public GameObject targetPosition;
	public enum FuncStates {Start, Target, MovingStart, MovingTarget,AjarMovingStart,AjarMovingTarget};
	public FuncStates startState;
	public float percentAjar = 0;
	public AudioClip SFXMoving;
	public AudioClip SFXStop;
	private Vector3 startPosition;
	private Vector3 goalPosition;
	private Vector3 tempVec;
	public AudioSource SFXSource;
	public FuncStates currentState; // save
	private Rigidbody rbody;
	private bool stopSoundPlayed;
	//private bool movedToLocation;
	private float dist;

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
		//movedToLocation = false;
		dist = 0;
	}
		
	public void Targetted (UseData ud) {
		//Debug.Log("Made it to Targetted on FuncWall!");
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
		//movedToLocation = false;
	}

	void MoveStart() {
		currentState = FuncStates.MovingStart;
	}

	void MoveTarget() {
		currentState = FuncStates.MovingTarget;
	}

	void FixedUpdate () {
		switch (currentState) {
			case FuncStates.Start:
				//if (!movedToLocation) {
					transform.position = startPosition;
					if (rbody.velocity.magnitude > 0) rbody.velocity = Vector3.zero;
					//movedToLocation = true;
				//}
				break;
			case FuncStates.Target:
				//if (!movedToLocation) {
					transform.position = targetPosition.transform.position;
					if (rbody.velocity.magnitude > 0) rbody.velocity = Vector3.zero;
					//movedToLocation = true;
				//}
				break;
			case FuncStates.MovingStart:
				goalPosition = startPosition;
				rbody.WakeUp();
				dist = speed * Time.deltaTime;
				tempVec = ((transform.position - goalPosition).normalized * dist * -1) + transform.position;
				rbody.MovePosition(tempVec);
				if (Vector3.Distance(transform.position,goalPosition) <= 0.04f) {
					currentState = FuncStates.Start;
					if (!stopSoundPlayed) {
						if (SFXSource != null && SFXStop != null && gameObject.activeInHierarchy)  {
							SFXSource.Stop ();
							SFXSource.loop = false;
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
				if (Vector3.Distance(transform.position,goalPosition) <= 0.04f) {
					currentState = FuncStates.Target;

					if (!stopSoundPlayed && SFXSource != null) {
						SFXSource.Stop ();
						SFXSource.loop = false;
						SFXSource.PlayOneShot (SFXStop);
						stopSoundPlayed = true;
					}
				}
				
				break;
		}
	}
}
