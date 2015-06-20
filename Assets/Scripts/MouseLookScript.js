#pragma strict

var lookSensitivity : float = 5;
@HideInInspector
var yRotation : float;
@HideInInspector
var xRotation : float;
@HideInInspector
var zRotation : float;
@HideInInspector
var currentYRotation : float;
@HideInInspector
var currentXRotation : float;
@HideInInspector
var currentZRotation : float;
@HideInInspector
var yRotationV : float;
@HideInInspector
var xRotationV : float;
var lookSmoothDamp : float = 0.1;
@HideInInspector
var zRotationV : float;

//var headbobSpeed : float = 1;
//var headbobStepCounter : float;
//var headbobAmountX : float = 1;
//var headbobAmountY : float = 1;
//var parentLastPos : Vector3;
//var eyeHeightRatio : float = 0.9;

//function Awake () {
	//parentLastPos = transform.parent.position;
//}

function Update () {
	//if (transform.parent.GetComponent(PlayerMovement).grounded)
		//headbobStepCounter += (Vector3.Distance(parentLastPos, transform.parent.position) * headbobSpeed);
	
	//transform.localPosition.x = (Mathf.Sin(headbobStepCounter) * headbobAmountX);
	//transform.localPosition.y = (Mathf.Cos(headbobStepCounter * 2) * headbobAmountY * -1) + (transform.localScale.y * eyeHeightRatio) - (transform.localScale.y / 2);
	//parentLastPos = transform.parent.position;

	yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
	xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
	
	xRotation = Mathf.Clamp(xRotation, -90, 90);  // Prevents you from looking up too far and breaking your neck.  Need to disable for Cyberspace!
	
	currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, xRotationV, lookSmoothDamp);  // Not really necessary, helps with smoothness
	currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, yRotationV, lookSmoothDamp);  // "  "   "
	
	transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);  // If removing the currentXRotation calls above, remove "current" in arguments
}