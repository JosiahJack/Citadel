using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class EmailInventory : MonoBehaviour {
	Text title;
	public int slotnum = 0;
	
	void Start () {
		title = GetComponent<Text>();
	}
	
	void Update () {
		title.text = EmailTitle.Instance.emailInventoryTitle[slotnum];
		if (slotnum == EmailCurrent.Instance.emailCurrent) {
			title.color = Const.a.ssYellowText; // Yellow
		} else {
			title.color = Const.a.ssGreenText; // Green
		}
	}
}
