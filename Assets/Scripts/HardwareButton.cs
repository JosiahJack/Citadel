using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HardwareButton : MonoBehaviour {
	//[SerializeField] private GameObject iconman;
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	public CenterTabButtons ctb;

	public void PtrEnter () {
		GUIState.a.isBlocking = true;
	}

	public void PtrExit () {
		GUIState.a.isBlocking = false;
	}

	void HardwareClick () {
		SFX.PlayOneShot(SFXClip);
		//centerTabManager.GetComponent<CenterTabButtons>().TabButtonClickSilent(4);
		if (ctb != null) ctb.TabButtonClickSilent(4);
		MFDManager.a.OpenEReaderInItemsTab();
	}

	[SerializeField] private Button HwButton = null; // assign in the editor
	void Start() {
		HwButton.onClick.AddListener(() => { HardwareClick();});
	}
}
