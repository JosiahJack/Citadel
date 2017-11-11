using UnityEngine;
using System.Collections;

public class LogicRelay : MonoBehaviour {
	public GameObject target;
	public GameObject target1;
	public GameObject target2;
	public GameObject target3;
	public float delay = 0f;
	private float delayFinished;
	private GameObject playerCamera;

	void Awake () {
		delayFinished = 0f;
	}

	public void Targetted (GameObject owner) {
		playerCamera = owner;  // set playerCamera to owner of the input (always should be the player camera)
		if (delay > 0) {
			delayFinished = Time.time + delay;
		} else {
			UseTargets();
		}
	}

	public void UseTargets () {
		if (target != null) {
			target.SendMessageUpwards("Targetted", playerCamera);
		}
		if (target1 != null) {
			target1.SendMessageUpwards("Targetted", playerCamera);
		}
		if (target2 != null) {
			target2.SendMessageUpwards("Targetted", playerCamera);
		}
		if (target3 != null) {
			target3.SendMessageUpwards("Targetted", playerCamera);
		}
	}

	void Update () {
		if ((delayFinished < Time.time) && delayFinished != 0) {
			delayFinished = 0;
			UseTargets();
		}
	}
}
