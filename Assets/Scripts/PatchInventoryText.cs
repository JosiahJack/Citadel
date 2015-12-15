using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PatchInventoryText : MonoBehaviour {
	Text text;
	[SerializeField] public int slotnum = 0;
	
	void Start () {
		text = GetComponent<Text>();
	}
	
	void Update () {
		//text.text = WeaponText.Instance.weaponInventoryText[slotnum];
		if (slotnum == PatchCurrent.PatchInstance.patchCurrent) {
			text.color = new Color(0.8902f, 0.8745f, 0f); // Yellow
		} else {
			text.color = new Color(0.3725f, 0.6549f, 0.1686f); // Green
		}
	}
}
