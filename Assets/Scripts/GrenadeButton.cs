using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GrenadeButton : MonoBehaviour {
	public int GrenButtonIndex;
	public GameObject playerCamera;
	public int useableItemIndex;
	public MFDManager mfdManager;
	private int itemLookup;
	private Texture2D cursorTexture;
	private Vector2 cursorHotspot;
	public GrenadeCurrent playerGrenCurrent;

	public void PtrEnter () {
		GUIState.a.PtrHandler(true,true,GUIState.ButtonType.Grenade,gameObject);
		playerCamera.GetComponent<MouseLookScript>().currentButton = gameObject;
	}
	
	public void PtrExit () {
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
    }

	void DoubleClick() {
		// Put grenade in the player's hand (cursor)
		playerCamera.GetComponent<MouseLookScript>().UseGrenade(useableItemIndex);
		//Debug.Log("Grenade double clicked");
	}

	void GrenadeInvClick () {
		mfdManager.SendInfoToItemTab(useableItemIndex);
		GrenadeCurrent.GrenadeInstance.grenadeCurrent = GrenButtonIndex;  //Set current
	}
	
	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { GrenadeInvClick();});
	}
}