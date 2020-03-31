using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Hopper : MonoBehaviour {
	public bool visitWaypointsRandomly = false;
	public bool inFront = false;
	public bool goIntoPain = false;
	public int index = 14; // NPC reference index for looking up constants in tables in Const.cs
	public enum collisionType{None,Box,Capsule,Sphere,Mesh};
	public collisionType normalCollider;
	public collisionType corpseCollider;
	public Const.aiState currentState;
	public float fieldOfViewAngle = 160f;
	public float fieldOfViewAttack = 80f;
	public float distToSeeWhenBehind = 2.5f;
	public float sightRange = 50f;
	public float walkSpeed = 0.8f;
	public float runSpeed = 0.8f;
	public float proj1Range = 17.92f;
	public float rangeToEnemy = 0f;
	public float timeToPain = 2f; // time between going into pain animation
	public float timeBetweenPain = 5f;
	public float timeTillDead = 1.5f;
	public float timeBetweenProj1 = 1f;
	public float changeEnemyTime = 3f; // Time before enemy will switch to different attacker
	public float timeToFire = 1f;
	public float delayBeforeFire = 0.5f;
	public float firingFinished;
	public float attackFinished;
	public float fireDelayFinished;
	public float yawSpeed = 50f;
	public float hopForce = 5f;
	public Vector3 navigationTargetPosition;
	public Vector3 idealTransformForward;
	[HideInInspector]
	public GameObject attacker;
	public GameObject enemy;
	public GameObject firePoint;
	public GameObject normalBody;
	public GameObject collisionAid1;
	public GameObject collisionAid2;
	public GameObject deathBody;
	//public GameObject[] gibs;
	public Transform[] walkWaypoints; // point(s) for NPC to walk to when roaming or patrolling
	public AudioClip SFXIdle;
	public AudioClip SFXFootstep;
	public AudioClip SFXSightSound;
	public AudioClip SFXAttack1;
	public AudioClip SFXPainClip;
	public AudioClip SFXDeathClip;
	public AudioClip SFXInspect;
	public AudioClip SFXInteracting;
	public GameObject muzzleBurst;
	public bool asleep;

	private bool hasSFX;
	private bool firstSighting;
	private bool dyingSetup;
	private bool painMade;
	private bool hadEnemy;
	private bool dying;
	private bool hopDone;
	private int nextPointIndex;
	//private int currentWaypoint;
	private enum painSelection{Pain1,Pain2,Pain3};
	private painSelection currentPainAnim;
	private float idleTime;
	//private float timeTillEnemyChangeFinished;
	private float timeTillDeadFinished;
	//private float timeTillPainFinished;
	private float painFinished;
	private float tick;
	private float tickFinished;
	private float lastHealth;
	private Vector3 fireEndPoint;
	private AudioSource SFX;
	//private NavMeshAgent nav;
	private Animation anim;
	private Rigidbody rbody;
	private HealthManager healthManager;
	private BoxCollider boxCollider;
	private CapsuleCollider capsuleCollider;
	private SphereCollider sphereCollider;
	private MeshCollider meshCollider;
	private bool justpaused = false;

	void Start () {
		healthManager = GetComponent<HealthManager>();
		rbody = GetComponent<Rigidbody>();
		SFX = GetComponent<AudioSource>();
		anim = GetComponent<Animation>();

		//Setup colliders for NPC and its corpse
		if (normalCollider == corpseCollider) {
			Debug.Log("ERROR: normalCollider and corpseCollider cannot be the same on NPC!");
			return;
		}
		switch(normalCollider) {
		case collisionType.Box: boxCollider = GetComponent<BoxCollider>(); boxCollider.enabled = true; break;
		case collisionType.Sphere: sphereCollider = GetComponent<SphereCollider>(); sphereCollider.enabled = true; break;
		case collisionType.Mesh: meshCollider = GetComponent<MeshCollider>(); meshCollider.enabled = true; break;
		case collisionType.Capsule: capsuleCollider = GetComponent<CapsuleCollider>(); capsuleCollider.enabled = true; break;
		}
		switch(corpseCollider) {
		case collisionType.Box: boxCollider = GetComponent<BoxCollider>(); boxCollider.enabled = false; break;
		case collisionType.Sphere: sphereCollider = GetComponent<SphereCollider>(); sphereCollider.enabled = false; break;
		case collisionType.Mesh: meshCollider = GetComponent<MeshCollider>(); meshCollider.enabled = false; break;
		case collisionType.Capsule: capsuleCollider = GetComponent<CapsuleCollider>(); capsuleCollider.enabled = false; break;
		}

		currentState = Const.aiState.Idle;
		//currentWaypoint = 0;
		tick = 0.05f; // Think every 0.05f seconds to save on CPU
		idealTransformForward = transform.forward;
		enemy = null;
		attacker = null;

		firstSighting = true;
		goIntoPain = false;
		dyingSetup = false;
		hopDone = false;

		idleTime = Time.time + Random.Range(3f,10f);
		attackFinished = Time.time + 1f;
		tickFinished = Time.time + tick + Random.value;
		timeTillDeadFinished = Time.time;
		firingFinished = Time.time;
		painFinished = Time.time;
		fireDelayFinished = Time.time;
		currentPainAnim = painSelection.Pain1;

		if (SFX == null) {
			Debug.Log ("WARNING: No audio source for npc at: " + transform.position.x.ToString () + ", " + transform.position.y.ToString () + ", " + transform.position.z + ".");
			hasSFX = false;
		} else {
			hasSFX = true;
		}
		
		if (healthManager.health > 0) {
			if (walkWaypoints.Length > 0 && walkWaypoints[0] != null) {
				currentState = Const.aiState.Walk; // If waypoints are set, start walking to them
			} else {
				currentState = Const.aiState.Idle; // Default to idle
			}
		}

		deathBody.SetActive(false);
		normalBody.SetActive(true);
		collisionAid1.SetActive(true);
		collisionAid2.SetActive(true);
	}

	void Update () {
		if (PauseScript.a != null && PauseScript.a.Paused()) {
			if (justpaused == false) {
				foreach (AnimationState state in anim) {
					state.speed = 0f;
				}
				justpaused = true;
			}
			return; // don't do any checks or anything else...we're paused!
		} else {
			if (justpaused) {
				foreach (AnimationState state in anim) {
					state.speed = 1f;
				}
				justpaused = false;
			}
		}

		// Only think every tick seconds to save on CPU and prevent race conditions
		if (tickFinished < Time.time) {
			Think();
			tickFinished = Time.time + tick;
		}
	}

	void Think () {
		CheckAndUpdateState ();

		switch (currentState) {
		case Const.aiState.Idle: 			Idle(); 		break;
		//case Const.aiState.Walk:	 		Walk(); 		break;
		case Const.aiState.Run: 			Run(); 			break;
		case Const.aiState.Attack1: 		Attack(); 		break;
		case Const.aiState.Pain: 			Pain();			break;
		case Const.aiState.Dying: 			Dying(); 		break;
		default: 							break;
		}
	}

	// All state changes go in here.  Check for stuff.  See what we need to be doing.  Need to change?
	void CheckAndUpdateState() {
		if (currentState == Const.aiState.Dead)	return;

		// Check to see if we got hurt
		if (healthManager.health < lastHealth) {
			if (healthManager.health <= 0) {
				currentState = Const.aiState.Dying;
				return;
			}

			lastHealth = healthManager.health;
			// Make initial sight sound if we aren't running, attacking, or already in pain
			if (currentState != Const.aiState.Run || currentState != Const.aiState.Attack1 || currentState != Const.aiState.Attack2 || currentState != Const.aiState.Attack3 || currentState != Const.aiState.Pain)
				SightSound ();
			
			enemy = healthManager.attacker;
			currentState = Const.aiState.Run;
			return;
		} else {
			lastHealth = healthManager.health;
		}

		if (asleep) return; // don't check for an enemy, we are sleeping! shh!!

		// Take a look around for enemies
		if (enemy == null) {
			// we don't have an enemy yet so let's look to see if we can see one
			if (CheckIfPlayerInSight ()) {
				currentState = Const.aiState.Run; // quit standing around and start fighting
				return;
			}
		} else {
			// We have an enemy now what...
			// Oh can we still see it?
			// Can I shoot it?
			// please??
			// Can I melee it?
			// cherry on top?
			if (anim.IsPlaying ("Hop")) {
				if (anim ["Hop"].time > (0.69f * 1.55f) || anim ["Hop"].time < (0.09f)) {
					if (CheckIfEnemyInSight ()) {
						float dist = Vector3.Distance (transform.position, enemy.transform.position);
						// See if we are close enough to attack
						if (dist < proj1Range) {
							if (attackFinished < Time.time) {
								if (CheckIfEnemyInFront (enemy)) {
									inFront = true;
									firingFinished = Time.time + timeBetweenProj1;
									fireDelayFinished = Time.time + delayBeforeFire;
									fireEndPoint = enemy.transform.position;
									currentState = Const.aiState.Attack1;
									return;
								} else {
									inFront = false;
								}
							}
						}
					}
				} else {
					inFront = false;
				}
			}
		}
	}

	void Idle() {
		// Set animation state
		anim["Idle"].wrapMode = WrapMode.Loop;
		anim.Play("Idle");

		if (asleep) {
			anim["Idle"].speed = 0f;
		} else {
			anim["Idle"].speed = 1f;
		}

		// Play an idle sound if the idle sound timer has timed out
		if ((idleTime < Time.time) && (SFXIdle != null)) {
			SFX.PlayOneShot(SFXIdle); // play idle sound
			idleTime = Time.time + Random.Range(3f,10f); // reset the timer to keep idle sounds from playing repetitively
		}
	}

	void SightSound() {
		if (firstSighting) {
			firstSighting = false;
			if (hasSFX) SFX.PlayOneShot(SFXSightSound);
		}
	}


	void AI_Face(GameObject goalLocation) {
		Vector3 dir = (goalLocation.transform.position - transform.position).normalized;
		dir.y = 0f;
		Quaternion lookRot = Quaternion.LookRotation(dir,Vector3.up);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRot, tick * yawSpeed); // rotate as fast as we can towards goal position
	}

	void Run() {
		// Set animation state
		anim["Hop"].wrapMode = WrapMode.Loop;
		anim.Play ("Hop");

		AI_Face (enemy); // turn and face your executioner

		// move it move it
		if (anim ["Hop"].time > (0.09f*1.55f)) {
			if (!hopDone) {
				hopDone = true;
				rbody.AddForce (transform.forward * hopForce); // moving forward!
			}
		} else {
			hopDone = false;
		}
	}



	void Attack() {
		if (fireDelayFinished > Time.time) {
			anim.Play ("Idle");
			anim ["Hop"].time = 0f;
		}

		if (fireDelayFinished < Time.time) {
			anim.Play ("Shoot");

			AI_Face (enemy);

			if (firingFinished < Time.time) {
				currentState = Const.aiState.Run;
				return;
			}

			if (attackFinished < Time.time && firingFinished > Time.time && anim ["Shoot"].time > 0.1f) {
				if (SFXAttack1 != null)
					SFX.PlayOneShot (SFXAttack1);

				if (muzzleBurst != null) muzzleBurst.SetActive(true);
				attackFinished = Time.time + timeBetweenProj1;
				bool useBlood = false;
				DamageData damageData = new DamageData ();
				damageData.ownerIsNPC = true;
				RaycastHit tempHit = new RaycastHit ();
				Vector3 tempvec = Vector3.Normalize (fireEndPoint - firePoint.transform.position);
				Ray tempRay = new Ray (firePoint.transform.position, tempvec);
				if (Physics.Raycast (tempRay, out tempHit, 200f)) {
					HealthManager tempHM = tempHit.transform.gameObject.GetComponent<HealthManager> ();
					if (tempHit.transform.gameObject.GetComponent<HealthManager> () != null) {
						useBlood = true;
					}

					GameObject impact = Const.a.GetObjectFromPool (Const.PoolType.HopperImpact);
					if (impact != null) {
						impact.transform.position = tempHit.point;
						impact.transform.rotation = Quaternion.FromToRotation (Vector3.up, tempHit.normal);
						impact.SetActive (true);
					}

					// Determine blood type of hit target and spawn corresponding blood particle effect from the Const.Pool
					GameObject impact2 = Const.a.GetObjectFromPool (Const.PoolType.SparksSmall);
					if (useBlood)
						impact2 = GetImpactType (tempHM);
					if (impact2 != null) {
						impact2.transform.position = tempHit.point;
						impact2.transform.rotation = Quaternion.FromToRotation (Vector3.up, tempHit.normal);
						impact2.SetActive (true);
					}

					damageData.ownerIsNPC = true;
					damageData.hit = tempHit;
					damageData.attacknormal = Vector3.Normalize (firePoint.transform.position - fireEndPoint);
					damageData.damage = Const.a.damagePerHitForWeapon [14];
					damageData.damage = Const.a.GetDamageTakeAmount(damageData); //Added this line since the damage amount wasn't getting parsed by GetDamageTakeAmount to account for damages properly
					if (tempHit.transform.gameObject.GetComponent<HealthManager>() != null) tempHit.transform.gameObject.GetComponent<HealthManager>().TakeDamage(damageData);
					
					GameObject dynamicObjectsContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
					if (dynamicObjectsContainer == null) return; //didn't find current level
					GameObject lasertracer = Instantiate(Const.a.useableItems[105],transform.position,Quaternion.identity) as GameObject;
					lasertracer.transform.SetParent(dynamicObjectsContainer.transform,true);
					lasertracer.GetComponent<LaserDrawing> ().startPoint = firePoint.transform.position;
					lasertracer.GetComponent<LaserDrawing> ().endPoint = tempHit.point;
					lasertracer.SetActive(true);
				}
			}
		}
	}
		
	void Pain() {
		if (painFinished < Time.time) {
			if (SFXPainClip != null) SFX.PlayOneShot(SFXPainClip);

			float r = Random.Range(0,1);
			if (r < 0.34) {
				currentPainAnim = painSelection.Pain1;
			} else {
				if (r < 0.67) {
					currentPainAnim = painSelection.Pain2;
				} else {
					currentPainAnim = painSelection.Pain3;
				}
			}

			switch (currentPainAnim) {
			case painSelection.Pain1:
				anim.Play ("Pain1");
				break;
			case painSelection.Pain2:
				anim.Play ("Pain2");
				break;
			case painSelection.Pain3:
				anim.Play ("Pain3");
				break;
			}
			painFinished = Time.time + timeBetweenPain;
		}
	}

	void Dying() {
		if (!dyingSetup) {
			dyingSetup = true;
			SFX.PlayOneShot(SFXDeathClip);

			// Turn off normal NPC collider and enable corpse collider for searching
			switch(normalCollider) {
			case collisionType.Box: boxCollider = GetComponent<BoxCollider>(); boxCollider.enabled = false; break;
			case collisionType.Sphere: sphereCollider = GetComponent<SphereCollider>(); sphereCollider.enabled = false; break;
			case collisionType.Mesh: meshCollider = GetComponent<MeshCollider>(); meshCollider.enabled = false; break;
			case collisionType.Capsule: capsuleCollider = GetComponent<CapsuleCollider>(); capsuleCollider.enabled = false; break;
			}
			switch(corpseCollider) {
			case collisionType.Box: boxCollider = GetComponent<BoxCollider>(); boxCollider.enabled = true; boxCollider.isTrigger = false; break;
			case collisionType.Sphere: sphereCollider = GetComponent<SphereCollider>(); sphereCollider.enabled = true; sphereCollider.isTrigger = false; break;
			case collisionType.Mesh: meshCollider = GetComponent<MeshCollider>(); meshCollider.enabled = true; meshCollider.isTrigger = false; break;
			case collisionType.Capsule: capsuleCollider = GetComponent<CapsuleCollider>(); capsuleCollider.enabled = true; capsuleCollider.isTrigger = false; break;
			}

			collisionAid1.SetActive(false);
			collisionAid2.SetActive(false);
			normalBody.SetActive(false);
			deathBody.SetActive(true);
			//for (int i=0;i<gibs.Length;i++) {
			//	gibs[i].SetActive(true);
			//	gibs[i].GetComponent<Rigidbody>().WakeUp();
			//}
			gameObject.tag = "Searchable"; // Enable searching
			gameObject.layer = 13; // set to Corpse layer for physics

			//nav.speed = nav.speed * 0.5f; // half the speed while collapsing or whatever
			timeTillDeadFinished = Time.time + timeTillDead; // wait for death animation to finish before going into Dead()
		}

		if (timeTillDeadFinished < Time.time) {
			//ai_dead = true;
			//ai_dying = false;
			//nav.isStopped = true; // Stop moving
			//rbody.isKinematic = true;
			currentState = Const.aiState.Dead;
		}
	}

	// Sub functions
	bool CheckIfEnemyInFront (GameObject target) {
		Vector3 vec = Vector3.Normalize(target.transform.position - transform.position);
		float dot = Vector3.Dot(vec,transform.forward);
		if (dot > 0.800) return true; // enemy is within ±18° of forward facing vector
		return false;
	}


	bool CheckIfEnemyInSight() {
		Vector3 checkline = enemy.transform.position - transform.position; // Get vector line made from enemy to found player
		RaycastHit hit;
		if(Physics.Raycast(transform.position + transform.up, checkline.normalized, out hit, sightRange)) {
			if (hit.collider.gameObject == enemy) {
				if (CheckIfEnemyInFront(enemy))
					return true;
			}
		}
		return false;
	}

	bool CheckIfPlayerInSight() {
		GameObject tempent = Const.a.player1.GetComponent<PlayerReferenceManager>().playerCapsule;
		Vector3 checkline = Vector3.Normalize(tempent.transform.position - transform.position); // Get vector line made from enemy to found player
		float dist = Vector3.Distance(tempent.transform.position,transform.position);  // Get distance between enemy and found player
		if (dist > sightRange) return false; // don't waste time doing raycasts if we won't be able to see them anyway
		dist = dist + 0.1f;  // just in case player is a hair over the distance for some reason
		if (dist < distToSeeWhenBehind) {
			SightSound();
			enemy = tempent;
			return true; // time to turn around and face your executioner!
		}

		float dot = Vector3.Dot(checkline,transform.forward.normalized);
		// enemy is within ±81° of forward facing vector
		if (dot > 0.10f) {
			RaycastHit hit;
			if(Physics.Raycast(transform.position, checkline, out hit, sightRange)) {
				if (hit.collider.gameObject == tempent) {
					if (firstSighting) {
						firstSighting = false;
						SightSound();
					}
					enemy = tempent;
					return true; // time to fight!
				}
			}
		}
		return false;
	}

	GameObject GetImpactType (HealthManager hm) {
		if (hm == null) return Const.a.GetObjectFromPool(Const.PoolType.SparksSmall);
		switch (hm.bloodType) {
		case HealthManager.BloodType.None: return Const.a.GetObjectFromPool(Const.PoolType.SparksSmall);
		case HealthManager.BloodType.Red: return Const.a.GetObjectFromPool(Const.PoolType.BloodSpurtSmall);
		case HealthManager.BloodType.Yellow: return Const.a.GetObjectFromPool(Const.PoolType.BloodSpurtSmallYellow);
		case HealthManager.BloodType.Green: return Const.a.GetObjectFromPool(Const.PoolType.BloodSpurtSmallGreen);
		case HealthManager.BloodType.Robot: return Const.a.GetObjectFromPool(Const.PoolType.SparksSmallBlue);
		}

		return Const.a.GetObjectFromPool(Const.PoolType.SparksSmall);
	}

	public void Alert(UseData ud) {
		GameObject playr1 = Const.a.player1;
		GameObject playr2 = Const.a.player2;
		GameObject playr3 = Const.a.player3;
		GameObject playr4 = Const.a.player4;

		if (playr1 == null) { Debug.Log("WARNING: NPC Alert() check - no host player 1."); return; }  // No host player
		if (playr1 != null) {playr1 = playr1.GetComponent<PlayerReferenceManager>().playerCapsule;}
		if (playr2 != null) {playr2 = playr2.GetComponent<PlayerReferenceManager>().playerCapsule;}
		if (playr3 != null) {playr3 = playr3.GetComponent<PlayerReferenceManager>().playerCapsule;}
		if (playr4 != null) {playr4 = playr4.GetComponent<PlayerReferenceManager>().playerCapsule;}

		GameObject tempent = null;

		for (int i=0;i<4;i++) {
			tempent = null;
			// Cycle through all the players to see if we can see anybody.  Defaults to earlier joined players.
			if (playr1 != null && i == 0) tempent = playr1;
			if (playr2 != null && i == 1) tempent = playr2;
			if (playr3 != null && i == 2) tempent = playr3;
			if (playr4 != null && i == 4) tempent = playr4;
			if (ud.owner == tempent) { enemy = tempent; } else { if (tempent != enemy) enemy = tempent;}
		}
	}

	public void AwakeFromSleep(UseData ud) {
		asleep = false;
		Alert(ud);
	}
}
