using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour {
	private Camera cam;
	private float tick;
	private float tickFinished;
	private bool frameOn;

	void Awake () {
		cam = GetComponent<Camera>();
		tick = 0.5f;
		tickFinished = Time.time + tick;
		frameOn = false;
	}

	// Update is called once per frame
	void Update () {
		if (tickFinished < Time.time) {
			cam.enabled = true;
			frameOn = true;
			tickFinished = Time.time + tick;
		}
	}

	void LateUpdate() {
		if (frameOn) cam.enabled = false; // Render to texture for 1 frame only!
	}
}
