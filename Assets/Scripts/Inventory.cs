using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
	// Access Cards
	[HideInInspector] public AccessCardType[] accessCardsOwned; // save
	private AccessCardType doorAccessTypeAcquired;

	// Hardware
	[HideInInspector] public bool[] hasHardware; // save
	[HideInInspector] public int[] hardwareVersion; // save
	[HideInInspector] public int[] hardwareVersionSetting; // save
	public GameObject[] hwButtons; // The UI buttons in the mfd center tab's Hardware tab.
	public HardwareButton hardwareButtonManager;
	[HideInInspector] public int hardwareInvCurrent; // save, Current slot in the general inventory (14 slots).
	[HideInInspector] public int hardwareInvIndex; // save, Current index to the item look-up table.
	[HideInInspector] public bool[] hardwareIsActive; // save
	public Text[] hardwareInvText;
	private int[] hardwareInvReferenceIndex;

	// General
	public GameObject[] genButtons;
	public Text[] genButtonsText;
    [HideInInspector] public int[] generalInventoryIndexRef; // save
	[HideInInspector] public int generalInvCurrent = new int(); // save, Current slot in the general inventory (14 slots).
	[HideInInspector] public int generalInvIndex = new int(); // save, Current index to the item look-up table.

	// Grenade
	[HideInInspector] public int[] grenAmmo; // save
	public GrenadeButton[] grenButtons;
	public Text[] grenInventoryText;
	public Text[] grenCountsText;
	public CapsuleCollider playerCapCollider;
	[HideInInspector] public int grenadeCurrent = new int(); // save
	[HideInInspector] public int grenadeIndex = new int(); // save
	[HideInInspector] public float nitroTimeSetting; // save
	[HideInInspector] public float earthShakerTimeSetting; // save
	private int[] grenCountsLastCount;

	// Logs
	public GameObject vmailbetajet;
	public GameObject vmailbridgesep;
	public GameObject vmailcitadestruct;
	public GameObject vmailgenstatus;
	public GameObject vmaillaserdest;
	public GameObject vmailshieldsup;
	public AudioSource SFXSource;
	[HideInInspector] public bool[] hasLog; // save
	[HideInInspector] public bool[] readLog; // save
	[HideInInspector] public int[] numLogsFromLevel; // save
	[HideInInspector] public int lastAddedIndex = -1; // save
	[HideInInspector] public bool beepDone = false; // save
	private AudioClip SFXClip;
	private int tempRefIndex = -1;
	private bool logPaused;
	[HideInInspector] public int emailCurrent; // save
	[HideInInspector] public int emailIndex; // save

	// Patches
	public GameObject patchInventoryContainer;
	public Text[] patchInventoryText;
	public Text[] patchCountTextObjects;
	public PatchButton[] patchButtonScripts;
	[HideInInspector] public int[] patchCounts;
	[HideInInspector] public int patchCurrent = new int(); // save, Current slot in the patch inventory (7 slots).
	[HideInInspector] public int patchIndex = new int(); // save, Current index to the look-up tables.
	private int[] patchLastCount;

	// Software
	public GameObject[] softs;
	public AudioSource SFX;
	public AudioClip SFXChangeWeapon;
	public AudioClip SFXAcquireCyberItem;
	public AudioClip SFXAcquireCyberData;
	public HealthManager hm;
	public SoftwareInvText pulserButtonText;
	public SoftwareInvText drillButtonText;
	public Text drillVersionText;
	public Text pulserVersionText;
	public Text cshieldVersionText;
	public Text turboCountText;
	public Text decoyCountText;
	public Text recallCountText;
	public GameObject decoyPrefab;
	public GameObject CyberSpaceStaticContainer;
	[HideInInspector] public int currentCyberItem = -1; // save
	[HideInInspector] public bool isPulserNotDrill = true; // save
	[HideInInspector] public int[] softVersions; // save
	[HideInInspector] public bool[] hasSoft; // save

	// Weapons
	[HideInInspector] public string[] weaponInventoryText;
	[HideInInspector] public int[] weaponInventoryIndices; // save
    [HideInInspector] public int[] weaponInventoryAmmoIndices; // save
	[HideInInspector] public string[] weaponInvTextSource;
	[HideInInspector] public int numweapons = 0; // save
	[HideInInspector] public int[] wepAmmo; // save
	[HideInInspector] public int[] wepAmmoSecondary; // save
	[HideInInspector] public float[] currentEnergyWeaponHeat; // save
	[HideInInspector] public bool[] wepLoadedWithAlternate; // save
	private int globalLookupIndex;
	private string retval;
	private string scorpSmall = "sm, ";
	private string scorpLg = "lg";
	public static string[] weaponShotsInventoryText = new string[]{
		"RELOAD","n 8","s 20","","OK","","" // Merely placeholder for checking.
	};
	public Text[] weaponShotsInventory;
	public Text[] weaponButtonText;

	// Singleton instance
	public static Inventory a;

	// Index cheat sheets
	//-------------------------------------------------------------------------
	// 0 = System Analyzer
	// 1 = Navigation Unit
	// 2 = Datareader/EReader
	// 3 = Sensaround
	// 4 = Target Identifier
	// 5 = Energy Shield
	// 6 = Biomonitor
	// 7 = Head Mounted Lantern
	// 8 = Envirosuit
	// 9 = Turbo Motion Booster
	//10 = Jump Jet Boots
	//11 = Infrared Night Sight Enhancement
	//12 = Tractor Beam
	//13 = Fry Salter

	// Hw referenceIndex, ref14Index
	// Sys 21,0
	// Nav 22,1
	// Ere 23,2
	// Sen 24,3
	// Trg 25,4
	// Shi 26,5
	// Bio 27,6
	// Lan 28,7
	// Env 29,8
	// Boo 30,9
	// Jum 31,10
	// Nig 32,11
	public int hardware14fromConstdex (int constdex) {
		switch (constdex) {
			case 21: return 0;
			case 22: return 1;
			case 23: return 2;
			case 24: return 3;
			case 25: return 4;
			case 26: return 5;
			case 27: return 6;
			case 28: return 7;
			case 29: return 8;
			case 30: return 9;
			case 31: return 10;
			case 32: return 11;
		}
		return 0; // Using zero in case I pass this straight into the ever dangerous [ ]
	}

	public int hardwareConstdexfrom14 (int dex14) {
		switch (dex14) {
			case 0: return 21;
			case 1: return 22;
			case 2: return 23;
			case 3: return 24;
			case 4: return 25;
			case 5: return 26;
			case 6: return 27;
			case 7: return 28;
			case 8: return 29;
			case 9: return 30;
			case 10: return 31;
			case 11: return 32;
			case 12: return 0;
			case 13: return 0;
		}
		return 0; // Using zero in case I pass this straight into the ever dangerous [ ]
	}

	void Awake() {
		a = this;

		// Access Cards
		a.accessCardsOwned = new AccessCardType[32];
		for (int i = 0; i < a.accessCardsOwned.Length; i++) {
			a.accessCardsOwned[i] = AccessCardType.None;
		}

		// Hardware
		a.hasHardware = new bool[14];
		a.hardwareVersion = new int[14];
		a.hardwareVersionSetting = new int[14];
		a.hardwareIsActive = new bool[14];
		a.hardwareInvReferenceIndex = new int[]{21,22,23,24,25,26,27,28,29,30,31,32,0,0}; // Hardcoded lookup indices into the Const main table.
		for (int i = 0; i < a.hasHardware.Length; i++) {
			a.hasHardware[i] = a.hardwareIsActive[i] = false; // Default to no hardware present.
			a.hardwareVersion[i] = a.hardwareVersionSetting[i] = 0; // Default to version 1 acquired for all hardware which is represented by 0.
		}
        a.hardwareInvCurrent = a.hardwareInvIndex = 0;

		// General
		a.generalInventoryIndexRef = new int[14];
        for (int i = 0; i < a.generalInventoryIndexRef.Length; i++) {
            a.generalInventoryIndexRef[i] = -1;
        }
        a.generalInvCurrent = a.generalInvIndex = 0;

		// Grenades
		a.grenAmmo = new int[7];
		a.grenCountsLastCount = new int[7];
		for (int i= 0; i<a.grenAmmo.Length; i++) {
			a.grenAmmo[i] = a.grenCountsLastCount[i] = 0;
		}
		a.grenadeCurrent = a.grenadeIndex = 0;
		a.nitroTimeSetting = Const.a.nitroDefaultTime;
		a.earthShakerTimeSetting = Const.a.earthShDefaultTime;

		// Logs
		a.hasLog = new bool[134];
		a.readLog = new bool[134];
        for (int i = 0; i < a.hasLog.Length; i++) {
            a.hasLog[i] = a.readLog[i] = false;
        }
		a.numLogsFromLevel = new int[10];
        for (int i = 0; i < a.numLogsFromLevel.Length; i++) {
            a.numLogsFromLevel[i] = 0;
        }
		a.lastAddedIndex = a.tempRefIndex = -1;
		a.logPaused = a.beepDone = false;
		a.emailCurrent = a.emailIndex = 0;

		// Patches
		a.patchCounts = new int[7];
		a.patchLastCount = new int[7];
		for (int i = 0; i < a.patchCounts.Length; i++) {
			a.patchCounts[i] = a.patchLastCount[i] = 0;
		}
		a.patchCurrent = a.patchIndex = 0;

		// Software
		a.currentCyberItem = -1;
		a.isPulserNotDrill = true;
		a.softVersions = new int[7];
		a.hasSoft = new bool[7];
        for (int i = 0; i < a.softVersions.Length; i++) {
            a.softVersions[i] = 0;
			a.hasSoft[i] = false;
        }

		// Weapons
        a.weaponInventoryText = new string[]{"","","","","","",""};;
        a.weaponInventoryIndices = new int[]{-1,-1,-1,-1,-1,-1,-1};
        a.weaponInventoryAmmoIndices = new int[]{-1,-1,-1,-1,-1,-1,-1};	
		a.globalLookupIndex = -1;
		a.retval = "0";
		a.weaponInvTextSource = new string[16]; // Used only by MouseLookScript.AddWeaponToInventory();
		a.weaponInvTextSource[0] = Const.a.stringTable[264];  // ASSLT RIFLE
		a.weaponInvTextSource[1] = Const.a.stringTable[265];  // BLASTER
		a.weaponInvTextSource[2] = Const.a.stringTable[266];  // DARTGUN
		a.weaponInvTextSource[3] = Const.a.stringTable[267];  // FLECHETTE
		a.weaponInvTextSource[4] = Const.a.stringTable[268];  // ION BEAM
		a.weaponInvTextSource[5] = Const.a.stringTable[269];  // LASER RAPIER
		a.weaponInvTextSource[6] = Const.a.stringTable[270];  // PIPE
		a.weaponInvTextSource[7] = Const.a.stringTable[271];  // MAGNUM
		a.weaponInvTextSource[8] = Const.a.stringTable[272];  // MAGPULSE
		a.weaponInvTextSource[9] = Const.a.stringTable[273];  // PISTOL
		a.weaponInvTextSource[10] = Const.a.stringTable[274]; // PLASMA RIFLE
		a.weaponInvTextSource[11] = Const.a.stringTable[275]; // RAIL GUN
		a.weaponInvTextSource[12] = Const.a.stringTable[276]; // RIOT GUN
		a.weaponInvTextSource[13] = Const.a.stringTable[277]; // SKORPION
		a.weaponInvTextSource[14] = Const.a.stringTable[278]; // SPARQ BEAM
		a.weaponInvTextSource[15] = Const.a.stringTable[279]; // STUNGUN
		a.wepAmmo = new int[16];
		a.wepAmmoSecondary = new int[16];
		for (int i=0;i<16;i++) {
			a.wepAmmo[i] = a.wepAmmoSecondary[i] = 0;
		}
		a.currentEnergyWeaponHeat = new float[7];
		a.wepLoadedWithAlternate = new bool[7];
		for (int i=0;i<7;i++) {
			a.wepLoadedWithAlternate[i] = false;
			a.currentEnergyWeaponHeat[i] = 0f;
		}
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			// General
			if (MFDManager.a.GeneralTab.activeInHierarchy) {
				for (int i=0; i<14; i++) {
					if (generalInventoryIndexRef[i] > -1) {
						if (!genButtons[i].activeInHierarchy) genButtons[i].SetActive(true);
					} else {
						if (genButtons[i].activeInHierarchy) genButtons[i].SetActive(false);
					}

					if (genButtons[i].activeInHierarchy) {
						int referenceIndex = genButtons[i].transform.GetComponent<GeneralInvButton>().useableItemIndex;
						if (referenceIndex > -1) {
							genButtonsText[i].text = Const.a.useableItemsNameText[referenceIndex];
						} else {
							genButtonsText[i].text = string.Empty;
						}

						if (i == generalInvCurrent) {
							genButtonsText[i].color = Const.a.ssYellowText; // Yellow
						} else {
							genButtonsText[i].color = Const.a.ssGreenText; // Green
						}
					}
				}
			}

			if (generalInvIndex >= 14 || generalInvIndex < 0) { Debug.Log("generalInvIndex out of bounds at " + generalInvIndex.ToString() + ", reset to 0."); generalInvIndex = 0; }
			//--- End General ---

			// Grenades
			if (GetInput.a.Grenade()) {
				if (MouseLookScript.a.inCyberSpace) {
					UseCyberspaceItem();
				} else {
					if (grenAmmo[grenadeCurrent] > 0) {
						MouseLookScript.a.UseGrenade(grenadeIndex);
					} else {
						Const.sprint(Const.a.stringTable[322] ); // Out of grenades.
					}
				}
			}

			if (GetInput.a.GrenadeCycUp()) {
				if (MouseLookScript.a.inCyberSpace) {
					CycleCyberSpaceItemUp();
				} else {
					GrenadeCycleUp();
				}
			}

			if (GetInput.a.GrenadeCycDown()) {
				if (MouseLookScript.a.inCyberSpace) {
					CycleCyberSpaceItemDn();
				} else {
					GrenadeCycleDown();
				}
			}

			if (MFDManager.a.MainTab.activeInHierarchy) {
				for (int i=0;i<grenCountsText.Length;i++) {
					if (grenButtons[i].gameObject.activeInHierarchy) {
						if (grenCountsLastCount[i] != grenAmmo[i]) {
							grenCountsText[i].text = grenAmmo[i].ToString();
							grenCountsLastCount[i] = grenAmmo[i];
						}

						if (i == grenadeCurrent) {
							grenInventoryText[i].color = Const.a.ssYellowText; // Yellow
							grenCountsText[i].color = Const.a.ssYellowText; // Yellow
						} else {
							grenInventoryText[i].color = Const.a.ssGreenText; // Green
							grenCountsText[i].color = Const.a.ssGreenText; // Green
						}
					}
				}
			}
			//--- End Grenades ---

			// Hardware
			if (MFDManager.a.HardwareTab.activeInHierarchy) {
				for (int i=0;i<hardwareInvText.Length;i++) {
					if (hardwareInvText[i].gameObject.activeInHierarchy) {
						hardwareInvText[i].text = Const.a.useableItemsNameText[hardwareInvReferenceIndex[i]];
						if (i == hardwareInvCurrent) {
							hardwareInvText[i].color = Const.a.ssYellowText; // Yellow
						} else {
							hardwareInvText[i].color = Const.a.ssGreenText; // Green
						}
					}
				}
			}
			if (hardwareInvIndex >= 14 || hardwareInvIndex < 0) { Debug.Log("hardwareInvIndex out of bounds at " + hardwareInvIndex.ToString() + ", reset to 0."); hardwareInvIndex = 0; }
			//--- End Hardware ---

			// Logs
			if (logPaused) {
				logPaused = false;
				if (SFXSource == null) SFXSource = GetComponent<AudioSource>();
				if (SFXSource == null) Debug.Log("ERROR: Missing SFXSource on Inventory!");
				else SFXSource.UnPause();
			}

			if(GetInput.a.RecentLog() && (hasHardware[2] == true)) {
				if (lastAddedIndex != -1) {
					PlayLog(lastAddedIndex);
					tempRefIndex = lastAddedIndex;
					lastAddedIndex = -1;
				} else {
					SFXSource.Stop();
					if (tempRefIndex != -1) lastAddedIndex = tempRefIndex;
					tempRefIndex = -1;
				}
			}
			//--- End Logs ---

			// Patches
			if (GetInput.a != null) {
				if (GetInput.a.Patch()) {
					if (patchCounts[patchCurrent] > 0) {
						patchButtonScripts[patchCurrent].DoubleClick();
					} else {
						Const.sprint(Const.a.stringTable[324] ); // Out of patches.
					}
				}
				if (GetInput.a.PatchCycUp())   PatchCycleUp(true);
				if (GetInput.a.PatchCycDown()) PatchCycleDown(true);
			}

			if (MFDManager.a.MainTab.activeInHierarchy) {
				for (int i = 0; i < patchLastCount.Length; i++) {
					// Toggle patch button visibility.  Turn on if we have patches of that type.
					if (patchCounts[i] > 0) {
						if (!patchButtonScripts[i].gameObject.activeInHierarchy) patchButtonScripts[i].gameObject.SetActive(true);
					} else {
						if (patchButtonScripts[i].gameObject.activeInHierarchy) patchButtonScripts[i].gameObject.SetActive(false);
					}

					// Update text and text color on active buttons.
					if (patchButtonScripts[i].gameObject.activeInHierarchy) {
						if (patchLastCount[i] != patchCounts[i]) {
							patchCountTextObjects[i].text = patchCounts[i].ToString();
							patchLastCount[i] = patchCounts[i];
						}

						if (i == patchCurrent) {
							patchInventoryText[i].color = Const.a.ssYellowText; // Yellow
							patchCountTextObjects[i].color = Const.a.ssYellowText; // Yellow
						} else {
							patchInventoryText[i].color = Const.a.ssGreenText; // Yellow
							patchCountTextObjects[i].color = Const.a.ssGreenText; // Green
						}
					}
				}
			}
			//--- End Patches ---

			// Weapons
			if (MFDManager.a.MainTab.activeInHierarchy) {
				for (int i=0;i<weaponShotsInventory.Length;i++) {
					if (weaponButtonText[i].gameObject.activeInHierarchy) {
						weaponButtonText[i].text = weaponInventoryText[i];
						weaponShotsInventory[i].text = weaponShotsInventoryText[i];
						if (i == WeaponCurrent.a.weaponCurrent) {
							weaponButtonText[i].color = Const.a.ssYellowText; // Yellow
							weaponShotsInventory[i].color = Const.a.ssYellowText; // Yellow
						} else {
							weaponButtonText[i].color = Const.a.ssGreenText; // Green
							weaponShotsInventory[i].color = Const.a.ssGreenText; // Green
						}
					}
				}
			}
			//--- End Weapons ---
		} else {
			// Logs pause exceptions.
			if (!logPaused) {
				logPaused = true;
				if (SFXSource == null) SFXSource = GetComponent<AudioSource>();
				if (SFXSource == null) Debug.Log("ERROR: Missing SFXSource on Inventory!");
				else SFXSource.Pause();
			}
			//--- End Logs ---
		}
	}

	// Access Cards
	public void ActivateHardwareButton (int index) {
		hwButtons[index].SetActive(true);
	}

	public bool HasAccessCard(AccessCardType card) {
		for (int i=0;i<accessCardsOwned.Length;i++) {
			if (accessCardsOwned[i] == card) return true;
		}
		return false;
	}

	public static string AccessCardCodeForType(AccessCardType acc) {
		switch(acc) {
			case AccessCardType.Standard: return "STD";
			case AccessCardType.Medical: return "MED";
			case AccessCardType.Science: return "SCI";
			case AccessCardType.Admin: return "ADM";
			case AccessCardType.Group1: return "Group-1";
			case AccessCardType.Group2: return "Group-2";
			case AccessCardType.Group3: return "Group-3";
			case AccessCardType.Group4: return "Group-4";
			case AccessCardType.GroupA: return "Group-A";
			case AccessCardType.GroupB: return "Group-B";
			case AccessCardType.Storage: return "STO";
			case AccessCardType.Engineering: return "ENG";
			case AccessCardType.Maintenance: return "MTN";
			case AccessCardType.Security: return "SEC";
			case AccessCardType.Per1: return "PER-1";
			case AccessCardType.Per2: return "PER-2";
			case AccessCardType.Per3: return "PER-3";
			case AccessCardType.Per4: return "PER-4";
			case AccessCardType.Per5: return "PER-5";
		}
		return "Group-2";
	}

	public void AddAccessCardToInventory (int index) {
		if (MouseLookScript.a.firstTimePickup) MFDManager.a.CenterTabButtonClickSilent(2,true);
		switch (index) {
			case 34: doorAccessTypeAcquired = AccessCardType.Admin; break;	  // Green Rim, Turquoise Inner with Yellow Cross (card_group5)
			case 81: doorAccessTypeAcquired = AccessCardType.Standard; break; //CHECKED! Good here.  Orange Rim, Turquoise Inner (card_std)
			case 83: doorAccessTypeAcquired = AccessCardType.Group1; break; //CHECKED! Good here.  Blue Rim, Orange Inner (card_group1_actual)
			case 84: doorAccessTypeAcquired = AccessCardType.Science; break; //CHECKED! Good here.  All Yellow (card_group1)
			case 85: doorAccessTypeAcquired = AccessCardType.Engineering; break;  //CHECKED! Good here.  Blue Rim, Turquoise Inner (card_eng)
			case 86: doorAccessTypeAcquired = AccessCardType.GroupB; break; //CHECKED! Good here.  Blue Rim, Orange Inner (card_group1_actual)
			case 87: doorAccessTypeAcquired = AccessCardType.Security; break; //CHECKED! Good here. Command = Security = Storage Red Rim, Yellow Inner (card_per1)
			case 88: doorAccessTypeAcquired = AccessCardType.Per5; break; // Purple Rim, Red Inner (card_per5)
			case 89: doorAccessTypeAcquired = AccessCardType.Medical; break; // Gray with Red Cross (card_medi)
			case 90: doorAccessTypeAcquired = AccessCardType.Group3; break; // CHECKED! Good here.  Blue Rim, Orange Inner with Yellow prongs (card_blue)
			case 91: doorAccessTypeAcquired = AccessCardType.Group4; break; // Cyberspace only
			case 110: doorAccessTypeAcquired = AccessCardType.Per1; break; // Darcy, Purple Rim, Red Inner (card_per5)
			default: 
				Const.sprint("Attempted to add an unmarked access card, we'll treat it as a STANDARD.");
				doorAccessTypeAcquired = AccessCardType.Standard;
				break;
		}

		 // Command = STO+MTN+SEC
		if (HasAccessCard(doorAccessTypeAcquired) || (index == 87
			  && HasAccessCard(AccessCardType.Storage) // If Command, only give
			  && HasAccessCard(AccessCardType.Security)//    message if missing
			  && HasAccessCard(AccessCardType.Maintenance))) { //        all 3
			Const.sprint(Const.a.stringTable[44] + AccessCardCodeForType(doorAccessTypeAcquired)); // Already have access: ##
		} else {
			bool added = false;
			if (index == 87) {
				for (int j=0;j<3;j++) {
					switch(j) {
						case 0: doorAccessTypeAcquired = AccessCardType.Storage; break;
						case 1: doorAccessTypeAcquired = AccessCardType.Security; break;
						case 2: doorAccessTypeAcquired = AccessCardType.Maintenance; break;
					}

					for (int i=0;i<accessCardsOwned.Length;i++) {
						if (accessCardsOwned[i] == AccessCardType.None){
							added = true;
							accessCardsOwned[i] = doorAccessTypeAcquired;
							break;
						}
					}
				}
			} else {
				for (int i=0;i<accessCardsOwned.Length;i++) {
					if (accessCardsOwned[i] == AccessCardType.None){
						added = true;
						accessCardsOwned[i] = doorAccessTypeAcquired;
						break;
					}
				}
			}

			if (added) {
				if (index == 87) {
					// New accesses gained STO MTN SEC
					Const.sprint(Const.a.stringTable[45] 
						+ AccessCardCodeForType(AccessCardType.Storage)
						+ ", "
						+ AccessCardCodeForType(AccessCardType.Security)
						+ ", "
						+ AccessCardCodeForType(AccessCardType.Maintenance)); 
				} else {
					 // New accesses gained ##
					Const.sprint(Const.a.stringTable[45]
						+ AccessCardCodeForType(doorAccessTypeAcquired));
				}

				MFDManager.a.NotifyToCenterTab(2);
				MFDManager.a.SendInfoToItemTab(index);
			} else {
				Const.sprint("BUG: Something went wrong when trying to add that access card.");
				MFDManager.a.ResetItemTab();
			}
		}
	}
	//--- End Access Cards ---

	// Hardware
	// index [0,11]: Reference into the list of just hardwares.
	// constIndex [0,102]: Reference into the Const.cs global item lists.
	public void AddHardwareToInventory(int index, int constIndex) {
		if (index < 0) return;

		int hwversion = MouseLookScript.a.heldObjectCustomIndex;
		if (hwversion < 1) {
			Const.sprint("BUG: Hardware picked up has no assigned versioning, defaulting to 1 (value of 0)" );
			hwversion = 1;
		}

		MFDManager.a.SendInfoToItemTab(constIndex);
		if (hwversion <= hardwareVersion[index]) { Const.sprint(Const.a.stringTable[46] ); return; }

		int textIndex = 21;
		int button8Index = -1;
		switch(index) {
			case 0: textIndex = 21; break; // System Analyzer
			case 1: textIndex = 22; // Navigation Unit
				MouseLookScript.a.compassContainer.SetActive(true); // Turn on HUD compass
				MouseLookScript.a.automapContainerLH.SetActive(true);
				MouseLookScript.a.automapContainerRH.SetActive(true);
				if (hwversion >= 2) MouseLookScript.a.compassMidpoints.SetActive(true);
				if (hwversion >= 3) {
					MouseLookScript.a.compassSmallTicks.SetActive(true);
					MouseLookScript.a.compassLargeTicks.SetActive(true);
				}
				MFDManager.a.OpenTab(2,true,TabMSG.None,0,Handedness.RH);
				break;
			case 2: textIndex = 23; button8Index = 5; break; // Datareader
			case 3: textIndex = 24; button8Index = 1; break; // Sensaround
			case 4: textIndex = 25; break; // Target Identifier
			case 5: textIndex = 26; button8Index = 3; break; // Energy Shield
			case 6: textIndex = 27; button8Index = 0; break; // Biomonitor
			case 7: textIndex = 28; button8Index = 2; break; // Head Mounted Lantern
			case 8: textIndex = 29; break; // Envirosuit
			case 9: textIndex = 30; button8Index = 6; break; // Turbo Motion Booster
			case 10:textIndex = 31; button8Index = 7; break; // Jump Jet Boots
			case 11:textIndex = 32; button8Index = 4; break; // Infrared Night Vision Enhancement
		}
		hardwareInvIndex = index;

		if (button8Index >= 0 && button8Index < 8) {
			MouseLookScript.a.hardwareButtons[button8Index].SetActive(true);  // Enable HUD button
			hardwareButtonManager.SetVersionIconForButton(hardwareIsActive[index],hardwareVersionSetting[index],4);
			hardwareButtonManager.buttons[button8Index].gameObject.SetActive(true);
			Debug.Log("Enabled a hardware button with index of " + button8Index.ToString());
		}
		hasHardware[index] = true;
		hardwareVersion[index] = hwversion;
		hardwareVersionSetting[index] = hwversion - 1;
		Const.sprint(Const.a.useableItemsNameText[textIndex] + " v" + hwversion.ToString() );
		if (MouseLookScript.a.firstTimePickup) MFDManager.a.CenterTabButtonClickSilent(1,true);
		ActivateHardwareButton(index);
		MFDManager.a.NotifyToCenterTab(1);
	}
	//--- End Hardware ---

	// General
    public bool AddGeneralObjectToInventory(int index) {
		if (index < 0) return false;

        for (int i=0;i<14;i++) {
            if (generalInventoryIndexRef[i] == -1) {
                generalInventoryIndexRef[i] = index;
				Const.sprint(Const.a.useableItemsNameText[index] + Const.a.stringTable[31] ); // Item added to general inventory
                generalInvCurrent = i;
				genButtons[i].transform.GetComponent<GeneralInvButton>().useableItemIndex = index;
				MFDManager.a.SendInfoToItemTab(index);
				MFDManager.a.NotifyToCenterTab(2);
				if (MouseLookScript.a.firstTimePickup) {
					MFDManager.a.CenterTabButtonClickSilent(2,true);
					MouseLookScript.a.firstTimePickup = false;
				}
				return true;
            }
        }
		return false;
    }
	//--- End General ---

	// Grenades
	public void GrenadeCycleDown() {
		int lastDex = grenadeCurrent;
		int nextIndex = grenadeCurrent - 1; // Add 1 to get slot above this.
		if (nextIndex < 0) nextIndex = 6; // Wraparound to top.
		int countCheck = 0;
		bool noGrenAmmo = (grenAmmo[nextIndex] <= 0);
		while (noGrenAmmo) {
			countCheck++;
			if (countCheck > 13) return; // No weapons!  Don't runaway loop.

			nextIndex--;
			if (nextIndex < 0) nextIndex = 6;
			noGrenAmmo = (grenAmmo[nextIndex] <= 0);
		}
		if (lastDex == nextIndex) return; // Don't do anything if we don't have more grenades.

		MFDManager.a.CenterTabButtonClickSilent(0,true);
		grenButtons[nextIndex].GrenadeInvClick();
		switch(grenadeCurrent) {
			case 0: Const.sprint(Const.a.stringTable[579]); break;
			case 1: Const.sprint(Const.a.stringTable[580]); break;
			case 2: Const.sprint(Const.a.stringTable[581]); break;
			case 3: Const.sprint(Const.a.stringTable[582]); break;
			case 4: Const.sprint(Const.a.stringTable[583]); break;
			case 5: Const.sprint(Const.a.stringTable[584]); break;
			case 6: Const.sprint(Const.a.stringTable[585]); break;
		}
	}

	public void GrenadeCycleUp() {
		int lastDex = grenadeCurrent;
		int nextIndex = grenadeCurrent + 1; // Add 1 to get slot above this.
		if (nextIndex > 6) nextIndex = 0; // Wraparound to bottom.
		int countCheck = 0;
		bool noGrenAmmo = (grenAmmo[nextIndex] <= 0);
		while (noGrenAmmo) {
			countCheck++;
			if (countCheck > 13) return; // No grenades!  Don't runaway loop.

			nextIndex++;
			if (nextIndex > 6) nextIndex = 0;
			noGrenAmmo = (grenAmmo[nextIndex] <= 0);
		}
		if (lastDex == nextIndex) return; // Don't do anything if we don't have more grenades.

		MFDManager.a.CenterTabButtonClickSilent(0,true);
		grenButtons[nextIndex].GrenadeInvClick();
		switch(grenadeCurrent) {
			case 0: Const.sprint(Const.a.stringTable[579]); break;
			case 1: Const.sprint(Const.a.stringTable[580]); break;
			case 2: Const.sprint(Const.a.stringTable[581]); break;
			case 3: Const.sprint(Const.a.stringTable[582]); break;
			case 4: Const.sprint(Const.a.stringTable[583]); break;
			case 5: Const.sprint(Const.a.stringTable[584]); break;
			case 6: Const.sprint(Const.a.stringTable[585]); break;
		}
	}

	// index [0,6]: Reference into the list of the 7 grenade types.
	// useableIndex [0,101]: Reference into the Const.cs global item array lists.
    public void AddGrenadeToInventory(int index, int useableIndex) {
		if (index < 0) return;

		if (MouseLookScript.a.firstTimePickup) MFDManager.a.CenterTabButtonClickSilent(0,true);
		grenAmmo[index]++;
		grenadeCurrent = index;
		grenadeIndex = useableIndex;
		Const.sprint(Const.a.useableItemsNameText[index] + Const.a.stringTable[34] );
		MFDManager.a.NotifyToCenterTab(0);
		MFDManager.a.SendInfoToItemTab(useableIndex);
    }

	public void RemoveGrenade(int index) {
		grenAmmo[index]--;
		if (grenAmmo[index] <= 0) {
			grenAmmo[index] = 0;
			GrenadeCycleDown();
		}
	}
	//--- End Grenades ---

	// Logs
	public void DeactivateVMail() {
		vmailbetajet.SetActive(false);
		vmailbridgesep.SetActive(false);
		vmailcitadestruct.SetActive(false);
		vmailgenstatus.SetActive(false);
		vmaillaserdest.SetActive(false);
		vmailshieldsup.SetActive(false);
		if(SFXSource != null) SFXSource.Stop();
	}

	public void PlayLog(int logIndex) {
		if (logIndex < 0) return;

		SFXClip = null;
		SFXSource.Stop();
		if (Const.a.audioLogs != null) SFXClip = Const.a.audioLogs[logIndex];
		Utils.PlayOneShotSavable(SFXSource,SFXClip); // Play the log audio
		if (!readLog[logIndex]) QuestLogNotesManager.a.LogAdded(logIndex);
		readLog[logIndex] = true;
		if (Const.a.audioLogType[logIndex] == AudioLogType.Vmail) {
			MouseLookScript.a.vmailActive = true; // allow click to end
			MouseLookScript.a.ForceInventoryMode();
			switch (logIndex) {
				case 119: vmailbetajet.SetActive(true); break;
				case 116: vmailbridgesep.SetActive(true); break;
				case 117: vmailcitadestruct.SetActive(true); break;
				case 110: vmailgenstatus.SetActive(true); break;
				case 114: vmaillaserdest.SetActive(true); break;
				case 120: vmailshieldsup.SetActive(true); break;
			}
		}
		MFDManager.a.SendAudioLogToDataTab(logIndex);
	}

	public void PlayLastAddedLog(int logIndex) {
		if (logIndex == -1) return;

		PlayLog(logIndex);
		lastAddedIndex = -1;
	}

	public void AddAudioLogToInventory(int index) {
		if (index < 0) { Debug.Log("BUG: Audio log picked up has no assigned index (-1)"); return; }

		if (index == 128) {
			// Trioptimum Funpack Module discovered!
			// UPDATE: Create minigames
			Const.sprint(Const.a.stringTable[309] );
			return;
		}
		hasLog[index] = true;
		lastAddedIndex = index;
		numLogsFromLevel[Const.a.audioLogLevelFound[index]]++;
		MouseLookScript.a.logContentsManager.InitializeLogsFromLevelIntoFolder();
		MFDManager.a.SendInfoToItemTab(6);
		if (Inventory.a.hasHardware[2] == true) {
			Const.sprint(Const.a.stringTable[36] + Const.a.audiologNames[index] + Const.a.stringTable[37] + Const.a.InputValues[Const.a.InputCodeSettings[15]] + Const.a.stringTable[38]); // Audio log ## picked up.  Press '##' to play back.
		} else {
			Const.sprint(Const.a.stringTable[36] + Const.a.audiologNames[index] + Const.a.stringTable[310]); // Audio log ## picked up.  Proper hardware not detected to play.
		}
	}
	//--- End Logs ---

	// Patches
	public void PatchCycleDown(bool useSound) {
		int nextIndex = patchCurrent - 1; // Add 1 to get slot above this.
		if (nextIndex < 0) nextIndex = 6; // Wraparound to top.
		int countCheck = 0;
		bool noPatches = (patchCounts[nextIndex] <= 0);
		while (noPatches) {
			countCheck++;
			if (countCheck > 13) return; // No weapons!  Don't runaway loop.
			nextIndex--;
			if (nextIndex < 0) nextIndex = 6;
			noPatches = (patchCounts[nextIndex] <= 0);
		}
		MFDManager.a.CenterTabButtonClickSilent(0,true);
		patchButtonScripts[nextIndex].PatchInvClick(useSound);
	}

	public void PatchCycleUp(bool useSound) {
		int nextIndex = patchCurrent + 1; // Add 1 to get slot above this.
		if (nextIndex > 6) nextIndex = 0; // Wraparound to bottom.
		int countCheck = 0;
		bool noPatches = (patchCounts[nextIndex] <= 0);
		while (noPatches) {
			countCheck++;
			if (countCheck > 13) return; // No grenades!  Don't runaway loop.
			nextIndex++;
			if (nextIndex > 6) nextIndex = 0;
			noPatches = (patchCounts[nextIndex] <= 0);
		}
		MFDManager.a.CenterTabButtonClickSilent(0,true);
		patchButtonScripts[nextIndex].PatchInvClick(useSound);
	}

	// index [0,6]: Index into the list of just the 7 grenade types.
	public void AddPatchToInventory (int index,int constIndex) {
		if (index < 0) return;

		if (MouseLookScript.a.firstTimePickup) MFDManager.a.CenterTabButtonClickSilent(0,true);
		patchCounts[index]++;
		patchCurrent = index;

		// Update UI text
		for (int i = 0; i < 7; i++) {
			patchCountTextObjects[i].text = patchCounts[i].ToString ();
			if (i == index) patchCountTextObjects[i].color = Const.a.ssYellowText; // Yellow
			else  patchCountTextObjects[i].color = Const.a.ssGreenText; // Green
		}
		MFDManager.a.SendInfoToItemTab(constIndex);
		MFDManager.a.NotifyToCenterTab(0);
		Const.sprint(Const.a.useableItemsNameText[constIndex] + Const.a.stringTable[35] );
    }
	//--- End Patches ---

	// Software
	public void UseCyberspaceItem() {
		if (currentCyberItem <= 0) {
			currentCyberItem = GetExistingCyberItemIndex(); // try one more time to be sure
			if (currentCyberItem <= 0) {
				Const.sprint(Const.a.stringTable[473],Const.a.player1); // Out of expendable softwares.
				return;
			}
			// oh it was good, ok then moving on down...
		}

		switch(currentCyberItem) {
			case 0:
				// Turbo
				if (softVersions[3] <= 0) {
					currentCyberItem = GetExistingCyberItemIndex();
					return; // out of turbos
				}
				UseTurbo();
				break;
			case 1:
				// Decoy
				if (softVersions[4] <= 0) {
					currentCyberItem = GetExistingCyberItemIndex();
					return; // out of decoys
				}
				UseDecoy();
				break;
			case 2:
				// Recall
				if (softVersions[5] <= 0) {
					currentCyberItem = GetExistingCyberItemIndex();
					return; // out of recalls
				}
				UseRecall();
				break;
		}
	}

	int GetExistingCyberItemIndex() {
		if (softVersions[3] > 0) return 0; // Turbo
		if (softVersions[4] > 0) return 1; // Decoy
		if (softVersions[5] > 0) return 2; // Recall
		return -1;
	}

	public void CycleCyberSpaceItemUp() {
		int nextIndex = currentCyberItem + 1; // add 1 to get slot above this
		if (nextIndex > 2) nextIndex = 0; // wraparound to bottom
		int countCheck = 0;
		bool buttonNotValid = hasSoft[nextIndex];
		while (buttonNotValid) {
			countCheck++;
			if (countCheck > 7) {
				currentCyberItem = -1;
				return; // no weapons!  don't runaway loop
			}
			nextIndex++;
			if (nextIndex > 2) nextIndex = 0;
			buttonNotValid = hasSoft[nextIndex];
		}
		currentCyberItem = nextIndex;
	}

	public void CycleCyberSpaceItemDn() {
		int nextIndex = currentCyberItem - 1; // add 1 to get slot above this
		if (nextIndex < 0) nextIndex = 2; // wraparound to top
		int countCheck = 0;
		bool buttonNotValid = (hasSoft[nextIndex] == false);
		while (buttonNotValid) {
			countCheck++;
			if (countCheck > 7) {
				currentCyberItem = -1;
				return; // no weapons!  don't runaway loop
			}
			nextIndex--;
			if (nextIndex < 0) nextIndex = 2;
			buttonNotValid = hasSoft[nextIndex];
		}
		currentCyberItem = nextIndex;
	}

	public void UseTurbo() {
		if (softVersions[3] <= 0) {
			softs[3].SetActive(false); // turn the button off now that we are out
			return; // out of turbos
		}
		softVersions[3]--; // reduce number of turbos we have left to use
		if (softVersions[3] <= 0) {
			hasSoft[3] = false;
			softs[3].SetActive(false); // turn the button off now that we are out
		}
		if (PlayerMovement.a.turboFinished > PauseScript.a.relativeTime) {
			PlayerMovement.a.turboFinished += PlayerMovement.a.turboCyberTime; // effect stacks
		} else {
			PlayerMovement.a.turboFinished = PlayerMovement.a.turboCyberTime + PauseScript.a.relativeTime;
		}
	}

	public void UseDecoy() {
		if (Const.a.decoyActive) {
			Const.sprint(Const.a.stringTable[537],Const.a.player1);
			return;
		}
		if (softVersions[4] <= 0) {
			softs[4].SetActive(false); // turn the button off now that we are out
			return; // out of decoys
		}
		softVersions[4]--; // reduce number of decoys we have left to use
		if (softVersions[4] <= 0) {
			hasSoft[4] = false;
			softs[4].SetActive(false); // turn the button off now that we are out
		}
		GameObject decoyObj = Instantiate(decoyPrefab,PlayerMovement.a.transform.position,MouseLookScript.a.transform.rotation) as GameObject;
		if (decoyObj != null) {
			decoyObj.transform.SetParent(CyberSpaceStaticContainer.transform,true);
		}
	}

	public void UseRecall() {
		if (softVersions[5] <= 0) {
			softs[5].SetActive(false); // turn the button off now that we are out
			return; // out of recalls
		}
		softVersions[5]--; // reduce number of recalls we have left to use
		if (softVersions[5] <= 0) {
			hasSoft[5] = false;
			softs[5].SetActive(false); // turn the button off now that we are out
		}
		PlayerMovement.a.transform.position = MouseLookScript.a.cyberspaceRecallPoint; // pop back to cyber section start
	}

	public bool AddSoftwareItem(SoftwareType type, int vers) {
		switch(type) {
			case SoftwareType.Drill:	
				softs[0].SetActive(true);
				if (isPulserNotDrill && !hasSoft[1]) {
					pulserButtonText.Select(false);
					drillButtonText.Select(true);
					isPulserNotDrill = false; // equip drill if we don't already have pulser
				}
				if (vers > softVersions[0]) softVersions[0] = vers;
				else Const.sprint(Const.a.stringTable[46],Const.a.player1);
				drillVersionText.text = softVersions[0].ToString();
				hasSoft[0] = true;
				Utils.PlayOneShotSavable(SFX,SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[444] + softVersions[0].ToString() + Const.a.stringTable[458],Const.a.player1);
				return true;
			case SoftwareType.Pulser:	
				softs[1].SetActive(true);
				if (!isPulserNotDrill && !hasSoft[1]) {
					pulserButtonText.Select(true);
					drillButtonText.Select(false);
					isPulserNotDrill = true; // always equip pulser when first picking it up
				}
				if (vers > softVersions[1]) softVersions[1] = vers;
				else Const.sprint(Const.a.stringTable[46],Const.a.player1);
				pulserVersionText.text = softVersions[1].ToString();
				hasSoft[1] = true;
				Utils.PlayOneShotSavable(SFX,SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[445] + softVersions[1].ToString() + Const.a.stringTable[458],Const.a.player1);
				return true;
			case SoftwareType.CShield:	
				softs[2].SetActive(true);
				if (vers > softVersions[2]) softVersions[2] = vers;
				else Const.sprint(Const.a.stringTable[46],Const.a.player1);
				cshieldVersionText.text = softVersions[2].ToString();
				hasSoft[2] = true;
				Utils.PlayOneShotSavable(SFX,SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[446] + softVersions[2].ToString() + Const.a.stringTable[458],Const.a.player1);
				return true;
			case SoftwareType.Turbo:
				if (currentCyberItem == -1f) currentCyberItem = 0;
				softs[3].SetActive(true);
				softVersions[3]++;
				turboCountText.text = softVersions[3].ToString();
				hasSoft[3] = true;
				Utils.PlayOneShotSavable(SFX,SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[447],Const.a.player1);
				return true;
			case SoftwareType.Decoy:	
				if (currentCyberItem == -1f) currentCyberItem = 1;
				softs[4].SetActive(true);
				softVersions[4]++;
				decoyCountText.text = softVersions[4].ToString();
				hasSoft[4] = true;
				Utils.PlayOneShotSavable(SFX,SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[448],Const.a.player1);
				return true;
			case SoftwareType.Recall:	
				if (currentCyberItem == -1f) currentCyberItem = 2;
				softs[5].SetActive(true);
				softVersions[5]++;
				recallCountText.text = softVersions[5].ToString();
				hasSoft[5] = true;
				Utils.PlayOneShotSavable(SFX,SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[449],Const.a.player1);
				return true;
			case SoftwareType.Game:		
				softs[6].SetActive(true);
				switch(vers) {
					case 0: // Ping
							// UPDATE: Create minigame Ping
							Const.sprint(Const.a.stringTable[450],Const.a.player1);
							break;
					case 1: // 15
							// UPDATE: Create minigame 15
							Const.sprint(Const.a.stringTable[451],Const.a.player1);
							break;
					case 2: // Wing 0
							// UPDATE: Create minigame Wing 0
							Const.sprint(Const.a.stringTable[452],Const.a.player1);
							break;
					case 3: // Botbounce
							// UPDATE: Create minigame Botbounce
							Const.sprint(Const.a.stringTable[453],Const.a.player1);
							break;
					case 4: // Eel Zapper
							// UPDATE: Create minigame Eel Zapper
							Const.sprint(Const.a.stringTable[454],Const.a.player1);
							break;
					case 5: // Road
							// UPDATE: Create minigame Road
							Const.sprint(Const.a.stringTable[455],Const.a.player1);
							break;
					case 6: // TriopToe
							// UPDATE: Create minigame TriopToe
							Const.sprint(Const.a.stringTable[456],Const.a.player1);
							break;
				}
				Utils.PlayOneShotSavable(SFX,SFXAcquireCyberItem);
				
				return true;
			case SoftwareType.Data:		
				Utils.PlayOneShotSavable(SFX,SFXAcquireCyberData);
				Const.sprint(Const.a.stringTable[457],Const.a.player1);
				hasLog[vers] = true;
				return true;
			case SoftwareType.Integrity:
				if (hm.cyberHealth >=255) return false;
				Utils.PlayOneShotSavable(SFX,SFXAcquireCyberItem);
				hm.cyberHealth += 77f;
				if (hm.cyberHealth > 255f) hm.cyberHealth = 255f;
				MFDManager.a.DrawTicks(true);
				Const.sprint(Const.a.stringTable[459],Const.a.player1);
				return true;
			case SoftwareType.Keycard:
				if (vers < 0 || vers > 110) vers = 81; // At least give them STD.
				AddAccessCardToInventory(vers);
				return true;
		}
		return false;
	}
	//--- End Software

	// Weapons
	public void RemoveWeapon(int wepButIndex) {
		weaponInventoryIndices[wepButIndex] = -1; // Remove the weapon by setting it to -1.
		weaponInventoryAmmoIndices[wepButIndex] = -1;
		weaponInventoryText[wepButIndex] = "-";
	}

	public void UpdateAmmoText() {
		for (int i=0;i<weaponShotsInventoryText.Length;i++) {
			weaponShotsInventoryText[i] = GetTextForWeaponAmmo(i);
		}
		int slot1 = weaponInventoryIndices [0];
		int slot2 = weaponInventoryIndices [1];
		int slot3 = weaponInventoryIndices [2];
		int slot4 = weaponInventoryIndices [3];
		int slot5 = weaponInventoryIndices [4];
		int slot6 = weaponInventoryIndices [5];
		int slot7 = weaponInventoryIndices [6];
		numweapons = 0;
		if (slot1 != -1) numweapons++;
		if (slot2 != -1) numweapons++;
		if (slot3 != -1) numweapons++;
		if (slot4 != -1) numweapons++;
		if (slot5 != -1) numweapons++;
		if (slot6 != -1) numweapons++;
		if (slot7 != -1) numweapons++;
	}

	public string GetTextForWeaponAmmo(int index) {
		globalLookupIndex = weaponInventoryIndices[index];
		retval = "0";
		switch (globalLookupIndex) {
		case 36:
			//Mark3 Assault Rifle
			retval = wepAmmo[0].ToString() + "mg, " + wepAmmoSecondary[0].ToString() + "pn";
			break;
		case 37:
			//ER-90 Blaster
			if (currentEnergyWeaponHeat[index] > 80f) {
				retval = Const.a.stringTable[14]; // OVERHEATED
			} else {
				retval = Const.a.stringTable[15]; // READY
			}
			break;
		case 38:
			//SV-23 Dartgun
			retval = wepAmmo[2].ToString() + "nd, " + wepAmmoSecondary[2].ToString() + "tq";
			break;
		case 39:
			//AM-27 Flechette
			retval = wepAmmo[3].ToString() + "hn, " + wepAmmoSecondary[3].ToString() + "sp";
			break;
		case 40:
			//RW-45 Ion Beam
			if (currentEnergyWeaponHeat[index] > 80f) {
				retval = Const.a.stringTable[14]; // OVERHEATED
			} else {
				retval = Const.a.stringTable[15]; // READY
			}
			break;
		case 41:
			//TS-04 Laser Rapier
			retval = "";
			break;
		case 42:
			//Lead Pipe
			retval = "";
			break;
		case 43:
			//Magnum 2100
			retval = wepAmmo[7].ToString() + "hw, " + wepAmmoSecondary[7].ToString() + "sg";
			break;
		case 44:
			//SB-20 Magpulse
			retval = wepAmmo[8].ToString() + "cr, " + wepAmmoSecondary[8].ToString() + "su";
			break;
		case 45:
			//ML-41 Pistol
			retval = wepAmmo[9].ToString() + "st, " + wepAmmoSecondary[9].ToString() + "tf";
			break;
		case 46:
			//LG-XX Plasma Rifle
			if (currentEnergyWeaponHeat[index] > 80f) {
				retval = Const.a.stringTable[14]; // OVERHEATED
			} else {
				retval = Const.a.stringTable[15]; // READY
			}
			break;
		case 47:
			//MM-76 Railgun
			retval = wepAmmo[11].ToString() + "rl";
			break;
		case 48:
			//DC-05 Riotgun
			retval = wepAmmo[12].ToString() + "rb";
			break;
		case 49:
			//RF-07 Skorpion
			retval = wepAmmo[13].ToString() + scorpSmall + wepAmmoSecondary[13].ToString() + scorpLg;
			break;
		case 50:
			//Sparq Beam
			if (currentEnergyWeaponHeat[index] > 80f) {
				retval = Const.a.stringTable[14]; // OVERHEATED
			} else {
				retval = Const.a.stringTable[15]; // READY
			}
			break;
		case 51:
			//DH-07 Stungun
			if (currentEnergyWeaponHeat[index] > 80f) {
				retval = Const.a.stringTable[14]; // OVERHEATED
			} else {
				retval = Const.a.stringTable[15]; // READY
			}
			break;
		}
		return retval;
	}

	public float GetDefaultEnergySettingForWeaponFrom16Index(int wep16Index) {
		switch (wep16Index) {
			case  1: return  3f; // Blaster
			case  4: return  5f; // Ion Beam
			case 10: return 13f; // Plasma rifle
			case 14: return  2f; // Sparq Beam
			case 15: return  3f; // Stungun
		}
		return 3f;
	}

	public void AddAmmoToInventory (int index, int constIndex, int amount, bool isSecondary) {
		if (index < 0) return;

		if (MouseLookScript.a.firstTimePickup) MFDManager.a.CenterTabButtonClickSilent (0,true);
		if (isSecondary) wepAmmoSecondary[index] += amount;
		else			 wepAmmo[index]          += amount;
		Const.sprint(Const.a.useableItemsNameText[index] + Const.a.stringTable[33]); // Item added to weapon inventory
		MFDManager.a.NotifyToCenterTab(0);
		MFDManager.a.SendInfoToItemTab(constIndex);
	}

    public bool AddWeaponToInventory(int index, int ammo1, int ammo2) {
		if (index < 0) return false;

		MFDManager.a.CenterTabButtonClickSilent (0,true); // Weapons are so important we always switch it.
        for (int i=0;i<7;i++) {
            if (weaponInventoryIndices[i] < 0) {
                weaponInventoryIndices[i] = index;
                weaponInventoryText[i] = weaponInvTextSource[(index - 36)]; // Yech!
                WeaponCurrent.a.weaponCurrent = i;
				WeaponCurrent.a.weaponIndex = index;
				int tempindex = WeaponFire.Get16WeaponIndexFromConstIndex(index);
				wepAmmo[tempindex] += ammo1;
				wepAmmoSecondary[tempindex] += ammo2;
				wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent] = false;
				if (ammo2 > 0) Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent] = true;
                MFDManager.a.wepbutMan.wepButtons[i].GetComponent<WeaponButton>().useableItemIndex = index;
				MFDManager.a.wepbutMan.wepButtons[i].GetComponent<WeaponButton>().WeaponInvClick();
				MFDManager.a.SetWepInfo(index);
				MFDManager.a.UpdateHUDAmmoCounts(WeaponCurrent.a.currentMagazineAmount[WeaponCurrent.a.weaponCurrent]);
				MFDManager.a.SendInfoToItemTab(index); // notify item tab we clicked on a weapon
				Const.sprint(Const.a.useableItemsNameText[index] + Const.a.stringTable[33]);
				WeaponCurrent.a.ReloadSecret(true);
				WeaponCurrent.a.weaponEnergySetting[i] = Inventory.a.GetDefaultEnergySettingForWeaponFrom16Index(tempindex);
				MFDManager.a.SendInfoToItemTab(index);
				MFDManager.a.NotifyToCenterTab(0);
				UpdateAmmoText();
                return true;
            }
        }
		return false;
    }
	//--- End Weapons ---


	public static string Save(GameObject go) {
		int j;
		Inventory inv = go.GetComponent<Inventory>();
		string line = System.String.Empty;
		if (inv == null) {
			Debug.Log("Inventory missing on Player!  GameObject.name: " + go.name);
			line = "u";
			for (j=1;j<7;j++) line += "u";
			for (j=1;j<7;j++) line += "u";
			line += "u";
			for (j=0;j<16;j++) line += "u";
			for (j=0;j<16;j++) line += "u";
			for (j=0;j<7;j++) line += "f";
			for (j=0;j<7;j++) line += "b";
			line += "uuff";
			for (j=0;j<7;j++) line += "u";
			line += "uu";
			for (j=0;j<7;j++) line += "u";
			for (j=0;j<134;j++) line += "b";
			for (j=0;j<134;j++) line += "b";
			for (j=0;j<10;j++) line += "u";
			line += "ub";
			for (j=0;j<13;j++) line += "b";
			for (j=0;j<13;j++) line += "u";
			for (j=0;j<13;j++) line += "u";
			line += "uu";
			for (j=0;j<13;j++) line += "b";
			for (j=0;j<32;j++) line += "i"; // u will be -1, i will be 0 for enums
			for (j=0;j<14;j++) line += "u";
			line += "uuub";
			for (j=0;j<7;j++) line += "u";
			for (j=0;j<7;j++) line += "b";
			line += "uu";
			return Utils.DTypeWordToSaveString(line);
		}

		line = Utils.UintToString(inv.weaponInventoryIndices[0]);
		for (j=1;j<7;j++) line += Utils.splitChar + Utils.UintToString(inv.weaponInventoryIndices[j]); // int
		for (j=0;j<7;j++) line += Utils.splitChar + Utils.UintToString(inv.weaponInventoryAmmoIndices[j]); // int
		line += Utils.splitChar + Utils.UintToString(inv.numweapons); // int
		for (j=0;j<16;j++) line += Utils.splitChar + Utils.UintToString(inv.wepAmmo[j]); // int
		for (j=0;j<16;j++) line += Utils.splitChar + Utils.UintToString(inv.wepAmmoSecondary[j]); // int
		for (j=0;j<7;j++) line += Utils.splitChar + Utils.FloatToString(inv.currentEnergyWeaponHeat[j]); // float
		for (j=0;j<7;j++) line += Utils.splitChar + Utils.BoolToString(inv.wepLoadedWithAlternate[j]); // bool
		line += Utils.splitChar + Utils.UintToString(inv.grenadeCurrent); // int
		line += Utils.splitChar + Utils.UintToString(inv.grenadeIndex); // int
		line += Utils.splitChar + Utils.FloatToString(inv.nitroTimeSetting); // float
		line += Utils.splitChar + Utils.FloatToString(inv.earthShakerTimeSetting); // float
		for (j=0;j<7;j++) { line += Utils.splitChar + Utils.UintToString(inv.grenAmmo[j]); } // int
		line += Utils.splitChar + Utils.UintToString(inv.patchCurrent); // int
		line += Utils.splitChar + Utils.UintToString(inv.patchIndex); // int
		for (j=0;j<7;j++) { line += Utils.splitChar + Utils.UintToString(inv.patchCounts[j]); } // int
		for (j=0;j<134;j++) { line += Utils.splitChar + Utils.BoolToString(inv.hasLog[j]); } // bool
		for (j=0;j<134;j++) { line += Utils.splitChar + Utils.BoolToString(inv.readLog[j]); } // bool
		for (j=0;j<10;j++) { line += Utils.splitChar + Utils.UintToString(inv.numLogsFromLevel[j]); } // int
		line += Utils.splitChar + Utils.UintToString(inv.lastAddedIndex); // int
		line += Utils.splitChar + Utils.BoolToString(inv.beepDone); // bool
		for (j=0;j<13;j++) { line += Utils.splitChar + Utils.BoolToString(inv.hasHardware[j]); } // bool
		for (j=0;j<13;j++) { line += Utils.splitChar + Utils.UintToString(inv.hardwareVersion[j]); } // int
		for (j=0;j<13;j++) { line += Utils.splitChar + Utils.UintToString(inv.hardwareVersionSetting[j]); } // int
		line += Utils.splitChar + Utils.UintToString(inv.hardwareInvCurrent); // int
		line += Utils.splitChar + Utils.UintToString(inv.hardwareInvIndex); // int
		for (j=0;j<13;j++) { line += Utils.splitChar + Utils.BoolToString(inv.hardwareIsActive[j]); } // bool
		for (j=0;j<32;j++) { line += Utils.splitChar + Utils.IntToString(Utils.AccessCardTypeToInt(inv.accessCardsOwned[j])); } // int
		for (j=0;j<14;j++) { line += Utils.splitChar + Utils.UintToString(inv.generalInventoryIndexRef[j]); } // int
		line += Utils.splitChar + Utils.UintToString(inv.generalInvCurrent); // int
		line += Utils.splitChar + Utils.UintToString(inv.generalInvIndex); // int
		line += Utils.splitChar + Utils.UintToString(inv.currentCyberItem); // int
		line += Utils.splitChar + Utils.BoolToString(inv.isPulserNotDrill); // bool
		for (j=0;j<7;j++) { line += Utils.splitChar + Utils.UintToString(inv.softVersions[j]); } // int 
		for (j=0;j<7;j++) { line += Utils.splitChar + Utils.BoolToString(inv.hasSoft[j]); } // bool
		line += Utils.splitChar + Utils.UintToString(inv.emailCurrent); // int
		line += Utils.splitChar + Utils.UintToString(inv.emailIndex); // int
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		Inventory inv = go.GetComponent<Inventory>();
		if (inv == null || index < 0 || entries == null) return index + 481;

		int j;
		for (j=0;j<7;j++) { inv.weaponInventoryIndices[j] = Utils.GetIntFromString(entries[index] ); index++; }
		for (j=0;j<7;j++) { inv.weaponInventoryAmmoIndices[j] = Utils.GetIntFromString(entries[index] ); index++; }
		inv.numweapons = Utils.GetIntFromString(entries[index] ); index++;
		for (j=0;j<16;j++) { inv.wepAmmo[j] = Utils.GetIntFromString(entries[index] ); index++; }
		for (j=0;j<16;j++) { inv.wepAmmoSecondary[j] = Utils.GetIntFromString(entries[index] ); index++; }
		for (j=0;j<7;j++) { inv.currentEnergyWeaponHeat[j] = Utils.GetFloatFromString(entries[index]); index++; }
		for (j=0;j<7;j++) { inv.wepLoadedWithAlternate[j] = Utils.GetBoolFromString(entries[index]); index++; }
		inv.grenadeCurrent = Utils.GetIntFromString(entries[index]); index++;
		inv.grenadeIndex = Utils.GetIntFromString(entries[index]); index++;
		inv.nitroTimeSetting = Utils.GetFloatFromString(entries[index]); index++;
		inv.earthShakerTimeSetting = Utils.GetFloatFromString(entries[index]); index++;
		for (j=0;j<7;j++) { inv.grenAmmo[j] = Utils.GetIntFromString(entries[index] ); index++; }
		inv.patchCurrent = Utils.GetIntFromString(entries[index]); index++;
		inv.patchIndex = Utils.GetIntFromString(entries[index]); index++;
		for (j=0;j<7;j++) { inv.patchCounts[j] = Utils.GetIntFromString(entries[index]); index++; }
		for (j=0;j<134;j++) { inv.hasLog[j] = Utils.GetBoolFromString(entries[index]); index++; }
		for (j=0;j<134;j++) { inv.readLog[j] = Utils.GetBoolFromString(entries[index]); index++; }
		for (j=0;j<10;j++) { inv.numLogsFromLevel[j] = Utils.GetIntFromString(entries[index]); index++; }
		inv.lastAddedIndex = Utils.GetIntFromString(entries[index]); index++;
		inv.beepDone = Utils.GetBoolFromString(entries[index]); index++;
		for (j=0;j<13;j++) { inv.hasHardware[j] = Utils.GetBoolFromString(entries[index]); index++; }
		if (Inventory.a.hasHardware[1]) {
			MouseLookScript.a.compassContainer.SetActive(true);
			MouseLookScript.a.automapContainerLH.SetActive(true);
			MouseLookScript.a.automapContainerRH.SetActive(true);
		}
		for (j=0;j<13;j++) { inv.hardwareVersion[j] = Utils.GetIntFromString(entries[index]); index++; }
		for (j=0;j<13;j++) { inv.hardwareVersionSetting[j] = Utils.GetIntFromString(entries[index]); index++; }
		inv.hardwareInvCurrent = Utils.GetIntFromString(entries[index]); index++;
		inv.hardwareInvIndex = Utils.GetIntFromString(entries[index]); index++;
		for (j=0;j<13;j++) { inv.hardwareIsActive[j] = Utils.GetBoolFromString(entries[index]); index++; }
		for (j=0;j<32;j++) { inv.accessCardsOwned[j] = Utils.IntToAccessCardType(Utils.GetIntFromString(entries[index])); index++; }
		for (j=0;j<14;j++) { inv.generalInventoryIndexRef[j] = Utils.GetIntFromString(entries[index]); index++; }
		inv.generalInvCurrent = Utils.GetIntFromString(entries[index]); index++;
		inv.generalInvIndex = Utils.GetIntFromString(entries[index]); index++;
		inv.currentCyberItem = Utils.GetIntFromString(entries[index]); index++;
		inv.isPulserNotDrill = Utils.GetBoolFromString(entries[index]); index++;
		for (j=0;j<7;j++) { inv.softVersions[j] = Utils.GetIntFromString(entries[index]); index++; }
		for (j=0;j<7;j++) { inv.hasSoft[j] = Utils.GetBoolFromString(entries[index]); index++; }
		inv.emailCurrent = Utils.GetIntFromString(entries[index]); index++;
		inv.emailIndex = Utils.GetIntFromString(entries[index]); index++;	
		return index;
	}
}
