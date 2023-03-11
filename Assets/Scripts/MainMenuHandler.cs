using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using SimpleFileBrowser;

public class MainMenuHandler : MonoBehaviour {
	public GameObject Button1;
	public GameObject Button2;
	public GameObject Button3;
	public GameObject Button4;
	public GameObject startFXObject;
	public AudioClip StartGameSFX;
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
	public AudioClip titleMusic;
	public AudioClip creditsMusic;
	public CreditsScroll credScrollManager;
	public GameObject IntroVideo;
	public GameObject IntroVideoContainer;
	public bool inCutscene = false;
	public AudioSource introSFX;
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

	[HideInInspector] public bool returnToPause = false;
	[HideInInspector] public bool dataFound = false;
	private enum Pages : byte {fp,sp,mp,np,lp,op,sv,cd};
	private Pages currentPage;
	private AudioSource StartSFX;
	private AudioSource BackGroundMusic;
	private bool typingSaveGame = false;
	private string tempSaveNameHolder;
	private int presetQuestionValue = -1;
	private LoadPageGetSaveNames lpgsn_load;
	private LoadPageGetSaveNames lpgsn_save;

	// For NewGameGraph duplicate of BiomonitorGraph but faked out
	private float ecgValue = 0;
	private float beatShift;
	public float beatThresh = 0.1f;
	public float beatVariation = 0.05f;
	private float beatFinished;
	public float freq = 35f;
	public float beatTime = 5f;

	void Awake() {
		a = this;
		StartSFX = startFXObject.GetComponent<AudioSource>();
		BackGroundMusic = GetComponent<AudioSource>();
		BackGroundMusic.ignoreListenerPause = true; // Play when paused.
		ResetPages();
		dataFound = false;
		inCutscene = false;
		FileBrowser.SetFilters(false,new FileBrowser.Filter("SHOCK RES Files",
															".RES", ".res"));
		FileBrowser.SetDefaultFilter( ".RES" );
		Config.SetVolume();
		StartCoroutine(CheckDataFiles());
		string indn = Utils.SafePathCombine(Application.streamingAssetsPath,
											"introdone.dat");
		if (System.IO.File.Exists(indn)) {
			IntroVideo.SetActive(false);	
			IntroVideoContainer.SetActive(false);
		} else {
			System.IO.File.Create(indn);
		}

		lpgsn_load = loadPage.GetComponent<LoadPageGetSaveNames>();
		lpgsn_save = savePage.GetComponent<LoadPageGetSaveNames>();
		beatFinished = Time.time;
	}

	// Improve menu performance.
	void DisableCameraDuringMenu() {
		if (MouseLookScript.a == null) return;

		MouseLookScript.a.playerCamera.enabled = false; 
	}

	void OnEnable() { DisableCameraDuringMenu(); }

	void OnDisable() { DisableCameraDuringMenu(); }

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
			IntroVideo.SetActive(true);	
		} else {
			// Fake like we are checking for the files to be there.
			// It's fake because we already did it and it is instant.
			// This is just to show the user that we did in fact look.
			InitialDisplay.SetActive(true);
			yield return new WaitForSeconds(0.3f);

			// OK, now show that we didn't find them
			InitialDisplay.SetActive(false);
			CouldNotFindDialogue.SetActive(true);
		}
	}

	void LeaveIntroCutscene() {
		inCutscene = false;
		IntroVideo.SetActive(false);
		IntroVideoContainer.SetActive(false);
		Const.a.WriteDatForIntroPlayed(false);
		if (gameObject.activeSelf) BackGroundMusic.Play();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)
			|| Input.GetMouseButtonDown(1)
			|| Input.GetKeyDown(KeyCode.Escape)
			|| Input.GetKeyDown(KeyCode.JoystickButton0)
			|| Input.GetKeyDown(KeyCode.JoystickButton1)
			|| Input.anyKey) {
			if ((inCutscene || IntroVideoContainer.activeSelf)
				&& !CouldNotFindDialogue.activeSelf) {
				LeaveIntroCutscene();
				return;
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1)) { // Escape/back button listener
			if (savePage.activeInHierarchy && !newgamePage.activeInHierarchy) {
				if (currentSaveSlot > 0
					&& currentSaveSlot < saveNameInputField.Length) {
					InputField infld = saveNameInputField[currentSaveSlot].GetComponentInChildren<InputField>();
					if (infld != null) infld.DeactivateInputField();
				}
				currentSaveSlot = -1;
				typingSaveGame = false;
				returnToPause = true;
			}

			GoBack();
			return;
		}

		if (!IntroVideoContainer.activeSelf) {
			if (!BackGroundMusic.isPlaying
				&& !saltTheFries.activeInHierarchy
				&& gameObject.activeSelf) {
				BackGroundMusic.Play();
			}
		}

		if (   (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.P))
			|| (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKey(KeyCode.P))) {
			StartGame(true);
			return;
		}

		if (typingSaveGame && (Input.GetKeyUp(KeyCode.Return)
							   || Input.GetKeyUp(KeyCode.KeypadEnter)
							   || Input.GetKeyDown(KeyCode.JoystickButton0))
			&& savePage.activeInHierarchy
			&& !newgamePage.activeInHierarchy) {
			if (currentSaveSlot < 0) return;

			string sname = saveNameInputField[currentSaveSlot].GetComponentInChildren<InputField>().text;
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

		NewGameGraphUpdate();
	}

	public void StartGame (bool isNew) {
		Utils.PlayOneShotSavable(StartSFX,StartGameSFX);
		if (isNew) {
			string pname = newgamePage.GetComponentInChildren<InputField>().text;
			if (string.IsNullOrWhiteSpace(pname)) pname = "Hacker";
			Const.a.playerName = pname;
			Const.a.difficultyCombat = combat.difficultySetting;
			Const.a.difficultyMission = mission.difficultySetting;
			Const.a.difficultyPuzzle = puzzle.difficultySetting;
			Const.a.difficultyCyber = cyber.difficultySetting;
			Const.a.NewGame();
		}

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

	public void GoToFrontPage () {
		ResetPages();
		frontPage.SetActive(true);
		currentPage = Pages.fp;
	}

	public void GoToSingleplayerSubmenu () {
		ResetPages();
		singleplayerPage.SetActive(true);
		currentPage = Pages.sp;
	}

	public void GoToMultiplayerSubmenu () {
		ResetPages();
		multiplayerPage.SetActive(true);
		currentPage = Pages.mp;
	}

	public void GoToOptionsSubmenu (bool accessedFromPause) {
		ResetPages();
		if (accessedFromPause) IntroVideoContainer.SetActive(false);
		optionsPage.SetActive(true);
		currentPage = Pages.op;
		returnToPause = accessedFromPause;
	}

	public void GoToNewGameSubmenu () {
		ResetPages();
		newgamePage.SetActive(true);
		newgameInputText.ActivateInputField();
		currentPage = Pages.np;
	}

	void NewGameGraphUpdate() {
		if (currentPage != Pages.np) return;
		if (NewGameGraphSystem.a == null) return;

		// Energy Usage
		NewGameGraphSystem.a.Graph(0,0f); // Take percentage of max JPM drain per second (449) and apply it to a scale of �1.0
		//NewGameGraphSystem.a.graphs[1].tex0 = (Texture2D)NewGameGraphSystem.a.OutputTexture.texture;
		//NewGameGraphSystem.a.graphs[1].tex1 = new Texture2D(NewGameGraphSystem.a.graphs[1].tex0.width, NewGameGraphSystem.a.graphs[1].tex0.height);
		//NewGameGraphSystem.a.graphs[1].texFlipFlop = true;

		// Chi Wave
		NewGameGraphSystem.a.Graph(1, Mathf.Sin(Time.time * beatTime * 2f) + UnityEngine.Random.Range(-0.3f,0.3f));
		//NewGameGraphSystem.a.graphs[2].tex0 = (Texture2D)NewGameGraphSystem.a.OutputTexture.texture;
		//NewGameGraphSystem.a.graphs[2].tex1 = new Texture2D(NewGameGraphSystem.a.graphs[2].tex0.width, NewGameGraphSystem.a.graphs[2].tex0.height);
		//NewGameGraphSystem.a.graphs[2].texFlipFlop = true;

		// ECG
		if (beatFinished < Time.time) beatFinished = Time.time + beatTime;

		// Create shifted sine wave for heart beat.
		beatShift = (beatFinished - Time.time)/beatTime;
		if (beatShift > 0.8f) ecgValue = Mathf.Sin(beatShift * freq);
		else ecgValue = 0;

		 // Inject variation when beating
		if (ecgValue > beatThresh || ecgValue < (beatThresh * -1f)) {
			ecgValue += UnityEngine.Random.Range((beatVariation * -1f),beatVariation);
		}
		NewGameGraphSystem.a.Graph(2, ecgValue);
	}

	string GetSaveName(int index) {
		string savName = "sav" + index.ToString() + ".txt";
		string sP = Utils.SafePathCombine(Application.streamingAssetsPath,
											savName);

		string retval = "! unknown !";
		Const.a.ConfirmExistsInStreamingAssetsMakeIfNot(savName);
		StreamReader sf = new StreamReader(sP);
		if (sf == null) return retval;

		using (sf) {
			retval = sf.ReadLine();
			if (retval == null) return "! unknown !"; // just in case

			sf.Close();
		}

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
		tempSaveNameHolder = lpgsn_save.loadButtonText[index].text;
		lpgsn_save.loadButtonText[index].text = System.String.Empty;
		saveNameInputField[index].ActivateInputField();
	}

	public void SaveQuickSaveButton () {
		SaveGame(7,"quicksave");
	}

	public void SaveGame(int index,string savename) {
		Const.a.StartSave(index,savename);
		Const.sprint(Const.a.stringTable[28] + index.ToString() + "!",Const.a.player1);
		PauseScript.a.EnablePauseUI();
		this.gameObject.SetActive(false);
	}

	public void LoadGame(int index) {
		if (lpgsn_load.loadButtonText[index].text == "- unused -"
			|| lpgsn_load.loadButtonText[index].text == "- unused quicksave -") {
			Const.sprint("No data to load.");
		} else Const.a.Load(index,false);
	}

	public void GoBack () {
		if (typingSaveGame) {
			saveNameInput[currentSaveSlot].SetActive(false);
			saveNamePlaceholder[currentSaveSlot].SetActive(false);
			typingSaveGame = false;
			lpgsn_save.loadButtonText[currentSaveSlot].text = tempSaveNameHolder;
			currentSaveSlot = -1;
			return;
		}

		if (returnToPause) {
			PauseScript.a.EnablePauseUI();
			ResetPages();
			MouseCursor.a.mainCamera.enabled = true;
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
				BackGroundMusic.clip = titleMusic;
				if (gameObject.activeSelf) BackGroundMusic.Play();
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
		// Initial path: default (Documents), Title: "Load File", submit button text: "Load"
		yield return FileBrowser.WaitForLoadDialog(true, false, System.String.Empty, "Select Path", "Select");

		// Dialog is closed
		// Print whether the user has selected a folder path or cancelled the operation (FileBrowser.Success)
		//Debug.Log(FileBrowser.Success);

		if (FileBrowser.Success) {
			dataPathInputText.text = FileBrowser.Result[0]; // Set the folder path only, e.g. C:\SHOCK\RES\DATA
			//Debug.Log("Found filepath for .RES files: " + FileBrowser.Result[0]);
		}
	}

	// Linked in Inspector to dataPathInputText.
	public void CopyFromPath() {
		// Copy CITALOG.RES and CITBARK.RES from data path if they exist
		string alogPath = Utils.SafePathCombine(dataPathInputText.text,
												"CITALOG.RES");

		string barkPath = Utils.SafePathCombine(dataPathInputText.text,
												"CITBARK.RES");

		// Must have both to get audio logs and SHODAN barks.
		if (File.Exists(alogPath) && File.Exists(barkPath)) {
			dataFound = true;
			string alogStrmPath =
				Utils.SafePathCombine(Application.streamingAssetsPath,
									  "CITALOG.RES");
			string barkStrmPath =
				Utils.SafePathCombine(Application.streamingAssetsPath,
									  "CITBARK.RES");

			// Set overwrite to true in case we have 1 and not the other.
			// from, to
			File.Copy(alogPath,alogStrmPath,true);
			File.Copy(barkPath,barkStrmPath,true);
		}
		StartCoroutine(CopyPathCheck());
	}

	IEnumerator CopyPathCheck () {
		if (dataFound) {
			SuccessBanner.SetActive(true);
			CouldNotFindDialogue.SetActive(false);
			Config.SetVolume();
			yield return new WaitForSeconds(0.5f);
			GoToFrontPage();
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
		PlayIntro();
	}

	public void PlayIntro() {
		Const.a.WriteDatForIntroPlayed(false);
		IntroVideoContainer.SetActive(true);
		IntroVideo.SetActive(true);
		inCutscene = true;
		BackGroundMusic.Stop();
	}

	public void PlayCredits () {
		ResetPages();
		creditsPage.SetActive(true);
		currentPage = Pages.cd;
		BackGroundMusic.clip = creditsMusic;
		if (gameObject.activeSelf) BackGroundMusic.Play();
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
