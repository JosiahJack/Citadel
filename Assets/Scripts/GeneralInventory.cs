using UnityEngine;
using System.Collections;

public class GeneralInventory : MonoBehaviour {
    public int[] generalInventoryIndexRef;
    public static GeneralInventory GeneralInventoryInstance;


    void Awake() {
        GeneralInventoryInstance = this;
        for (int i = 0; i < 14; i++) {
            GeneralInventoryInstance.generalInventoryIndexRef[i] = -1;
        }
    }
}
