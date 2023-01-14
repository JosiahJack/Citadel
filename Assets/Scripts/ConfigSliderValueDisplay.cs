using UnityEngine;
using UnityEngine.UI;

public class ConfigSliderValueDisplay : MonoBehaviour {
	// Externally assigned, required.
	public Slider slideControl;

	// Internal references
	private Text self;

	void Awake () {
		self = GetComponent<Text>();
		if (self == null) Debug.Log("BUG: No Slider component for self on ConfigSliderValueDisplay.");
		if (slideControl == null) Debug.Log("BUG: ConfigSliderValueDisplay missing manually assigned reference for slideControl.");
		if (self == null || slideControl == null) this.enabled = false;
	}

	void Update() {
		self.text = slideControl.value.ToString();
	}
}
