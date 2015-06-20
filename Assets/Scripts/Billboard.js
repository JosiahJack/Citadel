#pragma strict

var cameraObject : GameObject;
var lookPos : Vector3;
var damping : float = 1;

function Update () {
	
	lookPos = transform.position - cameraObject.transform.position;
	
	var rotationCorrection = Quaternion.LookRotation(lookPos);
	rotationCorrection *= Quaternion.Euler(0, -90, 90);
	//transform.rotation = Quaternion.Slerp(transform.rotation, rotationCorrection, Time.deltaTime * damping);
	transform.rotation = rotationCorrection;
}