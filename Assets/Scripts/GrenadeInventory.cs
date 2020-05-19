using UnityEngine;
using System.Collections;

public class GrenadeInventory : MonoBehaviour {
	public int[] grenAmmo; // save
	public static GrenadeInventory GrenadeInvInstance;

	void Awake () {
		GrenadeInvInstance = this;
		for (int i= 0; i<7; i++) {
			GrenadeInvInstance.grenAmmo[i] = 0;
		}
	}
}
