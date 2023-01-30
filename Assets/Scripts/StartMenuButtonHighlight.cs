using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartMenuButtonHighlight : MonoBehaviour {
	/*[DTValidator.Optional] */public MenuArrowKeyControls pageController; //not used on sub-buttons within the start menu page for new game difficulty settings
	public int menuItemIndex;
	public Text text;
	public Shadow textshadow;
	/*[DTValidator.Optional] */public Outline outlineGlow; // Only used on the difficulty number buttons, looked odd on the difficulty names.
	public Color lit;
	public Color dark;
	public Color darkshadow;
	public Color litshadow;

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
    public void OnPointerEnterDelegate(PointerEventData data) { CursorHighlight(); }

	// Handle OnPointerExit event, replaces OnMouseExit
    public void OnPointerExitDelegate(PointerEventData data) { CursorDeHighlight(); }

	public void DeHighlight () {
		if (textshadow != null) {
			textshadow.effectColor = darkshadow;
			// textshadow.enabled = true;
		}
		if (outlineGlow != null) outlineGlow.enabled = false;
		if (text != null) text.color = dark;
	}

	public void Highlight () {
		if (textshadow != null) {
			textshadow.effectColor = litshadow;
			// textshadow.enabled = false;
		}
		if (outlineGlow != null) outlineGlow.enabled = true;
		if (text != null) text.color = lit;
	}

	public void CursorHighlight () {
		if (pointerEntered) return;

		Highlight();
		if (pageController != null) pageController.SetIndex(menuItemIndex);
		pointerEntered = true;
	}

	public void CursorDeHighlight () {
		if (!pointerEntered) return;

		DeHighlight();
		if (pageController != null) pageController.currentIndex = 0;
		pointerEntered = false;
	}
}
