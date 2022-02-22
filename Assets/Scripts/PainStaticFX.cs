using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PainStaticFX : MonoBehaviour {
	public float lifetime = 0.1f;
	public Sprite painHeavy;
	public Sprite painHeavy2;
	public Sprite painMedium;
	public Sprite painLight;
	private float effectFinished;
	private Image img;

	void Awake () {
		img = GetComponent<Image>();
		img.enabled = false;
	}

	public void Flash(int intensity) {
		effectFinished = PauseScript.a.relativeTime + lifetime;
		img.overrideSprite = painLight;

		switch(intensity) {
			case 0: SetLightImage(); break;
			case 1: img.overrideSprite = painMedium; break;
			case 2: if (Random.value < 0.5f) {
						img.overrideSprite = painHeavy;
					} else {
						img.overrideSprite = painHeavy2;
					}
					break;
		}
		img.enabled = true;
	}

	void SetLightImage() {
		img.overrideSprite = painLight;
	}
	
	// Update is called once per frame
	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (img.enabled == false) return; // only care if flash is on
			if (effectFinished < PauseScript.a.relativeTime) Deactivate();
		}
	}

	public void Deactivate () {
		img.enabled = false;;
	}
}
