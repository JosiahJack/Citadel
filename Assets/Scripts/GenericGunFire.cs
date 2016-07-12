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
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	private float clipEnd;

	void  Update() {
		/*if (Input.GetButton("Fire1")) {
			if (waitTilNextFire <= 0) {
				if (bullet)
					Instantiate(bullet,bulletSpawn.transform.position + (bulletSpawn.transform.forward * -muzzleDistance), (bulletSpawn.transform.rotation * Quaternion.Euler(90,0,0)));
				waitTilNextFire = 1;
			}
		}*/

		if (!GUIState.isBlocking && !playerCamera.GetComponent<MouseLookScript>().holdingObject) {
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
			if (hit.transform.gameObject.GetComponent<EnemyHealth>() != null) {
				hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
			}
			waitTilNextFire = Time.time + fireSpeed;
		}
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