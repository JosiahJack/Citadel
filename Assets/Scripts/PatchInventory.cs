using UnityEngine;
using System.Collections;

public class PatchInventory : MonoBehaviour {
	public int[] patchCounts;
	public static PatchInventory PatchInvInstance;

	void Awake () {
		PatchInvInstance = this;
		for (int i= 0; i<7; i++) {
			PatchInvInstance.patchCounts[i] = 0;
		}
	}
}
