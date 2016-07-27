using UnityEngine;
using System.Collections;

public class RigidBodyKeepAwakeScript : MonoBehaviour {
	private Rigidbody rbody;

	void Awake () { rbody = GetComponent<Rigidbody>(); }
	void Update () {
		if (rbody != null && !PauseScript.a.paused)
			rbody.WakeUp();
	}
}
