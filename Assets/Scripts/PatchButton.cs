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
		for (int i = 0; i < 7; i++) {
			PatchCurrent.PatchInstance.patchCountsTextObjects [i].color = Const.a.ssGreenText;
		}
		PatchCurrent.PatchInstance.patchCountsTextObjects[PatchButtonIndex].color = Const.a.ssYellowText;
	}

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { PatchInvClick(); });
    }
}