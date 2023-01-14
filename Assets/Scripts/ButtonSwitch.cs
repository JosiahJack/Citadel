using UnityEngine;
using System.Collections;

public class ButtonSwitch : MonoBehaviour {
	// Individually set values per prefab isntance within the scene
	public int securityThreshhold = 100;  // save, If security level is not <= this level (100), this is unusable until security falls.
	public string target; // save
	public string argvalue; // save
	public string message; // save
	public int messageIndex = -1; // save
	public float delay = 0f; // save
	public AudioClip SFX;
	/*[DTValidator.Optional] */public AudioClip SFXLocked;
	public bool blinkWhenActive; // save
	public bool changeMatOnActive = true; // save
	public bool animateModel = false; // save
	public bool locked = false; // save
	public string lockedMessage; // save
	public int lockedMessageLingdex = 193; // save
	public bool active; // save

	// External references, optional depending on (changeMatOnActive || blinkWhenActive)
	/*[DTValidator.Optional] */public Material mainSwitchMaterial;
	/*[DTValidator.Optional] */public Material alternateSwitchMaterial;

	// Internal references
	private AudioSource SFXSource;
	private MeshRenderer mRenderer; // Optional depending on (changeMatOnActive || blinkWhenActive)
	private Animator anim;
	private GameObject player; // Set on use, no need for initialization check.
	private float tickTime = 1.5f;

	// Externally modified
	[HideInInspector] public float delayFinished; // save
	[HideInInspector] public float tickFinished; // save
	[HideInInspector] public bool alternateOn; // save

	void Awake () {
		SFXSource = GetComponent<AudioSource>();
		if (SFXSource == null) Debug.Log("BUG: ButtonSwitch missing component for SFXSource");
		else SFXSource.playOnAwake = false;
		mRenderer = GetComponent<MeshRenderer>();
		delayFinished = 0; // prevent using targets on awake
		if (animateModel) anim = GetComponent<Animator>();
		if (active) tickFinished = PauseScript.a.relativeTime + 1.5f + Random.value;
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position,ud);
			return;
		}

		if (LevelManager.a.superoverride || Const.a.difficultyMission == 0) locked = false; // SHODAN can go anywhere!  Full security override!

		if (locked) {
			Const.sprintByIndexOrOverride (lockedMessageLingdex, lockedMessage,ud.owner);
			Utils.PlayOneShotSavable(SFXSource,SFXLocked);
			return;
		}

		player = ud.owner;  // set playerCamera to owner of the input (always should be the player camera)
		Utils.PlayOneShotSavable(SFXSource,SFX);
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

	public static string Save(GameObject go) {
		ButtonSwitch bs = go.GetComponent<ButtonSwitch>();
		if (bs == null) { // bs?  null??  that's bs
			Debug.Log("ButtonSwitch missing on savetype of ButtonSwitch!  GameObject.name: " + go.name);
			return Utils.DTypeWordToSaveString("usssufbbbbsubbff");
		}

		string line = System.String.Empty;
		line = Utils.UintToString(bs.securityThreshhold);
		line += Utils.splitChar + bs.target;
		line += Utils.splitChar + bs.argvalue;
		line += Utils.splitChar + bs.message;
		line += Utils.splitChar + Utils.UintToString(bs.messageIndex);
		line += Utils.splitChar + Utils.FloatToString(bs.delay);
		line += Utils.splitChar + Utils.BoolToString(bs.blinkWhenActive);
		line += Utils.splitChar + Utils.BoolToString(bs.changeMatOnActive);
		line += Utils.splitChar + Utils.BoolToString(bs.animateModel);
		line += Utils.splitChar + Utils.BoolToString(bs.locked); // bool - is this switch locked
		line += Utils.splitChar + bs.lockedMessage;
		line += Utils.splitChar + Utils.UintToString(bs.lockedMessageLingdex);
		line += Utils.splitChar + Utils.BoolToString(bs.active); // bool - is the switch flashing?
		line += Utils.splitChar + Utils.BoolToString(bs.alternateOn); // bool - is the flashing material on?
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(bs.delayFinished); // float - time before firing targets
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(bs.tickFinished); // float - time before firing targets
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		ButtonSwitch bs = go.GetComponent<ButtonSwitch>(); // what a load of bs
		if (bs == null) {
			Debug.Log("ButtonSwitch.Load failure, bs == null");
			return index + 16;
		}

		if (index < 0) {
			Debug.Log("ButtonSwitch.Load failure, index < 0");
			return index + 16;
		}

		if (entries == null) {
			Debug.Log("ButtonSwitch.Load failure, entries == null");
			return index + 16;
		}

		bs.securityThreshhold = Utils.GetIntFromString(entries[index]); index++;
		bs.target = entries[index]; index++;
		bs.argvalue = entries[index]; index++;
		bs.message = entries[index]; index++;
		bs.messageIndex = Utils.GetIntFromString(entries[index]); index++;
		bs.delay = Utils.GetFloatFromString(entries[index]); index++;
		bs.blinkWhenActive = Utils.GetBoolFromString(entries[index]); index++;
		bs.changeMatOnActive = Utils.GetBoolFromString(entries[index]); index++;
		bs.animateModel = Utils.GetBoolFromString(entries[index]); index++;
		bs.locked = Utils.GetBoolFromString(entries[index]); index++; // bool - is this switch locked
		bs.lockedMessage = entries[index]; index++;
		bs.lockedMessageLingdex = Utils.GetIntFromString(entries[index]); index++;
		bs.active = Utils.GetBoolFromString(entries[index]); index++; // bool - is the switch flashing?
		bs.alternateOn = Utils.GetBoolFromString(entries[index]); index++; // bool - is the flashing material on?
		bs.delayFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before firing targets
		bs.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before thinking
		return index;
	}
}