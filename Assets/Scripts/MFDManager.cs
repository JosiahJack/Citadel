using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Handles the HUD UI.
public class MFDManager : MonoBehaviour  {
	// External references, required
	public TabButtons leftTC;
	public TabButtons rightTC;

	// Center tabs
	public Button MainTabButton;
	public Button HardwareTabButton;
	public Button GeneralTabButton;
	public Button SoftwareTabButton;
	public Sprite MFDSprite;
	public Sprite MFDSpriteSelected;
	public Sprite MFDSpriteNotification;
	public AudioSource TabSFX;
	public AudioClip TabSFXClip;
	public GameObject MainTab;
	public GameObject HardwareTab;
	public GameObject GeneralTab;
	public GameObject SoftwareTab;
	public GameObject DataReaderContentTab;

	// Multi Media Tab
	public GameObject startingSubTab;
	public GameObject secondaryTab1;
	public GameObject secondaryTab2;
	public GameObject emailTab;
	public GameObject dataTab;
	public GameObject notesTab;
	public Text multiMediaHeaderLabel;
	public EReaderSectionsButtons ersbLH;
	public EReaderSectionsButtons ersbRH;
	[HideInInspector] public int lastMultiMediaTabOpened = 1; // save, 0 = email table, 1 = log table, 2 = data table

	// Main MFD
	public ItemTabManager itemTabLH;
	public ItemTabManager itemTabRH;
	public GameObject SearchFXRH;
	public GameObject SearchFXLH;
	public Transform playerCapsuleTransform;
	public WeaponMagazineCounter wepmagCounterLH;
	public WeaponMagazineCounter wepmagCounterRH;
	public GameObject logReaderContainer;
	public GameObject logTable;
	public GameObject logLevelsFolder;
	public WeaponButtonsManager wepbutMan;
	public Image iconLH;
	public Image iconRH;
	public Sprite[] wepIcons;
	public Text weptextLH;
	public Text weptextRH;
	public GameObject headerTextLH;
	public GameObject headerTextRH;
	public Text headerText_textLH;
	public Text headerText_textRH;
	public GameObject noItemsTextLH;
	public GameObject noItemsTextRH;
	public GameObject blockedBySecurityLH;
	public GameObject blockedBySecurityRH;
	public GameObject elevatorUIControlLH;
	public GameObject elevatorUIControlRH;
	public GameObject keycodeUIControlLH;
	public GameObject keycodeUIControlRH;
	public GameObject[] searchItemImagesLH;
	public GameObject[] searchItemImagesRH;
	public GameObject searchCloseButtonLH;
	public GameObject searchCloseButtonRH;
	public GameObject audioLogContainerLH;
	public GameObject audioLogContainerRH;
	public GameObject puzzleGridLH;
	public GameObject puzzleGridRH;
	public GameObject puzzleWireLH;
	public GameObject puzzleWireRH;
	public SearchButton searchContainerLH;
	public SearchButton searchContainerRH;
	public GameObject sysAnalyzerLH;
	public GameObject sysAnalyzerRH;
	public GameObject viewWeaponsContainer;
	public GameObject hardwareButtonsContainer;
	public HardwareButton hwb;
	public GameObject ctbButtonMain;
	public GameObject ctbButtonHardware;
	public GameObject ctbButtonGeneral;
	public GameObject tabButtonsLHButtons;
	public GameObject tabButtonsRHButtons;
	public GameObject energyTickPanel;
	public GameObject energyIndicator;
	public GameObject healthIndicator;
	public GameObject cyberHealthIndicator;
	public GameObject cyberTimerT;
	public GameObject cyberTimer;
	public GameObject cyberSprintContainer;
	public Text cyberSprintText;
	public GameObject biograph;

	// Externally modifiable.
	// Not intended to be set in inspector, some are not HideInInspector for
	// reference only.  Also, don't care about encapsulation - it works, it's
	// good.
	public bool lastWeaponSideRH;
	public bool lastItemSideRH;
	public bool lastAutomapSideRH;
	public bool lastTargetSideRH;
	public bool lastDataSideRH;
	public bool lastSearchSideRH;
	public bool lastLogSideRH;
	public bool lastLogSecondarySideRH;
	public bool lastMinigameSideRH;
	[HideInInspector] public float logFinished;
	[HideInInspector] public bool logActive;
	[HideInInspector] public AudioLogType logType;
	[HideInInspector] public Door linkedElevatorDoor;
	[HideInInspector] public Vector3 objectInUsePos;
	[HideInInspector] public PuzzleGridPuzzle tetheredPGP = null;
	[HideInInspector] public PuzzleWirePuzzle tetheredPWP = null;
	[HideInInspector] public SearchableItem tetheredSearchable = null;
	[HideInInspector] public KeypadElevator tetheredKeypadElevator = null;
	[HideInInspector] public KeypadKeycode tetheredKeypadKeycode = null;
	[HideInInspector] public bool paperLogInUse = false;
	[HideInInspector] public bool usingObject = false;
	[HideInInspector] public int applyButtonReferenceIndex = 0;
	public int curCenterTab = 0;
	public bool mouseClickHeldOverGUI;

	public GameObject overallLeftMFD;
	public GameObject overallRightMFD;
	public GameObject overallCenterMFD;
	public GameObject overallHardwareButtons;
	public GameObject overallHealthTickPanel;
	public GameObject overallEnergyTickPanel;
	public GameObject overallEnergyIndicator;
	public GameObject overallEnergyDrainText;
	public GameObject overallEnergyJPMText;
	public GameObject overallTextWarnings;
	public GameObject overallMissionTimerT;
	public GameObject overallMissionTimer;

	// Internal references
	private bool isRH;
	// private string[] wepText = new string[]{"MARK3 ASSAULT RIFLE",
		// "ER-90 BLASTER","SV-23 DARTGUN","AM-27 FLECHETTE","RW-45 ION BEAM",
		// "TS-04 LASER RAPIER","LEAD PIPE","MAGNUM 2100","SB-20 MAGPULSE",
		// "ML-41 PISTOL","LG-XX PLASMA RIFLE","MM-76 RAIL GUN",
		// "DC-05 RIOT GUN","RF-07 SKORPION","SPARQ BEAM","DH-07 STUNGUN"};
	private int wep16index = 0;
	[HideInInspector] public LogDataTabContainerManager logDataTabInfoLH;
	[HideInInspector] public LogDataTabContainerManager logDataTabInfoRH;

	// For health and energy ticks
	public Sprite[] tickImages;
	public Image tickImageHealth;
	public Image tickImageEnergy;
	private int tempSpriteIndex;
	private float lastEnergy;
	private float lastHealth;

	// For FPS counter
	public GameObject FPS;
	private float deltaTime = 0.0f;
	private float msecs;
	private float fps;
	private string textString;
	private Text text;
	private float tickFinished; // Visual only, Time.time controlled
	public float tickSecs = 0.1f;
	private int count;
	private float thousand = 1000f;
	private int zero = 0;
	private int one = 1;
	private string formatToDisplayMS;
	private string formatToDisplayFPS;
	public Text msText;
	public Text fpsText;
	public Text versionText;

	public AmmoIconManager ammoIconManLH;
	public AmmoIconManager ammoIconManRH;
	public GameObject ammoIndicatorHunsLH;
	public GameObject ammoIndicatorTensLH;
	public GameObject ammoIndicatorOnesLH;
	public GameObject overloadButtonLH;
	public GameObject unloadButtonLH;
	public GameObject loadNormalAmmoButtonLH;
	public Text loadNormalAmmoButtonTextLH;
	public GameObject loadAlternateAmmoButtonLH;
	public Text loadAlternateAmmoButtonTextLH;
	public GameObject energySliderLH;
	public GameObject energyHeatTicksLH;

	public GameObject ammoIndicatorHunsRH;
	public GameObject ammoIndicatorTensRH;
	public GameObject ammoIndicatorOnesRH;
	public GameObject overloadButtonRH;
	public GameObject unloadButtonRH;
	public GameObject loadNormalAmmoButtonRH;
	public Text loadNormalAmmoButtonTextRH;
	public GameObject loadAlternateAmmoButtonRH;
	public Text loadAlternateAmmoButtonTextRH;
	public GameObject energySliderRH;
	public GameObject energyHeatTicksRH;

	public Sprite ammoButtonHighlighted;
	public Sprite ammoButtonDeHighlighted;

	// Center Tabs
	private float centerTabsTickTime = 0.5f;
	private int numTicks = 15;
	private bool[] tabNotified;
	private float centerTabsTickFinished; // Visual only, Time.time controlled
	private bool[] highlightStatus;
	private int[] highlightTickCount;
	private float blinkFinished;
	private float blinkTick = 1f;
	private float beepFinished;
	private float beepTick = 3f;
	private int beepCount = 0;

	// Singleton instance
	public static MFDManager a;

	public void Start () {
		a = this;
		a.logFinished = PauseScript.a.relativeTime;
		a.logActive = false;
		a.TabReset(true);
		a.TabReset(false);
		a.DrawTicks(true); // Health
		a.DrawTicks(false); // Energy
		a.text = GetComponent<Text> ();
		a.deltaTime = Time.time;
		a.count = 0;
		a.tickFinished = a.centerTabsTickFinished = Time.time + a.tickSecs
						 + UnityEngine.Random.value;
		a.formatToDisplayMS = "{0:0.0}";
		a.formatToDisplayFPS = "{0:0.0}";
		a.versionText.text = Const.a.versionString; // CITADEL PROJECT VERSION

		// Center tabs
		a.tabNotified = new bool[] {false, false, false, false};
		a.tabNotified = new bool[] {false, false, false, false};
		a.highlightStatus = new bool[] {false, false, false, false};
		a.highlightTickCount = new int[] {0,0,0,0};
		a.blinkFinished = blinkTick + PauseScript.a.relativeTime;
		a.beepFinished = beepTick + PauseScript.a.relativeTime;
		MainTabButton.image.overrideSprite = MFDSpriteSelected;
		DisableAllCenterTabs();
		HardwareTabButton.image.overrideSprite = MFDSprite;
		GeneralTabButton.image.overrideSprite = MFDSprite;
		SoftwareTabButton.image.overrideSprite = MFDSprite;
		curCenterTab = 0;
		a.logDataTabInfoLH = 
			audioLogContainerLH.GetComponent<LogDataTabContainerManager>();
		a.logDataTabInfoRH = 
			audioLogContainerRH.GetComponent<LogDataTabContainerManager>();
	}

	void WeaponCycleUp() {
		if (MouseLookScript.a.inCyberSpace) {
			// There's only two cyberspace weapons, up is down.
			Inventory.a.isPulserNotDrill = !Inventory.a.isPulserNotDrill;

			Utils.PlayOneShotSavable(Inventory.a.SFX,
									 Inventory.a.SFXChangeWeapon);

			if (Inventory.a.isPulserNotDrill) {
				Inventory.a.pulserButtonText.Select(true);
				Inventory.a.drillButtonText.Select(false);
			} else {
				Inventory.a.pulserButtonText.Select(false);
				Inventory.a.drillButtonText.Select(true);
			}
		} else {
			if (Const.a.InputInvertInventoryCycling) wepbutMan.WeaponCycleDown();
			else wepbutMan.WeaponCycleUp();
		}
	}

	void WeaponCycleDown() {
		if (MouseLookScript.a.inCyberSpace) {
			// There's only two cyberspace weapons, up is down.
			Inventory.a.isPulserNotDrill = !Inventory.a.isPulserNotDrill;
			Utils.PlayOneShotSavable(Inventory.a.SFX,
									 Inventory.a.SFXChangeWeapon);

			if (Inventory.a.isPulserNotDrill) {
				Inventory.a.pulserButtonText.Select(true);
				Inventory.a.drillButtonText.Select(false);
			} else {
				Inventory.a.pulserButtonText.Select(false);
				Inventory.a.drillButtonText.Select(true);
			}
		} else {
			if (Const.a.InputInvertInventoryCycling) wepbutMan.WeaponCycleUp();
			else wepbutMan.WeaponCycleDown();
		}
	}

	void Update() {
		// Actions during Pause and Unpause (always)
		if (FPS.activeInHierarchy) {
			count++;
			deltaTime += Time.unscaledDeltaTime;
			if (tickFinished < Time.time) {
				msecs = deltaTime/count*thousand;
				deltaTime = zero;
				fps = count * (one / tickSecs);
				count = zero;
				msText.text = string.Format (formatToDisplayMS, msecs);
				fpsText.text = string.Format (formatToDisplayFPS, fps);
				tickFinished = Time.time + tickSecs;
			}
		}

		// Unpaused Actions
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
			mouseClickHeldOverGUI = false;
		}

		HardwareButtonsUpdate();
		LogReaderUpdate();
		CenterTabBlink();
		if (lastEnergy != PlayerEnergy.a.energy) DrawTicks(false);
		lastEnergy = PlayerEnergy.a.energy;
		if (lastHealth != PlayerHealth.a.hm.health) DrawTicks(true);
		lastHealth = PlayerHealth.a.hm.health;
		WeaponButtonsManagerUpdate();
		UpdateAmmoAndLoadButtons();
		switch (WeaponCurrent.a.weaponCurrent) {
			case 37: ShowEnergyItems(); break;
			case 40: ShowEnergyItems(); break;
			case 46: ShowEnergyItems(); break;
			case 50: ShowEnergyItems(); break;
			case 51: ShowEnergyItems(); break;
		}
		if (GetInput.a.WeaponCycUp()) WeaponCycleUp();
		if (GetInput.a.WeaponCycDown()) WeaponCycleDown();
		if (Input.GetKeyDown(KeyCode.F1)) leftTC.TabButtonAction(0);   // Weapon
		if (Input.GetKeyDown(KeyCode.F2)) leftTC.TabButtonAction(1);   // Item
		if (Input.GetKeyDown(KeyCode.F3)) leftTC.TabButtonAction(2);   // Automap
		// Target tab removed as unnecessary for Citadel.              // Target
		if (Input.GetKeyDown(KeyCode.F4)) leftTC.TabButtonAction(4);   // Data

		if (Input.GetKeyDown(KeyCode.F5)) rightTC.TabButtonAction(0);  // Weapon
		if (Input.GetKeyDown(KeyCode.F7)) rightTC.TabButtonAction(1);  // Item
		if (Input.GetKeyDown(KeyCode.F8)) rightTC.TabButtonAction(2);  // Automap
		// Target tab removed as unnecessary for Citadel.              // Target
		if (Input.GetKeyDown(KeyCode.F10)) rightTC.TabButtonAction(4); // Data

		if (Input.GetKeyDown(KeyCode.PageUp)) {
			switch(curCenterTab) {
				case 0: CenterTabButtonAction(3); break;
				case 1: CenterTabButtonAction(0); break;
				case 2: CenterTabButtonAction(1); break;
				case 3: CenterTabButtonAction(2); break;
			}
		}
		if (Input.GetKeyDown(KeyCode.PageDown)) {
			switch(curCenterTab) {
				case 0: CenterTabButtonAction(1); break;
				case 1: CenterTabButtonAction(2); break;
				case 2: CenterTabButtonAction(3); break;
				case 3: CenterTabButtonAction(0); break;
			}
		}

		// Handle severing connection with in use keypads, puzzles, etc. when player drifts too far away
		if (usingObject) {
			if (Vector3.Distance(playerCapsuleTransform.position, objectInUsePos) > (Const.a.frobDistance + 0.16f)) {
				if (tetheredPGP != null) {
					ClosePuzzleGrid();
					tetheredPGP = null;
				}

				if (tetheredPWP != null) {
					ClosePuzzleWire();
					tetheredPWP = null;
				}

				if (tetheredKeypadElevator != null) {
					CloseElevatorPad();
					tetheredKeypadElevator = null;
				}

				if (tetheredKeypadKeycode != null) {
					CloseKeycodePad();
					tetheredKeypadKeycode = null;
				}

				if (tetheredSearchable != null) {
					CloseSearch();
					tetheredSearchable = null;
				}

				if (paperLogInUse) {
					ClosePaperLog();
					paperLogInUse = false;
				}
			}
		}

		// Update the weapon icon
		wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(WeaponCurrent.a.weaponIndex);
		if (wep16index >=0 && wep16index < 16) {
			if (leftTC.TabManager.WeaponTab.activeInHierarchy) {
				iconLH.overrideSprite = wepIcons[wep16index];
				if (Inventory.a.numweapons <= 0
					|| WeaponCurrent.a.weaponCurrentPending >= 0) {
					Utils.DisableImage(iconLH);
				} else {
					Utils.EnableImage(iconLH);
				}
			}

			if (rightTC.TabManager.WeaponTab.activeInHierarchy) {
				iconRH.overrideSprite = wepIcons[wep16index];
				if (Inventory.a.numweapons <= 0
					|| WeaponCurrent.a.weaponCurrentPending >= 0) {
					Utils.DisableImage(iconRH);
				} else {
					Utils.EnableImage(iconRH);
				}
			}
		}
	}

	void LateUpdate() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		// Neither LMB nor RMB is being held, reset this to false.
		if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
			mouseClickHeldOverGUI = false;
		}
	}

	void HardwareButtonsUpdate() {
		if (!hardwareButtonsContainer.activeInHierarchy) return; // In cyber space.

		hwb.ListenForHardwareHotkeys();

		// Check for and make the eReader button blink
		if (hwb.buttons[5].gameObject.activeSelf) {
			bool foundsome = false;
			for (int i=0;i<hwb.ecbm.mmLBs.Length;i++) {
				if (Inventory.a.hasLog[hwb.ecbm.mmLBs[i].logReferenceIndex] && !Inventory.a.readLog[hwb.ecbm.mmLBs[i].logReferenceIndex]) foundsome = true;
			}

			if (foundsome) {
				// You've got mail!
				if (blinkFinished < PauseScript.a.relativeTime) {
					blinkFinished = blinkTick + PauseScript.a.relativeTime;
					Inventory.a.hardwareIsActive[2] = !Inventory.a.hardwareIsActive[2];
					if (Inventory.a.hardwareIsActive[2]) {
						hwb.buttons[5].image.overrideSprite = hwb.buttonActive1[5];
					} else {
						hwb.buttons[5].image.overrideSprite = hwb.buttonDeactive[5];
					}
				}
				if (beepFinished < PauseScript.a.relativeTime && Inventory.a.beepDone) {
					beepFinished = beepTick + PauseScript.a.relativeTime;
					beepCount++;
					if (beepCount >= 3) { Inventory.a.beepDone = false; beepCount = 0; } // Reset beeping, notification done.
					Utils.PlayOneShotSavable(hwb.SFX,hwb.beepSFX); // GO active handled by guard clause.
				}
			} else {
				hwb.buttons[5].image.overrideSprite = hwb.buttonDeactive[5];
			}
		}
	}

	void LogReaderUpdate() {
		if (!logActive) return;
		if (logFinished >= PauseScript.a.relativeTime) return;
		if (logType == AudioLogType.Papers) return;
		if (logType == AudioLogType.TextOnly) return;

		logActive = false;
		if (itemTabLH.eReaderSectionsContainer.activeInHierarchy) {
			ReturnToLastTab(true);
		}

		if (itemTabRH.eReaderSectionsContainer.activeInHierarchy) {
			ReturnToLastTab(false);
		}

		if (DataReaderContentTab.activeInHierarchy) {
			CenterTabButtonClickSilent(0,true);
		}
	}

	void CenterTabBlink() {
		if (centerTabsTickFinished >= Time.time) return;

		for (int i=0;i<4;i++) {
			if (tabNotified[i]) ToggleHighlightOnCenterTabButton(i);
		}
		centerTabsTickFinished = Time.time + centerTabsTickTime;
	}

	// Called by Automap.cs.  This handles the UI changes to make room.
	public void AutomapGoFull() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		ctbButtonMain.SetActive(false);
		ctbButtonHardware.SetActive(false);
		ctbButtonGeneral.SetActive(false);
		DisableAllCenterTabs();
		TabReset(true); // right
		TabReset(false); // left
		tabButtonsLHButtons.SetActive(false);
		tabButtonsRHButtons.SetActive(false);
	}

	// Handles returning UI back to how it was before clearing the board.
	public void CloseFullmap() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		ctbButtonMain.SetActive(true);
		ctbButtonHardware.SetActive(true);
		ctbButtonGeneral.SetActive(true);
		CenterTabButtonClickSilent(curCenterTab,true);
		tabButtonsLHButtons.SetActive(true);
		tabButtonsRHButtons.SetActive(true);
		TabReset(true); // right
		TabReset(false); // left
		ReturnToLastTab(true);
		ReturnToLastTab(false);
	}

	// Called by MouseLookScript.cs
	public void EnterCyberspace() {
		TabReset(true); // right
		TabReset(false); // left
		rightTC.TurnAllTabsOff();
		leftTC.TurnAllTabsOff();
		ctbButtonMain.SetActive(false);
		ctbButtonHardware.SetActive(false);
		ctbButtonGeneral.SetActive(false);
		tabButtonsLHButtons.SetActive(false);
		tabButtonsRHButtons.SetActive(false);
		energyTickPanel.SetActive(false);
		energyIndicator.SetActive(false);
		healthIndicator.SetActive(false);
		if (!Const.a.noHUD) cyberHealthIndicator.SetActive(true);
		if (!Const.a.noHUD) cyberTimerT.SetActive(true);
		if (!Const.a.noHUD) cyberTimer.SetActive(true);
		hardwareButtonsContainer.SetActive(false);
		viewWeaponsContainer.SetActive(false);
		CyberTimer ct = cyberTimer.GetComponent<CyberTimer>();
		if (ct != null) ct.Reset(Const.a.difficultyCyber);
		CenterTabButtonClickSilent(3,true);
	}

	// Called by MouseLookScript
	public void ExitCyberspace() {
		TabReset(true);
		TabReset(false);
		ReturnToLastTab(true);
		ReturnToLastTab(false);
		ctbButtonMain.SetActive(true);
		ctbButtonHardware.SetActive(true);
		ctbButtonGeneral.SetActive(true);
		tabButtonsLHButtons.SetActive(true);
		tabButtonsRHButtons.SetActive(true);
		if (!Const.a.noHUD) {
			energyTickPanel.SetActive(true);
			energyIndicator.SetActive(true);
			healthIndicator.SetActive(true);
			hardwareButtonsContainer.SetActive(true);
		}
		cyberHealthIndicator.SetActive(false);
		cyberSprintContainer.SetActive(false);
		cyberTimerT.SetActive(false);
		cyberTimer.SetActive(false);
		viewWeaponsContainer.SetActive(true);
		CenterTabButtonClickSilent(0,true);
	}

	public void CyberSprint (string message) {
		cyberSprintContainer.SetActive(true);
		cyberSprintText.text = message;
	}

	public void TabReset(bool isRH) {
		if (isRH) {
			headerTextRH.SetActive(false);
			headerText_textRH.text = System.String.Empty;
			noItemsTextRH.SetActive(false);
			blockedBySecurityRH.SetActive(false);
			elevatorUIControlRH.SetActive(false);
			keycodeUIControlRH.SetActive(false);
			puzzleGridRH.SetActive(false);
			puzzleWireRH.SetActive(false);
			audioLogContainerRH.SetActive(false);
			sysAnalyzerRH.SetActive(false);
			searchCloseButtonRH.SetActive(false);
			for (int i=0; i<=3;i++) {
				searchItemImagesRH[i].SetActive(false);
			}
		} else {
			headerTextLH.SetActive(false);
			headerText_textLH.text = System.String.Empty;
			noItemsTextLH.SetActive(false);
			blockedBySecurityLH.SetActive(false);
			elevatorUIControlLH.SetActive(false);
			keycodeUIControlLH.SetActive(false);
			puzzleGridLH.SetActive(false);
			puzzleWireLH.SetActive(false);
			audioLogContainerLH.SetActive(false);
			sysAnalyzerLH.SetActive(false);
			searchCloseButtonLH.SetActive(false);
			for (int i=0; i<=3;i++) {
				searchItemImagesLH[i].SetActive(false);
			}
		}
	}

	public void RevertDataTabState() {
		TabReset(true);
		TabReset(false);
		usingObject = false;
		logTable.SetActive(false);
		logLevelsFolder.SetActive(false);
		logReaderContainer.SetActive(false);
		ReturnToLastTab(true);
		ReturnToLastTab(false);
	}

	public void ClosePuzzleGrid() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		PuzzleGrid pg = puzzleGridLH.GetComponent<PuzzleGrid>();
		PuzzleGrid pgr = puzzleGridRH.GetComponent<PuzzleGrid>();
		tetheredPGP.SendDataBackToPanel(pg);
		pg.Reset();
		pgr.Reset();
		tetheredPGP = null;
		RevertDataTabState();
	}

	public void ClosePuzzleWire() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		PuzzleWire pw = puzzleWireLH.GetComponent<PuzzleWire>();
		PuzzleWire pwr = puzzleWireRH.GetComponent<PuzzleWire>();
		tetheredPWP.SendDataBackToPanel(pw,false);
		pw.Reset();
		pwr.Reset();
		tetheredPWP = null;
		RevertDataTabState();
	}

	public void CloseElevatorPad() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		tetheredKeypadElevator.SendDataBackToPanel();
		TurnOffElevatorPad();
		tetheredKeypadElevator = null;
		linkedElevatorDoor = null;
		RevertDataTabState();
	}

	public void CloseKeycodePad() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		TurnOffKeypad();
		tetheredKeypadKeycode = null;
		RevertDataTabState();
	}

	public void CloseSearch() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		GUIState.a.ClearOverButton();
		if (tetheredSearchable != null) tetheredSearchable.ResetSearchable(false);
		tetheredSearchable = null;
		searchCloseButtonLH.SetActive(false);
		searchCloseButtonRH.SetActive(false);
		if (leftTC.TabManager.DataTab.activeSelf
			&& searchContainerLH.gameObject.activeSelf) {
			TabReset(false);
			logTable.SetActive(false);
			logLevelsFolder.SetActive(false);
			logReaderContainer.SetActive(false);
			ReturnToLastTab(false);
		}

		if (rightTC.TabManager.DataTab.activeSelf
			&& searchContainerRH.gameObject.activeSelf) {
			TabReset(true);
			ReturnToLastTab(true);
		}
		usingObject = false;
	}

	public void ClosePaperLog() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		CenterTabButtonClickSilent(curCenterTab,false);
	}

	public void DrawTicks(bool health) {
		tempSpriteIndex = -1;
		float checkVal = 0;
		if (health) {
			if (MouseLookScript.a.inCyberSpace) {
				checkVal = PlayerHealth.a.hm.cyberHealth;
			} else {
				checkVal = PlayerHealth.a.hm.health;
			}
		} else {
			checkVal = PlayerEnergy.a.energy;
		}

		// Always display ticks properly no matter what crazy value  we've been
		// hacked to have.
		if (checkVal > 255f) checkVal = 255f; 
		for (int i=1;i<24;i++) {
			if (checkVal < (11f * i)) tempSpriteIndex++;
		}

		tempSpriteIndex++;
		if (tempSpriteIndex >= 0 && tempSpriteIndex < 25) {
			if (health) {
				tickImageHealth.overrideSprite = tickImages[tempSpriteIndex];
			} else {
				tickImageEnergy.overrideSprite = tickImages[tempSpriteIndex];
			}
		} else {
			if (health) {
				tickImageHealth.overrideSprite = tickImages[24];
			} else {
				tickImageEnergy.overrideSprite = tickImages[24];
			}
		}
	}

	void WeaponButtonsManagerUpdate() {
		for (int i=0; i<7; i++) {
			WeaponButton wepbut = wepbutMan.wepButtonsScripts[i];
			GameObject buttonGO = wepbut.gameObject;
			if (Inventory.a.weaponInventoryIndices[i] > 0) {
				if (!buttonGO.activeInHierarchy) buttonGO.SetActive(true);
				wepbut.useableItemIndex = Inventory.a.weaponInventoryIndices[i];
				if (!wepbutMan.wepCountsText[i].activeInHierarchy) {
					wepbutMan.wepCountsText[i].SetActive(true);
				}
			} else {
				if (buttonGO.activeInHierarchy) buttonGO.SetActive(false);
				wepbut.useableItemIndex = -1;
				if (wepbutMan.wepCountsText[i].activeInHierarchy) {
					wepbutMan.wepCountsText[i].SetActive(false);
				}
			}
		}
	}

	public void OpenTab(int index, bool overrideToggling,TabMSG type,int intdata1, Handedness side) {
		if (side == Handedness.LH) {
			isRH = false;
		} else {
			isRH = true;
		}
		//switch (index) {
		//	case 0: isRH = lastWeaponSideRH; break;
		//	case 1: isRH = lastItemSideRH; break;
		//	case 2: isRH = lastAutomapSideRH; break;
		//	case 3: isRH = lastTargetSideRH; break;
		//	case 4: isRH = lastDataSideRH; break;
		//}
		if(!isRH) {
			// LH LEFT HAND MFD
			leftTC.TabButtonClickSilent(index,overrideToggling);
			if (type == TabMSG.Weapon) {
				TabReset(false);
			}

			if (type == TabMSG.AudioLog) {
				TabReset(false);
				audioLogContainerLH.SetActive(true);
			}

			if (type == TabMSG.Keypad) {
				TabReset(false);
				keycodeUIControlLH.SetActive(true);
				MouseLookScript.a.ForceInventoryMode();
			}

			if (type == TabMSG.Elevator) {
				TabReset(false);
				elevatorUIControlLH.SetActive(true);
				MouseLookScript.a.ForceInventoryMode();
			}

			if (type == TabMSG.GridPuzzle) {
				TabReset(false);
				puzzleGridLH.SetActive(true);
				MouseLookScript.a.ForceInventoryMode();
			}

			if (type == TabMSG.WirePuzzle) {
				TabReset(false);
				puzzleWireLH.SetActive(true);
				MouseLookScript.a.ForceInventoryMode();
			}

			if (type == TabMSG.EReader) {
				TabReset(false);
				itemTabLH.EReaderSectionSContainerOpen();
				MouseLookScript.a.ForceInventoryMode();
			}
			if (type == TabMSG.SystemAnalyzer) {
				TabReset(false);
				sysAnalyzerLH.SetActive(true);
			}
		} else {
			// RH RIGHT HAND MFD
			rightTC.TabButtonClickSilent(index,overrideToggling);
			if (type == TabMSG.AudioLog) {
				TabReset(true);
				audioLogContainerRH.SetActive(true);
			}

			if (type == TabMSG.Keypad) {
				TabReset(true);
				keycodeUIControlRH.SetActive(true);
			}

			if (type == TabMSG.Elevator) {
				TabReset(true);
				elevatorUIControlRH.SetActive(true);
			}

			if (type == TabMSG.GridPuzzle) {
				TabReset(true);
				puzzleGridRH.SetActive(true);
			}

			if (type == TabMSG.WirePuzzle) {
				TabReset(true);
				puzzleWireRH.SetActive(true);
				MouseLookScript.a.ForceInventoryMode();
			}

			if (type == TabMSG.EReader) {
				TabReset(true);
				itemTabRH.EReaderSectionSContainerOpen();
				MouseLookScript.a.ForceInventoryMode();
			}
			if (type == TabMSG.SystemAnalyzer) {
				TabReset(true);
				sysAnalyzerRH.SetActive(true);
			}
		}
	}

	public void ResetItemTab() {
		itemTabLH.Reset();
		itemTabRH.Reset();
	}

	public void SendInfoToItemTab(int index, int customIndex) {
		if (index < 0 || index > 110) { ResetItemTab(); return; }

		itemTabLH.SendItemDataToItemTab(index,customIndex);
		itemTabRH.SendItemDataToItemTab(index,customIndex);
	}

	// Clicking [Apply] button on left or right MFD's Item Tab to apply current patch or general inventory item.
	public void ApplyButtonClicked() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		itemTabLH.applyButton.SetActive(false);
		itemTabRH.applyButton.SetActive(false);
		if (applyButtonReferenceIndex < 0) return;

		if (applyButtonReferenceIndex == 55 // Health kit was applied
			|| applyButtonReferenceIndex == 52
			|| applyButtonReferenceIndex == 53) {
			// General Inventory
			// ----------------------------------------------------------------
			GameObject invbtn = 
				Inventory.a.genButtons[Inventory.a.generalInvCurrent];

			if (invbtn != null) {
				invbtn.GetComponent<GeneralInvButton>().DoubleClick();
			}

			int nextIndex = Inventory.a.generalInvIndex - 1;
			if (nextIndex < 0) nextIndex = 0;
			Inventory.a.generalInvIndex = nextIndex;

			// Set item tab to next general inv current.
			SendInfoToItemTab(Inventory.a.generalInvIndex,-1);
		} else {
			// Patches
			// ----------------------------------------------------------------
			Inventory.a.patchButtonScripts[Inventory.a.patchCurrent].DoubleClick();

			// Set item tab to next patch.
			SendInfoToItemTab(Inventory.a.patchIndex,-1);
		}
	}

	public void Search(bool isRH, string head, int numberFoundContents, int[] contents, int[] customIndex) {
		if (isRH) {
			headerTextRH.SetActive(true);
			headerText_textRH.enabled = true;
			headerText_textRH.text = head;
			if (numberFoundContents <= 0) {
				noItemsTextRH.SetActive(true);
				noItemsTextRH.GetComponent<Text>().enabled = true;
				return;
			}
			for (int i=0;i<4;i++) {
				if (contents[i] > -1) {
					searchCloseButtonRH.SetActive(true);
					searchItemImagesRH[i].SetActive(true);
					searchItemImagesRH[i].GetComponent<Image>().overrideSprite = Const.a.searchItemIconSprites[contents[i]];
					searchContainerRH.contents[i] = contents[i];
					searchContainerRH.customIndex[i] = customIndex[i];
				}
			}
			searchCloseButtonRH.SetActive(true);
		} else {
			headerTextLH.SetActive(true);
			headerText_textLH.enabled = true;
			headerText_textLH.text = head;
			if (numberFoundContents <= 0) {
				noItemsTextLH.SetActive(true);
				noItemsTextLH.GetComponent<Text>().enabled = true;
				return;
			}
			for (int i=0;i<4;i++) {
				if (contents[i] > -1) {
					searchCloseButtonLH.SetActive(true);
					searchItemImagesLH[i].SetActive(true);
					searchItemImagesLH[i].GetComponent<Image>().overrideSprite = Const.a.searchItemIconSprites[contents[i]];
					searchContainerLH.contents[i] = contents[i];
					searchContainerLH.customIndex[i] = customIndex[i];
				}
			}
			searchCloseButtonLH.SetActive(true);
		}
	}

	public void SendSearchToDataTab (string name, int contentCount, int[] resultContents, int[] resultsIndices, Vector3 searchPosition, SearchableItem si, bool useFX) {
		TabReset(true);
		TabReset(false);
		Search(true,name,contentCount,resultContents,resultsIndices);
		Search(false,name,contentCount,resultContents,resultsIndices);

		// Enable search box scaling effect
		if (lastSearchSideRH) {
			OpenTab(4,true,TabMSG.Search,0,Handedness.RH);
			if (useFX) SearchFXRH.SetActive(true);
		} else {
			OpenTab(4,true,TabMSG.Search,0,Handedness.LH);
			if (useFX) SearchFXLH.SetActive(true);
		}
		if (tetheredSearchable != si) {
			if (tetheredSearchable != null) {
				tetheredSearchable.ResetSearchable(false);
				tetheredSearchable = null;
			}
		}
		tetheredSearchable = si;
		objectInUsePos = searchPosition;
		usingObject = true;
	}

	public void SendGridPuzzleToDataTab (bool[] states, PuzzleCellType[] types,
										 PuzzleGridType gtype, int start,
										 int end, int width, int height,
										 HUDColor colors, string t1, 
										 UseData ud, Vector3 tetherPoint,
										 PuzzleGridPuzzle pgp) {
		if (lastDataSideRH) {
			// Send to RH tab
			TabReset(true);
			puzzleGridRH.GetComponent<PuzzleGrid>().SendGrid(states,types,
															 gtype,start,end,
															 width,height,
															 colors,t1,ud,pgp);
			OpenTab(4,true,TabMSG.GridPuzzle,0,Handedness.RH);
			SearchFXRH.SetActive(true);
		} else {
			// Send to LH tab
			TabReset(false);
			puzzleGridLH.GetComponent<PuzzleGrid>().SendGrid(states,types,
															 gtype,start,end,
															 width,height,
															 colors,t1,ud,pgp);
			OpenTab(4,true,TabMSG.GridPuzzle,0,Handedness.LH);
			SearchFXLH.SetActive(true);
		}
		objectInUsePos = tetherPoint;
		tetheredPGP = pgp;
		usingObject = true;
	}

	public void SendWirePuzzleToDataTab(bool[] sentWiresOn, bool[] sentNodeRowsActive, int[] sentCurrentPositionsLeft, int[] sentCurrentPositionsRight, int[] sentTargetsLeft, int[] sentTargetsRight, HUDColor theme, HUDColor[] wireColors, string t1, string a1, UseData udSent,Vector3 tetherPoint, PuzzleWirePuzzle pwp) {
		TabReset(lastDataSideRH);
		if (lastDataSideRH) {
			// Send to RH tab
			puzzleWireRH.GetComponent<PuzzleWire>().SendWirePuzzleData(sentWiresOn,sentNodeRowsActive,sentCurrentPositionsLeft,sentCurrentPositionsRight,sentTargetsLeft,sentTargetsRight,theme,wireColors,t1,a1,udSent,pwp);
			OpenTab(4,true,TabMSG.WirePuzzle,0,Handedness.RH);
			SearchFXRH.SetActive(true);
		} else {
			// Send to LH tab
			puzzleWireLH.GetComponent<PuzzleWire>().SendWirePuzzleData(sentWiresOn,sentNodeRowsActive,sentCurrentPositionsLeft,sentCurrentPositionsRight,sentTargetsLeft,sentTargetsRight,theme,wireColors,t1,a1,udSent,pwp);
			OpenTab(4,true,TabMSG.WirePuzzle,0,Handedness.LH);
			SearchFXLH.SetActive(true);
		}
		objectInUsePos = tetherPoint;
		tetheredPWP = pwp;
		usingObject = true;
	}

	public void SendPaperLogToDataTab(int index,Vector3 tetherPoint) {
		if (Const.a.audioLogImagesRefIndicesLH[index] != 0) { // LH, but only
															  // if has image.
			TabReset(false);
			OpenTab(4,true,TabMSG.AudioLog,index,Handedness.LH);
		}
		if (Const.a.audioLogImagesRefIndicesRH[index] != 0) { // RH, but only
															  // if has image.
			TabReset(true);
			OpenTab(4,true,TabMSG.AudioLog,index,Handedness.RH);
		}
		
		logDataTabInfoLH.SendLogData(index,false); // false for LH
		logDataTabInfoRH.SendLogData(index,true);  // true for RH
		objectInUsePos = tetherPoint;
		paperLogInUse = true;
		usingObject = true;
		OpenLogTextReader();
		DataReaderContentTab.SetActive(true);
		logReaderContainer.SetActive(true);
		logReaderContainer.GetComponent<LogTextReaderManager>().SendTextToReader(index);
		logTable.SetActive(false);
		logLevelsFolder.SetActive(false);
	}

	public void SendAudioLogToDataTab(int index) {
		TabReset(false);
		OpenTab(4,true,TabMSG.AudioLog,index,Handedness.LH);  // LH
		if (Const.a.audioLogImagesRefIndicesRH[index] != 0) { // RH, but only
															  // if has image.
			Debug.Log("Activating 2nd image for logs");
			TabReset(true);
			OpenTab(4,true,TabMSG.AudioLog,index,Handedness.RH);
		}

		logDataTabInfoLH.SendLogData(index,false); // false for LH
		logDataTabInfoRH.SendLogData(index,true);  // true for RH
		CenterTabButtonClickSilent(4,true);
		if (tetheredSearchable != null) tetheredSearchable.searchableInUse = false;
		OpenLogTextReader();
		DataReaderContentTab.SetActive(true);
		logReaderContainer.SetActive(true);
		logReaderContainer.GetComponent<LogTextReaderManager>().SendTextToReader(index);
		logTable.SetActive(false);
		logLevelsFolder.SetActive(false);
		if (Const.a.audioLogs[index] != null) logFinished = PauseScript.a.relativeTime + Const.a.audioLogs[index].length + 0.1f; //add slight delay after log is finished playing to make sure we don't cut off audio in case there's a frame delay for audio start
		logActive = true;
		logType = Const.a.audioLogType[index];
	}

	public void OpenLastItemSide() {
		if (lastItemSideRH) {
			OpenTab(1,true,TabMSG.EReader,-1,Handedness.RH);
		} else {
			OpenTab(1,true,TabMSG.EReader,-1,Handedness.LH);
		}
	}

	public void OpenEReaderInItemsTab() {
		OpenTab(1,true,TabMSG.EReader,-1,Handedness.LH);
		CenterTabButtonClickSilent(4,false);
		if (tetheredSearchable != null) tetheredSearchable.searchableInUse = false;
		logTable.SetActive(false);
		logLevelsFolder.SetActive(false);
		logReaderContainer.SetActive(false);
		OpenLastMultiMediaTab();
	}

	public void ClearDataTab(bool isRH) {
		TabReset(isRH);
	}

	public void TurnOffKeypad() {
		if (lastDataSideRH) {
			keycodeUIControlRH.SetActive(false);
		} else {
			keycodeUIControlLH.SetActive(false);
		}
	}

	public void TurnOffElevatorPad() {
		if (lastDataSideRH) {
			elevatorUIControlRH.SetActive(false);
		} else {
			elevatorUIControlLH.SetActive(false);
		}
	}

	public bool GetElevatorControlActiveState() {
		if (lastDataSideRH) {
			return elevatorUIControlRH.activeInHierarchy;
		} else {
			return elevatorUIControlLH.activeInHierarchy;
		}
	}

	public void BlockedBySecurity(Vector3 tetherPoint, UseData ud) {
		TabReset(lastDataSideRH);
		if (lastDataSideRH) {
			OpenTab(4,true,TabMSG.None,0,Handedness.RH);
			blockedBySecurityRH.SetActive(true);
		} else {
			OpenTab(4,true,TabMSG.None,0,Handedness.LH);
			blockedBySecurityLH.SetActive(true);
		}
		Const.sprint(Const.a.stringTable[25],ud.owner);
		objectInUsePos = tetherPoint;
		usingObject = true;
	}

	public void SendKeypadKeycodeToDataTab(int keycode, Vector3 tetherPoint,
										   KeypadKeycode keypad,
										   bool alreadySolved) {
		if (keycode < 0 || keypad == null) {
			KeypadKeycodeButtons kkbRH =
				keycodeUIControlRH.GetComponent<KeypadKeycodeButtons>();

			kkbRH.keycode = 0;
			kkbRH.keypad = null;
			kkbRH.ResetEntry();
			kkbRH.currentEntry = 0;


			KeypadKeycodeButtons kkbLH =
				keycodeUIControlLH.GetComponent<KeypadKeycodeButtons>();

			kkbLH.keycode = 0;
			kkbLH.keypad = null;
			kkbLH.ResetEntry();
			kkbLH.currentEntry = 0;
			return;
		}

		TabReset(lastDataSideRH);

		if (lastDataSideRH) {
			OpenTab(4,true,TabMSG.Keypad,0,Handedness.RH);
			keycodeUIControlRH.SetActive(true);
			KeypadKeycodeButtons kkb =
				keycodeUIControlRH.GetComponent<KeypadKeycodeButtons>();
			kkb.keycode = keycode;
			kkb.keypad = keypad;
			kkb.ResetEntry();
			if (Const.a.difficultyMission <= 1) {
				kkb.currentEntry = keycode;
			}
		} else {
			OpenTab(4,true,TabMSG.Keypad,0,Handedness.LH);
			keycodeUIControlLH.SetActive(true);
			KeypadKeycodeButtons kkb =
					keycodeUIControlLH.GetComponent<KeypadKeycodeButtons>();
			kkb.keycode = keycode;
			kkb.keypad = keypad;
			kkb.ResetEntry();
			if (Const.a.difficultyMission <= 1) {
				kkb.currentEntry = keycode;
			}
		}

		objectInUsePos = tetherPoint;
		tetheredKeypadKeycode = keypad;
		usingObject = true;
	}

	public void SendElevatorKeypadToDataTab(KeypadElevator ke, bool[] buttonsEnabled, bool[] buttonsDarkened, string[] buttonText,GameObject[] targetDestination,Vector3 tetherPoint,Door linkedDoor,int currentFloor) {
		TabReset(lastDataSideRH);
		ElevatorKeypad elevatorKeypad;
		if (lastDataSideRH) {
			elevatorKeypad = elevatorUIControlRH.GetComponent<ElevatorKeypad>();
		} else {
			elevatorKeypad = elevatorUIControlLH.GetComponent<ElevatorKeypad>();
		}
		for (int i=0;i<8;i++) {
			elevatorKeypad.buttonsEnabled[i] = buttonsEnabled[i];
			elevatorKeypad.buttonsDarkened[i] = buttonsDarkened[i];
			elevatorKeypad.buttonText[i] = buttonText[i];
			elevatorKeypad.targetDestination[i] = targetDestination[i];

			if (elevatorKeypad.buttonsEnabled[i]) {
				if (!elevatorKeypad.buttons[i].activeSelf) elevatorKeypad.buttons[i].SetActive(true);
				elevatorKeypad.buttonTextHolders[i].text = elevatorKeypad.buttonText[i];

				if (elevatorKeypad.buttonsDarkened[i]) {
					elevatorKeypad.buttonSprites[i].overrideSprite = elevatorKeypad.buttonDarkened;
					elevatorKeypad.buttonTextHolders[i].color = elevatorKeypad.textDarkenedColor;
					elevatorKeypad.buttonHandlers[i].GetComponent<ElevatorButton>().floorAccessible = false;
				} else {
					elevatorKeypad.buttonSprites[i].overrideSprite = elevatorKeypad.buttonNormal;
					elevatorKeypad.buttonSprites[i].overrideSprite = elevatorKeypad.buttonNormal;
					elevatorKeypad.buttonTextHolders[i].color = elevatorKeypad.textEnabledColor;
					elevatorKeypad.buttonHandlers[i].GetComponent<ElevatorButton>().floorAccessible = true;
					elevatorKeypad.buttonHandlers[i].GetComponent<ElevatorButton>().targetDestination = elevatorKeypad.targetDestination[i];
				}
			} else {
				if (elevatorKeypad.buttons[i].activeSelf) elevatorKeypad.buttons[i].SetActive(false);
			}
		}
		elevatorKeypad.currentFloor = currentFloor;
		elevatorKeypad.activeKeypad = ke.gameObject;
		elevatorKeypad.SetCurrentFloor();
		if (lastDataSideRH) {
			OpenTab(4,true,TabMSG.Elevator,0,Handedness.RH);
		} else {
			OpenTab(4,true,TabMSG.Elevator,0,Handedness.LH);
		}
		linkedElevatorDoor = linkedDoor;
		objectInUsePos = tetherPoint;
		tetheredKeypadElevator = ke;
		usingObject = true;
	}

	public void UpdateHUDAmmoCounts(int amount) {
		wepmagCounterLH.UpdateDigits(amount);
		wepmagCounterRH.UpdateDigits(amount);
	}

	public void DisableSearchItemImage(int index) {
		searchItemImagesLH[index].SetActive(false);
		searchItemImagesRH[index].SetActive(false);
	}

	public void ReturnTabsFromSearch() {
		if (leftTC.curTab == 4) leftTC.ReturnToLastTab();
		if (rightTC.curTab == 4) rightTC.ReturnToLastTab();
	}

	public void NotifySearchThatSearchableWasDestroyed() {
		if (tetheredSearchable != null) {
			tetheredSearchable.ResetSearchable(false); // reset the actual object
			// reset the HUD contents
			if (headerTextRH.activeSelf) {
				headerTextRH.SetActive(false);
				headerText_textRH.enabled = false;
				headerText_textRH.text = System.String.Empty;
				noItemsTextRH.SetActive(false);
				noItemsTextRH.GetComponent<Text>().enabled = false;
				searchCloseButtonRH.SetActive(false);
				for (int i=0;i<4;i++) {
					searchItemImagesRH[i].SetActive(false);
					searchItemImagesRH[i].GetComponent<Image>().overrideSprite = Const.a.searchItemIconSprites[101];
					searchContainerRH.contents[i] = -1;
					searchContainerRH.customIndex[i] = -1;
				}
			}

			if (headerTextLH.activeSelf) {
				headerTextLH.SetActive(false);
				headerText_textLH.enabled = false;
				headerText_textLH.text = System.String.Empty;
				noItemsTextLH.SetActive(false);
				noItemsTextLH.GetComponent<Text>().enabled = false;
				searchCloseButtonLH.SetActive(false);
				for (int i=0;i<4;i++) {
					searchItemImagesLH[i].SetActive(false);
					searchItemImagesLH[i].GetComponent<Image>().overrideSprite = Const.a.searchItemIconSprites[101];
					searchContainerLH.contents[i] = -1;
					searchContainerLH.customIndex[i] = -1;
				}
			}

			tetheredSearchable = null;
			ReturnTabsFromSearch();
		}
	}


	public void ShowAmmoItems(int normdex, int altdex) {
		Utils.Activate(ammoIndicatorHunsLH);
		Utils.Activate(ammoIndicatorTensLH);
		Utils.Activate(ammoIndicatorOnesLH);
		Utils.Activate(unloadButtonLH);
		Utils.Activate(loadNormalAmmoButtonLH);
		Utils.Activate(loadAlternateAmmoButtonLH);
		Utils.Deactivate(energySliderLH);
		Utils.Deactivate(energyHeatTicksLH);
		Utils.Deactivate(overloadButtonLH);
		if (loadNormalAmmoButtonTextLH != null) {
			if (normdex > 0 && normdex < Const.a.stringTable.Length) {
				loadNormalAmmoButtonTextLH.text = Const.a.stringTable[normdex];
			} else {
				loadNormalAmmoButtonTextLH.text = "";
			}
		}

		if (loadAlternateAmmoButtonTextLH != null) {
			if (altdex > 0 && altdex < Const.a.stringTable.Length) {
				loadAlternateAmmoButtonTextLH.text = Const.a.stringTable[altdex];
			} else {
				loadAlternateAmmoButtonTextLH.text = "";
			}
		}

		Utils.Activate(ammoIndicatorHunsRH);
		Utils.Activate(ammoIndicatorTensRH);
		Utils.Activate(ammoIndicatorOnesRH);
		Utils.Activate(unloadButtonRH);
		Utils.Activate(loadNormalAmmoButtonRH);
		Utils.Activate(loadAlternateAmmoButtonRH);
		Utils.Deactivate(energySliderRH);
		Utils.Deactivate(energyHeatTicksRH);
		Utils.Deactivate(overloadButtonRH);
		if (loadNormalAmmoButtonTextRH != null) {
			if (normdex > 0 && normdex < Const.a.stringTable.Length) {
				loadNormalAmmoButtonTextRH.text = Const.a.stringTable[normdex];
			} else {
				loadNormalAmmoButtonTextRH.text = "";
			}
		}

		if (loadAlternateAmmoButtonTextRH != null) {
			if (altdex > 0 && altdex < Const.a.stringTable.Length) {
				loadAlternateAmmoButtonTextRH.text = Const.a.stringTable[altdex];
			} else {
				loadAlternateAmmoButtonTextRH.text = "";
			}
		}
	}

	public void ShowEnergyItems() {
		Utils.Activate(energySliderLH);
		Utils.Activate(energyHeatTicksLH);
		Utils.Activate(overloadButtonLH);
		Utils.Deactivate(ammoIndicatorHunsLH);
		Utils.Deactivate(ammoIndicatorTensLH);
		Utils.Deactivate(ammoIndicatorOnesLH);
		Utils.Deactivate(loadNormalAmmoButtonLH);
		Utils.Deactivate(loadAlternateAmmoButtonLH);
		Utils.Deactivate(unloadButtonLH);

		Utils.Activate(energySliderRH);
		Utils.Activate(energyHeatTicksRH);
		Utils.Activate(overloadButtonRH);
		Utils.Deactivate(ammoIndicatorHunsRH);
		Utils.Deactivate(ammoIndicatorTensRH);
		Utils.Deactivate(ammoIndicatorOnesRH);
		Utils.Deactivate(loadNormalAmmoButtonRH);
		Utils.Deactivate(loadAlternateAmmoButtonRH);
		Utils.Deactivate(unloadButtonRH);
	}

	public void HideAmmoAndEnergyItems() {
		Utils.Deactivate(ammoIndicatorHunsLH);
		Utils.Deactivate(ammoIndicatorTensLH);
		Utils.Deactivate(ammoIndicatorOnesLH);
		Utils.Deactivate(loadNormalAmmoButtonLH);
		Utils.Deactivate(loadAlternateAmmoButtonLH);
		Utils.Deactivate(energySliderLH);
		Utils.Deactivate(energyHeatTicksLH);
		Utils.Deactivate(overloadButtonLH);
		Utils.Deactivate(unloadButtonLH);

		Utils.Deactivate(ammoIndicatorHunsRH);
		Utils.Deactivate(ammoIndicatorTensRH);
		Utils.Deactivate(ammoIndicatorOnesRH);
		Utils.Deactivate(loadNormalAmmoButtonRH);
		Utils.Deactivate(loadAlternateAmmoButtonRH);
		Utils.Deactivate(energySliderRH);
		Utils.Deactivate(energyHeatTicksRH);
		Utils.Deactivate(overloadButtonRH);
		Utils.Deactivate(unloadButtonRH);
	}

	public void HideAlternateAmmoButton() {
		Utils.Deactivate(loadAlternateAmmoButtonRH);
		Utils.Deactivate(loadAlternateAmmoButtonRH);
	}

	public void SetAmmoIcons(int index, bool alt) {
		ammoIconManLH.SetAmmoIcon(index,alt);
		ammoIconManRH.SetAmmoIcon(index,alt);
	}

	void ChangeAmmoButtons(GameObject loadNormalAmmoButton,
						   GameObject loadAlternateAmmoButton) {

		if (loadNormalAmmoButton == null || loadAlternateAmmoButton == null) {
			return;
		}

		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(
							WeaponCurrent.a.weaponIndex);

		if (wep16index == 1 || wep16index == 4 || wep16index == 10
			|| wep16index == 14 || wep16index == 15) {

			return; // Already hidden.
		}

		Image norm = loadNormalAmmoButton.GetComponent<Image>();
		Image anorm = loadAlternateAmmoButton.GetComponent<Image>();
		if (Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent]) {
			SetAmmoIcons(WeaponCurrent.a.weaponIndex,true);
			norm.overrideSprite = ammoButtonDeHighlighted;
			if (WeaponCurrent.a.currentMagazineAmount2[WeaponCurrent.a.weaponCurrent] > 0) {
				anorm.overrideSprite = ammoButtonHighlighted;
			} else {
				anorm.overrideSprite = ammoButtonDeHighlighted;
			}
		} else {
			SetAmmoIcons(WeaponCurrent.a.weaponIndex,false);
			anorm.overrideSprite = ammoButtonDeHighlighted;
			if (WeaponCurrent.a.currentMagazineAmount[WeaponCurrent.a.weaponCurrent] > 0) {
				norm.overrideSprite = ammoButtonHighlighted;
			} else {
				norm.overrideSprite = ammoButtonDeHighlighted;
			}
		}
	}

	public void UpdateHUDAmmoCountsEither() {
		if (WeaponCurrent.a.weaponCurrent >= 0) {
			if (Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent]) {
				UpdateHUDAmmoCounts(WeaponCurrent.a.currentMagazineAmount2[WeaponCurrent.a.weaponCurrent]);
			} else {
				UpdateHUDAmmoCounts(WeaponCurrent.a.currentMagazineAmount[WeaponCurrent.a.weaponCurrent]);
			}
		}
	}

	void UpdateAmmoAndLoadButtons() {
		if (WeaponCurrent.a.weaponCurrent < 0
			|| WeaponCurrent.a.weaponCurrentPending >= 0) {

			return;
		}

		UpdateHUDAmmoCountsEither();
		ChangeAmmoButtons(loadNormalAmmoButtonLH,loadAlternateAmmoButtonLH);
		ChangeAmmoButtons(loadNormalAmmoButtonRH,loadAlternateAmmoButtonRH);
	}

	public void SetWepInfo(int index) {
		if (index >= 0) {
			weptextRH.text = weptextLH.text = Const.a.useableItemsNameText[index];
			iconRH.overrideSprite = iconLH.overrideSprite = Const.a.useableItemsIcons[index];
		} else {
			weptextRH.text = weptextLH.text = "";
			iconRH.overrideSprite = Const.a.useableItemsIcons[0]; // Nullsprite
			iconLH.overrideSprite = Const.a.useableItemsIcons[0]; // Nullsprite
		}
	}

	public void ReturnToLastTab(bool isRightHand) {
		//Debug.Log("Returned to last tab from MFDManager.cs");
		usingObject = false;
		objectInUsePos = new Vector3(999f,999f,999f); // out of bounds
		if (isRightHand) {
			rightTC.ReturnToLastTab();
		} else {
			leftTC.ReturnToLastTab();
		}
	}


	// Center tabs
	public void DisableAllCenterTabs () {
		MainTab.SetActive(false);
		HardwareTab.SetActive(false);
		GeneralTab.SetActive(false);
		SoftwareTab.SetActive(false);
		DataReaderContentTab.SetActive(false);
	}

	void ToggleHighlightOnCenterTabButton (int buttonIndex) {
		Image buttonImage = null;
		switch (buttonIndex) {
			case 0: if (buttonImage != MainTabButton.image) buttonImage = MainTabButton.image; break;
			case 1: if (buttonImage != HardwareTabButton.image) buttonImage = HardwareTabButton.image; break;
			case 2: if (buttonImage != GeneralTabButton.image) buttonImage = GeneralTabButton.image; break;
			case 3: if (buttonImage != SoftwareTabButton.image) buttonImage = SoftwareTabButton.image; break;
		}

		if (buttonImage == null) return;
		if (highlightStatus[buttonIndex]) {
			if (buttonImage.overrideSprite != MFDSpriteNotification) buttonImage.overrideSprite = MFDSpriteNotification;
		} else {
			if (curCenterTab == buttonIndex) {
				if (buttonImage.overrideSprite != MFDSpriteSelected) buttonImage.overrideSprite = MFDSpriteSelected;
			} else {
				if (buttonImage.overrideSprite != MFDSprite) buttonImage.overrideSprite = MFDSprite;
			}
		}

		highlightTickCount[buttonIndex]++;
		highlightStatus[buttonIndex] = (!highlightStatus[buttonIndex]);

		if (highlightTickCount[buttonIndex] >= numTicks) {
			highlightStatus[buttonIndex] = false;
			highlightTickCount[buttonIndex] = 0;
			tabNotified[buttonIndex] = false; // stop blinking
			if (curCenterTab == buttonIndex) {
				if (buttonImage.overrideSprite != MFDSpriteSelected) buttonImage.overrideSprite = MFDSpriteSelected; // If we are on this tab, return to selected
			} else {
				if (buttonImage.overrideSprite != MFDSprite) buttonImage.overrideSprite = MFDSprite; // Return to normal
			}
		}
	}

	public void NotifyToCenterTab(int tabNum) {
		tabNotified[tabNum] = true;
		centerTabsTickFinished = PauseScript.a.relativeTime + centerTabsTickTime;
		ToggleHighlightOnCenterTabButton(tabNum);
	}

	public void CenterTabButtonClick(int tabNum) {
		MFDManager.a.mouseClickHeldOverGUI = true;
		CenterTabButtonAction(tabNum);
	}

	public void CenterTabButtonAction(int tabNum) {
		if (PauseScript.a.mainMenu.activeInHierarchy) return;
		Utils.PlayOneShotSavable(TabSFX,TabSFXClip);
		CenterTabButtonClickSilent(tabNum,false);
		if (Inventory.a.hardwareIsActive[3]) {
			hwb.SensaroundOff();
			Utils.PlayOneShotSavable(hwb.SFX,hwb.SFXClipDeactivate[1]);
		}
	}

	public void CenterTabButtonClickSilent(int tabNum, bool forceOn) {
		bool wasActive = false;

		switch (tabNum) {
		case 0:
			wasActive = MainTab.activeInHierarchy;
			DisableAllCenterTabs();
			if (curCenterTab == 0) {
				if (wasActive && !forceOn) {
					break;
				} else {
					MainTab.SetActive(true);
					break;
				}
			}
			MainTabButton.image.overrideSprite = MFDSpriteSelected;
			DisableAllCenterTabs();
			MainTab.SetActive(true);
			HardwareTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			SoftwareTabButton.image.overrideSprite = MFDSprite;
			curCenterTab = 0;
			break;
		case 1:
			wasActive = HardwareTab.activeInHierarchy;
			DisableAllCenterTabs();
			if (curCenterTab == 1) {
				if (wasActive && !forceOn) {
					break;
				} else {
					HardwareTab.SetActive(true);
					break;
				}
			}
			HardwareTabButton.image.overrideSprite = MFDSpriteSelected;
			DisableAllCenterTabs();
			HardwareTab.SetActive(true);
			MainTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			SoftwareTabButton.image.overrideSprite = MFDSprite;
			curCenterTab = 1;
			break;
		case 2:
			wasActive = GeneralTab.activeInHierarchy;
			DisableAllCenterTabs();
			if (curCenterTab == 2) {
				if (wasActive && !forceOn) {
					break;
				} else {
					GeneralTab.SetActive(true);
					break;
				}
			}
			GeneralTabButton.image.overrideSprite = MFDSpriteSelected;
			DisableAllCenterTabs();
			GeneralTab.SetActive(true);
			MainTabButton.image.overrideSprite = MFDSprite;
			HardwareTabButton.image.overrideSprite = MFDSprite;
			SoftwareTabButton.image.overrideSprite = MFDSprite;
			curCenterTab = 2;
			break;
		case 3:
			wasActive = SoftwareTab.activeInHierarchy;
			DisableAllCenterTabs();
			if (curCenterTab == 3) {
				if (wasActive && !forceOn) {
					break;
				} else {
					SoftwareTab.SetActive(true);
					break;
				}
			}
			SoftwareTabButton.image.overrideSprite = MFDSpriteSelected;
			DisableAllCenterTabs();
			SoftwareTab.SetActive(true);
			MainTabButton.image.overrideSprite = MFDSprite;
			HardwareTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			curCenterTab = 3;
			break;
		case 4:
			DisableAllCenterTabs();
			DataReaderContentTab.SetActive(true);
			OpenLogTableContents();
			MainTabButton.image.overrideSprite = MFDSprite;
			HardwareTabButton.image.overrideSprite = MFDSprite;
			GeneralTabButton.image.overrideSprite = MFDSprite;
			SoftwareTabButton.image.overrideSprite = MFDSprite;
			curCenterTab = 4;
			break;
		}
	}
	//--- End Center Tabs ---

	// Multi Media Tabs
	public void OpenLastMultiMediaTab() {
		switch (lastMultiMediaTabOpened) {
			case 0: OpenEmailTableContents(); break;
			case 1: OpenLogTableContents(); break;
			case 2: OpenDataTableContents(); break;
			case 3: OpenNotesTableContents(); break;
		}
	}

	public void ResetMultiMediaTabs() {
		startingSubTab.SetActive(false);
		secondaryTab1.SetActive(false);
		secondaryTab2.SetActive(false);
		emailTab.SetActive(false);
		ersbLH.SetEReaderSectionsButtonsHighlights(lastMultiMediaTabOpened);
		ersbRH.SetEReaderSectionsButtonsHighlights(lastMultiMediaTabOpened);
		dataTab.SetActive(false);
		notesTab.SetActive(false);
		multiMediaHeaderLabel.text = System.String.Empty;
	}

	public void OpenLogTableContents() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		ResetMultiMediaTabs();
		startingSubTab.SetActive(true);
		multiMediaHeaderLabel.text = "LOGS";
		Inventory.a.hasNewLogs = false;
		lastMultiMediaTabOpened = 1;
		ersbLH.SetEReaderSectionsButtonsHighlights(1);
		ersbRH.SetEReaderSectionsButtonsHighlights(1);
	}

	public void OpenLogsLevelFolder(int curlevel) {
		MFDManager.a.mouseClickHeldOverGUI = true;
		ResetMultiMediaTabs();
		secondaryTab1.SetActive(true);
		multiMediaHeaderLabel.text = "Level " + curlevel.ToString() + " Logs";
		secondaryTab1.GetComponent<LogContentsButtonsManager>().currentLevelFolder = curlevel;
		secondaryTab1.GetComponent<LogContentsButtonsManager>().InitializeLogsFromLevelIntoFolder();
	}

	public void OpenLogTextReader() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		ResetMultiMediaTabs();
		secondaryTab2.SetActive(true);
	}

	public void OpenEmailTableContents() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		ResetMultiMediaTabs();
		emailTab.SetActive(true);
		Inventory.a.hasNewEmail = false;
		multiMediaHeaderLabel.text = "EMAIL";
		lastMultiMediaTabOpened = 0;
		ersbLH.SetEReaderSectionsButtonsHighlights(0);
		ersbRH.SetEReaderSectionsButtonsHighlights(0);
	}

	public void OpenDataTableContents() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		ResetMultiMediaTabs();
		dataTab.SetActive(true);
		Inventory.a.hasNewData = false;
		multiMediaHeaderLabel.text = "DATA";
		lastMultiMediaTabOpened = 2;
		ersbLH.SetEReaderSectionsButtonsHighlights(2);
		ersbRH.SetEReaderSectionsButtonsHighlights(2);
	}

	public void OpenNotesTableContents() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		ResetMultiMediaTabs();
		notesTab.SetActive(true);
		Inventory.a.hasNewNotes = false;
		multiMediaHeaderLabel.text = "NOTES";
		lastMultiMediaTabOpened = 3;
		ersbLH.SetEReaderSectionsButtonsHighlights(3);
		ersbRH.SetEReaderSectionsButtonsHighlights(3);
	}
	//--- End Multi Media Tabs ---

	public static string Save(GameObject go) {
		MFDManager mfd = go.GetComponent<MFDManager>();
		if (mfd == null) {
			Debug.Log("MFDManager missing on Player!  GameObject.name: " + go.name);
			return Utils.DTypeWordToSaveString("bbbbbbbbbufffbbbbbbtbutuu");
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(mfd.lastWeaponSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastItemSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastAutomapSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastTargetSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastDataSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastSearchSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastLogSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastLogSecondarySideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastMinigameSideRH); // bool
		line += Utils.splitChar + Utils.UintToString(mfd.lastMultiMediaTabOpened); // int
		line += Utils.splitChar + Utils.FloatToString(mfd.objectInUsePos.x)
			  + Utils.splitChar + Utils.FloatToString(mfd.objectInUsePos.y)
			  + Utils.splitChar + Utils.FloatToString(mfd.objectInUsePos.z);
		// tetheredPGP
		// tetheredPWP
		// tetheredSearchable
		// tetheredKeypadElevator
		// tetheredKeypadKeycode
		line += Utils.splitChar + Utils.BoolToString(mfd.paperLogInUse); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.usingObject); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.logReaderContainer.activeSelf); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.DataReaderContentTab.activeSelf); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.logTable.activeSelf); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.logLevelsFolder.activeSelf); // bool
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(mfd.logFinished);
		line += Utils.splitChar + Utils.BoolToString(mfd.logActive); // bool
		line += Utils.splitChar + Utils.IntToString(Utils.GetIntFromAudioLogType(mfd.logType)); // int
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(mfd.cyberTimer.GetComponent<CyberTimer>().timerFinished);

		if (mfd.leftTC != null) {
			line += Utils.splitChar + Utils.UintToString(mfd.leftTC.curTab);
		} else line += Utils.splitChar + "0";

		if (mfd.rightTC != null) {
			line += Utils.splitChar + Utils.UintToString(mfd.rightTC.curTab);
		} else line += Utils.splitChar + "0";

		line += Utils.splitChar;
		line += Utils.BoolToString(mfd.leftTC.TabManager.WeaponTab.activeSelf,
								   "leftTC.TabManager.WeaponTab.activeSelf");

		line += Utils.splitChar;
		line += Utils.BoolToString(mfd.leftTC.TabManager.ItemTab.activeSelf,
								   "leftTC.TabManager.ItemTab.activeSelf");

		line += Utils.splitChar;
		line += Utils.BoolToString(mfd.leftTC.TabManager.AutomapTab.activeSelf,
								   "leftTC.TabManager.AutomapTab.activeSelf");

		line += Utils.splitChar;
		line += Utils.BoolToString(mfd.leftTC.TabManager.TargetTab.activeSelf,
								   "leftTC.TabManager.TargetTab.activeSelf");

		line += Utils.splitChar;
		line += Utils.BoolToString(mfd.leftTC.TabManager.DataTab.activeSelf,
								   "leftTC.TabManager.DataTab.activeSelf");

		line += Utils.splitChar;
		line += Utils.BoolToString(mfd.rightTC.TabManager.WeaponTab.activeSelf,
								   "rightTC.TabManager.WeaponTab.activeSelf");

		line += Utils.splitChar;
		line += Utils.BoolToString(mfd.rightTC.TabManager.ItemTab.activeSelf,
								   "rightTC.TabManager.ItemTab.activeSelf");

		line += Utils.splitChar;
		line += Utils.BoolToString(mfd.rightTC.TabManager.AutomapTab.activeSelf,
								   "rightTC.TabManager.AutomapTab.activeSelf");

		line += Utils.splitChar;
		line += Utils.BoolToString(mfd.rightTC.TabManager.TargetTab.activeSelf,
								   "rightTC.TabManager.TargetTab.activeSelf");

		line += Utils.splitChar;
		line += Utils.BoolToString(mfd.rightTC.TabManager.DataTab.activeSelf,
								   "rightTC.TabManager.DataTab.activeSelf");

		line += Utils.splitChar;
		line += Utils.UintToString(mfd.curCenterTab,"curCenterTab");
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		MFDManager mfd = go.GetComponent<MFDManager>();
		if (mfd == null) {
			Debug.Log("MFDManager.Load failure, mfd == null");
			return index + 25;
		}

		if (index < 0) {
			Debug.Log("MFDManager.Load failure, index < 0");
			return index + 25;
		}

		if (entries == null) {
			Debug.Log("MFDManager.Load failure, entries == null");
			return index + 25;
		}

		float readFloatx, readFloaty, readFloatz;
		mfd.lastWeaponSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastItemSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastAutomapSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastTargetSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastDataSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastSearchSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastLogSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastLogSecondarySideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastMinigameSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastMultiMediaTabOpened = Utils.GetIntFromString(entries[index]); index++;
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		mfd.objectInUsePos = new Vector3(readFloatx,readFloaty,readFloatz);
		// tetheredPGP
		// tetheredPWP
		// tetheredSearchable
		// tetheredKeypadElevator
		// tetheredKeypadKeycode
		mfd.paperLogInUse = Utils.GetBoolFromString(entries[index]); index++;
		mfd.usingObject = Utils.GetBoolFromString(entries[index]); index++;
		mfd.logReaderContainer.SetActive(Utils.GetBoolFromString(entries[index])); index++;
		mfd.DataReaderContentTab.SetActive(Utils.GetBoolFromString(entries[index])); index++;
		mfd.logTable.SetActive(Utils.GetBoolFromString(entries[index])); index++;
		mfd.logLevelsFolder.SetActive(Utils.GetBoolFromString(entries[index])); index++;
		mfd.logFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		mfd.logActive = Utils.GetBoolFromString(entries[index]); index++;
		mfd.logType = Utils.GetAudioLogTypeFromInt(Utils.GetIntFromString(entries[index])); index++;
		mfd.cyberTimer.GetComponent<CyberTimer>().timerFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		if (mfd.leftTC != null) {
			mfd.leftTC.curTab = Utils.GetIntFromString(entries[index]); index++;
			mfd.leftTC.SetCurrentAsLast();
			mfd.leftTC.ReturnToLastTab();
		} else index++;

		if (mfd.rightTC != null) {
			mfd.rightTC.curTab = Utils.GetIntFromString(entries[index]); index++;
			mfd.rightTC.SetCurrentAsLast();
			mfd.rightTC.ReturnToLastTab();
		} else index++;

		bool wepTabActiveLH = Utils.GetBoolFromString(entries[index],
									"leftTC.TabManager.WeaponTab.activeSelf");
		index++;

		bool itemTabActiveLH = Utils.GetBoolFromString(entries[index],
									"leftTC.TabManager.ItemTab.activeSelf");
		index++;

		bool amapTabActiveLH = Utils.GetBoolFromString(entries[index],
									"leftTC.TabManager.AutomapTab.activeSelf");
		index++;

		bool targTabActiveLH = Utils.GetBoolFromString(entries[index],
									"leftTC.TabManager.TargetTab.activeSelf");
		index++;

		bool dataTabActiveLH = Utils.GetBoolFromString(entries[index],
									"leftTC.TabManager.DataTab.activeSelf");
		index++;
		mfd.leftTC.TabManager.WeaponTab.SetActive(wepTabActiveLH);
		mfd.leftTC.TabManager.ItemTab.SetActive(itemTabActiveLH);
		mfd.leftTC.TabManager.AutomapTab.SetActive(amapTabActiveLH);
		mfd.leftTC.TabManager.TargetTab.SetActive(targTabActiveLH);
		mfd.leftTC.TabManager.DataTab.SetActive(dataTabActiveLH);



		bool wepTabActiveRH = Utils.GetBoolFromString(entries[index],
									"rightTC.TabManager.WeaponTab.activeSelf");
		index++;

		bool itemTabActiveRH = Utils.GetBoolFromString(entries[index],
									"rightTC.TabManager.ItemTab.activeSelf");
		index++;

		bool amapTabActiveRH = Utils.GetBoolFromString(entries[index],
									"rightTC.TabManager.AutomapTab.activeSelf");
		index++;

		bool targTabActiveRH = Utils.GetBoolFromString(entries[index],
									"rightTC.TabManager.TargetTab.activeSelf");
		index++;

		bool dataTabActiveRH = Utils.GetBoolFromString(entries[index],
										"rightTC.TabManager.DataTab.activeSelf");
		index++;
		mfd.rightTC.TabManager.WeaponTab.SetActive(wepTabActiveRH);
		mfd.rightTC.TabManager.ItemTab.SetActive(itemTabActiveRH);
		mfd.rightTC.TabManager.AutomapTab.SetActive(amapTabActiveRH);
		mfd.rightTC.TabManager.TargetTab.SetActive(targTabActiveRH);
		mfd.rightTC.TabManager.DataTab.SetActive(dataTabActiveRH);

		mfd.curCenterTab = Utils.GetIntFromString(entries[index],
												  "curCenterTab");
		mfd.CenterTabButtonClickSilent(mfd.curCenterTab,true);
		index++;

		mfd.SetWepInfo(WeaponCurrent.a.weaponIndex);
		return index;
	}
}
