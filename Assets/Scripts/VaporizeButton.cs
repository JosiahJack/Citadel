using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VaporizeButton : MonoBehaviour {
	public Image ico;
	public Text ict;

	public void PtrEnter () {
		GUIState.a.PtrHandler(true,true,GUIState.ButtonType.Generic,gameObject);
		MouseLookScript.a.currentButton = gameObject;
	}

	public void PtrExit () {
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
	}

	public void OnVaporizeClick() {
		Inventory.a.generalInventoryIndexRef[Inventory.a.generalInvCurrent] = -1; // Remove item
		Inventory.a.generalInvCurrent -= 1; // Set selection index up one in the list.
		if (Inventory.a.generalInvCurrent < 0) Inventory.a.generalInvCurrent = 0; // Bound to lowest.
		MFDManager.a.SendInfoToItemTab(Inventory.a.generalInventoryIndexRef[Inventory.a.generalInvCurrent]);
	}
}
