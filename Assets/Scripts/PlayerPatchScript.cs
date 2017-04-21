using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPatchScript : MonoBehaviour {
	public GameObject playerCamera;
	[HideInInspector]
	public float berserkFinishedTime;
	[HideInInspector]
	public float berserkIncrementFinishedTime;
	[HideInInspector]
	public float detoxFinishedTime;
	[HideInInspector]
	public float geniusFinishedTime;
	[HideInInspector]
	public float mediFinishedTime;
	[HideInInspector]
	public float reflexFinishedTime;
	[HideInInspector]
	public float sightFinishedTime;
	[HideInInspector]
	public float sightSideEffectFinishedTime;
	[HideInInspector]
	public float staminupFinishedTime;
	public int berserkIncrement;
	private int PATCH_BERSERK = 1;
	private int PATCH_DETOX = 2;
	private int PATCH_GENIUS = 4;
	private int PATCH_MEDI = 8;
	private int PATCH_REFLEX = 16;
	private int PATCH_SIGHT = 32;
	private int PATCH_STAMINUP = 64;
	private AudioSource SFX;
	private MouseLookScript playerMouseLookScript;
	private PlayerHealth playerHealthScript;
	private PlayerMovement playerMovementScript;
	public WeaponFire playerWeapon;
	public Texture2D b1;
	public Texture2D b2;
	public Texture2D b3;
	public Texture2D b4;
	public Texture2D b5;
	public Texture2D b6;
	public Texture2D b7;
	public AudioClip patchUseSFX;
	public Light sightLight;
	public Image sightDimming;
	public PatchInventory playerPI;
	private UnityStandardAssets.ImageEffects.BerserkEffect berserk;
	public int patchActive;  // bitflag carrier for active patches

	// Patches stack so multiple can be used at once
	// For instance, berserk + staminup + medi = 1 + 64 + 8 = 73
	// This is turning on bits in the int patchActive so above would be: 01001001,
	// meaning 3 patches are enabled out of the 7 types (short integer has 8 bits
	// but the 7th bit can be used for sign +/-)

	void Awake () {
		playerHealthScript = gameObject.GetComponent<PlayerHealth>();
		playerMouseLookScript = playerCamera.GetComponent<MouseLookScript>();
		playerMovementScript = gameObject.GetComponent<PlayerMovement>();
		SFX = GetComponent<AudioSource>();
		mediFinishedTime = -1f;
		reflexFinishedTime = -1f;
		sightFinishedTime = -1f;
		berserk = playerCamera.GetComponent<UnityStandardAssets.ImageEffects.BerserkEffect>();
	}

	public void ActivatePatch(int index) {
		switch (index) {
		case 14:
			// Berserk Patch
			playerPI.patchCounts[2]--;
			playerWeapon.berserkActive = true;
			if (!(Const.a.CheckFlags(patchActive, PATCH_BERSERK))) patchActive += PATCH_BERSERK;
			berserkFinishedTime = Time.time + Const.a.berserkTime;
			float berserkIncrementTime = Const.a.berserkTime/5f;
			berserkIncrementFinishedTime = Time.time + berserkIncrementTime;
			break;
		case 15: 
			// Detox Patch
			playerPI.patchCounts[6]--;
			patchActive = PATCH_DETOX; // overwrite all other active patches
			playerHealthScript.detoxPatchActive = true;
			detoxFinishedTime = Time.time + Const.a.detoxTime;
			break;
		case 16: 
			// Genius Patch
			playerPI.patchCounts[5]--;
			if (!(Const.a.CheckFlags(patchActive, PATCH_GENIUS))) patchActive += PATCH_GENIUS;
			geniusFinishedTime = Time.time + Const.a.geniusTime;
			break;
		case 17: 
			// Medi Patch
			playerPI.patchCounts[3]--;
			if (!(Const.a.CheckFlags(patchActive, PATCH_MEDI))) patchActive += PATCH_MEDI;
			playerHealthScript.mediPatchPulseCount = 0;
			mediFinishedTime = Time.time + Const.a.mediTime;
			break;
		case 18: 
			// Reflex Patch
			playerPI.patchCounts[4]--;
			Time.timeScale = Const.a.reflexTimeScale;
			if (!(Const.a.CheckFlags(patchActive, PATCH_REFLEX))) patchActive += PATCH_REFLEX;
			reflexFinishedTime = Time.realtimeSinceStartup + Const.a.reflexTime;
			break;
		case 19: 
			// Sight Patch
			playerPI.patchCounts[1]--;
			sightLight.enabled = true; // enable vision enhancement
			sightSideEffectFinishedTime = -1f;  // reset side effect timer from previous patch
			sightDimming.enabled = false; // deactivate side effect from previous patch
			if (!(Const.a.CheckFlags(patchActive, PATCH_SIGHT))) patchActive += PATCH_SIGHT;
			sightFinishedTime = Time.time + Const.a.sightTime;
			break;
		case 20: 
			// Staminup Patch
			playerPI.patchCounts[0]--;
			playerMovementScript.staminupActive = true;
			if (!(Const.a.CheckFlags(patchActive, PATCH_STAMINUP))) patchActive += PATCH_STAMINUP;
			staminupFinishedTime = Time.time + Const.a.staminupTime;
			break;
		}

		SFX.PlayOneShot(patchUseSFX);
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
	}

	void Update () {
		// ================================== DETOX PATCH =========================
		if (Const.a.CheckFlags(patchActive, PATCH_DETOX)) {
			// ---Disable Patch---
			if (detoxFinishedTime < Time.time) {
				playerHealthScript.detoxPatchActive = false;  // back to full force radiation effects
				patchActive -= PATCH_DETOX;
			} else {
				// ***Patch Effect***
				patchActive = 2; // remove all other effects, even medipatch
				playerHealthScript.detoxPatchActive = true;  // let health script know to ameliorate the effects of radiation
			}
		}

		// ================================== MEDI PATCH =========================
		if (Const.a.CheckFlags(patchActive, PATCH_MEDI)) {
			// ---Disable Patch---
			if (mediFinishedTime < Time.time && mediFinishedTime != -1) {
				patchActive -= PATCH_MEDI;
				playerHealthScript.mediPatchActive = false;
				mediFinishedTime = -1;
			} else {
				// ***Patch Effect***
				playerHealthScript.mediPatchActive = true;
			}
		}

		// ================================== REFLEX PATCH =======================
		if (Const.a.CheckFlags(patchActive, PATCH_REFLEX)) {
			// ---Disable Patch---
			if (reflexFinishedTime < Time.realtimeSinceStartup && reflexFinishedTime != -1) {
				patchActive -= PATCH_REFLEX;
				Time.timeScale = Const.a.defaultTimeScale;
				reflexFinishedTime = -1;
			} else {
				// ***Patch Effect***
				if (Time.timeScale != Const.a.reflexTimeScale) {
					Time.timeScale = Const.a.reflexTimeScale;
				}
			}
		}

		// ================================== BERSERK PATCH =======================
		if (Const.a.CheckFlags(patchActive, PATCH_BERSERK)) {
			// ---Disable Patch---
			if (berserkFinishedTime < Time.time) {
				berserkIncrement = 0;
				patchActive -= PATCH_BERSERK;
				playerWeapon.berserkActive = false;
				berserk.Reset();
				berserk.enabled = false;
			} else {
				// ***Patch Effect***
				playerWeapon.berserkActive = true;
				berserk.enabled = true;
				if (berserkIncrementFinishedTime < Time.time) {
					berserkIncrement++;
					switch (berserkIncrement) {
						case 0: berserk.swapTexture = b1; break;
						case 1: berserk.swapTexture = b2; berserk.effectStrength += 1f; break;
						case 2: berserk.swapTexture = b3; break;
						case 3: berserk.swapTexture = b4; berserk.effectStrength += 1f; berserk.hithreshold += 0.25f; break;
						case 4: berserk.swapTexture = b5; break;
						case 5: berserk.swapTexture = b6; berserk.effectStrength += 1f; berserk.hithreshold += 0.25f; break;
						case 6: berserk.swapTexture = b7; berserk.effectStrength += 1f; berserk.hithreshold += 0.25f; break;
					}
					float berserkIncrementTime = Const.a.berserkTime/5f;
					berserkIncrementFinishedTime = Time.time + berserkIncrementTime;
				}
			}
		}

		// ================================== GENIUS PATCH ========================
		if (Const.a.CheckFlags(patchActive, PATCH_GENIUS)) {
			// ---Disable Patch---
			if (geniusFinishedTime < Time.time) {
				playerMouseLookScript.geniusActive = false;
				patchActive -= PATCH_GENIUS;
			} else {
				// ***Patch Effect***
				playerMouseLookScript.geniusActive = true;  // so that LH/RH are swapped for mouse look
			}
		}

		// ================================== SIGHT PATCH =========================
		if (Const.a.CheckFlags(patchActive, PATCH_SIGHT)) {
			// [[[Enable Side Effect]]]
			if (sightFinishedTime < Time.time && sightFinishedTime != -1f) {
				sightFinishedTime = -1f;
				sightSideEffectFinishedTime = Time.time + Const.a.sightSideEffectTime;
				sightLight.enabled = false;
				sightDimming.enabled = true;
			}

			// ---Disable Patch---
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
			// ---Disable Patch---
			if (staminupFinishedTime < Time.time) {
				playerMovementScript.staminupActive = false;
				playerMovementScript.fatigue = 100f;  // side effect
				patchActive -= PATCH_STAMINUP;
			} else {
				// ***Patch Effect***
				playerMovementScript.fatigue = 0f;
				playerMovementScript.staminupActive = true;
			}
		}
	}
}
