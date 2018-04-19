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
		mfdManager.SendInfoToItemTab(useableItemIndex);
		mfdManager.OpenTab(1,true,MFDManager.TabMSG.None,useableItemIndex,MFDManager.handedness.LeftHand);
		GeneralInvCurrent.GeneralInvInstance.generalInvCurrent = GeneralInvButtonIndex;  //Set current
		switch (useableItemIndex) {
			case 52:
				playerEnergy.GiveEnergy(83f);
				reduce = true;
				break;
			case 53:
				playerEnergy.GiveEnergy(255f);
				reduce = true;
				break;
			case 55:
				playerHealth.hm.health = playerHealth.hm.maxhealth;
				reduce = true;
				break;
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
