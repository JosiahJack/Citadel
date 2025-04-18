﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

public class MouseLookScript : MonoBehaviour {
    // External references
	public GameObject player;
	public GameObject canvasContainer;
	public GameObject compassContainer;
	public GameObject automapContainerLH;
	public GameObject automapContainerRH;
	public GameObject compassMidpoints;
	public GameObject compassLargeTicks;
	public GameObject compassSmallTicks;
    [Tooltip("Game object that houses the MFD tabs")] public GameObject tabControl;
	[Tooltip("Text in the data tab in the MFD that displays when searching an object containing no items")] public Text dataTabNoItemsText;
	public LogContentsButtonsManager logContentsManager;
	public GameObject[] hardwareButtons;
	public PuzzleWire puzzleWire;
	public PuzzleGrid puzzleGrid;
	public GameObject shootModeButton;
	public HealthManager hm;
	public GameObject playerRadiationTreatmentFlash;
	public Vector2 lastMousePos;
	
    // Internal references
    [HideInInspector] public bool inventoryMode;
	public bool holdingObject;
    [HideInInspector] public Vector2 cursorHotspot;
    [HideInInspector] public Vector3 cameraFocusPoint;
	[HideInInspector] public GameObject currentButton;
	[HideInInspector] public GameObject currentSearchItem;
    public int heldObjectIndex; // save
	public int heldObjectCustomIndex; // save
	public int heldObjectAmmo; // save
	public int heldObjectAmmo2; // save
	public bool heldObjectLoadedAlternate; // save
	[HideInInspector] public bool firstTimePickup;
	[HideInInspector] public bool firstTimeSearch;
	public bool grenadeActive;
	public bool inCyberSpace;
    public float yRotation;
	[HideInInspector] public Vector3 cyberspaceReturnPoint; // save
	[HideInInspector] public Vector3 cyberspaceReturnCameraLocalRotation; // save
	[HideInInspector] public Vector3 cyberspaceReturnPlayerCapsuleLocalRotation; // save
	[HideInInspector] public int cyberspaceReturnLevel; // save
	[HideInInspector] public Vector3 cyberspaceRecallPoint; // save
	[HideInInspector] public bool vmailActive = false;
	[HideInInspector] public bool geniusActive = false;
	private float keyboardTurnSpeed = 15f; // Speed multiplier for turning the view with the keyboard.
    private float tossOffset = 0.5f; // Distance from player origin to spawn objects when tossing them.
    private float tossForce = 10f; // Force given to spawned objects when tossing them.
	private float[] cameraDistances;
    [HideInInspector] public float xRotation; // save
    private float zRotation;
    private float yRotationV;
    private float xRotationV;
    private float zRotationV;
    private float currentZRotation;
    private string mlookstring1;
    [HideInInspector] public Camera playerCamera;
    private GameObject heldObject;
	private Quaternion tempQuat;
	private Vector3 tempVec;
    private RaycastHit tempHit;
	private Vector3 cameraRecoilLerpPos;
	private float cyberSpinSensitivity = 0.6f;
	private float shakeFinished;
	private float shakeForce;
	private string f9 = "f9";
	private string f6 = "f6";
	private string qsavename = "quicksave";
	private string mouseX = "Mouse X";
	private string mouseY = "Mouse Y";
	private Vector3 cursorPoint;
	private float headBobTimeShift;
	private float headBobX;
	private float headBobY;
	private float headBobZ;
	private float rotSpeedX = 0f;
	private float rotSpeedY = 0f;
	private Transform playerCapsuleTransform;
	[HideInInspector] public float returnFromCyberspaceFinished;
	private float dropFinished;
	[HideInInspector] public float randomShakeFinished;
	[HideInInspector] public float randomKlaxonFinished;
	public Vector2 debugRT;
	public Vector2 debugAng;
	public float joyXStartTime;
	public float joyYStartTime;
	public float joyXSignLast;
	public float joyYSignLast;
	public float headBobShiftFinished;
	private float bobTarget;
	private float headBobXVel;
	private float headBobYVel;
	private static StringBuilder s1 = new StringBuilder();
    
	public static MouseLookScript a;

	void Awake() {
		a = this;
		a.playerCamera = GetComponent<Camera>(); // Needed elsewhere, do early.
	}

    void Start (){
		ResetHeldItem();
		Cursor.lockState = CursorLockMode.None;
		inventoryMode = false; // Start with inventory mode turned off.
		if (Application.platform == RuntimePlatform.Android) {
			ForceInventoryMode();
			shootModeButton.SetActive(true);
		} else {
			shootModeButton.SetActive(false);
		}

		cameraDistances = new float[32];
		SetCameraCullDistances();
		playerCamera.depthTextureMode = DepthTextureMode.Depth;
		grenadeActive = false;
		yRotation = 0;
		xRotation = 0;
		canvasContainer.SetActive(true); // Enable UI.
		firstTimePickup = true;
		firstTimeSearch = true;
		inCyberSpace = false;
		shakeFinished = PauseScript.a.relativeTime;
		returnFromCyberspaceFinished = 0;
		dropFinished = 0;

		// PlayerCapsule
		// -> LeanTransform
        // -> -> MainCamera: MouseLookScript component.
		playerCapsuleTransform = transform.parent.transform.parent.transform;

		randomShakeFinished = PauseScript.a.relativeTime;
		randomKlaxonFinished = PauseScript.a.relativeTime;
		headBobShiftFinished = PauseScript.a.relativeTime;
		bobTarget = 0.3f;
    }
    
    void OnPreCull() {
		DynamicCulling.a.Cull(false); // Update dynamic culling system.
	}

	void Update() {
		// Allow quick load straight from the menu or pause.
		if (Input.GetKeyUp(f9)) {
			if (inCyberSpace) {
				Const.sprint(Const.a.stringTable[1023]); // "Cannot load in cyberspace"
				return;
			}

			MainMenuHandler.a.LoadGame(7);
		}

        if (PauseScript.a.MenuActive()) {
			// Ignore mouselook and turn off camera when main menu is up.
			if (!MainMenuHandler.a.fileBrowserOpen) Cursor.visible = false;
			else Cursor.visible = true;

			if (playerCamera.enabled) playerCamera.enabled = false;
			return;
		}

		if (PauseScript.a.Paused()) return;
		if (PlayerMovement.a.ressurectingFinished > PauseScript.a.relativeTime) return;

		Utils.EnableCamera(playerCamera);

		// Unpaused, normal functions::
		// ====================================================================
		if (Input.GetKeyUp(f6)) {
			if (inCyberSpace) {
				Const.sprint(Const.a.stringTable[602]); // Cannot save in cyberspace
				return;
			}

			Const.a.StartSave(7,qsavename);
		}

		// Toggle inventory mode<->shoot mode
		if(GetInput.a.ToggleMode()) ToggleInventoryMode();

		if (Const.a.questData.SelfDestructActivated
			&& LevelManager.a.currentLevel != 13   // Not Cyberspace
			&& LevelManager.a.currentLevel != 9) { // Not the bridge, separated

			if (randomShakeFinished < PauseScript.a.relativeTime) {
				randomShakeFinished = PauseScript.a.relativeTime
				                      + UnityEngine.Random.Range(5f,20f);
				ScreenShake(3f,2f);
			}
			
			if (randomKlaxonFinished < PauseScript.a.relativeTime) {
				randomKlaxonFinished = PauseScript.a.relativeTime
				                       + UnityEngine.Random.Range(10f,20f);

				Utils.PlayUIOneShotSavable(104); // klaxon
			}
		}

		RecoilAndRest(); // Spring Back to Rest from Recoil
		keyboardTurnSpeed = 15f * Const.a.MouseSensitivity;
		if (Application.platform == RuntimePlatform.Android) {
			if (Const.a.MouseSensitivity == 100f) Const.a.MouseSensitivity = 20f;
		} 
		KeyboardTurn();
		KeyboardLookUpDn();
		TouchLook();
		if (inCyberSpace) { // Barrel roll!
			if (GetInput.a.LeanLeft()) {
				playerCapsuleTransform.RotateAround(
					playerCapsuleTransform.transform.position,
					playerCapsuleTransform.transform.forward,
					cyberSpinSensitivity * Time.deltaTime * 100f
				);
			}

			if (GetInput.a.LeanRight()) {
				playerCapsuleTransform.RotateAround(
					playerCapsuleTransform.transform.position,
					playerCapsuleTransform.transform.forward,
					cyberSpinSensitivity * Time.deltaTime * -1f * 100f
				);
			}
		} else {
			if (compassContainer.activeInHierarchy) {
				// Update automap player icon orientation.
				compassContainer.transform.rotation =
					Quaternion.Euler(0f, -yRotation + 180f, 0f);
			}
		}

		if (!inventoryMode) Mouselook(); // Only do mouselook in Shoot Mode.
		if(GetInput.a.Use()) Frob(); // Frob what is under our cursor.
	}

	public void Frob() {
		if (vmailActive && !inCyberSpace) {
			Inventory.a.DeactivateVMail(); vmailActive = false;
			return;
		}

		if (!GUIState.a.isBlocking && !inCyberSpace) {
			if (dropFinished < Time.time) {
				currentButton = null; // Force this to reset.
				if (holdingObject) {
					if (!FrobWithHeldObject()) DropHeldItem();
				} else FrobEmptyHanded();
			}
		} else {
			//We are holding cursor over the GUI
			if (holdingObject && !inCyberSpace) {
				AddItemToInventory(heldObjectIndex,heldObjectCustomIndex);
				MouseCursor.a.liveGrenade = false;
				ResetHeldItem();
			} else InventoryButtonUse();
		}
	}

	public void Mouselook() {
		if (returnFromCyberspaceFinished >= Time.time) return; // Not yet.

		returnFromCyberspaceFinished = 0;

		// PROCESS INPUT SIGNALS
		// ----------------------------------------------------------------
		float angX = 0f; // Angle change for X.
		float angY = 0f; // Angle change for Y.
		// Handle mouse input from a standard mouse.
		float deltaX = Input.GetAxisRaw(mouseX) * Const.a.MouseSensitivity * Const.a.GraphicsFOV;
		float deltaY = Input.GetAxisRaw(mouseY) * Const.a.MouseSensitivity * Const.a.GraphicsFOV;

		// Handle thumbstick input from a controller.
		Vector2 rightThumbstick = new Vector2(Input.GetAxisRaw("JoyAxis4"), // Horizontal Left < 0, Right > 0
											  Input.GetAxisRaw("JoyAxis5") * -1f); // Vertical Down > 0, Up < 0 Inverted
		// X
		float signX = rightThumbstick.x > 0.0f ? 1.0f
					  : rightThumbstick.x < 0.0f ? -1.0f : 0f;
		if (signX != joyXSignLast) {
			joyXStartTime = Time.time; // Zero crossing.
			rotSpeedX = 0f; // Reset integrator windup.
		}

		joyXSignLast = signX;
		rightThumbstick.x *= Const.a.MouseSensitivity * 20f;
		rotSpeedX += rightThumbstick.x; // Integrate to give fine initial.
		if (rightThumbstick.x == 0f) rotSpeedX = 0f;
		if (rotSpeedX != 0f) deltaX = rotSpeedX;

		// Y
		float signY = rightThumbstick.y > 0.0f ? 1.0f
					  : rightThumbstick.y < 0.0f ? -1.0f : 0f;
		if (signY != joyYSignLast) {
			joyYStartTime = Time.time; // Zero crossing.
			rotSpeedY = 0f; // Reset integrator windup.
		}

		joyYSignLast = signY;
		rightThumbstick.y *= Const.a.MouseSensitivity * 20f;
		rotSpeedY += rightThumbstick.y; // Integrate to give fine initial.
		if (rightThumbstick.y == 0) rotSpeedY = 0f;
		if (rotSpeedY != 0f) deltaY = rotSpeedY;

		// Apply input delta from mouse or controller to angles.
		if (signX != 0f || signY != 0f) {
			// Using controller, use integrated deltas.
			angX = deltaX;
			angY = deltaY;
		} else {
			// Using mouse, map input to deg per screen half / screen.
			angX = deltaX * ((Const.a.GraphicsFOV / 2f) / Screen.width / 2f);
			angY = deltaY * ((Const.a.GraphicsFOV / 2f) / Screen.height / 2f);
		}

		// For my inspector viewing pleasure.
		debugRT = new Vector2(deltaX,deltaY);
		debugAng = new Vector2(angX,angY);

		// High pass filter to prevent jumpy behavior.
		if (angX > Const.a.GraphicsFOV) angX = Const.a.GraphicsFOV;
		if (angY > Const.a.GraphicsFOV) angY = Const.a.GraphicsFOV;

		// APPLY MOUSE LOOK
		// --------------------------------------------------------------------
		if (inCyberSpace) {
			// CYBER MOUSE LOOK
			if (Const.a.InputInvertCyberspaceLook) xRotation = -angY;
			else xRotation = angY;

			xRotation = Clamp0360(xRotation); // Limit up/down to within 360°.
			yRotation = angX;
			playerCapsuleTransform.RotateAround(
				playerCapsuleTransform.transform.position,
				playerCapsuleTransform.transform.up,yRotation
			);

			playerCapsuleTransform.RotateAround(
				playerCapsuleTransform.transform.position,
				playerCapsuleTransform.transform.right,-xRotation
			);
		} else {
			// NORMAL MOUSE LOOK
			if (Const.a.InputInvertLook) xRotation += angY;
			else xRotation -= angY;

			xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit up/down.
			yRotation += angX;

			// Apply the mouselook. Left/Right component applied to capsule.
			playerCapsuleTransform.localRotation = Quaternion.Euler(0f,
																	yRotation,
																	0f);

			// Up down component only applied to camera.  Must be 0 for others
			// or else movement will go in wrong direction!
			transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
			float xCenter = (float)Screen.width * 0.5f;
			float yCenter = (float)Screen.height * 0.5f;
			float xOffset = ((float)Input.mousePosition.x - xCenter);
			float yOffset = ((float)Input.mousePosition.y - yCenter);
			if (xOffset > 2f || yOffset > 2f) {
				MouseCursor.SetCursorPosInternal((int)xCenter,(int)yCenter);
			}
		}
	}

	public void EnterCyberspace(Vector3 entryPoint) {
		cyberspaceRecallPoint = entryPoint;
		playerRadiationTreatmentFlash.SetActive(true);
		cyberspaceReturnPoint = PlayerMovement.a.transform.position;
		cyberspaceReturnCameraLocalRotation = transform.localRotation.eulerAngles;
		cyberspaceReturnPlayerCapsuleLocalRotation = playerCapsuleTransform.localRotation.eulerAngles;
		cyberspaceReturnLevel = LevelManager.a.currentLevel;
		MFDManager.a.EnterCyberspace();
		LevelManager.a.LoadLevel(13,cyberspaceRecallPoint);
		PlayerMovement.a.inCyberSpace = true;
		PlayerMovement.a.leanCapsuleCollider.enabled = false;
		hm.inCyberSpace = true;
		inCyberSpace = true;
		playerCamera.useOcclusionCulling = false;
		MFDManager.a.DrawTicks(true);
		SetCameraCullDistances();
		Utils.PlayUIOneShotSavable(81); // cyber
	}

	public void ExitCyberspace() {
		playerRadiationTreatmentFlash.SetActive(true);
		MFDManager.a.ExitCyberspace();
		LevelManager.a.LoadLevel(cyberspaceReturnLevel,cyberspaceReturnPoint);

		// Left/right component applied to capsule.
		playerCapsuleTransform.localRotation = Quaternion.Euler(0f,
			cyberspaceReturnPlayerCapsuleLocalRotation.y,0f);

		transform.localRotation = // Up down component applied to camera
			Quaternion.Euler(cyberspaceReturnCameraLocalRotation.x,
							 cyberspaceReturnCameraLocalRotation.y,
							 cyberspaceReturnCameraLocalRotation.z);

		xRotation = cyberspaceReturnCameraLocalRotation.x;
		yRotation = cyberspaceReturnPlayerCapsuleLocalRotation.y;

		returnFromCyberspaceFinished = Time.time + 0.1f; // Prevent mouselook
														 // messing it up.
		PlayerMovement.a.inCyberSpace = false;
		PlayerMovement.a.rbody.velocity = Const.a.vectorZero;
		PlayerMovement.a.leanCapsuleCollider.enabled = true;
		hm.inCyberSpace = false;
		inCyberSpace = false;
		playerCamera.useOcclusionCulling = true;
		Const.a.decoyActive = false;
		MFDManager.a.DrawTicks(true);
		Utils.PlayUIOneShotSavable(81); // cyber
		SetCameraCullDistances();
	}

	// Draw line from cursor - used for projectile firing, e.g. magpulse/stugngun/railgun/plasma
	public void SetCameraFocusPoint() {
		cursorPoint = MouseCursor.a.GetCursorScreenPointForRay();
        if (Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint), out tempHit, Mathf.Infinity)) cameraFocusPoint = tempHit.point;
	}

	// Clamp cyberspace up/down look rotation to with in +/- 360f.
	float Clamp0360(float val) {
		return (val - (Mathf.CeilToInt(val*(1f/360f)) * 360f)); // Subtract out 360 times the number of times 360 fits within val.
	}

	public void SetCameraCullDistances() {
		if (cameraDistances == null) cameraDistances = new float[32];
		else if (cameraDistances.Length < 32) cameraDistances = new float[32];
		
		if (inCyberSpace) {
			for (int i=0;i<32;i++) { cameraDistances[i] = 3350f; } // Increased from 2400 to fit the Saturn's rings without overlapping with star sphere.
		} else {
			for (int i=0;i<32;i++) { cameraDistances[i] = 79f; } // Can't see further than this.  31 * 2.56 - player radius 0.48 = 78.88f rounded up to be careful..longest line of sight is the crawlway on level 6
			cameraDistances[0]  = 45.1f; // Default, most static objects and some dynamic.
			cameraDistances[1]  = 16f;   // TransparentFX, for mist and drips
			cameraDistances[4]  = 30f;   // Water, used for effects like steam
										 // and well, spraying water.
			cameraDistances[14] = 30f;   // PhysObjects, patches, carts, barrels
			cameraDistances[15] = 3350f; // Sky is visible, only exception.
		}
		playerCamera.layerCullDistances = cameraDistances; // Cull anything beyond 79f except for sky layer.
	}
	
	void TouchLook() {
	    Vector2 rightTouchstick = GetInput.a.rightTS.Coordinate();
	    if (rightTouchstick.x < 0f) {
			yRotation -= keyboardTurnSpeed * rightTouchstick.x;
			playerCapsuleTransform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
		} else if (rightTouchstick.x > 0f) {
			yRotation += keyboardTurnSpeed * rightTouchstick.x;
			playerCapsuleTransform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
		}
		
		if (rightTouchstick.y < 0f) {
			if ((inCyberSpace && Const.a.InputInvertCyberspaceLook) || (!inCyberSpace && Const.a.InputInvertLook))
				xRotation -= keyboardTurnSpeed;
			else
				xRotation += keyboardTurnSpeed;

			if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle.
			transform.localRotation = Quaternion.Euler(xRotation,0f,
													   transform.localRotation.z);
		} else if (rightTouchstick.y > 0f) {
			if ((inCyberSpace && Const.a.InputInvertCyberspaceLook) || (!inCyberSpace && Const.a.InputInvertLook))
				xRotation += keyboardTurnSpeed * rightTouchstick.y;
			else
				xRotation -= keyboardTurnSpeed * rightTouchstick.y;

			if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle.
			transform.localRotation = Quaternion.Euler(xRotation, 0f,
													   transform.localRotation.z);
		}
	}

	void KeyboardTurn() {
		if (inCyberSpace) {
			float angX = 0f;
			if (GetInput.a.TurnLeft()) {
				// Modulate input to deg per screen half / screen.
				angX = -keyboardTurnSpeed * 18f * ((Const.a.GraphicsFOV / 2f) / Screen.width / 2f);
				yRotation = angX;
				playerCapsuleTransform.RotateAround(
					playerCapsuleTransform.transform.position,
					playerCapsuleTransform.transform.up,yRotation
				);
			} else if (GetInput.a.TurnRight()) {
				angX = keyboardTurnSpeed * 18f * ((Const.a.GraphicsFOV / 2f) / Screen.width / 2f);
				yRotation = angX;
				playerCapsuleTransform.RotateAround(
					playerCapsuleTransform.transform.position,
					playerCapsuleTransform.transform.up,yRotation
				);
			}
		} else {
			if (GetInput.a.TurnLeft()) {
				yRotation -= keyboardTurnSpeed;
				playerCapsuleTransform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
			} else if (GetInput.a.TurnRight()) {
				yRotation += keyboardTurnSpeed;
				playerCapsuleTransform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
			}
		}
		
		Vector2 rightTouchstick = GetInput.a.rightTS.Coordinate();
	}

	void KeyboardLookUpDn() {
		if (inCyberSpace) {
			float angY = 0f;
			if (GetInput.a.LookDown()) {
				// Modulate input to deg per screen half / screen.
				angY = -keyboardTurnSpeed * 18f * ((Const.a.GraphicsFOV / 2f) / Screen.height / 2f);
				if (Const.a.InputInvertCyberspaceLook) xRotation = -angY;
				else xRotation = angY;
			
				xRotation = Clamp0360(xRotation); // Limit up/down to within 360°.
				playerCapsuleTransform.RotateAround(
					playerCapsuleTransform.transform.position,
					playerCapsuleTransform.transform.right,-xRotation
				);
			} else if (GetInput.a.LookUp()) {
				angY = keyboardTurnSpeed * 18f * ((Const.a.GraphicsFOV / 2f) / Screen.height / 2f);
				if (Const.a.InputInvertCyberspaceLook) xRotation = -angY;
				else xRotation = angY;
			
				xRotation = Clamp0360(xRotation); // Limit up/down to within 360°.
					playerCapsuleTransform.RotateAround(
					playerCapsuleTransform.transform.position,
					playerCapsuleTransform.transform.right,-xRotation
				);
			}
		} else {
			// Cyberspace...more like a plane so giving the option to invert it separately.
			if (GetInput.a.LookDown()) {
				if ((inCyberSpace && Const.a.InputInvertCyberspaceLook) || (!inCyberSpace && Const.a.InputInvertLook))
					xRotation -= keyboardTurnSpeed;
				else
					xRotation += keyboardTurnSpeed;

				if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle.
				transform.localRotation = Quaternion.Euler(xRotation,0f,
														transform.localRotation.z);
			} else if (GetInput.a.LookUp()) {
				if ((inCyberSpace && Const.a.InputInvertCyberspaceLook) || (!inCyberSpace && Const.a.InputInvertLook))
					xRotation += keyboardTurnSpeed;
				else
					xRotation -= keyboardTurnSpeed;

				if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle.
				transform.localRotation = Quaternion.Euler(xRotation, 0f,
														transform.localRotation.z);
			}
		}
	}

	bool RayOffset() {
		bool successfulRay = false;
		successfulRay = Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint), out tempHit,Const.frobDistance,Const.a.layerMaskPlayerFrob);
// 		Debug.DrawRay(playerCamera.ScreenPointToRay(cursorPoint).origin,playerCamera.ScreenPointToRay(cursorPoint).direction * Const.frobDistance, Color.green,1f,true);
		if (successfulRay) {
			successfulRay = (tempHit.collider != null);
			if (successfulRay) {
				successfulRay = (tempHit.collider.CompareTag("Usable") || tempHit.collider.CompareTag("Searchable") || tempHit.collider.CompareTag("NPC"));
			}
		}
		return successfulRay;
	}

	bool TargetIDFrob(Vector3 cP) {
		if (Application.platform == RuntimePlatform.Android) {
			if (MouseLookScript.a.inCyberSpace) {
				WeaponFire.a.FireCyberWeapon();
				return true;
			}
		}

		if (inCyberSpace) return false;

		float dist = TargetID.GetTargetIDSensingRange(true);
		bool successfulRay = Physics.Raycast(playerCamera.ScreenPointToRay(cP),
											 out tempHit,dist,
											 Const.a.layerMaskPlayerTargetIDFrob);

		// Success here means hit a useable something.
		// If a ray hits a wall or other unusable something, that's not success
		if (successfulRay) {
			successfulRay = (tempHit.collider != null);
			if (successfulRay) {
				successfulRay = tempHit.collider.CompareTag("NPC");
			}
		}

		if (!successfulRay) return false;

		// Say we can't use enemy and give enemy name.
		AIController aic = tempHit.collider.gameObject.GetComponent<AIController>();
		if (aic == null) return false;

		HealthManager hm = Utils.GetMainHealthManager(tempHit);
		if (hm != null) {
			if (hm.health <= 0 && aic.searchColliderGO != null) {
				currentSearchItem = aic.searchColliderGO;
				SearchObject(currentSearchItem.GetComponent<SearchableItem>().lookUpIndex);
				return true; // True = do and check nothing further this frob.
			}
		}

		if (Inventory.a.hasHardware[4] && Inventory.a.hardwareVersion[4] > 1) {
			if (!aic.hasTargetIDAttached) {
				WeaponFire.a.CreateTargetIDInstance(-1f,aic.healthManager,-1f);
				if (Application.platform != RuntimePlatform.Android) {
					return true;
				}
			}
		}

		if (Application.platform == RuntimePlatform.Android) {
			// Cyber handled just above, normal fire condition only here.
			int constDex = WeaponCurrent.a.weaponIndex;
			int wepdex = WeaponFire.Get16WeaponIndexFromConstIndex(constDex);
			WeaponFire.a.StartNormalAttack(wepdex);
			return true;
		}

		// "Can't use <enemy>"
		Const.sprint(Const.a.stringTable[29] + Const.a.nameForNPC[aic.index],
					 player);

		return true;
	}

	void FrobEmptyHanded() {
		if (holdingObject) return;

		RaycastHit firstHit;
		float offset = Screen.height * 0.02f;
		cursorPoint = MouseCursor.a.GetCursorScreenPointForRay();
		if (TargetIDFrob(cursorPoint)) return;

		Ray castDir = playerCamera.ScreenPointToRay(cursorPoint);
		bool successfulRay = Physics.Raycast(castDir, out tempHit,
											 Const.frobDistance,
											 Const.a.layerMaskPlayerFrob);

// 		Debug.DrawRay(playerCamera.ScreenPointToRay(cursorPoint).origin,
// 					  playerCamera.ScreenPointToRay(cursorPoint).direction
// 					    * Const.frobDistance, Color.green,1f,true);

		firstHit = tempHit;
		// Success here means hit a useable something.
		// If a ray hits a wall or other unusable something,
		// that's not success and print "Can't use <something>".
		if (successfulRay) {
			successfulRay = (tempHit.collider != null);
			if (successfulRay) {
				successfulRay = (tempHit.collider.CompareTag("Usable")
								 || tempHit.collider.CompareTag("Searchable"));
			}
		}

		// Shoot rays in a pattern like this
		// * * *
		// * + *
		// * * *

		// In an order like this:
		// 8 3 6
		// 5 1 4
		// 7 2 9
		// To kind of walk around the center point to hopefully minimize rays
		// we try and tighten our lug nuts properly so the wheels don't fall 
		// off this thing.

		if (!successfulRay) { // Try down
			cursorPoint.y -= offset;
			successfulRay = RayOffset();
			cursorPoint.y += offset;
		}
		if (!successfulRay) { // Try up
			cursorPoint.y += offset;
			successfulRay = RayOffset();
			cursorPoint.y -= offset;
		}
		if (!successfulRay) { // Try to the right
			cursorPoint.x += offset;
			successfulRay = RayOffset();
			cursorPoint.x -= offset;
		}
		if (!successfulRay) { // Try to the left
			cursorPoint.x -= offset;
			successfulRay = RayOffset();
			cursorPoint.x += offset;
		}
		if (!successfulRay) { // Try up and to the right
			cursorPoint.x += offset;
			cursorPoint.y += offset;
			successfulRay = RayOffset();
			cursorPoint.x -= offset;
			cursorPoint.y -= offset;
		}
		if (!successfulRay) { // Try down and to the left
			cursorPoint.x -= offset;
			cursorPoint.y -= offset;
			successfulRay = RayOffset();
			cursorPoint.x += offset;
			cursorPoint.y += offset;
		}
		if (!successfulRay) { // Try up and to the left, cupid shuffle
			cursorPoint.x -= offset;
			cursorPoint.y += offset;
			successfulRay = RayOffset();
			cursorPoint.x += offset;
			cursorPoint.y -= offset;
		}
		if (!successfulRay) { // Try down and to the right
			cursorPoint.x += offset;
			cursorPoint.y -= offset;
			successfulRay = RayOffset();
			cursorPoint.x -= offset;
			cursorPoint.y += offset;
		}

		if (!successfulRay) tempHit = firstHit;

		// Okay we've checked first center, then in a box patter of 8, surely
		// we've hit something the player was reasonably aiming at by now.
		if (successfulRay) {
			if (tempHit.collider.CompareTag("Usable")) { // Use
				UseData ud = new UseData ();
				ud.owner = player;
				UseHandler uh = tempHit.collider.gameObject.GetComponent<UseHandler>();
				if (uh != null) {
					uh.Use(ud);
				} else {
					UseHandlerRelay uhr = tempHit.collider.gameObject.GetComponent<UseHandlerRelay>();
					if (uhr != null) {
						if (uhr.referenceUseHandler != null) {
							uhr.referenceUseHandler.Use(ud);
						}
					} else {
						Debug.Log("BUG: Attempting to use a useable without a UseHandler or UseHandlerRelay!");
					}
				}
			} else if (tempHit.collider.CompareTag("Searchable")) { // Search
				currentSearchItem = tempHit.collider.gameObject;
				SearchObject(currentSearchItem.GetComponent<SearchableItem>().lookUpIndex);
			} else {
				Const.sprint(29); // "Can't use "
			}
		} else { // Frobbed into empty space, so whatever it is is too far.
			if (tempHit.collider != null) {
				// Can't use <something>
				UseName.UseNameSprint(tempHit.collider.gameObject);
			} else {
				// You are too far away from that
				Const.sprint(Const.a.stringTable[30],player);
			}
		}
	}

	bool FrobWithHeldObject() {
		if (heldObjectIndex < 0) {
			Debug.Log("BUG: Attempting to frob with held object, but "
					  + "heldObjectIndex < 0.");
			return false; // Invalid item will be dropped, wasn't used up.
		}

		bool frobUser = (heldObjectIndex == 54 || heldObjectIndex == 56
						 || heldObjectIndex == 57 || heldObjectIndex == 61
						 || heldObjectIndex == 64 || heldObjectIndex == 92
						 || heldObjectIndex == 93 || heldObjectIndex == 94);

		if (!frobUser) return false;

		cursorPoint = MouseCursor.a.GetCursorScreenPointForRay();
		if (!Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint),
							 out tempHit, Const.frobDistance)) {
			return false; // Can't use it on something, go ahead and drop it.
		}

		// Cannot notify of attempt to frob with different index since this is
		// how we normally drop items.
		GameObject go = tempHit.collider.gameObject;
		if (go == null) return false;
		if (!tempHit.collider.CompareTag("Usable")) return false;

		UseData ud = new UseData();
		ud.owner = player;
		ud.mainIndex = heldObjectIndex;
		ud.customIndex = heldObjectCustomIndex;
		UseHandler uh = go.GetComponent<UseHandler>();
		bool playedSound = false;
		if (uh != null) {
			Utils.PlayUIOneShotSavable(91); // searchsound
			playedSound = true;
			uh.Use(ud);
			return true; // Item can get absorbed, not dropped.
		}

		UseHandlerRelay uhr = go.GetComponent<UseHandlerRelay>();
		if (uhr != null) {
			
			if (!playedSound) Utils.PlayUIOneShotSavable(91); // searchsound
			uhr.referenceUseHandler.Use(ud);
			return true; // Item can get absorbed, not dropped.
		}

		Debug.Log("BUG: Attempting to frob use a useable " + go.name
				  + " without a UseHandler or UseHandlerRelay!");

		return false;
	}

	void PutObjectInHand(int useableConstdex, int customIndex, int ammo1,
						 int ammo2, bool loadedAlt, bool fromButton) {
		if (useableConstdex < 0) return;

		holdingObject = true;
		heldObjectIndex = useableConstdex;
		heldObjectCustomIndex = customIndex;
		heldObjectAmmo = ammo1;
		heldObjectAmmo2 = ammo2;
		heldObjectLoadedAlternate = loadedAlt;
		if (fromButton) GUIState.a.ClearOverButton();
		ForceInventoryMode();
	}

	void RemoveWeapon() {
		// Take weapon out of inventory, removing weapon, remove weapon and any
		// other strings I need to CTRL+F my way to this buggy code!
		WeaponButton wepbut = currentButton.GetComponent<WeaponButton>();
		int indexPriorToRemoval = wepbut.useableItemIndex;
		int am1 = WeaponCurrent.a.currentMagazineAmount[wepbut.WepButtonIndex];
		WeaponCurrent.a.currentMagazineAmount[wepbut.WepButtonIndex] = 0;
		int am2 = WeaponCurrent.a.currentMagazineAmount2[wepbut.WepButtonIndex];
		WeaponCurrent.a.currentMagazineAmount2[wepbut.WepButtonIndex] = 0;
		bool loadAlt = false;
		if (am2 > 0) loadAlt = true;
		PutObjectInHand(indexPriorToRemoval,-1,am1,am2,loadAlt,true);
		WeaponCurrent.a.RemoveWeapon(wepbut.WepButtonIndex);
		Inventory.a.RemoveWeapon(wepbut.WepButtonIndex);
		MFDManager.a.SetAmmoIcons(-1,false) ; // Clear the ammo icons.
		MFDManager.a.HideAmmoAndEnergyItems();
		wepbut.useableItemIndex = -1;
		wepbut = MFDManager.a.wepbutMan.wepButtonsScripts[0];
		WeaponCurrent.a.WeaponChange(wepbut.useableItemIndex,
									 wepbut.WepButtonIndex);
	}

	// Because Unity does not see fit for their Button class to support right
	// click behavior...or any other reasonable mouse button interaction.
	void InventoryButtonUse() {
		if (holdingObject) return;
		if (!GUIState.a.overButton) return;
		if (GUIState.a.overButtonType == ButtonType.None) return;
		if (currentButton == null) return;

		int indexPriorToRemoval = -1;
		int customIndexPrior = -1;
		switch(GUIState.a.overButtonType) {
			case ButtonType.Weapon: RemoveWeapon(); break;
			case ButtonType.Grenade:
				GrenadeButton grenbut = currentButton.GetComponent<GrenadeButton>();
				indexPriorToRemoval = grenbut.useableItemIndex;
				Inventory.a.grenAmmo[grenbut.GrenButtonIndex]--;
				Inventory.a.GrenadeCycleDown();
				//Inventory.a.grenadeCurrent = -1; This was up here, and seemed fine.  Might need to revert line 473 add.
				if (Inventory.a.grenAmmo[grenbut.GrenButtonIndex] <= 0) {
					Inventory.a.grenAmmo[grenbut.GrenButtonIndex] = 0;
					Inventory.a.grenadeCurrent = -1;
					for (int i = 0; i < 7; i++) {
						if (Inventory.a.grenAmmo[i] > 0) {
							Inventory.a.grenadeCurrent = i;
						}
					}

					MFDManager.a.SendInfoToItemTab(Inventory.a.grenadeCurrent);
					if (Inventory.a.grenadeCurrent < 0) {
						Inventory.a.grenadeCurrent = 0;
					}
				}

				grenadeActive = true;
				PutObjectInHand(indexPriorToRemoval,-1,0,0,false,true);
				break;
			case ButtonType.Patch:
				PatchButton patbut = currentButton.GetComponent<PatchButton>();
				indexPriorToRemoval = patbut.useableItemIndex;
				Inventory.a.patchCounts[patbut.PatchButtonIndex]--;
				if (Inventory.a.patchCounts[patbut.PatchButtonIndex] <= 0) {
					Inventory.a.patchCounts[patbut.PatchButtonIndex] = 0;
					Inventory.a.patchCurrent = -1;
					GUIState.a.ClearOverButton();
					for (int i = 0; i < 7; i++) {
						if (Inventory.a.patchCounts[i] > 0) Inventory.a.patchCurrent = i;
					}
					MFDManager.a.SendInfoToItemTab(Inventory.a.patchCurrent);
					if (Inventory.a.patchCurrent < 0) {
						Inventory.a.patchCurrent = 0;
					}
				}
				PutObjectInHand(indexPriorToRemoval,-1,0,0,false,true);
				break;
			case ButtonType.GeneralInv:
				GeneralInvButton genbut = 
					currentButton.GetComponent<GeneralInvButton>();

				// Access Cards button
				if (genbut.GeneralInvButtonIndex == 0) {
					MFDManager.a.OpenLastItemSide();
					MFDManager.a.SendInfoToItemTab(81);
					return;
				}

				indexPriorToRemoval = genbut.useableItemIndex;
				customIndexPrior = genbut.customIndex;
				Inventory.a.generalInventoryIndexRef[genbut.GeneralInvButtonIndex] = -1;
				Inventory.a.generalInvCurrent = -1;
				for (int i = 0; i < 7; i++) {
					if (Inventory.a.generalInventoryIndexRef[i] >= 0) {
						Inventory.a.generalInvCurrent = i;
					}
				}
				int referenceIndex = -1;
				if (Inventory.a.generalInvCurrent >= 0) {
					referenceIndex = Inventory.a.genButtons[Inventory.a.generalInvCurrent].transform.GetComponent<GeneralInvButton>().useableItemIndex;
				}

				if (referenceIndex < 0 || referenceIndex > 110) {
					MFDManager.a.ResetItemTab();
				} else {
					MFDManager.a.SendInfoToItemTab(referenceIndex,genbut.customIndex);
				}
				PutObjectInHand(indexPriorToRemoval,customIndexPrior,0,0,false,true);
				break;
			case ButtonType.Search:
				SearchButton sebut = currentButton.GetComponentInParent<SearchButton>();
				int tempButtonindex = currentButton.GetComponent<SearchContainerButton>().refIndex;
				SearchButtonClick(tempButtonindex,sebut);
				break;
			case ButtonType.PGrid:
				PuzzleUIButton puib = currentButton.GetComponent<PuzzleUIButton>();
				if (puib != null) puzzleGrid.OnGridCellClick(puib.buttonIndex);
				break;
			case ButtonType.PWire:
				PuzzleUIButton wpuib = currentButton.GetComponent<PuzzleUIButton>();
				if (wpuib != null) {
					if (wpuib.isRH)
						puzzleWire.ClickRHNode(wpuib.buttonIndex);
					else
						puzzleWire.ClickLHNode(wpuib.buttonIndex);
				}
				break;
			case ButtonType.Vaporize:
				VaporizeButton vapB = currentButton.GetComponent<VaporizeButton>();
				if (vapB != null) {
					vapB.OnVaporizeClick();
				}
				break;
			case ButtonType.ShootMode:
				ForceShootMode();
				GUIState.a.ClearOverButton();
				break;
			case ButtonType.GrenadeTimerSlider:
				Button btn = currentButton.GetComponent<Button>();
				Debug.Log("GrenadeTimerSlider invoke");
				break;
		}
	}
	
	public void SearchButtonClick(int index, SearchButton sebut) {
		holdingObject = true;
		heldObjectIndex = sebut.contents[index];
		heldObjectCustomIndex = sebut.customIndex[index];
		if (currentSearchItem != null) {
			SearchableItem sitem = currentSearchItem.GetComponent<SearchableItem>();
			sitem.contents[index] = -1;
			sitem.customIndex[index] = -1;
		}
		
		sebut.contents[index] = -1;
		sebut.customIndex[index] = -1;
		MFDManager.a.DisableSearchItemImage(index);
		sebut.CheckForEmpty();
		GUIState.a.ClearOverButton();
		if (Const.a.InputQuickItemPickup) {
			AddItemToInventory(heldObjectIndex,heldObjectCustomIndex);
			ResetHeldItem();
		} else {
			Const.sprint(Const.a.stringTable[heldObjectIndex + 326] + Const.a.stringTable[319],player);
			ForceInventoryMode();
		}	
	}

	void RecoilAndRest() {
		float targetY = Const.a.playerCameraOffsetY
						* PlayerMovement.a.currentCrouchRatio;
		float targetX = 0f;
		if (PlayerMovement.a.relSideways > 0) targetX += 0.12f;
		if (PlayerMovement.a.relSideways < 0) targetX -= 0.12f;
		if (PlayerMovement.a.relForward != 0) targetY -= 0.08f;

		// If not shaking or bobbing, this will stay this to lerp to normal.
// 		headBobY = Const.a.playerCameraOffsetY
// 				   * PlayerMovement.a.currentCrouchRatio;
		if (shakeFinished > PauseScript.a.relativeTime) {
			headBobX = transform.localPosition.x
					   + UnityEngine.Random.Range(shakeForce * -0.17f,
												  shakeForce * 0.17f);

			headBobY = transform.localPosition.y
					   + UnityEngine.Random.Range(shakeForce * -0.08f,
												  shakeForce * 0.08f);

			headBobZ = transform.localPosition.z
					   + UnityEngine.Random.Range(shakeForce * -0.17f,
												  shakeForce * 0.17f);
		} else {
			headBobZ = 0f;
			Vector3 vel = PlayerMovement.a.rbody.velocity;
			vel.y = 0f;
			if (PlayerMovement.a.relForward + PlayerMovement.a.relSideways != 0
				&& Const.a.HeadBob) {

				if (headBobShiftFinished < PauseScript.a.relativeTime) {
					headBobShiftFinished = PauseScript.a.relativeTime + 0.2f;
					if (!PlayerMovement.a.isSprinting) {
						headBobShiftFinished += 0.1f;
					}

					bobTarget = Const.HeadBobAmount * -1f
								* Mathf.Sign(bobTarget);
				}

				if (PlayerMovement.a.rbody.velocity.magnitude > 0.1f){
					headBobY = Mathf.SmoothDamp(headBobY,targetY + bobTarget,ref headBobYVel,Const.HeadBobRate);
				}

				headBobX = Mathf.SmoothDamp(headBobX,targetX,ref headBobXVel,Const.HeadBobRate);
			} else {
				headBobX = Mathf.SmoothDamp(headBobX,0f,ref headBobXVel,Const.HeadBobRate);
				headBobY = Mathf.SmoothDamp(headBobY,Const.a.playerCameraOffsetY * PlayerMovement.a.currentCrouchRatio,ref headBobYVel,Const.HeadBobRate);
			}
		}
		
		if (inCyberSpace) {
			headBobX = 0f;
			headBobY = 0f;
			headBobZ = 0f;
		}
		
		transform.localPosition = new Vector3(headBobX,headBobY,headBobZ);
	}

	void AddItemFail(int index) { // Expects usableItem index
		DropHeldItem();
		Const.sprint(Const.a.stringTable[32] + Const.a.stringTable[index + 326]
					 + Const.a.stringTable[318],player); // Inventory full.
	}

	public void AddItemToInventory(int index, int customIndex) {
		MFDManager.a.mouseClickHeldOverGUI = true; // Prevent gun shooting.
		if (index < 0) index = 0; // Good check on paper.
		if (index > 110) index = 94; // Way to get a head.
		if ((index >= 0 && index <= 5)
             || index == 33
             || index == 35
             || (index >= 52 && index < 59)
             || (index >= 61 && index <= 64)
             || (index >= 92 && index <= 101)) {
			if (!Inventory.a.AddGeneralObjectToInventory(index,customIndex)) {
				AddItemFail(index);
			}
		} else if (index == 6) {
			Inventory.a.AddAudioLogToInventory(heldObjectCustomIndex);
		} else if (index >= 36 && index <= 51) {
			if (!Inventory.a.AddWeaponToInventory(index,heldObjectAmmo,
												  heldObjectAmmo2,
												  heldObjectLoadedAlternate)) {
				AddItemFail(index);
			}
		} else if (index == 34 || index == 81 || (index >= 83 && index <= 91) || index == 110) {
			Inventory.a.AddAccessCardToInventory(index);
		} else {
			switch (index) {
				case 7:  Inventory.a.AddGrenadeToInventory(0,index); break; // Frag
				case 8:  Inventory.a.AddGrenadeToInventory(3,index); break; // Concussion
				case 9:  Inventory.a.AddGrenadeToInventory(1,index); break; // EMP
				case 10: Inventory.a.AddGrenadeToInventory(6,index); break; // Earth Shaker
				case 11: Inventory.a.AddGrenadeToInventory(4,index); break; // Land Mine
				case 12: Inventory.a.AddGrenadeToInventory(5,index); break; // Nitropak
				case 13: Inventory.a.AddGrenadeToInventory(2,index); break; // Gas
				case 14: Inventory.a.AddPatchToInventory(2,index); break;
				case 15: Inventory.a.AddPatchToInventory(6,index); break;
				case 16: Inventory.a.AddPatchToInventory(5,index); break;
				case 17: Inventory.a.AddPatchToInventory(3,index); break;
				case 18: Inventory.a.AddPatchToInventory(4,index); break;
				case 19: Inventory.a.AddPatchToInventory(1,index); break;
				case 20: Inventory.a.AddPatchToInventory(0,index); break;
				case 21: Inventory.a.AddHardwareToInventory(0,index,customIndex,true); break;
				case 22: Inventory.a.AddHardwareToInventory(1,index,customIndex,true); break;
				case 23: Inventory.a.AddHardwareToInventory(2,index,customIndex,true); break;
				case 24: Inventory.a.AddHardwareToInventory(3,index,customIndex,true); break;
				case 25: Inventory.a.AddHardwareToInventory(4,index,customIndex,true); break;
				case 26: Inventory.a.AddHardwareToInventory(5,index,customIndex,true); break;
				case 27: Inventory.a.AddHardwareToInventory(6,index,customIndex,true); break;
				case 28: Inventory.a.AddHardwareToInventory(7,index,customIndex,true); break;
				case 29: Inventory.a.AddHardwareToInventory(8,index,customIndex,true); break;
				case 30: Inventory.a.AddHardwareToInventory(9,index,customIndex,true); break;
				case 31: Inventory.a.AddHardwareToInventory(10,index,customIndex,true); break;
				case 32: Inventory.a.AddHardwareToInventory(11,index,customIndex,true); break;
				case 60: Inventory.a.AddAmmoToInventory(12,index, Const.a.magazinePitchCountForWeapon[12], false); break; // rubber slugs
				case 65: Inventory.a.AddAmmoToInventory(8,index, Const.a.magazinePitchCountForWeapon2[8], true); break; // magpulse cartridge super
				case 66: Inventory.a.AddAmmoToInventory(2,index, Const.a.magazinePitchCountForWeapon[2], false); break; // needle darts
				case 67: Inventory.a.AddAmmoToInventory(2,index, Const.a.magazinePitchCountForWeapon2[2], true); break; // tranquilizer darts
				case 68: Inventory.a.AddAmmoToInventory(9,index, Const.a.magazinePitchCountForWeapon[9], false); break; // standard bullets
				case 69: Inventory.a.AddAmmoToInventory(9,index, Const.a.magazinePitchCountForWeapon2[9], true); break; // teflon bullets
				case 70: Inventory.a.AddAmmoToInventory(7,index, Const.a.magazinePitchCountForWeapon[7], false); break; // hollow point rounds
				case 71: Inventory.a.AddAmmoToInventory(7,index, Const.a.magazinePitchCountForWeapon2[7], true); break; // slug rounds
				case 72: Inventory.a.AddAmmoToInventory(0,index, Const.a.magazinePitchCountForWeapon[0], false); break; // magnesium tipped slugs
				case 73: Inventory.a.AddAmmoToInventory(0,index, Const.a.magazinePitchCountForWeapon2[0], true); break; // penetrator slugs
				case 74: Inventory.a.AddAmmoToInventory(3,index, Const.a.magazinePitchCountForWeapon[3], false); break; // hornet clip
				case 75: Inventory.a.AddAmmoToInventory(3,index, Const.a.magazinePitchCountForWeapon2[3], true); break; // splinter clip
				case 76: Inventory.a.AddAmmoToInventory(11,index, Const.a.magazinePitchCountForWeapon[11], false); break; // rail rounds
				case 77: Inventory.a.AddAmmoToInventory(13,index, Const.a.magazinePitchCountForWeapon[13], false); break; // slag magazine
				case 78: Inventory.a.AddAmmoToInventory(13,index, Const.a.magazinePitchCountForWeapon2[13], true); break; // large slag magazine
				case 79: Inventory.a.AddAmmoToInventory(8,index, Const.a.magazinePitchCountForWeapon[8], false); break; // magpulse cartridges
				case 80: Inventory.a.AddAmmoToInventory(8,index, Const.a.magazinePitchCountForWeapon2[8], false); break; // small magpulse cartridges
			}
		}

		Utils.PlayUIOneShotSavable(87); // frob_item
		int numberFoundContents = 0;
		if (currentSearchItem != null) {
			SearchableItem curSearchScript = currentSearchItem.GetComponent<SearchableItem>();
			if (curSearchScript != null) {
				int[] resultContents = {-1,-1,-1,-1};  // create blanked container for search results
				for (int i=3;i>=0;i--) {
					resultContents[i] = curSearchScript.contents[i];
					if (resultContents[i] > -1) numberFoundContents++; // if something was found, add 1 to count
				}
			}
	    	if (numberFoundContents == 0) {
				currentSearchItem = null;
				MFDManager.a.ReturnTabsFromSearch();
			}
		}
		firstTimePickup = false;
	}

	public void DropHeldItem() {
		dropFinished = Time.time + 0.2f; // Prevent immediate regrab at high fps
		if (heldObjectIndex < 0 || heldObjectIndex > 110) { 
			Debug.Log("BUG: Attempted to DropHeldItem with index out of bounds (<0 or >110) and heldObjectIndex = " + heldObjectIndex.ToString(),player);
			ResetHeldItem();
			return;
		}

		if (!grenadeActive) heldObject = Const.a.GetPrefab(heldObjectIndex + 307); // heldObject is set by UseGrenade() so don't override here.
		if (heldObject == null) {
			Const.sprint("BUG: Object "+heldObjectIndex.ToString()+" not assigned, vaporized.",player);
			ResetHeldItem();
			return;
		}

		GameObject tossObject = null;
		bool freeObjectInPoolFound = false;
		GameObject levelDynamicContainer = LevelManager.a.GetCurrentDynamicContainer();

		// Find any free inactive objects within the level's Levelnumber.Dynamic container and activate those before instantiating
		if (!grenadeActive) {
			for (int i=0;i<levelDynamicContainer.transform.childCount;i++) {
				Transform tr = levelDynamicContainer.transform.GetChild(i);
				GameObject go = tr.gameObject;
				UseableObjectUse reference = go.GetComponent<UseableObjectUse>();
				if (reference != null) {
					if (reference.useableItemIndex == heldObjectIndex && go.activeSelf == false) {
						reference.customIndex = heldObjectCustomIndex;
						tossObject = go;
						freeObjectInPoolFound = true;
						break;
					}
				}
			}

			if (freeObjectInPoolFound) {
				if (tossObject == null) {
					Const.sprint("BUG: Failed to get freeObjectInPool for object being dropped!",player);
					ResetHeldItem();
					return;
				} else {
					tossObject.transform.position = (transform.position + (transform.forward * tossOffset));
				}
			} else {
				// Debug.Log("WARNING: Failed to get freeObjectInPool for object " + heldObject.ToString() + "being dropped! MouseLookScript DropHeldItem.",player);
				tossObject = Instantiate(heldObject,(transform.position + (transform.forward * tossOffset)),Const.a.quaternionIdentity) as GameObject;  //effect
				if (tossObject == null) {
					Const.sprint("BUG: Failed to instantiate object being dropped!",player);
					ResetHeldItem();
					return;
				}
			}
			if (tossObject.activeSelf != true) tossObject.SetActive(true);
			if (levelDynamicContainer != null) {
				tossObject.transform.SetParent(levelDynamicContainer.transform,true);
			}

			Vector3 tossDir = MouseCursor.a.GetCursorScreenPointForRay();
			tossDir = playerCamera.ScreenPointToRay(tossDir).direction;
			Rigidbody rbody = tossObject.GetComponent<Rigidbody>();
			if (rbody != null) {
				rbody.isKinematic = false;
				rbody.useGravity = true;
				rbody.velocity = tossDir * tossForce;
			}

			UseableObjectUse uou = tossObject.GetComponent<UseableObjectUse>();
			uou.customIndex = heldObjectCustomIndex;
			uou.ammo = heldObjectAmmo;
			uou.ammo2 = heldObjectAmmo2;
			uou.heldObjectLoadedAlternate = heldObjectLoadedAlternate;
		} else {
			// Throw an active grenade
			grenadeActive = false;
			MFDManager.a.mouseClickHeldOverGUI = true; // Prevent shooting it.
			tossObject = Instantiate(heldObject,(transform.position + (transform.forward * tossOffset)),Const.a.quaternionIdentity) as GameObject;  //effect
			if (tossObject == null) {
				Const.sprint("BUG: Failed to instantiate object being dropped!",player);
				ResetHeldItem();
				return;
			}

            Const.a.grenadesThrown++;
			if (levelDynamicContainer != null){
				tossObject.transform.SetParent(levelDynamicContainer.transform,true);
			}
			tossObject.layer = 11; // Set to player bullets layer to prevent collision and still be visible.
			Vector3 tossDir = MouseCursor.a.GetCursorScreenPointForRay();
			tossDir = playerCamera.ScreenPointToRay(tossDir).direction;
			Rigidbody rbody = tossObject.GetComponent<Rigidbody>();
			if (rbody != null) {
				rbody.isKinematic = false;
				rbody.useGravity = true;
				rbody.velocity = tossDir * tossForce;
			}
			GrenadeActivate ga = tossObject.GetComponent<GrenadeActivate>();
			if (ga != null) ga.Activate(); // Time to boom!
			MouseCursor.a.liveGrenade = false;
		}
		ResetHeldItem();
	}

	public void ResetHeldItem() {
		heldObjectIndex = -1;
		heldObjectCustomIndex = -1;
		heldObjectAmmo = 0;
		heldObjectAmmo2 = 0;
		heldObjectLoadedAlternate = false;
		holdingObject = false;
		grenadeActive = false;
		MouseCursor.a.justDroppedItemInHelper = true;
	}

	public void ToggleInventoryMode() {
		if (inventoryMode)	ForceShootMode();
		else				ForceInventoryMode();
	}

	public void ForceShootMode() {
		if (Const.a.NoShootMode) return; // We are being like the original now!

		GUIState.a.ClearOverButton();
		MFDManager.a.mouseClickHeldOverGUI = false;
		Automap.a.CloseFullmap();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		inventoryMode = false;
		if (Application.platform == RuntimePlatform.Android) {
			shootModeButton.SetActive(true);
		} else {
			shootModeButton.SetActive(false);
		}

		if (vmailActive) {
			Inventory.a.DeactivateVMail();
			vmailActive = false;
		}
	}

	public void ForceInventoryMode() {
		if (inventoryMode) return;

		GUIState.a.ClearOverButton();
		if (PauseScript.a.MenuActive() || PauseScript.a.Paused()) {
			Cursor.lockState = CursorLockMode.None;
		} else {
			#if UNITY_EDITOR
				Cursor.lockState = CursorLockMode.None;
			#else	
				Cursor.lockState = CursorLockMode.Confined;
			#endif
		}
		MouseCursor.SetCursorPosInternal((int)(Screen.width * 0.5f),(int)(Screen.height * 0.5f));
		Cursor.visible = false;
		MouseCursor.a.deltaX = 0;
		MouseCursor.a.deltaY = 0;
		MouseCursor.a.cursorPosition.x = (Screen.width / 2);
		MouseCursor.a.cursorPosition.y = (Screen.height / 2);
		inventoryMode = true;
		if (!Const.a.noHUD) shootModeButton.SetActive(true);
		else shootModeButton.SetActive(false);
	}

	void SearchObject (int index){
		if (currentSearchItem == null) { Debug.Log("BUG: Early exit from SearchObject, currentSearchItem was null!"); return;}

		bool useFX = true;
		SearchableItem curSearchScript = currentSearchItem.GetComponent<SearchableItem>();
		if (curSearchScript.searchableInUse) {
			for (int i=0;i<4;i++) {
				if (curSearchScript.contents[i] >= 0) {
					MouseCursor.a.GetComponent<MouseCursor>().cursorImage = Const.a.useableItemsFrobIcons[curSearchScript.contents[i]];
					heldObjectIndex = curSearchScript.contents[i];
					heldObjectCustomIndex = curSearchScript.customIndex[i];
					curSearchScript.contents[i] = -1;
					curSearchScript.customIndex[i] = -1;
					if (heldObjectIndex != -1) holdingObject = true;
					Const.sprint(Const.a.stringTable[heldObjectIndex + 326]
								 + Const.a.stringTable[319],player); // picked up

					MFDManager.a.DisableSearchItemImage(i);
					useFX = false;
					break;
				}
			}
		} else {
			Utils.PlayUIOneShotSavable(91); // searchsound
		}

		curSearchScript.searchableInUse = true;

		// Search through array to see if any items are in the container
		int numberFoundContents = 0;
		int[] resultContents = {-1,-1,-1,-1};  // create blanked container for search results
		int[] resultCustomIndex = {-1,-1,-1,-1};  // create blanked container for search results custom indices
		for (int i=3;i>=0;i--) {
			resultContents[i] = curSearchScript.contents[i];
			resultCustomIndex[i] = curSearchScript.customIndex[i];
			// If something was found, add 1 to count.
			if (resultContents[i] > -1) numberFoundContents++;
		}

		if (firstTimeSearch) {
			firstTimeSearch = false;
			MFDManager.a.OpenTab (4, true, TabMSG.Search, -1,Handedness.LH);
		}
		MFDManager.a.SendSearchToDataTab(curSearchScript.objectName,
										 numberFoundContents,resultContents,
										 resultCustomIndex,
										 currentSearchItem.transform.position,
										 curSearchScript, useFX);
		ForceInventoryMode();
	}

	public void UseGrenade (int index) {
		if (holdingObject) { Const.sprint(Const.a.stringTable[311],player); return; } // Can't use grenade, hands full
		if (index < 7 || index > 13) { Debug.Log("BUG: index outside of 7 to 13 passed to UseGrenade() in MouseLookScript.cs"); return; }

		ForceInventoryMode();  // Inventory mode is turned on when picking something up.
		ResetHeldItem();
		MouseCursor.a.liveGrenade = true;
		grenadeActive = true;
		Const.sprint(Const.a.stringTable[index + 326]
					 + Const.a.stringTable[320],player); // activated, grenade is LIVE!

		switch(index) { // Subtract one from the correct grenade inventory
			case 7:  heldObject = Const.a.GetPrefab(370); Inventory.a.RemoveGrenade(0); break; // Frag
			case 8:  heldObject = Const.a.GetPrefab(372); Inventory.a.RemoveGrenade(3); break; // Concussion
			case 9:  heldObject = Const.a.GetPrefab(387); Inventory.a.RemoveGrenade(1); break; // EMP
			case 10: heldObject = Const.a.GetPrefab(389); Inventory.a.RemoveGrenade(6); break; // Earth Shaker
			case 11: heldObject = Const.a.GetPrefab(402); Inventory.a.RemoveGrenade(4); break; // Land Mine
			case 12: heldObject = Const.a.GetPrefab(403); Inventory.a.RemoveGrenade(5); break; // Nitropak
			case 13: heldObject = Const.a.GetPrefab(404); Inventory.a.RemoveGrenade(2); break; // Gas
		}
		MFDManager.a.ResetItemTab();
		PutObjectInHand(index,-1,0,0,false,true);
	}

	public void ScreenShake (float force, float duration) {
		shakeFinished = PauseScript.a.relativeTime + duration;
		if (force < 0.48f) shakeForce = force;
		else shakeForce = 0.48f;
	}

	public static string Save(GameObject go) {
		MouseLookScript ml = go.GetComponent<MouseLookScript>();
        s1.Clear();
		s1.Append(Utils.BoolToString(ml.gameObject.activeSelf,"MouseLookScript.gameObject.activeSelf"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ml.playerCamera.enabled,"playerCamera.enabled"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ml.inventoryMode,"inventoryMode"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ml.holdingObject,"holdingObject"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(ml.heldObjectIndex,"heldObjectIndex"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(ml.heldObjectCustomIndex,"heldObjectCustomIndex"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(ml.heldObjectAmmo,"heldObjectAmmo"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(ml.heldObjectAmmo2,"heldObjectAmmo2"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ml.heldObjectLoadedAlternate,"heldObjectLoadedAlternate"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ml.firstTimePickup,"firstTimePickup"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ml.firstTimeSearch,"firstTimeSearch"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ml.grenadeActive,"grenadeActive"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ml.inCyberSpace,"inCyberSpace"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.yRotation,"yRotation"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ml.geniusActive,"geniusActive"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.xRotation,"xRotation"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ml.vmailActive,"vmailActive"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceReturnPoint.x,"cyberspaceReturnPoint.x"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceReturnPoint.y,"cyberspaceReturnPoint.y"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceReturnPoint.z,"cyberspaceReturnPoint.z"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceReturnCameraLocalRotation.x,"cyberspaceReturnCameraLocalRotation.x"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceReturnCameraLocalRotation.y,"cyberspaceReturnCameraLocalRotation.y"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceReturnCameraLocalRotation.z,"cyberspaceReturnCameraLocalRotation.z"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceReturnPlayerCapsuleLocalRotation.x,"cyberspaceReturnPlayerCapsuleLocalRotation.x"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceReturnPlayerCapsuleLocalRotation.y,"cyberspaceReturnPlayerCapsuleLocalRotation.y"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceReturnPlayerCapsuleLocalRotation.z,"cyberspaceReturnPlayerCapsuleLocalRotation.z"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceRecallPoint.x, "cyberspaceRecallPoint.x"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceRecallPoint.y,"cyberspaceRecallPoint.y"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(ml.cyberspaceRecallPoint.z,"cyberspaceRecallPoint.z"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(ml.cyberspaceReturnLevel,"cyberspaceReturnLevel"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(ml.returnFromCyberspaceFinished,"returnFromCyberspaceFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(ml.randomShakeFinished,"randomShakeFinished"));
        s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(ml.randomKlaxonFinished,"randomKlaxonFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(ml.shakeFinished,"shakeFinished"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		MouseLookScript ml = go.GetComponent<MouseLookScript>();
		float readFloatx, readFloaty, readFloatz;
		ml.gameObject.SetActive(Utils.GetBoolFromString(entries[index],"MouseLookScript.gameObject.activeSelf")); index++;
		ml.playerCamera.enabled = Utils.GetBoolFromString(entries[index],"playerCamera.enabled"); index++;
		ml.inventoryMode = !Utils.GetBoolFromString(entries[index],"inventoryMode"); index++; // Take opposite because we are about to opposite again...
		ml.ToggleInventoryMode(); // ...correctly set cursor lock state, and opposite again, now it is what was saved
		PauseScript.a.previousInvMode = ml.inventoryMode; // Prevent it changing it inadvertently after load unpauses.
		ml.holdingObject = Utils.GetBoolFromString(entries[index],"holdingObject"); index++;
		ml.heldObjectIndex = Utils.GetIntFromString(entries[index],"heldObjectIndex"); index++;
		ml.heldObjectCustomIndex = Utils.GetIntFromString(entries[index],"heldObjectCustomIndex"); index++;
		ml.heldObjectAmmo = Utils.GetIntFromString(entries[index],"heldObjectAmmo"); index++;
		ml.heldObjectAmmo2 = Utils.GetIntFromString(entries[index],"heldObjectAmmo2"); index++;
		ml.heldObjectLoadedAlternate = Utils.GetBoolFromString(entries[index],"heldObjectLoadedAlternate"); index++;
		ml.firstTimePickup = Utils.GetBoolFromString(entries[index],"firstTimePickup"); index++;
		ml.firstTimeSearch = Utils.GetBoolFromString(entries[index],"firstTimeSearch"); index++;
		ml.grenadeActive = Utils.GetBoolFromString(entries[index],"grenadeActive"); index++;
		ml.inCyberSpace = Utils.GetBoolFromString(entries[index],"inCyberSpace"); index++;
		ml.yRotation = Utils.GetFloatFromString(entries[index],"yRotation"); index++;
		ml.geniusActive = Utils.GetBoolFromString(entries[index],"geniusActive"); index++;
		ml.xRotation = Utils.GetFloatFromString(entries[index],"xRotation"); index++;
		ml.vmailActive = Utils.GetBoolFromString(entries[index],"vmailActive"); index++;
		readFloatx = Utils.GetFloatFromString(entries[index],"cyberspaceReturnPoint.x"); index++;
		readFloaty = Utils.GetFloatFromString(entries[index],"cyberspaceReturnPoint.y"); index++;
		readFloatz = Utils.GetFloatFromString(entries[index],"cyberspaceReturnPoint.z"); index++;
		ml.cyberspaceReturnPoint = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = Utils.GetFloatFromString(entries[index],"cyberspaceReturnCameraLocalRotation.x"); index++;
		readFloaty = Utils.GetFloatFromString(entries[index],"cyberspaceReturnCameraLocalRotation.y"); index++;
		readFloatz = Utils.GetFloatFromString(entries[index],"cyberspaceReturnCameraLocalRotation.z"); index++;
		ml.cyberspaceReturnCameraLocalRotation = new Vector3(readFloatx,readFloaty,readFloatz);

		 // Euler Angles, only 3
		readFloatx = Utils.GetFloatFromString(entries[index],"cyberspaceReturnPlayerCapsuleLocalRotation.x"); index++;
		readFloaty = Utils.GetFloatFromString(entries[index],"cyberspaceReturnPlayerCapsuleLocalRotation.y"); index++;
		readFloatz = Utils.GetFloatFromString(entries[index],"cyberspaceReturnPlayerCapsuleLocalRotation.z"); index++;
		ml.cyberspaceReturnPlayerCapsuleLocalRotation = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = Utils.GetFloatFromString(entries[index],"cyberspaceRecallPoint.x"); index++;
		readFloaty = Utils.GetFloatFromString(entries[index],"cyberspaceRecallPoint.y"); index++;
		readFloatz = Utils.GetFloatFromString(entries[index],"cyberspaceRecallPoint.z"); index++;
		ml.cyberspaceRecallPoint = new Vector3(readFloatx,readFloaty,readFloatz);
		ml.cyberspaceReturnLevel = Utils.GetIntFromString(entries[index],"cyberspaceReturnLevel"); index++;
		ml.returnFromCyberspaceFinished = Utils.LoadRelativeTimeDifferential(entries[index],"returnFromCyberspaceFinished"); index++;
		ml.randomShakeFinished = Utils.LoadRelativeTimeDifferential(entries[index],"randomShakeFinished"); index++;
		ml.randomKlaxonFinished = Utils.LoadRelativeTimeDifferential(entries[index],"randomKlaxonFinished"); index++;
		ml.shakeFinished = Utils.LoadRelativeTimeDifferential(entries[index],"shakeFinished"); index++;

		// Prevent picking up first item immediately. Not currently possible to
		// save references (without a lot of work, aherm).
		ml.currentSearchItem = null;

		// Left/Right component applied to capsule.
		Transform capsuleTr = ml.transform.parent.transform.parent.transform;
		capsuleTr.localRotation = Quaternion.Euler(0f, ml.yRotation, 0f);

		// Up/Down component only applied to camera.
		Transform cameraTr = ml.transform;
		cameraTr.localRotation = Quaternion.Euler(ml.xRotation,0f,0f);
		return index;
	}
}
