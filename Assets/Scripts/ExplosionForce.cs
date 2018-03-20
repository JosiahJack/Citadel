using UnityEngine;
using System.Collections;

public class ExplosionForce : MonoBehaviour {
	public float radius = 10f;
	public float power = 1000f;
	
	// Unity builtin
	// pos: center of sphere
	public void ExplodeInner(Vector3 pos, float oldforce, float oldradius, DamageData dd) {
		Collider[] colliders = Physics.OverlapSphere(pos, oldradius);
		foreach (Collider c in colliders) {
			if (c != null && c.GetComponent<Rigidbody>() != null) {
				c.GetComponent<Rigidbody>().AddExplosionForce(oldforce, pos, oldradius, 1.0f);
				if (dd != null) {
					HealthManager hm = c.gameObject.GetComponent<HealthManager>();
					if (hm != null) hm.TakeDamage(dd);
				}
			}
		}
	}
	
	// Occlusion support
	// pos: center of sphere
	public void ExplodeOuter(Vector3 pos) {
		Collider[] colliders = Physics.OverlapSphere(pos, radius);
		foreach (Collider c in colliders) {
			if (c.GetComponent<Rigidbody>() == null) {
				continue;
			}
			
			Vector3 direction = c.transform.position - pos;
			Ray ray = new Ray(pos, direction);
			RaycastHit hit;
			
			// Raycast from explosion center to possible objective "c".
			if (!Physics.Raycast(ray, out hit, radius)) {
				continue;
			}
			
			// Raycast got direct hit with "c"?
			// - Yes: apply force
			// - No: ignore explosion force
			// TODO(by YOU): IMPORTANT Change this to match your problem!!
			if (!hit.collider.Equals(c) || !hit.transform.tag.Equals(c.tag)) {
				continue;
			}
			
			float distPenalty = Mathf.Pow((radius - hit.distance) / radius, 2);
			
			var force = direction * power * distPenalty;
			
			hit.rigidbody.AddForce(force);
		}
	}
	
}