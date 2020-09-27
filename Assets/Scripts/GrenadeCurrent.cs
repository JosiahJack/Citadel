using UnityEngine;
using System.Collections;

public class GrenadeCurrent : MonoBehaviour {
	[SerializeField] public int grenadeCurrent = new int(); // save
	[SerializeField] public int grenadeIndex = new int(); // save
	//public int[] grenadeInventoryIndices = new int[]{0,1,2,3,4,5,6};
	public float nitroTimeSetting; // save
	public float earthShakerTimeSetting; // save
	public static GrenadeCurrent GrenadeInstance;
	public CapsuleCollider playerCapCollider;
	public MouseLookScript mls;
	public GrenadeButton[] grenButtons;
	public SoftwareInventory sinv;
	
	void Start() {
		GrenadeInstance = this;
		GrenadeInstance.grenadeCurrent = 0; // Current slot in the grenade inventory (7 slots)
		GrenadeInstance.grenadeIndex = 0; // Current index to the grenade look-up tables
		nitroTimeSetting = Const.a.nitroDefaultTime;
		earthShakerTimeSetting = Const.a.earthShDefaultTime;
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (GetInput.a != null) {
				if (GetInput.a.Grenade()) {
					if (mls.inCyberSpace) {
						sinv.UseCyberspaceItem();
					} else {
						if (GrenadeInventory.GrenadeInvInstance.grenAmmo[grenadeCurrent] > 0) {
							mls.UseGrenade(grenadeIndex);
						} else {
							Const.sprint(Const.a.stringTable[322],mls.player); // Out of grenades.
						}
					}
				}

				if (GetInput.a.GrenadeCycUp()) {
					if (mls.inCyberSpace) {
						sinv.CycleCyberSpaceItemUp();
					} else {
						GrenadeCycleUp();
					}
				}

				if (GetInput.a.GrenadeCycDown()) {
					if (mls.inCyberSpace) {
						sinv.CycleCyberSpaceItemDn();
					} else {
						GrenadeCycleDown();
					}
				}
			}
		}
	}

	public void GrenadeCycleDown() {
		int nextIndex = grenadeCurrent - 1; // add 1 to get slot above this
		if (nextIndex < 0) nextIndex = 6; // wraparound to top
		int countCheck = 0;
		bool noGrenAmmo = (GrenadeInventory.GrenadeInvInstance.grenAmmo[nextIndex] <= 0);
		while (noGrenAmmo) {
			countCheck++;
			if (countCheck > 13) return; // no weapons!  don't runaway loop
			nextIndex--;
			if (nextIndex < 0) nextIndex = 6;
			noGrenAmmo = (GrenadeInventory.GrenadeInvInstance.grenAmmo[nextIndex] <= 0);
		}
		grenButtons[nextIndex].GrenadeInvClick();
	}

	public void GrenadeCycleUp() {
		int nextIndex = grenadeCurrent + 1; // add 1 to get slot above this
		if (nextIndex > 6) nextIndex = 0; // wraparound to bottom
		int countCheck = 0;
		bool noGrenAmmo = (GrenadeInventory.GrenadeInvInstance.grenAmmo[nextIndex] <= 0);
		while (noGrenAmmo) {
			countCheck++;
			if (countCheck > 13) return; // no grenades!  don't runaway loop
			nextIndex++;
			if (nextIndex > 6) nextIndex = 0;
			noGrenAmmo = (GrenadeInventory.GrenadeInvInstance.grenAmmo[nextIndex] <= 0);
		}
		grenButtons[nextIndex].GrenadeInvClick();
	}
}
