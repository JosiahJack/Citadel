using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPatch : MonoBehaviour {
	public GameObject playerCamera;
	public HealthManager hm;
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
	public BerserkEffect berserk;
	public BerserkEffect sensaroundCamCenterBerserk;
	public BerserkEffect sensaroundCamLeftBerserk;
	public BerserkEffect sensaroundCamRightBerserk;

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
	[HideInInspector] public int patchActive;  // bitflag carrier for active patches // save

	public static PlayerPatch a;

	// Patches stack so multiple can be used at once
	// For instance, berserk + staminup + medi = 1 + 64 + 8 = 73
	// This is turning on bits in the int patchActive so above would be: 01001001,
	// meaning 3 patches are enabled out of the 7 types (short integer has 8 bits
	// but the 7th bit can be used for sign +/-)

	void Awake () {
		a = this;
		a.SFX = GetComponent<AudioSource>();
		a.mediFinishedTime = -1f;
		a.reflexFinishedTime = -1f;
		a.sightFinishedTime = -1f;
		a.sightLight.enabled = false;
		a.BerserkDisable();
	}

	public void ActivatePatch(int index) {
		bool depleted = false;
		switch (index) {
		case 14:
			// Berserk Patch
			Inventory.a.patchCounts[2]--;
			if (Inventory.a.patchCounts[2] <= 0) { depleted = true; }
			if (!(Utils.CheckFlags(patchActive, PATCH_BERSERK))) patchActive += PATCH_BERSERK;
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
			if (!(Utils.CheckFlags(patchActive, PATCH_GENIUS))) patchActive += PATCH_GENIUS;
			if (geniusFinishedTime > PauseScript.a.relativeTime) {
				geniusFinishedTime += Const.a.geniusTime; // genius effect stacks
			} else {
				geniusFinishedTime = PauseScript.a.relativeTime + Const.a.geniusTime;
			}
			break;
		case 17:
			// Medi Patch
			if (hm.health >=255) {
				Const.sprint(Const.a.stringTable[304],MouseLookScript.a.player);
				return;
			}
			Inventory.a.patchCounts[3]--;
			if (Inventory.a.patchCounts[3] <= 0) { depleted = true; }
			if (!(Utils.CheckFlags(patchActive, PATCH_MEDI))) patchActive += PATCH_MEDI;
			PlayerHealth.a.mediPatchPulseCount = 0;
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
			if (!(Utils.CheckFlags(patchActive, PATCH_REFLEX))) patchActive += PATCH_REFLEX;
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
			if (!(Utils.CheckFlags(patchActive, PATCH_SIGHT))) patchActive += PATCH_SIGHT;
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
			PlayerMovement.a.staminupActive = true;
			if (!(Utils.CheckFlags(patchActive, PATCH_STAMINUP))) patchActive += PATCH_STAMINUP;
			if (staminupFinishedTime > PauseScript.a.relativeTime) {
				staminupFinishedTime += Const.a.staminupTime; // staminup effect stacks
			} else {
				staminupFinishedTime = PauseScript.a.relativeTime + Const.a.staminupTime;
			}
			break;
		}

		if (depleted) {
			Inventory.a.PatchCycleDown(false);
			Const.sprint((Const.a.stringTable[590] + Const.a.useableItemsNameText[index] + Const.a.stringTable[589]),MouseLookScript.a.player);
		} else {
			Const.sprint((Const.a.useableItemsNameText[index] + Const.a.stringTable[589]),MouseLookScript.a.player);
		}
		Utils.PlayOneShotSavable(SFX,patchUseSFX);
		GUIState.a.PtrHandler(false,false,ButtonType.None,null);
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			// ================================== DETOX PATCH =========================
			if (Utils.CheckFlags(patchActive, PATCH_DETOX)) {
				// ---Disable Patch---
				if (detoxFinishedTime < PauseScript.a.relativeTime) {
					patchActive -= PATCH_DETOX; // Back to full force radiation effects, if present.  All normal.
				} else {
					// ***Patch Effect***
					patchActive = PATCH_DETOX; // Lets health script know to ameliorate the effects of radiation.
				}
			}

			// ================================== MEDI PATCH =========================
			if (Utils.CheckFlags(patchActive, PATCH_MEDI)) {
				// ---Disable Patch---
				if (mediFinishedTime < PauseScript.a.relativeTime && mediFinishedTime != -1) {
					patchActive -= PATCH_MEDI;
					mediFinishedTime = -1;
				}
			}

			// ================================== REFLEX PATCH =======================
			if (Utils.CheckFlags(patchActive, PATCH_REFLEX)) {
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
			if (Utils.CheckFlags(patchActive, PATCH_BERSERK)) {
				// ---Disable Patch---
				if (berserkFinishedTime < PauseScript.a.relativeTime) {
					berserkIncrement = 0;
					patchActive -= PATCH_BERSERK;
					BerserkDisable();
				} else {
					// ***Patch Effect***
					BerserkEnable();
					if (berserkIncrementFinishedTime < PauseScript.a.relativeTime) {
						berserkIncrement++;
						switch (berserkIncrement) {
							case 0: berserk.swapTexture = b1; break;
							case 1: berserk.swapTexture = b2; berserk.IncrementStrength(); break;
							case 2: berserk.swapTexture = b3; break;
							case 3: berserk.swapTexture = b4; berserk.IncrementStats(); break;
							case 4: berserk.swapTexture = b5; break;
							case 5: berserk.swapTexture = b6; berserk.IncrementStats(); break;
							case 6: berserk.swapTexture = b7; berserk.IncrementStats(); break;
						}
						//gunCamBerserk.swapTexture = berserk.swapTexture;
						//gunCamBerserk.effectStrength = berserk.effectStrength;
						float berserkIncrementTime = Const.a.berserkTime/5f;
						berserkIncrementFinishedTime = PauseScript.a.relativeTime + berserkIncrementTime;
					}
				}
			}

			// ================================== GENIUS PATCH ========================
			if (Utils.CheckFlags(patchActive, PATCH_GENIUS)) {
				// ---Disable Patch---
				if (geniusFinishedTime < PauseScript.a.relativeTime) {
					MouseLookScript.a.geniusActive = false;
					patchActive -= PATCH_GENIUS;
					wirePuzzle.geniusActive = false;
				} else {
					// ***Patch Effect***
					MouseLookScript.a.geniusActive = true;  // so that LH/RH are swapped for mouse look
					wirePuzzle.geniusActive = true;
				}
			}

			// ================================== SIGHT PATCH =========================
			if (Utils.CheckFlags(patchActive, PATCH_SIGHT)) {
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
			if (Utils.CheckFlags(patchActive, PATCH_STAMINUP)) {
				// ---Disable Patch---
				if (staminupFinishedTime < PauseScript.a.relativeTime) {
					PlayerMovement.a.staminupActive = false;
					PlayerMovement.a.fatigue = 100f;  // side effect
					patchActive -= PATCH_STAMINUP;
				} else {
					// ***Patch Effect***
					PlayerMovement.a.fatigue = 0f;
					PlayerMovement.a.staminupActive = true;
				}
			}
		}
	}

	void BerserkEnable() {
		berserk.enabled = true;
		sensaroundCamCenterBerserk.enabled = true;
		sensaroundCamLeftBerserk.enabled = true;
		sensaroundCamRightBerserk.enabled = true;
	}

	void BerserkDisable() {
		berserk.Reset();
		berserk.enabled = false;
		sensaroundCamCenterBerserk.Reset();
		sensaroundCamCenterBerserk.enabled = false;
		sensaroundCamLeftBerserk.Reset();
		sensaroundCamLeftBerserk.enabled = false;
		sensaroundCamRightBerserk.Reset();
		sensaroundCamRightBerserk.enabled = false;
	}

	public void DisableAllPatches() {
		berserkFinishedTime = -1f;
		berserkIncrementFinishedTime =  -1f;
		berserkIncrement = 0;
		BerserkDisable();
		detoxFinishedTime =  -1f;
		geniusFinishedTime =  -1f;
		MouseLookScript.a.geniusActive = false;
		wirePuzzle.geniusActive = false;
		mediFinishedTime =  -1f;
		reflexFinishedTime =  -1f;
		Time.timeScale = Const.a.defaultTimeScale; // normal time speed
		sightFinishedTime =  -1f;
		sightSideEffectFinishedTime =  -1f;
		sightDimming.enabled = false;
		sightLight.enabled = false;
		staminupFinishedTime =  -1f;
		PlayerMovement.a.staminupActive = false;
		patchActive = 0;
	}

	public static string Save(GameObject go) {
		PlayerPatch pp = go.GetComponent<PlayerPatch>();
		if (pp == null) {
			Debug.Log("PlayerEnergy missing on savetype of Player!  GameObject.name: " + go.name);
			return Utils.DTypeWordToSaveString("fffffffffuu");
		}

		string line = System.String.Empty;
		line = Utils.SaveRelativeTimeDifferential(pp.berserkFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.berserkIncrementFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.detoxFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.geniusFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.mediFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.reflexFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.sightFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.sightSideEffectFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.staminupFinishedTime); // float
		line += Utils.splitChar + pp.berserkIncrement.ToString(); // int
		line += Utils.splitChar + pp.patchActive.ToString(); // int
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		PlayerPatch pp = go.GetComponent<PlayerPatch>();
		if (pp == null || index < 0 || entries == null) return index + 11;

		pp.berserkFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.berserkIncrementFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.detoxFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.geniusFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.mediFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.reflexFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.sightFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.sightSideEffectFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.staminupFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.berserkIncrement = Utils.GetIntFromString(entries[index]); index++;
		pp.patchActive = Utils.GetIntFromString(entries[index]); index++;
		return index;
	}
}
