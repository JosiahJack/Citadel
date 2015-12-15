using UnityEngine;
using System.Collections;

public class PatchCurrent : MonoBehaviour {
	[SerializeField] public int patchCurrent = new int();
	[SerializeField] public int patchIndex = new int();
	public int[] patchInventoryIndices = new int[]{0,1,2,3,4,5,6};
	public static PatchCurrent PatchInstance;
	
	void Awake() {
		PatchInstance = this;
		PatchInstance.patchCurrent = 0; // Current slot in the grenade inventory (7 slots)
		PatchInstance.patchIndex = 0; // Current index to the grenade look-up tables
	}
}
