using UnityEngine;
using System.Collections;

public class RigidBodyKeepAwakeScript : MonoBehaviour {
	private Rigidbody rbody;

	void Awake () { rbody = GetComponent<Rigidbody>(); }
	void FixedUpdate () {
		if (rbody != null && PauseScript.a != null && !PauseScript.a.paused)
			rbody.WakeUp();
	}
}
