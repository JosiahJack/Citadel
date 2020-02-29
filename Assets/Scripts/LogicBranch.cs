using UnityEngine;
using System.Collections;

// Allows for consistently switching back and forth between two targets
public class LogicBranch : MonoBehaviour {
	public string target;
	public string target2;
	public string argvalue;
	public string argvalue2;
	public bool startOnSecond = false;
	public bool thisTioOverridesSender = true;
	public float delay = 0f;
	public bool relayEnabled = true;
	private float delayFinished;
	private UseData tempUd;
	private string currenttarget;
	private string currentargvalue;
	private bool onSecond = false;

	void Awake() {
		delayFinished = -1f;
		if (startOnSecond) {
			currenttarget = target2;
			currentargvalue = argvalue2;
			onSecond = true;
		} else {
			currenttarget = target;
			currentargvalue = argvalue;
			onSecond = false;
		}
	}
	
	// swap targets
	void FlipTrackSwitch() {
		if (onSecond) {
			currenttarget = target;
			currentargvalue = argvalue;
			onSecond = false;
		} else {
			currenttarget = target2;
			currentargvalue = argvalue2;
			onSecond = true;
		}
	}

	void Update() {
		if (!relayEnabled) return;
		if (delay <=0) return;

		if (delayFinished != -1f && delayFinished < Time.time) {
			RunTargets(tempUd);
			delayFinished = -1f;
		}
	}

	public void Targetted (UseData ud) {
		if (!relayEnabled) return;

		if (delay <=0) {
			RunTargets(ud);
		} else {
			delayFinished = Time.time + delay;
			tempUd = ud;
		}
	}

	void RunTargets(UseData ud) {
		if (currenttarget == null || currenttarget == "" || currenttarget == " " || currenttarget == "  ") return; // no target, do nothing
		ud.argvalue = currentargvalue;
		if (thisTioOverridesSender) {
			TargetIO tio = GetComponent<TargetIO>();
			if (tio != null) {
				ud.SetBits(tio);
			} else {
				Debug.Log("BUG: no TargetIO.cs found on an object with a LogicRelay.cs script!  Trying to call UseTargets without parameters!");
			}
		}
		Const.a.UseTargets(ud,currenttarget);
		FlipTrackSwitch(); // Swap targets
	}
}