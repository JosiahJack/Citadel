using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radiation : MonoBehaviour {
	public float radiationAmount = 11f;

	private float intervalTime = 1f;
	private float radFinished = 0f;

	void Start() {
		radFinished = PauseScript.a.relativeTime + (intervalTime * 2);
	}

	void OnTriggerEnter (Collider col) {
		if (col.gameObject.CompareTag("Player")) {
			if (PlayerHealth.a.hm.health > 0f) {
				PlayerHealth.a.radiationArea = true;
				PlayerHealth.a.GiveRadiation(radiationAmount);
				radFinished = PauseScript.a.relativeTime + (intervalTime*Random.Range(1f,1.5f));
			}
		}
	}

	void  OnTriggerStay (Collider col) {
		if (col.gameObject.CompareTag("Player")) {
			if (PlayerHealth.a.hm.health > 0f && (radFinished < PauseScript.a.relativeTime)) {
				PlayerHealth.a.radiationArea = true;
				PlayerHealth.a.GiveRadiation(radiationAmount);
				radFinished = PauseScript.a.relativeTime + (intervalTime*Random.Range(1f,1.5f));
			}
		}
	}

	void OnTriggerExit (Collider col) {
		if (col.gameObject.CompareTag("Player")) { 
			if (PlayerHealth.a.hm.health > 0f) {
				PlayerHealth.a.radiationArea = false;
				radFinished = PauseScript.a.relativeTime;  // reset so re-triggering is instant
			}
		}
	}
}
