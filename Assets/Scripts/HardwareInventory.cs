using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardwareInventory : MonoBehaviour {
	public bool[] hasHardware;
	public int[] hardwareVersion;
	public int[] hardwareInventoryIndexRef;
	public static HardwareInventory a;
	public HardwareInventoryButtonsManager hwButtonsManager;

	void Awake() {
		a = this;
		for (int i = 0; i < 14; i++) {
			a.hardwareInventoryIndexRef[i] = -1;
		}
	}
}
