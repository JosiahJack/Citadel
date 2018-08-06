using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PatchInventory : MonoBehaviour {
	public int[] patchCounts;
	public static PatchInventory PatchInvInstance;
	public Text[] patchCountTextObjects;

	void Awake () {
		PatchInvInstance = this;
		for (int i= 0; i<7; i++) {
			PatchInvInstance.patchCounts[i] = 0;
		}
	}

	public void AddPatchToInventory (int index) {
		patchCounts[index]++;

		// Update UI text
		for (int i = 0; i < 7; i++) {
			patchCountTextObjects[i].text = patchCounts [i].ToString ();
			if (i == index) patchCountTextObjects[i].color = Const.a.ssYellowText; // Yellow
			else  patchCountTextObjects[i].color = Const.a.ssGreenText; // Green
		}
	}
}
