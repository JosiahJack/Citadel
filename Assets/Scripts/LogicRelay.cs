using UnityEngine;
using System.Collections;

public class LogicRelay : MonoBehaviour {
	public string target;
	public string argvalue;
	public bool thisTioOverridesSender = true;
	public float delay = 0f;
	public bool relayEnabled = true; // save
	private UseData tempUd;

	public void Targetted (UseData ud) {
		if (!relayEnabled) return;

		if (delay <=0) {
			RunTargets(ud);
		} else {
			StartCoroutine(DelayedTarget(ud));
		}
	}

    IEnumerator DelayedTarget(UseData ud) {
        yield return new WaitForSeconds(delay);
        if (relayEnabled) RunTargets(ud);
    }

	void RunTargets(UseData ud) {
		if (target == null || target == "" || target == " " || target == "  ") {
			Debug.Log("WARNING: logic relay attempting to target nothing");
			return; // no target, do nothing
		}
		ud.argvalue = argvalue;
		delay = -1f;
		if (thisTioOverridesSender) {
			TargetIO tio = GetComponent<TargetIO>();
			if (tio != null) {
				ud.SetBits(tio);
			} else {
				Debug.Log("BUG: no TargetIO.cs found on an object with a LogicRelay.cs script!  Trying to call UseTargets without parameters!");
			}
		}
		Const.a.UseTargets(ud,target);
	}

	// This is only here because I added this functionality to trigger_relay in
	// my Quake Keep mod when I thought it was a feature of Arcane Dimensions,
	// but then resulted in me having a completely broken halloween jam map for
	// I think, Halloween Jam 2 and thus felt I should really make it available
	// here as well since it's a whole thing now.
	//
	// Also I think I used this in a few places.  This is the relayEnabled by
	// the way.
	public static string Save(GameObject go) {
		LogicRelay lr = go.GetComponent<LogicRelay>();
		if (lr == null) {
			Debug.Log("LogicRelay missing on savetype of LogicRelay!  GameObject.name: " + go.name);
			return "1";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(lr.relayEnabled); // bool - is this enabled, Sherlock?
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		LogicRelay lr = go.GetComponent<LogicRelay>(); // Similar to the LB, also a handy L shaped junction box complete with a lid for easy wire pulling.  This time, the lid is to the Right instead of the Back, hence the R of LR.
		if (lr == null || index < 0 || entries == null) return index + 1;

		lr.relayEnabled = Utils.GetBoolFromString(entries[index]); index++; // bool - is this enabled
		return index;
	}
}
