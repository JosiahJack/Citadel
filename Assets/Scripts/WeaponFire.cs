using UnityEngine;
using System.Collections;

public class WeaponFire : MonoBehaviour {
	public bool isFullAuto = false;
	public float delayBetweenShots = 1f;
	public float damage = 1f;
	public float damageOverload = 1f;
	public float energyDrainLow = 1f;
	public float energyDrainHi = 2f;
	public float energyDrainOver = 3f;
	public float penetration = 1f;
	public float offense = 1f;
	public float range = 200f;
	public Const.AttackType attType = Const.AttackType.None;

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
	public Camera playerCamera; // assign in the editor
	public Camera gunCamera; // assign in the editor
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
	[SerializeField] private AudioClip SFXEmptyEnergy; // assign in the editor
	[SerializeField] private AudioClip SFXRicochet; // assign in the editor
	private float clipEnd;
	public Animator anim; // assign in the editor

	void GetWeaponData (int index) {
		if (index == -1) return;
		isFullAuto = Const.a.isFullAutoForWeapon[index];
		if (currentWeapon.weaponIsAlternateAmmo) {
			damage = Const.a.damagePerHitForWeapon2[index];
			delayBetweenShots = Const.a.delayBetweenShotsForWeapon2[index];
			penetration = Const.a.penetrationForWeapon2[index];
			offense = Const.a.offenseForWeapon2[index];
		} else {
			damage = Const.a.damagePerHitForWeapon[index];
			delayBetweenShots = Const.a.delayBetweenShotsForWeapon[index];
			penetration = Const.a.penetrationForWeapon[index];
			offense = Const.a.offenseForWeapon[index];
		}
		damageOverload = Const.a.damageOverloadForWeapon[index];
		energyDrainLow = Const.a.energyDrainLowForWeapon[index];
		energyDrainHi = Const.a.energyDrainHiForWeapon[index];
		energyDrainOver = Const.a.energyDrainOverloadForWeapon[index];
		range = Const.a.rangeForWeapon[index];
	}

	void  Update() {
		if (!PauseScript.a.paused) {
			if (!GUIState.a.isBlocking && !playerCamera.GetComponent<MouseLookScript>().holdingObject) {
				int i = -1;
				switch(currentWeapon.weaponIndex) {
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

				if (i == -1) return;
				GetWeaponData(i);
				if (GetInput.a.Attack(Const.a.isFullAutoForWeapon[i]) && waitTilNextFire < Time.time) {
					FireWeapon(i);
				}
			}
		}
	}

	void FireWeapon (int index) {
		switch(currentWeapon.weaponIndex) {
		case 36:
			//Mark3 Assault Rifle
			FireAssault(false);
			break;
		case 37:
			//ER-90 Blaster
			FireBlaster(false);
			break;
		case 38:
			//SV-23 Dartgun
			FireDart(false);
			break;
		case 39:
			//AM-27 Flechette
			FireFlechette(false);
			break;
		case 40:
			//RW-45 Ion Beam
			FireIon(false);
			break;
		case 41:
			//TS-04 Laser Rapier
			FireRapier(false);
			break;
		case 42:
			//Lead Pipe
			FirePipe(false);
			break;
		case 43:
			//Magnum 2100
			FireMagnum(false);
			break;
		case 44:
			//SB-20 Magpulse
			FireMagpulse(false);
			break;
		case 45:
			//ML-41 Pistol
			FirePistol(false);
			break;
		case 46:
			//LG-XX Plasma Rifle
			FirePlasma(false);
			break;
		case 47:
			//MM-76 Railgun
			FireRailgun(false);
			break;
		case 48:
			//DC-05 Riotgun
			FireRiotgun(false);
			break;
		case 49:
			//RF-07 Skorpion
			FireSkorpion(false);
			break;
		case 50:
			//Sparq Beam
			FireSparq(false);
			break;
		case 51:
			//DH-07 Stungun
			FireStungun(false);
			break;
		}
		waitTilNextFire = Time.time + delayBetweenShots;
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

		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, fireDistance)) {
			//drawDebugLine(playerCamera.transform.position,hit.point,Color.cyan,10f);
			GameObject impact = Const.a.GetObjectFromPool(Const.PoolType.DartImpacts);
			if (impact != null) {
				impact.transform.position = hit.point;
				impact.SetActive(true);
			}
			hit.transform.gameObject.SendMessage("TakeDamage", damage,SendMessageOptions.DontRequireReceiver);

		}
	}

	void FireRapier (bool silent) {
		if (!silent) { SFX.clip = SFXRapierMiss; SFX.Play(); }
	}
		
	void FirePipe (bool silent) {
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, fireDistance)) {
			anim.Play("Attack2");
			if (!silent) {
				SFX.clip = SFXPipeHit;
				SFX.Play();
			}

			float adjusteddamage = damage;
			if (berserkActive) adjusteddamage *= Const.a.berserkDamageMultiplier;
			hit.transform.gameObject.SendMessage("TakeDamage", adjusteddamage,SendMessageOptions.DontRequireReceiver);
			waitTilNextFire = Time.time + delayBetweenShots;
			return;
		}
		if (!silent) {
			SFX.clip = SFXPipeMiss;
			SFX.Play();
		}
		anim.Play("Attack1");
		waitTilNextFire = Time.time + delayBetweenShots;
	}

	void FireMagnum (bool silent) {
		if (!silent) { SFX.clip = SFXMagnumFire; SFX.Play(); }

	}

	void FireMagpulse (bool silent) {
		if (!silent) { SFX.clip = SFXMagpulseFire; SFX.Play(); }

	}

	void FirePistol (bool silent) {
		if (!silent) { SFX.clip = SFXPistolFire; SFX.Play(); }

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
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, fireDistance)) {
			GameObject impact = Const.a.GetObjectFromPool(Const.PoolType.SparqImpacts);
			if (impact != null) {
				impact.transform.position = hit.point;
				Debug.Log("Setting Sparq Impact to x: " + hit.point.x.ToString() + ",y: " + hit.point.y.ToString() + ",z: " + hit.point.z.ToString());
				impact.SetActive(true);
			}
			hit.transform.gameObject.SendMessage("TakeDamage",damage,SendMessageOptions.DontRequireReceiver);
			GameObject lasertracer = Const.a.GetObjectFromPool(Const.PoolType.LaserLines);
			if (lasertracer != null) {
				lasertracer.SetActive(true);
				Vector3 tempent = new Vector3 (playerCamera.transform.position.x, (playerCamera.transform.position.y + verticalOffset), playerCamera.transform.position.z);
				lasertracer.GetComponent<LaserDrawing>().startPoint = tempent;
				//Debug.Log("Setting Sparq laser endpoint to x: " + hit.point.x.ToString() + ",y: " + hit.point.y.ToString() + ",z: " + hit.point.z.ToString());
				lasertracer.GetComponent<LaserDrawing>().endPoint = hit.point;
				//lasertracer.GetComponent<LaserDrawing>().followStarter = playerCamera.gameObject;
			}
		}
		waitTilNextFire = Time.time + delayBetweenShots;
	}

	void FireStungun (bool silent) {
		if (!silent) { SFX.clip = SFXStungunFire; SFX.Play(); }

	}
}