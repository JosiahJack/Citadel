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
	public DamageData damageData;
	private RaycastHit tempHit;
	private Vector3 tempVec;

	void Awake () {
		damageData = new DamageData();
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
		damageData.ResetDamageData(damageData);
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
		playerCamera.GetComponent<MouseLookScript>().Recoil(index);
		if (currentWeapon.weaponIsAlternateAmmo) {
			waitTilNextFire = Time.time + Const.a.delayBetweenShotsForWeapon2[index];
		} else {
			waitTilNextFire = Time.time + Const.a.delayBetweenShotsForWeapon[index];
		}
	}

	//float centerx = (Screen.width/2);
	//float centery = (Screen.height/2);
	//float x,y;
	//if (MouseCursor.cursorX > centerx) {x = MouseCursor.cursorX-centerx;} else {x = centerx-MouseCursor.cursorX;}
	//if (MouseCursor.cursorY > centery) {y = MouseCursor.cursorY-centery;} else {y = centery-MouseCursor.cursorY;}
	//float angx = (Mathf.Atan2(-damageData.range,-x)/Mathf.PI*180f) + 180f;
	//float angy = (Mathf.Atan2(-damageData.range,-y)/Mathf.PI*180f) + 180f;
	//Vector3 aimdir = new Vector3(angx,angy,0f);
	//if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(MouseCursor.cursorX,Screen.height - MouseCursor.cursorY,playerCamera.nearClipPlane)), out hit, fireDistance)) {

	bool DidRayHit () {
		tempVec = new Vector3 (playerCamera.transform.position.x, playerCamera.transform.position.y + verticalOffset, playerCamera.transform.position.z);

		Ray tempray = playerCamera.ScreenPointToRay(new Vector3(MouseCursor.cursorX,Screen.height - MouseCursor.cursorY,playerCamera.nearClipPlane));
		if (Physics.Raycast(tempVec, tempray.direction, out tempHit, fireDistance)) {
			return true;
		}
		return false;
		//if (Physics.Raycast(playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center), out tempHit, fireDistance)) {
		//}
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
		if (Physics.Raycast(playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center), out hit, fireDistance)) {
			//drawDebugLine(playerCamera.transform.position,hit.point,Color.cyan,10f);
			GameObject impact = Const.a.GetObjectFromPool(Const.PoolType.DartImpacts);
			if (impact != null) {
				impact.transform.position = hit.point;
				impact.SetActive(true);
			}
			if (hit.transform.gameObject.tag == "NPC") {
				damageData.isOtherNPC = true;
			} else {
				damageData.isOtherNPC = false;
			}
			damageData.hit = hit;
			damageData.attacknormal = playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center).direction;
			damageData.damage = Const.a.damagePerHitForWeapon[2];
			//hit.transform.gameObject.SendMessage("TakeDamage", damageData,SendMessageOptions.DontRequireReceiver);
			HealthManager hm = hit.transform.gameObject.GetComponent<HealthManager>();
			if (hm == null) return;
			float take = Const.a.GetDamageTakeAmount(damageData);
			hm.health = hm.health - take;
		}
	}

	void FireRapier (bool silent) {
		if (!silent) { SFX.clip = SFXRapierMiss; SFX.Play(); }
	}
		
	void FirePipe (bool silent) {
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center), out hit, fireDistance)) {
			anim.Play("Attack2");
			if (!silent) {
				SFX.clip = SFXPipeHit;
				SFX.Play();
			}
			hit.transform.gameObject.SendMessage("TakeDamage", damageData,SendMessageOptions.DontRequireReceiver);
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
				impact.SetActive(true);
			}
			damageData.hit = tempHit;
			damageData.attacknormal = playerCamera.ScreenPointToRay(MouseCursor.drawTexture.center).direction;
			damageData.damage = Const.a.damagePerHitForWeapon[14];
			tempHit.transform.gameObject.SendMessage("TakeDamage",damageData,SendMessageOptions.DontRequireReceiver);
			GameObject lasertracer = Const.a.GetObjectFromPool(Const.PoolType.LaserLines);
			if (lasertracer != null) {
				lasertracer.SetActive(true);
				lasertracer.GetComponent<LaserDrawing>().startPoint = tempVec;
				lasertracer.GetComponent<LaserDrawing>().endPoint = tempHit.point;
			}
		}
	}

	void FireStungun (bool silent) {
		if (!silent) { SFX.clip = SFXStungunFire; SFX.Play(); }

	}
}