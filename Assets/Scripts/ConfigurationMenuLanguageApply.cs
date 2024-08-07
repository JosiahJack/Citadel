using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuLanguageApply : MonoBehaviour {
	private Dropdown picker;

	void Start() { // Wait for Const.a. to initialize.
		Initialize();
	}

	void OnEnable() {
		Initialize();
	}

	void Initialize() {
		if (picker == null) picker = GetComponent<Dropdown>();
		if (picker == null) Debug.Log("BUG: ConfigurationMenuLanguageApply missing component for picker.");

		picker.value = Const.a.AudioLanguage;
	}

	public void OnDropdownSelect () {
		Debug.Log("Language select");
		if (picker != null)
			Const.a.AudioLanguage = picker.value;
		else
			Const.a.AudioLanguage = 0; // Default to English

		Config.WriteConfig();
		Config.SetLanguage();
	}
}
