using UnityEngine;
using System.Collections;

public class PlayerPatchScript : MonoBehaviour {
	public int patchActive;
	public GameObject playerCamera;
	// Patch times in seconds
	public float berserkTime = 15.5f;
	public float detoxTime = 60f;
	public float geniusTime = 35f;
	public float mediTime = 35f;
	public float reflexTime = 155f;
	public float sightTime= 1f;
	public float staminupTime = 60f;
	public float sightSideEffectTime = 1f;
	private MouseLookScript playerMouseLookScript;
	private PlayerHealth playerHealthScript;
	private bool medipatchActive;
	private bool timeSlowed = false;
	private float staminupFinishedTime;
	//private bool sightSideEffect = false;
	private float sightFinishedTime;
	private int PATCH_BERSERK = 1;
	private int PATCH_DETOX = 2;
	private int PATCH_GENIUS = 4;
	private int PATCH_MEDI = 8;
	private int PATCH_REFLEX = 16;
	private int PATCH_SIGHT = 32;
	private int PATCH_STAMINUP = 64;

	// patchActive is a bitflag carrier for active patches
	// Patches stack so multiple can be used at once
	// For instance, berserk + staminup + medi = 1 + 64 + 8 = 73
	// This is turning on bits in the int patchActive so above would be: 01001001,
	// meaning 3 patches are enabled out of the 7 types (short integer has 8 bits
	// but the 7th bit can be used for sign +/-)

	void Awake () {
		playerHealthScript = gameObject.GetComponent<PlayerHealth>();
		playerMouseLookScript = playerCamera.GetComponent<MouseLookScript>();
	}

	void Update () {
		if (Const.a.CheckFlags(patchActive, PATCH_BERSERK)) {
			// TODO change melee damage modifier
			// TODO enable screen color scrambler effect
			// TODO step screen color scrambler effect intensity through ever worsening
		} else {
			// TODO reset melee damage modifier
			// TODO disable screen color scrambler effect
			// TODO reset screen color scrambler effect intensity
		}

		if (Const.a.CheckFlags(patchActive, PATCH_DETOX)) {
			patchActive = 2; // remove all other effects, even medipatch
			playerHealthScript.detoxPatchActive = true;  // let health script know to ameliorate the effects of radiation
		} else {
			playerHealthScript.detoxPatchActive = false;  // back to full force radiation effects
		}

		if (Const.a.CheckFlags(patchActive, PATCH_GENIUS)) {
			playerMouseLookScript.geniusActive = true;  // so that LH/RH are swapped for mouse look
			// TODO set bool in ActivePuzzle script for geniusActive to enable puzzle simplification
		} else {
			playerMouseLookScript.geniusActive = false;  // disable LH/RH swapping
			// TODO disable bool in ActivePuzzle script for geniusActive to return puzzles to normal
		}

		if (Const.a.CheckFlags(patchActive, PATCH_MEDI)) {
			playerHealthScript.mediPatchActive = true;
		} else {
			playerHealthScript.mediPatchActive = false;
		}

		if (Const.a.CheckFlags(patchActive, PATCH_REFLEX)) {
			if (!timeSlowed) {
				Time.timeScale = 0.25f; // you move so fast it is as if time is slowed, so you can react
				timeSlowed = true;
			} else {
				
			}
		} else {
			Time.timeScale = 1.0f; // return to normal time
			timeSlowed = false;
		}

		if (Const.a.CheckFlags(patchActive, PATCH_SIGHT)) {
			// TODO enable visibility enhancement image effect on playerCamera
		} else {
			// TODO disable visibility enhancement image effect on playerCamera
		}

		if (Const.a.CheckFlags(patchActive, PATCH_STAMINUP)) {
			// TODO set fatigue to 0%
		} else {
			// TODO set fatigue to 100%
		}
	}
}
