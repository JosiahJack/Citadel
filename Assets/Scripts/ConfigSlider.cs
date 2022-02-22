using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigSlider : MonoBehaviour {
	public int index;
	private Slider slideControl;

	void Start () {
		if (index < 0 || index >= 7) { index = 0; Debug.Log("BUG: Setting index on ConfigSlider to 0 because it was outside the range >=0index<7");}
		slideControl = GetComponent<Slider>();
		if (slideControl == null) Debug.Log("ERROR: No slider component for object with ConfigSlider script");
		switch(index) {
			case 0: slideControl.value = Const.a.GraphicsFOV; break;
			case 1: slideControl.value = Const.a.GraphicsGamma; break;
			case 2: slideControl.value = Const.a.AudioVolumeMaster; break;
			case 3: slideControl.value = Const.a.AudioVolumeMusic; break;
			case 4: slideControl.value = Const.a.AudioVolumeMessage; break;
			case 5: slideControl.value = Const.a.AudioVolumeEffects; break;
			case 6: slideControl.value = (Const.a.MouseSensitivity/2.01f*100f); break;
		}
	}

	public void SetValue() {
		switch(index) {
			case 0: Const.a.GraphicsFOV = (int)slideControl.value; Const.a.SetFOV(); break;
			case 1: Const.a.GraphicsGamma = (int)slideControl.value; Const.a.SetBrightness(); break;
			case 2: Const.a.AudioVolumeMaster = (int)slideControl.value; Const.a.SetVolume(); break;
			case 3: Const.a.AudioVolumeMusic = (int)slideControl.value; Const.a.SetVolume(); break;
			case 4: Const.a.AudioVolumeMessage = (int)slideControl.value; Const.a.SetVolume(); break;
			case 5: Const.a.AudioVolumeEffects = (int)slideControl.value; Const.a.SetVolume(); break;
			case 6: Const.a.MouseSensitivity = ((slideControl.value/100f) * 2f) + 0.01f; break;
		}
		Const.a.WriteConfig();
	}
}
