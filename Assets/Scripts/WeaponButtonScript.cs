using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponButtonScript : MonoBehaviour {
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject weptextman;
	[SerializeField] private int WepButtonIndex;
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	private int invslot;

	void WeaponInvClick () {
		invslot = WeaponText.Instance.weaponInventoryIndices[WepButtonIndex];
		if (invslot < 0)
			return;

		SFX.PlayOneShot(SFXClip);
		iconman.GetComponent<WeaponIconManager>().SetWepIcon(invslot);    //Set weapon icon for MFD
		weptextman.GetComponent<WeaponTextManager>().SetWepText(invslot); //Set weapon text for MFD
		WeaponCurrent.Instance.weaponCurrent = WepButtonIndex;				//Set current weapon
	}

	[SerializeField] private Button WepButton = null; // assign in the editor
	void Start() {
		WepButton.onClick.AddListener(() => { WeaponInvClick();});
	}
}
