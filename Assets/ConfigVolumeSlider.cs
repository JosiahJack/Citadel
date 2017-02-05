using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigVolumeSlider : MonoBehaviour {
	private Slider slideControl;

	void Awake () {
		slideControl = GetComponent<Slider>();
		if (slideControl == null) {
			Debug.Log("ERROR: No slider for object with ConfigVolumeSlider script");
		}
	}

	public void SetVolume(int type) {
		switch(type) {
		case 0:
			Const.a.volumeMaster = slideControl.value;
			break;
		case 1:
			Const.a.volumeMusic = slideControl.value;
			break;
		case 2:
			Const.a.volumeMessage = slideControl.value;
			break;
		case 3:
			Const.a.volumeAmbience = slideControl.value;
			break;
		}

		Const.a.SetVolume();
	}
}
