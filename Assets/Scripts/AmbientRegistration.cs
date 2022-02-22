using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientRegistration : MonoBehaviour {
	[HideInInspector] public AudioSource SFX;

    void Start() {
        SFX = GetComponent<AudioSource>();
		PauseScript.a.AddAmbientToRegistry(this);
    }
}
