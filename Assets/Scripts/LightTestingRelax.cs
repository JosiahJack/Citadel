using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTestingRelax : MonoBehaviour, IBatchUpdate {
	private Transform player1Ref;
	private Transform player2Ref;
	private Transform player3Ref;
	private Transform player4Ref;
	private List<GameObject> litGOs;
	void Start() {
		player1Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		player2Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		player3Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		player4Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		litGOs = new List<GameObject>();
		//Light[] childgoslights = GetComponentsInChildren<Light>();
		//for (int i=0;i<childgoslights.Length;i++) {
		//	lit.Add(childgoslights[i].gameObject);
		//}

		for (int i = 0; i < transform.childCount; ++i) {
			litGOs.Add(transform.GetChild(i).gameObject);
		}

		UpdateManager.Instance.RegisterSlicedUpdate(this, UpdateManager.UpdateMode.Always);
	}
    public void BatchUpdate() {
        //if (LevelManager.a.currentLevel != -1) return;
		for(int j=0;j<litGOs.Count;j++) {
			if (Vector3.Distance(player1Ref.position,litGOs[j].transform.position) < 79f) {
				if (!litGOs[j].activeSelf) litGOs[j].SetActive(true);
			} else {
				if (litGOs[j].activeSelf) litGOs[j].SetActive(false);
			}
		}
    }
}
