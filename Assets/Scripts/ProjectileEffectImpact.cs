using UnityEngine;
using System.Collections;

public class ProjectileEffectImpact : MonoBehaviour {
    public PoolType impactType;
	public bool destroyInsteadOfDeactivate = false;
	[HideInInspector] public GameObject host;
    [HideInInspector] public DamageData dd;
    [SerializeField] public int hitCountBeforeRemoval = 1;
    private Vector3 tempVec;
    [HideInInspector] public int numHits;

    private void OnEnable() {
        numHits = 0; // Reset when pulled from pool.
		if (hitCountBeforeRemoval < 1) hitCountBeforeRemoval = 1;
    }

    void OnCollisionEnter (Collision other) {
        if (other.gameObject != host) {
            numHits++;
            if (numHits >= hitCountBeforeRemoval) {
                GameObject impact = Const.a.GetObjectFromPool(impactType); // Get an impact effect
				Vector3 hitPos = other.contacts[0].point; // Enable the impact effect
                if (impact != null) {
                    impact.transform.position = hitPos;
                    impact.SetActive(true);
                }

                HealthManager hm = other.contacts[0].otherCollider.gameObject.GetComponent<HealthManager>();
				if (hm == null) {
					HealthManagerRelay hmr = other.contacts[0].otherCollider.gameObject.GetComponent<HealthManagerRelay>();
					if (hmr != null) hm = hmr.healthManagerToRedirectTo; // For hopper joint collisions or other combo-collider setups.
				}

				if (hm != null && (hm.health > 0 || hm.cyberHealth > 0)) {
					dd.other = other.gameObject;
					if (other.gameObject.CompareTag("NPC")) {
						dd.isOtherNPC = true;
					} else {
						dd.isOtherNPC = false;
					}
					// GetDamageTakeAmount expects damageData to already have the following set:
					//   damage
					//   offense
					//   penetration
					//   attackType
					//   berserkActive
					//   isOtherNPC
					//   armorvalue
					//   defense
					// Most already was when this was launched by AIController or WeaponFire
					dd.damage = DamageData.GetDamageTakeAmount(dd);
					dd.impactVelocity = dd.damage * 1.5f;
					if (!hm.inCyberSpace) {
						Utils.ApplyImpactForce(other.gameObject, dd.impactVelocity,dd.attacknormal,dd.hit.point);
					}
					float dmgFinal = hm.TakeDamage(dd); // send the damageData container to HealthManager of hit object and apply damage
					if (hm.isNPC || dd.isOtherNPC) Music.a.inCombat = true;
					if (dmgFinal < 0f) dmgFinal = 0f; // Less would = blank.
					WeaponFire.a.CreateTargetIDInstance(dmgFinal,hm);
				}

				if (dd.attackType == AttackType.Tranq) {
					AIController aic = other.contacts[0].otherCollider.gameObject.GetComponent<AIController>();
					if (aic !=null) aic.Tranquilize();
					else {
						aic = other.gameObject.GetComponent<AIController>();
						if (aic !=null) aic.Tranquilize();
					}
				}
				if (destroyInsteadOfDeactivate) Utils.SafeDestroy(gameObject);
                else gameObject.SetActive(false); // disable the projectile
            }
        }
	}

	public static string Save(GameObject go) {
		ProjectileEffectImpact pei = go.GetComponent<ProjectileEffectImpact>();
		if (pei == null) {
			Debug.Log("ProjectileEffectImpact missing on savetype of Projectile!  GameObject.name: " + go.name);
			return Utils.DTypeWordToSaveString("uu");

		}

		string line = System.String.Empty;
		line = Utils.UintToString(pei.hitCountBeforeRemoval);
		line += Utils.splitChar + Utils.UintToString(pei.numHits);
		line += Utils.splitChar + Utils.BoolToString(pei.destroyInsteadOfDeactivate);
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		ProjectileEffectImpact pei = go.GetComponent<ProjectileEffectImpact>();
		if (pei == null) {
			Debug.Log("ProjectileEffectImpact.Load failure, pei == null");
			return index + 3;
		}

		if (index < 0) {
			Debug.Log("ProjectileEffectImpact.Load failure, index < 0");
			return index + 3;
		}

		if (entries == null) {
			Debug.Log("ProjectileEffectImpact.Load failure, entries == null");
			return index + 3;
		}

		pei.hitCountBeforeRemoval = Utils.GetIntFromString(entries[index]); index++;
		pei.numHits = Utils.GetIntFromString(entries[index]); index++;
		pei.destroyInsteadOfDeactivate = Utils.GetBoolFromString(entries[index]); index++;
		return index;
	}
}