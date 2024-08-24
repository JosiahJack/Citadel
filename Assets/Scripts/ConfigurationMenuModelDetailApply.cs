using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuModelDetailApply : MonoBehaviour {
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
				case 0: shadList.Add(Const.a.stringTable[914]); break; // No Detail
				case 1: shadList.Add(Const.a.stringTable[915]); break; // High Detail
			}
		}
		picker.ClearOptions();
		picker.AddOptions(shadList);
	}

	void Initialize() {
		if (picker == null) picker = GetComponent<Dropdown>();
		if (picker == null) Debug.Log("BUG: ConfigurationMenuModelDetailApply missing component for picker.");

		SetOptionsText();
		if (picker.value != Const.a.GraphicsModelDetail) {
			picker.value = Const.a.GraphicsModelDetail;
		}
	}

	public void OnDropdownSelect () {
		if (picker != null)
			Const.a.GraphicsModelDetail = picker.value;
		else
			Const.a.GraphicsModelDetail = 0; // Default to off, huge performance impact

		Config.WriteConfig();
		Config.SetModelDetail();
	}
}
