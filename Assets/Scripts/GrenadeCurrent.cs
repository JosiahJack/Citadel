using UnityEngine;
using System.Collections;

public class GrenadeCurrent : MonoBehaviour {
	[SerializeField] public int grenadeCurrent = new int();
	[SerializeField] public int grenadeIndex = new int();
	public int[] grenadeInventoryIndices = new int[]{0,1,2,3,4,5,6};
	public static GrenadeCurrent Instance;
	
	void Awake() {
		Instance = this;
		Instance.grenadeCurrent = 0; // Current slot in the grenade inventory (7 slots)
		Instance.grenadeIndex = 0; // Current index to the grenade look-up tables
	}
}
