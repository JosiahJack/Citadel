using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberPush : MonoBehaviour {
	public float force = 15f;
	public Vector3 direction;
	private Rigidbody otherRbody;
	private Vector3 tempVec;

	void OnTriggerStay(Collider col) {
		if (Const.a.difficultyCyber < 1) return;

		if (col.gameObject.CompareTag("Player")) {
			PlayerMovement pm = col.gameObject.GetComponent<PlayerMovement>();
			if (pm != null) {
				otherRbody = col.gameObject.GetComponent<Rigidbody>();
				if (otherRbody != null) {
					otherRbody.AddForce(direction * force * Time.deltaTime, ForceMode.Acceleration);
					Music.a.NotifyCyberTube();
				}
			}
		}
	}
}
