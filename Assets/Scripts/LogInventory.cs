using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogInventory : MonoBehaviour {
	public bool[] hasLog;
	public bool[] readLog;
	//public int[] logCountFromLevel;
	public int[] numLogsFromLevel;
	public static LogInventory a;
	public int lastAddedIndex = -1;
	public CenterTabButtons centerTabButtonsControl;
	private AudioSource SFXSource;
	private AudioClip SFXClip;
	public GameObject vmailbetajet;
	public GameObject vmailbridgesep;
	public GameObject vmailcitadestruct;
	public GameObject vmailgenstatus;
	public GameObject vmaillaserdest;
	public GameObject vmailshieldsup;
	public MouseLookScript mls;
	public bool beepDone = false;
	private int tempRefIndex = -1;

	void Awake() {
		SFXSource = GetComponent<AudioSource>();
		if (SFXSource == null)
			SFXSource = gameObject.AddComponent<AudioSource>();
		
		a = this;
	}

	void Update () {
		if(GetInput.a.RecentLog() && (HardwareInventory.a.hasHardware[2] == true)) {
			if (lastAddedIndex != -1) {
				PlayLog(lastAddedIndex);
				tempRefIndex = lastAddedIndex;
				lastAddedIndex = -1;
			} else {
				SFXSource.Stop();
				if (tempRefIndex != -1) lastAddedIndex = tempRefIndex;
				tempRefIndex = -1;
			}
		}
	}

	public void DeactivateVMail () {
		vmailbetajet.SetActive(false);
		vmailbridgesep.SetActive(false);
		vmailcitadestruct.SetActive(false);
		vmailgenstatus.SetActive(false);
		vmaillaserdest.SetActive(false);
		vmailshieldsup.SetActive(false);
		if(SFXSource != null) SFXSource.Stop();
	}

	public void PlayLog (int logIndex) {
		if (logIndex == -1) return;

		// Play the log audio
		//if (Const.a.audioLogType[logIndex] == 1 || Const.a.audioLogType[logIndex] == 3) {
			SFXClip = null;
			SFXSource.Stop();
			if (Const.a.audioLogs != null) SFXClip = Const.a.audioLogs[logIndex];
			if (SFXClip != null && SFXSource != null) SFXSource.PlayOneShot(SFXClip);
		//}

		readLog[logIndex] = true;
		if (Const.a.audioLogType[logIndex] == 4) {
			mls.vmailActive = true; // allow click to end
			mls.ForceInventoryMode();
			switch (logIndex) {
				case 119: vmailbetajet.SetActive(true); break;
				case 116: vmailbridgesep.SetActive(true); break;
				case 117: vmailcitadestruct.SetActive(true); break;
				case 110: vmailgenstatus.SetActive(true); break;
				case 114: vmaillaserdest.SetActive(true); break;
				case 120: vmailshieldsup.SetActive(true); break;
			}
		}
		//centerTabButtonsControl.TabButtonClickSilent(4,true);	
		MFDManager.a.SendAudioLogToDataTab(logIndex);
	}

	public void PlayLastAddedLog (int logIndex) {
		if (logIndex == -1) return;
		PlayLog(logIndex);
		lastAddedIndex = -1;
	}
}
