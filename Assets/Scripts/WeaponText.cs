using UnityEngine;
using System.Collections;

public class WeaponText : MonoBehaviour {
	public string[] weaponInventoryText;
	public int[] weaponInventoryIndices;
	[SerializeField] public string[] weaponInvTextSource;
	public static WeaponText WepTextInstance;
	
	void Awake() {
		WepTextInstance = this;
		WepTextInstance.weaponInventoryText = new string[]{"MAGPULSE","DARTGUN","PISTOL","PIPE","STUNGUN","",""};;
		WepTextInstance.weaponInventoryIndices = new int[]{8,2,9,6,15,-1,-1};
	}
}
