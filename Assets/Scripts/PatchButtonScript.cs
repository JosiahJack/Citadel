using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PatchButtonScript : MonoBehaviour {
	public int PatchButtonIndex;
	public int useableItemIndex;
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject textman;
	public PlayerPatchScript pps;

	public void DoubleClick() {
		//print("Double click!");
		pps.ActivatePatch(useableItemIndex);
	}

	public void PatchInvClick () {
		iconman.GetComponent<ItemIconManager>().SetItemIcon(useableItemIndex);    //Set icon for MFD
		textman.GetComponent<ItemTextManager>().SetItemText(useableItemIndex); //Set text for MFD
		PatchCurrent.PatchInstance.patchCurrent = PatchButtonIndex;			//Set current
	}

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { PatchInvClick(); });
    }
}