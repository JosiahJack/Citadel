using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftwareInvButton : MonoBehaviour {
	public int index = 0;
	public MouseLookScript mls;

	public void DoubleClick() {
		SoftInvClick();
	}

    public void SoftInvClick() {
		switch(index) {
			case 0:
					// Drill
					Inventory.a.pulserButtonText.Select(false);
					Inventory.a.drillButtonText.Select(true);
					Inventory.a.isPulserNotDrill = false;
					if (Inventory.a.SFX != null && Inventory.a.SFXChangeWeapon != null) Inventory.a.SFX.PlayOneShot(Inventory.a.SFXChangeWeapon);
					break;
			case 1:
					// Pulser
					Inventory.a.pulserButtonText.Select(true);
					Inventory.a.drillButtonText.Select(false);
					Inventory.a.isPulserNotDrill = true;
					if (Inventory.a.SFX != null && Inventory.a.SFXChangeWeapon != null) Inventory.a.SFX.PlayOneShot(Inventory.a.SFXChangeWeapon);
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
						Inventory.a.UseTurbo();
						GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
					} else {
						Const.sprint(Const.a.stringTable[460],Const.a.player1);
					}
					break;
			case 4:
					// Decoy
					if (mls.inCyberSpace) {
						Inventory.a.UseDecoy();
						GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
					} else {
						Const.sprint(Const.a.stringTable[460],Const.a.player1);
					}
					break;
			case 5:
					// Recall
					if (mls.inCyberSpace) {
						Inventory.a.UseRecall();
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
