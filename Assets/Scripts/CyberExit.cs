using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberExit : MonoBehaviour {
	void  OnTriggerEnter (Collider col) {
		if (col.gameObject.CompareTag("Player")) {
			PlayerMovement pm = col.gameObject.GetComponent<PlayerMovement>(); // Prevent retrigger by other parts of player.
			if (pm != null) {
				MouseLookScript.a.ExitCyberspace();
			}
		}
	}
}
