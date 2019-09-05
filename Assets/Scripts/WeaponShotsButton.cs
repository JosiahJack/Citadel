using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponShotsButton : MonoBehaviour {
	public GameObject iconman;
	public GameObject ammoiconman;
	public GameObject weptextman;
	public int WepButtonIndex;
	public AudioSource SFX = null; // assign in the editor
	public AudioClip SFXClip = null; // assign in the editor
	private int invslot;
	private float ix;
	private float iy;
	private Vector3 transMouse;
	private Matrix4x4 m;
	private Matrix4x4 inv;
	private bool alternateAmmo = false;

	public void PtrEnter () {
		GUIState.a.isBlocking = true;
	}

	public void PtrExit () {
		GUIState.a.isBlocking = false;
	}

	void WeaponInvClick () {
		invslot = WeaponInventory.WepInventoryInstance.weaponInventoryIndices[WepButtonIndex];
		if (invslot < 0)
			return;

		SFX.PlayOneShot(SFXClip);
		if (ammoiconman.activeInHierarchy) ammoiconman.GetComponent<AmmoIconManager>().SetAmmoIcon(invslot, alternateAmmo);
		if (iconman.activeInHierarchy) iconman.GetComponent<WeaponIconManager>().SetWepIcon(invslot);    //Set weapon icon for MFD
		if (weptextman.activeInHierarchy) weptextman.GetComponent<WeaponTextManager>().SetWepText(invslot); //Set weapon text for MFD
		WeaponCurrent.WepInstance.weaponCurrent = WepButtonIndex;				//Set current weapon
	}

	[SerializeField] private Button WepButton = null; // assign in the editor
	void Start() {
		WepButton.onClick.AddListener(() => { WeaponInvClick();});
	}
}
