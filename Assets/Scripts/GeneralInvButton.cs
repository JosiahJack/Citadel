using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GeneralInvButton : MonoBehaviour {
    public int GeneralInvButtonIndex;
    public int useableItemIndex;
	public ItemTabManager itabManager;
    [SerializeField] private GameObject iconman;
    [SerializeField] private GameObject textman;
	public PlayerEnergy playerEnergy;
	public PlayerHealth playerHealth;
	public GeneralInventory playerGenInv;

    void GeneralInvClick() {
        iconman.GetComponent<ItemIconManager>().SetItemIcon(useableItemIndex);    //Set icon for MFD
        textman.GetComponent<ItemTextManager>().SetItemText(useableItemIndex); //Set text for MFD
        GeneralInvCurrent.GeneralInvInstance.generalInvCurrent = GeneralInvButtonIndex;  //Set current
		itabManager.lastCurrent = 0;
    }

	public void DoubleClick() {
		bool reduce = false;

		iconman.GetComponent<ItemIconManager>().SetItemIcon(useableItemIndex);    //Set icon for MFD
		textman.GetComponent<ItemTextManager>().SetItemText(useableItemIndex); //Set text for MFD
		MFDManager.a.OpenTab(1,true,MFDManager.TabMSG.None,useableItemIndex);
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
