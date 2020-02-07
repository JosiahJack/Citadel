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

/* From MouseLookScript.cs for reference
	void AddAudioLogToInventory () {
		if ((heldObjectCustomIndex != -1) && (logInventory != null)) {
			logInventory.hasLog[heldObjectCustomIndex] = true;
			logInventory.lastAddedIndex = heldObjectCustomIndex;
			int levelnum = Const.a.audioLogLevelFound[heldObjectCustomIndex];
			logInventory.numLogsFromLevel[levelnum]++;
			logContentsManager.InitializeLogsFromLevelIntoFolder();
			string audName = Const.a.audiologNames[heldObjectCustomIndex];
			string logPlaybackKey = Const.a.InputConfigNames[20];
			Const.sprint("Audio log " + audName + " picked up.  Press '" + logPlaybackKey + "' to playback.",player);
		} else {
			if (logInventory == null) {
				Const.sprint("Warning: logInventory is null",player);
			} else {
				Const.sprint("Warning: Audio log picked up has no assigned index (-1)",player);
			}
		}
	}
*/
}
