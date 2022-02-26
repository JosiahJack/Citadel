using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour {
	// External reference, required
	public Transform screenPoint;

	// External reference, optional
	[DTValidator.Optional] public Transform screenPoint2; // Added both of these for the bridge camera, only instance of having more than one screen for the same camera.
	[DTValidator.Optional] public Transform screenPoint3;

	// Internal references
	private Camera cam;
	private float tick = 0.1f;
	private float tickFinished; // Visual only, Time.time controlled
	private MeshRenderer mR;
	private MeshRenderer mR2;
	private MeshRenderer mR3;

	void Start () {
		cam = GetComponent<Camera>();
		if (cam == null) Debug.Log("BUG: CameraView missing component for cam");
		else cam.enabled = false;
		tickFinished = Time.time + tick;
		if (screenPoint == null) Debug.Log("BUG: CameraView missing manually assigned reference for screenPoint");
		else {
			mR = screenPoint.gameObject.GetComponent<MeshRenderer>();
		}
		if (mR == null) Debug.Log("BUG: CameraView missing component for screenPoint.gameObject to assign to mR");

		if (screenPoint2 != null) mR2 = screenPoint2.gameObject.GetComponent<MeshRenderer>();
		if (screenPoint3 != null) mR3 = screenPoint3.gameObject.GetComponent<MeshRenderer>();
		if (mR2 == null) mR2 = mR;
		if (mR3 == null) mR3 = mR;
	}

	void OnEnable() {
		if (cam != null) cam.Render();
	}

	void Update() {
		if (!PauseScript.a.paused && !PauseScript.a.MenuActive()) {
			if (!mR.isVisible && !mR2.isVisible && !mR3.isVisible) return;

			if (tickFinished < Time.time) {
				tickFinished = Time.time + tick;
				cam.Render();
			}
		}
	}
}
