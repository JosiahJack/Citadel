using UnityEngine;
using System.Collections;

public class ExplosionForce : MonoBehaviour {
	public float radius = 5.12f;
	public float power = 80f;
	private HealthManager hm;
	
	// Unity builtin
	// pos: center of sphere
	public void ExplodeInner(Vector3 pos, float oldforce, float oldradius, DamageData dd) {
		if (dd == null) return;
		float damageOriginal = dd.damage;
		Collider[] colliders = Physics.OverlapSphere(pos, oldradius);
		int i = 0;
		while (i < colliders.Length) {
			if (colliders[i] != null) {
				Const.a.ApplyImpactForce(colliders[i].gameObject, dd.impactVelocity,dd.attacknormal,dd.hit.point);
				hm = colliders[i].GetComponent<HealthManager>();
				if (hm != null) {
					DamageData dnew = dd;
					dnew.damage = dd.damage;// * ((Vector3.Distance(colliders[i].gameObject.transform.position,pos))/oldradius);
					if (hm.isPlayer) dnew.damage = dnew.damage * 0.5f; // give em a chance mate
					hm.TakeDamage(dd);
				}
			}
			
			i++;
		}
	}
	
	// Occlusion support
	// pos: center of sphere
	public void ExplodeOuter(Vector3 pos) {
		Collider[] colliders = Physics.OverlapSphere(pos, radius);
		int i = 0;
		while (i < colliders.Length) {
			if (colliders[i] != null && colliders[i].GetComponent<Rigidbody>() != null) {
				Vector3 direction = colliders[i].transform.position - pos;
				Ray ray = new Ray(pos, direction);
				RaycastHit hit;
				
				if (Physics.Raycast(ray, out hit, radius)) {
					if (hit.collider == colliders[i]) {
						float distPenalty = Mathf.Pow((radius - hit.distance) / radius, 2);
						Vector3 force = direction * power * distPenalty;
						hit.rigidbody.AddForce(force);
					}
				}
			}
			i++;
		}
	}
}