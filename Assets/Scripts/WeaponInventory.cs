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
		string retval = "0";
		switch (globalLookupIndex) {
		case 36:
			//Mark3 Assault Rifle
			retval = WeaponAmmo.a.wepAmmo[0].ToString() + "mg, " + WeaponAmmo.a.wepAmmoSecondary[0].ToString() + "pn";
			break;
		case 37:
			//ER-90 Blaster
			if (WeaponAmmo.a.currentEnergyWeaponState [1] == WeaponAmmo.energyWeaponStates.Overheated) {
				retval = "OVERHEATED";
			} else {
				retval = "READY";
			}
			break;
		case 38:
			//SV-23 Dartgun
			retval = WeaponAmmo.a.wepAmmo[2].ToString() + "nd, " + WeaponAmmo.a.wepAmmoSecondary[2].ToString() + "tq";
			break;
		case 39:
			//AM-27 Flechette
			retval = WeaponAmmo.a.wepAmmo[3].ToString() + "hn, " + WeaponAmmo.a.wepAmmoSecondary[3].ToString() + "sp";
			break;
		case 40:
			//RW-45 Ion Beam
			if (WeaponAmmo.a.currentEnergyWeaponState [4] == WeaponAmmo.energyWeaponStates.Overheated) {
				retval = "OVERHEATED";
			} else {
				retval = "READY";
			}
			break;
		case 41:
			//TS-04 Laser Rapier
			retval = "";
			break;
		case 42:
			//Lead Pipe
			retval = "";
			break;
		case 43:
			//Magnum 2100
			retval = WeaponAmmo.a.wepAmmo[7].ToString() + "hw, " + WeaponAmmo.a.wepAmmoSecondary[7].ToString() + "slg";
			break;
		case 44:
			//SB-20 Magpulse
			retval = WeaponAmmo.a.wepAmmo[8].ToString() + "car, " + WeaponAmmo.a.wepAmmoSecondary[8].ToString() + "sup";
			break;
		case 45:
			//ML-41 Pistol
			retval = WeaponAmmo.a.wepAmmo[9].ToString() + "st, " + WeaponAmmo.a.wepAmmoSecondary[9].ToString() + "tef";
			break;
		case 46:
			//LG-XX Plasma Rifle
			if (WeaponAmmo.a.currentEnergyWeaponState [10] == WeaponAmmo.energyWeaponStates.Overheated) {
				retval = "OVERHEATED";
			} else {
				retval = "READY";
			}
			break;
		case 47:
			//MM-76 Railgun
			retval = WeaponAmmo.a.wepAmmo[11].ToString() + "rail";
			break;
		case 48:
			//DC-05 Riotgun
			retval = WeaponAmmo.a.wepAmmo[12].ToString() + "rub";
			break;
		case 49:
			//RF-07 Skorpion
			retval = WeaponAmmo.a.wepAmmo[13].ToString() + "sm, " + WeaponAmmo.a.wepAmmoSecondary[13].ToString() + "lg";
			break;
		case 50:
			//Sparq Beam
			if (WeaponAmmo.a.currentEnergyWeaponState [14] == WeaponAmmo.energyWeaponStates.Overheated) {
				retval = "OVERHEATED";
			} else {
				retval = "READY";
			}
			break;
		case 51:
			//DH-07 Stungun
			if (WeaponAmmo.a.currentEnergyWeaponState [15] == WeaponAmmo.energyWeaponStates.Overheated) {
				retval = "OVERHEATED";
			} else {
				retval = "READY";
			}
			break;
		}
		return retval;
	}
}
