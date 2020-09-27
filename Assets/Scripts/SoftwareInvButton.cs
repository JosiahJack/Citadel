using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftwareInvButton : MonoBehaviour {
	public int index = 0;
	public SoftwareInventory sinv;
	public MouseLookScript mls;

	public void DoubleClick() {
		SoftInvClick();
	}

    public void SoftInvClick() {
		switch(index) {
			case 0:
					// Drill
					sinv.pulserButtonText.Select(false);
					sinv.drillButtonText.Select(true);
					sinv.isPulserNotDrill = false;
					if (sinv.SFX != null && sinv.SFXChangeWeapon != null) sinv.SFX.PlayOneShot(sinv.SFXChangeWeapon);
					break;
			case 1:
					// Pulser
					sinv.pulserButtonText.Select(true);
					sinv.drillButtonText.Select(false);
					sinv.isPulserNotDrill = true;
					if (sinv.SFX != null && sinv.SFXChangeWeapon != null) sinv.SFX.PlayOneShot(sinv.SFXChangeWeapon);
					break;
			case 2:
					// CyberShield
					if (mls.inCyberSpace) {
						Const.sprint(Const.a.stringTable[461],Const.a.player1);
					} else {
						Const.sprint(Const.a.stringTable[460],Const.a.player1);
					}
					break;
			case 3:
					// Turbo
					if (mls.inCyberSpace) {
						sinv.UseTurbo();
						GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
					} else {
						Const.sprint(Const.a.stringTable[460],Const.a.player1);
					}
					break;
			case 4:
					// Decoy
					if (mls.inCyberSpace) {
						sinv.UseDecoy();
						GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
					} else {
						Const.sprint(Const.a.stringTable[460],Const.a.player1);
					}
					break;
			case 5:
					// Recall
					if (mls.inCyberSpace) {
						sinv.UseRecall();
						GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
					} else {
						Const.sprint(Const.a.stringTable[460],Const.a.player1);
					}
					break;
			case 6:
					// Games
					if (mls.inCyberSpace) {
						Const.sprint(Const.a.stringTable[443],Const.a.player1);
					} else {
						// UPDATE: Add HUD minigames
						Const.sprint(Const.a.stringTable[309],Const.a.player1); // Trioptimum Funpack Module, don't play on company time!
					}
					break;
		}
	}
}
