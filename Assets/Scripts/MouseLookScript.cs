using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MouseLookScript : MonoBehaviour {
    // Internal to Prefab
    // ------------------------------------------------------------------------
	public GameObject player;
	public float recompile;
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
    public int heldObjectIndex;
	public int heldObjectCustomIndex;
	public int heldObjectAmmo;
	public bool heldObjectAmmoIsSecondary;
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
    private GameObject heldObject;
    private bool itemAdded = false;
	private int indexAdjustment;
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
	public GameObject compassMidpoints;
	public GameObject compassLargeTicks;
	public GameObject compassSmallTicks;
    [Tooltip("Game object that houses the MFD tabs")]
	public GameObject tabControl;
	[Tooltip("Text in the data tab in the MFD that displays when searching an object containing no items")]
	public Text dataTabNoItemsText;
	public DataTab dataTabControl;
	public LogContentsButtonsManager logContentsManager;
	public AudioSource SFXSource;
	public CenterTabButtons centerTabButtonsControl;
	public MFDManager mfdManager;
	public GameObject iconman;
	public GameObject itemiconman;
	public GameObject itemtextman;
	public GameObject ammoiconman;
	public GameObject weptextman;
	public GameObject ammoClipBox;
	public GrenadeCurrent grenadeCurrent;
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
	public GameObject mainMenu;
	public WeaponMagazineCounter wepmagCounter;
	public PlayerMovement playerMovement;

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
        //mouseCursor = GameObject.Find("MouseCursorHandler");
        // if (mouseCursor == null)
        //    Const.sprint("BUG: Could Not Find object 'MouseCursorHandler' in scene",player);
		
        ResetCursor();
		Cursor.lockState = CursorLockMode.None;
		inventoryMode = true;  // Start with inventory mode turned on
		playerCamera = GetComponent<Camera>();
		frobDistance = Const.a.frobDistance;
		holdingObject = false;
		heldObjectIndex = -1;
		heldObjectAmmo = 0;
		heldObjectAmmoIsSecondary = false;
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
		recoiling = true;
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

		// Spring Back to Rest from Recoil TODO only do this when necessary and add some private variables to prevent GarbageCollector
		float camz = Mathf.Lerp(transform.localPosition.z,0f,0.1f);
		Vector3 camPos = new Vector3(0f,Const.a.playerCameraOffsetY*playerMovement.currentCrouchRatio,camz);
		transform.localPosition = camPos;

        // Draw line from cursor - used for projectile firing, e.g. magpulse/stugngun/railgun/plasma
        RaycastHit rayhit = new RaycastHit();
        Vector3 cursorPoint0 = new Vector3(MouseCursor.drawTexture.x + (MouseCursor.drawTexture.width / 2), MouseCursor.drawTexture.y + (MouseCursor.drawTexture.height / 2), 0);
        cursorPoint0.y = Screen.height - cursorPoint0.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
        if (Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint0), out rayhit, Mathf.Infinity)) {
            cameraFocusPoint = rayhit.point;
            //drawMyLine(playerCamera.transform.position, rayhit.point, Color.red, .1f);
        }

        if (mainMenu.activeSelf == true) return;  // ignore mouselook when main menu is still up

		if (Input.GetKeyUp("f6")) {
			Const.a.Save(7);
		}

		if (Input.GetKeyUp("f9")) {
			Const.a.Load(7);
		}

        if (inventoryMode == false) {
			if (PauseScript.a != null && !PauseScript.a.Paused()) {
				float dx = Input.GetAxisRaw("Mouse X");
				float dy = Input.GetAxisRaw("Mouse Y");
				yRotation += (dx * lookSensitivity);
				xRotation -= (dy * lookSensitivity);
				if (!inCyberSpace) xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit up and down angle
				transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f); // left right component applied to capsule
				transform.localRotation = Quaternion.Euler(xRotation,0f,0f); // Up down component only applied to camera

				if (inCyberSpace) cyberLookDir = Vector3.Normalize (transform.forward);

				if (compassContainer.activeInHierarchy) {
					compassContainer.transform.rotation = Quaternion.Euler(0f, -yRotation + 180f, 0f);
				}
			} else {
				Const.sprint("ERROR: Paused is true and inventoryMode is false",player);
			}
		} else {
			if (Input.GetButton("Yaw")) {
				if (!PauseScript.a.Paused()) {
					if  (Input.GetAxisRaw("Yaw") > 0) {
						yRotation += keyboardTurnSpeed * lookSensitivity;
						tempQuat = transform.rotation;
						transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
						transform.parent.transform.parent.transform.localRotation = tempQuat; // preserve x axis, hacky
					} else {
						if (Input.GetAxisRaw("Yaw") < 0) {
							yRotation -= keyboardTurnSpeed * lookSensitivity;
							tempQuat = transform.rotation;
							transform.parent.transform.parent.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
							transform.parent.transform.parent.transform.localRotation = tempQuat; // preserve x axis, hacky
						}
					}
				}
			}
			
			if (Input.GetButton("Pitch")) {
				if (!PauseScript.a.Paused()) {
					if  (Input.GetAxisRaw("Pitch") > 0) {
						xRotation += keyboardTurnSpeed * lookSensitivity;
						transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
					} else {
						xRotation -= keyboardTurnSpeed * lookSensitivity;
						transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
					}
				}
			}
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
					if (!holdingObject) {
						// Send out Frob/use raycast to use whatever is under the cursor, if in reach
						RaycastHit hit = new RaycastHit();
						Vector3 cursorPoint = new Vector3(MouseCursor.drawTexture.x+(MouseCursor.drawTexture.width/2),MouseCursor.drawTexture.y+(MouseCursor.drawTexture.height/2),0); 
						cursorPoint.y = Screen.height - cursorPoint.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
						if (Physics.Raycast(playerCamera.ScreenPointToRay(cursorPoint), out hit, frobDistance)) {
							//Debug.Log("Screen.width = " + Screen.width.ToString() + ", Screen.height = " + Screen.height.ToString() +", Camera.pixelWidth = " + playerCamera.pixelWidth.ToString() + ", Camera.pixelHeight = " + playerCamera.pixelHeight.ToString() + ", drawTexture.x = " +MouseCursor.drawTexture.x.ToString() + ", drawTexture.y = " + MouseCursor.drawTexture.y.ToString());
							//drawMyLine(playerCamera.transform.position,hit.point,Color.green,10f);
							if (hit.collider == null) return;
						
							// Check if object is usable then use it
							if (hit.collider.tag == "Usable") {
								//Debug.Log("Raycast hit a usable!");
								UseData ud = new UseData ();
								ud.owner = player;
								/*
								//hit.transform.SendMessageUpwards("Use", ud); // send Use with self as owner of message DIE SendMessage DIE!!!
								if (hit.transform.GetComponent<UseHandler>() != null) {
									//Debug.Log("Found a UseHandler!");
									hit.transform.GetComponent<UseHandler>().Use(ud); // Just plain use it, handler checks for and has any and all scripts run their Use(UseData ud) function, passing along ud
								} else {
									//Debug.Log("Couldn't find a UseHandler");
									if (hit.transform.GetComponent<UseHandlerRelay>() != null) {
										hit.transform.GetComponent<UseHandlerRelay>().referenceUseHandler.Use(ud);
										//Debug.Log("Found a UseHandlerRelay!");
									} else {
										Debug.Log("Warning: could not find UseHandler or UseHandlerRelay on hit.transform");
									}
								}*/
								//Debug.Log("Attempting to use a useable...");
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
							if (hit.collider.tag == "Searchable") {
								currentSearchItem = hit.collider.gameObject;
								SearchObject(currentSearchItem.GetComponent<SearchableItem>().lookUpIndex);
								return;
							}
					
					        // If we can't use it, Give info about what we are looking at (e.g. "Molybdenum panelling")
							UseName un = hit.collider.gameObject.GetComponent<UseName> ();
							if (un != null) {
								Const.sprint("Can't use " + un.targetname,player);
							}
						}
						if (Physics.Raycast(playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center), out hit, 50f)) {
							//drawMyLine(playerCamera.transform.position,hit.point,Color.green,10f);
							// TIP: Use Camera.main.ViewportPointToRay for center of screen
							if (hit.collider == null)
								return;

							// Check if object is usable then use it
							if (hit.collider.tag == "Usable" || hit.collider.tag == "Searchable") {
								Const.sprint("You are too far away from that",player);
								return;
							}
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
									if (hit.collider.tag == "Usable") {
										UseData ud = new UseData ();
										ud.owner = player;
										ud.mainIndex = heldObjectIndex;
										ud.customIndex = heldObjectCustomIndex;
										//hit.transform.SendMessageUpwards("Use", ud); // send Use with self as owner of message
										//Debug.Log("Attempting to use a useable...");
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
								}
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
									WeaponButton wepbut = currentButton.GetComponent<WeaponButton>();
									cursorTexture = Const.a.useableItemsFrobIcons[wepbut.useableItemIndex];
									heldObjectIndex = wepbut.useableItemIndex;
									indexAdjustment = wepbut.WepButtonIndex;
									WeaponInventory.WepInventoryInstance.weaponInventoryIndices[indexAdjustment] = -1;
									WeaponInventory.WepInventoryInstance.weaponInventoryText[indexAdjustment] = "-";
									indexAdjustment--;
									if (indexAdjustment < 0)
										indexAdjustment = 0;
									WeaponCurrent.WepInstance.weaponCurrent = indexAdjustment;
									WeaponCurrent.WepInstance.justChangedWeap = true;
									//wepbut.ammoiconman.GetComponent<AmmoIconManager>().SetAmmoIcon(indexAdjustment,WeaponCurrent.WepInstance.weaponIsAlternateAmmo);
									//wepbut.iconman.GetComponent<WeaponIconManager>().SetWepIcon(indexAdjustment
									mfdManager.OpenTab (0, true, MFDManager.TabMSG.Weapon, 0,MFDManager.handedness.LeftHand);
									GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
									break;
								case GUIState.ButtonType.Grenade:
									GrenadeButton grenbut = currentButton.GetComponent<GrenadeButton>();
									cursorTexture = Const.a.useableItemsFrobIcons[grenbut.useableItemIndex];
									heldObjectIndex = grenbut.useableItemIndex;
									GrenadeInventory.GrenadeInvInstance.grenAmmo[grenbut.GrenButtonIndex]--;
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
									GeneralInvButton genbut = currentButton.GetComponent<GeneralInvButton>();
									cursorTexture = Const.a.useableItemsFrobIcons[genbut.useableItemIndex];
									heldObjectIndex = genbut.useableItemIndex;
									GeneralInventory.GeneralInventoryInstance.generalInventoryIndexRef[genbut.GeneralInvButtonIndex] = -1;
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
									sebut.GetComponentInParent<DataTab>().searchItemImages[tempButtonindex].SetActive(false);
									sebut.CheckForEmpty();
									GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
									break;
	                        }
							if (GUIState.a.overButtonType != GUIState.ButtonType.Generic) {
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
				Const.sprint("Item added to general inventory",player);
                itemAdded = true;
                break;
            }
        }

        if (!itemAdded) {
            DropHeldItem();
            ResetHeldItem();
            ResetCursor();
            Const.sprint("Inventory full, item dropped",player);
			return;
        }
        mainInventory.GetComponent<GeneralInvCurrent>().generalInvCurrent = index;
		mfdManager.SendInfoToItemTab(index);
		centerTabButtonsControl.NotifyToCenterTab(2);
    }

    void AddWeaponToInventory(int index) {
		//if (firstTimePickup) {
		//	firstTimePickup = false;
		//	centerTabButtonsControl.TabButtonClickSilent (0,true);
		//	mfdManager.OpenTab (0, true, MFDManager.TabMSG.Weapon, 0,MFDManager.handedness.LeftHand);
		//	centerTabButtonsControl.NotifyToCenterTab(0);
		//	iconman.GetComponent<WeaponIconManager>().SetWepIcon(index);    //Set weapon icon for MFD
		//	weptextman.GetComponent<WeaponTextManager>().SetWepText(index); //Set weapon text for MFD
		//}

		centerTabButtonsControl.TabButtonClickSilent (0,true);
		mfdManager.OpenTab (0, true, MFDManager.TabMSG.Weapon, 0,MFDManager.handedness.LeftHand);
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
                weaponButtonsManager.GetComponent<WeaponButtonsManager>().wepButtons[i].GetComponent<WeaponButton>().useableItemIndex = index;
				if (!ammoClipBox.activeInHierarchy)
					ammoClipBox.SetActive(true);
				

				if (ammoiconman.activeInHierarchy) ammoiconman.GetComponent<AmmoIconManager>().SetAmmoIcon(index, false);
				//if (iconman.activeInHierarchy) iconman.GetComponent<WeaponIconManager>().SetWepIcon(index);    //Set weapon icon for MFD
				iconman.GetComponent<WeaponIconManager>().SetWepIcon(index);    //Set weapon icon for MFD
				if (weptextman.activeInHierarchy) weptextman.GetComponent<WeaponTextManager>().SetWepText(index); //Set weapon text for MFD
				if (itemiconman.activeInHierarchy) itemiconman.SetActive(false);    //Set weapon icon for MFD
				if (itemtextman.activeInHierarchy) itemtextman.GetComponent<ItemTextManager>().SetItemText(index); //Set weapon text for MFD
				mfdManager.SendInfoToItemTab(index); // notify item tab we clicked on a weapon
				Const.sprint("Weapon added to inventory",player);
				itemAdded = true;

				centerTabButtonsControl.NotifyToCenterTab(0);
				tempindex = WeaponFire.Get16WeaponIndexFromConstIndex(index);
				if (heldObjectAmmo > 0) {
					int extra = 0;
					if (heldObjectAmmoIsSecondary) {
						if (heldObjectAmmo > Const.a.magazinePitchCountForWeapon2[tempindex]) {
							extra = (heldObjectAmmo - Const.a.magazinePitchCountForWeapon2[tempindex]);
							heldObjectAmmo = Const.a.magazinePitchCountForWeapon2[tempindex];
						}
					} else {
						if (heldObjectAmmo > Const.a.magazinePitchCountForWeapon[tempindex]) {
							extra = (heldObjectAmmo - Const.a.magazinePitchCountForWeapon[tempindex]);
							heldObjectAmmo = Const.a.magazinePitchCountForWeapon[tempindex];
						}
					}
					WeaponCurrent.WepInstance.weaponIsAlternateAmmo = WeaponAmmo.a.wepLoadedWithAlternate[tempindex];	
					if (WeaponCurrent.WepInstance.weaponIsAlternateAmmo) {
						if (WeaponCurrent.WepInstance.currentMagazineAmount2[tempindex] <=0) {
							WeaponCurrent.WepInstance.currentMagazineAmount2[tempindex] = heldObjectAmmo;

							// Update the counter
							MFDManager.a.UpdateHUDAmmoCounts(WeaponCurrent.WepInstance.currentMagazineAmount2[tempindex]);
						}
					} else {
						if (WeaponCurrent.WepInstance.currentMagazineAmount[tempindex] <=0) {
							WeaponCurrent.WepInstance.currentMagazineAmount[tempindex] = heldObjectAmmo;

							// Update the counter
							MFDManager.a.UpdateHUDAmmoCounts(WeaponCurrent.WepInstance.currentMagazineAmount[tempindex]);
						}
					}

					if (heldObjectAmmoIsSecondary && extra > 0) {
						WeaponAmmo.a.wepAmmoSecondary[tempindex] += extra;
					} else {
						WeaponAmmo.a.wepAmmo[tempindex] += extra;
					}
				}
				if (!WeaponInventory.WepInventoryInstance.weaponFound [tempindex])	WeaponInventory.WepInventoryInstance.weaponFound [tempindex] = true;
                break;
            }
        }
		if (!itemAdded) {
			DropHeldItem();
			ResetHeldItem();
			ResetCursor();
			Const.sprint("Inventory full, item dropped",player);
			return;
		}
		mfdManager.SendInfoToItemTab(index);
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
		centerTabButtonsControl.NotifyToCenterTab(0);
		mfdManager.SendInfoToItemTab(index);
	}

    void AddGrenadeToInventory (int index) {
		if (firstTimePickup) {
			firstTimePickup = false;
			centerTabButtonsControl.TabButtonClickSilent (0,true);
			centerTabButtonsControl.NotifyToCenterTab(0);
		}
		GrenadeInventory.GrenadeInvInstance.grenAmmo[index]++;
		grenadeCurrent.grenadeCurrent = index;
		Const.sprint("Grenade added to inventory",player);
		centerTabButtonsControl.NotifyToCenterTab(0);
		mfdManager.SendInfoToItemTab(index);
    }

	void AddPatchToInventory (int index) {
		if (firstTimePickup) {
			firstTimePickup = false;
			centerTabButtonsControl.TabButtonClickSilent (0,true);
			centerTabButtonsControl.NotifyToCenterTab(0);
		}
		PatchInventory.PatchInvInstance.AddPatchToInventory(index);
		//PatchInventory.PatchInvInstance.patchCounts[index]++;
		PatchCurrent.PatchInstance.patchCurrent = index;
		mfdManager.SendInfoToItemTab(index);
		Const.sprint("Patch added to inventory",player);
    }

	void AddAudioLogToInventory () {
		if ((heldObjectCustomIndex != -1) && (logInventory != null)) {
			if (heldObjectCustomIndex == 114) {
				// Trioptimum Funpack Module discovered!
				// TODO: Create minigames
				Const.sprint("Trioptimum Funpack Module, don't play on company time!",player);
				return;
			}
			logInventory.hasLog[heldObjectCustomIndex] = true;
			logInventory.lastAddedIndex = heldObjectCustomIndex;
			int levelnum = Const.a.audioLogLevelFound[heldObjectCustomIndex];
			logInventory.numLogsFromLevel[levelnum]++;
			logContentsManager.InitializeLogsFromLevelIntoFolder();
			string audName = Const.a.audiologNames[heldObjectCustomIndex];
			string logPlaybackKey = Const.a.InputConfigNames[20];
			Const.sprint("Audio log " + audName + " picked up.  Press '" + logPlaybackKey + "' to playback.",player);
		} else {
			if (logInventory == null) {
				Const.sprint("Warning: logInventory is null",player);
			} else {
				Const.sprint("Warning: Audio log picked up has no assigned index (-1)",player);
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
		case 86: doorAccessTypeAcquired = Door.accessCardType.Standard; break;
		case 87: doorAccessTypeAcquired = Door.accessCardType.Standard; break;
		case 88: doorAccessTypeAcquired = Door.accessCardType.Standard; break;
		case 89: doorAccessTypeAcquired = Door.accessCardType.Medical; break;
		case 90: doorAccessTypeAcquired = Door.accessCardType.Standard; break;
		case 91: doorAccessTypeAcquired = Door.accessCardType.Per1; break;
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
			Const.sprint ("Already have that access.", player);
			return;
		}

		if (accessAdded && !alreadyHave) {
			Const.sprint ("New accesses gained: " + doorAccessTypeAcquired.ToString(), player);
		}

		centerTabButtonsControl.NotifyToCenterTab(2);
		mfdManager.SendInfoToItemTab(index);
	}

	void AddHardwareToInventory (int index) {
		int hwversion = heldObjectCustomIndex;
		if (hwversion < 0) {
			Const.sprint("Warning: Hardware picked up has no assigned versioning, defaulting to 1",player);
			hwversion = 1;
		}

		switch(index) {
			case 0:
				// System Analyzer
				if (hwversion <= HardwareInventory.a.hardwareVersion[0]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[0] = true;
				HardwareInventory.a.hardwareVersion[0] = hwversion;
			//HardwareInvCurrent.a.hardwareInvIndex
				Const.sprint(Const.a.useableItemsNameText[21] + " v" + hwversion.ToString(),player);
				break;
			case 1:
				// Navigation Unit
				if (hwversion <= HardwareInventory.a.hardwareVersion[1]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[1] = true;
				HardwareInventory.a.hardwareVersion[1] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[22] + " v" + hwversion.ToString(),player);
				compassContainer.SetActive(true); // Turn on HUD compass
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
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[2] = true;
				HardwareInventory.a.hardwareVersion[2] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[23] + " v" + hwversion.ToString(),player);
				hardwareButtons[5].SetActive(true);  // Enable HUD button
				break;
			case 3:
				// Sensaround
				if (hwversion <= HardwareInventory.a.hardwareVersion[3]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[3] = true;
				HardwareInventory.a.hardwareVersion[3] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[24] + " v" + hwversion.ToString(),player);
				hardwareButtons[1].SetActive(true);  // Enable HUD button
				break;
			case 4:
				// Target Identifier
				if (hwversion <= HardwareInventory.a.hardwareVersion[4]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[4] = true;
				HardwareInventory.a.hardwareVersion[4] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[25] + " v" + hwversion.ToString(),player);
				break;
			case 5:
				// Energy Shield
				if (hwversion <= HardwareInventory.a.hardwareVersion[5]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[5] = true;
				HardwareInventory.a.hardwareVersion[5] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[26] + " v" + hwversion.ToString(),player);
				hardwareButtons[3].SetActive(true);  // Enable HUD button
				break;
			case 6:
				// Biomonitor
				if (hwversion <= HardwareInventory.a.hardwareVersion[6]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[6] = true;
				HardwareInventory.a.hardwareVersion[6] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[27] + " v" + hwversion.ToString(),player);
				hardwareButtons[0].SetActive(true);  // Enable HUD button
				break;
			case 7:
				// Head Mounted Lantern
				if (hwversion <= HardwareInventory.a.hardwareVersion[7]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[7] = true;
				HardwareInventory.a.hardwareVersion[7] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[28] + " v" + hwversion.ToString(),player);
				hardwareButtons[2].SetActive(true);  // Enable HUD button
				break;
			case 8:
				// Envirosuit
				if (hwversion <= HardwareInventory.a.hardwareVersion[8]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[8] = true;
				HardwareInventory.a.hardwareVersion[8] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[29] + " v" + hwversion.ToString(),player);
				break;
			case 9:
				// Turbo Motion Booster
				if (hwversion <= HardwareInventory.a.hardwareVersion[9]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[9] = true;
				HardwareInventory.a.hardwareVersion[9] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[30] + " v" + hwversion.ToString(),player);
				hardwareButtons[6].SetActive(true);  // Enable HUD button
				break;
			case 10:
				// Jump Jet Boots
				if (hwversion <= HardwareInventory.a.hardwareVersion[10]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[10] = true;
				HardwareInventory.a.hardwareVersion[10] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[31] + " v" + hwversion.ToString(),player);
				hardwareButtons[7].SetActive(true);  // Enable HUD button
				break;
			case 11:
				// Infrared Night Vision Enhancement
				if (hwversion <= HardwareInventory.a.hardwareVersion[11]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.",player);
					return;
				}
				HardwareInventory.a.hasHardware[11] = true;
				HardwareInventory.a.hardwareVersion[11] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[32] + " v" + hwversion.ToString(),player);
				hardwareButtons[4].SetActive(true);  // Enable HUD button
				break;
		}
		if (firstTimePickup) {
			firstTimePickup = false;
			centerTabButtonsControl.TabButtonClickSilent (1,true);
		}

		HardwareInventory.a.hwButtonsManager.ActivateHardwareButton(index);
		mfdManager.SendInfoToItemTab(index);
		centerTabButtonsControl.NotifyToCenterTab(1);
	}

	void AddItemToInventory (int index) {
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
			    AddGrenadeToInventory(0); // Frag
			    break;
		    case 8:
			    AddGrenadeToInventory(3); // Concussion
			    break;
		    case 9:
			    AddGrenadeToInventory(1); // EMP
			    break;
		    case 10:
			    AddGrenadeToInventory(6); // Earth Shaker
			    break;
		    case 11:
			    AddGrenadeToInventory(4); // Land Mine
			    break;
		    case 12:
			    AddGrenadeToInventory(5); // Nitropak
			    break;
		    case 13:
			    AddGrenadeToInventory(2); // Gas
			    break;
		    case 14:
			    AddPatchToInventory(2);
			    break;
		    case 15:
			    AddPatchToInventory(6);
			    break;
		    case 16:
			    AddPatchToInventory(5);
			    break;
		    case 17:
			    AddPatchToInventory(3);
			    break;
		    case 18:
			    AddPatchToInventory(4);
			    break;
		    case 19:
			    AddPatchToInventory(1);
			    break;
		    case 20:
			    AddPatchToInventory(0);
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
			case 113:
				AddAmmoToInventory(12, Const.a.magazinePitchCountForWeapon[12], false); // rubber slugs
				break;
        }
		SFXSource.PlayOneShot(pickclip);
		firstTimePickup = false;
	}

	public void DropHeldItem() {
		if (heldObjectIndex < 0 || heldObjectIndex > 254) {
			Const.sprint("BUG: Attempted to DropHeldItem with index out of bounds (<0 or >255) and heldObjectIndex = " + heldObjectIndex.ToString(),player);
			return;
		}

		if (!grenadeActive) heldObject = Const.a.useableItems[heldObjectIndex]; // set by UseGrenade()
		if (heldObject != null) {
			GameObject tossObject = null;
			bool freeObjectInPoolFound = false;
			GameObject levelDynamicContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
			if (levelDynamicContainer == null) {
				Const.sprint("BUG: Failed to find dynamicObjectContainer for level: " + LevelManager.a.currentLevel.ToString(),player);
				return;
			}
			// Fnd any free inactive objects within the level's Levelnumber.Dynamic container and activate those before instantiating
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
				tossObject.transform.SetParent(levelDynamicContainer.transform,true);
				tossObject.GetComponent<Rigidbody>().velocity = transform.forward * tossForce; // TODO modify velocity direction and tossForce when tossing in InventoryMode to use 2D cursor movement vector to 3D yaw and pitch and speed of cursor movement to force
				tossObject.GetComponent<UseableObjectUse>().customIndex = heldObjectCustomIndex;
				tossObject.GetComponent<UseableObjectUse>().ammo = heldObjectAmmo;
			} else {
				// Throw an active grenade
				grenadeActive = false;
				tossObject = Instantiate(heldObject,(transform.position + (transform.forward * tossOffset)),Quaternion.identity) as GameObject;  //effect
				if (tossObject == null) {
					Const.sprint("BUG: Failed to instantiate object being dropped!",player);
					return;
				}
				tossObject.transform.SetParent(levelDynamicContainer.transform,true);
				GrenadeActivate ga = tossObject.GetComponent<GrenadeActivate>();
				if (ga != null) {
					ga.Activate(heldObjectIndex, grenadeCurrent); // time to boom
				}
				mouseCursor.liveGrenade = false;
			}
		} else {
			Const.sprint("Warning: Object "+heldObjectIndex.ToString()+" not assigned, vaporized.",player);
		}
	}

	public void ResetHeldItem() {
		//yield return new WaitForSeconds(0.05f);
		heldObjectIndex = -1;
		heldObjectCustomIndex = -1;
		heldObjectAmmo = 0;
		heldObjectAmmoIsSecondary = false;
		holdingObject = false;
		mouseCursor.justDroppedItemInHelper = true;
	}

	public void ResetCursor () {
        if (mouseCursor != null) {
            cursorTexture = cursorDefaultTexture;
            mouseCursor.cursorImage = cursorTexture;
			mouseCursor.liveGrenade = false;
        } else {
			Const.sprint("Warning: Could Not Find object 'MouseCursorHandler' in scene\n",Const.a.allPlayers);
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

		// Fill container with random items, overrides manually entered data
		// UPDATE: Handled on SearchableItem's awake to prevent save, search, load, search, repeat cheat/hack to get whatever you want
		/*if (curSearchScript.generateContents) {
			if (curSearchScript.lookUpIndex >= 0) {
				// Refer to lookUp tables
				// TODO: create lookUp tables for generic randomized search items such as NPCs
			} else {
				// Use random items chosen
				for (int movingIndex=0;movingIndex<curSearchScript.numSlots;movingIndex++) {
					if (Random.Range(0f,1f) < 0.5f) {
						curSearchScript.contents[movingIndex] = curSearchScript.randomItem[movingIndex];
						curSearchScript.customIndex[movingIndex] = curSearchScript.randomItemCustomIndex[movingIndex];
						movingIndex++;
					} else {
						curSearchScript.contents[movingIndex] = -1;
						curSearchScript.customIndex[movingIndex] = -1;
						movingIndex++;
					}
				}
			}
		}*/

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
			mfdManager.OpenTab (4, true, MFDManager.TabMSG.Search, -1,MFDManager.handedness.LeftHand);
		}
		MFDManager.a.SendSearchToDataTab(curSearchScript.objectName, numberFoundContents, resultContents, resultCustomIndex,currentSearchItem.transform.position);
		ForceInventoryMode();
	}

	// Returns string for describing the walls/floors/etc. based on the material name
	/*
	string GetTextureDescription (string material_name){
		string retval = System.String.Empty; // temporary string to hold the return value

		switch(material_name) {
		case "bridg2_3": retval = "monitoring post"; break;
		case "bridg2_4": retval = "video observation screen"; break;
		case "bridg2_5": retval = "cyber station"; break;
		case "bridg2_6": retval = "burnished platinum panelling"; break;
		case "bridg2_7": retval = "burnished platinum panelling"; break;
		case "bridg2_8": retval = "SHODAN neural bud"; break;
		case "bridg2_9": retval = "computer"; break;
		case "cabinet": retval = "cabinet"; break;
		case "charge_stat": retval = "energy charge station"; break;
		case "citmat1_1": retval = "CPU node"; break;
		case "citmat1_2": retval = "chair"; break;
		case "citmat1_3": retval = "catwalk"; break;
		case "citmat1_4": retval = "catwalk"; break;
		case "citmat1_5": retval = "surface"; break;
		case "citmat1_6": retval = "cabinet"; break;
		case "citmat1_7": retval = "catwalk"; break;
		case "citmat1_8": retval = "table top"; break;
		case "citmat1_9": retval = "catwalk"; break;
		case "citmat2_1": retval = "catwalk"; break;
		case "citmat2_2": retval = "cabinet"; break;
		case "citmat2_3": retval = "cabinet"; break;
		case "citmat2_4": retval = "cabinet"; break;
		case "console1_1": retval = "computer"; break;
		case "console1_2": retval = "computer"; break;
		case "console1_3": retval = "cart"; break;
		case "console1_4": retval = "computer"; break;
		case "console1_5": retval = "computer"; break;
		case "console1_6": retval = "console"; break;
		case "console1_7": retval = "console"; break;
		case "console1_8": retval = "console"; break;
		case "console1_9": retval = "console"; break;
		case "console2_1": retval = "console panel"; break;
		case "console2_2": retval = "desk"; break;
		case "console2_3": retval = "computer panel"; break;
		case "console2_4": retval = "computer panel"; break;
		case "console2_5": retval = "computer console"; break;
		case "console2_6": retval = "console controls"; break;
		case "console2_7": retval = "console"; break;
		case "console2_8": retval = "console controls"; break;
		case "console2_9": retval = "console"; break;
		case "console3_1": retval = "cyber space port"; break;
		case "console3_2": retval = "computer"; break;
		case "console3_3": retval = "computer"; break;
		case "console3_4": retval = "keyboard"; break;
		case "console3_5": retval = "computer panelling"; break;
		case "console3_6": retval = "normal screens"; break;
		case "console3_7": retval = "destroyed screen"; break;
		case "console3_8": retval = "desk"; break;
		case "console3_9": retval = "desk"; break;
		case "console4_1": retval = "console controls"; break;
		case "cyber": retval = "x-ray machine"; break;
		case "d_arrow1": retval = "repulsor lights"; break;
		case "d_arrow2": retval = "repulsor lights"; break;
		case "eng1_1": retval = "environmental regulator"; break;
		case "eng1_1d": retval = "atmospheric regulator"; break;
		case "eng1_2": retval = "fluid transport pipes"; break;
		case "eng1_2d": retval = "fluid transport pipes"; break;
		case "eng1_3": retval = "engineering panelling"; break;
		case "eng1_3d": retval = "fluid transport pipes"; break;
		case "eng1_4": retval = "ladder"; break;
		case "eng1_5": retval = "engineering panelling"; break;
		case "eng1_5d": retval = "panelling"; break;
		case "eng1_6": retval = "system function gauges"; break;
		case "eng1_6d": retval = "instruments"; break;
		case "eng1_7": retval = "engineering instruments"; break;
		case "eng1_7d": retval = "instruments"; break;
		case "eng1_8": retval = "electric cable access"; break;
		case "eng1_9": retval = "data circuit access port"; break;
		case "eng1_9d": retval = "data access ports"; break;
		case "eng2_1": retval = "hi-grip surface"; break;
		case "eng2_1d": retval = "hi-grip surface"; break;
		case "eng2_2": retval = "halogen light fixture"; break;
		case "eng2_2d": retval = "damaged light fixture"; break;
		case "eng2_3": retval = "observation station"; break;
		case "eng2_3d": retval = "instruments"; break;
		case "eng2_4": retval = "thick rug"; break;
		case "eng2_5": retval = "modular panelling"; break;
		case "exec1_1": retval = "soft panelling"; break;
		case "exec1_1d": retval = "burnt panelling"; break;
		case "exec1_2": retval = "tech-rack"; break;	
		case "exec1_2d": retval = "tech-rack"; break;
		case "exec2_1": retval = "corridor wall"; break;
		case "exec2_2": retval = "corridor wall"; break;
		case "exec2_2d": retval = "corridor wall"; break;
		case "exec2_3": retval = "oak panelling"; break;
		case "exec2_4": retval = "titanium panelling"; break;
		case "exec2_5": retval = "molybdenum panelling"; break;
		case "exec2_6": retval = "molybdenum panelling"; break;
		case "exec2_7": retval = "light fixture"; break;
		case "exec3_1": retval = "corridor wall"; break;
		case "exec3_1d": retval = "corridor wall"; break;
		case "exec3_2": retval = "corridor wall"; break;
		case "exec3_4": retval = "carpet"; break;
		case "exec4_1": retval = "automatic teller machine"; break;
		case "exec4_2": retval = "elevator panelling"; break;
		case "exec4_3": retval = "elevator panelling"; break;
		case "exec4_4": retval = "duct"; break;
		case "exec4_5": retval = "carpet"; break;
		case "exec4_6": retval = "marble slab"; break;
		case "exec6_1": retval = "display screen"; break;
		case "flight1_1": retval = "energ-light"; break;
		case "flight1_1b": retval = "energ-light"; break;
		case "flight1_2": retval = "non-dent steel panelling"; break;
		case "flight1_3": retval = "non-dent steel panelling"; break;
		case "flight1_4": retval = "non-dent steel panelling"; break;
		case "flight1_5": retval = "non-dent steel panelling"; break;
		case "flight1_6": retval = "environmental regulator"; break;
		case "flight2_1": retval = "grip surface"; break;
		case "flight2_2": retval = "energ-light"; break;
		case "flight2_3": retval = "energ-light"; break;
		case "grate1_1": retval = "grating"; break;
		case "grate1_2": retval = "grating"; break;
		case "grate1_3": retval = "grating"; break;
		case "grove1_1": retval = "observation ceiling"; break;
		case "grove1_2": retval = "grass"; break;
		case "grove1_3": retval = "grass"; break;
		case "grove1_4": retval = "wet grass"; break;
		case "grove1_5": retval = "virus infestation"; break;
		case "grove1_6": retval = "virus infestation"; break;
		case "grove1_7": retval = "virus infestation"; break;
		case "grove2_1": retval = "environment pod wall"; break;
		case "grove2_2": retval = "overgrowth"; break;
		case "grove2_3": retval = "environment pod wall"; break;
		case "grove2_4": retval = "overgrown panel"; break;
		case "grove2_5": retval = "environment regulator"; break;
		case "grove2_6": retval = "overgrowth"; break;
		case "grove2_7": retval = "sprinkler system"; break;
		case "grove2_8": retval = "overgrowth"; break;
		case "grove2_9": retval = "virus infestation"; break;
		case "grove2_9b": retval = "virus infestation"; break;
		case "grove2_9c": retval = "virus infestation"; break;
		case "maint1_1": retval = "industrial tiles"; break;
		case "maint1_2": retval = "storage area"; break;
		case "maint1_2d": retval = "storage area"; break;
		case "maint1_3": retval = "chemical storage"; break;
		case "maint1_3b": retval = "chemical dispensory"; break;
		case "maint1_4": retval = "repair station"; break;
		case "maint1_4b": retval = "repair station"; break;
		case "maint1_5": retval = "chemical dispensory"; break;
		case "maint1_6": retval = "robot diagnostic system"; break;
		case "maint1_7": retval = "repair station"; break;
		case "maint1_9": retval = "industrial tiles"; break;
		case "maint1_9d": retval = "industrial tiles"; break;
		case "maint2_1": retval = "quartz light fixture"; break;
		case "maint2_1b": retval = "ladder"; break;
		case "maint2_1d": retval = "quartz light fixture"; break;
		case "maint2_2": retval = "incandescent light"; break;
		case "maint2_3": retval = "grating"; break;
		case "maint2_3d": retval = "access station"; break;
		case "maint2_4": retval = "access station"; break;
		case "maint2_5": retval = "access station"; break;
		case "maint2_5d": retval = "fluid transport pipes"; break;
		case "maint2_6": retval = "fluid transport pipes"; break;
		case "maint2_6d": retval = "access station"; break;
		case "maint2_7": retval = "access station"; break;
		case "maint2_7d": retval = "fluid transport pipes"; break;
		case "maint2_8": retval = "fluid transport pipes"; break;
		case "maint2_9": retval = "power conduits"; break;
		case "maint3_1": retval = "duralloy panelling"; break;
		case "maint3_1d": retval = "duralloy panelling"; break;
		case "maint24_d": retval = "access station"; break;
		case "med1_1": retval = "soft panelling"; break;
		case "med1_1d": retval = "soft panelling"; break;
		case "med1_2": retval = "comm port"; break;
		case "med1_2d": retval = "comm port"; break;
		case "med1_3": retval = "environmental regulator"; break;
		case "med1_3d": retval = "environmental regulator"; break;
		case "med1_4": retval = "sof-impac panelling"; break;
		case "med1_5": retval = "flourescent light"; break;
		case "med1_6": retval = "flourescent light"; break;
		case "med1_7": retval = "tile panelling"; break;
		case "med1_7d": retval = "tile panelling"; break;
		case "med1_8": retval = "flourescent lighting"; break;
		case "med1_8d": retval = "flourescent lighting"; break;
		case "med1_9": retval = "flourescent lighting"; break;
		case "med1_9d": retval = "rubberized panelling"; break;
		case "med2_1": retval = "medical diagnostic tools"; break;
		case "med2_1d": retval = "clinical panelling"; break;
		case "med2_2": retval = "clinical panelling"; break;
		case "med2_2d": retval = "clinical panelling"; break;
		case "med2_3": retval = "clinical panelling"; break;
		case "med2_3d": retval = "clinical panelling"; break;
		case "med2_4": retval = "medical computer"; break;
		case "med2_5": retval = "healing incubator"; break;
		case "med2_6": retval = "clinical panelling"; break;
		case "med2_7": retval = "restoration bay"; break;
		case "med2_8": retval = "clinical panelling"; break;
		case "med2_9": retval = "environmental regulator"; break;
		case "med2_9d": retval = "environmental regulator"; break;
		case "med3_1": retval = "flood light"; break;
		case "rad1_1": retval = "cracked radiation tile"; break;
		case "rad1_2": retval = "cracked radiation tile"; break;
		case "reac1_1": retval = "molybdenum panelling"; break;
		case "reac1_2": retval = "power coupling"; break;
		case "reac1_3": retval = "halogen lighting"; break;
		case "reac1_4": retval = "circuit relay"; break;
		case "reac1_5": retval = "relay access port"; break;
		case "reac1_6": retval = "power monitor"; break;
		case "reac1_7": retval = "data transfer array"; break;
		case "reac1_8": retval = "diagnostic module"; break;
		case "reac1_9": retval = "comm panel"; break;
		case "reac2_1": retval = "energy conduits"; break;
		case "reac2_1b": retval = "energy conduits"; break;
		case "reac2_2": retval = "energy conduits"; break;
		case "reac2_4": retval = "energy conduits"; break;
		case "reac2_5": retval = "equipment storage"; break;
		case "reac2_6": retval = "energy conduits"; break;
		case "reac2_7": retval = "energy monitoring station"; break;
		case "reac2_8": retval = "rad observation console"; break;
		case "reac2_9": retval = "high energy transformer"; break;
		case "reac3_1": retval = "molybdenum panelling"; break;
		case "reac3_2": retval = "molybdenum panelling"; break;
		case "reac3_3": retval = "cable access port"; break;
		case "reac3_4": retval = "duct"; break;
		case "reac3_5": retval = "molybdenum panelling"; break;
		case "reac3_6": retval = "molybdenum panelling"; break;
		case "reac3_7": retval = "relay network"; break;
		case "reac4_1": retval = "sensor grid"; break;
		case "reac4_2": retval = "halogen lamp"; break;
		case "reac5_1": retval = "magnetic containment system"; break;
		case "reac5_2": retval = "magnetic containment system"; break;
		case "reac5_3": retval = "magnetic containment system"; break;
		case "reac6_1": retval = "hi-grip surface"; break;
		case "reac6_2": retval = "quartz light fixture"; break;
		case "reac6_3": retval = "duct"; break;
		case "sci1_1": retval = "aluminum panelling"; break;
		case "sci1_1d": retval = "aluminum panelling"; break;
		case "sci1_2": retval = "aluminum panelling"; break;
		case "sci1_2d": retval = "damaged panelling"; break;
		case "sci1_3": retval = "matter converter"; break;
		case "sci1_4": retval = "matter converter"; break;
		case "sci1_5": retval = "aluminum panelling"; break;
		case "sci1_6": retval = "aluminum panelling"; break;
		case "sci1_7": retval = "environmental regulator"; break;
		case "sci1_7d": retval = "instruments"; break;
		case "sci1_8": retval = "molecular analyzer"; break;
		case "sci1_8d": retval = "instrument panel"; break;
		case "sci1_9": retval = "flourescent lighting"; break;
		case "sci1_9d": retval = "flourescent lighting"; break;
		case "sci2_1": retval = "duct"; break;
		case "sci2_1d": retval = "duct"; break;
		case "sci2_2": retval = "environmental regulator"; break;
		case "sci2_2d": retval = "damaged regulator"; break;
		case "sci2_3": retval = "aluminum panelling"; break;
		case "sci2_4": retval = "aluminum panelling"; break;
		case "sci2_5": retval = "high-power light"; break;
		case "sci2_5d": retval = "high-power light"; break;
		case "sci3_1": retval = "diagnostic panel"; break;
		case "sci3_1d": retval = "instrument panel"; break;
		case "sci3_2": retval = "composite panelling"; break;
		case "sci3_3": retval = "diagnostic panel"; break;
		case "sci3_4": retval = "data conduit"; break;
		case "sci3_5": retval = "atmospheric regulator"; break;
		case "sci3_6": retval = "comm port"; break;
		case "sec1_1": retval = "trioptimum logo"; break;
		case "sec1_1b": retval = "trioptimum logo"; break;
		case "sec1_1c": retval = "obsidian slab"; break;
		case "sec1_2": retval = "silver panelling"; break;
		case "sec1_2b": retval = "silver panelling"; break;
		case "sec1_3": retval = "light fixture"; break;
		case "stor1_1": retval = "no-scrape storeroom wall"; break;
		case "stor1_2": retval = "no-scrape storeroom wall"; break;
		case "stor1_3": retval = "no-scrape storeroom wall"; break;
		case "stor1_4": retval = "no-scrape storeroom wall"; break;
		case "stor1_5": retval = "no-scrape storeroom wall"; break;
		case "stor1_6": retval = "structural pillar"; break;
		case "stor1_7": retval = "industrial tiles"; break;
		case "stor1_7d": retval = "industrial tiles"; break;
		default: retval = System.String.Empty; break;
		}
		return retval;
	}
*/
	static Mesh GetMesh(GameObject go) {
		if (go) {
			MeshFilter mf = go.GetComponent<MeshFilter>();
			if (mf) {
				Mesh m = mf.sharedMesh;
				if (!m)
					m = mf.mesh;

				if (m)
					return m;
			}
		}
		return (Mesh)null;
	}

	public void UseGrenade (int index) {
		if (heldObject) return;
		ForceInventoryMode();  // inventory mode is turned on when picking something up
		ResetHeldItem();
		holdingObject = true;
		heldObjectIndex = index;
		mouseCursor.cursorImage = Const.a.useableItemsFrobIcons[index];
		mouseCursor.liveGrenade = true;
		grenadeActive = true;
		
		switch(index) {
			case 7: heldObject = Const.a.useableItems[154]; break; // Frag
			case 8: heldObject = Const.a.useableItems[150]; break; // Concussion
			case 9: heldObject = Const.a.useableItems[152]; break; // EMP
			case 10: heldObject = Const.a.useableItems[151]; break; // Earth Shaker
			case 11: heldObject = Const.a.useableItems[156]; break; // Land Mine
			case 12: heldObject = Const.a.useableItems[157]; break; // Nitropak
			case 13: heldObject = Const.a.useableItems[155]; break; // Gas
		}
	}

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
	}
}