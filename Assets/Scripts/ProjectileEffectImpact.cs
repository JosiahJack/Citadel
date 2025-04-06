using UnityEngine;
using System.Collections;
using System.Text;

public class ProjectileEffectImpact : MonoBehaviour {
    public PoolType impactType;
	public bool destroyInsteadOfDeactivate = false;
	[HideInInspector] public GameObject host;
    [HideInInspector] public DamageData dd;
    [SerializeField] public int hitCountBeforeRemoval = 1;
    private Vector3 tempVec;
    [HideInInspector] public int numHits;
	private static StringBuilder s1 = new StringBuilder();

    private void OnEnable() {
        numHits = 0; // Reset when pulled from pool.
		if (hitCountBeforeRemoval < 1) hitCountBeforeRemoval = 1;
    }

    void OnCollisionEnter (Collision other) {
        if (other.gameObject == host) return;

		numHits++;
		float stunAmount = 3f + ((WeaponFire.a.stungunSetting / 100f) * 7f); // Const.a.damagePerHitForWeapon[wep16Index] vs Const.a.damagePerHitForWeapon2[wep16Index] for Stungun.
		stunAmount = Mathf.Clamp(stunAmount, 3f, 10f);
		dd.other = other.gameObject;
		dd.isOtherNPC = false;
		// GetDamageTakeAmount expects damageData to already have the
		// following set:
		//   damage
		//   offense
		//   penetration
		//   attackType
		//   berserkActive
		//   isOtherNPC
		//   armorvalue
		//   defense
		// Most already was when launched by AIController or WeaponFire.
		dd.damage = DamageData.GetDamageTakeAmount(dd);
		if (impactType == PoolType.RailgunImpacts) {
			Utils.ApplyImpactForceSphere(dd,transform.position,3.2f,1f);
			WeaponFire.a.fogFac += 4;
		}

		GameObject hitGO = other.contacts[0].otherCollider.gameObject;
		HealthManager hm = Utils.GetMainHealthManager(hitGO);
		if (hm != null) {
			// Get an impact effect
			GameObject impact = Const.a.GetObjectFromPool(impactType); 
			Vector3 hitPos = other.contacts[0].point; 
			if (impact != null) {
				impact.transform.position = hitPos;
				impact.SetActive(true); // Enable the impact effect
			}

			if (hm.health > 0 || hm.cyberHealth > 0) {
				if (other.gameObject.CompareTag("NPC")) dd.isOtherNPC = true;


				if (numHits < hitCountBeforeRemoval) {
					dd.damage = dd.damage * 0.85f; // Lose small amount each hit
				}

				dd.impactVelocity = dd.damage * 1.5f;
				if (numHits > 0) {
				    dd.impactVelocity = dd.impactVelocity / 3f;
				}
				
				if (LevelManager.a.currentLevel != 13 && !host.CompareTag("NPC")) {
					Utils.ApplyImpactForce(other.gameObject,dd.impactVelocity,
										   dd.attacknormal,dd.hit.point);
				}

				float dmgFinal = hm.TakeDamage(dd); // Send the damageData
													// container to
													// HealthManager of hit
													// object and damage it.
				float tranq = -1f;
				if (dd.isOtherNPC || hm.isNPC) {
					if (hm.aic != null) {
						if (!hm.aic.asleep) Music.a.inCombat = true;
						if (dd.attackType == AttackType.Tranq) {
							tranq = hm.aic.Tranquilize(stunAmount,true);
						}
					}
				}

				if (dmgFinal < 0f) dmgFinal = 0f; // Less would = blank.
				WeaponFire.a.CreateTargetIDInstance(dmgFinal,hm,tranq);
			}
		}

		if (numHits >= hitCountBeforeRemoval) {
			// Get an impact effect
			GameObject impact = Const.a.GetObjectFromPool(impactType); 
			Vector3 hitPos = other.contacts[0].point; 
			if (impact != null) {
				impact.transform.position = hitPos;
				impact.SetActive(true); // Enable the impact effect
			}

			if (destroyInsteadOfDeactivate) Utils.SafeDestroy(gameObject);
			else gameObject.SetActive(false); // disable the projectile
		}
	}

	public static string Save(GameObject go) {
		ProjectileEffectImpact pei = go.GetComponent<ProjectileEffectImpact>();
		s1.Clear();
		s1.Append(Utils.UintToString(pei.hitCountBeforeRemoval,"hitCountBeforeRemoval"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(pei.numHits,"numHits"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(pei.destroyInsteadOfDeactivate,"destroyInsteadOfDeactivate"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		ProjectileEffectImpact pei = go.GetComponent<ProjectileEffectImpact>();
		pei.hitCountBeforeRemoval = Utils.GetIntFromString(entries[index],"hitCountBeforeRemoval"); index++;
		pei.numHits = Utils.GetIntFromString(entries[index],"numHits"); index++;
		pei.destroyInsteadOfDeactivate = Utils.GetBoolFromString(entries[index],"destroyInsteadOfDeactivate"); index++;
		return index;
	}
}
