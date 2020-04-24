using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Email : MonoBehaviour {
	public int emailIndex;
	public bool autoPlayEmail = false;
	private LogInventory pinv;

    public void Targetted() {
		SendEmailToPlayer(LogInventory.a, emailIndex); // give the email
	}

	void SendEmailToPlayer(LogInventory linv, int index) {
		if (linv.hasLog[index]) return; // already have it
		linv.hasLog[index] = true;
		linv.lastAddedIndex = index;
		if (Const.a.audioLogType[index] == 2) {
			LogInventory.a.beepDone = true;
		}
		if (autoPlayEmail) linv.PlayLastAddedLog(index);
	}
}
