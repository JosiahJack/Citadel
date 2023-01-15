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

				if (hm != null && hm.health > 0) {
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
					Utils.ApplyImpactForce(other.gameObject, dd.impactVelocity,dd.attacknormal,dd.hit.point);
					float dmgFinal = hm.TakeDamage(dd); // send the damageData container to HealthManager of hit object and apply damage
					if (hm.isNPC || dd.isOtherNPC) Music.a.inCombat = true;
					float linkDistForTargID = 10f;
					switch (Inventory.a.hardwareVersion[4]) {
						case 1: linkDistForTargID = 10f; break;
						case 2: linkDistForTargID = 15f; break;
						case 3: linkDistForTargID = 25f; break;
						case 4: linkDistForTargID = 30f; break;
					}
					bool showHealth = false;
					bool showRange = false;
					bool showAttitude = false;
					bool showName = false;
					if (dmgFinal <= 0 && (hm.isNPC || dd.isOtherNPC)) {
						if (Inventory.a.hasHardware[4]) {
							if (Inventory.a.hardwareVersion[4] > 0) {
								showRange = true; // Display Range
							}

							if (Inventory.a.hardwareVersion[4] > 1) {
								showAttitude = true; // Display Attitude
								showName = true; // Display Name
							}

							if (Inventory.a.hardwareVersion[4] > 3) {
								showHealth = true; // Display enemy health
							}
						}
						WeaponFire.a.CreateTargetIDInstance(Const.a.stringTable[511], other.contacts[0].otherCollider.transform,hm,WeaponFire.a.playerCapsule.transform,linkDistForTargID,showRange,showHealth,showAttitude,showName);
						if (WeaponFire.a.noDamageIndicator != null) {
							WeaponFire.a.noDamageIndicator.transform.position = hitPos; // center on what we just shot
							if (hm.aic != null) {
								if (hm.aic.index == 14) {
									// Adjust position for hopper origin since it's special and all melty
									Vector3 adjustment = hitPos;
									adjustment.y += 1f;
									WeaponFire.a.noDamageIndicator.transform.position = adjustment;
								}
							}
							WeaponFire.a.noDamageIndicator.SetActive(true); // do this regardless of target identifier version to show player that hey, it no workie
						}
					} else {
						if (Inventory.a.hasHardware[4]) {
							if (Inventory.a.hardwareVersion[4] > 0) showRange = true; // Display Range
							if (Inventory.a.hardwareVersion[4] > 1) {
								showAttitude = true; // Display Attitude
								showName = true; // Display Name
							}

							if (Inventory.a.hardwareVersion[4] > 2) {
								if (dmgFinal > hm.maxhealth * 0.75f) {
									// Severe Damage
									WeaponFire.a.CreateTargetIDInstance(Const.a.stringTable[514], other.contacts[0].otherCollider.transform,hm,WeaponFire.a.playerCapsule.transform,linkDistForTargID,showRange,showHealth,showAttitude,showName);
								}
								if (dmgFinal > hm.maxhealth * 0.50f) {
									// Major Damage
									WeaponFire.a.CreateTargetIDInstance(Const.a.stringTable[515], other.contacts[0].otherCollider.transform,hm,WeaponFire.a.playerCapsule.transform,linkDistForTargID,showRange,showHealth,showAttitude,showName);
								}

								// Normal Damage
								if (dmgFinal > hm.maxhealth * 0.25f) WeaponFire.a.CreateTargetIDInstance(Const.a.stringTable[513], other.contacts[0].otherCollider.transform,hm,WeaponFire.a.playerCapsule.transform,linkDistForTargID,showRange,showHealth,showAttitude,showName);

								// Minor Damage
								if (dmgFinal > 0f) WeaponFire.a.CreateTargetIDInstance(Const.a.stringTable[512], other.contacts[0].otherCollider.transform,hm,WeaponFire.a.playerCapsule.transform,linkDistForTargID,showRange,showHealth,showAttitude,showName);
							}

							if (Inventory.a.hardwareVersion[4] > 3) showHealth = true; // Display enemy health
						} else {
							WeaponFire.a.noDamageIndicator.SetActive(false); // I'm assuming that this will auto deactivate after 1sec, but in case the player is snappy about weapon switching, added this
						}
					}
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