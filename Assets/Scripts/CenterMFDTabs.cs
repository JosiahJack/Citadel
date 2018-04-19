using UnityEngine;
using System.Collections;

public class CenterMFDTabs : MonoBehaviour {
	[SerializeField] public GameObject MainTab = null; // assign in the editor
	[SerializeField] public GameObject HardwareTab = null; // assign in the editor
	[SerializeField] public GameObject GeneralTab = null; // assign in the editor
	[SerializeField] public GameObject SoftwareTab = null; // assign in the editor
	[SerializeField] public GameObject DataReaderContentTab = null; // assign in the editor

	public void DisableAllTabs () {
		MainTab.SetActive(false);
		HardwareTab.SetActive(false);
		GeneralTab.SetActive(false);
		SoftwareTab.SetActive(false);
		DataReaderContentTab.SetActive(false);
	}
}
