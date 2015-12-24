using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RightNeuroButtonsScript : MonoBehaviour {
	
	[SerializeField] private Button UnknownTabButton; // assign in the editor
	[SerializeField] private Button CompassTabButton; // assign in the editor
	[SerializeField] private Button DataReaderTabButton; // assign in the editor
	[SerializeField] private Button Unknown2TabButton; // assign in the editor
	[SerializeField] private Button Unknown3TabButton; // assign in the editor
	[SerializeField] private Sprite EmptyNeuroTabSprite; // assign in the editor
	[SerializeField] private AudioSource TabSFX; // assign in the editor
	[SerializeField] private AudioClip TabSFXClip; // assign in the editor
	
	#region For DataReader
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject itemtextman;
	[SerializeField] private LeftMFDTabs LeftTabManager; // assign in the editor
	[SerializeField] private CenterMFDTabs CenterTabManager; // assign in the editor
	[SerializeField] private TabButtonsScript LeftTabButtonsManager; // assign in the editor
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