﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyRotate : MonoBehaviour {
	public float rotateSpeed = 0.015f;
	public float defaultSpeed = 0.015f;
	private float timeIncrement = 0.016f;
	private float nextThink;
	private Vector3 rot;

	// Start instead of Awake to allow PauseScript to initialize.
	void Start() {
		nextThink = PauseScript.a.relativeTime + timeIncrement;
		rot = new Vector3(0,rotateSpeed,0);
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (nextThink < PauseScript.a.relativeTime) transform.Rotate(new Vector3(0,rotateSpeed,0));
		}
	}
}
