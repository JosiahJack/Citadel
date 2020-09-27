using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberPush : MonoBehaviour {
	public float force = 15f;
	public Vector3 direction;
	private Rigidbody otherRbody;
	private Vector3 tempVec;

	void OnTriggerStay(Collider other) {
		if (Const.a.difficultyCyber < 1) return;

		if (other.gameObject.CompareTag("Player")) {
			//Debug.Log("CyberPush.cs-ing player!");
			otherRbody = other.gameObject.GetComponent<Rigidbody>();
			if (otherRbody != null) {
				otherRbody.AddForce(direction * force * Time.deltaTime, ForceMode.Acceleration);
				Music.a.PlayTrack(LevelManager.a.currentLevel,Music.TrackType.Cybertube,Music.MusicType.Override);
			} else {
				Debug.Log("Failed to get PlayerManager on collision with player in CyberPush.cs");
			}
		}
	}
}
