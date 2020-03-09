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
			if (otherRbody != null) {
				if (otherRbody.velocity.y < strength)
					otherRbody.AddForce(new Vector3(0f, (strength-otherRbody.velocity.y), 0f));
			}
		} else {
			// apply weak force for inactive state - applies some force for gentle descent
			otherRbody = other.gameObject.GetComponent<Rigidbody>();
			if (otherRbody != null) {
				if (otherRbody.velocity.y < (strength*offStrengthFactor))
					otherRbody.AddForce(new Vector3(0f, ((offStrengthFactor*strength)-otherRbody.velocity.y), 0f));
			}
		}
	}

	public void Toggle() {
		active = !active;
	}
}
