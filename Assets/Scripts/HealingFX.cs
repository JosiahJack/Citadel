using UnityEngine;
using System.Collections;

public class HealingFX : MonoBehaviour {
	public float activeTime = 0.1f;
	private float effectFinished;

	void OnEnable () {
		effectFinished = Time.time + activeTime;
	}
		
	void Deactivate () {
		gameObject.SetActive(false);
	}

	void Update () {
		if (effectFinished < Time.time) {
			Deactivate();
		}
	}
}
