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
	private enum Pages : byte {fp,sp,mp,np,lp,op,sv,cd};
	private Pages currentPage;
	private AudioSource StartSFX;
	private AudioSource BackGroundMusic;
	public AudioClip titleMusic;
	public AudioClip creditsMusic;
	[HideInInspector]
	public bool returnToPause = false;
	private bool typingSaveGame = false;
	private string tempSaveNameHolder;
	public CreditsScroll credScrollManager;
	[HideInInspector]
	public bool dataFound = false;
	public GameObject IntroVideo;
	public GameObject IntroVideoContainer;
	public bool inCutscene = false;
	public AudioSource introSFX;
	public GameObject PresetConfirmDialog;
	public Text presetQuestionText;
	public string presetQuestionDefault = "RESET ALL KEYS TO DEFAULT?";
	public string presetQuestionLegacy = "CHANGE ALL KEYS TO LEGACY PRESET?";
	private int presetQuestionValue = -1;
	public static MainMenuHandler a;
	public ConfigKeybindButton[] keybindButtons;

	void Awake() {
		a = this;
		StartSFX = startFXObject.GetComponent<AudioSource>();
		BackGroundMusic = GetComponent<AudioSource>();
		BackGroundMusic.ignoreListenerPause = true; // Play when paused.
		ResetPages();
		dataFound = false;
		inCutscene = false;
		FileBrowser.SetFilters(false,new FileBrowser.Filter("SHOCK RES Files", ".RES", ".res"));
		FileBrowser.SetDefaultFilter( ".RES" );
		Config.SetVolume();
		StartCoroutine(CheckDataFiles());
		if (System.IO.File.Exists(Application.dataPath + "/StreamingAssets/introdone.dat")) {
			IntroVideo.SetActive(false);	
			IntroVideoContainer.SetActive(false);
		} else {
			System.IO.File.Create(Application.dataPath + "/StreamingAssets/introdone.dat");
		}
	}

	IEnumerator CheckDataFiles () {
		BackGroundMusic.Stop();
		bool found = (System.IO.File.Exists(Application.dataPath + "/StreamingAssets/CITALOG.RES") && System.IO.File.Exists(Application.dataPath + "/StreamingAssets/CITBARK.RES"));
		if (found) {
			// Go right on into the game, all good here.
			dataFound = true;
			Config.SetVolume();
			GoToFrontPage();
			IntroVideo.SetActive(true);	
		} else {
			// Fake like we are checking for the files to be there for a quick bit of time
			InitialDisplay.SetActive(true);
			yield return new WaitForSeconds(0.3f);
			// OK, now show that we didn't find them
			InitialDisplay.SetActive(false);
			CouldNotFindDialogue.SetActive(true);
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) { // Escape/back button listener
			if (savePage.activeInHierarchy && !newgamePage.activeInHierarchy) {
				currentSaveSlot = -1;
				typingSaveGame = false;
				saveNameInputField[currentSaveSlot].GetComponentInChildren<InputField>().DeactivateInputField();
			}
			if (inCutscene && !CouldNotFindDialogue.activeSelf) {
				inCutscene = false;
				IntroVideo.SetActive(false);
				IntroVideoContainer.SetActive(false);
				Const.a.WriteDatForIntroPlayed(false);
				if (gameObject.activeSelf) BackGroundMusic.Play();
			}
			GoBack();
		}

		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.anyKey) {
			if (IntroVideoContainer.activeSelf && !CouldNotFindDialogue.activeSelf) {
				inCutscene = false;
				IntroVideo.SetActive(false);
				IntroVideoContainer.SetActive(false);
				Const.a.WriteDatForIntroPlayed(false);
				if (gameObject.activeSelf) BackGroundMusic.Play();
			}
		}

		if (!IntroVideoContainer.activeSelf) {
			if (!BackGroundMusic.isPlaying && !saltTheFries.activeInHierarchy && gameObject.activeSelf) BackGroundMusic.Play();
		}
		if ( (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.P)) || (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKey(KeyCode.P)) ) StartGame(true);
		if (typingSaveGame && (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter)) && savePage.activeInHierarchy && !newgamePage.activeInHierarchy) {
			if (currentSaveSlot < 0) return;
			string sname = saveNameInputField[currentSaveSlot].GetComponentInChildren<InputField>().text;
			if (!string.IsNullOrEmpty(sname)) {
				//Debug.Log("Calling savegame with name of: " + sname);
				SaveGame(currentSaveSlot,sname);
				saveNameInput[currentSaveSlot].SetActive(false);
				saveNamePlaceholder[currentSaveSlot].SetActive(false);
				saveButtonText[currentSaveSlot].text = sname;
				typingSaveGame = false;
				currentSaveSlot = -1;
			} else {
				GoBack();
			}
		}
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
			Const.a.Load(-1); // load default scene
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

	public void GoToLoadGameSubmenu (bool accessedFromPause) {
		ResetPages();
		loadPage.SetActive(true);
		currentPage = Pages.lp;
		returnToPause = accessedFromPause;

		string readline; // variable to hold each string read in from the file
		for (int i=0;i<8;i++) {
			StreamReader sf = new StreamReader(Application.streamingAssetsPath + "/sav"+i.ToString()+".txt");
			if (sf != null) {
				using (sf) {
					readline = sf.ReadLine();
					if (readline == null) break; // just in case
					loadButtonText[i].text = readline;
					sf.Close();
				}
			}
		}	
	}

	public void GoToSaveGameSubmenu (bool fromPause) {
		ResetPages();
		savePage.SetActive(true);
		currentPage = Pages.sv;
		returnToPause = fromPause;

		string readline; // variable to hold each string read in from the file
		for (int i=0;i<8;i++) {
			StreamReader sf = new StreamReader(Application.streamingAssetsPath + "/sav"+i.ToString()+".txt");
			if (sf != null) {
				using (sf) {
					readline = sf.ReadLine();
					if (readline == null) break; // just in case
					saveButtonText[i].text = readline;
					sf.Close();
				}
			}
		}	
	}

	public void SaveGameEntry (int index) {
		currentSaveSlot = index;
		typingSaveGame = true;
		saveNameInput[index].SetActive(true);
		saveNamePlaceholder[index].SetActive(true);
		saveButtonText[index].text = System.String.Empty;
		tempSaveNameHolder = savePage.GetComponent<LoadPageGetSaveNames>().loadButtonText[index].text;
		savePage.GetComponent<LoadPageGetSaveNames>().loadButtonText[index].text = System.String.Empty;
		saveNameInputField[index].ActivateInputField();
	}

	public void SaveQuickSaveButton () {
		SaveGame (7,"quicksave");
	}

	public void SaveGame (int index,string savename) {
		Const.a.StartSave(index,savename);
		Const.sprint(Const.a.stringTable[28] + index.ToString() + "!",Const.a.player1);
		PauseScript.a.EnablePauseUI();
		this.gameObject.SetActive(false);
	}

	public void LoadGame (int index) {
		if (loadPage.GetComponent<LoadPageGetSaveNames>().loadButtonText[index].text == "- unused -" || loadPage.GetComponent<LoadPageGetSaveNames>().loadButtonText[index].text == "- unused quicksave -") {
			Const.sprint("No data to load.");
		} else {
			if (Const.a.introNotPlayed) {
				StartGame(false);
			} else {
				Const.a.Load(index);
				StartGame(false);
			}
		}
	}

	public void GoBack () {
		if (typingSaveGame) {
			saveNameInput[currentSaveSlot].SetActive(false);
			saveNamePlaceholder[currentSaveSlot].SetActive(false);
			typingSaveGame = false;
			savePage.GetComponent<LoadPageGetSaveNames>().loadButtonText[currentSaveSlot].text = tempSaveNameHolder;
			currentSaveSlot = -1;
			return;
		}

		if (returnToPause) {
			PauseScript.a.EnablePauseUI();
			ResetPages();
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

	public void CopyFromPath() {
		// Copy CITALOG.RES and CITBARK.RES from data path if they exist
		if (System.IO.File.Exists(System.IO.Path.Combine(dataPathInputText.text,"CITALOG.RES")) && System.IO.File.Exists(System.IO.Path.Combine(dataPathInputText.text,"CITBARK.RES"))) {
			dataFound = true;
			System.IO.File.Copy(System.IO.Path.Combine(dataPathInputText.text,"CITALOG.RES"),Application.dataPath + "/StreamingAssets/CITALOG.RES",true); // Set to true in case we have 1 and not the other we can overwrite
			System.IO.File.Copy(System.IO.Path.Combine(dataPathInputText.text,"CITBARK.RES"),Application.dataPath + "/StreamingAssets/CITBARK.RES",true); // Set to true in case we have 1 and not the other we can overwrite
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
				break;
		}
		presetQuestionValue = -1;	
		Config.WriteConfig(); // Save config.  Always set to autosave.
		for (int i=0;i<keybindButtons.Length;i++) {
			keybindButtons[i].UpdateText();
		}
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
		#if UNITY_EDITOR_WIN
			UnityEditor.EditorApplication.isPlaying = false;
		#endif

		Application.Quit();
	}
}
