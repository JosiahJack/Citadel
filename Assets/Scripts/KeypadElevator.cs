using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeypadElevator : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	/*[DTValidator.Optional] */public GameObject[] targetDestination; // set by ElevatorKeypad.cs in Use(), which actually gets it from ElevatorButton.cs
	public Door linkedDoor;
	public bool[] buttonsEnabled;
	public bool[] buttonsDarkened;
	public string[] buttonText;
	public int currentFloor;
	public AudioClip SFX;
	private AudioSource SFXSource;
	public bool padInUse; // save
	public bool locked = false; // save
	public string lockedTarget;
	public string argvalue;
	public string lockedMessage;

	void Start () {
		padInUse = false;
		SFXSource = GetComponent<AudioSource>();
		if (linkedDoor == null) Debug.Log("BUG: no linked Door for KeypadElevator at location: " + transform.position.ToString());
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position,ud);
			return;
		}

		if (LevelManager.a.superoverride || Const.a.difficultyMission == 0) {
			// SHODAN can go anywhere!  Full security override!
			locked = false;
		}

		if (locked) {
			Const.sprint(lockedMessage,ud.owner);
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

		padInUse = true;
		Utils.PlayOneShotSavable(SFXSource,SFX);
		MFDManager.a.SendElevatorKeypadToDataTab(this,buttonsEnabled,buttonsDarkened,buttonText,targetDestination,transform.position,linkedDoor,currentFloor);
	}

	public void SendDataBackToPanel() {
		padInUse = false;
	}

	public static string Save(GameObject go) {
		KeypadElevator ke = go.GetComponent<KeypadElevator>();
		if (ke == null) {
			Debug.Log("KeypadElevator missing on savetype of KeypadElevator!  GameObject.name: " + go.name);
			return "0|0";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(ke.padInUse); // bool - is the pad being used by a player
		line += Utils.splitChar + Utils.BoolToString(ke.locked); // bool - locked?
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		KeypadElevator ke = go.GetComponent<KeypadElevator>();
		if (ke == null || index < 0 || entries == null) return index + 2;

		ke.padInUse = Utils.GetBoolFromString(entries[index]); index++; // bool - is the pad being used by a player
		ke.locked = Utils.GetBoolFromString(entries[index]); index++; // bool - locked?
		return index;
	}
}
