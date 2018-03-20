using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {
	public float health = -1f; // current health
	public float maxhealth; // maximum health
	public float gibhealth; // point at which we splatter
	public bool gibOnDeath = false; // used for things like crates to "gib" and shatter
	public bool gibCorpse = true;
	public bool isNPC = false;
	public bool isObject = false;
	public bool applyImpact = false;
	public bool damagingGetsAttention = true;
	public int[] gibIndices;
	public int index;
	public GameObject attacker;
	public Const.PoolType deathFX;
	public enum BloodType {None,Red,Yellow,Green,Robot};
	public BloodType bloodType;

	private bool initialized = false;
	private bool deathDone = false;
	private AIController aic;
	private Rigidbody rbody;
	private MeshCollider meshCol;
	private BoxCollider boxCol;
	private SphereCollider sphereCol;
	private CapsuleCollider capCol;
	//private SearchableItem searchItems;

	void Awake () {
		initialized = false;
		deathDone = false;
		rbody = GetComponent<Rigidbody>();
		meshCol = GetComponent<MeshCollider>();
		boxCol = GetComponent<BoxCollider>();
		sphereCol = GetComponent<SphereCollider>();
		capCol = GetComponent<CapsuleCollider>();
		if (maxhealth < 1) maxhealth = health;
		//if (isNPC) {
		//	aic = GetComponent<AIController>();
		//	if (aic == null) Debug.Log("BUG: No AIController script on NPC!");
		//}
		attacker = null;
		//searchItems = GetComponent<SearchableItem>();
	}

	void Update () {
		if (health <= 0) {
			if (!deathDone && isObject) ObjectDeath(null);
			return;
		}

		if (!initialized) {
			initialized = true;
			Const.a.RegisterObjectWithHealth(this);
		}
			
		if (health > maxhealth) health = maxhealth; // Don't go past max.  Ever.
	}

	void Gib() {
		//for (int i=0;i<gibIndices.Length;i++) {
		//	if (gibIndices[i] 
		//}
	}

	public void TakeDamage(DamageData dd) {
		if (health <= 0) return;
		health -= dd.damage;
		attacker = dd.owner;
		if (applyImpact && rbody != null) {
			rbody.AddForce(dd.impactVelocity*dd.attacknormal,ForceMode.Impulse);
		}

		if (health <= 0f) {
			if (isObject) ObjectDeath(null);
		}
	}

	public void ObjectDeath(AudioClip deathSound) {
		deathDone = true;

		// Disable collision
		if (boxCol != null) boxCol.enabled = false;
		if (meshCol != null) meshCol.enabled = false;
		if (sphereCol != null) sphereCol.enabled = false;
		if (capCol != null) capCol.enabled = false;

		// Enabel death effects (e.g. explosion particle effect)
		if (deathFX != Const.PoolType.LaserLines) {
			GameObject explosionEffect = Const.a.GetObjectFromPool(deathFX);
			if (explosionEffect != null) {
				explosionEffect.SetActive(true);
				explosionEffect.transform.position = transform.position;
				// TODO: Do I need more than one temporary audio entity for this sort of thing?
				if (deathSound != null) {
					GameObject tempAud = GameObject.Find("TemporaryAudio");
					tempAud.transform.position = transform.position;
					AudioSource aS = tempAud.GetComponent<AudioSource>();
					aS.PlayOneShot(deathSound);
				}
				gameObject.SetActive(false);
			}
		}
	}
}
