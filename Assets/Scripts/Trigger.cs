using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {
	public float delayBeforeFire = 0f;
	public float delayBeforeReset = 1.0f;
	public float initialDelayOnAwake = 0f;
	public bool allowNPC = false;
	public bool ignoreSecondaryTriggers = false;
	public int numPlayers = 0;
	public int numNPCs = 0;
	public GameObject target;
	public GameObject target1;
	public GameObject target2;
	public GameObject target3;
	private GameObject recentMostActivator;
	private float delayFireFinished;
	private float delayResetFinished;
	public bool firing;

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
	}

	void Update () {
		if (firing) {
			if (delayFireFinished < Time.time) {
				UseTargets(recentMostActivator);
				recentMostActivator = null;
				firing = false;
			}
		}
	}

	public void UseTargets (GameObject activator) {
		UseData ud = new UseData();
		ud.owner = activator;

		if (target != null) {
			target.SendMessageUpwards("Targetted", ud);
		}
		if (target1 != null) {
			target1.SendMessageUpwards("Targetted", ud);
		}
		if (target2 != null) {
			target2.SendMessageUpwards("Targetted", ud);
		}
		if (target3 != null) {
			target3.SendMessageUpwards("Targetted", ud);
		}
	}

	void TriggerTripped (Collider col, bool initialEntry) {
		if (((col.gameObject.tag == "Player") && (col.gameObject.GetComponent<PlayerHealth>().hm.health > 0f)) || (allowNPC && (col.gameObject.tag == "NPC") && (col.gameObject.GetComponent<HealthManager>().health > 0f)) ) {
			if (delayResetFinished < Time.time) {
				if (recentMostActivator != null) {
					if (ignoreSecondaryTriggers)
						return;
				}
				if (initialEntry && col.gameObject.tag == "Player") numPlayers++;
				if (initialEntry && col.gameObject.tag == "NPC") numNPCs++;
				delayFireFinished = Time.time + delayBeforeFire;
				delayResetFinished = Time.time + delayBeforeReset;
				firing = true;
			}
		}

	}
	void OnTriggerEnter (Collider col) {
		TriggerTripped(col,true);
	}

	void  OnTriggerStay (Collider col) {
		TriggerTripped (col, false);
	}

	void OnTriggerExit (Collider col) {
		if (col.gameObject.tag == "Player") numPlayers--;
		if (col.gameObject.tag == "NPC") numNPCs--;
	}
}
