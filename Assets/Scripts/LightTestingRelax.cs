using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTestingRelax : MonoBehaviour {
	public bool activated = true;
	private Light lit;
	private Transform player1Ref;
	private Transform player2Ref;
	private Transform player3Ref;
	private Transform player4Ref;
	void Start() {
		lit = GetComponent<Light>();
		player1Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		player2Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		player3Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		player4Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
	}
    void Update() {
		/*
        if (LevelManager.a.currentLevel != -1) return;
		if (lit != null && activated) {
			if (Vector3.Distance(Const.a.player1.transform.position,transform.position) < 20f) {
				lit.enabled = true;
			} else {
				lit.enabled = false;
			}
		}*/
    }
}
