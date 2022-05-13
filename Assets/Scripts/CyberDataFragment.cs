using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberDataFragment : MonoBehaviour {
	public int textIndex = 0;

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.CompareTag("Player")) {
			PlayerMovement pm = col.gameObject.GetComponent<PlayerMovement>();
			if (pm != null) {
				MFDManager.a.CyberSprint(Const.a.stringTable[textIndex]);
			}
		}
	}
}
