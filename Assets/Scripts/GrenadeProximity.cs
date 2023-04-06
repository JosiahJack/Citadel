using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProximity : MonoBehaviour {
	public GrenadeActivate ga;

	void OnTriggerEnter(Collider col) {
		if (col.transform.gameObject.CompareTag("Player")) {
			ga.proxSensed = true;
		}

		if (!col.transform.gameObject.CompareTag("NPC")) return;

		if (col.transform.gameObject.GetComponent<AIController>() != null) {
			ga.proxSensed = true;
		}
	}

	void OnTriggerExit(Collider col) {
		ga.proxSensed = false;
	}
}
