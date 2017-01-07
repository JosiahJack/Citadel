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
	public GameObject quitButton;
	public GameObject mainMenu;

	void Awake() {a = this; }

	void Update () {
		if (mainMenu.activeSelf == false) {
			if (Input.GetKeyDown(KeyCode.Escape)) {
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
		paused = true;
		pauseText.enabled = true;
		previousInvMode = mouselookScript.inventoryMode;
		previousCursorImage = mouseCursor.cursorImage;
		mouseCursor.cursorImage = mouselookScript.cursorDefaultTexture;
		mouselookScript.inventoryMode = true;
		for (int i=0;i>disableUIOnPause.Length;i++) {
			disableUIOnPause[i].SetActive(false);
		}

		for (int j=0;j>enableUIOnPause.Length;j++) {
			enableUIOnPause[j].SetActive(true);
		}
		quitButton.SetActive(true);
	}

	void PauseDisable() {
		paused = false;
		pauseText.enabled = false;
		mouselookScript.inventoryMode = previousInvMode;
		mouseCursor.cursorImage = previousCursorImage;
		for (int i=0;i>disableUIOnPause.Length;i++) {
			disableUIOnPause[i].SetActive(true);
		}

		for (int j=0;j>enableUIOnPause.Length;j++) {
			enableUIOnPause[j].SetActive(false);
		}
		quitButton.SetActive(false);
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
