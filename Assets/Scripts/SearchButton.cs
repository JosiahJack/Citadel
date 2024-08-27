using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SearchButton : MonoBehaviour {
	public bool isRH = false;
	public int[] contents;
	public int[] customIndex;

	void Awake () {
		for (int i=0;i<=3;i++) {
			contents[i] = -1;
			customIndex[i] = -1;
		}
	}

	public void CheckForEmpty () {
		if (contents[0] == -1 && contents[1] == -1 && contents[2] == -1 && contents[3] == -1) {
			MFDManager.a.ReturnToLastTab(isRH);
		}
	}

	public void SearchButtonClick (int buttonIndex) {
		MFDManager.a.mouseClickHeldOverGUI = true;
		MouseLookScript.a.SearchButtonClick(buttonIndex,this);
		GUIState.a.ClearOverButton();
	}
}
