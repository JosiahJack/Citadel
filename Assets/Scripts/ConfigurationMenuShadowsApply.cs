using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuShadowsApply : MonoBehaviour {
	private Dropdown picker;

	void Awake() {
		picker = GetComponent<Dropdown>();
		if (picker == null) Debug.Log("BUG: ConfigurationMenuShadowsApply missing component for picker.");
	}

	void OnEnable() {
		if (picker != null) picker.value = Const.a.GraphicsShadowMode;
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
