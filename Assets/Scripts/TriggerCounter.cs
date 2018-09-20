using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCounter : MonoBehaviour {
	public int countToTrigger;
	public int counter;
	public GameObject[] target;
	public float[] delay;
	public bool dontReset;
	private float[] delayFinished;
	private bool[] delayStarted;

	void Awake() {
		// Check and set delays array to equal the target array
		if (delay.Length < target.Length) {
			float[] tempDelays = delay; // temporarily store the delays
			delay = new float[target.Length]; // wipe out array with new larger array same size as the targets
			for (int j = 0; j < target.Length; j++) {
				if (j >= tempDelays.Length) {
					delay [j] = 0;
					continue;
				}
				delay [j] = tempDelays [j]; // put values back
			}
		}

		// Set delayFinished and delayStarted array to equal the target array length before zeroing out
		delayFinished = new float[target.Length];
		delayStarted = new bool[target.Length];

		// zero out the private arrays for the delay time keepers
		for (int i = 0; i < target.Length; i++) {
			delayStarted [i] = false;
			delayFinished [i] = Time.time;
		}
	}

	public void Targetted (UseData ud) {
		counter++;

		if (counter == countToTrigger) {
			Target ();
		}
	}

	void Target() {
		if (!dontReset) counter = 0;

		UseData ud = new UseData();
		ud.owner = Const.a.allPlayers;  // TODO: should we have a different Const.a.nullPlayer?
		for (int i = 0; i < target.Length; i++) {
			if (delay [i] > 0) {
				delayStarted [i] = true; // tell Update() to check the timekeeper to see if we are past the delay
				delayFinished[i] = Time.time + delay [i]; // set delay timekeeper
			} else {
				if (target [i] != null) {
					target [i].SendMessageUpwards ("Targetted", ud);
				}
			}
		}
	}

	void Update () {
		for (int i = 0; i < target.Length; i++) {
			if (delayStarted [i]) {
				if (delayFinished[i] < Time.time) {
					UseData ud = new UseData();
					ud.owner = Const.a.allPlayers; // TODO: should we have a different Const.a.nullPlayer?

					delayStarted [i] = false; // reset bit so we don't keep triggering every frame
					if (target[i] != null) {
						target[i].SendMessageUpwards ("Targetted", ud); // fire in the hole
					}
				}
			}
		}
	}
}
