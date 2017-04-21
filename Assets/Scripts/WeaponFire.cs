using UnityEngine;
using System.Collections;

public class WeaponFire : MonoBehaviour {
	public float fireSpeed = 1.5f;
	[HideInInspector]
	public float waitTilNextFire = 0f;
	public float muzzleDistance = 0.10f;
	public float hitOffset = 0.2f;
	public float normalHitOffset = 0.2f;
	public float verticalOffset = -0.2f; // For laser beams
	public float fireDistance = 200f;
	public float damage = 1f;
	public bool isFullAuto = false;
	public bool berserkActive = false;
	public GameObject bullet;
	public GameObject impactEffect;
	//public GameObject bulletSpawn;
	public Camera playerCamera;
	public Camera gunCamera;
	public WeaponCurrent currentWeapon;
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	[SerializeField] private AudioClip SFXSparqClip = null; // assign in the editor
	[SerializeField] private AudioClip PipeMissClip = null; // assign in the editor
	[SerializeField] private AudioClip PipeHitClip = null; // assign in the editor
	private float clipEnd;
	public Animator anim;

	void  Update() {
		/*if (Input.GetButton("Fire1")) {
			if (waitTilNextFire <= 0) {
				if (bullet)
					Instantiate(bullet,bulletSpawn.transform.position + (bulletSpawn.transform.forward * -muzzleDistance), (bulletSpawn.transform.rotation * Quaternion.Euler(90,0,0)));
				waitTilNextFire = 1;
			}
		}*/
		if (!PauseScript.a.paused) {
			if (!GUIState.a.isBlocking && !playerCamera.GetComponent<MouseLookScript>().holdingObject) {
				if (GetInput.a.Attack() && waitTilNextFire < Time.time) {
				switch(currentWeapon.weaponIndex) {
					case 36:
						
						break;
					case 37:

						break;
					case 38:
						if (isFullAuto)
								FireDart(fireDistance, false, damage);
						else
								FireRaycastBullet(fireDistance, false,damage);
						break;
					case 39:

						break;
					case 40:

						break;
					case 41:

						break;
					case 42:
						FirePipe(2f,false,20f);
						break;
					case 43:

						break;
					case 44:

						break;
					case 45:

						break;
					case 46:

						break;
					case 47:

						break;
					case 48:

						break;
					case 49:

						break;
					case 50:
						FireSparq(100f,false,damage);
						break;
					case 51:

						break;
					}
				}
			}
		}
	}

	void FireDart (float dist, bool silent, float dmg) {
		FireRaycastBullet(dist,silent,dmg);
	}
	void FireRaycastBullet (float dist, bool silent, float specificdamage) {
		if (!silent) {
			SFX.clip = SFXClip;
			SFX.Play();
		}

		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, dist)) {
			//drawDebugLine(playerCamera.transform.position,hit.point,Color.cyan,10f);
			//Vector3 direction = playerCamera.transform.position - hit.point;
			//Instantiate(impactEffect,hit.point+(direction.normalized*hitOffset)+(hit.normal*normalHitOffset),Quaternion.identity);  //effect
			GameObject impact = Const.a.GetObjectFromPool(Const.PoolType.DartImpacts);
			if (impact != null) {
				impact.transform.position = hit.point;
				impact.SetActive(true);
			}
			hit.transform.gameObject.SendMessage("TakeDamage", specificdamage,SendMessageOptions.DontRequireReceiver);
			waitTilNextFire = Time.time + fireSpeed;
		}
	}

	void FireSparq (float dist, bool silent, float dmg) {
		if (!silent) {
			SFX.clip = SFXSparqClip;
			SFX.Play();
		}
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, dist)) {
			GameObject impact = Const.a.GetObjectFromPool(Const.PoolType.SparqImpacts);
			if (impact != null) {
				impact.transform.position = hit.point;
				Debug.Log("Setting Sparq Impact to x: " + hit.point.x.ToString() + ",y: " + hit.point.y.ToString() + ",z: " + hit.point.z.ToString());
				impact.SetActive(true);
			}
			hit.transform.gameObject.SendMessage("TakeDamage",dmg,SendMessageOptions.DontRequireReceiver);
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
		waitTilNextFire = Time.time + fireSpeed;
	}

	void FirePipe (float dist, bool silent, float specificdamage) {
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, dist)) {
			anim.Play("Attack2");
			if (!silent) {
				SFX.clip = PipeHitClip;
				SFX.Play();
			}

			if (berserkActive) specificdamage *= Const.a.berserkDamageMultiplier;
			hit.transform.gameObject.SendMessage("TakeDamage", specificdamage,SendMessageOptions.DontRequireReceiver);
			waitTilNextFire = Time.time + fireSpeed;
			return;
		}
		if (!silent) {
			SFX.clip = PipeMissClip;
			SFX.Play();
		}
		anim.Play("Attack1");
		waitTilNextFire = Time.time + fireSpeed;
	}
}