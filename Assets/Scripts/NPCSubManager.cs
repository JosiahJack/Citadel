using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSubManager : MonoBehaviour {
	public int levelID = 0;
	public GameObject[] childrenNPCs;
	public AIController[] childrenNPCsAICs;
	private int count = 0;

	void Awake() {
		count = transform.childCount;
		SaveObject so;
		AIController aic;
		childrenNPCs = new GameObject[count];
		childrenNPCsAICs = new AIController[count];
		for (int i=0;i<count;i++) {
			childrenNPCs[i] = transform.GetChild(i).gameObject;
			so = childrenNPCs[i].GetComponent<SaveObject>();
			if (so != null) {
				so.levelParentID = levelID;
			}
			aic = childrenNPCs[i].GetComponent<AIController>();
			if (aic != null) {
				childrenNPCsAICs[i] = aic;
			}
			
		}
	}

	public void PullOutNPCs() {
		for (int i=0;i<count;i++) {
			//childrenNPCs[i].transform.SetParent(null);
		}
	}

	public void PutBackNPCs() {
		for (int i=0;i<count;i++) {
			//childrenNPCs[i].transform.SetParent(transform);
		}
	}
}
