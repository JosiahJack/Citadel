using UnityEngine;
using System.Collections;

public class GravityLift : MonoBehaviour {
	public float strength = 12f;
	public float offStrengthFactor = 0.3f;
	public float topPointDampening = 50f;
	public float topPointDampeningRange = 0.8f;
	private Rigidbody otherRbody;
	private float modulatedStrengthY;
	public bool active = true;
	//public Transform topPoint;
	public Vector3 topPoint;
	private BoxCollider boxcol;

	void Awake() {
		boxcol = GetComponent<BoxCollider>();
		if (boxcol == null) return;
		topPoint = new Vector3(0f,boxcol.bounds.max.y + 1f,0f); // add half the player's capsule height(1f) to the top extent of the box collider
	}

	void OnTriggerStay (Collider other) {
		if (active) {
			otherRbody = other.gameObject.GetComponent<Rigidbody>();
			if (otherRbody != null) {
				if (otherRbody.velocity.y < strength) {
					//if (topPoint != null) {
						if (Vector3.Distance(topPoint,other.gameObject.transform.position) < topPointDampeningRange) {
							otherRbody.AddForce(new Vector3(0f, (strength-(otherRbody.velocity.y*topPointDampening)), 0f));
						} else {
							otherRbody.AddForce(new Vector3(0f, (strength-(otherRbody.velocity.y)), 0f));
						}
					//}
				}
			}
		} else {
			// apply weak force for inactive state - applies some force for gentle descent
			otherRbody = other.gameObject.GetComponent<Rigidbody>();
			if (otherRbody != null) {
				if (otherRbody.velocity.y < offStrengthFactor) {
					otherRbody.AddForce(new Vector3(0f, ((offStrengthFactor)-otherRbody.velocity.y), 0f));
				}
			}
		}
	}

	public void Toggle() {
		active = !active;
	}
}
