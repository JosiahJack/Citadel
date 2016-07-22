using UnityEngine;
using System.Collections;

public class GenericGunFire : MonoBehaviour {
	public float fireSpeed = 1.5f;
	[HideInInspector]
	public float waitTilNextFire = 0f;
	public float muzzleDistance = 0.10f;
	public float hitOffset = 0.2f;
	public float normalHitOffset = 0.2f;
	public float fireDistance = 200f;
	public float damage = 1f;
	public bool isFullAuto = false;
	public GameObject bullet;
	public GameObject impactEffect;
	//public GameObject bulletSpawn;
	public Camera playerCamera;
	public Camera gunCamera;
	public WeaponCurrent currentWeapon;
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
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
				switch(currentWeapon.weaponIndex) {
					case 38:
						if (isFullAuto) {
							if (Input.GetButton("Fire1") && waitTilNextFire < Time.time) {
								FireDart(fireDistance, false, damage);
								break;
							}
						} else {
							if (Input.GetButtonDown("Fire1")) {
								if (waitTilNextFire < Time.time) {
									FireRaycastBullet(fireDistance, false,damage);
								}
							}
						}
						break;
					case 42:
						if (Input.GetButton("Fire1") && waitTilNextFire < Time.time) {
							FirePipe(2f,false,20f);
						}
					break;
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
			//drawMyLine(playerCamera.transform.position,hit.point,Color.cyan,10f);
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

	void FirePipe (float dist, bool silent, float specificdamage) {
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, dist)) {
			anim.Play("Attack2");
			if (!silent) {
				SFX.clip = PipeHitClip;
				SFX.Play();
			}
			hit.transform.gameObject.SendMessage("TakeDamage", specificdamage,SendMessageOptions.DontRequireReceiver);
			waitTilNextFire = Time.time + fireSpeed;
			return;
		}
		if (!silent) {
			SFX.clip = PipeMissClip;
			SFX.Play();
		}
		anim.Play("Attack1");
	}

	void drawMyLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		StartCoroutine( drawLine(start, end, color, duration));
	}

	IEnumerator drawLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		GameObject myLine = new GameObject ();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer> ();
		LineRenderer lr = myLine.GetComponent<LineRenderer> ();
		lr.material = new Material (Shader.Find ("Particles/Additive"));
		lr.SetColors (color,color);
		lr.SetWidth (0.05f,0.05f);
		lr.SetPosition (0, start);
		lr.SetPosition (1, end);
		yield return new WaitForSeconds(duration);
		GameObject.Destroy (myLine);
	}
	
}