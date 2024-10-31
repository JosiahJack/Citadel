using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Trigger : MonoBehaviour {
	public float delay = 0;
	public bool onlyOnce = false;
	public bool ignoreSecondaryTriggers = false;
	public int numPlayers = 0;
	public string target;
	public string argvalue; // e.g. how much to set a counter to
	[HideInInspector] public GameObject recentMostActivator;
	[HideInInspector] public float delayFireFinished;
	[HideInInspector] public float delayResetFinished;
	[HideInInspector] public bool allDone = false;

    IEnumerator DelayedTarget(GameObject activator) {
        yield return new WaitForSeconds(delay);
        UseTargets(activator);
    }

	public void UseTargets (GameObject activator) {
		UseData ud = new UseData();
		ud.owner = activator;
		ud.argvalue = argvalue;
		Const.a.UseTargets(gameObject,ud,target);
	}

	void TriggerTripped (Collider col, bool initialEntry) {
		if (col == null) Debug.Log("BUG: TriggerTripped was fed a null col!");

		if (col.gameObject.CompareTag("Player")) {
			HealthManager hm = Utils.GetMainHealthManager(col.gameObject);
			if (hm != null) {
				if (hm.health > 0f && hm.isPlayer) {
					if (recentMostActivator != null) {
						if (ignoreSecondaryTriggers) return;
					}
					recentMostActivator = col.gameObject;

					if (initialEntry && recentMostActivator.CompareTag("Player")) numPlayers++;
					if (onlyOnce) allDone = true;
					
					if (delay <=0) {
						UseTargets(recentMostActivator);
					} else {
						StartCoroutine(DelayedTarget(recentMostActivator));
					}
				}
			}
		}
	}

	void OnTriggerEnter (Collider col) {
		if (allDone) return;
		if (col == null) return;
		if (col.gameObject == null) return;
		if (col.gameObject.CompareTag("Player"))
			TriggerTripped(col,true);
	}

	void  OnTriggerStay (Collider col) {
		if (allDone) return;
		if (col == null) return;
		if (col.gameObject == null) return;
		if (col.gameObject.CompareTag("Player"))
			TriggerTripped (col, false);
	}

	void OnTriggerExit (Collider col) {
		if (allDone) return;
		if (col.gameObject.CompareTag("Player")) numPlayers--;
	}

	public void Targetted (UseData ud) {
		if (ignoreSecondaryTriggers) recentMostActivator = ud.owner;
		//TriggerTripped (Collider col, bool initialEntry);
	}

	public static string Save(GameObject go) {
		Trigger trig = go.GetComponent<Trigger>();
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.BoolToString(trig.allDone,"allDone"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(trig.numPlayers,"numPlayers"));
		s1.Append(Utils.splitChar);
		if (trig.recentMostActivator != null) s1.Append("hadRecentActivator:1");
		else s1.Append("hadRecentActivator:0");

		s1.Append(Utils.splitChar);		
		s1.Append(Utils.SaveRelativeTimeDifferential(trig.delayFireFinished,"delayFireFinished"));
		s1.Append(Utils.splitChar);		
		s1.Append(Utils.SaveRelativeTimeDifferential(trig.delayResetFinished,"delayResetFinished"));
		s1.Append(Utils.splitChar);		
		s1.Append(Utils.FloatToString(trig.delay,"delay"));
		s1.Append(Utils.splitChar);		
		s1.Append(Utils.BoolToString(trig.onlyOnce,"onlyOnce"));
		s1.Append(Utils.splitChar);		
		s1.Append(Utils.BoolToString(trig.ignoreSecondaryTriggers,"ignoreSecondaryTriggers"));
		s1.Append(Utils.splitChar);		
		s1.Append(Utils.SaveString(trig.target,"target"));
		s1.Append(Utils.splitChar);		
		s1.Append(Utils.SaveString(trig.argvalue,"argvalue"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		Trigger trig = go.GetComponent<Trigger>();
		trig.allDone = Utils.GetBoolFromString(entries[index],"allDone"); index++;
		trig.numPlayers = Utils.GetIntFromString(entries[index],"numPlayers"); index++;
		bool hadRecentActivator = Utils.GetBoolFromString(entries[index],"hadRecentActivator"); index++;
		if (hadRecentActivator) trig.recentMostActivator = PlayerMovement.a.gameObject;
		trig.delayFireFinished = Utils.LoadRelativeTimeDifferential(entries[index],"delayFireFinished"); index++;
		trig.delayResetFinished = Utils.LoadRelativeTimeDifferential(entries[index],"delayResetFinished"); index++;
		trig.delay = Utils.GetFloatFromString(entries[index],"delay"); index++;
		trig.onlyOnce = Utils.GetBoolFromString(entries[index],"onlyOnce"); index++;
		trig.ignoreSecondaryTriggers = Utils.GetBoolFromString(entries[index],"ignoreSecondaryTriggers"); index++;
		trig.target = Utils.LoadString(entries[index],"target"); index++;
		trig.argvalue = Utils.LoadString(entries[index],"argvalue"); index++;
		return index;
	}
}
