using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMeleeDamageCollider : MonoBehaviour {
	private int index;
	private int meleeColliderCounter = 0;
	private float impactMelee = 0f;
	private GameObject ownedBy;
    private AIController ownerAIC;
    private MeshCollider meshCollider;
    private BoxCollider boxCollider;
    private SphereCollider sphereCollider;
    private CapsuleCollider capCollider;

	public void MeleeColliderSetup (int ind, int colCount, float impact,
									GameObject sourceOwner) {
		index = ind;
		meleeColliderCounter = colCount;
		impactMelee = impact;
		ownedBy = sourceOwner;
        ownerAIC = ownedBy.GetComponent<AIController>();
        meshCollider = GetComponent<MeshCollider>();
        boxCollider = GetComponent<BoxCollider>();
        sphereCollider = GetComponent<SphereCollider>();
        capCollider = GetComponent<CapsuleCollider>();

        if (meshCollider != null)     meshCollider.isTrigger = true;
        if (boxCollider != null)       boxCollider.isTrigger = true;
        if (sphereCollider != null) sphereCollider.isTrigger = true;
        if (capCollider != null)       capCollider.isTrigger = true;
    }

	void OnTriggerEnter (Collider other) {
		if (other == null) return;
		if (ownerAIC.meleeDamageFinished >= PauseScript.a.relativeTime) return;
        if (!(other.CompareTag("Player") || other.CompareTag("NPC"))) return;
		if (ownerAIC == null) {
			Debug.Log("BUG: AIMeleeDamageCollider on but no AIC assigned!");
			gameObject.SetActive(false);
			return;
		}

		// Make sure we aren't hitting the lean transform by accident...target
		// the actual player capsule with its health manager.
		HealthManager hm = other.GetComponent<HealthManager>();
		if (hm == null) return;

		ownerAIC.meleeDamageFinished = PauseScript.a.relativeTime + Const.a.timeToActualAttack1ForNPC[index];
		DamageData ddNPC = DamageData.SetNPCData(index,1,ownedBy);
		ddNPC.other = other.gameObject;
		ddNPC.attacknormal = Vector3.Normalize(other.transform.position - transform.position);
		ddNPC.damage = 11f;
		float take = DamageData.GetDamageTakeAmount(ddNPC);
		take = (take / meleeColliderCounter); //split it for multiple tap melee attacks, e.g. double paw swipe
		ddNPC.damage = take;
		ddNPC.impactVelocity = ddNPC.damage * 1.5f;
		//Utils.ApplyImpactForce(other.gameObject,ddNPC.impactVelocity,ddNPC.attacknormal,ddNPC.hit.point);
		hm.TakeDamage(ddNPC);
	}
}
