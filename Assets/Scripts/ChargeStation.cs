using UnityEngine;
using System.Collections;

public class ChargeStation : MonoBehaviour {

	public float amount = 170;  //default to 2/3 of 255, the total energy player can have
	public float resetTime = 150; //150 seconds
	public bool requireReset;
	public float minSecurityLevel = 0;
	private float nextthink;
	
	void Awake () {
		nextthink = Time.time;
	}

	void Use (GameObject owner) {
		//if (security<minSecurityLevel) {
			if (nextthink < Time.time) {
				//owner.GetComponent<PlayerEnergy>().energy += amount;
				if (requireReset) {
					nextthink = Time.time + resetTime;
				}
			} else {
				print("Station is recharging\n");
			}
		//}
	}
}
