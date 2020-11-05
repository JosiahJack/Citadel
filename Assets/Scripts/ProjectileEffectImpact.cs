using UnityEngine;
using System.Collections;

public class ProjectileEffectImpact : MonoBehaviour {
    public Const.PoolType impactType;
	[HideInInspector] public GameObject host;
    [HideInInspector] public DamageData dd;
    [SerializeField] public int hitCountBeforeRemoval = 1;
    private Vector3 tempVec;
    private int numHits;
    //private Rigidbody rbody;

    private void OnEnable() {
        numHits = 0; // reset when pulled from pool
        //rbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter (Collision other) {
        if (other.gameObject != host) {
            numHits++;
            if (numHits >= hitCountBeforeRemoval) {
                // get an impact effect
                GameObject impact = Const.a.GetObjectFromPool(impactType);
                // enable the impact effect
				Vector3 hitPos = other.contacts[0].point;
                if (impact != null) {
                    impact.transform.position = hitPos;
                    impact.SetActive(true);
                }

                HealthManager hm = other.contacts[0].otherCollider.gameObject.GetComponent<HealthManager>();
				if (hm == null) {
					HealthManagerRelay hmr = other.contacts[0].otherCollider.gameObject.GetComponent<HealthManagerRelay>();
					if (hmr != null) hm = hmr.healthManagerToRedirectTo; // for hopper joint collisions
				}

                //if (hm != null) {
                    //hm.TakeDamage(dd);
				if (hm != null && hm.health > 0) {
					float dmgFinal = hm.TakeDamage(dd); // send the damageData container to HealthManager of hit object and apply damage
					if (hm.isNPC) Music.a.inCombat = true;
					float linkDistForTargID = 10f;
					switch (HardwareInventory.a.hardwareVersion[4]) {
						case 1: linkDistForTargID = 10f; break;
						case 2: linkDistForTargID = 15f; break;
						case 3: linkDistForTargID = 25f; break;
						case 4: linkDistForTargID = 30f; break;
					}
					bool showHealth = false;
					bool showRange = false;
					bool showAttitude = false;
					bool showName = false;
					if (dmgFinal <= 0 && hm.isNPC) {
						if (HardwareInventory.a.hasHardware[4]) {
							if (HardwareInventory.a.hardwareVersion[4] > 0) {
								// Display Range
								showRange = true;
							}

							if (HardwareInventory.a.hardwareVersion[4] > 1) {
								// Display Attitude
								showAttitude = true;
								// Display Name
								showName = true;
							}

							if (HardwareInventory.a.hardwareVersion[4] > 3) {
								// Display enemy health
								showHealth = true;
							}
						}
						WeaponFire.a.CreateTargetIDInstance(Const.a.stringTable[511], other.contacts[0].otherCollider.transform,hm,WeaponFire.a.playerCapsule.transform,linkDistForTargID,showRange,showHealth,showAttitude,showName);
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
					} else {
						if (HardwareInventory.a.hasHardware[4]) {
							if (HardwareInventory.a.hardwareVersion[4] > 0) {
								// Display Range
								showRange = true;
							}

							if (HardwareInventory.a.hardwareVersion[4] > 1) {
								// Display Attitude
								showAttitude = true;
								// Display Name
								showName = true;
							}

							if (HardwareInventory.a.hardwareVersion[4] > 2) {
								if (dmgFinal > hm.maxhealth * 0.75f) {
									// Severe Damage
									WeaponFire.a.CreateTargetIDInstance(Const.a.stringTable[514], other.contacts[0].otherCollider.transform,hm,WeaponFire.a.playerCapsule.transform,linkDistForTargID,showRange,showHealth,showAttitude,showName);
								}
								if (dmgFinal > hm.maxhealth * 0.50f) {
									// Major Damage
									WeaponFire.a.CreateTargetIDInstance(Const.a.stringTable[515], other.contacts[0].otherCollider.transform,hm,WeaponFire.a.playerCapsule.transform,linkDistForTargID,showRange,showHealth,showAttitude,showName);
								}
								if (dmgFinal > hm.maxhealth * 0.25f) {
									// Normal Damage
									WeaponFire.a.CreateTargetIDInstance(Const.a.stringTable[513], other.contacts[0].otherCollider.transform,hm,WeaponFire.a.playerCapsule.transform,linkDistForTargID,showRange,showHealth,showAttitude,showName);
								}

								if (dmgFinal > 0f) {
									// Minor Damage
									WeaponFire.a.CreateTargetIDInstance(Const.a.stringTable[512], other.contacts[0].otherCollider.transform,hm,WeaponFire.a.playerCapsule.transform,linkDistForTargID,showRange,showHealth,showAttitude,showName);
								}
							}

							if (HardwareInventory.a.hardwareVersion[4] > 3) {
								// Display enemy health
								showHealth = true;
							}
						} else {
							WeaponFire.a.noDamageIndicator.SetActive(false); // I'm assuming that this will auto deactivate after 1sec, but in case the player is snappy about weapon switching, added this
						}
					}
				}

				if (dd.attackType == Const.AttackType.Tranq) {
					AIController aic = other.contacts[0].otherCollider.gameObject.GetComponent<AIController>();
					if (aic !=null) aic.Tranquilize();
				}
                gameObject.SetActive(false); // disable the projectile
            }// else {
               // Vector3 dir = other.contacts[0].normal;
                //rbody.AddForce(dir * rbody.velocity.magnitude * 0.75f, ForceMode.Impulse); //buggy, added to physics material instead
            //}
        }
	}
}
