using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSubManager : MonoBehaviour {
	public int levelID = 0;
	public GameObject[] childrenNPCs;
	public AIController[] childrenNPCsAICs;

	void Awake() {
		RepopulateChildList();
	}

	public void RepopulateChildList() {
		int count = transform.childCount;
		if (count < 1) return;

		AIController aic;
		childrenNPCs = new GameObject[count];
		childrenNPCsAICs = new AIController[count];
		for (int i=0;i<count;i++) {
			childrenNPCs[i] = transform.GetChild(i).gameObject;
			aic = childrenNPCs[i].GetComponent<AIController>();
			if (aic != null) {
				childrenNPCsAICs[i] = aic;
			}
		}
	}

	// There is in fact a not so inconsequential performance gain by having
	// NPCs pulled out with no parent.  However, there are some weird things
	// happen as a consequence of them belonging to particular levels and need
	// managed very carefully.  Leaving these here as a conceptual reminder,
	// but probably not going to use these.
	//public void PullOutNPCs() {
		//for (int i=0;i<count;i++) {
			//childrenNPCs[i].transform.SetParent(null);
		//}
	//}

	//public void PutBackNPCs() {
	//	for (int i=0;i<count;i++) {
			//childrenNPCs[i].transform.SetParent(transform);
	//	}
	//}
}
