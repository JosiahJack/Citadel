﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CyberTimer : MonoBehaviour {
	private float t;
	public Text text;
	private float minutes;
	private float seconds;
	[HideInInspector]
	public float timerFinished; // save

	void Awake() {
		t = 60f * 10f;
		timerFinished = PauseScript.a.relativeTime + 1f;
	}

    public void Reset(int diff) {
		switch(diff) {
			case 0: t = 10f * 60f; break;
			case 1: t = 5f * 60f; break;
			case 2: t = 4f * 60f; break;
			case 3: t = 3f * 60f; break;
		}
    }

    void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (t <= 0) MouseLookScript.a.ExitCyberspace();
			if (timerFinished < PauseScript.a.relativeTime) {
				t -= 1f;
				minutes = Mathf.Floor(t/60f);
				seconds = t - (minutes*60);
				text.text = (minutes.ToString("00") + ":" + seconds.ToString("00"));
				timerFinished = PauseScript.a.relativeTime + 1f;
			}
		}
    }
}
