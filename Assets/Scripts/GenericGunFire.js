#pragma strict

var fireSpeed : float = 8000;
@HideInInspector
var waitTilNextFire : float = 0;
var muzzleDistance : float = 0.10;
var bullet : GameObject;
var bulletSpawn : GameObject;

function Update () {
	if (Input.GetButton("Fire1")) {
		if (waitTilNextFire <= 0) {
			if (bullet)
				Instantiate(bullet,bulletSpawn.transform.position + (bulletSpawn.transform.forward * -muzzleDistance), (bulletSpawn.transform.rotation * Quaternion.Euler(90,0,0)));
			waitTilNextFire = 1;
		}
	}
	waitTilNextFire -= Time.deltaTime * fireSpeed;
}