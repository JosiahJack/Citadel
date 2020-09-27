using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberSwitch : MonoBehaviour {
	public int textIndex = 463;
	public bool active = false; // save
	public string target;
	public string argvalue;
	public GameObject activeCenter;
	public GameObject deactiveCenter;

	void Awake() {
		if (active) {
			deactiveCenter.SetActive(false);
			activeCenter.SetActive(true);
		} else {
			deactiveCenter.SetActive(true);
			activeCenter.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Player")) {
			MFDManager.a.CyberSprint(Const.a.stringTable[textIndex]);
			active = true;
			deactiveCenter.SetActive(false);
			activeCenter.SetActive(true);

			UseData ud = new UseData();
			ud.owner = other.gameObject;
			ud.argvalue = argvalue;
			TargetIO tio = GetComponent<TargetIO>();
			if (tio != null) {
				ud.SetBits(tio);
			} else {
				Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
			}
			Const.a.UseTargets(ud,target);
		}
	}
}
