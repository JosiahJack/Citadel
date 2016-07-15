using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GeneralInvButtonScript : MonoBehaviour {
    public int GeneralInvButtonIndex;
    public int useableItemIndex;
    [SerializeField] private GameObject iconman;
    [SerializeField] private GameObject textman;

    void GeneralInvClick() {
        iconman.GetComponent<ItemIconManager>().SetItemIcon(useableItemIndex);    //Set icon for MFD
        textman.GetComponent<ItemTextManager>().SetItemText(useableItemIndex); //Set text for MFD
        GeneralInvCurrent.GeneralInvInstance.generalInvCurrent = GeneralInvButtonIndex;  //Set current
    }

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { GeneralInvClick(); });
    }

    void Update() {
        useableItemIndex = GeneralInventory.GeneralInventoryInstance.generalInventoryIndexRef[GeneralInvButtonIndex];
    }
}
