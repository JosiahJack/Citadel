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
	public GameObject trigger;
	public bool useTrigger;
	public bool useOnlyActivatesTrigger;
	private Vector3 startPosition;
	private Vector3 goalPosition;
	private Vector3 tempVec;
	private AudioSource SFXSource;
	private FuncStates currentState;
	private Rigidbody rbody;
	private bool stopSoundPlayed;
	private bool movedToLocation;
	private float dist;

	void Awake () {
		currentState = startState; // set door position to picked state
		startPosition = transform.position;
		if (currentState == FuncStates.AjarMovingStart || currentState == FuncStates.AjarMovingTarget) {
			tempVec = ((transform.position - targetPosition.transform.position).normalized * (Vector3.Distance(transform.position,targetPosition.transform.position) * percentAjar * -1)) + transform.position;
			transform.position = tempVec;
			if (useTrigger) trigger.SetActive (false);
		}
		rbody = GetComponent<Rigidbody>();
		rbody.isKinematic = true;
		SFXSource = GetComponent<AudioSource>();
		stopSoundPlayed = false;
		movedToLocation = false;
		dist = 0;
		if (useOnlyActivatesTrigger)
			trigger.SetActive (false);
	}
		
	public void Targetted (UseData ud) {
		if (useOnlyActivatesTrigger) {
			trigger.SetActive (true);
			useOnlyActivatesTrigger = false;
			return;
		}

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
		SFXSource.clip = SFXMoving;
		SFXSource.loop = true;
		SFXSource.Play();
		stopSoundPlayed = false;
		movedToLocation = false;
	}

	void MoveStart() {
		currentState = FuncStates.MovingStart;
		if (useTrigger) {
			trigger.SetActive (false);
		}
	}

	void MoveTarget() {
		currentState = FuncStates.MovingTarget;
		if (useTrigger) {
			trigger.SetActive (false);
		}
	}

	void FixedUpdate () {
		switch (currentState) {
		case FuncStates.Start:
				if (!movedToLocation) {
					transform.position = startPosition;
					rbody.velocity = Vector3.zero;
					movedToLocation = true;
				}
				break;
			case FuncStates.Target:
				if (!movedToLocation) {
					transform.position = targetPosition.transform.position;
					rbody.velocity = Vector3.zero;
					movedToLocation = true;
				}
				break;
			case FuncStates.MovingStart:
				goalPosition = startPosition;
				//transform.position = Vector3.MoveTowards (transform.position, goalPosition, (speed * Time.deltaTime));
				rbody.WakeUp();
				//rbody.velocity = ((goalPosition - transform.position) * speed);
				dist = speed * Time.deltaTime;
				tempVec = ((transform.position - goalPosition).normalized * dist * -1) + transform.position;
				rbody.MovePosition(tempVec);
				if ((Vector3.Distance(transform.position,goalPosition)) <= 0.02f) {
					if (currentState == FuncStates.MovingStart) {
						currentState = FuncStates.Start;
					} else {
						currentState = FuncStates.Target;
					}

					if (!stopSoundPlayed) {
						SFXSource.Stop ();
						SFXSource.loop = false;
						SFXSource.PlayOneShot (SFXStop);
						stopSoundPlayed = true;
					}
					if (useTrigger) trigger.SetActive (true);
				}
				break;
			case FuncStates.MovingTarget:
				goalPosition = targetPosition.transform.position;
				//transform.position = Vector3.MoveTowards (transform.position, goalPosition, (speed * Time.deltaTime));
				rbody.WakeUp();
				//rbody.velocity = ((goalPosition - transform.position) * speed);
				//rbody.velocity = new Vector3(0,-1f,0);
				//rbody.velocity = (goalPosition - transform.position) * speed;
				//rbody.MovePosition(goalPosition);
				dist = speed * Time.deltaTime;
				tempVec = ((transform.position - goalPosition).normalized * dist * -1) + transform.position;
				rbody.MovePosition(tempVec);
				if ((Vector3.Distance(transform.position,goalPosition)) <= 0.01f) {
					if (currentState == FuncStates.MovingStart) {
						currentState = FuncStates.Start;
					} else {
						currentState = FuncStates.Target;
					}

					if (!stopSoundPlayed) {
						SFXSource.Stop ();
						SFXSource.loop = false;
						SFXSource.PlayOneShot (SFXStop);
						stopSoundPlayed = true;
					}

					if (useTrigger) {
						trigger.SetActive (true);
					}
				}
				
				break;
		}
	}
}
