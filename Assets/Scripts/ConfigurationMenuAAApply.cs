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

	void Initialize() {
		if (aaPicker == null) aaPicker = GetComponent<Dropdown>();
		if (aaPicker == null) {
			Debug.Log("BUG: ConfigurationMenuAAApply missing component for aaPicker.");
			return;
		}

		aaPicker.value = Const.a.GraphicsAAMode;
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
