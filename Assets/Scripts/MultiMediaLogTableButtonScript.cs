using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MultiMediaLogTableButtonScript : MonoBehaviour {
	public int logTableButtonIndex;
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject textman;

	void LogTableButtonClick() {
		//iconman.GetComponent<ItemIconManager>().SetItemIcon(useableItemIndex);    //Set icon for MFD
		//textman.GetComponent<ItemTextManager>().SetItemText(useableItemIndex); //Set text for MFD
		//GeneralInvCurrent.GeneralInvInstance.generalInvCurrent = logTableButtonIndex;  //Set current
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { LogTableButtonClick(); });
	}
}
