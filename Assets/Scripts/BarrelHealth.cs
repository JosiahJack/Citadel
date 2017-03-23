using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelHealth : MonoBehaviour {
	public float health = 1f;

	public void TakeDamage (float take) {
		if (health <= 0f)
			return;

		health -= take;
		if (health <= 0f) {
			health = 0f;
			GameObject explosionEffect = Const.a.GetObjectFromPool(Const.PoolType.CameraExplosions);
			explosionEffect.SetActive(true);
			explosionEffect.transform.position = transform.position;
			gameObject.SetActive(false);
		}
	}
}
