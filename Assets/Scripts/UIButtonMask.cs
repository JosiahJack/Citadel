using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButtonMask : MonoBehaviour {
    public GameObject playerCamera;
	private MouseCursor mCursor;
	public GUIState.ButtonType overButtonType = GUIState.ButtonType.Generic;  // default to generic button
	private float doubleClickTime;
	private float dbclickFinished;
	public int doubleClickTicks;  // takes 2 to activate double click function
	public bool doubleClickEnabled = false;
	public int toolTipLingdex = -1;
	public Handedness toolTipType;
	private EventTrigger pointerTrigger;

	void Start () {
		mCursor = playerCamera.GetComponent<MouseLookScript>().mouseCursor.GetComponent<MouseCursor>();
		mCursor.RegisterRaycastRect(gameObject,GetComponent<RectTransform>());
		if (playerCamera == null) {
			Const.sprint("BUG: UIButtonMask script could not find playerCamera",Const.a.allPlayers);
		}
		pointerTrigger = GetComponent<EventTrigger>();
		if (pointerTrigger == null) {
			pointerTrigger = gameObject.AddComponent<EventTrigger>();
			if (pointerTrigger == null) {
				Const.sprint("BUG: Could not create EventTrigger for UIButtonMask",Const.a.allPlayers);
				return;
			}
		}

		if (doubleClickEnabled) {
			doubleClickTime = Const.a.doubleClickTime;
			dbclickFinished = PauseScript.a.relativeTime;
			doubleClickTicks = 0;
			GetComponent<Button>().onClick.AddListener(() => { UiButtonMaskClick(); });
		}
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (doubleClickEnabled) {
				if (dbclickFinished < PauseScript.a.relativeTime) {
					doubleClickTicks--;
					if (doubleClickTicks < 0)
						doubleClickTicks = 0;
				}
			}
		}
	}

	public void PtrEnter () {
		GUIState.a.PtrHandler(true,true,overButtonType,gameObject);
        playerCamera.GetComponent<MouseLookScript>().currentButton = gameObject;
		doubleClickTicks = 0;

		if (toolTipLingdex >= 0) {
			mCursor.toolTip = Const.a.stringTable[toolTipLingdex];
			mCursor.toolTipType = toolTipType;
		}
    }

	public void PtrExit () {
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
		doubleClickTicks = 0;
		if (toolTipLingdex >= 0) {
			mCursor.toolTip = string.Empty;
		}
    }

	void UiButtonMaskClick () {
		doubleClickTicks++;
		if (doubleClickTicks == 2) {
			gameObject.SendMessage("DoubleClick");
			doubleClickTicks = 0;
			dbclickFinished = PauseScript.a.relativeTime + doubleClickTime;
			return;
		}
		dbclickFinished = PauseScript.a.relativeTime + doubleClickTime;
	}
}
