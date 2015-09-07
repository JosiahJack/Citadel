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
	//public GameObject bulletSpawn;
	public Camera playerCamera;
	public Camera gunCamera;
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor

	void  Update() {
		/*if (Input.GetButton("Fire1")) {
			if (waitTilNextFire <= 0) {
				if (bullet)
					Instantiate(bullet,bulletSpawn.transform.position + (bulletSpawn.transform.forward * -muzzleDistance), (bulletSpawn.transform.rotation * Quaternion.Euler(90,0,0)));
				waitTilNextFire = 1;
			}
		}*/

		if (!GUIState.isBlocking) {
			if (isFullAuto) {
				if (Input.GetButton("Fire1")) {
					if (waitTilNextFire <= Time.time) {
						FireRaycastBullet(fireDistance, false);
					}
				}
			} else {
				if (Input.GetButtonDown("Fire1")) {
					if (waitTilNextFire <= Time.time) {
						FireRaycastBullet(fireDistance, false);
					}
				}
			}

		}
		waitTilNextFire -= Time.deltaTime;
	}

	void FireRaycastBullet (float dist, bool silent) {
		if (!silent)
			SFX.PlayOneShot(SFXClip);

		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(gunCamera.ScreenPointToRay(Input.mousePosition), out hit, dist)) {
			Vector3 direction = gunCamera.transform.position - hit.point;
			Instantiate(bullet,hit.point+(direction.normalized*hitOffset)+(hit.normal*normalHitOffset),Quaternion.identity);  //effect
			if (hit.transform.gameObject.GetComponent<EnemyHealth>() != null) {
				hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
			}
			waitTilNextFire = Time.time + fireSpeed;
		}
	}
}