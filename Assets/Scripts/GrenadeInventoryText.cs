using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrenadeInventoryText : MonoBehaviour {
	Text text;
	[SerializeField] public int slotnum = 0;
	
	void Start () {
		text = GetComponent<Text>();
	}
	
	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			//text.text = WeaponText.Instance.weaponInventoryText[slotnum];
			if (slotnum == GrenadeCurrent.GrenadeInstance.grenadeCurrent) {
				text.color = Const.a.ssYellowText; // Yellow
			} else {
				text.color = Const.a.ssGreenText; // Green
			}
		}
	}
}
