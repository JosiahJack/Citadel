using UnityEngine;
using System.Collections;

public class WeaponCurrent : MonoBehaviour {
	[SerializeField] public int weaponCurrent = new int();
	[SerializeField] public int weaponIndex = new int();
	public static WeaponCurrent WepInstance;
	public GameObject pipeViewModel;
	private bool justChangedWeap = true;
	private int lastIndex = 0;

	void Awake() {
		WepInstance = this;
		WepInstance.weaponCurrent = 0; // Current slot in the weapon inventory (7 slots)
		WepInstance.weaponIndex = 0; // Current index to the weapon look-up tables
	}

	void Update() {
		if (justChangedWeap) {
			justChangedWeap = false;
			pipeViewModel.SetActive(false);
		}

		if (lastIndex != weaponIndex) {
			justChangedWeap = true;
			lastIndex = weaponIndex;
		}
		
		switch (weaponIndex) {
		case 42:
			pipeViewModel.SetActive(true);
			break;
		}
	}
}