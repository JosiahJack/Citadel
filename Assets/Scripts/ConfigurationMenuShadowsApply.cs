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

	void Initialize() {
		if (picker == null) picker = GetComponent<Dropdown>();
		if (picker == null) Debug.Log("BUG: ConfigurationMenuShadowsApply missing component for picker.");

		picker.value = Const.a.GraphicsShadowMode;
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
