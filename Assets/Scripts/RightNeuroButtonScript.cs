using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RightNeuroButtonsScript : MonoBehaviour {
	
	public Button UnknownTabButton; // assign in the editor
	public Button CompassTabButton; // assign in the editor
	public Button DataReaderTabButton; // assign in the editor
	public Button Unknown2TabButton; // assign in the editor
	public Button Unknown3TabButton; // assign in the editor
	public Sprite EmptyNeuroTabSprite; // assign in the editor
	public AudioSource TabSFX; // assign in the editor
	public AudioClip TabSFXClip; // assign in the editor
	
	#region For DataReader
	public GameObject iconman;
	public GameObject itemtextman;
	public LeftMFDTabs LeftTabManager; // assign in the editor
	public CenterMFDTabs CenterTabManager; // assign in the editor
	public TabButtons LeftTabButtonsManager; // assign in the editor
	#endregion
	
	//private TabButtonsScript LeftMFDTabsScript = TabManager.GetComponent <TabButtonsScript> ();
	
	public bool CompassActivated = false;
	public bool DataReaderActivated = false;
	
	public void TabButtonClick (int tabNum) {
		TabSFX.PlayOneShot(TabSFXClip);
		switch (tabNum) {
		case 2: //Data Reader
			//Debug.Log ("Data Reader Activated");
			LeftTabButtonsManager.TabButtonClick (1);
			iconman.GetComponent<ItemIconManager>().SetItemIcon(0);    //Set Data Reader icon for MFD
			itemtextman.GetComponent<ItemTextManager>().SetItemText(7);    //Set Data Reader text for MFD
			DataReaderActivated = true;
			break;
		}
	}
}