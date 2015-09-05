using UnityEngine;
using System.Collections;

public class GravityLift : MonoBehaviour {
	void TriggerEnter (Collision other) {
		if (other.gameObject.tag != "Geometry") {
			if (other.gameObject.GetComponent<PlayerMovement>() != null) {
				other.gameObject.GetComponent<PlayerMovement>().gravliftState = false;
			}
		}
	}

	void TriggerStay (Collision other) {
		if (other.gameObject.tag != "Geometry") {
			if (other.gameObject.GetComponent<Rigidbody>() != null) {
				other.gameObject.GetComponent<Rigidbody>().AddForce( new Vector3(0f,1.1f,0f), ForceMode.Force);
			}
		}
	}

	void TriggerExit (Collision other) {
		if (other.gameObject.tag != "Geometry") {
			if (other.gameObject.GetComponent<PlayerMovement>() != null) {
				other.gameObject.GetComponent<PlayerMovement>().gravliftState = true;
			}
		}
	}
}
