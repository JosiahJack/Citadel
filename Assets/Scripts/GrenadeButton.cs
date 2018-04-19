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
	public GrenadeInventory playerGrenInv;
	public GrenadeCurrent playerGrenCurrent;

	public void PtrEnter () {
		GUIState.a.PtrHandler(true,true,GUIState.ButtonType.Grenade,gameObject);
		playerCamera.GetComponent<MouseLookScript>().currentButton = gameObject;
	}
	
	public void PtrExit () {
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
    }

	void DoubleClick() {
		// Subtract one from grenade inventory
		switch(useableItemIndex) {
		case 7: playerGrenInv.grenAmmo[0]--; break; // Frag
		case 8: playerGrenInv.grenAmmo[3]--; break; // Concussion
		case 9: playerGrenInv.grenAmmo[1]--; break; // EMP
		case 10: playerGrenInv.grenAmmo[6]--; break; // Earth Shaker
		case 11: playerGrenInv.grenAmmo[4]--; break; // Land Mine
		case 12: playerGrenInv.grenAmmo[5]--; break; // Nitropak
		case 13: playerGrenInv.grenAmmo[2]--; break; // Gas
		}

		// Put grenade in the player's hand (cursor)
		playerCamera.GetComponent<MouseLookScript>().UseGrenade(useableItemIndex);
	}

	void GrenadeInvClick () {
		mfdManager.SendInfoToItemTab(useableItemIndex);
		GrenadeCurrent.GrenadeInstance.grenadeCurrent = GrenButtonIndex;  //Set current
	}
	
	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { GrenadeInvClick();});
	}
}