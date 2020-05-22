using UnityEngine;
using System.Collections;

public class GravityLift : MonoBehaviour {
	public float strength = 12f;
	public float offStrengthFactor = 0.3f;
	private Rigidbody otherRbody;
	private float modulatedStrengthY;
	public float distancePaddingToTopPoint = 0.32f;
	public bool active = true;
	//public Transform topPoint;
	public Vector3 topPoint;
	private BoxCollider boxcol;

	void Awake() {
		boxcol = GetComponent<BoxCollider>();
		if (boxcol == null) return;
		topPoint = new Vector3(0f,boxcol.bounds.max.y,0f); // add half the player's capsule height(1f) to the top extent of the box collider
	}

	void OnTriggerStay (Collider other) {
		if (active) {
			otherRbody = other.gameObject.GetComponent<Rigidbody>();
			if (otherRbody != null) {
				if (otherRbody.velocity.y < strength) {
					if (Vector3.Distance(topPoint,other.gameObject.transform.position) < ((other.bounds.max.y/2f) + distancePaddingToTopPoint)) {
						otherRbody.AddForce(new Vector3(0f, (9.83f-(otherRbody.velocity.y)), 0f),ForceMode.Acceleration);
						//otherRbody.velocity = new Vector3(otherRbody.velocity.x,0f,otherRbody.velocity.z);
					} else {
						otherRbody.AddForce(new Vector3(0f, (strength-(otherRbody.velocity.y)), 0f));
					}
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
