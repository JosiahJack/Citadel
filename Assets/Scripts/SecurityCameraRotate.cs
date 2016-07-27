using UnityEngine;
using System.Collections;

// For security cameras
public class SecurityCameraRotate : MonoBehaviour {
	public float startYAngle = 0f;
	public float endYAngle = 180f;
	public float degreesYPerSecond = 5f;
	public float waitTime = 0.8f;
	private float waitingTime = 0f;
	private bool rotatePositive;
	private float tickTime = 0.1f;

	void Awake () {
		waitingTime = Time.time;
		rotatePositive = true;
	}

	void Update () {
		if (waitingTime < Time.time) {
			if (rotatePositive) {
				RotatePositive();
			} else {
				RotateNegative();
			}
		}
	}

	void RotatePositive () {
		if (((transform.rotation.eulerAngles.y + 1f) >= endYAngle) && ((transform.rotation.eulerAngles.y - 1f) <= endYAngle)) {
			rotatePositive = false;
			waitingTime = Time.time + waitTime;
			return;
		}
		transform.Rotate(new Vector3(0,degreesYPerSecond * tickTime,0),Space.World);
	}

	void RotateNegative () {
		if (((transform.rotation.eulerAngles.y + 1f) >= startYAngle) && ((transform.rotation.eulerAngles.y - 1f) <= startYAngle)) {
			rotatePositive = true;
			waitingTime = Time.time + waitTime;
			return;
		}
		transform.Rotate(new Vector3(0,degreesYPerSecond * tickTime * -1,0),Space.World);
	}
}
