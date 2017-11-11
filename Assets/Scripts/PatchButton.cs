using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PatchButton: MonoBehaviour {
	public int PatchButtonIndex;
	public int useableItemIndex;
	public ItemTabManager itabManager;
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject textman;
	public PlayerPatch pps;

	public void DoubleClick() {
		pps.ActivatePatch(useableItemIndex);
	}

	public void PatchInvClick () {
		iconman.GetComponent<ItemIconManager>().SetItemIcon(useableItemIndex);    //Set icon for MFD
		textman.GetComponent<ItemTextManager>().SetItemText(useableItemIndex); //Set text for MFD
		PatchCurrent.PatchInstance.patchCurrent = PatchButtonIndex;			//Set current
		itabManager.lastCurrent = 1;
	}

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { PatchInvClick(); });
    }
}