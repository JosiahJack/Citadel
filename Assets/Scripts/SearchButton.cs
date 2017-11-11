using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SearchButton : MonoBehaviour {
	public DataTab dataTabController;
	public TabButtons tabButtonController;
	public MouseLookScript playerCamera;
	public int[] contents;
	public int[] customIndex;
	public int j;

	void Awake () {
		for (int i=0;i<=3;i++) {
			contents[i] = -1;
			customIndex[i] = -1;
		}
	}

	public void CheckForEmpty () {
		j = 0;
		for (int i=0;i<=3;i++) {
			if (contents[i] == -1)
				j++;
		}
		if (j == 4) {
			tabButtonController.ReturnToLastTab();
		}
	}

	public void SearchButtonClick (int buttonIndex) {
		switch (buttonIndex) {
		case 0:
			playerCamera.mouseCursor.GetComponent<MouseCursor>().cursorImage = Const.a.useableItemsFrobIcons[contents[0]];
			playerCamera.heldObjectIndex = contents[0];
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().contents[0] = -1;
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().customIndex[0] = -1;
			playerCamera.holdingObject = true;
			contents[0] = -1;
			customIndex[0] = -1;
			dataTabController.searchItemImages[0].SetActive(false);
			CheckForEmpty();
			break;
		case 1:
			playerCamera.mouseCursor.GetComponent<MouseCursor>().cursorImage = Const.a.useableItemsFrobIcons[contents[1]];
			playerCamera.heldObjectIndex = contents[1];
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().contents[1] = -1;
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().customIndex[1] = -1;
			playerCamera.holdingObject = true;
			contents[1] = -1;
			customIndex[1] = -1;
			dataTabController.searchItemImages[1].SetActive(false);
			CheckForEmpty();
			break;
		case 2:
			playerCamera.mouseCursor.GetComponent<MouseCursor>().cursorImage = Const.a.useableItemsFrobIcons[contents[2]];
			playerCamera.heldObjectIndex = contents[2];
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().contents[2] = -1;
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().customIndex[2] = -1;
			playerCamera.holdingObject = true;
			contents[2] = -1;
			customIndex[2] = -1;
			dataTabController.searchItemImages[2].SetActive(false);
			CheckForEmpty();
			break;
		case 3:
			playerCamera.mouseCursor.GetComponent<MouseCursor>().cursorImage = Const.a.useableItemsFrobIcons[contents[3]];
			playerCamera.heldObjectIndex = contents[3];
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().contents[3] = -1;
			playerCamera.currentSearchItem.GetComponent<SearchableItem>().customIndex[3] = -1;
			playerCamera.holdingObject = true;
			contents[3] = -1;
			customIndex[3] = -1;
			dataTabController.searchItemImages[3].SetActive(false);
			CheckForEmpty();
			break;
		}
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
	}
}
