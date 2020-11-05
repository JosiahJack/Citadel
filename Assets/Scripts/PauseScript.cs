using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PauseScript : MonoBehaviour {
	public GameObject pauseText;
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
	public GameObject hardSaveDialog;
	public bool onSaveDialog = false;
	public PlayerMovement pm;
	[HideInInspector]
	public float relativeTime;
	public List<AmbientRegistration> ambientRegistry;

	void Awake() {
		a = this;
		a.ambientRegistry = new List<AmbientRegistration>();
	}

	void Update () {
		if (mainMenu == null) {
			Const.sprint("BUG: PauseScript: mainMenu is null!",Const.a.allPlayers);
			return;
		}

		if (Input.GetKeyDown(KeyCode.F12)) {
			TakeScreenshot();
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

		if (paused) {
			// yep...relativeTime is stuck;
		} else {
			relativeTime += Time.deltaTime;
		}
	}

	public void PauseToggle() {
		if (paused)
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
		pauseText.SetActive(true);
		previousCursorImage = mouseCursor.cursorImage;
		mouseCursor.cursorImage = mouselookScript.cursorDefaultTexture;
		for (int i=0;i<disableUIOnPause.Length;i++) {
			disableUIOnPause[i].SetActive(false);
		}

		EnablePauseUI();
		//System.GC.Collect();
		//PauseRigidbody[] prb = FindObjectsOfType<PauseRigidbody>();
		for (int k=0;k<Const.a.prb.Length;k++) {
			Const.a.prb[k].Pause();
		}

		for (int u=0;u<ambientRegistry.Count;u++) {
			if (ambientRegistry[u].SFX != null) ambientRegistry[u].SFX.Pause();
		}
	}

	public void Loading() {
		for (int j=0;j<enableUIOnPause.Length;j++) {
			enableUIOnPause[j].SetActive(false);
		}
	}

	public void PauseDisable() {
		paused = false;
		pauseText.SetActive(false);
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

		//PauseRigidbody[] prb = FindObjectsOfType<PauseRigidbody>();
		for (int k=0;k<Const.a.prb.Length;k++) {
			Const.a.prb[k].UnPause();
		}

		for (int u=0;u<ambientRegistry.Count;u++) {
			if (ambientRegistry[u].SFX != null) ambientRegistry[u].SFX.UnPause();
		}

		pm.ConsoleDisable();
	}

	public void OpenSaveDialog() {
		if (onSaveDialog) return;
		if (Const.a.justSavedTimeStamp < Time.time) {
			onSaveDialog = true;
			saveDialog.SetActive(true);
		}
	}

	public void OpenSaveDialogHard() {
		if (onSaveDialog) return;
		if (Const.a.justSavedTimeStamp < Time.time) {
			onSaveDialog = true;
			hardSaveDialog.SetActive(true);
		}
	}
		
	public void ExitSaveDialog() {
		saveDialog.SetActive(false);
		hardSaveDialog.SetActive(false);
		onSaveDialog = false;
	}

	public void SavePause() {
		if (onSaveDialog) return;
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(false);
		}
		saveDialog.SetActive(false); // turn off dialog
		mainMenu.SetActive(true);
		mainMenu.GetComponent<MainMenuHandler>().GoToSaveGameSubmenu(true);
	}

	public void LoadPause() {
		if (onSaveDialog) return;
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(false);
		}
		saveDialog.SetActive(false); // turn off dialog
		mainMenu.SetActive(true);
		mainMenu.GetComponent<MainMenuHandler>().GoToLoadGameSubmenu(true);
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
		//if (hardSaveQuit) { PauseQuitHard(); return; } // Application.Quit();
		mainMenu.GetComponent<MainMenuHandler>().GoToFrontPage();
	}

	public void PauseQuitHard () {
		mainMenu.SetActive(true);
		mainMenu.GetComponent<MainMenuHandler>().Quit();
	}

	public void EnablePauseUI () {
		//if (hardSaveQuit) { mainMenu.SetActive(true); PauseQuitHard(); return; } // Application.Quit();

		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(true);
		}
	}

	public void PauseOptions () {
		if (onSaveDialog) return;
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(false);
		}
		mainMenu.SetActive(true);
		mainMenu.GetComponent<MainMenuHandler>().GoToOptionsSubmenu(true);
	}

	public bool Paused() {
		//if (paused || mainMenu.activeSelf == true) { // there are cases where I want to check for main menu separately
		//	return true;
		//}
		//return false;
		return paused;
	}

	public void TakeScreenshot() {
		string sname = System.DateTime.UtcNow.ToString("ddMMMyyyy_HH_mm_ss") + "_v0.91.png";
		string spath = Application.dataPath + "/Screenshots/" + sname;
		ScreenCapture.CaptureScreenshot(spath);
		Const.sprint("Wrote screenshot " + sname,Const.a.allPlayers);
	}

	public void AddAmbientToRegistry(AmbientRegistration ar) {
		ambientRegistry.Add(ar);
	}
}

// Fore use with LiveSplit or other future speedrunner utilities for doing speedruns
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public static class AutoSplitterData {
	public static long magicNumber = 0x1337133713371337;
	public static double thisRunTime = 0;
	public static bool isLoading = false;
	public static int missionSplitID = 0;
}