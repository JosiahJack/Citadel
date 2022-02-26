using UnityEngine;
using System.Collections;

public class ChargeStation : MonoBehaviour {
	// Externally modified per prefab instance
	public float amount = 170;  //default to 2/3 of 255, the total energy player can have
	public float resetTime = 150; //150 seconds
	public bool requireReset;
	public float minSecurityLevel = 100;
	public float damageOnUse = 0f; 
	public string target;
	public string argvalue;
	public string rechargeMsg;
	public int rechargeMsgLingdex = 1;
	public string usedMsg;
	public int usedMsgLingdex = 0;

	// Internal references
	[HideInInspector] public float nextthink; // save, stores the time after which this will be usable again.  Soem charge stations must recharge.
	
	void Awake() {
		nextthink = PauseScript.a.relativeTime;
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > minSecurityLevel) { MFDManager.a.BlockedBySecurity (transform.position,ud); return; }
		
		if (nextthink < PauseScript.a.relativeTime) {
			PlayerEnergy pe = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsule.GetComponent<PlayerEnergy>();
            if (pe != null) {
				if (pe.energy >= pe.maxenergy) {
					Const.sprint(Const.a.stringTable[303],ud.owner.GetComponent<PlayerReferenceManager>().playerCapsule);
					return;
				} else {
					pe.GiveEnergy(amount, 1);
				}
			}
			if (damageOnUse > 0f) {
				DamageData dd = new DamageData();
				dd.damage = damageOnUse;
				HealthManager hm = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsule.GetComponent<HealthManager>();
				if (hm != null) {
					if (hm.health <= dd.damage) dd.damage = hm.health - 1; // don't ever kill the player from this, way too cheap
					if (hm.god) dd.damage = 0;
					// No impact force here, it's a zap.
					if (dd.damage > 0) hm.TakeDamage(dd);  // ouch it zapped me...that really hurt Charlie, that hurt my finger, owhow, OW! ow, hahahow ow! OWW!  Charlie zapped my finger (it helps if you use a British accent)
				}
			}
			Const.sprintByIndexOrOverride (usedMsgLingdex, usedMsg,ud.owner);
			if (requireReset) nextthink = PauseScript.a.relativeTime + resetTime;
			if (!string.IsNullOrWhiteSpace(target)) {
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
			Const.sprintByIndexOrOverride (rechargeMsgLingdex, rechargeMsg,ud.owner);
		}
	}

	public void ForceRecharge() {
		nextthink = 0;
	}
}