using System.Collections;
using System.Collections.Generic;
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
			HealthManager hm = col.gameObject.GetComponent<HealthManager>();
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
		if (trig == null) {
			Debug.Log("Trigger missing on trigger GameObject.name: " + go.name);
			return Utils.DTypeWordToSaveString("bibfffbbss");
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(trig.allDone);
		line += Utils.splitChar + Utils.IntToString(trig.numPlayers);
		if (trig.recentMostActivator != null) line += Utils.splitChar + "1";
		else line += Utils.splitChar + "0";
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(trig.delayFireFinished);
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(trig.delayResetFinished);
		line += Utils.splitChar + Utils.FloatToString(trig.delay);
		line += Utils.splitChar + Utils.BoolToString(trig.onlyOnce);
		line += Utils.splitChar + Utils.BoolToString(trig.ignoreSecondaryTriggers);
		line += Utils.splitChar + trig.target;
		line += Utils.splitChar + trig.argvalue;
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		Trigger trig = go.GetComponent<Trigger>();
		if (trig == null) {
			Debug.Log("Trigger.Load failure, trig == null");
			return index + 10;
		}

		if (index < 0) {
			Debug.Log("Trigger.Load failure, index < 0");
			return index + 10;
		}

		if (entries == null) {
			Debug.Log("Trigger.Load failure, entries == null");
			return index + 10;
		}

		trig.allDone = Utils.GetBoolFromString(entries[index]); index++;
		trig.numPlayers = Utils.GetIntFromString(entries[index]); index++;
		bool hadRecentActivator = Utils.GetBoolFromString(entries[index]); index++;
		if (hadRecentActivator) trig.recentMostActivator = PlayerMovement.a.gameObject;
		trig.delayFireFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		trig.delayResetFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		trig.delay = Utils.GetFloatFromString(entries[index]); index++;
		trig.onlyOnce = Utils.GetBoolFromString(entries[index]); index++;
		trig.ignoreSecondaryTriggers = Utils.GetBoolFromString(entries[index]); index++;
		trig.target = entries[index]; index++;
		trig.argvalue = entries[index]; index++;
		return index;
	}
}
