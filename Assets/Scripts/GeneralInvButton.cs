using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GeneralInvButton : MonoBehaviour {
    public int GeneralInvButtonIndex;
    public int useableItemIndex;
	public PlayerEnergy playerEnergy;
	public PlayerHealth playerHealth;
	public GeneralInventory playerGenInv;
	public MFDManager mfdManager;
	private bool reduce = false;

    void GeneralInvClick() {
        GeneralInvCurrent.GeneralInvInstance.generalInvCurrent = GeneralInvButtonIndex;  //Set current
		mfdManager.SendInfoToItemTab(useableItemIndex);
    }

    public void DoubleClick() {
        reduce = false;

        if (useableItemIndex == 52 || useableItemIndex == 53 || useableItemIndex == 55) {
            switch (useableItemIndex) {
                case 52:
					if (playerEnergy.energy >= 255f) {
						Const.sprint(Const.a.stringTable[303],playerHealth.mainPlayerParent.GetComponent<PlayerReferenceManager>().playerCapsule);
						reduce = false;
					} else {
						playerEnergy.GiveEnergy(83f,0);
						reduce = true;
					}
                    break;
                case 53:
					if (playerEnergy.energy >= 255f) {
						Const.sprint(Const.a.stringTable[303],playerHealth.mainPlayerParent.GetComponent<PlayerReferenceManager>().playerCapsule);
						reduce = false;
					} else {
						playerEnergy.GiveEnergy(255f,0);
						reduce = true;
					}
                    break;
                case 55:
					if (playerHealth.hm.health >= playerHealth.hm.maxhealth) {
						Const.sprint(Const.a.stringTable[304],playerHealth.mainPlayerParent.GetComponent<PlayerReferenceManager>().playerCapsule);
					} else {
						playerHealth.hm.health = playerHealth.hm.maxhealth;
					}
                    reduce = true;
                    break;
            }
        } else {
            mfdManager.SendInfoToItemTab(useableItemIndex);
            mfdManager.OpenTab(1, true, MFDManager.TabMSG.None, useableItemIndex, MFDManager.handedness.LeftHand);
            GeneralInvCurrent.GeneralInvInstance.generalInvCurrent = GeneralInvButtonIndex;  //Set current
        }

		if (reduce) playerGenInv.generalInventoryIndexRef[GeneralInvButtonIndex] = -1;
	}

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { GeneralInvClick(); });
    }

    void Update() {
        useableItemIndex = GeneralInventory.GeneralInventoryInstance.generalInventoryIndexRef[GeneralInvButtonIndex];
    }
}
