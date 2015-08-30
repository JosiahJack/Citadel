using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public bool isCapsLockOn = false;
	public float walkAcceleration = 2000;
	public float walkDeacceleration = 0.15f;
	public float walkAccelAirRatio = 0.1f;
	public float verticalAcceleration = 1200;

	private float walkDeaccelerationVolx;
	private float walkDeaccelerationVolz;

	public GameObject cameraObject;
	public float playerSpeed;
	public float maxWalkSpeed = 5;
	public float maxCrouchSpeed = 3.5f;
	public float maxProneSpeed = 2;
	public float maxSprintSpeed = 10;
	public float maxSkateSpeed = 12;
	public float maxVerticalSpeed = 5;
	public bool isSprinting = false;
	public bool  isSkating = false;

	private Vector2 horizontalMovement;
	private float verticalMovement;
	private float jumpTime;

	public float jumpImpulseTime = 2.0f;
	public float jumpVelocity = 1.1f;

	private bool  grounded = false;

	public float maxSlope = 60;
	public float crouchRatio = 0.55f;
	public float proneRatio = 0.3f;
	public float transitionToCrouchSec = 0.2f;
	public float transitionToProneAdd = 0.1f;

	private float crouchingVelocity = 1;

	public float currentCrouchRatio = 1;

	private float originalLocalScaleY;

	public float crouchLocalScaleY;

	private float lastCrouchRatio;

	public float capsuleHeight;
	public float capsuleRadius;

	private int layerGeometry = 9;
	private int layerMask;
	//private float originalCapsuleHeight;

	public int bodyState = 0;
	//public float playerGravity = 500;
	public bool ladderState = false;

	private Rigidbody rbody;
	private MouseLookScript mlookScript;
	
	void  Awake (){
		isCapsLockOn = false;
		currentCrouchRatio = 1;
		bodyState = 0;
		originalLocalScaleY = transform.localScale.y;
		crouchLocalScaleY = transform.localScale.y * crouchRatio;
		rbody = GetComponent<Rigidbody>();
		//rbody.useGravity = false;
		mlookScript = cameraObject.GetComponent<MouseLookScript>();
		capsuleHeight = GetComponent<CapsuleCollider>().height;
		//originalCapsuleHeight = capsuleHeight;
		capsuleRadius = GetComponent<CapsuleCollider>().radius;
		layerMask = 1 << layerGeometry;
	}
	
	bool CantStand (){
		return Physics.CheckCapsule(cameraObject.transform.position, cameraObject.transform.position + new Vector3(0f,(1.6f-0.84f),0f), capsuleRadius, layerMask);
	}
	
	bool CantCrouch (){
		return Physics.CheckCapsule(cameraObject.transform.position, cameraObject.transform.position + new Vector3(0f,0.4f,0f), capsuleRadius, layerMask);
	} 
	
	void  Update (){
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
			if (grounded) {
				if (isCapsLockOn) {
					isSprinting = false;
				} else {
					isSprinting = true;
				}
			}
		} else {
			if (grounded) {
				if (isCapsLockOn) {
					isSprinting = true;
				} else {
					isSprinting = false;
				}
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

	public void LocalScaleSetX(Transform t, float s ) {
		Vector3 v = t.localScale;
		v.x  = s;
		t.localScale = v;
	}

	public void LocalScaleSetY(Transform t, float s ) {
		Vector3 v = t.localScale;
		v.y  = s;
		t.localScale = v;
	}

	public void LocalScaleSetZ(Transform t, float s ) {
		Vector3 v = t.localScale;
		v.z  = s;
		t.localScale = v;
	}

	public void LocalPositionSetX(Transform t, float s ) {
		Vector3 v = t.position;
		v.x  = s;
		t.position = v;
	}
	
	public void LocalPositionSetY(Transform t, float s ) {
		Vector3 v = t.position;
		v.y  = s;
		t.position = v;
	}
	
	public void LocalPositionSetZ(Transform t, float s ) {
		Vector3 v = t.position;
		v.z  = s;
		t.position = v;
	}

	public void RigidbodySetVelocityX(Rigidbody t, float s ) {
		Vector3 v = t.velocity;
		v.x  = s;
		t.velocity = v;
	}
	
	public void RigidbodySetVelocityY(Rigidbody t, float s ) {
		Vector3 v = t.velocity;
		v.y  = s;
		t.velocity = v;
	}
	
	public void RigidbodySetVelocityZ(Rigidbody t, float s ) {
		Vector3 v = t.velocity;
		v.z  = s;
		t.velocity = v;
	}

	void  FixedUpdate (){
		// Crouch
		//transform.localScale.y = new Vector3(0,originalLocalScaleY * currentCrouchRatio,0);
		LocalScaleSetY(transform,(originalLocalScaleY * currentCrouchRatio)); 
		
		if ((bodyState == 2) || (bodyState == 5))
			currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, -0.01f, ref crouchingVelocity, transitionToCrouchSec);
		
		if (bodyState == 3) {
			lastCrouchRatio = currentCrouchRatio;
			currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, 1.01f, ref crouchingVelocity, transitionToCrouchSec);
			//transform.position.y += ((currentCrouchRatio - lastCrouchRatio) * capsuleHeight)/2;
			LocalPositionSetY(transform,(((currentCrouchRatio - lastCrouchRatio) * capsuleHeight)/2)+transform.position.y);
		}
		
		if (bodyState == 6) {
			lastCrouchRatio = currentCrouchRatio;
			currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, 1.01f, ref crouchingVelocity, (transitionToCrouchSec+transitionToProneAdd));
			//transform.position.y += ((currentCrouchRatio - lastCrouchRatio) * capsuleHeight)/2;
			LocalPositionSetY(transform,(((currentCrouchRatio - lastCrouchRatio) * capsuleHeight)/2)+transform.position.y);
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
		horizontalMovement = new Vector2(rbody.velocity.x, rbody.velocity.z);
		
		if (horizontalMovement.magnitude > playerSpeed) {
			horizontalMovement = horizontalMovement.normalized;
			if (isSprinting) {
				playerSpeed = maxSprintSpeed;
			}
			horizontalMovement *= playerSpeed;
		}	
		//rbody.velocity.x = horizontalMovement.x;
		//rbody.velocity.z = horizontalMovement.y;
		RigidbodySetVelocityX(rbody, horizontalMovement.x);
		RigidbodySetVelocityZ(rbody, horizontalMovement.y);
		//rbody.velocity.y = verticalMovement;
		verticalMovement = rbody.velocity.y;
		if (verticalMovement > maxVerticalSpeed) {
			verticalMovement = maxVerticalSpeed;
		}
		RigidbodySetVelocityY(rbody, verticalMovement);
		
		// Ground friction ( Disable TIP   for Cyberspace)
		if (grounded) {
			//rbody.velocity.x = Mathf.SmoothDamp(rbody.velocity.x, 0, ref walkDeaccelerationVolx, walkDeacceleration);
			//rbody.velocity.z = Mathf.SmoothDamp(rbody.velocity.z, 0, ref walkDeaccelerationVolz, walkDeacceleration);
			RigidbodySetVelocityX(rbody, (Mathf.SmoothDamp(rbody.velocity.x, 0, ref walkDeaccelerationVolx, walkDeacceleration)));
			RigidbodySetVelocityZ(rbody, (Mathf.SmoothDamp(rbody.velocity.z, 0, ref walkDeaccelerationVolz, walkDeacceleration)));
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
				if (isSprinting) {
					rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime);
				} else {
					rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio *  Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * walkAccelAirRatio * Time.deltaTime);
				}
			}
		}
		
		// Gravity
		if (ladderState) {
			rbody.useGravity = false;
			//rbody.velocity.y = Mathf.SmoothDamp(rbody.velocity.y, 0, ref walkDeaccelerationVolz, walkDeacceleration);  //Set vertical movement towards 0
			RigidbodySetVelocityY(rbody, (Mathf.SmoothDamp(rbody.velocity.y, 0, ref walkDeaccelerationVolz, walkDeacceleration)));
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
	
	void  OnCollisionStay ( Collision collision  ){
		foreach(ContactPoint contact in collision.contacts) {
			if (Vector3.Angle(contact.normal,Vector3.up) < maxSlope)
				grounded = true;
		}
	}
	
	void  OnCollisionExit (){
		grounded = false;
	}
}