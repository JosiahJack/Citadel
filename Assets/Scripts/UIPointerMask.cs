using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIPointerMask : MonoBehaviour {
	private BoxCollider boxCol;
	private RectTransform rect;
	private EventTrigger evenT;
	private bool pointerEntered;

	void Awake () {
		// Create box collider for cursor entry detection.
		rect = GetComponent<RectTransform>();
		boxCol = gameObject.AddComponent<BoxCollider>();
		float width = rect.sizeDelta.x * rect.localScale.x;
		float height = rect.sizeDelta.y * rect.localScale.y;
		if (width < 0) width *= -1f;
		if (height < 0) height *= -1f; // Cannot have negative size on box colliders.
		boxCol.size = new Vector3(width,height,1f);
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

	// Old Unity UI system
	//public void OnMouseEnter() { PtrEnter(); }
	//public void OnMouseExit() { PtrExit(); }

	public void PtrEnter () {
		if (pointerEntered) return;

		GUIState.a.isBlocking = true;
		pointerEntered = true;
	}
	
	public void PtrExit () {
		if (!pointerEntered) return;

		GUIState.a.isBlocking = false;
		pointerEntered = false;
	}

	public void OnPointerDown(PointerEventData eventData) {
		if (EventSystem.current == null) {
			return; // Ignore system cursor clicks in lieu of the fake cursor.
		}
		if (EventSystem.current.currentSelectedGameObject != gameObject) {
			return;
		}

		MFDManager.a.mouseClickHeldOverGUI = true;
		Debug.Log("Mask click");
	}
}
