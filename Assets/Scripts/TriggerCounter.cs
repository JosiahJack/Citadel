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

	public void Targetted (UseData ud) {
		counter++;
		if (counter == countToTrigger) {
			if (delay <=0) {
				Target (ud);
			} else {
				StartCoroutine(DelayedTarget(ud));
			}

			//!dontReset == reset, bleh double negatives why'd I do that
			if (!dontReset) counter = 0;
		}
	}

	void Target(UseData ud) {
		ud.argvalue = argvalue;
		Const.a.UseTargets(gameObject,ud,target);
	}

    IEnumerator DelayedTarget(UseData ud) {
        yield return new WaitForSeconds(delay);
        Target(ud);
    }

	public static string Save(GameObject go) {
		TriggerCounter tc = go.GetComponent<TriggerCounter>();
		return Utils.IntToString(tc.counter,"counter");	
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		TriggerCounter tc = go.GetComponent<TriggerCounter>();
		tc.counter = Utils.GetIntFromString(entries[index],"counter"); index++;
		return index;
	}
}
