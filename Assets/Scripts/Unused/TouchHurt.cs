using UnityEngine;
using System.Collections;

public class TouchHurt : MonoBehaviour {
	public float damage = 1; // assign in the editor
	/* Moved to Unused, JJ 9/5/19
	void  OnCollisionEnter ( Collision col  ){
		if (col.gameObject.tag == "Player") {
			DamageData dd = new DamageData();
			dd.damage = damage;
			dd.owner = gameObject;
			dd.attackType = Const.AttackType.Melee;
			dd.isOtherNPC = false;
			col.gameObject.GetComponent<PlayerHealth>().TakeDamage(dd);
		}
	}
	*/
}