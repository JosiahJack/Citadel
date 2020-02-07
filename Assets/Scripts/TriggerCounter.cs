using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCounter : MonoBehaviour {
	public int countToTrigger;
	public int counter;
	public string target;
	public string argvalue;
	public float delay;
	public bool dontReset;
	private float delayFinished;
	private bool delayStarted;
	private UseData delayedUd;

	void Awake() {
		// Set delayFinished and delayStarted array to equal the target array length before zeroing out
		delayFinished = Time.time;
		delayStarted = false;
	}

	public void Targetted (UseData ud) {
		counter++;
		//owner = ud.owner;  Nope, last targetter becomes the owner

		if (counter == countToTrigger) {
			if (delay > 0) {
				delayFinished = Time.time + delay;
				delayStarted = true;
				delayedUd = ud;
			} else {
				Target (ud);
			}

			if (!dontReset) {
				counter = 0; //reset, bleh double negatives why'd I do that
			}
		}
	}

	void Target(UseData ud) {
		ud.argvalue = argvalue;
		TargetIO tio = GetComponent<TargetIO>();
		if (tio != null) {
			ud.SetBits(tio);
		} else {
			Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
		}
		Const.a.UseTargets(ud,target);
	}

	void Update () {
		if (delayStarted && delayFinished < Time.time) {
			delayStarted = false; // reset bit so we don't keep triggering every frame
			Target (delayedUd);
		}
	}
}
