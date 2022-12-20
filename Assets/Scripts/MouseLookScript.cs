using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MouseLookScript : MonoBehaviour {
    // External references
	public GameObject player;
	[Tooltip("The current cursor texture (For Reference or Testing)")] public Texture2D cursorTexture;
    [Tooltip("The default cursor texture (Developer sets default)")] public Texture2D cursorDefaultTexture;
    [Tooltip("The cyberspace cursor")] public Texture2D cyberspaceCursor;
    [Tooltip("Sound effect to play when searching an object")] public AudioClip SearchSFX;
    [Tooltip("Sound effect to play when picking-up/frobbing an object")] public AudioClip PickupSFX;
    [Tooltip("Sound effect to play when picking-up/frobbing a hardware item")] public AudioClip hwPickupSFX;
    [Tooltip("Sound effect to play when entering/exiting cyberspace")] public AudioClip CyberSFX;
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
	public AudioSource SFXSource;
	public GameObject[] hardwareButtons;
	public PuzzleWire puzzleWire;
	public PuzzleGrid puzzleGrid;
	public GameObject shootModeButton;
	public HealthManager hm;
	public GameObject playerRadiationTreatmentFlash;

    // Internal references
    [HideInInspector] public bool inventoryMode;
	[HideInInspector] public bool holdingObject;
    [HideInInspector] public Vector2 cursorHotspot;
    [HideInInspector] public Vector3 cameraFocusPoint;
	[HideInInspector] public GameObject currentButton;
	[HideInInspector] public GameObject currentSearchItem;
	[HideInInspector] public Vector3 cyberLookDir;
    [HideInInspector] public int heldObjectIndex; // save
	[HideInInspector] public int heldObjectCustomIndex; // save
	[HideInInspector] public int heldObjectAmmo; // save
	[HideInInspector] public int heldObjectAmmo2; // save
	[HideInInspector] public bool firstTimePickup;
	[HideInInspector] public bool firstTimeSearch;
	[HideInInspector] public bool grenadeActive;
	[HideInInspector] public bool inCyberSpace;
    [HideInInspector] public float yRotation;
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
    private Camera playerCamera;
    private GameObject heldObject;
	private Quaternion tempQuat;
	private Vector3 tempVec;
    private RaycastHit tempHit;
	private Vector3 cameraRecoilLerpPos;
	private float cyberSpinSensitivity = 0.5f;
	private float shakeFinished;
	private float shakeForce;
	private string f9 = "f9";
	private string f6 = "f6";
	private string qsavename = "quicksave";
	private string mouseX = "Mouse X";
	private string mouseY = "Mouse Y";
	private Vector3 cursorPoint;

	public static MouseLookScript a;

	void Awake() {
		a = this;
	}

    void Start (){
        ResetCursor();
		ResetHeldItem();
		Cursor.lockState = CursorLockMode.None;
		inventoryMode = false; // Start with inventory mode turned off.
		shootModeButton.SetActive(false);
		playerCamera = GetComponent<Camera>();
		cameraDistances = new float[32];
		SetCameraCullDistances();
		playerCamera.depthTextureMode = DepthTextureMode.Depth;
		grenadeActive = false;
		yRotation = 0;
		xRotation = 0;
		cyberLookDir = Const.a.vectorZero;
		canvasContainer.SetActive(true); // Enable UI.
		firstTimePickup = true;
		firstTimeSearch = true;
		inCyberSpace = false;
		shakeFinished = PauseScript.a.relativeTime;
    }

	void Update() {
		// Always allowed regardless of pause state:
        if (Cursor.visible) Cursor.visible = false; // Hides main cursor so we can show custom cursor textures and position cursor smartly independently.
		if (Input.GetKeyUp(f9)) Const.a.Load(7,false); // Allow quick load straight from the menu.

        if (PauseScript.a.MenuActive()) { if (playerCamera.enabled) playerCamera.enabled = false; return; } // Ignore mouselook and turn off camera when main menu is up.
		else 							  if (!playerCamera.enabled) playerCamera.enabled = true;

		if (PauseScript.a.Paused()) return;
		if (PlayerMovement.a.ressurectingFinished > PauseScript.a.relativeTime) return;

		// Unpaused, normal functions::
		// ==============================================================================================================================================================================================================================================================================================
		if (Input.GetKeyUp(f6)) Const.a.StartSave(7,qsavename);
		if(GetInput.a.ToggleMode()) ToggleInventoryMode(); // Toggle inventory mode<->shoot mode
		RecoilAndRest(); // Spring Back to Rest from Recoil
		keyboardTurnSpeed = 15f * Const.a.MouseSensitivity;
		KeyboardTurn();
		KeyboardLookUpDn();
		if (inCyberSpace) { // Barrel roll!
			if (GetInput.a.LeanLeft()) transform.RotateAround(transform.position,transform.forward,cyberSpinSensitivity * Time.deltaTime * -1f);
			if (GetInput.a.LeanRight()) transform.RotateAround(transform.position,transform.forward,cyberSpinSensitivity * Time.deltaTime);
		} else {
			if (compassContainer.activeInHierarchy) compassContainer.transform.rotation = Quaternion.Euler(0f, -yRotation + 180f, 0f); // Update automap player icon orientation.
		}
		if (!inventoryMode) Mouselook(Input.GetAxisRaw(mouseX) * Const.a.MouseSensitivity,Input.GetAxisRaw(mouseY) * Const.a.MouseSensitivity);  // Only do mouselook in Shoot Mode.

		// Frob what is under our cursor.
		if(GetInput.a.Use()) {
			if (vmailActive) { Inventory.a.DeactivateVMail(); vmailActive = false; return; }

			if (!GUIState.a.isBlocking) {
				currentButton = null; // Force this to reset.
				if (holdingObject) {
					if (!FrobWithHeldObject()) DropHeldItem();
				} else FrobEmptyHanded();
			} else {
				//We are holding cursor over the GUI
				if (holdingObject) {
					AddItemToInventory(heldObjectIndex);
					ResetHeldItem();
					ResetCursor();
				} else InventoryButtonUse();
			}
		}
	}

	void Mouselook(float dx, float dy) {
		if ((inCyberSpace && Const.a.InputInvertCyberspaceLook) || (!inCyberSpace && Const.a.InputInvertLook))
			xRotation += dy;
		else 
			xRotation -= dy;

		if (inCyberSpace) {
			xRotation = Clamp0360(xRotation);  // Limit up and down angle to within 360°.
			if(xRotation > 90f || xRotation < -90f && xRotation > -270f) // More flippideedoo shenanigans.
				yRotation -= dx;
			else
				yRotation += dx;

			 // Do the cyber mouselook.  Everybody now!  Dance the cyber mouselook.
			cyberLookDir = Vector3.Normalize (transform.forward);
		} else {
			xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle
			yRotation += dx;
		}

		// Apply the normal mouselook
		transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f); // left right component applied to capsule
		transform.localRotation = Quaternion.Euler(xRotation,0f,0f); // Up down component only applied to camera.  Must be 0 for others or else movement will go in wrong direction!
	}

	public void EnterCyberspace(GameObject entryPoint) {
		cyberspaceRecallPoint = entryPoint.transform.position;
		playerRadiationTreatmentFlash.SetActive(true);
		cyberspaceReturnPoint = PlayerMovement.a.transform.position;
		cyberspaceReturnCameraLocalRotation = transform.localRotation.eulerAngles;
		cyberspaceReturnPlayerCapsuleLocalRotation = transform.parent.transform.parent.transform.localRotation.eulerAngles;
		cyberspaceReturnLevel = LevelManager.a.currentLevel;
		MFDManager.a.EnterCyberspace();
		LevelManager.a.LoadLevel(13,entryPoint,cyberspaceRecallPoint);
		PlayerMovement.a.inCyberSpace = true;
		PlayerMovement.a.leanCapsuleCollider.enabled = false;
		hm.inCyberSpace = true;
		inCyberSpace = true;
		playerCamera.useOcclusionCulling = false;
		SetCameraCullDistances();
		Utils.PlayOneShotSavable(SFXSource,CyberSFX);
	}

	public void ExitCyberspace() {
		playerRadiationTreatmentFlash.SetActive(true);
		MFDManager.a.ExitCyberspace();
		LevelManager.a.LoadLevel(cyberspaceReturnLevel,null,cyberspaceReturnPoint);
		transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(cyberspaceReturnPlayerCapsuleLocalRotation.x, cyberspaceReturnPlayerCapsuleLocalRotation.y, cyberspaceReturnPlayerCapsuleLocalRotation.z); // left right component applied to capsule
		transform.localRotation = Quaternion.Euler(cyberspaceReturnCameraLocalRotation.x,cyberspaceReturnCameraLocalRotation.y,cyberspaceReturnCameraLocalRotation.z); // Up down component applied to camera
		PlayerMovement.a.inCyberSpace = false;
		PlayerMovement.a.rbody.velocity = Const.a.vectorZero;
		PlayerMovement.a.leanCapsuleCollider.enabled = true;
		hm.inCyberSpace = false;
		inCyberSpace = false;
		playerCamera.useOcclusionCulling = false;
		Const.a.decoyActive = false;
		Utils.PlayOneShotSavable(SFXSource,CyberSFX);
		SetCameraCullDistances();
	}

	// Draw line from cursor - used for projectile firing, e.g. magpulse/stugngun/railgun/plasma
	public void SetCameraFocusPoint() {
        Vector3 cursorPoint0 = new Vector3(MouseCursor.a.drawTexture.x + (MouseCursor.a.drawTexture.width * 0.5f), MouseCursor.a.drawTexture.y + (MouseCursor.a.drawTexture.height * 0.5f), 0f);
        cursorPoint0.y = Screen.height - cursorPoint0.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
        if (Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint0), out tempHit, Mathf.Infinity)) cameraFocusPoint = tempHit.point;
	}

	// Clamp cyberspace up/down look rotation to with in +/- 360f.
	float Clamp0360(float val) {
		return (val - (Mathf.CeilToInt(val*(1f/360f)) * 360f)); // Subtract out 360 times the number of times 360 fits within val.
	}

	public void SetCameraCullDistances() {
		if (inCyberSpace) {
			for (int i=0;i<32;i++) { cameraDistances[i] = 2400f; }
		} else {
			for (int i=0;i<32;i++) { cameraDistances[i] = 79f; } // Can't see further than this.  31 * 2.56 - player radius 0.48 = 78.88f rounded up to be careful..longest line of sight is the crawlway on level 6
			cameraDistances[15] = 2400f; // Sky is visible, only exception.
		}
		playerCamera.layerCullDistances = cameraDistances; // Cull anything beyond 79f except for sky layer.
	}

	void KeyboardTurn() {
		if (GetInput.a.TurnLeft()) {
			yRotation -= keyboardTurnSpeed;
			transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
		} else if (GetInput.a.TurnRight()) {
			yRotation += keyboardTurnSpeed;
			transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
		}
	}

	void KeyboardLookUpDn() {
		// Cyberspace...more like a plane so giving the option to invert it separately.
		if (GetInput.a.LookDown()) {
			if ((inCyberSpace && Const.a.InputInvertCyberspaceLook) || (!inCyberSpace && Const.a.InputInvertLook))
				xRotation -= keyboardTurnSpeed;
			else
				xRotation += keyboardTurnSpeed;

			if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle.
			transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
		} else if (GetInput.a.LookUp()) {
			if ((inCyberSpace && Const.a.InputInvertCyberspaceLook) || (!inCyberSpace && Const.a.InputInvertLook))
				xRotation += keyboardTurnSpeed;
			else
				xRotation -= keyboardTurnSpeed;

			if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle.
			transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
		}
	}

	bool RayOffset() {
		bool successfulRay = false;
		successfulRay = Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint), out tempHit,Const.a.frobDistance,Const.a.layerMaskPlayerFrob);
		Debug.DrawRay(playerCamera.ScreenPointToRay(cursorPoint).origin,playerCamera.ScreenPointToRay(cursorPoint).direction * Const.a.frobDistance, Color.green,1f,true);
		if (successfulRay) {
			successfulRay = (tempHit.collider != null);
			if (successfulRay) {
				successfulRay = (tempHit.collider.CompareTag("Usable") || tempHit.collider.CompareTag("Searchable") || tempHit.collider.CompareTag("NPC"));
			}
		}
		return successfulRay;
	}

	void FrobEmptyHanded() {
		RaycastHit firstHit;
		cursorPoint.x = MouseCursor.a.cursorPosition.x; //MouseCursor.a.drawTexture.x+(MouseCursor.a.drawTexture.width/2f);
		cursorPoint.y = Screen.height - MouseCursor.a.cursorPosition.y; //Screen.height - MouseCursor.a.drawTexture.y+(MouseCursor.a.drawTexture.height/2f); // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
		cursorPoint.z = 0f;
		float offset = Screen.height * 0.02f;
		bool successfulRay = Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint), out tempHit,Const.a.frobDistance,Const.a.layerMaskPlayerFrob); // Separate from below which uses different mask
		Debug.DrawRay(playerCamera.ScreenPointToRay(cursorPoint).origin,playerCamera.ScreenPointToRay(cursorPoint).direction * Const.a.frobDistance, Color.green,1f,true);
		firstHit = tempHit;
		// Success here means hit a useable something.
		// If a ray hits a wall or other unusable something, that's not success and print "Can't use <something>"
		if (successfulRay) {
			successfulRay = (tempHit.collider != null);
			if (successfulRay) {
				successfulRay = (tempHit.collider.CompareTag("Usable") || tempHit.collider.CompareTag("Searchable") || tempHit.collider.CompareTag("NPC"));
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
		// To kind of walk around the center point to hopefully minimize rays we try.
		// And tighten our lug nuts properly so the wheels don't fall off this thing.

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
		if (!successfulRay) { // Try up and to the left
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

		// Okay we've checked first center, then in a box patter of 8, surely we've hit something the player was reasonably aiming at by now.
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
						uhr.referenceUseHandler.Use(ud);
					} else {
						Debug.Log("BUG: Attempting to use a useable without a UseHandler or UseHandlerRelay!");
					}
				}
			} else if (tempHit.collider.CompareTag("Searchable")) { // Search
				currentSearchItem = tempHit.collider.gameObject;
				SearchObject(currentSearchItem.GetComponent<SearchableItem>().lookUpIndex);
			} else if (tempHit.collider.CompareTag("NPC")) { // Say we can't use enemy and give enemy name.
				AIController aic = tempHit.collider.gameObject.GetComponent<AIController>();
				if (aic != null) Const.sprint(Const.a.stringTable[29] + Const.a.nameForNPC[aic.index],player); // "Can't use <enemy>"
			} else {
				Const.sprint(Const.a.stringTable[29],player); // "Can't use "
			}
		} else { // Frobbed into empty space, so whatever it is is too far.
			if (tempHit.collider != null) UseNameSprint(tempHit.collider.gameObject); // This sprints the "Can't use <something>" text.
			else Const.sprint(Const.a.stringTable[30],player); // You are too far away from that
		}
	}

	bool UseNameSprint(GameObject go) {
		UseName un = go.GetComponent<UseName>();
		if (un == null) un = go.transform.parent.gameObject.GetComponent<UseName>(); // Ok, maybe the parent has it.
		if (un == null) un = go.GetComponentInChildren<UseName>(); // Ok...so maybe a child has UseName on it, find it in the children.
		if (un != null) {
			Const.sprint(Const.a.stringTable[29] + un.targetname,player); // "Can't use <blah blah blah>"
			return true;
		}

		// Fine we don't know what it is.
		Const.sprint(Const.a.stringTable[29],player); // "Can't use "
		return false; 
	}

	bool FrobWithHeldObject() {
		if (heldObjectIndex < 0) { Debug.Log("BUG: Attempting to frob with held object, but heldObjectIndex < 0."); return false; } // Invalid item will be dropped, wasn't used up.

		if (heldObjectIndex == 54 || heldObjectIndex == 56 || heldObjectIndex == 57 || heldObjectIndex == 61 || heldObjectIndex == 64) {
			cursorPoint.x = MouseCursor.a.drawTexture.x+(MouseCursor.a.drawTexture.width/2f);
			cursorPoint.y = Screen.height - MouseCursor.a.drawTexture.y+(MouseCursor.a.drawTexture.height/2f); // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
			if (Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint), out tempHit, Const.a.frobDistance)) {
				if (tempHit.collider.CompareTag("Usable")) {
					UseData ud = new UseData ();
					ud.owner = player;
					ud.mainIndex = heldObjectIndex;
					ud.customIndex = heldObjectCustomIndex;
					UseHandler uh = tempHit.collider.gameObject.GetComponent<UseHandler>();
					if (uh != null) {
						Utils.PlayOneShotSavable(SFXSource,SearchSFX);
						uh.Use(ud);
						return true; // Item can get absorbed and not dropped.
					} else {
						UseHandlerRelay uhr = tempHit.collider.gameObject.GetComponent<UseHandlerRelay>();
						if (uhr != null) {
							Utils.PlayOneShotSavable(SFXSource,SearchSFX);
							uhr.referenceUseHandler.Use(ud);
							return true; // Item can get absorbed and not dropped.
						} else {
							Debug.Log("BUG: Attempting to use a useable without a UseHandler or UseHandlerRelay!");
							return false;
						}
					}
				}
			}
		} // Cannot notify of attempt to frob with different index since this is how we normally drop items.
		return false; // Item will be dropped, wasn't used up.
	}

	void PutObjectInHand(int useableConstdex, int customIndex, int ammo1, int ammo2, Texture2D cursorTex, bool fromButton) {
		holdingObject = true;
		heldObjectIndex = useableConstdex;
		heldObjectCustomIndex = customIndex;
		heldObjectAmmo = ammo1;
		heldObjectAmmo2 = ammo2;
		cursorTexture = cursorTex;
		MouseCursor.a.cursorImage = cursorTexture;
		if (fromButton) GUIState.a.PtrHandler(false,false,ButtonType.None,null);
		ForceInventoryMode();
	}

	void InventoryButtonUse() {
		if (!GUIState.a.overButton) return;
		if (GUIState.a.overButtonType == ButtonType.None) return;
		if (currentButton == null) return;

		switch(GUIState.a.overButtonType) {
			case ButtonType.Weapon:
				// Take weapon out of inventory, removing weapon, remove weapon and any other strings I need to CTRL+F my way to this buggy code!
				WeaponButton wepbut = currentButton.GetComponent<WeaponButton>();
				int am1 = WeaponCurrent.a.currentMagazineAmount[wepbut.WepButtonIndex];
				int am2 = WeaponCurrent.a.currentMagazineAmount2[wepbut.WepButtonIndex];
				WeaponCurrent.a.currentMagazineAmount[wepbut.WepButtonIndex] = 0; // zero out the current ammo
				WeaponCurrent.a.currentMagazineAmount2[wepbut.WepButtonIndex] = 0; // zero out the current ammo
				Inventory.a.RemoveWeapon(wepbut.WepButtonIndex);
				WeaponCurrent.a.SetAllViewModelsDeactive();
				WeaponCurrent.a.weaponCurrent = -1;
				WeaponCurrent.a.weaponIndex = 0;
				wepbut.useableItemIndex = -1;
				MFDManager.a.wepbutMan.WeaponCycleDown();
				MFDManager.a.SendInfoToItemTab(WeaponCurrent.a.weaponIndexPending);
				MFDManager.a.OpenTab(0, true, TabMSG.Weapon, 0,Handedness.LH);
				PutObjectInHand(wepbut.useableItemIndex,-1,am1,am2,
								Const.a.useableItemsFrobIcons[wepbut.useableItemIndex], true);
				break;
			case ButtonType.Grenade:
				GrenadeButton grenbut = currentButton.GetComponent<GrenadeButton>();
				Inventory.a.grenAmmo[grenbut.GrenButtonIndex]--;
				Inventory.a.GrenadeCycleDown();
				//Inventory.a.grenadeCurrent = -1; This was up here, and seemed fine.  Might need to revert line 473 add.
				if (Inventory.a.grenAmmo[grenbut.GrenButtonIndex] <= 0) {
					Inventory.a.grenAmmo[grenbut.GrenButtonIndex] = 0;
					Inventory.a.grenadeCurrent = -1;
					for (int i = 0; i < 7; i++) {
						if (Inventory.a.grenAmmo[i] > 0) Inventory.a.grenadeCurrent = i;
					}
					MFDManager.a.SendInfoToItemTab(Inventory.a.grenadeCurrent);
					if (Inventory.a.grenadeCurrent < 0) Inventory.a.grenadeCurrent = 0;
				}
				PutObjectInHand(grenbut.useableItemIndex,-1,0,0,Const.a.useableItemsFrobIcons[grenbut.useableItemIndex],true);
				break;
			case ButtonType.Patch:
				PatchButton patbut = currentButton.GetComponent<PatchButton>();
				Inventory.a.patchCounts[patbut.PatchButtonIndex]--;
				if (Inventory.a.patchCounts[patbut.PatchButtonIndex] <= 0) {
					Inventory.a.patchCounts[patbut.PatchButtonIndex] = 0;
					Inventory.a.patchCurrent = -1;
					GUIState.a.PtrHandler(false,false,ButtonType.None,null);
					for (int i = 0; i < 7; i++) {
						if (Inventory.a.patchCounts[i] > 0) Inventory.a.patchCurrent = i;
					}
					MFDManager.a.SendInfoToItemTab(Inventory.a.patchCurrent);
					if (Inventory.a.patchCurrent < 0) Inventory.a.patchCurrent = 0;
				}
				PutObjectInHand(patbut.useableItemIndex,-1,0,0,Const.a.useableItemsFrobIcons[patbut.useableItemIndex],true);
				break;
			case ButtonType.GeneralInv:
				GeneralInvButton genbut = currentButton.GetComponent<GeneralInvButton>();
				Inventory.a.generalInventoryIndexRef[genbut.GeneralInvButtonIndex] = -1;
				Inventory.a.generalInvCurrent = -1;
				for (int i = 0; i < 7; i++) {
					if (Inventory.a.generalInventoryIndexRef[i] >= 0) Inventory.a.generalInvCurrent = i;
				}
				int referenceIndex = -1;
				if (Inventory.a.generalInvCurrent >= 0) referenceIndex = Inventory.a.genButtons[Inventory.a.generalInvCurrent].transform.GetComponent<GeneralInvButton>().useableItemIndex;
				if (referenceIndex < 0 || referenceIndex > 110) MFDManager.a.ResetItemTab();
				else MFDManager.a.SendInfoToItemTab(referenceIndex);
				PutObjectInHand(genbut.useableItemIndex,-1,0,0,Const.a.useableItemsFrobIcons[genbut.useableItemIndex],true);
				break;
			case ButtonType.Search:
				SearchButton sebut = currentButton.GetComponentInParent<SearchButton>();
				int tempButtonindex = currentButton.GetComponent<SearchContainerButton>().refIndex;
				cursorTexture = Const.a.useableItemsFrobIcons[sebut.contents[tempButtonindex]];
				holdingObject = true;
				heldObjectIndex = sebut.contents[tempButtonindex];
				heldObjectCustomIndex = sebut.customIndex[tempButtonindex];
				if (currentSearchItem != null) {
					currentSearchItem.GetComponent<SearchableItem>().contents[tempButtonindex] = -1;
					currentSearchItem.GetComponent<SearchableItem>().customIndex[tempButtonindex] = -1;
				}
				sebut.contents[tempButtonindex] = -1;
				sebut.customIndex[tempButtonindex] = -1;
				MFDManager.a.DisableSearchItemImage(tempButtonindex);
				sebut.CheckForEmpty();
				GUIState.a.PtrHandler(false,false,ButtonType.None,null);
				if (Const.a.InputQuickItemPickup) {
					AddItemToInventory(heldObjectIndex);
					ResetHeldItem();
					ResetCursor();
				} else {
					Const.sprint(Const.a.useableItemsNameText[heldObjectIndex] + Const.a.stringTable[319],player);
					MouseCursor.a.cursorImage = cursorTexture;
					ForceInventoryMode();
				}
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
		}
	}

	void RecoilAndRest() {
		float camz = Mathf.Lerp(transform.localPosition.z,0f,0.1f);
		Vector3 camPos = new Vector3(transform.localPosition.x,Const.a.playerCameraOffsetY*PlayerMovement.a.currentCrouchRatio,camz);
		transform.localPosition = camPos;
		if (shakeFinished > PauseScript.a.relativeTime) {
			float x = transform.localPosition.x + UnityEngine.Random.Range(shakeForce * -0.17f,shakeForce * 0.17f);
			float y = transform.localPosition.y + UnityEngine.Random.Range(shakeForce * -0.08f,shakeForce * 0.08f);
			float z = transform.localPosition.z + UnityEngine.Random.Range(shakeForce * -0.17f,shakeForce * 0.17f);
			transform.localPosition = new Vector3(x,y,z);
		} else {
			transform.localPosition = new Vector3(0.0f,0.84f,0.0f);
		}
	}

	void AddItemFail(int index) {
		DropHeldItem();
		Const.sprint(Const.a.stringTable[32] + Const.a.useableItemsNameText[index] + Const.a.stringTable[318],player); // Inventory full.
	}

	public void AddItemToInventory (int index) {
		if (index < 0) index = 0; // Good check on paper.
		if (index > 110) index = 94; // Way to get a head.

		AudioClip pickclip = PickupSFX;
		if ((index >= 0 && index <= 5)
             || index == 33
             || index == 35
             || (index >= 52 && index < 59)
             || (index >= 61 && index <= 64)
             || (index >= 92 && index <= 101)) {
			if (!Inventory.a.AddGeneralObjectToInventory(index)) {
				AddItemFail(index);
			}
		} else if (index == 6) {
			Inventory.a.AddAudioLogToInventory(heldObjectCustomIndex);
		} else if (index >= 36 && index <= 51) {
			if (!Inventory.a.AddWeaponToInventory(index,heldObjectAmmo,
												        heldObjectAmmo2)) {
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
				case 21: Inventory.a.AddHardwareToInventory(0,index); break;
				case 22: Inventory.a.AddHardwareToInventory(1,index); break;
				case 23: Inventory.a.AddHardwareToInventory(2,index); break;
				case 24: Inventory.a.AddHardwareToInventory(3,index); break;
				case 25: Inventory.a.AddHardwareToInventory(4,index); break;
				case 26: Inventory.a.AddHardwareToInventory(5,index); break;
				case 27: Inventory.a.AddHardwareToInventory(6,index); break;
				case 28: Inventory.a.AddHardwareToInventory(7,index); break;
				case 29: Inventory.a.AddHardwareToInventory(8,index); break;
				case 30: Inventory.a.AddHardwareToInventory(9,index); break;
				case 31: Inventory.a.AddHardwareToInventory(10,index); break;
				case 32: Inventory.a.AddHardwareToInventory(11,index); break;
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
		Utils.PlayOneShotSavable(SFXSource,pickclip);
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
				MFDManager.a.OpenTab(0, true, TabMSG.Weapon, 0,Handedness.LH);
				currentSearchItem = null;
			}
		}
		firstTimePickup = false;
	}

	public void DropHeldItem() {
		if (heldObjectIndex < 0 || heldObjectIndex > 110) { 
			Debug.Log("BUG: Attempted to DropHeldItem with index out of bounds (<0 or >110) and heldObjectIndex = " + heldObjectIndex.ToString(),player);
			ResetHeldItem();
			ResetCursor();
			return;
		}

		if (!grenadeActive) heldObject = Const.a.useableItems[heldObjectIndex]; // heldObject is set by UseGrenade() so don't override here.
		if (heldObject != null) {
			GameObject tossObject = null;
			bool freeObjectInPoolFound = false;
			GameObject levelDynamicContainer = LevelManager.a.GetCurrentDynamicContainer();

			// Find any free inactive objects within the level's Levelnumber.Dynamic container and activate those before instantiating
			if (!grenadeActive) {
				if (levelDynamicContainer != null) {
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
				}

				if (freeObjectInPoolFound) {
					if (tossObject == null) {
						Const.sprint("BUG: Failed to get freeObjectInPool for object being dropped!",player);
						ResetHeldItem();
						ResetCursor();
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
						ResetCursor();
						return;
					}
				}
				if (tossObject.activeSelf != true) tossObject.SetActive(true);
				if (levelDynamicContainer != null) {
					tossObject.transform.SetParent(levelDynamicContainer.transform,true);
				}

				Vector3 tossDir = new Vector3(MouseCursor.a.drawTexture.x+(MouseCursor.a.drawTexture.width/2),MouseCursor.a.drawTexture.y+(MouseCursor.a.drawTexture.height/2),0);
				tossDir.y = Screen.height - tossDir.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
				tossDir = playerCamera.ScreenPointToRay(tossDir).direction;
				Rigidbody rbody = tossObject.GetComponent<Rigidbody>();
				if (rbody != null) {
					rbody.isKinematic = false;
					rbody.useGravity = true;
					rbody.velocity = tossDir * tossForce;
				}
				tossObject.GetComponent<UseableObjectUse>().customIndex = heldObjectCustomIndex;
				tossObject.GetComponent<UseableObjectUse>().ammo = heldObjectAmmo;
				tossObject.GetComponent<UseableObjectUse>().ammo2 = heldObjectAmmo2;

			} else {
				// Throw an active grenade
				grenadeActive = false;
				tossObject = Instantiate(heldObject,(transform.position + (transform.forward * tossOffset)),Const.a.quaternionIdentity) as GameObject;  //effect
				if (tossObject == null) {
					Const.sprint("BUG: Failed to instantiate object being dropped!",player);
					ResetHeldItem();
					ResetCursor();
					return;
				}

				if (levelDynamicContainer != null){
					tossObject.transform.SetParent(levelDynamicContainer.transform,true);
				}
				tossObject.layer = 11; // Set to player bullets layer to prevent collision and still be visible.
				Vector3 tossDir = new Vector3(MouseCursor.a.drawTexture.x+(MouseCursor.a.drawTexture.width/2),MouseCursor.a.drawTexture.y+(MouseCursor.a.drawTexture.height/2),0);
				tossDir.y = Screen.height - tossDir.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
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
		} else {
			Const.sprint("BUG: Object "+heldObjectIndex.ToString()+" not assigned, vaporized.",player);
		}
		ResetHeldItem();
		ResetCursor();
	}

	public void ResetHeldItem() {
		heldObjectIndex = -1;
		heldObjectCustomIndex = -1;
		heldObjectAmmo = 0;
		heldObjectAmmo2 = 0;
		holdingObject = false;
		MouseCursor.a.justDroppedItemInHelper = true;
	}

	public void ResetCursor () {
        if (MouseCursor.a != null) {
            cursorTexture = cursorDefaultTexture;
            MouseCursor.a.cursorImage = cursorTexture;
			MouseCursor.a.liveGrenade = false;
        } else {
			Const.sprint("BUG: Could Not Find object 'MouseCursorHandler' in scene\n");
        }
	}

	public void ToggleInventoryMode (){
		if (inventoryMode)	ForceShootMode();
		else				ForceInventoryMode();
	}

	public void ForceShootMode() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		inventoryMode = false;
		shootModeButton.SetActive(false);
		if (vmailActive) {
			Inventory.a.DeactivateVMail();
			vmailActive = false;
		}
	}

	public void ForceInventoryMode() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = false;
		inventoryMode = true;
		shootModeButton.SetActive(true);
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
					curSearchScript.contents[i] = -1;
					curSearchScript.customIndex[i] = -1;
					if (heldObjectIndex != -1) holdingObject = true;
					Const.sprint(Const.a.useableItemsNameText[heldObjectIndex] + Const.a.stringTable[319],player);
					MFDManager.a.DisableSearchItemImage(i);
					useFX = false;
					break;
				}
			}
		} else {
			Utils.PlayOneShotSavable(SFXSource,SearchSFX); // Play search sound
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
		Const.sprint(Const.a.useableItemsNameText[index] + Const.a.stringTable[320],player);
		switch(index) { // Subtract one from the correct grenade inventory
			case 7: heldObject = Const.a.useableItems[63];  Inventory.a.RemoveGrenade(0); break; // Frag
			case 8: heldObject = Const.a.useableItems[65];  Inventory.a.RemoveGrenade(3); break; // Concussion
			case 9: heldObject = Const.a.useableItems[80];  Inventory.a.RemoveGrenade(1); break; // EMP
			case 10: heldObject = Const.a.useableItems[82]; Inventory.a.RemoveGrenade(6); break; // Earth Shaker
			case 11: heldObject = Const.a.useableItems[95]; Inventory.a.RemoveGrenade(4); break; // Land Mine
			case 12: heldObject = Const.a.useableItems[96]; Inventory.a.RemoveGrenade(5); break; // Nitropak
			case 13: heldObject = Const.a.useableItems[97]; Inventory.a.RemoveGrenade(2); break; // Gas
		}
		MFDManager.a.ResetItemTab();
		PutObjectInHand(index,-1,0,0,Const.a.useableItemsFrobIcons[index], true);
	}

	public void ScreenShake (float force) {
		shakeFinished = PauseScript.a.relativeTime + 1f;
		if (force < 0.48f) shakeForce = force;
		else shakeForce = 0.48f;
	}

	public static string Save(GameObject go) {
		MouseLookScript ml = go.GetComponent<MouseLookScript>();
		if (ml == null) {
			Debug.Log("MouseLook missing on savetype of Player!  GameObject.name: " + go.name);
			return "0|0|-1|-1|0|0|0|0|0|0|0000.00000|0|0000.00000|0|0000.00000|0000.00000|0000.00000|0000.00000|0000.00000|0000.00000|0000.00000|0000.00000|0000.00000|0000.00000|0000.00000|0000.00000|1";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(ml.inventoryMode); // bool
		line += Utils.splitChar + Utils.BoolToString(ml.holdingObject); // bool
		line += Utils.splitChar + ml.heldObjectIndex.ToString(); // int
		line += Utils.splitChar + ml.heldObjectCustomIndex.ToString(); // int
		line += Utils.splitChar + ml.heldObjectAmmo.ToString(); // int
		line += Utils.splitChar + ml.heldObjectAmmo2.ToString(); // int
		line += Utils.splitChar + Utils.BoolToString(ml.firstTimePickup); // bool
		line += Utils.splitChar + Utils.BoolToString(ml.firstTimeSearch); // bool
		line += Utils.splitChar + Utils.BoolToString(ml.grenadeActive); // bool
		line += Utils.splitChar + Utils.BoolToString(ml.inCyberSpace); // bool
		line += Utils.splitChar + Utils.FloatToString(ml.yRotation); // float
		line += Utils.splitChar + Utils.BoolToString(ml.geniusActive); // bool
		line += Utils.splitChar + Utils.FloatToString(ml.xRotation); // float
		line += Utils.splitChar + Utils.BoolToString(ml.vmailActive); // bool
		line += Utils.splitChar + Utils.FloatToString(ml.cyberspaceReturnPoint.x) + Utils.splitChar
						 	    + Utils.FloatToString(ml.cyberspaceReturnPoint.y) + Utils.splitChar
						  	    + Utils.FloatToString(ml.cyberspaceReturnPoint.z);
		line += Utils.splitChar + Utils.FloatToString(ml.cyberspaceReturnCameraLocalRotation.x) + Utils.splitChar
								+ Utils.FloatToString(ml.cyberspaceReturnCameraLocalRotation.y) + Utils.splitChar
						  		+ Utils.FloatToString(ml.cyberspaceReturnCameraLocalRotation.z);
		line += Utils.splitChar + Utils.FloatToString(ml.cyberspaceReturnPlayerCapsuleLocalRotation.x) + Utils.splitChar
						  		+ Utils.FloatToString(ml.cyberspaceReturnPlayerCapsuleLocalRotation.y) + Utils.splitChar
						  		+ Utils.FloatToString(ml.cyberspaceReturnPlayerCapsuleLocalRotation.z);
		line += Utils.splitChar + Utils.FloatToString(ml.cyberspaceRecallPoint.x) + Utils.splitChar
						  		+ Utils.FloatToString(ml.cyberspaceRecallPoint.y) + Utils.splitChar
						  		+ Utils.FloatToString(ml.cyberspaceRecallPoint.z); // Vector3 (float|float|float)
		line += Utils.splitChar + ml.cyberspaceReturnLevel.ToString(); // int
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		MouseLookScript ml = go.GetComponent<MouseLookScript>();
		if (ml == null || index < 0 || entries == null) return index + 27;

		float readFloatx, readFloaty, readFloatz;
		ml.inventoryMode = !Utils.GetBoolFromString(entries[index]); index++; // take opposite because we are about to opposite again
		ml.ToggleInventoryMode(); // correctly set cursor lock state, and opposite again, now it is what was saved
		ml.holdingObject = Utils.GetBoolFromString(entries[index]); index++;
		ml.heldObjectIndex = Utils.GetIntFromString(entries[index]); index++;
		ml.heldObjectCustomIndex = Utils.GetIntFromString(entries[index]); index++;
		ml.heldObjectAmmo = Utils.GetIntFromString(entries[index]); index++;
		ml.heldObjectAmmo2 = Utils.GetIntFromString(entries[index]); index++;
		ml.firstTimePickup = Utils.GetBoolFromString(entries[index]); index++;
		ml.firstTimeSearch = Utils.GetBoolFromString(entries[index]); index++;
		ml.grenadeActive = Utils.GetBoolFromString(entries[index]); index++;
		ml.inCyberSpace = Utils.GetBoolFromString(entries[index]); index++;
		ml.yRotation = Utils.GetFloatFromString(entries[index]); index++;
		ml.geniusActive = Utils.GetBoolFromString(entries[index]); index++;
		ml.xRotation = Utils.GetFloatFromString(entries[index]); index++;
		ml.vmailActive = Utils.GetBoolFromString(entries[index]); index++;
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		ml.cyberspaceReturnPoint = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		ml.cyberspaceReturnCameraLocalRotation = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		ml.cyberspaceReturnPlayerCapsuleLocalRotation = new Vector3(readFloatx,readFloaty,readFloatz); // Euler Angles, only 3
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		ml.cyberspaceRecallPoint = new Vector3(readFloatx,readFloaty,readFloatz);
		ml.cyberspaceReturnLevel = Utils.GetIntFromString(entries[index]); index++;
		ml.currentSearchItem = null; // Prevent picking up first item immediately. Not currently possible to save references.
		ml.transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(0f, ml.yRotation, 0f); // left right component applied to capsule
		ml.transform.localRotation = Quaternion.Euler(ml.xRotation,0f,0f); // Up down component only applied to camera
		return index;
	}
}
