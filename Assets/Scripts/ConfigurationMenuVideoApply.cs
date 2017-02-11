using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuVideoApply : MonoBehaviour {
	public Dropdown resolutionPicker;

	public void OnApplyClick () {
		Screen.SetResolution(Screen.resolutions[resolutionPicker.value].width,Screen.resolutions[resolutionPicker.value].height,true);
	}
}
