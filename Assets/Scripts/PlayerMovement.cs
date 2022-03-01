using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerMovement : MonoBehaviour {
	// External references, required
	public GameObject cameraObject;
	public MouseLookScript mlookScript;
	public Camera automapCamera;
	public GameObject automapContainerLH;
	public GameObject automapContainerRH;
	public GameObject automapTabLH;
	public GameObject automapTabRH;
	public Transform automapCameraTransform;
	public Image[] automapFoWTiles;
	public RectTransform[] automapFoWTilesRects;
	public Image automapBaseImage;
	public Image automapInnerCircle;
	public Image automapOuterCircle;
	public Sprite[] automapsBaseImages;	
	public Image[] automapsHazardOverlays;
	public Transform automapFullPlayerIcon;
	public Transform automapNormalPlayerIconLH;
	public Transform automapNormalPlayerIconRH;
	public Transform cheatG1Spawn;
	public Transform cheatG2Spawn;
	public Transform cheatG4Spawn;
	public GameObject cheatL1arsenal;
	public GameObject cheatLRarsenal;
	public GameObject cheatL2arsenal;
	public GameObject cheatL3arsenal;
	public GameObject cheatL4arsenal;
	public GameObject cheatL5arsenal;
	public GameObject cheatL6arsenal;
	public GameObject cheatL7arsenal;
	public GameObject cheatL8arsenal;
	public GameObject cheatL9arsenal;
	public GameObject levelOverlayContainerR;
	public GameObject levelOverlayContainer1;
	public GameObject levelOverlayContainer2;
	public GameObject levelOverlayContainer3;
	public GameObject levelOverlayContainer4;
	public GameObject levelOverlayContainer5;
	public GameObject levelOverlayContainer6;
	public GameObject levelOverlayContainer7;
	public GameObject levelOverlayContainer8;
	public GameObject levelOverlayContainer9;
	public GameObject levelOverlayContainerG1;
	public GameObject levelOverlayContainerG2;
	public GameObject levelOverlayContainerG4;
	public Vector2[] automapLevelHomePositions; // R= 43.97, 85.66 | 1= -8.53, 85.99 | 2= 10.2, 44.8 | 3= 9.4, 63.83 | 4= -55.65, 116.8 | 5= -9.4, 71.8 | 6= 29.7, 85.5 | 7= 5, 76.55 | 8= 25.1, 84.4 | 9= 39.8, 72.6
	public HardwareButton hwbJumpJets;
	public TextWarningsManager twm;
	public CapsuleCollider leanCapsuleCollider;
	public Image consolebg;
    public InputField consoleinpFd;
    public GameObject consoleplaceholderText;
	public GameObject consoleTitle;
	public Text consoleentryText;
	public Transform leanTransform;
	public AudioSource PlayerNoise;
	public AudioClip SFXJump;
	public AudioClip SFXJumpLand;
	public AudioClip SFXLadder;
	public GameObject fpsCounter;
	public GameObject locationIndicator;
	public Text locationText;
	public WeaponCurrent wepCur;
	public HealthManager hm;
	public GameObject poolContainerAutomapBotOverlays;
	public GameObject poolContainerAutomapMutantOverlays;
	public GameObject poolContainerAutomapCyborgOverlays;

	// Public values, visible for the purpose of only assigning size to the arrays of 4096.
	public bool[] automapExploredR; // save
	public bool[] automapExplored1; // save
	public bool[] automapExplored2; // save
	public bool[] automapExplored3; // save
	public bool[] automapExplored4; // save
	public bool[] automapExplored5; // save
	public bool[] automapExplored6; // save
	public bool[] automapExplored7; // save
	public bool[] automapExplored8; // save
	public bool[] automapExplored9; // save
	public bool[] automapExploredG1; // save
	public bool[] automapExploredG2; // save
	public bool[] automapExploredG4; // save

	// Internal references
	[HideInInspector] public float playerSpeed; // save
	[HideInInspector] public int bodyState = 0; // save
	[HideInInspector] public bool isSprinting = false;
	[HideInInspector] public bool grounded = false; // save
	[HideInInspector] public bool ladderState = false; // save
	[HideInInspector] public bool gravliftState = false; // save
	[HideInInspector] public bool inCyberSpace = false; // save
	[HideInInspector] public bool inFullMap;
	[HideInInspector] public float walkAcceleration = 2000f;
	private float walkDeacceleration = 0.1f; // was 0.30f
	private float walkDeaccelerationBooster = 1f; // was 2f, adjusted player physics material to reduce friction for moving up stairs
	private float deceleration;
	private float walkAccelAirRatio = 0.75f;
	private float maxWalkSpeed = 3.2f;
	private float maxCyberSpeed = 5f;
	private float maxCyberUltimateSpeed = 12f;
	private float maxCrouchSpeed = 1.25f; //1.75f
	private float maxProneSpeed = .5f; //1f
	private float maxSprintSpeed = 8.8f;
	private float maxSprintSpeedFatigued = 5.5f;
	private float maxVerticalSpeed = 5f;
	private float boosterSpeedBoost = 0.5f; // ammount to boost by when booster is active
	private float jumpImpulseTime = 2.0f;
	private float jumpVelocityBoots = 0.5f;
	private float jumpVelocity = 1.1f;
	private float jumpVelocityFatigued = 0.6f;
	private float crouchRatio = 0.6f;
	private float proneRatio = 0.2f;
	private float transitionToCrouchSec = 0.2f;
	private float transitionToProneAdd = 0.1f;
	[HideInInspector] public float currentCrouchRatio = 1f; // save
	private float capsuleHeight;
	private float capsuleRadius;
	private float ladderSpeed = 0.4f;
	private float fallDamage = 75f;
	private float automapUpdateFinished;
	private bool[] automapExplored;
	private float automapZoom0 = 1.2f;
	private float automapZoom1 = 0.75f;
	private float automapZoom2 = 0.55f;
	private int currentAutomapZoomLevel = 0;
	// private float circleInnerRangev1 = 7.679999f; //(2.5f * 2.56f) + 1.28f;
	// private float circleOuterRangev1 = 11.52f; //(4f * 2.56f) + 1.28f;
	// private float circleInnerRangev2 = 8.96f; //(3f * 2.56f) + 1.28f;
	// private float circleOuterRangev2 = 12.8f; //(4.5f * 2.56f) + 1.28f;
	// private float circleInnerRangev3 = 14.08f; //(5f * 2.56f) + 1.28f;
	// private float circleOuterRangev3 = 20.48f; //(7.5f * 2.56f) + 1.28f;
	// private float automapFactorx = 1.25f;
	// private float automapFactory = 1.135f;
	[HideInInspector] public bool CheatWallSticky; // save
    [HideInInspector] public bool CheatNoclip; // save
    [HideInInspector] public bool staminupActive = false;
	private Vector2 horizontalMovement;
	private float verticalMovement;
	[HideInInspector] public float jumpTime; // save
	private float crouchingVelocity = 1f;
	private float lastCrouchRatio;
	private int layerGeometry = 9;
	private int layerMask;
	[HideInInspector] public Rigidbody rbody;
	private float fallDamageSpeed = 11.72f;
	[HideInInspector] public Vector3 oldVelocity; // save
	[HideInInspector] public float fatigue; // save
	private float jumpFatigue = 8.25f;
	private float fatigueWanePerTick = 1f;
	private float fatigueWanePerTickCrouched = 2f;
	private float fatigueWanePerTickProne = 3.5f;
	private float fatigueWaneTickSecs = 0.3f;
	private float fatiguePerWalkTick = 0.9f;
	private float fatiguePerSprintTick = 3.0f;
	[HideInInspector] public bool justJumped = false; // save
	[HideInInspector] public float fatigueFinished; // save
	[HideInInspector] public float fatigueFinished2; // save
	private int defIndex = 0;
	private int def1 = 1;
	private int onehundred = 100;
	private bool running = false;
	private float relForward = 0f;
	private float relSideways = 0f;
	[HideInInspector] public bool cyberSetup = false; // save
	[HideInInspector] public bool cyberDesetup = false; // save
	private SphereCollider cyberCollider;
	private CapsuleCollider capsuleCollider;
	[HideInInspector] public int oldBodyState; // save
	private float bonus;
    private float walkDeaccelerationVolx;
    private float walkDeaccelerationVoly;
    private float walkDeaccelerationVolz;
	[HideInInspector] public bool consoleActivated; // save
	[HideInInspector] public float leanTarget = 0f; // save
	[HideInInspector] public float leanShift = 0f; // save
	private float leanMaxAngle = 35f;
	private float leanMaxShift = 0.48f;
	[HideInInspector] public float jumpSFXFinished; // save
	[HideInInspector] public float ladderSFXFinished;
	private float ladderSFXIntervalTime = 1f;
	private float jumpSFXIntervalTime = 1f;
	[HideInInspector] public float jumpLandSoundFinished; // save
	[HideInInspector] public float jumpJetEnergySuckTickFinished; // save
	private float jumpJetEnergySuckTick = 1f;
	private Vector3 tempVec;
	private Vector2 tempVec2;
	private Vector2 tempVec2b;
	private float tempFloat;
	private int tempInt;
	private float leanSpeed = 70f;
	[HideInInspector] public bool Notarget = false; // for cheat to disable enemy sight checks against this player
	[HideInInspector] public bool fatigueWarned; // save
	private PlayerEnergy pe;
	[HideInInspector] public float ressurectingFinished; // save
	private float burstForce = 35f;
	[HideInInspector] public float doubleJumpFinished; // save
	private Vector3 playerHome;
	private Texture2D tempTexture;
	[HideInInspector] public float turboFinished = 0f; // save
	[HideInInspector] public float turboCyberTime = 15f;
	private int doubleJumpTicks = 0;
	private float automapCorrectionX = -0.008f;
	private float automapCorrectionY = 0.099f;
	private float automapTileCorrectionX = -516;
	private float automapTileCorrectionY = -516;
	private float automapFoWRadius = 30f;
	private float automapTileBCorrectionX = 0f;
	private float automapTileBCorrectionY = 0f;
	private Vector3 tempVecRbody;
	private float automapPlayerIconZAdjusted;
	private bool inputtingMovement;
	private float updateTime;

    void Start (){
		currentCrouchRatio = def1;
		bodyState = defIndex;
		cyberDesetup = false;
		oldBodyState = bodyState;
		fatigueFinished = PauseScript.a.relativeTime;
		fatigueFinished2 = PauseScript.a.relativeTime;
		ladderSFXFinished = PauseScript.a.relativeTime;
		rbody = GetComponent<Rigidbody>();
		oldVelocity = rbody.velocity;
		capsuleCollider = GetComponent<CapsuleCollider>();
		capsuleHeight = capsuleCollider.height;
		capsuleRadius = capsuleCollider.radius;
		layerMask = def1 << layerGeometry;
		staminupActive = false;
		cyberCollider = GetComponent<SphereCollider>();
		consoleActivated = false;
		jumpLandSoundFinished = PauseScript.a.relativeTime;
		justJumped = false;
		jumpSFXFinished = PauseScript.a.relativeTime;
		fatigueWarned = false;
		pe = GetComponent<PlayerEnergy>();	
		jumpJetEnergySuckTickFinished = PauseScript.a.relativeTime;
		ressurectingFinished = PauseScript.a.relativeTime;
		tempInt = -1;
		tempFloat = 0;
		doubleJumpFinished = PauseScript.a.relativeTime;
		doubleJumpTicks = 0;
		turboFinished = PauseScript.a.relativeTime;
		playerHome = transform.localPosition;	
		automapExplored = new bool[4096];
		automapUpdateFinished = PauseScript.a.relativeTime;
		if (LevelManager.a != null)
			SetAutomapExploredReference(LevelManager.a.currentLevel);
		else
			SetAutomapExploredReference(1);
		AutomapZoomAdjust();
		automapPlayerIconZAdjusted = 0f;
    }
	
	void  Update() {
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
        if (GetInput.a.Console()) ToggleConsole();
		if (Input.GetKeyDown(KeyCode.Return) && !PauseScript.a.mainMenu.activeSelf == true && consoleActivated) ConsoleEntry();
		if (locationIndicator.activeInHierarchy) locationText.text = "location: " +(transform.position.x.ToString("00.00")+" "+transform.position.y.ToString("00.00")+" "+transform.position.z.ToString("00.00"));

		if (consoleActivated) {
			if (!String.IsNullOrEmpty(consoleentryText.text)) {
				if (consoleplaceholderText.activeSelf) consoleplaceholderText.SetActive(false);
			} else {
				if (!consoleplaceholderText.activeSelf) consoleplaceholderText.SetActive(true);
			}
		} else {
			if (consoleplaceholderText.activeSelf) consoleplaceholderText.SetActive(false);
		}

		if (PauseScript.a.mainMenu.activeSelf == true) {
			if (rbody.useGravity) rbody.useGravity = false;
			return;  // ignore movement when main menu is still up
		} else {
			if (!inCyberSpace && !rbody.useGravity) rbody.useGravity = true;
		}

		if (!PauseScript.a.Paused() && (ressurectingFinished < PauseScript.a.relativeTime)) {
			rbody.WakeUp();
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
						Const.sprint(Const.a.stringTable[187],Const.a.player1);
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
								Const.sprint(Const.a.stringTable[188],Const.a.player1);
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
							Const.sprint(Const.a.stringTable[187],Const.a.player1);
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
			if (fatigueFinished < PauseScript.a.relativeTime && !inCyberSpace && !CheatNoclip) {
				fatigueFinished = PauseScript.a.relativeTime + fatigueWaneTickSecs;
				switch (bodyState) {
				case 0: fatigue -= fatigueWanePerTick; break;
				case 1: fatigue -= fatigueWanePerTickCrouched; break;
				case 4: fatigue -= fatigueWanePerTickProne; break;
				default: fatigue -= fatigueWanePerTick; break;
				}
				if (fatigue < defIndex) fatigue = defIndex; // clamp at 0
			}
			if (fatigue > onehundred) fatigue = onehundred; // clamp at 100 using dummy variables to hang onto the value and not get collected by garbage collector (really?  hey it was back in the old days when we didn't have incremental garbage collector, pre Unity 2019.2 versions
			if (fatigue > 80 && !fatigueWarned && !inCyberSpace) {
				twm.SendWarning(("Fatigue high"),0.1f,0,TextWarningsManager.warningTextColor.white,324);
				fatigueWarned = true;
			} else {
				fatigueWarned = false;
			}
		}
	}

	void FixedUpdate() {
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive() || ressurectingFinished > PauseScript.a.relativeTime) return;

		if (capsuleCollider.height != (currentCrouchRatio * 2f)) capsuleCollider.height = currentCrouchRatio * 2f; // Crouch
		if (leanCapsuleCollider.height != capsuleCollider.height) leanCapsuleCollider.height = capsuleCollider.height;
		
		// Handle body state speeds and body position lerping for smooth transitions
		if (inCyberSpace) {
			playerSpeed = maxCyberSpeed; //Cyber space state
			if (rbody.velocity.magnitude > maxCyberUltimateSpeed) RigidbodySetVelocity(rbody, maxCyberUltimateSpeed); // Limit movement speed in all axes x,y,z in cyberspace
		} else {
			if (Inventory.a.hasHardware[1]) UpdateAutomap(); // Update the map
			bonus = 0f;
			if (Inventory.a.hardwareIsActive [9]) bonus = boosterSpeedBoost;
			switch (bodyState) {
				case 0: playerSpeed = maxWalkSpeed + bonus; break; // Standing
				case 1: playerSpeed = maxCrouchSpeed + bonus; break; // Crouched
				case 2: currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, -0.01f, ref crouchingVelocity, transitionToCrouchSec); break; // Crouching down
				case 3: // Standing up
					lastCrouchRatio = currentCrouchRatio;
					currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, 1.01f, ref crouchingVelocity, transitionToCrouchSec);
					LocalPositionSetY (transform, (((currentCrouchRatio - lastCrouchRatio) * capsuleHeight) / 2) + transform.position.y);
					break;
				case 4: playerSpeed = maxProneSpeed + bonus;  break; // Prone
				case 5: currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, -0.01f, ref crouchingVelocity, transitionToCrouchSec); break; // Proning down
				case 6: // Prone to crouch
					lastCrouchRatio = currentCrouchRatio;
					currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, 1.01f, ref crouchingVelocity, (transitionToCrouchSec + transitionToProneAdd));
					LocalPositionSetY (transform, (((currentCrouchRatio - lastCrouchRatio) * capsuleHeight) / 2) + transform.position.y);
					break;
			}

			// Limit movement speed horizontally for normal movement
			horizontalMovement = new Vector2(rbody.velocity.x, rbody.velocity.z);
			if (horizontalMovement.magnitude > playerSpeed) {
				horizontalMovement = horizontalMovement.normalized;
				if (isSprinting && running) {
					if (fatigue > 80f) {
						playerSpeed = maxSprintSpeedFatigued + bonus;
					} else {
						playerSpeed = maxSprintSpeed + bonus;
					}

					if (bodyState == 1 || bodyState == 2 || bodyState == 3) playerSpeed -= ((maxWalkSpeed - maxCrouchSpeed)*1.5f);  // subtract off the difference in speed between walking and crouching from the sprint speed
					else if (bodyState == 4 || bodyState == 5 || bodyState == 6) playerSpeed -= ((maxWalkSpeed - maxProneSpeed)*2f);  // subtract off the difference in speed between walking and proning from the sprint speed
				}
				if (CheatNoclip) playerSpeed = maxCyberSpeed*4f; // Cheat speed.
				horizontalMovement *= playerSpeed;  // cap velocity to max speed
			}

			// Set horizontal velocity
			RigidbodySetVelocityX(rbody, horizontalMovement.x);
			RigidbodySetVelocityZ(rbody, horizontalMovement.y); // NOT A BUG - already passed rbody.velocity.z into the .y of this Vector2

			// Set vertical velocity
			verticalMovement = rbody.velocity.y;
			if (verticalMovement > maxVerticalSpeed) verticalMovement = maxVerticalSpeed;
			if (!CheatNoclip) RigidbodySetVelocityY(rbody, verticalMovement);

			// Ground friction
			if (grounded) {
				tempVecRbody = rbody.velocity;
				deceleration = Inventory.a.hardwareIsActive[9] ? walkDeaccelerationBooster : walkDeacceleration;
				if (CheatNoclip) {
					deceleration = 0.05f;
					tempVecRbody.y = Mathf.SmoothDamp(rbody.velocity.y, 0, ref walkDeaccelerationVoly, deceleration); // Prevent gravity from affecting.
				} else tempVecRbody.y = rbody.velocity.y; // Don't affect gravity and let gravity keep pulling down.
				tempVecRbody.x = Mathf.SmoothDamp(rbody.velocity.x, 0, ref walkDeaccelerationVolx, deceleration);
				tempVecRbody.z = Mathf.SmoothDamp(rbody.velocity.z, 0, ref walkDeaccelerationVolz, deceleration);
				rbody.velocity = tempVecRbody;
			}
		}

		relForward = 0f;
		relSideways = 0f;
		running = false;
		if (GetInput.a.Forward() && !consoleActivated) {
			relForward = 1f;
			if (leanTarget > 0) {
				if (Mathf.Abs(leanTarget - 0) < 0.02f) leanTarget = 0;
				else leanTarget -= (leanSpeed * Time.deltaTime);

				if (Mathf.Abs(leanShift - 0) < 0.02f) leanShift = 0;
				else leanShift = -1 * (leanMaxShift * (leanTarget/leanMaxAngle));
			} else {
				if (Mathf.Abs(leanTarget - 0) < 0.02f) leanTarget = 0;
				else leanTarget += (leanSpeed * Time.deltaTime);

				if (Mathf.Abs(leanShift - 0) < 0.02f) leanShift = 0;
				else leanShift = leanMaxShift * (leanTarget/(leanMaxAngle * -1));
			}
		}

		if (GetInput.a.Backpedal() && !consoleActivated) relForward = -1f;
		if (GetInput.a.StrafeLeft() && !consoleActivated) relSideways = -1f;
		if (GetInput.a.StrafeRight() && !consoleActivated) relSideways = 1f;
		if (relForward != 0 || relSideways != 0) running = true; // we are mashing a run button down

		// Handle movement
		if (!inCyberSpace) {
			if (grounded == true || CheatNoclip ||  (Inventory.a.hardwareIsActive[10])) {
				if (ladderState && !CheatNoclip) {
					rbody.AddRelativeForce (relSideways * walkAcceleration * Time.deltaTime, relForward * walkAcceleration * Time.deltaTime, 0); // Climbing when touching the ground
				} else {
					//Walking on the ground
					if (CheatNoclip) {
						rbody.AddRelativeForce(relSideways * 2f * walkAcceleration * Time.deltaTime, 0, relForward * 2f * walkAcceleration * Time.deltaTime);
					} else {
						rbody.AddRelativeForce(relSideways * walkAcceleration * Time.deltaTime, 0, relForward * walkAcceleration * Time.deltaTime);
					}

					// Noclip up and down
					if (GetInput.a.SwimUp() && !consoleActivated) {
						if (CheatNoclip) rbody.AddRelativeForce(0, 4f * walkAcceleration * Time.deltaTime, 0);
					}

					if (GetInput.a.SwimDn() && !consoleActivated) {
						if (CheatNoclip) rbody.AddRelativeForce(0, 4f * walkAcceleration * Time.deltaTime * -1, 0);
					}

					if (fatigueFinished2 < PauseScript.a.relativeTime && relForward != defIndex && !CheatNoclip) {
						fatigueFinished2 = PauseScript.a.relativeTime + fatigueWaneTickSecs;
						if (isSprinting) {
							fatigue += fatiguePerSprintTick;
							if (staminupActive) fatigue = 0;
						} else {
							fatigue += fatiguePerWalkTick;
							if (staminupActive) fatigue = 0;
						}
					}
				}
			} else {
				if (ladderState) {
					// Climbing off the ground
					if (ladderSFXFinished < PauseScript.a.relativeTime && rbody.velocity.y > ladderSpeed * 0.5f) {
						if (PlayerNoise != null) {
							PlayerNoise.pitch = (UnityEngine.Random.Range(0.8f,1.2f));
							PlayerNoise.PlayOneShot(SFXLadder,0.2f);
						}
						ladderSFXFinished = PauseScript.a.relativeTime + ladderSFXIntervalTime;
					}
					rbody.AddRelativeForce (relSideways * walkAcceleration * walkAccelAirRatio * Time.deltaTime * 0.2f, ladderSpeed * relForward * walkAcceleration * Time.deltaTime, 0);
				} else {
					// Sprinting in the air
					if (isSprinting && running && !inCyberSpace) {
						rbody.AddRelativeForce (relSideways * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime, 0, relForward * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime);
					} else {
						// Walking in the air, we're floating in the moonlit sky, the people far below are sleeping as we fly
						rbody.AddRelativeForce(relSideways * walkAcceleration * walkAccelAirRatio * Time.deltaTime, 0, relForward * walkAcceleration * walkAccelAirRatio * Time.deltaTime);
					}
				}
			}

			// Handle leaning, double tap to reset is handled in Update to prevent repeat calls within the same frame
			if (GetInput.a.LeanRight()) {
				leanTarget -= (leanSpeed * Time.deltaTime);
				if (leanTarget < (leanMaxAngle * -1)) leanTarget = (leanMaxAngle * -1);
				leanShift = -1 * (leanMaxShift * (leanTarget/leanMaxAngle));
			}
			if (GetInput.a.LeanLeft()) {
				leanTarget += (leanSpeed * Time.deltaTime);
				if (leanTarget > leanMaxAngle) leanTarget = leanMaxAngle;
				leanShift = leanMaxShift * (leanTarget/(leanMaxAngle * -1));
			}
			leanTransform.localRotation = Quaternion.Euler(0, 0, leanTarget);
			leanTransform.localPosition = new Vector3(leanShift,0,0);

			// Handle gravity and ladders
			if (ladderState) {
				if (rbody.useGravity) rbody.useGravity = false;
				if (Inventory.a.hardwareIsActive [9] && Inventory.a.hardwareVersionSetting[9] == 0)
					deceleration = walkDeaccelerationBooster;
				else
					deceleration = walkDeacceleration;

				RigidbodySetVelocityY(rbody, (Mathf.SmoothDamp(rbody.velocity.y, 0, ref walkDeaccelerationVoly, deceleration))); // Set vertical velocity towards 0 when climbing
			} else {
				// Check if using a gravity lift
				if (gravliftState == true || CheatNoclip) {
					if (rbody.useGravity) rbody.useGravity = false;
				} else {
					// Disables gravity when touching the ground to prevent player sliding down ramps...hacky?
					if (grounded == true || CheatNoclip) {
						if (rbody.useGravity) rbody.useGravity = false;
					} else {
						if (!inCyberSpace && !rbody.useGravity) rbody.useGravity = true;
					}
				}
			}

			// Get input for Jump and set impulse time, removed "&& (ladderState == false)" since I want to be able to jump off a ladder			 
			if (!inCyberSpace && !consoleActivated && !CheatNoclip) {
				if (doubleJumpFinished < PauseScript.a.relativeTime) {
					doubleJumpTicks--;
					if (doubleJumpTicks < 0) doubleJumpTicks = 0;
				}	

				if (GetInput.a.Jump() && !CheatNoclip) {
					if (!justJumped) {
						if (grounded || gravliftState || Inventory.a.hardwareIsActive[10]) {
							jumpTime = jumpImpulseTime;
							doubleJumpFinished = PauseScript.a.relativeTime + Const.a.doubleClickTime;
							doubleJumpTicks++;
							justJumped = true;
						} else {
							if (ladderState) {
								jumpTime = jumpImpulseTime;
								justJumped = true;
							}
						}
					}

					if (Inventory.a.hardwareIsActive [9] && Inventory.a.hardwareVersionSetting[9] == 1) {
						if (justJumped && doubleJumpTicks == 2) {
							rbody.AddForce(new Vector3(transform.forward.x * burstForce,transform.forward.y * burstForce,transform.forward.z * burstForce),ForceMode.Impulse); // Booster thrust
							pe.TakeEnergy(22f);
							justJumped = false;
							jumpTime = 0;
							doubleJumpTicks = 0;
							doubleJumpFinished = PauseScript.a.relativeTime - 1f; // Make sure we can't do it again right away.
						}
					}
				} 
			}
		
			// Perform Jump
			while (jumpTime > 0) {
				jumpTime -= Time.smoothDeltaTime;
				if (fatigue > 80 && !(Inventory.a.hardwareIsActive[10])) {
					rbody.AddForce (new Vector3 (0, jumpVelocityFatigued * rbody.mass, 0), ForceMode.Force);  // huhnh!
				} else {
					if (Inventory.a.hardwareIsActive [10]) {
						if (pe.energy > 11f) {
							rbody.AddForce (new Vector3 (0, jumpVelocityBoots * rbody.mass, 0), ForceMode.Force);  // huhnh!
							float energysuck = 25f;
							switch (Inventory.a.hardwareVersion[10]) {
								case 0: energysuck = 25f; break;
								case 1: energysuck = 30f; break;
								case 2: energysuck = 35f; break;
							}
							if (jumpJetEnergySuckTickFinished < PauseScript.a.relativeTime) {
								jumpJetEnergySuckTickFinished = PauseScript.a.relativeTime + jumpJetEnergySuckTick;
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

			if (justJumped && !(Inventory.a.hardwareIsActive[10])) {
				// Play jump sound
				if (jumpSFXFinished < PauseScript.a.relativeTime) {
					jumpSFXFinished = PauseScript.a.relativeTime + jumpSFXIntervalTime;
					PlayerNoise.pitch = 1f;
					if (fatigue > 80)
						PlayerNoise.PlayOneShot(SFXJump,0.5f); // Quietly, we are tired.
					else
						PlayerNoise.PlayOneShot(SFXJump);
				}
				justJumped = false;
				fatigue += jumpFatigue;
				if (staminupActive) fatigue = 0;
			}

			// Handle fall damage (no impact damage in cyber space 5/5/18, JJ)
			if (Mathf.Abs ((oldVelocity.y - rbody.velocity.y)) > fallDamageSpeed && !inCyberSpace && !CheatNoclip) {
				DamageData dd = new DamageData ();
				dd.damage = fallDamage; // No need for GetDamageTakeAmount since this is strictly internal to Player
				dd.attackType = Const.AttackType.None;
				dd.offense = 0f;
				dd.isOtherNPC = false;
				// No impact force from fall damage.
				hm.TakeDamage (dd);
			}
			oldVelocity = rbody.velocity;
			if (!CheatWallSticky || gravliftState) grounded = false; // Automatically set grounded to false to prevent ability to climb any wall
		} else {
			// Handle cyberspace movement
			inputtingMovement = false;
			if (GetInput.a.Forward() && !consoleActivated) {
				if (turboFinished > PauseScript.a.relativeTime) {
					if (Vector3.Project(rbody.velocity, (cameraObject.transform.forward)).magnitude < playerSpeed * 2f)
					rbody.AddForce (cameraObject.transform.forward * walkAcceleration * 1.3f * 2f * Time.deltaTime,ForceMode.Acceleration); // double speed with turbo on
				} else {
					if (Vector3.Project(rbody.velocity, cameraObject.transform.forward).magnitude < playerSpeed) 
					rbody.AddForce (cameraObject.transform.forward * walkAcceleration * 1.3f * Time.deltaTime,ForceMode.Acceleration);
				}
				inputtingMovement = true;
			}

			if (GetInput.a.Backpedal() && !consoleActivated) {
				if (turboFinished > PauseScript.a.relativeTime) {
					if (Vector3.Project(rbody.velocity, (cameraObject.transform.forward * -1f)).magnitude < playerSpeed * 2f)
					rbody.AddForce (cameraObject.transform.forward * walkAcceleration * 1.3f * 2f * Time.deltaTime * -1f,ForceMode.Acceleration); // double speed with turbo on
				} else {
					if (Vector3.Project(rbody.velocity, cameraObject.transform.forward * -1f).magnitude < playerSpeed) 
					rbody.AddForce (cameraObject.transform.forward * walkAcceleration * 1.3f * Time.deltaTime * -1f,ForceMode.Acceleration);
				}
				inputtingMovement = true;
			}

			if (GetInput.a.StrafeLeft() && !consoleActivated) {
				if (turboFinished > PauseScript.a.relativeTime) {
					if (Vector3.Project(rbody.velocity, (cameraObject.transform.right * -1f)).magnitude < playerSpeed * 2f)
					rbody.AddForce (cameraObject.transform.right * walkAcceleration * 1.3f * 2f * Time.deltaTime * -1f,ForceMode.Acceleration); // double speed with turbo on
				} else {
					if (Vector3.Project(rbody.velocity, cameraObject.transform.right * -1f).magnitude < playerSpeed) 
					rbody.AddForce (cameraObject.transform.right * walkAcceleration * 1.3f * Time.deltaTime * -1f,ForceMode.Acceleration);
				}
				inputtingMovement = true;
			}

			if (GetInput.a.StrafeRight() && !consoleActivated) {
				if (turboFinished > PauseScript.a.relativeTime) {
					if (Vector3.Project(rbody.velocity, cameraObject.transform.right).magnitude < playerSpeed * 2f)
					rbody.AddForce (cameraObject.transform.right * walkAcceleration * 1.3f * 2f * Time.deltaTime,ForceMode.Acceleration); // double speed with turbo on
				} else {
					if (Vector3.Project(rbody.velocity, cameraObject.transform.right).magnitude < playerSpeed) 
					rbody.AddForce (cameraObject.transform.right * walkAcceleration * 1.3f * Time.deltaTime,ForceMode.Acceleration);
				}
				inputtingMovement = true;
			}

			if (Const.a.difficultyCyber > 1) {
				rbody.AddForce (cameraObject.transform.forward * walkAcceleration*0.05f * Time.deltaTime); // turbo doesn't affect detrimental forces :)
			} else {
				if (!inputtingMovement) {
					rbody.velocity = Const.a.vectorZero;
				}
			}
		}
	}

	bool CantStand() { return Physics.CheckCapsule(cameraObject.transform.position, cameraObject.transform.position + new Vector3(0f,(1.6f-0.84f),0f), capsuleRadius, layerMask); }
	bool CantCrouch() { return Physics.CheckCapsule(cameraObject.transform.position, cameraObject.transform.position + new Vector3(0f,0.4f,0f), capsuleRadius, layerMask); } 

	public void LocalScaleSetX(Transform t, float s) {
		tempVecRbody = t.localScale;
		tempVecRbody.x  = s;
		t.localScale = tempVecRbody;
	}

	public void LocalScaleSetY(Transform t, float s) {
		tempVecRbody = t.localScale;
		tempVecRbody.y  = s;
		t.localScale = tempVecRbody;
	}

	public void LocalScaleSetZ(Transform t, float s) {
		tempVecRbody = t.localScale;
		tempVecRbody.z  = s;
		t.localScale = tempVecRbody;
	}

	public void LocalPositionSetX(Transform t, float s) {
		tempVecRbody = t.position;
		tempVecRbody.x  = s;
		t.position = tempVecRbody;
	}
	
	public void LocalPositionSetY(Transform t, float s) {
		tempVecRbody = t.position;
		tempVecRbody.y  = s;
		t.position = tempVecRbody;
	}
	
	public void LocalPositionSetZ(Transform t, float s) {
		tempVecRbody = t.position;
		tempVecRbody.z  = s;
		t.position = tempVecRbody;
	}

	public void RigidbodySetVelocity(Rigidbody t, float s) {
		tempVecRbody = t.velocity;
		tempVecRbody = tempVecRbody.normalized * s;
		t.velocity = tempVecRbody;
	}

	public void RigidbodySetVelocityX(Rigidbody t, float s) {
		tempVecRbody = t.velocity;
		tempVecRbody.x = s;
		t.velocity = tempVecRbody;
	}
	
	public void RigidbodySetVelocityY(Rigidbody t, float s) {
		tempVecRbody = t.velocity;
		tempVecRbody.y  = s;
		t.velocity = tempVecRbody;
	}
	
	public void RigidbodySetVelocityZ(Rigidbody t, float s) {
		tempVecRbody = t.velocity;
		tempVecRbody.z  = s;
		t.velocity = tempVecRbody;
	}

	// AUTOMAP - rather than making yet another singleton Automap.a. because that's annoying
	// =====================================================================================
	// Update automap location
	public void UpdateAutomap () {
		if (inCyberSpace) return;

		if (Inventory.a.hardwareVersion[1] < 2) {
			if (poolContainerAutomapBotOverlays.activeSelf) poolContainerAutomapBotOverlays.SetActive(false);
		} else {
			if (!poolContainerAutomapBotOverlays.activeSelf) poolContainerAutomapBotOverlays.SetActive(true);
		}

		if (Inventory.a.hardwareVersion[1] < 3) {
			if (poolContainerAutomapCyborgOverlays.activeSelf) poolContainerAutomapCyborgOverlays.SetActive(false);
			if (poolContainerAutomapMutantOverlays.activeSelf) poolContainerAutomapMutantOverlays.SetActive(false);
		} else {
			if (!poolContainerAutomapCyborgOverlays.activeSelf) poolContainerAutomapCyborgOverlays.SetActive(true);
			if (!poolContainerAutomapMutantOverlays.activeSelf) poolContainerAutomapMutantOverlays.SetActive(true);
		}

		if (automapUpdateFinished < PauseScript.a.relativeTime) {
			if (!automapBaseImage.enabled) automapBaseImage.enabled = true;
			if (automapBaseImage.overrideSprite != automapsBaseImages[LevelManager.a.currentLevel]) automapBaseImage.overrideSprite = automapsBaseImages[LevelManager.a.currentLevel];
			// private float camMaxAmount = 0.2548032f;
			// private float mapWorldMaxN = 85.83999f;
			// private float mapWorldMaxS = -78.00001f;
			// private float mapWorldMaxE = -70.44f;
			// private float mapWorldMaxW = 93.4f;
			tempVec.x = (((transform.localPosition.z - Const.a.mapWorldMaxE)/(Const.a.mapWorldMaxW - Const.a.mapWorldMaxE)) * (Const.a.camMaxAmount * 2f)) + (Const.a.camMaxAmount * -1f);
			tempVec.y = (((transform.localPosition.x - Const.a.mapWorldMaxS)/(Const.a.mapWorldMaxN - Const.a.mapWorldMaxS)) * (Const.a.camMaxAmount * 2f)) + (Const.a.camMaxAmount * -1f);
			tempVec.z = automapCameraTransform.localPosition.z;
			tempVec.x = (tempVec.x * -1f) + automapCorrectionX;
			tempVec.y += automapCorrectionY;

			// private float mapTileMinX = 8; // top left corner
			// private float mapTileMaxY = -8; // top left corner
			// private float mapTileMinY = -1016; // bottom right corner
			// private float mapTileMaxX = 1016; // bottom right corner
			tempVec2b.x = (((transform.localPosition.z - Const.a.mapWorldMaxE)/(Const.a.mapWorldMaxW - Const.a.mapWorldMaxE)) * 1008f) + Const.a.mapTileMinX + automapTileBCorrectionX;
			tempVec2b.y = ((((transform.localPosition.x - Const.a.mapWorldMaxS)/(Const.a.mapWorldMaxN - Const.a.mapWorldMaxS)) * 1008f) + Const.a.mapTileMinY + automapTileBCorrectionY);

			if (inFullMap) {
				tempVec2b.x -= automapTileBCorrectionX;
				tempVec2b.y -= automapTileBCorrectionY;
				automapFullPlayerIcon.localPosition = tempVec2b;
				tempVec.x = 0;
				tempVec.y = 0;
				automapCameraTransform.localPosition = tempVec; // move the map to center
			} else {
				automapCameraTransform.localPosition = tempVec; // move the map to reflect player movement
			}

			// Update player icon rotation
			automapPlayerIconZAdjusted = (transform.eulerAngles.y * (-1) + 90); // Rotation is adjusted for player view and direction vs UI space
			if (Mathf.Abs(automapNormalPlayerIconLH.localRotation.z - automapPlayerIconZAdjusted) > 0.5f) automapNormalPlayerIconLH.localRotation = Quaternion.Euler(0,0,automapPlayerIconZAdjusted);
			if (Mathf.Abs(automapNormalPlayerIconRH.localRotation.z - automapPlayerIconZAdjusted) > 0.5f) automapNormalPlayerIconRH.localRotation = Quaternion.Euler(0,0,automapPlayerIconZAdjusted);
			if (Mathf.Abs(automapFullPlayerIcon.localRotation.z - automapPlayerIconZAdjusted) > 0.5f) automapFullPlayerIcon.localRotation = Quaternion.Euler(0,0,automapPlayerIconZAdjusted);

			// Update explored tiles
			for (int i=0;i<4096;i++) {
				if (automapExplored[i]) {
					if (automapFoWTiles[i] != null) automapFoWTiles[i].enabled = false;
				} else {
					tempVec2.x = automapFoWTilesRects[i].localPosition.x * -1f - automapTileCorrectionX;// - tempVec.x;
					tempVec2.y = automapFoWTilesRects[i].localPosition.y + automapTileCorrectionY;// - tempVec.y;
					if (Vector2.Distance(tempVec2,tempVec2b) < automapFoWRadius) {
						automapExplored[i] = true;
						SetAutomapTileExplored(LevelManager.a.currentLevel,i);
						if (automapFoWTiles[i] != null) automapFoWTiles[i].enabled = false;
					} else {
						if (automapFoWTiles[i] != null) automapFoWTiles[i].enabled = true;
					}
				}
			}

			updateTime = 0.2f;
			if (Inventory.a.hardwareVersion[1] > 1) updateTime = 0.1f;// Display just bot overlays - Handled by AIController since it updates it anyways.
			if (Inventory.a.hardwareVersion[1] > 2) {
				updateTime = 0.05f;

				// Display hazards
				for (int j=0;j<13;j++) {
					if (j != LevelManager.a.currentLevel) {
						if (automapsHazardOverlays[j].enabled) automapsHazardOverlays[j].enabled = false;
					} else {
						if (!automapsHazardOverlays[j].enabled) automapsHazardOverlays[j].enabled = true;
					}
				}
				// Display cyborg and mutant overlays - Handled by AIController since it updates it anyways.
			}
			automapUpdateFinished = PauseScript.a.relativeTime + updateTime;
		}

		if (Inventory.a.hasHardware[1]) {
			if (automapTabLH.activeInHierarchy || automapTabRH.activeInHierarchy || inFullMap) {
				automapCamera.enabled = true;
				switch (LevelManager.a.currentLevel) {
					case 0: if (!levelOverlayContainerR.activeSelf) {levelOverlayContainerR.SetActive(true);}   DeactivateLevelOverlayContainersExcept(0); break;
					case 1: if (!levelOverlayContainer1.activeSelf) {levelOverlayContainer1.SetActive(true);}   DeactivateLevelOverlayContainersExcept(1); break;
					case 2: if (!levelOverlayContainer2.activeSelf) {levelOverlayContainer2.SetActive(true);}   DeactivateLevelOverlayContainersExcept(2); break;
					case 3: if (!levelOverlayContainer3.activeSelf) {levelOverlayContainer3.SetActive(true);}   DeactivateLevelOverlayContainersExcept(3); break;
					case 4: if (!levelOverlayContainer4.activeSelf) {levelOverlayContainer4.SetActive(true);}   DeactivateLevelOverlayContainersExcept(4); break;
					case 5: if (!levelOverlayContainer5.activeSelf) {levelOverlayContainer5.SetActive(true);}   DeactivateLevelOverlayContainersExcept(5); break;
					case 6: if (!levelOverlayContainer6.activeSelf) {levelOverlayContainer6.SetActive(true);}   DeactivateLevelOverlayContainersExcept(6); break;
					case 7: if (!levelOverlayContainer7.activeSelf) {levelOverlayContainer7.SetActive(true);}   DeactivateLevelOverlayContainersExcept(7); break;
					case 8: if (!levelOverlayContainer8.activeSelf) {levelOverlayContainer8.SetActive(true);}   DeactivateLevelOverlayContainersExcept(8); break;
					case 9: if (!levelOverlayContainer9.activeSelf) {levelOverlayContainer9.SetActive(true);}   DeactivateLevelOverlayContainersExcept(9); break;
					case 10:if (!levelOverlayContainerG1.activeSelf) {levelOverlayContainerG1.SetActive(true);} DeactivateLevelOverlayContainersExcept(10);break;
					case 11:if (!levelOverlayContainerG2.activeSelf) {levelOverlayContainerG2.SetActive(true);} DeactivateLevelOverlayContainersExcept(11);break;
					case 12:if (!levelOverlayContainerG4.activeSelf) {levelOverlayContainerG4.SetActive(true);} DeactivateLevelOverlayContainersExcept(12);break;
				}
			}
		} else {
			automapCamera.enabled = false;
		}
	}

	void DeactivateLevelOverlayContainersExcept(int current) {
		if (current != 0) {if (levelOverlayContainerR.activeSelf) levelOverlayContainerR.SetActive(false); }
		if (current != 1) {if (levelOverlayContainer1.activeSelf) levelOverlayContainer1.SetActive(false); }
		if (current != 2) {if (levelOverlayContainer2.activeSelf) levelOverlayContainer2.SetActive(false); }
		if (current != 3) {if (levelOverlayContainer3.activeSelf) levelOverlayContainer3.SetActive(false); }
		if (current != 4) {if (levelOverlayContainer4.activeSelf) levelOverlayContainer4.SetActive(false); }
		if (current != 5) {if (levelOverlayContainer5.activeSelf) levelOverlayContainer5.SetActive(false); }
		if (current != 6) {if (levelOverlayContainer6.activeSelf) levelOverlayContainer6.SetActive(false); }
		if (current != 7) {if (levelOverlayContainer7.activeSelf) levelOverlayContainer7.SetActive(false); }
		if (current != 8) {if (levelOverlayContainer8.activeSelf) levelOverlayContainer8.SetActive(false); }
		if (current != 9) {if (levelOverlayContainer9.activeSelf) levelOverlayContainer9.SetActive(false); }
		if (current != 10){if (levelOverlayContainerG1.activeSelf)levelOverlayContainerG1.SetActive(false);}
		if (current != 11){if (levelOverlayContainerG2.activeSelf)levelOverlayContainerG2.SetActive(false);}
		if (current != 12){if (levelOverlayContainerG4.activeSelf)levelOverlayContainerG4.SetActive(false);}
	}

	public void SetAutomapExploredReference(int currentLevel) {
		switch(currentLevel) {
			case 0: for (int i=0;i<4096;i++) { automapExplored[i] = automapExploredR[i]; } break;
			case 1: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored1[i]; } break;
			case 2: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored2[i]; } break;
			case 3: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored3[i]; } break;
			case 4: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored4[i]; } break;
			case 5: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored5[i]; } break;
			case 6: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored6[i]; } break;
			case 7: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored7[i]; } break;
			case 8: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored8[i]; } break;
			case 9: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored9[i]; } break;
			case 10:for (int i=0;i<4096;i++) { automapExplored[i] = automapExploredG1[i];} break;
			case 11:for (int i=0;i<4096;i++) { automapExplored[i] = automapExploredG2[i];} break;
			case 12:for (int i=0;i<4096;i++) { automapExplored[i] = automapExploredG4[i];} break;
		}
	}

	void SetAutomapTileExplored(int currentLevel, int index) {
		switch(currentLevel) {
			case 0: automapExploredR[index] = true; break;
			case 1: automapExplored1[index] = true; break;
			case 2: automapExplored2[index] = true; break;
			case 3: automapExplored3[index] = true; break;
			case 4: automapExplored4[index] = true; break;
			case 5: automapExplored5[index] = true; break;
			case 6: automapExplored6[index] = true; break;
			case 7: automapExplored7[index] = true; break;
			case 8: automapExplored8[index] = true; break;
			case 9: automapExplored9[index] = true; break;
			case 10:automapExploredG1[index]= true; break;
			case 11:automapExploredG2[index]= true; break;
			case 12:automapExploredG4[index]= true; break;
		}
	}

	public void AutomapZoomOut() {
		if (Inventory.a.hardwareVersion[1] < 2) {
			Const.sprint(Const.a.stringTable[465],Const.a.player1); // Map hardware version doesn't support zoom.
			return;
		}

		currentAutomapZoomLevel++;
		if (currentAutomapZoomLevel > 2) {
			currentAutomapZoomLevel = 2;
			Const.sprint(Const.a.stringTable[316],Const.a.player1); // zoom at max
			return;
		}
		AutomapZoomAdjust();
	}

	public void AutomapZoomIn() {
		if (Inventory.a.hardwareVersion[1] < 2) {
			Const.sprint(Const.a.stringTable[465],Const.a.player1); // Map hardware version doesn't support zoom.
			return;
		}

		currentAutomapZoomLevel--;
		if (currentAutomapZoomLevel < 0) {
			currentAutomapZoomLevel = 0;
			Const.sprint(Const.a.stringTable[317],Const.a.player1); // zoom at min
			return;
		}
		AutomapZoomAdjust();
	}

	void AutomapZoomAdjust() {
		Vector3 scaleVec = new Vector3(0f,0f,0f);
		switch (currentAutomapZoomLevel) {
			case 0: scaleVec = new Vector3(automapZoom0, automapZoom0, automapZoom0); break;
			case 1: scaleVec = new Vector3(automapZoom1, automapZoom1, automapZoom1); break;
			case 2: scaleVec = new Vector3(automapZoom2, automapZoom2, automapZoom2); break;
		}
		automapContainerLH.transform.localScale = scaleVec;
		automapContainerRH.transform.localScale = scaleVec;
	}

	public void AutomapGoSide() {
		Const.sprint("Unable to connect to side map, try updating to version 1.00",Const.a.player1); // zoom at max
	}

	public void AutomapGoFull() {
		MFDManager.a.AutomapGoFull();
	}
	// =====================================================================================
	// =====================================================================================


	// Reset grounded to false when player is mid-air
	void OnCollisionExit (){
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			// Automatically set grounded to false to prevent ability to climb any wall (Cheat!)
			if (CheatWallSticky == true) {
				grounded = false;
			}
		}
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
					return;
				}
			}
		}
	}

	public void ConsoleDisable() { // Note this is called during Load from a save.
		consoleActivated = false;
		consoleplaceholderText.SetActive(false);
		consoleTitle.SetActive(false);
		consoleinpFd.DeactivateInputField();
		consoleinpFd.enabled = false;
		consolebg.enabled = false;
		consoleentryText.text = "";
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

	// CHEAT CODES you cheaty cheatface you...why here?? why not.
	// =====================================================================================
    public void ConsoleEntry() {
		string ts = consoleinpFd.text.ToLower(); // test string = lower case text
		string tn = consoleinpFd.text; // test number = number searching
        if (ts.Contains("noclip") || ts.Contains("idclip")) {
			if (CheatNoclip) {
				CheatNoclip = false;
				rbody.useGravity = true;
				capsuleCollider.enabled = true;
				leanCapsuleCollider.enabled = true;
				Const.sprint("noclip disabled");
			} else {
				CheatNoclip = true;
				rbody.useGravity = false;
				capsuleCollider.enabled = false;
				leanCapsuleCollider.enabled = false;
				Const.sprint("noclip activated!");
			}
        } else if (ts.Contains("notarget")) {
			if (Notarget) {
				Notarget = false;
				Const.sprint("notarget disabled");
			} else {
				Notarget = true;
				Const.sprint("notarget activated!");
			}
        } else if (ts.Contains("god") || (ts.Contains("power") && ts.Contains("overwhelming")) || ts.Contains("whosyourdaddy") || ts.Contains("iddqd")) {
			if (hm.god) {
				Const.sprint("god mode disabled");
				hm.god = false;
			} else {
				Const.sprint("god mode activated!");
				hm.god = true;
			}
        } else if (ts.Contains("load") && tn.Contains("0")) {
			LevelManager.a.LoadLevel(0,LevelManager.a.ressurectionLocation[0].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[0].position);
        } else if (ts.Contains("load") && tn.Contains("1")) {
			LevelManager.a.LoadLevel(1,LevelManager.a.ressurectionLocation[1].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[1].position);
        } else if (ts.Contains("load") && tn.Contains("2")) {
			LevelManager.a.LoadLevel(2,LevelManager.a.ressurectionLocation[2].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[2].position);
        } else if (ts.Contains("load") && tn.Contains("3")) {
			LevelManager.a.LoadLevel(3,LevelManager.a.ressurectionLocation[3].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[3].position);
        } else if (ts.Contains("load") && tn.Contains("4")) {
			LevelManager.a.LoadLevel(4,LevelManager.a.ressurectionLocation[4].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[4].position);
        } else if (ts.Contains("load") && tn.Contains("5")) {
			LevelManager.a.LoadLevel(5,LevelManager.a.ressurectionLocation[5].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[5].position);
        } else if (ts.Contains("load") && tn.Contains("6")) {
			LevelManager.a.LoadLevel(6,LevelManager.a.ressurectionLocation[6].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[6].position);
        } else if (ts.Contains("load") && tn.Contains("7")) {
			LevelManager.a.LoadLevel(7,LevelManager.a.ressurectionLocation[7].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[7].position);
        } else if (ts.Contains("load") && tn.Contains("8")) {
			LevelManager.a.LoadLevel(8,LevelManager.a.ressurectionLocation[8].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[8].position);
        } else if (ts.Contains("load") && tn.Contains("9")) {
			LevelManager.a.LoadLevel(9,LevelManager.a.ressurectionLocation[9].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[9].position);
        } else if (ts.Contains("load") && ts.Contains("g1")) {
			LevelManager.a.LoadLevel(10,cheatG1Spawn.gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[10].position);
        } else if (ts.Contains("load") && ts.Contains("g2")) {
			LevelManager.a.LoadLevel(11,cheatG2Spawn.gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[11].position);
        } else if (ts.Contains("load") && ts.Contains("g4")) {
			LevelManager.a.LoadLevel(12,cheatG4Spawn.gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[12].position);
		} else if (ts.Contains("load") && ts.Contains("g3")) {
			Const.sprint("Gamma grove already jettisoned!  Those poor arrogant people.");
		} else if (ts.Contains("loadarsenalr") || ts.Contains("loadarsenal r")) {
			GameObject cheatArsenal = Instantiate(cheatLRarsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenal1") || ts.Contains("loadarsenal 1")) {
			GameObject cheatArsenal = Instantiate(cheatL1arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenal2") || ts.Contains("loadarsenal 2")) {
			GameObject cheatArsenal = Instantiate(cheatL2arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenal3") || ts.Contains("loadarsenal 3")) {
			GameObject cheatArsenal = Instantiate(cheatL3arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenal4") || ts.Contains("loadarsenal 4")) {
			GameObject cheatArsenal = Instantiate(cheatL4arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenal5") || ts.Contains("loadarsenal 5")) {
			GameObject cheatArsenal = Instantiate(cheatL5arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenal6") || ts.Contains("loadarsenal 6")) {
			GameObject cheatArsenal = Instantiate(cheatL6arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenal7") || ts.Contains("loadarsenal 7")) {
			GameObject cheatArsenal = Instantiate(cheatL7arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenal8") || ts.Contains("loadarsenal 8")) {
			GameObject cheatArsenal = Instantiate(cheatL8arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenal9") || ts.Contains("loadarsenal 9")) {
			GameObject cheatArsenal = Instantiate(cheatL9arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenalg1") || ts.Contains("loadarsenal g1")) {
			GameObject cheatArsenal = Instantiate(cheatL6arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenalg2") || ts.Contains("loadarsenal g2")) {
			GameObject cheatArsenal = Instantiate(cheatL6arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (ts.Contains("loadarsenalg3") || ts.Contains("loadarsenal g3")) {
			GameObject cheatArsenal = Instantiate(cheatL6arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
        } else if (ts.Contains("loadarsenalg4") || ts.Contains("loadarsenal g4")) {
			GameObject cheatArsenal = Instantiate(cheatL6arsenal,transform.position,Const.a.quaternionIdentity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
        } else if (ts.Contains("bottomless") && ts.Contains("clip")) { // bottomlessclip
			if (wepCur.bottomless) {
				Const.sprint("Hose disconnected from interdimensional wormhole. Normal ammo operation restored.");
				wepCur.bottomless = false;
			} else {
				Const.sprint("bottomlessclip!  Bring it!");
				wepCur.bottomless = true;
			}
        } else if (ts.Contains("ifeelthepower") || (ts.Contains("i") && ts.Contains("feel") && ts.Contains("the") && ts.Contains("power"))) { // ifeelthepower
			if (wepCur.redbull) {
				Const.sprint("Energy usage normal");
				wepCur.redbull = false;
			} else {
				Const.sprint("I feel the power! 0 energy consumption!");
				wepCur.redbull = true;
			}
        } else if (ts.Contains("show") && ts.Contains("fps")) { // showfps
			Const.sprint("Toggling FPS counter for framerate (bottom right corner)...");
			fpsCounter.SetActive(!fpsCounter.activeInHierarchy);
        } else if (ts.Contains("show") && ts.Contains("location")) { // showlocation
			Const.sprint("Toggling locationIndicator (bottom left corner)...");
			locationIndicator.SetActive(!locationIndicator.activeInHierarchy);
		} else if (ts.Contains("i") && ts.Contains("am") && ts.Contains("shodan")) { // iamshodan
			if (LevelManager.a.superoverride) {
				Const.sprint("SHODAN has regained control of security from you");
				LevelManager.a.superoverride = false;
			} else {
				Const.sprint("Full security override enabled!");
				LevelManager.a.superoverride = true;
			}
		} else if (consoleinpFd.text == "Mr. Bean") {
				Const.sprint("Nice try, there are no go carts to slow down here");
		} else if (consoleinpFd.text == "Simon Foster") {
				Const.sprint("Nice try, nothing to paint here");
		} else if (consoleinpFd.text == "Motherlode" || consoleinpFd.text == "Rosebud" || consoleinpFd.text == "Kaching" || consoleinpFd.text == "money") {
				Const.sprint("Nice try, there's no money here.");
		} else if (consoleinpFd.text == "Richard Branson") {
				Const.sprint("Nice try, there's no money here.  You do realize this isn't Rollercoaster Tycoon right?");
		} else if (consoleinpFd.text == "John Wardley") {
				Const.sprint("WOW!");
		} else if (consoleinpFd.text == "John Mace") {
				Const.sprint("Nice try, there's nothing to pay double for here");
		} else if (consoleinpFd.text == "Melanie Warn") {
				Const.sprint("I feel happy!!!");
		} else if (consoleinpFd.text == "Damon Hill") {
				Const.sprint("Nice try, there are no go carts to speed up here");
		} else if (consoleinpFd.text == "Michael Schumacher") {
				Const.sprint("Nice try, there are no go carts to give ludicrous speed here");
		} else if (consoleinpFd.text == "Tony Day") {
				Const.sprint("Ok, now I want a hamburger");
		} else if (consoleinpFd.text == "Katie Brayshaw") {
				Const.sprint("Hi there! Hello! Hey! Howdy!");
		} else if (ts.Contains("sudo") || ts.Contains("admin")) {
				Const.sprint("Super user access granted...ERROR: access restricted by SHODAN");
		} else if (ts.Contains("git")) {
				if (ts.Contains("pull") || ts.Contains("fetch")) Const.sprint("remote: Enumerating objects: 24601, done. Failed, could not connect with origin/triop.");
				else if (ts.Contains("status")) Const.sprint("Your branch is up to date with origin/triop. Working directory clean.");
				else if (ts.Contains("log")) Const.sprint("<Merge pull request #451 from SHODAN/NeuralLinkBugfix> 6 months ago...");
				else if (ts.Contains("reflog")) Const.sprint("dc51440 HEAD0 -> master: commit: Establish neural connection ... ERROR: invalid ID `2-4601`");
				else if (ts.Contains("merge")) Const.sprint("Failed, could not connect with origin/triop");
				else if (ts.Contains("push")) Const.sprint("Could not find Username for 'triopttp://192.168.1.451'");
				else if (ts.Contains("clone")) Const.sprint("Failed, connection blocked by SHODAN. Employee ID invalid.");
				else if (ts.Contains("branch") || ts.Contains("-b")) Const.sprint("Created new branch " + Const.a.GetIntFromString(ts.Split(' ').Last()));
				else if (ts.Contains("checkout")) Const.sprint("Branch name not recognized.  Contact your TriopBucket representative.");
				else Const.sprint("Branch name not recognized.  Contact your TriopBucket representative.");
		} else if (ts.Contains("restart")) {
				Const.sprint("Yeah...better not");
		} else if (ts.Contains("quit") || ts.Contains("exit")) {
				Const.sprint("Use the Pause Menu by hitting Escape and clicking QUIT");
		} else if (ts.Contains("cd") || ts.Contains("./")) {
				Const.sprint("Attempting to access directory... already at root");
		} else if (ts.Contains("kill") || ts.Contains("kick") || ts.Contains("ban") || ts.Contains("destroy") || ts.Contains("attack") || ts.Contains("suicide") || ts.Contains("die")) {
				Const.sprint("Player decides to become a cyborg.");
				DamageData dd = new DamageData();
				dd.damage = hm.health + 1.0f;
				dd.other = gameObject;
				hm.TakeDamage(dd);
		} else if (ts.Contains("justinbailey")) {
				Const.sprint("Well, you don't have a suit already so...");
		} else if (ts.Contains("woodstock")) {
				Const.sprint("How much wood could a woodchuck chuck...there's no wood in SPACE!");
		} else if (ts.Contains("quarry")) {
				Const.sprint("There's obsidian on levels 6 and 8 if want to feel decadant, otherwise we are lacking in the stone department.");
		} else if (ts.Contains("help")) {
				Const.sprint("There's no one to save you now Hacker!");
		} else if (ts.Contains("zelda")) {
				Const.sprint("Too late, already been to level 1");
		} else if (ts.Contains("allyourbasearebelongtous") || (ts.Contains("all") && ts.Contains("your") && ts.Contains("base"))) {
				Const.sprint("ERROR: SHODAN has overriden your command, remove SHODAN first."); // This is not an easter egg if you run this after removing SHODAN!!
		} else if (ts.Contains("i") && ts.Contains("am") && ((ts.Contains("iron") && ts.Contains("man")) || ts.Contains("amazing") || ts.Contains("cool") || ts.Contains("best"))) {
				Const.sprint("That's nice dear.");
		} else if ((ts.Contains("impulse") && tn.Contains("9")) || ts.Contains("idkfa")) {
				Const.sprint("I can only hold 7 weapons!! Nice try dearies!");
		} else if (ts.Contains("summon_obj")) {
			int val = Const.a.GetIntFromString(ts.Split(' ').Last()); // That's a slow line to compute!
			if (val < 256 && val >= 0) {
				GameObject cheatObject = Instantiate(Const.a.useableItems[val],transform.position,Const.a.quaternionIdentity) as GameObject;
				if (cheatObject != null) cheatObject.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
			}
        } else {
            Const.sprint("Uknown command or function: " + consoleinpFd.text);
        }

        // Reset console and hide it, command was entered
        consoleinpFd.text = "";
        ToggleConsole();
    }
}