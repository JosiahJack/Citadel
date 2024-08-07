using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuSSRApply : MonoBehaviour {
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
		for (int i=0;i<3;i++) {
			switch(i) {
				case 0: ssrList.Add(Const.a.stringTable[788]); break;
				case 1: ssrList.Add(Const.a.stringTable[789]); break;
				case 2: ssrList.Add(Const.a.stringTable[790]); break;
			}
		}
		picker.ClearOptions();
		picker.AddOptions(ssrList);
	}

	void Initialize() {
		if (picker == null) picker = GetComponent<Dropdown>();
		if (picker == null) {
			Debug.Log("BUG: ConfigurationMenuSSRApply missing component for "
					  + "aaPicker.");
		}

		SetOptionsText();
		if (picker.value != Const.a.GraphicsSSRMode) {
			picker.value = Const.a.GraphicsSSRMode;
		}
	}

	public void OnDropdownSelect () {
		if (picker != null) Const.a.GraphicsSSRMode = picker.value;
		else Const.a.GraphicsSSRMode = 0; // Default to off

		Config.WriteConfig();
		Config.SetSSR();
	}
}
