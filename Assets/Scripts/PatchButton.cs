using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PatchButton: MonoBehaviour {
	public int PatchButtonIndex;
	public int useableItemIndex;
	public MFDManager mfdManager;
	public PlayerPatch pps;

	public void DoubleClick() {
		pps.ActivatePatch(useableItemIndex);
	}

	public void PatchInvClick () {
		mfdManager.SendInfoToItemTab(useableItemIndex);
		PatchCurrent.PatchInstance.patchCurrent = PatchButtonIndex;			//Set current
	}

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { PatchInvClick(); });
    }
}