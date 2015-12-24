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
			title.color = new Color(0.8902f, 0.8745f, 0f); // Yellow
		} else {
			title.color = new Color(0.3725f, 0.6549f, 0.1686f); // Green
		}
	}
}
