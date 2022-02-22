using UnityEngine;

// Holds the data about attacker and attackee for use when applying damage.
// Useful to pass all of this data around rather than having a ton of function arguments.
public class DamageData {
	// Attacker (self [a]) data
	public GameObject owner;
	public bool ownerIsNPC;
	public GameObject ownersCamera;
	public WeaponFire ownersWeaponFireScript;
	public Const.AttackType attackType;
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
	public float impactVelocity;
	public bool isOtherNPC;

	public void ResetDamageData (DamageData damageData) {
		damageData.owner = null;
		damageData.ownersCamera = null;
		damageData.ownersWeaponFireScript = null;
		damageData.other = null;
		damageData.attackType = Const.AttackType.None;
		damageData.ownerIsNPC = false;
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
		damageData.hit = new RaycastHit();
		damageData.attacknormal = Const.a.vectorZero;
		damageData.impactVelocity = 0f;
	}
}
