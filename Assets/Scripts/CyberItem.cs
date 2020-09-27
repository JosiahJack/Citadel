using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberItem : MonoBehaviour {
	public SoftwareInventory.SoftwareType type;
	public int version;

	void Start() {
		if (Const.a.difficultyMission == 0) {
			if (type == SoftwareInventory.SoftwareType.Data) this.gameObject.SetActive(false); // disable data objects when Mission difficulty is 0
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Player")) {
			PlayerMovement pm = other.gameObject.GetComponent<PlayerMovement>();
			if (pm != null) {
				if (pm.sinv != null) {
					if (pm.sinv.AddItem(type,version)) {
						GameObject explosionEffect = Const.a.GetObjectFromPool(Const.PoolType.CyberDissolve);
						if (explosionEffect != null) {
							explosionEffect.SetActive(true);
							explosionEffect.transform.position = transform.position; // put vaporization effect at raycast center
						}
						this.gameObject.SetActive(false); //we've been picked up, quick hide like you were
					}
				} else {Debug.Log("Failed to get SoftwareInventory on collision with player in CyberItem.cs");}
			}
		}
	}
}
