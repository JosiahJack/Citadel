using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public float walkAcceleration = 2000;
	public float walkDeacceleration = 0.15f;
	public float walkAccelAirRatio = 0.1f;
	public float verticalAcceleration = 1200;
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
	public float jumpImpulseTime = 2.0f;
	public float jumpVelocity = 1.1f;
	public bool  grounded = false;
	public float maxSlope = 60;
	public float crouchRatio = 0.55f;
	public float proneRatio = 0.3f;
	public float transitionToCrouchSec = 0.2f;
	public float transitionToProneAdd = 0.1f;
	public float currentCrouchRatio = 1;
	public float crouchLocalScaleY;
	public float capsuleHeight;
	public float capsuleRadius;
	public int bodyState = 0;
	//public float playerGravity = 500;
	public bool ladderState = false;
	public float ladderSpeed = 0.25f;
	public float damageSpeed = 11.72f;
	public float fallDamage = 75f;
	public bool gravliftState = false;
	public GameObject automapContainer;
	public Texture2D automapMaskTex;
	public float automapFactor = 0.02f;
	private Sprite automapMaskSprite;
	//[HideInInspector]
	public bool CheatWallSticky;
	public bool staminupActive = false;
	private float walkDeaccelerationVolx;
	private float walkDeaccelerationVolz;
	private Vector2 horizontalMovement;
	private float verticalMovement;
	private float jumpTime;
	private float crouchingVelocity = 1;
	private float originalLocalScaleY;
	private float lastCrouchRatio;
	private int layerGeometry = 9;
	private int layerMask;
	//private float originalCapsuleHeight;
	private Rigidbody rbody;
	private float fallDamageSpeed = 11.72f;
	private Vector3 oldVelocity;
	public GameObject mainMenu;
	public float fatigue;
	public float maxFatigueSpeed = 3f;
	
	void  Awake (){
		currentCrouchRatio = 1;
		bodyState = 0;
		originalLocalScaleY = transform.localScale.y;
		crouchLocalScaleY = transform.localScale.y * crouchRatio;
		rbody = GetComponent<Rigidbody>();
		oldVelocity = rbody.velocity;
		//rbody.useGravity = false;
		capsuleHeight = GetComponent<CapsuleCollider>().height;
		//originalCapsuleHeight = capsuleHeight;
		capsuleRadius = GetComponent<CapsuleCollider>().radius;
		layerMask = 1 << layerGeometry;
		staminupActive = false;
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
		if (mainMenu.activeSelf == true) return;  // ignore movement when main menu is still up
		if (!PauseScript.a.paused) {
			rbody.WakeUp();

			if (GetInput.a.Sprint()) {
				if (grounded) {
					if (GetInput.a.CapsLockOn()) {
						isSprinting = false;
					} else {
						isSprinting = true;
					}
				}
			} else {
				if (grounded) {
					if (GetInput.a.CapsLockOn()) {
						isSprinting = true;
					} else {
						isSprinting = false;
					}
				}
			}
			
			if (GetInput.a.Crouch()) {
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
			
			if (GetInput.a.Prone()) {
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
		if (mainMenu.activeSelf == true) return;  // ignore movement when main menu is still up
		if (PauseScript.a == null) {
			Const.sprint("ERROR->PlayerMovement: PauseScript is null",transform.parent.gameObject);
			return;
		}

		if (!PauseScript.a.paused) {
			// Crouch
			LocalScaleSetY(transform,(originalLocalScaleY * currentCrouchRatio));
			
			// Handle body state speeds and body position lerping for smooth transitions
			switch (bodyState) {
				case 0: 
					playerSpeed = maxWalkSpeed; //TODO:: lerp from other speeds
					break;
				case 1:
					playerSpeed = maxCrouchSpeed; //TODO:: lerp from other speeds
					break;
				case 2: 
					currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, -0.01f, ref crouchingVelocity, transitionToCrouchSec);
					break;
				case 3: 
					lastCrouchRatio = currentCrouchRatio;
					currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, 1.01f, ref crouchingVelocity, transitionToCrouchSec);
					LocalPositionSetY(transform,(((currentCrouchRatio - lastCrouchRatio) * capsuleHeight)/2)+transform.position.y);
					break;
				case 4: 
					playerSpeed = maxProneSpeed; //TODO:: lerp from other speeds
					break;
				case 5: 
					currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, -0.01f, ref crouchingVelocity, transitionToCrouchSec);
					break;
				case 6:
					lastCrouchRatio = currentCrouchRatio;
					currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio, 1.01f, ref crouchingVelocity, (transitionToCrouchSec+transitionToProneAdd));
					LocalPositionSetY(transform,(((currentCrouchRatio - lastCrouchRatio) * capsuleHeight)/2)+transform.position.y);
					break;
			}
			
			// Limit movement speed horizontally
			horizontalMovement = new Vector2(rbody.velocity.x, rbody.velocity.z);
			
			if (horizontalMovement.magnitude > playerSpeed) {
				horizontalMovement = horizontalMovement.normalized;
				if (isSprinting)
					playerSpeed = maxSprintSpeed;
				horizontalMovement *= playerSpeed;  // set back down to max speed this frame
			}	

			// Set horizontal velocity
			RigidbodySetVelocityX(rbody, horizontalMovement.x);
			RigidbodySetVelocityZ(rbody, horizontalMovement.y);

			// Update automap position
			UpdateAutomap();
			if (horizontalMovement.x != 0 || horizontalMovement.y != 0) {
				//automapContainer.GetComponent<ScrollRect>().verticalNormalizedPosition += horizontalMovement.y * automapFactor * (-1);
				//automapContainer.GetComponent<ScrollRect>().horizontalNormalizedPosition += horizontalMovement.x * automapFactor * (-1);
				//UpdateAutomap();
			}

			// Set vertical velocity
			verticalMovement = rbody.velocity.y;
			if (verticalMovement > maxVerticalSpeed) {
				verticalMovement = maxVerticalSpeed;
			}
			RigidbodySetVelocityY(rbody, verticalMovement);
			
			// Ground friction (TODO: disable grounded for Cyberspace)
			if (grounded) {
				RigidbodySetVelocityX(rbody, (Mathf.SmoothDamp(rbody.velocity.x, 0, ref walkDeaccelerationVolx, walkDeacceleration)));
				RigidbodySetVelocityZ(rbody, (Mathf.SmoothDamp(rbody.velocity.z, 0, ref walkDeaccelerationVolz, walkDeacceleration)));
			}

			// Set rotation of playercapsule from mouselook script TODO: Is this needed?
			//transform.rotation = Quaternion.Euler(0,mlookScript.yRotation,0); //Change 0 values for x and z for use in Cyberspace

			float relForward = 0f;
			float relSideways = 0f;
			if (GetInput.a.Forward())
				relForward = 1f;

			if (GetInput.a.Backpedal())
				relForward = -1f;

			if (GetInput.a.StrafeLeft())
				relSideways = -1f;

			if (GetInput.a.StrafeRight())
				relSideways = 1f;

			// Handle ladder movement
			if (grounded == true) {
				if (ladderState) {
					//rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * Time.deltaTime, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime, 0);
					rbody.AddRelativeForce(relSideways * walkAcceleration * Time.deltaTime, relForward * walkAcceleration * Time.deltaTime, 0);
				} else {
					//rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime);
					rbody.AddRelativeForce(relSideways * walkAcceleration * Time.deltaTime, 0, relForward * walkAcceleration * Time.deltaTime);
				}
			} else {
				if (ladderState) {
					//rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio * Time.deltaTime, ladderSpeed * Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime, 0);
					rbody.AddRelativeForce(relSideways * walkAcceleration * walkAccelAirRatio * Time.deltaTime, ladderSpeed * relForward * walkAcceleration * Time.deltaTime, 0);
				} else {
					if (isSprinting) {
						//rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime);
						rbody.AddRelativeForce(relSideways * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime, 0, relForward * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime);
					} else {
						//rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio *  Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * walkAccelAirRatio * Time.deltaTime);
						rbody.AddRelativeForce(relSideways * walkAcceleration * walkAccelAirRatio *  Time.deltaTime, 0, relForward * walkAcceleration * walkAccelAirRatio * Time.deltaTime);
					}
				}
			}
			
			// Handle gravity and ladders
			if (ladderState) {
				rbody.useGravity = false;
				// Set vertical velocity towards 0 when climbing
				RigidbodySetVelocityY(rbody, (Mathf.SmoothDamp(rbody.velocity.y, 0, ref walkDeaccelerationVolz, walkDeacceleration)));
			} else {
				// Check if using a gravity lift
				if (gravliftState == true) {
					rbody.useGravity = false;
				} else {
					// Disables gravity when touching the ground to prevent player sliding down ramps
					if (grounded == true) {
						rbody.useGravity = false;
					} else {
						rbody.useGravity = true;
					}
				}
				// Apply gravity - OBSOLETE: Now handled by gravity of the RigidBody physics system
				//rbody.AddForce(0, (-1 * playerGravity * Time.deltaTime), 0); //Apply gravity force
			}
			
			// Get input for Jump and set impulse time
			if (GetInput.a.Jump() && (grounded || gravliftState) && (ladderState==false))
				jumpTime = jumpImpulseTime;
			
			// Perform Jump
			while (jumpTime > 0) {
				jumpTime -= Time.smoothDeltaTime;
				rbody.AddForce( new Vector3(0,jumpVelocity*rbody.mass,0), ForceMode.Force);  // huhnh!
			}

			// Handle fall damage
			if (Mathf.Abs((oldVelocity.y - rbody.velocity.y)) > fallDamageSpeed) {
				DamageData dd = new DamageData();
				dd.damage = fallDamage;
				dd.attackType = Const.AttackType.None;
				dd.offense = 0f;
				dd.isOtherNPC = false;
				GetComponent<PlayerHealth>().TakeDamage(dd);
			}
			oldVelocity = rbody.velocity;

			// Automatically set grounded to false to prevent ability to climb any wall
			if (CheatWallSticky == false || gravliftState)
				grounded = false;	
		}
	}

	// Update automap location
	public void UpdateAutomap () {
		//Texture2D tex = new Texture2D(512,512);  // 722,658
		//tex.SetPixels(automapMaskTex.GetPixels(0,0,512,512), 0);
		//tex.Apply();
		//automapMaskSprite = Sprite.Create(tex, new Rect(0, 0, 512, 512), new Vector2(50,50));
		//automapContainer.GetComponent<Image>().sprite = automapMaskSprite;
	}

	// Sets grounded based on normal angle of the impact point (NOTE: This is not the surface normal!)
	void OnCollisionStay (Collision collision  ){
		if (!PauseScript.a.paused) {
			foreach(ContactPoint contact in collision.contacts) {
				if (Vector3.Angle(contact.normal,Vector3.up) < maxSlope) {
					grounded = true;
				}
			}
		}
	}

	// Reset grounded to false when player is mid-air
	void OnCollisionExit (){
		if (!PauseScript.a.paused) {
			// Automatically set grounded to false to prevent ability to climb any wall (Cheat!)
			if (CheatWallSticky == true) {
				grounded = false;
			}
		}
	}
}