using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Email : MonoBehaviour {
	public int emailIndex;
	public bool autoPlayEmail = false;
	private LogInventory pinv;

    public void Targetted() {
		if (Const.a.player1 != null) {
			LogInventory pinv = Const.a.player1.GetComponent<PlayerReferenceManager>().playerInventory.GetComponent<LogInventory>();
			SendEmailToPlayer(pinv, emailIndex); // give the email
		}

		if (Const.a.player2 != null) {
			LogInventory pinv = Const.a.player2.GetComponent<PlayerReferenceManager>().playerInventory.GetComponent<LogInventory>();
			SendEmailToPlayer(pinv, emailIndex); // give the email
		}

		if (Const.a.player3 != null) {
			LogInventory pinv = Const.a.player3.GetComponent<PlayerReferenceManager>().playerInventory.GetComponent<LogInventory>();
			SendEmailToPlayer(pinv, emailIndex); // give the email
		}

		if (Const.a.player4 != null) {
			LogInventory pinv = Const.a.player4.GetComponent<PlayerReferenceManager>().playerInventory.GetComponent<LogInventory>();
			SendEmailToPlayer(pinv, emailIndex); // give the email
		}
	}

	void SendEmailToPlayer(LogInventory linv, int index) {
		linv.hasLog[index] = true;
		linv.lastAddedIndex = index;
		if (autoPlayEmail) linv.PlayLastAddedLog(index);
	}
}
