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
		invslot = WeaponInventory.WepInventoryInstance.weaponInventoryIndices[WepButtonIndex];
		if (invslot >= 0 && WeaponCurrent.WepInstance.weaponCurrent >= 0) {
			if (ammoiconman.activeInHierarchy) ammoiconman.GetComponent<AmmoIconManager>().SetAmmoIcon(invslot,WeaponAmmo.a.wepLoadedWithAlternate[WeaponCurrent.WepInstance.weaponCurrent]);
		}

        if (gameObject.activeInHierarchy && SFX != null && SFXClip != null) SFX.PlayOneShot(SFXClip);
		WeaponCurrent.WepInstance.weaponCurrent = WepButtonIndex;				//Set current weapon
		WeaponCurrent.WepInstance.weaponIndex = useableItemIndex;				//Set current weapon
		WeaponCurrent.WepInstance.UpdateHUDAmmoCountsEither();
		if (useableItemIndex != -1) iconman.GetComponent<WeaponIconManager>().SetWepIcon(useableItemIndex);    //Set weapon icon for MFD
		if (weptextman.activeInHierarchy && useableItemIndex != -1) weptextman.GetComponent<WeaponTextManager>().SetWepText(useableItemIndex); //Set weapon text for MFD
		if (useableItemIndex != -1) MFDManager.a.SendInfoToItemTab(useableItemIndex);

	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { WeaponInvClick();});
	}
}
