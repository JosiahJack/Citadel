using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButtonMask : MonoBehaviour {
    public GameObject playerCamera;
	public GUIState.ButtonType overButtonType = GUIState.ButtonType.Generic;  // default to generic button
	private float doubleClickTime;
	private float dbclickFinished;
	public int doubleClickTicks;  // takes 2 to activate double click function
	public bool doubleClickEnabled = false;
	public string toolTipText;
	public Handedness toolTipType;

	void Start () {
		if (playerCamera == null) {
			Const.sprint("Warning: UIButtonMask script could not find playerCamera",Const.a.allPlayers);
		}
		EventTrigger pointerTrigger = GetComponent<EventTrigger>();
		if (pointerTrigger == null) {
			pointerTrigger = gameObject.AddComponent<EventTrigger>();
			if (pointerTrigger == null) {
				Const.sprint("Warning: Could not create Event Trigger for UIButtonMask",Const.a.allPlayers);
				return;
			}
		}
			
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener((eventData) => { PtrEnter(); } );
		pointerTrigger.triggers.Add(entry);

		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerExit;
		entry2.callback.AddListener((eventData) => { PtrExit(); } );
		pointerTrigger.triggers.Add(entry2);

		if (doubleClickEnabled) {
			doubleClickTime = Const.a.doubleClickTime;
			dbclickFinished = Time.time;
			doubleClickTicks = 0;
			GetComponent<Button>().onClick.AddListener(() => { UiButtonMaskClick(); });
		}
	}

	void Update () {
		if (doubleClickEnabled) {
			if (dbclickFinished < Time.time) {
				doubleClickTicks--;
				if (doubleClickTicks < 0)
					doubleClickTicks = 0;
			}
		}
	}

	public void PtrEnter () {
		GUIState.a.PtrHandler(true,true,overButtonType,gameObject);
        playerCamera.GetComponent<MouseLookScript>().currentButton = gameObject;
		doubleClickTicks = 0;

		if (toolTipText != null && toolTipText != string.Empty) {
			playerCamera.GetComponent<MouseLookScript>().mouseCursor.GetComponent<MouseCursor>().toolTip = toolTipText;
			playerCamera.GetComponent<MouseLookScript>().mouseCursor.GetComponent<MouseCursor>().toolTipType = toolTipType;
		}
    }

	public void PtrExit () {
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
		doubleClickTicks = 0;
		if (toolTipText != null && toolTipText != string.Empty) {
			playerCamera.GetComponent<MouseLookScript>().mouseCursor.GetComponent<MouseCursor>().toolTip = string.Empty;
		}
    }

	void UiButtonMaskClick () {
		doubleClickTicks++;
		if (doubleClickTicks == 2) {
			gameObject.SendMessage("DoubleClick");
			doubleClickTicks = 0;
			dbclickFinished = Time.time + doubleClickTime;
			return;
		}
		dbclickFinished = Time.time + doubleClickTime;
	}
}
