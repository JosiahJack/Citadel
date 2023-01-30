using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedSpawn : MonoBehaviour {
    public float delay = 0.5f; // save
	public GameObject[] objectsToSpawn;
	public bool despawnInstead = false; // save
	public bool doSelfAfterList = false; // save
	public bool destroyAfterListInsteadOfDeactivate = false; // save
	[HideInInspector] public float timerFinished; // save
	[HideInInspector] public bool active; // save

	void OnEnable() {
		if (PauseScript.a != null) timerFinished = PauseScript.a.relativeTime + delay;
        else timerFinished = delay;

		active = true;
    }

	void Update() {
		if (!active) return;
		if (timerFinished >= PauseScript.a.relativeTime) return;

		active = false; // Once only, unless we do self after the list.
		for (int i=0;i<objectsToSpawn.Length;i++) {
			if (despawnInstead) {
				if (objectsToSpawn[i] != null) objectsToSpawn[i].SetActive(false);
			} else {
				if (objectsToSpawn[i] != null) objectsToSpawn[i].SetActive(true);
			}
		}

		if (doSelfAfterList) {
			if (despawnInstead) {
				if (destroyAfterListInsteadOfDeactivate) {
					Utils.SafeDestroy(gameObject);
				} else {
					gameObject.SetActive(false);
				}
			} else {
				gameObject.SetActive(true);
			}
		}
    }

	public static string Save(GameObject go) {
		DelayedSpawn ds = go.GetComponent<DelayedSpawn>();
		if (ds == null) {
			Debug.Log("DelayedSpawn missing on savetype of DelayedSpawn!  GameObject.name: " + go.name);
			return Utils.DTypeWordToSaveString("ffbbbbu");

		}

		string line = System.String.Empty;
		line = Utils.FloatToString(ds.delay);
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(ds.timerFinished);
		line += Utils.splitChar + Utils.BoolToString(ds.active);
		line += Utils.splitChar + Utils.BoolToString(ds.despawnInstead);
		line += Utils.splitChar + Utils.BoolToString(ds.doSelfAfterList);
		line += Utils.splitChar + Utils.BoolToString(ds.destroyAfterListInsteadOfDeactivate);
		for (int i=0;i<ds.objectsToSpawn.Length;i++) {
			line += Utils.splitChar + Utils.SaveSubActivatedGOState(ds.objectsToSpawn[i]);
		}

		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		DelayedSpawn ds = go.GetComponent<DelayedSpawn>();
		if (ds == null) {
			Debug.Log("DelayedSpawn.Load failure, ds == null");
			return index + 6;
		}

		if (index < 0) {
			Debug.Log("DelayedSpawn.Load failure, index < 0");
			return index + 6;
		}

		if (entries == null) {
			Debug.Log("DelayedSpawn.Load failure, entries == null");
			return index + 6;
		}

		ds.delay = Utils.GetFloatFromString(entries[index]); index++;
		ds.timerFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		ds.active = Utils.GetBoolFromString(entries[index]); index++;
		ds.despawnInstead = Utils.GetBoolFromString(entries[index]); index++;
		ds.doSelfAfterList = Utils.GetBoolFromString(entries[index]); index++;
		ds.destroyAfterListInsteadOfDeactivate = Utils.GetBoolFromString(entries[index]); index++;
		for (int i=0; i<ds.objectsToSpawn.Length; i++) {
			index = Utils.LoadSubActivatedGOState(ds.objectsToSpawn[i],ref entries,index);
		}

		return index;
	}
}
