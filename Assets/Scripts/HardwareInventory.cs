using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardwareInventory : MonoBehaviour {
	public bool[] hasHardware;
	public int[] hardwareVersion;
	public static HardwareInventory a;

	void Awake() {
		a = this;
	}
}
