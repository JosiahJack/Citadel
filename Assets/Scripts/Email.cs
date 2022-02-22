using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Email : MonoBehaviour {
	public int emailIndex;
	public bool autoPlayEmail = false;

    public void Targetted() {
		// Give email.
		if (Inventory.a.hasLog[emailIndex]) return; // Already have it.
		Inventory.a.hasLog[emailIndex] = true;
		Inventory.a.lastAddedIndex = emailIndex;
		if (Const.a.audioLogType[emailIndex] == 2) Inventory.a.beepDone = true;
		if (autoPlayEmail) Inventory.a.PlayLastAddedLog(emailIndex);
	}
}
