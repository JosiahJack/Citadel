using UnityEngine;
using System.Collections;

public class ButtonSwitch : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public string target;
	public string argvalue;
	public string message;
	public int messageIndex;
	public float delay = 0f;
	public Material mainSwitchMaterial;
	public Material alternateSwitchMaterial;
	public AudioClip SFX;
	public AudioClip SFXLocked;
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
	public bool animateModel = false;
	private Animator anim;
	public bool locked = false;
	public string lockedMessage;
	public int lockedMessageLingdex = 193;

	void Awake () {
		SFXSource = GetComponent<AudioSource>();
		mRenderer = GetComponent<MeshRenderer>();
		delayFinished = 0; // prevent using targets on awake
		if (animateModel) anim = GetComponent<Animator>();
	}

	void Start () {
		if (string.IsNullOrWhiteSpace(lockedMessage)) {
			if (lockedMessageLingdex < Const.a.stringTable.Length)
				lockedMessage = Const.a.stringTable[lockedMessageLingdex];
		}
	}

	public void Use (UseData ud) {
		//Debug.Log("Using a ButtonSwitch.cs!");
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position);
			return;
		}

		if (LevelManager.a.superoverride)
			locked = false; // SHODAN can go anywhere!  Full security override!

		if (locked) {
			Const.sprint(lockedMessage,ud.owner);
			if (SFXLocked != null && SFXSource != null) SFXSource.PlayOneShot(SFXLocked);
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
					tickFinished = Time.time + tickTime;
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
