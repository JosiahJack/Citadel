using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientRegistration : MonoBehaviour {
	[HideInInspector] public AudioSource SFX;
    public float normalVolume = 1.0f;

    void Start() {
        SFX = GetComponent<AudioSource>();
        normalVolume = SFX.volume;
		PauseScript.a.AddAmbientToRegistry(this);
    }
}
