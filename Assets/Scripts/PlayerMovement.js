#pragma strict

var walkAcceleration : float = 1500;
var walkDeacceleration : float = 0.15;
var walkAccelAirRatio : float = 0.25;
var verticalAcceleration : float = 1200;
@HideInInspector
var walkDeaccelerationVolx : float;
@HideInInspector
var walkDeaccelerationVolz : float;

var cameraObject : GameObject;
var maxWalkSpeed : float = 5;
var maxClimbSpeed : float = 5;
@HideInInspector
var horizontalMovement : Vector2;
@HideInInspector
var verticalMovement : float;
var jumpVelocity : float = 200;
@HideInInspector
var grounded : boolean = false;
var maxSlope : float = 60;

var crouchRatio : float = 0.3;
var transitionToCrouchSec : float = 0.2;
var crouchingVelocity : float;
var currentCrouchRatio : float = 1;
var originalLocalScaleY : float;
var crouchLocalScaleY : float;
var bodyState : float = 0;
var bodyCollisionDetection : GameObject;
var playerGravity : float = 500;
@HideInInspector
var ladderState : boolean = false;

function Awake () {
	currentCrouchRatio = 1;
	originalLocalScaleY = transform.localScale.y;
	crouchLocalScaleY = transform.localScale.y * crouchRatio;
	//GetComponent.<Rigidbody>().useGravity = false;
}

function Update () {
	// Crouch
	//if (crouchLocalScaleY > originalLocalScaleY)
	//	transform.localPosition.y = (transform.localPosition.y - 0.5);
		
	//transform.localScale.y = Mathf.Lerp(crouchLocalScaleY,originalLocalScaleY,currentCrouchRatio);

	// Body states:
	// 0 = Standing
	// 1 = Crouched
	// 2 = Crouching down in process
	// 3 = Standing up in process
	// 4 = Prone
	// 5 = Proning down
	// 6 = Proning up to crouch in process

	//if (Input.GetButtonDown("Crouch")) {
	//	if (bodyState == 1) {
	//		if (bodyCollisionDetection.GetComponent(BodyState).collisionDetected == false) {
	//			bodyState = 3;
	//		}
	//	} else {
	//		bodyState = 2;
	//	}
	//}
	
	//if (bodyState == 2)
	//	currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, 0, crouchingVelocity, transitionToCrouchSec);
		
	//if (bodyState == 3 && (bodyCollisionDetection.GetComponent(BodyState).collisionDetected == false))
	//	currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, 1, crouchingVelocity, transitionToCrouchSec);
	
	//if ((currentCrouchRatio < 1) && (bodyState == 3) && (currentCrouchRatio > 0.99)) {
	//	currentCrouchRatio = 1;
	//	bodyState = 0;
	//} else {
	//	if ((currentCrouchRatio < 0.5) && (bodyState == 2) && (currentCrouchRatio > crouchRatio)) {
	//		currentCrouchRatio = crouchRatio;
	//		bodyState = 1;
	//	}
	//}
		
	// Maximize movement speed
	horizontalMovement = Vector2(GetComponent.<Rigidbody>().velocity.x, GetComponent.<Rigidbody>().velocity.z);
	if (horizontalMovement.magnitude > maxWalkSpeed) {
		horizontalMovement = horizontalMovement.normalized;
		horizontalMovement *= maxWalkSpeed;
	}
	verticalMovement = GetComponent.<Rigidbody>().velocity.y;
	if (verticalMovement > maxClimbSpeed)
		verticalMovement = maxClimbSpeed;
		
	GetComponent.<Rigidbody>().velocity.x = horizontalMovement.x;
	GetComponent.<Rigidbody>().velocity.z = horizontalMovement.y;
	GetComponent.<Rigidbody>().velocity.y = verticalMovement;
	// End maximization of movement speed
	
	if (grounded) {
		GetComponent.<Rigidbody>().velocity.x = Mathf.SmoothDamp(GetComponent.<Rigidbody>().velocity.x, 0, walkDeaccelerationVolx, walkDeacceleration);
		GetComponent.<Rigidbody>().velocity.z = Mathf.SmoothDamp(GetComponent.<Rigidbody>().velocity.z, 0, walkDeaccelerationVolz, walkDeacceleration);	
	}
	
	transform.rotation = Quaternion.Euler(0, cameraObject.GetComponent(MouseLookScript).currentYRotation,0); //Change 0 values for x and z for use in Cyberspace
	
	if (grounded == true) {
		if (ladderState) {
			GetComponent.<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * Time.deltaTime, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime, 0);
		} else {
			GetComponent.<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime);
		}
	} else {
		if (ladderState) {
			GetComponent.<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio * Time.deltaTime, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime, 0);
		} else {
			GetComponent.<Rigidbody>().AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio * Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * walkAccelAirRatio * Time.deltaTime);
		}
	}
	
	// Gravity
	if (ladderState) {
		GetComponent.<Rigidbody>().useGravity = false;
		GetComponent.<Rigidbody>().velocity.y = Mathf.SmoothDamp(GetComponent.<Rigidbody>().velocity.y, 0, walkDeaccelerationVolz, walkDeacceleration);  //Set vertical movement towards 0
	} else {
		GetComponent.<Rigidbody>().useGravity = true;
		//GetComponent.<Rigidbody>().AddRelativeForce(0, (-1 * playerGravity * Time.deltaTime), 0); //Apply gravity force
	}

	if (Input.GetKeyDown(KeyCode.Space) && grounded)
		GetComponent.<Rigidbody>().AddForce(transform.up*jumpVelocity);	//GetComponent.<Rigidbody>().AddRelativeForce(0,jumpVelocity,0);
}

function OnCollisionStay (collision : Collision) {

	for (var contact : ContactPoint in collision.contacts) {
		if (Vector3.Angle(contact.normal,Vector3.up) < maxSlope)
			grounded = true;
	}

}

function OnCollisionExit () {
	grounded = false;
}