using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrenadeButtonScript : MonoBehaviour {
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject textman;
	[SerializeField] private int GrenButtonIndex;
	private int itemLookup;

	void GrenadeInvClick () {
		itemLookup = GrenadeCurrent.Instance.grenadeInventoryIndices[GrenButtonIndex];
		if (itemLookup < 0)
			return;

		iconman.GetComponent<ItemIconManager>().SetItemIcon(itemLookup);    //Set weapon icon for MFD
		textman.GetComponent<ItemTextManager>().SetItemText(itemLookup); //Set weapon text for MFD
		GrenadeCurrent.Instance.grenadeCurrent = GrenButtonIndex;			//Set current weapon
	}

	[SerializeField] private Button GrenButton = null; // assign in the editor
	void Start() {
		GrenButton.onClick.AddListener(() => { GrenadeInvClick();});
	}
}