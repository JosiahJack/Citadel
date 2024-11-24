using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GrenadeButton : MonoBehaviour {
	public int GrenButtonIndex;
	public int useableItemIndex;
	public AudioSource SFX;

	private int itemLookup;
	private Texture2D cursorTexture;
	private Vector2 cursorHotspot;
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

		GUIState.a.PtrHandler(true,true,ButtonType.Grenade,gameObject);
		MouseLookScript.a.currentButton = gameObject;
		pointerEntered = true;
	}

	public void PtrExit () {
		if (!pointerEntered) return;

		GUIState.a.ClearOverButton();
		pointerEntered = false;
    }

	void DoubleClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;

		// Put grenade in the player's hand (cursor)
		MouseLookScript.a.UseGrenade(useableItemIndex);
	}

	public void GrenadeInvClick () {
		MFDManager.a.mouseClickHeldOverGUI = true;
		GrenadeInvSelect();
	}

	public void GrenadeInvSelect() {
		MFDManager.a.SendInfoToItemTab(useableItemIndex);
		Inventory.a.grenadeCurrent = GrenButtonIndex; // Set current
		Utils.PlayOneShotSavable(SFX,Const.a.sounds[80]); //changeweapon
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { GrenadeInvClick();});
	}
}
