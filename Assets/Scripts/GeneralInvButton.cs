using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GeneralInvButton : MonoBehaviour {
    public int GeneralInvButtonIndex;
    public int useableItemIndex;
	private bool reduce = false;

    void GeneralInvClick() {
        Inventory.a.generalInvCurrent = GeneralInvButtonIndex;  //Set current
		useableItemIndex = Inventory.a.generalInventoryIndexRef[GeneralInvButtonIndex];
		MFDManager.a.SendInfoToItemTab(useableItemIndex);
    }

    public void DoubleClick() {
        reduce = false;
		useableItemIndex = Inventory.a.generalInventoryIndexRef[GeneralInvButtonIndex];
        if (useableItemIndex == 52 || useableItemIndex == 53 || useableItemIndex == 55) {
            switch (useableItemIndex) {
                case 52:
					if (PlayerEnergy.a.energy >= 255f) {
						Const.sprint(Const.a.stringTable[303],PlayerReferenceManager.a.playerCapsule);
						reduce = false;
					} else {
						PlayerEnergy.a.GiveEnergy(83f,0);
						reduce = true;
					}
                    break;
                case 53:
					if (PlayerEnergy.a.energy >= 255f) {
						Const.sprint(Const.a.stringTable[303],PlayerReferenceManager.a.playerCapsule);
						reduce = false;
					} else {
						PlayerEnergy.a.GiveEnergy(255f,0);
						reduce = true;
					}
                    break;
                case 55:
					if (PlayerHealth.a.hm.health >= PlayerHealth.a.hm.maxhealth) {
						Const.sprint(Const.a.stringTable[304],PlayerReferenceManager.a.playerCapsule);
					} else {
						PlayerHealth.a.hm.health = PlayerHealth.a.hm.maxhealth;
						MFDManager.a.DrawTicks(true);
					}
                    reduce = true;
                    break;
            }
        } else {
            MFDManager.a.SendInfoToItemTab(useableItemIndex);
            MFDManager.a.OpenTab(1, true, MFDManager.TabMSG.None, useableItemIndex, Handedness.LH);
            Inventory.a.generalInvCurrent = GeneralInvButtonIndex; // Set current.
        }

		if (reduce)  {
			Inventory.a.generalInventoryIndexRef[GeneralInvButtonIndex] = -1;
			GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
		}
	}

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { GeneralInvClick(); });
    }
}
