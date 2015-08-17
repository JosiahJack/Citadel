using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//public string[] weaponInventoryText = new string[]{"","","","","","",""};

[System.Serializable]
public class WeaponShotsInventory : MonoBehaviour {

	Text text;
	public int shotSlotnum = 0;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		text.text = WeaponShotsText.weaponShotsInventoryText[shotSlotnum];
	}
}


/*#pragma strict

public var weaponInventoryText = new Array();
var wepbutton1 : GameObject;
var wepbutton2 : GameObject;
var wepbutton3 : GameObject;
var wepbutton4 : GameObject;
var wepbutton5 : GameObject;
var wepbutton6 : GameObject;
var wepbutton7 : GameObject;

function Start () {
	for (var i=0;i<7;i++) {
		weaponInventoryText[i] = "";
	}
}

function Update () {
	wepbutton1.text = weaponInventoryText[0];
	wepbutton2.text = weaponInventoryText[1];
	wepbutton3.text = weaponInventoryText[2];
	wepbutton4.text = weaponInventoryText[3];
	wepbutton5.text = weaponInventoryText[4];
	wepbutton6.text = weaponInventoryText[5];
	wepbutton7.text = weaponInventoryText[6];
}
*/