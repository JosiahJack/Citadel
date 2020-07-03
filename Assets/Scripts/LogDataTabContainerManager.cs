using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogDataTabContainerManager : MonoBehaviour {
	public Text logName;
	public Text logSender;
	public Text logSubject;
	public Image logImage;

	public void SendLogData(int referenceIndex, bool isRH) {
		//Debug.Log("SendLogData received referenceIndex of " + referenceIndex.ToString());
		logName.text = Const.a.audiologNames[referenceIndex];
		if (!isRH) {
			logSender.text = Const.a.audiologSenders[referenceIndex];
			logSubject.text = Const.a.audiologSubjects[referenceIndex];
			logImage.overrideSprite = Const.a.logImages[Const.a.audioLogImagesRefIndicesLH[referenceIndex]];
		} else {
			logSender.text = System.String.Empty; // blank on RH side
			logSubject.text = System.String.Empty; // blank on RH side
			logImage.overrideSprite = Const.a.logImages[Const.a.audioLogImagesRefIndicesRH[referenceIndex]];
		}
	}
}
