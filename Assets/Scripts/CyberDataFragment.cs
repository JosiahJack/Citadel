using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberDataFragment : MonoBehaviour {
	public int textIndex = 0;

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Player")) {
			MFDManager.a.CyberSprint(Const.a.stringTable[textIndex]);
		}
	}
}
