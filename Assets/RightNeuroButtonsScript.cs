using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RightNeuroButtonsScript : MonoBehaviour {

	[SerializeField] private Button UnknownTabButton = null; // assign in the editor
	[SerializeField] private Button CompassTabButton = null; // assign in the editor
	[SerializeField] private Button DataReaderTabButton = null; // assign in the editor
	[SerializeField] private Button Unknown2TabButton = null; // assign in the editor
	[SerializeField] private Button Unknown3TabButton = null; // assign in the editor
	[SerializeField] private Sprite EmptyNeuroTabSprite = null; // assign in the editor
	[SerializeField] private AudioSource TabSFX = null; // assign in the editor
	[SerializeField] private AudioClip TabSFXClip = null; // assign in the editor

	#region For DataReader
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject itemtextman;
	[SerializeField] private LeftMFDTabs LeftTabManager = null; // assign in the editor
	[SerializeField] private CenterMFDTabs CenterTabManager = null; // assign in the editor
	[SerializeField] private TabButtonsScript LeftTabButtonsManager = null; // assign in the editor
	#endregion

	//private TabButtonsScript LeftMFDTabsScript = TabManager.GetComponent <TabButtonsScript> ();

	public bool CompassActivated = false;
	public bool DataReaderActivated = false;

	public void TabButtonClick (int tabNum) {
		TabSFX.PlayOneShot(TabSFXClip);
		switch (tabNum) {
		case 2: //Data Reader
			Debug.Log ("Data Reader Activated");
			LeftTabButtonsManager.TabButtonClick (1);
			iconman.GetComponent<ItemIconManager>().SetItemIcon(0);    //Set Data Reader icon for MFD
			itemtextman.GetComponent<ItemTextManager>().SetItemText(7);    //Set Data Reader text for MFD
			DataReaderActivated = true;
			break;
		}
	}
}
