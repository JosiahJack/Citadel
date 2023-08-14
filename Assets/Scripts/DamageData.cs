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
		if (NPCindex < 0 || NPCindex > 28) {
			NPCindex = 0;
			Debug.Log("BUG: NPCindex incorrect on NPC.  Not 0 to 28 on NPC at: "
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

	// ========================DAMAGE SYSTEM===========================
	// 0. First checks against whether the entity is damageable (i.e. not the
	//    world) - handled by Physics Layers.
	// 1. Armor Absorption (see ICE Breaker Guide for all of 4 these)
	// 2. Weapon Vulnerabilities based on attack type and the a_att_type bits
	//    stored in the npc.
	// 3. Critical Hits, chance for critical hit damage based on defense and
	//    offense of attack and target.
	// 4. Random Factor, +/- 10% damage for randomness
	// 5. Apply Velocity for damage, this is after all the above because
	//    otherwise the damage multipliers wouldn't affect velocity.
	// 6. Berserk Damage Increase
	// 7. Return the damage to original TakeDamage() function
	public static float GetDamageTakeAmount (DamageData dd) {
		float take, crit;

		// a_* = attacker
		float a_damage = dd.damage;
		float a_offense = dd.offense;
		float a_penetration = dd.penetration;
		AttackType a_att_type = dd.attackType;
		bool a_berserk = dd.berserkActive;

		// o_* = other (one being attacked)
		bool o_isnpc = dd.isOtherNPC;
		float o_armorvalue = dd.armorvalue;
		float o_defense = dd.defense;

 		// 1. Armor Absorption (NPC armor, not player)
		take = (o_armorvalue > a_penetration ? a_damage - a_penetration : a_damage);

		// 2. Weapon Vulnerabilities
		//    Handled by HealthManager.

		// 3. Critical Hits (NPCs only)
		if (o_isnpc) {
			crit = (a_offense - o_defense);
			if (crit > 0) {
				// 71% success with 5/6  5 = f, 6 = max offense or defense.
				// 62% success with 4/6
				// 50% success with 3/6
				// 24% success with 2/6
				// 10% success with 1/6
				// chance of f/6 = 5/6|4/6|3/6|2/6|1/6 = .833|.666|.5|.333|.166
				if ((Random.Range(0f,1f) < (crit/6))
					&& (Random.Range(0f,1f) < 0.2f)) {

					// SUCCESS! Maximum extra is 5X + 1X Damage.
					take += crit * take;
				}
			}
		}

		// 4. Random Factor +/- 10% (aka 0.10 damage).
		take *= Random.Range(0.9f,1.1f);

		// 5. Apply Impact Velocity for Damage
		//    Handled by HealthManager.

		// 6. Berserk Damage Increase.
		if (a_berserk) {
			take *= Const.a.berserkDamageMultiplier;
		}

		// 7. Return the Damage.
		return take;
	}
}
