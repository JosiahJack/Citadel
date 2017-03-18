using UnityEngine;
using System.Collections;

public class ButtonSwitch : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public GameObject target;
	public GameObject target1;
	public GameObject target2;
	public GameObject target3;
	public float delay = 0f;
	public AudioClip SFX;
	private float delayFinished;
	private GameObject playerCamera;
	private AudioSource SFXSource;

	void Awake () {
		SFXSource = GetComponent<AudioSource>();
	}

	public void Use (GameObject owner) {
		if (LevelManager.a.levelSecurity[LevelManager.a.currentLevel] > securityThreshhold) {
			MFDManager.a.BlockedBySecurity();
			return;
		}

		playerCamera = owner;  // set playerCamera to owner of the input (always should be the player camera)
		SFXSource.PlayOneShot(SFX);
		if (delay > 0) {
			delayFinished = Time.time + delay;
		} else {
			UseTargets();
		}
	}

	public void UseTargets () {
		if (target != null) {
			target.SendMessageUpwards("Targetted", playerCamera);
		}
		if (target1 != null) {
			target.SendMessageUpwards("Targetted", playerCamera);
		}
		if (target2 != null) {
			target.SendMessageUpwards("Targetted", playerCamera);
		}
		if (target3 != null) {
			target.SendMessageUpwards("Targetted", playerCamera);
		}
	}

	void Update () {
		if ((delayFinished < Time.time) && delayFinished != 0) {
			delayFinished = 0;
			UseTargets();
		}
	}
}
