using UnityEngine;
using System.Collections;

public class GravityLift : MonoBehaviour {
	public float strength = 12f;
	public float offStrengthFactor = 0.3f;
	private Rigidbody otherRbody;
	private float modulatedStrengthY;
	public bool active = true;

	void OnTriggerStay (Collider other) {
		if (active) {
			otherRbody = other.gameObject.GetComponent<Rigidbody>();
			// TODO check if flier bot, avian mutant, or projectile and return without affecting it
			if (otherRbody != null) {
				if (otherRbody.velocity.y < strength)
					otherRbody.AddForce(new Vector3(0f, (strength-otherRbody.velocity.y), 0f));
			}
		} else {
			// apply weak force for falling, 1/10th of normal
			otherRbody = other.gameObject.GetComponent<Rigidbody>();
			// TODO check if flier bot, avian mutant, or projectile and return without affecting it
			if (otherRbody != null) {
				if (otherRbody.velocity.y < (strength*offStrengthFactor))
					otherRbody.AddForce(new Vector3(0f, ((offStrengthFactor*strength)-otherRbody.velocity.y), 0f));
			}
		}
	}

	public void Toggle() {
		// Debug.Log("Toggled gravity lift!");
		active = !active;
	}
}
