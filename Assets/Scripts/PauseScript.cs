using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseScript : MonoBehaviour {
	public Text pauseText;
	public bool paused = false;
	public static PauseScript a;
	public MouseLookScript mouselookScript;
	public MouseCursor mouseCursor;
	public GameObject[] disableUIOnPause;
	private bool previousInvMode = false;
	private Texture2D previousCursorImage;
	public GameObject saltTheFries;
	public GameObject[] enableUIOnPause;
	public GameObject mainMenu;
	public GameObject saveDialog;
	public bool onSaveDialog;

	void Awake() {a = this;}

	void Update () {
		if (mainMenu == null) {
			Const.sprint("ERROR->PauseScript: mainMenu is null!",Const.a.allPlayers);
			return;
		}

		if (mainMenu.activeSelf == false) {
			if (GetInput.a.Menu()) {
				if (onSaveDialog) {
					ExitSaveDialog();
				} else {
					PauseToggle();
				}
			}

			if (Input.GetKeyDown(KeyCode.Home)) {
				PauseEnable();
			}

			if (Input.GetKeyDown(KeyCode.Menu)) {
				PauseEnable();
			}
		}
	}

	public void PauseToggle() {
		if (paused == true)
			PauseDisable();
		else
			PauseEnable();
	}

	public void PauseEnable() {
		previousInvMode = mouselookScript.inventoryMode;
		if (mouselookScript.inventoryMode == false) {
			mouselookScript.ToggleInventoryMode();
			mouselookScript.ToggleAudioPause();
		}
		paused = true;
		pauseText.enabled = true;
		previousCursorImage = mouseCursor.cursorImage;
		mouseCursor.cursorImage = mouselookScript.cursorDefaultTexture;
		for (int i=0;i<disableUIOnPause.Length;i++) {
			disableUIOnPause[i].SetActive(false);
		}

		EnablePauseUI();
		//System.GC.Collect();
		PauseRigidbody[] prb = FindObjectsOfType<PauseRigidbody>();
		for (int k=0;k<prb.Length;k++) {
			prb[k].Pause();
		}
	}

	public void PauseDisable() {
		paused = false;
		pauseText.enabled = false;
		if (previousInvMode != mouselookScript.inventoryMode) {
			mouselookScript.ToggleInventoryMode();
		}
		mouseCursor.cursorImage = previousCursorImage;
		for (int i=0;i<disableUIOnPause.Length;i++) {
			disableUIOnPause[i].SetActive(true);
		}

		for (int j=0;j<enableUIOnPause.Length;j++) {
			enableUIOnPause[j].SetActive(false);
		}

		PauseRigidbody[] prb = FindObjectsOfType<PauseRigidbody>();
		for (int k=0;k<prb.Length;k++) {
			prb[k].UnPause();
		}
	}

	public void OpenSaveDialog() {
		onSaveDialog = true;
		saveDialog.SetActive(true);
	}
		
	public void ExitSaveDialog() {
		saveDialog.SetActive(false);
		onSaveDialog = false;
	}

	public void SavePauseQuit() {
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(false);
		}
		saveDialog.SetActive(false); // turn off dialog
		mainMenu.SetActive(true);
		mainMenu.GetComponent<MainMenuHandler>().GoToSaveGameSubmenu(true);
	}

	public void NoSavePauseQuit () {
		//StartCoroutine(quitFunction());
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(false);
		}
		saveDialog.SetActive(false); // turn off dialog
		mainMenu.SetActive(true);
		mainMenu.GetComponent<MainMenuHandler>().GoToFrontPage();
	}

	public void EnablePauseUI () {
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(true);
		}
	}

	public void PauseOptions () {
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(false);
		}
		mainMenu.SetActive(true);
		mainMenu.GetComponent<MainMenuHandler>().GoToOptionsSubmenu(true);
	}

	public bool Paused() {
		if (paused || mainMenu.activeSelf == true) {
			return true;
		}

		return false;
	}
}
