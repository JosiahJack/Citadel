using UnityEngine;
using System.Collections;

public class ChargeStation : MonoBehaviour {
	public float amount = 170;  //default to 2/3 of 255, the total energy player can have
	public float resetTime = 150; //150 seconds
	public bool requireReset;
	public float minSecurityLevel = 0;
	private float nextthink;
	// private float maxResetTime = 10f;
	
	void Awake () {
		nextthink = Time.time;
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity () >= minSecurityLevel) {
			MFDManager.a.BlockedBySecurity ();
			return;
		}
		
		if (nextthink < Time.time) {
            ud.owner.GetComponent<PlayerReferenceManager>().playerCapsule.GetComponent<PlayerEnergy>().GiveEnergy(amount, 1);
			Const.sprint("Energy drawn from Power Station.", ud.owner);
			if (requireReset) {
				nextthink = Time.time + resetTime;
			}
		} else {
			Const.sprint("Power Station is recharging\n", ud.owner);
		}
	}
}
