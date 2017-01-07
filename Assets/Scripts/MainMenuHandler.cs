using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.ImageEffects;
using System.Collections;

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
	public GameObject optionsPage;
	public InputField newgameInputText;
	public StartMenuDifficultyController combat;
	public StartMenuDifficultyController mission;
	public StartMenuDifficultyController puzzle;
	public StartMenuDifficultyController cyber;
	private enum Pages {fp,sp,mp,np,lp,op};
	private Pages currentPage;
	private AudioSource StartSFX;
	private AudioSource BackGroundMusic;

	void Awake () {
		StartSFX = startFXObject.GetComponent<AudioSource>();
		BackGroundMusic = GetComponent<AudioSource>();
		GoToFrontPage();
	}

	void Update () {
		// Escape/back button listener
		if (Input.GetKeyDown(KeyCode.Escape)) {
			GoBack();
		}

		if ( (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.P)) || (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKey(KeyCode.P)) ) {
			Debug.Log("Skipping main menu. Debug cheat");
			StartGame();
		}
	}

	public void StartGame () {
		StartSFX.PlayOneShot(StartGameSFX);
		Const.a.playerName = newgamePage.GetComponentInChildren<InputField>().text;
		Const.a.difficultyCombat = combat.difficultySetting;
		Const.a.difficultyMission = mission.difficultySetting;
		Const.a.difficultyPuzzle = puzzle.difficultySetting;
		Const.a.difficultyCyber = cyber.difficultySetting;
		this.gameObject.SetActive(false);
	}

	void ResetPages() {
		singleplayerPage.SetActive(false);
		multiplayerPage.SetActive(false);
		newgamePage.SetActive(false);
		frontPage.SetActive(false);
		loadPage.SetActive(false);
		optionsPage.SetActive(false);
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

	public void GoToOptionsSubmenu () {
		ResetPages();
		optionsPage.SetActive(true);
		currentPage = Pages.op;
	}

	public void GoToNewGameSubmenu () {
		ResetPages();
		newgamePage.SetActive(true);
		newgameInputText.ActivateInputField();
		currentPage = Pages.np;
	}

	public void GoToLoadGameSubmenu () {
		ResetPages();
		loadPage.SetActive(true);
		currentPage = Pages.lp;
	}

	void GoBack () {
		// Go Back to front page
		if (currentPage == Pages.sp || currentPage == Pages.mp || currentPage == Pages.op) {
			GoToFrontPage();
			return;
		}

		// Go Back to singlepayer page
		if (currentPage == Pages.np || currentPage == Pages.lp) {
			GoToSingleplayerSubmenu();
			return;
		}
	}

	public void PlayIntro () {
		Debug.Log("Playing intro video");
	}

	public void PlayCredits () {
		Debug.Log("Playing credits");
	}

	public void Quit () {
		StartCoroutine(quitFunction());
	}

	IEnumerator quitFunction () {
		BackGroundMusic.Stop();
		saltTheFries.SetActive(true);
		yield return new WaitForSeconds(0.8f);
		#if UNITY_EDITOR_WIN
			UnityEditor.EditorApplication.isPlaying = false;
		#endif
		Application.Quit();
	}
}
