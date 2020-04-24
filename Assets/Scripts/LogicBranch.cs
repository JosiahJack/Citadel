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
	public bool relayEnabled = true; // save
	private UseData tempUd;
	private string currenttarget;
	private string currentargvalue;
	[HideInInspector]
	public bool onSecond = false; // save
	public bool autoFlipOnTarget = true;

	void Awake() {
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
	public void FlipTrackSwitch() {
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

    IEnumerator DelayedTarget(UseData ud) {
        yield return new WaitForSeconds(delay);
        if (relayEnabled) RunTargets(ud);
    }

	public void Targetted (UseData ud) {
		if (!relayEnabled) return;

		if (delay <=0) {
			RunTargets(ud);
		} else {
			StartCoroutine(DelayedTarget(ud));
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
		if (autoFlipOnTarget) FlipTrackSwitch();
	}
}