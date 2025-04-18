﻿using UnityEngine;
using System.Collections;
using System.Text;

public class LogicRelay : MonoBehaviour {
	public string target;
	public string argvalue;
	public bool thisTioOverridesSender = true;
	public float delay = 0f;
	public bool relayEnabled = true; // save
	public bool onceEver = false; // save
	[HideInInspector] public bool alreadyDone = false; // save
	private UseData tempUd;
	private static StringBuilder s1 = new StringBuilder();

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
		if (onceEver && alreadyDone) return;

		ud.argvalue = argvalue;
		if (thisTioOverridesSender) {
			TargetIO tio = GetComponent<TargetIO>();
			if (tio != null) {
				ud.SetBits(tio);
			} else {
				Debug.Log("BUG: no TargetIO.cs found on an object with a "
						  + "LogicRelay.cs script!  Trying to call UseTargets"
						  + " without parameters!");
			}
		}

		Const.a.UseTargets(null,ud,target);
		if (onceEver) alreadyDone = true;
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

		s1.Clear();
		s1.Append(Utils.SaveString(lr.target,"target"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveString(lr.argvalue,"argvalue"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(lr.relayEnabled,"relayEnabled"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(lr.onceEver,"onceEver"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(lr.alreadyDone,"alreadyDone"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		LogicRelay lr = go.GetComponent<LogicRelay>(); // Similar to the LB, also a handy L shaped junction box complete with a lid for easy wire pulling.  This time, the lid is to the Right instead of the Back, hence the R of LR.
		if (lr == null) {
			Debug.Log("LogicRelay.Load failure, lr == null");
			return index + 1;
		}

		if (index < 0) {
			Debug.Log("LogicRelay.Load failure, index < 0");
			return index + 1;
		}

		if (entries == null) {
			Debug.Log("LogicRelay.Load failure, entries == null");
			return index + 1;
		}

		lr.target = Utils.LoadString(entries[index],"target");
		index++;

		lr.argvalue = Utils.LoadString(entries[index],"argvalue");
		index++;

		lr.relayEnabled = Utils.GetBoolFromString(entries[index],"relayEnabled");
		index++;

		lr.onceEver = Utils.GetBoolFromString(entries[index],"onceEver");
		index++;

		lr.alreadyDone = Utils.GetBoolFromString(entries[index],"alreadyDone");
		index++;
		return index;
	}
}
