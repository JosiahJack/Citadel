using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponButton : MonoBehaviour {
    public GameObject playerCamera;
    public int useableItemIndex;
	public int WepButtonIndex;
    public GameObject iconman;
	public GameObject ammoiconman;
	public GameObject weptextman;
	public AudioSource SFX = null; // assign in the editor
	public AudioClip SFXClip = null; // assign in the editor
	private int invslot;

	public void WeaponInvClick () {
		if (WeaponCurrent.a.reloadFinished > PauseScript.a.relativeTime) return;

		WeaponCurrent.a.WeaponChange(useableItemIndex, WepButtonIndex);
		if (gameObject.activeInHierarchy && SFX != null && SFXClip != null) Utils.PlayOneShotSavable(SFX,SFXClip);
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { WeaponInvClick();});
	}
}
