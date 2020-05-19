using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTestingRelax : MonoBehaviour {
	private Transform player1Ref;
	//private Transform player2Ref;
	//private Transform player3Ref;
	//private Transform player4Ref;
	private List<GameObject> litGOs;
	//private List<Light> lits;
	//private List<float> litsRanges;
	//public float maxLightVisibleRange = 79f;
	//public float maxLightUsesNormalRange = 30f;
	//public HardwareInvCurrent hwc;
	private float tick = 0.06f;
	private float tickFinished;
	private int count;

	void Start() {
		player1Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		//player2Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		//player3Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		//player4Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		litGOs = new List<GameObject>();
		//lits = new List<Light>();
		//litsRanges = new List<float>();
		//Light[] childgoslights = GetComponentsInChildren<Light>();
		//for (int i=0;i<childgoslights.Length;i++) {
		//	lit.Add(childgoslights[i].gameObject);
		//}
		count = transform.childCount;
		for (int i = 0; i < count; i++) {
			if (transform.GetChild(i).gameObject.GetComponent<Light>() != null)
				litGOs.Add(transform.GetChild(i).gameObject);
		}

		// for (int j = 0; j < count; j++) {
			// if (litGOs[j].GetComponent<Light>() != null)
				// lits.Add(litGOs[j].GetComponent<Light>());
				// litsRanges.Add(litGOs[j].GetComponent<Light>().range);
		// }

		tickFinished = Time.time + tick;
	}

    void Update() {
		//if (tickFinished < Time.time) {
			if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
				//if (LevelManager.a.currentLevel != -1) return;
				// if (hwc != null) {
					// if (hwc.hardwareIsActive[11]) {
						// for(int p=0;p<count;p++) {
							// if (litGOs[p].activeSelf) litGOs[p].SetActive(false);
						// }
						// return;
					// }
				// }

				for(int j=0;j<count;j++) {
					if ((player1Ref.position.x - litGOs[j].transform.position.x) > 35f && (player1Ref.position.z - litGOs[j].transform.position.z) > 35f) {
						//if (Vector3.Dot((player1Ref.position - litGOs[j].transform.position),player1Ref.transform.forward) > 0) {
							if (litGOs[j].activeSelf) litGOs[j].SetActive(false);
						} else {
							if (!litGOs[j].activeSelf) litGOs[j].SetActive(true);
						}
					// if (Vector3.Distance(player1Ref.position,litGOs[j].transform.position) < maxLightVisibleRange) {
						// if (!litGOs[j].activeSelf) litGOs[j].SetActive(true);
					// } else {
						// if (litGOs[j].activeSelf) litGOs[j].SetActive(false);
					// }
					//}
				}

				// for(int k=0;k<lits.Count;k++) {
					// if (lits[k] != null) {
						// if (Vector3.Distance(player1Ref.position,lits[k].gameObject.transform.position) < maxLightUsesNormalRange) {
							// if (lits[k].range != litsRanges[k]) lits[k].range = litsRanges[k];
						// } else {
							// if (lits[k].range != 2f) lits[k].range = 2f; //.5
						// }
					// }
				// }
			}
			//tickFinished = Time.time + tick;
		//}
    }
}
