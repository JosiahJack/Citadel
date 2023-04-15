using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GeneralInvButton : MonoBehaviour {
    public int GeneralInvButtonIndex;
    public int useableItemIndex;
	public int customIndex;
	public GameObject activateButton;
	private bool reduce = false;

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => {
			GeneralInvClick();
		});
    }

    void GeneralInvClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		GeneralInvUse();
	}

	public void GeneralInvUse() {
        Inventory.a.generalInvCurrent = GeneralInvButtonIndex; //Set current
		useableItemIndex =
			Inventory.a.generalInventoryIndexRef[GeneralInvButtonIndex];

		// Access Cards
		if (GeneralInvButtonIndex == 0) {
			MFDManager.a.SendInfoToItemTab(81,-1);
		} else {
			MFDManager.a.SendInfoToItemTab(useableItemIndex,customIndex);
		}
    }

    public void DoubleClick() {
        Inventory.a.generalInvCurrent = GeneralInvButtonIndex; //Set current
		MFDManager.a.mouseClickHeldOverGUI = true;
		GeneralInvApply();
	}

	void ApplyBattery() {
		if (PlayerEnergy.a.energy >= 255f) {
			Const.sprint(Const.a.stringTable[303]);
			reduce = false;
		}

		PlayerEnergy.a.GiveEnergy(83f,EnergyType.Battery);
		reduce = true;
	}

	void ApplyIcadBattery() {
		if (PlayerEnergy.a.energy >= 255f) {
			Const.sprint(Const.a.stringTable[303]);
			reduce = false;
			return;
		}

		PlayerEnergy.a.GiveEnergy(255f,EnergyType.Battery);
		reduce = true;
	}

	void ApplyHealthkit() {
		if (PlayerHealth.a.hm.health >= PlayerHealth.a.hm.maxhealth) {
			Const.sprint(Const.a.stringTable[304]);
			reduce = false;
			return;
		}

		PlayerHealth.a.hm.health = PlayerHealth.a.hm.maxhealth;
		MFDManager.a.DrawTicks(true);
		reduce = true;
	}

	public void GeneralInvApply() {
		// Access Cards button
		if (GeneralInvButtonIndex == 0) {
			MFDManager.a.SendInfoToItemTab(81,-1);
			MFDManager.a.OpenTab(1,true,TabMSG.None, useableItemIndex,
								 Handedness.LH);
			return;
		}

        reduce = false;
		useableItemIndex =
			Inventory.a.generalInventoryIndexRef[GeneralInvButtonIndex];
		switch (useableItemIndex) {
			case 52: ApplyBattery(); break;
			case 53: ApplyIcadBattery(); break;
			case 55: ApplyHealthkit(); break;
			default:
				MFDManager.a.SendInfoToItemTab(useableItemIndex,customIndex);
				MFDManager.a.OpenTab(1,true,TabMSG.None, useableItemIndex,
									 Handedness.LH);

				// Set current.
				Inventory.a.generalInvCurrent = GeneralInvButtonIndex;
				break;
		}

		if (reduce)  {
			Inventory.a.generalInventoryIndexRef[GeneralInvButtonIndex] = -1;
			GUIState.a.ClearOverButton();
		}
	}
}
