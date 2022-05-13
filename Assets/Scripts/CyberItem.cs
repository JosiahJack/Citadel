using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberItem : MonoBehaviour {
	public Inventory.SoftwareType type;
	public int version;

	void Start() {
		if (Const.a.difficultyMission == 0) {
			if (type == Inventory.SoftwareType.Data) this.gameObject.SetActive(false); // disable data objects when Mission difficulty is 0
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Player")) {
			PlayerMovement pm = other.gameObject.GetComponent<PlayerMovement>();
			if (pm != null) {
				if (Inventory.a.AddSoftwareItem(type,version)) {
					GameObject explosionEffect = Const.a.GetObjectFromPool(PoolType.CyberDissolve);
					if (explosionEffect != null) {
						explosionEffect.SetActive(true);
						explosionEffect.transform.position = transform.position; // put vaporization effect at raycast center
					}
					this.gameObject.SetActive(false); //we've been picked up, quick hide like you were
				}
			}
		}
	}
}
