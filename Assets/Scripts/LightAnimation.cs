using UnityEngine;
using System.Collections;

public class LightAnimation : MonoBehaviour {
	[Tooltip("Set minimum intensity of light animations")]
	public float minIntensity = 0f;
	[Tooltip("Set maximum intensity of light animations (overrides Light settings)")]
	public float maxIntensity = 1f;
	[Tooltip("Whether the light is on")]
	public bool lightOn = true;  //useful for disabling light via switch, for instance  // save
	[Tooltip("Whether to interpolate smoothly between intensities")]
	public bool lerpOn = true; // save
	[Tooltip("Alternating lerpUp then lerpDown for set times (float)")]
	public float[] intervalSteps; //alternating
	[Tooltip("Whether each alternating step should lerp (bool)")]
	public bool[] intervalStepisLerping; //lerp for this step? assign per step
	[Tooltip("Current interval element in float array")]
	public int currentStep; //save

	private bool lerpUp;
	private bool noSteps = false;
	[HideInInspector] public float lerpTime = 0.5f; //save
	[HideInInspector] public float stepTime; //save
	[HideInInspector] public float lerpStartTime; //save
	private Light animLight;
	private float differenceInIntensity;
	[HideInInspector] public float lerpValue; //save

	void Start () {
		if (minIntensity < 0) minIntensity = 0;

		animLight = GetComponent<Light>();
		animLight.intensity = minIntensity;
		currentStep = 0;
		lerpUp = true;
		differenceInIntensity = (maxIntensity - minIntensity);
		if (intervalSteps.Length != 0) {
			stepTime = intervalSteps[currentStep];
			lerpTime = PauseScript.a.relativeTime + stepTime;
			lerpStartTime = PauseScript.a.relativeTime;
		} else {
			noSteps = true;
			animLight.intensity = maxIntensity;
		}
	}

	public void TurnOn() {
		lightOn = true;
		animLight.intensity = maxIntensity;
		animLight.enabled = true;
	}

	public void TurnOff() {
		lightOn = false;
		animLight.intensity = minIntensity;
		animLight.enabled = false;
	}

	public void Toggle() {
		if (lightOn) {
			TurnOff();
		} else {
			TurnOn();
		}
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (lightOn) {
				if (!noSteps) {
					if (lerpUp) {
						// Going from minIntensity to maxIntensity
						if (lerpTime < PauseScript.a.relativeTime) {
							if (animLight.intensity != maxIntensity) animLight.intensity = maxIntensity;
							lerpUp = false;
							currentStep++;
							if (currentStep == intervalSteps.Length)
								currentStep = 0;

							stepTime = intervalSteps[currentStep];
							lerpTime = PauseScript.a.relativeTime + stepTime;
							lerpStartTime = PauseScript.a.relativeTime;
							if (lerpTime == 0f)
								lerpTime = 0.1f;
						} else {
							if (lerpOn) {
								if (currentStep < intervalStepisLerping.Length) {
									if (intervalStepisLerping[currentStep]) {
										lerpValue = (PauseScript.a.relativeTime - lerpStartTime)/(lerpTime - lerpStartTime); // percent towards goal time
										lerpValue = minIntensity + (differenceInIntensity * (lerpValue));
										if (animLight.intensity != lerpValue) animLight.intensity = lerpValue;
									}
								}
							}
						}
					} else {
						// Going from maxIntensity to minIntensity
						if (lerpTime < PauseScript.a.relativeTime) {
							if (animLight.intensity != minIntensity) animLight.intensity = minIntensity;
							lerpUp = true;
							currentStep++;
							if (currentStep == intervalSteps.Length)
								currentStep = 0;
							
							stepTime = intervalSteps[currentStep];
							lerpTime = PauseScript.a.relativeTime + stepTime;
							lerpStartTime = PauseScript.a.relativeTime;
							if (lerpTime == 0f)
								lerpTime = 0.1f;
						} else {
							if (lerpOn) {
								if (currentStep == intervalSteps.Length)
									currentStep = 0;

								if (currentStep < intervalStepisLerping.Length) {
									if (intervalStepisLerping[currentStep]) {
										lerpValue = (PauseScript.a.relativeTime - lerpStartTime)/(lerpTime - lerpStartTime); // percent towards goal time
										lerpValue = minIntensity + (differenceInIntensity * (1-lerpValue));
										if (animLight.intensity != lerpValue) animLight.intensity = lerpValue;
									}
								}
							}
						}
					}

				} else {
					// Light is on but no steps so set to editor setting
					if (animLight.intensity != maxIntensity) animLight.intensity = maxIntensity;
				}
			} else {
				// Light is turned off.
				if (animLight.intensity != minIntensity) animLight.intensity = minIntensity;
			}
		}
	}
}
