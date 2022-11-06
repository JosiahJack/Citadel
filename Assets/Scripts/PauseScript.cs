using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class PauseScript : MonoBehaviour {
	public GameObject pauseText;
	public GameObject[] disableUIOnPause;
	public GameObject saltTheFries;
	public GameObject[] enableUIOnPause;
	public GameObject mainMenu;
	public GameObject saveDialog;
	public GameObject hardSaveDialog;
	public PlayerMovement pm;

	[HideInInspector] public bool paused = false;
	private bool previousInvMode = false;
	private Texture2D previousCursorImage;
	[HideInInspector] public bool onSaveDialog = false;
	[HideInInspector] public float relativeTime;
	[HideInInspector] public List<AmbientRegistration> ambientRegistry;
	private bool menuActive = true; // Store the state of the main menu gameobject active state
									// so that we don't have to do a gameobject engine call more
									// than once on every Update all over the code.

	public static PauseScript a;

	void Awake() {
		a = this;
		a.ambientRegistry = new List<AmbientRegistration>();
	}

	// The whole point right here:
	public bool Paused() { return paused; }
	public bool MenuActive() { return menuActive; }

	void Update() {
		if (Input.GetKeyDown(KeyCode.F12)) TakeScreenshot();

		menuActive = mainMenu.activeSelf;
		if (!menuActive) {
			if (GetInput.a.Menu()) {
				if (onSaveDialog)
					ExitSaveDialog();
				else
					PauseToggle();
			}
			if (Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Menu)) PauseEnable();
		}

		if (!Paused()) relativeTime = relativeTime + Time.deltaTime;
	}

	public void PauseToggle() {
		if (Paused())	PauseDisable();
		else			PauseEnable();
	}

	public void ToggleAudioPause() {
		if (Paused())	AudioListener.pause = true;
		else			AudioListener.pause = false;
	}

	public void PauseEnable() {
		previousInvMode = MouseLookScript.a.inventoryMode;
		if (MouseLookScript.a.inventoryMode == false) {
			MouseLookScript.a.ToggleInventoryMode();
			ToggleAudioPause();
		}
		paused = true;
		pauseText.SetActive(true);
		previousCursorImage = MouseCursor.a.cursorImage;
		MouseCursor.a.cursorImage = MouseLookScript.a.cursorDefaultTexture;
		for (int i=0;i<disableUIOnPause.Length;i++) {
			disableUIOnPause[i].SetActive(false);
		}

		EnablePauseUI();
		for (int k=0;k<Const.a.prb.Count;k++) {
			Const.a.prb[k].Pause();
		}

		for (int k=0;k<Const.a.psys.Count;k++) {
			Const.a.psys[k].Pause();
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
		if (previousInvMode != MouseLookScript.a.inventoryMode) {
			MouseLookScript.a.ToggleInventoryMode();
			MouseLookScript.a.SetCameraCullDistances();
		}
		MouseCursor.a.cursorImage = previousCursorImage;
		for (int i=0;i<disableUIOnPause.Length;i++) {
			disableUIOnPause[i].SetActive(true);
		}

		for (int j=0;j<enableUIOnPause.Length;j++) {
			enableUIOnPause[j].SetActive(false);
		}

		for (int k=0;k<Const.a.prb.Count;k++) {
			Const.a.prb[k].UnPause();
		}

		for (int k=0;k<Const.a.psys.Count;k++) {
			Const.a.psys[k].UnPause();
		}

		for (int u=0;u<ambientRegistry.Count;u++) {
			if (ambientRegistry[u].SFX != null) ambientRegistry[u].SFX.UnPause();
		}

		pm.ConsoleDisable();
	}

	public void OpenSaveDialog() {
		if (onSaveDialog) return;

		DisablePauseUI();
		if (Const.a.justSavedTimeStamp < Time.time) {
			onSaveDialog = true;
			saveDialog.SetActive(true);
		}
	}

	public void OpenSaveDialogHard() {
		if (onSaveDialog) return;

		DisablePauseUI();
		if (Const.a.justSavedTimeStamp < Time.time) {
			onSaveDialog = true;
			hardSaveDialog.SetActive(true);
		}
	}

	public void ExitSaveDialog() {
		EnablePauseUI();
		saveDialog.SetActive(false);
		hardSaveDialog.SetActive(false);
		onSaveDialog = false;
	}

	public void SavePause() {
		if (onSaveDialog) return;

		DisablePauseUI();
		saveDialog.SetActive(false); // turn off dialog
		mainMenu.SetActive(true);
		MainMenuHandler.a.GoToSaveGameSubmenu(true);
	}

	public void LoadPause() {
		if (onSaveDialog) return;

		DisablePauseUI();
		saveDialog.SetActive(false); // turn off dialog
		mainMenu.SetActive(true);
		MainMenuHandler.a.GoToLoadGameSubmenu(true);
	}

	public void SavePauseQuit() {
		DisablePauseUI();
		saveDialog.SetActive(false); // turn off dialog
		mainMenu.SetActive(true);
		MainMenuHandler.a.GoToSaveGameSubmenu(true);
	}

	public void NoSavePauseQuit() {
		DisablePauseUI();
		saveDialog.SetActive(false); // turn off dialog
		mainMenu.SetActive(true);
		MainMenuHandler.a.GoToFrontPage();
	}

	public void PauseQuitHard() {
		mainMenu.SetActive(true);
		MainMenuHandler.a.Quit();
	}

	public void EnablePauseUI() {
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(true);
		}
	}

	public void DisablePauseUI() {
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(false);
		}
	}

	public void PauseOptions () {
		if (onSaveDialog) return;
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(false);
		}
		mainMenu.SetActive(true);
		MainMenuHandler.a.GoToOptionsSubmenu(true);
	}

	public void TakeScreenshot() {
		string sname = System.DateTime.UtcNow.ToString("ddMMMyyyy_HH_mm_ss") + "_v0.91.png";
		string spath = Application.dataPath + "/StreamingAssets/Screenshots/" + sname;
		ScreenCapture.CaptureScreenshot(spath);
		Const.sprint("Wrote screenshot " + sname);
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
