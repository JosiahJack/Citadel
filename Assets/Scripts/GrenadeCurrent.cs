using UnityEngine;
using System.Collections;

public class GrenadeCurrent : MonoBehaviour {
	[SerializeField] public int grenadeCurrent = new int();
	[SerializeField] public int grenadeIndex = new int();
	public int[] grenadeInventoryIndices = new int[]{0,1,2,3,4,5,6};
	public float nitroTimeSetting;
	public float earthShakerTimeSetting;
	public static GrenadeCurrent GrenadeInstance;
	public CapsuleCollider playerCapCollider;
	
	void Start() {
		GrenadeInstance = this;
		GrenadeInstance.grenadeCurrent = 0; // Current slot in the grenade inventory (7 slots)
		GrenadeInstance.grenadeIndex = 0; // Current index to the grenade look-up tables
		nitroTimeSetting = Const.a.nitroDefaultTime;
		earthShakerTimeSetting = Const.a.earthShDefaultTime;
	}
}
