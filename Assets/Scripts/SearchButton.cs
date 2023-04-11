using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SearchButton : MonoBehaviour {
	public bool isRH = false;
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
		MFDManager.a.mouseClickHeldOverGUI = true;
		switch (buttonIndex) {
		case 0:
			if (contents[0] == -1) return;
			MouseCursor.a.cursorImage = Const.a.useableItemsFrobIcons[contents[0]];
			MouseLookScript.a.heldObjectIndex = contents[0];
			if (MouseLookScript.a.currentSearchItem != null) {
				MouseLookScript.a.currentSearchItem.GetComponent<SearchableItem>().contents[0] = -1;
				MouseLookScript.a.currentSearchItem.GetComponent<SearchableItem>().customIndex[0] = -1;
			}
			MouseLookScript.a.holdingObject = true;
			Const.sprint(Const.a.useableItemsNameText[contents[0]] + Const.a.stringTable[319],MouseLookScript.a.player);
			contents[0] = -1;
			customIndex[0] = -1;
			if (Const.a.InputQuickItemPickup) {
				MouseLookScript.a.AddItemToInventory(MouseLookScript.a.heldObjectIndex,MouseLookScript.a.heldObjectCustomIndex);
				MouseLookScript.a.ResetHeldItem();
				MouseLookScript.a.ResetCursor();
				MouseLookScript.a.holdingObject = false;
			}
			MFDManager.a.DisableSearchItemImage(0);
			CheckForEmpty();
			break;
		case 1:
			if (contents[1] == -1) return;
			MouseCursor.a.cursorImage = Const.a.useableItemsFrobIcons[contents[1]];
			MouseLookScript.a.heldObjectIndex = contents[1];
			if (MouseLookScript.a.currentSearchItem != null) {
				MouseLookScript.a.currentSearchItem.GetComponent<SearchableItem>().contents[1] = -1;
				MouseLookScript.a.currentSearchItem.GetComponent<SearchableItem>().customIndex[1] = -1;
			}
			MouseLookScript.a.holdingObject = true;
			Const.sprint(Const.a.useableItemsNameText[contents[1]] + Const.a.stringTable[319],MouseLookScript.a.player);
			contents[1] = -1;
			customIndex[1] = -1;
			if (Const.a.InputQuickItemPickup) {
				MouseLookScript.a.AddItemToInventory(MouseLookScript.a.heldObjectIndex,MouseLookScript.a.heldObjectCustomIndex);
				MouseLookScript.a.ResetHeldItem();
				MouseLookScript.a.ResetCursor();
				MouseLookScript.a.holdingObject = false;
			}
			MFDManager.a.DisableSearchItemImage(1);
			CheckForEmpty();
			break;
		case 2:
			if (contents[2] == -1) return;
			MouseCursor.a.cursorImage = Const.a.useableItemsFrobIcons[contents[2]];
			MouseLookScript.a.heldObjectIndex = contents[2];
			if (MouseLookScript.a.currentSearchItem != null) {
				MouseLookScript.a.currentSearchItem.GetComponent<SearchableItem>().contents[2] = -1;
				MouseLookScript.a.currentSearchItem.GetComponent<SearchableItem>().customIndex[2] = -1;
			}
			MouseLookScript.a.holdingObject = true;
			Const.sprint(Const.a.useableItemsNameText[contents[2]] + Const.a.stringTable[319],MouseLookScript.a.player);
			contents[2] = -1;
			customIndex[2] = -1;
			if (Const.a.InputQuickItemPickup) {
				MouseLookScript.a.AddItemToInventory(MouseLookScript.a.heldObjectIndex,MouseLookScript.a.heldObjectCustomIndex);
				MouseLookScript.a.ResetHeldItem();
				MouseLookScript.a.ResetCursor();
				MouseLookScript.a.holdingObject = false;
			}
			MFDManager.a.DisableSearchItemImage(2);
			CheckForEmpty();
			break;
		case 3:
			if (contents[3] == -1) return;
			MouseCursor.a.cursorImage = Const.a.useableItemsFrobIcons[contents[3]];
			MouseLookScript.a.heldObjectIndex = contents[3];
			if (MouseLookScript.a.currentSearchItem != null) {
				MouseLookScript.a.currentSearchItem.GetComponent<SearchableItem>().contents[3] = -1;
				MouseLookScript.a.currentSearchItem.GetComponent<SearchableItem>().customIndex[3] = -1;
			}
			MouseLookScript.a.holdingObject = true;
			Const.sprint(Const.a.useableItemsNameText[contents[3]] + Const.a.stringTable[319],MouseLookScript.a.player);
			contents[3] = -1;
			customIndex[3] = -1;
			if (Const.a.InputQuickItemPickup) {
				MouseLookScript.a.AddItemToInventory(MouseLookScript.a.heldObjectIndex,MouseLookScript.a.heldObjectCustomIndex);
				MouseLookScript.a.ResetHeldItem();
				MouseLookScript.a.ResetCursor();
				MouseLookScript.a.holdingObject = false;
			}
			MFDManager.a.DisableSearchItemImage(3);
			CheckForEmpty();
			break;
		}
		GUIState.a.ClearOverButton();
	}
}
