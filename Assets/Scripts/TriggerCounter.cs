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
		//owner = ud.owner;  Nope, last targetter becomes the owner
		// Debug.Log("Trigger_counter count at " + counter.ToString() + ", with targetname of " + GetComponent<TargetIO>().targetname);
		if (counter == countToTrigger) {
			if (delay <=0) {
				Target (ud);
			} else {
				StartCoroutine(DelayedTarget(ud));
			}

			//!dontReset == reset, bleh double negatives why'd I do that
			if (!dontReset) {
				counter = 0; 
			}
		}
	}

	void Target(UseData ud) {
		ud.argvalue = argvalue;
		TargetIO tio = GetComponent<TargetIO>();
		if (tio != null) {
			ud.SetBits(tio);
			//Debug.Log("Set tio bits in Target on TriggerCounter.cs");
			//if (tio.targetname == "lev1count1") Debug.Log("tio.lockCodeToScreenMaterialChanger = " + ud.lockCodeToScreenMaterialChanger.ToString());
		} else {
			Debug.Log("BUG: no TargetIO.cs found on an object with a TriggerCounter.cs script!  Trying to call UseTargets without parameters!");
		}
		Const.a.UseTargets(ud,target);
		//Debug.Log("Trigger_counter fired normally");
	}

    IEnumerator DelayedTarget(UseData ud) {
        yield return new WaitForSeconds(delay);
        Target(ud);
    }

}
