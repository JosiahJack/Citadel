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
	public AudioSource SFX = null; // assign in the editor
	public AudioClip SFXClip = null; // assign in the editor
	private int invslot;

	public void WeaponInvClick () {
		invslot = WeaponInventory.WepInventoryInstance.weaponInventoryIndices[WepButtonIndex];
		if (invslot >= 0)
			if (ammoiconman.activeInHierarchy) ammoiconman.GetComponent<AmmoIconManager>().SetAmmoIcon(invslot,WeaponCurrent.WepInstance.weaponIsAlternateAmmo);

        SFX.PlayOneShot(SFXClip);
		//if (iconman.activeInHierarchy) iconman.GetComponent<WeaponIconManager>().SetWepIcon(useableItemIndex);    //Set weapon icon for MFD
		iconman.GetComponent<WeaponIconManager>().SetWepIcon(useableItemIndex);    //Set weapon icon for MFD
		if (weptextman.activeInHierarchy) weptextman.GetComponent<WeaponTextManager>().SetWepText(useableItemIndex); //Set weapon text for MFD
		MFDManager.a.SendInfoToItemTab(useableItemIndex);
		WeaponCurrent.WepInstance.weaponCurrent = WepButtonIndex;				//Set current weapon
		WeaponCurrent.WepInstance.weaponIndex = useableItemIndex;				//Set current weapon
		WeaponCurrent.WepInstance.UpdateHUDAmmoCountsEither();
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { WeaponInvClick();});
	}
}
