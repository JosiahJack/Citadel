using UnityEngine;
using System.Collections;

public class GrenadeActivate : MonoBehaviour {
	public int constIndex = -1;
	public float nearforce;
	public float nearradius;
	public float damage = 11f;
	public float penetration = 20f;
	public float offense = 3f;
	public Const.AttackType attackType = Const.AttackType.Projectile;
	public bool proxSensed = false;
	public float tickTime = 0.8f;
	private GameObject explosionEffect;

	[HideInInspector]
	public float timeFinished; // save
	[HideInInspector]
	public bool explodeOnContact = false; // save
	[HideInInspector]
	public bool useTimer = false; // save
	public bool useProx = false; // save

	private ExplosionForce explosion;
	private BoxCollider boxCol;
	private SphereCollider sphereCol;
	private MeshCollider meshCol;
	private CapsuleCollider capCol;
	public AudioClip deathSound;
	public Const.PoolType explosionType = Const.PoolType.GrenadeFragExplosions;
	private GrenadeCurrent grenadeCurrent;
	public bool active = false;
	public bool test;
	public int testIndex;
	public float testTimer;
	private Rigidbody rbody;

	void Awake () {
		meshCol = GetComponent<MeshCollider>();
		boxCol = GetComponent<BoxCollider>();
		sphereCol = GetComponent<SphereCollider>();
		capCol = GetComponent<CapsuleCollider>();
		rbody = GetComponent<Rigidbody>();
	}

	public void AwakeFromLoad(float health) {
		if (health > 0) {
			rbody.useGravity = true;
			if (boxCol != null) boxCol.enabled = true;
			if (meshCol != null) meshCol.enabled = true;
			if (sphereCol != null) sphereCol.enabled = true;
			if (capCol != null) capCol.enabled = true;
		} else {
			rbody.useGravity = false;
			if (boxCol != null) boxCol.enabled = false;
			if (meshCol != null) meshCol.enabled = false;
			if (sphereCol != null) sphereCol.enabled = false;
			if (capCol != null) capCol.enabled = false;
		}
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (test) {
				if (!active) {
					Activate(testIndex,null);
				}
			}
			if (active) {
				if (useTimer) {
					if (timeFinished < PauseScript.a.relativeTime) {
						Explode();
					}
				}

				if (useProx) {
					if (proxSensed) {
						Explode();
					}
				}
			}
		}
	}

	public void Activate (int index, GrenadeCurrent gc) {
		grenadeCurrent = gc;
		switch(index) {
			case 7: explodeOnContact = true; break; // Fragmentation Grenade
			case 8: explodeOnContact = true; break; // Concussion Grenade
			case 9: explodeOnContact = true; break; // EMP Grenade
			case 10: if (test) { timeFinished = PauseScript.a.relativeTime + testTimer; } else { timeFinished = PauseScript.a.relativeTime + gc.earthShakerTimeSetting; } useTimer = true; break; // Earthshaker Bomb
			case 11: useProx = true; explodeOnContact = false; break; // Land Mine
			case 12: if (test) { timeFinished = PauseScript.a.relativeTime + testTimer; } else { timeFinished = PauseScript.a.relativeTime + gc.nitroTimeSetting; } useTimer = true; break; // Nitropack Explosive
			case 13: explodeOnContact = true; break; // Gas Grenade
			default: break;
		}
		active = true;
	}

	void OnCollisionStay(Collision col) {
		if (grenadeCurrent != null) {
			if (col.collider == grenadeCurrent.playerCapCollider) {
				//Debug.Log("Grenade self hit, ignoring.");
				return; // don't collide with the player who threw the grenade!
			}
		}

		if (explodeOnContact) {
			Explode();
			return;
		}
	}

	public void Explode() {
		// Disable collision
		rbody.useGravity = false;
		if (boxCol != null) boxCol.enabled = false;
		if (meshCol != null) meshCol.enabled = false;
		if (sphereCol != null) sphereCol.enabled = false;
		if (capCol != null) capCol.enabled = false;

		explosion = GetComponent<ExplosionForce>();
		if (explosion == null) { Debug.Log("Missing ExplosionForce script on GrenadeActivate!"); return; }

		DamageData dd = new DamageData();
		dd.damage = damage;
		//dd.owner = 
		dd.attackType = attackType;
		dd.penetration = penetration;
		dd.offense = offense;
		explosion.ExplodeInner(transform.position, nearforce, nearradius, dd);
		explosion.ExplodeOuter(transform.position);

		GameObject explosionEffect = Const.a.GetObjectFromPool(explosionType);
		if (explosionEffect != null) {
			explosionEffect.SetActive(true);
			explosionEffect.transform.position = transform.position;
			if (deathSound != null) {
				GameObject tempAud = Const.a.GetObjectFromPool(Const.PoolType.TempAudioSources);
				if (tempAud != null) {
					tempAud.transform.position = transform.position; // set temporary audiosource to right here
					tempAud.SetActive(true);
					AudioSource aS = tempAud.GetComponent<AudioSource>();
					if (aS != null) aS.PlayOneShot(deathSound);
				}
			}
		}

		Const.a.Shake(false,-1,-1);
		gameObject.SetActive(false);
	}
}
