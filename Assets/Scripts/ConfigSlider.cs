using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigSlider : MonoBehaviour {
	private Slider slideControl;
	public int index;

	void Start () {
		slideControl = GetComponent<Slider>();
		if (slideControl == null) {
			Debug.Log("ERROR: No slider for object with ConfigVolumeSlider script");
		}
		switch(index) {
		case 0: slideControl.value = Const.a.GraphicsFOV; break;
		case 1: slideControl.value = Const.a.GraphicsBrightness; break;
		case 2: slideControl.value = Const.a.GraphicsGamma; break;
		case 3: slideControl.value = Const.a.AudioVolumeMaster; break;
		case 4: slideControl.value = Const.a.AudioVolumeMusic; break;
		case 5: slideControl.value = Const.a.AudioVolumeMessage; break;
		case 6: slideControl.value = Const.a.AudioVolumeEffects; break;
		}
	}

	public void SetValue() {
		switch(index) {
		case 0: Const.a.GraphicsFOV = (int)slideControl.value; Const.a.SetFOV(); break;
		case 1: Const.a.GraphicsBrightness = (int)slideControl.value; Const.a.SetBrightness(); break;
		case 2: Const.a.GraphicsGamma = (int)slideControl.value; Const.a.SetBrightness(); break;
		case 3: Const.a.AudioVolumeMaster = (int)slideControl.value; Const.a.SetVolume(); break;
		case 4: Const.a.AudioVolumeMusic = (int)slideControl.value; Const.a.SetVolume(); break;
		case 5: Const.a.AudioVolumeMessage = (int)slideControl.value; Const.a.SetVolume(); break;
		case 6: Const.a.AudioVolumeEffects = (int)slideControl.value; Const.a.SetVolume(); break;
		}
		Const.a.WriteConfig();
	}
}
