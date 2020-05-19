using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class GeneralInvText : MonoBehaviour {
	Text text;
	public int slotnum = 0;
    private int referenceIndex = -1;
	
	void Start () {
		text = GetComponent<Text>();
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			referenceIndex = gameObject.transform.GetComponentInParent<GeneralInvButton>().useableItemIndex;
			if (referenceIndex > -1) {
				text.text = Const.a.useableItemsNameText[referenceIndex];
			} else {
				text.text = string.Empty;
			}

			if (slotnum == GeneralInvCurrent.GeneralInvInstance.generalInvCurrent) {
				text.color = Const.a.ssYellowText; // Yellow
			} else {
				text.color = Const.a.ssGreenText; // Green
			}
		}
	}
}