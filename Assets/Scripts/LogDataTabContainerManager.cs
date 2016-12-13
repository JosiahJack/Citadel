using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogDataTabContainerManager : MonoBehaviour {
	public Text logName;
	public Text logSender;
	public Text logSubject;

	public void SendLogData(int referenceIndex) {
		logName.text = Const.a.audiologNames[referenceIndex];
		logSender.text = Const.a.audiologSenders[referenceIndex];
		logSubject.text = Const.a.audiologSubjects[referenceIndex];
	}
}
