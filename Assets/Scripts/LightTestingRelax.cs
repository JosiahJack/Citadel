using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTestingRelax : MonoBehaviour, IBatchUpdate {
	private Transform player1Ref;
	private Transform player2Ref;
	private Transform player3Ref;
	private Transform player4Ref;
	private List<GameObject> litGOs;
	private List<Light> lits;
	private List<float> litsRanges;
	public float maxLightVisibleRange = 79f;
	public float maxLightUsesNormalRange = 30f;
	public HardwareInvCurrent hwc;

	void Start() {
		player1Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		player2Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		player3Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		player4Ref = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
		litGOs = new List<GameObject>();
		lits = new List<Light>();
		litsRanges = new List<float>();
		//Light[] childgoslights = GetComponentsInChildren<Light>();
		//for (int i=0;i<childgoslights.Length;i++) {
		//	lit.Add(childgoslights[i].gameObject);
		//}

		for (int i = 0; i < transform.childCount; i++) {
			if (transform.GetChild(i).gameObject.GetComponent<Light>() != null)
				litGOs.Add(transform.GetChild(i).gameObject);
		}

		for (int j = 0; j < litGOs.Count; j++) {
			if (litGOs[j].GetComponent<Light>() != null)
				lits.Add(litGOs[j].GetComponent<Light>());
				litsRanges.Add(litGOs[j].GetComponent<Light>().range);
		}

		UpdateManager.Instance.RegisterSlicedUpdate(this, UpdateManager.UpdateMode.Always);
	}
    public void BatchUpdate() {
        //if (LevelManager.a.currentLevel != -1) return;
		if (hwc != null) {
			if (hwc.hardwareIsActive[11]) {
				for(int p=0;p<litGOs.Count;p++) {
					if (litGOs[p].activeSelf) litGOs[p].SetActive(false);
				}
				return;
			}
		}

		for(int j=0;j<litGOs.Count;j++) {
			if (Vector3.Distance(player1Ref.position,litGOs[j].transform.position) < maxLightVisibleRange) {
				if (!litGOs[j].activeSelf) litGOs[j].SetActive(true);
			} else {
				if (litGOs[j].activeSelf) litGOs[j].SetActive(false);
			}
		}

		for(int k=0;k<lits.Count;k++) {
			if (lits[k] != null) {
				if (Vector3.Distance(player1Ref.position,lits[k].gameObject.transform.position) < maxLightUsesNormalRange) {
					lits[k].range = litsRanges[k];
				//if (lits[k].range > 2f) {
					//lits[k].enabled = false;
				} else {
					lits[k].range = 2f; //.5
					//brightness*2.5
					//lits[k].enabled = true;
				}
			}
		}
    }
}
