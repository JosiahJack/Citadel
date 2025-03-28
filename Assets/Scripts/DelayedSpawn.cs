using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DelayedSpawn : MonoBehaviour {
    public float delay = 0.5f; // save
	public GameObject[] objectsToSpawn;
	public bool despawnInstead = false; // save
	public bool doSelfAfterList = false; // save
	public bool destroyAfterListInsteadOfDeactivate = false; // save
	[HideInInspector] public float timerFinished; // save
	[HideInInspector] public bool active; // save
	private static StringBuilder s1 = new StringBuilder();

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
		s1.Clear();
		s1.Append(Utils.FloatToString(ds.delay,"delay"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(ds.timerFinished,"timerFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ds.active,"active"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ds.despawnInstead,"despawnInstead"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ds.doSelfAfterList,"doSelfAfterList"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ds.destroyAfterListInsteadOfDeactivate,"destroyAfterListInsteadOfDeactivate"));
		for (int i=0;i<ds.objectsToSpawn.Length;i++) {
			s1.Append(Utils.splitChar);
			s1.Append(Utils.SaveSubActivatedGOState(ds.objectsToSpawn[i]));
		}

		return s1.ToString();
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

		ds.delay = Utils.GetFloatFromString(entries[index],"delay"); index++;
		ds.timerFinished = Utils.LoadRelativeTimeDifferential(entries[index],"timerFinished"); index++;
		ds.active = Utils.GetBoolFromString(entries[index],"active"); index++;
		ds.despawnInstead = Utils.GetBoolFromString(entries[index],"despawnInstead"); index++;
		ds.doSelfAfterList = Utils.GetBoolFromString(entries[index],"doSelfAfterList"); index++;
		ds.destroyAfterListInsteadOfDeactivate = Utils.GetBoolFromString(entries[index],"destroyAfterListInsteadOfDeactivate"); index++;
		for (int i=0; i<ds.objectsToSpawn.Length; i++) {
			index = Utils.LoadSubActivatedGOState(ds.objectsToSpawn[i],ref entries,index);
		}

		return index;
	}
}
