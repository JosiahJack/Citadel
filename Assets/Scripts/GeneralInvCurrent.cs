using UnityEngine;
using System.Collections;

public class GeneralInvCurrent : MonoBehaviour {
	[SerializeField] public int generalInvCurrent = new int();
	[SerializeField] public int generalInvIndex = new int();
	public int[] generalInventoryIndices = new int[]{0,1,2,3,4,5,6,7,8,9,10,11,12,13};
	public static GeneralInvCurrent GeneralInvInstance;
	
	void Awake() {
		GeneralInvInstance = this;
        GeneralInvInstance.generalInvCurrent = 0; // Current slot in the general inventory (14 slots)
        GeneralInvInstance.generalInvIndex = 0; // Current index to the item look-up table
	}
}
