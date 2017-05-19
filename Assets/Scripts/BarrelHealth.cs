using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelHealth : MonoBehaviour {
	public float health = 45f;

	public void TakeDamage (DamageData dd) {
		if (health <= 0f)
			return;

		// Update damage data with this object's info
		dd.defense = 0f;
		dd.armorvalue = 0f;
		dd.isOtherNPC = false;

		float take = Const.a.GetDamageTakeAmount(dd);
		health -= take;
		if (health <= 1f) {
			health = 0f;
			GameObject explosionEffect = Const.a.GetObjectFromPool(Const.PoolType.CameraExplosions);
			explosionEffect.SetActive(true);
			explosionEffect.transform.position = transform.position;
			gameObject.SetActive(false);
		} else {
			Rigidbody rbody = GetComponent<Rigidbody>();
			rbody.AddForceAtPosition((dd.hit.normal * take),dd.hit.point);
		}
	}
}
