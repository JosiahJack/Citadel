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
	private float tickFinished;
	public float tickSecs = 0.1f;
	private int count;

	void Start() {
		text = GetComponent<Text> ();
		deltaTime = Time.time;
		count = 0;
		tickFinished = Time.time + tickSecs;
	}

	void Update() {
		//deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		count++;
		deltaTime += Time.unscaledDeltaTime;
		if (tickFinished < Time.time) {
			//msecs = (Time.time - timeStart);
			msecs = deltaTime/count*1000f;
			//msecsGPU = deltaTime/countGUI*1000f;
			deltaTime = 0;
			fps = count * (1 / tickSecs);
			count = 0;
			//msecs = deltaTime * 1000.0f;
			//fps = 1.0f / deltaTime;
			textString = string.Format ("{0:0.0} ms ({1:0.0} fps)", msecs, fps);
			text.text = textString;
			tickFinished = Time.time + tickSecs;
		}
	}
}
