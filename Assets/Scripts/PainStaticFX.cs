using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainStaticFX : MonoBehaviour {
	public float lifetime = 0.1f;
	private float effectFinished;

	void OnEnable () {
		effectFinished = Time.time + lifetime;
	}
	
	// Update is called once per frame
	void Update () {
		if (effectFinished < Time.time) Deactivate();
	}

	void Deactivate () {
		gameObject.SetActive(false);
	}
}
