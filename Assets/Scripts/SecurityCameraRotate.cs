﻿using UnityEngine;
using System.Collections;

// Rotates a security camera back and forth between two angle values, pausing
// at each angle value for an instance-set amount of time.  This assumes that
// the current transform is either the camera directly or a wrapper GameObject
// that contains a child whose drooped angle will be preserved while this
// parent transform rotates along true up/down axis.
public class SecurityCameraRotate : MonoBehaviour {
	public float startYAngle = 0f;
	public float endYAngle = 180f;
	public float waitTime = 0.8f;
	public MeshRenderer mR;

	[HideInInspector] public bool active;
	[HideInInspector] public bool rotatePositive; // save
	private float degreesYPerSecond = 4f;
	private float waitingFinished = 0f;
	private float tickTime = 0.1f;

	void Start () {
		waitingFinished = PauseScript.a.relativeTime;
		rotatePositive = true;
		if (this.enabled) active = true;
		else active = false;

		if (mR == null) {
			mR = gameObject.GetComponentInChildren<MeshRenderer>(true);
		}
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (mR != null) {
				if (!mR.isVisible || !mR.enabled) return;
			} else {
				mR = gameObject.GetComponentInChildren<MeshRenderer>(true);
				return;
			}

			if (waitingFinished < PauseScript.a.relativeTime) {
				if (rotatePositive) RotatePositive();
				else                RotateNegative();
			}
		}
	}

	void RotatePositive () {
		if (((transform.rotation.eulerAngles.y + 1f) >= endYAngle)
			&& ((transform.rotation.eulerAngles.y - 1f) <= endYAngle)) {
			rotatePositive = false;
			waitingFinished = PauseScript.a.relativeTime + waitTime;
			return;
		}
		
		transform.Rotate(new Vector3(0,degreesYPerSecond * tickTime,0),
						 Space.World);
	}

	void RotateNegative () {
		if (((transform.rotation.eulerAngles.y + 1f) >= startYAngle)
			&& ((transform.rotation.eulerAngles.y - 1f) <= startYAngle)) {
			rotatePositive = true;
			waitingFinished = PauseScript.a.relativeTime + waitTime;
			return;
		}
		
		transform.Rotate(new Vector3(0,degreesYPerSecond * tickTime * -1,0),
						 Space.World);
	}

	public static string Save(GameObject go) {
		SecurityCameraRotate scr = go.GetComponent<SecurityCameraRotate>();
		string line = System.String.Empty;
		line = Utils.SaveRelativeTimeDifferential(scr.waitingFinished,"waitingFinished");
		line += Utils.splitChar + Utils.BoolToString(scr.enabled,"enabled");
		line += Utils.splitChar + Utils.FloatToString(scr.startYAngle,"startYAngle");
		line += Utils.splitChar + Utils.FloatToString(scr.endYAngle,"endYAngle");
		line += Utils.splitChar + Utils.FloatToString(scr.waitTime,"waitTime");
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		SecurityCameraRotate scr = go.GetComponent<SecurityCameraRotate>();
		scr.waitingFinished = Utils.LoadRelativeTimeDifferential(entries[index],"waitingFinished"); index++;
		scr.enabled = Utils.GetBoolFromString(entries[index],"enabled"); index++;
		scr.startYAngle = Utils.GetFloatFromString(entries[index],"startYAngle"); index++;
		scr.endYAngle = Utils.GetFloatFromString(entries[index],"endYAngle"); index++;
		scr.waitTime = Utils.GetFloatFromString(entries[index],"waitTime"); index++;
		return index;
	}
}
