using UnityEngine;
using System.Collections;

public class GravityLift : MonoBehaviour {
	public float strength = 12f;
	public float offStrengthFactor = 0.3f;
	private Rigidbody otherRbody;
	private float modulatedStrengthY;
	public float distancePaddingToTopPoint = 0.32f; // Add half the player's capsule height(1f) to the top extent of the box collider
	public bool active = true;
	public Vector3 topPoint;
	private BoxCollider boxcol;

	void Awake() {
		boxcol = GetComponent<BoxCollider>();
		if (boxcol == null) return;
		topPoint = new Vector3(0f,boxcol.bounds.max.y,0f);
	}

	void OnTriggerStay (Collider other) {
		if (active) {
			otherRbody = other.gameObject.GetComponent<Rigidbody>();
			if (otherRbody != null) {
				if (otherRbody.velocity.y < (strength * otherRbody.mass)) {
					if (Vector3.Distance(topPoint,other.gameObject.transform.position) < ((other.bounds.max.y/2f) + distancePaddingToTopPoint)) {
						otherRbody.AddForce(new Vector3(0f, (9.83f-(otherRbody.velocity.y)), 0f),ForceMode.Acceleration);
					} else {
						otherRbody.AddForce(new Vector3(0f, ((strength * otherRbody.mass)-(otherRbody.velocity.y)), 0f));
					}
				}
			}
		} else {
			// apply weak force for inactive state - applies some force for gentle descent, never really off completely
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

	public static string Save(GameObject go) {
		GravityLift gl = go.GetComponent<GravityLift>(); // Not quite Open, but hey
		if (gl == null) {
			Debug.Log("GravityLift missing on savetype of GravityLift!  GameObject.name: " + go.name);
			return "1";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(gl.active); // bool - is this gravlift on?
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		GravityLift gl = go.GetComponent<GravityLift>();
		if (gl == null) {
			Debug.Log("GravityLift.Load failure, gl == null");
			return index + 1;
		}

		if (index < 0) {
			Debug.Log("GravityLift.Load failure, index < 0");
			return index + 1;
		}

		if (entries == null) {
			Debug.Log("GravityLift.Load failure, entries == null");
			return index + 1;
		}

		gl.active = Utils.GetBoolFromString(entries[index]); index++; // bool - is this gravlift on?
		return index;
	}
}
