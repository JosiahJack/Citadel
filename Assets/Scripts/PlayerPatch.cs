using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

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
	[HideInInspector] public const int PATCH_BERSERK = 1;
	[HideInInspector] public const int PATCH_DETOX = 2;
	[HideInInspector] public const int PATCH_GENIUS = 4;
	[HideInInspector] public const int PATCH_MEDI = 8;
	[HideInInspector] public const int PATCH_REFLEX = 16;
	[HideInInspector] public const int PATCH_SIGHT = 32;
	[HideInInspector] public const int PATCH_STAMINUP = 64;
	[HideInInspector] public int patchActive;  // bitflag carrier for active patches // save

	public static PlayerPatch a;

	// Patches stack so multiple can be used at once
	// For instance, berserk + staminup + medi = 1 + 64 + 8 = 73
	// This is turning on bits in the int patchActive so above would be: 01001001,
	// meaning 3 patches are enabled out of the 7 types (short integer has 8 bits
	// but the 7th bit can be used for sign +/-)

	void Awake () {
		a = this;
		a.mediFinishedTime = -1f;
		a.reflexFinishedTime = -1f;
		a.sightFinishedTime = -1f;
		a.sightLight.enabled = false;
		a.BerserkDisable();
	}

	public void ActivatePatch(int index) { // Expects the usableItems index
		bool depleted = false;
		switch (index) {
		case 14:
			// Berserk Patch
			Inventory.a.patchCounts[2]--;
			if (Inventory.a.patchCounts[2] <= 0) { depleted = true; }
			if (!(Utils.CheckFlags(patchActive, PATCH_BERSERK))) patchActive += PATCH_BERSERK;
			berserkFinishedTime = PauseScript.a.relativeTime + Const.berserkTime;
			float berserkIncrementTime = Const.berserkTime/5f;
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
			detoxFinishedTime = PauseScript.a.relativeTime + Const.detoxTime; // detox doesn't stack, it cancels itself lol
			break;
		case 16:
			// Genius Patch
			Inventory.a.patchCounts[5]--;
			if (Inventory.a.patchCounts[5] <= 0) { depleted = true; }
			if (!(Utils.CheckFlags(patchActive, PATCH_GENIUS))) patchActive += PATCH_GENIUS;
			if (geniusFinishedTime > PauseScript.a.relativeTime) {
				geniusFinishedTime += Const.geniusTime; // genius effect stacks
			} else {
				geniusFinishedTime = PauseScript.a.relativeTime + Const.geniusTime;
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
				mediFinishedTime += Const.mediTime; // medipatch effect stacks
			} else {
				mediFinishedTime = PauseScript.a.relativeTime + Const.mediTime;
			}
			break;
		case 18:
			// Reflex Patch
			Inventory.a.patchCounts[4]--;
			if (Inventory.a.patchCounts[4] <= 0) { depleted = true; }
			Time.timeScale = Const.reflexTimeScale;
			if (!(Utils.CheckFlags(patchActive, PATCH_REFLEX))) patchActive += PATCH_REFLEX;
			if (reflexFinishedTime > Time.realtimeSinceStartup ) {
				reflexFinishedTime += Const.reflexTime; // reflex effect stacks
			} else {
				reflexFinishedTime = Time.realtimeSinceStartup + Const.reflexTime;
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
				sightFinishedTime += Const.sightTime; // sight effect stacks
			} else {
				sightFinishedTime = PauseScript.a.relativeTime + Const.sightTime;
			}
			break;
		case 20:
			// Staminup Patch
			Inventory.a.patchCounts[0]--;
			if (Inventory.a.patchCounts[0] <= 0) depleted = true;
			PlayerMovement.a.staminupActive = true;
			if (!(Utils.CheckFlags(patchActive, PATCH_STAMINUP))) patchActive += PATCH_STAMINUP;
			if (staminupFinishedTime > PauseScript.a.relativeTime) {
				staminupFinishedTime += Const.staminupTime; // staminup effect stacks
			} else {
				staminupFinishedTime = PauseScript.a.relativeTime + Const.staminupTime;
			}

			break;
		}

		if (depleted) {
			Inventory.a.PatchCycleDown(false);
			Const.sprint((Const.a.stringTable[590]
						 + Const.a.stringTable[index + 326]
						 + Const.a.stringTable[589]),MouseLookScript.a.player);
		} else {
			Const.sprint((Const.a.stringTable[index + 326]
						 + Const.a.stringTable[589]),MouseLookScript.a.player);
		}

		Utils.PlayUIOneShotSavable(89);
		GUIState.a.ClearOverButton();
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
					Time.timeScale = Const.defaultTimeScale;
					reflexFinishedTime = -1;
				} else {
					// ***Patch Effect***
					if (Time.timeScale != Const.reflexTimeScale) {
						Time.timeScale = Const.reflexTimeScale;
					}
				}
			} else {
			    if (Time.timeScale != Const.defaultTimeScale) {
					Time.timeScale = Const.defaultTimeScale;
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
						float berserkIncrementTime = Const.berserkTime/5f;
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
					sightSideEffectFinishedTime = PauseScript.a.relativeTime + Const.sightSideEffectTime;
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
		Time.timeScale = Const.defaultTimeScale; // normal time speed
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
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.SaveRelativeTimeDifferential(pp.berserkFinishedTime,"berserkFinishedTime"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pp.berserkIncrementFinishedTime,"berserkIncrementFinishedTime"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pp.detoxFinishedTime,"detoxFinishedTime"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pp.geniusFinishedTime,"geniusFinishedTime"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pp.mediFinishedTime,"mediFinishedTime"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(pp.reflexFinishedTime - Time.realtimeSinceStartup,"reflexFinishedTime"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pp.sightFinishedTime,"sightFinishedTime"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pp.sightSideEffectFinishedTime,"sightSideEffectFinishedTime"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(pp.staminupFinishedTime,"staminupFinishedTime"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(pp.berserkIncrement,"berserkIncrement"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(pp.patchActive,"patchActive"));
		s1.Append(Utils.splitChar);

		// Grayscale saved within each SaveCamera
		// SaveCamera 2
		// BerserkEffect 3
		s1.Append(BerserkEffect.Save(pp.berserk.gameObject));
		s1.Append(Utils.splitChar);
		s1.Append(BerserkEffect.Save(pp.sensaroundCamCenterBerserk.gameObject));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveCamera(pp.sensaroundCamCenterBerserk.gameObject));
		s1.Append(Utils.splitChar);
		s1.Append(BerserkEffect.Save(pp.sensaroundCamLeftBerserk.gameObject));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveCamera(pp.sensaroundCamLeftBerserk.gameObject));
		s1.Append(Utils.splitChar);
		s1.Append(BerserkEffect.Save(pp.sensaroundCamRightBerserk.gameObject));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveCamera(pp.sensaroundCamRightBerserk.gameObject));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		PlayerPatch pp = go.GetComponent<PlayerPatch>();
		pp.berserkFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index],"berserkFinishedTime"); index++;
		pp.berserkIncrementFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index],"berserkIncrementFinishedTime"); index++;
		pp.detoxFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index],"detoxFinishedTime"); index++;
		pp.geniusFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index],"geniusFinishedTime"); index++;
		pp.mediFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index],"mediFinishedTime"); index++;
		pp.reflexFinishedTime = Utils.GetFloatFromString(entries[index],"reflexFinishedTime");
		pp.reflexFinishedTime += Time.realtimeSinceStartup; index++;
		pp.sightFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index],"sightFinishedTime"); index++;
		pp.sightSideEffectFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index],"sightSideEffectFinishedTime"); index++;
		pp.staminupFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index],"staminupFinishedTime"); index++;
		pp.berserkIncrement = Utils.GetIntFromString(entries[index],"berserkIncrement"); index++;
		pp.patchActive = Utils.GetIntFromString(entries[index],"patchActive"); index++;
		index = BerserkEffect.Load(pp.berserk.gameObject,ref entries,index);
		index = BerserkEffect.Load(pp.sensaroundCamCenterBerserk.gameObject,ref entries,index);
		index = Utils.LoadCamera(pp.sensaroundCamCenterBerserk.gameObject,ref entries,index);
		index = BerserkEffect.Load(pp.sensaroundCamLeftBerserk.gameObject,ref entries,index);
		index = Utils.LoadCamera(pp.sensaroundCamLeftBerserk.gameObject,ref entries,index);
		index = BerserkEffect.Load(pp.sensaroundCamRightBerserk.gameObject,ref entries,index);
		index = Utils.LoadCamera(pp.sensaroundCamRightBerserk.gameObject,ref entries,index);
		return index;
	}
}
