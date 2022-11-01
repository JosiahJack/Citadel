using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GrenadeButton : MonoBehaviour {
	public int GrenButtonIndex;
	public int useableItemIndex;
	public AudioClip SFXClick;
	public AudioSource SFX;

	private int itemLookup;
	private Texture2D cursorTexture;
	private Vector2 cursorHotspot;

	public void PtrEnter () {
		GUIState.a.PtrHandler(true,true,ButtonType.Grenade,gameObject);
		MouseLookScript.a.currentButton = gameObject;
	}

	public void PtrExit () {
		GUIState.a.PtrHandler(false,false,ButtonType.None,null);
    }

	void DoubleClick() {
		// Put grenade in the player's hand (cursor)
		MouseLookScript.a.UseGrenade(useableItemIndex);
	}

	public void GrenadeInvClick () {
		MFDManager.a.SendInfoToItemTab(useableItemIndex);
		Inventory.a.grenadeCurrent = GrenButtonIndex;  //Set current
		Inventory.a.grenadeIndex = useableItemIndex;  //Set current
		Utils.PlayOneShotSavable(SFX,SFXClick);
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { GrenadeInvClick();});
	}
}
