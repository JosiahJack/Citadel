using UnityEngine;
using System.Collections;

public class GeneralInvCurrent : MonoBehaviour {
	[SerializeField] public int generalInvCurrent = new int();
	[SerializeField] public int generalInvIndex = new int();
	public int[] generalInventoryIndices = new int[]{0,1,2,3,4,5,6,7,8,9,10,11,12,13};
	public static GeneralInvCurrent GeneralInvInstance;
	public GameObject vaporizeButton;
	public GameObject activateButton;
	public GameObject applyButton;
	public ItemTabManager itabManager;
	
	void Awake() {
		GeneralInvInstance = this;
        GeneralInvInstance.generalInvCurrent = 0; // Current slot in the general inventory (14 slots)
        GeneralInvInstance.generalInvIndex = 0; // Current index to the item look-up table
	}

	void Update() {
		bool setVaporizeButtonOn = false;
		bool setActivateButtonOn = false;
		bool setApplyButtonOn = false;

		switch( GeneralInvInstance.generalInvCurrent) {
			case 0: setVaporizeButtonOn = true; break;
			case 1: setVaporizeButtonOn = true; break;
			case 2: setVaporizeButtonOn = true; break;
			case 3: setVaporizeButtonOn = true; break;
			case 4: setVaporizeButtonOn = true; break;
			case 5: setVaporizeButtonOn = true; break;
			case 33: setVaporizeButtonOn = true; break;
			case 34: setVaporizeButtonOn = true; break;
			case 35: setVaporizeButtonOn = true; break;
			case 52: setActivateButtonOn = true; break;
			case 53: setActivateButtonOn = true; break;
			case 55: setActivateButtonOn = true; break;
			case 58: setVaporizeButtonOn = true; break;
			case 62: setVaporizeButtonOn = true; break;
		}

		switch (PatchCurrent.PatchInstance.patchCurrent) {
			case 0: setApplyButtonOn = true; break;
			case 1: setApplyButtonOn = true; break;
			case 2: setApplyButtonOn = true; break;
			case 3: setApplyButtonOn = true; break;
			case 4: setApplyButtonOn = true; break;
			case 5: setApplyButtonOn = true; break;
			case 6: setApplyButtonOn = true; break;
		}

		if (setVaporizeButtonOn && itabManager.lastCurrent == 0) vaporizeButton.SetActive(true);
		else vaporizeButton.SetActive(false);

		if (setActivateButtonOn && itabManager.lastCurrent == 0) activateButton.SetActive(true);
		else activateButton.SetActive(false);

		if (setApplyButtonOn && itabManager.lastCurrent == 1) applyButton.SetActive(true);
		else applyButton.SetActive(false);
	}
}
