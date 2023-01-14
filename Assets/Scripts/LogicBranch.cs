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
		if (string.IsNullOrWhiteSpace(currenttarget)) return; // no target, do nothing

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

	public static string Save(GameObject go) {
		LogicBranch lb = go.GetComponent<LogicBranch>();
		if (lb == null) {
			Debug.Log("LogicBranch missing on savetype of LogicBranch!  GameObject.name: " + go.name);
			return "1|0";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(lb.relayEnabled); // bool - is this enabled
		line += Utils.splitChar + Utils.BoolToString(lb.onSecond); // bool - He is. But who's on third? What's on first? Wait what??
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		LogicBranch lb = go.GetComponent<LogicBranch>(); // A handy L shaped junction box complete with a lid for easy wire pulling.  Who knew LB's could be so cool!
		if (lb == null) {
			Debug.Log("LogicBranch.Load failure, lb == null");
			return index + 2;
		}

		if (index < 0) {
			Debug.Log("LogicBranch.Load failure, index < 0");
			return index + 2;
		}

		if (entries == null) {
			Debug.Log("LogicBranch.Load failure, entries == null");
			return index + 2;
		}

		lb.relayEnabled = Utils.GetBoolFromString(entries[index]); index++; // bool - is this enabled
		lb.onSecond = Utils.GetBoolFromString(entries[index]); index++; // bool - which one are we on?  Tap tap...this thing on?  I'll second that.
		return index;
	}
}