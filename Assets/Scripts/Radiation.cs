using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radiation : MonoBehaviour {
	public float intervalTime = 1f;
	public float radiationAmount = 11f;
	public float radFinished = 0f;
	public int numPlayers = 0;
	public bool isEnabled = true;

	void OnTriggerEnter (Collider col) {
		if (!isEnabled) return;
		if (col.gameObject.CompareTag("Player")) {
			PlayerHealth ph = col.gameObject.GetComponent<PlayerHealth>();
			if (ph != null) {
				if (ph.hm.health > 0f) {
					numPlayers++;
					ph.radiationArea = true;
					ph.GiveRadiation(radiationAmount);
					radFinished = PauseScript.a.relativeTime + (intervalTime*Random.Range(1f,1.5f));
				}
			}
		}
	}

	void  OnTriggerStay (Collider col) {
		if (!isEnabled) return;
		if (col.gameObject.CompareTag("Player")) {
			PlayerHealth ph = col.gameObject.GetComponent<PlayerHealth>();
			if (ph != null) {
				if (ph.hm.health > 0f && (radFinished < PauseScript.a.relativeTime)) {
					ph.radiationArea = true;
					ph.GiveRadiation(radiationAmount);
					radFinished = PauseScript.a.relativeTime + (intervalTime*Random.Range(1f,1.5f));
				}
			}
		}
	}

	void OnTriggerExit (Collider col) {
		if (!isEnabled) return;
		if (col.gameObject.CompareTag("Player")) { 
			PlayerHealth ph = col.gameObject.GetComponent<PlayerHealth>();
			if (ph != null) {
				if (ph.hm.health > 0f) {
					ph.radiationArea = false;
					numPlayers--;
					if (numPlayers == 0) radFinished = PauseScript.a.relativeTime;  // reset so re-triggering is instant
				}
			}
		}
	}
}
