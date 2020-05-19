using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MouseLookScript : MonoBehaviour {
    // Internal to Prefab
    // ------------------------------------------------------------------------
	public GameObject player;
    [Tooltip("Shows current state of Inventory Mode (Don't set yourself!)")]
	public bool inventoryMode;
	[Tooltip("Shows current state of Holding an Object (Don't set yourself!)")]
	public bool holdingObject;
	[Tooltip("The current cursor texture (For Reference or Testing)")]
	public Texture2D cursorTexture;
    [Tooltip("The default cursor texture (Developer sets default)")]
    public Texture2D cursorDefaultTexture;
    [HideInInspector]
    public Vector2 cursorHotspot;
    [Tooltip("Mouselook sensitivity (Developer sets default)")]
    public float lookSensitivity = 5;
    [Tooltip("Sound effect to play when searching an object")]
    public AudioClip SearchSFX;
    [Tooltip("Sound effect to play when picking-up/frobbing an object")]
    public AudioClip PickupSFX;
    [Tooltip("Sound effect to play when picking-up/frobbing a hardware item")]
    public AudioClip hwPickupSFX;
    [Tooltip("Distance from player origin to spawn objects when tossing them")]
    public float tossOffset = 0.10f;
    [Tooltip("Force given to spawned objects when tossing them")]
    public float tossForce = 200f;
    //[HideInInspector]
    public int heldObjectIndex; // save
	public int heldObjectCustomIndex; // save
	public int heldObjectAmmo; // save
	public int heldObjectAmmo2; // save
	public bool firstTimePickup;
	public bool firstTimeSearch;
	public bool grenadeActive;
	public bool inCyberSpace;
    [HideInInspector]
    public float yRotation;
	[Tooltip("Initial camera x rotation")]
	public float startxRotation = 0f;
	[Tooltip("Initial camera y rotation")]
	public float startyRotation = 0f;
	[Tooltip("Indicates whether genius patch is active for reversing LH & RH")]
	public bool geniusActive;
	[Tooltip("How far player can reach to use, pickup, search, etc. objects")]
	public float frobDistance = 4.5f;
	[Tooltip("Speed multiplier for turning the view with the keyboard")]
	public float keyboardTurnSpeed = 1.5f;
    public float xRotation;
	[HideInInspector]
	public Vector3 cyberLookDir;
    public Vector3 cameraFocusPoint;
	private int tempindex;
    private float zRotation;
    private float yRotationV;
    private float xRotationV;
    private float zRotationV;
    private float currentZRotation;
    private string mlookstring1;
    private Camera playerCamera;
	public Camera gunCamera;
    private GameObject heldObject;
    private bool itemAdded = false;
	private Quaternion tempQuat;
	private Vector3 tempVec;
	//private Quaternion cameraDefaultLocalRot;
	private Vector3 cameraRecoilLerpPos;
	[SerializeField] private bool recoiling;
	private Door.accessCardType doorAccessTypeAcquired;

    // External to Prefab
    // ------------------------------------------------------------------------
	public MouseCursor mouseCursor;
	public GameObject canvasContainer;
	public GameObject compassContainer;
	public GameObject automapContainer;
	public GameObject compassMidpoints;
	public GameObject compassLargeTicks;
	public GameObject compassSmallTicks;
    [Tooltip("Game object that houses the MFD tabs")]
	public GameObject tabControl;
	[Tooltip("Text in the data tab in the MFD that displays when searching an object containing no items")]
	public Text dataTabNoItemsText;
	public LogContentsButtonsManager logContentsManager;
	public AudioSource SFXSource;
	public CenterTabButtons centerTabButtonsControl;
	public GameObject iconman;
	public GameObject itemiconman;
	public GameObject itemtextman;
	public GameObject ammoiconman;
	public GameObject weptextman;
	public GameObject ammoClipBox;
	public GrenadeCurrent grenadeCurrent;
	public GrenadeInventory playerGrenInv;
	[HideInInspector]
	public GameObject currentButton;
	[HideInInspector]
	public GameObject currentSearchItem;
	[HideInInspector]
	//public GameObject mouseCursor;
    public GameObject weaponButtonsManager;
    public GameObject mainInventory;
	[HideInInspector]
	public LogInventory logInventory;
	public AccessCardInventory accessCardInventory;
	public GameObject[] hardwareButtons;
	public WeaponMagazineCounter wepmagCounter;
	public PlayerMovement playerMovement;
	private float[] cameraDistances;
	[HideInInspector]
	public bool vmailActive = false;
	public PuzzleWire puzzleWire;
	public PuzzleGrid puzzleGrid;
	//private AudioListener audListener;

    //float headbobSpeed = 1;
    //float headbobStepCounter;
    //float headbobAmountX = 1;
    //float headbobAmountY = 1;
    //Vector3 parentLastPos;
    //float eyeHeightRatio = 0.9f;

    //void  Awake (){
    //parentLastPos = transform.parent.position;
    //}

    void Start (){
        ResetCursor();
		Cursor.lockState = CursorLockMode.None;
		inventoryMode = true;  // Start with inventory mode turned on
		playerCamera = GetComponent<Camera>();
		cameraDistances = new float[32];
		for (int i=0;i<32;i++) {
			cameraDistances[i] = 79f; //can't see further than this please.  31 * 2.56 - player radius 0.48 = 78.88f rounded up to be careful..longest line of sight is the crawlway on level 6
		}
		cameraDistances[15] = 2400f; // sky is visible
		playerCamera.layerCullDistances = cameraDistances; // cull anything beyond 79f except for sky layer
		//audListener = GetComponent<AudioListener>();
		frobDistance = Const.a.frobDistance;
		holdingObject = false;
		heldObjectIndex = -1;
		heldObjectAmmo = 0;
		heldObjectAmmo2 = 0;
		grenadeActive = false;
		yRotation = startyRotation;
		xRotation = startxRotation;
		cyberLookDir = Vector3.zero;
		logInventory = mainInventory.GetComponent<LogInventory>();

		if (canvasContainer == null)
			Const.sprint("BUG: No canvas given for camera to display UI",player);

		canvasContainer.SetActive(true); //enable UI
		//cameraDefaultLocalPos = transform.localPosition;
		//cameraDefaultLocalRot = transform.localRotation;
		recoiling = false;
		firstTimePickup = true;
		firstTimeSearch = true;
    }

	public void Recoil (int i) {
		float strength = Const.a.recoilForWeapon[i];
		//Debug.Log("Recoil from gun index: "+i.ToString()+" with strength of " +strength.ToString());
		if (strength <= 0f) return;
		if (playerMovement.fatigue > 80) strength = strength * 2f;
		//Vector3 cameraJoltPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, (transform.localPosition.z - strength));
		//transform.localPosition = cameraJoltPosition;
		//transform.rotation = Quaternion.Euler(transform.rotation.x - strength,0,0);
		recoiling = true;
	}

	public void SetCameraFocusPoint() {
        // Draw line from cursor - used for projectile firing, e.g. magpulse/stugngun/railgun/plasma
        RaycastHit rayhit = new RaycastHit();
        Vector3 cursorPoint0 = new Vector3(MouseCursor.drawTexture.x + (MouseCursor.drawTexture.width / 2), MouseCursor.drawTexture.y + (MouseCursor.drawTexture.height / 2), 0);
        cursorPoint0.y = Screen.height - cursorPoint0.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
        if (Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint0), out rayhit, Mathf.Infinity)) {
            cameraFocusPoint = rayhit.point;
            //drawMyLine(playerCamera.transform.position, rayhit.point, Color.red, .1f);
        }
	}

	void Update () {
        Cursor.visible = false; // Hides hardware cursor so we can show custom cursor textures

		if (recoiling && !inCyberSpace) {
			//float x = transform.localPosition.x; // side to side
			//float y = transform.localPosition.y; // up and down
			//float z = transform.localPosition.z; // forward and back
			//z = Mathf.Lerp(z,cameraDefaultLocalPos.z,Time.deltaTime);
			//tempVec = new Vector3(x,y,z);
			//transform.localPosition = tempVec;
		}
		//Debug.Log("MouseLookScript:: Input.mousePosition.x: " + Input.mousePosition.x.ToString() + ", Input.mousePosition.y: " + Input.mousePosition.y.ToString());

        //if (transform.parent.GetComponent<PlayerMovement>().grounded)
        //headbobStepCounter += (Vector3.Distance(parentLastPos, transform.parent.position) * headbobSpeed);

        //transform.localPosition.x = (Mathf.Sin(headbobStepCounter) * headbobAmountX);
        //transform.localPosition.y = (Mathf.Cos(headbobStepCounter * 2) * headbobAmountY * -1) + (transform.localScale.y * eyeHeightRatio) - (transform.localScale.y / 2);
        //parentLastPos = transform.parent.position;

		// Spring Back to Rest from Recoil
		float camz = Mathf.Lerp(transform.localPosition.z,0f,0.1f);
		Vector3 camPos = new Vector3(0f,Const.a.playerCameraOffsetY*playerMovement.currentCrouchRatio,camz);
		transform.localPosition = camPos;

        if (PauseScript.a.mainMenu.activeSelf == true) {
			if (playerCamera.enabled) playerCamera.enabled = false;
			if (gunCamera.enabled) gunCamera.enabled = false;
			// for (int i=0;i<32;i++) {
				// cameraDistances[i] = 2f;
			// }
			// playerCamera.layerCullDistances = cameraDistances; // cull anything beyond 50f except for sky layer
			//cameraDistances[15] = 2400f; // sky is visible
			return;  // ignore mouselook when main menu is still up
		} else {
			if (!playerCamera.enabled) playerCamera.enabled = true;
			if (!gunCamera.enabled) gunCamera.enabled = true;
			for (int i=0;i<32;i++) {
				cameraDistances[i] = 79f;
			}
			cameraDistances[15] = 2400f; // sky is visible
			playerCamera.layerCullDistances = cameraDistances; // cull anything beyond 50f except for sky layer
		}

		if (Input.GetKeyUp("f6")) {
			Const.a.StartSave(7,"quicksave");
		}

		if (Input.GetKeyUp("f9")) {
			Const.a.Load(7);
		}

        if (inventoryMode == false) {
			if (!PauseScript.a.Paused() && playerMovement.ressurectingFinished < PauseScript.a.relativeTime) {
				float dx = Input.GetAxisRaw("Mouse X");
				float dy = Input.GetAxisRaw("Mouse Y");
				yRotation += (dx * lookSensitivity);
				if (inCyberSpace) {
					if (Const.a.InputInvertCyberspaceLook) {
						xRotation += (dy * lookSensitivity);
					} else {
						xRotation -= (dy * lookSensitivity);
					}
				} else {
					if (Const.a.InputInvertLook) {
						xRotation += (dy * lookSensitivity);
					} else {
						xRotation -= (dy * lookSensitivity);
					}
				}
				if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle
				transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f); // left right component applied to capsule
				transform.localRotation = Quaternion.Euler(xRotation,0f,0f); // Up down component only applied to camera

				if (inCyberSpace) cyberLookDir = Vector3.Normalize (transform.forward);

				if (compassContainer.activeInHierarchy) {
					compassContainer.transform.rotation = Quaternion.Euler(0f, -yRotation + 180f, 0f);
				}

				if (GetInput.a.TurnLeft()) {
					yRotation -= keyboardTurnSpeed * lookSensitivity;
					//tempQuat = transform.rotation;
					transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
					//transform.parent.transform.parent.transform.localRotation = tempQuat; // preserve x axis, hacky
				}

				if (GetInput.a.TurnRight()) {
					yRotation += keyboardTurnSpeed * lookSensitivity;
					//tempQuat = transform.rotation;
					transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
					//transform.parent.transform.parent.transform.localRotation = tempQuat; // preserve x axis, hacky
				}

				if (GetInput.a.LookDown()) {
					xRotation += keyboardTurnSpeed * lookSensitivity;
					if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle
					transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
				}

				if (GetInput.a.LookUp()) {
					xRotation -= keyboardTurnSpeed * lookSensitivity;
					if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle
					transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
				}
			}
		} else {
			if (!PauseScript.a.Paused() && playerMovement.ressurectingFinished < PauseScript.a.relativeTime) {
				if (GetInput.a.TurnLeft()) {
					yRotation -= keyboardTurnSpeed * lookSensitivity;
					//tempQuat = transform.rotation;
					transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
					//transform.parent.transform.parent.transform.localRotation = tempQuat; // preserve x axis, hacky
				}

				if (GetInput.a.TurnRight()) {
					yRotation += keyboardTurnSpeed * lookSensitivity;
					//tempQuat = transform.rotation;
					transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
					//transform.parent.transform.parent.transform.localRotation = tempQuat; // preserve x axis, hacky
				}

				if (GetInput.a.LookDown()) {
					if (inCyberSpace) {
						// Cyberspace...more like a plane so give the option to invert it separately
						if (Const.a.InputInvertCyberspaceLook) {
							xRotation -= keyboardTurnSpeed * lookSensitivity;
						} else {
							xRotation += keyboardTurnSpeed * lookSensitivity;
						}
					} else {
						// Normal
						if (Const.a.InputInvertLook) {
							xRotation -= keyboardTurnSpeed * lookSensitivity;
						} else {
							xRotation += keyboardTurnSpeed * lookSensitivity;
						}
					}
					if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle
					transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
				}

				if (GetInput.a.LookUp()) {
					if (inCyberSpace) {
						// Cyberspace...more like a plane so give the option to invert it separately
						if (Const.a.InputInvertCyberspaceLook) {
							xRotation += keyboardTurnSpeed * lookSensitivity;
						} else {
							xRotation -= keyboardTurnSpeed * lookSensitivity;
						}
					} else {
						// Normal
						if (Const.a.InputInvertLook) {
							xRotation += keyboardTurnSpeed * lookSensitivity;
						} else {
							xRotation -= keyboardTurnSpeed * lookSensitivity;
						}
					}
					if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle
					transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
				}
			}
			/*
			if (Input.GetButton("Yaw")) {
				if (!PauseScript.a.Paused()) {
					if  (Input.GetAxisRaw("Yaw") > 0) {
						yRotation += keyboardTurnSpeed * lookSensitivity;
						tempQuat = transform.rotation;
						transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
						transform.parent.transform.parent.transform.localRotation = tempQuat; // preserve x axis, hacky
					} else {
						if (Input.GetAxisRaw("Yaw") < 0) {
							yRotation -= keyboardTurnSpeed * lookSensitivity;
							tempQuat = transform.rotation;
							transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
							transform.parent.transform.parent.transform.localRotation = tempQuat; // preserve x axis, hacky
						}
					}
				}
			}
			
			if (Input.GetButton("Pitch")) {
				if (!PauseScript.a.Paused()) {
					if  (Input.GetAxisRaw("Pitch") > 0) {
						xRotation += keyboardTurnSpeed * lookSensitivity;
						transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
					} else {
						if (Input.GetAxisRaw("Pitch") < 0) {
							xRotation -= keyboardTurnSpeed * lookSensitivity;
							transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
						}
					}
				}
			}*/
		}

		// Toggle inventory mode<->shoot mode
		if (GetInput.a != null && PauseScript.a != null) {
			if(GetInput.a.ToggleMode() && !PauseScript.a.Paused()) {
				ToggleInventoryMode();
			}
		}

		// Frob/use if the cursor is not on the UI
		if (!GUIState.a.isBlocking) {
			if (!PauseScript.a.Paused()) {
				currentButton = null;
				if(GetInput.a.Use()) {
					if (vmailActive) {
						logInventory.DeactivateVMail();
						vmailActive = false;
						return;
					}
					if (!holdingObject) {
						// Send out Frob/use raycast to use whatever is under the cursor, if in reach
						RaycastHit hit = new RaycastHit();
						int layMask = LayerMask.GetMask("Default","Geometry","Water","Corpse","Door","InterDebris","PhysObjects","Player2","Player3","Player4","NPC");
						Vector3 cursorPoint = new Vector3(MouseCursor.drawTexture.x+(MouseCursor.drawTexture.width/2),MouseCursor.drawTexture.y+(MouseCursor.drawTexture.height/2),0); 
						cursorPoint.y = Screen.height - cursorPoint.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
						if (Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint), out hit, frobDistance,layMask)) {
							//Debug.Log("Screen.width = " + Screen.width.ToString() + ", Screen.height = " + Screen.height.ToString() +", Camera.pixelWidth = " + playerCamera.pixelWidth.ToString() + ", Camera.pixelHeight = " + playerCamera.pixelHeight.ToString() + ", drawTexture.x = " +MouseCursor.drawTexture.x.ToString() + ", drawTexture.y = " + MouseCursor.drawTexture.y.ToString());
							//drawMyLine(playerCamera.transform.position,hit.point,Color.green,10f);
							if (hit.collider == null) return;
						
							// Check if object is usable then use it
							if (hit.collider.CompareTag("Usable")) {
								//Debug.Log("Raycast hit a usable!");
								UseData ud = new UseData ();
								ud.owner = player;
								UseHandler uh = hit.collider.gameObject.GetComponent<UseHandler>();
								if (uh != null) {
									uh.Use(ud);
								} else {
									UseHandlerRelay uhr = hit.collider.gameObject.GetComponent<UseHandlerRelay>();
									if (uhr != null) {
										uhr.referenceUseHandler.Use(ud);
									} else {
										Debug.Log("BUG: Attempting to use a useable without a UseHandler or UseHandlerRelay!");
									}
								}
								return;
							}
					
							// Check if object is searchable then search it
							if (hit.collider.CompareTag("Searchable")) {
								currentSearchItem = hit.collider.gameObject;
								SearchObject(currentSearchItem.GetComponent<SearchableItem>().lookUpIndex);
								return;
							}
					
					        // If we can't use it, Give info about what we are looking at (e.g. "Molybdenum panelling")
							UseName un = hit.collider.gameObject.GetComponent<UseName> ();
							if (un != null) {
								Const.sprint(Const.a.stringTable[29] + un.targetname,player); // Can't use
							}
						}
						//if (Physics.Raycast(playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center), out hit, 50f)) {
						if (Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint), out hit, 50f)) {
							//drawMyLine(playerCamera.transform.position,hit.point,Color.green,10f);
							// TIP: Use Camera.main.ViewportPointToRay for center of screen
							if (hit.collider == null)
								return;

							// Check if object is usable then use it
							//if (hit.collider.CompareTag("Usable") || hit.collider.CompareTag("Searchable")) {
								Const.sprint(Const.a.stringTable[30],player); // You are too far away from that
								return;
							//}
						}
					} else {
						// First check and see if we can apply held object in a use, or else Drop the object we are holding
						if (heldObjectIndex != -1) {
							if (heldObjectIndex == 54 || heldObjectIndex == 56 || heldObjectIndex == 57 || heldObjectIndex == 61 || heldObjectIndex == 64) {
								RaycastHit hit = new RaycastHit();
								Vector3 cursorPoint = new Vector3(MouseCursor.drawTexture.x+(MouseCursor.drawTexture.width/2),MouseCursor.drawTexture.y+(MouseCursor.drawTexture.height/2),0); 
								cursorPoint.y = Screen.height - cursorPoint.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
								if (Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint), out hit, frobDistance)) {
									//Debug.Log("Screen.width = " + Screen.width.ToString() + ", Screen.height = " + Screen.height.ToString() +", Camera.pixelWidth = " + playerCamera.pixelWidth.ToString() + ", Camera.pixelHeight = " + playerCamera.pixelHeight.ToString() + ", drawTexture.x = " +MouseCursor.drawTexture.x.ToString() + ", drawTexture.y = " + MouseCursor.drawTexture.y.ToString());
									//drawMyLine(playerCamera.transform.position,hit.point,Color.green,10f);

									// Check if object is usable then use it
									if (hit.collider.CompareTag("Usable")) {
										UseData ud = new UseData ();
										ud.owner = player;
										ud.mainIndex = heldObjectIndex;
										ud.customIndex = heldObjectCustomIndex;
										//hit.transform.SendMessageUpwards("Use", ud); // send Use with self as owner of message
										//Debug.Log("Attempting to use a useable...");
										UseHandler uh = hit.collider.gameObject.GetComponent<UseHandler>();
										if (uh != null) {
											SFXSource.PlayOneShot(SearchSFX);
											uh.Use(ud);
										} else {
											UseHandlerRelay uhr = hit.collider.gameObject.GetComponent<UseHandlerRelay>();
											if (uhr != null) {
												SFXSource.PlayOneShot(SearchSFX);
												uhr.referenceUseHandler.Use(ud);
											} else {
												Debug.Log("BUG: Attempting to use a useable without a UseHandler or UseHandlerRelay!");
											}
										}
										return;
									}
								}
								// Drop it
								DropHeldItem ();
								ResetHeldItem();
								ResetCursor ();
							} else {
								// Drop it
								DropHeldItem ();
								ResetHeldItem ();
								ResetCursor ();
							}
						}
					}
				}
			}
		} else {
			//We are holding cursor over the GUI
			if(GetInput.a.Use()) {
				if (holdingObject && !PauseScript.a.Paused()) {
					AddItemToInventory(heldObjectIndex);
					ResetHeldItem();
					ResetCursor();
				} else {
					if (vmailActive) {
						logInventory.DeactivateVMail();
						vmailActive = false;
						return;
					}
					if (GUIState.a.overButton && GUIState.a.overButtonType != GUIState.ButtonType.None) {
                        // overButtonTypes:
                        // -1   Not over a button
                        // 0 WeaponButton
                        // 1 GrenadeButton
                        // 2 PatchButton
                        // 3 GeneralInventoryButton
						// 4 Search contents button
						if (!PauseScript.a.Paused()) {
							switch(GUIState.a.overButtonType) {
								case GUIState.ButtonType.Weapon:
									// Take weapon out of inventory, removing weapon, remove weapon and any other strings I need to CTRL+F my way to this buggy code!
									WeaponButton wepbut = currentButton.GetComponent<WeaponButton>();
									cursorTexture = Const.a.useableItemsFrobIcons[wepbut.useableItemIndex];
									heldObjectIndex = wepbut.useableItemIndex;
									heldObjectCustomIndex = -1;
									heldObjectAmmo = WeaponCurrent.WepInstance.currentMagazineAmount[wepbut.WepButtonIndex];
									heldObjectAmmo2 = WeaponCurrent.WepInstance.currentMagazineAmount2[wepbut.WepButtonIndex];
									//WeaponInventory.WepInventoryInstance.weaponInventoryIndices[wepbut.WepButtonIndex] = -1;
									//WeaponInventory.WepInventoryInstance.weaponInventoryText[wepbut.WepButtonIndex] = "-";
									WeaponCurrent.WepInstance.currentMagazineAmount[wepbut.WepButtonIndex] = 0; // zero out the current ammo
									WeaponCurrent.WepInstance.currentMagazineAmount2[wepbut.WepButtonIndex] = 0; // zero out the current ammo
									WeaponInventory.WepInventoryInstance.RemoveWeapon(wepbut.WepButtonIndex);
									WeaponCurrent.WepInstance.SetAllViewModelsDeactive();
									WeaponCurrent.WepInstance.weaponCurrent = -1;
									WeaponCurrent.WepInstance.weaponIndex = 0;
									wepbut.useableItemIndex = -1;
									wepbut.wbm.WeaponCycleDown();
									MFDManager.a.OpenTab (0, true, MFDManager.TabMSG.Weapon, 0,MFDManager.handedness.LeftHand);
									GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
									break;
								case GUIState.ButtonType.Grenade:
									GrenadeButton grenbut = currentButton.GetComponent<GrenadeButton>();
									cursorTexture = Const.a.useableItemsFrobIcons[grenbut.useableItemIndex];
									heldObjectIndex = grenbut.useableItemIndex;
									GrenadeInventory.GrenadeInvInstance.grenAmmo[grenbut.GrenButtonIndex]--;
									GrenadeCurrent.GrenadeInstance.GrenadeCycleDown();
									if (GrenadeInventory.GrenadeInvInstance.grenAmmo[grenbut.GrenButtonIndex] <= 0) {
										GrenadeInventory.GrenadeInvInstance.grenAmmo[grenbut.GrenButtonIndex] = 0; 
										for (int i = 0; i < 7; i++) {
	                                        if (GrenadeInventory.GrenadeInvInstance.grenAmmo[i] > 0) {
	                                            mainInventory.GetComponent<GrenadeCurrent>().grenadeCurrent = i;
	                                        }
	                                    }
	                                }
									break;
								case GUIState.ButtonType.Patch:
									PatchButton patbut = currentButton.GetComponent<PatchButton>();
									cursorTexture = Const.a.useableItemsFrobIcons[patbut.useableItemIndex];
									heldObjectIndex = patbut.useableItemIndex;
									PatchInventory.PatchInvInstance.patchCounts[patbut.PatchButtonIndex]--;
									if (PatchInventory.PatchInvInstance.patchCounts[patbut.PatchButtonIndex] <= 0) {
										PatchInventory.PatchInvInstance.patchCounts[patbut.PatchButtonIndex] = 0;
										GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
										for (int i = 0; i < 7; i++) {
											if (PatchInventory.PatchInvInstance.patchCounts[i] > 0) {
												mainInventory.GetComponent<PatchCurrent>().patchCurrent = i;
											}
										}
									}
									break;
								case GUIState.ButtonType.GeneralInv:
									if (currentButton == null) {
										Debug.Log("MouseLookScript: currentButton null when trying to right click and over ButtonType.GeneralInv");
									} else {
										GeneralInvButton genbut = currentButton.GetComponent<GeneralInvButton>();
										cursorTexture = Const.a.useableItemsFrobIcons[genbut.useableItemIndex];
										heldObjectIndex = genbut.useableItemIndex;
										GeneralInventory.GeneralInventoryInstance.generalInventoryIndexRef[genbut.GeneralInvButtonIndex] = -1;
									}
	                                break;
								case GUIState.ButtonType.Search:
									SearchButton sebut = currentButton.GetComponentInParent<SearchButton>();
									int tempButtonindex = currentButton.GetComponent<SearchContainerButton>().refIndex;
									cursorTexture = Const.a.useableItemsFrobIcons[sebut.contents[tempButtonindex]];
									heldObjectIndex = sebut.contents[tempButtonindex];
									heldObjectCustomIndex = sebut.customIndex[tempButtonindex];
									currentSearchItem.GetComponent<SearchableItem>().contents[tempButtonindex] = -1;
									currentSearchItem.GetComponent<SearchableItem>().customIndex[tempButtonindex] = -1;
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
									}
									break;
								case GUIState.ButtonType.PGrid:
									PuzzleUIButton puib = currentButton.GetComponent<PuzzleUIButton>();
									if (puib != null) puzzleGrid.OnGridCellClick(puib.buttonIndex);
									break;
								case GUIState.ButtonType.PWire:
									PuzzleUIButton wpuib = currentButton.GetComponent<PuzzleUIButton>();
									if (wpuib != null) {
										if (wpuib.isRH) {
											puzzleWire.ClickRHNode(wpuib.buttonIndex);
										} else {
											puzzleWire.ClickLHNode(wpuib.buttonIndex);
										}
									}
									break;
	                        }
							if (GUIState.a.overButtonType != GUIState.ButtonType.Generic && GUIState.a.overButtonType != GUIState.ButtonType.PGrid && GUIState.a.overButtonType != GUIState.ButtonType.PWire) {
	                       		mouseCursor.cursorImage = cursorTexture;
								ForceInventoryMode();
		                        holdingObject = true;
							}
						}
					}
				}
			}
		}
	}

	// ==============================================		
	// Add usable items to inventory:
	// ==============================================
    void AddGenericObjectToInventory(int index) {
		if (firstTimePickup) {
			firstTimePickup = false;
			centerTabButtonsControl.TabButtonClickSilent (2,true);
			centerTabButtonsControl.NotifyToCenterTab(2);
		}

		itemAdded = false; //prevent false positive
        for (int i=0;i<14;i++) {
            if (GeneralInventory.GeneralInventoryInstance.generalInventoryIndexRef[i] == -1) {
                GeneralInventory.GeneralInventoryInstance.generalInventoryIndexRef[i] = index;
				Const.sprint(Const.a.useableItemsNameText[heldObjectIndex] + Const.a.stringTable[31],player); // Item added to general inventory
                itemAdded = true;
                break;
            }
        }

        if (!itemAdded) {
            DropHeldItem();
            ResetHeldItem();
            ResetCursor();
            Const.sprint(Const.a.stringTable[32] + Const.a.useableItemsNameText[index] + Const.a.stringTable[318],player);
			return;
        }
        mainInventory.GetComponent<GeneralInvCurrent>().generalInvCurrent = index;
		MFDManager.a.SendInfoToItemTab(index);
		centerTabButtonsControl.NotifyToCenterTab(2);
    }

    void AddWeaponToInventory(int index) {
		//if (firstTimePickup) {
		//	firstTimePickup = false;
		//	centerTabButtonsControl.TabButtonClickSilent (0,true);
		//	MFDManager.a.OpenTab (0, true, MFDManager.TabMSG.Weapon, 0,MFDManager.handedness.LeftHand);
		//	centerTabButtonsControl.NotifyToCenterTab(0);
		//	iconman.GetComponent<WeaponIconManager>().SetWepIcon(index);    //Set weapon icon for MFD
		//	weptextman.GetComponent<WeaponTextManager>().SetWepText(index); //Set weapon text for MFD
		//}

		centerTabButtonsControl.TabButtonClickSilent (0,true);
		MFDManager.a.OpenTab (0, true, MFDManager.TabMSG.Weapon, 0,MFDManager.handedness.LeftHand);
		centerTabButtonsControl.NotifyToCenterTab(0);
		iconman.GetComponent<WeaponIconManager>().SetWepIcon(index);    //Set weapon icon for MFD
		weptextman.GetComponent<WeaponTextManager>().SetWepText(index); //Set weapon text for MFD

		itemAdded = false; //prevent false positive
        for (int i=0;i<7;i++) {
            if (WeaponInventory.WepInventoryInstance.weaponInventoryIndices[i] < 0) {
                WeaponInventory.WepInventoryInstance.weaponInventoryIndices[i] = index;
                WeaponInventory.WepInventoryInstance.weaponInventoryText[i] = WeaponInventory.WepInventoryInstance.weaponInvTextSource[(index - 36)];
                WeaponCurrent.WepInstance.weaponCurrent = i;
				WeaponCurrent.WepInstance.weaponIndex = index;
				WeaponAmmo.a.wepAmmo[tempindex] += heldObjectAmmo;
				WeaponAmmo.a.wepAmmoSecondary[tempindex] += heldObjectAmmo2;
				WeaponAmmo.a.wepLoadedWithAlternate[WeaponCurrent.WepInstance.weaponCurrent] = false;
				if (heldObjectAmmo2 > 0) WeaponAmmo.a.wepLoadedWithAlternate[WeaponCurrent.WepInstance.weaponCurrent] = true;
				tempindex = WeaponFire.Get16WeaponIndexFromConstIndex(index);

                weaponButtonsManager.GetComponent<WeaponButtonsManager>().wepButtons[i].GetComponent<WeaponButton>().useableItemIndex = index;
				weaponButtonsManager.GetComponent<WeaponButtonsManager>().wepButtons[i].GetComponent<WeaponButton>().WeaponInvClick();
				//if (!ammoClipBox.activeInHierarchy)
				//	ammoClipBox.SetActive(true);
				
				//if (ammoiconman.activeInHierarchy) ammoiconman.GetComponent<AmmoIconManager>().SetAmmoIcon(index, false);
				//if (iconman.activeInHierarchy) iconman.GetComponent<WeaponIconManager>().SetWepIcon(index);    //Set weapon icon for MFD
				//iconman.GetComponent<WeaponIconManager>().SetWepIcon(index);    //Set weapon icon for MFD
				if (weptextman.activeInHierarchy) weptextman.GetComponent<WeaponTextManager>().SetWepText(index); //Set weapon text for MFD
				//if (itemiconman.activeInHierarchy) itemiconman.SetActive(false);    //Set weapon icon for MFD
				//if (itemtextman.activeInHierarchy) itemtextman.GetComponent<ItemTextManager>().SetItemText(index); //Set weapon text for MFD
				MFDManager.a.SendInfoToItemTab(index); // notify item tab we clicked on a weapon
				Const.sprint(Const.a.useableItemsNameText[index] + Const.a.stringTable[33],player);
				itemAdded = true;
				
				WeaponCurrent.WepInstance.ReloadSecret(true);
				WeaponCurrent.WepInstance.weaponEnergySetting[i] = WeaponCurrent.WepInstance.GetDefaultEnergySettingForWeaponFrom16Index(tempindex);
				//WeaponCurrent.WepInstance.UpdateHUDAmmoCountsEither();
				centerTabButtonsControl.NotifyToCenterTab(0);
				//if (!WeaponInventory.WepInventoryInstance.weaponFound [tempindex])	WeaponInventory.WepInventoryInstance.weaponFound [tempindex] = true;
                break;
            }
        }
		if (!itemAdded) {
			DropHeldItem();
			ResetHeldItem();
			ResetCursor();
            Const.sprint(Const.a.stringTable[32] + Const.a.useableItemsNameText[index] + Const.a.stringTable[318],player);
			return;
		}
		MFDManager.a.SendInfoToItemTab(index);
    }

	void AddAmmoToInventory (int index, int amount, bool isSecondary) {
		if (index == -1)	return;
		if (firstTimePickup) {
			firstTimePickup = false;
			centerTabButtonsControl.TabButtonClickSilent (0,true);
			centerTabButtonsControl.NotifyToCenterTab(0);
		}

		if (isSecondary) {
			WeaponAmmo.a.wepAmmoSecondary [index] += amount;
		} else {
			WeaponAmmo.a.wepAmmo [index] += amount;
		}

		Const.sprint(Const.a.useableItemsNameText[index] + Const.a.stringTable[33],player); // Item added to weapon inventory
		centerTabButtonsControl.NotifyToCenterTab(0);
		MFDManager.a.SendInfoToItemTab(index);
	}

    void AddGrenadeToInventory (int index, int useableIndex) {
		if (firstTimePickup) {
			firstTimePickup = false;
			centerTabButtonsControl.TabButtonClickSilent (0,true);
			centerTabButtonsControl.NotifyToCenterTab(0);
		}
		GrenadeInventory.GrenadeInvInstance.grenAmmo[index]++;
		grenadeCurrent.grenadeCurrent = index;
		grenadeCurrent.grenadeIndex = useableIndex;
		Const.sprint(Const.a.useableItemsNameText[heldObjectIndex] + Const.a.stringTable[34],player);
		centerTabButtonsControl.NotifyToCenterTab(0);
		MFDManager.a.SendInfoToItemTab(index);
    }

	void AddPatchToInventory (int index,int constIndex) {
		if (firstTimePickup) {
			firstTimePickup = false;
			centerTabButtonsControl.TabButtonClickSilent (0,true);
			centerTabButtonsControl.NotifyToCenterTab(0);
		}
		PatchInventory.PatchInvInstance.AddPatchToInventory(index);
		//PatchInventory.PatchInvInstance.patchCounts[index]++;
		PatchCurrent.PatchInstance.patchCurrent = index;
		MFDManager.a.SendInfoToItemTab(index);
		Const.sprint(Const.a.useableItemsNameText[constIndex] + Const.a.stringTable[35],player);
    }

	void AddAudioLogToInventory () {
		if ((heldObjectCustomIndex != -1) && (logInventory != null)) {
			if (heldObjectCustomIndex == 128) {
				// Trioptimum Funpack Module discovered!
				// UPDATE: Create minigames
				Const.sprint(Const.a.stringTable[309],player);
				return;
			}
			logInventory.hasLog[heldObjectCustomIndex] = true;
			logInventory.lastAddedIndex = heldObjectCustomIndex;
			int levelnum = Const.a.audioLogLevelFound[heldObjectCustomIndex];
			logInventory.numLogsFromLevel[levelnum]++;
			logContentsManager.InitializeLogsFromLevelIntoFolder();
			string audName = Const.a.audiologNames[heldObjectCustomIndex];
			string logPlaybackKey = Const.a.InputConfigNames[20];
			if (HardwareInventory.a.hasHardware[2] == true) {
				Const.sprint(Const.a.stringTable[36] + audName + Const.a.stringTable[37] + logPlaybackKey + Const.a.stringTable[38],player); // Audio log ## picked up.  Press '##' to play back.
			} else {
				Const.sprint(Const.a.stringTable[36] + audName + Const.a.stringTable[310],player); // Audio log ## picked up.  Proper hardware not detected to play.
			}
		} else {
			if (logInventory == null) {
				Const.sprint("BUG: logInventory is null",player);
			} else {
				Const.sprint("BUG: Audio log picked up has no assigned index (-1)",player);
			}
		}
	}

	void AddAccessCardToInventory (int index) {
		if (firstTimePickup) {
			firstTimePickup = false;
			centerTabButtonsControl.TabButtonClickSilent (2,true);
		}
			
		bool alreadyHave = false;
		bool accessAdded = false;

		switch (index) {
		case 81: doorAccessTypeAcquired = Door.accessCardType.Standard; break; //CHECKED! Good here
		case 82: doorAccessTypeAcquired = Door.accessCardType.Per1; break; //CHECKED! Good here
		case 83: doorAccessTypeAcquired = Door.accessCardType.Group1; break; //CHECKED! Good here
		case 84: doorAccessTypeAcquired = Door.accessCardType.Science; break; //CHECKED! Good here
		case 85: doorAccessTypeAcquired = Door.accessCardType.Engineering; break;  //CHECKED! Good here
		case 86: doorAccessTypeAcquired = Door.accessCardType.GroupB; break; //CHECKED! Good here
		case 87: doorAccessTypeAcquired = Door.accessCardType.Security; break; //CHECKED! Good here
		case 88: doorAccessTypeAcquired = Door.accessCardType.Per5; break;
		case 89: doorAccessTypeAcquired = Door.accessCardType.Medical; break;
		case 90: doorAccessTypeAcquired = Door.accessCardType.Standard; break;
		case 91: doorAccessTypeAcquired = Door.accessCardType.Per1; break;
		case 114: doorAccessTypeAcquired = Door.accessCardType.Admin; break;
		}

		for (int j = 0; j < accessCardInventory.accessCardsOwned.Length; j++) {
			if (accessCardInventory.accessCardsOwned [j] == doorAccessTypeAcquired) alreadyHave = true; // check if we already have this card
		}

		for (int i = 0; i < accessCardInventory.accessCardsOwned.Length; i++) {
			if (accessCardInventory.accessCardsOwned [i] == Door.accessCardType.None) {
				if (!alreadyHave) {
					accessCardInventory.accessCardsOwned [i] = doorAccessTypeAcquired;
					accessAdded = true;
					break;
				}
			}
		}

		if (alreadyHave) {
			Const.sprint (Const.a.stringTable[44] + doorAccessTypeAcquired.ToString(), player); // Already have access: ##
			return;
		}

		if (accessAdded && !alreadyHave) {
			Const.sprint (Const.a.stringTable[45] + doorAccessTypeAcquired.ToString(), player); // New accesses gained ##
		}

		centerTabButtonsControl.NotifyToCenterTab(2);
		MFDManager.a.SendInfoToItemTab(index);
	}

	void AddHardwareToInventory (int index) {
		int hwversion = heldObjectCustomIndex;
		if (hwversion < 0) {
			Const.sprint("BUG: Hardware picked up has no assigned versioning, defaulting to 1",player);
			hwversion = 1;
		}

		switch(index) {
			case 0:
				// System Analyzer
				if (hwversion <= HardwareInventory.a.hardwareVersion[0]) {
					Const.sprint(Const.a.stringTable[46],player); // THAT WARE IS OBSOLETE. DISCARDED.
					return;
				}
				HardwareInventory.a.hasHardware[0] = true;
				HardwareInventory.a.hardwareVersion[0] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[0] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[21] + " v" + hwversion.ToString(),player);
				break;
			case 1:
				// Navigation Unit
				if (hwversion <= HardwareInventory.a.hardwareVersion[1]) {
					Const.sprint(Const.a.stringTable[46],player);
					return;
				}
				HardwareInventory.a.hasHardware[1] = true;
				HardwareInventory.a.hardwareVersion[1] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[1] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[22] + " v" + hwversion.ToString(),player);
				compassContainer.SetActive(true); // Turn on HUD compass
				automapContainer.SetActive(true);
				if (hwversion == 2) {
					compassMidpoints.SetActive(true);
				}
				if (hwversion > 2) {
					compassSmallTicks.SetActive(true);
					compassLargeTicks.SetActive(true);
				}
				break;
			case 2:
				// Datareader
				if (hwversion <= HardwareInventory.a.hardwareVersion[2]) {
					Const.sprint(Const.a.stringTable[46],player);
					return;
				}
				HardwareInventory.a.hasHardware[2] = true;
				HardwareInventory.a.hardwareVersion[2] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[2] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[23] + " v" + hwversion.ToString(),player);
				hardwareButtons[5].SetActive(true);  // Enable HUD button
				break;
			case 3:
				// Sensaround
				if (hwversion <= HardwareInventory.a.hardwareVersion[3]) {
					Const.sprint(Const.a.stringTable[46],player);
					return;
				}
				HardwareInventory.a.hasHardware[3] = true;
				HardwareInventory.a.hardwareVersion[3] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[3] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[24] + " v" + hwversion.ToString(),player);
				hardwareButtons[1].SetActive(true);  // Enable HUD button
				HardwareInvCurrent.a.hardwareButtons[1].UpdateIconForVersionUpgrade();
				break;
			case 4:
				// Target Identifier
				if (hwversion <= HardwareInventory.a.hardwareVersion[4]) {
					Const.sprint(Const.a.stringTable[46],player);
					return;
				}
				HardwareInventory.a.hasHardware[4] = true;
				HardwareInventory.a.hardwareVersion[4] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[4] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[25] + " v" + hwversion.ToString(),player);
				break;
			case 5:
				// Energy Shield
				if (hwversion <= HardwareInventory.a.hardwareVersion[5]) {
					Const.sprint(Const.a.stringTable[46],player);
					return;
				}
				HardwareInventory.a.hasHardware[5] = true;
				HardwareInventory.a.hardwareVersion[5] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[5] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[26] + " v" + hwversion.ToString(),player);
				hardwareButtons[3].SetActive(true);  // Enable HUD button
				HardwareInvCurrent.a.hardwareButtons[3].UpdateIconForVersionUpgrade();
				break;
			case 6:
				// Biomonitor
				if (hwversion <= HardwareInventory.a.hardwareVersion[6]) {
					Const.sprint(Const.a.stringTable[46],player);
					return;
				}
				HardwareInventory.a.hasHardware[6] = true;
				HardwareInventory.a.hardwareVersion[6] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[6] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[27] + " v" + hwversion.ToString(),player);
				hardwareButtons[0].SetActive(true);  // Enable HUD button
				HardwareInvCurrent.a.hardwareButtons[0].UpdateIconForVersionUpgrade();
				break;
			case 7:
				// Head Mounted Lantern
				if (hwversion <= HardwareInventory.a.hardwareVersion[7]) {
					Const.sprint(Const.a.stringTable[46],player);
					return;
				}
				HardwareInventory.a.hasHardware[7] = true;
				HardwareInventory.a.hardwareVersion[7] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[7] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[28] + " v" + hwversion.ToString(),player);
				hardwareButtons[2].SetActive(true);  // Enable HUD button
				HardwareInvCurrent.a.hardwareButtons[2].UpdateIconForVersionUpgrade();
				break;
			case 8:
				// Envirosuit
				if (hwversion <= HardwareInventory.a.hardwareVersion[8]) {
					Const.sprint(Const.a.stringTable[46],player);
					return;
				}
				HardwareInventory.a.hasHardware[8] = true;
				HardwareInventory.a.hardwareVersion[8] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[8] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[29] + " v" + hwversion.ToString(),player);
				break;
			case 9:
				// Turbo Motion Booster
				if (hwversion <= HardwareInventory.a.hardwareVersion[9]) {
					Const.sprint(Const.a.stringTable[46],player);
					return;
				}
				HardwareInventory.a.hasHardware[9] = true;
				HardwareInventory.a.hardwareVersion[9] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[9] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[30] + " v" + hwversion.ToString(),player);
				hardwareButtons[6].SetActive(true);  // Enable HUD button
				HardwareInvCurrent.a.hardwareButtons[6].UpdateIconForVersionUpgrade();
				break;
			case 10:
				// Jump Jet Boots
				if (hwversion <= HardwareInventory.a.hardwareVersion[10]) {
					Const.sprint(Const.a.stringTable[46],player);
					return;
				}
				HardwareInventory.a.hasHardware[10] = true;
				HardwareInventory.a.hardwareVersion[10] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[10] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[31] + " v" + hwversion.ToString(),player);
				hardwareButtons[7].SetActive(true);  // Enable HUD button
				HardwareInvCurrent.a.hardwareButtons[7].UpdateIconForVersionUpgrade();
				break;
			case 11:
				// Infrared Night Vision Enhancement
				if (hwversion <= HardwareInventory.a.hardwareVersion[11]) {
					Const.sprint(Const.a.stringTable[46],player);
					return;
				}
				HardwareInventory.a.hasHardware[11] = true;
				HardwareInventory.a.hardwareVersion[11] = hwversion;
				HardwareInventory.a.hardwareVersionSetting[11] = hwversion - 1;
				Const.sprint(Const.a.useableItemsNameText[32] + " v" + hwversion.ToString(),player);
				hardwareButtons[4].SetActive(true);  // Enable HUD button
				HardwareInvCurrent.a.hardwareButtons[4].UpdateIconForVersionUpgrade();
				break;
		}
		if (firstTimePickup) {
			firstTimePickup = false;
			centerTabButtonsControl.TabButtonClickSilent (1,true);
		}

		HardwareInventory.a.hwButtonsManager.ActivateHardwareButton(index);
		MFDManager.a.SendInfoToItemTab(index);
		centerTabButtonsControl.NotifyToCenterTab(1);
	}

	public void AddItemToInventory (int index) {
		AudioClip pickclip;
		pickclip = PickupSFX;
		switch (index) {
            case 0:
                AddGenericObjectToInventory(0);
                break;
            case 1:
                AddGenericObjectToInventory(1);
                break;
            case 2:
                AddGenericObjectToInventory(2);
                break;
            case 3:
                AddGenericObjectToInventory(3);
                break;
			case 4:
				AddGenericObjectToInventory(4);
				break;
			case 5:
				AddGenericObjectToInventory(5);
				break;
			case 6:
				AddAudioLogToInventory();
				break;
            case 7:
			    AddGrenadeToInventory(0,7); // Frag
			    break;
		    case 8:
			    AddGrenadeToInventory(3,8); // Concussion
			    break;
		    case 9:
			    AddGrenadeToInventory(1,9); // EMP
			    break;
		    case 10:
			    AddGrenadeToInventory(6,10); // Earth Shaker
			    break;
		    case 11:
			    AddGrenadeToInventory(4,11); // Land Mine
			    break;
		    case 12:
			    AddGrenadeToInventory(5,12); // Nitropak
			    break;
		    case 13:
			    AddGrenadeToInventory(2,13); // Gas
			    break;
		    case 14:
			    AddPatchToInventory(2,14);
			    break;
		    case 15:
			    AddPatchToInventory(6,15);
			    break;
		    case 16:
			    AddPatchToInventory(5,16);
			    break;
		    case 17:
			    AddPatchToInventory(3,17);
			    break;
		    case 18:
			    AddPatchToInventory(4,18);
			    break;
		    case 19:
			    AddPatchToInventory(1,19);
			    break;
		    case 20:
			    AddPatchToInventory(0,20);
			    break;
			case 21:
				AddHardwareToInventory(0);
				break;
			case 22:
				AddHardwareToInventory(1);
				break;
			case 23:
				AddHardwareToInventory(2);
				break;
			case 24:
				AddHardwareToInventory(3);
				break;
			case 25:
				AddHardwareToInventory(4);
				break;
			case 26:
				AddHardwareToInventory(5);
			    break;
			case 27:
				AddHardwareToInventory(6);
			    break;
			case 28:
				AddHardwareToInventory(7);
				break;
			case 29:
				AddHardwareToInventory(8);
				break;
			case 30:
				AddHardwareToInventory(9);
				break;
			case 31:
				AddHardwareToInventory(10);
				break;
			case 32:
				AddHardwareToInventory(11);
				break;
			case 33:
				AddGenericObjectToInventory(33);
				break;
			case 34:
				AddGenericObjectToInventory(34);
				break;
			case 35:
				AddGenericObjectToInventory(35);
				break;
			case 36:
				AddWeaponToInventory(36);
				break;
			case 37:
				AddWeaponToInventory(37);
				break;
			case 38:
				AddWeaponToInventory(38);
				break;
            case 39:
                AddWeaponToInventory(39);
                break;
            case 40:
                AddWeaponToInventory(40);
                break;
            case 41:
                AddWeaponToInventory(41);
                break;
            case 42:
				AddWeaponToInventory(42);
				break;
            case 43:
                AddWeaponToInventory(43);
                break;
            case 44:
                AddWeaponToInventory(44);
                break;
            case 45:
                AddWeaponToInventory(45);
                break;
			case 46:
				AddWeaponToInventory(46);
				break;
			case 47:
				AddWeaponToInventory(47);
				break;
			case 48:
				AddWeaponToInventory(48);
				break;
			case 49:
				AddWeaponToInventory(49);
				break;
			case 50:
				AddWeaponToInventory(50);
				break;
			case 51:
				AddWeaponToInventory(51);
				break;
			case 52:
				AddGenericObjectToInventory(52);
				break;
			case 53:
				AddGenericObjectToInventory(53);
				break;
			case 54:
				AddGenericObjectToInventory(54);
				break;
			case 55:
				AddGenericObjectToInventory(55);
				break;
			case 56:
				AddGenericObjectToInventory(56);
				break;
			case 57:
				AddGenericObjectToInventory(57);
				break;
			case 58:
				AddGenericObjectToInventory(58);
				break;
			case 59:
				AddGenericObjectToInventory(59);
				break;
			case 61:
				AddGenericObjectToInventory(61);
				break;
			case 62:
				AddGenericObjectToInventory(62);
				break;
			case 63:
				AddGenericObjectToInventory(63);
				break;
			case 64:
				AddGenericObjectToInventory(64);
				break;
			case 65:
				AddAmmoToInventory(8, Const.a.magazinePitchCountForWeapon2[8], true); // magpulse cartridge super
				break;
			case 66:
				AddAmmoToInventory(2, Const.a.magazinePitchCountForWeapon[2], false); // needle darts
				break;
			case 67:
				AddAmmoToInventory(2, Const.a.magazinePitchCountForWeapon2[2], true); // tranquilizer darts
				break;
			case 68:
				AddAmmoToInventory(9, Const.a.magazinePitchCountForWeapon[9], false); // standard bullets
				break;
			case 69:
				AddAmmoToInventory(9, Const.a.magazinePitchCountForWeapon2[9], true); // teflon bullets
				break;
			case 70:
				AddAmmoToInventory(7, Const.a.magazinePitchCountForWeapon[7], false); // hollow point rounds
				break;
			case 71:
				AddAmmoToInventory(7, Const.a.magazinePitchCountForWeapon2[7], true); // slug rounds
				break;
			case 72:
				AddAmmoToInventory(0, Const.a.magazinePitchCountForWeapon[0], false); // magnesium tipped slugs
				break;
			case 73:
				AddAmmoToInventory(0, Const.a.magazinePitchCountForWeapon2[0], true); // penetrator slugs
				break;
			case 74:
				AddAmmoToInventory(3, Const.a.magazinePitchCountForWeapon[3], false); // hornet clip
				break;
			case 75:
				AddAmmoToInventory(3, Const.a.magazinePitchCountForWeapon2[3], true); // splinter clip
				break;
			case 76:
				AddAmmoToInventory(11, Const.a.magazinePitchCountForWeapon[11], false); // rail rounds
				break;
			case 77:
				AddAmmoToInventory(13, Const.a.magazinePitchCountForWeapon[13], false); // slag magazine
				break;
			case 78:
				AddAmmoToInventory(13, Const.a.magazinePitchCountForWeapon2[13], true); // large slag magazine
				break;
			case 79:
				AddAmmoToInventory(8, Const.a.magazinePitchCountForWeapon[8], false); // magpulse cartridges
				break;
			case 80:
				AddAmmoToInventory(8, Const.a.magazinePitchCountForWeapon2[8], false); // small magpulse cartridges
				break;
			case 81:
				AddAccessCardToInventory(81);
				break;
			case 82:
				AddAccessCardToInventory(82);
				break;
			case 83:
				AddAccessCardToInventory(83);
				break;
			case 84:
				AddAccessCardToInventory(84);
				break;
			case 85:
				AddAccessCardToInventory(85);
				break;
			case 86:
				AddAccessCardToInventory(86);
				break;
			case 87:
				AddAccessCardToInventory(87);
				break;
			case 88:
				AddAccessCardToInventory(88);
				break;
			case 89:
				AddAccessCardToInventory(89);
				break;
			case 90:
				AddAccessCardToInventory(90);
				break;
			case 91:
				AddAccessCardToInventory(91);
				break;
			case 92:
				AddGenericObjectToInventory(92);
				break;
			case 93:
				AddGenericObjectToInventory(93);
				break;
			case 94:
				AddGenericObjectToInventory(94);
				break;
			case 95:
				AddGenericObjectToInventory(95);
				break;
			case 96:
				AddGenericObjectToInventory(96);
				break;
			case 97:
				AddGenericObjectToInventory(97);
				break;
			case 98:
				AddGenericObjectToInventory(98);
				break;
			case 99:
				AddGenericObjectToInventory(99);
				break;
			case 100:
				AddGenericObjectToInventory(100);
				break;
			case 101:
				AddGenericObjectToInventory(101);
				break;
			case 113:
				AddAmmoToInventory(12, Const.a.magazinePitchCountForWeapon[12], false); // rubber slugs
				break;
			case 114:
				AddAccessCardToInventory(114);
				break;
        }
		SFXSource.PlayOneShot(pickclip);
		firstTimePickup = false;
	}

	public void DropHeldItem() {
		if (heldObjectIndex < 0 || heldObjectIndex > 255) {
			Const.sprint("BUG: Attempted to DropHeldItem with index out of bounds (<0 or >255) and heldObjectIndex = " + heldObjectIndex.ToString(),player);
			return;
		}

		if (!grenadeActive) heldObject = Const.a.useableItems[heldObjectIndex]; // set by UseGrenade()
		if (heldObject != null) {
			GameObject tossObject = null;
			bool freeObjectInPoolFound = false;
			GameObject levelDynamicContainer = LevelManager.a.GetCurrentLevelDynamicContainer();

			// Fnd any free inactive objects within the level's Levelnumber.Dynamic container and activate those before instantiating
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
					tossObject.transform.position = (transform.position + (transform.forward * tossOffset));
					if (tossObject == null) {
						Const.sprint("BUG: Failed to get freeObjectInPool for object being dropped!",player);
						return;
					}
				} else {
					tossObject = Instantiate(heldObject,(transform.position + (transform.forward * tossOffset)),Quaternion.identity) as GameObject;  //effect
					if (tossObject == null) {
						Const.sprint("BUG: Failed to instantiate object being dropped!",player);
						return;
					}
				}
				if (tossObject.activeSelf != true) {
					tossObject.SetActive(true);
				}
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
				tossObject = Instantiate(heldObject,(transform.position + (transform.forward * tossOffset)),Quaternion.identity) as GameObject;  //effect
				if (tossObject == null) {
					Const.sprint("BUG: Failed to instantiate object being dropped!",player);
					return;
				}
				if (levelDynamicContainer != null){
					tossObject.transform.SetParent(levelDynamicContainer.transform,true);
					SaveObject so = tossObject.GetComponent<SaveObject>();
					if (so != null) so.levelParentID = LevelManager.a.currentLevel;
				}

				Vector3 tossDir = new Vector3(MouseCursor.drawTexture.x+(MouseCursor.drawTexture.width/2),MouseCursor.drawTexture.y+(MouseCursor.drawTexture.height/2),0);
				tossDir.y = Screen.height - tossDir.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
				tossDir = playerCamera.ScreenPointToRay(tossDir).direction;
				tossObject.GetComponent<Rigidbody>().velocity = tossDir * tossForce;
				GrenadeActivate ga = tossObject.GetComponent<GrenadeActivate>();
				if (ga != null) {
					ga.Activate(heldObjectIndex, grenadeCurrent); // time to boom
				}
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
			Const.sprint("BUG: Could Not Find object 'MouseCursorHandler' in scene\n",Const.a.allPlayers);
        }
	}
		
	public void ToggleInventoryMode (){
		if (inventoryMode) {
			ForceShootMode();
		} else {
			ForceInventoryMode();
		}
	}

	public void ForceShootMode() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		inventoryMode = false;
		if (vmailActive) {
			logInventory.DeactivateVMail();
			vmailActive = false;
		}
	}

	public void ForceInventoryMode() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = false;
		inventoryMode = true;
	}
	
	void  SearchObject ( int index  ){
		SearchableItem curSearchScript = currentSearchItem.GetComponent<SearchableItem>();
		curSearchScript.searchableInUse = true;
		curSearchScript.currentPlayerCapsule = transform.parent.gameObject;  // Get playerCapsule of player this is attached to

		// Play search sound
		SFXSource.PlayOneShot(SearchSFX);

		// Search through array to see if any items are in the container
		int numberFoundContents = 0;
		int[] resultContents = {-1,-1,-1,-1};  // create blanked container for search results
		int[] resultCustomIndex = {-1,-1,-1,-1};  // create blanked container for search results custom indices
		for (int i=curSearchScript.numSlots - 1;i>=0;i--) {
			//Const.sprint("Search index = " + i.ToString() + ", and SearchableItem.customIndex.Length = " + currentSearchItem.GetComponent<SearchableItem>().customIndex.Length.ToString());
			resultContents[i] = curSearchScript.contents[i];
			resultCustomIndex[i] = curSearchScript.customIndex[i];
			if (resultContents[i] > -1) {
				numberFoundContents++; // if something was found, add 1 to count
			}
		}

		if (firstTimeSearch) {
			firstTimeSearch = false;
			MFDManager.a.OpenTab (4, true, MFDManager.TabMSG.Search, -1,MFDManager.handedness.LeftHand);
		}
		MFDManager.a.SendSearchToDataTab(curSearchScript.objectName, numberFoundContents, resultContents, resultCustomIndex,currentSearchItem.transform.position,curSearchScript);
		ForceInventoryMode();
	}

	public void UseGrenade (int index) {
		if (holdingObject) { Const.sprint(Const.a.stringTable[311],player); return; } // Can't use grenade, hands full
		if (index < 7 || index > 13) { Debug.Log("BUG: index outside of 7 to 13 passed to UseGrenade() in MouseLookScript.cs"); return; }
		ForceInventoryMode();  // inventory mode is turned on when picking something up
		ResetHeldItem();
		holdingObject = true;
		heldObjectIndex = index;
		mouseCursor.cursorImage = Const.a.useableItemsFrobIcons[index];
		mouseCursor.liveGrenade = true;
		grenadeActive = true;
		Const.sprint(Const.a.useableItemsNameText[index] + Const.a.stringTable[320],player);

		// Subtract one from grenade inventory
		switch(index) {
		case 7: heldObject = Const.a.useableItems[154]; playerGrenInv.grenAmmo[0]--; Debug.Log("Frag grenade double clicked"); break; // Frag
		case 8: heldObject = Const.a.useableItems[150]; playerGrenInv.grenAmmo[3]--; Debug.Log("Concussion grenade double clicked"); break; // Concussion
		case 9: heldObject = Const.a.useableItems[152]; playerGrenInv.grenAmmo[1]--; Debug.Log("EMP grenade double clicked"); break; // EMP
		case 10: heldObject = Const.a.useableItems[151]; playerGrenInv.grenAmmo[6]--;Debug.Log("Earth Shaker grenade double clicked"); break; // Earth Shaker
		case 11: heldObject = Const.a.useableItems[156]; playerGrenInv.grenAmmo[4]--; Debug.Log("Land Mine grenade double clicked"); break; // Land Mine
		case 12: heldObject = Const.a.useableItems[157]; playerGrenInv.grenAmmo[5]--; Debug.Log("Nitropak grenade double clicked"); break; // Nitropak
		case 13: heldObject = Const.a.useableItems[155]; playerGrenInv.grenAmmo[2]--; Debug.Log("Gas grenade double clicked"); break; // Gas
		}
	}

	/*
	void drawMyLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		StartCoroutine( drawLine(start, end, color, duration));
	}

	IEnumerator drawLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		GameObject myLine = new GameObject ();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer> ();
		LineRenderer lr = myLine.GetComponent<LineRenderer> ();
		lr.material = new Material (Shader.Find ("Legacy Shaders/Particles/Additive"));
		lr.startColor = color;
		lr.endColor = color;
		lr.startWidth = 0.1f;
		lr.endWidth = 0.1f;
		lr.SetPosition (0, start);
		lr.SetPosition (1, end);
		yield return new WaitForSeconds(duration);
		GameObject.Destroy (myLine);
	}*/

	public void ScreenShake (float force) {
		Debug.Log("Screen shake signal received by MouseLookScript!");
	}

	public void ToggleAudioPause() {
		if (PauseScript.a.Paused()) {
			AudioListener.pause = true;
		} else {
			AudioListener.pause = false;
		}
	}
}