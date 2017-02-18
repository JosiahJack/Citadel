using UnityEngine;
using System.Collections;

public class ListenerHardwareEreader : MonoBehaviour {
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	[SerializeField] public GameObject centerTabContainer;

	void  Update (){
		if (HardwareInventory.a.hasHardware[2] && GetInput.a.Email()) {
			SFX.PlayOneShot(SFXClip);
			centerTabContainer.GetComponent<CenterMFDTabs>().DisableAllTabs();
			centerTabContainer.GetComponent<CenterMFDTabs>().DataReaderContentTab.SetActive(true);
		}
	}
}