using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponButtonsManager : MonoBehaviour {
	public GameObject[] wepButtons;
	public WeaponButton[] wepButtonsScripts;
	public GameObject[] wepCountsText;

	public void WeaponCycleUp() {
		int initialIndex = WeaponCurrent.a.weaponCurrent;
		int nextIndex = initialIndex + 1; // add 1 to get slot above this
		if (nextIndex > 6) nextIndex = 0; // wraparound to bottom
		int countCheck = 0;
		bool buttonNotValid = (Inventory.a.weaponInventoryIndices[nextIndex] == -1);
		while (buttonNotValid) {
			countCheck++;
			if (countCheck > 13) {
				return; // no weapons!  don't runaway loop
			}
			nextIndex++;
			if (nextIndex > 6) nextIndex = 0;
			buttonNotValid = (Inventory.a.weaponInventoryIndices[nextIndex] == -1);
		}
		if (wepButtons[nextIndex].activeSelf && nextIndex != initialIndex) wepButtonsScripts[nextIndex].WeaponInvClick ();
	}

	public void WeaponCycleDown() {
		int initialIndex = WeaponCurrent.a.weaponCurrent;
		int nextIndex = initialIndex - 1; // add 1 to get slot above this
		if (nextIndex < 0) nextIndex = 6; // wraparound to top
		int countCheck = 0;
		bool buttonNotValid = (Inventory.a.weaponInventoryIndices[nextIndex] == -1);
		while (buttonNotValid) {
			countCheck++;
			if (countCheck > 13) {
				return; // no weapons!  don't runaway loop
			}
			nextIndex--;
			if (nextIndex < 0) nextIndex = 6;
			buttonNotValid = (Inventory.a.weaponInventoryIndices[nextIndex] == -1);
		}
		if (wepButtons[nextIndex].activeSelf && nextIndex != initialIndex) wepButtonsScripts[nextIndex].WeaponInvClick ();
	}
}
