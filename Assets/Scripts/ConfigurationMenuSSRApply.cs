using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuSSRApply : MonoBehaviour {
	private Dropdown picker;

	void Awake() {
		picker = GetComponent<Dropdown>();
		if (picker == null) Debug.Log("BUG: ConfigurationMenuAAApply missing component for aaPicker.");
	}

	public void OnDropdownSelect () {
		if (picker != null)
			Const.a.GraphicsSSRMode = picker.value;
		else
			Const.a.GraphicsSSRMode = 0; // Default to off

		Config.WriteConfig();
		Config.SetSSR();
	}
}
