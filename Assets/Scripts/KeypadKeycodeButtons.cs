using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeypadKeycodeButtons : MonoBehaviour {
	public KeypadKeycode keypad;
	public KeycodeDigitImage digit1s;
	public KeycodeDigitImage digit10s;
	public KeycodeDigitImage digit100s;
	public int keycode;
	public AudioClip SFX;
	public AudioClip SFX_Incorrect;
	public AudioClip SFX_Success;
	private int currentEntry;
	private int entryOnes;
	private int entryTens;
	private int entryHuns;
	private AudioSource SFXSource;
	private bool sfxPlayed;
	private bool done;

	void Awake () {
		currentEntry = -1;
		entryOnes = -1;
		entryTens = -1;
		entryHuns = -1;
		SFXSource = GetComponent<AudioSource>();
		sfxPlayed = false;
		done = false;
	}
	
	public void Keypress (int button) {
		if (done)
			return;
		
		SFXSource.PlayOneShot(SFX);
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
			}
			if (entryTens != -1) {
				entryOnes = entryTens;
				entryTens = -1;
				currentEntry = entryOnes;
				return;
			}
			if (entryOnes != -1) {
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
		}
		if (entryTens == -1) {
			entryTens = entryOnes;
			entryOnes = value;
			currentEntry = (entryOnes + (entryTens * 10));
			return;
		}
		if (entryHuns == -1) {
			entryHuns = entryTens;
			entryTens = entryOnes;
			entryOnes = value;
			currentEntry = (entryOnes + (entryTens * 10) + (entryHuns * 100));
			sfxPlayed = false;
			return;
		}
	}

	void Update () {
		digit1s.digitIndex = entryOnes;
		digit10s.digitIndex = entryTens;
		digit100s.digitIndex = entryHuns;
		if (done)
			return;
		
		if (currentEntry == keycode) {
			if ((entryHuns != -1) && sfxPlayed == false) {
				SFXSource.PlayOneShot(SFX_Success);
				sfxPlayed = true;  // prevent play sfx every frame once 3 digits have been entered
			}
			keypad.UseTargets();
			done = true;
		} else {
			if ((entryHuns != -1) && sfxPlayed == false) {
				SFXSource.PlayOneShot(SFX_Incorrect);
				sfxPlayed = true;  // prevent play sfx every frame once 3 digits have been entered
			}
		}
	}
}
