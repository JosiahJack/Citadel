using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberExit : MonoBehaviour {
	public float tick = 0.1f;
	private float tickFinished;

	void Awake() {
		tickFinished = PauseScript.a.relativeTime + 2f;
	}

	void  OnTriggerEnter (Collider col) {
		if (tickFinished < PauseScript.a.relativeTime) {
			if (col.gameObject.CompareTag("Player")) {
				PlayerMovement pm = col.gameObject.GetComponent<PlayerMovement>();
				if (pm != null) {
					MouseLookScript mls = pm.mlookScript;
					if (mls != null) {
						mls.ExitCyberspace();
					} else {
						Debug.Log("BUG: Missing MouseLookScript on owner's pm for CyberExit");
					}
				}
			}
			tickFinished = PauseScript.a.relativeTime + tick;
		}
	}
}
