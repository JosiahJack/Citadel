using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CyberTimer : MonoBehaviour {
	private float t;
	public Text text;
	private float minutes;
	private float seconds;
	private float timerFinished;

	void Awake() {
		t = 60f * 10f;
		timerFinished = Time.time + 1f;
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
		if (timerFinished < Time.time) {
			t -= 1f;
			minutes = Mathf.Floor(t/60f);
			seconds = t - (minutes*60);
			text.text = (minutes.ToString("00") + ":" + seconds.ToString("00"));
			timerFinished = Time.time + 1f;
		}
    }
}
