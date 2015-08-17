#pragma strict

var isCapsLockOn : boolean = false;
var walkAcceleration : float = 2000;
var walkDeacceleration : float = 0.15;
var walkAccelAirRatio : float = 0.1;
var verticalAcceleration : float = 1200;
@HideInInspector
var walkDeaccelerationVolx : float;
@HideInInspector
var walkDeaccelerationVolz : float;
var cameraObject : GameObject;
var playerSpeed : float;
var maxWalkSpeed : float = 5;
var maxCrouchSpeed : float = 3.5;
var maxProneSpeed : float = 2;
var maxSprintSpeed : float = 10;
var maxSkateSpeed : float = 12;
var isSprinting : boolean = false;
var isSkating : boolean = false;
@HideInInspector
var horizontalMovement : Vector2;
@HideInInspector
var verticalMovement : float;
@HideInInspector
var jumpTime : float;
var jumpImpulseTime : float = 2.0;
var jumpVelocity : float = 1.1;
@HideInInspector
var grounded : boolean = false;
var maxSlope : float = 60;

var crouchRatio : float = 0.55;
var proneRatio : float = 0.3;
var transitionToCrouchSec : float = 0.2;
var transitionToProneAdd : float = 0.1;
@HideInInspector
var crouchingVelocity : float = 1;
var currentCrouchRatio : float = 1;
@HideInInspector
var originalLocalScaleY : float;
var crouchLocalScaleY : float;
@HideInInspector
var lastCrouchRatio : float;
var capsuleHeight : float;
var capsuleRadius : float;
@HideInInspector
var layerGeometry : int = 9;
@HideInInspector
var layerMask : int;
@HideInInspector
var originalCapsuleHeight : float;
var bodyState : int = 0;
//var playerGravity : float = 500;
@HideInInspector
var ladderState : boolean = false;
@HideInInspector
var rbody : Rigidbody;
@HideInInspector
var mlookScript : MouseLookScript;

function Awake () {
	isCapsLockOn = false;
	currentCrouchRatio = 1;
	bodyState = 0;
	originalLocalScaleY = transform.localScale.y;
	crouchLocalScaleY = transform.localScale.y * crouchRatio;
	rbody = GetComponent.<Rigidbody>();
    //rbody.useGravity = false;
	mlookScript = cameraObject.GetComponent(MouseLookScript);
	capsuleHeight = GetComponent.<CapsuleCollider>().height;
	originalCapsuleHeight = capsuleHeight;
	capsuleRadius = GetComponent.<CapsuleCollider>().radius;
	layerMask = 1 << layerGeometry;
}

function CantStand() : boolean {
	return Physics.CheckCapsule(cameraObject.transform.position, cameraObject.transform.position + new Vector3(0f,(1.6f-0.84f),0f), capsuleRadius, layerMask);
}

function CantCrouch() : boolean {
	return Physics.CheckCapsule(cameraObject.transform.position, cameraObject.transform.position + new Vector3(0f,0.4f,0f), capsuleRadius, layerMask);
} 

function Update() {
	// Crouch input state machine
		// Body states:
		// 0 = Standing
		// 1 = Crouch
		// 2 = Crouching down in process
		// 3 = Standing up in process
		// 4 = Prone
		// 5 = Proning down in process
		// 6 = Proning up to crouch in process
		
	if (Input.GetKeyDown(KeyCode.CapsLock)) {
		isCapsLockOn = !isCapsLockOn;
	}
	if (Input.GetKey(KeyCode.LeftShift)) {
		if (isCapsLockOn) {
			isSprinting = false;
		} else {
			isSprinting = true;
		}
	} else {
		if (isCapsLockOn) {
			isSprinting = true;
		} else {
			isSprinting = false;
		}
	}
	
	if (Input.GetButtonDown("Crouch")) {
		if ((bodyState == 1) || (bodyState == 2)) {
			if (!(CantStand())) {
				bodyState = 3; // Start standing up
			} else {
				print("Can't stand here.");
			}
		} else {
			if ((bodyState == 0) || (bodyState == 3)) {
				bodyState = 2; // Start crouching down
			} else {
				if ((bodyState == 4) || (bodyState == 5)) {
					if (!(CantCrouch())) {
						bodyState = 6; // Start getting up to crouch
					} else {
						print("Can't crouch here.");
					}
				}
			}
		}
	}
	
	if (Input.GetButtonDown("Prone")) {
		if (bodyState == 0 || bodyState == 1 || bodyState == 2 || bodyState == 3 || bodyState == 6) {
			bodyState = 5; // Start proning down
		} else {
			if (bodyState == 4 || bodyState == 5) {
				if (!CantStand()) {
					bodyState = 3; // Start standing up
				} else {
					print("Can't stand here.");
				}
			}
		}
	}
	
	if (currentCrouchRatio > 1) {
		if (bodyState == 0 || bodyState == 3) {
			currentCrouchRatio = 1; //Clamp it
			bodyState = 0;
		}
	} else {	
		if (currentCrouchRatio < crouchRatio) {
			if (bodyState == 1 || bodyState == 2) {
				currentCrouchRatio = crouchRatio; //Clamp it
				bodyState = 1;
			} else {
				if (bodyState == 4 || bodyState == 5) {
					if (currentCrouchRatio < proneRatio) {
						currentCrouchRatio = proneRatio; //Clamp it
						bodyState = 4;
						
					}
				}
			}
		} else {
			if (bodyState == 6) {
				if (currentCrouchRatio > crouchRatio) {
					currentCrouchRatio = crouchRatio; //Clamp it
					bodyState = 1;
				}
			}
		}
	}
}

function FixedUpdate () {
	// Crouch
	transform.localScale.y = (originalLocalScaleY * currentCrouchRatio);
	
	if ((bodyState == 2) || (bodyState == 5))
		currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, -0.01, crouchingVelocity, transitionToCrouchSec);
	
	if (bodyState == 3) {
		lastCrouchRatio = currentCrouchRatio;
		currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, 1.01, crouchingVelocity, transitionToCrouchSec);
		transform.position.y += ((currentCrouchRatio - lastCrouchRatio) * capsuleHeight)/2;
	}
		
	if (bodyState == 6) {
		lastCrouchRatio = currentCrouchRatio;
		currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, 1.01, crouchingVelocity, (transitionToCrouchSec+transitionToProneAdd));
		transform.position.y += ((currentCrouchRatio - lastCrouchRatio) * capsuleHeight)/2;
	}
	
	// Set speed	
	switch (bodyState) {
		case 0: playerSpeed = maxWalkSpeed; //TODO:: lerp from other speeds
				break;
		case 1: playerSpeed = maxCrouchSpeed; //TODO:: lerp from other speeds
				break;
		case 4: playerSpeed = maxProneSpeed; //TODO:: lerp from other speeds
				break;
	}

	//if (isSkating)
	//	playerSpeed = maxSkateSpeed;

	// Limit movement speed
	horizontalMovement = Vector2(rbody.velocity.x, rbody.velocity.z);
	
	if (horizontalMovement.magnitude > playerSpeed) {
		horizontalMovement = horizontalMovement.normalized;
		if (isSprinting) {
			playerSpeed = maxSprintSpeed;
		}
		horizontalMovement *= playerSpeed;
	}
	//verticalMovement = rbody.velocity.y;	
	rbody.velocity.x = horizontalMovement.x;
	rbody.velocity.z = horizontalMovement.y;
	//rbody.velocity.y = verticalMovement;
	
	// Ground friction (TIP: Disable for Cyberspace)
	if (grounded) {
		rbody.velocity.x = Mathf.SmoothDamp(rbody.velocity.x, 0, walkDeaccelerationVolx, walkDeacceleration);
		rbody.velocity.z = Mathf.SmoothDamp(rbody.velocity.z, 0, walkDeaccelerationVolz, walkDeacceleration);	
	}
	
	transform.rotation = Quaternion.Euler(0,mlookScript.yRotation,0); //Change 0 values for x and z for use in Cyberspace
	
	if (grounded == true) {
		if (ladderState) {
			rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * Time.deltaTime, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime, 0);
		} else {
			rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime);
		}
	} else {
		if (ladderState) {
			rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio * Time.deltaTime, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime, 0);
		} else {
			rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio * Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * walkAccelAirRatio * Time.deltaTime);
		}
	}
	
	// Gravity
	if (ladderState) {
		rbody.useGravity = false;
		rbody.velocity.y = Mathf.SmoothDamp(rbody.velocity.y, 0, walkDeaccelerationVolz, walkDeacceleration);  //Set vertical movement towards 0
	} else {
		rbody.useGravity = true;
		//rbody.AddForce(0, (-1 * playerGravity * Time.deltaTime), 0); //Apply gravity force
	}

	// Get input for Jump and set impulse time
	if (Input.GetKey(KeyCode.Space) && grounded && (ladderState==false)) {
		jumpTime = jumpImpulseTime;
	}
	
	// Perform Jump
	while (jumpTime > 0) {
		jumpTime -= Time.smoothDeltaTime;
		rbody.AddForce( new Vector3(0,jumpVelocity*rbody.mass,0), ForceMode.Force);
	}
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