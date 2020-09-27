using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberMine : MonoBehaviour {
	private float dmg = 22f;

    void Start() {
		dmg = 55f;
        if (Const.a.difficultyCyber < 3) {
			if (UnityEngine.Random.Range(0,1f) < 0.2f) gameObject.SetActive(false); // 20% chance of not spawning on normal
			dmg = 33f;
		}

        if (Const.a.difficultyCyber < 2) {
			if (UnityEngine.Random.Range(0,1f) < 0.33f) gameObject.SetActive(false); // 33% chance of not spawning on easy
			dmg = 22f;
		}

        if (Const.a.difficultyCyber < 1) {
			if (UnityEngine.Random.Range(0,1f) < 0.50f) gameObject.SetActive(false); // 50% chance of not spawning on grandma
			dmg = 11f;
		}
    }

	void  OnTriggerEnter (Collider col) {
		if (col.gameObject.CompareTag("Player")) {
			PlayerMovement pm = col.gameObject.GetComponent<PlayerMovement>();
			if (pm != null) {
				DamageData damageData = new DamageData();
				damageData.other = gameObject;
				damageData.isOtherNPC = false;
				damageData.attacknormal = (transform.position - col.transform.position);
				damageData.owner = gameObject;
				damageData.attackType = Const.AttackType.None;
				damageData.damage = dmg;
				pm.hm.TakeDamage(damageData);
				pm.hm.ph.PlayerNoise.PlayOneShot(pm.hm.ph.CyberMineSFXClip);
				gameObject.SetActive(false);
			}
		}
	}
}
