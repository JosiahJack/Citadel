using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponButtonsManager : MonoBehaviour {
	public WeaponButton[] wepButtonsScripts;
	public GameObject[] wepCountsText;

	public void WeaponCycleUp() {
		if (WeaponFire.a.reloadFinished > PauseScript.a.relativeTime) return;

		int initialIndex = WeaponCurrent.a.weaponCurrent;
		if (initialIndex < 0) initialIndex = 0;
		if (initialIndex > 6) initialIndex = 0;
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

		if (wepButtonsScripts[nextIndex].gameObject.activeSelf
			&& nextIndex != initialIndex) {
			WeaponCurrent.a.WeaponChange(wepButtonsScripts[nextIndex].useableItemIndex,
										 wepButtonsScripts[nextIndex].WepButtonIndex);
		}
	}

	public void WeaponCycleDown() {
		if (WeaponFire.a.reloadFinished > PauseScript.a.relativeTime) return;

		int initialIndex = WeaponCurrent.a.weaponCurrent;
		if (initialIndex < 0) initialIndex = 0;
		if (initialIndex > 6) initialIndex = 0;
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

		if (wepButtonsScripts[nextIndex].gameObject.activeSelf
			&& nextIndex != initialIndex) {
			WeaponCurrent.a.WeaponChange(wepButtonsScripts[nextIndex].useableItemIndex,
										 wepButtonsScripts[nextIndex].WepButtonIndex);
		}
	}
}
