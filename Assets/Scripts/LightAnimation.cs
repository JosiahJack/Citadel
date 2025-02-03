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
	[HideInInspector] public Light animLight;
	private float differenceInIntensity;
	[HideInInspector] public float lerpValue; //save
	private GameObject segiEmitter;

	void Start () {
		if (minIntensity < 0.01f) minIntensity = 0.01f;
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
		
		if (segiEmitter == null) segiEmitter = Utils.CreateSEGIEmitter(gameObject,LevelManager.a.currentLevel,0,animLight);
	}
	
	private void EnableSEGIEmitter() {
        if (segiEmitter == null) return;
        
        segiEmitter.SetActive(true);
    }
    
    private void DisableSEGIEmitter() {
        if (segiEmitter == null) return;
        
        segiEmitter.SetActive(false);
    }
    
    public void ScaleSEGIEmitter() {
        if (segiEmitter == null) return;
		
		float fac = (animLight.intensity - minIntensity) / maxIntensity;
		segiEmitter.transform.localScale = new Vector3(animLight.range * Const.a.segiVoxelSize * fac,
													   animLight.range * Const.a.segiVoxelSize * fac,
													   animLight.range * Const.a.segiVoxelSize * fac);
    }

	public void TurnOn() {
		lightOn = true;
		EnableSEGIEmitter();
		animLight.intensity = maxIntensity;
		animLight.enabled = true;
	}

	public void TurnOff() {
		lightOn = false;
		DisableSEGIEmitter();
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
							if (animLight.intensity != maxIntensity) {
								animLight.intensity = maxIntensity;
								ScaleSEGIEmitter();
							}
							
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
										if (animLight.intensity != lerpValue) {
											animLight.intensity = lerpValue;
											ScaleSEGIEmitter();
										}
									}
								}
							}
						}
					} else {
						// Going from maxIntensity to minIntensity
						if (lerpTime < PauseScript.a.relativeTime) {
							if (animLight.intensity != minIntensity) {
								animLight.intensity = minIntensity;
								ScaleSEGIEmitter();
							}
							
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
										if (animLight.intensity != lerpValue) {
											animLight.intensity = lerpValue;
											ScaleSEGIEmitter();
										}
									}
								}
							}
						}
					}

				} else {
					// Light is on but no steps so set to editor setting
					if (animLight.intensity != maxIntensity) {
						animLight.intensity = maxIntensity;
						ScaleSEGIEmitter();
					}
				}
			} else {
				// Light is turned off.
				if (animLight.intensity != minIntensity) {
					animLight.intensity = minIntensity;
					ScaleSEGIEmitter();
				}
			}
		}
	}

	public static string Save(GameObject go) {
		LightAnimation la = go.GetComponent<LightAnimation>();
		if (la == null) {
			Debug.Log("LightAnimation missing on savetype of Light!  GameObject.name: " + go.name);
			return "1|0|0|0000.00000|0000.00000|0000.00000|0000.00000";
		}

		if (la.animLight == null) la.animLight = la.GetComponent<Light>();

		string line = System.String.Empty;
		line = Utils.BoolToString(la.lightOn,"lightOn"); // bool
		line += Utils.splitChar + Utils.BoolToString(la.lerpOn,"lerpOn"); // bool
		line += Utils.splitChar + Utils.IntToString(la.currentStep,"currentStep"); // int
		line += Utils.splitChar + Utils.FloatToString(la.lerpValue,"lerpValue"); // %
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(la.lerpTime,"lerpTime");
		line += Utils.splitChar + Utils.FloatToString(la.stepTime,"stepTime"); // Not a timer, current time amount
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(la.lerpStartTime,"lerpStartTime");
		line += Utils.splitChar + Utils.BoolToString(la.animLight.enabled,"light.enabled");
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		LightAnimation la = go.GetComponent<LightAnimation>();
		if (la == null) {
			Debug.Log("LightAnimation.Load failure, la == null");
			return index + 7;
		}
		if (la.animLight == null) la.animLight = la.GetComponent<Light>();

		if (index < 0) {
			Debug.Log("LightAnimation.Load failure, index < 0");
			return index + 7;
		}

		if (entries == null) {
			Debug.Log("LightAnimation.Load failure, entries == null");
			return index + 7;
		}

		la.lightOn = Utils.GetBoolFromString(entries[index],"lightOn"); index++;
		la.lerpOn = Utils.GetBoolFromString(entries[index],"lerpOn"); index++;
		la.currentStep = Utils.GetIntFromString(entries[index],"currentStep"); index++;
		la.lerpValue = Utils.GetFloatFromString(entries[index],"lerpValue"); index++; // %
		la.lerpTime = Utils.LoadRelativeTimeDifferential(entries[index],"lerpTime"); index++;
		la.stepTime = Utils.GetFloatFromString(entries[index],"stepTime"); index++; // Not a timer, current time amount
		la.lerpStartTime = Utils.LoadRelativeTimeDifferential(entries[index],"lerpStartTime"); index++;
		la.animLight.enabled = Utils.GetBoolFromString(entries[index],"light.enabled"); index++;
		la.ScaleSEGIEmitter();
		return index;
	}
}
