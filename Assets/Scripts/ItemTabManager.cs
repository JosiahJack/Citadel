using UnityEngine;
using System.Collections;

public class ItemTabManager : MonoBehaviour {
    public GameObject iconManager;
    public GameObject textManager;
    public int itemTabType;

	void Update () {
        // itemTabType's
        // 0 = Weapon
        // 1 = Grenade
        // 2 = Patch
        // 3 = General Inventory
        // 4 = Other

        switch (itemTabType) {
            case 0:
                //iconManager.GetComponent<ItemIconManager>().SetItemIcon(useableItemIndex);    //Set icon for MFD
                //textManager.GetComponent<ItemTextManager>().SetItemText(useableItemIndex); //Set text for MFD
                break;
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
            default:
                break;
        }
        //iconManager.GetComponent<ItemIconManager>().SetItemIcon(useableItemIndex);    //Set icon for MFD
        //textManager.GetComponent<ItemTextManager>().SetItemText(useableItemIndex); //Set text for MFD
    }
}
