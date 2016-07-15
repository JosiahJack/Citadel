using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GrenadeButtonScript : MonoBehaviour {
	public int GrenButtonIndex;
	public GameObject playerCamera;
	public int useableItemIndex;
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject textman;
	private int itemLookup;
	private Texture2D cursorTexture;
	private Vector2 cursorHotspot;

	public void PtrEnter () {
		GUIState.a.isBlocking = true;
		playerCamera.GetComponent<MouseLookScript>().overButton = true;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = 1;
        playerCamera.GetComponent<MouseLookScript>().currentButton = gameObject;
	}
	
	public void PtrExit () {
		GUIState.a.isBlocking = false;
		playerCamera.GetComponent<MouseLookScript>().overButton = false;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = -1;
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