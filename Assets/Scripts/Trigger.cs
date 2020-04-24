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
	private GameObject recentMostActivator;
	private float delayFireFinished;
	private float delayResetFinished;
	private AudioSource SFX;
	public AudioClip SFXClip;
	public bool playAudioOnTrigger = false;
	private bool allDone = false;

	void Start () {
		SFX = GetComponent<AudioSource>();
	}


    IEnumerator DelayedTarget(GameObject activator) {
        yield return new WaitForSeconds(delay);
        UseTargets(activator);
    }

	public void UseTargets (GameObject activator) {
		UseData ud = new UseData();
		ud.owner = activator;
		ud.argvalue = argvalue;
		TargetIO tio = GetComponent<TargetIO>();
		if (tio != null) {
			ud.SetBits(tio);
		} else {
			Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
		}

		Const.a.UseTargets(ud,target);

		if (playAudioOnTrigger && SFX != null && SFXClip != null) {
			SFX.PlayOneShot(SFXClip);
		}
	}

	void TriggerTripped (Collider col, bool initialEntry) {
		if (col == null) Debug.Log("TriggerTripped was fed a null col!");

		if (col.gameObject.tag == "Player") {
			HealthManager hm = col.gameObject.GetComponent<HealthManager>();
			if (hm != null) {
				if (hm.health > 0f && hm.isPlayer) {
					if (recentMostActivator != null) {
						if (ignoreSecondaryTriggers) return;
					}
					recentMostActivator = col.gameObject;

					if (initialEntry && recentMostActivator.tag == "Player") numPlayers++;
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
		if (col.gameObject.tag == "Player")
			TriggerTripped(col,true);
	}

	void  OnTriggerStay (Collider col) {
		if (allDone) return;
		if (col == null) return;
		if (col.gameObject == null) return;
		if (col.gameObject.tag == "Player")
			TriggerTripped (col, false);
	}

	void OnTriggerExit (Collider col) {
		if (allDone) return;
		if (col.gameObject.tag == "Player") numPlayers--;
	}

	public void Targetted (UseData ud) {
		if (ignoreSecondaryTriggers) recentMostActivator = ud.owner;
		//TriggerTripped (Collider col, bool initialEntry);
	}
}
