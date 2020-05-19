using UnityEngine;
using System.Collections;

public class GeneralInvCurrent : MonoBehaviour {
	public int generalInvCurrent = new int(); // save
	public int generalInvIndex = new int(); // save
	public static GeneralInvCurrent GeneralInvInstance;
	public GameObject vaporizeButton;
	public GameObject activateButton;
	public GameObject applyButton;
	
	void Awake() {
		GeneralInvInstance = this;
        GeneralInvInstance.generalInvCurrent = 0; // Current slot in the general inventory (14 slots)
        GeneralInvInstance.generalInvIndex = 0; // Current index to the item look-up table
	}
}
