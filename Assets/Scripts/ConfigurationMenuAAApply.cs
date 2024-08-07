using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuAAApply : MonoBehaviour {
	private Dropdown aaPicker;

	void Start() { // Wait for Const.a. to initialize.
		Initialize();
	}

	void OnEnable() {
		Initialize();
	}
	
	public void SetOptionsText() {
		if (Const.a == null) return;
		if (!Const.a.stringTableLoaded) return;
		if (aaPicker == null) return;

		List<string> aaList = new List<string>();
		for (int i=0;i<6;i++) {
			switch(i) {
				case 0: aaList.Add(Const.a.stringTable[779]); break;
				case 1: aaList.Add(Const.a.stringTable[780]); break;
				case 2: aaList.Add(Const.a.stringTable[781]); break;
				case 3: aaList.Add(Const.a.stringTable[782]); break;
				case 4: aaList.Add(Const.a.stringTable[783]); break;
				case 5: aaList.Add(Const.a.stringTable[784]); break;
			}
		}
		aaPicker.ClearOptions();
		aaPicker.AddOptions(aaList);
	}

	void Initialize() {
		if (aaPicker == null) aaPicker = GetComponent<Dropdown>();
		if (aaPicker == null) {
			Debug.Log("BUG: ConfigurationMenuAAApply missing component for aaPicker.");
			return;
		}
		
		SetOptionsText();
		if (aaPicker.value != Const.a.GraphicsAAMode) {
			aaPicker.value = Const.a.GraphicsAAMode;
		}
	}

	public void OnDropdownSelect () {
		if (aaPicker != null)
			Const.a.GraphicsAAMode = aaPicker.value;
		else
			Const.a.GraphicsAAMode = 1; // Default to FXAA Extreme Performance

		Config.WriteConfig();
		Config.SetAA();
	}
}
