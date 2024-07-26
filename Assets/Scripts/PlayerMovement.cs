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
	public HardwareButton hwbJumpJets;
	public TextWarningsManager twm;
	public CapsuleCollider leanCapsuleCollider;
	public Image consolebg;
    public InputField consoleinpFd;
    public GameObject consoleplaceholderText;
	public GameObject consoleTitle;
	public GameObject consoleEntryButton;
	public Text consoleentryText;
	public Transform leanTransform;
	public AudioSource SFX;
	public AudioSource SFXFootsteps;
	public AudioSource SFXClothes;
	[HideInInspector] public int SFXJump = 135;
	[HideInInspector] public int SFXJumpLand = 136;
	[HideInInspector] public int SFXLadder = 137;
	public GameObject fpsCounter;
	public GameObject locationIndicator;
	public Text locationText;
	public HealthManager hm;
	public float playerSpeed; // save
	public float playerSpeedActual;
	public float playerSpeedHorizontalActual;
	public bool isSprinting = false;
	public bool grounded = false; // save
	public string lastCommand0;
	public string lastCommand1;
	public string lastCommand2;
	public string lastCommand3;
	public string lastCommand4;
	public string lastCommand5;
	public string lastCommand6;
	public int consoleMemdex;
	private float feetRayLength = 0.9f;
	[HideInInspector] public bool FatigueCheat;

	// Internal references
	public BodyState bodyState; // save
	[HideInInspector] public bool ladderState = false; // save
	public bool gravliftState = false; // save
	[HideInInspector] public bool inCyberSpace = false; // save
	[HideInInspector] public float walkAcceleration = 2000f;
	[HideInInspector] public int SFXIndex = -1; // save
	private float walkDeacceleration = 0.1f; // was 0.30f
	private float walkDeaccelerationBooster = 0.5f; // was 2f, adjusted player physics material to reduce friction for moving up stairs
	private float deceleration;
	private float walkAccelAirRatio = 0.75f;
	private float maxWalkSpeed = 3.2f;
	private float maxCyberSpeed = 5f;
	private float maxCyberUltimateSpeed = 12f;
	private float maxCrouchSpeed = 1.25f; //1.75f
	private float maxProneSpeed = .5f; //1f
	private float maxSprintSpeed = 8.8f;
	private float maxSprintSpeedFatigued = 5.5f;
	private float maxVerticalSpeed = 10f;
	private float boosterSpeedBoost = 1.2f; // ammount to boost by when booster is active
	private float jumpImpulseTime = 4.0f;
	private float jumpVelocityBoots = 0.45f;
	private float jumpVelocity = 1.1f;
	private float jumpVelocityFatigued = 0.6f;
	public float crouchRatio = 0.6f;
	public float proneRatio = 0.2f;
	public float transitionToCrouchSec = 0.2f;
	public float transitionToProneAdd = 0.1f;
	public float currentCrouchRatio = 1f; // save
	public float capsuleHeight;
	private float capsuleRadius;
	private float ladderSpeed = 0.4f;
	private float fallDamage = 75f;
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
	public float fatigue; // save
	private float jumpFatigue = 7.0f;
	private float fatigueWanePerTick = 1f;
	private float fatigueWanePerTickCrouched = 2f;
	private float fatigueWanePerTickProne = 3.5f;
	private float fatigueWaneTickSecs = 0.3f;
	private float fatiguePerWalkTick = 0.9f;
	private float fatiguePerSprintTick = 3.0f;
	[HideInInspector] public bool justJumped = false; // save
	[HideInInspector] public float fatigueFinished; // save
	[HideInInspector] public float fatigueFinished2; // save
	private int def1 = 1;
	public bool running = false;
	public float relForward = 0f;
	public float relSideways = 0f;
	[HideInInspector] public bool cyberSetup = false; // save
	[HideInInspector] public bool cyberDesetup = false; // save
	[HideInInspector] public SphereCollider cyberCollider;
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
	private float leanMaxShift = 0.8f;
	[HideInInspector] public float jumpSFXFinished; // save
	[HideInInspector] public float ladderSFXFinished;
	private float ladderSFXIntervalTime = 1f;
	private float jumpSFXIntervalTime = 1f;
	[HideInInspector] public float jumpLandSoundFinished; // save
	[HideInInspector] public float jumpJetEnergySuckTickFinished; // save
	private float jumpJetEnergySuckTick = 1f;
	private Vector3 tempVec;
	private Vector2 tempVec2;
	private int tempInt;
	private float leanSpeed = 70f;
	[HideInInspector] public bool Notarget = false; // for cheat to disable enemy sight checks against this player
	[HideInInspector] public bool fatigueWarned; // save
	[HideInInspector] public float ressurectingFinished; // save
	private float burstForce = 35f;
	[HideInInspector] public float doubleJumpFinished; // save
	private Vector3 playerHome;
	[HideInInspector] public float turboFinished = 0f; // save
	[HideInInspector] public float turboCyberTime = 15f;
	[HideInInspector] public bool inCyberTube = false;
	[HideInInspector] public float stepFinished;
	[HideInInspector] public float rustleFinished;
	private int doubleJumpTicks = 0;
	private Vector3 tempVecRbody;
	private bool inputtingMovement;
	private float accel;
	private RaycastHit tempHit;
	public float floorDot;
	public Vector3 floorAng;
	private float slideAngle = 0.9f;
	private float gravFinished;

	public static PlayerMovement a;

	void Awake() {
		a = this;
	}

    void Start() {
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
		doubleJumpFinished = PauseScript.a.relativeTime;
		doubleJumpTicks = 0;
		turboFinished = PauseScript.a.relativeTime;
		playerHome = transform.localPosition;
		ConsoleEmulator.lastCommand = new string[7];
		ConsoleEmulator.consoleMemdex = consoleMemdex = 0;
		FatigueCheat = false;
		if (Application.platform == RuntimePlatform.Android) {
		    fpsCounter.SetActive(true);
		}

		stepFinished = PauseScript.a.relativeTime;
		rustleFinished = PauseScript.a.relativeTime;
    }

	void Update() {
		// Always allowed items, even when paused...
		lastCommand0 = ConsoleEmulator.lastCommand[0];
		lastCommand1 = ConsoleEmulator.lastCommand[1];
		lastCommand2 = ConsoleEmulator.lastCommand[2];
		lastCommand3 = ConsoleEmulator.lastCommand[3];
		lastCommand4 = ConsoleEmulator.lastCommand[4];
		lastCommand5 = ConsoleEmulator.lastCommand[5];
		lastCommand6 = ConsoleEmulator.lastCommand[6];
		consoleMemdex = ConsoleEmulator.consoleMemdex;
		ConsoleEmulator.ConsoleUpdate();

		// Bug Hunter feedback (puts it into their screenshots for me)
		if (locationIndicator.activeInHierarchy) {
			locationText.text = "location: "
								+ (transform.position.x.ToString("00.00")
								+ " " + transform.position.y.ToString("00.00")
								+ " " + transform.position.z.ToString("00.00"));
		}

		// Prevent falling or movement while menu is up. Force it here in case
		// PauseScript didn't catch it at startup.
		if (PauseScript.a.mainMenu.activeSelf == true) {
			rbody.useGravity = false;
			rbody.Sleep();
			return;
		}

		if (PauseScript.a.Paused()
			|| (ressurectingFinished >= PauseScript.a.relativeTime)) {
			return;
		}

		// Normal play when not paused...

		rbody.WakeUp(); // Force player physics to never sleep.		
		if (rbody.isKinematic) rbody.isKinematic = false; // Allow physics.
		CyberSetup();
		if (!inCyberSpace) {
			CyberDestupOrNoclipMaintain();
		} else {
			PlayerHealth.a.makingNoise = true; // Cyber enemies more aware.
		}

		isSprinting = GetSprintInputState();
		Crouch();
		Prone();
		EndCrouchProneTransition();
		FatigueApply(); // Here fatigue me out, except in cyberspace
		Automap.a.UpdateAutomap(transform.localPosition); // Update the map.
	}

	void FixedUpdate() {
		// Readout for debugging in Inspector.
		playerSpeedActual = rbody.velocity.magnitude;
		
		Vector2 hz = new Vector2(rbody.velocity.x, rbody.velocity.z);
		playerSpeedHorizontalActual = hz.magnitude;

		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) return;
		if (ressurectingFinished > PauseScript.a.relativeTime) return;
		if (consoleActivated) return;

		// Crouch/Prone by shrinking the capsule height.
		if (capsuleCollider.height != (currentCrouchRatio * 2f)) {
			capsuleCollider.height = currentCrouchRatio * 2f;
		}

		// Lean capsule should always match stalk capsule.
		if (leanCapsuleCollider.height != capsuleCollider.height) {
			leanCapsuleCollider.height = capsuleCollider.height;
		}

		SetRunningRelForwardsAndSidewaysFlags();
		playerSpeed = GetBasePlayerSpeed();
		ApplyBodyStateLerps(); // Handle body lerping for smooth transitions.
		Noclip();
		ApplyGroundFriction();
		bool grav = GetGravity();

		// Avoid useless setting of the rbody.
		if (rbody.useGravity != grav) rbody.useGravity = grav;
		if (grav) ApplyGravity();

		if (inCyberSpace) {
			if (rbody.velocity.magnitude > playerSpeed && !CheatNoclip) {
				rbody.velocity = rbody.velocity.normalized * playerSpeed;
			}

			CyberspaceMovement();
			return;
		}

		// Non-cyberspace Normal Movement
		// --------------------------------------------------------------------
		// Clamp horizontal movement speed.
		horizontalMovement = GetClampedHorizontalMovement();
		RigidbodySetVelocityX(rbody, horizontalMovement.x);
		RigidbodySetVelocityZ(rbody, horizontalMovement.y); // NOT A BUG:
															// Already passed
															// rbody.velocity.z
															// into the .y of
															// this Vector2.
		// Clamp vertical movement speed.
		verticalMovement = GetClampedVerticalMovement();
		RigidbodySetVelocityY(rbody, verticalMovement);
		Lean();
		WalkRun();
		LadderStates();			 
		Jump();
		FallDamage();
		FeetRayChecks();
		oldVelocity = rbody.velocity;
	}

	// Parse surface below to allow for playing different footstep sets for
	// different types of flooring.
	void FeetRayChecks() {
		if (inCyberSpace) return;

		// Using value of 1.06 = (player capsule height / 2) + 0.06 = 1 + 0.06;
		bool successfulRay = Physics.Raycast(transform.position, Vector3.down,
											 out tempHit,feetRayLength,
											 Const.a.layerMaskPlayerFeet);

		// Success here means hit a useable something.
		// If a ray hits a wall or other unusable something, that's not success
		// and print "Can't use <something>"
		if (!successfulRay || tempHit.collider == null) {
			// Automatically set grounded false, prevents ability to climb any wall
			if (!CheatWallSticky || gravliftState) grounded = false;
			return;
		}

		GameObject hitGO = tempHit.collider.transform.gameObject;
		if (hitGO == null) {
			// Automatically set grounded false, prevents ability to climb any wall
			if (!CheatWallSticky || gravliftState) grounded = false;
			return;
		}

		if (!Const.a.Footsteps) {
			SFXClothes.Stop();
			SFXFootsteps.Stop();
			return;
		}

		if (rbody.velocity.sqrMagnitude <= 0f) {
			SFXClothes.Stop();
			return;
		}
		if ((relForward + relSideways) == 0) return;

		if (rustleFinished < PauseScript.a.relativeTime) {
			rustleFinished = isSprinting
							 ? PauseScript.a.relativeTime
							   + UnityEngine.Random.Range(0.4f,0.6f)
							 : PauseScript.a.relativeTime
							   + UnityEngine.Random.Range(0.8f,1.2f);

			AudioClip rustle =
				Const.a.sounds[UnityEngine.Random.Range(459,465 + 1)];

			Utils.PlayOneShotSavable(SFXClothes,rustle,
									 UnityEngine.Random.Range(0.3f,0.5f));
		}
		if (!grounded) return;

		PrefabIdentifier prefID = hitGO.GetComponent<PrefabIdentifier>();
		if (prefID == null) return;

		// Footsteps
		if (stepFinished < PauseScript.a.relativeTime) {
			stepFinished = isSprinting
						   ? PauseScript.a.relativeTime
							 + UnityEngine.Random.Range(0.2f,0.3f)
						   : PauseScript.a.relativeTime
							 + UnityEngine.Random.Range(0.35f,0.65f);

			FootStepType fstep = GetFootstepTypeForPrefab(prefID.constIndex);
			AudioClip stcp = FootStepSound(fstep);
			Utils.PlayOneShotSavable(SFXFootsteps,stcp,
									 UnityEngine.Random.Range(0.4f,0.55f));
		}
	}

	FootStepType GetFootstepTypeForPrefab(int pid) {
		switch(pid) {
			case 0: return FootStepType.None;
			case 1: return FootStepType.Glass;
			case 2: return FootStepType.Squish;
			case 3: return FootStepType.Squish;
			case 4: return FootStepType.Squish;
			case 5: return FootStepType.Squish;
			case 6: return FootStepType.Squish;
			case 7: return FootStepType.Squish;
			case 8: return FootStepType.Squish;
			case 9: return FootStepType.Squish;
			case 10: return FootStepType.Squish;
			case 11: return FootStepType.Metpanel;
			case 12: return FootStepType.Marble;
			case 13: return FootStepType.Metal2;
			case 14: return FootStepType.Metal2;
			case 15: return FootStepType.Metal2;
			case 16: return FootStepType.Metal2;
			case 17: return FootStepType.Metal2;
			case 18: return FootStepType.Metal2;
			case 19: return FootStepType.Glass;
			case 20: return FootStepType.Wood2;
			case 21: return FootStepType.None;
			case 22: return FootStepType.None;
			case 23: return FootStepType.Plastic2;
			case 24: return FootStepType.Plastic2;
			case 25: return FootStepType.Plastic2;
			case 26: return FootStepType.Plastic2;
			case 27: return FootStepType.Plastic2;
			case 28: return FootStepType.Plastic2;
			case 29: return FootStepType.Plastic2;
			case 30: return FootStepType.Plastic2;
			case 31: return FootStepType.Plastic2;
			case 32: return FootStepType.Plastic2;
			case 33: return FootStepType.Plastic2;
			case 34: return FootStepType.Plastic2;
			case 35: return FootStepType.Plastic2;
			case 36: return FootStepType.Plastic2;
			case 37: return FootStepType.Plastic2;
			case 38: return FootStepType.Plastic2;
			case 39: return FootStepType.Plastic2;
			case 40: return FootStepType.Plastic2;
			case 41: return FootStepType.Plastic;
			case 42: return FootStepType.Plastic;
			case 43: return FootStepType.Plastic;
			case 44: return FootStepType.Plastic;
			case 45: return FootStepType.Plastic;
			case 46: return FootStepType.Plastic;
			case 47: return FootStepType.Plastic;
			case 48: return FootStepType.Plastic2;
			case 49: return FootStepType.Plastic2;
			case 50: return FootStepType.Carpet;
			case 51: return FootStepType.Metpanel;
			case 52: return FootStepType.Metpanel;
			case 53: return FootStepType.Plastic2;
			case 54: return FootStepType.Gravel;
			case 55: return FootStepType.Gravel;
			case 56: return FootStepType.Metpanel;
			case 57: return FootStepType.Metpanel;
			case 58: return FootStepType.Plastic;
			case 59: return FootStepType.Plastic;
			case 60: return FootStepType.Plastic;
			case 61: return FootStepType.Marble;
			case 62: return FootStepType.Metal;
			case 63: return FootStepType.Metal;
			case 64: return FootStepType.Sand;
			case 65: return FootStepType.Sand;
			case 66: return FootStepType.Sand;
			case 67: return FootStepType.Plastic;
			case 68: return FootStepType.Plastic;
			case 69: return FootStepType.Plastic;
			case 70: return FootStepType.Carpet;
			case 71: return FootStepType.Metpanel;
			case 72: return FootStepType.Marble;
			case 73: return FootStepType.Marble;
			case 74: return FootStepType.Plaster;
			case 75: return FootStepType.Carpet;
			case 76: return FootStepType.Marble;
			case 77: return FootStepType.Glass;
			case 78: return FootStepType.Metal;
			case 79: return FootStepType.Grate;
			case 80: return FootStepType.Rubber;
			case 81: return FootStepType.Rubber;
			case 82: return FootStepType.Metal2;
			case 83: return FootStepType.Metal2;
			case 84: return FootStepType.Metal2;
			case 85: return FootStepType.Metal2;
			case 86: return FootStepType.Metal2;
			case 87: return FootStepType.Metal2;
			case 88: return FootStepType.Metal2;
			case 89: return FootStepType.Metal;
			case 90: return FootStepType.Plastic;
			case 91: return FootStepType.Plastic;
			case 92: return FootStepType.Plastic;
			case 93: return FootStepType.Glass;
			case 94: return FootStepType.Grass;
			case 95: return FootStepType.Grass;
			case 96: return FootStepType.Grass;
			case 97: return FootStepType.Water;
			case 98: return FootStepType.Squish;
			case 99: return FootStepType.Squish;
			case 100: return FootStepType.Squish;
			case 101: return FootStepType.GrittyCrete;
			case 102: return FootStepType.GrittyCrete;
			case 103: return FootStepType.GrittyCrete;
			case 104: return FootStepType.GrittyCrete;
			case 105: return FootStepType.GrittyCrete;
			case 106: return FootStepType.GrittyCrete;
			case 107: return FootStepType.GrittyCrete;
			case 108: return FootStepType.GrittyCrete;
			case 109: return FootStepType.GrittyCrete;
			case 110: return FootStepType.Squish;
			case 111: return FootStepType.GrittyCrete;
			case 112: return FootStepType.Metal;
			case 113: return FootStepType.Panel;
			case 114: return FootStepType.Panel;
			case 115: return FootStepType.Panel;
			case 116: return FootStepType.Metpanel;
			case 117: return FootStepType.Metpanel;
			case 118: return FootStepType.Panel;
			case 119: return FootStepType.Panel;
			case 120: return FootStepType.Metpanel;
			case 121: return FootStepType.Metpanel;
			case 122: return FootStepType.Glass;
			case 123: return FootStepType.Panel;
			case 124: return FootStepType.Rubber;
			case 125: return FootStepType.Rubber;
			case 126: return FootStepType.Glass;
			case 127: return FootStepType.Metal;
			case 128: return FootStepType.Glass;
			case 129: return FootStepType.Metal;
			case 130: return FootStepType.Grate;
			case 131: return FootStepType.Metal;
			case 132: return FootStepType.Metal;
			case 133: return FootStepType.Metal;
			case 134: return FootStepType.Metal;
			case 135: return FootStepType.Metpanel;
			case 136: return FootStepType.Metpanel;
			case 137: return FootStepType.Metal;
			case 138: return FootStepType.Metal;
			case 139: return FootStepType.Metpanel;
			case 140: return FootStepType.Metpanel;
			case 141: return FootStepType.Metal;
			case 142: return FootStepType.Metal;
			case 143: return FootStepType.Metal;
			case 144: return FootStepType.Vent;
			case 145: return FootStepType.Vent;
			case 146: return FootStepType.Vent;
			case 147: return FootStepType.Vent;
			case 148: return FootStepType.Vent;
			case 149: return FootStepType.Plastic;
			case 150: return FootStepType.Plastic;
			case 151: return FootStepType.Plastic;
			case 152: return FootStepType.Plastic;
			case 153: return FootStepType.Plastic;
			case 154: return FootStepType.Plastic;
			case 155: return FootStepType.Plastic;
			case 156: return FootStepType.Plastic;
			case 157: return FootStepType.Plastic;
			case 158: return FootStepType.Plastic;
			case 159: return FootStepType.Plastic;
			case 160: return FootStepType.Panel;
			case 161: return FootStepType.Panel;
			case 162: return FootStepType.Plastic2;
			case 163: return FootStepType.Plastic2;
			case 164: return FootStepType.Plastic2;
			case 165: return FootStepType.Plastic2;
			case 166: return FootStepType.Plastic2;
			case 167: return FootStepType.Plastic2;
			case 168: return FootStepType.Plastic2;
			case 169: return FootStepType.Panel;
			case 170: return FootStepType.Panel;
			case 171: return FootStepType.Panel;
			case 172: return FootStepType.Panel;
			case 173: return FootStepType.Panel;
			case 174: return FootStepType.Panel;
			case 175: return FootStepType.Panel;
			case 176: return FootStepType.Panel;
			case 177: return FootStepType.Panel;
			case 178: return FootStepType.Plastic;
			case 179: return FootStepType.Plastic;
			case 180: return FootStepType.Plastic;
			case 181: return FootStepType.Plastic;
			case 182: return FootStepType.Plastic;
			case 183: return FootStepType.Plastic;
			case 184: return FootStepType.Plastic;
			case 185: return FootStepType.Plastic;
			case 186: return FootStepType.Plastic;
			case 187: return FootStepType.Glass;
			case 188: return FootStepType.Plastic;
			case 189: return FootStepType.Metal;
			case 190: return FootStepType.Plastic;
			case 191: return FootStepType.Plastic;
			case 192: return FootStepType.Plastic;
			case 193: return FootStepType.Plastic;
			case 194: return FootStepType.Plastic;
			case 195: return FootStepType.Plastic;
			case 196: return FootStepType.Metal;
			case 197: return FootStepType.Metal2;
			case 198: return FootStepType.Metal2;
			case 199: return FootStepType.Metal;
			case 200: return FootStepType.Metal2;
			case 201: return FootStepType.Metal2;
			case 202: return FootStepType.Metal2;
			case 203: return FootStepType.Metal;
			case 204: return FootStepType.Metpanel;
			case 205: return FootStepType.Metpanel;
			case 206: return FootStepType.Metpanel;
			case 207: return FootStepType.Metpanel;
			case 208: return FootStepType.Metal;
			case 209: return FootStepType.Metal;
			case 210: return FootStepType.Metal;
			case 211: return FootStepType.Metal;
			case 212: return FootStepType.Metal;
			case 213: return FootStepType.Metal;
			case 214: return FootStepType.Metal;
			case 215: return FootStepType.Metal;
			case 216: return FootStepType.Metal;
			case 217: return FootStepType.Metal;
			case 218: return FootStepType.Metal;
			case 219: return FootStepType.Metal;
			case 220: return FootStepType.Metal;
			case 221: return FootStepType.Glass;
			case 222: return FootStepType.Metal;
			case 223: return FootStepType.Metal;
			case 224: return FootStepType.Metal;
			case 225: return FootStepType.Metal;
			case 226: return FootStepType.Metal;
			case 227: return FootStepType.Metal;
			case 228: return FootStepType.Metal;
			case 229: return FootStepType.Metal;
			case 230: return FootStepType.Metal;
			case 231: return FootStepType.Grate;
			case 232: return FootStepType.Plastic;
			case 233: return FootStepType.Plastic;
			case 234: return FootStepType.Metpanel;
			case 235: return FootStepType.Glass;
			case 236: return FootStepType.Glass;
			case 237: return FootStepType.Glass;
			case 238: return FootStepType.Metal;
			case 239: return FootStepType.Metal;
			case 240: return FootStepType.Metal;
			case 241: return FootStepType.Plastic;
			case 242: return FootStepType.Plastic;
			case 243: return FootStepType.Plastic;
			case 244: return FootStepType.Plastic;
			case 245: return FootStepType.Plastic;
			case 246: return FootStepType.Plastic;
			case 247: return FootStepType.Plastic;
			case 248: return FootStepType.Plastic;
			case 249: return FootStepType.Plastic;
			case 250: return FootStepType.Plastic;
			case 251: return FootStepType.Plastic;
			case 252: return FootStepType.Plastic;
			case 253: return FootStepType.Panel;
			case 254: return FootStepType.Panel;
			case 255: return FootStepType.Panel;
			case 256: return FootStepType.Plastic;
			case 257: return FootStepType.Plastic;
			case 258: return FootStepType.Plastic;
			case 259: return FootStepType.Plastic;
			case 260: return FootStepType.Glass;
			case 261: return FootStepType.Glass;
			case 262: return FootStepType.Grate;
			case 263: return FootStepType.Grate;
			case 264: return FootStepType.Grate;
			case 265: return FootStepType.Grate;
			case 266: return FootStepType.Plastic;
			case 267: return FootStepType.Plastic;
			case 268: return FootStepType.Plastic;
			case 269: return FootStepType.Plastic;
			case 270: return FootStepType.Glass;
			case 271: return FootStepType.Glass;
			case 272: return FootStepType.Plastic;
			case 273: return FootStepType.Plastic;
			case 274: return FootStepType.Plastic;
			case 275: return FootStepType.Plastic;
			case 276: return FootStepType.Plastic;
			case 277: return FootStepType.Plastic;
			case 278: return FootStepType.Plastic;
			case 279: return FootStepType.Glass;
			case 280: return FootStepType.Marble;
			case 281: return FootStepType.Marble;
			case 282: return FootStepType.Marble;
			case 283: return FootStepType.Marble;
			case 284: return FootStepType.Marble;
			case 285: return FootStepType.Marble;
			case 286: return FootStepType.Marble;
			case 287: return FootStepType.Marble;
			case 288: return FootStepType.Plastic;
			case 289: return FootStepType.Plastic;
			case 290: return FootStepType.Plastic;
			case 291: return FootStepType.Plastic;
			case 292: return FootStepType.Metal;
			case 293: return FootStepType.Metal;
			case 294: return FootStepType.Metal;
			case 295: return FootStepType.Metal;
			case 296: return FootStepType.Metal;
			case 297: return FootStepType.Metal;
			case 298: return FootStepType.Metal;
			case 299: return FootStepType.Metal;
			case 300: return FootStepType.Metal;
			case 301: return FootStepType.Metal;
			case 302: return FootStepType.Rubber;
			case 303: return FootStepType.Rubber;
			case 304: return FootStepType.Rubber;
			case 305: return FootStepType.Metal;
			case 306: return FootStepType.Plaster;

			// Props
			case 458: return FootStepType.Metpanel;
			case 459: return FootStepType.Metpanel;
			case 460: return FootStepType.Metpanel;
			case 461: return FootStepType.Metal;

			case 463: return FootStepType.Metal;
			case 464: return FootStepType.Wood2;

			case 472: return FootStepType.Wood2;
			case 473: return FootStepType.Wood2;
			case 474: return FootStepType.Wood2;
			case 475: return FootStepType.Wood2;
			case 476: return FootStepType.Wood2;
			case 477: return FootStepType.Metpanel;
			case 478: return FootStepType.Metpanel;
			case 479: return FootStepType.Metpanel;

			case 500: return FootStepType.Metal;

			case 515: return FootStepType.Panel;
			case 516: return FootStepType.Metal;

			case 525: return FootStepType.Metal;
			case 526: return FootStepType.Metal;
			case 527: return FootStepType.Grate;
			case 528: return FootStepType.Grate;
			case 529: return FootStepType.Grate;
			default: return FootStepType.Plastic;
		}
	}

	AudioClip FootStepSound(FootStepType fstep) {
		switch(fstep) {
			case FootStepType.None: return Const.a.sounds[0];
			// + 1 because its exclusive, :eyeroll:
			case FootStepType.Carpet:      return Const.a.sounds[UnityEngine.Random.Range(268,275 + 1)];
			case FootStepType.Concrete:    return Const.a.sounds[UnityEngine.Random.Range(276,283 + 1)];
			case FootStepType.GrittyCrete: return Const.a.sounds[UnityEngine.Random.Range(284,291 + 1)];
			case FootStepType.Grass:       return Const.a.sounds[UnityEngine.Random.Range(292,299 + 1)];
			case FootStepType.Gravel:      return Const.a.sounds[UnityEngine.Random.Range(300,307 + 1)];
			case FootStepType.Rock:        return Const.a.sounds[UnityEngine.Random.Range(308,315 + 1)];
			case FootStepType.Glass:       return Const.a.sounds[UnityEngine.Random.Range(316,323 + 1)];
			case FootStepType.Marble:      return Const.a.sounds[UnityEngine.Random.Range(324,331 + 1)];
			case FootStepType.Metal:       return Const.a.sounds[UnityEngine.Random.Range(332,339 + 1)];
			case FootStepType.Grate:       return Const.a.sounds[UnityEngine.Random.Range(340,347 + 1)];
			case FootStepType.Metal2:      return Const.a.sounds[UnityEngine.Random.Range(348,355 + 1)];
			case FootStepType.Metpanel:    return Const.a.sounds[UnityEngine.Random.Range(356,363 + 1)];
			case FootStepType.Panel:       return Const.a.sounds[UnityEngine.Random.Range(364,371 + 1)];
			case FootStepType.Plaster:     return Const.a.sounds[UnityEngine.Random.Range(372,379 + 1)];
			case FootStepType.Plastic:     return Const.a.sounds[UnityEngine.Random.Range(380,387 + 1)];
			case FootStepType.Plastic2:    return Const.a.sounds[UnityEngine.Random.Range(388,395 + 1)];
			case FootStepType.Rubber:      return Const.a.sounds[UnityEngine.Random.Range(396,403 + 1)];
			case FootStepType.Sand:        return Const.a.sounds[UnityEngine.Random.Range(404,411 + 1)];
			case FootStepType.Squish:      return Const.a.sounds[UnityEngine.Random.Range(412,427 + 1)];
			case FootStepType.Vent:        return Const.a.sounds[UnityEngine.Random.Range(428,437 + 1)];
			case FootStepType.Water:       return Const.a.sounds[UnityEngine.Random.Range(438,442 + 1)];
			case FootStepType.Wood:        return Const.a.sounds[UnityEngine.Random.Range(443,450 + 1)];
			case FootStepType.Wood2:       return Const.a.sounds[UnityEngine.Random.Range(451,458 + 1)];
		}

		return Const.a.sounds[0]; // null wav
	}

	float GetBasePlayerSpeed() {
		// Cheat speeds
		if (CheatNoclip && isSprinting) return maxCyberSpeed * 2.5f;
		if (CheatNoclip) return maxCyberSpeed * 1.5f;

		if (inCyberSpace) return maxCyberSpeed; //Cyber space speed

		float retval = maxWalkSpeed;
		bonus = 0f;
		if (Inventory.a.BoosterActive()) bonus = boosterSpeedBoost;
		switch (bodyState) {
			case BodyState.Standing: 		retval = maxWalkSpeed;   break;
			case BodyState.Crouch: 			retval = maxCrouchSpeed; break;
			case BodyState.CrouchingDown: 	retval = maxCrouchSpeed; break;
			case BodyState.StandingUp: 		retval = maxWalkSpeed;   break;
			case BodyState.Prone: 			retval = maxProneSpeed;  break;
			case BodyState.ProningDown: 	retval = maxProneSpeed;  break;
			case BodyState.ProningUp: 		retval = maxProneSpeed;  break;
		}

		if ((isSprinting || Inventory.a.BoosterActive()) && running) {
			if (fatigue > 80f && !Inventory.a.BoosterActive()) {
				retval = maxSprintSpeedFatigued;
			} else {
				retval = maxSprintSpeed;
			}

			if (bodyState == BodyState.Standing
				|| bodyState == BodyState.Crouch
				|| bodyState == BodyState.CrouchingDown) {

				// Subtract off the difference in speed between walking and
				// crouching from the sprint speed
				retval -= ((maxWalkSpeed - maxCrouchSpeed)*1.5f);
			} else if (bodyState == BodyState.Prone
					   || bodyState == BodyState.ProningDown
					   || bodyState == BodyState.ProningUp) {

				// Subtract off the difference in speed between walking and
				// proning from the sprint speed.
				retval -= ((maxWalkSpeed - maxProneSpeed)*2f);
			}
		}

		return retval + bonus;
	}

	void ApplyBodyStateLerps() {
		switch (bodyState) {
		case BodyState.CrouchingDown:
			currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio,-0.01f,
												   ref crouchingVelocity,
												   transitionToCrouchSec);
			break;
		case BodyState.StandingUp:
			lastCrouchRatio = currentCrouchRatio;
			currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio,1.01f,
												   ref crouchingVelocity,
												   transitionToCrouchSec);

			LocalPositionSetY(transform,(((currentCrouchRatio - lastCrouchRatio)
										  * capsuleHeight) / 2)
										+ transform.position.y);
			break;
		case BodyState.ProningDown:
			currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio,-0.01f,
												  ref crouchingVelocity,
												  transitionToCrouchSec);
			break;
		case BodyState.ProningUp: // Prone to crouch
			lastCrouchRatio = currentCrouchRatio;
			currentCrouchRatio = Mathf.SmoothDamp(currentCrouchRatio,1.01f,
												  ref crouchingVelocity,
												  (transitionToCrouchSec
												   + transitionToProneAdd));

			LocalPositionSetY(transform,(((currentCrouchRatio - lastCrouchRatio)
										  * capsuleHeight) / 2)
										+ transform.position.y);
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

		// Now check for thumbstick/joystick input
		Vector2 leftThumbstick = new Vector2(
			Input.GetAxisRaw("JoyAxis1"), // Horizontal Left < 0, Right > 0
			Input.GetAxisRaw("JoyAxis2") * -1f // Vertical Down > 0,
											   //   Up < 0 Inverted
		);

		Vector2 leftTouchstick = GetInput.a.leftTS.Coordinate();
		relForward += leftThumbstick.y + leftTouchstick.y;
		relSideways += leftThumbstick.x + leftTouchstick.x;

		// We are mashing a run button down.
		running = ((relForward != 0) || (relSideways != 0));

		float leanReset = relForward != 0.0f ? 1f : 0f;

		if (leanReset == 0) return; // Don't affect lean moving non-forward.
		if (inCyberSpace) return; // Don't affect lean transform in cyber.

		if (leanTarget > 0) {
			if (Mathf.Abs(leanTarget - 0) < 0.05f) {
				leanShift = 0;
				leanTarget = 0;
			} else {
				leanTarget -= (leanSpeed * Time.deltaTime * leanReset);
			}

			if (Mathf.Abs(leanShift - 0) < 0.05f) {
				leanShift = 0;
				leanTarget = 0;
			} else {
				leanShift = -1 * (leanMaxShift * (leanTarget/leanMaxAngle))
							* leanReset;
			}
		} else {
			if (Mathf.Abs(leanTarget - 0) < 0.05f) {
				leanShift = 0;
				leanTarget = 0;
			} else {
				leanTarget += (leanSpeed * Time.deltaTime * leanReset);
			}

			if (Mathf.Abs(leanShift - 0) < 0.05f) {
				leanShift = 0;
				leanTarget = 0;
			} else {
				leanShift = leanMaxShift * (leanTarget/(leanMaxAngle * -1))
							* leanReset;
			}
		}
	}

	void ApplyGravity() {
// 		if (gravFinished < PauseScript.a.relativeTime) {
// 			gravFinished = PauseScript.a.relativeTime + 0.01f;
// 			rbody.AddRelativeForce(Vector3.down * 9.83f * 9.83f);
// 		}
	}

	void ApplyGroundFriction() {
		if (running) {
			if (!CheatNoclip) {
				if (isSprinting) return;
			} else {
				if (isSprinting) {
					if (GetInput.a.SwimUp()) return;
					if (GetInput.a.SwimDn()) return;
				}
			}
		}

		tempVecRbody = rbody.velocity;
		Vector3 movDir = rbody.velocity;
		movDir.y = 0;
		movDir = movDir.normalized;
		if (Vector3.Dot(movDir,floorAng) < 0f && running) return;

		deceleration = walkDeacceleration;
		if (!grounded && !ladderState && !justJumped) deceleration *= 1.5f;
		if (CheatNoclip) {
			deceleration = 0.05f;
			// Prevent gravity from affecting and decelerate like a horizontal.
			tempVecRbody.y = Mathf.SmoothDamp(rbody.velocity.y,0,
											  ref walkDeaccelerationVoly,
											  deceleration);
			if (isSprinting && running) return;
		} else {
			if (Inventory.a.BoosterActive()) {
				deceleration = walkDeaccelerationBooster;
			}

			tempVecRbody.y = rbody.velocity.y; // Don't affect gravity and let 
											   // gravity keep pulling down.
		}

		tempVecRbody.x = Mathf.SmoothDamp(rbody.velocity.x,0,
										  ref walkDeaccelerationVolx,
										  deceleration);

		tempVecRbody.z = Mathf.SmoothDamp(rbody.velocity.z,0,
										  ref walkDeaccelerationVolz,
										  deceleration);
		if (inCyberSpace) {
			tempVecRbody.y = Mathf.SmoothDamp(rbody.velocity.y,0,
											  ref walkDeaccelerationVolz,
											  deceleration);
		}

		rbody.velocity = tempVecRbody;
	}

	void Lean() {
		if (inCyberSpace) return; // 6dof handled in MouseLookScript for this.
		if (CheatNoclip) return;

		if (GetInput.a.LeanRight()) {
			float trigFrac = Input.GetAxisRaw("JoyAxis3"); // L2
			float spd = leanSpeed;
			if (trigFrac > 0) spd *= trigFrac;
			leanTarget -= (spd * Time.deltaTime);
			if (leanTarget < (leanMaxAngle * -1)) {
				leanTarget = (leanMaxAngle * -1);
			}

			leanShift = -1 * (leanMaxShift * (leanTarget/leanMaxAngle));
		}
		if (GetInput.a.LeanLeft()) {
			float trigFrac = Input.GetAxisRaw("JoyAxis6"); // R2
			float spd = leanSpeed;
			if (trigFrac > 0) spd *= trigFrac;
			leanTarget += (spd * Time.deltaTime);
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
		if (bodyState == BodyState.StandingUp
			|| bodyState == BodyState.CrouchingDown
			|| bodyState == BodyState.ProningDown
			|| bodyState == BodyState.ProningUp) {
// 			Debug.Log("Crouching gravity! " + rbody.useGravity.ToString());
			return true;
		}
		if (isSprinting) return true;

		// Disables gravity when touching steep ground to prevent player
		// sliding down ramps...hacky?
		if (grounded && floorDot >= slideAngle) return false;
		return true;
	}

	// Get input for Jump and set impulse time, removed
	// "&& (ladderState == false)" since I want to be able to jump off a ladder
	void Jump() {
		if (CheatNoclip && !Inventory.a.JumpJetsActive()) return;

		if (doubleJumpFinished < PauseScript.a.relativeTime) {
			doubleJumpTicks--;
			if (doubleJumpTicks < 0) doubleJumpTicks = 0;
		}

		if ((!gravliftState && GetInput.a.Jump())
			|| gravliftState && GetInput.a.JumpDown()) {

			if (!justJumped) {
				if (grounded || gravliftState || Inventory.a.JumpJetsActive()) {
					jumpTime = jumpImpulseTime;
					doubleJumpFinished = PauseScript.a.relativeTime + Const.a.doubleClickTime;
					doubleJumpTicks++;
					justJumped = true;
					if (!Inventory.a.JumpJetsActive() && !Inventory.a.BoosterActive()) {
						fatigue += jumpFatigue;
					}
				} else {
					if (ladderState) {
						jumpTime = jumpImpulseTime;
						justJumped = true;
						if (!Inventory.a.JumpJetsActive() && !Inventory.a.BoosterActive()) {
							fatigue += jumpFatigue;
						}
					}
				}
			}

			if (Inventory.a.BoosterActive() && Inventory.a.BoosterSetToBoost()) {
				if (justJumped && doubleJumpTicks == 2) {
					// Booster thrust
					rbody.AddForce(new Vector3(transform.forward.x * burstForce,
											   transform.forward.y * burstForce,
											   transform.forward.z * burstForce),
											   ForceMode.Impulse);
					PlayerEnergy.a.TakeEnergy(22f);
					if (BiomonitorGraphSystem.a != null) {
						BiomonitorGraphSystem.a.EnergyPulse(22f);
					}

					justJumped = false;
					jumpTime = 0;
					doubleJumpTicks = 0;

					// Make sure we can't do it again right away.
					doubleJumpFinished = PauseScript.a.relativeTime - 1f;
				}
			}
		}

		if (staminupActive || FatigueCheat) fatigue = 0;
		
		// Perform Jump
		float jumpVelocityApply = jumpVelocity * rbody.mass;
		Vector3 jumpVel = new Vector3 (0,jumpVelocityApply,0);
		float jumpTimeMod = jumpTime;
		if (isSprinting) jumpTimeMod *= 0.5f;
		while (jumpTimeMod > 0) { // Why is this a `while` instead of an `if`??
							   // Because otherwise it don't work, duh!
			jumpTimeMod -= Time.smoothDeltaTime;
			if (fatigue > 80 && !Inventory.a.JumpJetsActive()) {
				jumpVelocityApply = jumpVelocityFatigued * rbody.mass;
				jumpVel.y = jumpVelocityApply;
			}

			if (Inventory.a.JumpJetsActive()) {
				float energysuck = 25f;
				jumpVelocityApply = jumpVelocityBoots * rbody.mass;
				jumpVel.y = jumpVelocityApply;
				switch (Inventory.a.JumpJetsVersion()) {
					case 0: energysuck = 11f; break;
					case 1: energysuck = 26f; break;
					case 2: energysuck = 22f; break;
				}

				if (PlayerEnergy.a.energy >= energysuck) {
					rbody.AddForce(jumpVel,ForceMode.Force);  // huhnh!
					if (jumpJetEnergySuckTickFinished < PauseScript.a.relativeTime) {
						jumpJetEnergySuckTickFinished = PauseScript.a.relativeTime + jumpJetEnergySuckTick;
						PlayerEnergy.a.TakeEnergy(energysuck);
						if (BiomonitorGraphSystem.a != null) {
							BiomonitorGraphSystem.a.EnergyPulse(energysuck);
						}
					}
				} else {
					hwbJumpJets.JumpJetsOff();
				}
			} else {
				if (ladderState) {
					// Jump off ladder in direction of player facing.
					jumpVel = transform.forward * jumpVelocityApply * rbody.mass;
				}

				rbody.AddForce(jumpVel,ForceMode.Force);  // huhnh!
			}
		}

		if (jumpTimeMod <= 0) justJumped = false; // for jump jets to work 
		jumpTime = jumpTimeMod;

		if (justJumped && !Inventory.a.JumpJetsActive()) {
			// Play jump sound
			if (jumpSFXFinished < PauseScript.a.relativeTime) {
				jumpSFXFinished = PauseScript.a.relativeTime + jumpSFXIntervalTime;
				SFX.pitch = 1f;
				float jumpSFXVolume = 1.0f;
				if (fatigue > 80) jumpSFXVolume = 0.5f; // Quietly, we tired.
				Utils.PlayOneShotSavable(SFX,SFXJump,jumpSFXVolume);
			}
			justJumped = false;
		}
	}

	void LadderStates() {
		if (CheatNoclip) return;
		if (!ladderState) return;

		float sidForce = 0f;
		float forForce = 0f;
		float upForce = 0f;
		if (grounded || Inventory.a.JumpJetsActive()) {
			// Ladder climb, allow while grounded
			float bonus = 1f;
			if (Inventory.a.JumpJetsActive()) bonus = 2f;

			sidForce = relSideways * walkAcceleration * Time.deltaTime;
			forForce = relForward * walkAcceleration * Time.deltaTime;
			upForce = ladderSpeed * relForward * walkAcceleration
							* Time.deltaTime * bonus;

			// Climbing when touching the ground
			rbody.AddRelativeForce(sidForce,upForce,forForce);
		} else {
			// Climbing off the ground
			if (ladderSFXFinished < PauseScript.a.relativeTime
				&& rbody.velocity.y > ladderSpeed * 0.5f) {

				SFX.pitch = (UnityEngine.Random.Range(0.8f,1.2f));
				Utils.PlayOneShotSavable(SFX,SFXLadder,0.2f);
				ladderSFXFinished = PauseScript.a.relativeTime
									+ ladderSFXIntervalTime;
			}

			float ladderSpeedMod = ladderSpeed;
			if (isSprinting && running) ladderSpeedMod = 1.2f; // Climb fast!

			sidForce = relSideways * walkAcceleration * Time.deltaTime * walkAccelAirRatio * 0.2f;
			forForce = relForward * walkAcceleration * Time.deltaTime * walkAccelAirRatio * 0.2f;
			upForce = ladderSpeedMod * relForward * walkAcceleration
							* Time.deltaTime;

			rbody.AddRelativeForce(sidForce,upForce,forForce);
		}

		if (Inventory.a.BoosterActive() && Inventory.a.BoosterSetToSkates()) {
			deceleration = walkDeaccelerationBooster;
		} else {
			deceleration = walkDeacceleration;
		}

		// Set vertical velocity towards 0 when climbing.
		RigidbodySetVelocityY(rbody,(Mathf.SmoothDamp(rbody.velocity.y,0,
													  ref walkDeaccelerationVoly,
													  deceleration)));
	}

	private float runTime;

	void WalkRun() {
		if (CheatNoclip) return;
		if (ladderState) return;

		float sidForce = relSideways * walkAcceleration * Time.deltaTime;
		float forForce = relForward * walkAcceleration * Time.deltaTime;
		float upForce = 0f;
// 		if (rbody.velocity.magnitude < playerSpeed) {
// 			upForce = (floorDot - 1.0f)/1.0f * playerSpeed;
// 			if (upForce != 0) Debug.Log("upForce is " + upForce.ToString());
// 		}

		if (isSprinting) {
			sidForce *= 1.75f;
			forForce *= 2.00f;
		}

		Vector3 movDir = rbody.velocity;
		movDir.y = 0;
		movDir = movDir.normalized;
		if (floorDot < 0.98f) {
			if (Vector3.Dot(movDir,floorAng) < 0f) {
				if (Inventory.a.BoosterActive()) forForce *= 2f;
			}
		}

		if (grounded || Inventory.a.JumpJetsActive()) {
			// Normal walking
			runTime += Time.deltaTime;
			if (relForward == 0 && relSideways == 0) runTime = 0;

			rbody.AddRelativeForce(sidForce,upForce,forForce);
			movDir = rbody.velocity; // Updated after force add.
			movDir.y = 0;
			if (floorDot > 0.9f) rbody.velocity = movDir;
			movDir = movDir.normalized;
			if (fatigueFinished2 < PauseScript.a.relativeTime
				&& movDir.sqrMagnitude > 0f && grounded
				&& (relForward != 0 || relSideways != 0)) {

				fatigueFinished2 = PauseScript.a.relativeTime
								   + fatigueWaneTickSecs;

				if (!Inventory.a.BoosterActive()) {
					if (isSprinting) fatigue += fatiguePerSprintTick;
					else fatigue += fatiguePerWalkTick;
				}
			}
		} else {

			// Sprinting in the air
			sidForce *= walkAccelAirRatio;
			forForce *= walkAccelAirRatio;
			upForce *= walkAccelAirRatio;

			// Walking in the air, we're floating in the moonlit sky, the
			// people far below are sleeping as we fly!
			rbody.AddRelativeForce(sidForce,upForce,forForce);
		}

		if (staminupActive || FatigueCheat) fatigue = 0;
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
		if (rbody.velocity.magnitude > maxCyberUltimateSpeed) {
			// Limit movement speed in all axes x,y,z in cyberspace
			RigidbodySetVelocity(rbody, maxCyberUltimateSpeed);
		}

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
			horizontalMovement *= playerSpeed; // Cap velocity to current max.
		}
		return horizontalMovement;
	}

	float GetClampedVerticalMovement() {
		if (grounded && !isSprinting) return 0f; // Prevent inadvertent view
												 // bob from floating.
		if (rbody.velocity.y >= maxVerticalSpeed) return maxVerticalSpeed;
		return rbody.velocity.y;
	}

	void FatigueApply() {
		if (fatigue > 100f) fatigue = 100f; // Clamp at 100% maximum
		if (fatigue < 0) fatigue = 0; // Clamp at 0% minimum.

		if (fatigue > 80f && !fatigueWarned && !inCyberSpace) {
			twm.SendWarning(("Fatigue high"),0.1f,0,HUDColor.White,324);
			fatigueWarned = true;
		} else {
			fatigueWarned = false;
		}

		if (inCyberSpace) return;
		if (CheatNoclip || FatigueCheat) { fatigue = 0; return; }
		if (fatigueFinished >= PauseScript.a.relativeTime) return;

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
		if (fatigue < 0) fatigue = 0; // Clamp at 0% minimum.
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
		float ofsY = (1.6f-(Const.a.playerCameraOffsetY * currentCrouchRatio));
		Vector3 ofs = new Vector3(0f,ofsY,0f);
		return Physics.CheckCapsule(cameraObject.transform.position,
									cameraObject.transform.position + ofs,
									capsuleRadius,layerMask);
	}

	bool CantCrouch() {
		return Physics.CheckCapsule(cameraObject.transform.position,
									cameraObject.transform.position
									+ new Vector3(0f,0.2f,0f),
									capsuleRadius,layerMask);
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

		bool conditions = (grounded || CheatNoclip || ladderState
						   || gravliftState);

		if (GetInput.a.Sprint()) {
			if (conditions) return !(GetInput.a.CapsLockOn());
			return false;
		} else {
			if (conditions) return GetInput.a.CapsLockOn();
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
	void OnCollisionStay(Collision collision) {
		if (!PauseScript.a.Paused() && !inCyberSpace) {
			float maxSlope = 0.35f;
			for(tempInt=0;tempInt<collision.contacts.Length;tempInt++) {
				floorAng = collision.contacts[tempInt].normal;
				floorDot = Vector3.Dot(collision.contacts[tempInt].normal,
									   Vector3.up);
				maxSlope = 0.35f;
				if (Inventory.a.BoosterActive()) maxSlope = 0.7f;
				if (floorDot <= 1f && floorDot >= maxSlope) {
					if (!grounded) stepFinished = PauseScript.a.relativeTime;
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
		consoleEntryButton.SetActive(false);
		consoleTitle.SetActive(false);
		consoleinpFd.DeactivateInputField();
		consoleinpFd.enabled = false;
		consolebg.enabled = false;
		consoleentryText.text = "";
		consoleentryText.enabled = false;
		ConsoleEmulator.consoleMemdex = consoleMemdex = 0;
	}

	void ConsoleEnable() {
		consoleActivated = true;
		consoleplaceholderText.SetActive(true);
		if (Application.platform == RuntimePlatform.Android) {
			consoleEntryButton.SetActive(true);
		}
		consoleTitle.SetActive(true);
		consoleinpFd.enabled = true;
		consoleinpFd.ActivateInputField();
		consolebg.enabled = true;
		consoleentryText.enabled = true;
		ConsoleEmulator.consoleMemdex = consoleMemdex = 0;
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
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.SaveTransform(go.transform));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRigidbody(go));
		s1.Append(Utils.splitChar);
		if (pm == null) {
			Debug.Log("PlayerMovement missing on savetype of Player!"
					  + "GameObject.name: " + go.name + ", "
					  + SaveObject.currentObjectInfo + " ["
					  + SaveObject.currentSaveEntriesIndex + "]");

			s1.Append("fbfibbbbbfffffbffbbifftttbtttfu");
			return Utils.DTypeWordToSaveString(s1.ToString());
		}

		s1.Append(Utils.FloatToString(pm.playerSpeed,"playerSpeed"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.grounded,"grounded"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.currentCrouchRatio,
									  "currentCrouchRatio")); 
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(Utils.BodyStateToInt(pm.bodyState),
									 "bodyState"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.ladderState,"ladderState"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.gravliftState,"gravliftState"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.inCyberSpace,"inCyberSpace"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.CheatWallSticky,"CheatWallSticky"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.CheatNoclip,"CheatNoclip"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.jumpTime,"jumpTime")); // not a timer
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.oldVelocity.x,"oldVelocity.x"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.oldVelocity.y,"oldVelocity.y"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.oldVelocity.z,"oldVelocity.z"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.fatigue,"fatigue"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.justJumped,"justJumped"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.fatigueFinished,
													 "fatigueFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.fatigueFinished2,
													 "fatigueFinished2"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.cyberSetup,"cyberSetup"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.cyberDesetup,"cyberDesetup"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(Utils.BodyStateToInt(pm.oldBodyState),
									 "oldBodyState"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.leanTarget,"leanTarget"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.leanShift,"leanShift"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.jumpSFXFinished,
													 "jumpSFXFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.jumpLandSoundFinished,
													 "jumpLandSoundFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(
											pm.jumpJetEnergySuckTickFinished,
											"jumpJetEnergySuckTickFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pm.fatigueWarned,"fatigueWarned"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.turboFinished,
													 "turboFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.ressurectingFinished,
													 "ressurectingFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pm.doubleJumpFinished,
													 "doubleJumpFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pm.SFX.time,"SFX.time"));
		s1.Append(Utils.splitChar);
		if (!pm.SFX.isPlaying) pm.SFXIndex = -1; // Safely can set to null, not
												 // playing a sound.

		s1.Append(Utils.UintToString(pm.SFXIndex,"SFXIndex"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		PlayerMovement pm = go.GetComponent<PlayerMovement>();
		if (pm == null) {
			Debug.Log("PlayerMovement.Load failure, pm == null, "
					  + SaveObject.currentObjectInfo);

			return index + 10 + 4 + 31;
		}

		if (index < 0) {
			Debug.Log("PlayerMovement.Load failure, index < 0, "
					  + SaveObject.currentObjectInfo);

			return index + 10 + 4 + 31;
		}

		if (entries == null) {
			Debug.Log("PlayerMovement.Load failure, entries == null, "
					  + SaveObject.currentObjectInfo);

			return index + 10 + 4 + 31;
		}

		float readFloatx, readFloaty, readFloatz;
		string oldpos = go.transform.localPosition.ToString();
		index = Utils.LoadTransform(go.transform,ref entries,index);
		index = Utils.LoadRigidbody(go,ref entries,index);

		pm.playerSpeed = Utils.GetFloatFromString(entries[index],
												  "playerSpeed");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.grounded = Utils.GetBoolFromString(entries[index],"grounded");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.currentCrouchRatio = Utils.GetFloatFromString(entries[index],
														 "currentCrouchRatio");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.bodyState =
			Utils.IntToBodyState(Utils.GetIntFromString(entries[index],
														"bodyState"));
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.ladderState = Utils.GetBoolFromString(entries[index],"ladderState");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.gravliftState = Utils.GetBoolFromString(entries[index],
												   "gravliftState");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.inCyberSpace = Utils.GetBoolFromString(entries[index],
												  "inCyberSpace");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.CheatWallSticky = Utils.GetBoolFromString(entries[index],
													 "CheatWallSticky");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.CheatNoclip = Utils.GetBoolFromString(entries[index],"CheatNoclip");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		// Not a timer.
		pm.jumpTime = Utils.GetFloatFromString(entries[index],"jumpTime");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		readFloatx = Utils.GetFloatFromString(entries[index],"oldVelocity.x");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		readFloaty = Utils.GetFloatFromString(entries[index],"oldVelocity.y");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		readFloatz = Utils.GetFloatFromString(entries[index],"oldVelocity.z");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		pm.oldVelocity = new Vector3(readFloatx,readFloaty,readFloatz);

		pm.fatigue = Utils.GetFloatFromString(entries[index],"fatigue");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.justJumped = Utils.GetBoolFromString(entries[index],"justJumped");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.fatigueFinished =
			Utils.LoadRelativeTimeDifferential(entries[index],
											   "fatigueFinished");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.fatigueFinished2 =
			Utils.LoadRelativeTimeDifferential(entries[index],
											   "fatigueFinished2");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.cyberSetup = Utils.GetBoolFromString(entries[index],"cyberSetup");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.cyberDesetup = Utils.GetBoolFromString(entries[index],
												  "cyberDesetup");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.oldBodyState =
			Utils.IntToBodyState(Utils.GetIntFromString(entries[index],
														"oldBodyState"));
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.leanTarget = Utils.GetFloatFromString(entries[index],"leanTarget");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		pm.leanShift = Utils.GetFloatFromString(entries[index],"leanShift");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		pm.leanTransform.localRotation = Quaternion.Euler(0, 0, pm.leanTarget);
		pm.leanTransform.localPosition = new Vector3(pm.leanShift,0,0);

		pm.jumpSFXFinished =
			Utils.LoadRelativeTimeDifferential(entries[index],
											   "jumpSFXFinished");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.jumpLandSoundFinished =
			Utils.LoadRelativeTimeDifferential(entries[index],
											   "jumpLandSoundFinished");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.jumpJetEnergySuckTickFinished =
			Utils.LoadRelativeTimeDifferential(entries[index],
											  "jumpJetEnergySuckTickFinished");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.fatigueWarned = Utils.GetBoolFromString(entries[index],
												   "fatigueWarned");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.turboFinished = Utils.LoadRelativeTimeDifferential(entries[index],
															  "turboFinished");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.ressurectingFinished =
			Utils.LoadRelativeTimeDifferential(entries[index],
											   "ressurectingFinished");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		pm.doubleJumpFinished =
			Utils.LoadRelativeTimeDifferential(entries[index],
											   "doubleJumpFinished");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();

		float sfxTime = Utils.GetFloatFromString(entries[index],"SFX.time");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		pm.SFXIndex = Utils.GetIntFromString(entries[index],"SFXIndex");
		index++; SaveObject.currentSaveEntriesIndex = index.ToString();
		pm.ladderSFXFinished = 0;
		if (pm.SFXIndex >= 0) {
			pm.SFX.time = sfxTime;
			pm.SFX.clip = Const.a.sounds[pm.SFXIndex];
			Utils.PlayOneShotSavable(pm.SFX,Const.a.sounds[pm.SFXIndex]);
		}

		pm.ConsoleDisable();
		return index;
	}
}
