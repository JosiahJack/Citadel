using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponButtonScript : MonoBehaviour {
    public GameObject playerCamera;
    public int useableItemIndex;
	public int WepButtonIndex;
    [SerializeField] private GameObject iconman;
	[SerializeField] private GameObject itemiconman;
	[SerializeField] private GameObject itemtextman;
	[SerializeField] private GameObject ammoiconman;
	[SerializeField] private GameObject weptextman;
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	private int invslot;
	private bool alternateAmmo = false;

	public void WeaponInvClick () {
		invslot = WeaponInventory.WepInventoryInstance.weaponInventoryIndices[WepButtonIndex];
		if (invslot > 0)
			ammoiconman.GetComponent<AmmoIconManager>().SetAmmoIcon(invslot, alternateAmmo);

        SFX.PlayOneShot(SFXClip);
		iconman.GetComponent<WeaponIconManager>().SetWepIcon(useableItemIndex);    //Set weapon icon for MFD
		weptextman.GetComponent<WeaponTextManager>().SetWepText(useableItemIndex); //Set weapon text for MFD

		itemiconman.SetActive(false);    //Set weapon icon for MFD
		itemtextman.GetComponent<ItemTextManager>().SetItemText(useableItemIndex); //Set weapon text for MFD
		WeaponCurrent.WepInstance.weaponCurrent = WepButtonIndex;				//Set current weapon
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { WeaponInvClick();});
	}
}
