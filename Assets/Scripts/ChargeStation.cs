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
	public int rechargeMsgLingdex = 1;
	public int usedMsgLingdex = 0;

	// Internal references
	[HideInInspector] public float nextthink; // save, stores the time after which this will be usable again.  Soem charge stations must recharge.
	
	void Awake() {
		nextthink = PauseScript.a.relativeTime;
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > minSecurityLevel) { 
			MFDManager.a.BlockedBySecurity (transform.position);
			return;
		}
		
		if (nextthink < PauseScript.a.relativeTime) {
			if (PlayerEnergy.a.energy >= PlayerEnergy.a.maxenergy) {
				Const.sprint(303);
				return;
			} else {
				PlayerEnergy.a.GiveEnergy(amount, EnergyType.ChargeStation);
				MFDManager.a.energySurge.SetActive(true);
			}

			if (damageOnUse > 0f) {
				DamageData dd = new DamageData();

				// Don't ever kill the player from this, way too cheap.
				dd.damage = Mathf.Min(damageOnUse,PlayerHealth.a.hm.health - 1);

				// No impact force here, it's a zap.  Ouch, it zapped me...that
				// really hurt Chargie, that hurt my finger, owhow, OW! ow,
				// hahahow ow! OWW!  Chargie zapped my finger (it helps if you
				// use a British accent and refer to Charlie Bit My Finger).
				if (dd.damage > 0) PlayerHealth.a.hm.TakeDamage(dd);
			}

			Const.sprint(usedMsgLingdex);
			if (requireReset) nextthink = PauseScript.a.relativeTime + resetTime;
			ud.argvalue = argvalue;
			Const.a.UseTargets(gameObject,ud,target);
		} else {
			Const.sprint(rechargeMsgLingdex);
		}
	}

	public void ForceRecharge() {
		nextthink = 0;
	}

	public static string Save(GameObject go) {
		ChargeStation chg = go.GetComponent<ChargeStation>();
		string line = System.String.Empty;
		line = Utils.SaveRelativeTimeDifferential(chg.nextthink,"nextthink"); // float - time before recharged
		line += Utils.splitChar + Utils.FloatToString(chg.amount,"amount");
		line += Utils.splitChar + Utils.FloatToString(chg.resetTime,"resetTime");
		line += Utils.splitChar + Utils.BoolToString(chg.requireReset,"requireReset");
		line += Utils.splitChar + Utils.FloatToString(chg.minSecurityLevel,"minSecurityLevel");
		line += Utils.splitChar + Utils.FloatToString(chg.damageOnUse,"damageOnUse");
		line += Utils.splitChar + Utils.SaveString(chg.target,"target");
		line += Utils.splitChar + Utils.SaveString(chg.argvalue,"argvalue");
		line += Utils.splitChar + Utils.UintToString(chg.rechargeMsgLingdex,"rechargeMsgLingdex");
		line += Utils.splitChar + Utils.UintToString(chg.usedMsgLingdex,"usedMsgLingdex");
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		ChargeStation chg = go.GetComponent<ChargeStation>();
		if (chg == null) {
			Debug.Log("ChargeStation.Load failure, chg == null");
			return index + 12;
		}

		if (index < 0) {
			Debug.Log("ChargeStation.Load failure, index < 0");
			return index + 12;
		}

		if (entries == null) {
			Debug.Log("ChargeStation.Load failure, entries == null");
			return index + 12;
		}

		chg.nextthink = Utils.LoadRelativeTimeDifferential(entries[index],"nextthink"); index++; // float - time before recharged
		chg.amount  = Utils.GetFloatFromString(entries[index],"amount"); index++;
		chg.resetTime  = Utils.GetFloatFromString(entries[index],"resetTime"); index++;
		chg.requireReset  = Utils.GetBoolFromString(entries[index],"requireReset"); index++;
		chg.minSecurityLevel  = Utils.GetFloatFromString(entries[index],"minSecurityLevel"); index++;
		chg.damageOnUse  = Utils.GetFloatFromString(entries[index],"damageOnUse"); index++;
		chg.target = Utils.LoadString(entries[index],"target"); index++;
		chg.argvalue = Utils.LoadString(entries[index],"argvalue"); index++;
		chg.rechargeMsgLingdex = Utils.GetIntFromString(entries[index],"rechargeMsgLingdex"); index++;
		chg.usedMsgLingdex = Utils.GetIntFromString(entries[index],"usedMsgLingdex"); index++;
		return index;
	}
}
