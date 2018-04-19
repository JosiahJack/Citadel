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
			text.color = Const.a.ssYellowText; // Yellow
		} else {
			text.color = Const.a.ssGreenText; // Green
		}
	}
}
