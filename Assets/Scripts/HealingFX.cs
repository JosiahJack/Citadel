using UnityEngine;
using System.Collections;

public class HealingFX : MonoBehaviour {
	public float activeTime = 0.1f;
	private float effectFinished; // Visual only, Time.time controlled

	void OnEnable () {
		effectFinished = Time.time + activeTime;
	}
		
	void Deactivate () {
		gameObject.SetActive(false);
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (effectFinished < Time.time) {
				Deactivate();
			}
		}
	}
}
