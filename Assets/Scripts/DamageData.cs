using UnityEngine;

// Holds the data about attacker and attackee for use when applying damage.
// Useful to pass all of this data around rather than having a ton of function arguments.
// Relying on null checks, hence a class...bad?
public class DamageData {
	// Attacker (self [a]) data
	public GameObject owner;
	public bool ownerIsNPC;
	public GameObject ownersCamera;
	public AttackType attackType;
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
		damageData.other = null;
		damageData.attackType = AttackType.None;
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

	public static DamageData SetNPCData (int NPCindex, int attackNum,
											   GameObject ownedBy) {
		if (NPCindex < 0 || NPCindex > 23) {
			NPCindex = 0;
			Debug.Log("BUG: NPCindex incorrect on NPC.  Not 0 to 23 on NPC at: "
					  + ownedBy.transform.position.x.ToString() + ", "
					  + ownedBy.transform.position.y.ToString() + ", "
					  + ownedBy.transform.position.z + ".");
		}

		if (attackNum < 1 || attackNum > 3) attackNum = 1;
		DamageData dd = new DamageData(); 
		// Attacker (self [a]) data
		dd.owner = ownedBy;
		switch (attackNum) {
		case 1:
			dd.damage = Const.a.damageForNPC[NPCindex];
			break;
		case 2:
			dd.damage = Const.a.damageForNPC2[NPCindex];
			break;
		case 3:
			dd.damage = Const.a.damageForNPC3[NPCindex];
			break;
		default:
			Debug.Log("BUG: NPC attackIndex not 0,1, or 2!  Damage set to 1.");
			dd.damage = 1f;
			break;
		}
		dd.penetration = 0;
		dd.offense = 0;
		return dd;
	}
}
