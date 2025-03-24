using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationMenuVideoApply : MonoBehaviour {
	public Dropdown resolutionPicker;

	public void OnApplyClick () {
		int x = Screen.resolutions[resolutionPicker.value].width;
		int y = Screen.resolutions[resolutionPicker.value].height;
		Const.sprint(Const.a.stringTable[1016] + x.ToString() + ", "
				     + y.ToString() + ", " + Const.a.stringTable[1017] + ": "
				     + Const.a.GraphicsFullscreen.ToString());
		Screen.SetResolution(x,y,true);
		Screen.fullScreen = Const.a.GraphicsFullscreen;
		Const.a.GraphicsResWidth = Screen.resolutions[resolutionPicker.value].width;
		Const.a.GraphicsResHeight = Screen.resolutions[resolutionPicker.value].height;
		Config.WriteConfig();
	}
}
