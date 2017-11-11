using UnityEngine;
using System.Collections;

public class WeaponInventory : MonoBehaviour {
	public string[] weaponInventoryText;
	public int[] weaponInventoryIndices;
    public int[] weaponInventoryAmmoIndices;
    [SerializeField] public string[] weaponInvTextSource;
	public static WeaponInventory WepInventoryInstance;
    public bool[] weaponFound;
    public bool[] hasWeapon;
	
	void Awake() {
        WepInventoryInstance = this;
        WepInventoryInstance.weaponInventoryText = new string[]{"","","","","","",""};;
        WepInventoryInstance.weaponInventoryIndices = new int[]{-1,-1,-1,-1,-1,-1,-1};
        WepInventoryInstance.weaponInventoryAmmoIndices = new int[]{-1,-1,-1,-1,-1,-1,-1};
    }

	void Update() {
		for (int i=0;i<WeaponShotsText.weaponShotsInventoryText.Length;i++) {
			WeaponShotsText.weaponShotsInventoryText[i] = GetTextForWeaponAmmo(i);
		}
	}

	string GetTextForWeaponAmmo(int index) {
		int globalLookupIndex = weaponInventoryIndices[index];
		switch (globalLookupIndex) {
			case 38:
				//Dartgun
				return "8n";
			case 42:
				// Pipe
				return System.String.Empty;
			case 44:
				// Magpulse
				return "RELOAD";
			case 45:
				// Pistol
				return "RELOAD";
		}
		return "ERROR";
	}
}
