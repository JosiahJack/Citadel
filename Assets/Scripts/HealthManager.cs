using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {
	public float health = -1f; // current health
	public float maxhealth; // maximum health
	public float gibhealth; // point at which we splatter
	public bool gibOnDeath = false; // used for things like crates to "gib" and shatter
	public bool gibCorpse = false;
    public bool vaporizeCorpse = true;
	public bool isNPC = false;
	public bool isObject = false;
	public bool applyImpact = false;
	public int[] gibIndices;
	public GameObject[] gibObjects;
	public int index;
	public int securityAmount;
	public GameObject attacker;
	public Const.PoolType deathFX;
	public enum BloodType {None,Red,Yellow,Green,Robot};
	public BloodType bloodType;
	public GameObject[] targetOnDeath;
	public AudioClip backupDeathSound;
    public bool debugMessages = false;

	private bool initialized = false;
	private bool deathDone = false;
	private AIController aic;
	private Rigidbody rbody;
	private MeshCollider meshCol;
	private BoxCollider boxCol;
	private SphereCollider sphereCol;
	private CapsuleCollider capCol;
    private Vector3 tempVec;
    private float tempFloat;

	void Awake () {
		initialized = false;
		deathDone = false;
		rbody = GetComponent<Rigidbody>();
		meshCol = GetComponent<MeshCollider>();
		boxCol = GetComponent<BoxCollider>();
		sphereCol = GetComponent<SphereCollider>();
		capCol = GetComponent<CapsuleCollider>();
		if (maxhealth < 1) maxhealth = health;

		if (isNPC) {
			aic = GetComponent<AIController>();
			if (aic == null) Debug.Log("BUG: No AIController script on NPC!");
            index = aic.index;

            // TODO: Uncomment this for final game
            //if (Const.a.difficultyCombat == 0) {
            //	maxhealth = 1;
            //	health = maxhealth;
            //}
        }
        attacker = null;
		//searchItems = GetComponent<SearchableItem>();
	}

	void Update () {
		if (!initialized) {
			initialized = true;
			Const.a.RegisterObjectWithHealth(this);
		}
			
		if (health > maxhealth) health = maxhealth; // Don't go past max.  Ever.
	}

	public void TakeDamage(DamageData dd) {
		if (health <= 0) return;
        tempFloat = health;
		health -= dd.damage;
        if (debugMessages) Const.sprint("Health before: " + tempFloat.ToString() + "| Health after: " + health.ToString(), Const.a.allPlayers);

        if (aic != null) aic.goIntoPain = true;
		attacker = dd.owner;
		if (applyImpact && rbody != null) {
			rbody.AddForce(dd.impactVelocity*dd.attacknormal,ForceMode.Impulse);
		}

        if (health <= 0f) {
            if (!deathDone) {
                if (isObject) ObjectDeath(null);

                if (isNPC) NPCDeath(null);

                if (targetOnDeath != null) {
                    if (targetOnDeath.Length > 0) {
                        UseData ud = new UseData();
                        ud.owner = Const.a.allPlayers;
                        for (int i = 0; i < targetOnDeath.Length; i++) {
                            targetOnDeath[i].SendMessageUpwards("Targetted", ud);
                        }
                    }
                }
            } else {
                if (vaporizeCorpse && health < (0 - (maxhealth / 2))) {
                    GetComponent<MeshRenderer>().enabled = false;
                    GameObject explosionEffect = Const.a.GetObjectFromPool(Const.PoolType.Vaporize);
                    if (explosionEffect != null) {
                        explosionEffect.SetActive(true);
                        tempVec = transform.position;
                        tempVec.y += aic.verticalViewOffset;
                        explosionEffect.transform.position = tempVec; // put vaporization effect at raycast center
                    }
                }
            }
		}
	}

	public void NPCDeath (AudioClip deathSound) {
		switch (index) {
		case 0:
			GetComponent<MeshRenderer> ().enabled = false;
			break;
		}

		// Enable death effects (e.g. explosion particle effect)
		if (deathFX != Const.PoolType.LaserLines) {
			GameObject explosionEffect = Const.a.GetObjectFromPool(deathFX);
			if (explosionEffect != null) {
				explosionEffect.SetActive(true);
				explosionEffect.transform.position = transform.position;
				// TODO: Do I need more than one temporary audio entity for this sort of thing?
				if (deathSound != null) {
					GameObject tempAud = GameObject.Find ("TemporaryAudio");
					tempAud.transform.position = transform.position;
					AudioSource aS = tempAud.GetComponent<AudioSource> ();
					aS.PlayOneShot (deathSound);
				} else {
					GameObject tempAud = GameObject.Find ("TemporaryAudio");
					tempAud.transform.position = transform.position;
					AudioSource aS = tempAud.GetComponent<AudioSource> ();
					aS.PlayOneShot (backupDeathSound);
				}
			}
		}

        if (gibOnDeath) Gib();
	}

    void Gib() {
        if (gibObjects[0] != null) {
            for (int i = 0; i < gibObjects.Length; i++) {
                gibObjects[i].SetActive(true); // turn on all the gibs to fall apart
                //TODO: add force to gibs?
            }
        }
    }

	public void ObjectDeath(AudioClip deathSound) {
		deathDone = true;

		// Disable collision
		if (boxCol != null) boxCol.enabled = false;
		if (meshCol != null) meshCol.enabled = false;
		if (sphereCol != null) sphereCol.enabled = false;
		if (capCol != null) capCol.enabled = false;

		if (securityAmount > 0)
			LevelManager.a.ReduceCurrentLevelSecurity (securityAmount);

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

			}
		}

		if (gibOnDeath) Gib();

		//gameObject.SetActive(false); // turn off the main object
		GetComponent<MeshRenderer>().enabled = false;
	}
}
