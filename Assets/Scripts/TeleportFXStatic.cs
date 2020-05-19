using UnityEngine;
using System.Collections;

public class TeleportFXStatic : MonoBehaviour {
	public float intervalTime = 0.08f;
	public float activeTime = 1f;
	[HideInInspector]
	public GameObject mouseCursor;
	public Texture2D tempCursorTexture;
	[HideInInspector]
	public Texture2D cursorTexture;
	private float effectFinished;
	private float flipTime;
	private float randHolder;
	private bool xFlipped = false;
	private bool yFlipped = false;
	private RectTransform rect;

	void OnEnable () {
		mouseCursor = GameObject.Find("MouseCursorHandler");
		if (mouseCursor == null) {
			print("Warning: Could Not Find object 'MouseCursorHandler' in scene\n");
			return;
		}
		cursorTexture = mouseCursor.GetComponent<MouseCursor>().cursorImage; //store correct cursor
		mouseCursor.GetComponent<MouseCursor>().cursorImage = tempCursorTexture; //give dummy cursor to hide it
		effectFinished = PauseScript.a.relativeTime + activeTime;
		rect = GetComponent<RectTransform>();
		flipTime = PauseScript.a.relativeTime + intervalTime;
	}

	void FlipX () {
		if (xFlipped) {
			xFlipped = false;
			rect.localScale = new Vector3(1f, 1f, 1f);
		} else {
			xFlipped = true;
			rect.localScale = new Vector3(-1f, 1f, 1f);
		}
	}

	void FlipY () {
		if (yFlipped) {
			yFlipped = false;
			rect.localScale = new Vector3(1f, 1f, 1f);
		} else {
			yFlipped = true;
			rect.localScale = new Vector3(1f, -1f, 1f);
		}
	}

	void Deactivate () {
		mouseCursor.GetComponent<MouseCursor>().cursorImage = cursorTexture; //return to previous cursor
		gameObject.SetActive(false);
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (effectFinished < PauseScript.a.relativeTime) {
				Deactivate();
			}
			if (flipTime < PauseScript.a.relativeTime) {
				flipTime = PauseScript.a.relativeTime + intervalTime;
				randHolder = Random.Range(0f,1f);
				if (randHolder < 0.5) {
					FlipX();
				} else {
					FlipY();
				}
			}
		}
	}
}
