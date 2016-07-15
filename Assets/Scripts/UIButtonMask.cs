using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButtonMask : MonoBehaviour {
    public GameObject playerCamera;
	public int overButtonType = 77;  // default to generic button
	private float doubleClickTime;
	private float dbclickFinished;
	public int doubleClickTicks;  // takes 2 to activate double click function
	public bool doubleClickEnabled = false;

	void Start () {
		if (playerCamera == null) {
			Const.sprint("Warning: UIButtonMask script could not find playerCamera");
		}
		EventTrigger pointerTrigger = GetComponent<EventTrigger>();
		if (pointerTrigger == null) {
			pointerTrigger = gameObject.AddComponent<EventTrigger>();
			if (pointerTrigger == null) {
				Const.sprint("Warning: Could not create Event Trigger for UIButtonMask");
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

		doubleClickTime = Const.a.doubleClickTime;
		dbclickFinished = Time.time;
		doubleClickTicks = 0;

		if (doubleClickEnabled) {
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
		GUIState.a.isBlocking = true;
        playerCamera.GetComponent<MouseLookScript>().overButton = true;
		playerCamera.GetComponent<MouseLookScript>().overButtonType = overButtonType; //generic button code
        playerCamera.GetComponent<MouseLookScript>().currentButton = gameObject;
		doubleClickTicks = 0;
    }

	public void PtrExit () {
		GUIState.a.isBlocking = false;
        playerCamera.GetComponent<MouseLookScript>().overButton = false;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = -1;
		doubleClickTicks = 0;
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
