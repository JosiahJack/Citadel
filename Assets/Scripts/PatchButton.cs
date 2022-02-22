using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PatchButton: MonoBehaviour {
	public int PatchButtonIndex;
	public int useableItemIndex;
	public MFDManager mfdManager;
	public PlayerPatch pps;
	public AudioClip SFXClick;
	public AudioSource SFX;

	public void DoubleClick() {
		pps.ActivatePatch(useableItemIndex);
	}

	public void PatchInvClick () {
		mfdManager.SendInfoToItemTab(useableItemIndex);
		Inventory.a.patchCurrent = PatchButtonIndex; // Set current.
		for (int i = 0; i < 7; i++) {
			Inventory.a.patchCountTextObjects [i].color = Const.a.ssGreenText;
		}
		Inventory.a.patchCountTextObjects[PatchButtonIndex].color = Const.a.ssYellowText;
		if (SFX != null && SFXClick != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXClick);
	}

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { PatchInvClick(); });
    }
}