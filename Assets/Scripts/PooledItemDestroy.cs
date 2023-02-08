using UnityEngine;
using System.Collections;

public class PooledItemDestroy : MonoBehaviour {
	public float itemLifeTime = 3.00f;
	public bool onlyOnce = false;
	private bool doneYet = false;
	private float timerFinished = 9999999f;

	void OnEnable () {
		timerFinished = PauseScript.a.relativeTime + itemLifeTime;
	}

	void Update() {
		if (onlyOnce && doneYet) return;

		if (timerFinished < PauseScript.a.relativeTime) {
			timerFinished = 9999999f;
			if (onlyOnce) doneYet = true;
			gameObject.SetActive(false);
		}
	}
}
