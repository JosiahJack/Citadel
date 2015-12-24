using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponButtonScript : MonoBehaviour {
    public GameObject playerCamera;
    public int useableItemIndex;
    [SerializeField] private GameObject iconman;
	[SerializeField] private GameObject itemiconman;
	[SerializeField] private GameObject itemtextman;
	[SerializeField] private GameObject ammoiconman;
	[SerializeField] private GameObject weptextman;
	[SerializeField] private int WepButtonIndex;
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	private int invslot;
	private float ix;
	private float iy;
	private Vector3 transMouse;
	private Matrix4x4 m;
	private Matrix4x4 inv;
	private bool alternateAmmo = false;

	public void PtrEnter () {
		GUIState.isBlocking = true;
        playerCamera.GetComponent<MouseLookScript>().overButton = true;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = 0;
        playerCamera.GetComponent<MouseLookScript>().currentButton = gameObject;
    }

	public void PtrExit () {
		GUIState.isBlocking = false;
        playerCamera.GetComponent<MouseLookScript>().overButton = false;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = -1;
    }

	void WeaponInvClick () {
		invslot = WeaponText.WepTextInstance.weaponInventoryIndices[WepButtonIndex];
		if (invslot < 0)
			return;

		SFX.PlayOneShot(SFXClip);
		ammoiconman.GetComponent<AmmoIconManager>().SetAmmoIcon(invslot, alternateAmmo);
		iconman.GetComponent<WeaponIconManager>().SetWepIcon(invslot);    //Set weapon icon for MFD
		weptextman.GetComponent<WeaponTextManager>().SetWepText(invslot); //Set weapon text for MFD

		//itemiconman.GetComponent<ItemIconManager>().SetItemIcon(invslot+7);    //Set weapon icon for MFD
		//itemtextman.GetComponent<ItemTextManager>().SetItemText(invslot+7); //Set weapon text for MFD
		WeaponCurrent.WepInstance.weaponCurrent = WepButtonIndex;				//Set current weapon
	}

	[SerializeField] private Button WepButton = null; // assign in the editor
	void Start() {
		WepButton.onClick.AddListener(() => { WeaponInvClick();});
	}
}
