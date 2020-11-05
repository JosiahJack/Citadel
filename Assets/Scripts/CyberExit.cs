using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberExit : MonoBehaviour {
	void  OnTriggerEnter (Collider col) {
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
	}
}
