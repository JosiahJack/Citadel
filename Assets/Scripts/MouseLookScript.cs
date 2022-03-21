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
	public MouseCursor mouseCursor;
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
	public WeaponMagazineCounter wepmagCounter;
	public PlayerMovement playerMovement;
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

    void Start (){
        ResetCursor();
		ResetHeldItem();
		Cursor.lockState = CursorLockMode.None;
		inventoryMode = true; // Start with inventory mode turned on.
		shootModeButton.SetActive(true);
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
		if (Input.GetKeyUp(f9)) Const.a.Load(7); // Allow quick load straight from the menu.

        if (PauseScript.a.MenuActive()) { if (playerCamera.enabled) playerCamera.enabled = false; return; } // Ignore mouselook and turn off camera when main menu is up.
		else 							  if (!playerCamera.enabled) playerCamera.enabled = true;

		if (PauseScript.a.Paused()) return;
		if (playerMovement.ressurectingFinished > PauseScript.a.relativeTime) return;

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
					ResetHeldItem();
					ResetCursor();
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
		transform.localRotation = Quaternion.Euler(xRotation,0f,0f); // Up down component only applied to camera
	}

	public void EnterCyberspace(GameObject entryPoint) {
		cyberspaceRecallPoint = entryPoint.transform.position;
		playerRadiationTreatmentFlash.SetActive(true);
		cyberspaceReturnPoint = playerMovement.transform.position;
		cyberspaceReturnCameraLocalRotation = transform.localRotation.eulerAngles;
		cyberspaceReturnPlayerCapsuleLocalRotation = transform.parent.transform.parent.transform.localRotation.eulerAngles;
		cyberspaceReturnLevel = LevelManager.a.currentLevel;
		MFDManager.a.EnterCyberspace();
		LevelManager.a.LoadLevel (13,entryPoint,player,cyberspaceRecallPoint);
		playerMovement.inCyberSpace = true;
		playerMovement.leanCapsuleCollider.enabled = false;
		hm.inCyberSpace = true;
		inCyberSpace = true;
		playerCamera.useOcclusionCulling = false;
		SetCameraCullDistances();
		SFXSource.PlayOneShot(CyberSFX);
	}

	public void ExitCyberspace() {
		playerRadiationTreatmentFlash.SetActive(true);
		MFDManager.a.ExitCyberspace();
		LevelManager.a.LoadLevel(cyberspaceReturnLevel,null,player,cyberspaceReturnPoint);
		transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(cyberspaceReturnPlayerCapsuleLocalRotation.x, cyberspaceReturnPlayerCapsuleLocalRotation.y, cyberspaceReturnPlayerCapsuleLocalRotation.z); // left right component applied to capsule
		transform.localRotation = Quaternion.Euler(cyberspaceReturnCameraLocalRotation.x,cyberspaceReturnCameraLocalRotation.y,cyberspaceReturnCameraLocalRotation.z); // Up down component applied to camera
		playerMovement.inCyberSpace = false;
		playerMovement.rbody.velocity = Const.a.vectorZero;
		playerMovement.leanCapsuleCollider.enabled = true;
		hm.inCyberSpace = false;
		inCyberSpace = false;
		playerCamera.useOcclusionCulling = false;
		Const.a.decoyActive = false;
		SFXSource.PlayOneShot(CyberSFX);
		SetCameraCullDistances();
	}

	// Draw line from cursor - used for projectile firing, e.g. magpulse/stugngun/railgun/plasma
	public void SetCameraFocusPoint() {
        Vector3 cursorPoint0 = new Vector3(MouseCursor.drawTexture.x + (MouseCursor.drawTexture.width * 0.5f), MouseCursor.drawTexture.y + (MouseCursor.drawTexture.height * 0.5f), 0f);
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
		cursorPoint.x = MouseCursor.cursorPosition.x; //MouseCursor.drawTexture.x+(MouseCursor.drawTexture.width/2f);
		cursorPoint.y = Screen.height - MouseCursor.cursorPosition.y; //Screen.height - MouseCursor.drawTexture.y+(MouseCursor.drawTexture.height/2f); // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
		cursorPoint.z = 0f;
		float offset = Screen.height * 0.02f;
		bool successfulRay = Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint), out tempHit,Const.a.frobDistance,Const.a.layerMaskPlayerFrob); // Separate from below which uses different mask
		Debug.DrawRay(playerCamera.ScreenPointToRay(cursorPoint).origin,playerCamera.ScreenPointToRay(cursorPoint).direction * Const.a.frobDistance, Color.green,1f,true);
		if (successfulRay) {
			successfulRay = (tempHit.collider != null);
			if (successfulRay) {
				successfulRay = (tempHit.collider.CompareTag("Usable") || tempHit.collider.CompareTag("Searchable") || tempHit.collider.CompareTag("NPC"));
			}
		}

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
				if (aic != null) Const.sprint(Const.a.stringTable[29] + Const.a.nameForNPC[aic.index],player); // "Can not use <enemy>"
			} else {
				Const.sprint(Const.a.stringTable[29],player); // "Can not use "
			}
		} else { // Frobbed into empty space, so whatever it is is too far.
			if (tempHit.collider != null) {
				if (tempHit.collider.CompareTag("Geometry")) { // Give info about what we are looking at, e.g. "Molybdenum panelling".
					UseName un = tempHit.collider.gameObject.GetComponent<UseName> ();
					if (un != null) Const.sprint(Const.a.stringTable[29] + un.targetname,player); // "Can not use <blah blah blah>"
				}
			}
			Const.sprint(Const.a.stringTable[30],player); // You are too far away from that
		}
	}

	bool FrobWithHeldObject() {
		if (heldObjectIndex < 0) { Debug.Log("BUG: Attempting to frob with held object, but heldObjectIndex < 0."); return false; } // Invalid item will be dropped, wasn't used up.

		if (heldObjectIndex == 54 || heldObjectIndex == 56 || heldObjectIndex == 57 || heldObjectIndex == 61 || heldObjectIndex == 64) {
			cursorPoint.x = MouseCursor.drawTexture.x+(MouseCursor.drawTexture.width/2f);
			cursorPoint.y = Screen.height - MouseCursor.drawTexture.y+(MouseCursor.drawTexture.height/2f); // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
			if (Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint), out tempHit, Const.a.frobDistance)) {
				if (tempHit.collider.CompareTag("Usable")) {
					UseData ud = new UseData ();
					ud.owner = player;
					ud.mainIndex = heldObjectIndex;
					ud.customIndex = heldObjectCustomIndex;
					UseHandler uh = tempHit.collider.gameObject.GetComponent<UseHandler>();
					if (uh != null) {
						SFXSource.PlayOneShot(SearchSFX);
						uh.Use(ud);
						return true; // Item can get absorbed and not dropped.
					} else {
						UseHandlerRelay uhr = tempHit.collider.gameObject.GetComponent<UseHandlerRelay>();
						if (uhr != null) {
							SFXSource.PlayOneShot(SearchSFX);
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

	void InventoryButtonUse() {
		if (!GUIState.a.overButton) return;
		if (GUIState.a.overButtonType == GUIState.ButtonType.None) return;

		// overButtonTypes:
		//-1 Not over a button
		// 0 WeaponButton
		// 1 GrenadeButton
		// 2 PatchButton
		// 3 GeneralInventoryButton
		// 4 Search contents button
		switch(GUIState.a.overButtonType) {
			case GUIState.ButtonType.Weapon:
				// Take weapon out of inventory, removing weapon, remove weapon and any other strings I need to CTRL+F my way to this buggy code!
				WeaponButton wepbut = currentButton.GetComponent<WeaponButton>();
				cursorTexture = Const.a.useableItemsFrobIcons[wepbut.useableItemIndex];
				holdingObject = true;
				heldObjectIndex = wepbut.useableItemIndex;
				heldObjectCustomIndex = -1;
				heldObjectAmmo = WeaponCurrent.WepInstance.currentMagazineAmount[wepbut.WepButtonIndex];
				heldObjectAmmo2 = WeaponCurrent.WepInstance.currentMagazineAmount2[wepbut.WepButtonIndex];
				WeaponCurrent.WepInstance.currentMagazineAmount[wepbut.WepButtonIndex] = 0; // zero out the current ammo
				WeaponCurrent.WepInstance.currentMagazineAmount2[wepbut.WepButtonIndex] = 0; // zero out the current ammo
				Inventory.a.RemoveWeapon(wepbut.WepButtonIndex);
				WeaponCurrent.WepInstance.SetAllViewModelsDeactive();
				WeaponCurrent.WepInstance.weaponCurrent = -1;
				WeaponCurrent.WepInstance.weaponIndex = 0;
				wepbut.useableItemIndex = -1;
				MFDManager.a.wepbutMan.WeaponCycleDown();
				MFDManager.a.OpenTab (0, true, MFDManager.TabMSG.Weapon, 0,Handedness.LH);
				GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
				mouseCursor.cursorImage = cursorTexture;
				ForceInventoryMode();
				break;
			case GUIState.ButtonType.Grenade:
				GrenadeButton grenbut = currentButton.GetComponent<GrenadeButton>();
				cursorTexture = Const.a.useableItemsFrobIcons[grenbut.useableItemIndex];
				holdingObject = true;
				heldObjectIndex = grenbut.useableItemIndex;
				Inventory.a.grenAmmo[grenbut.GrenButtonIndex]--;
				Inventory.a.GrenadeCycleDown();
				Inventory.a.grenadeCurrent = 0;
				if (Inventory.a.grenAmmo[grenbut.GrenButtonIndex] <= 0) {
					Inventory.a.grenAmmo[grenbut.GrenButtonIndex] = 0; 
					for (int i = 0; i < 7; i++) {
						if (Inventory.a.grenAmmo[i] > 0) Inventory.a.grenadeCurrent = i;
					}
				}
				mouseCursor.cursorImage = cursorTexture;
				ForceInventoryMode();
				break;
			case GUIState.ButtonType.Patch:
				PatchButton patbut = currentButton.GetComponent<PatchButton>();
				cursorTexture = Const.a.useableItemsFrobIcons[patbut.useableItemIndex];
				holdingObject = true;
				heldObjectIndex = patbut.useableItemIndex;
				Inventory.a.patchCounts[patbut.PatchButtonIndex]--;
				if (Inventory.a.patchCounts[patbut.PatchButtonIndex] <= 0) {
					Inventory.a.patchCounts[patbut.PatchButtonIndex] = 0;
					GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
					for (int i = 0; i < 7; i++) {
						if (Inventory.a.patchCounts[i] > 0) Inventory.a.patchCurrent = i;
					}
				}
				mouseCursor.cursorImage = cursorTexture;
				ForceInventoryMode();
				break;
			case GUIState.ButtonType.GeneralInv:
				if (currentButton == null) {
					Debug.Log("BUG:: MouseLookScript: currentButton null when trying to right click and over ButtonType.GeneralInv");
				} else {
					GeneralInvButton genbut = currentButton.GetComponent<GeneralInvButton>();
					cursorTexture = Const.a.useableItemsFrobIcons[genbut.useableItemIndex];
					holdingObject = true;
					heldObjectIndex = genbut.useableItemIndex;
					Inventory.a.generalInventoryIndexRef[genbut.GeneralInvButtonIndex] = -1;
				}
				mouseCursor.cursorImage = cursorTexture;
				ForceInventoryMode();
				break;
			case GUIState.ButtonType.Search:
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
				GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
				if (Const.a.InputQuickItemPickup) {
					AddItemToInventory(heldObjectIndex);
					ResetHeldItem();
					ResetCursor();
				} else {
					Const.sprint(Const.a.useableItemsNameText[heldObjectIndex] + Const.a.stringTable[319],player);
					mouseCursor.cursorImage = cursorTexture;
					ForceInventoryMode();
				}
				break;
			case GUIState.ButtonType.PGrid:
				PuzzleUIButton puib = currentButton.GetComponent<PuzzleUIButton>();
				if (puib != null) puzzleGrid.OnGridCellClick(puib.buttonIndex);
				break;
			case GUIState.ButtonType.PWire:
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
		Vector3 camPos = new Vector3(transform.localPosition.x,Const.a.playerCameraOffsetY*playerMovement.currentCrouchRatio,camz);
		transform.localPosition = camPos;
		if (shakeFinished > PauseScript.a.relativeTime) transform.localPosition = new Vector3(transform.localPosition.x + UnityEngine.Random.Range(shakeForce * -1f,shakeForce),transform.localPosition.y + UnityEngine.Random.Range(shakeForce * -1f,shakeForce),transform.localPosition.z + UnityEngine.Random.Range(shakeForce * -1f,shakeForce));
	}

	void AddItemFail(int index) {
		DropHeldItem();
		ResetHeldItem();
		ResetCursor();
		Const.sprint(Const.a.stringTable[32] + Const.a.useableItemsNameText[index] + Const.a.stringTable[318],player); // Inventory full.
	}

	public void AddItemToInventory (int index) {
		AudioClip pickclip;
		pickclip = PickupSFX;
		if ((index >= 0 && index <= 5) || index == 33 || index == 35 || (index >= 52 && index < 59) || (index >= 61 && index <= 64) || (index >= 92 && index <= 101)) { if (!Inventory.a.AddGenericObjectToInventory(index)) AddItemFail(index); }
		else if (index == 6) Inventory.a.AddAudioLogToInventory(heldObjectCustomIndex);
		else if (index >= 36 && index <= 51) { if (!Inventory.a.AddWeaponToInventory(index,heldObjectAmmo,heldObjectAmmo2)) AddItemFail(index); }
		else if (index == 81 || (index >= 83 && index <= 91)) Inventory.a.AddAccessCardToInventory(index);
		else {
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
				case 34: Inventory.a.AddAccessCardToInventory(34); break;
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
		SFXSource.PlayOneShot(pickclip);
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
		}
		if (numberFoundContents == 0) MFDManager.a.OpenTab (0, true, MFDManager.TabMSG.Weapon, 0,Handedness.LH);
		firstTimePickup = false;
	}

	public void DropHeldItem() {
		if (heldObjectIndex < 0 || heldObjectIndex > 255) { Debug.Log("BUG: Attempted to DropHeldItem with index out of bounds (<0 or >255) and heldObjectIndex = " + heldObjectIndex.ToString(),player); return; }

		if (!grenadeActive) heldObject = Const.a.useableItems[heldObjectIndex]; // set by UseGrenade()
		if (heldObject != null) {
			GameObject tossObject = null;
			bool freeObjectInPoolFound = false;
			GameObject levelDynamicContainer = LevelManager.a.GetCurrentLevelDynamicContainer();

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
						return;
					} else {
						tossObject.transform.position = (transform.position + (transform.forward * tossOffset));
					}
				} else {
					// Debug.Log("WARNING: Failed to get freeObjectInPool for object " + heldObject.ToString() + "being dropped! MouseLookScript DropHeldItem.",player);
					tossObject = Instantiate(heldObject,(transform.position + (transform.forward * tossOffset)),Const.a.quaternionIdentity) as GameObject;  //effect
					if (tossObject == null) {
						Const.sprint("BUG: Failed to instantiate object being dropped!",player);
						return;
					} else {
						SaveObject so = tossObject.GetComponent<SaveObject>();
						if (so != null) {
							so.instantiated = true;
							so.constLookupTable = 0; // Standard useableItems table.
							so.constLookupIndex = heldObjectIndex;
						}
					}
				}
				if (tossObject.activeSelf != true) tossObject.SetActive(true);
				if (levelDynamicContainer != null) {
					tossObject.transform.SetParent(levelDynamicContainer.transform,true);
					SaveObject so = tossObject.GetComponent<SaveObject>();
					if (so != null) so.levelParentID = LevelManager.a.currentLevel;
				}

				Vector3 tossDir = new Vector3(MouseCursor.drawTexture.x+(MouseCursor.drawTexture.width/2),MouseCursor.drawTexture.y+(MouseCursor.drawTexture.height/2),0);
				tossDir.y = Screen.height - tossDir.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
				tossDir = playerCamera.ScreenPointToRay(tossDir).direction;
				tossObject.GetComponent<Rigidbody>().velocity = tossDir * tossForce;
				tossObject.GetComponent<UseableObjectUse>().customIndex = heldObjectCustomIndex;
				tossObject.GetComponent<UseableObjectUse>().ammo = heldObjectAmmo;
				tossObject.GetComponent<UseableObjectUse>().ammo2 = heldObjectAmmo2;
			} else {
				// Throw an active grenade
				grenadeActive = false;
				tossObject = Instantiate(heldObject,(transform.position + (transform.forward * tossOffset)),Const.a.quaternionIdentity) as GameObject;  //effect
				if (tossObject == null) { Const.sprint("BUG: Failed to instantiate object being dropped!",player); return; }

				if (levelDynamicContainer != null){
					tossObject.transform.SetParent(levelDynamicContainer.transform,true);
					SaveObject so = tossObject.GetComponent<SaveObject>();
					if (so != null) so.levelParentID = LevelManager.a.currentLevel;
				}
				tossObject.layer = 11; // Set to player bullets layer to prevent collision and still be visible.
				Vector3 tossDir = new Vector3(MouseCursor.drawTexture.x+(MouseCursor.drawTexture.width/2),MouseCursor.drawTexture.y+(MouseCursor.drawTexture.height/2),0);
				tossDir.y = Screen.height - tossDir.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
				tossDir = playerCamera.ScreenPointToRay(tossDir).direction;
				tossObject.GetComponent<Rigidbody>().velocity = tossDir * tossForce;
				GrenadeActivate ga = tossObject.GetComponent<GrenadeActivate>();
				if (ga != null) ga.Activate(heldObjectIndex); // Time to boom!
				mouseCursor.liveGrenade = false;
			}
		} else {
			Const.sprint("BUG: Object "+heldObjectIndex.ToString()+" not assigned, vaporized.",player);
		}
	}

	public void ResetHeldItem() {
		heldObjectIndex = -1;
		heldObjectCustomIndex = -1;
		heldObjectAmmo = 0;
		heldObjectAmmo2 = 0;
		holdingObject = false;
		mouseCursor.justDroppedItemInHelper = true;
	}

	public void ResetCursor () {
        if (mouseCursor != null) {
            cursorTexture = cursorDefaultTexture;
            mouseCursor.cursorImage = cursorTexture;
			mouseCursor.liveGrenade = false;
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
					mouseCursor.GetComponent<MouseCursor>().cursorImage = Const.a.useableItemsFrobIcons[curSearchScript.contents[i]];
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
			SFXSource.PlayOneShot(SearchSFX); // Play search sound
		}
		curSearchScript.searchableInUse = true;
		curSearchScript.currentPlayerCapsule = transform.parent.gameObject;  // Get playerCapsule of player this is attached to

		// Search through array to see if any items are in the container
		int numberFoundContents = 0;
		int[] resultContents = {-1,-1,-1,-1};  // create blanked container for search results
		int[] resultCustomIndex = {-1,-1,-1,-1};  // create blanked container for search results custom indices
		for (int i=3;i>=0;i--) {
			resultContents[i] = curSearchScript.contents[i];
			resultCustomIndex[i] = curSearchScript.customIndex[i];
			if (resultContents[i] > -1) numberFoundContents++; // if something was found, add 1 to count
		}

		if (firstTimeSearch) {
			firstTimeSearch = false;
			MFDManager.a.OpenTab (4, true, MFDManager.TabMSG.Search, -1,Handedness.LH);
		}
		MFDManager.a.SendSearchToDataTab(curSearchScript.objectName, numberFoundContents, resultContents, resultCustomIndex,currentSearchItem.transform.position,curSearchScript, useFX);
		ForceInventoryMode();
	}

	public void UseGrenade (int index) {
		if (holdingObject) { Const.sprint(Const.a.stringTable[311],player); return; } // Can't use grenade, hands full
		if (index < 7 || index > 13) { Debug.Log("BUG: index outside of 7 to 13 passed to UseGrenade() in MouseLookScript.cs"); return; }

		ForceInventoryMode();  // Inventory mode is turned on when picking something up.
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
		ResetHeldItem();
		holdingObject = true;
		heldObjectIndex = index;
		mouseCursor.cursorImage = Const.a.useableItemsFrobIcons[index];
		mouseCursor.liveGrenade = true;
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
	}

	public void ScreenShake (float force) {
		shakeFinished = PauseScript.a.relativeTime + 1f;
		if (force < 0.48f) shakeForce = force;
		else shakeForce = 0.48f;
	}
}
