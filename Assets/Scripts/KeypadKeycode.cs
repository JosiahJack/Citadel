using UnityEngine;
using System.Collections;

public class KeypadKeycode : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public int keycode; // the access code
	public bool locked = false; // save
	public string target;
	public string argvalue;
	public string lockedTarget;
	public int successMessageLingdex = -1;
	public string successMessage = "";
	public int lockedMessageLingdex = -1;
	public string lockedMessage = "";
	public AudioClip SFX;
	private float disconnectDist;
	private AudioSource SFXSource;
	[HideInInspector]
	public bool padInUse = false; // save
	private GameObject playerCamera;
	public bool solved = false; // save
	public bool useQuestKeycode1 = false;
	public bool useQuestKeycode2 = false;

	void Start () {
		padInUse = false;
		disconnectDist = Const.a.frobDistance;
		SFXSource = GetComponent<AudioSource>();
		playerCamera = PlayerReferenceManager.a.playerCapsuleMainCamera;
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) { MFDManager.a.BlockedBySecurity(transform.position,ud); return; }

		if (LevelManager.a.superoverride || Const.a.difficultyMission == 0) locked = false; // SHODAN can go anywhere!  Full security override!
		if (locked) {
			Const.sprintByIndexOrOverride (lockedMessageLingdex, lockedMessage,ud.owner);
			
			if (!string.IsNullOrWhiteSpace(lockedTarget)) {
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
				Const.sprint(Const.a.stringTable[289],ud.owner);
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
				Const.sprint(Const.a.stringTable[290],ud.owner);
				return;
			}
		}

		padInUse = true;
		SFXSource.PlayOneShot(SFX);
		MouseLookScript.a.ForceInventoryMode();
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
		Const.sprintByIndexOrOverride (successMessageLingdex, successMessage,ud.owner);
	}
}
