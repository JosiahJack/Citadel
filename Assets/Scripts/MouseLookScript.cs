using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MouseLookScript : MonoBehaviour {
    // Internal to Prefab
    // ------------------------------------------------------------------------
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
    [HideInInspector]
    public float yRotation;
	[Tooltip("Shows what button type cursor is over")]
    public int overButtonType;
	[Tooltip("Shows whether mouse cursor is over a button (to block shooting through UI)")]
    public bool overButton;
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

    // External to Prefab
    // ------------------------------------------------------------------------
	public GameObject canvasContainer;
    [Tooltip("Game object that houses the MFD tabs")]
	public GameObject tabControl;
	[Tooltip("Text in the data tab in the MFD that displays when searching an object containing no items")]
	public Text dataTabNoItemsText;
	public DataTab dataTabControl;
	public AudioSource SFXSource;
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject itemiconman;
	[SerializeField] private GameObject itemtextman;
	[SerializeField] private GameObject ammoiconman;
	[SerializeField] private GameObject weptextman;
	[SerializeField] private GameObject ammoClipBox;
	[HideInInspector]
	public GameObject currentButton;
	[HideInInspector]
	public GameObject currentSearchItem;
	[HideInInspector]
	public GameObject mouseCursor;
    public GameObject weaponButtonsManager;
    public GameObject mainInventory;
	public LogInventory logInventory;
	public GameObject[] hardwareButtons;
	public GameObject mainMenu;

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
        mouseCursor = GameObject.Find("MouseCursorHandler");
        if (mouseCursor == null)
            Const.sprint("BUG: Could Not Find object 'MouseCursorHandler' in scene");
		
        ResetCursor();
        Cursor.lockState = CursorLockMode.None;
		inventoryMode = true;  // Start with inventory mode turned on
		playerCamera = GetComponent<Camera>();
        overButton = false;
        overButtonType = -1;
		frobDistance = Const.a.frobDistance;
		holdingObject = false;
		heldObjectIndex = -1;
		yRotation = startyRotation;
		xRotation = startxRotation;

		if (canvasContainer == null)
			Const.sprint("BUG: No canvas given for camera to display UI");

		canvasContainer.SetActive(true); //enable UI
    }

	void Update () {
        Cursor.visible = false; // Hides hardware cursor so we can show custom cursor textures
		//Debug.Log("MouseLookScript:: Input.mousePosition.x: " + Input.mousePosition.x.ToString() + ", Input.mousePosition.y: " + Input.mousePosition.y.ToString());

        //if (transform.parent.GetComponent<PlayerMovement>().grounded)
        //headbobStepCounter += (Vector3.Distance(parentLastPos, transform.parent.position) * headbobSpeed);

        //transform.localPosition.x = (Mathf.Sin(headbobStepCounter) * headbobAmountX);
        //transform.localPosition.y = (Mathf.Cos(headbobStepCounter * 2) * headbobAmountY * -1) + (transform.localScale.y * eyeHeightRatio) - (transform.localScale.y / 2);
        //parentLastPos = transform.parent.position;
		if (mainMenu.activeSelf == true) return;  // ignore mouselook when main menu is still up

		if (Input.GetKeyUp("f6")) {
			Const.a.Save(7);
		}

		if (Input.GetKeyUp("f9")) {
			Const.a.Load(7);
		}

        if (inventoryMode == false) {
			if (PauseScript.a != null && !PauseScript.a.paused) {
				yRotation += (Input.GetAxis("Mouse X") * lookSensitivity);
				xRotation -= (Input.GetAxis("Mouse Y") * lookSensitivity);
				xRotation = Mathf.Clamp(xRotation, -90, 90);  // Limit up and down angle. TIP:: Need to disable clamp for Cyberspace!
				transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
			} else {
				Const.sprint("ERROR: Paused is true and inventoryMode is false");
			}
		} else {
			if (Input.GetButton("Yaw")) {
				if (!PauseScript.a.paused) {
					if  (Input.GetAxisRaw("Yaw") > 0) {
						yRotation += keyboardTurnSpeed * lookSensitivity;
						tempQuat = transform.rotation;
						transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
						transform.rotation = tempQuat; // preserve x axis, hacky
					} else {
						if (Input.GetAxisRaw("Yaw") < 0) {
							yRotation -= keyboardTurnSpeed * lookSensitivity;
							tempQuat = transform.rotation;
							transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
							transform.rotation = tempQuat; // preserve x axis, hacky
						}
					}
				}
			}
			
			if (Input.GetButton("Pitch")) {
				if (!PauseScript.a.paused) {
					if  (Input.GetAxisRaw("Pitch") > 0) {
						xRotation += keyboardTurnSpeed * lookSensitivity;
						transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
					} else {
						xRotation -= keyboardTurnSpeed * lookSensitivity;
						transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
					}
				}
			}
		}

		// Toggle inventory mode<->shoot mode
		if(GetInput.a.ToggleMode() && !PauseScript.a.paused)
			ToggleInventoryMode();

		// Frob if the cursor is not on the UI
		if (!GUIState.a.isBlocking) {
			if (!PauseScript.a.paused) {
				currentButton = null;
				if(GetInput.a.Use()) {
					if (!holdingObject) {
						// Send out Frob raycast
						RaycastHit hit = new RaycastHit();
						if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, frobDistance)) {
							//drawMyLine(playerCamera.transform.position,hit.point,Color.green,10f);
							// TIP: Use Camera.main.ViewportPointToRay for center of screen
							if (hit.collider == null)
								return;
						
							// Check if object is usable then use it
							if (hit.collider.tag == "Usable") {
								hit.transform.SendMessageUpwards("Use", gameObject); // send Use with self as owner of message
								return;
							}
					
							// Check if object is searchable then search it
							if (hit.collider.tag == "Searchable") {
								currentSearchItem = hit.collider.gameObject;
								SearchObject(currentSearchItem.GetComponent<SearchableItem>().lookUpIndex);
								return;
							}
					
					        // If we can't use it, Give info about what we are looking at (e.g. "Molybdenum panelling")
							Mesh m = GetMesh(hit.collider.gameObject);
							if (m) {
								int[] hittedTriangle = new int[] {
									m.triangles[hit.triangleIndex * 3], m.triangles[hit.triangleIndex * 3 + 1], m.triangles[hit.triangleIndex * 3 + 2]
								};
								for (int i=0;i<m.subMeshCount;i++) {
									int[] submeshTris = m.GetTriangles(i);
									for (int j=0;j<submeshTris.Length;j+=3) {
										if ((submeshTris[j] == hittedTriangle[0]) && (submeshTris[j+1] == hittedTriangle[1]) && (submeshTris[j+2] == hittedTriangle[2])) {
											mlookstring1 = "Can't use " + GetTextureDescription(hit.collider.gameObject.GetComponent<MeshRenderer>().materials[i].mainTexture.name);
										}
									}
								}
								Const.sprint(mlookstring1);
							}
						}
						if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, 50f)) {
							//drawMyLine(playerCamera.transform.position,hit.point,Color.green,10f);
							// TIP: Use Camera.main.ViewportPointToRay for center of screen
							if (hit.collider == null)
								return;

							// Check if object is usable then use it
							if (hit.collider.tag == "Usable" || hit.collider.tag == "Searchable") {
								Const.sprint("You are too far away from that");
								return;
							}
						}
					} else {
						// Drop the object we are holding
						if (heldObjectIndex != -1) {
							DropHeldItem();
							ResetHeldItem();
							ResetCursor();
						}
					}
				}
			}
		} else {
			//We are holding cursor over the GUI
			if(GetInput.a.Use()) {
				if (holdingObject && !PauseScript.a.paused) {
					AddItemToInventory(heldObjectIndex);
					ResetHeldItem();
					ResetCursor();
				} else {
					if (overButton && overButtonType != -1) {
                        // overButtonTypes:
                        // -1   Not over a button
                        // 0 WeaponButton
                        // 1 GrenadeButton
                        // 2 PatchButton
                        // 3 GeneralInventoryButton
						// 4 Search contents button
						if (!PauseScript.a.paused) {
	                        switch(overButtonType) {
	                            case 0:
	                                cursorTexture = Const.a.useableItemsFrobIcons[currentButton.GetComponent<WeaponButtonScript>().useableItemIndex];
	                                heldObjectIndex = currentButton.GetComponent<WeaponButtonScript>().useableItemIndex;
									indexAdjustment = currentButton.GetComponent<WeaponButtonScript>().WepButtonIndex;
									WeaponInventory.WepInventoryInstance.weaponInventoryIndices[indexAdjustment] = -1;
									WeaponInventory.WepInventoryInstance.weaponInventoryText[indexAdjustment] = "-";
									indexAdjustment--;
									if (indexAdjustment < 0)
										indexAdjustment = 0;
									WeaponCurrent.WepInstance.weaponCurrent = indexAdjustment;
									overButton = false;
									overButtonType = -1;
									break;
	                            case 1:
	                                cursorTexture = Const.a.useableItemsFrobIcons[currentButton.GetComponent<GrenadeButtonScript>().useableItemIndex];
	                                heldObjectIndex = currentButton.GetComponent<GrenadeButtonScript>().useableItemIndex;
	                                GrenadeInventory.GrenadeInvInstance.grenAmmo[currentButton.GetComponent<GrenadeButtonScript>().GrenButtonIndex]--;
	                                if (GrenadeInventory.GrenadeInvInstance.grenAmmo[currentButton.GetComponent<GrenadeButtonScript>().GrenButtonIndex] <= 0) {
										GrenadeInventory.GrenadeInvInstance.grenAmmo[currentButton.GetComponent<GrenadeButtonScript>().GrenButtonIndex] = 0; 
										for (int i = 0; i < 7; i++) {
	                                        if (GrenadeInventory.GrenadeInvInstance.grenAmmo[i] > 0) {
	                                            mainInventory.GetComponent<GrenadeCurrent>().grenadeCurrent = i;
	                                        }
	                                    }
	                                }
									break;
	                            case 2:
	                                cursorTexture = Const.a.useableItemsFrobIcons[currentButton.GetComponent<PatchButtonScript>().useableItemIndex];
	                                heldObjectIndex = currentButton.GetComponent<PatchButtonScript>().useableItemIndex;
									PatchInventory.PatchInvInstance.patchCounts[currentButton.GetComponent<PatchButtonScript>().PatchButtonIndex]--;
									if (PatchInventory.PatchInvInstance.patchCounts[currentButton.GetComponent<PatchButtonScript>().PatchButtonIndex] <= 0) {
										PatchInventory.PatchInvInstance.patchCounts[currentButton.GetComponent<PatchButtonScript>().PatchButtonIndex] = 0;
										for (int i = 0; i < 7; i++) {
											if (PatchInventory.PatchInvInstance.patchCounts[i] > 0) {
												mainInventory.GetComponent<PatchCurrent>().patchCurrent = i;
											}
										}
									}
									break;
	                            case 3:
	                                cursorTexture = Const.a.useableItemsFrobIcons[currentButton.GetComponent<GeneralInvButtonScript>().useableItemIndex];
	                                heldObjectIndex = currentButton.GetComponent<GeneralInvButtonScript>().useableItemIndex;
	                                GeneralInventory.GeneralInventoryInstance.generalInventoryIndexRef[currentButton.GetComponent<GeneralInvButtonScript>().GeneralInvButtonIndex] = -1;
	                                break;
								case 4:
									int tempButtonindex = currentButton.GetComponent<SearchContainerButtonScript>().refIndex;
									cursorTexture = Const.a.useableItemsFrobIcons[currentButton.GetComponentInParent<SearchButtonsScript>().contents[tempButtonindex]];
									heldObjectIndex = currentButton.GetComponentInParent<SearchButtonsScript>().contents[tempButtonindex];
									heldObjectCustomIndex = currentButton.GetComponentInParent<SearchButtonsScript>().customIndex[tempButtonindex];
									currentSearchItem.GetComponent<SearchableItem>().contents[tempButtonindex] = -1;
									currentSearchItem.GetComponent<SearchableItem>().customIndex[tempButtonindex] = -1;
									currentButton.GetComponentInParent<SearchButtonsScript>().contents[tempButtonindex] = -1;
									currentButton.GetComponentInParent<SearchButtonsScript>().customIndex[tempButtonindex] = -1;
									currentButton.GetComponentInParent<SearchButtonsScript>().GetComponentInParent<DataTab>().searchItemImages[tempButtonindex].SetActive(false);
									currentButton.GetComponentInParent<SearchButtonsScript>().CheckForEmpty();
									overButton = false;
									overButtonType = -1;
									break;
	                        }
							if (overButtonType != 77) {
	                       		mouseCursor.GetComponent<MouseCursor>().cursorImage = cursorTexture;
								ForceInventoryMode();
		                        holdingObject = true;
							}
						}
					}
				}
			}
		}
	}

    void AddGenericObjectToInventory(int index) {
		itemAdded = false; //prevent false positive
        for (int i=0;i<14;i++) {
            if (GeneralInventory.GeneralInventoryInstance.generalInventoryIndexRef[i] == -1) {
                GeneralInventory.GeneralInventoryInstance.generalInventoryIndexRef[i] = index;
				Const.sprint("Item added to general inventory");
                itemAdded = true;
                break;
            }
        }

        if (!itemAdded) {
            DropHeldItem();
            ResetHeldItem();
            ResetCursor();
            Const.sprint("Inventory full, item dropped");
			return;
        }
        mainInventory.GetComponent<GeneralInvCurrent>().generalInvCurrent = index;
    }

    void AddWeaponToInventory(int index) {
		itemAdded = false; //prevent false positive
        for (int i=0;i<7;i++) {
            if (WeaponInventory.WepInventoryInstance.weaponInventoryIndices[i] < 0) {
                WeaponInventory.WepInventoryInstance.weaponInventoryIndices[i] = index;
                WeaponInventory.WepInventoryInstance.weaponInventoryText[i] = WeaponInventory.WepInventoryInstance.weaponInvTextSource[(index - 36)];
                WeaponCurrent.WepInstance.weaponCurrent = i;
				WeaponCurrent.WepInstance.weaponIndex = index;
                weaponButtonsManager.GetComponent<WeaponButtonsManager>().wepButtons[i].GetComponent<WeaponButtonScript>().useableItemIndex = index;
				if (!ammoClipBox.activeInHierarchy)
					ammoClipBox.SetActive(true);
				
				ammoiconman.GetComponent<AmmoIconManager>().SetAmmoIcon(index, false);
				iconman.GetComponent<WeaponIconManager>().SetWepIcon(index);    //Set weapon icon for MFD
				weptextman.GetComponent<WeaponTextManager>().SetWepText(index); //Set weapon text for MFD
				itemiconman.SetActive(false);    //Set weapon icon for MFD
				itemtextman.GetComponent<ItemTextManager>().SetItemText(index); //Set weapon text for MFD
				Const.sprint("Weapon added to inventory");
				itemAdded = true;
                break;
            }
        }
		if (!itemAdded) {
			DropHeldItem();
			ResetHeldItem();
			ResetCursor();
			Const.sprint("Inventory full, item dropped");
			return;
		}
    }

    void AddGrenadeToInventory (int index) {
		GrenadeInventory.GrenadeInvInstance.grenAmmo[index]++;
		GrenadeCurrent.GrenadeInstance.grenadeCurrent = index;
		Const.sprint("Grenade added to inventory");
    }

	void AddPatchToInventory (int index) {
		PatchInventory.PatchInvInstance.patchCounts[index]++;
		PatchCurrent.PatchInstance.patchCurrent = index;
		Const.sprint("Patch added to inventory");
    }

	void AddAudioLogToInventory () {
		if ((heldObjectCustomIndex != -1) && (logInventory != null)) {
			logInventory.hasLog[heldObjectCustomIndex] = true;
			logInventory.lastAddedIndex = heldObjectCustomIndex;
			int levelnum = Const.a.audioLogLevelFound[heldObjectCustomIndex];
			logInventory.numLogsFromLevel[levelnum]++;
			string audName = Const.a.audiologNames[heldObjectCustomIndex];
			string logPlaybackKey = "u"; // TODO add code for handling custom key
			Const.sprint("Audio log " + audName + " picked up.  Press '" + logPlaybackKey + "' to playback.");
		} else {
			Const.sprint("Warning: Audio log picked up has no assigned index (-1)");
		}
	}

	void AddHardwareToInventory (int index) {
		int hwversion = heldObjectCustomIndex;
		if (hwversion < 0) {
			Const.sprint("Warning: Hardware picked up has no assigned versioning, defaulting to 1");
			hwversion = 1;
		}

		switch(index) {
			case 0:
				// System Analyzer
				if (hwversion <= HardwareInventory.a.hardwareVersion[0]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[0] = true;
				HardwareInventory.a.hardwareVersion[0] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[21] + " v" + hwversion.ToString());
				break;
			case 1:
				// Navigation Unit
				if (hwversion <= HardwareInventory.a.hardwareVersion[1]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[1] = true;
				HardwareInventory.a.hardwareVersion[1] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[22] + " v" + hwversion.ToString());
				//TODO add HUD compass; // Turn on HUD compass
				break;
			case 2:
				// Datareader
				if (hwversion <= HardwareInventory.a.hardwareVersion[2]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[2] = true;
				HardwareInventory.a.hardwareVersion[2] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[23] + " v" + hwversion.ToString());
				hardwareButtons[5].SetActive(true);  // Enable HUD button
				break;
			case 3:
				// Sensaround
				if (hwversion <= HardwareInventory.a.hardwareVersion[3]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[3] = true;
				HardwareInventory.a.hardwareVersion[3] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[24] + " v" + hwversion.ToString());
				hardwareButtons[1].SetActive(true);  // Enable HUD button
				break;
			case 4:
				// Target Identifier
				if (hwversion <= HardwareInventory.a.hardwareVersion[4]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[4] = true;
				HardwareInventory.a.hardwareVersion[4] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[25] + " v" + hwversion.ToString());
				break;
			case 5:
				// Energy Shield
				if (hwversion <= HardwareInventory.a.hardwareVersion[5]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[5] = true;
				HardwareInventory.a.hardwareVersion[5] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[26] + " v" + hwversion.ToString());
				hardwareButtons[3].SetActive(true);  // Enable HUD button
				break;
			case 6:
				// Biomonitor
				if (hwversion <= HardwareInventory.a.hardwareVersion[6]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[6] = true;
				HardwareInventory.a.hardwareVersion[6] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[27] + " v" + hwversion.ToString());
				hardwareButtons[0].SetActive(true);  // Enable HUD button
				break;
			case 7:
				// Head Mounted Lantern
				if (hwversion <= HardwareInventory.a.hardwareVersion[7]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[7] = true;
				HardwareInventory.a.hardwareVersion[7] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[28] + " v" + hwversion.ToString());
				hardwareButtons[2].SetActive(true);  // Enable HUD button
				break;
			case 8:
				// Envirosuit
				if (hwversion <= HardwareInventory.a.hardwareVersion[8]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[8] = true;
				HardwareInventory.a.hardwareVersion[8] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[29] + " v" + hwversion.ToString());
				break;
			case 9:
				// Turbo Motion Booster
				if (hwversion <= HardwareInventory.a.hardwareVersion[9]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[9] = true;
				HardwareInventory.a.hardwareVersion[9] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[30] + " v" + hwversion.ToString());
				hardwareButtons[6].SetActive(true);  // Enable HUD button
				break;
			case 10:
				// Jump Jet Boots
				if (hwversion <= HardwareInventory.a.hardwareVersion[10]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[10] = true;
				HardwareInventory.a.hardwareVersion[10] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[31] + " v" + hwversion.ToString());
				hardwareButtons[7].SetActive(true);  // Enable HUD button
				break;
			case 11:
				// Infrared Night Vision Enhancement
				if (hwversion <= HardwareInventory.a.hardwareVersion[11]) {
					Const.sprint("THAT WARE IS OBSOLETE. DISCARDED.");
					return;
				}
				HardwareInventory.a.hasHardware[11] = true;
				HardwareInventory.a.hardwareVersion[11] = hwversion;
				Const.sprint(Const.a.useableItemsNameText[32] + " v" + hwversion.ToString());
				hardwareButtons[4].SetActive(true);  // Enable HUD button
				break;
		}
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
			case 6:
				AddAudioLogToInventory();
				break;
            case 7:
			    AddGrenadeToInventory(0);
			    break;
		    case 8:
			    AddGrenadeToInventory(3);
			    break;
		    case 9:
			    AddGrenadeToInventory(1);
			    break;
		    case 10:
			    AddGrenadeToInventory(6);
			    break;
		    case 11:
			    AddGrenadeToInventory(4);
			    break;
		    case 12:
			    AddGrenadeToInventory(5);
			    break;
		    case 13:
			    AddGrenadeToInventory(2);
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
			case 38:
				AddWeaponToInventory(38);
				break;
			case 42:
				AddWeaponToInventory(42);
				break;
            case 44:
                AddWeaponToInventory(44);
                break;
            case 45:
                AddWeaponToInventory(45);
                break;
        }
		SFXSource.PlayOneShot(pickclip);
	}

	void DropHeldItem() {
		heldObject = Const.a.useableItems[heldObjectIndex];
		if (heldObject != null) {
			GameObject tossObject = null;
			bool freeObjectInPoolFound = false;
			GameObject levelDynamicContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
			if (levelDynamicContainer == null) {
				Const.sprint("BUG: Failed to find dynamicObjectContainer for level: " + LevelManager.a.currentLevel.ToString());
				return;
			}
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
			} else {
				tossObject = Instantiate(heldObject,(transform.position + (transform.forward * tossOffset)),Quaternion.identity) as GameObject;  //effect
				if (tossObject == null) {
					Const.sprint("BUG: Failed to instantiate object being dropped!");
					return;
				}
			}
			if (tossObject.activeSelf != true) {
				tossObject.SetActive(true);
			}
			tossObject.transform.SetParent(levelDynamicContainer.transform,true);
			tossObject.GetComponent<Rigidbody>().velocity = transform.forward * tossForce;
		} else {
			Const.sprint("Warning: Object "+heldObjectIndex.ToString()+" not assigned, vaporized.");
		}
	}

	void ResetHeldItem() {
		//yield return new WaitForSeconds(0.05f);
		heldObjectIndex = -1;
		heldObjectCustomIndex = -1;
		holdingObject = false;
		mouseCursor.GetComponent<MouseCursor>().justDroppedItemInHelper = true;
	}

	void ResetCursor () {
        if (mouseCursor != null) {
            cursorTexture = cursorDefaultTexture;
            mouseCursor.GetComponent<MouseCursor>().cursorImage = cursorTexture;
        } else {
            print("Warning: Could Not Find object 'MouseCursorHandler' in scene\n");
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
		//string randomItem;
		// Fill container with random items
		//if (currentSearchItem.GetComponent<SearchableItem>().contents[0] == null) {
		//	if (currentSearchItem.GetComponent<SearchableItem>().generateContents) {
		//		switch(index) {
		//		case 0: randomItem = "";
		//			break;
		//		case 1: randomItem = "";
		//			break;
		//		default:randomItem = "";
		//			break;
		//		}
		//	}
		//}

		// Play search sound
		SFXSource.PlayOneShot(SearchSFX);

		// Search through array to see if any items are in the container
		int numberFoundContents = 0;
		int[] resultContents = {-1,-1,-1,-1};  // create blanked container for search results
		int[] resultCustomIndex = {-1,-1,-1,-1};  // create blanked container for search results custom indices
		for (int i=currentSearchItem.GetComponent<SearchableItem>().numSlots - 1;i>=0;i--) {
			//Const.sprint("Search index = " + i.ToString() + ", and SearchableItem.customIndex.Length = " + currentSearchItem.GetComponent<SearchableItem>().customIndex.Length.ToString());
			resultContents[i] = currentSearchItem.GetComponent<SearchableItem>().contents[i];
			resultCustomIndex[i] = currentSearchItem.GetComponent<SearchableItem>().customIndex[i];
			if (resultContents[i] > -1) {
				numberFoundContents++; // if something was found, add 1 to count
			}
		}

		MFDManager.a.SendSearchToDataTab(currentSearchItem.GetComponent<SearchableItem>().objectName, numberFoundContents, resultContents, resultCustomIndex);
		ForceInventoryMode();
	}

	// Returns string for describing the walls/floors/etc. based on the material name
	string GetTextureDescription (string material_name){
		string retval = ""; // temporary string to hold the return value

		if (material_name.StartsWith("+"))
			retval = "normal screens";
		
		if (material_name.Contains("fan"))
			retval = "field generation rotor";

		// handle wierd case of animated texture having same name as plain one, oops.
		if ((material_name.StartsWith("+")) &&(material_name.Contains("eng2_5"))) {
			retval = "power exchanger";  
			return retval;
		}
		
		if (material_name.Contains("lift"))
			retval = "repulsor lift";
		
		if (material_name.Contains("alert"))
			retval = "warning indicator";
		
		if (material_name.Contains("telepad"))
			retval = "jump disk";
		
		if (material_name.Contains("crate"))
			retval = "storage crate";
		
		switch(material_name) {
		case "bridg1_2": retval = "biological infestation"; break;
		case "bridg1_3": retval = "biological infestation"; break;
		case "bridg1_3b": retval = "biological infestation"; break;
		case "bridg1_4": retval = "biological infestation"; break;
		case "bridg1_5": retval = "data transfer schematic"; break;
		case "bridg2_1": retval = "monitoring port"; break;
		case "bridg2_2": retval = "stone mosaic tiling"; break;
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
		case "eng1_9d": retval = "data access porta"; break;
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
		default: retval = ""; break;
		}
		return retval;
	}

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

	void drawMyLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		StartCoroutine( drawLine(start, end, color, duration));
	}

	IEnumerator drawLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		GameObject myLine = new GameObject ();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer> ();
		LineRenderer lr = myLine.GetComponent<LineRenderer> ();
		lr.material = new Material (Shader.Find ("Particles/Additive"));
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