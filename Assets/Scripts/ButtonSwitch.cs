using UnityEngine;
using System.Collections;
using System.Text;

public class ButtonSwitch : MonoBehaviour {
	// Individually set values per prefab isntance within the scene
	public int securityThreshhold = 100;  // save, If security level is 
	                                      // not <= this level (100), this is
	                                      // unusable until security falls.
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

	// External references, optional depending on (changeMatOnActive
	// || blinkWhenActive)
	/*[DTValidator.Optional] */public Material mainSwitchMaterial;
	/*[DTValidator.Optional] */public Material alternateSwitchMaterial;

	// Internal references
	private AudioSource SFXSource;
	private MeshRenderer mRenderer; // Optional depending on (changeMatOnActive
	                                // || blinkWhenActive)
	private Animator anim;
	private GameObject player; // Set on use, no need for initialization check.
	private float tickTime = 1.5f;
	private bool awakeInitialized = false;

	// Externally modified
	[HideInInspector] public float delayFinished; // save
	[HideInInspector] public float tickFinished; // save
	[HideInInspector] public bool alternateOn; // save

	public void Awake () {
		if (awakeInitialized) return;

		SFXSource = GetComponent<AudioSource>();
		if (SFXSource == null) {
		    Debug.Log("BUG: ButtonSwitch missing component for SFXSource");
		} else SFXSource.playOnAwake = false;
		
		mRenderer = GetComponent<MeshRenderer>();
		delayFinished = 0; // prevent using targets on awake
		if (animateModel) anim = GetComponent<Animator>();
		if (active) {
		    tickFinished = PauseScript.a.relativeTime + 1.5f + Random.value;
		}
		
		awakeInitialized = true;
	}

	public void Use (UseData ud) {
	    if (LevelManager.a.superoverride || Const.a.difficultyMission == 0) {
	        locked = false; // SHODAN can go anywhere!  Full security override!
	    } else if (LevelManager.a.GetCurrentLevelSecurity()
	               > securityThreshhold) {
	                   
			MFDManager.a.BlockedBySecurity(transform.position,ud);
			return;
		}

		if (locked) {
			Const.sprintByIndexOrOverride(lockedMessageLingdex,
			                              lockedMessage,ud.owner);
			                              
			Utils.PlayOneShotSavable(SFXSource,SFXLocked);
			return;
		}

        // Set playerCamera to owner of the input (always should be the camera)
		player = ud.owner;
		Utils.PlayOneShotSavable(SFXSource,SFX);
		Const.sprintByIndexOrOverride (messageIndex, message,ud.owner);
		if (delay > 0) {
			delayFinished = PauseScript.a.relativeTime + delay;
		} else {
			UseTargets();
		}
	}

	public void Targetted (UseData ud) {
		Use(ud);
	}

	public void ToggleLocked() {
		string was = locked.ToString();
		locked = !locked;
		Debug.Log("ButtonSwitch was " + was.ToString() + ", now "
		          + locked.ToString());
	}

	public void UseTargets () {
		UseData ud = new UseData();
		ud.owner = player;
		ud.argvalue = argvalue;
		TargetIO tio = GetComponent<TargetIO>();
		if (tio != null) {
			ud.SetBits(tio);
		} else {
			Debug.Log("BUG: no TargetIO.cs found on an object with a "
			          + "ButtonSwitch.cs script!  Trying to call UseTargets "
			          + "without parameters!");
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

	public void SetMaterialToAlternate() {
		if (!blinkWhenActive) return;

		if (mRenderer.material != alternateSwitchMaterial) {
		    mRenderer.material = alternateSwitchMaterial;
		}
	}

	public void SetMaterialToNormal() {
		if (!blinkWhenActive) return;

		if (mRenderer.material != mainSwitchMaterial) {
		    mRenderer.material = mainSwitchMaterial;
		}
	}

	void Update() {
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) return;

		if ((delayFinished < PauseScript.a.relativeTime)
		    && delayFinished != 0) {

			delayFinished = 0;
			UseTargets();
		}

		// blink the switch when active
		if (blinkWhenActive) {
			if (active) {
				if (tickFinished < PauseScript.a.relativeTime) {
					if (mRenderer.isVisible) {
						if (alternateOn) SetMaterialToAlternate();
						else SetMaterialToNormal();
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
			Debug.Log("ButtonSwitch missing on savetype of ButtonSwitch!  "
			          + "GameObject.name: " + go.name);
			return Utils.DTypeWordToSaveString("usssufbbbbsubbff");
		}

		string line = System.String.Empty;
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.UintToString(bs.securityThreshhold,
		                             "securityThreshhold"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.SaveString(bs.target,"target"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.SaveString(bs.argvalue,"argvalue"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.SaveString(bs.message,"message"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.UintToString(bs.messageIndex,"messageIndex"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.FloatToString(bs.delay,"delay"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.BoolToString(bs.blinkWhenActive,"blinkWhenActive"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.BoolToString(bs.changeMatOnActive,"changeMatOnActive"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.BoolToString(bs.animateModel,"animateModel"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.BoolToString(bs.locked,"locked"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.SaveString(bs.lockedMessage,"lockedMessage"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.UintToString(bs.lockedMessageLingdex,
		                             "lockedMessageLingdex"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.BoolToString(bs.active,"active"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.BoolToString(bs.alternateOn,"alternateOn"));
		s1.Append(Utils.splitChar);
		
		s1.Append(Utils.SaveRelativeTimeDifferential(bs.delayFinished,
		                                             "delayFinished"));
	    s1.Append(Utils.splitChar);
	    
		s1.Append(Utils.SaveRelativeTimeDifferential(bs.tickFinished,
		                                             "tickFinished"));
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

		bs.securityThreshhold = Utils.GetIntFromString(entries[index],
		                                               "securityThreshhold");
		index++;
		
		bs.target = Utils.LoadString(entries[index],"target");
		index++;
		
		bs.argvalue = Utils.LoadString(entries[index],"argvalue");
		index++;
		
		bs.message = Utils.LoadString(entries[index],"message");
		index++;
		
		bs.messageIndex = Utils.GetIntFromString(entries[index],"messageIndex");
		index++;
		
		bs.delay = Utils.GetFloatFromString(entries[index],"delay");
		index++;
		
		bs.blinkWhenActive = Utils.GetBoolFromString(entries[index],"blinkWhenActive");
		index++;
		
		bs.changeMatOnActive = Utils.GetBoolFromString(entries[index],"changeMatOnActive");
		index++;
		
		bs.animateModel = Utils.GetBoolFromString(entries[index],"animateModel");
		index++;
		
		bs.locked = Utils.GetBoolFromString(entries[index],"locked");
		index++;
		
		bs.lockedMessage = Utils.LoadString(entries[index],"lockedMessage");
		index++;
		
		bs.lockedMessageLingdex = Utils.GetIntFromString(entries[index],
		                                               "lockedMessageLingdex");
		index++;
		
		bs.active = Utils.GetBoolFromString(entries[index]),"active");
		index++;
		
		bs.alternateOn = Utils.GetBoolFromString(entries[index],"alternateOn");
		index++;
		
		bs.delayFinished = Utils.LoadRelativeTimeDifferential(entries[index],
		                                                      "delayFinished");
		index++;
		
		bs.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index],
		                                                     "tickFinished");
		index++;
		
		if (bs.active) {
		    if ((bs.tickFinished - PauseScript.a.relativeTime) > 1.5f) {
			    bs.tickFinished = PauseScript.a.relativeTime + 1.5f;
		    }
		} else {
			if (bs.blinkWhenActive) {
				bs.Awake();
				bs.SetMaterialToNormal();
			}
		}
		return index;
	}
}