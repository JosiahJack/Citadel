using UnityEngine;
using UnityEngine.UI;
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
	public float reflexTimeScale = 0.2f;
	public float sightTime= 1f;
	public float staminupTime = 60f;
	public float sightSideEffectTime = 1f;
	public float sightSideEffectFinishedTime;
	private MouseLookScript playerMouseLookScript;
	private PlayerHealth playerHealthScript;
	public PatchInventory playerPI;
	//private bool medipatchActive;
	private bool timeSlowed = false;
	public float staminupFinishedTime;
	//private bool sightSideEffect = false;
	public float sightFinishedTime;
	public float reflexFinishedTime;
	public int berserkIncrement;
	public float mediFinishedTime = -1;
	private int PATCH_BERSERK = 1;
	private int PATCH_DETOX = 2;
	private int PATCH_GENIUS = 4;
	private int PATCH_MEDI = 8;
	private int PATCH_REFLEX = 16;
	private int PATCH_SIGHT = 32;
	private int PATCH_STAMINUP = 64;
	public AudioClip patchUseSFX;
	private AudioSource SFX;
	public Light sightLight;
	public Image sightDimming;

	// patchActive is a bitflag carrier for active patches
	// Patches stack so multiple can be used at once
	// For instance, berserk + staminup + medi = 1 + 64 + 8 = 73
	// This is turning on bits in the int patchActive so above would be: 01001001,
	// meaning 3 patches are enabled out of the 7 types (short integer has 8 bits
	// but the 7th bit can be used for sign +/-)

	void Awake () {
		playerHealthScript = gameObject.GetComponent<PlayerHealth>();
		playerMouseLookScript = playerCamera.GetComponent<MouseLookScript>();
		SFX = GetComponent<AudioSource>();
		mediFinishedTime = -1f;
		reflexFinishedTime = -1f;
		sightFinishedTime = -1f;
	}

	public void ActivatePatch(int index) {
		switch (index) {
		case 14:
			// Berserk Patch
			playerPI.patchCounts[2]--;
			if (!(Const.a.CheckFlags(patchActive, PATCH_BERSERK))) patchActive += PATCH_BERSERK;
			break;
		case 15: 
			// Detox Patch
			playerPI.patchCounts[6]--;
			patchActive = PATCH_DETOX; // overwrite all other active patches
			break;
		case 16: 
			// Genius Patch
			playerPI.patchCounts[5]--;
			if (!(Const.a.CheckFlags(patchActive, PATCH_GENIUS))) patchActive += PATCH_GENIUS;
			break;
		case 17: 
			// Medi Patch
			playerPI.patchCounts[3]--;
			if (!(Const.a.CheckFlags(patchActive, PATCH_MEDI))) patchActive += PATCH_MEDI;
			mediFinishedTime = Time.time + mediTime;
			playerHealthScript.mediPatchPulseCount = 0;
			break;
		case 18: 
			// Reflex Patch
			playerPI.patchCounts[4]--;
			timeSlowed = true;
			reflexFinishedTime = Time.realtimeSinceStartup + reflexTime;
			Time.timeScale = reflexTimeScale;
			if (!(Const.a.CheckFlags(patchActive, PATCH_REFLEX))) patchActive += PATCH_REFLEX;
			break;
		case 19: 
			// Sight Patch
			playerPI.patchCounts[1]--;
			sightLight.enabled = true;
			sightFinishedTime = Time.time + sightTime;
			sightSideEffectFinishedTime = -1f;
			sightDimming.enabled = false;
			if (!(Const.a.CheckFlags(patchActive, PATCH_SIGHT))) patchActive += PATCH_SIGHT;
			break;
		case 20: 
			// Staminup Patch
			playerPI.patchCounts[0]--;
			if (!(Const.a.CheckFlags(patchActive, PATCH_STAMINUP))) patchActive += PATCH_STAMINUP;
			break;
		}

		SFX.PlayOneShot(patchUseSFX);
	}

	void Update () {
		// ================================== MEDI PATCH =========================
		if (Const.a.CheckFlags(patchActive, PATCH_MEDI)) {
			playerHealthScript.mediPatchActive = true;
			if (mediFinishedTime < Time.time && mediFinishedTime != -1) {
				patchActive -= PATCH_MEDI;
				mediFinishedTime = -1;
			}
		} else {
			playerHealthScript.mediPatchActive = false;
			mediFinishedTime = -1;
		}

		// ================================== REFLEX PATCH =======================
		if (Const.a.CheckFlags(patchActive, PATCH_REFLEX)) {
			if (!timeSlowed) {
				Time.timeScale = 0.25f;
				timeSlowed = true;
			}

			if (reflexFinishedTime < Time.realtimeSinceStartup && reflexFinishedTime != -1) {
				patchActive -= PATCH_REFLEX;
				Time.timeScale = 1.0f;
				reflexFinishedTime = -1;
				timeSlowed = false;
			}
		}

		// ================================== BERSERK PATCH =======================
		if (Const.a.CheckFlags(patchActive, PATCH_BERSERK)) {
			// TODO change melee damage modifier
			// TODO enable screen color scrambler effect
			// TODO step screen color scrambler effect intensity through ever worsening using berserkIncrement
		} else {
			// TODO reset melee damage modifier
			// TODO disable screen color scrambler effect
			// TODO reset screen color scrambler effect intensity and reset berserkIncrement
		}

		// ================================== DETOX PATCH =========================
		if (Const.a.CheckFlags(patchActive, PATCH_DETOX)) {
			patchActive = 2; // remove all other effects, even medipatch
			playerHealthScript.detoxPatchActive = true;  // let health script know to ameliorate the effects of radiation
		} else {
			playerHealthScript.detoxPatchActive = false;  // back to full force radiation effects
		}

		// ================================== GENIUS PATCH ========================
		if (Const.a.CheckFlags(patchActive, PATCH_GENIUS)) {
			playerMouseLookScript.geniusActive = true;  // so that LH/RH are swapped for mouse look
			// TODO set bool in ActivePuzzle script for geniusActive to enable puzzle simplification
		} else {
			playerMouseLookScript.geniusActive = false;  // disable LH/RH swapping
			// TODO disable bool in ActivePuzzle script for geniusActive to return puzzles to normal
		}

		// ================================== SIGHT PATCH =========================
		if (Const.a.CheckFlags(patchActive, PATCH_SIGHT)) {
			if (sightFinishedTime < Time.time && sightFinishedTime != -1f) {
				sightFinishedTime = -1f;
				sightSideEffectFinishedTime = Time.time + sightSideEffectTime;
				sightLight.enabled = false;
				sightDimming.enabled = true;
			}

			if (sightSideEffectFinishedTime < Time.time && sightSideEffectFinishedTime != -1f) {
				sightSideEffectFinishedTime = -1f;
				sightFinishedTime = -1f;
				sightDimming.enabled = false;
				sightLight.enabled = false;
				patchActive -= PATCH_SIGHT;
			}
		}

		// ================================== STAMINUP PATCH ======================
		if (Const.a.CheckFlags(patchActive, PATCH_STAMINUP)) {
			// TODO set fatigue to 0%
		} else {
			// TODO set fatigue to 100%
		}
	}
}
