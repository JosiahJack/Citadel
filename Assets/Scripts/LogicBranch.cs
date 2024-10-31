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
		ud.argvalue = currentargvalue;
		if (thisTioOverridesSender) {
			TargetIO tio = GetComponent<TargetIO>();
			if (tio != null) {
				ud.SetBits(tio);
			} else {
				Debug.Log("BUG: no TargetIO.cs found on an object with a "
						  + "LogicRelay.cs script!  Trying to call UseTargets"
						  + " without parameters!");
			}
		}

		Const.a.UseTargets(null,ud,currenttarget);
		if (autoFlipOnTarget) FlipTrackSwitch();
	}

	public static string Save(GameObject go) {
		LogicBranch lb = go.GetComponent<LogicBranch>();
		string line = System.String.Empty;
		line = Utils.BoolToString(lb.relayEnabled,"relayEnabled");
		line += Utils.splitChar + Utils.BoolToString(lb.onSecond,"onSecond"); // He is. But who's on third? What's on first? Wait what??
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		LogicBranch lb = go.GetComponent<LogicBranch>(); // A handy L shaped junction Box complete with a lid for easy wire pulling.  Who knew LB's could be so cool!
		lb.relayEnabled = Utils.GetBoolFromString(entries[index],"relayEnabled"); index++;
		lb.onSecond = Utils.GetBoolFromString(entries[index],"onSecond"); index++;
		return index;
	}
}
