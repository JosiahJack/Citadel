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
				lastAddedIndex = -1;
			}
		}
	}

	public void PlayLog (int logIndex) {
		if (logIndex == -1) return;

		// Play the log audio
		if (Const.a.audioLogType[logIndex] == 1) {
			SFXClip = Const.a.audioLogs[logIndex];
			SFXSource.PlayOneShot(SFXClip);
		}
			
		MFDManager.a.SendAudioLogToDataTab(logIndex);
		centerTabButtonsControl.TabButtonClickSilent(4,true);
	}

	public void PlayLastAddedLog (int logIndex) {
		if (logIndex == -1) return;
		PlayLog(logIndex);
		lastAddedIndex = -1;
	}
}
