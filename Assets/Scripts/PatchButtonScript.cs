using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PatchButtonScript : MonoBehaviour {
	public int PatchButtonIndex;
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject textman;
	private int itemLookup;

	public void PtrEnter () {
		GUIState.isBlocking = true;
	}
	
	public void PtrExit () {
		GUIState.isBlocking = false;
	}

	void PatchInvClick () {
		itemLookup = PatchCurrent.PatchInstance.patchInventoryIndices[PatchButtonIndex];
		if (itemLookup < 0)
			return;

		iconman.GetComponent<ItemIconManager>().SetItemIcon(itemLookup);    //Set weapon icon for MFD
		textman.GetComponent<ItemTextManager>().SetItemText(itemLookup); //Set weapon text for MFD
		PatchCurrent.PatchInstance.patchCurrent = PatchButtonIndex;			//Set current weapon
	}

	[SerializeField] private Button PatchButton = null; // assign in the editor
	void Start() {
		PatchButton.onClick.AddListener(() => { PatchInvClick();});
	}
	
}