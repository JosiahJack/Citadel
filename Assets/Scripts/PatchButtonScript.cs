using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PatchButtonScript : MonoBehaviour, IPointerClickHandler {
	public int PatchButtonIndex;
	public GameObject playerCamera;
	public int useableItemIndex;
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject textman;
	private int itemLookup;
	private Texture2D cursorTexture;
	private Vector2 cursorHotspot;

	public void PtrEnter () {
		GUIState.isBlocking = true;
        playerCamera.GetComponent<MouseLookScript>().overButton = true;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = 2;
        playerCamera.GetComponent<MouseLookScript>().currentButton = gameObject;
    }
	
	public void PtrExit () {
		GUIState.isBlocking = false;
        playerCamera.GetComponent<MouseLookScript>().overButton = false;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = -1;
    }

	void PatchInvClick () {
		itemLookup = PatchCurrent.PatchInstance.patchInventoryIndices[PatchButtonIndex];
		if (itemLookup < 0)
			return;

		iconman.GetComponent<ItemIconManager>().SetItemIcon(itemLookup);    //Set weapon icon for MFD
		textman.GetComponent<ItemTextManager>().SetItemText(itemLookup); //Set weapon text for MFD
		PatchCurrent.PatchInstance.patchCurrent = PatchButtonIndex;			//Set current weapon
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Left) {
			PatchInvClick();
		} else {
			if (eventData.button == PointerEventData.InputButton.Middle) {
				//print("Middle Click!\n");
			} else {
				if (eventData.button == PointerEventData.InputButton.Right) {
					if (!playerCamera.GetComponent<MouseLookScript>().holdingObject) {
						cursorTexture = Const.a.useableItemsFrobIcons[useableItemIndex];
						cursorHotspot = new Vector2 (cursorTexture.width/2, cursorTexture.height/2);
						Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
						Cursor.visible = true;
						Cursor.lockState = CursorLockMode.None;
						playerCamera.GetComponent<MouseLookScript>().inventoryMode = true;  // inventory mode turned on
						playerCamera.GetComponent<MouseLookScript>().holdingObject = true;
						playerCamera.GetComponent<MouseLookScript>().heldObjectIndex = useableItemIndex;
					}
				}
			}
		}
	}
	
}