using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;

public class PauseScript : MonoBehaviour {
	public GameObject pauseText;
	public GameObject[] disableUIOnPause;
	public GameObject saltTheFries;
	public GameObject[] enableUIOnPause;
	public GameObject mainMenu;
	public GameObject saveDialog;
	public GameObject hardSaveDialog;

	[HideInInspector] public bool paused = false;
	private bool previousInvMode = false;
	private Texture2D previousCursorImage;
	[HideInInspector] public bool onSaveDialog = false;
	public float relativeTime;
	public float absoluteTime;
	[HideInInspector] public List<AmbientRegistration> ambientRegistry;
	private bool menuActive = true; // Store the state of the main menu
	                                // gameobject active state so that we don't
									// have to do a gameobject engine call more
									// than once on every Update all over the
									// code.

	public static PauseScript a;

	void Awake() {
		a = this;
		a.ambientRegistry = new List<AmbientRegistration>();
	}

	// The whole point right here:
	public bool Paused() { return paused; }
	public bool MenuActive() { return menuActive; }

	void Update() {
	    if (relativeTime > 0f) absoluteTime += Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.F12)) TakeScreenshot();

		menuActive = mainMenu.activeSelf;
		if (!menuActive) {
			if (GetInput.a.Menu()) {
				if (onSaveDialog)
					ExitSaveDialog();
				else
					PauseToggle();
			}
			if (Input.GetKeyDown(KeyCode.Home)
			    || Input.GetKeyDown(KeyCode.Menu)) {
			        
			    PauseEnable();
			}
			CheckForSuperWinCmdKey();

			if (!Paused()) {
				// Raytraced Audio Occlusion ;)
				int hitCount = 0;
				float newVolume = 1.0f;
				RaycastHit[] results = new RaycastHit[6];
				for (int i=0;i<ambientRegistry.Count;i++) {
					if (ambientRegistry[i] == null) continue;

					hitCount = Physics.RaycastNonAlloc(
								MouseLookScript.a.transform.position,
								ambientRegistry[i].transform.position
								- MouseLookScript.a.transform.position,
								results,32f,Const.a.layerMaskPlayerFrob,
								QueryTriggerInteraction.UseGlobal);

					ambientRegistry[i].SFX.volume =
						ambientRegistry[i].normalVolume;

					if (hitCount > 0) {
						if (hitCount > 5) {
							newVolume = ambientRegistry[i].normalVolume * 0.40f;
						} else if (hitCount == 5) {
							newVolume = ambientRegistry[i].normalVolume * 0.50f;
						} else if (hitCount == 4) {
							newVolume = ambientRegistry[i].normalVolume * 0.60f;
						} else if (hitCount == 3) {
							newVolume = ambientRegistry[i].normalVolume * 0.70f;
						} else if (hitCount == 2) {
							newVolume = ambientRegistry[i].normalVolume * 0.80f;
						} else {
							newVolume = ambientRegistry[i].normalVolume * 0.90f;
						}

						ambientRegistry[i].SFX.volume = newVolume;
					}
				}

				Const.a.NPCAudioOcclusion();
			}
		}

		if (!Paused()) relativeTime += Time.deltaTime;
	}

	public void ConsoleEntryEnterDelegate() {
		ConsoleEmulator.ConsoleEntryEnter();
	}

/*
    void FixedUpdate() {
		Debug.Log("ObjectContainmentSystem active floor chunks: "
				  + ObjectContainmentSystem.ActiveFloorChunks.Count.ToString());

		GameObject go = null; // Contain the currently checked floor.
		Rigidbody rb = null; // Contain the currently checked rigidbody.
		PauseRigidbody pb = null; // Reference to all the rigidbodies each.
		Vector3 flrPos = null;
		Vector3 objPos = null;
		float x, y, z;

        // Iterate through each floor and check for overlapping rigidbodies
        for (int i=0;i < ObjectContainmentSystem.ActiveFloorChunks.Count;i++) {
			go = ObjectContainmentSystem.ActiveFloorChunks[i];
			flrPos = go.transform.position;
			for (int k=0;k<Const.a.prb.Count;k++) {
				pb = Const.a.prb[k];
				if (!pm.gameObject.activeInHierarchy) continue;

				objPos = pb.gameObject.transform.position;
				if (!PhysObjAffectedByFloor(objPos, flrPos)) continue;

                Rigidbody rb = pb.rbody;
                if (flrPos.y - objPos.y > 1.28f)  {
                    // If the rigidbody falls below the barrier height, set its position to the barrier height
                    rb.position = new Vector3(rb.position.x, barrierHeight, rb.position.z);
                    // If the rigidbody has a velocity in the downward direction, set its velocity to zero
                    if (rb.velocity.y < 0f)
                    {
                        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                    }
				}
			}



            Vector3 barrierCenter = new Vector3(barrierPositions[i].x * gridSize + gridSize / 2f, barrierHeight, barrierPositions[i].y * gridSize + gridSize / 2f);
            float barrierSize = gridSize / 2f;
            List<Rigidbody> rigidbodiesInBarrier = rigidbodyOctree.GetObjectsInRange(barrierCenter, barrierSize);
            for (int j = 0; j < rigidbodiesInBarrier.Count; j++)
            {

            }
        }
    }*/

	private bool PhysObjAffectedByFloor(Vector3 objpos, Vector3 floorpos) {
		if (objpos.x - floorpos.x > 1.28f && objpos.z - floorpos.z > 1.28f) {
			return true;
		}

		return false;
	}

	void CheckForSuperWinCmdKey() {
		if (   Input.GetKeyDown(KeyCode.LeftCommand)     // Apple / Linux
			|| Input.GetKeyDown(KeyCode.RightCommand)    // Apple / Linux
			|| Input.GetKeyDown(KeyCode.LeftWindows)     // Windows
			|| Input.GetKeyDown(KeyCode.RightWindows)) { // Windows
			PauseEnable();
		}
	}

	public void PauseToggle() {
		if (Paused())	PauseDisable();
		else			PauseEnable();
	}

	public void PauseEnable() {
		AudioListener.pause = true;
		PauseSystems();
		previousInvMode = MouseLookScript.a.inventoryMode;
		if (MouseLookScript.a.inventoryMode == false) {
			MouseLookScript.a.ToggleInventoryMode();
		}
		EnablePauseUI();
		pauseText.SetActive(true);
	}

	public void PauseDisable() {
		AudioListener.pause = false;
		UnpauseSystems();
		if (previousInvMode != MouseLookScript.a.inventoryMode) {
			MouseLookScript.a.ToggleInventoryMode();
			MouseLookScript.a.SetCameraCullDistances();
		}
		DisablePauseUI();
		pauseText.SetActive(false);
	}

	public void PauseSystems() {
		paused = true;
		previousCursorImage = MouseCursor.a.cursorImage;
		MouseCursor.a.cursorImage = MouseLookScript.a.cursorDefaultTexture;
		for (int i=0;i<disableUIOnPause.Length;i++) {
			disableUIOnPause[i].SetActive(false);
		}

		for (int k=0;k<Const.a.prb.Count;k++) Const.a.prb[k].Pause();
		for (int k=0;k<Const.a.psys.Count;k++) Const.a.psys[k].Pause();
		for (int k=0;k<Const.a.panimsList.Count;k++) {
			Const.a.panimsList[k].Pause();
		}

		PauseAmbients();
	}

	public void PauseAmbients() {
		for (int u=0;u<ambientRegistry.Count;u++) {
			if (ambientRegistry[u].SFX != null) ambientRegistry[u].SFX.Pause();
		}
	}

	public void UnpauseAmbients() {
		for (int u=0;u<ambientRegistry.Count;u++) {
			if (ambientRegistry[u].SFX != null) ambientRegistry[u].SFX.UnPause();
		}
	}

	public void UnpauseSystems() {
		paused = false;
		MouseCursor.a.cursorImage = previousCursorImage;
		for (int i=0;i<disableUIOnPause.Length;i++) {
			disableUIOnPause[i].SetActive(true);
		}

		for (int k=0;k<Const.a.prb.Count;k++) Const.a.prb[k].UnPause();
		for (int k=0;k<Const.a.psys.Count;k++) Const.a.psys[k].UnPause();
		for (int k=0;k<Const.a.panimsList.Count;k++) {
			Const.a.panimsList[k].UnPause();
		}

		UnpauseAmbients();
		PlayerMovement.a.ConsoleDisable();
	}

	public void OpenSaveDialog() {
		if (onSaveDialog) return;

		if (PlayerMovement.a.inCyberSpace) {
			Const.sprint(Const.a.stringTable[602]); // Cannot save in cyberspace
			OpenSaveDialogHard();
			return;
		}

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
		if (PlayerMovement.a.inCyberSpace) {
			Const.sprint(Const.a.stringTable[602]); // Cannot save in cyberspace
			return;
		}
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
		GameObject newGameIndicator = GameObject.Find("NewGameIndicator");
		GameObject loadGameIndicator = GameObject.Find("LoadGameIndicator");
		GameObject freshGame = GameObject.Find("GameNotYetStarted");
		if (newGameIndicator != null) Utils.SafeDestroy(newGameIndicator);
		if (loadGameIndicator != null) Utils.SafeDestroy(loadGameIndicator);
		if (freshGame != null) Utils.SafeDestroy(freshGame);
		MainMenuHandler.a.GoToSaveGameSubmenu(true);
	}

	public void NoSavePauseQuit() {
		DisablePauseUI();
		saveDialog.SetActive(false); // turn off dialog
		mainMenu.SetActive(true);
		GameObject newGameIndicator = GameObject.Find("NewGameIndicator");
		GameObject loadGameIndicator = GameObject.Find("LoadGameIndicator");
		GameObject freshGame = GameObject.Find("GameNotYetStarted");
		if (newGameIndicator != null) Utils.SafeDestroy(newGameIndicator);
		if (loadGameIndicator != null) Utils.SafeDestroy(loadGameIndicator);
		if (freshGame != null) Utils.SafeDestroy(freshGame);
		MainMenuHandler.a.GoToFrontPage();
	}

	public void PauseQuitHard() {
		mainMenu.SetActive(true);
		MainMenuHandler.a.Quit();
	}

	public void EnablePauseUI() {
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(true);
			StartMenuButtonHighlight smbh = 
				enableUIOnPause[i].GetComponent<StartMenuButtonHighlight>();

			if (smbh != null) {
				smbh.DeHighlight(); // Prevent persisted states.
				if (i == 3 && PlayerMovement.a.inCyberSpace) { // Save button
					smbh.enabled = false;
				} else {
					smbh.enabled = true;
				}
			}
		}
	}

	public void DisablePauseUI() {
		for (int i=0;i<enableUIOnPause.Length;i++) {
			enableUIOnPause[i].SetActive(false);
		}
	}

	public void PauseOptions () {
		if (onSaveDialog) return;

		DisablePauseUI();
		mainMenu.SetActive(true);
		MainMenuHandler.a.GoToOptionsSubmenu(true);
	}


	public void TakeScreenshot() {
		string sname = System.DateTime.UtcNow.ToString("ddMMMyyyy_HH_mm_ss")
					   + "_" + Const.a.versionString + ".png";
		string spath = Utils.SafePathCombine(Application.streamingAssetsPath,
											 "Screenshots");

		// Check and recreate Screenshots folder if it was deleted.
        if (!Directory.Exists(spath)) Directory.CreateDirectory(spath);
		spath = Utils.SafePathCombine(spath,sname);
		ScreenCapture.CaptureScreenshot(spath);
		StartCoroutine(ScreenshotSprint(sname));
	}

	// Let screenshot save without putting text in it.
	public IEnumerator ScreenshotSprint(string sname) {
		yield return new WaitForSeconds(0.1f);
		Const.sprint("Wrote screenshot " + sname);

	}

	// No need to clear, these are all unsaved and static.
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
