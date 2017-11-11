using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class GeneralInvText : MonoBehaviour {
	Text text;
	public int slotnum = 0;
    //public string blankText = System.String.Empty;
    private int referenceIndex = -1;
	
	void Start () {
		text = GetComponent<Text>();
	}

	void Update () {
        referenceIndex = gameObject.transform.GetComponentInParent<GeneralInvButton>().useableItemIndex;
        if (referenceIndex > -1) {
            text.text = Const.a.useableItemsNameText[referenceIndex];
        } else {
            text.text = System.String.Empty;
        }

		if (slotnum == GeneralInvCurrent.GeneralInvInstance.generalInvCurrent) {
			text.color = new Color(0.8902f, 0.8745f, 0f); // Yellow
		} else {
			text.color = new Color(0.3725f, 0.6549f, 0.1686f); // Green
		}
	}
}