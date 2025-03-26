using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Inventory : MonoBehaviour {
	// Access Cards
	public AccessCardType[] accessCardsOwned; // save
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
	public Button[] hardwareButtons;
	public Sprite[] hardwareButtonDeactive;
	public Sprite[] hardwareButtonActive1;
	public Sprite[] hardwareButtonActive2;
	public Sprite[] hardwareButtonActive3;
	public Sprite[] hardwareButtonActive4;
	public GameObject sensaroundCenter;
	public GameObject sensaroundLH;
	public GameObject sensaroundRH;
	public GameObject sensaroundCenterCamera;
	public GameObject sensaroundLHCamera;
	public GameObject sensaroundRHCamera;
	public GameObject bioMonitorContainer;
	public Light infraredLight;
	public GameObject playerCamera;
	public Light headlight;
	public EmailContentsButtonsManager ecbm;
	public GameObject ShieldActivateFX;
	public GameObject ShieldDeactivateFX;

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
	public int grenadeCurrent = new int(); // save
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
	public VideoPlayer vmailbetajetVideo;
	public VideoPlayer vmailbridgesepVideo;
	public VideoPlayer vmailcitadestructVideo;
	public VideoPlayer vmailgenstatusVideo;
	public VideoPlayer vmaillaserdestVideo;
	public VideoPlayer vmailshieldsupVideo;
	
	private AudioSource SFXSource;
	[HideInInspector] public bool[] hasLog; // save
	[HideInInspector] public bool[] readLog; // save
	[HideInInspector] public int[] numLogsFromLevel; // save
	[HideInInspector] public int lastAddedIndex = -1; // save
	[HideInInspector] public bool beepDone = false; // save
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
	[HideInInspector] public bool[] hasMinigame; // save
	public GameObject[] miniGameButton;

	// Weapons
	[HideInInspector] public int[] weaponInventoryIndices; // save
    [HideInInspector] public int[] weaponInventoryAmmoIndices; // save
	[HideInInspector] public int numweapons = 0; // save
	[HideInInspector] public int[] wepAmmo; // save
	[HideInInspector] public int[] wepAmmoSecondary; // save
	[HideInInspector] public float[] currentEnergyWeaponHeat; // save
	[HideInInspector] public bool[] wepLoadedWithAlternate; // save

	public bool hasNewNotes = true; // save
	public bool hasNewData = false; // save
	public bool hasNewLogs = false; // save
	public bool hasNewEmail = true; // save

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
            if (i != 0) a.generalInventoryIndexRef[i] = -1;
 			a.genButtons[i].SetActive(false);
        }

        a.generalInvCurrent = a.generalInvIndex = 0;
        a.generalInventoryIndexRef[0] = 81;
		a.genButtonsText[0].text = Const.a.stringTable[597]; // ACCESS CARDS

		// Grenades
		a.grenAmmo = new int[7];
		a.grenCountsLastCount = new int[7];
		for (int i= 0; i<a.grenAmmo.Length; i++) {
			a.grenAmmo[i] = a.grenCountsLastCount[i] = 0;
		}

		a.grenadeCurrent = 0;
		a.nitroTimeSetting = Const.nitroDefaultTime;
		a.earthShakerTimeSetting = Const.earthShDefaultTime;

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
		a.hasNewEmail = true;
		a.hasNewNotes = true;
		a.hasMinigame = new bool[7];
		for (int i=0;i<7;i++) a.hasMinigame[i] = false;

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
        a.weaponInventoryIndices = new int[]{-1,-1,-1,-1,-1,-1,-1};
        a.weaponInventoryAmmoIndices = new int[]{-1,-1,-1,-1,-1,-1,-1};	
		a.globalLookupIndex = -1;
		a.retval = "0";
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
		
		a.SFXSource = GetComponent<AudioSource>();
	}

	void UpdateGeneralInventory() {
		for (int i=0; i<14; i++) {
			if (i != 0) { // Active state of the Access Cards button is below.
				if (generalInventoryIndexRef[i] > -1) {
					if (!genButtons[i].activeInHierarchy) genButtons[i].SetActive(true);
				} else {
					if (genButtons[i].activeInHierarchy) genButtons[i].SetActive(false);
				}
			}

			if (genButtons[i].activeInHierarchy) {
				GeneralInvButton genbut = genButtons[i].transform.GetComponent<GeneralInvButton>();
				int referenceIndex = genbut.useableItemIndex;
				if (referenceIndex > -1) {
					if (i != 0) { // Access Cards text set in Awake the once.
						genButtonsText[i].text =
							Const.a.stringTable[referenceIndex + 326];
					}
				} else {
					genButtonsText[i].text = string.Empty;
				}

				if (i == generalInvCurrent) {
					genButtonsText[i].color = Const.a.ssYellowText; // Yellow
				} else {
					genButtonsText[i].color = Const.a.ssGreenText; // Green
				}

				// Enable Apply button for consumables.
				if ((referenceIndex >= 14 && referenceIndex < 21) || referenceIndex == 52
					|| referenceIndex == 53 || referenceIndex == 55) {
					if (genbut.activateButton != null) genbut.activateButton.SetActive(true); // null check because AccessCardsButton has none
				} else {
					if (genbut.activateButton != null) genbut.activateButton.SetActive(false); // null check because AccessCardsButton has none
				}
			}
		}

		// Access Cards button
		if (genButtons[0].activeSelf) return;

		for (int j = 0; j < a.accessCardsOwned.Length; j++) {
			if (a.accessCardsOwned[j] != AccessCardType.None) {
				genButtons[0].SetActive(true);
				break;
			}
		}
	}

	public void CheckForUnreadLogs() {
		int numUnreadEmails = 0;
		int numUnreadLogs = 0;
		for (int i=readLog.Length - 1;i>=0;i--) {
			if (!readLog[i] && hasLog[i]) {
				if (Const.a.audioLogType[i] == AudioLogType.Email) {
					numUnreadEmails++;
				} else numUnreadLogs++;
			}
		}

		if (numUnreadEmails == 0) hasNewEmail = false;
		if (numUnreadLogs == 0) hasNewLogs = false;
	}

	void Update() {
		// Logs pause exceptions.
		if ((PauseScript.a.Paused() || PauseScript.a.MenuActive())
			&& !logPaused) {
			logPaused = true;
			if (SFXSource == null) SFXSource = GetComponent<AudioSource>();
			if (SFXSource == null) {
				Debug.Log("ERROR: Missing SFXSource on Inventory!");
			} else SFXSource.Pause();
		}
		//--- End Logs ---
	
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		// Update Senaraound camera positions to match player camer height.
		Vector3 camPos =
		  hardwareButtonManager.sensaroundCenterCamera.transform.localPosition;

		camPos.y = MouseLookScript.a.transform.localPosition.y;
		hardwareButtonManager.sensaroundCenterCamera.transform.localPosition =
			camPos;

		camPos =
		  hardwareButtonManager.sensaroundCenterCamera.transform.localPosition;

		camPos.y = MouseLookScript.a.transform.localPosition.y;
		hardwareButtonManager.sensaroundLHCamera.transform.localPosition =
			camPos;

		camPos =
		  hardwareButtonManager.sensaroundCenterCamera.transform.localPosition;

		camPos.y = MouseLookScript.a.transform.localPosition.y;
		hardwareButtonManager.sensaroundRHCamera.transform.localPosition =
			camPos;

		// General
		if (MFDManager.a.GeneralTab.activeInHierarchy) {
			UpdateGeneralInventory();
		}

		if (generalInvIndex >= 14 || generalInvIndex < 0) {
			Debug.Log("generalInvIndex out of bounds at "
					  + generalInvIndex.ToString() + ", reset to 0.");
			generalInvIndex = 0;
		}
		//--- End General ---

		// Grenades
		if (GetInput.a.Grenade()) {
			if (MouseLookScript.a.inCyberSpace) {
				UseCyberspaceItem();
			} else {
				if (grenadeCurrent >= 0 && grenadeCurrent < 7) {
					if (grenAmmo[grenadeCurrent] > 0) {
						MouseLookScript.a.UseGrenade(
							grenButtons[grenadeCurrent].useableItemIndex
						);
					} else {
						Const.sprint(Const.a.stringTable[322] ); // Out of grenades.
					}
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
					hardwareInvText[i].text = 
					  Const.a.stringTable[hardwareInvReferenceIndex[i] + 326]
					  + " v" + hardwareVersion[i].ToString();
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
			if (lastAddedIndex != -1 && !SFXSource.isPlaying) {
				PlayLog(lastAddedIndex);
				tempRefIndex = lastAddedIndex;
				lastAddedIndex = FindNextUnreadLog();
				if (lastAddedIndex == tempRefIndex) lastAddedIndex = -1;
				CheckForUnreadLogs();
			} else {
				SFXSource.Stop();
				tempRefIndex = lastAddedIndex;
				lastAddedIndex = FindNextUnreadLog();
				if (lastAddedIndex == tempRefIndex) lastAddedIndex = -1;
				CheckForUnreadLogs();
				Const.sprint(Const.a.stringTable[1019]); // "Log playback stopped"
			}
		}
		//--- End Logs ---

		// Patches
		if (GetInput.a != null) {
			if (GetInput.a.Patch()) {
				if (patchCounts[patchCurrent] > 0) {
					patchButtonScripts[patchCurrent].PatchUse();
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
			UpdateAmmoText();
			int yellowWep = WeaponCurrent.a.weaponCurrent;
			int dullYellowWep = -1;
			if (WeaponCurrent.a.weaponCurrentPending >= 0) {
				dullYellowWep = WeaponCurrent.a.weaponCurrentPending; // Next
				yellowWep = -1; // Last wep
			}

			for (int i=0;i<weaponShotsInventory.Length;i++) {
				if (weaponButtonText[i].gameObject.activeInHierarchy) {
					weaponButtonText[i].text =
						Const.a.stringTable[326 + weaponInventoryIndices[i]];

					weaponShotsInventory[i].text = weaponShotsInventoryText[i];
					if (i == yellowWep) {
						weaponButtonText[i].color = Const.a.ssYellowText; // Yellow
						weaponShotsInventory[i].color = Const.a.ssYellowText; // Yellow
					} else if (i == dullYellowWep) {
						weaponButtonText[i].color = Const.a.ssDarkYellowText; // Green
						weaponShotsInventory[i].color = Const.a.ssDarkYellowText; // Green
					} else {
						weaponButtonText[i].color = Const.a.ssGreenText; // Green
						weaponShotsInventory[i].color = Const.a.ssGreenText; // Green
					}
				}
			}
		}
		//--- End Weapons ---
	}

	// Access Cards
	public void ActivateHardwareButton (int index) {
		hwButtons[index].SetActive(true);
	}

	public bool HasAnyAccessCards() {
		if (HasAccessCard(AccessCardType.Standard)) return true;
		if (HasAccessCard(AccessCardType.Medical)) return true;
		if (HasAccessCard(AccessCardType.Science)) return true;
		if (HasAccessCard(AccessCardType.Admin)) return true;
		if (HasAccessCard(AccessCardType.Group1)) return true;
		if (HasAccessCard(AccessCardType.Group2)) return true;
		if (HasAccessCard(AccessCardType.Group3)) return true;
		if (HasAccessCard(AccessCardType.Group4)) return true;
		if (HasAccessCard(AccessCardType.GroupA)) return true;
		if (HasAccessCard(AccessCardType.GroupB)) return true;
		if (HasAccessCard(AccessCardType.Storage)) return true;
		if (HasAccessCard(AccessCardType.Engineering)) return true;
		if (HasAccessCard(AccessCardType.Maintenance)) return true;
		if (HasAccessCard(AccessCardType.Security)) return true;
		if (HasAccessCard(AccessCardType.Per1)) return true;
		if (HasAccessCard(AccessCardType.Per2)) return true;
		if (HasAccessCard(AccessCardType.Per3)) return true;
		if (HasAccessCard(AccessCardType.Per4)) return true;
		if (HasAccessCard(AccessCardType.Per5)) return true;
		return false;
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
				Const.sprint("BUG: Attempted to add an unmarked access card, we'll treat it as a STANDARD.");
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

				MFDManager.a.SendInfoToItemTab(index);
				MFDManager.a.NotifyToCenterTab(2);
				if (MouseLookScript.a.firstTimePickup) {
					MFDManager.a.CenterTabButtonClickSilent(2,true);
					MouseLookScript.a.firstTimePickup = false;
				}
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
	// overt = update HUD in response to player action, else silent add.
	public void AddHardwareToInventory(int index, int constIndex,
	                                   int hwversion, bool overt) {
		if (index < 0) return;

		if (hwversion < 1) {
			Const.sprint("BUG: Adding hardware with no version, using 0 (v1)");
			hwversion = 1;
		}

		if (overt) MFDManager.a.SendInfoToItemTab(constIndex);
		if (hwversion <= hardwareVersion[index] && overt) {
		    Const.sprint(Const.a.stringTable[46] );
		    return;
		}

		int textIndex = 21;
		int button8Index = -1;
		switch(index) {
			case 0: textIndex = 21; break; // System Analyzer
			case 1: // Navigation Unit
			    textIndex = 22;
			    // Turn on HUD compass
				MouseLookScript.a.compassContainer.SetActive(true);
				MouseLookScript.a.automapContainerLH.SetActive(true);
				MouseLookScript.a.automapContainerRH.SetActive(true);
				if (hwversion >= 2) {
				    MouseLookScript.a.compassMidpoints.SetActive(true);
				}
				
				if (hwversion >= 3) {
					MouseLookScript.a.compassSmallTicks.SetActive(true);
					MouseLookScript.a.compassLargeTicks.SetActive(true);
				}
				
				if (overt) {
				    MFDManager.a.OpenTab(2,true,TabMSG.None,0,MFDManager.a.lastAutomapSideRH ? Handedness.RH : Handedness.LH);
				}

				// Go through all HealthManagers in the game and initialize the
				// linked overlays now.
				int i,k;
				List<GameObject> hmGOs = new List<GameObject>();
				List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
				for (i=0;i<allParents.Count;i++) {
					Component[] compArray = allParents[i].GetComponentsInChildren(typeof(HealthManager),true); // find all HealthManager components, including inactive (hence the true here at the end)
					for (k=0;k<compArray.Length;k++) {
						hmGOs.Add(compArray[k].gameObject); //add the gameObject associated with all HealthManager components in the scene
					}
				}
				
				allParents.Clear();
				allParents = null; // Done with it.

				for (i=0;i<hmGOs.Count;i++) {
					if (hmGOs[i] == null) continue;

					HealthManager hmT = hmGOs[i].GetComponent<HealthManager>();
					if (hmT == null) continue;

					if ((hmT.isNPC || hmT.isSecCamera)) {
						hmT.Awake(); // Set up slots.
						hmT.Start(); // Setup overlay.
					}
				}
				
				hmGOs.Clear();
				hmGOs = null; // Done with it.
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
		hasHardware[index] = true;
		hardwareVersion[index] = hwversion; // One-based.
		hardwareVersionSetting[index] = hwversion - 1; // Zero-based...hey it
													   // made sense at some
													   // point ok!
		// Enable HUD button
		if (button8Index >= 0 && button8Index < 8) {
			MouseLookScript.a.hardwareButtons[button8Index].SetActive(true);
			hardwareButtonManager.SetVersionIconForButton(hardwareIsActive[index],
			                                  hardwareVersionSetting[index],4);
			hardwareButtonManager.buttons[button8Index].gameObject.SetActive(true);
		}

		if (overt) {
		    Const.sprint(Const.a.stringTable[textIndex + 326] + " v"
					     + hwversion.ToString() );
		}

		if (MouseLookScript.a.firstTimePickup && overt) {
			MFDManager.a.CenterTabButtonClickSilent(1,true);
		}

		ActivateHardwareButton(index);
		if (overt) MFDManager.a.NotifyToCenterTab(1);
	}

	// The following utility functions make the code more explicit by removing
	// magic numbers all over the place and allow for adding index protections.
	// ------------------------------------------------------------------------
	// Navunit utility functions. [1]
	public int NavUnitVersion() {
		return hardwareVersion[1];
	}

	// Biomonitor utility functions. [6]
	public int BioMonitorVersion() {
		return hardwareVersion[6];
	}

	public bool BioMonitorActive() {
		return hasHardware[6] && hardwareIsActive[6];
	}

	// Lantern utility functions. [7]
	public int LanternVersion() {
		return hardwareVersion[7];
	}
	
	public bool LanternActive() {
		return hasHardware[7] && hardwareIsActive[7];
	}

	// Envirosuit utility functions. [8]
	public int EnvirosuitVersion() {
		return hardwareVersion[8];
	}

	// Booster utility functions. [9]
	public bool BoosterSetToSkates() {
		return hardwareVersionSetting[9] == 0;
	}

	public bool BoosterSetToBoost() {
		return hardwareVersionSetting[9] >= 1;
	}

	public bool BoosterActive() {
		return hasHardware[9] && hardwareIsActive[9];
	}

	// JumpJets utility functions. [10]
	public void JumpJetsToggle() {
		hardwareIsActive[10] = !hardwareIsActive[10];
	}

	public int JumpJetsVersion() {
		return hardwareVersion[10];
	}

	public bool JumpJetsActive() {
		return hasHardware[10] && hardwareIsActive[10];
	}

	// Called by main menu since as this uses OnGUI it draws on top.
	public void HideBioMonitor() {
		if (hardwareButtonManager == null) return;
		if (hardwareButtonManager.bioMonitorContainer == null) return;

		hardwareButtonManager.bioMonitorContainer.SetActive(false);
	}

	// Called by main menu to restore it to what it was.
	public void UnHideBioMonitor() {
		if (!BioMonitorActive()) return; // Wasn't on before.
		if (hardwareButtonManager == null) return;
		if (hardwareButtonManager.bioMonitorContainer == null) return;

		hardwareButtonManager.bioMonitorContainer.SetActive(true);
	}
	// ------------------------------------------------------------------------
	//--- End Hardware ---

	// General
    public bool AddGeneralObjectToInventory(int index, int customIndex) {
		if (index < 0) return false;

        for (int i=1;i<14;i++) { // Skip index 0, Access Cards button
            if (generalInventoryIndexRef[i] != -1) continue;

			if (!HasAnyAccessCards() && generalInvCurrent == 0) { // Nothing.
				generalInvCurrent = i; // Set to 1 (ideally).
			}

			generalInventoryIndexRef[i] = index;

			// Item added to general inventory
			Const.sprint(Const.a.stringTable[index + 326]
						 + Const.a.stringTable[31]);

			GeneralInvButton gv = genButtons[i].GetComponent<GeneralInvButton>();
			if (gv != null) {
				gv.useableItemIndex = index;
				gv.customIndex = customIndex;
			}

			if (Inventory.a.generalInvCurrent == i) { // Only if current.
				MFDManager.a.SendInfoToItemTab(index,customIndex);
			}

			MFDManager.a.NotifyToCenterTab(2);
			if (MouseLookScript.a.firstTimePickup) {
				MFDManager.a.CenterTabButtonClickSilent(2,true);
				MouseLookScript.a.firstTimePickup = false;
			}

			return true;
        }
		return false;
    }

	public void GeneralInventoryActivate() {
		GeneralInvButton ginvb =
			    genButtons[generalInvCurrent].GetComponent<GeneralInvButton>();
		if (ginvb!= null) {
			ginvb.GeneralInvUse();
			if (generalInvCurrent != 0) {
				generalInventoryIndexRef[generalInvCurrent] = -1;
			}
		} else Debug.Log("BUG: Current general inv button was null");
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
		grenButtons[nextIndex].GrenadeInvSelect();
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
		grenButtons[nextIndex].GrenadeInvSelect();
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

		if (MouseLookScript.a.firstTimePickup) {
			MFDManager.a.CenterTabButtonClickSilent(0,true);
		}

		if (grenAmmo[0] == 0 && grenAmmo[1] == 0 && grenAmmo[2] == 0
			&& grenAmmo[3] == 0 && grenAmmo[4] == 0 && grenAmmo[5] == 0
			&& grenAmmo[6] == 0) {

			grenadeCurrent = index;
		}

		grenAmmo[index]++;
		Const.sprint(Const.a.stringTable[useableIndex + 326]
					 + Const.a.stringTable[34] );

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

	private int FindNextUnreadLog() {
		for (int i=readLog.Length - 1;i>=0;i--) {
			if (!readLog[i] && hasLog[i]) return i;
		}
		return -1;
	}

	public void PlayLog(int logIndex) {
		if (logIndex < 0) return;

		SFXSource.Stop();
		if (!Inventory.a.hasHardware[2]) return;

		Utils.PlayOneShotSavable(SFXSource,Const.a.audioLogs[logIndex],((float)Const.a.AudioVolumeMessage)/100f); // Play the log audio
		if (!readLog[logIndex]) QuestLogNotesManager.a.LogAdded(logIndex);
		readLog[logIndex] = true;
		if (Const.a.audioLogType[logIndex] == AudioLogType.Vmail) {
			MouseLookScript.a.vmailActive = true; // allow click to end
			MouseLookScript.a.ForceInventoryMode();
			switch (logIndex) {
				case 119:
					vmailbetajet.SetActive(true);
					vmailbetajetVideo.url = Application.streamingAssetsPath + "/betajet.webm";
					vmailbetajetVideo.Play();
					if (!MainMenuHandler.a.dataFound) vmailbetajetVideo.SetDirectAudioMute(0,true);
					else vmailbetajetVideo.SetDirectAudioMute(0,false);

					break;
				case 116:
					vmailbridgesep.SetActive(true);
					vmailbridgesepVideo.url = Application.streamingAssetsPath + "/bridgesep.webm";
					vmailbridgesepVideo.Play();
					if (!MainMenuHandler.a.dataFound) vmailbridgesepVideo.SetDirectAudioMute(0,true);
					else vmailbridgesepVideo.SetDirectAudioMute(0,false);

					break;
				case 117:
					vmailcitadestruct.SetActive(true);
					vmailcitadestructVideo.url = Application.streamingAssetsPath + "/citadestruct.webm";
					vmailcitadestructVideo.Play();
					if (!MainMenuHandler.a.dataFound) vmailcitadestructVideo.SetDirectAudioMute(0,true);
					else vmailcitadestructVideo.SetDirectAudioMute(0,false);

					break;
				case 110:
					vmailgenstatus.SetActive(true);
					vmailgenstatusVideo.url = Application.streamingAssetsPath + "/genstatus.webm";
					vmailgenstatusVideo.Play();
					if (!MainMenuHandler.a.dataFound) vmailgenstatusVideo.SetDirectAudioMute(0,true);
					else vmailgenstatusVideo.SetDirectAudioMute(0,false);

					break;
				case 114:
					vmaillaserdest.SetActive(true);
					vmaillaserdestVideo.url = Application.streamingAssetsPath + "/laserdest.webm";
					vmaillaserdestVideo.Play();
					if (!MainMenuHandler.a.dataFound) vmaillaserdestVideo.SetDirectAudioMute(0,true);
					else vmaillaserdestVideo.SetDirectAudioMute(0,false);

					break;
				case 120:
					vmailshieldsup.SetActive(true);
					vmailshieldsupVideo.url = Application.streamingAssetsPath + "/shieldsup.webm";
					vmailshieldsupVideo.Play();
					if (!MainMenuHandler.a.dataFound) vmailshieldsupVideo.SetDirectAudioMute(0,true);
					else vmailshieldsupVideo.SetDirectAudioMute(0,false);

					break;
			}
		}
		Const.sprint(Const.a.stringTable[1020] + Const.a.audiologNames[logIndex]); // "Playing "
		MFDManager.a.SendAudioLogToDataTab(logIndex);
	}

	public void PlayLastAddedLog(int logIndex) {
		if (logIndex == -1) return;

		PlayLog(logIndex);
		lastAddedIndex = -1;
	}

	public void AddAudioLogToInventory(int index) {
		if (index < 0) {
			Debug.Log("BUG: Audio log picked up has no assigned index (-1)");
			return;
		}

		if (index == 128) {
			// Trioptimum Funpack Module, don't play on company time!
			Const.sprint(Const.a.stringTable[309]);
			return;
		}

		hasLog[index] = true;
		lastAddedIndex = index;
		numLogsFromLevel[Const.a.audioLogLevelFound[index]]++;
		MouseLookScript.a.logContentsManager.InitializeLogsFromLevelIntoFolder();
		MFDManager.a.SendInfoToItemTab(6);
		if (Const.a.audioLogType[index] == AudioLogType.Email) {
			hasNewEmail = true;
		} else if (Const.a.audioLogType[index] == AudioLogType.Normal) {
			hasNewLogs = true;
		}

		if (hasHardware[2] == true) {
			// Audio log ## picked up.  Press '##' to play back.
			Const.sprint(Const.a.stringTable[36] + Const.a.audiologNames[index]
						 + Const.a.stringTable[37]
						 + Const.a.InputValues[Const.a.InputCodeSettings[15]]
						 + Const.a.stringTable[38]);
		} else {
			// Audio log ## picked up.  Proper hardware not detected to play.
			Const.sprint(Const.a.stringTable[36] + Const.a.audiologNames[index]
						 + Const.a.stringTable[310]);
		}
	}
	//--- End Logs ---

	// Patches
	public void PatchCycleDown(bool useSound) {
		int nextIndex = patchCurrent - 1; // Add 1 to get slot above this.
		if (nextIndex < 0) nextIndex = 6; // Wraparound to top.
		patchCurrent = nextIndex;
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
		patchButtonScripts[nextIndex].PatchSelect(useSound);
	}

	public void PatchCycleUp(bool useSound) {
		int nextIndex = patchCurrent + 1; // Add 1 to get slot above this.
		if (nextIndex > 6) nextIndex = 0; // Wraparound to bottom.
		patchCurrent = nextIndex;
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
		patchButtonScripts[nextIndex].PatchSelect(useSound);
	}

	// index [0,6]: Index into the list of just the 7 grenade types.
	public void AddPatchToInventory (int index,int constIndex) {
		if (index < 0) return;

		if (MouseLookScript.a.firstTimePickup) MFDManager.a.CenterTabButtonClickSilent(0,true);
		patchCounts[index]++;
		if (patchCounts[patchCurrent] == 0) patchCurrent = index;

		// Update UI text
		for (int i = 0; i < 7; i++) {
			patchCountTextObjects[i].text = patchCounts[i].ToString ();
			if (i == index) patchCountTextObjects[i].color = Const.a.ssYellowText; // Yellow
			else  patchCountTextObjects[i].color = Const.a.ssGreenText; // Green
		}
		MFDManager.a.SendInfoToItemTab(constIndex);
		MFDManager.a.NotifyToCenterTab(0);
		Const.sprint(Const.a.stringTable[constIndex + 326]
					 + Const.a.stringTable[35]); //  added to patch inventory
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
				Utils.PlayUIOneShotSavable(86); // frob_hardware
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
				Utils.PlayUIOneShotSavable(86); // frob_hardware
				Const.sprint(Const.a.stringTable[445] + softVersions[1].ToString() + Const.a.stringTable[458],Const.a.player1);
				return true;
			case SoftwareType.CShield:	
				softs[2].SetActive(true);
				if (vers > softVersions[2]) softVersions[2] = vers;
				else Const.sprint(Const.a.stringTable[46],Const.a.player1);
				cshieldVersionText.text = softVersions[2].ToString();
				hasSoft[2] = true;
				Utils.PlayUIOneShotSavable(86); // frob_hardware
				Const.sprint(Const.a.stringTable[446] + softVersions[2].ToString() + Const.a.stringTable[458],Const.a.player1);
				return true;
			case SoftwareType.Turbo:
				if (currentCyberItem == -1f) currentCyberItem = 0;
				softs[3].SetActive(true);
				softVersions[3]++;
				turboCountText.text = softVersions[3].ToString();
				hasSoft[3] = true;
				Utils.PlayUIOneShotSavable(86); // frob_hardware
				Const.sprint(Const.a.stringTable[447],Const.a.player1);
				return true;
			case SoftwareType.Decoy:	
				if (currentCyberItem == -1f) currentCyberItem = 1;
				softs[4].SetActive(true);
				softVersions[4]++;
				decoyCountText.text = softVersions[4].ToString();
				hasSoft[4] = true;
				Utils.PlayUIOneShotSavable(86); // frob_hardware
				Const.sprint(Const.a.stringTable[448],Const.a.player1);
				return true;
			case SoftwareType.Recall:	
				if (currentCyberItem == -1f) currentCyberItem = 2;
				softs[5].SetActive(true);
				softVersions[5]++;
				recallCountText.text = softVersions[5].ToString();
				hasSoft[5] = true;
				Utils.PlayUIOneShotSavable(86); // frob_hardware
				Const.sprint(Const.a.stringTable[449],Const.a.player1);
				return true;
			case SoftwareType.Game:		
				softs[6].SetActive(true);
				hasNewData = true;
				hasMinigame[vers] = true;
				miniGameButton[vers].SetActive(true);
				switch(vers) {
					case 0: // Ping
							Const.sprint(Const.a.stringTable[450],Const.a.player1);
							break;
					case 1: // 15
							Const.sprint(Const.a.stringTable[451],Const.a.player1);
							break;
					case 2: // Wing 0
							Const.sprint(Const.a.stringTable[452],Const.a.player1);
							break;
					case 3: // Botbounce
							Const.sprint(Const.a.stringTable[453],Const.a.player1);
							break;
					case 4: // Eel Zapper
							Const.sprint(Const.a.stringTable[454],Const.a.player1);
							break;
					case 5: // Road
							Const.sprint(Const.a.stringTable[455],Const.a.player1);
							break;
					case 6: // TriopToe
							Const.sprint(Const.a.stringTable[456],Const.a.player1);
							break;
				}
				Utils.PlayUIOneShotSavable(86); // frob_hardware
				
				return true;
			case SoftwareType.Data:
				hasNewData = true;
				Utils.PlayUIOneShotSavable(87); // frob_item
				Const.sprint(Const.a.stringTable[457],Const.a.player1);
				hasLog[vers] = true;
				return true;
			case SoftwareType.Integrity:
				//Debug.Log("Cyber integrity touched");
				if (hm.cyberHealth >=255) return false;
				Utils.PlayUIOneShotSavable(86); // frob_hardware
				hm.cyberHealth += 77f;
				if (hm.cyberHealth > 255f) hm.cyberHealth = 255f;
				MFDManager.a.DrawTicks(true);
				Const.sprint(Const.a.stringTable[459],Const.a.player1);
				return true;
			case SoftwareType.Keycard:
				hasNewData = true;
				if (vers < 0 || vers > 110) vers = 81; // At least give them STD.
				AddAccessCardToInventory(vers);
				return true;
		}
		return false;
	}
	//--- End Software

	// Weapons
	public void RemoveWeapon(int wepButIndex) {
		weaponInventoryIndices[wepButIndex] = -1;     // Remove the weapon by
		weaponInventoryAmmoIndices[wepButIndex] = -1; // setting it to -1.
		weaponButtonText[wepButIndex].text = string.Empty;
	}

	public void UpdateAmmoText() {
		for (int i=0;i<weaponShotsInventoryText.Length;i++) {
			weaponShotsInventoryText[i] = GetTextForWeaponAmmo(i);
		}
		int slot1 = weaponInventoryIndices[0];
		int slot2 = weaponInventoryIndices[1];
		int slot3 = weaponInventoryIndices[2];
		int slot4 = weaponInventoryIndices[3];
		int slot5 = weaponInventoryIndices[4];
		int slot6 = weaponInventoryIndices[5];
		int slot7 = weaponInventoryIndices[6];
		numweapons = 0;
		if (slot1 != -1) numweapons++;
		if (slot2 != -1) numweapons++;
		if (slot3 != -1) numweapons++;
		if (slot4 != -1) numweapons++;
		if (slot5 != -1) numweapons++;
		if (slot6 != -1) numweapons++;
		if (slot7 != -1) numweapons++;

		if (WeaponCurrent.a.weaponCurrent < 0) return;
		if (WeaponCurrent.a.weaponCurrent > 7) return;

		MFDManager.a.SetAmmoIcons(WeaponCurrent.a.weaponIndex,
						wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent]); 
	}

	public string GetTextForWeaponAmmo(int index) {
		globalLookupIndex = weaponInventoryIndices[index];
		retval = "0";
		switch (globalLookupIndex) {
		case 36:
			//Mark3 Assault Rifle
			if (wepLoadedWithAlternate[index]) {
				retval = WeaponCurrent.a.currentMagazineAmount2[index].ToString() + "pn | ";
			} else {
				retval = WeaponCurrent.a.currentMagazineAmount[index].ToString() + "mg | ";
			}

			retval += wepAmmo[0].ToString() + "mg, " + wepAmmoSecondary[0].ToString() + "pn";
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
			if (wepLoadedWithAlternate[index]) {
				retval = WeaponCurrent.a.currentMagazineAmount2[index].ToString() + "tq | ";
			} else {
				retval = WeaponCurrent.a.currentMagazineAmount[index].ToString() + "nd | ";
			}

			retval += wepAmmo[2].ToString() + "nd, " + wepAmmoSecondary[2].ToString() + "tq";
			break;
		case 39:
			//AM-27 Flechette
			if (wepLoadedWithAlternate[index]) {
				retval = WeaponCurrent.a.currentMagazineAmount2[index].ToString() + "sp | ";
			} else {
				retval = WeaponCurrent.a.currentMagazineAmount[index].ToString() + "hn | ";
			}

			retval += wepAmmo[3].ToString() + "hn, " + wepAmmoSecondary[3].ToString() + "sp";
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
			if (wepLoadedWithAlternate[index]) {
				retval = WeaponCurrent.a.currentMagazineAmount2[index].ToString() + "sg | ";
			} else {
				retval = WeaponCurrent.a.currentMagazineAmount[index].ToString() + "hw | ";
			}

			retval += wepAmmo[7].ToString() + "hw, " + wepAmmoSecondary[7].ToString() + "sg";
			break;
		case 44:
			//SB-20 Magpulse
			if (wepLoadedWithAlternate[index]) {
				retval = WeaponCurrent.a.currentMagazineAmount2[index].ToString() + "su | ";
			} else {
				retval = WeaponCurrent.a.currentMagazineAmount[index].ToString() + "cr | ";
			}

			retval += wepAmmo[8].ToString() + "cr, " + wepAmmoSecondary[8].ToString() + "su";
			break;
		case 45:
			//ML-41 Pistol
			if (wepLoadedWithAlternate[index]) {
				retval = WeaponCurrent.a.currentMagazineAmount2[index].ToString() + "tf | ";
			} else {
				retval = WeaponCurrent.a.currentMagazineAmount[index].ToString() + "st | ";
			}

			retval += wepAmmo[9].ToString() + "st, " + wepAmmoSecondary[9].ToString() + "tf";
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
			retval = WeaponCurrent.a.currentMagazineAmount[index].ToString() + "rl | ";
			retval += wepAmmo[11].ToString() + "rl";
			break;
		case 48:
			//DC-05 Riotgun
			retval = WeaponCurrent.a.currentMagazineAmount[index].ToString() + "rb | ";
			retval += wepAmmo[12].ToString() + "rb";
			break;
		case 49:
			//RF-07 Skorpion
			if (wepLoadedWithAlternate[index]) {
				retval = WeaponCurrent.a.currentMagazineAmount2[index].ToString() + scorpLg + " | ";
			} else {
				retval = WeaponCurrent.a.currentMagazineAmount[index].ToString() + scorpSmall + " | ";
			}

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

		Const.sprint(Const.a.stringTable[constIndex + 326]
					 + Const.a.stringTable[630]); // Item added to ammo

		MFDManager.a.NotifyToCenterTab(0);
		MFDManager.a.SendInfoToItemTab(constIndex);
	}

    public bool AddWeaponToInventory(int index, int ammo1, int ammo2,
									 bool loadedAlt) { // index = usableItem index
		if (index < 0) return false;

		MFDManager.a.OpenTab(0, true, TabMSG.Weapon, 0,Handedness.LH);
		MFDManager.a.CenterTabButtonClickSilent (0,true); // Weapons are so important we always switch it.
        for (int i=0;i<7;i++) {
            if (weaponInventoryIndices[i] >= 0) continue;

			weaponInventoryIndices[i] = index;
			weaponButtonText[i].text = Const.a.stringTable[326 + index];
			int index16 = WeaponFire.Get16WeaponIndexFromConstIndex(index);
			WeaponButton wepBut = MFDManager.a.wepbutMan.wepButtonsScripts[i];
			wepBut.useableItemIndex = index;
			float egSet = GetDefaultEnergySettingForWeaponFrom16Index(index16);
			WeaponCurrent.a.weaponEnergySetting[i] = egSet;
			if (i == 0) {
				WeaponCurrent.a.weaponCurrentPending = i;
				WeaponCurrent.a.weaponIndexPending = index;
				WeaponFire.a.StartWeaponDip(0.5f);

				// Pop it back to start to be sure
				WeaponFire.a.reloadContainer.localPosition =
					WeaponFire.a.reloadContainerHome;

				WeaponCurrent.a.justChangedWeap = true;
				MFDManager.a.SendInfoToItemTab(index); // Notify item tab we
				MFDManager.a.SendInfoToItemTab(index); // clicked on a weapon.
				MFDManager.a.UpdateHUDAmmoCountsEither();
				WeaponFire.a.CompleteWeaponChange();
			}

			if (loadedAlt && ammo2 > 0) {
				WeaponCurrent.a.currentMagazineAmount2[i] = ammo2;
				if (ammo1 > 0) wepAmmo[index16] += ammo1;
				wepLoadedWithAlternate[i] = true;
			} else {
				WeaponCurrent.a.currentMagazineAmount[i] = ammo1;
				if (ammo2 > 0) wepAmmoSecondary[index16] += ammo2;
				wepLoadedWithAlternate[i] = false;

			}

			Const.sprint(Const.a.stringTable[index + 326]
						 + Const.a.stringTable[33]);

			MFDManager.a.NotifyToCenterTab(0);
			return true;
        }
		return false;
    }
	//--- End Weapons ---


	public static string Save(GameObject go) {
		int j;
		Inventory inv = go.GetComponent<Inventory>();
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.UintToString(inv.weaponInventoryIndices[0],"weaponInventoryIndices[0]"));
		for (j=1;j<7;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.UintToString(inv.weaponInventoryIndices[j],"weaponInventoryIndices[" + j.ToString() + "]")); }
		for (j=0;j<7;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.UintToString(inv.weaponInventoryAmmoIndices[j],"weaponInventoryAmmoIndices[" + j.ToString() + "]")); }
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.numweapons,"numweapons"));
		for (j=0;j<16;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.UintToString(inv.wepAmmo[j],"wepAmmo[" + j.ToString() + "]")); }
		for (j=0;j<16;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.UintToString(inv.wepAmmoSecondary[j],"wepAmmoSecondary[" + j.ToString() + "]")); }
		for (j=0;j<7;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.FloatToString(inv.currentEnergyWeaponHeat[j],"currentEnergyWeaponHeat[" + j.ToString() + "]")); }
		for (j=0;j<7;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.BoolToString(inv.wepLoadedWithAlternate[j],"wepLoadedWithAlternate[" + j.ToString() + "]")); }
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.grenadeCurrent,"grenadeCurrent"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(inv.nitroTimeSetting,"nitroTimeSetting"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(inv.earthShakerTimeSetting,"earthShakerTimeSetting"));
		for (j=0;j<7;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.UintToString(inv.grenAmmo[j],"grenAmmo[" + j.ToString() + "]")); }
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.patchCurrent,"patchCurrent"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.patchIndex,"patchIndex"));
		for (j=0;j<7;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.UintToString(inv.patchCounts[j],"patchCounts[" + j.ToString() + "]")); }
		for (j=0;j<134;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.BoolToString(inv.hasLog[j],"hasLog[" + j.ToString() + "]")); }
		for (j=0;j<134;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.BoolToString(inv.readLog[j],"readLog[" + j.ToString() + "]")); }
		for (j=0;j<10;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.UintToString(inv.numLogsFromLevel[j],"numLogsFromLevel[" + j.ToString() + "]")); }
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.lastAddedIndex,"lastAddedIndex"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(inv.beepDone,"beepDone"));
		for (j=0;j<13;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.BoolToString(inv.hasHardware[j],"hasHardware[" + j.ToString() + "]")); }
		for (j=0;j<13;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.UintToString(inv.hardwareVersion[j],"hardwareVersion[" + j.ToString() + "]")); }
		for (j=0;j<13;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.UintToString(inv.hardwareVersionSetting[j],"hardwareVersionSetting[" + j.ToString() + "]")); }
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.hardwareInvCurrent,"hardwareInvCurrent"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.hardwareInvIndex,"hardwareInvIndex"));
		for (j=0;j<13;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.BoolToString(inv.hardwareIsActive[j],"hardwareIsActive[" + j.ToString() + "]")); }
		for (j=0;j<32;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.IntToString(Utils.AccessCardTypeToInt(inv.accessCardsOwned[j]),"accessCardsOwned[" + j.ToString() + "]")); } 
		for (j=0;j<14;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.UintToString(inv.generalInventoryIndexRef[j],"generalInventoryIndexRef[" + j.ToString() + "]")); }
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.generalInvCurrent,"generalInvCurrent"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.generalInvIndex,"generalInvIndex"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.currentCyberItem,"currentCyberItem"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(inv.isPulserNotDrill,"isPulserNotDrill"));
		for (j=0;j<7;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.UintToString(inv.softVersions[j],"softVersions[" + j.ToString() + "]")); } 
		for (j=0;j<7;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.BoolToString(inv.hasSoft[j],"hasSoft[" + j.ToString() + "]")); }
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.emailCurrent,"emailCurrent"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(inv.emailIndex,"emailIndex"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(inv.hasNewNotes,"hasNewNotes"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(inv.hasNewData,"hasNewData"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(inv.hasNewLogs,"hasNewLogs"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(inv.hasNewEmail,"hasNewEmail"));
		for (j=0;j<7;j++) { s1.Append(Utils.splitChar); s1.Append(Utils.BoolToString(inv.hasMinigame[j],"hasMinigame[" + j.ToString() + "]")); }
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		Inventory inv = go.GetComponent<Inventory>();
		int j;
		for (j=0;j<7;j++) { inv.weaponInventoryIndices[j] = Utils.GetIntFromString(entries[index],"weaponInventoryIndices[" + j.ToString() + "]"); index++; }
		for (j=0;j<7;j++) { inv.weaponInventoryAmmoIndices[j] = Utils.GetIntFromString(entries[index],"weaponInventoryAmmoIndices[" + j.ToString() + "]"); index++; }
		inv.numweapons = Utils.GetIntFromString(entries[index],"numweapons"); index++;
		for (j=0;j<16;j++) { inv.wepAmmo[j] = Utils.GetIntFromString(entries[index],"wepAmmo[" + j.ToString() + "]"); index++; }
		for (j=0;j<16;j++) { inv.wepAmmoSecondary[j] = Utils.GetIntFromString(entries[index],"wepAmmoSecondary[" + j.ToString() + "]"); index++; }
		for (j=0;j<7;j++) { inv.currentEnergyWeaponHeat[j] = Utils.GetFloatFromString(entries[index],"currentEnergyWeaponHeat[" + j.ToString() + "]"); index++; }
		for (j=0;j<7;j++) { inv.wepLoadedWithAlternate[j] = Utils.GetBoolFromString(entries[index],"wepLoadedWithAlternate[" + j.ToString() + "]"); index++; }
        for (int i=0;i<7;i++) {
			int dex = inv.weaponInventoryIndices[i];
			if (dex < 0) continue;

			inv.weaponButtonText[i].text = Const.a.stringTable[dex + 326];
		}

		inv.grenadeCurrent = Utils.GetIntFromString(entries[index],"grenadeCurrent"); index++;
		inv.nitroTimeSetting = Utils.GetFloatFromString(entries[index],"nitroTimeSetting"); index++;
		inv.earthShakerTimeSetting = Utils.GetFloatFromString(entries[index],"earthShakerTimeSetting"); index++;
		for (j=0;j<7;j++) { inv.grenAmmo[j] = Utils.GetIntFromString(entries[index],"grenAmmo[" + j.ToString() + "]"); index++; }
		inv.patchCurrent = Utils.GetIntFromString(entries[index],"patchCurrent"); index++;
		inv.patchIndex = Utils.GetIntFromString(entries[index],"patchIndex"); index++;
		for (j=0;j<7;j++) { inv.patchCounts[j] = Utils.GetIntFromString(entries[index],"patchCounts[" + j.ToString() + "]"); index++; }
		for (j=0;j<134;j++) { inv.hasLog[j] = Utils.GetBoolFromString(entries[index],"hasLog[" + j.ToString() + "]"); index++; }
		for (j=0;j<134;j++) { inv.readLog[j] = Utils.GetBoolFromString(entries[index],"readLog[" + j.ToString() + "]"); index++; }
		for (j=0;j<10;j++) { inv.numLogsFromLevel[j] = Utils.GetIntFromString(entries[index],"numLogsFromLevel[" + j.ToString() + "]"); index++; }
		inv.lastAddedIndex = Utils.GetIntFromString(entries[index],"lastAddedIndex"); index++;
		inv.beepDone = Utils.GetBoolFromString(entries[index],"beepDone"); index++;
		for (j=0;j<13;j++) { inv.hasHardware[j] = Utils.GetBoolFromString(entries[index],"hasHardware[" + j.ToString() + "]"); index++; }
		for (j=0;j<13;j++) { inv.hardwareVersion[j] = Utils.GetIntFromString(entries[index],"hardwareVersion[" + j.ToString() + "]"); index++; }
		for (j=0;j<13;j++) { inv.hardwareVersionSetting[j] = Utils.GetIntFromString(entries[index],"hardwareVersionSetting[" + j.ToString() + "]"); index++; }
		inv.hardwareInvCurrent = Utils.GetIntFromString(entries[index],"hardwareInvCurrent"); index++;
		inv.hardwareInvIndex = Utils.GetIntFromString(entries[index],"hardwareInvIndex"); index++;
		for (j=0;j<13;j++) { inv.hardwareIsActive[j] = Utils.GetBoolFromString(entries[index],"hardwareIsActive[" + j.ToString() + "]"); index++; }
        if (Inventory.a.hasHardware[1]) { // Explicitly check primary instance.
			MouseLookScript.a.compassContainer.SetActive(true);
			MouseLookScript.a.automapContainerLH.SetActive(true);
			MouseLookScript.a.automapContainerRH.SetActive(true);
			if (inv.hardwareVersion[1] >= 2) {
			    MouseLookScript.a.compassMidpoints.SetActive(true);
			}

			if (inv.hardwareVersion[1] >= 3) {
				MouseLookScript.a.compassSmallTicks.SetActive(true);
				MouseLookScript.a.compassLargeTicks.SetActive(true);
			}
		} else {
			MouseLookScript.a.compassContainer.SetActive(false);
			MouseLookScript.a.automapContainerLH.SetActive(false);
			MouseLookScript.a.automapContainerRH.SetActive(false);
			MouseLookScript.a.compassSmallTicks.SetActive(false);
			MouseLookScript.a.compassLargeTicks.SetActive(false);
		}

		
		
	    for (j=0;j<12;j++) {
    		int button8Index = -1;
    		switch(j) {
    			// 0 System Analyzer
    			// 1 Navigation Unit (done above just for the else.
    			case 2: button8Index = 5; break; // Datareader
    			case 3: button8Index = 1; break; // Sensaround
    			// 4 Target Identifier
    			case 5: button8Index = 3; break; // Energy Shield
    			case 6: button8Index = 0; break; // Biomonitor
    			case 7: button8Index = 2; break; // Head Mounted Lantern
    			// 8 Envirosuit
    			case 9: button8Index = 6; break; // Turbo Motion Booster
    			case 10:button8Index = 7; break; // Jump Jet Boots
    			case 11:button8Index = 4; break; // Infrared Night Vision Enhancement
    		}
    		
    		if (!inv.hasHardware[j]) {
				if (button8Index >= 0 && button8Index < 8) {
					MouseLookScript.a.hardwareButtons[button8Index].SetActive(false);
					inv.hardwareButtonManager.SetVersionIconForButton(
						inv.hardwareIsActive[j],
						inv.hardwareVersionSetting[j],4
					);

					inv.hardwareButtonManager.buttons[button8Index].gameObject.SetActive(false);
				}

				inv.hwButtons[j].SetActive(false);
			} else {
				if (button8Index >= 0 && button8Index < 8) {
					MouseLookScript.a.hardwareButtons[button8Index].SetActive(true);
					inv.hardwareButtonManager.SetVersionIconForButton(
						inv.hardwareIsActive[j],
						inv.hardwareVersionSetting[j],4
					);

					inv.hardwareButtonManager.buttons[button8Index].gameObject.SetActive(true);
				}

				inv.ActivateHardwareButton(j);
			}
	    }
		
		// 0
		
		// 1
		
	    // 2 UI states saved elsewhere for EReader
		
	    if (inv.hardwareIsActive[3]) inv.hardwareButtonManager.SensaroundOn(); // 3
		else inv.hardwareButtonManager.SensaroundOff(); 
		
		// 4
		
	    if (inv.hardwareIsActive[5]) inv.hardwareButtonManager.ShieldOn(); // 5
		else inv.hardwareButtonManager.ShieldOff();
		
	    if (inv.BioMonitorActive()) inv.hardwareButtonManager.BioOn(); // 6
		else inv.hardwareButtonManager.BioOff();
		
	    if (inv.LanternActive()) inv.hardwareButtonManager.LanternOn(); // 7
		else inv.hardwareButtonManager.LanternOff();
		
		// 8
		
	    if (inv.BoosterActive()) inv.hardwareButtonManager.BoosterOn(); // 9
		else inv.hardwareButtonManager.BoosterOff();
		
	    if (inv.JumpJetsActive()) inv.hardwareButtonManager.JumpJetsOn(); // 10
		else inv.hardwareButtonManager.JumpJetsOff();

	    if (inv.hardwareIsActive[11]) inv.hardwareButtonManager.InfraredOn(); // 11
		else inv.hardwareButtonManager.InfraredOff();
			
		// 12

		for (j=0;j<32;j++) { inv.accessCardsOwned[j] = Utils.IntToAccessCardType(Utils.GetIntFromString(entries[index],"accessCardsOwned[" + j.ToString() + "]")); index++; }
		for (j=0;j<14;j++) { inv.generalInventoryIndexRef[j] = Utils.GetIntFromString(entries[index],"generalInventoryIndexRef[" + j.ToString() + "]"); index++; }
		inv.generalInvCurrent = Utils.GetIntFromString(entries[index],"generalInvCurrent"); index++;
		inv.generalInvIndex = Utils.GetIntFromString(entries[index],"generalInvIndex"); index++;

		for (int i=1; i<14; i++) {
			GeneralInvButton genbut = inv.genButtons[i].transform.GetComponent<GeneralInvButton>();
			genbut.useableItemIndex = inv.generalInventoryIndexRef[i];
			int referenceIndex = genbut.useableItemIndex;
			if (inv.generalInventoryIndexRef[i] > -1) {
				inv.genButtonsText[i].text =
					Const.a.stringTable[inv.generalInventoryIndexRef[i] + 326];
			} else {
				inv.genButtonsText[i].text = string.Empty;
			}

			if (i == inv.generalInvCurrent) {
				inv.genButtonsText[i].color = Const.a.ssYellowText; // Yellow
			} else {
				inv.genButtonsText[i].color = Const.a.ssGreenText; // Green
			}
		}
		inv.currentCyberItem = Utils.GetIntFromString(entries[index],"currentCyberItem"); index++;
		inv.isPulserNotDrill = Utils.GetBoolFromString(entries[index],"isPulserNotDrill"); index++;
		for (j=0;j<7;j++) { inv.softVersions[j] = Utils.GetIntFromString(entries[index],"softVersions[" + j.ToString() + "]"); index++; }
		for (j=0;j<7;j++) { inv.hasSoft[j] = Utils.GetBoolFromString(entries[index],"hasSoft[" + j.ToString() + "]"); index++; }
		inv.emailCurrent = Utils.GetIntFromString(entries[index],"emailCurrent"); index++;
		inv.emailIndex = Utils.GetIntFromString(entries[index],"emailIndex"); index++;
		inv.hasNewNotes = Utils.GetBoolFromString(entries[index],"hasNewNotes"); index++;
		inv.hasNewData = Utils.GetBoolFromString(entries[index],"hasNewData"); index++;
		inv.hasNewLogs = Utils.GetBoolFromString(entries[index],"hasNewLogs"); index++;
		inv.hasNewEmail = Utils.GetBoolFromString(entries[index],"hasNewEmail"); index++;
		for (j=0;j<7;j++) {
			inv.hasMinigame[j] = Utils.GetBoolFromString(entries[index],"hasMinigame[" + j.ToString() + "]"); index++;
			inv.miniGameButton[j].SetActive(inv.hasMinigame[j]);
		}
		return index;
	}
}
