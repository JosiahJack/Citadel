using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuAAApply : MonoBehaviour {
	private Dropdown aaPicker;

	void Awake() {
		aaPicker = GetComponent<Dropdown>();
		if (aaPicker == null) Debug.Log("BUG: ConfigurationMenuAAApply missing component for aaPicker.");
	}

	void OnEnable() {
		if (aaPicker != null) aaPicker.value = Const.a.GraphicsAAMode;
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
