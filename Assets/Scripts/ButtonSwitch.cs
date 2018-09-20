using UnityEngine;
using System.Collections;

public class ButtonSwitch : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public GameObject target;
	public GameObject target1;
	public GameObject target2;
	public GameObject target3;
	public string message;
	public float delay = 0f;
	public Material mainSwitchMaterial;
	public Material alternateSwitchMaterial;
	public AudioClip SFX;
	public float tickTime = 1.5f;
	public bool active;
	public bool blinkWhenActive;
	public bool changeMatOnActive = true;
	private float delayFinished;
	private float tickFinished;
	private GameObject player;
	private AudioSource SFXSource;
	private bool alternateOn;
	private MeshRenderer mRenderer;

	void Awake () {
		SFXSource = GetComponent<AudioSource>();
		mRenderer = GetComponent<MeshRenderer>();
		delayFinished = 0; // prevent using targets on awake
	}

	public void Use (UseData ud) {
		if (LevelManager.a.levelSecurity[LevelManager.a.currentLevel] > securityThreshhold) {
			MFDManager.a.BlockedBySecurity();
			return;
		}

		player = ud.owner;  // set playerCamera to owner of the input (always should be the player camera)
		SFXSource.PlayOneShot(SFX);
		if (message != null && message != "") Const.sprint(message,ud.owner);
		if (delay > 0) {
			delayFinished = Time.time + delay;
		} else {
			UseTargets();
		}
	}

	public void UseTargets () {
		UseData ud = new UseData();
		ud.owner = player;

		if (target != null) {
			target.SendMessageUpwards("Targetted", ud);
		}
		if (target1 != null) {
			target1.SendMessageUpwards("Targetted", ud);
		}
		if (target2 != null) {
			target2.SendMessageUpwards("Targetted", ud);
		}
		if (target3 != null) {
			target3.SendMessageUpwards("Targetted", ud);
		}
			
		active = !active;
		alternateOn = active;
		if (changeMatOnActive) {
			if (blinkWhenActive) {
				ToggleMaterial ();
				if (active)
					tickFinished = Time.time + tickTime;
			} else {
				ToggleMaterial ();
			}
		}
	}

	void ToggleMaterial() {
		if (alternateOn)
			mRenderer.material = alternateSwitchMaterial;
		else
			mRenderer.material = mainSwitchMaterial;
	}

	void Update () {
		if ((delayFinished < Time.time) && delayFinished != 0) {
			delayFinished = 0;
			UseTargets();
		}

		// blink the switch when active
		if (blinkWhenActive) {
			if (active) {
				if (tickFinished < Time.time) {
					if (alternateOn) {
						mRenderer.material = mainSwitchMaterial;
					} else {
						mRenderer.material = alternateSwitchMaterial;
					}
					alternateOn = !alternateOn;
					tickFinished = Time.time + tickTime;
				}
			}
		}
	}
}
