using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {
	public float delayBeforeFire = 0f;
	public float delayBeforeReset = 1.0f;
	public float initialDelayOnAwake = 0f;
	public bool onlyOnce = false;
	public bool npcOnly = false;
	public GameObject npcTargetGO;
	public bool ignoreSecondaryTriggers = false;
	public int numPlayers = 0;
	public int numNPCs = 0;
	public string target;
	public string argvalue; // e.g. how much to set a counter to
	private GameObject recentMostActivator;
	private float delayFireFinished;
	private float delayResetFinished;
	public bool firing;
	public bool npcUseIsNormal = false;
	private AudioSource SFX;
	public AudioClip SFXClip;
	public bool playAudioOnTrigger = false;
	private bool allDone = false;

	void Awake () {
		InitialState();
	}

	void Start () {
		InitialState();
	}

	void OnEnable() {
		InitialState();
	}

	void InitialState () {
		delayResetFinished = Time.time + initialDelayOnAwake;
		delayFireFinished = Time.time;
		firing = false;
		if (playAudioOnTrigger) {
			SFX = GetComponent<AudioSource>();
		}
	}

	void Update () {
		if (allDone) return;
		if (firing) {
			if (delayFireFinished < Time.time) {
				if (playAudioOnTrigger && SFX != null && SFXClip != null) {
					SFX.PlayOneShot(SFXClip);
				}
				UseTargets(recentMostActivator);
				recentMostActivator = null;
				firing = false;
			}
		}
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

		if (activator.tag == "NPC"){
			if (!npcUseIsNormal) {
				// Special NPC handling for trigger
				if (npcTargetGO != null) {
					npcTargetGO.GetComponent<TargetIO>().Targetted(ud); // bypass string based Const.a.UseTargets and allow for secret GO targetting by NPC's, used by door triggers
				}
				return;
			}
		}
		Const.a.UseTargets(ud,target);
	}

	void TriggerTripped (Collider col, bool initialEntry) {
		if (col == null) Debug.Log("TriggerTripped was fed a null col!");
		if (npcOnly) {
			Debug.Log("NPC Touching trigger!");
			Debug.Log("Checking if the col was an NPC");
			if (col.gameObject.tag != "NPC") return; 
			Debug.Log("It was an NPC.");
			recentMostActivator = col.gameObject;
			if (recentMostActivator.tag == "NPC" && recentMostActivator.GetComponent<HealthManager>().health > 0f) {
				numNPCs++;
				delayFireFinished = Time.time + delayBeforeFire;
				delayResetFinished = Time.time + delayBeforeReset;
				if (onlyOnce) delayResetFinished = -1;
				firing = true;
			}
			return;
		}

		if (col.gameObject.tag == "Player" && col.gameObject.tag != "NPC") {
			if (col.gameObject.GetComponent<HealthManager>().health > 0f) {
				Debug.Log("triggered a player!");
				if (delayResetFinished > -1 && delayResetFinished < Time.time) {
					if (recentMostActivator != null) {
						if (ignoreSecondaryTriggers)
							return;
					}
					recentMostActivator = col.gameObject;
					if (recentMostActivator == null) { Debug.Log("BUG: TriggerTripped had a null col.gameObject!"); return;}

					if (initialEntry && !npcOnly && recentMostActivator.tag == "Player") numPlayers++;
					delayFireFinished = Time.time + delayBeforeFire;
					delayResetFinished = Time.time + delayBeforeReset;
					if (onlyOnce) allDone = true;
					firing = true;
				}
			} else {
				//Debug.Log("Touching a dead player!");
			}
		}

	}

	void OnTriggerEnter (Collider col) {
		if (allDone) return;
		if (col == null) return;
		if (col.gameObject == null) return;
		if (col.gameObject.tag == "Player" || col.gameObject.tag == "NPC")
			TriggerTripped(col,true);
	}

	void  OnTriggerStay (Collider col) {
		if (allDone) return;
		if (col == null) return;
		if (col.gameObject == null) return;
		if (col.gameObject.tag == "Player" || col.gameObject.tag == "NPC")
		TriggerTripped (col, false);
	}

	void OnTriggerExit (Collider col) {
		if (allDone) return;
		if (col.gameObject.tag == "Player" && !npcOnly) numPlayers--;
		if (col.gameObject.tag == "NPC" && npcOnly) numNPCs--;
	}

	public void Targetted (UseData ud) {
		if (ignoreSecondaryTriggers) recentMostActivator = ud.owner; 
		firing = true;
		delayFireFinished = Time.time + delayBeforeFire;
		delayResetFinished = Time.time + delayBeforeReset;
	}
}
