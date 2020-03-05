using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour {
	private float walkAcceleration = 2000f;
	private float walkDeacceleration = 0.15f;
	private float walkDeaccelerationBooster = 2f;
	private float deceleration;
	private float walkAccelAirRatio = 0.15f;
	public GameObject cameraObject;
	public MouseLookScript mlookScript;
	public float playerSpeed;
	private float maxWalkSpeed = 3f;
	private float maxCyberSpeed = 4f;
	private float maxCrouchSpeed = 1.75f;
	private float maxProneSpeed = 1f;
	private float maxSprintSpeed = 9f;
	private float maxSprintSpeedFatigued = 5.5f;
	private float maxVerticalSpeed = 5f;
	private float boosterSpeedBoost = 0.5f; // ammount to boost by when booster is active
	public bool isSprinting = false;
	public bool  isSkating = false;
	private float jumpImpulseTime = 2.0f;
	public float jumpVelocityBoots = 0.5f;
	private float jumpVelocity = 1.1f;
	private  float jumpVelocityFatigued = 0.6f;
	public bool  grounded = false;
	private float maxSlope = 90f;
	private float crouchRatio = 0.6f;
	private float proneRatio = 0.2f;
	private float transitionToCrouchSec = 0.2f;
	private float transitionToProneAdd = 0.1f;
	public float currentCrouchRatio = 1f;
	public float crouchLocalScaleY;
	public float capsuleHeight;
	public float capsuleRadius;
	public int bodyState = 0;
	public bool ladderState = false;
	private float ladderSpeed = 0.25f;
	private float fallDamage = 75f;
	public bool gravliftState = false;
	public bool inCyberSpace = false;
	public GameObject automapContainer;
	public Texture2D automapMaskTex;
	public float automapFactor = 0.000285f;
	private Sprite automapMaskSprite;
	//[HideInInspector]
	public bool CheatWallSticky;
    //[HideInInspector]
    public bool CheatNoclip;
    public bool staminupActive = false;
	private Vector2 horizontalMovement;
	private float verticalMovement;
	private float jumpTime;
	private float crouchingVelocity = 1f;
	private float originalLocalScaleY;
	private float lastCrouchRatio;
	private int layerGeometry = 9;
	private int layerMask;
	private Rigidbody rbody;
	private float fallDamageSpeed = 11.72f;
	private Vector3 oldVelocity;
	public GameObject mainMenu;
	public HardwareInvCurrent hwc;
	public float fatigue;
	private float jumpFatigue = 8f;
	private float fatigueWanePerTick = 1f;
	private float fatigueWanePerTickCrouched = 2f;
	private float fatigueWanePerTickProne = 3.5f;
	private float fatigueWaneTickSecs = 0.3f;
	private float fatiguePerWalkTick = 0.9f;
	private float fatiguePerSprintTick = 2.5f;
	public string cantStandText = "Can't stand here.";
	public string cantCrouchText = "Can't crouch here.";
	private bool justJumped = false;
	private float fatigueFinished;
	private float fatigueFinished2;
	public TextWarningsManager twm;

	private int defIndex = 0;
	private int def1 = 1;
	private int onehundred = 100;
	public bool running = false;
	public bool cyberSetup = false;
	public bool cyberDesetup = false;
	private SphereCollider cyberCollider;
	private CapsuleCollider capsuleCollider;
	public CapsuleCollider leanCapsuleCollider;
	private int oldBodyState;
	private float bonus;
    private float walkDeaccelerationVolx;
    private float walkDeaccelerationVoly;
    private float walkDeaccelerationVolz;

	public Image consolebg;
    public InputField consoleinpFd;
    public GameObject consoleplaceholderText;
	public GameObject consoleTitle;
	public Text consoleentryText;
	public bool consoleActivated;

	public Transform leanTransform;

	private float leanLeftDoubleFinished;
	private float leanRightDoubleFinished;
	private float keyboardButtonDoubleTapTime = 0.25f; // max time before a double tap of a keyboard button is registered
	private float leanTarget = 0f;

	public AudioSource PlayerNoise;
	public AudioClip SFXJump;
	public AudioClip SFXJumpLand;
	public float jumpSFXFinished;
	public float jumpSFXIntervalTime = 1f;
	public float jumpLandSoundFinished;
	private Vector3 tempVec;
	public float leanSpeed = 5f;
	public bool leanLHFirstPressed = false;
	public bool leanRHFirstPressed = false;
	public bool leanLHReset = false;
	public bool leanRHReset = false;
	public bool Notarget = false; // for cheat to disable enemy sight checks against this player
	public GameObject fpsCounter;
	public WeaponCurrent wepCur;
	private bool fatigueWarned;

    void Awake (){
		currentCrouchRatio = def1;
		bodyState = defIndex;
		cyberDesetup = false;
		oldBodyState = bodyState;
		fatigueFinished = Time.time;
		fatigueFinished2 = Time.time;
		originalLocalScaleY = transform.localScale.y;
		crouchLocalScaleY = transform.localScale.y * crouchRatio;
		rbody = GetComponent<Rigidbody>();
		oldVelocity = rbody.velocity;
		capsuleCollider = GetComponent<CapsuleCollider>();
		capsuleHeight = capsuleCollider.height;
		capsuleRadius = capsuleCollider.radius;
		layerMask = def1 << layerGeometry;
		staminupActive = false;
		cyberCollider = GetComponent<SphereCollider>();
		consoleActivated = false;
		leanLeftDoubleFinished = Time.time;
		leanRightDoubleFinished = Time.time;
		jumpLandSoundFinished = Time.time;
		justJumped = false;
		leanLHFirstPressed = false;
		leanRHFirstPressed = false;
		leanLHReset = false;
		leanRHReset = false;
		jumpSFXFinished = Time.time;
		fatigueWarned = false;
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

		// Always allow console, even when paused
        if (GetInput.a.Console()) {
            ToggleConsole();
        }

		if (Input.GetKeyDown(KeyCode.Return)) {
			ConsoleEntry();
		}

		if (consoleActivated) {
			if (!String.IsNullOrEmpty(consoleentryText.text)) {
				consoleplaceholderText.SetActive(false);
			} else {
				consoleplaceholderText.SetActive(true);
			}
		} else {
			consoleplaceholderText.SetActive(false);
		}

		if (mainMenu.activeSelf == true) return;  // ignore movement when main menu is still up
		if (!PauseScript.a.Paused()) {
			rbody.WakeUp();

			if (inCyberSpace && !cyberSetup) {
				cyberCollider.enabled = true;
				capsuleCollider.enabled = false;
				rbody.useGravity = false;
				mlookScript.inCyberSpace = true; // enable full camera rotation up/down by disabling clamp
				oldBodyState = bodyState;
				bodyState = defIndex; // reset to "standing" to prevent speed anomolies
				// TODO enable dummy player for coop games
				cyberSetup = true;
				cyberDesetup = true;
			}
				
			if (!inCyberSpace && cyberDesetup || CheatNoclip) {
                if (CheatNoclip) {
                    // Flying cheat...also map editing mode!
                    cyberCollider.enabled = false; // can't touch dis
                    capsuleCollider.enabled = false; //na nana na, na na, can't touch dis
					leanCapsuleCollider.enabled = false;
                    rbody.useGravity = false; // look ma! no legs

                    Mathf.Clamp(mlookScript.xRotation, -90f, 90f); // pre-clamp camera rotation - still useful
                    mlookScript.inCyberSpace = false; // disable full camera rotation up/down by enabling auto clamp
                    bodyState = oldBodyState; // return to what we were doing in the "real world" (real lol)
                                              // TODO disable dummy player for coop games  dummyPlayerModel.SetActive(false); dummyPlayerCapsule.enabled = false;
                    cyberSetup = false;
                    cyberDesetup = false;
                } else {
                    cyberCollider.enabled = false;
                    capsuleCollider.enabled = true;
					leanCapsuleCollider.enabled = true;
                    rbody.useGravity = true;

                    Mathf.Clamp(mlookScript.xRotation, -90f, 90f); // pre-clamp camera rotation
                    mlookScript.inCyberSpace = false; // disable full camera rotation up/down by enabling auto clamp
                    bodyState = oldBodyState; // return to what we were doing in the "real world" (real lol)
                                              // TODO disable dummy player for coop games
                    cyberSetup = false;
                    cyberDesetup = false;
                }
			}

			if (GetInput.a.Sprint() && !consoleActivated) {
				if (grounded || CheatNoclip) {
					if (GetInput.a.CapsLockOn()) {
						isSprinting = false;
					} else {
						isSprinting = true;
					}
				}
			} else {
				if (grounded || CheatNoclip) {
					if (GetInput.a.CapsLockOn()) {
						isSprinting = true;
					} else {
						isSprinting = false;
					}
				}
			}
			
			if (GetInput.a.Crouch() && !CheatNoclip && !consoleActivated) {
				if ((bodyState == 1) || (bodyState == 2)) {
					if (!(CantStand())) {
						bodyState = 3; // Start standing up
						//Debug.Log ("Standing up from crouch...");
					} else {
						Const.sprint(cantStandText,Const.a.player1);
					}
				} else {
					if ((bodyState == 0) || (bodyState == 3)) {
						//Debug.Log ("Crouching down...");
						bodyState = 2; // Start crouching down
					} else {
						if ((bodyState == 4) || (bodyState == 5)) {
							if (!(CantCrouch())) {
								//Debug.Log ("Getting up from prone to crouch...");
								bodyState = 6; // Start getting up to crouch
							} else {
								Const.sprint(cantCrouchText,Const.a.player1);
							}
						}
					}
				}
			}
			
			if (GetInput.a.Prone() && !CheatNoclip && !consoleActivated) {
				if (bodyState == 0 || bodyState == 1 || bodyState == 2 || bodyState == 3 || bodyState == 6) {
					//Debug.Log ("Proning down...");
					bodyState = 5; // Start proning down
				} else {
					if (bodyState == 4 || bodyState == 5) {
						if (!CantStand()) {
							//Debug.Log ("Getting up from prone to standing...");
							bodyState = 3; // Start standing up
						} else {
							Const.sprint(cantStandText,Const.a.player1);
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
			// here fatigue me out, except in cyberspace
			if (fatigueFinished < Time.time && !inCyberSpace && !CheatNoclip) {
				fatigueFinished = Time.time + fatigueWaneTickSecs;
				switch (bodyState) {
				case 0: fatigue -= fatigueWanePerTick; break;
				case 1: fatigue -= fatigueWanePerTickCrouched; break;
				case 4: fatigue -= fatigueWanePerTickProne; break;
				default: fatigue -= fatigueWanePerTick; break;
				}
				if (fatigue < defIndex) fatigue = defIndex; // clamp at 0
			}
			if (fatigue > onehundred) fatigue = onehundred; // clamp at 100 using dummy variables to hang onto the value and not get collected by garbage collector (really?  hey it was back in the old days when we didn't have incremental garbage collector, pre Unity 2019.2 versions
			if (fatigue > 80 && !fatigueWarned) {
				twm.SendWarning(("Fatigue high"),0.1f,0,TextWarningsManager.warningTextColor.white,324);
				fatigueWarned = true;
			} else {
				fatigueWarned = false;
			}

			// Handle Right Leaning start
			if (GetInput.a.LeanRightStart()) {
				leanLHFirstPressed = false;	
				leanLHReset = false;	
				if (!leanRHFirstPressed) {
					leanRHFirstPressed = true;
				} else {
					if ((Time.time - leanRightDoubleFinished) < keyboardButtonDoubleTapTime) {
						//if (leanTransform.eulerAngles.z < 15f && leanTransform.eulerAngles.z > -15f) { // Wasn't working, maybe too sensitive?
						//	Debug.Log("Leaning all the way Right!");
						//	leanTarget = -22.5f;
						//} else {
						//	Debug.Log("Leaning reset from right");
						//	leanTarget = 0;
						//}
						leanTarget = 0;
						leanRHReset = true;
						leanRHFirstPressed = false;
					} else {
						leanRHReset = false;
					}
				}
			}
			// Handle Left Leaning start
			if (GetInput.a.LeanLeftStart()) {
				leanRHFirstPressed = false;
				leanRHReset = false;
				if (!leanLHFirstPressed) {
					leanLHFirstPressed = true;
				} else {
					if ((Time.time - leanLeftDoubleFinished) < keyboardButtonDoubleTapTime) {
						//if (leanTransform.eulerAngles.z < 15f && leanTransform.eulerAngles.z > -15f) {
						//	Debug.Log("Leaning all the way Left!");
						//	leanTarget = 22.5f;
						//} else {
						//	Debug.Log("Leaning reset from left");
						//	leanTarget = 0;
						//}
						leanTarget = 0;
						leanLHReset = true;
						leanLHFirstPressed = false;
					} else {
						leanLHReset = false;
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

	public void RigidbodySetVelocity(Rigidbody t, float s ) {
		Vector3 v = t.velocity;
		v = v.normalized * s;
		t.velocity = v;
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

		if (!PauseScript.a.Paused()) {
            if (CheatNoclip) grounded = true;
			// Crouch
			//LocalScaleSetY(transform,(originalLocalScaleY * currentCrouchRatio));
			capsuleCollider.height = currentCrouchRatio * 2f;
			leanCapsuleCollider.height = currentCrouchRatio * 2f;
			
			// Handle body state speeds and body position lerping for smooth transitions
			if (!inCyberSpace && !CheatNoclip) {
				bonus = 0f;
				if (hwc.hardwareIsActive [9]) bonus = boosterSpeedBoost;

				switch (bodyState) {
					case 0:
						playerSpeed = maxWalkSpeed + bonus; //TODO:: lerp from other speeds
						break;
					case 1:
						//Debug.Log("Crouched");
						playerSpeed = maxCrouchSpeed + bonus; //TODO:: lerp from other speeds
						break;
					case 2: 
						//Debug.Log("Crouching down lerp from standing...");
						currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, -0.01f, ref crouchingVelocity, transitionToCrouchSec);
						break;
					case 3:
						//Debug.Log("Standing up lerp from crouch or prone...");
						lastCrouchRatio = currentCrouchRatio;
						currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, 1.01f, ref crouchingVelocity, transitionToCrouchSec);
						LocalPositionSetY (transform, (((currentCrouchRatio - lastCrouchRatio) * capsuleHeight) / 2) + transform.position.y);
						break;
					case 4: 
						playerSpeed = maxProneSpeed + bonus; //TODO:: lerp from other speeds
						break;
					case 5: 
						currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, -0.01f, ref crouchingVelocity, transitionToCrouchSec);
						break;
					case 6:
						lastCrouchRatio = currentCrouchRatio;
						currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, 1.01f, ref crouchingVelocity, (transitionToCrouchSec + transitionToProneAdd));
						LocalPositionSetY (transform, (((currentCrouchRatio - lastCrouchRatio) * capsuleHeight) / 2) + transform.position.y);
						break;
				}
			} else {
                if (!CheatNoclip) {
                    //Cyber space state
                    playerSpeed = maxCyberSpeed;
                }
			}

			if (CheatNoclip) playerSpeed = maxCyberSpeed*2f;
		
			if (inCyberSpace && !CheatNoclip) {
				// Limit movement speed in all axes x,y,z in cyberspace
				if (rbody.velocity.magnitude > playerSpeed) {
					RigidbodySetVelocity(rbody, playerSpeed);
				}
			} else {
				// Limit movement speed horizontally for normal movement
				horizontalMovement = new Vector2(rbody.velocity.x, rbody.velocity.z);
				
				if (horizontalMovement.magnitude > playerSpeed) {
					horizontalMovement = horizontalMovement.normalized;
					if (isSprinting && running && !inCyberSpace) {
						if (fatigue > 80f) {
							playerSpeed = maxSprintSpeedFatigued + bonus;
						} else {
							playerSpeed = maxSprintSpeed + bonus;
						}
						if (CheatNoclip) playerSpeed = maxSprintSpeed + (bonus*1.5f);
					}
					horizontalMovement *= playerSpeed;  // cap velocity to max speed
				}

				// Set horizontal velocity
				RigidbodySetVelocityX(rbody, horizontalMovement.x);
				RigidbodySetVelocityZ(rbody, horizontalMovement.y); // NOT A BUG - already passed rbody.velocity.z into the .y of this Vector2

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
				if (!CheatNoclip) RigidbodySetVelocityY(rbody, verticalMovement);

				// Ground friction (TODO: disable grounded for Cyberspace)
				if (grounded || CheatNoclip) {
					if (hwc.hardwareIsActive [9]) {
						deceleration = walkDeaccelerationBooster;
					} else {
						deceleration = walkDeacceleration;
					}
                    if (CheatNoclip) deceleration = 0.05f;
                    RigidbodySetVelocityX(rbody, (Mathf.SmoothDamp(rbody.velocity.x, 0, ref walkDeaccelerationVolx, deceleration)));
                    if (CheatNoclip) RigidbodySetVelocityY(rbody, (Mathf.SmoothDamp(rbody.velocity.y, 0, ref walkDeaccelerationVoly, deceleration)));
                    RigidbodySetVelocityZ(rbody, (Mathf.SmoothDamp(rbody.velocity.z, 0, ref walkDeaccelerationVolz, deceleration)));

				}
			}
				
			// Set rotation of playercapsule from mouselook script TODO: Is this needed?
			//transform.rotation = Quaternion.Euler(0,mlookScript.yRotation,0); //Change 0 values for x and z for use in Cyberspace

			float relForward = 0f;
			float relSideways = 0f;
			running = false;
			if (GetInput.a.Forward() && !consoleActivated) {
				relForward = 1f;
				leanTarget = 0;
				leanRHReset = true;
				leanLHReset = true;
				leanRHFirstPressed = false;
				leanLHFirstPressed = false;
			}

			if (GetInput.a.Backpedal() && !consoleActivated)
				relForward = -1f;

			if (GetInput.a.StrafeLeft() && !consoleActivated)
				relSideways = -1f;

			if (GetInput.a.StrafeRight() && !consoleActivated)
				relSideways = 1f;

            if (relForward != 0 || relSideways != 0) running = true; // we are mashing a run button down

			// Handle movement
			if (!inCyberSpace) {
				if (grounded == true || CheatNoclip) {
					if (ladderState && !CheatNoclip) {
						// CLimbing when touching the ground
						//rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * Time.deltaTime, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime, 0);
						rbody.AddRelativeForce (relSideways * walkAcceleration * Time.deltaTime, relForward * walkAcceleration * Time.deltaTime, 0);
					} else {
                        //Walking on the ground
                        //rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime);
                        if (CheatNoclip) {
                            rbody.AddRelativeForce(relSideways * 2f * walkAcceleration * Time.deltaTime, 0, relForward * 2f * walkAcceleration * Time.deltaTime);
                        } else {
                            rbody.AddRelativeForce(relSideways * walkAcceleration * Time.deltaTime, 0, relForward * walkAcceleration * Time.deltaTime);
                        }

                        // Noclip up and down
                        if (GetInput.a.SwimUp() && !consoleActivated) {
                            if (CheatNoclip) {
                                //Debug.Log("Floating Up!");
                                rbody.AddRelativeForce(0, 4 * walkAcceleration * Time.deltaTime, 0);
                            }
                        }

                        if (GetInput.a.SwimDn() && !consoleActivated) {
                            if (CheatNoclip) {
                                //Debug.Log("Floating Dn!");
                                rbody.AddRelativeForce(0, 4 * walkAcceleration * Time.deltaTime * -1, 0);
                            }
                        }

                        if (fatigueFinished2 < Time.time && relForward != defIndex && !CheatNoclip) {
							fatigueFinished2 = Time.time + fatigueWaneTickSecs;
							if (isSprinting) {
								fatigue += fatiguePerSprintTick;
								if (staminupActive)
									fatigue = 0;
							} else {
								fatigue += fatiguePerWalkTick;
								if (staminupActive)
									fatigue = 0;
							}
						}
					}
				} else {
					if (ladderState) {
						// Climbing off the ground
						//rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio * Time.deltaTime, ladderSpeed * Input.GetAxis("Vertical") * walkAcceleration * Time.deltaTime, 0);
						rbody.AddRelativeForce (relSideways * walkAcceleration * walkAccelAirRatio * Time.deltaTime, ladderSpeed * relForward * walkAcceleration * Time.deltaTime, 0);
					} else {
						// Sprinting in the air
						if (isSprinting && running && !inCyberSpace && !CheatNoclip) {
							//rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime);
							rbody.AddRelativeForce (relSideways * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime, 0, relForward * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime);
						} else {
                            // Walking in the air, we're floating in the moonlit sky, the people far below are sleeping as we fly
                            //rbody.AddRelativeForce(Input.GetAxis("Horizontal") * walkAcceleration * walkAccelAirRatio *  Time.deltaTime, 0, Input.GetAxis("Vertical") * walkAcceleration * walkAccelAirRatio * Time.deltaTime);
                            if (CheatNoclip) {
                                //rbody.AddRelativeForce(relSideways * walkAcceleration * 1.5f * Time.deltaTime, 0, relForward * walkAcceleration * 1.5f * Time.deltaTime);
                            } else {
                                rbody.AddRelativeForce(relSideways * walkAcceleration * walkAccelAirRatio * Time.deltaTime, 0, relForward * walkAcceleration * walkAccelAirRatio * Time.deltaTime);
                            }
                        }
					}
				}

				// Handle leaning, double tap to reset is handled in Update to prevent repeat calls within the same frame
				if (GetInput.a.LeanRight() && !leanRHReset) {
					leanTarget -= (leanSpeed * Time.deltaTime);
					if (leanTarget < -22.5f) leanTarget = -22.5f;
					leanRightDoubleFinished = Time.time;
				} else {
					if ((Time.time - leanRightDoubleFinished) > keyboardButtonDoubleTapTime) {
						leanRHReset = false;
						leanRHFirstPressed = false;
					}
				}

				if (GetInput.a.LeanLeft() && !leanLHReset) {
					leanTarget += (leanSpeed * Time.deltaTime);
					if (leanTarget > 22.5f) leanTarget = 22.5f;
					leanLeftDoubleFinished = Time.time;
				} else {
					if ((Time.time - leanLeftDoubleFinished) > keyboardButtonDoubleTapTime) {
						leanLHReset = false;
						leanLHFirstPressed = false;
					}
				}

				leanTransform.localRotation = Quaternion.Euler(0, 0, leanTarget);

                // Handle gravity and ladders
                if (ladderState) {
					rbody.useGravity = false;
					// Set vertical velocity towards 0 when climbing
					if (hwc.hardwareIsActive [9]) {
						deceleration = walkDeaccelerationBooster;
					} else {
						deceleration = walkDeacceleration;
					}
                    RigidbodySetVelocityY(rbody, (Mathf.SmoothDamp(rbody.velocity.y, 0, ref walkDeaccelerationVoly, deceleration)));
				} else {
					// Check if using a gravity lift
					if (gravliftState == true || CheatNoclip) {
						rbody.useGravity = false;
					} else {
						// Disables gravity when touching the ground to prevent player sliding down ramps...hacky?
						if (grounded == true) {
							rbody.useGravity = false;
						} else {
							if (!inCyberSpace)
								rbody.useGravity = true;
						}
					}
					// Apply gravity - OBSOLETE: Now handled by gravity of the RigidBody physics system
					//rbody.AddForce(0, (-1 * playerGravity * Time.deltaTime), 0); //Apply gravity force
				}

				// Get input for Jump and set impulse time, removed "&& (ladderState == false)" since I want to be able to jump off a ladder
				if (!inCyberSpace && (GetInput.a.Jump() && !consoleActivated) && !CheatNoclip && !justJumped) {
					if (grounded || gravliftState || (hwc.hardwareIsActive[10])) {
						jumpTime = jumpImpulseTime;
						justJumped = true;
					}
				}
			
				// Perform Jump
				while (jumpTime > 0) {
					jumpTime -= Time.smoothDeltaTime;
					if (fatigue > 80 && !(hwc.hardwareIsActive[10])) {
						rbody.AddForce (new Vector3 (0, jumpVelocityFatigued * rbody.mass, 0), ForceMode.Force);  // huhnh!
					} else {
						if (hwc.hardwareIsActive [10]) {
							rbody.AddForce (new Vector3 (0, jumpVelocityBoots * rbody.mass, 0), ForceMode.Force);  // huhnh!
						} else {
							rbody.AddForce (new Vector3 (0, jumpVelocity * rbody.mass, 0), ForceMode.Force);  // huhnh!
						}
					}
				}

				//if (jumpTime < 0) justJumped = false; // just in case 

				if (justJumped && !(hwc.hardwareIsActive[10])) {
					// Play jump sound
					if (fatigue > 80) {
						// quiet, we are tired
						if (jumpSFXFinished < Time.time) {
							jumpSFXFinished = Time.time + jumpSFXIntervalTime;
							PlayerNoise.PlayOneShot(SFXJump,0.5f);
						}
					} else {
						if (jumpSFXFinished < Time.time) {
							jumpSFXFinished = Time.time + jumpSFXIntervalTime;
							PlayerNoise.PlayOneShot(SFXJump);
						}
					}
					justJumped = false;
					fatigue += jumpFatigue;
					if (staminupActive)
						fatigue = 0;
				}

				// Handle fall damage (no impact damage in cyber space 5/5/18, JJ)
				if (Mathf.Abs ((oldVelocity.y - rbody.velocity.y)) > fallDamageSpeed && !inCyberSpace && !CheatNoclip) {
					DamageData dd = new DamageData ();
					dd.damage = fallDamage; // No neead for GetDamageTakeAmount since this is strictly internal to Player
					dd.attackType = Const.AttackType.None;
					dd.offense = 0f;
					dd.isOtherNPC = false;
					GetComponent<HealthManager>().TakeDamage (dd);  // was previously referring to the no longer used TakeDamage within PlayerHealth.cs
				}
				oldVelocity = rbody.velocity;

				// Automatically set grounded to false to prevent ability to climb any wall
				if (CheatWallSticky == false || gravliftState)
					grounded = false;
			} else {
				// Handle cyberspace movement
				if (GetInput.a.Forward() && !consoleActivated) {
					//Vector3 tempvec = mlookScript.cyberLookDir;
					rbody.AddForce (cameraObject.transform.forward * walkAcceleration * Time.deltaTime);
					//rbody.AddRelativeForce (relSideways * walkAcceleration * Time.deltaTime, 0, relForward * walkAcceleration * Time.deltaTime);
				}
			}
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
		if (!PauseScript.a.Paused() && !inCyberSpace) {
			foreach(ContactPoint contact in collision.contacts) {
				if (Vector3.Angle(contact.normal,Vector3.up) < maxSlope) {
					grounded = true;
				}
			}
		}
	}

	// Reset grounded to false when player is mid-air
	void OnCollisionExit (){
		if (!PauseScript.a.Paused()) {
			// Automatically set grounded to false to prevent ability to climb any wall (Cheat!)
			if (CheatWallSticky == true) {
				grounded = false;
			}
		}
	}

    private void ToggleConsole() {
		if (consoleActivated) {
			consoleActivated = false;
			consoleplaceholderText.SetActive(false);
			consoleTitle.SetActive(false);
			consoleinpFd.DeactivateInputField();
			consoleinpFd.enabled = false;
			consolebg.enabled = false;
			consoleentryText.text = null;
			consoleentryText.enabled = false;
			PauseScript.a.PauseDisable();
		} else {
			consoleActivated = true;
			consoleplaceholderText.SetActive(true);
			consoleTitle.SetActive(true);
			consoleinpFd.enabled = true;
			consoleinpFd.ActivateInputField();
			consolebg.enabled = true;
			consoleentryText.enabled = true;
			PauseScript.a.PauseEnable();
		}
    }

    public void ConsoleEntry() {
        if (consoleinpFd.text == "noclip") {
			if (CheatNoclip) {
				CheatNoclip = false;
				rbody.useGravity = true;
				capsuleCollider.enabled = true;
				leanCapsuleCollider.enabled = true;
				Const.sprint("Noclip disabled", Const.a.allPlayers);
			} else {
				CheatNoclip = true;
				rbody.useGravity = false;
				capsuleCollider.enabled = false;
				leanCapsuleCollider.enabled = false;
				Const.sprint("Noclip activated!", Const.a.allPlayers);
			}
        } else if (consoleinpFd.text == "notarget" || consoleinpFd.text == "NOTARGET" || consoleinpFd.text == "Notarget") {
			if (Notarget) {
				Notarget = false;
				Const.sprint("Notarget disabled", Const.a.allPlayers);
			} else {
				Notarget = true;
				Const.sprint("Notarget activated!", Const.a.allPlayers);
			}
        } else if (consoleinpFd.text == "god") {
			if (GetComponent<HealthManager>().god) {
				Const.sprint("God mode disabled", Const.a.allPlayers);
				GetComponent<HealthManager>().god = false;
			} else {
				Const.sprint("God mode activated!", Const.a.allPlayers);
				GetComponent<HealthManager>().god = true;
			}
        } else if (consoleinpFd.text == "bottomlessclip") {
			if (wepCur.bottomless) {
				Const.sprint("Hose disconnected, normal ammo operation restored", Const.a.allPlayers);
				wepCur.bottomless = false;
			} else {
				Const.sprint("Bottomless clip!  Bring it!", Const.a.allPlayers);
				wepCur.bottomless = true;
			}
        } else if (consoleinpFd.text == "ifeelthepower") {
			if (wepCur.redbull) {
				Const.sprint("Energy usage normal", Const.a.allPlayers);
				wepCur.redbull = false;
			} else {
				Const.sprint("I feel the power! 0 energy consumption!", Const.a.allPlayers);
				wepCur.redbull = true;
			}
        } else if (consoleinpFd.text == "showfps") {
			Const.sprint("Toggling FPS counter for framerate (bottom right corner)...", Const.a.allPlayers);
			fpsCounter.SetActive(!fpsCounter.activeInHierarchy);
		} else if (consoleinpFd.text == "iamshodan") {
			if (LevelManager.a.superoverride) {
				Const.sprint("SHODAN has regained control of security", Const.a.allPlayers);
				LevelManager.a.superoverride = false;
			} else {
				Const.sprint("Full security override enabled!", Const.a.allPlayers);
				LevelManager.a.superoverride = true;
			}
			Const.sprint("Full security override enabled!", Const.a.allPlayers);
		} else {
            Const.sprint("Uknown command or function: " + consoleinpFd.text, Const.a.allPlayers);
        }

        // Reset console and hide it, command was entered
        consoleinpFd.text = "";
        ToggleConsole();
    }
}