﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class KeypadElevator : MonoBehaviour {
	public Door linkedDoor;
	public GameObject[] targetDestination; // Set by ElevatorKeypad.cs in Use()
										   // which actually gets it from
										   // ElevatorButton.cs.

	public int securityThreshhold = 100; // If security level is not below this
										 // level, this is unusable.
	public bool[] buttonsEnabled;
	public bool[] buttonsDarkened;
	public string[] buttonText;
	public int currentFloor;
	public bool padInUse; // save
	public bool locked = false; // save
	public string lockedTarget;
	public string argvalue;
	public int lockedMessageIndex = -1;
	
	private static StringBuilder s1 = new StringBuilder();

	void Start () {
		padInUse = false;
		if (linkedDoor == null) {
			Debug.Log("BUG: no linked Door for KeypadElevator at location: "
					  + transform.position.ToString());
		}
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position);
			return;
		}

		if (LevelManager.a.superoverride || Const.a.difficultyMission == 0) {
			// SHODAN can go anywhere!  Full security override!
			locked = false;
		}

		if (locked) {
			// Target something because we are locked like an info_message to say
			// hey we are locked, e.g. vox: "Non emergency life pods disabled."
			Const.sprint(lockedMessageIndex);
			ud.argvalue = argvalue;
			Const.a.UseTargets(gameObject,ud,lockedTarget);
			return;
		}

		padInUse = true;
		Utils.PlayUIOneShotSavable(91);
		MFDManager.a.SendElevatorKeypadToDataTab(this,buttonsEnabled,
												 buttonsDarkened,buttonText,
												 targetDestination,
												 transform.position,linkedDoor,
												 currentFloor);
	}

	public void SendDataBackToPanel() {
		padInUse = false;
	}

	public static string Save(GameObject go) {
		KeypadElevator ke = go.GetComponent<KeypadElevator>();
		if (ke == null) {
			Debug.Log("KeypadElevator missing on savetype of KeypadElevator! "
					  + " GameObject.name: " + go.name);

			return "0|0";
		}

		s1.Clear();
		s1.Append(Utils.BoolToString(ke.padInUse,"KeypadElevator.padInUse"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ke.locked,"locked"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		KeypadElevator ke = go.GetComponent<KeypadElevator>();
		if (ke == null) {
			Debug.Log("KeypadElevator.Load failure, ke == null");
			return index + 2;
		}

		if (index < 0) {
			Debug.Log("KeypadElevator.Load failure, index < 0");
			return index + 2;
		}

		if (entries == null) {
			Debug.Log("KeypadElevator.Load failure, entries == null");
			return index + 2;
		}

		ke.padInUse = Utils.GetBoolFromString(entries[index],
											  "KeypadElevator.padInUse");
		index++;

		ke.locked = Utils.GetBoolFromString(entries[index],"locked"); index++;
		return index;
	}
}
