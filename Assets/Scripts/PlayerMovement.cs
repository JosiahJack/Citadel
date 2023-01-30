using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
	// External references, required
	public GameObject cameraObject;
	public Camera automapCamera;
	public GameObject automapCanvasGO;
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
	public Vector2[] automapLevelHomePositions;
		// R  =  43.97,  85.66
		// 1  =  -8.53,  85.99
		// 2  =  10.20,  44.80
		// 3  =   9.40,  63.83
		// 4  = -55.65, 116.80
		// 5  =  -9.40,  71.80
		// 6  =  29.70,  85.50
		// 7  =   5.00,  76.55
		// 8  =  25.10,  84.40
		// 9  =  39.80,  72.60
		// 10 = 440.80, 200.60
		// 11 =  80.16,-196.68
		// 12 =  99.50, 416.90
		// 13 =   0.00,   0.00
	
	public HardwareButton hwbJumpJets;
	public TextWarningsManager twm;
	public CapsuleCollider leanCapsuleCollider;
	public Image consolebg;
    public InputField consoleinpFd;
    public GameObject consoleplaceholderText;
	public GameObject consoleTitle;
	public Text consoleentryText;
	public Transform leanTransform;
	public AudioSource SFX;
	[HideInInspector] public int SFXJump = 135;
	[HideInInspector] public int SFXJumpLand = 136;
	[HideInInspector] public int SFXLadder = 137;
	public GameObject fpsCounter;
	public GameObject locationIndicator;
	public Text locationText;
	public HealthManager hm;
	public GameObject poolContainerAutomapBotOverlays;
	public GameObject poolContainerAutomapMutantOverlays;
	public GameObject poolContainerAutomapCyborgOverlays;

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
	public float playerSpeed; // save
	public float playerSpeedActual;
	public float playerSpeedHorizontalActual;
	[HideInInspector] public BodyState bodyState; // save
	public bool isSprinting = false;
	public bool grounded = false; // save
	[HideInInspector] public bool ladderState = false; // save
	[HideInInspector] public bool gravliftState = false; // save
	[HideInInspector] public bool inCyberSpace = false; // save
	[HideInInspector] public bool inFullMap;
	[HideInInspector] public float walkAcceleration = 2000f;
	[HideInInspector] public int SFXIndex = -1; // save
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
	private float automapUpdateFinished; // save
	private bool[] automapExplored;
	private float automapZoom0 = 1.2f;
	private float automapZoom1 = 0.75f;
	private float automapZoom2 = 0.55f;
	[HideInInspector] public int currentAutomapZoomLevel = 0;
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
	public Vector2 horizontalMovement;
	public float verticalMovement;
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
	public bool running = false;
	private float relForward = 0f;
	private float relSideways = 0f;
	[HideInInspector] public bool cyberSetup = false; // save
	[HideInInspector] public bool cyberDesetup = false; // save
	private SphereCollider cyberCollider;
	[HideInInspector] public CapsuleCollider capsuleCollider;
	[HideInInspector] public BodyState oldBodyState; // save
	public float bonus;
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
	[HideInInspector] public float ressurectingFinished; // save
	private float burstForce = 35f;
	[HideInInspector] public float doubleJumpFinished; // save
	private Vector3 playerHome;
	private Texture2D tempTexture;
	[HideInInspector] public float turboFinished = 0f; // save
	[HideInInspector] public float turboCyberTime = 15f;
	[HideInInspector] public bool inCyberTube = false;
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

	public static PlayerMovement a;

	void Awake() {
		a = this;
	}

    void Start (){
		currentCrouchRatio = def1;
		bodyState = BodyState.Standing;
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
		ConsoleEmulator.lastCommand = new string[7];
		ConsoleEmulator.consoleMemdex = 0;
    }

	void Update() {
		// Always allowed items, even when paused...
		ConsoleEmulator.ConsoleUpdate();

		// Bug Hunter feedback (puts it into their screenshots for me)
		if (locationIndicator.activeInHierarchy) locationText.text = "location: " +(transform.position.x.ToString("00.00")
																	 + " " + transform.position.y.ToString("00.00")
																	 + " " + transform.position.z.ToString("00.00"));

		// Prevent falling or movement while menu is up. Force it here in case PauseScript didn't catch it at startup.
		if (PauseScript.a.mainMenu.activeSelf == true) { rbody.useGravity = false; rbody.Sleep(); return; }
		if (PauseScript.a.Paused() || (ressurectingFinished >= PauseScript.a.relativeTime)) return;

		// Normal play when not paused...

		rbody.WakeUp(); // Force player physics to never sleep.
		if (rbody.isKinematic) rbody.isKinematic = false; // Allow physics to react and move.
		CyberSetup();
		if (!inCyberSpace) {
			CyberDestupOrNoclipMaintain();
		} else PlayerHealth.a.makingNoise = true; // Cyber enemies to look more aware

		isSprinting = GetSprintInputState();
		Crouch();
		Prone();
		EndCrouchProneTransition();
		FatigueApply(); // Here fatigue me out, except in cyberspace
	}

	void FixedUpdate() {
		playerSpeedActual = rbody.velocity.magnitude; // Readout for debugging in Inspector.
		
		Vector2 hz = new Vector2(rbody.velocity.x, rbody.velocity.z);
		playerSpeedHorizontalActual = hz.magnitude;

		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) return;
		if (ressurectingFinished > PauseScript.a.relativeTime) return;
		if (consoleActivated) return;

		if (capsuleCollider.height != (currentCrouchRatio * 2f)) capsuleCollider.height = currentCrouchRatio * 2f; // Crouch
		if (leanCapsuleCollider.height != capsuleCollider.height) leanCapsuleCollider.height = capsuleCollider.height; // Lean should always match stalk.
		SetRunningRelForwardsAndSidewaysFlags();
		playerSpeed = GetBasePlayerSpeed();
		if (Inventory.a.hasHardware[1]) UpdateAutomap(); // Update the map
		ApplyBodyStateLerps(); // Handle body position lerping for smooth transitions
		Noclip();
		ApplyGroundFriction();
		bool grav = GetGravity();
		if (rbody.useGravity != grav) rbody.useGravity = grav; // Avoid useless setting of the rbody.
		//Noclip();
		if (!inCyberSpace) {
			horizontalMovement = GetClampedHorizontalMovement(); // Limit movement speed horizontally for normal movement
			RigidbodySetVelocityX(rbody, horizontalMovement.x); // Clamp horizontal movement
			RigidbodySetVelocityZ(rbody, horizontalMovement.y); // NOT A BUG - already passed rbody.velocity.z into the .y of this Vector2
			verticalMovement = GetClampedVerticalMovement();
			RigidbodySetVelocityY(rbody, verticalMovement); // Clamp vetical movement
			Lean();
			WalkRun();
			LadderStates();			 
			Jump();
			FallDamage();
			oldVelocity = rbody.velocity;
			if (!CheatWallSticky || gravliftState) grounded = false; // Automatically set grounded to false to prevent ability to climb any wall
		} else {
			if (rbody.velocity.magnitude > playerSpeed && !CheatNoclip) {
				rbody.velocity = rbody.velocity.normalized * playerSpeed;
			}

			CyberspaceMovement();
		}
	}

	float GetBasePlayerSpeed() {
		if (CheatNoclip && isSprinting) return maxCyberSpeed * 2.5f; // Cheat speed.
		if (CheatNoclip) return maxCyberSpeed * 1.5f; // Cheat speed.
		if (inCyberSpace) return maxCyberSpeed; //Cyber space state

		float retval = maxWalkSpeed;
		bonus = 0f;
		if (Inventory.a.hardwareIsActive[9] && Inventory.a.hasHardware[9]) bonus = boosterSpeedBoost;
		switch (bodyState) {
			case BodyState.Standing: 		retval = maxWalkSpeed;   break;
			case BodyState.Crouch: 			retval = maxCrouchSpeed; break;
			case BodyState.CrouchingDown: 	retval = maxCrouchSpeed; break;
			case BodyState.StandingUp: 		retval = maxWalkSpeed;   break;
			case BodyState.Prone: 			retval = maxProneSpeed;  break;
			case BodyState.ProningDown: 	retval = maxProneSpeed;  break;
			case BodyState.ProningUp: 		retval = maxProneSpeed;  break;
		}

		if (isSprinting && running) {
			if (fatigue > 80f) retval = maxSprintSpeedFatigued;
			else retval = maxSprintSpeed;

			if (bodyState == BodyState.Standing || bodyState == BodyState.Crouch || bodyState == BodyState.CrouchingDown) {
				retval -= ((maxWalkSpeed - maxCrouchSpeed)*1.5f);  // Subtract off the difference in speed between walking and crouching from the sprint speed
			} else if (bodyState == BodyState.Prone || bodyState == BodyState.ProningDown || bodyState == BodyState.ProningUp) {
				retval -= ((maxWalkSpeed - maxProneSpeed)*2f);  // Subtract off the difference in speed between walking and proning from the sprint speed
			}
		}

		return retval + bonus;
	}

	void ApplyBodyStateLerps() {
		switch (bodyState) {
		case BodyState.CrouchingDown:
			currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, -0.01f, ref crouchingVelocity, transitionToCrouchSec);
			break;
		case BodyState.StandingUp:
			lastCrouchRatio = currentCrouchRatio;
			currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, 1.01f, ref crouchingVelocity, transitionToCrouchSec);
			LocalPositionSetY (transform, (((currentCrouchRatio - lastCrouchRatio) * capsuleHeight) / 2) + transform.position.y);
			break;
		case BodyState.ProningDown:
			currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, -0.01f, ref crouchingVelocity, transitionToCrouchSec);
			break;
		case BodyState.ProningUp: // Prone to crouch
			lastCrouchRatio = currentCrouchRatio;
			currentCrouchRatio = Mathf.SmoothDamp (currentCrouchRatio, 1.01f, ref crouchingVelocity, (transitionToCrouchSec + transitionToProneAdd));
			LocalPositionSetY (transform, (((currentCrouchRatio - lastCrouchRatio) * capsuleHeight) / 2) + transform.position.y);
			break;
		}
	}

	void SetRunningRelForwardsAndSidewaysFlags() {
		relForward = GetInput.a.Backpedal() ? -1f : 0f;
		if (GetInput.a.Forward()) {
			relForward = 1f;
		}
		relSideways = GetInput.a.StrafeLeft() ? -1f : 0f;
		if (GetInput.a.StrafeRight()) relSideways = 1f;
		running = ((relForward != 0) || (relSideways != 0)); // We are mashing a run button down.

		// Now check for thumbstick/joystick input
		Vector2 leftThumbstick = new Vector2(Input.GetAxisRaw("JoyAxis1"), // Horizontal Left < 0, Right > 0
											 Input.GetAxisRaw("JoyAxis2") * -1f); // Vertical Down > 0, Up < 0 Inverted
		if (leftThumbstick.magnitude < 0.05f) leftThumbstick = Vector2.zero;
		else leftThumbstick = leftThumbstick.normalized * ((leftThumbstick.magnitude - 0.05f) / (1.0f - 0.05f));

		relForward += leftThumbstick.y;
		relSideways += leftThumbstick.x;

		if (relForward > 0) {
			if (!inCyberSpace) {
				if (leanTarget > 0) {
					if (Mathf.Abs(leanTarget - 0) < 0.02f) leanTarget = 0;
					else leanTarget -= (leanSpeed * Time.deltaTime * relForward);

					if (Mathf.Abs(leanShift - 0) < 0.02f) leanShift = 0;
					else leanShift = -1 * (leanMaxShift * (leanTarget/leanMaxAngle)) * relForward;
				} else {
					if (Mathf.Abs(leanTarget - 0) < 0.02f) leanTarget = 0;
					else leanTarget += (leanSpeed * Time.deltaTime * relForward);

					if (Mathf.Abs(leanShift - 0) < 0.02f) leanShift = 0;
					else leanShift = leanMaxShift * (leanTarget/(leanMaxAngle * -1)) * relForward;
				}
			}
		}
	}

	void ApplyGroundFriction() {
		if (running && isSprinting) return;
		if (!CheatNoclip) {
			if (!grounded) return;
		}

		tempVecRbody = rbody.velocity;
		deceleration = walkDeacceleration;
		if (CheatNoclip) {
			deceleration = 0.05f;
			// Prevent gravity from affecting and decelerate like a horizontal.
			tempVecRbody.y = Mathf.SmoothDamp(rbody.velocity.y, 0, ref walkDeaccelerationVoly, deceleration); 
		} else {
			if (Inventory.a.hardwareIsActive[9]) deceleration = walkDeaccelerationBooster;
			tempVecRbody.y = rbody.velocity.y; // Don't affect gravity and let gravity keep pulling down.
		}
		tempVecRbody.x = Mathf.SmoothDamp(rbody.velocity.x, 0, ref walkDeaccelerationVolx, deceleration);
		tempVecRbody.z = Mathf.SmoothDamp(rbody.velocity.z, 0, ref walkDeaccelerationVolz, deceleration);
		rbody.velocity = tempVecRbody;
	}

	void Lean() {
		if (inCyberSpace) return; // 6dof handled in MouseLookScript for this portion.
		if (CheatNoclip) return;

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
	}

	bool GetGravity() {
		if (inCyberSpace) return false;
		if (CheatNoclip) return false;
		if (ladderState) return false;
		if (gravliftState) return false;
		if (grounded) return false; // Disables gravity when touching the ground to prevent player sliding down ramps...hacky?
		return true;
	}

	// Get input for Jump and set impulse time, removed "&& (ladderState == false)" since I want to be able to jump off a ladder
	void Jump() {
		if (CheatNoclip && !Inventory.a.hardwareIsActive[10]) return;

		if (doubleJumpFinished < PauseScript.a.relativeTime) {
			doubleJumpTicks--;
			if (doubleJumpTicks < 0) doubleJumpTicks = 0;
		}

		if (GetInput.a.Jump()) {
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
					PlayerEnergy.a.TakeEnergy(22f);
					justJumped = false;
					jumpTime = 0;
					doubleJumpTicks = 0;
					doubleJumpFinished = PauseScript.a.relativeTime - 1f; // Make sure we can't do it again right away.
				}
			}
		}
		// Perform Jump
		while (jumpTime > 0) { // Why ~was~ is this a `while` instead of an `if`??  Because otherwise it don't work, duh!
			jumpTime -= Time.smoothDeltaTime;
			if (fatigue > 80 && !(Inventory.a.hardwareIsActive[10])) {
				rbody.AddForce (new Vector3 (0, jumpVelocityFatigued * rbody.mass, 0), ForceMode.Force);  // huhnh!
			} else {
				if (Inventory.a.hardwareIsActive [10]) {
					if (PlayerEnergy.a.energy > 11f) {
						rbody.AddForce (new Vector3 (0, jumpVelocityBoots * rbody.mass, 0), ForceMode.Force);  // huhnh!
						float energysuck = 25f;
						switch (Inventory.a.hardwareVersion[10]) {
							case 0: energysuck = 25f; break;
							case 1: energysuck = 30f; break;
							case 2: energysuck = 35f; break;
						}
						if (jumpJetEnergySuckTickFinished < PauseScript.a.relativeTime) {
							jumpJetEnergySuckTickFinished = PauseScript.a.relativeTime + jumpJetEnergySuckTick;
							PlayerEnergy.a.TakeEnergy(energysuck);
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

		if (jumpTime <= 0) justJumped = false; // for jump jets to work 

		if (justJumped && !(Inventory.a.hardwareIsActive[10])) {
			// Play jump sound
			if (jumpSFXFinished < PauseScript.a.relativeTime) {
				jumpSFXFinished = PauseScript.a.relativeTime + jumpSFXIntervalTime;
				SFX.pitch = 1f;
				if (fatigue > 80)
					Utils.PlayOneShotSavable(SFX,SFXJump,0.5f); // Quietly, we are tired.
				else
					Utils.PlayOneShotSavable(SFX,SFXJump);
			}
			justJumped = false;
			fatigue += jumpFatigue;
			if (staminupActive) fatigue = 0;
		}
	}

	void LadderStates() {
		if (CheatNoclip) return;
		if (!ladderState) return;

		if (grounded || Inventory.a.hardwareIsActive[10]) {
			// Ladder climb, allow while grounded
			rbody.AddRelativeForce(relSideways * walkAcceleration * Time.deltaTime, ladderSpeed * relForward * walkAcceleration * Time.deltaTime, 0); // Climbing when touching the ground
		} else {
			// Climbing off the ground
			if (ladderSFXFinished < PauseScript.a.relativeTime && rbody.velocity.y > ladderSpeed * 0.5f) {
				SFX.pitch = (UnityEngine.Random.Range(0.8f,1.2f));
				Utils.PlayOneShotSavable(SFX,SFXLadder,0.2f);
				ladderSFXFinished = PauseScript.a.relativeTime + ladderSFXIntervalTime;
			}
			rbody.AddRelativeForce(relSideways * walkAcceleration * walkAccelAirRatio * Time.deltaTime * 0.2f, ladderSpeed * relForward * walkAcceleration * Time.deltaTime, 0);
		}

		if (Inventory.a.hardwareIsActive [9] && Inventory.a.hardwareVersionSetting[9] == 0)
			deceleration = walkDeaccelerationBooster;
		else
			deceleration = walkDeacceleration;

		RigidbodySetVelocityY(rbody, (Mathf.SmoothDamp(rbody.velocity.y, 0, ref walkDeaccelerationVoly, deceleration))); // Set vertical velocity towards 0 when climbing
	}

	void WalkRun() {
		if (CheatNoclip) return;
		if (ladderState) return;

		if (grounded || Inventory.a.hardwareIsActive[10]) {
			// Normal walking
			rbody.AddRelativeForce(relSideways * walkAcceleration * Time.deltaTime, 0, relForward * walkAcceleration * Time.deltaTime);
			if (fatigueFinished2 < PauseScript.a.relativeTime && relForward != defIndex) {
				fatigueFinished2 = PauseScript.a.relativeTime + fatigueWaneTickSecs;
				fatigue += isSprinting ? fatiguePerSprintTick : fatiguePerWalkTick;
				if (staminupActive) fatigue = 0;
			}
		} else {
			// Sprinting in the air
			if (isSprinting && running) {
				rbody.AddRelativeForce (relSideways * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime, 0, relForward * walkAcceleration * walkAccelAirRatio * 0.01f * Time.deltaTime);
			} else {
				// Walking in the air, we're floating in the moonlit sky, the people far below are sleeping as we fly
				rbody.AddRelativeForce(relSideways * walkAcceleration * walkAccelAirRatio * Time.deltaTime, 0, relForward * walkAcceleration * walkAccelAirRatio * Time.deltaTime);
			}
		}
	}

	void FallDamage() {
		if (CheatNoclip) return;
		if (ladderState) return;

		// Handle fall damage (no impact damage in cyber space 5/5/18, JJ)
		if (Mathf.Abs((oldVelocity.y - rbody.velocity.y)) > fallDamageSpeed) {
			DamageData dd = new DamageData ();
			float falltake = fallDamage - UnityEngine.Random.Range(0,68f);
			if (falltake > hm.health && falltake - hm.health < 5f) falltake = hm.health - 1f; // some small saving grace
			dd.damage = falltake; // No need for GetDamageTakeAmount since this is strictly internal to Player
			dd.attackType = AttackType.None;
			dd.offense = 0f;
			dd.isOtherNPC = false;
			// No impact force from fall damage.
			hm.TakeDamage (dd);
		}
	}

	void CyberspaceMovement() {
		if (!inCyberSpace) return;
		if (CheatNoclip) return;

		leanTransform.localRotation = Quaternion.Euler(0, 0, 0);
		leanTransform.localPosition = new Vector3(0,0,0);
		if (rbody.velocity.magnitude > maxCyberUltimateSpeed) RigidbodySetVelocity(rbody, maxCyberUltimateSpeed); // Limit movement speed in all axes x,y,z in cyberspace
		inputtingMovement = false;

		if (GetInput.a.Forward()) {
			if (turboFinished > PauseScript.a.relativeTime) {
				if (Vector3.Project(rbody.velocity, (cameraObject.transform.forward)).magnitude < playerSpeed * 2f)
					rbody.AddForce(cameraObject.transform.forward * walkAcceleration * 1.3f * 2f * Time.deltaTime,ForceMode.Acceleration); // double speed with turbo on
			} else {
				if (Vector3.Project(rbody.velocity, cameraObject.transform.forward).magnitude < playerSpeed)
					rbody.AddForce(cameraObject.transform.forward * walkAcceleration * 1.3f * Time.deltaTime,ForceMode.Acceleration);
			}
			inputtingMovement = true;
		}

		if (GetInput.a.Backpedal()) {
			if (turboFinished > PauseScript.a.relativeTime) {
				if (Vector3.Project(rbody.velocity, (cameraObject.transform.forward * -1f)).magnitude < playerSpeed * 2f)
				rbody.AddForce(cameraObject.transform.forward * walkAcceleration * 1.3f * 2f * Time.deltaTime * -1f,ForceMode.Acceleration); // double speed with turbo on
			} else {
				if (Vector3.Project(rbody.velocity, cameraObject.transform.forward * -1f).magnitude < playerSpeed) 
				rbody.AddForce(cameraObject.transform.forward * walkAcceleration * 1.3f * Time.deltaTime * -1f,ForceMode.Acceleration);
			}
			inputtingMovement = true;
		}

		if (GetInput.a.StrafeLeft()) {
			if (turboFinished > PauseScript.a.relativeTime) {
				if (Vector3.Project(rbody.velocity, (cameraObject.transform.right * -1f)).magnitude < playerSpeed * 2f)
				rbody.AddForce(cameraObject.transform.right * walkAcceleration * 1.3f * 2f * Time.deltaTime * -1f,ForceMode.Acceleration); // double speed with turbo on
			} else {
				if (Vector3.Project(rbody.velocity, cameraObject.transform.right * -1f).magnitude < playerSpeed) 
				rbody.AddForce(cameraObject.transform.right * walkAcceleration * 1.3f * Time.deltaTime * -1f,ForceMode.Acceleration);
			}
			inputtingMovement = true;
		}

		if (GetInput.a.StrafeRight()) {
			if (turboFinished > PauseScript.a.relativeTime) {
				if (Vector3.Project(rbody.velocity, cameraObject.transform.right).magnitude < playerSpeed * 2f)
				rbody.AddForce(cameraObject.transform.right * walkAcceleration * 1.3f * 2f * Time.deltaTime,ForceMode.Acceleration); // double speed with turbo on
			} else {
				if (Vector3.Project(rbody.velocity, cameraObject.transform.right).magnitude < playerSpeed) 
				rbody.AddForce(cameraObject.transform.right * walkAcceleration * 1.3f * Time.deltaTime,ForceMode.Acceleration);
			}
			inputtingMovement = true;
		}

		if (Const.a.difficultyCyber > 1) {
			if (rbody.velocity.magnitude < walkAcceleration * 0.05f) {
				tempVec = MouseCursor.a.GetCursorScreenPointForRay();
				tempVec = MouseLookScript.a.playerCamera.ScreenPointToRay(tempVec).direction;
				rbody.AddForce(tempVec * walkAcceleration*0.05f * Time.deltaTime); // turbo doesn't affect detrimental forces :)
			}
		} else {
			if (!inputtingMovement && !inCyberTube) rbody.velocity = Const.a.vectorZero;
		}
	}

	void Noclip() {
		if (!CheatNoclip) return;

		rbody.AddRelativeForce(relSideways * 2f * walkAcceleration * Time.deltaTime, 0, relForward * 2f * walkAcceleration * Time.deltaTime);
		if (GetInput.a.SwimUp()) rbody.AddRelativeForce(0, 4f * walkAcceleration * Time.deltaTime, 0); // Noclip up and down
		if (GetInput.a.SwimDn()) rbody.AddRelativeForce(0, 4f * walkAcceleration * Time.deltaTime * -1, 0);
	}

	Vector2 GetClampedHorizontalMovement() {
		horizontalMovement = new Vector2(rbody.velocity.x, rbody.velocity.z);
		if (horizontalMovement.magnitude > playerSpeed) {
			horizontalMovement = horizontalMovement.normalized;
			horizontalMovement *= playerSpeed;  // Cap velocity to current max speed.
		}
		return horizontalMovement;
	}

	float GetClampedVerticalMovement() {
		float retval = (rbody.velocity.y > maxVerticalSpeed) ? maxVerticalSpeed : rbody.velocity.y;
		return retval;
	}

	void FatigueApply() {
		if (fatigueFinished < PauseScript.a.relativeTime && !inCyberSpace && !CheatNoclip) {
			fatigueFinished = PauseScript.a.relativeTime + fatigueWaneTickSecs;
			switch (bodyState) {
				case BodyState.Standing:    fatigue -= fatigueWanePerTick; break;
				case BodyState.Crouch:      fatigue -= fatigueWanePerTickCrouched; break;
				case BodyState.StandingUp:  fatigue -= fatigueWanePerTickCrouched; break;
				case BodyState.ProningDown: fatigue -= fatigueWanePerTickCrouched; break;
				case BodyState.Prone:       fatigue -= fatigueWanePerTickProne; break;
				case BodyState.ProningUp:   fatigue -= fatigueWanePerTickProne; break;
				default: fatigue -= fatigueWanePerTick; break;
			}
			if (fatigue < defIndex) fatigue = defIndex; // clamp at 0
		}
		if (fatigue > onehundred) fatigue = onehundred; // clamp at 100 using dummy variables to hang onto the value and not get collected by garbage collector (really?  hey it was back in the old days when we didn't have incremental garbage collector, pre Unity 2019.2 versions
		if (fatigue > 80 && !fatigueWarned && !inCyberSpace) {
			twm.SendWarning(("Fatigue high"),0.1f,0,HUDColor.White,324);
			fatigueWarned = true;
		} else {
			fatigueWarned = false;
		}
	}

	void EndCrouchProneTransition() {
		if (inCyberSpace) return;

		if (currentCrouchRatio >= 1) {
			if (bodyState == BodyState.StandingUp // Should overshoot slightly.
			    || bodyState == BodyState.Standing) { // Maintain it.
				currentCrouchRatio = 1; //Clamp it
				bodyState = BodyState.Standing;
			}
		} else if (currentCrouchRatio < crouchRatio) {
			if (bodyState == BodyState.CrouchingDown // Should undershoot slightly
				|| bodyState == BodyState.Crouch) { // Maintain it.
				currentCrouchRatio = crouchRatio; //Clamp it
				bodyState = BodyState.Crouch;
			} else if (bodyState == BodyState.ProningDown // Should undershoot slightly
					   || bodyState == BodyState.Prone) { // Maintain it.
				if (currentCrouchRatio < proneRatio) {
					currentCrouchRatio = proneRatio; //Clamp it
					bodyState = BodyState.Prone;
				}
			}
		} else {
			if (bodyState == BodyState.ProningUp) { // Should overshoot slightly
				if (currentCrouchRatio > crouchRatio) {
					currentCrouchRatio = crouchRatio; //Clamp it
					bodyState = BodyState.Crouch;
				}
			}
		}
	}

	void Prone() {
		if (inCyberSpace) return;
		if (CheatNoclip) return;
		if (consoleActivated) return;
		if (!GetInput.a.Prone()) return;

		if (bodyState != BodyState.Prone && bodyState != BodyState.ProningDown) {
			bodyState = BodyState.ProningDown;
		} else {
			if (bodyState == BodyState.Prone || bodyState == BodyState.ProningDown) {
				if (CantStand()) {
					if (CantCrouch()) {
						Const.sprint(Const.a.stringTable[188]);
						return; // Can't crouch here
					} else bodyState = BodyState.ProningUp; // Can't stand, but can crouch here

					return;
				}
				
				bodyState = BodyState.StandingUp;
			}
		}
	}

	bool CantStand() {
		return Physics.CheckCapsule(cameraObject.transform.position, cameraObject.transform.position + new Vector3(0f,(1.6f-0.84f),0f), capsuleRadius, layerMask);
	}

	bool CantCrouch() {
		return Physics.CheckCapsule(cameraObject.transform.position, cameraObject.transform.position + new Vector3(0f,0.4f,0f), capsuleRadius, layerMask);
	} 

	void Crouch() {
		if (inCyberSpace) return;
		if (CheatNoclip) return;
		if (consoleActivated) return;
		if (!GetInput.a.Crouch()) return;

		if ((bodyState == BodyState.Crouch) || (bodyState == BodyState.CrouchingDown)) {
			if (CantStand()) Const.sprint(Const.a.stringTable[187]); // Can't stand here
			else bodyState = BodyState.StandingUp; // Start standing up
		} else {
			if ((bodyState == BodyState.Standing) || (bodyState == BodyState.StandingUp)) {
				bodyState = BodyState.CrouchingDown; // Start crouching down
			} else {
				if ((bodyState == BodyState.Prone) || (bodyState == BodyState.ProningDown)) {
					if ((CantCrouch())) { Const.sprint(Const.a.stringTable[188]); return; } // Can't crouch here
					
					bodyState = BodyState.ProningUp; // Start getting up to crouch
				}
			}
		}
	}

	bool GetSprintInputState() {
		if (consoleActivated) return false;

		if (GetInput.a.Sprint()) {
			if (grounded || CheatNoclip) return !(GetInput.a.CapsLockOn());
			return false;
		} else {
			if (grounded || CheatNoclip) return GetInput.a.CapsLockOn();
			return false; // Can't sprint in the air.
		}
	}

	void CyberSetup() {
		if (inCyberSpace && !cyberSetup) {
			cyberCollider.enabled = true;
			capsuleCollider.enabled = false;
			MouseLookScript.a.inCyberSpace = true; // Enable full camera rotation up/down by disabling clamp
			oldBodyState = bodyState;
			bodyState = BodyState.Standing; // Put to "standing" to prevent speed anomolies
			cyberSetup = true;
			cyberDesetup = true;
		}
	}

	void CyberDestupOrNoclipMaintain() {
		if (cyberDesetup || CheatNoclip) {
			cyberDesetup = false;
			cyberSetup = false;
			cyberCollider.enabled = false; // Can't touch dis!
			Mathf.Clamp(MouseLookScript.a.xRotation, -90f, 90f); // Pre-clamp camera rotation.
			MouseLookScript.a.inCyberSpace = false; // Disable full camera rotation up/down by enabling auto clamp.
			bodyState = oldBodyState; // Return to what we were doing in the "real world" (real lol)
			if (CheatNoclip) { // Flying cheat...also map editing mode!
				capsuleCollider.enabled = false; //na nana na, na na, can't touch dis
				leanCapsuleCollider.enabled = false;
			} else {
				capsuleCollider.enabled = true;
				leanCapsuleCollider.enabled = true;
			}
		}
	}

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

		Vector3 playerPosition = transform.localPosition;

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
			tempVec.x = (((playerPosition.z - Const.a.mapWorldMaxE)/(Const.a.mapWorldMaxW - Const.a.mapWorldMaxE)) * (Const.a.camMaxAmount * 2f)) + (Const.a.camMaxAmount * -1f);
			tempVec.y = (((playerPosition.x - Const.a.mapWorldMaxS)/(Const.a.mapWorldMaxN - Const.a.mapWorldMaxS)) * (Const.a.camMaxAmount * 2f)) + (Const.a.camMaxAmount * -1f);
			tempVec.z = automapCameraTransform.localPosition.z;
			tempVec.x = (tempVec.x * -1f) + automapCorrectionX;
			tempVec.y += automapCorrectionY;

			// private float mapTileMinX = 8; // top left corner
			// private float mapTileMaxY = -8; // top left corner
			// private float mapTileMinY = -1016; // bottom right corner
			// private float mapTileMaxX = 1016; // bottom right corner
			tempVec2b.x = (((playerPosition.z - Const.a.mapWorldMaxE)/(Const.a.mapWorldMaxW - Const.a.mapWorldMaxE)) * 1008f) + Const.a.mapTileMinX + automapTileBCorrectionX;
			tempVec2b.y = ((((playerPosition.x - Const.a.mapWorldMaxS)/(Const.a.mapWorldMaxN - Const.a.mapWorldMaxS)) * 1008f) + Const.a.mapTileMinY + automapTileBCorrectionY);

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
					automapFoWTiles[i].enabled = false;
				} else {
					tempVec2.x = automapFoWTilesRects[i].localPosition.x * -1f - automapTileCorrectionX;// - tempVec.x;
					tempVec2.y = automapFoWTilesRects[i].localPosition.y + automapTileCorrectionY;// - tempVec.y;
					if (Vector2.Distance(tempVec2,tempVec2b) < automapFoWRadius) {
						automapExplored[i] = true;
						SetAutomapTileExplored(LevelManager.a.currentLevel,i);
						automapFoWTiles[i].enabled = false;
					} else {
						automapFoWTiles[i].enabled = true;
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
				automapCanvasGO.SetActive(true);
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
			automapCanvasGO.SetActive(false);
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
		Const.sprint("Unable to connect to side map, try updating to version 1.10",Const.a.player1); // zoom at max
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

	public void EnableCheatArsenal(int lev) {
		GameObject arsenal;
		switch(lev) {
			case 0: arsenal = cheatLRarsenal; break;
			case 1: arsenal = cheatL1arsenal; break;
			case 2: arsenal = cheatL2arsenal; break;
			case 3: arsenal = cheatL3arsenal; break;
			case 4: arsenal = cheatL4arsenal; break;
			case 5: arsenal = cheatL5arsenal; break;
			case 6: arsenal = cheatL6arsenal; break;
			case 7: arsenal = cheatL7arsenal; break;
			case 8: arsenal = cheatL8arsenal; break;
			case 9: arsenal = cheatL9arsenal; break;
			case 10: arsenal = cheatL6arsenal; break;
			case 11: arsenal = cheatL6arsenal; break;
			case 12: arsenal = cheatL6arsenal; break;
			default: arsenal = cheatL1arsenal; break;
		}
		GameObject cheatArsenal = Instantiate(arsenal,transform.position,
								    Const.a.quaternionIdentity) as GameObject;
		if (cheatArsenal == null) return; // Failed!

		Transform prt = LevelManager.a.GetCurrentDynamicContainer().transform;
		cheatArsenal.transform.SetParent(prt);
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
		ConsoleEmulator.consoleMemdex = 0;
	}

	void ConsoleEnable() {
		consoleActivated = true;
		consoleplaceholderText.SetActive(true);
		consoleTitle.SetActive(true);
		consoleinpFd.enabled = true;
		consoleinpFd.ActivateInputField();
		consolebg.enabled = true;
		consoleentryText.enabled = true;
		ConsoleEmulator.consoleMemdex = 0;
	}

    public void ToggleConsole() {
		if (consoleActivated) {
			ConsoleDisable();
			PauseScript.a.PauseDisable();
		} else {
			ConsoleEnable();
			PauseScript.a.PauseEnable();
		}
    }

	public static string Save(GameObject go) {
		PlayerMovement pm = go.GetComponent<PlayerMovement>();
		int j = 0;
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		if (pm == null) {
			Debug.Log("PlayerMovement missing on savetype of Player!  GameObject.name: " + go.name);
			s1.Append("fbfibbb");
			for (j=0;j<(4096 * 13);j++) s1.Append("b");
			s1.Append("bbfffffbffbbifftttbtttu");
			return Utils.DTypeWordToSaveString(s1.ToString());
		}

		Debug.Log("Mid Save saved player at " + go.transform.localPosition.ToString());
		s1.Append(Utils.SaveTransform(go.transform));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRigidbody(go));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.playerSpeed)); // float
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.grounded)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.currentCrouchRatio)); // float
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(Utils.BodyStateToInt(pm.bodyState)));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.ladderState)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.gravliftState)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.inCyberSpace)); // bool
		s1.Append(Utils.splitChar);
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExploredR[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExplored1[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExplored2[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExplored3[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExplored4[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExplored5[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExplored6[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExplored7[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExplored8[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExplored9[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExploredG1[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExploredG2[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(pm.automapExploredG4[j])); s1.Append(Utils.splitChar); } // bool
		s1.Append(Utils.UintToString(pm.currentAutomapZoomLevel));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.CheatWallSticky)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.CheatNoclip)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.jumpTime)); // float, not a timer
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.oldVelocity.x));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.oldVelocity.y));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.oldVelocity.z)); // Vector3 (float|float|float)
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.fatigue)); // float
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.justJumped)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.fatigueFinished)); // float
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.fatigueFinished2)); // float
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.cyberSetup)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.cyberDesetup)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(Utils.BodyStateToInt(pm.oldBodyState)));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.leanTarget)); // float
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.leanShift));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.jumpSFXFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.jumpLandSoundFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.jumpJetEnergySuckTickFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.automapUpdateFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.fatigueWarned));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.ressurectingFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.doubleJumpFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.turboFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.SFX.time)); // float
		s1.Append(Utils.splitChar);
		if (!pm.SFX.isPlaying) pm.SFXIndex = -1; // Safely can set to null, not playing a sound.
		s1.Append(Utils.UintToString(pm.SFXIndex));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		PlayerMovement pm = go.GetComponent<PlayerMovement>();
		int j = 0;
		if (pm == null) {
			Debug.Log("PlayerMovement.Load failure, pm == null");
			return index + 29 + (4096 * 13);
		}

		if (index < 0) {
			Debug.Log("PlayerMovement.Load failure, index < 0");
			return index + 29 + (4096 * 13);
		}

		if (entries == null) {
			Debug.Log("PlayerMovement.Load failure, entries == null");
			return index + 29 + (4096 * 13);
		}

		float readFloatx, readFloaty, readFloatz;
		string oldpos = go.transform.localPosition.ToString();
		index = Utils.LoadTransform(go.transform,ref entries,index);
		Debug.Log("Mid Load loaded player from " + oldpos + " to " + go.transform.localPosition.ToString());
		index = Utils.LoadRigidbody(go,ref entries,index);
		pm.playerSpeed = Utils.GetFloatFromString(entries[index]); index++;
		pm.grounded = Utils.GetBoolFromString(entries[index]); index++;
		pm.currentCrouchRatio = Utils.GetFloatFromString(entries[index]); index++;
		pm.bodyState = Utils.IntToBodyState(Utils.GetIntFromString(entries[index])); index++;
		pm.ladderState = Utils.GetBoolFromString(entries[index]); index++;
		pm.gravliftState = Utils.GetBoolFromString(entries[index]); index++;
		pm.inCyberSpace = Utils.GetBoolFromString(entries[index]); index++;
		for (j=0;j<4096;j++) { pm.automapExploredR[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored1[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored2[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored3[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored4[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored5[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored6[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored7[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored8[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored9[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExploredG1[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExploredG2[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExploredG4[j] = entries[index].Equals("1"); index++; }
		if (LevelManager.a != null) pm.SetAutomapExploredReference(LevelManager.a.currentLevel);
		else pm.SetAutomapExploredReference(1);

		pm.currentAutomapZoomLevel = Utils.GetIntFromString(entries[index]); index++;
		if (pm.currentAutomapZoomLevel < 0) pm.currentAutomapZoomLevel = 0;
		if (pm.currentAutomapZoomLevel > 2) pm.currentAutomapZoomLevel = 2;
		pm.AutomapZoomAdjust();

		pm.CheatWallSticky = Utils.GetBoolFromString(entries[index]); index++;
		pm.CheatNoclip = Utils.GetBoolFromString(entries[index]); index++;
		pm.jumpTime = Utils.GetFloatFromString(entries[index]); index++; // not a timer
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		pm.oldVelocity = new Vector3(readFloatx,readFloaty,readFloatz);
		pm.fatigue = Utils.GetFloatFromString(entries[index]); index++;
		pm.justJumped = Utils.GetBoolFromString(entries[index]); index++;
		pm.fatigueFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.fatigueFinished2 = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.cyberSetup = Utils.GetBoolFromString(entries[index]); index++;
		pm.cyberDesetup = Utils.GetBoolFromString(entries[index]); index++;
		pm.oldBodyState = Utils.IntToBodyState(Utils.GetIntFromString(entries[index])); index++;
		pm.ConsoleDisable();
		pm.leanTarget = Utils.GetFloatFromString(entries[index]); index++;
		pm.leanShift = Utils.GetFloatFromString(entries[index]); index++;
		pm.leanTransform.localRotation = Quaternion.Euler(0, 0, pm.leanTarget);
		pm.leanTransform.localPosition = new Vector3(pm.leanShift,0,0);

		pm.jumpSFXFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.jumpLandSoundFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.jumpJetEnergySuckTickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.automapUpdateFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.fatigueWarned = Utils.GetBoolFromString(entries[index]); index++;
		pm.turboFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.ressurectingFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.doubleJumpFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		float sfxTime = Utils.GetFloatFromString(entries[index]); index++;
		pm.SFXIndex = Utils.GetIntFromString(entries[index]); index++;
		pm.ladderSFXFinished = 0;
		if (pm.SFXIndex >= 0) {
			pm.SFX.time = sfxTime;
			pm.SFX.clip = Const.a.sounds[pm.SFXIndex];
			Utils.PlayOneShotSavable(pm.SFX,Const.a.sounds[pm.SFXIndex]);
		}
		return index;
	}
}
