using UnityEngine;
using System.Collections;

public class ButtonSwitch : MonoBehaviour {
	// External references, optional depending on (changeMatOnActive || blinkWhenActive)
	/*[DTValidator.Optional] */public Material mainSwitchMaterial;
	/*[DTValidator.Optional] */public Material alternateSwitchMaterial;

	// Internal references
	private AudioSource SFXSource;
	private MeshRenderer mRenderer; // Optional depending on (changeMatOnActive || blinkWhenActive)
	private Animator anim;
	private GameObject player; // Set on use, no need for initialization check.

	// Individually set values per prefab isntance within the scene
	public int securityThreshhold = 100; // If security level is not <= this level (100), this is unusable until security falls.
	public string target;
	public string argvalue;
	public string message;
	public int messageIndex = -1;
	public float delay = 0f;
	public AudioClip SFX;
	/*[DTValidator.Optional] */public AudioClip SFXLocked;
	public float tickTime = 1.5f;
	public bool blinkWhenActive;
	public bool changeMatOnActive = true;
	public bool animateModel = false;
	public bool locked = false; // save
	public string lockedMessage;
	public int lockedMessageLingdex = 193;
	public bool active; // save

	// Externally modified
	[HideInInspector] public float delayFinished; // save
	[HideInInspector] public float tickFinished; // save
	[HideInInspector] public bool alternateOn; // save

	void Awake () {
		SFXSource = GetComponent<AudioSource>();
		if (SFXSource == null) Debug.Log("BUG: ButtonSwitch missing component for SFXSource");
		else SFXSource.playOnAwake = false;
		mRenderer = GetComponent<MeshRenderer>();
		if (mRenderer == null && (changeMatOnActive || blinkWhenActive)) Debug.Log("BUG: ButtonSwitch missing component for mRenderer");
		if (mainSwitchMaterial == null && (changeMatOnActive || blinkWhenActive)) Debug.Log("BUG: ButtonSwitch missing manually assigned reference for mainSwitchMaterial");
		if (alternateSwitchMaterial == null && (changeMatOnActive || blinkWhenActive)) Debug.Log("BUG: ButtonSwitch missing manually assigned reference for alternateSwitchMaterial");
		delayFinished = 0; // prevent using targets on awake
		if (animateModel) anim = GetComponent<Animator>();
		if (active) tickFinished = PauseScript.a.relativeTime + tickTime + Random.value;
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position,ud);
			return;
		}

		if (LevelManager.a.superoverride || Const.a.difficultyMission == 0) locked = false; // SHODAN can go anywhere!  Full security override!

		if (locked) {
			Const.sprintByIndexOrOverride (lockedMessageLingdex, lockedMessage,ud.owner);
			if (SFXLocked != null && SFXSource != null && gameObject.activeSelf) SFXSource.PlayOneShot(SFXLocked);
			return;
		}

		player = ud.owner;  // set playerCamera to owner of the input (always should be the player camera)
		if (SFX != null && SFXSource != null && gameObject.activeSelf) SFXSource.PlayOneShot(SFX);
		Const.sprintByIndexOrOverride (messageIndex, message,ud.owner);
		if (delay > 0) {
			delayFinished = PauseScript.a.relativeTime + delay;
		} else {
			UseTargets();
		}
	}

	public void Targetted (UseData ud) {
		if (ud.owner != player) Use(ud); // Prevents runaway circular targetting of self with this if statement.
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
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) return; // Don't do any checks or anything else...we're paused!
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