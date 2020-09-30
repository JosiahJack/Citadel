using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PatchCurrent : MonoBehaviour {
	[SerializeField] public int patchCurrent = new int(); // save
	[SerializeField] public int patchIndex = new int(); // save
	public Text[] patchCountsTextObjects;
	//public int[] patchInventoryIndices = new int[]{0,1,2,3,4,5,6};
	public static PatchCurrent PatchInstance;
	public PatchButton[] patchButtonScripts;
	public MouseLookScript mls;
	
	void Awake() {
		PatchInstance = this;
		PatchInstance.patchCurrent = 0; // Current slot in the patch inventory (7 slots)
		PatchInstance.patchIndex = 0; // Current index to the look-up tables
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (GetInput.a != null) {
				if (GetInput.a.Patch()) {
					if (PatchInventory.PatchInvInstance.patchCounts[patchCurrent] > 0) {
						patchButtonScripts[patchCurrent].DoubleClick();
					} else {
						Const.sprint(Const.a.stringTable[324],mls.player); // Out of patches.
					}
				}

				if (GetInput.a.PatchCycUp()) {
					PatchCycleUp();
				}

				if (GetInput.a.PatchCycDown()) {
					PatchCycleDown();
				}
			}
		}
	}

	public void PatchCycleDown() {
		int nextIndex = patchCurrent - 1; // add 1 to get slot above this
		if (nextIndex < 0) nextIndex = 6; // wraparound to top
		int countCheck = 0;
		bool noPatches = (PatchInventory.PatchInvInstance.patchCounts[nextIndex] <= 0);
		while (noPatches) {
			countCheck++;
			if (countCheck > 13) return; // no weapons!  don't runaway loop
			nextIndex--;
			if (nextIndex < 0) nextIndex = 6;
			noPatches = (PatchInventory.PatchInvInstance.patchCounts[nextIndex] <= 0);
		}
		MFDManager.a.ctb.TabButtonClickSilent(0,true);
		patchButtonScripts[nextIndex].PatchInvClick();
	}

	public void PatchCycleUp() {
		int nextIndex = patchCurrent + 1; // add 1 to get slot above this
		if (nextIndex > 6) nextIndex = 0; // wraparound to bottom
		int countCheck = 0;
		bool noPatches = (PatchInventory.PatchInvInstance.patchCounts[nextIndex] <= 0);
		while (noPatches) {
			countCheck++;
			if (countCheck > 13) return; // no grenades!  don't runaway loop
			nextIndex++;
			if (nextIndex > 6) nextIndex = 0;
			noPatches = (PatchInventory.PatchInvInstance.patchCounts[nextIndex] <= 0);
		}
		MFDManager.a.ctb.TabButtonClickSilent(0,true);
		patchButtonScripts[nextIndex].PatchInvClick();
	}
}
