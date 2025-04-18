using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HardwareInvButton : MonoBehaviour {
    public int HardwareInvButtonIndex;
    public int useableItemIndex;
	public Text butText;

    void HardwareInvClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		Inventory.a.hardwareInvCurrent = HardwareInvButtonIndex;  //Set current
		MFDManager.a.SendInfoToItemTab(useableItemIndex);
		if (useableItemIndex == 23 || useableItemIndex == 29
			|| useableItemIndex == 21 || useableItemIndex == 22) {

			DoubleClick(); // Activate non-toggleables on single click.
		}
	}

	public void DoubleClick() {
		MFDManager.a.mouseClickHeldOverGUI = true;
		switch(useableItemIndex) {
			case 21: // System Analyzer
				MFDManager.a.OpenTab(4,true,TabMSG.SystemAnalyzer,0,MFDManager.a.lastDataSideRH ? Handedness.RH : Handedness.LH);
				if (MFDManager.a.lastDataSideRH) {
					MFDManager.a.rightTC.SetCurrentAsLast();
					MFDManager.a.TabReset(false);
					MFDManager.a.sysAnalyzerLH.SetActive(true);
				} else {
					MFDManager.a.leftTC.SetCurrentAsLast();
					MFDManager.a.TabReset(true);
					MFDManager.a.sysAnalyzerRH.SetActive(true);
				}
				break;
			case 22: // Navigation Unit
				MFDManager.a.OpenTab(2,true,TabMSG.None,0,MFDManager.a.lastAutomapSideRH ? Handedness.RH : Handedness.LH);
				if (MFDManager.a.lastAutomapSideRH) {
					MFDManager.a.rightTC.SetCurrentAsLast();					
				} else {
					MFDManager.a.leftTC.SetCurrentAsLast();
				}
				break;
			case 23: // E-Reader
				Inventory.a.hardwareButtonManager.EReaderClick(); break;
			case 24: // Sensaround
				Inventory.a.hardwareButtonManager.SensaroundClick(); break;
			case 25: // Target Identifier
				Const.sprint(Const.a.stringTable[510],Const.a.player1); break;
			case 26: // Shield
				Inventory.a.hardwareButtonManager.ShieldClick(); break;
			case 27: // Biological Systems Monitor
				Inventory.a.hardwareButtonManager.BioClick(); break;
			case 28: // Lantern
				Inventory.a.hardwareButtonManager.LanternClick(); break;
			case 29: // Envirosuit
				Const.sprint(Const.a.stringTable[588] + " v"
							 + Inventory.a.EnvirosuitVersion()); break;
			case 30: // Booster
				Inventory.a.hardwareButtonManager.BoosterClick(); break;
			case 31: // Jumpjets
				Inventory.a.hardwareButtonManager.JumpJetsClick(); break;
			case 32: // Infrared
				Inventory.a.hardwareButtonManager.InfraredClick(); break;	
		}
    }

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { HardwareInvClick(); });
		butText.text = Const.a.stringTable[useableItemIndex + 326];
    }
}
