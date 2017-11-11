using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GrenadeButton : MonoBehaviour {
	public int GrenButtonIndex;
	public GameObject playerCamera;
	public int useableItemIndex;
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject textman;
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
		print("Double click!");
	}

	void GrenadeInvClick () {
        //itemLookup = GrenadeCurrent.GrenadeInstance.grenadeInventoryIndices[GrenButtonIndex];
		//if (itemLookup < 0)
		//	return;

		iconman.GetComponent<ItemIconManager>().SetItemIcon(useableItemIndex);    //Set icon for MFD
		textman.GetComponent<ItemTextManager>().SetItemText(useableItemIndex); //Set text for MFD
		GrenadeCurrent.GrenadeInstance.grenadeCurrent = GrenButtonIndex;  //Set current
	}
	
	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { GrenadeInvClick();});
	}
}