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
	public bool isAlternateAmmo = false;
	public bool berserkActive = false;
	[HideInInspector]
	public float[] args;
	public GameObject bullet;
	public GameObject impactEffect;
	public WeaponMagazineCounter wepmagCounter;
	public Camera playerCamera; // assign in the editor
	public Camera gunCamera; // assign in the editor
	public PlayerEnergy curEnergy;
	public GameObject playerCapsule;
	public WeaponCurrent currentWeapon; // assign in the editor
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXMark3Fire; // assign in the editor
	[SerializeField] private AudioClip SFXBlasterFire; // assign in the editor
	//[SerializeField] private AudioClip SFXBlasterOverFire; // assign in the editor
	[SerializeField] private AudioClip SFXDartFire; // assign in the editor
	[SerializeField] private AudioClip SFXFlechetteFire; // assign in the editor
	[SerializeField] private AudioClip SFXIonFire; // assign in the editor
	//[SerializeField] private AudioClip SFXIonOverFire; // assign in the editor
	[SerializeField] private AudioClip SFXRapierMiss; // assign in the editor
	[SerializeField] private AudioClip SFXRapierHit; // assign in the editor
	[SerializeField] private AudioClip SFXPipeMiss; // assign in the editor
	[SerializeField] private AudioClip SFXPipeHit; // assign in the editor
	[SerializeField] private AudioClip SFXPipeHitFlesh; // assign in the editor
	[SerializeField] private AudioClip SFXMagnumFire; // assign in the editor
	[SerializeField] private AudioClip SFXMagpulseFire; // assign in the editor
	[SerializeField] private AudioClip SFXPistolFire; // assign in the editor
	[SerializeField] private AudioClip SFXPlasmaFire; // assign in the editor
	[SerializeField] private AudioClip SFXRailgunFire; // assign in the editor
	[SerializeField] private AudioClip SFXRiotgunFire; // assign in the editor
	[SerializeField] private AudioClip SFXSkorpionFire; // assign in the editor
	[SerializeField] private AudioClip SFXSparqBeamFire; // assign in the editor
	//[SerializeField] private AudioClip SFXSparqBeamOverFire; // assign in the editor
	[SerializeField] private AudioClip SFXStungunFire; // assign in the editor
	[SerializeField] private AudioClip SFXEmpty; // assign in the editor
	[SerializeField] private AudioClip SFXRicochet; // assign in the editor
	private float clipEnd;
	public Animator anim; // assign in the editor
	public DamageData damageData;
	private RaycastHit tempHit;
	private Vector3 tempVec;
	private bool useBlood;
	private HealthManager tempHM;

	void Awake () {
		damageData = new DamageData();
		tempHit = new RaycastHit();
		tempVec = new Vector3(0f, 0f, 0f);
	}

	void GetWeaponData (int index) {
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

	public static int Get16WeaponIndexFromConstIndex (int index) {
		int i = -1;
		switch(index) {
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

	void  Update() {
		if (!PauseScript.a.paused) {
			if (!GUIState.a.isBlocking && !playerCamera.GetComponent<MouseLookScript>().holdingObject) {
				int i = Get16WeaponIndexFromConstIndex (currentWeapon.weaponIndex);
				if (i == -1) return;

				GetWeaponData(i);
				if (GetInput.a.Attack(Const.a.isFullAutoForWeapon[i]) && waitTilNextFire < Time.time) {
					// Check weapon type and check ammo before firing
					if (i == 5 || i == 6) {
						// Pipe or Laser Rapier, attack without prejudice
						FireWeapon(i, false); // weapon index, isSilent == false so play normal SFX
					} else {
						// Energy weapons so check energy level
						if (i == 1 || i == 4 || i == 10 || i == 14 || i == 15) {
							// Even if we have only 1 energy, we still fire with all we've got up to the energy level setting of course
							if (curEnergy.energy > 0) FireWeapon (i, false); // weapon index, isSilent == false so play normal SFX
						} else {
							// Uses normal ammo, check versus alternate or normal to see if we have ammo then fire
							if (currentWeapon.weaponIsAlternateAmmo) {
								if (WeaponCurrent.WepInstance.currentMagazineAmount2[i] > 0) {
									FireWeapon (i, false); // weapon index, isSilent == false so play normal SFX
								} else {
									SFX.PlayOneShot (SFXEmpty);
								}
							} else {
								if (WeaponCurrent.WepInstance.currentMagazineAmount[i] > 0) {
									FireWeapon (i, false); // weapon index, isSilent == false so play normal SFX
								}else {
									SFX.PlayOneShot (SFXEmpty);
								}
							}
						}
					}
				}

				if (GetInput.a.Reload ()) {
					WeaponCurrent.WepInstance.Reload();
				}
			}
		}
	}

	// index is used to get recoil down at the bottom, otherwise the cases use currentWeapon.weaponIndex
	void FireWeapon (int index, bool isSilent) {
		damageData.ResetDamageData(damageData);
		switch(currentWeapon.weaponIndex) {
		case 36:
			//Mark3 Assault Rifle
			FireAssault(isSilent);
			break;
		case 37:
			//ER-90 Blaster
			FireBlaster(isSilent);
			break;
		case 38:
			//SV-23 Dartgun
			FireDart(isSilent);
			break;
		case 39:
			//AM-27 Flechette
			FireFlechette(isSilent);
			break;
		case 40:
			//RW-45 Ion Beam
			FireIon(isSilent);
			break;
		case 41:
			//TS-04 Laser Rapier
			FireRapier(isSilent);
			break;
		case 42:
			//Lead Pipe
			FirePipe(isSilent);
			break;
		case 43:
			//Magnum 2100
			FireMagnum(isSilent);
			break;
		case 44:
			//SB-20 Magpulse
			FireMagpulse(isSilent);
			break;
		case 45:
			//ML-41 Pistol
			FirePistol(isSilent);
			break;
		case 46:
			//LG-XX Plasma Rifle
			FirePlasma(isSilent);
			break;
		case 47:
			//MM-76 Railgun
			FireRailgun(isSilent);
			break;
		case 48:
			//DC-05 Riotgun
			FireRiotgun(isSilent);
			break;
		case 49:
			//RF-07 Skorpion
			FireSkorpion(isSilent);
			break;
		case 50:
			//Sparq Beam
			FireSparq(isSilent);
			break;
		case 51:
			//DH-07 Stungun
			FireStungun(isSilent);
			break;
		}

		// TAKE AMMO
		// no weapons subtract more than 1 at a time in a shot, subtracting 1
		// Check weapon type before subtracting ammo or energy
		if (index == 5 || index == 6) {
			// Pipe or Laser Rapier
			// ammo is already 0, do nothing.  This is here to prevent subtracting ammo on the first slot of .wepAmmo[index] on the last else clause below
		} else {
			// Energy weapons so check energy level
			if (index == 1 || index == 4 || index == 10 || index == 14 || index == 15) {
				curEnergy.TakeEnergy (currentWeapon.weaponEnergySetting [index]);
			} else {
				if (currentWeapon.weaponIsAlternateAmmo) {
					WeaponCurrent.WepInstance.currentMagazineAmount2[index]--;

					// Update the counter
					MFDManager.a.UpdateHUDAmmoCounts(WeaponCurrent.WepInstance.currentMagazineAmount2[index]);
				} else {
					WeaponCurrent.WepInstance.currentMagazineAmount[index]--;

					// Update the counter
					MFDManager.a.UpdateHUDAmmoCounts(WeaponCurrent.WepInstance.currentMagazineAmount[index]);
				}
			}
		}



		playerCamera.GetComponent<MouseLookScript>().Recoil(index);
		if (currentWeapon.weaponIsAlternateAmmo) {
			waitTilNextFire = Time.time + Const.a.delayBetweenShotsForWeapon2[index];
		} else {
			waitTilNextFire = Time.time + Const.a.delayBetweenShotsForWeapon[index];
		}
	}

	bool DidRayHit () {
		tempHit = new RaycastHit();
		tempVec = new Vector3(MouseCursor.drawTexture.x+(MouseCursor.drawTexture.width/2),MouseCursor.drawTexture.y+(MouseCursor.drawTexture.height/2)+verticalOffset,0); 
		tempVec.y = Screen.height - tempVec.y; // Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
		if (Physics.Raycast(playerCamera.ScreenPointToRay(tempVec), out tempHit, fireDistance)) {
			tempHM = tempHit.transform.gameObject.GetComponent<HealthManager>();
			if (tempHit.transform.gameObject.GetComponent<HealthManager>() != null) {
				useBlood = true;
			}
			return true;
		}
		return false;
	}

	GameObject GetImpactType (HealthManager hm) {
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

	void CreateStandardImpactEffects (bool onlyBloodIfHitHasHM) {
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

	void FireAssault (bool silent) {
		if (!silent) { SFX.clip = SFXMark3Fire; SFX.Play(); }

	}

	void FireBlaster (bool silent) {
		if (!silent) { SFX.clip = SFXBlasterFire; SFX.Play(); }

	}

	void FireFlechette (bool silent) {
		if (!silent) { SFX.clip = SFXFlechetteFire; SFX.Play(); }

	}

	void FireIon (bool silent) {
		if (!silent) { SFX.clip = SFXIonFire; SFX.Play(); }

	}

	void FireDart (bool silent) {
		if (!silent) { SFX.clip = SFXDartFire; SFX.Play(); }
		if (DidRayHit()) {
			CreateStandardImpactEffects(false);
			damageData.other = tempHit.transform.gameObject;
			if (tempHit.transform.gameObject.tag == "NPC") {
				damageData.isOtherNPC = true;
			} else {
				damageData.isOtherNPC = false;
			}
			damageData.hit = tempHit;
			damageData.attacknormal = playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center).direction;
			damageData.damage = Const.a.damagePerHitForWeapon[2];
			damageData.damage = Const.a.GetDamageTakeAmount(damageData);
			damageData.owner = playerCapsule;
			damageData.attackType = Const.AttackType.Projectile;
			HealthManager hm = tempHit.transform.gameObject.GetComponent<HealthManager>();
			if (hm == null) return;
			hm.TakeDamage(damageData);
		}
	}

	void FireRapier (bool silent) {
		if (!silent) { SFX.clip = SFXRapierMiss; SFX.Play(); }
	}
		
	void FirePipe (bool silent) {
		if (DidRayHit()) {
			anim.Play("Attack2");
			if (!silent) {
				SFX.clip = SFXPipeHit;
				SFX.Play();
			}
			CreateStandardImpactEffects(true);
			damageData.other = tempHit.transform.gameObject;
			if (tempHit.transform.gameObject.tag == "NPC") {
				damageData.isOtherNPC = true;
			} else {
				damageData.isOtherNPC = false;
			}
			damageData.hit = tempHit;
			damageData.attacknormal = playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center).direction;
			damageData.damage = Const.a.damagePerHitForWeapon[6];
			damageData.damage = Const.a.GetDamageTakeAmount(damageData);
			damageData.owner = playerCapsule;
			damageData.attackType = Const.AttackType.Projectile;
			HealthManager hm = tempHit.transform.gameObject.GetComponent<HealthManager>();
			if (hm == null) return;
			hm.TakeDamage(damageData);
			return;
		}
		if (!silent) {
			SFX.clip = SFXPipeMiss;
			SFX.Play();
		}
		anim.Play("Attack1");
	}

	void FireMagnum (bool silent) {
		if (!silent) { SFX.clip = SFXMagnumFire; SFX.Play(); }

	}

	void FireMagpulse (bool silent) {
		if (!silent) { SFX.clip = SFXMagpulseFire; SFX.Play(); }

	}

	void FirePistol (bool silent) {
		if (!silent) { SFX.clip = SFXPistolFire; SFX.Play(); }
		if (DidRayHit()) {
			CreateStandardImpactEffects(false);
			if (tempHit.transform.gameObject.tag == "NPC") {
				damageData.isOtherNPC = true;
			} else {
				damageData.isOtherNPC = false;
			}
			damageData.hit = tempHit;
			damageData.attacknormal = playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center).direction;
			damageData.damage = Const.a.damagePerHitForWeapon[9];
			//hit.transform.gameObject.SendMessage("TakeDamage", damageData,SendMessageOptions.DontRequireReceiver);
			HealthManager hm = tempHit.transform.gameObject.GetComponent<HealthManager>();
			if (hm == null) return;
			float take = Const.a.GetDamageTakeAmount(damageData);
			hm.health = hm.health - take;
		}
	}

	void FirePlasma (bool silent) {
		if (!silent) { SFX.clip = SFXPlasmaFire; SFX.Play(); }

	}

	void FireRailgun (bool silent) {
		if (!silent) { SFX.clip = SFXRailgunFire; SFX.Play(); }

	}

	void FireRiotgun (bool silent) {
		if (!silent) { SFX.clip = SFXRiotgunFire; SFX.Play(); }

	}

	void FireSkorpion (bool silent) {
		if (!silent) { SFX.clip = SFXSkorpionFire; SFX.Play(); }

	}

	void FireSparq (bool silent) {
		if (!silent) {
			SFX.clip = SFXSparqBeamFire;
			SFX.Play();
		}

		if (DidRayHit()) {
			GameObject impact = Const.a.GetObjectFromPool(Const.PoolType.SparqImpacts);
			if (impact != null) {
				impact.transform.position = tempHit.point;
				impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, tempHit.normal);
				impact.SetActive(true);
			}
			if (useBlood) CreateStandardImpactEffects(true);
			damageData.hit = tempHit;
			damageData.attacknormal = playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center).direction;
			damageData.damage = Const.a.damagePerHitForWeapon[14];
			tempHit.transform.gameObject.SendMessage("TakeDamage",damageData,SendMessageOptions.DontRequireReceiver);
			GameObject lasertracer = Const.a.GetObjectFromPool(Const.PoolType.LaserLines);
			if (lasertracer != null) {
				lasertracer.SetActive(true);
				tempVec = transform.position;
				tempVec.y += verticalOffset;
				lasertracer.GetComponent<LaserDrawing>().startPoint = tempVec;
				lasertracer.GetComponent<LaserDrawing>().endPoint = tempHit.point;
			}
		}
	}

	void FireStungun (bool silent) {
		if (!silent) { SFX.clip = SFXStungunFire; SFX.Play(); }

	}
}