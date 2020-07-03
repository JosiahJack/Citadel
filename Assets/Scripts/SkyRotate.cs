using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyRotate : MonoBehaviour {
	public float rotateSpeed = 0.1f;
	public float timeIncrement = 0.05f;
	private float nextThink;

	void Start() {
		nextThink = PauseScript.a.relativeTime + timeIncrement;
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (nextThink < PauseScript.a.relativeTime) {
				Vector3 rot = new Vector3(0,rotateSpeed,0);
				transform.Rotate(rot);
			}
		}
	}
}
