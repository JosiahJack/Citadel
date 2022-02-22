using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPatch : MonoBehaviour {
	public GameObject playerCamera;
	public GameObject gunCamera;
	public HealthManager hm;
	public PlayerMovement playerMovementScript;
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
	public PuzzleWire wirePuzzle;

	[HideInInspector] public float berserkFinishedTime; // save
	[HideInInspector] public float berserkIncrementFinishedTime; // save
	[HideInInspector] public float detoxFinishedTime; // save
	[HideInInspector] public float geniusFinishedTime; // save
	[HideInInspector] public float mediFinishedTime; // save
	[HideInInspector] public float reflexFinishedTime; // save
	[HideInInspector] public float sightFinishedTime; // save
	[HideInInspector] public float sightSideEffectFinishedTime; // save
	[HideInInspector] public float staminupFinishedTime; // save
	[HideInInspector] public int berserkIncrement; // save
	[HideInInspector] public int PATCH_BERSERK = 1;
	[HideInInspector] public int PATCH_DETOX = 2;
	[HideInInspector] public int PATCH_GENIUS = 4;
	[HideInInspector] public int PATCH_MEDI = 8;
	[HideInInspector] public int PATCH_REFLEX = 16;
	[HideInInspector] public int PATCH_SIGHT = 32;
	[HideInInspector] public int PATCH_STAMINUP = 64;
	private AudioSource SFX;
	private MouseLookScript playerMouseLookScript;
	private PlayerHealth playerHealthScript;
	private UnityStandardAssets.ImageEffects.BerserkEffect berserk;
	private UnityStandardAssets.ImageEffects.BerserkEffect gunCamBerserk;
	[HideInInspector] public int patchActive;  // bitflag carrier for active patches // save

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
		sightLight.enabled = false;
		berserk = playerCamera.GetComponent<UnityStandardAssets.ImageEffects.BerserkEffect>();
		gunCamBerserk = gunCamera.GetComponent<UnityStandardAssets.ImageEffects.BerserkEffect>();
	}

	public void ActivatePatch(int index) {
		bool depleted = false;
		switch (index) {
		case 14:
			// Berserk Patch
			Inventory.a.patchCounts[2]--;
			if (Inventory.a.patchCounts[2] <= 0) { depleted = true; }
			if (!(Const.a.CheckFlags(patchActive, PATCH_BERSERK))) patchActive += PATCH_BERSERK;
			berserkFinishedTime = PauseScript.a.relativeTime + Const.a.berserkTime;
			float berserkIncrementTime = Const.a.berserkTime/5f;
			if (berserkIncrementFinishedTime > PauseScript.a.relativeTime) {
				berserkIncrementFinishedTime += berserkIncrementTime; // berserk effect stacks
			} else {
				berserkIncrementFinishedTime = PauseScript.a.relativeTime + berserkIncrementTime;
			}
			break;
		case 15: 
			// Detox Patch
			Inventory.a.patchCounts[6]--;
			if (Inventory.a.patchCounts[6] <= 0) { depleted = true; }
			DisableAllPatches(); // remove all other effects, even medipatch
			patchActive = PATCH_DETOX; // overwrite all other active patches
			detoxFinishedTime = PauseScript.a.relativeTime + Const.a.detoxTime; // detox doesn't stack, it cancels itself lol
			break;
		case 16: 
			// Genius Patch
			Inventory.a.patchCounts[5]--;
			if (Inventory.a.patchCounts[5] <= 0) { depleted = true; }
			if (!(Const.a.CheckFlags(patchActive, PATCH_GENIUS))) patchActive += PATCH_GENIUS;
			if (geniusFinishedTime > PauseScript.a.relativeTime) {
				geniusFinishedTime += Const.a.geniusTime; // genius effect stacks
			} else {
				geniusFinishedTime = PauseScript.a.relativeTime + Const.a.geniusTime;
			}
			break;
		case 17: 
			// Medi Patch
			if (hm.health >=255) {
				Const.sprint(Const.a.stringTable[304],playerMouseLookScript.player);
				return;
			}
			Inventory.a.patchCounts[3]--;
			if (Inventory.a.patchCounts[3] <= 0) { depleted = true; }
			if (!(Const.a.CheckFlags(patchActive, PATCH_MEDI))) patchActive += PATCH_MEDI;
			playerHealthScript.mediPatchPulseCount = 0;
			if (mediFinishedTime > PauseScript.a.relativeTime) {
				mediFinishedTime += Const.a.mediTime; // medipatch effect stacks
			} else {
				mediFinishedTime = PauseScript.a.relativeTime + Const.a.mediTime;
			}
			break;
		case 18: 
			// Reflex Patch
			Inventory.a.patchCounts[4]--;
			if (Inventory.a.patchCounts[4] <= 0) { depleted = true; }
			Time.timeScale = Const.a.reflexTimeScale;
			if (!(Const.a.CheckFlags(patchActive, PATCH_REFLEX))) patchActive += PATCH_REFLEX;
			if (reflexFinishedTime > PauseScript.a.relativeTime) {
				reflexFinishedTime += Const.a.reflexTime; // reflex effect stacks
			} else {
				reflexFinishedTime = PauseScript.a.relativeTime + Const.a.reflexTime;
			}
			break;
		case 19: 
			// Sight Patch
			Inventory.a.patchCounts[1]--;
			if (Inventory.a.patchCounts[1] <= 0) { depleted = true; }
			sightLight.enabled = true; // enable vision enhancement
			sightSideEffectFinishedTime = -1f;  // reset side effect timer from previous patch
			sightDimming.enabled = false; // deactivate side effect from previous patch
			if (!(Const.a.CheckFlags(patchActive, PATCH_SIGHT))) patchActive += PATCH_SIGHT;
			if (sightFinishedTime > PauseScript.a.relativeTime) {
				sightFinishedTime += Const.a.sightTime; // sight effect stacks
			} else {
				sightFinishedTime = PauseScript.a.relativeTime + Const.a.sightTime;
			}
			break;
		case 20: 
			// Staminup Patch
			Inventory.a.patchCounts[0]--;
			if (Inventory.a.patchCounts[0] <= 0) { depleted = true; }
			playerMovementScript.staminupActive = true;
			if (!(Const.a.CheckFlags(patchActive, PATCH_STAMINUP))) patchActive += PATCH_STAMINUP;
			if (staminupFinishedTime > PauseScript.a.relativeTime) {
				staminupFinishedTime += Const.a.staminupTime; // staminup effect stacks
			} else {
				staminupFinishedTime = PauseScript.a.relativeTime + Const.a.staminupTime;
			}
			break;
		}

		if (depleted) Inventory.a.PatchCycleDown();
		SFX.PlayOneShot(patchUseSFX);
		GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			// ================================== DETOX PATCH =========================
			if (Const.a.CheckFlags(patchActive, PATCH_DETOX)) {
				// ---Disable Patch---
				if (detoxFinishedTime < PauseScript.a.relativeTime) {
					patchActive -= PATCH_DETOX; // Back to full force radiation effects, if present.  All normal.
				} else {
					// ***Patch Effect***
					patchActive = PATCH_DETOX; // Lets health script know to ameliorate the effects of radiation.
				}
			}

			// ================================== MEDI PATCH =========================
			if (Const.a.CheckFlags(patchActive, PATCH_MEDI)) {
				// ---Disable Patch---
				if (mediFinishedTime < PauseScript.a.relativeTime && mediFinishedTime != -1) {
					patchActive -= PATCH_MEDI;
					mediFinishedTime = -1;
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
				if (berserkFinishedTime < PauseScript.a.relativeTime) {
					berserkIncrement = 0;
					patchActive -= PATCH_BERSERK;
					berserk.Reset();
					gunCamBerserk.Reset();
					berserk.enabled = false;
					gunCamBerserk.enabled = false;
				} else {
					// ***Patch Effect***
					berserk.enabled = true;
					gunCamBerserk.enabled = true;
					if (berserkIncrementFinishedTime < PauseScript.a.relativeTime) {
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
						gunCamBerserk.swapTexture = berserk.swapTexture;
						gunCamBerserk.effectStrength = berserk.effectStrength;
						float berserkIncrementTime = Const.a.berserkTime/5f;
						berserkIncrementFinishedTime = PauseScript.a.relativeTime + berserkIncrementTime;
					}
				}
			}

			// ================================== GENIUS PATCH ========================
			if (Const.a.CheckFlags(patchActive, PATCH_GENIUS)) {
				// ---Disable Patch---
				if (geniusFinishedTime < PauseScript.a.relativeTime) {
					playerMouseLookScript.geniusActive = false;
					patchActive -= PATCH_GENIUS;
					wirePuzzle.geniusActive = false;
				} else {
					// ***Patch Effect***
					playerMouseLookScript.geniusActive = true;  // so that LH/RH are swapped for mouse look
					wirePuzzle.geniusActive = true;
				}
			}

			// ================================== SIGHT PATCH =========================
			if (Const.a.CheckFlags(patchActive, PATCH_SIGHT)) {
				// [[[Enable Side Effect]]]
				if (sightFinishedTime < PauseScript.a.relativeTime && sightFinishedTime != -1f) {
					sightFinishedTime = -1f;
					sightSideEffectFinishedTime = PauseScript.a.relativeTime + Const.a.sightSideEffectTime;
					sightLight.enabled = false;
					sightDimming.enabled = true;
				}

				// ---Disable Patch---
				if (sightSideEffectFinishedTime < PauseScript.a.relativeTime && sightSideEffectFinishedTime != -1f) {
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
				if (staminupFinishedTime < PauseScript.a.relativeTime) {
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

	public void DisableAllPatches() {
		berserkFinishedTime = -1f;
		berserkIncrementFinishedTime =  -1f;
		berserkIncrement = 0;
		berserk.Reset();
		berserk.enabled = false;
		gunCamBerserk.Reset();
		gunCamBerserk.enabled = false;
		detoxFinishedTime =  -1f;
		geniusFinishedTime =  -1f;
		playerMouseLookScript.geniusActive = false;
		wirePuzzle.geniusActive = false;
		mediFinishedTime =  -1f;
		reflexFinishedTime =  -1f;
		Time.timeScale = Const.a.defaultTimeScale; // normal time speed
		sightFinishedTime =  -1f;
		sightSideEffectFinishedTime =  -1f;
		sightDimming.enabled = false;
		sightLight.enabled = false;
		staminupFinishedTime =  -1f;
		playerMovementScript.staminupActive = false;
		patchActive = 0;
	}
}
