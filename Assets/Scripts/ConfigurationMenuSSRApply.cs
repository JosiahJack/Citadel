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

	void Initialize() {
		if (picker == null) picker = GetComponent<Dropdown>();
		if (picker == null) {
			Debug.Log("BUG: ConfigurationMenuAAApply missing component for "
					  + "aaPicker.");
		}

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
