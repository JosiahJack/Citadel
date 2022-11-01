using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PatchButton: MonoBehaviour {
	public int PatchButtonIndex;
	public int useableItemIndex;
	public AudioClip SFXClick;
	public AudioSource SFX;

	public void DoubleClick() {
		PlayerPatch.a.ActivatePatch(useableItemIndex);
	}

	public void PatchInvClick (bool useSound) {
		MFDManager.a.SendInfoToItemTab(useableItemIndex);
		Inventory.a.patchCurrent = PatchButtonIndex; // Set current.
		for (int i = 0; i < 7; i++) {
			Inventory.a.patchCountTextObjects [i].color = Const.a.ssGreenText;
		}
		Inventory.a.patchCountTextObjects[PatchButtonIndex].color = Const.a.ssYellowText;
		if (useSound) Utils.PlayOneShotSavable(SFX,SFXClick);
	}

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { PatchInvClick(true); });
    }
}
