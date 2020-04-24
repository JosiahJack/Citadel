using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour {
	private float walkAcceleration = 2000f;
	private float walkDeacceleration = 0.30f;
	private float walkDeaccelerationBooster = 2f;
	private float deceleration;
	private float walkAccelAirRatio = 0.6f;
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
	public HardwareInventory hwi;
	public HardwareButton hwbJumpJets;
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
	private float jumpJetEnergySuckTickFinished;
	public float jumpJetEnergySuckTick = 1f;
	private Vector3 tempVec;
	private float tempFloat;
	private int tempInt;
	public float leanSpeed = 5f;
	public bool leanLHFirstPressed = false;
	public bool leanRHFirstPressed = false;
	public bool leanLHReset = false;
	public bool leanRHReset = false;
	public bool Notarget = false; // for cheat to disable enemy sight checks against this player
	public GameObject fpsCounter;
	public WeaponCurrent wepCur;
	private bool fatigueWarned;
	private PlayerEnergy pe;
	[HideInInspector]
	public float ressurectingFinished;
	public float burstForce = 50f;
	private float doubleJumpFinished;

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
		pe = GetComponent<PlayerEnergy>();	
		jumpJetEnergySuckTickFinished = Time.time;
		ressurectingFinished = Time.time;
		tempInt = -1;
		tempFloat = 0;
		doubleJumpFinished = Time.time;
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

		if (Input.GetKeyDown(KeyCode.Return) && !mainMenu.activeSelf && consoleActivated) {
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
		if (!PauseScript.a.Paused() && (ressurectingFinished < Time.time)) {
			rbody.WakeUp();

			//LevelManager.a.SetLeaf(transform.position); // hey we are here, see if and set we are in a leaf for this level

			if (inCyberSpace && !cyberSetup) {
				cyberCollider.enabled = true;
				capsuleCollider.enabled = false;
				rbody.useGravity = false;
				mlookScript.inCyberSpace = true; // enable full camera rotation up/down by disabling clamp
				oldBodyState = bodyState;
				bodyState = defIndex; // reset to "standing" to prevent speed anomolies
				// UPDATE enable dummy player for coop games
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
                                              // UPDATE disable dummy player for coop games  dummyPlayerModel.SetActive(false); dummyPlayerCapsule.enabled = false;
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
                                              // UPDATE disable dummy player for coop games
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
			Const.sprint("BUG: PlayerMovement: PauseScript is null",transform.parent.gameObject);
			return;
		}

		if (!PauseScript.a.Paused() && ressurectingFinished < Time.time) {
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
						playerSpeed = maxWalkSpeed + bonus;
						break;
					case 1:
						//Debug.Log("Crouched");
						playerSpeed = maxCrouchSpeed + bonus;
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
						playerSpeed = maxProneSpeed + bonus;
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
							if (bodyState == 1 || bodyState == 2 || bodyState == 3) {
								playerSpeed -= (maxWalkSpeed - maxCrouchSpeed);  // subtract off the difference in speed between walking and crouching from the sprint speed
							}
							if (bodyState == 4 || bodyState == 5 || bodyState == 6) {
								playerSpeed -= (maxWalkSpeed - maxProneSpeed);  // subtract off the difference in speed between walking and proning from the sprint speed
							}
						} else {
							playerSpeed = maxSprintSpeed + bonus;
							if (bodyState == 1 || bodyState == 2 || bodyState == 3) {
								playerSpeed -= (maxWalkSpeed - maxCrouchSpeed);  // subtract off the difference in speed between walking and crouching from the sprint speed
							}
							if (bodyState == 4 || bodyState == 5 || bodyState == 6) {
								playerSpeed -= (maxWalkSpeed - maxProneSpeed);  // subtract off the difference in speed between walking and proning from the sprint speed
							}
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

				// Ground friction
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
				
			// Set rotation of playercapsule from mouselook script
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
				if (grounded == true || CheatNoclip ||  (hwc.hardwareIsActive[10])) {
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
                                rbody.AddRelativeForce(0, 4f * walkAcceleration * Time.deltaTime, 0);
                            }
                        }

                        if (GetInput.a.SwimDn() && !consoleActivated) {
                            if (CheatNoclip) {
                                //Debug.Log("Floating Dn!");
                                rbody.AddRelativeForce(0, 4f * walkAcceleration * Time.deltaTime * -1, 0);
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
					if (hwc.hardwareIsActive [9] && hwi.hardwareVersionSetting[9] == 0) {
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
				 
				if (!inCyberSpace && !consoleActivated && !CheatNoclip) {
					if (GetInput.a.Jump()) {
						if (!justJumped) {
							//if (gravliftState || (hwc.hardwareIsActive[10])) {
								if (grounded || gravliftState || hwc.hardwareIsActive[10]) {
									jumpTime = jumpImpulseTime;
									doubleJumpFinished = Time.time + Const.a.doubleClickTime;
									justJumped = true;
								} else {
									if (ladderState) {
										jumpTime = jumpImpulseTime;
										justJumped = true;
									}
								}
							//}
						}

						if (hwc.hardwareIsActive [9] && hwi.hardwareVersionSetting[9] == 1) {
							if (doubleJumpFinished < Time.time){
								rbody.AddForce(new Vector3(transform.forward.x * burstForce,transform.forward.y * burstForce,transform.forward.z * burstForce),ForceMode.Impulse);
								pe.TakeEnergy(40f);
							}
						}
					} 
				}
			
				// Perform Jump
				while (jumpTime > 0) {
					jumpTime -= Time.smoothDeltaTime;
					if (fatigue > 80 && !(hwc.hardwareIsActive[10])) {
						rbody.AddForce (new Vector3 (0, jumpVelocityFatigued * rbody.mass, 0), ForceMode.Force);  // huhnh!
					} else {
						if (hwc.hardwareIsActive [10]) {
							if (pe.energy > 11f) {
								rbody.AddForce (new Vector3 (0, jumpVelocityBoots * rbody.mass, 0), ForceMode.Force);  // huhnh!
								float energysuck = 25f;
								switch (hwi.hardwareVersion[10]) {
									case 0: energysuck = 25f; break;
									case 1: energysuck = 30f; break;
									case 2: energysuck = 35f; break;
								}
								if (jumpJetEnergySuckTickFinished < Time.time) {
									jumpJetEnergySuckTickFinished = Time.time + jumpJetEnergySuckTick;
									pe.TakeEnergy(energysuck);
								}
							} else {
								hwbJumpJets.JumpJetsOff();
							}
						} else {
							if (ladderState) {
								Vector3 jumpDir = transform.forward * jumpVelocity * rbody.mass;
								rbody.AddForce (jumpDir, ForceMode.Force);  // jump off ladder in direction of player facing
							} else {
								rbody.AddForce (new Vector3 (0, jumpVelocity * rbody.mass, 0), ForceMode.Force);  // huhnh!
							}
						}
					}
				}

				if (jumpTime < 0) justJumped = false; // for jump jets to work 

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
			tempFloat = 0;
			//foreach(ContactPoint contact in collision.contacts) {
			for(tempInt=0;tempInt<collision.contacts.Length;tempInt++) {
				//if (Vector3.Angle(contact.normal,Vector3.up) < maxSlope) {
				tempFloat = Vector3.Dot(collision.contacts[tempInt].normal,Vector3.up);
				//Debug.Log("Contact.normal for player OnCollisionStay is " + ang.ToString());
				if (tempFloat <= 1 && tempFloat >= 0.35) {
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

	public void ConsoleDisable() {
		consoleActivated = false;
		consoleplaceholderText.SetActive(false);
		consoleTitle.SetActive(false);
		consoleinpFd.DeactivateInputField();
		consoleinpFd.enabled = false;
		consolebg.enabled = false;
		consoleentryText.text = null;
		consoleentryText.enabled = false;
	}

	void ConsoleEnable() {
		consoleActivated = true;
		consoleplaceholderText.SetActive(true);
		consoleTitle.SetActive(true);
		consoleinpFd.enabled = true;
		consoleinpFd.ActivateInputField();
		consolebg.enabled = true;
		consoleentryText.enabled = true;
	}

    void ToggleConsole() {
		if (consoleActivated) {
			ConsoleDisable();
			PauseScript.a.PauseDisable();
		} else {
			ConsoleEnable();
			PauseScript.a.PauseEnable();
		}
    }

	// CHEAT CODES you cheaty cheatface you
    public void ConsoleEntry() {
        if (consoleinpFd.text == "noclip" || consoleinpFd.text == "NOCLIP" || consoleinpFd.text == "Noclip" || consoleinpFd.text == "nOCLIP" || consoleinpFd.text == "idclip" || consoleinpFd.text == "IDCLIP") {
			if (CheatNoclip) {
				CheatNoclip = false;
				rbody.useGravity = true;
				capsuleCollider.enabled = true;
				leanCapsuleCollider.enabled = true;
				Const.sprint("noclip disabled", Const.a.allPlayers);
			} else {
				CheatNoclip = true;
				rbody.useGravity = false;
				capsuleCollider.enabled = false;
				leanCapsuleCollider.enabled = false;
				Const.sprint("noclip activated!", Const.a.allPlayers);
			}
        } else if (consoleinpFd.text == "notarget" || consoleinpFd.text == "NOTARGET" || consoleinpFd.text == "Notarget" || consoleinpFd.text == "nOTARGET" || consoleinpFd.text == "no target") {
			if (Notarget) {
				Notarget = false;
				Const.sprint("notarget disabled", Const.a.allPlayers);
			} else {
				Notarget = true;
				Const.sprint("notarget activated!", Const.a.allPlayers);
			}
        } else if (consoleinpFd.text == "god" || consoleinpFd.text == "GOD" || consoleinpFd.text == "God"  || consoleinpFd.text == "gOD" || consoleinpFd.text == "power overwhelming" || consoleinpFd.text == "POWER OVERWHELMING" || consoleinpFd.text == "poweroverwhelming" || consoleinpFd.text == "POWEROVERWHELMING" || consoleinpFd.text == "WhosYourDaddy" || consoleinpFd.text == "WHOSYOURDADDY" || consoleinpFd.text == "wHOSyOURdADDY" || consoleinpFd.text == "iddqd") {
			if (GetComponent<HealthManager>().god) {
				Const.sprint("god mode disabled", Const.a.allPlayers);
				GetComponent<HealthManager>().god = false;
			} else {
				Const.sprint("god mode activated!", Const.a.allPlayers);
				GetComponent<HealthManager>().god = true;
			}
        } else if (consoleinpFd.text == "load0" || consoleinpFd.text == "LOAD0" || consoleinpFd.text == "Load0") {
			LevelManager.a.LoadLevel(0,LevelManager.a.ressurectionLocation[LevelManager.a.currentLevel].gameObject,Const.a.player1);
        } else if (consoleinpFd.text == "load1" || consoleinpFd.text == "LOAD1" || consoleinpFd.text == "Load1") {
			LevelManager.a.LoadLevel(1,LevelManager.a.ressurectionLocation[LevelManager.a.currentLevel].gameObject,Const.a.player1);
        } else if (consoleinpFd.text == "load2" || consoleinpFd.text == "LOAD2" || consoleinpFd.text == "Load2") {
			LevelManager.a.LoadLevel(2,LevelManager.a.ressurectionLocation[LevelManager.a.currentLevel].gameObject,Const.a.player1);
        } else if (consoleinpFd.text == "load3" || consoleinpFd.text == "LOAD3" || consoleinpFd.text == "Load3") {
			LevelManager.a.LoadLevel(3,LevelManager.a.ressurectionLocation[LevelManager.a.currentLevel].gameObject,Const.a.player1);
        } else if (consoleinpFd.text == "load4" || consoleinpFd.text == "LOAD4" || consoleinpFd.text == "Load4") {
			LevelManager.a.LoadLevel(4,LevelManager.a.ressurectionLocation[LevelManager.a.currentLevel].gameObject,Const.a.player1);
        } else if (consoleinpFd.text == "load5" || consoleinpFd.text == "LOAD5" || consoleinpFd.text == "Load5") {
			LevelManager.a.LoadLevel(5,LevelManager.a.ressurectionLocation[LevelManager.a.currentLevel].gameObject,Const.a.player1);
        } else if (consoleinpFd.text == "load6" || consoleinpFd.text == "LOAD6" || consoleinpFd.text == "Load6") {
			LevelManager.a.LoadLevel(6,LevelManager.a.ressurectionLocation[LevelManager.a.currentLevel].gameObject,Const.a.player1);
        } else if (consoleinpFd.text == "load7" || consoleinpFd.text == "LOAD7" || consoleinpFd.text == "Load7") {
			LevelManager.a.LoadLevel(7,LevelManager.a.ressurectionLocation[LevelManager.a.currentLevel].gameObject,Const.a.player1);
        } else if (consoleinpFd.text == "bottomlessclip" || consoleinpFd.text == "BOTTOMLESSCLIP"  || consoleinpFd.text == "Bottomlessclip" || consoleinpFd.text == "bOTTOMLESSCLIP" || consoleinpFd.text == "bottomless clip" || consoleinpFd.text == "BOTTOMLESS CLIP") {
			if (wepCur.bottomless) {
				Const.sprint("Hose disconnected, normal ammo operation restored", Const.a.allPlayers);
				wepCur.bottomless = false;
			} else {
				Const.sprint("bottomlessclip!  Bring it!", Const.a.allPlayers);
				wepCur.bottomless = true;
			}
        } else if (consoleinpFd.text == "ifeelthepower" || consoleinpFd.text == "IFEELTHEPOWER" || consoleinpFd.text == "Ifeelthepower" || consoleinpFd.text == "iFEELTHEPOWER") {
			if (wepCur.redbull) {
				Const.sprint("Energy usage normal", Const.a.allPlayers);
				wepCur.redbull = false;
			} else {
				Const.sprint("I feel the power! 0 energy consumption!", Const.a.allPlayers);
				wepCur.redbull = true;
			}
        } else if (consoleinpFd.text == "showfps" || consoleinpFd.text == "SHOWFPS" || consoleinpFd.text == "show fps" || consoleinpFd.text == "cl_showfps 1" || consoleinpFd.text == "r_showfps 1"  || consoleinpFd.text == "Showfps" || consoleinpFd.text == "sHOWFPS" || consoleinpFd.text == "show_fps 1") {
			Const.sprint("Toggling FPS counter for framerate (bottom right corner)...", Const.a.allPlayers);
			fpsCounter.SetActive(!fpsCounter.activeInHierarchy);
		} else if (consoleinpFd.text == "iamshodan" || consoleinpFd.text == "IAMSHODAN" || consoleinpFd.text == "Iamshodan" || consoleinpFd.text == "iAMSHODAN" || consoleinpFd.text == "I AM SHODAN" || consoleinpFd.text == "i am shodan" || consoleinpFd.text == "I am shodan" || consoleinpFd.text == "I am SHODAN"  || consoleinpFd.text == "I am Shodan") {
			if (LevelManager.a.superoverride) {
				Const.sprint("SHODAN has regained control of security from you", Const.a.allPlayers);
				LevelManager.a.superoverride = false;
			} else {
				Const.sprint("Full security override enabled!", Const.a.allPlayers);
				LevelManager.a.superoverride = true;
			}
		} else if (consoleinpFd.text == "Mr. Bean") {
				Const.sprint("Nice try, there are no go carts to slow down here", Const.a.allPlayers);
		} else if (consoleinpFd.text == "Simon Foster") {
				Const.sprint("Nice try, nothing to paint here", Const.a.allPlayers);
		} else if (consoleinpFd.text == "Motherlode" || consoleinpFd.text == "Rosebud" || consoleinpFd.text == "Kaching" || consoleinpFd.text == "money") {
				Const.sprint("Nice try, there's no money here.", Const.a.allPlayers);
		} else if (consoleinpFd.text == "Richard Branson") {
				Const.sprint("Nice try, there's no money here.  You do realize this isn't Rollercoaster Tycoon right?", Const.a.allPlayers);
		} else if (consoleinpFd.text == "John Wardley") {
				Const.sprint("WOW!", Const.a.allPlayers);
		} else if (consoleinpFd.text == "John Mace") {
				Const.sprint("Nice try, there's nothing to pay double for here", Const.a.allPlayers);
		} else if (consoleinpFd.text == "Melanie Warn") {
				Const.sprint("I feel happy!!!", Const.a.allPlayers);
		} else if (consoleinpFd.text == "Damon Hill") {
				Const.sprint("Nice try, there are no go carts to speed up here", Const.a.allPlayers);
		} else if (consoleinpFd.text == "Michael Schumacher") {
				Const.sprint("Nice try, there are no go carts to give ludicrous speed here", Const.a.allPlayers);
		} else if (consoleinpFd.text == "Tony Day") {
				Const.sprint("Ok, now I want a hamburger", Const.a.allPlayers);
		} else if (consoleinpFd.text == "Katie Brayshaw") {
				Const.sprint("Hi there! Hello! Hey! Howdy!", Const.a.allPlayers);
		} else if (consoleinpFd.text == "sudo" || consoleinpFd.text == "sudo app" || consoleinpFd.text == "sudo app get" || consoleinpFd.text == "sudo update") {
				Const.sprint("Super user access granted...ERROR: access restricted by SHODAN", Const.a.allPlayers);
		} else if (consoleinpFd.text == "restart") {
				Const.sprint("Yeah...better not", Const.a.allPlayers);
		} else if (consoleinpFd.text == "kill") {
				Const.sprint("Ok, give me a minute and I'll send a Cortex Reaver to help with that", Const.a.allPlayers);
		} else if (consoleinpFd.text == "kill me") {
				Const.sprint("Ok, give me a minute and I'll send a Cortex Reaver to help with that", Const.a.allPlayers);
		} else if (consoleinpFd.text == "justinbailey" || consoleinpFd.text == "JUSTINBAILEY") {
				Const.sprint("Well, you don't have a suit already so...", Const.a.allPlayers);
		} else if (consoleinpFd.text == "woodstock" || consoleinpFd.text == "WOODSTOCK") {
				Const.sprint("How much wood could a woodchuck chuck...there's no wood in SPACE!", Const.a.allPlayers);
		} else if (consoleinpFd.text == "quarry" || consoleinpFd.text == "QUARRY") {
				Const.sprint("There's obsidian on levels 6 and 8 if want to feel decadant, otherwise we are lacking in the stone department.", Const.a.allPlayers);
		} else if (consoleinpFd.text == "help" || consoleinpFd.text == "HELP") {
				Const.sprint("There's no one to save you now Hacker!", Const.a.allPlayers);
		} else if (consoleinpFd.text == "zelda" || consoleinpFd.text == "ZELDA") {
				Const.sprint("Too late, already been to level 1", Const.a.allPlayers);
		} else if (consoleinpFd.text == "allyourbasearebelongtous" || consoleinpFd.text == "ALLYOURBASEAREBELONGTOUS") {
				Const.sprint("ERROR: SHODAN has overriden your command, remove SHODAN first.", Const.a.allPlayers);
		} else if (consoleinpFd.text == "IAmIronMan" || consoleinpFd.text == "iamironman" || consoleinpFd.text == "iaMiRONmAN") {
				Const.sprint("That's nice dear.", Const.a.allPlayers);
		} else if (consoleinpFd.text == "impulse 9" || consoleinpFd.text == "idkfa" || consoleinpFd.text == "IDKFA" || consoleinpFd.text == "IMPULSE 9") {
				Const.sprint("I can only hold 7 weapons!! Nice try dearies!", Const.a.allPlayers);
		} else if (consoleinpFd.text == "summon_obj" || consoleinpFd.text == "SUMMON_OBJ") {
				Const.sprint("What do I look like have that kind of developer time to copy System Shock 2??", Const.a.allPlayers); // hmm...had time for all that junk up there didn't I
		} else {
            Const.sprint("Uknown command or function: " + consoleinpFd.text, Const.a.allPlayers);
        }

        // Reset console and hide it, command was entered
        consoleinpFd.text = "";
        ToggleConsole();
    }
}