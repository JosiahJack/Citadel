using UnityEngine;
using System.Collections;

// For security cameras
public class SecurityCameraRotate : MonoBehaviour {
	public float startYAngle = 0f;
	public float endYAngle = 180f;
	private float degreesYPerSecond = 4f;
	public float waitTime = 0.8f;
	private float waitingTime = 0f;
	private bool rotatePositive;
	private float tickTime = 0.1f;
	[HideInInspector]
	public bool active;
	public MeshRenderer mR;

	void Start () {
		waitingTime = PauseScript.a.relativeTime;
		rotatePositive = true;
		if (this.enabled) active = true; else active = false;
		if (mR == null) mR = gameObject.GetComponentInChildren<MeshRenderer>();
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (mR != null) {
				if (!mR.isVisible) return;
			} else {
				mR = gameObject.GetComponentInChildren<MeshRenderer>();
				return;
			}

			if (waitingTime < PauseScript.a.relativeTime) {
				if (rotatePositive) {
					RotatePositive();
				} else {
					RotateNegative();
				}
			}
		}
	}

	void RotatePositive () {
		if (((transform.rotation.eulerAngles.y + 1f) >= endYAngle) && ((transform.rotation.eulerAngles.y - 1f) <= endYAngle)) {
			rotatePositive = false;
			waitingTime = PauseScript.a.relativeTime + waitTime;
			return;
		}
		transform.Rotate(new Vector3(0,degreesYPerSecond * tickTime,0),Space.World);
	}

	void RotateNegative () {
		if (((transform.rotation.eulerAngles.y + 1f) >= startYAngle) && ((transform.rotation.eulerAngles.y - 1f) <= startYAngle)) {
			rotatePositive = true;
			waitingTime = PauseScript.a.relativeTime + waitTime;
			return;
		}
		transform.Rotate(new Vector3(0,degreesYPerSecond * tickTime * -1,0),Space.World);
	}
}
