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
}
