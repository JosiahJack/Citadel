using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using System;

public class FPSCounter : MonoBehaviour {
	private float deltaTime = 0.0f;
	private float msecs;
	private float fps;
	private string textString;
	private Text text;
	private float tickFinished; // Visual only, Time.time controlled
	public float tickSecs = 0.1f;
	private int count;
	private float thousand = 1000f;
	private int zero = 0;
	private int one = 1;
	private string formatToDisplayMS;
	private string formatToDisplayFPS;
	public Text msText;
	public Text fpsText;
	public Text versionText;

	void Start() {
		text = GetComponent<Text> ();
		deltaTime = Time.time;
		count = 0;
		tickFinished = Time.time + tickSecs + UnityEngine.Random.value;
		formatToDisplayMS = "{0:0.0}";
		formatToDisplayFPS = "{0:0.0}";
		versionText.text = Const.a.versionString; // CITADEL PROJECT VERSION
	}

	// Costs 0.06ms to run this!  FYI
	void Update() {
		count++;
		deltaTime += Time.unscaledDeltaTime;
		if (tickFinished < Time.time) {
			msecs = deltaTime/count*thousand;
			deltaTime = zero;
			fps = count * (one / tickSecs);
			count = zero;
			msText.text = string.Format (formatToDisplayMS, msecs);
			fpsText.text = string.Format (formatToDisplayFPS, fps);
			tickFinished = Time.time + tickSecs;
		}
	}
}
