using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class ActivateButton : MonoBehaviour {
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
            pointerEnter.callback.AddListener((data) => { 
				OnPointerEnterDelegate((PointerEventData)data);
			});

            evenT.triggers.Add(pointerEnter);

            // Create a new entry for the PointerExit event
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => {
				OnPointerExitDelegate((PointerEventData)data);
			});

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
		MouseLookScript.a.currentButton = gameObject;
		pointerEntered = true;
	}

	public void PtrExit () {
		if (!pointerEntered) return;

		GUIState.a.ClearOverButton();
		pointerEntered = false;
	}

	public void OnActivateClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		Inventory.a.GeneralInventoryActivate();
	}
	
	void OnDestroy() {
		evenT = null;
	}
}
