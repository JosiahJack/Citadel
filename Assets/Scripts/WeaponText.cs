using UnityEngine;
using System.Collections;

public class WeaponText : MonoBehaviour {
	public string[] weaponInventoryText = new string[]{"MAGPULSE","DARTGUN","PISTOL","PIPE","STUNGUN","",""};
	public int[] weaponInventoryIndices = new int[]{8,2,9,6,15,-1,-1};
	[SerializeField] public string[] weaponInvTextSource;
	public static WeaponText Instance;
	
	void Awake() {
		Instance = this;
	}
}
