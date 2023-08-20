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
		if (tc == null) {
			Debug.LogError("TriggerCounter missing on savetype of "
					       + "TriggerCounter!  GameObject.name: " + go.name);
			return "0";
		}

		string line = System.String.Empty;
		line = tc.counter.ToString(); // int - how many counts we have
		return line;	
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		TriggerCounter tc = go.GetComponent<TriggerCounter>();
		if (tc == null) {
			Debug.LogError("TriggerCounter.Load failure, tc == null");
			return index + 1;
		}

		if (index < 0) {
			Debug.LogError("TriggerCounter.Load failure, index < 0");
			return index + 1;
		}

		if (entries == null) {
			Debug.LogError("TriggerCounter.Load failure, entries == null");
			return index + 1;
		}

		tc.counter = Utils.GetIntFromString(entries[index]); index++;
		return index;
	}
}
