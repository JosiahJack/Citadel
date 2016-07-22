using UnityEngine;
using System.Collections;

// For security cameras
public class CameraRotate : MonoBehaviour {
	public Transform target1;
	public Transform target2;
	public float degreesYPerSecond = 5f;
	public bool rotatePositive = true;
	public float startAngle = 0f;
	public float endAngle = -1f;
	public float waitTime = 0.8f;
	public float tickTime = 0.1f;
	private float rotateTime = 0f;
	private float waitingTime = 0f;
	private Transform target;

	void Awake () {
		rotateTime = Time.time + tickTime;
		waitingTime = Time.time;
		target = target1;
	}

	void Update () {
		if (rotateTime < Time.time && waitingTime < Time.time) {
			if (transform.rotation == target.rotation) {
				if (target.rotation == target1.rotation)
					target = target2;
				else
					target = target1;
			}
			transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, degreesYPerSecond * tickTime);
			//if (rotatePositive) {
			//	RotatePositive();
			//} else {
			//	RotateNegative();
				//waitingTime = Time.time + waitTime;
			//}
		}
	}

	void RotatePositive () {
		//if ((transform.rotation.y + tickTime) > 360f) {
		//	float xRotation = transform.rotation.x;
		//	float yRotation = 0f;
		//	float zRotation = transform.rotation.z;
			//transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
		//}
		transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, degreesYPerSecond * tickTime);
		//transform.Rotate(new Vector3(0,degreesYPerSecond * tickTime,0),Space.World);
	}

	void RotateNegative () {
		//if ((transform.rotation.y + tickTime) < 0f) {
		//	float xRotation = transform.rotation.x;
		//	float yRotation = 360f;
		//	float zRotation = transform.rotation.z;
			//transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
		//}
		transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, degreesYPerSecond * tickTime);
		//transform.Rotate(new Vector3(0,degreesYPerSecond * tickTime * -1,0),Space.World);
	}
}
