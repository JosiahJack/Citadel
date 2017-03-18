using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Master input handling functions from configuration
public class GetInput : MonoBehaviour {
	public static GetInput a;
	public bool isCapsLockOn;

	void Awake() {
		a = this;
		isCapsLockOn = false;
	}

	public bool Forward() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[0]])) return true; else return false; }
	public bool StrafeLeft() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[1]])) return true; else return false; }
	public bool Backpedal() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[2]])) return true; else return false; }
	public bool StrafeRight() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[3]])) return true; else return false; }
	public bool Jump() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[4]])) return true; else return false; }
	public bool Crouch() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[5]])) return true; else return false; }
	public bool Prone() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[6]])) return true; else return false; }
	public bool LeanLeft() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[7]])) return true; else return false; }
	public bool LeanRight() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[8]])) return true; else return false; }
	public bool Sprint() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[9]])) return true; else return false; }
	public bool ToggleSprint() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[10]])) return true; else return false; }
	public bool TurnLeft() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[11]])) return true; else return false; }
	public bool TurnRight() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[12]])) return true; else return false; }
	public bool LookUp() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[13]])) return true; else return false; }
	public bool LookDown() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[14]])) return true; else return false; }
	public bool RecentLog() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[15]])) return true; else return false; }
	public bool Biomonitor() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[16]])) return true; else return false; }
	public bool Sensaround() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[17]])) return true; else return false; }
	public bool Lantern() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[18]])) return true; else return false; }
	public bool Shield() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[19]])) return true; else return false; }
	public bool Infrared() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[20]])) return true; else return false; }
	public bool Email() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[21]])) return true; else return false; }
	public bool Booster() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[22]])) return true; else return false; }
	public bool Jumpjets() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[23]])) return true; else return false; }
	public bool Attack() { if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[24]])) return true; else return false; }
	public bool Use() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[25]])) return true; else return false; }
	public bool Menu() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[26]])) return true; else return false; }
	public bool ToggleMode() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[27]])) return true; else return false; }
	public bool Reload() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[28]])) return true; else return false; }
	public bool WeaponCycUp() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[29]])) return true; else return false; }
	public bool WeaponCycDown() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[30]])) return true; else return false; }
	public bool Grenade() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[31]])) return true; else return false; }
	public bool GrenadeCycUp() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[32]])) return true; else return false; }
	public bool GrenadeCycDown() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[33]])) return true; else return false; }
	public bool HardwareCycUp() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[34]])) return true; else return false; }
	public bool HardwareCycDown() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[35]])) return true; else return false; }
	public bool Patch() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[36]])) return true; else return false; }
	public bool PatchCycUp() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[37]])) return true; else return false; }
	public bool PatchCycDown() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[38]])) return true; else return false; }
	public bool Map() { if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[39]])) return true; else return false; }
	public bool CapsLockOn() { if (Input.GetKeyDown(KeyCode.CapsLock)) isCapsLockOn = !isCapsLockOn; return isCapsLockOn; }
	public bool Numpad0() { if (Input.GetKeyDown(KeyCode.Keypad0)) return true; else return false; }
	public bool Numpad1() { if (Input.GetKeyDown(KeyCode.Keypad1)) return true; else return false; }
	public bool Numpad2() { if (Input.GetKeyDown(KeyCode.Keypad2)) return true; else return false; }
	public bool Numpad3() { if (Input.GetKeyDown(KeyCode.Keypad3)) return true; else return false; }
	public bool Numpad4() { if (Input.GetKeyDown(KeyCode.Keypad4)) return true; else return false; }
	public bool Numpad5() { if (Input.GetKeyDown(KeyCode.Keypad5)) return true; else return false; }
	public bool Numpad6() { if (Input.GetKeyDown(KeyCode.Keypad6)) return true; else return false; }
	public bool Numpad7() { if (Input.GetKeyDown(KeyCode.Keypad7)) return true; else return false; }
	public bool Numpad8() { if (Input.GetKeyDown(KeyCode.Keypad8)) return true; else return false; }
	public bool Numpad9() { if (Input.GetKeyDown(KeyCode.Keypad9)) return true; else return false; }
	public bool NumpadPeriod() { if (Input.GetKeyDown(KeyCode.KeypadPeriod)) return true; else return false; }
	public bool NumpadMinus() { if (Input.GetKeyDown(KeyCode.KeypadMinus)) return true; else return false; }
}
