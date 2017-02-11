using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigSliderValueDisplay : MonoBehaviour {
	public Slider slideControl;
	private Text self;

	void Awake () {
		self = GetComponent<Text>();
		if (self == null) {
			Debug.Log("ERROR: No slider for object with ConfigSliderValueDisplay script");
		}
	}

	void Update () {
		self.text = slideControl.value.ToString();
	}
}
