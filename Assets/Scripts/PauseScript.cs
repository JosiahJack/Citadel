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

	void Awake() {a = this; }

	void Update () {
		if (mainMenu == null) {
			Const.sprint("ERROR->PauseScript: mainMenu is null!",Const.a.allPlayers);
			return;
		}

		if (mainMenu.activeSelf == false) {
			if (GetInput.a.Menu()) {
				PauseToggle();
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

	void PauseEnable() {
		previousInvMode = mouselookScript.inventoryMode;
		if (mouselookScript.inventoryMode == false) {
			mouselookScript.ToggleInventoryMode();
		}
		paused = true;
		pauseText.enabled = true;
		previousCursorImage = mouseCursor.cursorImage;
		mouseCursor.cursorImage = mouselookScript.cursorDefaultTexture;
		for (int i=0;i<disableUIOnPause.Length;i++) {
			disableUIOnPause[i].SetActive(false);
		}

		for (int j=0;j<enableUIOnPause.Length;j++) {
			enableUIOnPause[j].SetActive(true);
		}

		PauseRigidbody[] prb = FindObjectsOfType<PauseRigidbody>();
		for (int k=0;k<prb.Length;k++) {
			prb[k].Pause();
		}
	}

	void PauseDisable() {
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

	public void PauseQuit () {
		StartCoroutine(quitFunction());
	}

	IEnumerator quitFunction () {
		saltTheFries.SetActive(true);
		yield return new WaitForSeconds(0.8f);
		#if UNITY_EDITOR_WIN
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
		Application.Quit();
	}
}
