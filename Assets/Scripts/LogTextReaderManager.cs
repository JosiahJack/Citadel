using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogTextReaderManager : MonoBehaviour {
	public GameObject moreButton;
	public GameObject logTextOutput;
	public Text moreButtonText;
	public MFDManager mfdManager;
	public GameObject dataTabAudioLogContainer;
	public GameObject dataTabManager;
	
	// Update is called once per frame
	void Update () {
		if (logTextOutput.GetComponent<Text>().text.Length > 568) {
			moreButtonText.text = "[MORE]";
		} else {
			moreButtonText.text = "[CLOSE]";
		}
	}

	public void SendTextToReader(int referenceIndex) {
		logTextOutput.GetComponent<Text>().text = Const.a.audioLogSpeech2Text[referenceIndex];
		mfdManager.OpenTab(4,true,MFDManager.TabMSG.AudioLog,referenceIndex);
		//dataTabManager.GetComponent<DataTab>().Reset();
		//dataTabAudioLogContainer.SetActive(true);
		//dataTabAudioLogContainer.GetComponent<LogDataTabContainerManager>().SendLogData(referenceIndex);
	}
}
