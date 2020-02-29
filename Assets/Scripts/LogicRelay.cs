using UnityEngine;
using System.Collections;

public class LogicRelay : MonoBehaviour {
	public string target;
	public string argvalue;
	public bool thisTioOverridesSender = true;
	public float delay = 0f;
	public bool relayEnabled = true;
	private float delayFinished;
	private UseData tempUd;

	void Awake() {
		delayFinished = -1f;
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
		if (target == null || target == "" || target == " " || target == "  ") {
			Debug.Log("WARNING: logic relay attempting to target nothing");
			return; // no target, do nothing
		}
		ud.argvalue = argvalue;
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
