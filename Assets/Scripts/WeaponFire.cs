using UnityEngine;
using System.Collections;

public class WeaponFire : MonoBehaviour {
	// External references, required
    public GameObject impactEffect;
	public GameObject noDamageIndicator;
	public Camera gunCamera;
    public Camera playerCamera; // assign in the editor
    public GameObject playerCapsule;
    public EnergyOverloadButton energoverButton;
    public EnergyHeatTickManager energheatMgr;
	public GameObject bulletHoleTiny;
	public GameObject bulletHoleSmall;
	public GameObject bulletHoleSpread;
	public GameObject bulletHoleLarge;
	public GameObject bulletHoleScorchSmall;
	public GameObject bulletHoleScorchLarge;
    public AudioSource SFX = null; // assign in the editor
    public AudioClip SFXMark3Fire; // assign in the editor
    public AudioClip SFXBlasterFire; // assign in the editor
    public AudioClip SFXDartFire; // assign in the editor
    public AudioClip SFXFlechetteFire; // assign in the editor
    public AudioClip SFXIonFire; // assign in the editor
    public AudioClip SFXRapierMiss; // assign in the editor
    public AudioClip SFXRapierHit; // assign in the editor
    public AudioClip SFXPipeMiss; // assign in the editor
    public AudioClip SFXPipeHit; // assign in the editor
    public AudioClip SFXPipeHitFlesh; // assign in the editor
    public AudioClip SFXMagnumFire; // assign in the editor
    public AudioClip SFXMagpulseFire; // assign in the editor
    public AudioClip SFXPistolFire; // assign in the editor
    public AudioClip SFXPlasmaFire; // assign in the editor
    public AudioClip SFXRailgunFire; // assign in the editor
    public AudioClip SFXRiotgunFire; // assign in the editor
    public AudioClip SFXSkorpionFire; // assign in the editor
    public AudioClip SFXSparqBeamFire; // assign in the editor
    public AudioClip SFXStungunFire; // assign in the editor
    public AudioClip SFXEmpty; // assign in the editor
    public AudioClip SFXRicochet; // assign in the editor
    public AudioClip SFXPulserFire; // assign in the editor
    public AudioClip SFXDrillFire; // assign in the editor
    public Animator anim; // assign in the editor
	public Animator rapieranim; // assign in the editor
	public GameObject muzFlashMK3;
	public GameObject muzFlashBlaster;
	public GameObject muzFlashDartgun;
	public GameObject muzFlashFlechette;
	public GameObject muzFlashIonBeam;
	public GameObject muzFlashMagnum;
	public GameObject muzFlashPistol;
	public GameObject muzFlashMagpulse;
	public GameObject muzFlashPlasma;
	public GameObject muzFlashRailgun;
	public GameObject muzFlashRiotgun;
	public GameObject muzFlashSkorpion;
	public GameObject muzFlashSparq;
	public GameObject muzFlashStungun;
	public Transform reloadContainer; // Recoil the weapon view models
    public bool overloadEnabled; // save
	public float reloadFinished; // save
	public float lerpStartTime; // save
	public float reloadLerpValue; // save

    [HideInInspector] public DamageData damageData;
    [HideInInspector] public float waitTilNextFire = 0f; // save
    [HideInInspector] public float sparqSetting = 50f; // save
    [HideInInspector] public float ionSetting = 100f; // save
    [HideInInspector] public float blasterSetting = 15f; // save
    [HideInInspector] public float plasmaSetting = 40f; // save
    [HideInInspector] public float stungunSetting = 20f;  // save
	[HideInInspector] public Vector3 reloadContainerHome;
	[HideInInspector] public bool recoiling; // save
	[HideInInspector] public int lerpUp = 0; // 0 = not lerping, 1 = up, 2 = dn
	[HideInInspector] public float justFired; // save
	[HideInInspector] public float energySliderClickedTime; // save
	[HideInInspector] public float cyberWeaponAttackFinished; // save
	[HideInInspector] public float reloadContainerDropAmount = 0.66f;
	[HideInInspector] public float targetY; // save

	// Internal references
    private float hitOffset = 0f;
    private float verticalOffset = -0.2f; // For laser beams
    private float fireDistance = 200f;
    private float hitscanDistance = 200f;
    private float meleescanDistance = 3.2f;
	private float overheatedPercent = 80f;
    private float magpulseShotForce = 2.2f;
    private float stungunShotForce = 2.2f;
    private float railgunShotForce = 5f;
    private float plasmaShotForce = 1.5f;
	private float inventoryModeViewRotateMax = 48f;
    private float clipEnd;
    private RaycastHit tempHit;
    private Vector3 tempVec;
    private bool useBlood;
    private HealthManager tempHM;
    private float retval;
    private float heatTickFinished;
    private float heatTickTime = 0.50f;
	private Rigidbody playercapRbody;
	private float wepYRot;

	// Not needed on Const as this only exists in one unique place on player.
	private float[] driftForWeapon = new float[16]{5f,0f,15f,50f,0f,0f,0f,8f,
												   3f,3f,3f,12f,10f,30f,0f,3f};

	// Singleton instance
	public static WeaponFire a;

	void Awake() {
		a = this;
	}

    void Start() {
        damageData = new DamageData();
        tempHit = new RaycastHit();
        tempVec = new Vector3(0f, 0f, 0f);
        heatTickFinished = PauseScript.a.relativeTime + heatTickTime;
		reloadContainerHome = reloadContainer.localPosition;

		// Set less than 30s before PauseScript.a.relativeTime to guarantee we
		// don't immediately play action music.
		justFired = (PauseScript.a.relativeTime - 31f);

		energySliderClickedTime = PauseScript.a.relativeTime;
		playercapRbody = playerCapsule.GetComponent<Rigidbody>();
		cyberWeaponAttackFinished = PauseScript.a.relativeTime;
		wepYRot = 0f;
		sparqSetting = 50f;
		ionSetting = 100f;
		blasterSetting = 15f;
		plasmaSetting = 40f;
		stungunSetting = 20f;
		reloadLerpValue = 0;
		reloadFinished = PauseScript.a.relativeTime;
		lerpStartTime = PauseScript.a.relativeTime;
    }

    void GetWeaponData(int index) {
        if (index < 0) return;
		if (WeaponCurrent.a.weaponCurrent < 0) return;

        damageData.isFullAuto = Const.a.isFullAutoForWeapon[index];
        if (Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent]) {
			// Alternate (2)
            damageData.damage = Const.a.damagePerHitForWeapon2[index];
            damageData.delayBetweenShots = 
				Const.a.delayBetweenShotsForWeapon2[index];

            damageData.penetration = Const.a.penetrationForWeapon2[index];
            damageData.offense = Const.a.offenseForWeapon2[index];
        } else {
			// Normal
            damageData.damage = Const.a.damagePerHitForWeapon[index];
            damageData.delayBetweenShots =
				Const.a.delayBetweenShotsForWeapon[index];

            damageData.penetration = Const.a.penetrationForWeapon[index];
            damageData.offense = Const.a.offenseForWeapon[index];
        }

        damageData.damageOverload = Const.a.damageOverloadForWeapon[index];
        damageData.energyDrainLow = Const.a.energyDrainLowForWeapon[index];
        damageData.energyDrainHi = Const.a.energyDrainHiForWeapon[index];
        damageData.energyDrainOver = Const.a.energyDrainOverloadForWeapon[index];
        damageData.range = Const.a.rangeForWeapon[index];
        damageData.attackType = Const.a.attackTypeForWeapon[index];
        damageData.berserkActive = (Utils.CheckFlags(PlayerPatch.a.patchActive,
												 PlayerPatch.a.PATCH_BERSERK));
    }

    public static int Get16WeaponIndexFromConstIndex(int index) {
        switch (index) {
            case 36: return 0; // Mark3 Assault Rifle
            case 37: return 1; // ER-90 Blaster
            case 38: return 2; // SV-23 Dartgun
            case 39: return 3; // AM-27 Flechette
            case 40: return 4; // RW-45 Ion Beam
            case 41: return 5; // TS-04 Laser Rapier
            case 42: return 6; // Lead Pipe
            case 43: return 7; // Magnum 2100
            case 44: return 8; // SB-20 Magpulse
            case 45: return 9; // ML-41 Pistol
            case 46: return 10;// LG-XX Plasma Rifle
            case 47: return 11;// MM-76 Railgun
            case 48: return 12;// DC-05 Riotgun
            case 49: return 13;// RF-07 Skorpion
            case 50: return 14;// Sparq Beam
            case 51: return 15;// DH-07 Stungun
        }
        return -1;
    }

    bool CurrentWeaponUsesEnergy () {
        if (WeaponCurrent.a.weaponIndex == 37 || WeaponCurrent.a.weaponIndex == 40 ||
			WeaponCurrent.a.weaponIndex == 46 || WeaponCurrent.a.weaponIndex == 50 ||
			WeaponCurrent.a.weaponIndex == 51)
			return true;
        return false;
    }

    bool WeaponsHaveAnyHeat() {
		if (WeaponCurrent.a.redbull) return false;
		if (Inventory.a.currentEnergyWeaponHeat[0] > 0f) return true;
		if (Inventory.a.currentEnergyWeaponHeat[1] > 0f) return true;
		if (Inventory.a.currentEnergyWeaponHeat[2] > 0f) return true;
		if (Inventory.a.currentEnergyWeaponHeat[3] > 0f) return true;
		if (Inventory.a.currentEnergyWeaponHeat[4] > 0f) return true;
		if (Inventory.a.currentEnergyWeaponHeat[5] > 0f) return true;
		if (Inventory.a.currentEnergyWeaponHeat[6] > 0f) return true;
        return false;
    }

    void HeatBleedOff() {
        if (heatTickFinished < PauseScript.a.relativeTime) {
			Inventory.a.currentEnergyWeaponHeat[0] -= 10f; if (Inventory.a.currentEnergyWeaponHeat[0] <= 0f) Inventory.a.currentEnergyWeaponHeat[0] = 0f;
			Inventory.a.currentEnergyWeaponHeat[1] -= 10f; if (Inventory.a.currentEnergyWeaponHeat[1] <= 0f) Inventory.a.currentEnergyWeaponHeat[1] = 0f;
			Inventory.a.currentEnergyWeaponHeat[2] -= 10f; if (Inventory.a.currentEnergyWeaponHeat[2] <= 0f) Inventory.a.currentEnergyWeaponHeat[2] = 0f;
			Inventory.a.currentEnergyWeaponHeat[3] -= 10f; if (Inventory.a.currentEnergyWeaponHeat[3] <= 0f) Inventory.a.currentEnergyWeaponHeat[3] = 0f;
			Inventory.a.currentEnergyWeaponHeat[4] -= 10f; if (Inventory.a.currentEnergyWeaponHeat[4] <= 0f) Inventory.a.currentEnergyWeaponHeat[4] = 0f;
			Inventory.a.currentEnergyWeaponHeat[5] -= 10f; if (Inventory.a.currentEnergyWeaponHeat[5] <= 0f) Inventory.a.currentEnergyWeaponHeat[5] = 0f;
			Inventory.a.currentEnergyWeaponHeat[6] -= 10f; if (Inventory.a.currentEnergyWeaponHeat[6] <= 0f) Inventory.a.currentEnergyWeaponHeat[6] = 0f;
            if (CurrentWeaponUsesEnergy()) energheatMgr.HeatBleed(Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent]); // update hud heat ticks if current weapon uses energy
            heatTickFinished = PauseScript.a.relativeTime + heatTickTime;
        }
    }

	public void Recoil (int i) {
		float strength = Const.a.recoilForWeapon[i];
		//Debug.Log("Recoil from gun index: "+i.ToString()+" with strength of " +strength.ToString());
		if (strength <= 0f) return;
		if (PlayerMovement.a.fatigue > 80) strength = strength * 2f;
		strength = strength * 0.25f;
		Vector3 wepJoltPosition = new Vector3(reloadContainer.localPosition.x - (strength * 0.5f * Random.Range(-1f,1f)), reloadContainer.localPosition.y, (reloadContainerHome.z - strength));
		if (wepJoltPosition.x > 999f) wepJoltPosition.x = 0;
		if (wepJoltPosition.y > 999f) wepJoltPosition.y = 0;
		if (wepJoltPosition.z > 999f) wepJoltPosition.z = 0;
		reloadContainer.localPosition = wepJoltPosition;
		recoiling = true;
	}

	void WeaponLerpGetTargetUp() {
		// Percentage of this half of the trip.
		reloadLerpValue = (0.5f - (1 - reloadLerpValue))/0.5f;
		targetY = (-1 * reloadContainerDropAmount * (1 - reloadLerpValue));
		if (targetY > reloadContainerHome.y) targetY = reloadContainerHome.y;
	}

	void WeaponLerpGetTargetDown() {
		// Percentage of this half of the trip.
		reloadLerpValue = reloadLerpValue/0.5f;
		targetY = reloadContainerHome.y - reloadContainerDropAmount;
		targetY *= reloadLerpValue;
	}

	void Recoiling() {
		if (!recoiling) return;

		float x = reloadContainer.localPosition.x; // side to side
		float z = reloadContainer.localPosition.z; // forward and back
		z = Mathf.Lerp(z,reloadContainerHome.z,Time.deltaTime);
		x = Mathf.Lerp(x,reloadContainerHome.x,Time.deltaTime);
		reloadContainer.localPosition = 
			new Vector3(x,reloadContainer.localPosition.y,z);
	}

	void UpdateWeaponReloadDip() {
		// Move weapon transform up/down for reload "animation" & weapon swap.
		int i = Get16WeaponIndexFromConstIndex(WeaponCurrent.a.weaponIndex);
		if (i < 0 || i > 15) i = 0;
		if (reloadFinished > PauseScript.a.relativeTime) {
			float elapsed = (PauseScript.a.relativeTime - lerpStartTime);

			// Percent towards goal time total (both halves of the action).
			reloadLerpValue = (elapsed/Const.a.reloadTime[i]);
			if (reloadLerpValue >= 0.5f) { // Flip back to lerp up.
				lerpUp = 1;
				WeaponLerpGetTargetUp();
				CompleteWeaponChange();
			} else {
				lerpUp = 2;
				WeaponLerpGetTargetDown();
			}

			Mathf.Clamp(targetY, -100f, 100f);
			Vector3 pos = new Vector3(reloadContainer.localPosition.x,
									  targetY,
									  reloadContainer.localPosition.z);

			reloadContainer.localPosition = pos;
		} else {
			lerpUp = 0;
			Vector3 pos = new Vector3(reloadContainer.localPosition.x,
									  reloadContainerHome.y,
									  reloadContainer.localPosition.z);

			reloadContainer.localPosition = pos;
		}
	}

    void Update() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		// Slowly cool off any weapons that have been heated from firing
		if (WeaponsHaveAnyHeat() || CurrentWeaponUsesEnergy()) HeatBleedOff();

		UpdateWeaponReloadDip();
		RotateViewWeapon();
		Recoiling();
		CheckAttackInput();
		CheckReloadInput();
		CheckAmmoChangeInput();
    }

	public void CompleteWeaponChange() {
		if (WeaponCurrent.a.weaponCurrentPending == -1) return;

		// Set current weapon 7 slot
		WeaponCurrent.a.weaponCurrent = WeaponCurrent.a.weaponCurrentPending;
        if (CurrentWeaponUsesEnergy()) {
			// Update hud heat ticks if current weapon uses energy
			int iC = WeaponCurrent.a.weaponCurrent;
			energheatMgr.HeatBleed(Inventory.a.currentEnergyWeaponHeat[iC]);
		}

		// Set current weapon inventory lookup index
		WeaponCurrent.a.weaponIndex = WeaponCurrent.a.weaponIndexPending;

		// Reset pending indices now that transition is done
		WeaponCurrent.a.weaponCurrentPending = -1;
		WeaponCurrent.a.weaponIndexPending = -1;

		// Update the ammo icons.
		int ind = WeaponCurrent.a.weaponIndex;
		bool alt = false;
		if (ind >= 0 && ind < 16) alt = Inventory.a.wepLoadedWithAlternate[ind];
		MFDManager.a.SetAmmoIcons(ind,alt);
		MFDManager.a.SetWepInfo(WeaponCurrent.a.weaponIndex);
	}

	public void StartWeaponDip(float delay) {
		if (delay < 0) delay = 0;
		reloadFinished = PauseScript.a.relativeTime + delay;
		lerpStartTime = PauseScript.a.relativeTime;
	}

	void RotateViewWeapon() {
		if (MouseLookScript.a.inventoryMode) {
			float screenHalf = (Screen.width/2f);
			float cursorX = MouseCursor.a.drawTexture.center.x;
			float distFromCenter = (cursorX - screenHalf);
			float percentRotated = (distFromCenter / screenHalf);
			wepYRot = percentRotated * inventoryModeViewRotateMax;
			reloadContainer.localRotation = Quaternion.Euler(0f,wepYRot,0f);
		} else {
			reloadContainer.localRotation = Quaternion.Euler(0f,0f,0f);
		}
	}

	void CheckAttackInput() {
		// Check for other things that must capture and override clicks
		if (GetInput.a.Attack(true)) {
			if (MouseLookScript.a.vmailActive) {
				Inventory.a.DeactivateVMail();
				MouseLookScript.a.vmailActive = false;
				waitTilNextFire = PauseScript.a.relativeTime + 0.8f;
				return;
			}

			if (MouseLookScript.a.inCyberSpace) {
				FireCyberWeapon();
				return;
			}

			if (MouseLookScript.a.holdingObject
				&& !MFDManager.a.mouseClickHeldOverGUI) { // !Just clicked
				if (!GUIState.a.isBlocking) {
					// Drop it
					MouseLookScript.a.DropHeldItem ();
					return;
				} else {
					MouseLookScript.a.AddItemToInventory(MouseLookScript.a.heldObjectIndex,MouseLookScript.a.heldObjectCustomIndex);
					MouseLookScript.a.ResetHeldItem();
					MouseLookScript.a.ResetCursor();
					return;
				}
			}
		}

		int wepdex = Get16WeaponIndexFromConstIndex(WeaponCurrent.a.weaponIndex);
		if (wepdex == -1) return; // No weapon.
		if (GUIState.a.isBlocking) return;
		if (MouseLookScript.a.holdingObject) return;
		if (MFDManager.a.mouseClickHeldOverGUI) return;

		GetWeaponData(wepdex);
		if (GetInput.a.Attack(Const.a.isFullAutoForWeapon[wepdex])
			&& waitTilNextFire < PauseScript.a.relativeTime
			&& (PauseScript.a.relativeTime - energySliderClickedTime) > 0.1f
			&& reloadFinished < PauseScript.a.relativeTime) {
			StartCoroutine(CheckUIStateAndAttack(wepdex));
		}
	}

	IEnumerator CheckUIStateAndAttack(int wepdex) {
		yield return null; // Ensure next frame

		if (GUIState.a.isBlocking) yield break;
		if (MouseLookScript.a.holdingObject) yield break;
		if (MFDManager.a.mouseClickHeldOverGUI) yield break;
		if (reloadFinished >= PauseScript.a.relativeTime) yield break;
		if (waitTilNextFire >= PauseScript.a.relativeTime) yield break;
		if (wepdex < 0 || wepdex > 15) yield break;
		if (Automap.a.inFullMap) yield break;

		justFired = PauseScript.a.relativeTime; // set justFired so that Music.cs can see it and play corresponding music in a little bit from now or keep playing action music
		// Check weapon type and check ammo before firing
		switch (wepdex) {
			case 1: goto case 15;
			case 4: goto case 15;
			case 5: goto case 6;
			case 6:
				// Pipe or Laser Rapier, attack without prejudice.
				// isSilent == false here so play normal SFX.
				FireWeapon(wepdex, false); 
				break;
			case 10: goto case 15;
			case 14: goto case 15;
			case 15: 
				// Energy weapons so check energy level
				// Even if we have only 1 energy, we still fire with all we've got up to the energy level setting of course
				if (PlayerEnergy.a.energy > 0
					|| WeaponCurrent.a.bottomless
					|| WeaponCurrent.a.redbull) {
					if (Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] > overheatedPercent
						&& !WeaponCurrent.a.bottomless
						&& !WeaponCurrent.a.redbull) {
						Utils.PlayOneShotSavable(SFX,SFXEmpty);
						waitTilNextFire = PauseScript.a.relativeTime + 0.8f;
						Const.sprint(Const.a.stringTable[11]);
					} else {
						FireWeapon(wepdex, false); // weapon index, isSilent == false so play normal SFX
					}
				} else {
					Const.sprint(Const.a.stringTable[207]); // Not enough energy to fire weapon.
				}
				break;
			default:
				// Uses normal ammo, check versus alternate or normal to see if we have ammo then fire
				if (Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent]) {
					if (WeaponCurrent.a.currentMagazineAmount2[WeaponCurrent.a.weaponCurrent] > 0
						|| WeaponCurrent.a.bottomless) {
						FireWeapon(wepdex, false); // weapon index, isSilent == false so play normal SFX
					} else {
						Utils.PlayOneShotSavable(SFX,SFXEmpty);
						waitTilNextFire = PauseScript.a.relativeTime + 0.8f;
					}
				} else {
					if (WeaponCurrent.a.currentMagazineAmount[WeaponCurrent.a.weaponCurrent] > 0
						|| WeaponCurrent.a.bottomless) {
						FireWeapon(wepdex, false); // weapon index, isSilent == false so play normal SFX
					} else {
						Utils.PlayOneShotSavable(SFX,SFXEmpty);
						waitTilNextFire = PauseScript.a.relativeTime + 0.8f;
					}
				}
				break;
		}
	}

	void CheckReloadInput() {
		if (reloadFinished >= PauseScript.a.relativeTime) return;
		if (!GetInput.a.Reload()) return;

		if (Const.a.InputQuickReloadWeapons) {
			// Press reload once, to do both unload then reload
			WeaponCurrent.a.Reload();
			return;
		}

		if (WeaponCurrent.a.weaponCurrent < 0) return;

		// First press reload to unload, then press again to load
		int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(WeaponCurrent.a.weaponIndex);
		if (wep16index < 0) return;

		if (Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent]) {
			if (WeaponCurrent.a.currentMagazineAmount2[WeaponCurrent.a.weaponCurrent] <= 0
				|| Inventory.a.wepAmmoSecondary[wep16index] <= 0) { // True for no wepAmmoSecondary causes Reload to run and display no ammo message.
				WeaponCurrent.a.Reload();
				// Debug.Log("Reload step");
			} else {
				WeaponCurrent.a.Unload(false);
				// Debug.Log("Unload step");
			}
		} else {
			if (WeaponCurrent.a.currentMagazineAmount[WeaponCurrent.a.weaponCurrent] <= 0
				|| Inventory.a.wepAmmo[wep16index] <= 0) { // True for no wepAmmo causes Reload to run and display no ammo message.
				WeaponCurrent.a.Reload();
				// Debug.Log("Reload step");
			} else {
				WeaponCurrent.a.Unload(false);
				// Debug.Log("Unload step");
			}
		}
	}

	void CheckAmmoChangeInput() {
		if (reloadFinished >= PauseScript.a.relativeTime) return;
		if (!GetInput.a.ChangeAmmoType()) return;

		if (Const.a.InputQuickReloadWeapons) {
			// Press change ammo type button once, to both unload then reload.
			WeaponCurrent.a.ChangeAmmoType();
		} else {
			// First press ammo type button to unload, then again to load.
			int wep16index = WeaponFire.Get16WeaponIndexFromConstIndex(WeaponCurrent.a.weaponIndex);
			if (wep16index < 0) return;

			int ammoAvailable = 0;
			if (Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent]) {
				ammoAvailable = Inventory.a.wepAmmoSecondary[wep16index];
			} else {
				ammoAvailable = Inventory.a.wepAmmo[wep16index];
			}

			if (ammoAvailable <= 0) WeaponCurrent.a.ChangeAmmoType();
			else WeaponCurrent.a.Unload(false);
		}
	}

	void FireCyberWeapon() {
		if (cyberWeaponAttackFinished < PauseScript.a.relativeTime) {
			if (Inventory.a.isPulserNotDrill) {
				if (Inventory.a.hasSoft[1]) {
					// Fire pulser
					if (Inventory.a.hasSoft[1]) FireCyberBeachball(true,railgunShotForce,492);
					if (SFXPulserFire != null) { SFX.clip = SFXPulserFire; SFX.Play(); }
					cyberWeaponAttackFinished = PauseScript.a.relativeTime + 0.08f;
				}
			} else {
				if (Inventory.a.hasSoft[0]) {
					// Fire I.C.E. drill
					if (Inventory.a.hasSoft[0]) FireCyberBeachball(false,plasmaShotForce,495);
					if (SFXDrillFire != null) { SFX.clip = SFXDrillFire; SFX.Play(); }
					cyberWeaponAttackFinished = PauseScript.a.relativeTime + 0.5f;
				}
			}
		}
	}

	void FireCyberBeachball(bool isPulser, float shoveForce, int prefabID) {
        // Create and hurl a beachball-like object.  On the developer commentary they said that the projectiles act
        // like a beachball for collisions with enemies, but act like a baseball for walls/floor to prevent hitting corners
        GameObject beachball = ConsoleEmulator.SpawnDynamicObject(prefabID,-1);
        if (beachball != null) {
			damageData.damage = 10f * Inventory.a.softVersions[0];
			if (isPulser) damageData.damage = 1f; // Cyberspace enemies don't have much health
            damageData.owner = playerCapsule;
            damageData.attackType = AttackType.ProjectileLaunched;
			if (!isPulser) damageData.attackType = AttackType.Drill;
            beachball.GetComponent<ProjectileEffectImpact>().dd = damageData;
            beachball.GetComponent<ProjectileEffectImpact>().host = playerCapsule;
            beachball.transform.position = playerCamera.transform.position;
			MouseLookScript.a.SetCameraFocusPoint();
            tempVec = MouseLookScript.a.cameraFocusPoint - playerCamera.transform.position;
            beachball.transform.forward = tempVec.normalized;
            beachball.SetActive(true);
            Vector3 shove = beachball.transform.forward * shoveForce;
            beachball.GetComponent<Rigidbody>().velocity = Const.a.vectorZero; // prevent random variation from the last shot's velocity
            beachball.GetComponent<Rigidbody>().AddForce(shove, ForceMode.Impulse);
        }
	}

    // index is used to get recoil down at the bottom and pass along ref for damageData, otherwise the cases use WeaponCurrent.a.weaponIndex
    void FireWeapon(int index, bool isSilent) {
		if (WeaponCurrent.a.weaponIndex != 41 && WeaponCurrent.a.weaponIndex != 42) {
			PlayerHealth.a.makingNoise = true;
			PlayerHealth.a.noiseFinished = PauseScript.a.relativeTime + 0.5f;
		}

        switch (WeaponCurrent.a.weaponIndex) {
            case 36:
                //Mark3 Assault Rifle
                if (!isSilent) { SFX.clip = SFXMark3Fire; SFX.Play(); }
                if (DidRayHit(index)) HitScanFire(index);
				muzFlashMK3.SetActive(true);
                break;
            case 37:
                //ER-90 Blaster
				blasterSetting = WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent];
				//Debug.Log("Blaster fired with energy setting of " + blasterSetting.ToString());
				if (!isSilent) { SFX.clip = SFXBlasterFire; SFX.Play(); }
				if (DidRayHit(index)) HitScanFire(index);
				muzFlashBlaster.SetActive(true);
                if (overloadEnabled) {
                    Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] = 100f;
                } else {
                    Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] += blasterSetting;
					if (Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] > 100f) Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] = 100f; // cap it
                }
                break;
            case 38:
                //SV-23 Dartgun
                if (!isSilent) { SFX.clip = SFXDartFire; SFX.Play(); }
                if (DidRayHit(index)) HitScanFire(index);
				muzFlashDartgun.SetActive(true);
                break;
            case 39:
                //AM-27 Flechette
                if (!isSilent) { SFX.clip = SFXFlechetteFire; SFX.Play(); }
                if (DidRayHit(index)) HitScanFire(index);
				muzFlashFlechette.SetActive(true);
                break;
            case 40:
                //RW-45 Ion Beam
				ionSetting = WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent];
				//Debug.Log("Ion rifle fired with energy setting of " + ionSetting.ToString());
                if (!isSilent) { SFX.clip = SFXIonFire; SFX.Play(); }
                if (DidRayHit(index)) HitScanFire(index);
				muzFlashIonBeam.SetActive(true);
                if (overloadEnabled) {
                    Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] = 100f;
                } else {
                    Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] += ionSetting;
					if (Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] > 100f) Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] = 100f; // cap it
                }
                break;
            case 41:
                //TS-04 Laser Rapier
                FireRapier(index, isSilent);
                break;
            case 42:
                //Lead Pipe
                FirePipe(index, isSilent);
                break;
            case 43:
                //Magnum 2100
                if (!isSilent) { SFX.clip = SFXMagnumFire; SFX.Play(); }
                if (DidRayHit(index)) HitScanFire(index);
				muzFlashMagnum.SetActive(true);
                break;
            case 44:
                //SB-20 Magpulse
                if (!isSilent) { SFX.clip = SFXMagpulseFire; SFX.Play(); }
                FireMagpulse(index);
				muzFlashMagpulse.SetActive(true);
                break;
            case 45:
                //ML-41 Pistol
                if (!isSilent) { SFX.clip = SFXPistolFire; SFX.Play(); }
                if (DidRayHit(index)) HitScanFire(index);
				muzFlashPistol.SetActive(true);
                break;
            case 46:
                //LG-XX Plasma Rifle
				plasmaSetting = WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent];
				//Debug.Log("Plasma rifle fired with energy setting of " + plasmaSetting.ToString());
                if (!isSilent) { SFX.clip = SFXPlasmaFire; SFX.Play(); }
                FirePlasma(index);
				muzFlashPlasma.SetActive(true);
                if (overloadEnabled) {
                    Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] = 100f;
                } else {
                    Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] += plasmaSetting;
					if (Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] > 100f) Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] = 100f; // cap it
                }
                break;
            case 47:
                //MM-76 Railgun
                if (!isSilent) { SFX.clip = SFXRailgunFire; SFX.Play(); }
                FireRailgun(index);
				muzFlashRailgun.SetActive(true);
                break;
            case 48:
                //DC-05 Riotgun
                if (!isSilent) { SFX.clip = SFXRiotgunFire; SFX.Play(); }
                if (DidRayHit(index)) HitScanFire(index);
				muzFlashRiotgun.SetActive(true);
                break;
            case 49:
                //RF-07 Skorpion
                if (!isSilent) { SFX.clip = SFXSkorpionFire; SFX.Play(); }
                if (DidRayHit(index)) HitScanFire(index);
				muzFlashSkorpion.SetActive(true);
                break;
            case 50:
                //Sparq Beam
				sparqSetting = WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent];
                if (!isSilent) { SFX.clip = SFXSparqBeamFire; SFX.Play(); }
                if (DidRayHit(index)) HitScanFire(index);
				muzFlashSparq.SetActive(true);
                if (overloadEnabled) {
                    Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] = 100f;
                } else {
                    Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] += sparqSetting;
					if (Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] > 100f) Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] = 100f; // cap it
                }
                break;
            case 51:
                //DH-07 Stungun
				stungunSetting = WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent];
                if (!isSilent) { SFX.clip = SFXStungunFire; SFX.Play(); }
                FireStungun(index);
				muzFlashStungun.SetActive(true);
                if (overloadEnabled) {
                    Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] = 100f;
                } else {
                    Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] += stungunSetting;
					if (Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] > 100f) Inventory.a.currentEnergyWeaponHeat[WeaponCurrent.a.weaponCurrent] = 100f; // cap it
                }
                break;
        }

        // TAKE AMMO
        // no weapons subtract more than 1 at a time in a shot except for energy weapons, subtracting 1
        // Check weapon type before subtracting ammo or energy
        if (index == 5 || index == 6) {
            // Pipe or Laser Rapier
            // ammo is already 0, do nothing.  This is here to prevent subtracting ammo on the first slot of .wepAmmo[index] on the last else clause below
        } else {
            // Energy weapons so check energy level
            if (index == 1 || index == 4 || index == 10 || index == 14 || index == 15) {
                if (overloadEnabled) {
                    energoverButton.OverloadFired();
                    if (!WeaponCurrent.a.bottomless && !WeaponCurrent.a.redbull) {
						PlayerEnergy.a.TakeEnergy(Const.a.energyDrainOverloadForWeapon[index]); //take large amount
						if (BiomonitorGraphSystem.a != null) {
							BiomonitorGraphSystem.a.EnergyPulse(Const.a.energyDrainOverloadForWeapon[index]);
						}
					}
                } else {
                    float takeEnerg = (WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent] / 100f) * (Const.a.energyDrainHiForWeapon[index] - Const.a.energyDrainLowForWeapon[index]);
                    if (!WeaponCurrent.a.bottomless && !WeaponCurrent.a.redbull) {
						PlayerEnergy.a.TakeEnergy(takeEnerg);
						if (BiomonitorGraphSystem.a != null) {
							BiomonitorGraphSystem.a.EnergyPulse(takeEnerg);
						}
					}
                }
            } else {
                if (Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent]) {
                    if (!WeaponCurrent.a.bottomless) WeaponCurrent.a.currentMagazineAmount2[WeaponCurrent.a.weaponCurrent]--; // Take ammo away
                } else {
                    if (!WeaponCurrent.a.bottomless) WeaponCurrent.a.currentMagazineAmount[WeaponCurrent.a.weaponCurrent]--; // Take ammo away
                }
            }
        }

		Recoil(index);
        if (Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent] || overloadEnabled) {
            overloadEnabled = false;
            waitTilNextFire = PauseScript.a.relativeTime + Const.a.delayBetweenShotsForWeapon2[index];
        } else {
            waitTilNextFire = PauseScript.a.relativeTime + Const.a.delayBetweenShotsForWeapon[index];
        }

		Inventory.a.UpdateAmmoText();
    }

    bool DidRayHit(int wep16Index) {
        tempHit = new RaycastHit();
		tempVec = MouseCursor.a.GetCursorScreenPointForRay();
		tempVec.x += UnityEngine.Random.Range(-driftForWeapon[wep16Index],driftForWeapon[wep16Index]);
		tempVec.y += UnityEngine.Random.Range(-driftForWeapon[wep16Index],driftForWeapon[wep16Index]);
        if (Physics.Raycast(playerCamera.ScreenPointToRay(tempVec), out tempHit, fireDistance,Const.a.layerMaskPlayerAttack)) {
			tempHM = tempHit.collider.transform.gameObject.GetComponent<HealthManager>(); // Thanks andeeeeeee!!
			if (tempHM == null) tempHM = tempHit.transform.gameObject.GetComponent<HealthManager>();
			
            if (tempHM != null) {
                useBlood = true;
            }
            return true;
        }
        return false;
    }

	void CreateStandardImpactMarks(int wep16index) {
		// Don't create bullet holes on objects that move
		if (tempHit.collider.transform.gameObject == null) return;

		GameObject hitGO = tempHit.collider.transform.gameObject;
		if (hitGO.GetComponent<Rigidbody>() != null) return;
		if (hitGO.GetComponent<HealthManager>() != null) return; // don't create bullet holes on objects that die
		if (hitGO.GetComponent<Animator>() != null) return; // don't create bullet holes on objects that animate
		if (hitGO.GetComponent<Animation>() != null) return; // don't create bullet holes on objects that animate
		if (hitGO.GetComponent<Door>() != null) return; // don't create bullet holes on doors, makes them ghost and flicker through walls
		//Debug.Log("Generating standard impact marks");
		// Add bullethole
		tempVec = tempHit.normal * 0.16f;
		GameObject holetype = bulletHoleSmall;
		PoolType constHoleType = PoolType.BulletHoleTiny;
		switch(wep16index) {
			case 0: holetype = bulletHoleLarge;
					constHoleType = PoolType.BulletHoleLarge;
					break;
			case 1: holetype = bulletHoleScorchSmall;
					constHoleType = PoolType.BulletHoleScorchSmall;
					break;
			case 2: holetype = bulletHoleTiny;
					constHoleType = PoolType.BulletHoleTiny;
					break;
			case 3: holetype = bulletHoleSmall;
					constHoleType = PoolType.BulletHoleSmall;
					break;
			case 4: holetype = bulletHoleScorchLarge;
					constHoleType = PoolType.BulletHoleScorchLarge;
					break;
			case 5: holetype = bulletHoleScorchSmall;
					constHoleType = PoolType.BulletHoleScorchSmall;
					break;
			case 6: return; // no impact marks for lead pipe
			case 7: holetype = bulletHoleLarge;
					constHoleType = PoolType.BulletHoleLarge;
					break;
			case 8: holetype = bulletHoleScorchLarge;
					constHoleType = PoolType.BulletHoleScorchLarge;
					break;
			case 9: holetype = bulletHoleSmall;
					constHoleType = PoolType.BulletHoleSmall;
					break;
			case 10: holetype = bulletHoleScorchLarge;
					constHoleType = PoolType.BulletHoleScorchLarge;
					break;
			case 11: holetype = bulletHoleScorchLarge;
					constHoleType = PoolType.BulletHoleScorchLarge;
					break;
			case 12: holetype = bulletHoleSpread;
					constHoleType = PoolType.BulletHoleTinySpread;
					break;
			case 13: holetype = bulletHoleLarge;
					constHoleType = PoolType.BulletHoleLarge;
					break;
			case 14: holetype = bulletHoleScorchSmall;
					constHoleType = PoolType.BulletHoleScorchSmall;
					break;
			case 15: holetype = bulletHoleScorchSmall;
					constHoleType = PoolType.BulletHoleScorchSmall;
					break;
		}

		if (holetype != null) {
			GameObject impactMark = Const.a.GetObjectFromPool(constHoleType);
			if (impactMark == null) {
				impactMark = (GameObject)Instantiate(holetype, (tempHit.point + tempVec),
													 Quaternion.LookRotation(tempHit.normal*-1,Vector3.up),
													 hitGO.transform);

				// No need to get SaveObject to set `instantiated` bit since the prefab already has it set as it is never not considered an instantiated object.
				if (impactMark == null) {
					Debug.Log("BUG: Couldn't find pool object or instantiate holetype for CreateStandardImpactMarks");
					return;
				}
			}
			int rint = Random.Range(0,3);
			Quaternion roll = impactMark.transform.localRotation;
			roll *= Quaternion.Euler(0f,0f,rint * 90f);
			impactMark.transform.localRotation = roll;
			GameObject dynamicObjectsContainer = LevelManager.a.GetCurrentDynamicContainer();
			if (dynamicObjectsContainer != null) impactMark.transform.parent = dynamicObjectsContainer.transform;
		}
	}

    void CreateStandardImpactEffects(bool onlyBloodIfHitHasHM) {
        // Determine blood type of hit target and spawn corresponding blood particle effect from the Const.Pool
        if (useBlood) {
            GameObject impact = Const.a.GetImpactType(tempHM);
            if (impact != null) {
                tempVec = tempHit.normal * hitOffset;
				impact.transform.SetPositionAndRotation(tempHit.point + tempVec,Quaternion.FromToRotation(Vector3.up, tempHit.normal));
                impact.SetActive(true);
            }
        } else {
            // Allow for skipping adding sparks after special override impact effects per attack functions below
            if (!onlyBloodIfHitHasHM) {
                GameObject impact = Const.a.GetObjectFromPool(PoolType.SparksSmall); //Didn't hit an object with a HealthManager script, use sparks
                if (impact != null) {
                    tempVec = tempHit.normal * hitOffset;
					impact.transform.SetPositionAndRotation(tempHit.point + tempVec,Quaternion.FromToRotation(Vector3.up, tempHit.normal));
                    impact.SetActive(true);
                }
            }
        }
    }

    void CreateBeamImpactEffects(int wep16index) {
        GameObject impact = Const.a.GetObjectFromPool(PoolType.SparqImpacts);
        if (wep16index == 1) {
            impact = Const.a.GetObjectFromPool(PoolType.BlasterImpacts);  //Red laser for blaster
        } else {
            if (wep16index == 4) {
                impact = Const.a.GetObjectFromPool(PoolType.IonImpacts); // Yellow laser for ion
            }
        }

        if (impact != null) {
            // impact.transform.position = tempHit.point;
            // impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, tempHit.normal);
			impact.transform.SetPositionAndRotation(tempHit.point,Quaternion.FromToRotation(Vector3.up, tempHit.normal));
            impact.SetActive(true);
        }
    }

    void CreateBeamEffects(int wep16index) {
        int laserIndex = 98; // Turquoise/Pale-Teal for sparq
        if (wep16index == 1) {
            laserIndex = 99;  //Red laser for blaster
        } else {
           if (wep16index == 4) {
               laserIndex = 100; // Yellow laser for ion
           }
        }

		GameObject dynamicObjectsContainer = LevelManager.a.GetCurrentDynamicContainer();
		if (dynamicObjectsContainer == null) return; //didn't find current level
		GameObject lasertracer = Instantiate(Const.a.useableItems[laserIndex],transform.position,Const.a.quaternionIdentity) as GameObject;
		// Temporary object only, no need to save or mark as instantiated.
		if (lasertracer != null) {
			lasertracer.transform.SetParent(dynamicObjectsContainer.transform,true);
			tempVec = transform.position;
			tempVec.y += verticalOffset;
			lasertracer.GetComponent<LaserDrawing>().startPoint = tempVec;
			lasertracer.GetComponent<LaserDrawing>().endPoint = tempHit.point;
			lasertracer.SetActive(true);
		}
    }

	// dmg_min is Const.a.damagePerHitForWeapon[wep16Index], dmg_max is Const.a.damagePerHitForWeapon2[wep16Index]
	float DamageForPower(int wep16Index) {
	    float retval, dmg_min, dmg_max, ener_min, ener_max;

        // overload overrides current setting and uses overload damage
        if (overloadEnabled) {
            retval = Const.a.damageOverloadForWeapon[wep16Index];
            return retval;
        }

		dmg_min = Const.a.damagePerHitForWeapon[wep16Index];
		dmg_max = Const.a.damagePerHitForWeapon2[wep16Index];
        ener_min = Const.a.energyDrainLowForWeapon[wep16Index];
        ener_max = Const.a.energyDrainHiForWeapon[wep16Index];
		// Calculates damage based on min and max values and applies a curve of the slopes based on the linear plotting of the slope from min at min to max at max...that makes sense right?
		// Right then, the beautifully ugly formula:
		retval = ((WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent]/100f)*((dmg_max/ener_max)-(dmg_min/ener_min)) + 3f) * (((WeaponCurrent.a.weaponEnergySetting[WeaponCurrent.a.weaponCurrent])/100f)*(ener_max-ener_min) + ener_min);
		//Debug.Log("returning DamageForPower of " + retval.ToString() + ", for wep16Index of " + wep16Index.ToString());
		return retval;
		// You gotta love maths!  There is a spreadsheet for this (.ods LibreOffice file format, found with src code) that shows the calculations to make this dmg curve. 
	}

	// TargetID Instance
	public void CreateTargetIDInstance(float dmgFinal, HealthManager hm) {
		if (hm == null) return;
		if (!hm.isNPC) return;
		if (hm.health <= 0f) return;
		if (dmgFinal > 0 && !Inventory.a.hasHardware[4] && dmgFinal != -2f) {
			return;
		}

		float linkDistForTargID = TargetID.GetTargetIDTetherRange();
		bool showHealth = false;
		bool showRange = false;
		bool showAttitude = false;
		bool showName = false;
		if (Inventory.a.hasHardware[4]) {
			showRange = true;
			if (Inventory.a.hardwareVersion[4] > 1) {
				showAttitude = true;
				showName = true;
			}

			if (Inventory.a.hardwareVersion[4] > 2) showHealth = true;
		}

		string damageText = "";
		if (Inventory.a.hasHardware[4]) {
			if (Inventory.a.hardwareVersion[4] > 1) {
				if (dmgFinal > (hm.maxhealth * 0.75f)) {
					damageText = Const.a.stringTable[514]; // SEVERE DAMAGE
				} else if (dmgFinal > (hm.maxhealth * 0.50f)) {
					damageText = Const.a.stringTable[515]; // MAJOR DAMAGE
				} else if (dmgFinal > (hm.maxhealth * 0.25f)) {
					damageText = Const.a.stringTable[513]; // NORMAL DAMAGE
				} else if (dmgFinal > 0f) {
					damageText = Const.a.stringTable[512]; // MINOR DAMAGE
				}
			} else {
				if (dmgFinal > 0f) {
					damageText = Const.a.stringTable[596]; // DAMAGED				
				}
			}
		}


        GameObject idFrame = Const.a.GetObjectFromPool(PoolType.TargetIDInstances);
        if (idFrame == null) return;

		TargetID tid = idFrame.GetComponent<TargetID>();
		if (tid == null) return;

		if (dmgFinal == -2f) damageText = Const.a.stringTable[536]; // STUNNED

		// Even when TargetID hardware not acquired, still show no damage/tranq
		// to show player that hey, it no workie.  No hasHardware[4] check.
		if (dmgFinal == 0f) {
 			damageText = Const.a.stringTable[511]; // NO DAMAGE
			noDamageIndicator = idFrame;
			tid.lifetime = 1f;
			tid.lifetimeFinished = PauseScript.a.relativeTime + tid.lifetime;
		} else {
			tid.damageTime = 1f;
			tid.lifetime = 9999999f;
			tid.lifetimeFinished = PauseScript.a.relativeTime + tid.lifetime;
		}

		tid.currentText = damageText;
		tid.parent = hm.transform;

		// Center on what we just shot
		Vector3 adjustment = hm.transform.position;
		if (hm.aic != null) {
			if (hm.aic.index == 14) {
				// Adjust position for hopper origin since it's special melty.
				adjustment.y += 1f;
			}
		}

		idFrame.transform.position = adjustment;
		idFrame.SetActive(true);
		tid.linkedHM = hm;
		hm.linkedTargetID = tid;
		tid.partSys.Play();
		tid.playerCapsuleTransform = playerCapsule.transform;
		tid.playerLinkDistance = linkDistForTargID;
		tid.displayRange = showRange;
		tid.displayHealth = showHealth;
		tid.displayAttitude = showAttitude;
		tid.displayName = showName;
		tid.linkedHM.aic.hasTargetIDAttached = true;
	}

    // WEAPON FIRING CODE:
    // ==============================================================================================================================
    // Hitscan Weapons
    //----------------------------------------------------------------------------------------------------------
    // Guns and laser beams, used by most weapons
    void HitScanFire(int wep16Index) {
		//damageData.ResetDamageData(damageData);
        if (wep16Index == 1 || wep16Index == 4 || wep16Index == 14) {
            CreateBeamImpactEffects(wep16Index); // laser burst effect overrides standard blood spurts/robot sparks
        } else {
            CreateStandardImpactEffects(false); // standard blood spurts/robot sparks

			// the only exception
			if (wep16Index == 2 && Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent]) damageData.attackType = AttackType.Tranq; // tranquilize the untranquil....yes
        }

        // Fill the damageData container
		// -------------------------------
		// Using tempHit.transform instead of tempHit.collider.transform to ensure we get overall NPC parent instead of its children.
        damageData.other = tempHit.transform.gameObject;
		HealthManager hm = damageData.other.GetComponent<HealthManager>();
        if (damageData.other.CompareTag("NPC")) {
            damageData.isOtherNPC = true;
			if (damageData.attackType == AttackType.Tranq) {
				// Using tempHit.transform instead of tempHit.collider.transform to ensure we get overall NPC parent instead of its children.
				AIController taic = damageData.other.GetComponent<AIController>();
				if (taic !=null) taic.Tranquilize();
			}
        } else {
            damageData.isOtherNPC = false;
			if (damageData.other.CompareTag("Geometry")) {
				CreateStandardImpactMarks(wep16Index);
			}
        }
        damageData.hit = tempHit;
		damageData.attacknormal = MouseCursor.a.GetCursorScreenPointForRay();
        damageData.attacknormal = playerCamera.ScreenPointToRay(damageData.attacknormal).direction;
        if (Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent]) {
            damageData.damage = Const.a.damagePerHitForWeapon2[wep16Index];
			damageData.offense = Const.a.offenseForWeapon2[wep16Index];
			damageData.penetration = Const.a.penetrationForWeapon2[wep16Index];
        } else {
			if (CurrentWeaponUsesEnergy()) {
                damageData.damage = DamageForPower(wep16Index);
			} else {
				damageData.damage = Const.a.damagePerHitForWeapon[wep16Index];
			}
			damageData.offense = Const.a.offenseForWeapon[wep16Index];
			damageData.penetration = Const.a.penetrationForWeapon[wep16Index];
        }
		damageData.attackType = Const.a.attackTypeForWeapon[wep16Index];
        damageData.damage = DamageData.GetDamageTakeAmount(damageData);
        damageData.owner = playerCapsule;
		damageData.impactVelocity = 1f;
		if (wep16Index == 12) {
			damageData.impactVelocity = 300f;
			if (hm != null) {
				if (hm.isObject) damageData.damage *= 10f; // babamm boxes be like, u ded
			}
		}

		float dmgFinal = 0f;
		bool dartTranq = (wep16Index == 2
		  && Inventory.a.wepLoadedWithAlternate[WeaponCurrent.a.weaponCurrent]);
		GameObject hitGO = tempHit.collider.transform.gameObject;
        if (hm != null && hm.health > 0 && !dartTranq) {
			dmgFinal = hm.TakeDamage(damageData); // send the damageData container to HealthManager of hit object and apply damage
			damageData.impactVelocity += (damageData.damage * 0.5f);
			if (!damageData.isOtherNPC || wep16Index == 12) {
				Utils.ApplyImpactForce(hitGO,damageData.impactVelocity,
									   damageData.attacknormal,
									   damageData.hit.point);
			}
			if (hm.isNPC) Music.a.inCombat = true;
		}

		if (dmgFinal < 0f) dmgFinal = 0f; // Less would = blank.
		if (dartTranq) dmgFinal = -2f;
		if (damageData.attackType == AttackType.Tranq) dmgFinal = -2f;
		CreateTargetIDInstance(dmgFinal,hm);

		UseableObjectUse uou = hitGO.GetComponent<UseableObjectUse>();
		if (uou != null) uou.HitForce(damageData); // knock objects around

        // Draw a laser beam for beam weapons
        if (wep16Index == 1 || wep16Index == 4 || wep16Index == 14) {
			CreateBeamEffects(wep16Index);
		}
    }

    // Melee weapons
    //-------------------------------------------------------------------------
    // Rapier and pipe.  Need extra code to handle anims for view model and
	// sound for swing-and-a-miss! vs. hit
	IEnumerator ApplyMeleeHit(int index16, GameObject targ, bool isRapier, 
							  bool silent,AudioClip hit, AudioClip miss,
							  AudioClip hitflesh) {
		if (targ.layer == gameObject.layer) yield break;

		if (isRapier) yield return new WaitForSeconds(0.28f);
		else yield return new WaitForSeconds(0.15f);

		damageData.other = targ;
		damageData.isOtherNPC = false;
		if (targ.CompareTag("NPC")) damageData.isOtherNPC = true;
		damageData.attacknormal = MouseCursor.a.GetCursorScreenPointForRay();
		damageData.attacknormal =
			playerCamera.ScreenPointToRay(damageData.attacknormal).direction;
		damageData.damage = Const.a.damagePerHitForWeapon[index16]; 
		damageData.damage = DamageData.GetDamageTakeAmount(damageData);
		damageData.offense = Const.a.offenseForWeapon[index16];
		damageData.penetration = Const.a.penetrationForWeapon[index16];
		damageData.owner = playerCapsule;
		if (isRapier) {
			damageData.attackType = AttackType.MeleeEnergy;
			if (PlayerEnergy.a.energy < 4f) {
				// Half pipe
				damageData.damage = Const.a.damagePerHitForWeapon[6] / 2f;
			}
		} else {
			damageData.attackType = AttackType.Melee;
		}

		UseableObjectUse uou = targ.GetComponent<UseableObjectUse>();
		if (uou != null) uou.HitForce(damageData); // knock objects around
		HealthManager hm = targ.GetComponent<HealthManager>();
		if (hm == null) {
			if (!silent) {
				Utils.PlayOneShotSavable(SFX,hit);
				PlayerHealth.a.makingNoise = true;
				PlayerHealth.a.noiseFinished = PauseScript.a.relativeTime+0.5f;
			}
			yield break;
		}

		damageData.impactVelocity = damageData.damage * 1.5f;
		if (!damageData.isOtherNPC || index16 == 12) {
			if (!isRapier || (isRapier && PlayerEnergy.a.energy >= 4f)) {
			Utils.ApplyImpactForce(targ, damageData.impactVelocity,
								   damageData.attacknormal,
								   damageData.hit.point);
			}
		}

		float dmgFinal = hm.TakeDamage(damageData);
		if (dmgFinal < 0f) dmgFinal = 0f; // Less would = blank.
		CreateTargetIDInstance(dmgFinal,hm);
		if (hm.isNPC) Music.a.inCombat = true;
		if (!silent) {
			PlayerHealth.a.makingNoise = true;
			PlayerHealth.a.noiseFinished = PauseScript.a.relativeTime + 0.5f;
			if ((hm.bloodType == BloodType.Red)
				|| (hm.bloodType == BloodType.Yellow)
				|| (hm.bloodType == BloodType.Green)) {
				Utils.PlayOneShotSavable(SFX,hitflesh);
			} else if (isRapier && PlayerEnergy.a.energy < 4f) {
				Utils.PlayOneShotSavable(SFX,Const.a.sounds[67]);
			} else {
				Utils.PlayOneShotSavable(SFX,hit);
			}
		}

		CreateStandardImpactEffects(true);
		if (isRapier) {
			PlayerEnergy.a.TakeEnergy(3.666f); // 3 hits per tick.
			if (BiomonitorGraphSystem.a != null) {
				BiomonitorGraphSystem.a.EnergyPulse(3.666f);
			}
		}
	}

	// These are a bit silly.
    void FireRapier(int i16, bool sil) {
		FireMelee(i16,true,sil,SFXRapierHit,SFXRapierMiss,SFXRapierHit,true);
	}

    void FirePipe(int i16, bool sil) {
		FireMelee(i16,false,sil,SFXPipeHit,SFXPipeMiss,SFXPipeHitFlesh,false);
	}

	void FireMelee(int index16, bool isRapier, bool silent, AudioClip hit,
				   AudioClip miss,AudioClip hitflesh, bool rapier) {
		// Do normal straightline raytrace at center first.
		fireDistance = meleescanDistance;
		if (DidRayHit(index16)) {
			fireDistance = hitscanDistance; // Reset before any returns.
			if (rapier) {
				if (rapieranim != null) rapieranim.Play("Attack2");
			} else {
				if (anim != null) anim.Play("Attack2");
			}

			GameObject hitGO = tempHit.collider.transform.gameObject;
			StartCoroutine(ApplyMeleeHit(index16,hitGO,isRapier,silent,hit,
										 miss,hitflesh));
			return;
		}

		fireDistance = hitscanDistance; // Reset since raycast failed.

		// Check all objects we can hurt have HealthManager, that they are in
		// meleescanDistance range, that they are within player facing angle by
		// 60° (±30°)	
		for (int i=0;i<Const.a.healthObjectsRegistration.Length;i++) {
			if (Const.a.healthObjectsRegistration[i] == null) continue;

			GameObject ho = Const.a.healthObjectsRegistration[i].gameObject;
			// Don't hurt deactive objects, like you know, corpse on
			// living entities...at least don't do it again please.
			if (ho == null) continue;
			if (!ho.activeInHierarchy) continue;

			HealthManager hm = ho.GetComponent<HealthManager>();
			if (hm == null) continue;
			if (Vector3.Distance(ho.transform.position,
								 playerCapsule.transform.position)
				>= meleescanDistance) {
				continue;
			}

			MouseLookScript.a.SetCameraFocusPoint();
			tempVec = MouseLookScript.a.cameraFocusPoint
						- playerCamera.transform.position;

			tempVec = tempVec.normalized;
			Vector3 ang = ho.transform.position
							- playerCamera.transform.position;

			ang = ang.normalized;
			float dot = Vector3.Dot(tempVec,ang);
			if (dot <= 0.666f) continue;

			if (rapier) {
				if (rapieranim != null) rapieranim.Play("Attack2");
			} else {
				if (anim != null) anim.Play("Attack2");
			}

			StartCoroutine(ApplyMeleeHit(index16,ho,isRapier,silent,hit,
											miss,hitflesh));
			return;
		}

		// Swing and a miss, steeeerike!!
		if (!silent) Utils.PlayOneShotSavable(SFX,miss);
		if (rapier) {
			if (rapieranim != null) rapieranim.Play("Attack2");
		} else {
			if (anim != null) anim.Play("Attack1");
		}
	}

    // Projectile weapons
    //-------------------------------------------------------------------------
    void FirePlasma(int index16) { FireBeachball(index16,plasmaShotForce,485); }
    void FireRailgun(int index16) { FireBeachball(index16,railgunShotForce,484); }
    void FireMagpulse(int index16) { FireBeachball(index16,magpulseShotForce,482); }
    void FireStungun(int index16) { FireBeachball(index16,stungunShotForce,483); }

	void FireBeachball(int index16, float shoveForce, int prefabID) {
        // Create and hurl a beachball-like object.  On the developer
		// commentary they said that the projectiles act like a beachball for
		// collisions with enemies, but act like a baseball for walls/floor to
		// prevent hitting corners.
        GameObject beachball = ConsoleEmulator.SpawnDynamicObject(prefabID,1);
        if (beachball != null) {
			if (CurrentWeaponUsesEnergy()) {
                damageData.damage = DamageForPower(index16);
			} else {
				damageData.damage = Const.a.damagePerHitForWeapon[index16];
			}
            damageData.owner = playerCapsule;
            damageData.attackType = Const.a.attackTypeForWeapon[index16];
			damageData.offense = Const.a.offenseForWeapon[index16];
			damageData.penetration = Const.a.penetrationForWeapon[index16];
            beachball.GetComponent<ProjectileEffectImpact>().dd = damageData;
            beachball.GetComponent<ProjectileEffectImpact>().host = playerCapsule;
            beachball.transform.position = playerCamera.transform.position;
			MouseLookScript.a.SetCameraFocusPoint();
            tempVec = MouseLookScript.a.cameraFocusPoint - playerCamera.transform.position;
            beachball.transform.forward = tempVec.normalized;
            beachball.SetActive(true);
            Vector3 shove = beachball.transform.forward * shoveForce;

			// Force starting with zero pior to adding impulse force.
            beachball.GetComponent<Rigidbody>().velocity = Const.a.vectorZero;
            beachball.GetComponent<Rigidbody>().AddForce(shove,ForceMode.Impulse);
        }
	}

    public Vector3 ScreenPointToDirectionVector() {
        Vector3 retval = Const.a.vectorZero;
        retval = playerCamera.transform.forward;
        return retval;
    }

	public static string Save(GameObject go) {
		WeaponFire wf = go.GetComponent<WeaponFire>();
		if (wf == null) {
			Debug.Log("WeaponFire missing on Player!  GameObject.name: " + go.name);
			return "0000.00000|0|0000.00000|0000.00000|0000.00000|0000.00000|0000.00000|0|0000.00000|0000.00000|0000.00000";
		}

		string line = System.String.Empty;
		line = Utils.SaveRelativeTimeDifferential(wf.waitTilNextFire,"waitTilNextFire");
		line += Utils.splitChar + Utils.BoolToString(wf.overloadEnabled,"overloadEnabled");
		line += Utils.splitChar + Utils.FloatToString(wf.sparqSetting,"sparqSetting");
		line += Utils.splitChar + Utils.FloatToString(wf.ionSetting,"ionSetting");
		line += Utils.splitChar + Utils.FloatToString(wf.blasterSetting,"blasterSetting");
		line += Utils.splitChar + Utils.FloatToString(wf.plasmaSetting,"plasmaSetting");
		line += Utils.splitChar + Utils.FloatToString(wf.stungunSetting,"stungunSetting");
		line += Utils.splitChar + Utils.BoolToString(wf.recoiling,"recoiling");
		line += Utils.splitChar + Utils.FloatToString(wf.reloadLerpValue,"reloadLerpValue");
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(wf.reloadFinished,"reloadFinished");
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(wf.lerpStartTime,"lerpStartTime");
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(wf.justFired,"justFired");
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(wf.energySliderClickedTime,"energySliderClickedTime");
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(wf.cyberWeaponAttackFinished,"cyberWeaponAttackFinished");
		line += Utils.splitChar + Utils.BoolToString(wf.gunCamera.enabled,"gunCamera");
		line += Utils.splitChar + BerserkEffect.Save(go);
		line += Utils.splitChar + Utils.SaveCamera(go); // Grayscale saved here
		line += Utils.splitChar + Utils.SaveTransform(wf.reloadContainer.transform);
		line += Utils.splitChar + Utils.FloatToString(wf.targetY,"targetY");
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		WeaponFire wf = go.GetComponent<WeaponFire>();
		if (wf == null) {
			Debug.Log("WeaponFire.Load failure, wf == null");
			return index + 11;
		}

		if (index < 0) {
			Debug.Log("WeaponFire.Load failure, index < 0");
			return index + 11;
		}

		if (entries == null) {
			Debug.Log("WeaponFire.Load failure, entries == null");
			return index + 11;
		}

		wf.waitTilNextFire = Utils.LoadRelativeTimeDifferential(entries[index],"waitTilNextFire"); index++;
		wf.overloadEnabled = Utils.GetBoolFromString(entries[index],"overloadEnabled"); index++;
		wf.sparqSetting = Utils.GetFloatFromString(entries[index],"sparqSetting"); index++;
		wf.ionSetting = Utils.GetFloatFromString(entries[index],"ionSetting"); index++;
		wf.blasterSetting = Utils.GetFloatFromString(entries[index],"blasterSetting"); index++;
		wf.plasmaSetting = Utils.GetFloatFromString(entries[index],"plasmaSetting"); index++;
		wf.stungunSetting = Utils.GetFloatFromString(entries[index],"stungunSetting"); index++;
		wf.recoiling = Utils.GetBoolFromString(entries[index],"recoiling"); index++;
		wf.reloadLerpValue = Utils.GetFloatFromString(entries[index],"reloadLerpValue"); index++;
		wf.reloadFinished = Utils.LoadRelativeTimeDifferential(entries[index],"reloadFinished"); index++;
		wf.lerpStartTime = Utils.LoadRelativeTimeDifferential(entries[index],"lerpStartTime"); index++;
		wf.justFired = Utils.LoadRelativeTimeDifferential(entries[index],"justFired"); index++;
		wf.energySliderClickedTime = Utils.LoadRelativeTimeDifferential(entries[index],"energySliderClickedTime"); index++;
		wf.cyberWeaponAttackFinished = Utils.LoadRelativeTimeDifferential(entries[index],"cyberWeaponAttackFinished"); index++;
		wf.gunCamera.enabled = Utils.GetBoolFromString(entries[index],"gunCamera"); index++;
		index = BerserkEffect.Load(go,ref entries,index);
		index = Utils.LoadCamera(go,ref entries,index); // Grayscale loaded here
		index = Utils.LoadTransform(wf.reloadContainer.transform,ref entries,index);
		wf.targetY = Utils.GetFloatFromString(entries[index],"targetY"); index++;
		return index;
	}
}
