﻿using UnityEngine;
using System.Collections;

public class ButtonSwitch : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public string target;
	public string argvalue;
	public string message;
	public int messageIndex = -1;
	public float delay = 0f;
	public Material mainSwitchMaterial;
	public Material alternateSwitchMaterial;
	public AudioClip SFX;
	public AudioClip SFXLocked;
	public float tickTime = 1.5f;
	public bool active; // save
	public bool blinkWhenActive;
	public bool changeMatOnActive = true;
	[HideInInspector]
	public float delayFinished; // save
	[HideInInspector]
	public float tickFinished; // save
	private GameObject player;
	private AudioSource SFXSource;
	[HideInInspector]
	public bool alternateOn; // save
	private MeshRenderer mRenderer;
	public bool animateModel = false;
	private Animator anim;
	public bool locked = false; // save
	public string lockedMessage;
	public int lockedMessageLingdex = 193;

	void Awake () {
		SFXSource = GetComponent<AudioSource>();
		if (SFXSource != null) SFXSource.playOnAwake = false;
		mRenderer = GetComponent<MeshRenderer>();
		delayFinished = 0; // prevent using targets on awake
		if (animateModel) anim = GetComponent<Animator>();
		if (active) tickFinished = PauseScript.a.relativeTime + tickTime + Random.value;
	}

	public void Use (UseData ud) {
		//Debug.Log("Using a ButtonSwitch.cs!");
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position,ud);
			return;
		}

		if (LevelManager.a.superoverride || Const.a.difficultyMission == 0)
			locked = false; // SHODAN can go anywhere!  Full security override!

		if (locked) {
			Const.sprintByIndexOrOverride (lockedMessageLingdex, lockedMessage,ud.owner);
			if (SFXLocked != null && SFXSource != null) SFXSource.PlayOneShot(SFXLocked);
			return;
		}

		player = ud.owner;  // set playerCamera to owner of the input (always should be the player camera)
		if (SFX != null && SFXSource != null) SFXSource.PlayOneShot(SFX);
		Const.sprintByIndexOrOverride (messageIndex, message,ud.owner);
		if (delay > 0) {
			delayFinished = PauseScript.a.relativeTime + delay;
		} else {
			UseTargets();
		}
	}

	public void Targetted (UseData ud) {
		// prevent runaway circular targetting of self
		if (ud.owner != player) {
			Use(ud); 
		}
	}

	public void ToggleLocked() {
		locked = !locked;
	}

	public void UseTargets () {
		UseData ud = new UseData();
		ud.owner = player;
		ud.argvalue = argvalue;
		TargetIO tio = GetComponent<TargetIO>();
		if (tio != null) {
			ud.SetBits(tio);
		} else {
			Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
		}
		Const.a.UseTargets(ud,target);

		active = !active;
		alternateOn = active;
		if (changeMatOnActive) {
			if (blinkWhenActive) {
				ToggleMaterial ();
				if (active)
					tickFinished = PauseScript.a.relativeTime + tickTime;
			} else {
				ToggleMaterial ();
			}
		}
		if (animateModel) {
			if (active) {
				anim.Play("Activating");
			} else {
				anim.Play("Deactivating");
			}
		}
	}

	void ToggleMaterial() {
		if (alternateOn)
			mRenderer.material = alternateSwitchMaterial;
		else
			mRenderer.material = mainSwitchMaterial;
	}

	void Update() {
		if (PauseScript.a.Paused() || PauseScript.a.mainMenu.activeInHierarchy) {
			return; // don't do any checks or anything else...we're paused!
		}
		if (!gameObject.activeSelf) return;

		if ((delayFinished < PauseScript.a.relativeTime) && delayFinished != 0) {
			delayFinished = 0;
			UseTargets();
		}

		// blink the switch when active
		if (blinkWhenActive) {
			if (active) {
				if (tickFinished < PauseScript.a.relativeTime) {
					if (mRenderer.isVisible) {
						if (alternateOn) {
							if (mRenderer.material != mainSwitchMaterial) mRenderer.material = mainSwitchMaterial;
						} else {
							if (mRenderer.material != alternateSwitchMaterial) mRenderer.material = alternateSwitchMaterial;
						}
					}
					alternateOn = !alternateOn;
					tickFinished = PauseScript.a.relativeTime + tickTime;
				}
			}
		}
	}
}
