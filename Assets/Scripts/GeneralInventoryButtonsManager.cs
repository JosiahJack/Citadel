using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GeneralInventoryButtonsManager : MonoBehaviour {
	[SerializeField] private GameObject[] genButtons;

	void Update() {
		for (int i=0; i<14; i++) {
			if (GeneralInventory.GeneralInventoryInstance.generalInventoryIndexRef[i] > -1) {
				genButtons[i].SetActive(true);
			} else {
				genButtons[i].SetActive(false);
			}
		}
	}
}
