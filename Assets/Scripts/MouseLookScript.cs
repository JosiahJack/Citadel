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
    private float xRotation;
    private float zRotation;
    private float yRotationV;
    private float xRotationV;
    private float zRotationV;
    private float currentZRotation;
    private string mlookstring1;
    private GameObject currentSearchItem;
    private Camera playerCamera;
    private GameObject heldObject;
    private GameObject mouseCursor;
    private bool itemAdded = false;
	private int indexAdjustment;

    // External to Prefab
    // ------------------------------------------------------------------------
    [Tooltip("Game object that houses the MFD tabs")]
	public GameObject tabControl;
	[Tooltip("Text at the top of the data tab in the MFD")]
	public Text dataTabHeader;
	[Tooltip("Text in the data tab in the MFD that displays when searching an object containing no items")]
	public Text dataTabNoItemsText;
	public GameObject searchFX;
	public AudioSource SFXSource;
	public GameObject searchOriginContainer;
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject itemiconman;
	[SerializeField] private GameObject itemtextman;
	[SerializeField] private GameObject ammoiconman;
	[SerializeField] private GameObject weptextman;
	[SerializeField] private GameObject ammoClipBox;
	[HideInInspector]
	public GameObject currentButton;
    public GameObject weaponButtonsManager;
    public GameObject mainInventory;

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
    }

	void Update (){
        Cursor.visible = false; // Hides hardware cursor so we can show custom cursor textures

        //if (transform.parent.GetComponent<PlayerMovement>().grounded)
        //headbobStepCounter += (Vector3.Distance(parentLastPos, transform.parent.position) * headbobSpeed);

        //transform.localPosition.x = (Mathf.Sin(headbobStepCounter) * headbobAmountX);
        //transform.localPosition.y = (Mathf.Cos(headbobStepCounter * 2) * headbobAmountY * -1) + (transform.localScale.y * eyeHeightRatio) - (transform.localScale.y / 2);
        //parentLastPos = transform.parent.position;
        if (inventoryMode == false) {
			yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
			xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
			xRotation = Mathf.Clamp(xRotation, -90, 90);  // Limit up and down angle. TIP:: Need to disable for Cyberspace!
			transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
		}

		// Toggle inventory mode<->shoot mode
		if(Input.GetKeyDown(KeyCode.Tab))
			ToggleInventoryMode();

		// Frob if the cursor is not on the UI
		if (!GUIState.isBlocking) {
			currentButton = null;
			if(Input.GetMouseButtonDown(1)) {
				if (!holdingObject) {
					// Send out Frob raycast
					RaycastHit hit = new RaycastHit();
					if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, frobDistance)) {
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
				} else {
					// Drop the object we are holding
					if (heldObjectIndex != -1) {
						DropHeldItem();
						ResetHeldItem();
						ResetCursor();
					}
				}
			}
		} else {
			//We are holding cursor over the GUI
			if(Input.GetMouseButtonDown(1)) {
				if (holdingObject) {
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
						// 77 Center tabs button

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
                        }
                        mouseCursor.GetComponent<MouseCursor>().cursorImage = cursorTexture;
                        Cursor.lockState = CursorLockMode.None;
                        inventoryMode = true;  // inventory mode turned on
                        holdingObject = true;
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
			case 38:
				AddWeaponToInventory(38);
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
		GameObject tossObject = Instantiate(heldObject,(transform.position + (transform.forward * tossOffset)),Quaternion.identity) as GameObject;  //effect
		tossObject.GetComponent<Rigidbody>().velocity = transform.forward * tossForce;
	}

	void ResetHeldItem() {
		//yield return new WaitForSeconds(0.05f);
		heldObjectIndex = -1;
		holdingObject = false;
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
			Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            inventoryMode = false;
		} else {
			Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            inventoryMode = true;
		}
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

		// Enable search scaling box effect
		searchOriginContainer.GetComponent<RectTransform>().position = Input.mousePosition;
		searchFX.SetActive(true);
		searchFX.GetComponent<Animation>().Play();

		// Set header text on data tab
		dataTabHeader.text = currentSearchItem.GetComponent<SearchableItem>().objectName;

		// Turn off the text that displays "No Items" by default
		dataTabNoItemsText.enabled = false;

		int numberFoundContents = 0;

		for (int i=currentSearchItem.GetComponent<SearchableItem>().numSlots - 1;i>=0;i--) {
			if (currentSearchItem.GetComponent<SearchableItem>().contents[i] != null)
				numberFoundContents++;
		}

		if (numberFoundContents <=0)
			dataTabNoItemsText.enabled = true;  // show that there was nothing found in search

		// Change last active MFD tab (RH or LH depending on which was used last) to Data tab to show search contents
		SetActiveTab(4);
	}

	public void SetActiveTab (int tabIndex) {
		if (tabIndex < 0 || tabIndex > 4) {
			Const.sprint("BUG: tabIndex outside of bounds (0 to 4) sent to MouseLookScript.SetActiveTab()");
			return;
		}

		if (tabControl.GetComponent<TabButtonsScript>().curTab != tabIndex)
			tabControl.GetComponent<TabButtonsScript>().TabButtonClickSilent(tabIndex);
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
}