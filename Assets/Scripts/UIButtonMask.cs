using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButtonMask : MonoBehaviour {
	public ButtonType overButtonType = ButtonType.Generic;  // default to generic button
	public int doubleClickTicks;  // takes 2 to activate double click function
	public bool doubleClickEnabled = false;
	public int toolTipLingdex = -1;
	public Handedness toolTipType;

	private float doubleClickTime;
	private float dbclickFinished;
	private BoxCollider boxCol;
	private RectTransform rect;
	private EventTrigger evenT;
	private bool pointerEntered;

	void Start() { // Start for the PauseScript.a and MouseScript.a references.
		rect = GetComponent<RectTransform>(); // Create box collider for cursor entry detection.
		pointerEntered = false;
		boxCol = gameObject.AddComponent<BoxCollider>();
		float width = rect.sizeDelta.x * rect.localScale.x;
		float height = rect.sizeDelta.y * rect.localScale.y;
		if (width < 0) width *= -1f;
		if (height < 0) height *= -1f; // Cannot have negative size on box colliders.
		boxCol.size = new Vector3(width,height,1f);
		MouseCursor.a.RegisterRaycastRect(gameObject,GetComponent<RectTransform>());

		pointerEntered = false;
		evenT = GetComponent<EventTrigger>();
		if (evenT == null) evenT = gameObject.AddComponent<EventTrigger>();
		if (evenT != null) {
			// Create a new entry for the PointerEnter event
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
            evenT.triggers.Add(pointerEnter);

            // Create a new entry for the PointerExit event
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
            evenT.triggers.Add(pointerExit);
		} else Debug.Log("Failed to add EventTrigger to " + gameObject.name);

		if (doubleClickEnabled) {
			doubleClickTime = Const.a.doubleClickTime;
			dbclickFinished = PauseScript.a.relativeTime;
			doubleClickTicks = 0;
			GetComponent<Button>().onClick.AddListener(() => { UiButtonMaskClick(); });
		}
	}

	void OnEnable() {
		pointerEntered = false;
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (doubleClickEnabled) {
				if (dbclickFinished < PauseScript.a.relativeTime) {
					doubleClickTicks--;
					if (doubleClickTicks < 0) doubleClickTicks = 0;
				}
			}
		}
	}

	// These interface with the EventTrigger system and call the real meat and potatoes below.

	// Handle OnPointerEnter event, replaces OnMouseEnter
    public void OnPointerEnterDelegate(PointerEventData data) { PtrEnter(); }

	// Handle OnPointerExit event, replaces OnMouseExit
    public void OnPointerExitDelegate(PointerEventData data) { PtrExit(); }

	// Old Unity UI system
	//public void OnMouseEnter() { PtrEnter(); }
	//public void OnMouseExit() { PtrExit(); }

	public void PtrEnter () {
		if (pointerEntered) return;

		GUIState.a.PtrHandler(true,true,overButtonType,gameObject);
        MouseLookScript.a.currentButton = gameObject;
		doubleClickTicks = 0;

		if (toolTipLingdex >= 0) {
			MouseCursor.a.toolTip = Const.a.stringTable[toolTipLingdex];
			MouseCursor.a.toolTipType = toolTipType;
		}
		pointerEntered = true;
    }

	public void PtrExit () {
		if (!pointerEntered) return;

		GUIState.a.PtrHandler(false,false,ButtonType.None,null);
		doubleClickTicks = 0;
		if (toolTipLingdex >= 0) {
			MouseCursor.a.toolTip = string.Empty;
		}
		pointerEntered = false;
    }

	void UiButtonMaskClick () {
		if (EventSystem.current == null) {
			return; // Ignore system cursor clicks in lieu of the fake cursor.
		}
		if (EventSystem.current.currentSelectedGameObject != gameObject) {
			return;
		}

		doubleClickTicks++;
		dbclickFinished = PauseScript.a.relativeTime + doubleClickTime;
		if (doubleClickTicks == 2) {
			gameObject.SendMessage("DoubleClick");
			doubleClickTicks = 0;
		}
	}
}
