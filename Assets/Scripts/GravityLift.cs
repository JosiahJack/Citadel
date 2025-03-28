using UnityEngine;
using System.Collections;
using System.Text;

public class GravityLift : MonoBehaviour {
	public float strength = 12f;
	public float offStrengthFactor = 0.3f;
	private Rigidbody otherRbody;
	private float modulatedStrengthY;
	public float distancePaddingToTopPoint = 0.32f; // Add half the player's capsule height(1f) to the top extent of the box collider
	public bool active = true;
	public Vector3 topPoint;
	public float initialBurstFinished;
	private BoxCollider boxcol;
	private static StringBuilder s1 = new StringBuilder();

	void Awake() {
		boxcol = GetComponent<BoxCollider>();
		if (boxcol == null) return;
		topPoint = new Vector3(0f,boxcol.bounds.max.y,0f);
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.GetComponent<PlayerMovement>() != null) {
			PlayerMovement.a.gravliftState = false;
		}
	}

	void OnForce(Collider other, bool initial) {
		if (other.gameObject.layer == 12) { // Player
			if (other.gameObject.GetComponent<PlayerMovement>() != null) {
				PlayerMovement.a.gravliftState = true;
			}
		}

		float topY = transform.position.y + (boxcol.size.y/2f);
		float dist = topY - other.gameObject.transform.position.y + 0.48f;
		float velY = otherRbody.velocity.y;
		if (otherRbody.velocity.y < 0f) velY = 0f; // Saturate at bottom end.

		if (dist < distancePaddingToTopPoint) {
			Vector3 force = new Vector3(0f,9.81f - velY,0f);
			otherRbody.AddForce(force,ForceMode.Acceleration);
		} else {
			if (otherRbody.velocity.y < (strength * otherRbody.mass)) {
				float yForce = ((strength * otherRbody.mass)
								- otherRbody.velocity.y);

				if (initial
					|| initialBurstFinished > PauseScript.a.relativeTime) {

					yForce *= 2f;
				}

				otherRbody.AddForce(new Vector3(0f,yForce,0f));
			}
		}
	}

	void OffForce(Collider other, bool initial) {
		// Apply weak force for inactive state - applies some force for gentle
		// descent, never really off completely.
		if (other.gameObject.GetComponent<PlayerMovement>() != null) {
			PlayerMovement.a.gravliftState = true;
		}

		if (otherRbody.velocity.y < offStrengthFactor) {
			float yForce = ((offStrengthFactor)-otherRbody.velocity.y);
			if (initial
				|| initialBurstFinished > PauseScript.a.relativeTime) {

				yForce *= 2f;
			}

			otherRbody.AddForce(new Vector3(0f,yForce,0f));
		}
	}

	void OnTriggerEnter(Collider other) {
		otherRbody = other.gameObject.GetComponent<Rigidbody>();
		if (otherRbody == null) return; // Not a physical object.

		initialBurstFinished = PauseScript.a.relativeTime + 1.0f;
		if (active) OnForce(other,true);
		else OffForce(other,true);
	}

	void OnTriggerStay(Collider other) {
		otherRbody = other.gameObject.GetComponent<Rigidbody>();
		if (otherRbody == null) return; // Not a physical object.

		if (active) OnForce(other,false);
		else OffForce(other,false);
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

		s1.Clear();
		s1.Append(Utils.BoolToString(gl.active,"active")); // bool - is this gravlift on?
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(gl.initialBurstFinished,"initialBurstFinished"));
		return s1.ToString();
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

		gl.active = Utils.GetBoolFromString(entries[index],"active"); index++; // bool - is this gravlift on?
		gl.initialBurstFinished = Utils.LoadRelativeTimeDifferential(entries[index],"initialBurstFinished"); index++;
		return index;
	}
}
