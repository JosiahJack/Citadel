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
}
