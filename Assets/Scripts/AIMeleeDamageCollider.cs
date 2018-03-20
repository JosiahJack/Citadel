using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMeleeDamageCollider : MonoBehaviour {
	private int index;
	private int meleeColliderCounter = 0;
	private float impactMelee = 0f;
	private GameObject ownedBy;

	public void MeleeColliderSetup (int ind, int colCount, float impact, GameObject sourceOwner) {
		index = ind;
		meleeColliderCounter = colCount;
		impactMelee = impact;
		ownedBy = sourceOwner;
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player" || other.tag == "NPC") {
			DamageData ddNPC = Const.SetNPCDamageData(index, Const.aiState.Attack1,ownedBy);
			ddNPC.other = gameObject;
			ddNPC.attacknormal = Vector3.Normalize(other.transform.position - transform.position);
			ddNPC.impactVelocity = impactMelee;
			float take = Const.a.GetDamageTakeAmount(ddNPC);
			take = (take/meleeColliderCounter); //split it for multiple tap melee attacks, e.g. double paw swipe
			ddNPC.damage = take;
			other.GetComponent<HealthManager>().TakeDamage(ddNPC);
		}
	}
}
