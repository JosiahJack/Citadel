using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData {
	// Attacker (self [a]) data
	public GameObject owner;
	public GameObject ownersCamera;
	public WeaponFire ownersWeaponFireScript;
	public Const.AttackType attackType;
	public bool isOtherNPC;
	public bool isFullAuto;
	public float damage;
	public float delayBetweenShots;
	public float penetration;
	public float offense;
	public float damageOverload;
	public float energyDrainLow;
	public float energyDrainHi;
	public float energyDrainOver;
	public float range;
	public bool berserkActive;

	// Attacked (other [o]) data
	public float armorvalue;
	public float defense;
	public GameObject other;
	public int indexNPC;
	public RaycastHit hit;
	public Vector3 attacknormal;

	public void ResetDamageData (DamageData damageData) {
		damageData.owner = null;
		damageData.ownersCamera = null;
		damageData.ownersWeaponFireScript = null;
		damageData.other = null;
		damageData.attackType = Const.AttackType.None;
		damageData.isOtherNPC = false;
		damageData.isFullAuto = false;
		damageData.damage = 0f;
		damageData.delayBetweenShots = 0.8f;
		damageData.penetration = 0f;
		damageData.offense = 0f;
		damageData.damageOverload = 0f;
		damageData.energyDrainLow = 0f;
		damageData.energyDrainHi = 0f;
		damageData.energyDrainOver = 0f;
		damageData.range = 200f;
		damageData.berserkActive = false;
		damageData.armorvalue = 0f;
		damageData.defense = 0f;
		damageData.other = null;
		damageData.indexNPC = -1;
		damageData.attacknormal = Vector3.zero;
	}
}
