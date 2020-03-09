using UnityEngine;
using System.Collections;

public class ChargeStation : MonoBehaviour {
	public float amount = 170;  //default to 2/3 of 255, the total energy player can have
	public float resetTime = 150; //150 seconds
	public bool requireReset;
	public float minSecurityLevel = 100;
	public float damageOnUse = 0f; 
	private float nextthink;
	public string target;
	public string argvalue;
	public string rechargeMsg;
	public int rechargeMsgLingdex = 1;
	public string usedMsg;
	public int usedMsgLingdex = 0;
	// private float maxResetTime = 10f;
	
	void Awake () {
		nextthink = Time.time;
	}

	void Start () {
		if (string.IsNullOrWhiteSpace(rechargeMsg)) {
			if (rechargeMsgLingdex < Const.a.stringTable.Length)
				rechargeMsg = Const.a.stringTable[rechargeMsgLingdex];
		}

		if (string.IsNullOrWhiteSpace(usedMsg)) {
			if (usedMsgLingdex < Const.a.stringTable.Length)
				usedMsg = Const.a.stringTable[usedMsgLingdex];
		}
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > minSecurityLevel) {
			MFDManager.a.BlockedBySecurity (transform.position);
			//Debug.Log("Failed to use charge station, minSecurityLevel was " + minSecurityLevel.ToString() + ", while level security is " + LevelManager.a.GetCurrentLevelSecurity().ToString());
			return;
		}
		
		if (nextthink < Time.time) {
            ud.owner.GetComponent<PlayerReferenceManager>().playerCapsule.GetComponent<PlayerEnergy>().GiveEnergy(amount, 1);
			if (damageOnUse > 0f) {
				DamageData dd = new DamageData();
				dd.damage = damageOnUse;
				HealthManager hm = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsule.GetComponent<HealthManager>();
				if (hm.health <= dd.damage) dd.damage = hm.health - 1; // don't ever kill the player from this, way too cheap
				if (dd.damage > 0) hm.TakeDamage(dd);  // ouch it zapped me...that really hurt Charlie, that hurt my finger, owhow, OW! ow, hahahow ow! OWW!  Charlie zapped my finger (it helps if you use a British accent)
			}
			Const.sprint(usedMsg, ud.owner);
			if (requireReset) {
				nextthink = Time.time + resetTime;
			}

			if (target != "" && target != " " && target != "  ") {
				ud.argvalue = argvalue;
				TargetIO tio = GetComponent<TargetIO>();
				if (tio != null) {
					ud.SetBits(tio);
				} else {
					Debug.Log("BUG: no TargetIO.cs found on an object with a ChargeStation.cs script!  Trying to call UseTargets without parameters!");
				}
				Const.a.UseTargets(ud,target);
			}
		} else {
			Const.sprint(rechargeMsg, ud.owner);
		}
	}

	public void ForceRecharge() {
		nextthink = 0;
	}
}