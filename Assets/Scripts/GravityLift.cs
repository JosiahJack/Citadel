using UnityEngine;
using System.Collections;

public class GravityLift : MonoBehaviour {
	public float gravStrength;
	public float gravityFactor;
	public float stabilizeTime;
	public float stabilizeThreshold;
	public GameObject basePoint;
	public bool isAffecting;
	public float maxDistance;
	private GameObject previous;
	private RaycastHit hit;
	private Vector3 forceVector;
	private float nextThink;

	void Awake () {
		previous = basePoint;
	}

	void OnTriggerStay (Collider other) {
		if (other.gameObject.GetComponent<Rigidbody>() != null) {
			//Ray ray = new Ray (basePoint.transform.position, -transform.up);
			previous = other.gameObject;
			//previous.GetComponent<Rigidbody>().useGravity = false;
			//if (Physics.Raycast(ray, out hit, maxDistance)) {
			//	if (hit.distance < maxDistance) {
					//forceVector = (Vector3.up * ((maxDistance - hit.distance) / maxDistance) * gravStrength);
					//other.gameObject.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.VelocityChange);
			float distFactor = other.gameObject.transform.position.y - basePoint.transform.position.y;
			distFactor = (maxDistance - distFactor)/maxDistance;
			if (distFactor < 0)
				distFactor = 0;

			other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, (gravStrength * distFactor) + gravityFactor, 0f);

			//	}
			//}
		}
		if (other.gameObject.GetComponent<PlayerMovement>() != null) {
			other.gameObject.GetComponent<PlayerMovement>().grounded = true;
			other.gameObject.GetComponent<PlayerMovement>().gravliftState = true;
		}
	}

	void OnTriggerExit () {
		nextThink = Time.time + stabilizeTime;
	}

	void Update () {
		if (nextThink < Time.time) {
			if (previous.GetComponent<Rigidbody>() != null)
				previous.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);

			if (previous.GetComponent<PlayerMovement>() != null)
				previous.GetComponent<PlayerMovement>().gravliftState = false;
		}
	}
	
}
