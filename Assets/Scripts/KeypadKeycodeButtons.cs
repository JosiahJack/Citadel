﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeypadKeycodeButtons : MonoBehaviour {
	public KeycodeDigitImage digit1s;
	public KeycodeDigitImage digit10s;
	public KeycodeDigitImage digit100s;
	public int keycode; // Set externally by KeypadKeycode.cs's Use()
	
	[HideInInspector] public KeypadKeycode keypad; // dido, also set externally
	[HideInInspector] public int currentEntry;
	private int entryOnes;
	private int entryTens;
	private int entryHuns;
	private AudioSource SFXSource;

	void Awake () {
		ResetEntry();
		SFXSource = GetComponent<AudioSource>();
	}

	public void ResetEntry() {
		currentEntry = -1;
		entryOnes = -1;
		entryTens = -1;
		entryHuns = -1;
	}
	
	public void Keypress (int button) {
		MFDManager.a.mouseClickHeldOverGUI = true;
		KeypressAction(button);
	}

	void KeypressAction(int button) { // Wrapper for non-click events.
		if (keypad == null) return;
		if (keypad.solved) return;
		
		Utils.PlayOneShotSavable(SFXSource,39);
		// Digit key pressed 0 thru 9
		if ((button >= 0) && (button <= 9))
			SetDigit(button);

		// Backspace dash '-' button pressed
		if (button == 10) {
			if (entryHuns != -1) {
				entryOnes = entryTens;
				entryTens = entryHuns;
				entryHuns = -1;
				currentEntry = (entryOnes + (entryTens * 10));
				return;
			} else if (entryTens != -1) {
				entryOnes = entryTens;
				entryTens = -1;
				currentEntry = entryOnes;
				return;
			} else if (entryOnes != -1) {
				entryOnes = -1;
				currentEntry = -1;
				return;
			}
		}

		// Clear 'C' key pressed
		if (button == 11) {
			currentEntry = -1;
			entryOnes = -1;
			entryTens = -1;
			entryHuns = -1;
		}
		
		if (currentEntry == keycode) {
			if ((entryHuns != -1)) {
				Utils.PlayOneShotSavable(SFXSource,46);
			}
			keypad.UseTargets();
			keypad.solved = true;
		} else {
			if ((entryHuns != -1)) {
				Utils.PlayOneShotSavable(SFXSource,43);
			}
		}
	}

	void SetDigit(int value) {
		if ((value > 9) || (value < 0)) {
			Const.sprint("BUG: incorrect value sent to keypad controller");
			return;
		}

		if (entryOnes == -1) {
			entryOnes = value;
			currentEntry = entryOnes;
			return;
		} else if (entryTens == -1) {
			entryTens = entryOnes;
			entryOnes = value;
			currentEntry = (entryOnes + (entryTens * 10));
			return;
		} else if (entryHuns == -1) {
			entryHuns = entryTens;
			entryTens = entryOnes;
			entryOnes = value;
			currentEntry = (entryOnes + (entryTens * 10) + (entryHuns * 100));
			return;
		} else {
			// Slide it over
			entryHuns = entryTens;
			entryTens = entryOnes;
			entryOnes = value;
			currentEntry = (entryOnes + (entryTens * 10) + (entryHuns * 100));
		}
	}

	void Update() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive())  return;
		if (keypad == null) return;

		digit1s.digitIndex = entryOnes;
		digit10s.digitIndex = entryTens;
		digit100s.digitIndex = entryHuns;
		if (keypad.solved) return;

		if (GetInput.a.Numpad0()) { KeypressAction(0); }
		if (GetInput.a.Numpad1()) { KeypressAction(1); }
		if (GetInput.a.Numpad2()) { KeypressAction(2); }
		if (GetInput.a.Numpad3()) { KeypressAction(3); }
		if (GetInput.a.Numpad4()) { KeypressAction(4); }
		if (GetInput.a.Numpad5()) { KeypressAction(5); }
		if (GetInput.a.Numpad6()) { KeypressAction(6); }
		if (GetInput.a.Numpad7()) { KeypressAction(7); }
		if (GetInput.a.Numpad8()) { KeypressAction(8); }
		if (GetInput.a.Numpad9()) { KeypressAction(9); }
		if (GetInput.a.NumpadMinus()) { KeypressAction(10); }
		if (GetInput.a.NumpadPeriod()) { KeypressAction(11); }
		if (GetInput.a.Backspace()) { KeypressAction(10); }
	}
}
