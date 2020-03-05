using UnityEngine;
using System.Collections;

public class KeypadKeycode : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	//public DataTab dataTabResetter;
	//public GameObject keypadControl;
	public int keycode; // the access code
	public bool locked = false;
	public string target;
	public string argvalue;
	public string lockedTarget;
	public string successMessage = "";
	public string lockedMessage = "";
	public AudioClip SFX;
	private float disconnectDist;
	private AudioSource SFXSource;
	private bool padInUse = false;
	private GameObject playerCamera;
	private GameObject playerCapsule;
	public bool solved = false;
	public bool useQuestKeycode1 = false;
	public bool useQuestKeycode2 = false;

	void Start () {
		padInUse = false;
		disconnectDist = Const.a.frobDistance;
		SFXSource = GetComponent<AudioSource>();
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			Const.sprint("Blocked by SHODAN level Security.",ud.owner);
			MFDManager.a.BlockedBySecurity(transform.position);
			return;
		}

		if (LevelManager.a.superoverride) {
			// SHODAN can go anywhere!  Full security override!
			locked = false;
		}

		if (locked) {
			Const.sprint(lockedMessage,ud.owner);
			if (lockedTarget != null && lockedTarget != "" && lockedTarget != " ") {
				ud.argvalue = argvalue;
				TargetIO tio = GetComponent<TargetIO>();
				if (tio != null) {
					ud.SetBits(tio);
				} else {
					Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
				}
				Const.a.UseTargets(ud,lockedTarget); //target something because we are locked like a Vox message to say hey we are locked, e.g. "Non emergency life pods disabled."
			}
			return;
		}

		if (useQuestKeycode1) {
			if (Const.a.questData.lev1SecCode != -1) {
				if (Const.a.questData.lev2SecCode != -1) {
					if (Const.a.questData.lev3SecCode != -1) {
						int tempones = Const.a.questData.lev3SecCode;
						int temptens = Const.a.questData.lev2SecCode * 10;
						int temphuns = Const.a.questData.lev1SecCode * 100;
						keycode = temphuns + temptens + tempones; //decode digits into keycode from levels 1, 2, and 3 in order huns, tens, ones
					}
				}
			} else {
				Const.sprint("Blocked by extant CPU nodes on one of levels 1, 2, or 3.",ud.owner);
				return;
			}
		}

		if (useQuestKeycode2) {
			if (Const.a.questData.lev4SecCode != -1) {
				if (Const.a.questData.lev5SecCode != -1) {
					if (Const.a.questData.lev6SecCode != -1) {
						int tempones = Const.a.questData.lev6SecCode;
						int temptens = Const.a.questData.lev5SecCode * 10;
						int temphuns = Const.a.questData.lev4SecCode * 100;
						keycode = temphuns + temptens + tempones; //decode digits into keycode from levels 4, 5, and 6 in order huns, tens, ones
					}
				}
			} else {
				Const.sprint("Blocked by extant CPU nodes on one of levels 4, 5, or 6.",ud.owner);
				return;
			}
		}

		padInUse = true;
		SFXSource.PlayOneShot(SFX);
		//dataTabResetter.Reset();
		//keypadControl.SetActive(true);
		//keypadControl.GetComponent<KeypadKeycodeButtons>().keycode = keycode;
		//keypadControl.GetComponent<KeypadKeycodeButtons>().keypad = this;
		playerCamera = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera;
		playerCapsule = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsule; // Get player capsule of player using this pad
		playerCamera.GetComponent<MouseLookScript>().ForceInventoryMode();
		//MFDManager.a.OpenTab(4,true,MFDManager.TabMSG.Keypad,0,MFDManager.handedness.LeftHand);
		MFDManager.a.SendKeypadKeycodeToDataTab(keycode,transform.position,this,solved);
	}

	public void UseTargets () {
		UseData ud = new UseData();
		ud.owner = playerCamera;
		ud.argvalue = argvalue;
		TargetIO tio = GetComponent<TargetIO>();
		if (tio != null) {
			ud.SetBits(tio);
		} else {
			Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
		}
		Const.a.UseTargets(ud,target);
		if (successMessage != "" && successMessage != " " && successMessage != "  ") Const.sprint(successMessage,ud.owner);
	}

	void Update () {
		if (padInUse) {
			if (Vector3.Distance(playerCapsule.transform.position, gameObject.transform.position) > disconnectDist) {
				padInUse = false;
				MFDManager.a.TurnOffKeypad();
				//keypadControl.SetActive(false);
			}
		}
	}
}
