using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponButton : MonoBehaviour {
    public GameObject playerCamera;
    public int useableItemIndex;
	public int WepButtonIndex;
    public GameObject iconman;
	//public MFDManager mfdManager;
	public GameObject ammoiconman;
	public GameObject weptextman;
	public WeaponButtonsManager wbm;
	public AudioSource SFX = null; // assign in the editor
	public AudioClip SFXClip = null; // assign in the editor
	private int invslot;

	public void WeaponInvClick () {
		if (WeaponCurrent.WepInstance.reloadFinished > PauseScript.a.relativeTime) return;

		// invslot = WeaponInventory.WepInventoryInstance.weaponInventoryIndices[WepButtonIndex];
		// if (invslot >= 0 && WeaponCurrent.WepInstance.weaponCurrent >= 0) {
			// if (ammoiconman.activeInHierarchy) ammoiconman.GetComponent<AmmoIconManager>().SetAmmoIcon(invslot,WeaponAmmo.a.wepLoadedWithAlternate[WeaponCurrent.WepInstance.weaponCurrent]);
		// }

		WeaponCurrent.WepInstance.WeaponChange(useableItemIndex, WepButtonIndex);
		if (gameObject.activeInHierarchy && SFX != null && SFXClip != null) SFX.PlayOneShot(SFXClip);
	}
		// gameObject.SetActive(true);
		// StartCoroutine(WeaponChangeOnClick());
	// }

	// IEnumerator WeaponChangeOnClick() {
		// int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(useableItemIndex); // Get index into the list of 16 weapons
		// float delay = Const.a.reloadTime[wep16index];
		// WeaponCurrent.WepInstance.reloadFinished = PauseScript.a.relativeTime + delay;
		// WeaponCurrent.WepInstance.lerpStartTime = PauseScript.a.relativeTime;
		// WeaponCurrent.WepInstance.reloadContainer.transform.localPosition = WeaponCurrent.WepInstance.reloadContainerOrigin; // Pop it back to start to be sure, in case it was partly lerping.
		// yield return new WaitForSeconds(delay/2f); // Allow time for weapon to go below bottom edge of screen, or at least mostly.

        // if (gameObject.activeInHierarchy && SFX != null && SFXClip != null) SFX.PlayOneShot(SFXClip);
		// WeaponCurrent.WepInstance.weaponCurrent = WepButtonIndex;				//Set current weapon 7 slot
		// WeaponCurrent.WepInstance.weaponIndex = useableItemIndex;				//Set current weapon inventory lookup index
		// WeaponCurrent.WepInstance.UpdateHUDAmmoCountsEither();
		// if (useableItemIndex != -1) iconman.GetComponent<WeaponIconManager>().SetWepIcon(useableItemIndex);    //Set weapon icon for MFD
		// if (weptextman.activeInHierarchy && useableItemIndex != -1) weptextman.GetComponent<WeaponTextManager>().SetWepText(useableItemIndex); //Set weapon text for MFD
		// if (useableItemIndex != -1) MFDManager.a.SendInfoToItemTab(useableItemIndex);
	// }

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { WeaponInvClick();});
	}
}
