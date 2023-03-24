using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberAccess : MonoBehaviour {
	public string target;
	public string argvalue;
	public GameObject entryPosition;

    public void Use (UseData ud) {
		if (!string.IsNullOrEmpty(target)) {
			ud.argvalue = argvalue;
			TargetIO tio = GetComponent<TargetIO>();
			if (tio != null) {
				ud.SetBits(tio);
			} else {
				Debug.Log("BUG: no TargetIO.cs found on an object with a "
						  + "CyberAccess.cs script!  Trying to call UseTargets"
						  + " without parameters!");
			}

			Const.a.UseTargets(ud,target);
		}

		Const.sprint(Const.a.stringTable[441]); // Entering Cyberspace!
		MouseLookScript.a.EnterCyberspace(entryPosition);
	}
}
