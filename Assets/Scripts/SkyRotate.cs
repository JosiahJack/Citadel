using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyRotate : MonoBehaviour {
	public float rotateSpeed = 0.1f;
	public float timeIncrement = 0.05f;
	private float nextThink;

	void Awake() {
		nextThink = Time.time + timeIncrement;
	}

	void Update () {
		if (nextThink < Time.time) {
			Vector3 rot = new Vector3(0,rotateSpeed,0);
			transform.Rotate(rot);
		}
	}
}
