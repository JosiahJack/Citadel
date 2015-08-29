using UnityEngine;
using System.Collections;

public class GenericGunFire : MonoBehaviour {
	public float fireSpeed = 8000;
	[HideInInspector]
	public float waitTilNextFire = 0;
	public float muzzleDistance = 0.10f;
	public GameObject bullet;
	public GameObject bulletSpawn;
	public Camera playerCamera;

	void  Update() {
		/*if (Input.GetButton("Fire1")) {
			if (waitTilNextFire <= 0) {
				if (bullet)
					Instantiate(bullet,bulletSpawn.transform.position + (bulletSpawn.transform.forward * -muzzleDistance), (bulletSpawn.transform.rotation * Quaternion.Euler(90,0,0)));
				waitTilNextFire = 1;
			}
		}*/
		if (Input.GetButton("Fire1")) {
			if (waitTilNextFire <= 0) {
				RaycastHit hit = new RaycastHit();
				if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, 200f)) {
					Object bulletparticle = Instantiate(Resources.Load("Prefabs/bullethit1"),hit.point,Quaternion.identity);
					Destroy(bulletparticle,2);
					waitTilNextFire = 1;
				}
			}
		}
		waitTilNextFire -= Time.deltaTime * fireSpeed;
	}
}