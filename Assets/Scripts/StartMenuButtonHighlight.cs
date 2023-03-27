using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartMenuButtonHighlight : MonoBehaviour {
	// Page controller not used on sub-buttons within the start menu page for
	// new game difficulty settings.
	/*[DTValidator.Optional] */public MenuArrowKeyControls pageController;
	public int menuItemIndex;
	public Text text;
	public Shadow textshadow;

	// Only used on the difficulty number buttons, looked odd on the difficulty
	// names.
	/*[DTValidator.Optional] */public Outline outlineGlow;
	public Color lit;
	public Color dark;
	public Color darkshadow;
	public Color litshadow;
	public bool usePointerHighlight = true;
	public bool darkenInCyberspace = false;
	public Color darkened;
	public Color darkenedshadow;

	private EventTrigger evenT;
	private bool pointerEntered;

	void Awake() {
		pointerEntered = false;
		if (usePointerHighlight) {
			evenT = GetComponent<EventTrigger>();
			if (evenT == null) evenT = gameObject.AddComponent<EventTrigger>();
			if (evenT != null) {
				// Create a new entry for the PointerEnter event
				EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
				pointerEnter.eventID = EventTriggerType.PointerEnter;
				pointerEnter.callback.AddListener(
					(data) => {
						OnPointerEnterDelegate((PointerEventData)data);
					}
				);

				evenT.triggers.Add(pointerEnter);

				// Create a new entry for the PointerExit event
				EventTrigger.Entry pointerExit = new EventTrigger.Entry();
				pointerExit.eventID = EventTriggerType.PointerExit;
				pointerExit.callback.AddListener(
					(data) => {
						OnPointerExitDelegate((PointerEventData)data);
					}
				);

				evenT.triggers.Add(pointerExit);
			} else Debug.Log("Failed to add EventTrigger to " + gameObject.name);
		}
	}

	void OnEnable() {
		pointerEntered = false;
		if (darkenInCyberspace && PlayerMovement.a.inCyberSpace) {
			if (textshadow != null) {
				textshadow.effectColor = darkenedshadow;
			}

			if (outlineGlow != null) outlineGlow.enabled = false;
			if (text != null) text.color = darkened;
		} else {
			DeHighlight();
		}
	}

	// Handle OnPointerEnter event, replaces OnMouseEnter
    public void OnPointerEnterDelegate(PointerEventData data) {
		CursorHighlight();
	}

	// Handle OnPointerExit event, replaces OnMouseExit
    public void OnPointerExitDelegate(PointerEventData data) {
		CursorDeHighlight();
	}

	public void DeHighlight() {
		if (darkenInCyberspace && PlayerMovement.a.inCyberSpace) return;

		if (textshadow != null) {
			textshadow.effectColor = darkshadow;
		}
		if (outlineGlow != null) outlineGlow.enabled = false;
		if (text != null) text.color = dark;
	}

	public void Highlight () {
		if (darkenInCyberspace && PlayerMovement.a.inCyberSpace) return;

		if (textshadow != null) {
			textshadow.effectColor = litshadow;
		}
		if (outlineGlow != null) outlineGlow.enabled = true;
		if (text != null) text.color = lit;
	}

	public void CursorHighlight () {
		if (darkenInCyberspace && PlayerMovement.a.inCyberSpace) return;
		if (pointerEntered) return;

		Highlight();
		if (pageController != null) pageController.SetIndex(menuItemIndex);
		pointerEntered = true;
	}

	public void CursorDeHighlight () {
		if (darkenInCyberspace && PlayerMovement.a.inCyberSpace) return;
		if (!pointerEntered) return;

		DeHighlight();
		if (pageController != null) pageController.currentIndex = 0;
		pointerEntered = false;
	}
}
