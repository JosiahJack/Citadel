using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HardwareInventory : MonoBehaviour {
	public bool[] hasHardware; // save
	public int[] hardwareVersion; // save
	public int[] hardwareVersionSetting; // save
	//public int[] hardwareInventoryIndexRef;
	public string[] hardwareInventoryText;
	public static HardwareInventory a;
	public HardwareInventoryButtonsManager hwButtonsManager;

	void Awake() {
		a = this;
		for (int i = 0; i < 14; i++) {
			//a.hardwareInventoryIndexRef[i] = -1;
			a.hardwareVersionSetting [i] = 0; // set to version 1 setting
		}
	}

	// 0 = System Analyzer
	// 1 = Navigation Unit
	// 2 = Datareader
	// 3 = Sensaround
	// 4 = Target Identifier
	// 5 = Energy Shield
	// 6 = Biomonitor
	// 7 = Head Mounted Lantern
	// 8 = Envirosuit
	// 9 = Turbo Motion Booster
	//10 = Jump Jet Boots
	//11 = Infrared Night Sight Enhancement
}
