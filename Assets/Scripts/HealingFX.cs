using UnityEngine;
using System.Collections;

public class HealingFX : MonoBehaviour {
	public float activeTime = 0.1f;
	private float effectFinished;

	void OnEnable () {
		effectFinished = PauseScript.a.relativeTime + activeTime;
	}
		
	void Deactivate () {
		gameObject.SetActive(false);
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (effectFinished < PauseScript.a.relativeTime) {
				Deactivate();
			}
		}
	}
}
