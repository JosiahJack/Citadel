using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrenadeButtonScript : MonoBehaviour {
	public int GrenButtonIndex;
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject textman;
	private int itemLookup;

	public void PtrEnter () {
		GUIState.isBlocking = true;
	}
	
	public void PtrExit () {
		GUIState.isBlocking = false;
	}

	void GrenadeInvClick () {
		itemLookup = GrenadeCurrent.GrenadeInstance.grenadeInventoryIndices[GrenButtonIndex];
		if (itemLookup < 0)
			return;

		iconman.GetComponent<ItemIconManager>().SetItemIcon(itemLookup);    //Set weapon icon for MFD
		textman.GetComponent<ItemTextManager>().SetItemText(itemLookup); //Set weapon text for MFD
		GrenadeCurrent.GrenadeInstance.grenadeCurrent = GrenButtonIndex;			//Set current weapon
	}

	[SerializeField] private Button GrenButton = null; // assign in the editor
	void Start() {
		GrenButton.onClick.AddListener(() => { GrenadeInvClick();});
	}
	
}