using UnityEngine;
using System.Collections;

public class WeaponFire : MonoBehaviour {
    [HideInInspector]
    public float waitTilNextFire = 0f;
    public float muzzleDistance = 0.10f;
    public float hitOffset = 0.2f;
    public float normalHitOffset = 0.2f;
    public float verticalOffset = -0.2f; // For laser beams
    public float fireDistance = 200f;
    public float hitscanDistance = 200f;
    public float meleescanDistance = 2.5f;
	public float overheatedPercent = 80f;
    public bool isAlternateAmmo = false;
    public bool berserkActive = false;
    public float magpulseShotForce = 2.20f;
    public float stungunShotForce = 1.92f;
    public float railgunShotForce = 2.60f;
    public float plasmaShotForce = 1.50f;
    [HideInInspector]
    public float[] args;
    public GameObject bullet;
    public GameObject impactEffect;
	public GameObject noDamageIndicator;
    public WeaponMagazineCounter wepmagCounter;
    public Camera playerCamera; // assign in the editor
	private MouseLookScript mls;
	public PlayerMovement playerMovement; // assign in editor
    public Camera gunCamera; // assign in the editor
    public PlayerEnergy curEnergy;
    public GameObject playerCapsule;
    public WeaponCurrent currentWeapon; // assign in the editor
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
    //public AudioClip SFXBlasterOverFire; // assign in the editor
    public AudioClip SFXDartFire; // assign in the editor
    public AudioClip SFXFlechetteFire; // assign in the editor
    public AudioClip SFXIonFire; // assign in the editor
    //public AudioClip SFXIonOverFire; // assign in the editor
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
                                                         //[SerializeField] private AudioClip SFXSparqBeamOverFire; // assign in the editor
    public AudioClip SFXStungunFire; // assign in the editor
    public AudioClip SFXEmpty; // assign in the editor
    public AudioClip SFXRicochet; // assign in the editor

    public bool overloadEnabled;
    public float sparqHeat;
    public float ionHeat;
    public float blasterHeat;
    public float stungunHeat;
    public float plasmaHeat;
    public float sparqSetting;
    public float ionSetting;
    public float blasterSetting;
    public float plasmaSetting;
    public float stungunSetting;
    private float clipEnd;
    public Animator anim; // assign in the editor
	public Animator rapieranim; // assign in the editor
    public DamageData damageData;
    private RaycastHit tempHit;
    private Vector3 tempVec;
    private bool useBlood;
    private HealthManager tempHM;
    private float retval;
    private float heatTickFinished;
    private float heatTickTime = 0.50f;

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

	// Recoil the weapon view models
	public GameObject wepView;
	private Vector3 wepViewDefaultLocalPos;
	[SerializeField] private bool recoiling;
	[HideInInspector]
	public float justFired;
	public float energySliderClickedTime;

    void Awake() {
        damageData = new DamageData();
        tempHit = new RaycastHit();
        tempVec = new Vector3(0f, 0f, 0f);
        heatTickFinished = Time.time + heatTickTime;
		wepViewDefaultLocalPos = wepView.transform.localPosition;
		justFired = (Time.time - 31f); // set less than 30s before Time.time to guarantee we don't immediately play action music
		energySliderClickedTime = Time.time;
		mls = playerCamera.GetComponent<MouseLookScript>();
    }

    void GetWeaponData(int index) {
        if (index == -1) return;
        damageData.isFullAuto = Const.a.isFullAutoForWeapon[index];
        if (currentWeapon.weaponIsAlternateAmmo) {
            damageData.damage = Const.a.damagePerHitForWeapon2[index];
            damageData.delayBetweenShots = Const.a.delayBetweenShotsForWeapon2[index];
            damageData.penetration = Const.a.penetrationForWeapon2[index];
            damageData.offense = Const.a.offenseForWeapon2[index];
        } else {
            damageData.damage = Const.a.damagePerHitForWeapon[index];
            damageData.delayBetweenShots = Const.a.delayBetweenShotsForWeapon[index];
            damageData.penetration = Const.a.penetrationForWeapon[index];
            damageData.offense = Const.a.offenseForWeapon[index];
        }
        damageData.damageOverload = Const.a.damageOverloadForWeapon[index];
        damageData.energyDrainLow = Const.a.energyDrainLowForWeapon[index];
        damageData.energyDrainHi = Const.a.energyDrainHiForWeapon[index];
        damageData.energyDrainOver = Const.a.energyDrainOverloadForWeapon[index];
        damageData.range = Const.a.rangeForWeapon[index];
        damageData.attackType = Const.a.attackTypeForWeapon[index];
        damageData.berserkActive = berserkActive;
    }

    public static int Get16WeaponIndexFromConstIndex(int index) {
        int i = -1;
        switch (index) {
            case 36:
                //Mark3 Assault Rifle
                i = 0; break;
            case 37:
                //ER-90 Blaster
                i = 1; break;
            case 38:
                //SV-23 Dartgun
                i = 2; break;
            case 39:
                //AM-27 Flechette
                i = 3; break;
            case 40:
                //RW-45 Ion Beam
                i = 4; break;
            case 41:
                //TS-04 Laser Rapier
                i = 5; break;
            case 42:
                //Lead Pipe
                i = 6; break;
            case 43:
                //Magnum 2100
                i = 7; break;
            case 44:
                //SB-20 Magpulse
                i = 8; break;
            case 45:
                //ML-41 Pistol
                i = 9; break;
            case 46:
                //LG-XX Plasma Rifle
                i = 10; break;
            case 47:
                //MM-76 Railgun
                i = 11; break;
            case 48:
                //DC-05 Riotgun
                i = 12; break;
            case 49:
                //RF-07 Skorpion
                i = 13; break;
            case 50:
                //Sparq Beam
                i = 14; break;
            case 51:
                //DH-07 Stungun
                i = 15; break;
        }
        return i;
    }

    bool CurrentWeaponUsesEnergy () {
        if (currentWeapon.weaponIndex == 37 || currentWeapon.weaponIndex == 40 || currentWeapon.weaponIndex == 46 || currentWeapon.weaponIndex == 50 || currentWeapon.weaponIndex == 51)
            return true;

        return false;
    }

    bool WeaponsHaveAnyHeat() {
		if (currentWeapon.redbull) return false;
        if (ionHeat > 0) return true;
        if (plasmaHeat > 0) return true;
        if (sparqHeat > 0) return true;
        if (stungunHeat > 0) return true;
        if (blasterHeat > 0) return true;
        return false;
    }

    void HeatBleedOff() {
        if (heatTickFinished < Time.time) {
            ionHeat -= 10f;  if (ionHeat < 0) ionHeat = 0;
            blasterHeat -= 10f; if (blasterHeat < 0) blasterHeat = 0;
            sparqHeat -= 10f; if (sparqHeat < 0) sparqHeat = 0;
            stungunHeat -= 10f; if (stungunHeat < 0) stungunHeat = 0;
            plasmaHeat -= 10f; if (plasmaHeat < 0) plasmaHeat = 0;
            if (CurrentWeaponUsesEnergy())
                energheatMgr.HeatBleed(GetHeatForCurrentWeapon()); // update hud heat ticks if current weapon uses energy

            heatTickFinished = Time.time + heatTickTime;
        }
    }

	public void Recoil (int i) {
		float strength = Const.a.recoilForWeapon[i];
		//Debug.Log("Recoil from gun index: "+i.ToString()+" with strength of " +strength.ToString());
		if (strength <= 0f) return;
		if (playerMovement.fatigue > 80) strength = strength * 2f;
		strength = strength * 0.25f;
		Vector3 wepJoltPosition = new Vector3(wepView.transform.localPosition.x - (strength * 0.5f * Random.Range(-1f,1f)), wepView.transform.localPosition.y, (wepViewDefaultLocalPos.z - strength));
		wepView.transform.localPosition = wepJoltPosition;
		recoiling = true;
	}

    void Update() {
        if (!PauseScript.a.Paused()) {
            if (WeaponsHaveAnyHeat()) HeatBleedOff(); // Slowly cool off any weapons that have been heated from firing
			int i = Get16WeaponIndexFromConstIndex(currentWeapon.weaponIndex);

			if (currentWeapon.reloadFinished > Time.time) {
				currentWeapon.reloadLerpValue = ((Time.time - currentWeapon.lerpStartTime)/Const.a.reloadTime[i]); // percent towards goal time total (both halves of the action)
				//Debug.Log("reloadLerpValue initial: " + currentWeapon.reloadLerpValue.ToString());
				if (currentWeapon.reloadLerpValue >= 0.5) {
					//currentWeapon.targetY = (currentWeapon.reloadContainerDropAmount - currentWeapon.reloadContainerOrigin.y);
					currentWeapon.reloadLerpValue = (0.5f- (1 - currentWeapon.reloadLerpValue))/0.5f; // percentage of this half of the trip
					currentWeapon.targetY = (-1 * currentWeapon.reloadContainerDropAmount * (1 - currentWeapon.reloadLerpValue));
				} else {
					currentWeapon.targetY = (currentWeapon.reloadContainerOrigin.y - currentWeapon.reloadContainerDropAmount);
					currentWeapon.reloadLerpValue = currentWeapon.reloadLerpValue/0.5f; // percentage of this half of the trip
					currentWeapon.targetY = (currentWeapon.targetY * currentWeapon.reloadLerpValue);
				}
				//Debug.Log("reloadLerpValue splithalf: " + currentWeapon.reloadLerpValue.ToString());
				//Debug.Log("Setting Y to: " + (currentWeapon.targetY * currentWeapon.reloadLerpValue).ToString());
				wepView.transform.localPosition = new Vector3(wepView.transform.localPosition.x,currentWeapon.targetY,wepView.transform.localPosition.z);
			}

			if (recoiling) {
				float x = wepView.transform.localPosition.x; // side to side
				float z = wepView.transform.localPosition.z; // forward and back
				z = Mathf.Lerp(z,wepViewDefaultLocalPos.z,Time.deltaTime);
				x = Mathf.Lerp(x,wepViewDefaultLocalPos.x,Time.deltaTime);
				wepView.transform.localPosition = new Vector3(x,wepView.transform.localPosition.y,z);
			}

            if (!GUIState.a.isBlocking && !playerCamera.GetComponent<MouseLookScript>().holdingObject) {
                if (i == -1) return;

                GetWeaponData(i);
                if (GetInput.a.Attack(Const.a.isFullAutoForWeapon[i]) && waitTilNextFire < Time.time && (Time.time - energySliderClickedTime) > 0.1f && currentWeapon.reloadFinished < Time.time) {
					justFired = Time.time; // set justFired so that Music.cs can see it and play corresponding music in a little bit from now or keep playing action music
                    // Check weapon type and check ammo before firing
                    if (i == 5 || i == 6) {
                        // Pipe or Laser Rapier, attack without prejudice
                        FireWeapon(i, false); // weapon index, isSilent == false so play normal SFX
                    } else {
                        // Energy weapons so check energy level
                        if (i == 1 || i == 4 || i == 10 || i == 14 || i == 15) {
                            // Even if we have only 1 energy, we still fire with all we've got up to the energy level setting of course
                            if (curEnergy.energy > 0 || currentWeapon.bottomless || currentWeapon.redbull) {
								if (GetHeatForCurrentWeapon() > overheatedPercent && !currentWeapon.bottomless && !currentWeapon.redbull) {
									if (SFXEmpty != null) SFX.PlayOneShot(SFXEmpty);
                                    waitTilNextFire = Time.time + 0.8f;
                                    Const.sprint(Const.a.stringTable[11],Const.a.allPlayers);
								} else {
									FireWeapon(i, false); // weapon index, isSilent == false so play normal SFX
								}
							} else {
									Const.sprint(Const.a.stringTable[207],Const.a.allPlayers);
							}
                        } else {
                            // Uses normal ammo, check versus alternate or normal to see if we have ammo then fire
                            if (currentWeapon.weaponIsAlternateAmmo) {
                                if (WeaponCurrent.WepInstance.currentMagazineAmount2[i] > 0 || currentWeapon.bottomless) {
                                    FireWeapon(i, false); // weapon index, isSilent == false so play normal SFX
                                } else {
                                    if (SFXEmpty != null) SFX.PlayOneShot(SFXEmpty);
                                    waitTilNextFire = Time.time + 0.8f;
                                }
                            } else {
                                if (WeaponCurrent.WepInstance.currentMagazineAmount[i] > 0 || currentWeapon.bottomless) {
                                    FireWeapon(i, false); // weapon index, isSilent == false so play normal SFX
                                } else {
                                    if (SFXEmpty != null) SFX.PlayOneShot(SFXEmpty);
                                    waitTilNextFire = Time.time + 0.8f;
                                }
                            }
                        }
                    }
                }

                if (GetInput.a.Reload() && currentWeapon.reloadFinished < Time.time) {
                    WeaponCurrent.WepInstance.Reload();
                }
            }
        }
    }

    // index is used to get recoil down at the bottom and pass along ref for damageData,
    // otherwise the cases use currentWeapon.weaponIndex
    void FireWeapon(int index, bool isSilent) {
        damageData.ResetDamageData(damageData);
		if (currentWeapon.weaponIndex != 41 && currentWeapon.weaponIndex != 42) {
			for (int ij=0;ij<Const.a.healthObjectsRegistration.Length;ij++) {
				if (Const.a.healthObjectsRegistration[ij] != null) {
					if (Const.a.healthObjectsRegistration[ij].isNPC) {
						if (Vector3.Distance(Const.a.healthObjectsRegistration[ij].gameObject.transform.position,playerCapsule.transform.position) < Const.a.healthObjectsRegistration[ij].aic.rangeToHear) {
							Const.a.healthObjectsRegistration[ij].NotifyEnemyNearby(playerCapsule);
						}
					}
				}
			}
		}

        switch (currentWeapon.weaponIndex) {
            case 36:
                //Mark3 Assault Rifle
                if (!isSilent) { SFX.clip = SFXMark3Fire; SFX.Play(); }
                if (DidRayHit()) HitScanFire(index);
				muzFlashMK3.SetActive(true);
                break;
            case 37:
                //ER-90 Blaster
				blasterSetting = currentWeapon.weaponEnergySetting[1];
				if (!isSilent) { SFX.clip = SFXBlasterFire; SFX.Play(); }
				if (DidRayHit()) HitScanFire(index);
				muzFlashBlaster.SetActive(true);
                if (overloadEnabled) {
                    blasterHeat = 100f;
                } else {
                    blasterHeat += blasterSetting;
                }
				if (blasterHeat > 100f) blasterHeat = 100f;
                break;
            case 38:
                //SV-23 Dartgun
                if (!isSilent) { SFX.clip = SFXDartFire; SFX.Play(); }
                if (DidRayHit()) HitScanFire(index);
				muzFlashDartgun.SetActive(true);
                break;
            case 39:
                //AM-27 Flechette
                if (!isSilent) { SFX.clip = SFXFlechetteFire; SFX.Play(); }
                if (DidRayHit()) HitScanFire(index);
				muzFlashFlechette.SetActive(true);
                break;
            case 40:
                //RW-45 Ion Beam
				ionSetting = currentWeapon.weaponEnergySetting[4];
                if (!isSilent) { SFX.clip = SFXIonFire; SFX.Play(); }
                if (DidRayHit()) HitScanFire(index);
				muzFlashIonBeam.SetActive(true);
                if (overloadEnabled) {
                    ionHeat = 100f;
                } else {
                    ionHeat += ionSetting;
                }
                if (ionHeat > 100f) ionHeat = 100f;
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
                if (DidRayHit()) HitScanFire(index);
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
                if (DidRayHit()) HitScanFire(index);
				muzFlashPistol.SetActive(true);
                break;
            case 46:
                //LG-XX Plasma Rifle
				plasmaSetting = currentWeapon.weaponEnergySetting[10];
                if (!isSilent) { SFX.clip = SFXPlasmaFire; SFX.Play(); }
                FirePlasma(index);
				muzFlashPlasma.SetActive(true);
                plasmaHeat += plasmaSetting;
                if (plasmaHeat > 100f) plasmaHeat = 100f;
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
                if (DidRayHit()) HitScanFire(index);
				muzFlashRiotgun.SetActive(true);
                break;
            case 49:
                //RF-07 Skorpion
                if (!isSilent) { SFX.clip = SFXSkorpionFire; SFX.Play(); }
                if (DidRayHit()) HitScanFire(index);
				muzFlashSkorpion.SetActive(true);
                break;
            case 50:
                //Sparq Beam
				sparqSetting = currentWeapon.weaponEnergySetting[14];
                if (!isSilent) { SFX.clip = SFXSparqBeamFire; SFX.Play(); }
                if (DidRayHit()) HitScanFire(index);
				muzFlashSparq.SetActive(true);
                if (overloadEnabled) {
                    sparqHeat = 100f;
                } else {
                    sparqHeat += sparqSetting;
                }
                if (sparqHeat > 100f) sparqHeat = 100f;
                break;
            case 51:
                //DH-07 Stungun
				stungunSetting = currentWeapon.weaponEnergySetting[15];
                if (!isSilent) { SFX.clip = SFXStungunFire; SFX.Play(); }
                FireStungun(index);
				muzFlashStungun.SetActive(true);
                stungunHeat += stungunSetting;
                if (stungunHeat > 100f) stungunHeat = 100f;
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
                    if (!currentWeapon.bottomless && !currentWeapon.redbull) curEnergy.TakeEnergy(Const.a.energyDrainOverloadForWeapon[index]); //take large amount
                } else {
                    float takeEnerg = 256*(currentWeapon.weaponEnergySetting[index] / 100f) * (Const.a.energyDrainHiForWeapon[index] - Const.a.energyDrainLowForWeapon[index]);
                    if (!currentWeapon.bottomless && !currentWeapon.redbull) curEnergy.TakeEnergy(takeEnerg);
                }
            } else {
                if (currentWeapon.weaponIsAlternateAmmo) {
                    if (!currentWeapon.bottomless) WeaponCurrent.WepInstance.currentMagazineAmount2[index]--;
                    // Update the counter
                    //MFDManager.a.UpdateHUDAmmoCounts(WeaponCurrent.WepInstance.currentMagazineAmount2[index]);
                } else {
                    if (!currentWeapon.bottomless) WeaponCurrent.WepInstance.currentMagazineAmount[index]--;
                    // Update the counter
                    //MFDManager.a.UpdateHUDAmmoCounts(WeaponCurrent.WepInstance.currentMagazineAmount[index]);
                }
            }
        }

        playerCamera.GetComponent<MouseLookScript>().Recoil(index);
		Recoil(index);
        if (currentWeapon.weaponIsAlternateAmmo || overloadEnabled) {
            overloadEnabled = false;
            waitTilNextFire = Time.time + Const.a.delayBetweenShotsForWeapon2[index];
        } else {
            waitTilNextFire = Time.time + Const.a.delayBetweenShotsForWeapon[index];
        }
    }

    bool DidRayHit() {
        tempHit = new RaycastHit();
        tempVec = new Vector3(MouseCursor.drawTexture.x + (MouseCursor.drawTexture.width / 2), MouseCursor.drawTexture.y + (MouseCursor.drawTexture.height / 2) + verticalOffset, 0);
        tempVec.y = Screen.height - tempVec.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
        int layMask = LayerMask.GetMask("Default","Water","Geometry","NPC","Corpse","Door","InterDebris"); //UPDATE: Can't shoot players, but we can't shoot the back of our eyeballs now
        if (Physics.Raycast(playerCamera.ScreenPointToRay(tempVec), out tempHit, fireDistance,layMask)) {
            tempHM = tempHit.transform.gameObject.GetComponent<HealthManager>();
            if (tempHit.transform.gameObject.GetComponent<HealthManager>() != null) {
                useBlood = true;
            }
            return true;
        }
        return false;
    }

    GameObject GetImpactType(HealthManager hm) {
        if (hm == null) return Const.a.GetObjectFromPool(Const.PoolType.SparksSmall);
        switch (hm.bloodType) {
            case HealthManager.BloodType.None: return Const.a.GetObjectFromPool(Const.PoolType.SparksSmall);
            case HealthManager.BloodType.Red: return Const.a.GetObjectFromPool(Const.PoolType.BloodSpurtSmall);
            case HealthManager.BloodType.Yellow: return Const.a.GetObjectFromPool(Const.PoolType.BloodSpurtSmallYellow);
            case HealthManager.BloodType.Green: return Const.a.GetObjectFromPool(Const.PoolType.BloodSpurtSmallGreen);
            case HealthManager.BloodType.Robot: return Const.a.GetObjectFromPool(Const.PoolType.SparksSmallBlue);
        }

        return Const.a.GetObjectFromPool(Const.PoolType.SparksSmall);
    }

	void CreateStandardImpactMarks(int wep16index) {
		// Don't create bullet holes on objects that move
		if (tempHit.transform.GetComponent<Rigidbody>() != null) return;
		if (tempHit.transform.GetComponent<HealthManager>() != null) return; // don't create bullet holes on objects that die
		if (tempHit.transform.GetComponent<Animator>() != null) return; // don't create bullet holes on objects that animate
		if (tempHit.transform.GetComponent<Animation>() != null) return; // don't create bullet holes on objects that animate
		if (tempHit.transform.GetComponent<Door>() != null) return; // don't create bullet holes on doors, makes them ghost and flicker through walls

		// Add bullethole
		tempVec = tempHit.normal * 0.16f;
		GameObject holetype = bulletHoleSmall;
		switch(wep16index) {
			case 0: holetype = bulletHoleLarge;
					break;
			case 1: holetype = bulletHoleScorchSmall;
					break;
			case 2: holetype = bulletHoleTiny;
					break;
			case 3: holetype = bulletHoleSmall;
					break;
			case 4: holetype = bulletHoleScorchLarge;
					break;
			case 5: holetype = bulletHoleScorchSmall;
					break;
			case 6: return; // no impact marks for lead pipe
			case 7: holetype = bulletHoleLarge;
					break;
			case 8: holetype = bulletHoleScorchLarge;
					break;
			case 9: holetype = bulletHoleSmall;
					break;
			case 10: holetype = bulletHoleScorchLarge;
					break;
			case 11: holetype = bulletHoleScorchLarge;
					break;
			case 12: holetype = bulletHoleSpread;
					break;
			case 13: holetype = bulletHoleLarge;
					break;
			case 14: holetype = bulletHoleScorchSmall;
					break;
			case 15: holetype = bulletHoleScorchSmall;
					break;
		}

		GameObject impactMark = (GameObject) Instantiate(holetype, (tempHit.point + tempVec),  Quaternion.LookRotation(tempHit.normal*-1,Vector3.up), tempHit.transform.gameObject.transform);
		int rint = Random.Range(0,3);
		Quaternion roll = impactMark.transform.localRotation;
		roll *= Quaternion.Euler(0f,0f,rint * 90f);
		impactMark.transform.localRotation = roll;
	}

    void CreateStandardImpactEffects(bool onlyBloodIfHitHasHM) {
        // Determine blood type of hit target and spawn corresponding blood particle effect from the Const.Pool
        if (useBlood) {
            GameObject impact = GetImpactType(tempHM);
            if (impact != null) {
                tempVec = tempHit.normal * hitOffset;
                impact.transform.position = tempHit.point + tempVec;
                impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, tempHit.normal);
                impact.SetActive(true);
            }
        } else {
            // Allow for skipping adding sparks after special override impact effects per attack functions below
            if (!onlyBloodIfHitHasHM) {
                GameObject impact = Const.a.GetObjectFromPool(Const.PoolType.SparksSmall); //Didn't hit an object with a HealthManager script, use sparks
                if (impact != null) {
                    tempVec = tempHit.normal * hitOffset;
                    impact.transform.position = tempHit.point + tempVec;
                    impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, tempHit.normal);
                    impact.SetActive(true);
                }
            }
        }
    }

    void CreateBeamImpactEffects(int wep16index) {
        GameObject impact = Const.a.GetObjectFromPool(Const.PoolType.SparqImpacts);
        if (wep16index == 1) {
            impact = Const.a.GetObjectFromPool(Const.PoolType.BlasterImpacts);  //Red laser for blaster
        } else {
            if (wep16index == 4) {
                impact = Const.a.GetObjectFromPool(Const.PoolType.IonImpacts); // Yellow laser for ion
            }
        }

        if (impact != null) {
            impact.transform.position = tempHit.point;
            impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, tempHit.normal);
            impact.SetActive(true);
        }
    }

    void CreateBeamEffects(int wep16index) {
        int laserIndex = 102; // Turquoise/Pale-Teal for sparq
        if (wep16index == 1) {
            laserIndex = 103;  //Red laser for blaster
        } else {
           if (wep16index == 4) {
               laserIndex = 104; // Yellow laser for ion
           }
        }
        
		GameObject dynamicObjectsContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
		if (dynamicObjectsContainer == null) return; //didn't find current level
		GameObject lasertracer = Instantiate(Const.a.useableItems[laserIndex],transform.position,Quaternion.identity) as GameObject;
		lasertracer.transform.SetParent(dynamicObjectsContainer.transform,true);
        tempVec = transform.position;
        tempVec.y += verticalOffset;
		lasertracer.GetComponent<LaserDrawing>().startPoint = tempVec;
		lasertracer.GetComponent<LaserDrawing>().endPoint = tempHit.point;
		lasertracer.SetActive(true);
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
		retval = ((currentWeapon.weaponEnergySetting[wep16Index]/100f)*((dmg_max/ener_max)-(dmg_min/ener_min)) + 3f) * (((currentWeapon.weaponEnergySetting[wep16Index])/100f)*(ener_max-ener_min) + ener_min);
		//Debug.Log("returning DamageForPower of " + retval.ToString() + ", for wep16Index of " + wep16Index.ToString());
		return retval;
		// You gotta love maths!  There is a spreadsheet for this (.ods LibreOffice file format, found with src code) that shows the calculations to make this dmg curve. 
	}

    // WEAPON FIRING CODE:
    // ==============================================================================================================================
    // Hitscan Weapons
    //----------------------------------------------------------------------------------------------------------
    // Guns and laser beams, used by most weapons
    void HitScanFire(int wep16Index) {
		damageData.ResetDamageData(damageData);
        if (wep16Index == 1 || wep16Index == 4 || wep16Index == 14) {
            CreateBeamImpactEffects(wep16Index); // laser burst effect overrides standard blood spurts/robot sparks
            damageData.attackType = Const.AttackType.EnergyBeam;
        } else {
            CreateStandardImpactEffects(false); // standard blood spurts/robot sparks
			switch (wep16Index) {
				case 2: damageData.attackType = Const.AttackType.ProjectileNeedle; break;
				case 3: damageData.attackType = Const.AttackType.ProjectileNeedle; break;
				case 8: damageData.attackType = Const.AttackType.Magnetic; break;
				default: damageData.attackType = Const.AttackType.Projectile; break; // pew pew pew
			}
			if (wep16Index == 2 && currentWeapon.weaponIsAlternateAmmo) damageData.attackType = Const.AttackType.Tranq; // tranquilize the untranquil....yes
        }
        // Fill the damageData container
        damageData.other = tempHit.transform.gameObject;
        if (tempHit.transform.gameObject.tag == "NPC") {
            damageData.isOtherNPC = true;
        } else {
            damageData.isOtherNPC = false;
			CreateStandardImpactMarks(wep16Index);
        }
        damageData.hit = tempHit;
        damageData.attacknormal = playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center).direction;
        if (currentWeapon.weaponIsAlternateAmmo) {
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
        damageData.damage = Const.a.GetDamageTakeAmount(damageData);
		damageData.attackType = Const.a.attackTypeForWeapon[wep16Index];
        damageData.owner = playerCapsule;
        HealthManager hm = tempHit.transform.gameObject.GetComponent<HealthManager>();
        if (hm != null && hm.health > 0) {
			float dmgFinal = hm.TakeDamage(damageData); // send the damageData container to HealthManager of hit object and apply damage
			if (dmgFinal <= 0) {
				noDamageIndicator.transform.position = tempHit.transform.position; // center on what we just shot
				noDamageIndicator.SetActive(true); // do this regardless of target identifier version to show player that hey, it no workie
			} else {
				noDamageIndicator.SetActive(false); // I'm assuming that this will auto deactivate after 1sec, but in case the player is snappy about weapon switching, added this
			}
		}
		UseableObjectUse uou = tempHit.transform.gameObject.GetComponent<UseableObjectUse>();
		if (uou != null) uou.HitForce(damageData); // knock objects around

        // Draw a laser beam for beam weapons
        if (wep16Index == 1 || wep16Index == 4 || wep16Index == 14) {
            CreateBeamEffects(wep16Index);
        }
    }

    // Melee weapons
    //----------------------------------------------------------------------------------------------------------
    // Rapier and pipe.  Need extra code to handle anims for view model and sound for swing-and-a-miss! vs. hit
	//void ApplyMeleeHit(int index16, GameObject targ, RaycastHit tHit, int numTargets, bool isRapier,bool silent) {
	void ApplyMeleeHit(int index16, GameObject targ, int numTargets,bool isRapier, bool silent,AudioClip hit, AudioClip miss,AudioClip hitflesh) {
		damageData.other = targ;
		if (targ.tag == "NPC") {
			damageData.isOtherNPC = true;
		} else {
			damageData.isOtherNPC = false;
			CreateStandardImpactMarks(index16);
		}
		//damageData.hit = tHit;
		damageData.attacknormal = playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center).direction;
		damageData.damage = Const.a.damagePerHitForWeapon[index16]/numTargets; // divide across multiple targets
		damageData.damage = Const.a.GetDamageTakeAmount(damageData);
		damageData.offense = Const.a.offenseForWeapon[index16];
		damageData.penetration = Const.a.penetrationForWeapon[index16];
		damageData.owner = playerCapsule;
		if (isRapier) {
			damageData.attackType = Const.AttackType.MeleeEnergy;
		} else {
			damageData.attackType = Const.AttackType.Melee;
		}
		UseableObjectUse uou = targ.GetComponent<UseableObjectUse>();
		if (uou != null) uou.HitForce(damageData); // knock objects around
		HealthManager hm = targ.GetComponent<HealthManager>();
		if (hm == null) {
			SFX.clip = hit;
			SFX.Play();
			return;
		}
		if (hm!= null) hm.TakeDamage(damageData); //no need to check if damage was done and if we need noDamageIndicator since melee weapons always do damage against all types
		if (!silent) {
			if ((hm.bloodType == HealthManager.BloodType.Red) || (hm.bloodType == HealthManager.BloodType.Yellow) || (hm.bloodType == HealthManager.BloodType.Green)) {
				SFX.clip = hitflesh;
			} else {
				SFX.clip = hit;
			}
			SFX.Play();
		}
		return;
	}

    void FireRapier(int index16, bool silent) { FireMelee(index16, true, silent, SFXRapierHit, SFXRapierMiss, SFXRapierHit, true); }
    void FirePipe(int index16, bool silent) { FireMelee(index16, false, silent, SFXPipeHit, SFXPipeMiss, SFXPipeHitFlesh, false); }

	void FireMelee(int index16, bool isRapier, bool silent, AudioClip hit, AudioClip miss,AudioClip hitflesh, bool rapier) {
		bool foundSomeHMs = false;
		bool swunghit = false;
		int numtargets = 0;
		// check all objects we can hurt have HealthManager, that they are in meleescanDistance range, that they are within player facing angle by 60° (±30°)
		for (int i=0;i<Const.a.healthObjectsRegistration.Length;i++) {
			if (Const.a.healthObjectsRegistration[i] != null) {
				HealthManager hm = Const.a.healthObjectsRegistration[i].gameObject.GetComponent<HealthManager>();
				GameObject ho = Const.a.healthObjectsRegistration[i].gameObject;
				if (hm != null && Vector3.Distance(ho.transform.position,playerCapsule.transform.position) < meleescanDistance) {
					mls.SetCameraFocusPoint();
					tempVec = mls.cameraFocusPoint - playerCamera.transform.position;
					tempVec = tempVec.normalized;
					Vector3 ang = playerCamera.transform.position - ho.transform.position;
					ang = ang.normalized;
					float dot = Vector3.Dot(tempVec,ang);
					if (dot > 0.666f) {
						anim.Play("Attack2");
						if (!silent) {
							SFX.clip = hit;
							SFX.Play();
						}

						CreateStandardImpactEffects(true);
						numtargets++;
						if (numtargets <= 0) numtargets = 1; //don't divide by 0
						ApplyMeleeHit(index16,ho,numtargets,isRapier,silent,hit,miss,hitflesh);
						swunghit = true;
						foundSomeHMs = true;
						
					}
				}
			}
			i++;
		}

		// if we didn't find any objects with HealthManagers above, let's just do normal straightline raytrace at center like old method
		if (!foundSomeHMs) {
			fireDistance = meleescanDistance;
			if (DidRayHit()) {
				fireDistance = hitscanDistance; // reset before any returns
				if (rapier) {
					rapieranim.Play("Attack2");
				} else {
					anim.Play("Attack2");
				}
				if (!silent) {
					SFX.clip = hit;
					SFX.Play();
				}
				CreateStandardImpactEffects(true);
				for (int ij=0;ij<Const.a.healthObjectsRegistration.Length;ij++) {
					if (Const.a.healthObjectsRegistration[ij] != null) {
						if (Const.a.healthObjectsRegistration[ij].isNPC) {
							if (Vector3.Distance(Const.a.healthObjectsRegistration[ij].gameObject.transform.position,playerCapsule.transform.position) < Const.a.healthObjectsRegistration[ij].aic.rangeToHear) {
								Const.a.healthObjectsRegistration[ij].NotifyEnemyNearby(playerCapsule);
							}
						}
					}
				}
				if (numtargets <= 0) numtargets = 1; //don't divide by 0
				ApplyMeleeHit(index16,tempHit.transform.gameObject,numtargets,isRapier,silent,hit,miss,hitflesh);
				swunghit = true; // we hit something at least
			} else {
				fireDistance = hitscanDistance; //reset in case raycast failed
				
				if (!silent) {
					SFX.clip = miss;
					SFX.Play();
				}
				if (rapier) {
					rapieranim.Play("Attack2");
				} else {
					anim.Play("Attack1");
				}
			}
		}

		if (swunghit) {
			// notify other HM objects nearby of player attack
			for (int ij=0;ij<Const.a.healthObjectsRegistration.Length;ij++) {
				if (Const.a.healthObjectsRegistration[ij] != null) {
					if (Const.a.healthObjectsRegistration[ij].isNPC) {
						if (Vector3.Distance(Const.a.healthObjectsRegistration[ij].gameObject.transform.position,playerCapsule.transform.position) < Const.a.healthObjectsRegistration[ij].aic.rangeToHear) {
							Const.a.healthObjectsRegistration[ij].NotifyEnemyNearby(playerCapsule);
						}
					}
				}
			}
		}
	}

    // Projectile weapons
    //----------------------------------------------------------------------------------------------------------
    void FirePlasma(int index16) {
        // Create and hurl a beachball-like object.  On the developer commentary they said that the projectiles act
        // like a beachball for collisions with enemies, but act like a baseball for walls/floor to prevent hitting corners
        GameObject beachball = Const.a.GetObjectFromPool(Const.PoolType.PlasmaShots);
        if (beachball != null) {
            damageData.damage = Const.a.damagePerHitForWeapon[index16];
            damageData.owner = playerCapsule;
            damageData.attackType = Const.a.attackTypeForWeapon[index16];
			damageData.offense = Const.a.offenseForWeapon[index16];
			damageData.penetration = Const.a.penetrationForWeapon[index16];
            beachball.GetComponent<ProjectileEffectImpact>().dd = damageData;
            beachball.GetComponent<ProjectileEffectImpact>().host = playerCapsule;
			beachball.layer = playerCapsule.gameObject.layer; // don't touch the player who shot us
            beachball.transform.position = playerCamera.transform.position;
			mls.SetCameraFocusPoint();
            tempVec = mls.cameraFocusPoint - playerCamera.transform.position;
            beachball.transform.forward = tempVec.normalized;
            //drawMyLine(beachball.transform.position, mls.cameraFocusPoint, Color.green, 2f);
            beachball.SetActive(true);
            Vector3 shove = beachball.transform.forward * plasmaShotForce;
            beachball.GetComponent<Rigidbody>().velocity = Vector3.zero; // prevent random variation from the last shot's velocity
            beachball.GetComponent<Rigidbody>().AddForce(shove, ForceMode.Impulse);
        }
    }

    void FireRailgun(int index16) {
        // Create and hurl a beachball-like object.  On the developer commentary they said that the projectiles act
        // like a beachball for collisions with enemies, but act like a baseball for walls/floor to prevent hitting corners
        GameObject beachball = Const.a.GetObjectFromPool(Const.PoolType.RailgunShots);
        if (beachball != null) {
            damageData.damage = Const.a.damagePerHitForWeapon[index16];
            damageData.owner = playerCapsule;
            damageData.attackType = Const.a.attackTypeForWeapon[index16];
			damageData.offense = Const.a.offenseForWeapon[index16];
			damageData.penetration = Const.a.penetrationForWeapon[index16];
            beachball.GetComponent<ProjectileEffectImpact>().dd = damageData;
            beachball.GetComponent<ProjectileEffectImpact>().host = playerCapsule;
			beachball.layer = playerCapsule.gameObject.layer; // don't touch the player who shot us
            beachball.transform.position = playerCamera.transform.position;
			mls.SetCameraFocusPoint();
            tempVec = mls.cameraFocusPoint - playerCamera.transform.position;
            beachball.transform.forward = tempVec.normalized;
            //drawMyLine(beachball.transform.position,mls.cameraFocusPoint, Color.green, 2f);
            beachball.SetActive(true);
            Vector3 shove = beachball.transform.forward * railgunShotForce;
            beachball.GetComponent<Rigidbody>().velocity = Vector3.zero; // prevent random variation from the last shot's velocity
            beachball.GetComponent<Rigidbody>().AddForce(shove, ForceMode.Impulse);
        }
    }

    void FireStungun(int index16) {
        // Create and hurl a beachball-like object.  On the developer commentary they said that the projectiles act
        // like a beachball for collisions with enemies, but act like a baseball for walls/floor to prevent hitting corners
        GameObject beachball = Const.a.GetObjectFromPool(Const.PoolType.StungunShots);
        if (beachball != null) {
            damageData.damage = Const.a.damagePerHitForWeapon[index16];
            damageData.owner = playerCapsule;
            damageData.attackType = Const.a.attackTypeForWeapon[index16];
			damageData.offense = Const.a.offenseForWeapon[index16];
			damageData.penetration = Const.a.penetrationForWeapon[index16];
            beachball.GetComponent<ProjectileEffectImpact>().dd = damageData;
            beachball.GetComponent<ProjectileEffectImpact>().host = playerCapsule;
			beachball.layer = playerCapsule.gameObject.layer; // don't touch the player who shot us
            beachball.transform.position = playerCamera.transform.position;
			mls.SetCameraFocusPoint();
            tempVec = mls.cameraFocusPoint - playerCamera.transform.position;
            beachball.transform.forward = tempVec.normalized;
            //drawMyLine(beachball.transform.position, mls.cameraFocusPoint, Color.green, 2f);
            beachball.SetActive(true);
            Vector3 shove = beachball.transform.forward * stungunShotForce;
            beachball.GetComponent<Rigidbody>().velocity = Vector3.zero; // prevent random variation from the last shot's velocity
            beachball.GetComponent<Rigidbody>().AddForce(shove, ForceMode.Impulse);
        }
    }

    public Vector3 ScreenPointToDirectionVector() {
        Vector3 retval = Vector3.zero;
        retval = playerCamera.transform.forward;
        
        return retval;
    }

    void FireMagpulse(int index16) {
        // Create and hurl a beachball-like object.  On the developer commentary they said that the projectiles act
        // like a beachball for collisions with enemies, but act like a baseball for walls/floor to prevent hitting corners
        GameObject beachball = Const.a.GetObjectFromPool(Const.PoolType.MagpulseShots);
        if (beachball != null) {
            damageData.damage = Const.a.damagePerHitForWeapon[index16];
            damageData.owner = playerCapsule;
            damageData.attackType = Const.a.attackTypeForWeapon[index16];
			damageData.offense = Const.a.offenseForWeapon[index16];
			damageData.penetration = Const.a.penetrationForWeapon[index16];
            beachball.GetComponent<ProjectileEffectImpact>().dd = damageData;
            beachball.GetComponent<ProjectileEffectImpact>().host = playerCapsule;
			beachball.layer = playerCapsule.gameObject.layer; // don't touch the player who shot us
            beachball.transform.position = playerCamera.transform.position;
			mls.SetCameraFocusPoint();
            tempVec = mls.cameraFocusPoint - playerCamera.transform.position;
            beachball.transform.forward = tempVec.normalized;
            //drawMyLine(beachball.transform.position, mls.cameraFocusPoint, Color.green, 2f);
            beachball.SetActive(true);
            Vector3 shove = beachball.transform.forward * magpulseShotForce;
            beachball.GetComponent<Rigidbody>().velocity = Vector3.zero; // prevent random variation from the last shot's velocity
            beachball.GetComponent<Rigidbody>().AddForce(shove, ForceMode.Impulse);
        }
    }

    public float GetHeatForCurrentWeapon() {
        retval = 0f;
        switch (currentWeapon.weaponIndex) {
            case 37:
                retval = blasterHeat;
                break;
            case 40:
                retval = ionHeat;
                break;
            case 46:
                retval = plasmaHeat;
                break;
            case 50:
                retval = sparqHeat;
                break;
            case 51:
                retval = stungunHeat;
                break;
            default:
                retval = 0f;
                break;
        }
        if (retval > 100f) retval = 100f;
        if (retval < 0) retval = 0;
        return retval;
    }

    void drawMyLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        StartCoroutine(drawLine(start, end, color, duration));
    }

    IEnumerator drawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Additive"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        yield return new WaitForSeconds(duration);
        GameObject.Destroy(myLine);
    }
}