using UnityEngine;
using System.Collections;

public class GravityLift : MonoBehaviour {
	public float strength = 30f;
	private Rigidbody otherRbody;
	private float modulatedStrengthY;

	void OnTriggerStay (Collider other) {
		otherRbody = other.gameObject.GetComponent<Rigidbody>();
		// TODO check if flier bot, avian mutant, or projectile and return without affecting it
		if (otherRbody != null) {
			if (otherRbody.velocity.y < strength)
				otherRbody.AddForce(new Vector3(0f, (strength-otherRbody.velocity.y), 0f));
		}
	}
}
