using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuAudioModeApply : MonoBehaviour {
	private Dropdown picker;

	void Start() { // Wait for Const.a. to initialize.
		Initialize();
	}

	void OnEnable() {
		Initialize();
	}
	
	public void SetOptionsText() {
		if (Const.a == null) return;
		if (!Const.a.stringTableLoaded) return;
		if (picker == null) return;

		List<string> ssrList = new List<string>();
		for (int i=0;i<7;i++) {
			switch(i) {
				case 0: ssrList.Add(Const.a.stringTable[795]); break;
				case 1: ssrList.Add(Const.a.stringTable[796]); break;
				case 2: ssrList.Add(Const.a.stringTable[797]); break;
				case 3: ssrList.Add(Const.a.stringTable[798]); break;
				case 4: ssrList.Add(Const.a.stringTable[799]); break;
				case 5: ssrList.Add(Const.a.stringTable[800]); break;
				case 6: ssrList.Add(Const.a.stringTable[801]); break;
			}
		}
		picker.ClearOptions();
		picker.AddOptions(ssrList);
	}

	void Initialize() {
		if (picker == null) picker = GetComponent<Dropdown>();
		if (picker == null) {
			Debug.Log("BUG: ConfigurationMenuAudioModeApply missing component for "
					  + "aaPicker.");
		}

		SetOptionsText();
		if (picker.value != Const.a.AudioSpeakerMode) {
			picker.value = Const.a.AudioSpeakerMode;
		}
	}

	public void OnDropdownSelect () {
		if (picker != null) Const.a.AudioSpeakerMode = picker.value;
		else Const.a.AudioSpeakerMode = 1; // Default to Stereo

		Config.WriteConfig();
		Config.SetAudioMode();
	}
}
