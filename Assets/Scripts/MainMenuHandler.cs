using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.IO;
using System.Collections;
using SimpleFileBrowser;

public class MainMenuHandler : MonoBehaviour {
	public GameObject Button1;
	public GameObject Button2;
	public GameObject Button3;
	public GameObject Button4;
	public GameObject startFXObject;
	public GameObject saltTheFries;
	public GameObject mainCamera;
	public GameObject singleplayerPage;
	public GameObject multiplayerPage;
	public GameObject newgamePage;
	public GameObject frontPage;
	public GameObject loadPage;
	public GameObject savePage;
	public GameObject optionsPage;
	public GameObject creditsPage;
	public GameObject CouldNotFindDialogue;
	public GameObject SuccessBanner;
	public GameObject FailureBanner;
	public GameObject InitialDisplay;
	public InputField dataPathInputText;
	public InputField newgameInputText;
	public StartMenuDifficultyController combat;
	public StartMenuDifficultyController mission;
	public StartMenuDifficultyController puzzle;
	public StartMenuDifficultyController cyber;
	public InputField[] saveNameInputField;
	public GameObject[] saveNameInput;
	public GameObject[] saveNamePlaceholder;
	public Text[] saveButtonText;
	public Text[] loadButtonText;
	public int currentSaveSlot = -1;
	public CreditsScroll credScrollManager;
	public GameObject IntroVideo;
	public GameObject IntroVideoContainer;
	public bool inCutscene = false;
	public GameObject PresetConfirmDialog;
	public Text presetQuestionText;
	public string presetQuestionDefault = "RESET ALL KEYS TO DEFAULT?";
	public string presetQuestionLegacy = "CHANGE ALL KEYS TO LEGACY PRESET?";
	public static MainMenuHandler a;
	public ConfigKeybindButton[] keybindButtons;
	public ConfigToggles ctInvertUpDnLook;
	public ConfigToggles ctInvertUpDnCyberLook;
	public ConfigToggles ctInvertInventoryCyc;
	public ConfigToggles ctQuickItemPickUp;
	public ConfigToggles ctQuickReload;
	public ConfigToggles ctNoShootMode;
	public GameObject introVideoTextGO1;
	public GameObject introVideoTextGO2;
	public GameObject introVideoTextGO3;
	public GameObject introVideoTextGO4;
	public GameObject introVideoTextGO5;
	public GameObject introVideoTextGO6;
	public GameObject introVideoTextGO7;
	public GameObject introVideoTextGO8;
	public GameObject introVideoTextGO9;
	public GameObject introVideoTextGO10;
	public GameObject introVideoTextGO11;
	public GameObject introVideoTextGO12;
	public GameObject introVideoTextGO13;
	public GameObject introVideoTextGO14;
	public GameObject introVideoTextGO15;
	public Text introVideoText1;
	public Text introVideoText2;
	public Text introVideoText3;
	public Text introVideoText4;
	public Text introVideoText5;
	public Text introVideoText6;
	public Text introVideoText7;
	public Text introVideoText8;
	public Text introVideoText9;
	public Text introVideoText10;
	public Text introVideoText11;
	public Text introVideoText12;
	public Text introVideoText13;
	public Text introVideoText14;
	public Text introVideoText15;
	public VideoPlayer introPlayer;

	public GameObject DeathVideo;
	public GameObject DeathVideoContainer;
	public VideoPlayer deathPlayer;
	public GameObject deathVideoTextGO1;
	public GameObject deathVideoTextGO2;
	public Text deathVideoText1;
	public Text deathVideoText2;

	public GameObject GraphicsTab;
	public GameObject InputTab;
	public GameObject AudioTab;
	public Image GraphicsTabButtonImage;
	public Image InputTabButtonImage;
	public Image AudioTabButtonImage;
	public Text GraphicsTabButtonText;
	public Text InputTabButtonText;
	public Text AudioTabButtonText;
	public Sprite OptionsTabDehilited;
	public Sprite OptionsTabHilited;
	public Camera configCamera;
	public AudioSource BackGroundMusic;
	public ConfigurationMenuAAApply aaaApply;
	public ConfigurationMenuShadowsApply shadApply;
	public ConfigurationMenuSSRApply ssrApply;
	public ConfigurationMenuAudioModeApply audModeApply;
	public ConfigurationMenuModelDetailApply mdlDetApply;

	[HideInInspector] public bool returnToPause = false;
	public bool dataFound = false;
	private enum Pages : byte {fp,sp,mp,np,lp,op,sv,cd};
	private Pages currentPage;
	private bool typingSaveGame = false;
	private string tempSaveNameHolder;
	private int presetQuestionValue = -1;
	private float vidFinished;
	private float vidLength = 117.5f;
	private float deathvidLength = 16.8f;
	private float vidStartTime;

	void Awake() {
		a = this;
		BackGroundMusic.ignoreListenerPause = true; // Play when paused.
		ResetPages();
		dataFound = false;
		inCutscene = false;
		Config.SetVolume();
		if (Application.platform == RuntimePlatform.Android) {
			dataFound = true;
			Config.SetVolume();
			GoToFrontPage();
			CheckAndPlayIntro();
			return;
		}
		FileBrowser.SetFilters(false,new FileBrowser.Filter("SHOCK RES Files",
															".RES",".res"));
		FileBrowser.SetDefaultFilter( ".RES" );
		StartCoroutine(CheckDataFiles());
	}

	// Improve menu performance.
	void DisableCameraDuringMenu() {
		if (MouseLookScript.a == null) return;

		MouseLookScript.a.playerCamera.enabled = false; 
	}

	void ReEnableCamera() {
		if (MouseLookScript.a == null) return;

		MouseLookScript.a.playerCamera.enabled = true; 
	}

	void OnEnable() {
		if (Inventory.a != null) Inventory.a.HideBioMonitor();
		DisableCameraDuringMenu();
		if (IntroVideoContainer.activeSelf) {
			vidFinished = Time.time + vidLength;
			vidStartTime = Time.time;

			// Setup text.
			introVideoText1.text = Const.a.stringTable[613];
			introVideoText2.text = Const.a.stringTable[614];
			introVideoText3.text = Const.a.stringTable[615];
			introVideoText4.text = Const.a.stringTable[616];
			introVideoText5.text = Const.a.stringTable[617];
			introVideoText6.text = Const.a.stringTable[618];
			introVideoText7.text = Const.a.stringTable[619];
			introVideoText8.text = Const.a.stringTable[620];
			introVideoText9.text = Const.a.stringTable[621];
			introVideoText10.text = Const.a.stringTable[622];
			introVideoText11.text = Const.a.stringTable[623];
			introVideoText12.text = Const.a.stringTable[624];
			introVideoText13.text = Const.a.stringTable[625];
			introVideoText14.text = Const.a.stringTable[626];
			introVideoText15.text = Const.a.stringTable[627];
			Utils.Activate(introVideoTextGO1);
			Utils.Deactivate(introVideoTextGO2);
			Utils.Deactivate(introVideoTextGO3);
			Utils.Deactivate(introVideoTextGO4);
			Utils.Deactivate(introVideoTextGO5);
			Utils.Deactivate(introVideoTextGO6);
			Utils.Deactivate(introVideoTextGO7);
			Utils.Deactivate(introVideoTextGO8);
			Utils.Deactivate(introVideoTextGO9);
			Utils.Deactivate(introVideoTextGO10);
			Utils.Deactivate(introVideoTextGO11);
			Utils.Deactivate(introVideoTextGO12);
			Utils.Deactivate(introVideoTextGO13);
			Utils.Deactivate(introVideoTextGO14);
			Utils.Deactivate(introVideoTextGO15);
		}
	}

	void OnDisable() {
		if (Inventory.a != null) Inventory.a.UnHideBioMonitor();
		ReEnableCamera();
	}
	
	void CheckAndPlayIntro() {
		string indn = Utils.SafePathCombine(Application.streamingAssetsPath,"introdone.dat");
		if (System.IO.File.Exists(indn)) {
			IntroVideo.SetActive(false);	
			IntroVideoContainer.SetActive(false);
		} else {
			System.IO.File.Create(indn);
			PlayIntro();
		}
	}

	IEnumerator CheckDataFiles () {
		BackGroundMusic.Stop();
		string alogPath = Utils.SafePathCombine(Application.streamingAssetsPath,
												"CITALOG.RES");

		string barkPath = Utils.SafePathCombine(Application.streamingAssetsPath,
												"CITBARK.RES");
		if (File.Exists(alogPath) && File.Exists(barkPath)) {
			// Go right on into the game, all good here.
			dataFound = true;
			Config.SetVolume();
			GoToFrontPage();
			CheckAndPlayIntro();
		} else {
			// Fake like we are checking for the files to be there.
			// It's fake because we already did it and it is instant.
			// This is just to show the user that we did in fact look.
			InitialDisplay.SetActive(true);
			IntroVideo.SetActive(false);	
			IntroVideoContainer.SetActive(false);
			yield return new WaitForSeconds(0.3f);

			// OK, now show that we didn't find them
			InitialDisplay.SetActive(false);
			CouldNotFindDialogue.SetActive(true);
			dataFound = false;
		}
	}

	void LeaveDeathCutscene() {
		inCutscene = false;
		DeathVideo.SetActive(false);
		DeathVideoContainer.SetActive(false);
		BackGroundMusic.clip = Music.a.titleMusic;
		if (gameObject.activeSelf && dataFound) BackGroundMusic.Play();
	}

	void LeaveIntroCutscene() {
		inCutscene = false;
		IntroVideo.SetActive(false);
		IntroVideoContainer.SetActive(false);
		Const.a.WriteDatForIntroPlayed(false);
		if (gameObject.activeSelf && dataFound) BackGroundMusic.Play();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)
			|| Input.GetMouseButtonDown(1)
			|| Input.GetKeyDown(KeyCode.Escape)
			|| Input.GetKeyDown(KeyCode.JoystickButton0)
			|| Input.GetKeyDown(KeyCode.JoystickButton1)
			|| Input.anyKey) {
			if ((inCutscene || IntroVideoContainer.activeSelf
				|| DeathVideoContainer.activeSelf)
				&& !CouldNotFindDialogue.activeSelf) {
				if (IntroVideoContainer.activeSelf) {
					LeaveIntroCutscene();
				} else if (DeathVideoContainer.activeSelf) {
					LeaveDeathCutscene();
				}

				return;
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape)
			|| Input.GetKeyDown(KeyCode.JoystickButton1)) { // Escape/back
															// button listener
			if (savePage.activeInHierarchy && !newgamePage.activeInHierarchy) {
				if (currentSaveSlot > 0
					&& currentSaveSlot < saveNameInputField.Length) {
					InputField infld = saveNameInputField[currentSaveSlot];
					if (infld != null) infld.DeactivateInputField();
				}
				currentSaveSlot = -1;
				typingSaveGame = false;
				returnToPause = true;
			}

			GoBack();
			return;
		}

		if (IntroVideoContainer.activeSelf) {
			if (vidFinished > 0 && (Time.time - vidStartTime) > 6.7f
				&& introVideoTextGO1.activeSelf
				&& !introVideoTextGO2.activeSelf) {

				Utils.Deactivate(introVideoTextGO1);
				Utils.Activate(introVideoTextGO2);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 9.9f
				&& introVideoTextGO2.activeSelf
				&& !introVideoTextGO3.activeSelf) {

				Utils.Deactivate(introVideoTextGO2);
				Utils.Activate(introVideoTextGO3);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 19.2f
				&& introVideoTextGO3.activeSelf
				&& !introVideoTextGO4.activeSelf) {

				Utils.Deactivate(introVideoTextGO3);
				Utils.Activate(introVideoTextGO4);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 30.7f
				&& introVideoTextGO4.activeSelf
				&& !introVideoTextGO5.activeSelf) {

				Utils.Deactivate(introVideoTextGO4);
				Utils.Activate(introVideoTextGO5);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 37.9f
				&&  introVideoTextGO5.activeSelf
				&& !introVideoTextGO6.activeSelf) {

				Utils.Deactivate(introVideoTextGO5);
				Utils.Activate(  introVideoTextGO6);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 43.7f
				&&  introVideoTextGO6.activeSelf
				&& !introVideoTextGO7.activeSelf) {

				Utils.Deactivate(introVideoTextGO6);
				Utils.Activate(  introVideoTextGO7);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 48.1f
				&&  introVideoTextGO7.activeSelf
				&& !introVideoTextGO8.activeSelf) {

				Utils.Deactivate(introVideoTextGO7);
				Utils.Activate(  introVideoTextGO8);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 59.3f
				&&  introVideoTextGO8.activeSelf
				&& !introVideoTextGO9.activeSelf) {

				Utils.Deactivate(introVideoTextGO8);
				Utils.Activate(  introVideoTextGO9);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 69.1f
				&&  introVideoTextGO9.activeSelf
				&& !introVideoTextGO10.activeSelf) {

				Utils.Deactivate(introVideoTextGO9);
				Utils.Activate(  introVideoTextGO10);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 74.5f
				&&  introVideoTextGO10.activeSelf
				&& !introVideoTextGO11.activeSelf) {

				Utils.Deactivate(introVideoTextGO10);
				Utils.Activate(  introVideoTextGO11);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 81.2f
				&&  introVideoTextGO11.activeSelf
				&& !introVideoTextGO12.activeSelf) {

				Utils.Deactivate(introVideoTextGO11);
				Utils.Activate(  introVideoTextGO12);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 89.2f
				&&  introVideoTextGO12.activeSelf
				&& !introVideoTextGO13.activeSelf) {

				Utils.Deactivate(introVideoTextGO12);
				Utils.Activate(  introVideoTextGO13);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 98.4f
				&&  introVideoTextGO13.activeSelf
				&& !introVideoTextGO14.activeSelf) {

				Utils.Deactivate(introVideoTextGO13);
				Utils.Activate(  introVideoTextGO14);
			}

			if (vidFinished > 0 && (Time.time - vidStartTime) > 105.0f
				&&  introVideoTextGO14.activeSelf
				&& !introVideoTextGO15.activeSelf) {

				Utils.Deactivate(introVideoTextGO14);
				Utils.Activate(  introVideoTextGO15);
			}


			if (vidFinished < Time.time && IntroVideoContainer.activeSelf
				&& vidFinished > 0) {

				vidFinished = 0;
				Utils.Deactivate(IntroVideoContainer);
				Utils.Deactivate(introVideoTextGO1);
				Utils.Deactivate(introVideoTextGO2);
				Utils.Deactivate(introVideoTextGO3);
				Utils.Deactivate(introVideoTextGO4);
				Utils.Deactivate(introVideoTextGO5);
				Utils.Deactivate(introVideoTextGO6);
				Utils.Deactivate(introVideoTextGO7);
				Utils.Deactivate(introVideoTextGO8);
				Utils.Deactivate(introVideoTextGO9);
				Utils.Deactivate(introVideoTextGO10);
				Utils.Deactivate(introVideoTextGO11);
				Utils.Deactivate(introVideoTextGO12);
				Utils.Deactivate(introVideoTextGO13);
				Utils.Deactivate(introVideoTextGO14);
				Utils.Deactivate(introVideoTextGO15);
			}
		} else if (DeathVideoContainer.activeSelf) {
			if (vidFinished > 0 && (Time.time - vidStartTime) > 5.53f
				&& deathVideoTextGO1.activeSelf
				&& !deathVideoTextGO2.activeSelf) {

				Utils.Deactivate(deathVideoTextGO1);
				Utils.Activate(deathVideoTextGO2);
			}

			if (vidFinished < Time.time && DeathVideoContainer.activeSelf
				&& vidFinished > 0) {

				vidFinished = 0;
				Utils.Deactivate(DeathVideoContainer);
				Utils.Deactivate(deathVideoTextGO1);
				Utils.Deactivate(deathVideoTextGO2);
				BackGroundMusic.clip = Music.a.titleMusic;
				if (gameObject.activeSelf && dataFound) BackGroundMusic.Play();
			}
		} else {
			if (!BackGroundMusic.isPlaying
				&& !saltTheFries.activeInHierarchy
				&& gameObject.activeSelf) {
				BackGroundMusic.clip = Music.a.titleMusic;
				if (gameObject.activeSelf && dataFound) BackGroundMusic.Play();
			}
		}

		// Qmaster's cheat
		if ((   (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.P))
			 || (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKey(KeyCode.P)))
			&& !CouldNotFindDialogue.activeInHierarchy) {
			if (string.IsNullOrWhiteSpace(Const.a.playerName)) {
				Const.a.playerName = "Qmaster";
			}

			StartGame(true);
			return;
		}

		if (typingSaveGame && (Input.GetKeyUp(KeyCode.Return)
							   || Input.GetKeyUp(KeyCode.KeypadEnter)
							   || Input.GetKeyDown(KeyCode.JoystickButton0))
			&& savePage.activeInHierarchy
			&& !newgamePage.activeInHierarchy) {
			if (currentSaveSlot < 0) return;

			InputField infldTemp = saveNameInputField[currentSaveSlot];
			string sname = infldTemp.text;
			if (!string.IsNullOrEmpty(sname)) {
				if (sname == "- unused -") sname = "Savegame: - unused - "
												   + currentSaveSlot.ToString();
				SaveGame(currentSaveSlot,sname);
				saveNameInput[currentSaveSlot].SetActive(false);
				saveNamePlaceholder[currentSaveSlot].SetActive(false);
				saveButtonText[currentSaveSlot].text = sname;
				typingSaveGame = false;
				currentSaveSlot = -1;
			} else {
				GoBack();
				return;
			}
		}

		UpdateConfigTabTextColor();
	}

	public void StartGame (bool isNew) {
		Const.a.difficultyCombat = combat.difficultySetting;
		Const.a.difficultyMission = mission.difficultySetting;
		Const.a.difficultyPuzzle = puzzle.difficultySetting;
		Const.a.difficultyCyber = cyber.difficultySetting;
		if (isNew) {
			string pname = newgamePage.GetComponentInChildren<InputField>(true).text;
			if (string.IsNullOrWhiteSpace(pname)) pname = "Hacker";
			Const.a.playerName = pname;
			Const.a.NewGame();
		}

		MouseCursor.a.mainCamera.enabled = true;
		this.gameObject.SetActive(false);
	}

	void ResetPages() {
		singleplayerPage.SetActive(false);
		multiplayerPage.SetActive(false);
		newgamePage.SetActive(false);
		frontPage.SetActive(false);
		loadPage.SetActive(false);
		savePage.SetActive(false);
		optionsPage.SetActive(false);
		creditsPage.SetActive(false);
	}

	public void GoToFrontPage() {
		ResetPages();
		frontPage.SetActive(true);
		currentPage = Pages.fp;
	}

	public void GoToSingleplayerSubmenu() {
		ResetPages();
		singleplayerPage.SetActive(true);
		currentPage = Pages.sp;
	}

	public void GoToMultiplayerSubmenu() {
		ResetPages();
		multiplayerPage.SetActive(true);
		currentPage = Pages.mp;
	}

	public void GoToOptionsSubmenu(bool accessedFromPause) {
		ResetPages();
		if (accessedFromPause) IntroVideoContainer.SetActive(false);
		optionsPage.SetActive(true);
		SetOptionsTabGraphics();
		currentPage = Pages.op;
		returnToPause = accessedFromPause;
		RenderConfigView();
	}

	public void SetOptionsTabGraphics() {
		GraphicsTab.SetActive(true);
		InputTab.SetActive(false);
		AudioTab.SetActive(false);
		GraphicsTabButtonImage.overrideSprite = OptionsTabHilited;
		InputTabButtonImage.overrideSprite = OptionsTabDehilited;
		AudioTabButtonImage.overrideSprite = OptionsTabDehilited;
		UpdateConfigTabTextColor();
	}

	public void SetOptionsTabInput() {
		GraphicsTab.SetActive(false);
		InputTab.SetActive(true);
		AudioTab.SetActive(false);
		GraphicsTabButtonImage.overrideSprite = OptionsTabDehilited;
		InputTabButtonImage.overrideSprite = OptionsTabHilited;
		AudioTabButtonImage.overrideSprite = OptionsTabDehilited;
		UpdateConfigTabTextColor();
	}

	public void SetOptionsTabAudio() {
		GraphicsTab.SetActive(false);
		InputTab.SetActive(false);
		AudioTab.SetActive(true);
		GraphicsTabButtonImage.overrideSprite = OptionsTabDehilited;
		InputTabButtonImage.overrideSprite = OptionsTabDehilited;
		AudioTabButtonImage.overrideSprite = OptionsTabHilited;
		UpdateConfigTabTextColor();
	}

	void UpdateConfigTabTextColor() {
		if (GraphicsTab.activeInHierarchy) {
			GraphicsTabButtonText.color = Const.a.ssYellowText;
			InputTabButtonText.color = Const.a.ssGreenText;
			AudioTabButtonText.color = Const.a.ssGreenText;
		} else if (InputTab.activeInHierarchy) {
			GraphicsTabButtonText.color = Const.a.ssGreenText;
			InputTabButtonText.color = Const.a.ssYellowText;
			AudioTabButtonText.color = Const.a.ssGreenText;
		} else if (AudioTab.activeInHierarchy) {
			GraphicsTabButtonText.color = Const.a.ssGreenText;
			InputTabButtonText.color = Const.a.ssGreenText;
			AudioTabButtonText.color = Const.a.ssYellowText;
		}
	}

	public IEnumerator RenderConfigViewDelay() {
		yield return null;
		if (!optionsPage.activeInHierarchy) yield break;
		if (!GraphicsTab.activeInHierarchy) yield break;

		configCamera.targetTexture.Release();
		configCamera.targetTexture.width = Screen.width;
		configCamera.targetTexture.height = Screen.height;
		Grayscale gsc = configCamera.gameObject.GetComponent<Grayscale>();
		Grayscale gscMain = Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<Grayscale>();
		if (gsc != null && gscMain != null) gsc.enabled = gscMain.enabled;
		
		UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion sao = configCamera.gameObject.GetComponent<UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion>();
		if (sao != null) sao.enabled = Const.a.GraphicsSSAO;
		SEGI sega = configCamera.gameObject.GetComponent<SEGI>();
		if (sega != null) sega.enabled = Const.a.GraphicsSEGI;
		configCamera.Render();
		if (Const.a.GraphicsSEGI) {
			yield return null;
			configCamera.Render();
			yield return null;
			configCamera.Render();
			yield return null;
			configCamera.Render();
			yield return null;
			configCamera.Render();
		}
	}

	public void RenderConfigView() {
		StartCoroutine(RenderConfigViewDelay());
	}

	public void GoToNewGameSubmenu() {
		ResetPages();
		newgamePage.SetActive(true);
		newgameInputText.ActivateInputField();
		currentPage = Pages.np;
	}

	string GetSaveName(int index) {
		string savName = "sav" + index.ToString() + ".txt";
		string sP = Utils.SafePathCombine(Application.streamingAssetsPath,
											savName);

		string retval = "! unknown !";
		Utils.ConfirmExistsInStreamingAssetsMakeIfNot(savName);
		StreamReader sf = new StreamReader(sP);
		if (sf == null) {
			Debug.Log("GetSaveName error! sf null");
			return retval;
		}

		using (sf) {
			retval = sf.ReadLine();
			if (retval == null) {
				Debug.Log("GetSaveName error! retval null");
				return "! unknown !"; // just in case
			}

			sf.Close();
		}

		Debug.Log("GetSaveName retval: " + retval);
		return retval;
	}

	public void GoToLoadGameSubmenu (bool accessedFromPause) {
		ResetPages();
		loadPage.SetActive(true);
		currentPage = Pages.lp;
		returnToPause = accessedFromPause;
		for (int i=0;i<8;i++) {
			loadButtonText[i].text = GetSaveName(i);
		}	
	}

	public void GoToSaveGameSubmenu (bool accessedFromPause) {
		ResetPages();
		savePage.SetActive(true);
		currentPage = Pages.sv;
		returnToPause = accessedFromPause;
		for (int i=0;i<8;i++) {
			saveButtonText[i].text = GetSaveName(i);
		}	
	}

	public void SaveGameEntry (int index) {
		currentSaveSlot = index;
		typingSaveGame = true;
		saveNameInput[index].SetActive(true);
		saveNamePlaceholder[index].SetActive(true);
		saveButtonText[index].text = System.String.Empty;
		tempSaveNameHolder = saveButtonText[index].text;
		saveButtonText[index].text = System.String.Empty;
		saveNameInputField[index].ActivateInputField();
	}

	public void SaveQuickSaveButton () {
		SaveGame(7,"quicksave");
	}

	public void SaveGame(int index,string savename) {
		Const.a.StartSave(index,savename);
		Const.sprint(Const.a.stringTable[28] + index.ToString() + "!",Const.a.player1);
		PauseScript.a.EnablePauseUI();
		MouseCursor.a.mainCamera.enabled = true;
		this.gameObject.SetActive(false);
	}

	public void LoadGame(int index) {
		if (loadButtonText[index].text == "- unused -"
			|| loadButtonText[index].text == "- unused quicksave -") {
			Const.sprint("No data to load.");
		} else Const.a.Load(index,false);
	}

	public void GoBack () {
		if (typingSaveGame) {
			saveNameInput[currentSaveSlot].SetActive(false);
			saveNamePlaceholder[currentSaveSlot].SetActive(false);
			typingSaveGame = false;
			loadButtonText[currentSaveSlot].text = tempSaveNameHolder;
			currentSaveSlot = -1;
			return;
		}

		if (returnToPause) {
			PauseScript.a.ExitSaveDialog();
			ResetPages();
			MouseCursor.a.mainCamera.enabled = true;
			returnToPause = false;
			this.gameObject.SetActive(false);
			return;
		}

		if (currentPage == Pages.sv) {
			GoToFrontPage();
			return;
		}

		// Go Back to front page
		if (currentPage == Pages.sp || currentPage == Pages.mp || currentPage == Pages.op) {
			GoToFrontPage();
			return;
		}

		// Go Back to singlepayer page
		if (currentPage == Pages.np || currentPage == Pages.lp || currentPage == Pages.cd) {
			if (currentPage == Pages.cd) {
				BackGroundMusic.clip = Music.a.titleMusic;
				if (gameObject.activeSelf && dataFound) BackGroundMusic.Play();
			}
			GoToSingleplayerSubmenu();
			return;
		}
	}

	public void PathSearch() {
		// Open dialogue to search for path to C:\SHOCK\RES\DATA
			StartCoroutine(ShowSelectPathCoroutine());
	}

	IEnumerator ShowSelectPathCoroutine () {
		// Show a select path dialog and wait for a response from user
		// Path folder: folder, Allow multiple selection: false
		// Initial path: default (Documents), Title: "Load File", submit button
		// text: "Load"
		yield return FileBrowser.WaitForLoadDialog(true,false,
												   System.String.Empty,
												   "Select Path","Select");
		// Dialog is closed
		// Print whether the user has selected a folder path or cancelled the
		// operation (FileBrowser.Success).
		if (FileBrowser.Success) {
			// Set the folder path only, e.g. C:\SHOCK\RES\DATA
			dataPathInputText.text = FileBrowser.Result[0];
		}
	}

	// Linked in Inspector to dataPathInputText.
	public void CopyFromPath() {
		// Copy CITALOG.RES and CITBARK.RES from data path if they exist
		string alogPath,barkPath;
		alogPath = Utils.SafePathCombine(dataPathInputText.text,"CITALOG.RES");
		barkPath = Utils.SafePathCombine(dataPathInputText.text,"CITBARK.RES");

		// Must have both to get audio logs and SHODAN barks.
		if (File.Exists(alogPath) && File.Exists(barkPath)) {
			dataFound = true;
			string alogStrmPath,barkStrmPath;
			if (Application.platform == RuntimePlatform.Android) {
				alogStrmPath =
					Utils.SafePathCombine(Application.persistentDataPath,
									      "CITALOG.RES");
				barkStrmPath =
					Utils.SafePathCombine(Application.persistentDataPath,
									      "CITBARK.RES");
			} else {
				alogStrmPath =
					Utils.SafePathCombine(Application.streamingAssetsPath,
									     "CITBARK.RES");
				barkStrmPath =
					Utils.SafePathCombine(Application.streamingAssetsPath,
									      "CITBARK.RES");
			}

			// Set overwrite to true in case we have 1 and not the other.
			// from, to
			File.Copy(alogPath,alogStrmPath,true);
			File.Copy(barkPath,barkStrmPath,true);
		}
		StartCoroutine(CopyPathCheck());
	}

	IEnumerator CopyPathCheck() {
		if (dataFound) {
			SuccessBanner.SetActive(true);
			CouldNotFindDialogue.SetActive(false);
			Config.SetVolume();
			yield return new WaitForSeconds(0.5f);

			GoToFrontPage();
			CheckAndPlayIntro();
		} else {
			CouldNotFindDialogue.SetActive(false);
			FailureBanner.SetActive(true);
			yield return new WaitForSeconds(2f);
			
			FailureBanner.SetActive(false);
			CouldNotFindDialogue.SetActive(true);
		}
	}

	public void CloseDataFileNotification() {
		// Close data file notification without finding sound files CITALOG.RES and CITBARK.RES from data path
		CouldNotFindDialogue.SetActive(false);
		Config.SetVolume(); // probably not needed here, but just in case
		GoToFrontPage();
		CheckAndPlayIntro();
	}

	public void PlayDeathVideo() {
		DeathVideoContainer.SetActive(true);
		DeathVideo.SetActive(true);
		deathPlayer.url = Application.streamingAssetsPath + "/death.webm";
		deathPlayer.Play();
		deathPlayer.SetDirectAudioMute(0,true);
		deathVideoText1.text = Const.a.stringTable[628];
		deathVideoText2.text = Const.a.stringTable[629];
		Utils.Activate(deathVideoTextGO1);
		Utils.Deactivate(deathVideoTextGO2);
		if (Const.a.DynamicMusic) {
			BackGroundMusic.clip = Music.a.levelMusicDeath[LevelManager.a.currentLevel];
		} else {
			BackGroundMusic.clip = Music.a.levelMusicLooped[16];
		}

		gameObject.SetActive(true);
		if (dataFound) BackGroundMusic.Play();
		vidFinished = Time.time + deathvidLength;
		vidStartTime = Time.time;
	}

	public void PlayIntro() {
		Const.a.WriteDatForIntroPlayed(false);
		IntroVideoContainer.SetActive(true);
		IntroVideo.SetActive(true);
		introPlayer.url = Application.streamingAssetsPath + "/intro.webm";
		introPlayer.Play();
		if (!dataFound) introPlayer.SetDirectAudioMute(0,true);
		else introPlayer.SetDirectAudioMute(0,false);

		inCutscene = true;
		BackGroundMusic.Stop();
		vidFinished = Time.time + vidLength;
		vidStartTime = Time.time;

		// Setup text.
		introVideoText1.text = Const.a.stringTable[613];
		introVideoText2.text = Const.a.stringTable[614];
		introVideoText3.text = Const.a.stringTable[615];
		introVideoText4.text = Const.a.stringTable[616];
		introVideoText5.text = Const.a.stringTable[617];
		introVideoText6.text = Const.a.stringTable[618];
		introVideoText7.text = Const.a.stringTable[619];
		introVideoText8.text = Const.a.stringTable[620];
		introVideoText9.text = Const.a.stringTable[621];
		introVideoText10.text = Const.a.stringTable[622];
		introVideoText11.text = Const.a.stringTable[623];
		introVideoText12.text = Const.a.stringTable[624];
		introVideoText13.text = Const.a.stringTable[625];
		introVideoText14.text = Const.a.stringTable[626];
		introVideoText15.text = Const.a.stringTable[627];
		Utils.Activate(introVideoTextGO1);
		Utils.Deactivate(introVideoTextGO2);
		Utils.Deactivate(introVideoTextGO3);
		Utils.Deactivate(introVideoTextGO4);
		Utils.Deactivate(introVideoTextGO5);
		Utils.Deactivate(introVideoTextGO6);
		Utils.Deactivate(introVideoTextGO7);
		Utils.Deactivate(introVideoTextGO8);
		Utils.Deactivate(introVideoTextGO9);
		Utils.Deactivate(introVideoTextGO10);
		Utils.Deactivate(introVideoTextGO11);
		Utils.Deactivate(introVideoTextGO12);
		Utils.Deactivate(introVideoTextGO13);
		Utils.Deactivate(introVideoTextGO14);
		Utils.Deactivate(introVideoTextGO15);
	}

	public void PlayCredits () {
		ResetPages();
		creditsPage.SetActive(true);
		currentPage = Pages.cd;
		if (Const.a.DynamicMusic) {
			BackGroundMusic.clip = Music.a.creditsMusic;
		} else {
			BackGroundMusic.clip = Music.a.levelMusicLooped[17];
		}

		if (gameObject.activeSelf && dataFound) BackGroundMusic.Play();
	}

	public void SetConfigPreset(int index) {
		presetQuestionValue = index;

		if (presetQuestionValue == 1)  presetQuestionText.text = presetQuestionLegacy;
		else presetQuestionText.text = presetQuestionDefault;

		PresetConfirmDialog.SetActive(true);
	}

	public void CancelPresetSet() {
		presetQuestionValue = -1;
		PresetConfirmDialog.SetActive(false);
	}
	
	public void ApplyPreset() {
		switch (presetQuestionValue) {
			case 0: // Default
				Const.a.InputCodeSettings[0] = 22; // Forward = w
				Const.a.InputCodeSettings[1] = 0; // Strafe Left = a
				Const.a.InputCodeSettings[2] = 18; // Backpedal = s
				Const.a.InputCodeSettings[3] = 3; // Strafe Right = d
				Const.a.InputCodeSettings[4] = 87; // Jump = space
				Const.a.InputCodeSettings[5] = 2; // Crouch = c
				Const.a.InputCodeSettings[6] = 23; // Prone = x
				Const.a.InputCodeSettings[7] = 16; // Lean Left = q
				Const.a.InputCodeSettings[8] = 4; // Lean Right = e
				Const.a.InputCodeSettings[9] = 46; // Sprint = left shift
				Const.a.InputCodeSettings[10] = 139; // Toggle Sprint = capslock
				Const.a.InputCodeSettings[11] = 38; // Turn Left = left
				Const.a.InputCodeSettings[12] = 39; // Turn Right = right
				Const.a.InputCodeSettings[13] = 36; // Look Up = up
				Const.a.InputCodeSettings[14] = 37; // Look Down = down
				Const.a.InputCodeSettings[15] = 20; // Recent Log = u
				Const.a.InputCodeSettings[16] = 26; // Biomonitor = 1
				Const.a.InputCodeSettings[17] = 27; // Sensaround = 2
				Const.a.InputCodeSettings[18] = 28; // Lantern = 3
				Const.a.InputCodeSettings[19] = 29; // Shield = 4
				Const.a.InputCodeSettings[20] = 30; // Infrared = 5
				Const.a.InputCodeSettings[21] = 31; // Email = 6
				Const.a.InputCodeSettings[22] = 32; // Booster = 7
				Const.a.InputCodeSettings[23] = 33; // Jumpjets = 8
				Const.a.InputCodeSettings[24] = 53; // Attack = mouse 0
				Const.a.InputCodeSettings[25] = 54; // Use = mouse 1
				Const.a.InputCodeSettings[26] = 86; // Menu/Back = escape
				Const.a.InputCodeSettings[27] = 84; // Toggle Mode = tab
				Const.a.InputCodeSettings[28] = 17; // Reload = r
				Const.a.InputCodeSettings[29] = 153; // Weapon + = mwheel up
				Const.a.InputCodeSettings[30] = 154; // Weapon - = mwheel dn
				Const.a.InputCodeSettings[31] = 6; // Grenade = g
				Const.a.InputCodeSettings[32] = 19; // Grenade + = t
				Const.a.InputCodeSettings[33] = 1; // Grenade - = b
				Const.a.InputCodeSettings[34] = 21; // Ammo Type = v
				Const.a.InputCodeSettings[35] = 109; // Unused
				Const.a.InputCodeSettings[36] = 9; // Patch Use = j
				Const.a.InputCodeSettings[37] = 8; // Patch + = i
				Const.a.InputCodeSettings[38] = 133; // Patch - = ,
				Const.a.InputCodeSettings[39] = 12; // Full Map = m
				Const.a.NoShootMode = false;
				Const.a.InputQuickReloadWeapons = true;
				Const.a.InputQuickItemPickup = false;
				break;
			case 1: // Legacy SS1
				Const.a.InputCodeSettings[0] = 18; // Forward = s
				Const.a.InputCodeSettings[1] = 25; // Strafe Left = z
				Const.a.InputCodeSettings[2] = 23; // Backpedal = x
				Const.a.InputCodeSettings[3] = 2; // Strafe Right = c
				Const.a.InputCodeSettings[4] = 87; // Jump = space
				Const.a.InputCodeSettings[5] = 6; // Crouch = g
				Const.a.InputCodeSettings[6] = 1; // Prone = b
				Const.a.InputCodeSettings[7] = 16; // Lean Left = q
				Const.a.InputCodeSettings[8] = 4; // Lean Right = e
				Const.a.InputCodeSettings[9] = 46; // Sprint = left shift
				Const.a.InputCodeSettings[10] = 139; // Toggle Sprint = capslock
				Const.a.InputCodeSettings[11] = 0; // Turn Left = a
				Const.a.InputCodeSettings[12] = 3; // Turn Right = d
				Const.a.InputCodeSettings[13] = 17; // Look Up = r
				Const.a.InputCodeSettings[14] = 21; // Look Down = v
				Const.a.InputCodeSettings[15] = 20; // Recent Log = p
				Const.a.InputCodeSettings[16] = 26; // Biomonitor = 1
				Const.a.InputCodeSettings[17] = 28; // Sensaround = 3
				Const.a.InputCodeSettings[18] = 29; // Lantern = 4
				Const.a.InputCodeSettings[19] = 30; // Shield = 5
				Const.a.InputCodeSettings[20] = 31; // Infrared = 6
				Const.a.InputCodeSettings[21] = 33; // Email = 8
				Const.a.InputCodeSettings[22] = 34; // Booster = 9
				Const.a.InputCodeSettings[23] = 35; // Jumpjets = 0
				Const.a.InputCodeSettings[24] = 54; // Use = mouse 1
				Const.a.InputCodeSettings[25] = 53; // Attack = mouse 0
				Const.a.InputCodeSettings[26] = 86; // Menu/Back = escape
				Const.a.InputCodeSettings[27] = 84; // Toggle Mode = tab
				Const.a.InputCodeSettings[28] = 19; // Reload = t
				Const.a.InputCodeSettings[29] = 153; // Weapon + = mwheel up
				Const.a.InputCodeSettings[30] = 154; // Weapon - = mwheel dn
				Const.a.InputCodeSettings[31] = 7; // Grenade = h
				Const.a.InputCodeSettings[32] = 24; // Grenade + = y
				Const.a.InputCodeSettings[33] = 13; // Grenade - = n
				Const.a.InputCodeSettings[34] = 10; // Ammo Type = k
				Const.a.InputCodeSettings[35] = 109; // Unused
				Const.a.InputCodeSettings[36] = 9; // Patch Use = j
				Const.a.InputCodeSettings[37] = 8; // Patch + = i
				Const.a.InputCodeSettings[38] = 133; // Patch - = ,
				Const.a.InputCodeSettings[39] = 12; // Full Map = m
				Const.a.NoShootMode = true;
				Const.a.InputQuickReloadWeapons = false;
				Const.a.InputQuickItemPickup = false;
				break;
		}
		presetQuestionValue = -1;	
		Config.WriteConfig(); // Save config.  Always set to autosave.
		for (int i=0;i<keybindButtons.Length;i++) {
			keybindButtons[i].UpdateText();
		}
		ctInvertUpDnLook.AlignWithConfigFile();
		ctInvertUpDnCyberLook.AlignWithConfigFile();
		ctInvertInventoryCyc.AlignWithConfigFile();
		ctQuickItemPickUp.AlignWithConfigFile();
		ctQuickReload.AlignWithConfigFile();
		ctNoShootMode.AlignWithConfigFile();
		CancelPresetSet();
	}

	public void Quit () {
		StartCoroutine(quitFunction());
	}

	IEnumerator quitFunction () {
		BackGroundMusic.Stop();
		Const.a.WriteDatForIntroPlayed(false);
		saltTheFries.SetActive(true);
		yield return new WaitForSeconds(0.8f);
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#endif

		Application.Quit();
	}
}
