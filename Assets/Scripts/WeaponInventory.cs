using UnityEngine;
using System.Collections;

public class WeaponInventory : MonoBehaviour {
	public string[] weaponInventoryText;
	public int[] weaponInventoryIndices; // save
    public int[] weaponInventoryAmmoIndices; // save
	[HideInInspector] 
    public string[] weaponInvTextSource;
	public static WeaponInventory WepInventoryInstance;
	public int numweapons = 0; // save
	private int globalLookupIndex;
	private string retval;
	private string zeroString;
	private string scorpSmall = "sm, ";
	private string scorpLg = "lg";

	public void RemoveWeapon(int wepButIndex) {
		// Remove the weapon by setting it to -1;
		weaponInventoryIndices[wepButIndex] = -1;
		weaponInventoryAmmoIndices[wepButIndex] = -1;
		weaponInventoryText[wepButIndex] = "-";
	}

	void Start() {
        WepInventoryInstance = this;
        WepInventoryInstance.weaponInventoryText = new string[]{"","","","","","",""};;
        WepInventoryInstance.weaponInventoryIndices = new int[]{-1,-1,-1,-1,-1,-1,-1};
        WepInventoryInstance.weaponInventoryAmmoIndices = new int[]{-1,-1,-1,-1,-1,-1,-1};	
		globalLookupIndex = -1;
		retval = "0";
		zeroString = "0";

		// Used only by MouseLookScript.AddWeaponToInventory();
		weaponInvTextSource[0] = Const.a.stringTable[264];  // ASSLT RIFLE
		weaponInvTextSource[1] = Const.a.stringTable[265];  // BLASTER
		weaponInvTextSource[2] = Const.a.stringTable[266];  // DARTGUN
		weaponInvTextSource[3] = Const.a.stringTable[267];  // FLECHETTE
		weaponInvTextSource[4] = Const.a.stringTable[268];  // ION BEAM
		weaponInvTextSource[5] = Const.a.stringTable[269];  // LASER RAPIER
		weaponInvTextSource[6] = Const.a.stringTable[270];  // PIPE
		weaponInvTextSource[7] = Const.a.stringTable[271];  // MAGNUM
		weaponInvTextSource[8] = Const.a.stringTable[272];  // MAGPULSE
		weaponInvTextSource[9] = Const.a.stringTable[273];  // PISTOL
		weaponInvTextSource[10] = Const.a.stringTable[274]; // PLASMA RIFLE
		weaponInvTextSource[11] = Const.a.stringTable[275]; // RAIL GUN
		weaponInvTextSource[12] = Const.a.stringTable[276]; // RIOT GUN
		weaponInvTextSource[13] = Const.a.stringTable[277]; // SKORPION
		weaponInvTextSource[14] = Const.a.stringTable[278]; // SPARQ BEAM
		weaponInvTextSource[15] = Const.a.stringTable[279]; // STUNGUN
    }

	public void GetNumWeapons() {
			int slot1 = WeaponInventory.WepInventoryInstance.weaponInventoryIndices [0];
			int slot2 = WeaponInventory.WepInventoryInstance.weaponInventoryIndices [1];
			int slot3 = WeaponInventory.WepInventoryInstance.weaponInventoryIndices [2];
			int slot4 = WeaponInventory.WepInventoryInstance.weaponInventoryIndices [3];
			int slot5 = WeaponInventory.WepInventoryInstance.weaponInventoryIndices [4];
			int slot6 = WeaponInventory.WepInventoryInstance.weaponInventoryIndices [5];
			int slot7 = WeaponInventory.WepInventoryInstance.weaponInventoryIndices [6];
			numweapons = 0;
			if (slot1 != -1) numweapons++;
			if (slot2 != -1) numweapons++;
			if (slot3 != -1) numweapons++;
			if (slot4 != -1) numweapons++;
			if (slot5 != -1) numweapons++;
			if (slot6 != -1) numweapons++;
			if (slot7 != -1) numweapons++;
	}

	public string GetTextForWeaponAmmo(int index) {
		globalLookupIndex = weaponInventoryIndices[index];
		retval = zeroString;
		switch (globalLookupIndex) {
		case 36:
			//Mark3 Assault Rifle
			retval = WeaponAmmo.a.wepAmmo[0].ToString() + "mg, " + WeaponAmmo.a.wepAmmoSecondary[0].ToString() + "pn";
			break;
		case 37:
			//ER-90 Blaster
			if (WeaponAmmo.a.currentEnergyWeaponHeat[index] > 80f) {
				retval = Const.a.stringTable[14]; // OVERHEATED
			} else {
				retval = Const.a.stringTable[15]; // READY
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
			if (WeaponAmmo.a.currentEnergyWeaponHeat[index] > 80f) {
				retval = Const.a.stringTable[14]; // OVERHEATED
			} else {
				retval = Const.a.stringTable[15]; // READY
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
			retval = WeaponAmmo.a.wepAmmo[7].ToString() + "hw, " + WeaponAmmo.a.wepAmmoSecondary[7].ToString() + "sg";
			break;
		case 44:
			//SB-20 Magpulse
			retval = WeaponAmmo.a.wepAmmo[8].ToString() + "cr, " + WeaponAmmo.a.wepAmmoSecondary[8].ToString() + "su";
			break;
		case 45:
			//ML-41 Pistol
			retval = WeaponAmmo.a.wepAmmo[9].ToString() + "st, " + WeaponAmmo.a.wepAmmoSecondary[9].ToString() + "tf";
			break;
		case 46:
			//LG-XX Plasma Rifle
			if (WeaponAmmo.a.currentEnergyWeaponHeat[index] > 80f) {
				retval = Const.a.stringTable[14]; // OVERHEATED
			} else {
				retval = Const.a.stringTable[15]; // READY
			}
			break;
		case 47:
			//MM-76 Railgun
			retval = WeaponAmmo.a.wepAmmo[11].ToString() + "rl";
			break;
		case 48:
			//DC-05 Riotgun
			retval = WeaponAmmo.a.wepAmmo[12].ToString() + "rb";
			break;
		case 49:
			//RF-07 Skorpion
			retval = WeaponAmmo.a.wepAmmo[13].ToString() + scorpSmall + WeaponAmmo.a.wepAmmoSecondary[13].ToString() + scorpLg;
			break;
		case 50:
			//Sparq Beam
			if (WeaponAmmo.a.currentEnergyWeaponHeat[index] > 80f) {
				retval = Const.a.stringTable[14]; // OVERHEATED
			} else {
				retval = Const.a.stringTable[15]; // READY
			}
			break;
		case 51:
			//DH-07 Stungun
			if (WeaponAmmo.a.currentEnergyWeaponHeat[index] > 80f) {
				retval = Const.a.stringTable[14]; // OVERHEATED
			} else {
				retval = Const.a.stringTable[15]; // READY
			}
			break;
		}
		return retval;
	}
}
