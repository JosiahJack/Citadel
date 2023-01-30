using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class KeycodeButton : MonoBehaviour {
	public GameObject playerCamera;
	public GameObject keycodeController;
	public int index;
	private EventTrigger evenT;
	private bool pointerEntered;

	void Awake() {
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
	}

	void OnEnable() {
		pointerEntered = false;
	}

	// Handle OnPointerEnter event, replaces OnMouseEnter
    public void OnPointerEnterDelegate(PointerEventData data) { PtrEnter(); }

	// Handle OnPointerExit event, replaces OnMouseExit
    public void OnPointerExitDelegate(PointerEventData data) { PtrExit(); }

	public void PtrEnter () {
		if (pointerEntered) return;

		GUIState.a.PtrHandler(true,true,ButtonType.Generic,gameObject);
		pointerEntered = true;
	}

	public void PtrExit () {
		if (!pointerEntered) return;

		GUIState.a.PtrHandler(false,false,ButtonType.None,null);
		pointerEntered = false;
	}

	void KeycodeButtonClick () {
		keycodeController.GetComponent<KeypadKeycodeButtons>().Keypress(index);
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { KeycodeButtonClick(); });
	}
}
