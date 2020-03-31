using UnityEngine;
using System.Collections;

public class LightAnimation : MonoBehaviour, IBatchUpdate {
	[Tooltip("Set minimum intensity of light animations")]
	public float minIntensity = 0f;
	[Tooltip("Set maximum intensity of light animations (overrides Light settings)")]
	public float maxIntensity = 1f;
	[Tooltip("Whether the light is on")]
	public bool lightOn = true;  //useful for disabling light via switch, for instance
	[Tooltip("Whether to interpolate smoothly between intensities")]
	public bool lerpOn = true;
	[Tooltip("Alternating lerpUp then lerpDown for set times (float)")]
	public float[] intervalSteps; //alternating
	[Tooltip("Whether each alternating step should lerp (bool)")]
	public bool[] intervalStepisLerping; //lerp for this step? assign per step
	[Tooltip("Current interval element in float array")]
	public int currentStep;
	public float lerpRef = 0f;
	private bool lerpUp;
	private bool noSteps = false;
	private float lerpTime = 0.5f;
	private float stepTime;
	private float lerpStartTime;
	private Light animLight;
	private float differenceInIntensity;
	private float setIntensity;
	private float lerpValue;

	void Start () {
		animLight = GetComponent<Light>();
		animLight.intensity = minIntensity;
		currentStep = 0;
		//lightOn = true;
		lerpUp = true;
		differenceInIntensity = (maxIntensity - minIntensity);
		if (intervalSteps.Length != 0) {
			stepTime = intervalSteps[currentStep];
			lerpTime = Time.time + stepTime;
			lerpStartTime = Time.time;
		} else {
			noSteps = true;
			//setIntensity = GetComponent<Light>().intensity;
			animLight.intensity = maxIntensity;
		}
		UpdateManager.Instance.RegisterSlicedUpdate(this, UpdateManager.UpdateMode.BucketB);
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

	public void BatchUpdate () {
		if (lightOn) {
			if (!noSteps) {
				if (lerpUp) {
					// Going from minIntensity to maxIntensity
					if (lerpTime < Time.time) {
						animLight.intensity = maxIntensity;
						lerpUp = false;
						currentStep++;
						if (currentStep == intervalSteps.Length)
							currentStep = 0;

						stepTime = intervalSteps[currentStep];
						lerpTime = Time.time + stepTime;
						lerpStartTime = Time.time;
						if (lerpTime == 0f)
							lerpTime = 0.1f;
					} else {
						if (lerpOn) {
							if (currentStep < intervalStepisLerping.Length) {
								if (intervalStepisLerping[currentStep]) {
									lerpValue = (Time.time - lerpStartTime)/(lerpTime - lerpStartTime); // percent towards goal time
									animLight.intensity = minIntensity + (differenceInIntensity * (lerpValue));
								}
							}
						}
					}
				} else {
					// Going from maxIntensity to minIntensity
					if (lerpTime < Time.time) {
						animLight.intensity = minIntensity;
						lerpUp = true;
						currentStep++;
						if (currentStep == intervalSteps.Length)
							currentStep = 0;
						
						stepTime = intervalSteps[currentStep];
						lerpTime = Time.time + stepTime;
						lerpStartTime = Time.time;
						if (lerpTime == 0f)
							lerpTime = 0.1f;
					} else {
						if (lerpOn) {
							if (currentStep == intervalSteps.Length)
								currentStep = 0;

							if (currentStep < intervalStepisLerping.Length) {
								if (intervalStepisLerping[currentStep]) {
									lerpValue = (Time.time - lerpStartTime)/(lerpTime - lerpStartTime); // percent towards goal time
									animLight.intensity = minIntensity + (differenceInIntensity * (1-lerpValue));
								}
							}
						}
					}
				}

			} else {
				// Light is on but no steps so set to editor setting
				animLight.intensity = maxIntensity;
			}
		} else {
			// Light is turned off.
			animLight.intensity = minIntensity;
		}
	}
}
