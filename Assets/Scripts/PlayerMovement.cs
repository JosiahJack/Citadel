using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerMovement : MonoBehaviour {
	[HideInInspector]
	public float walkAcceleration = 2000f;
	private float walkDeacceleration = 0.1f; // was 0.30f
	private float walkDeaccelerationBooster = 1f; // was 2f, adjusted player physics material to reduce friction for moving up stairs
	private float deceleration;
	private float walkAccelAirRatio = 0.75f;
	public GameObject cameraObject;
	public MouseLookScript mlookScript;
	public float playerSpeed; // save
	private float maxWalkSpeed = 3.2f;
	private float maxCyberSpeed = 5f;
	private float maxCyberUltimateSpeed = 12f;
	private float maxCrouchSpeed = 1.25f; //1.75f
	private float maxProneSpeed = .5f; //1f
	private float maxSprintSpeed = 8.8f;
	private float maxSprintSpeedFatigued = 5.5f;
	private float maxVerticalSpeed = 5f;
	private float boosterSpeedBoost = 0.5f; // ammount to boost by when booster is active
	public bool isSprinting = false;
	private float jumpImpulseTime = 2.0f;
	public float jumpVelocityBoots = 0.5f;
	private float jumpVelocity = 1.1f;
	private  float jumpVelocityFatigued = 0.6f;
	public bool  grounded = false; // save
	private float crouchRatio = 0.6f;
	private float proneRatio = 0.2f;
	private float transitionToCrouchSec = 0.2f;
	private float transitionToProneAdd = 0.1f;
	public float currentCrouchRatio = 1f; // save
	public float capsuleHeight;
	public float capsuleRadius;
	public int bodyState = 0; // save
	public bool ladderState = false; // save
	private float ladderSpeed = 0.4f;
	private float fallDamage = 75f;
	public bool gravliftState = false; // save
	public bool inCyberSpace = false; // save
	private float automapUpdateFinished;
	public Camera automapCamera;
	public GameObject automapContainerLH;
	public GameObject automapContainerRH;
	public GameObject automapTabLH;
	public GameObject automapTabRH;
	public Transform automapCameraTransform;
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
	private bool[] automapExplored;
	public Image[] automapFoWTiles;
	public RectTransform[] automapFoWTilesRects;
	public float automapZoom0 = 1.2f;
	public float automapZoom1 = 0.75f;
	public float automapZoom2 = 0.55f;
	public bool inFullMap;
	private int currentAutomapZoomLevel = 0;
	public float circleInnerRangev1 = (2.5f * 2.56f) + 1.28f;
	public float circleOuterRangev1 = (4f * 2.56f) + 1.28f;
	public float circleInnerRangev2 = (3f * 2.56f) + 1.28f;
	public float circleOuterRangev2 = (4.5f * 2.56f) + 1.28f;
	public float circleInnerRangev3 = (5f * 2.56f) + 1.28f;
	public float circleOuterRangev3 = (7.5f * 2.56f) + 1.28f;
	public Vector2[] automapLevelHomePositions; // R= 43.97, 85.66 | 1= -8.53, 85.99 | 2= 10.2, 44.8 | 3= 9.4, 63.83 | 4= -55.65, 116.8 | 5= -9.4, 71.8 | 6= 29.7, 85.5 | 7= 5, 76.55 | 8= 25.1, 84.4 | 9= 39.8, 72.6
	public Image automapBaseImage;
	public Image automapInnerCircle;
	public Image automapOuterCircle;
	public Sprite[] automapsBaseImages;	
	public Image[] automapsHazardOverlays;
	public float automapFactorx = 0.000285f;
	public float automapFactory = 0.000285f;
	public Transform automapFullPlayerIcon;
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
	//[HideInInspector]
	public bool CheatWallSticky; // save
    //[HideInInspector]
    public bool CheatNoclip; // save
    public bool staminupActive = false;
	private Vector2 horizontalMovement;
	private float verticalMovement;
	[HideInInspector]
	public float jumpTime; // save
	private float crouchingVelocity = 1f;
	private float lastCrouchRatio;
	private int layerGeometry = 9;
	private int layerMask;
	[HideInInspector]
	public Rigidbody rbody;
	private float fallDamageSpeed = 11.72f;
	[HideInInspector]
	public Vector3 oldVelocity; // save
	public GameObject mainMenu;
	public HardwareInvCurrent hwc;
	public HardwareInventory hwi;
	public SoftwareInventory sinv;
	public HardwareButton hwbJumpJets;
	public float fatigue; // save
	private float jumpFatigue = 8.25f;
	private float fatigueWanePerTick = 1f;
	private float fatigueWanePerTickCrouched = 2f;
	private float fatigueWanePerTickProne = 3.5f;
	private float fatigueWaneTickSecs = 0.3f;
	private float fatiguePerWalkTick = 0.9f;
	private float fatiguePerSprintTick = 3.0f;
	[HideInInspector]
	public bool justJumped = false; // save
	[HideInInspector]
	public float fatigueFinished; // save
	[HideInInspector]
	public float fatigueFinished2; // save
	public TextWarningsManager twm;

	private int defIndex = 0;
	private int def1 = 1;
	private int onehundred = 100;
	public bool running = false; // save
	public bool cyberSetup = false; // save
	public bool cyberDesetup = false; // save
	private SphereCollider cyberCollider;
	private CapsuleCollider capsuleCollider;
	public CapsuleCollider leanCapsuleCollider;
	[HideInInspector]
	public int oldBodyState; // save
	private float bonus;
    private float walkDeaccelerationVolx;
    private float walkDeaccelerationVoly;
    private float walkDeaccelerationVolz;

	public Image consolebg;
    public InputField consoleinpFd;
    public GameObject consoleplaceholderText;
	public GameObject consoleTitle;
	public Text consoleentryText;
	public bool consoleActivated; // save

	public Transform leanTransform;

	[HideInInspector]
	public float leanLeftDoubleFinished; // save
	[HideInInspector]
	public float leanRightDoubleFinished; // save
	private float keyboardButtonDoubleTapTime = 0.25f; // max time before a double tap of a keyboard button is registered
	[HideInInspector]
	public float leanTarget = 0f; // save
	[HideInInspector]
	public float leanShift = 0f; // save
	public float leanMaxAngle = 35f;
	public float leanMaxShift = 0.48f;

	public AudioSource PlayerNoise;
	public AudioClip SFXJump;
	public AudioClip SFXJumpLand;
	public AudioClip SFXLadder;
	public float jumpSFXFinished; // save
	public float ladderSFXFinished;
	public float ladderSFXIntervalTime = 1f;
	public float jumpSFXIntervalTime = 1f;
	public float jumpLandSoundFinished; // save
	[HideInInspector]
	public float jumpJetEnergySuckTickFinished; // save
	public float jumpJetEnergySuckTick = 1f;
	private Vector3 tempVec;
	private Vector2 tempVec2;
	private Vector2 tempVec2b;
	private float tempFloat;
	private int tempInt;
	public float leanSpeed = 6.5f;
	public bool leanLHFirstPressed = false; // save
	public bool leanRHFirstPressed = false; // save
	public bool leanLHReset = false; // save
	public bool leanRHReset = false; // save
	public bool Notarget = false; // for cheat to disable enemy sight checks against this player
	public GameObject fpsCounter;
	public GameObject locationIndicator;
	public Text locationText;
	public WeaponCurrent wepCur;
	[HideInInspector]
	public bool fatigueWarned; // save
	private PlayerEnergy pe;
	[HideInInspector]
	public float ressurectingFinished; // save
	public float burstForce = 50f;
	[HideInInspector]
	public float doubleJumpFinished; // save
	private Vector3 playerHome;
	public HealthManager hm;
	private Texture2D tempTexture;
	// private Color[] mapColorsArray;
	// private Color[] hazardColors;
	[HideInInspector]
	public float turboFinished = 0f; // save
	public float turboCyberTime = 15f;
	private int doubleJumpTicks = 0;
	public float automapCorrectionX;
	public float automapCorrectionY;
	public float automapTileCorrectionX;
	public float automapTileCorrectionY;
	public float automapFoWRadius = 35f;
	public float automapTileBCorrectionX;
	public float automapTileBCorrectionY;
	public GameObject poolContainerAutomapBotOverlays;
	public GameObject poolContainerAutomapMutantOverlays;
	public GameObject poolContainerAutomapCyborgOverlays;


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
		leanLeftDoubleFinished = PauseScript.a.relativeTime;
		leanRightDoubleFinished = PauseScript.a.relativeTime;
		jumpLandSoundFinished = PauseScript.a.relativeTime;
		justJumped = false;
		leanLHFirstPressed = false;
		leanRHFirstPressed = false;
		leanLHReset = false;
		leanRHReset = false;
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
		// mapColorsArray = new Color[1024*1024];
		// hazardColors = new Color[1024*1024];
		automapExplored = new bool[4096];
		automapUpdateFinished = PauseScript.a.relativeTime;
		SetAutomapExploredReference(LevelManager.a.currentLevel);
		AutomapZoomAdjust();
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

		if (locationIndicator.activeInHierarchy) {
			locationText.text = "location: " +(transform.position.x.ToString("00.00")+" "+transform.position.y.ToString("00.00")+" "+transform.position.z.ToString("00.00"));
		}

		if (consoleActivated) {
			if (!String.IsNullOrEmpty(consoleentryText.text)) {
				if (consoleplaceholderText.activeSelf) consoleplaceholderText.SetActive(false);
			} else {
				if (!consoleplaceholderText.activeSelf) consoleplaceholderText.SetActive(true);
			}
		} else {
			if (consoleplaceholderText.activeSelf) consoleplaceholderText.SetActive(false);
		}

		if (mainMenu.activeSelf == true) {
			rbody.useGravity = false;
			return;  // ignore movement when main menu is still up
		} else {
			if (!inCyberSpace)
				rbody.useGravity = true;
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

			// Handle Right Leaning start
			if (GetInput.a.LeanRightStart()) {
				leanLHFirstPressed = false;	
				leanLHReset = false;	
				if (!leanRHFirstPressed) {
					leanRHFirstPressed = true;
				} else {
					if ((PauseScript.a.relativeTime - leanRightDoubleFinished) < keyboardButtonDoubleTapTime) {
						//if (leanTransform.eulerAngles.z < 15f && leanTransform.eulerAngles.z > -15f) { // Wasn't working, maybe too sensitive?
						//	Debug.Log("Leaning all the way Right!");
						//	leanTarget = -22.5f;
						//} else {
						//	Debug.Log("Leaning reset from right");
						//	leanTarget = 0;
						//}
						leanTarget = 0;
						leanShift = 0;
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
					if ((PauseScript.a.relativeTime - leanLeftDoubleFinished) < keyboardButtonDoubleTapTime) {
						//if (leanTransform.eulerAngles.z < 15f && leanTransform.eulerAngles.z > -15f) {
						//	Debug.Log("Leaning all the way Left!");
						//	leanTarget = 22.5f;
						//} else {
						//	Debug.Log("Leaning reset from left");
						//	leanTarget = 0;
						//}
						leanTarget = 0;
						leanShift = 0;
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

		if (!PauseScript.a.Paused() && ressurectingFinished < PauseScript.a.relativeTime) {
            if (CheatNoclip) grounded = true;
			// Crouch
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

			if (CheatNoclip) playerSpeed = maxCyberSpeed*4f;
		
			if (inCyberSpace && !CheatNoclip) {
				// Limit movement speed in all axes x,y,z in cyberspace
				if (rbody.velocity.magnitude > maxCyberUltimateSpeed) {
					RigidbodySetVelocity(rbody, maxCyberUltimateSpeed);
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
								playerSpeed -= ((maxWalkSpeed - maxCrouchSpeed)*1.5f);  // subtract off the difference in speed between walking and crouching from the sprint speed
							}
							if (bodyState == 4 || bodyState == 5 || bodyState == 6) {
								playerSpeed -= (maxWalkSpeed - maxProneSpeed);  // subtract off the difference in speed between walking and proning from the sprint speed
							}
						} else {
							playerSpeed = maxSprintSpeed + bonus;
							if (bodyState == 1 || bodyState == 2 || bodyState == 3) {
								playerSpeed -= ((maxWalkSpeed - maxCrouchSpeed)*1.5f);  // subtract off the difference in speed between walking and crouching from the sprint speed
							}
							if (bodyState == 4 || bodyState == 5 || bodyState == 6) {
								playerSpeed -= ((maxWalkSpeed - maxProneSpeed)*2f);  // subtract off the difference in speed between walking and proning from the sprint speed
							}
						}
						if (CheatNoclip) playerSpeed = maxSprintSpeed + (bonus*1.5f);
					}
					horizontalMovement *= playerSpeed;  // cap velocity to max speed
				}

				// Set horizontal velocity
				RigidbodySetVelocityX(rbody, horizontalMovement.x);
				RigidbodySetVelocityZ(rbody, horizontalMovement.y); // NOT A BUG - already passed rbody.velocity.z into the .y of this Vector2

				// Update the map
				if (HardwareInventory.a.hasHardware[1]) UpdateAutomap();

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
				leanShift = 0;
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

                        if (fatigueFinished2 < PauseScript.a.relativeTime && relForward != defIndex && !CheatNoclip) {
							fatigueFinished2 = PauseScript.a.relativeTime + fatigueWaneTickSecs;
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
						if (ladderSFXFinished < PauseScript.a.relativeTime && rbody.velocity.y > ladderSpeed * 0.5f) {
							if (PlayerNoise != null) {
								PlayerNoise.pitch = (UnityEngine.Random.Range(0.8f,1.2f));
								PlayerNoise.PlayOneShot(SFXLadder,0.2f);
								//PlayerNoise.pitch = 1f;
							}
							ladderSFXFinished = PauseScript.a.relativeTime + ladderSFXIntervalTime;
						}
						rbody.AddRelativeForce (relSideways * walkAcceleration * walkAccelAirRatio * Time.deltaTime * 0.2f, ladderSpeed * relForward * walkAcceleration * Time.deltaTime, 0);
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
					if (leanTarget < (leanMaxAngle * -1)) leanTarget = (leanMaxAngle * -1);
					leanShift = -1 * (leanMaxShift * (leanTarget/leanMaxAngle));
					leanRightDoubleFinished = PauseScript.a.relativeTime;
				} else {
					if ((PauseScript.a.relativeTime - leanRightDoubleFinished) > keyboardButtonDoubleTapTime) {
						leanRHReset = false;
						leanRHFirstPressed = false;
					}
				}

				if (GetInput.a.LeanLeft() && !leanLHReset) {
					leanTarget += (leanSpeed * Time.deltaTime);
					if (leanTarget > leanMaxAngle) leanTarget = leanMaxAngle;

					leanShift = leanMaxShift * (leanTarget/(leanMaxAngle * -1));
					leanLeftDoubleFinished = PauseScript.a.relativeTime;
				} else {
					if ((PauseScript.a.relativeTime - leanLeftDoubleFinished) > keyboardButtonDoubleTapTime) {
						leanLHReset = false;
						leanLHFirstPressed = false;
					}
				}

				//leanMaxShift
				leanTransform.localRotation = Quaternion.Euler(0, 0, leanTarget);
				leanTransform.localPosition = new Vector3(leanShift,0,0);

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
					if (doubleJumpFinished < PauseScript.a.relativeTime) {
						doubleJumpTicks--;
						if (doubleJumpTicks < 0) doubleJumpTicks = 0;
					}	

					if (GetInput.a.Jump()) {
						if (!justJumped) {
							if (grounded || gravliftState || hwc.hardwareIsActive[10]) {
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

						if (hwc.hardwareIsActive [9] && hwi.hardwareVersionSetting[9] == 1) {
							if (justJumped && doubleJumpTicks == 2) {
								//Debug.Log("Booster thrust!");
								rbody.AddForce(new Vector3(transform.forward.x * burstForce,transform.forward.y * burstForce,transform.forward.z * burstForce),ForceMode.Impulse);
								pe.TakeEnergy(22f);
								justJumped = false;
								jumpTime = 0;
								doubleJumpTicks = 0;
								doubleJumpFinished = PauseScript.a.relativeTime - 1f; // make sure we can't do it again right away
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

				if (justJumped && !(hwc.hardwareIsActive[10])) {
					// Play jump sound
					if (fatigue > 80) {
						// quiet, we are tired
						if (jumpSFXFinished < PauseScript.a.relativeTime) {
							jumpSFXFinished = PauseScript.a.relativeTime + jumpSFXIntervalTime;
							PlayerNoise.pitch = 1f;
							PlayerNoise.PlayOneShot(SFXJump,0.5f);
						}
					} else {
						if (jumpSFXFinished < PauseScript.a.relativeTime) {
							jumpSFXFinished = PauseScript.a.relativeTime + jumpSFXIntervalTime;
							PlayerNoise.pitch = 1f;
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
					hm.TakeDamage (dd);  // was previously referring to the no longer used TakeDamage within PlayerHealth.cs
				}
				oldVelocity = rbody.velocity;

				// Automatically set grounded to false to prevent ability to climb any wall
				if (CheatWallSticky == false || gravliftState)
					grounded = false;
			} else {
				// Handle cyberspace movement
				bool inputtingMovement = false;
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
						rbody.velocity = Vector3.zero;
					}
				}
			}
		}
	}

	// AUTOMAP - rather than making yet another singleton Automap.a. because that's annoying
	// =====================================================================================
	// Update automap location
	public void UpdateAutomap () {
		if (inCyberSpace) return;

		if (hwi.hardwareVersion[1] < 2) {
			if (poolContainerAutomapBotOverlays.activeSelf) poolContainerAutomapBotOverlays.SetActive(false);
		} else {
			if (!poolContainerAutomapBotOverlays.activeSelf) poolContainerAutomapBotOverlays.SetActive(true);
		}

		if (hwi.hardwareVersion[1] < 3) {
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

			float updateTime = 0.2f;
			if (hwi.hardwareVersion[1] > 1) {
				updateTime = 0.1f;
				// Display just bot overlays

			}
			if (hwi.hardwareVersion[1] > 2) {
				updateTime = 0.05f;

				// Display hazards
				for (int j=0;j<13;j++) {
					if (j != LevelManager.a.currentLevel) {
						if (automapsHazardOverlays[j].enabled) automapsHazardOverlays[j].enabled = false;
					} else {
						if (!automapsHazardOverlays[j].enabled) automapsHazardOverlays[j].enabled = true;
					}
				}

				// Display cyborg and mutant overlays

			}
			automapUpdateFinished = PauseScript.a.relativeTime + updateTime;
		}

		if (hwi.hasHardware[1]) {
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
		if (hwi.hardwareVersion[1] < 2) {
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
		if (hwi.hardwareVersion[1] < 2) {
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
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
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
	// =====================================================================================
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
			if (hm.god) {
				Const.sprint("god mode disabled", Const.a.allPlayers);
				hm.god = false;
			} else {
				Const.sprint("god mode activated!", Const.a.allPlayers);
				hm.god = true;
			}
        } else if (consoleinpFd.text == "load0" || consoleinpFd.text == "LOAD0" || consoleinpFd.text == "Load0") {
			LevelManager.a.LoadLevel(0,LevelManager.a.ressurectionLocation[0].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[0].position);
        } else if (consoleinpFd.text == "load1" || consoleinpFd.text == "LOAD1" || consoleinpFd.text == "Load1") {
			LevelManager.a.LoadLevel(1,LevelManager.a.ressurectionLocation[1].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[1].position);
        } else if (consoleinpFd.text == "load2" || consoleinpFd.text == "LOAD2" || consoleinpFd.text == "Load2") {
			LevelManager.a.LoadLevel(2,LevelManager.a.ressurectionLocation[2].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[2].position);
        } else if (consoleinpFd.text == "load3" || consoleinpFd.text == "LOAD3" || consoleinpFd.text == "Load3") {
			LevelManager.a.LoadLevel(3,LevelManager.a.ressurectionLocation[3].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[3].position);
        } else if (consoleinpFd.text == "load4" || consoleinpFd.text == "LOAD4" || consoleinpFd.text == "Load4") {
			LevelManager.a.LoadLevel(4,LevelManager.a.ressurectionLocation[4].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[4].position);
        } else if (consoleinpFd.text == "load5" || consoleinpFd.text == "LOAD5" || consoleinpFd.text == "Load5") {
			LevelManager.a.LoadLevel(5,LevelManager.a.ressurectionLocation[5].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[5].position);
        } else if (consoleinpFd.text == "load6" || consoleinpFd.text == "LOAD6" || consoleinpFd.text == "Load6") {
			LevelManager.a.LoadLevel(6,LevelManager.a.ressurectionLocation[6].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[6].position);
        } else if (consoleinpFd.text == "load7" || consoleinpFd.text == "LOAD7" || consoleinpFd.text == "Load7") {
			LevelManager.a.LoadLevel(7,LevelManager.a.ressurectionLocation[7].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[7].position);
        } else if (consoleinpFd.text == "load8" || consoleinpFd.text == "LOAD8" || consoleinpFd.text == "Load8") {
			LevelManager.a.LoadLevel(8,LevelManager.a.ressurectionLocation[8].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[8].position);
        } else if (consoleinpFd.text == "load9" || consoleinpFd.text == "LOAD9" || consoleinpFd.text == "Load9") {
			LevelManager.a.LoadLevel(9,LevelManager.a.ressurectionLocation[9].gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[9].position);
        } else if (consoleinpFd.text == "loadg1" || consoleinpFd.text == "LOADG1" || consoleinpFd.text == "Loadg1") {
			LevelManager.a.LoadLevel(10,cheatG1Spawn.gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[10].position);
        } else if (consoleinpFd.text == "loadg2" || consoleinpFd.text == "LOADG2" || consoleinpFd.text == "Loadg2") {
			LevelManager.a.LoadLevel(11,cheatG2Spawn.gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[11].position);
        } else if (consoleinpFd.text == "loadg4" || consoleinpFd.text == "LOADG4" || consoleinpFd.text == "Loadg4") {
			LevelManager.a.LoadLevel(12,cheatG4Spawn.gameObject,Const.a.player1,LevelManager.a.ressurectionLocation[12].position);
		} else if (consoleinpFd.text == "loadarsenalr" || consoleinpFd.text == "LOADARSENALR" || consoleinpFd.text == "Loadarsenalr" || consoleinpFd.text == "LoadarsenalR" || consoleinpFd.text == "loadarsenalR") {
			GameObject cheatArsenal = Instantiate(cheatLRarsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenal1" || consoleinpFd.text == "LOADARSENAL1" || consoleinpFd.text == "Loadarsenal1") {
			GameObject cheatArsenal = Instantiate(cheatL1arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenal2" || consoleinpFd.text == "LOADARSENAL2" || consoleinpFd.text == "Loadarsenal2") {
			GameObject cheatArsenal = Instantiate(cheatL2arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenal3" || consoleinpFd.text == "LOADARSENAL3" || consoleinpFd.text == "Loadarsenal3") {
			GameObject cheatArsenal = Instantiate(cheatL3arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenal4" || consoleinpFd.text == "LOADARSENAL4" || consoleinpFd.text == "Loadarsenal4") {
			GameObject cheatArsenal = Instantiate(cheatL4arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenal5" || consoleinpFd.text == "LOADARSENAL5" || consoleinpFd.text == "Loadarsenal5") {
			GameObject cheatArsenal = Instantiate(cheatL5arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenal6" || consoleinpFd.text == "LOADARSENAL6" || consoleinpFd.text == "Loadarsenal6") {
			GameObject cheatArsenal = Instantiate(cheatL6arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenal7" || consoleinpFd.text == "LOADARSENAL7" || consoleinpFd.text == "Loadarsenal7") {
			GameObject cheatArsenal = Instantiate(cheatL7arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenal8" || consoleinpFd.text == "LOADARSENAL8" || consoleinpFd.text == "Loadarsenal8") {
			GameObject cheatArsenal = Instantiate(cheatL8arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenal9" || consoleinpFd.text == "LOADARSENAL9" || consoleinpFd.text == "Loadarsenal9") {
			GameObject cheatArsenal = Instantiate(cheatL9arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenalg1" || consoleinpFd.text == "LOADARSENALG1" || consoleinpFd.text == "Loadarsenalg1" || consoleinpFd.text == "LoadarsenalG1" || consoleinpFd.text == "loadarsenalG1") {
			GameObject cheatArsenal = Instantiate(cheatL6arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenalg2" || consoleinpFd.text == "LOADARSENALG2" || consoleinpFd.text == "Loadarsenalg2" || consoleinpFd.text == "LoadarsenalG2" || consoleinpFd.text == "loadarsenalG2") {
			GameObject cheatArsenal = Instantiate(cheatL6arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
		} else if (consoleinpFd.text == "loadarsenalg3" || consoleinpFd.text == "LOADARSENALG3" || consoleinpFd.text == "Loadarsenalg3" || consoleinpFd.text == "LoadarsenalG3" || consoleinpFd.text == "loadarsenalG3") {
			GameObject cheatArsenal = Instantiate(cheatL6arsenal,transform.position,Quaternion.identity) as GameObject;
			if (cheatArsenal != null) cheatArsenal.transform.SetParent(LevelManager.a.GetCurrentLevelDynamicContainer().transform);
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
        } else if (consoleinpFd.text == "showlocation" || consoleinpFd.text == "SHOWLOCATION" || consoleinpFd.text == "show location") {
			Const.sprint("Toggling locationIndicator (bottom left corner)...", Const.a.allPlayers);
			locationIndicator.SetActive(!locationIndicator.activeInHierarchy);
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