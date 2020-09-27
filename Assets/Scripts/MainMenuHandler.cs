using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using UnityStandardAssets.ImageEffects;
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
	public PauseScript pauseHandler;
	public InputField[] saveNameInputField;
	public GameObject[] saveNameInput;
	public GameObject[] saveNamePlaceholder;
	public Text[] saveButtonText;
	public Text[] loadButtonText;
	public int currentSaveSlot = -1;
	private enum Pages {fp,sp,mp,np,lp,op,sv,cd};
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

	void Awake () {
		StartSFX = startFXObject.GetComponent<AudioSource>();
		BackGroundMusic = GetComponent<AudioSource>();
		ResetPages();
		dataFound = false;
		FileBrowser.SetFilters(false,new FileBrowser.Filter("SHOCK RES Files", ".RES", ".res"));
		FileBrowser.SetDefaultFilter( ".RES" );
		Const.a.SetVolume();
		StartCoroutine(CheckDataFiles());
	}

	IEnumerator CheckDataFiles () {
		bool found = (System.IO.File.Exists(Application.dataPath + "/StreamingAssets/CITALOG.RES") && System.IO.File.Exists(Application.dataPath + "/StreamingAssets/CITBARK.RES"));
		if (found) {
			// go right on into the game, all good here
			dataFound = true;
			Const.a.SetVolume();
			GoToFrontPage();
		} else {
			// Fake like we are checking for the files to be there for a quick bit of time
			InitialDisplay.SetActive(true);
			yield return new WaitForSeconds(0.3f);
			// OK, now show that we didn't find them
			InitialDisplay.SetActive(false);
			CouldNotFindDialogue.SetActive(true);
		}
	}

	void Update () {
		// Escape/back button listener
		if (Input.GetKeyDown(KeyCode.Escape)) {
			GoBack();
		}

		//if (creditsPage.activeSelf) {
			//if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)){ //|| Input.anyKey) {
				//if (credScrollManager.stopped) {
				//	credScrollManager.Continue();
				//} else {
					//GoBack();
				//}
			//}
		//}

		if ( (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.P)) || (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKey(KeyCode.P)) ) {
			//Debug.Log("Skipping main menu. Debug cheat");
			StartGame(true);
		}

		if (Input.GetKeyUp(KeyCode.Return) && savePage.activeInHierarchy && !newgamePage.activeInHierarchy) {
			string sname = saveNameInputField[currentSaveSlot].GetComponentInChildren<InputField>().text;
			if (currentSaveSlot != -1 && !string.IsNullOrEmpty(sname)) {
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
		StartSFX.PlayOneShot(StartGameSFX);
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
		typingSaveGame = true;
		saveNameInput[index].SetActive(true);
		saveNamePlaceholder[index].SetActive(true);
		saveButtonText[index].text = System.String.Empty;
		tempSaveNameHolder = savePage.GetComponent<LoadPageGetSaveNames>().loadButtonText[index].text;
		savePage.GetComponent<LoadPageGetSaveNames>().loadButtonText[index].text = System.String.Empty;
		saveNameInputField[index].ActivateInputField();
		currentSaveSlot = index;
	}

	public void SaveQuickSaveButton () {
		SaveGame (7,"quicksave");
	}

	public void SaveGame (int index,string savename) {
		Const.a.StartSave(index,savename);
		Const.sprint(Const.a.stringTable[28] + index.ToString() + "!",Const.a.player1);
		pauseHandler.EnablePauseUI();
		this.gameObject.SetActive(false);
	}

	public void LoadGame (int index) {
		if (loadPage.GetComponent<LoadPageGetSaveNames>().loadButtonText[index].text == "- unused -") {
			Const.sprint("No data to load",Const.a.allPlayers);
		} else {
			if (Const.a.freshRun) {
				StartGame(false);
				Const.a.WriteDatForNewGame(false,false);
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
			if (Const.a.justSavedTimeStamp < Time.time) {
				pauseHandler.hardSaveQuit = false;
			}
			pauseHandler.EnablePauseUI();
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
				BackGroundMusic.Play();
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
		yield return FileBrowser.WaitForLoadDialog(true, false, null, "Select Path", "Select");

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
			Const.a.SetVolume();
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
		Const.a.SetVolume(); // probably not needed here, but just in case
		GoToFrontPage();
	}

	public void PlayIntro () {
		//Debug.Log("Playing intro video");
	}

	public void PlayCredits () {
		//Debug.Log("Playing credits");
		ResetPages();
		creditsPage.SetActive(true);
		currentPage = Pages.cd;
		BackGroundMusic.clip = creditsMusic;
		BackGroundMusic.Play();
	}

	public void Quit () {
		StartCoroutine(quitFunction());
	}

	IEnumerator quitFunction () {
		BackGroundMusic.Stop();
		Const.a.WriteDatForNewGame(false,true);
		saltTheFries.SetActive(true);
		yield return new WaitForSeconds(0.8f);
		#if UNITY_EDITOR_WIN
			UnityEditor.EditorApplication.isPlaying = false;
		#endif

		Application.Quit();
	}
}
