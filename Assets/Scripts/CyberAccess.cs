using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberAccess : MonoBehaviour {
	public string target;
	public string argvalue;
	public GameObject entryPosition;

    public void Use (UseData ud) {
		ud.argvalue = argvalue;
		Const.a.UseTargets(gameObject,ud,target);
		Const.sprint(Const.a.stringTable[441]); // Entering Cyberspace!
		MouseLookScript.a.EnterCyberspace(entryPosition);
	}
}
