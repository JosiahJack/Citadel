using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuShadowsApply : MonoBehaviour {
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

		List<string> shadList = new List<string>();
		for (int i=0;i<3;i++) {
			switch(i) {
				case 0: shadList.Add(Const.a.stringTable[785]); break;
				case 1: shadList.Add(Const.a.stringTable[786]); break;
				case 2: shadList.Add(Const.a.stringTable[787]); break;
			}
		}
		picker.ClearOptions();
		picker.AddOptions(shadList);
	}

	void Initialize() {
		if (picker == null) picker = GetComponent<Dropdown>();
		if (picker == null) Debug.Log("BUG: ConfigurationMenuShadowsApply missing component for picker.");

		SetOptionsText();
		if (picker.value != Const.a.GraphicsShadowMode) {
			picker.value = Const.a.GraphicsShadowMode;
		}
	}

	public void OnDropdownSelect () {
		if (picker != null)
			Const.a.GraphicsShadowMode = picker.value;
		else
			Const.a.GraphicsShadowMode = 0; // Default to off, huge performance impact

		Config.WriteConfig();
		Config.SetShadows();
	}
}
