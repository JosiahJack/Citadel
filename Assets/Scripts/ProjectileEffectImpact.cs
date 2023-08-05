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
        if (other.gameObject == host) return;

		numHits++;
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
			int mask = Const.a.layerMaskPlayerAttack;
			if (gameObject.layer != 11) { // Bullet
				mask = Const.a.layerMaskNPCAttack;
			} else {
				mask = Const.a.layerMaskPlayerAttack;
			}

			Utils.ApplyImpactForceSphere(dd,transform.position,3.2f,1f,mask);
		}

		GameObject hitGO = other.contacts[0].otherCollider.gameObject;
		HealthManager hm = hitGO.GetComponent<HealthManager>();
		if (hm == null) {
			// For hopper joint collisions or other combo-collider setups.
			HealthManagerRelay hmr = hitGO.GetComponent<HealthManagerRelay>();
			if (hmr != null) hm = hmr.healthManagerToRedirectTo;
		}

		if (hm != null) {
			// Get an impact effect
			GameObject impact = Const.a.GetObjectFromPool(impactType); 
			Vector3 hitPos = other.contacts[0].point; 
			if (impact != null) {
				impact.transform.position = hitPos;
				impact.SetActive(true); // Enable the impact effect
			}

			if (hm != null && (hm.health > 0 || hm.cyberHealth > 0)) {
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

				if (hm.isNPC || dd.isOtherNPC) Music.a.inCombat = true;
				if (dmgFinal < 0f) dmgFinal = 0f; // Less would = blank.
				if (dd.attackType == AttackType.Tranq) dmgFinal = -2f;
				WeaponFire.a.CreateTargetIDInstance(dmgFinal,hm);
			}

			if (dd.attackType == AttackType.Tranq) {
				AIController aic = hitGO.GetComponent<AIController>();
				if (aic != null) {
					aic.Tranquilize();
				} else {
					aic = other.gameObject.GetComponent<AIController>();
					if (aic !=null) aic.Tranquilize();
				}
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
		if (pei == null) {
			Debug.Log("ProjectileEffectImpact missing on savetype of "
					  + "Projectile!  GameObject.name: " + go.name);
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
		if (pei.hitCountBeforeRemoval > 1) {
			Rigidbody rbody = go.GetComponent<Rigidbody>();
			if (rbody != null) {
				Debug.Log("plasma shot velocity after load: " + rbody.velocity.ToString());
			}
		}
		pei.numHits = Utils.GetIntFromString(entries[index]); index++;
		pei.destroyInsteadOfDeactivate = Utils.GetBoolFromString(entries[index]); index++;
		return index;
	}
}