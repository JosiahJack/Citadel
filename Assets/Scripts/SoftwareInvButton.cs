using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftwareInvButton : MonoBehaviour {
	public int index = 0;

	public void DoubleClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		SoftInvClick();
	}

    public void SoftInvClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		switch(index) {
			case 0:
					// Drill
					Inventory.a.pulserButtonText.Select(false);
					Inventory.a.drillButtonText.Select(true);
					Inventory.a.isPulserNotDrill = false;
					Utils.PlayOneShotSavable(Inventory.a.SFX,Inventory.a.SFXChangeWeapon);
					break;
			case 1:
					// Pulser
					Inventory.a.pulserButtonText.Select(true);
					Inventory.a.drillButtonText.Select(false);
					Inventory.a.isPulserNotDrill = true;
					Utils.PlayOneShotSavable(Inventory.a.SFX,Inventory.a.SFXChangeWeapon);
					break;
			case 2:
					// CyberShield
					if (MouseLookScript.a.inCyberSpace) {
						Const.sprint(Const.a.stringTable[461],Const.a.player1);
					} else {
						Const.sprint(Const.a.stringTable[460],Const.a.player1);
					}
					break;
			case 3:
					// Turbo
					if (MouseLookScript.a.inCyberSpace) {
						Inventory.a.UseTurbo();
						GUIState.a.PtrHandler(false,false,ButtonType.None,null);
					} else {
						Const.sprint(Const.a.stringTable[460],Const.a.player1);
					}
					break;
			case 4:
					// Decoy
					if (MouseLookScript.a.inCyberSpace) {
						Inventory.a.UseDecoy();
						GUIState.a.PtrHandler(false,false,ButtonType.None,null);
					} else {
						Const.sprint(Const.a.stringTable[460],Const.a.player1);
					}
					break;
			case 5:
					// Recall
					if (MouseLookScript.a.inCyberSpace) {
						Inventory.a.UseRecall();
						GUIState.a.PtrHandler(false,false,ButtonType.None,null);
					} else {
						Const.sprint(Const.a.stringTable[460],Const.a.player1);
					}
					break;
			case 6:
					// Games
					if (MouseLookScript.a.inCyberSpace) {
						Const.sprint(Const.a.stringTable[443],Const.a.player1);
					} else {
						// UPDATE: Add HUD minigames
						Const.sprint(Const.a.stringTable[309],Const.a.player1); // Trioptimum Funpack Module, don't play on company time!
					}
					break;
		}
	}
}
