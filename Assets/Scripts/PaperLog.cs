using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperLog : MonoBehaviour {
	public int logIndex;

	public void Use (UseData ud) {
		MFDManager.a.CenterTabButtonClickSilent(4,true);
		MFDManager.a.SendPaperLogToDataTab(logIndex,transform.position);
		MouseLookScript.a.ForceInventoryMode();
	}
}
