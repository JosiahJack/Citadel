using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SearchButton : MonoBehaviour {
	public bool isRH = false;
	public MouseLookScript playerCamera;
	public int[] contents;
	public int[] customIndex;

	void Awake () {
		for (int i=0;i<=3;i++) {
			contents[i] = -1;
			customIndex[i] = -1;
		}
	}

	public void CheckForEmpty () {
		if (contents[0] == -1 && contents[1] == -1 && contents[2] == -1 && contents[3] == -1) {
			MFDManager.a.ReturnToLastTab(isRH);
		}
	}

	public void SearchButtonClick (int buttonIndex) {
		switch (buttonIndex) {
		case 0:
			if (contents[0] == -1) return;
			playerCamera.mouseCursor.GetComponent<MouseCursor>().cursorImage = Const.a.useableItemsFrobIcons[contents[0]];
			playerCamera.heldObjectIndex = contents[0];
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().contents[0] = -1;
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().customIndex[0] = -1;
			playerCamera.holdingObject = true;
			Const.sprint(Const.a.useableItemsNameText[contents[0]] + Const.a.stringTable[319],playerCamera.player);
			contents[0] = -1;
			customIndex[0] = -1;
			if (Const.a.InputQuickItemPickup) {
				playerCamera.AddItemToInventory(playerCamera.heldObjectIndex);
				playerCamera.ResetHeldItem();
				playerCamera.ResetCursor();
				playerCamera.holdingObject = false;
			}
			MFDManager.a.DisableSearchItemImage(0);
			CheckForEmpty();
			break;
		case 1:
			if (contents[1] == -1) return;
			playerCamera.mouseCursor.GetComponent<MouseCursor>().cursorImage = Const.a.useableItemsFrobIcons[contents[1]];
			playerCamera.heldObjectIndex = contents[1];
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().contents[1] = -1;
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().customIndex[1] = -1;
			playerCamera.holdingObject = true;
			Const.sprint(Const.a.useableItemsNameText[contents[1]] + Const.a.stringTable[319],playerCamera.player);
			contents[1] = -1;
			customIndex[1] = -1;
			if (Const.a.InputQuickItemPickup) {
				playerCamera.AddItemToInventory(playerCamera.heldObjectIndex);
				playerCamera.ResetHeldItem();
				playerCamera.ResetCursor();
				playerCamera.holdingObject = false;
			}
			MFDManager.a.DisableSearchItemImage(1);
			CheckForEmpty();
			break;
		case 2:
			if (contents[2] == -1) return;
			playerCamera.mouseCursor.GetComponent<MouseCursor>().cursorImage = Const.a.useableItemsFrobIcons[contents[2]];
			playerCamera.heldObjectIndex = contents[2];
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().contents[2] = -1;
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().customIndex[2] = -1;
			playerCamera.holdingObject = true;
			Const.sprint(Const.a.useableItemsNameText[contents[2]] + Const.a.stringTable[319],playerCamera.player);
			contents[2] = -1;
			customIndex[2] = -1;
			if (Const.a.InputQuickItemPickup) {
				playerCamera.AddItemToInventory(playerCamera.heldObjectIndex);
				playerCamera.ResetHeldItem();
				playerCamera.ResetCursor();
				playerCamera.holdingObject = false;
			}
			MFDManager.a.DisableSearchItemImage(2);
			CheckForEmpty();
			break;
		case 3:
			if (contents[3] == -1) return;
			playerCamera.mouseCursor.GetComponent<MouseCursor>().cursorImage = Const.a.useableItemsFrobIcons[contents[3]];
			playerCamera.heldObjectIndex = contents[3];
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().contents[3] = -1;
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().customIndex[3] = -1;
			playerCamera.holdingObject = true;
			Const.sprint(Const.a.useableItemsNameText[contents[3]] + Const.a.stringTable[319],playerCamera.player);
			contents[3] = -1;
			customIndex[3] = -1;
			if (Const.a.InputQuickItemPickup) {
				playerCamera.AddItemToInventory(playerCamera.heldObjectIndex);
				playerCamera.ResetHeldItem();
				playerCamera.ResetCursor();
				playerCamera.holdingObject = false;
			}
			MFDManager.a.DisableSearchItemImage(3);
			CheckForEmpty();
			break;
		}
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
	}
}
