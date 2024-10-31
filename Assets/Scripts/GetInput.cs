using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Master input handling functions from configuration
public class GetInput : MonoBehaviour {
	public MobileInputController leftTS;
	public MobileInputController rightTS;
	public UIButtonMask spaceButton;
	public UIButtonMask swimUpButton;
	public UIButtonMask swimDownButton;
	public UIButtonMask lmbButton;
	public UIButtonMask consoleButton;
	public GameObject touchablesContainer;
	public static GetInput a;
	[HideInInspector] public bool isCapsLockOn;
	private bool lastjoy3 = false;
	private bool lastUse = false;
	private Vector2 LTSCenter;
	public float LTSRadius;

	void Awake() {
		a = this;
		isCapsLockOn = false;
		if (Application.platform != RuntimePlatform.Android) {
			touchablesContainer.SetActive(false);
		} else {
			touchablesContainer.SetActive(true);
		}
	}

// 	void Update() {
	//	// Used for testing with a DualShock PlayStation 4 remote aka the DS4 for the PS4.
	//	if (Input.GetKeyDown(KeyCode.JoystickButton0 )) Debug.Log("JoystickButton0 pressed"); // X (A)
	//	if (Input.GetKeyDown(KeyCode.JoystickButton1 )) Debug.Log("JoystickButton1 pressed"); // Circle (B)
	//	if (Input.GetKeyDown(KeyCode.JoystickButton2 )) Debug.Log("JoystickButton2 pressed"); // Square (X)
	//	if (Input.GetKeyDown(KeyCode.JoystickButton3 )) Debug.Log("JoystickButton3 pressed"); // Triangle (Y)
	//	if (Input.GetKeyDown(KeyCode.JoystickButton4 )) Debug.Log("JoystickButton4 pressed"); // L1 (Left Bumper)
	//	if (Input.GetKeyDown(KeyCode.JoystickButton5 )) Debug.Log("JoystickButton5 pressed"); // R1 (Right Bumper)
	//	if (Input.GetKeyDown(KeyCode.JoystickButton6 )) Debug.Log("JoystickButton6 pressed"); // Share (Select)
	//	if (Input.GetKeyDown(KeyCode.JoystickButton7 )) Debug.Log("JoystickButton7 pressed"); // Options (Start)
	//	if (Input.GetKeyDown(KeyCode.JoystickButton8 )) Debug.Log("JoystickButton8 pressed"); // PS (Xbox)
	//	if (Input.GetKeyDown(KeyCode.JoystickButton9 )) Debug.Log("JoystickButton9 pressed"); // LJ Press
	//	if (Input.GetKeyDown(KeyCode.JoystickButton10)) Debug.Log("JoystickButton10 pressed"); // RJ Press
	//	if (Input.GetAxisRaw("JoyAxis1") > 0) Debug.Log("JoyAxis1 positive"); // LJ Right (Left Thumbstick Right)
	//	if (Input.GetAxisRaw("JoyAxis1") < 0) Debug.Log("JoyAxis1 negative"); // LJ Left (Left Thumbstick Left)
	//	if (Input.GetAxisRaw("JoyAxis2") > 0) Debug.Log("JoyAxis2 positive"); // LJ Down (Left Thumbstick Down), Pull toward you
	//	if (Input.GetAxisRaw("JoyAxis2") < 0) Debug.Log("JoyAxis2 negative"); // LJ Up (Left Thumbstick Up), Push away from you
	//	if (Input.GetAxisRaw("JoyAxis3") > 0) Debug.Log("JoyAxis3 positive"); // L2 (Left Trigger)
	//	if (Input.GetAxisRaw("JoyAxis3") < 0) Debug.Log("JoyAxis3 negative"); // L2 fell off haha
	//	if (Input.GetAxisRaw("JoyAxis4") > 0) Debug.Log("JoyAxis4 positive"); // RJ Right (Right Thumbstick Right)
	//	if (Input.GetAxisRaw("JoyAxis4") < 0) Debug.Log("JoyAxis4 negative"); // RJ Left (Right Thumbstick Left)
	//	if (Input.GetAxisRaw("JoyAxis5") > 0) Debug.Log("JoyAxis5 positive"); // RJ Down (Right Thumbstick Down) Pitch Up, Pull towards you
	//	if (Input.GetAxisRaw("JoyAxis5") < 0) Debug.Log("JoyAxis5 negative"); // RJ Up (Right Thumbstick Up) Pitch Down, Push away from you
	//	if (Input.GetAxisRaw("JoyAxis6") > 0) Debug.Log("JoyAxis6 positive"); // R2 (Right Trigger)
	//	if (Input.GetAxisRaw("JoyAxis6") < 0) Debug.Log("JoyAxis6 negative"); // R2 fell off haha
	//	if (Input.GetAxisRaw("JoyAxis7") > 0) Debug.Log("JoyAxis7 positive"); // D-Pad Right
	//	if (Input.GetAxisRaw("JoyAxis7") < 0) Debug.Log("JoyAxis7 negative"); // D-Pad Left
	//	if (Input.GetAxisRaw("JoyAxis8") > 0) Debug.Log("JoyAxis8 positive"); // D-Pad Down
	//	if (Input.GetAxisRaw("JoyAxis8") < 0) Debug.Log("JoyAxis8 negative"); // D-Pad Up
// 	}

	// Generics
	public bool MouseWheelBoundAndRolled(int setCode) {
		if (setCode == 153) return MouseWheelUp();
		if (setCode == 154) return MouseWheelDn();
		return false;
	}

	public bool GetKeyRiseEdgeOrHeld(int codeSettingIndex, bool risingEdge) {
		int setCode = Const.a.InputCodeSettings[codeSettingIndex];
		if (MouseWheelBoundAndRolled(setCode)) return true;
		if (risingEdge) {
			if (Input.GetKeyDown(Const.a.InputValues[setCode])) return true;
		} else {
			if (Input.GetKey(Const.a.InputValues[setCode])) return true;
		}

		return false;
	}

	public bool GetKey(int codeSettingIndex) { // True while held down.
		return GetKeyRiseEdgeOrHeld(codeSettingIndex,false);
	}

	public bool GetKeyDown(int codeSettingIndex) { // True 1st frame down.
		return GetKeyRiseEdgeOrHeld(codeSettingIndex,true);
	}

	// Feature Specific
	public bool Forward() { return GetKey(0); }
	public bool StrafeLeft() { return GetKey(1); }
	public bool Backpedal(){ return GetKey(2); }
	public bool StrafeRight() { return GetKey(3); }
	public bool Jump() {
		if (spaceButton.held) return true; // Touch
		if (Input.GetKey(KeyCode.JoystickButton0)) return true; // Controller
		return GetKey(4); // Mouse or Keyboard
	}

	public bool JumpDown() {
		if (spaceButton.justHeld) return true; // Touch
		if (Input.GetKeyDown(KeyCode.JoystickButton0)) return true; // Cntrller
		return GetKeyDown(4); // Mouse or Keyboard
	}

	public bool Crouch() {
		// Touch
		if (swimDownButton.justHeld) return true;
		if (Input.GetKeyDown(KeyCode.JoystickButton1)) return true;
		return GetKeyDown(5);
	}
	public bool Prone()	{
		// Touch
		if (swimDownButton.justHeld) return true;
		
		// Controller
		if (!lastjoy3
			&& (Input.GetAxisRaw("JoyAxis3") > 0)
			&& (Input.GetAxisRaw("JoyAxis6") > 0)) {

			lastjoy3 = true; return true;
		}

		lastjoy3 = false;
		if (Const.a.InputCodeSettings[6] == 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[6] == 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[6]])) {
			return true;
		}

		return false;
	}

	public bool LeanLeft() {
		if ((Input.GetAxisRaw("JoyAxis3") > 0)
			&& (Input.GetAxisRaw("JoyAxis6") < 0.05f)) {
			
			return true;
		}

		if (Const.a.InputCodeSettings[7] == 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[7] == 154) return MouseWheelDn();
		if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[7]])) {
			return true;
		}
		return false;
	}

	public bool LeanRight() {
		if ((Input.GetAxisRaw("JoyAxis6") > 0)
			&& (Input.GetAxisRaw("JoyAxis3") < 0.05f)) {

			return true;
		}

		if (Const.a.InputCodeSettings[8] == 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[8] == 154) return MouseWheelDn();
		if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[8]])) {
			return true;
		}

		return false;
	}

	public bool Sprint() {
		if (Input.GetKey(KeyCode.JoystickButton9)) return true;
		if (Const.a.InputCodeSettings[9] == 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[9] == 154) return MouseWheelDn();
		if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[9]])) {
			return true;
		}

		return false;
	}

	public bool TurnLeft() {
		if (Const.a.InputCodeSettings[11]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[11]== 154) return MouseWheelDn();
		if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[11]])) {
			return true;
		}

		return false;
	}

	public bool TurnRight() {
		if (Const.a.InputCodeSettings[12]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[12]== 154) return MouseWheelDn();
		if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[12]])) {
			return true;
		}

		return false;
	}

	public bool LookUp() {
		if (Const.a.InputCodeSettings[13]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[13]== 154) return MouseWheelDn();
		if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[13]])) {
			return true;
		}

		return false;
	}

	public bool LookDown() {
		if (Const.a.InputCodeSettings[14]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[14]== 154) return MouseWheelDn();
		if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[14]])) {
			return true;
		}

		return false;
	}

	public bool RecentLog() {
		if (Const.a.InputCodeSettings[15]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[15]== 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[15]])) {
			return true;
		}

		return false;
	}

	public bool Biomonitor() {
		if (Const.a.InputCodeSettings[16]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[16]== 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[16]])) {
			return true;
		}

		return false;
	}

	public bool Sensaround() {
		if (Const.a.InputCodeSettings[17]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[17]== 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[17]])) {
			return true;
		}

		return false;
	}

	public bool Lantern() {
		if (Input.GetKeyDown(KeyCode.JoystickButton10)) return true;
		if (Const.a.InputCodeSettings[18]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[18]== 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[18]])) {
			return true;
		}

		return false;
	}

	public bool Shield() {
		if (Const.a.InputCodeSettings[19]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[19]== 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[19]])) {
			return true;
		}

		return false;
	}

	public bool Infrared() {
		if (Const.a.InputCodeSettings[20]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[20]== 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[20]])) {
			return true;
		}

		return false;
	}

	public bool Email() { if (Const.a.InputCodeSettings[21]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[21]== 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[21]]))return true;
		return false;
	}

	public bool Booster() { if (Const.a.InputCodeSettings[22]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[22]== 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[22]]))return true;
		return false;
	}

	public bool Jumpjets() { if (Const.a.InputCodeSettings[23]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[23]== 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[23]]))return true;
		return false;
	}

	public bool Attack() {
	    if (lmbButton.held) return true;
	    
		if (Const.a.InputCodeSettings[24] == 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[24] == 154) return MouseWheelDn();
		if (Input.GetKey(KeyCode.JoystickButton5)) return true;
		if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[24]])) {
			return true;
		} else {
			if (lmbButton.held) return true;
			return false;
		}
	}

	public bool Use() {
		if (Application.platform == RuntimePlatform.Android) {
			if (Input.touchCount > 0) {
				if (!lastUse) {
					lastUse = true;
					return true;
				}
			} else {
				lastUse = false;
			}
		}

		if (Input.GetKeyDown(KeyCode.JoystickButton4)) return true;
		if (Const.a.InputCodeSettings[25]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[25]== 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[25]])) {
			return true;
		}

		return false;
	}

	public bool Menu()			{ if (Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.JoystickButton8)) return true; if (Const.a.InputCodeSettings[26]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[26]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[26]])) return true; else return false; }
	public bool ToggleMode()	{ if (Input.GetAxisRaw("JoyAxis7") < 0) return true; if (Const.a.InputCodeSettings[27]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[27]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[27]])) return true; else return false; }
	public bool Reload()		{ if (Input.GetKeyDown(KeyCode.JoystickButton2)) return true; if (Const.a.InputCodeSettings[28]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[28]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[28]])) return true; else return false; }
	public bool WeaponCycUp()	{ if (Input.GetKeyDown(KeyCode.JoystickButton3)) return true; if (Const.a.InputCodeSettings[29]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[29]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[29]])) return true; else return false; }
	public bool WeaponCycDown() { if (Const.a.InputCodeSettings[30]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[30]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[30]])) return true; else return false; }
	public bool Grenade()		{ if (Input.GetKeyDown(KeyCode.JoystickButton6)) return true; if (Const.a.InputCodeSettings[31]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[31]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[31]])) return true; else return false; }
	public bool GrenadeCycUp()	{ if (Input.GetAxisRaw("JoyAxis7") > 0) return true; if (Const.a.InputCodeSettings[32]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[32]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[32]])) return true; else return false; }
	public bool GrenadeCycDown(){ if (Const.a.InputCodeSettings[33]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[33]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[33]])) return true; else return false; }
	public bool ChangeAmmoType(){ if (Const.a.InputCodeSettings[34]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[34]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[34]])) return true; else return false; }
	public bool Patch()			{ if (Input.GetAxisRaw("JoyAxis8") < 0) return true; if (Const.a.InputCodeSettings[36]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[36]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[36]])) return true; else return false; }
	public bool PatchCycUp()	{ if (Const.a.InputCodeSettings[37]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[37]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[37]])) return true; else return false; }
	public bool PatchCycDown()	{ if (Const.a.InputCodeSettings[38]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[38]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[38]])) return true; else return false; }
	public bool Map()			{ if (Input.GetAxisRaw("JoyAxis8") > 0) return true; if (Const.a.InputCodeSettings[39]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[39]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[39]])) return true; else return false; }
    public bool SwimUp() {
		if (Const.a.InputCodeSettings[40]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[40]== 154) return MouseWheelDn();
		if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[40]])) {
			return true;
		}

		if (swimUpButton.held) return true;
		return false;
	}

    public bool SwimDn() {
		if (Const.a.InputCodeSettings[41]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[41]== 154) return MouseWheelDn();
		if (Input.GetKey(Const.a.InputValues[Const.a.InputCodeSettings[41]])) {
			return true;
		}

		if (swimDownButton.held) return true;
		return false;
	}

    // public bool SwapAmmoType()	{ if (Const.a.InputCodeSettings[42]== 153) return MouseWheelUp(); if (Const.a.InputCodeSettings[42]== 154) return MouseWheelDn(); if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[42]])) return true; else return false; }
    public bool Console() {
		if (Const.a.InputCodeSettings[43]== 153) return MouseWheelUp();
		if (Const.a.InputCodeSettings[43]== 154) return MouseWheelDn();
		if (Input.GetKeyDown(Const.a.InputValues[Const.a.InputCodeSettings[43]])
			|| Input.GetKeyDown(KeyCode.Caret)) {
			return true;
		}

		if (consoleButton.justHeld) return true;
		return false;
	} // UPDATE should replace the 94 check here with a system to handle Shift+6 binding to set directly?

	public bool Numpad0()		{ if (Input.GetKeyDown(KeyCode.Keypad0)) return true; else return false; }
	public bool Numpad1()		{ if (Input.GetKeyDown(KeyCode.Keypad1)) return true; else return false; }
	public bool Numpad2()		{ if (Input.GetKeyDown(KeyCode.Keypad2)) return true; else return false; }
	public bool Numpad3()		{ if (Input.GetKeyDown(KeyCode.Keypad3)) return true; else return false; }
	public bool Numpad4()		{ if (Input.GetKeyDown(KeyCode.Keypad4)) return true; else return false; }
	public bool Numpad5()		{ if (Input.GetKeyDown(KeyCode.Keypad5)) return true; else return false; }
	public bool Numpad6()		{ if (Input.GetKeyDown(KeyCode.Keypad6)) return true; else return false; }
	public bool Numpad7()		{ if (Input.GetKeyDown(KeyCode.Keypad7)) return true; else return false; }
	public bool Numpad8()		{ if (Input.GetKeyDown(KeyCode.Keypad8)) return true; else return false; }
	public bool Numpad9()		{ if (Input.GetKeyDown(KeyCode.Keypad9)) return true; else return false; }
	public bool NumpadPeriod()	{ if (Input.GetKeyDown(KeyCode.KeypadPeriod)) return true; else return false; }
	public bool NumpadMinus()	{ if (Input.GetKeyDown(KeyCode.KeypadMinus)) return true; else return false; }
	public bool Backspace()		{ if (Input.GetKeyDown(KeyCode.Backspace)) return true; else return false; }
    public bool CapsLockOn()	{ if (Input.GetKeyDown(KeyCode.CapsLock)) isCapsLockOn = !isCapsLockOn; return isCapsLockOn; }
	public bool MouseWheelUp()	{ if (Input.GetAxis("Mouse ScrollWheel") > 0f) return true; else return false; }
	public bool MouseWheelDn()	{ if (Input.GetAxis("Mouse ScrollWheel") < 0f) return true; else return false; }
}
