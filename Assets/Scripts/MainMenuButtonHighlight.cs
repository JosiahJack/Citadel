using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuButtonHighlight : MonoBehaviour {
	public MenuArrowKeyControls pageController;
	public int menuItemIndex;
	public Text subtext;
	public GameObject pad;
	public Sprite padlit;
	public Sprite paddrk;

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

	void DeHighlight () {
		Color tempcol = subtext.color;
		tempcol.a = 0.5f;
		subtext.color = tempcol;
		if (pad != null) {
			if (pad.GetComponent<Image>() != null) pad.GetComponent<Image>().overrideSprite = paddrk;
		}
	}

	void Highlight () {
		Color tempcol = subtext.color;
		tempcol.a = 1.0f;
		subtext.color = tempcol;
		if (pad != null) {
			if (pad.GetComponent<Image>() != null) pad.GetComponent<Image>().overrideSprite = padlit;
		}
	}

	public void CursorHighlight () {
		if (pointerEntered) return;

		Highlight();
		pageController.SetIndex(menuItemIndex);
		pointerEntered = true;

	}

	public void CursorDeHighlight () {
		if (!pointerEntered) return;

		DeHighlight();
		pageController.currentIndex = 0;
		pointerEntered = false;
	}
}
