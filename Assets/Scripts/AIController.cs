using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {
	public int index = 0; // NPC reference index for looking up constants in tables in Const.cs

	public enum collisionType{None,Box,Capsule,Sphere,Mesh};
	public collisionType normalCollider;
	public collisionType corpseCollider;
	public Const.aiState currentState;
	public GameObject enemy;
	public float yawspeed = 180f;
	public float fieldOfViewAngle = 180f;
	public float fieldOfViewAttack = 80f;
	public float distToSeeWhenBehind = 2.5f;
	public float sightRange = 50f;
	public float walkSpeed = 0.8f;
	public float runSpeed = 0.8f;
	public float meleeSpeed = 0.5f;
	public float proj1Speed = 0f;
	public float proj2Speed = 0f;
	public float meleeRange = 2f;
	public float proj1Range = 10f;
	public float proj2Range = 20f;
	public float attack3Force = 15f;
	public float attack3Radius = 10f;
	public float timeToPain = 2f; // time between going into pain animation
	public float timeBetweenPain = 5f;
	public float timeTillDead = 1.5f;
	public float timeTillMeleeDamage = 0.5f;
	public float timeTillMeleeDamage2 = 0.5f;
	public float timeBetweenMelee = 1.2f;
	public float timeBetweenProj1 = 1.5f;
	public float timeBetweenProj2 = 3f;
	public float changeEnemyTime = 3f; // Time before enemy will switch to different attacker
	public float impactMelee = 10f;
	public float impactMelee2 = 10f;
	public Vector3 explosionOffset;
	public AudioClip SFXIdle;
	public AudioClip SFXFootstep;
	public AudioClip SFXSightSound;
	public AudioClip SFXAttack1;
	public AudioClip SFXAttack2;
	public AudioClip SFXAttack3;
	public AudioClip SFXPainClip;
	public AudioClip SFXDeathClip;
	public AudioClip SFXInspect;
	public AudioClip SFXInteracting;
	public bool rememberEnemyIfOutOfSight = false;
	public bool walkPathOnStart = false;
	public bool visitWaypointsRandomly = false;
	public bool hasMelee = true;
	public bool hasProj1 = false;
	public bool hasProj2 = false;
	public bool twoMeleeHits = false;
	public Transform[] walkWaypoints; // point(s) for NPC to walk to when roaming or patrolling
	public bool inSight = false;
	public bool backTurned = false;
	public bool goIntoPain = false;
	public bool explodeOnAttack3 = false;
	public float rangeToEnemy = 0f;
	public GameObject[] meleeDamageColliders;
	[HideInInspector]
	public GameObject attacker;

	private bool hasSFX;
	private bool firstSighting;
	private bool dyingSetup;
	private bool ai_dying;
	private bool ai_dead;
	private int currentWaypoint;
	private float idleTime;
	private float attack1SoundTime;
	private float attack2SoundTime;
	private float attack3SoundTime;
	private float timeBetweenMeleeFinished;
	private float timeTillEnemyChangeFinished;
	private float timeTillDeadFinished;
	private float timeTillPainFinished;
	private AudioSource SFX;
	private NavMeshAgent nav;
	private Rigidbody rbody;
	private HealthManager healthManager;
	private BoxCollider boxCollider;
	private CapsuleCollider capsuleCollider;
	private SphereCollider sphereCollider;
	private MeshCollider meshCollider;
	private float tick;
	private float tickFinished;
	private bool hadEnemy;

	// QUAKE based variables
	public enum enemyRangeType {Melee,Near,Mid,Far};
	public enum enemyMoveType {None,Walk,Fly,Swim,Noclip};
	public enemyMoveType selfMoveType;
	public GameObject goalEntity;
	public GameObject sightEntity;
	public Vector3 idealTransformForward;
	public float attackFinished;
	public float showHostileTime;
	public float sightEntityTime;
	public bool infront;

	// Initialization and find components
	void Awake () {
		//resetPosition = new Vector3(0f,-100000f,0f); // Null position below playable area
		nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
		nav.updatePosition = true;
		nav.angularSpeed = yawspeed;
		//anim = GetComponent<Animator>();
		rbody = GetComponent<Rigidbody>();
		rbody.isKinematic = false;
		healthManager = GetComponent<HealthManager>();

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
		currentWaypoint = 0;
		enemy = null;
		firstSighting = true;
		inSight = false;
		hasSFX = false;
		goIntoPain = false;
		dyingSetup = false;
		ai_dead = false;
		ai_dying = false;
		attacker = null;
		idleTime = Time.time + Random.Range(3f,10f);
		attack1SoundTime = Time.time;
		attack2SoundTime = Time.time;
		attack3SoundTime = Time.time;
		timeBetweenMeleeFinished = Time.time;
		timeTillEnemyChangeFinished = Time.time;
		//timeBetweenProj1Finished = Time.time;
		//timeBetweenProj2Finished = Time.time;
		timeTillPainFinished = Time.time;
		timeTillDeadFinished = Time.time;
		SFX = GetComponent<AudioSource>();
		if (SFX == null)
			Debug.Log("WARNING: No audio source for AI character at: " + transform.position.x.ToString() + ", " + transform.position.y.ToString() + ", " + transform.position.z + ".");
		else
			hasSFX = true;

		if (walkWaypoints.Length == 0 || walkWaypoints[0] == null) {
			currentState = Const.aiState.Idle; // No waypoints, stay put
		} else {
			currentState = Const.aiState.Walk; // If waypoints are set, start walking them from the get go
		}
			
		//RuntimeAnimatorController ac = anim.runtimeAnimatorController;
		//for (int i=0;i<ac.animationClips.Length;i++) {
		//	if (ac.animationClips[i].name == "Death") {
		//		timeTillDead = ac.animationClips[i].length;
		//		break;
		//	}
		//}
		tick = 0.05f;
		tickFinished = Time.time + tick;

		//QUAKE based AI
		attackFinished = Time.time + 1f;
		showHostileTime = Time.time;
		sightEntityTime = Time.time;
		idealTransformForward = transform.forward;
	}

	void Update () {
		if (PauseScript.a != null && PauseScript.a.paused) {
			//anim.speed = 0f; // don't animate, we're paused
			nav.isStopped = true;  // don't move, we're paused
			return; // don't do any checks or anything else...we're paused!
		} else {
			//anim.speed = 1f;
			nav.isStopped = false;
		}

		// Only think every tick seconds to save on CPU and prevent race conditions
		if (tickFinished < Time.time) {
			Think();
			tickFinished = Time.time + tick;
		}
	}

	void Think () {
		if (healthManager.health <= 0) {
			// If we haven't gone into dying and we aren't dead, going into dying
			if (!ai_dying && !ai_dead) {
				ai_dying = true; //no going back
				currentState = Const.aiState.Dying; //start to collapse in a heap, melt, explode, etc.
			}
		}

		//check enemy health here

		switch (currentState) {
			case Const.aiState.Idle: 			Idle(); 		break;
			case Const.aiState.Walk:	 		Walk(); 		break;
			case Const.aiState.Run: 			Run(); 			break;
			case Const.aiState.Attack1: 		Attack1(); 		break;
			case Const.aiState.Attack2: 		Attack2(); 		break;
			case Const.aiState.Attack3: 		Attack3(); 		break;
			case Const.aiState.Pain: 			Pain();			break;
			case Const.aiState.Dying: 		Dying(); 		break;
			case Const.aiState.Dead: 			Dead(); 		break;
			case Const.aiState.Inspect: 		Inspect(); 		break;
			case Const.aiState.Interacting: 	Interacting();	break;
			default: 					Idle(); 		break;
		}

		if (currentState == Const.aiState.Dead || currentState == Const.aiState.Dying) return; // Don't do any checks, we're dead

		inSight = CheckIfPlayerInSight();
		//if (inSight) backTurned = CheckIfBackIsTurned();
		if (inSight) infront = enemyInFront(enemy);
		if (enemy != null) rangeToEnemy = Vector3.Distance(enemy.transform.position,transform.position);
	}

	bool CheckPain() {
		if (goIntoPain) {
			currentState = Const.aiState.Pain;
			if (attacker != null) {
				if (timeTillEnemyChangeFinished < Time.time) {
					timeTillEnemyChangeFinished = Time.time + changeEnemyTime;
					enemy = attacker; // Switch to whoever just attacked us
				}
			}
			goIntoPain = false;
			timeTillPainFinished = Time.time + timeToPain;
			return true;
		}
		return false;
	}

	void Idle() {
		if (enemy != null) {
			currentState = Const.aiState.Run;
			return;
		}
		nav.isStopped = true;
		//anim.SetBool("Walk",false);
		//anim.SetBool("Pain",false);
		if (idleTime < Time.time && SFXIdle) {
			SFX.PlayOneShot(SFXIdle);
			idleTime = Time.time + Random.Range(3f,10f);
		}
			
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		CheckIfPlayerInSight();
	}

	void Walk() {
		nav.isStopped = false;
		nav.speed = walkSpeed;
		int nextPointIndex = currentWaypoint++;
		if ((nextPointIndex == walkWaypoints.Length) || (walkWaypoints[nextPointIndex] == null)) nextPointIndex = 0; // Wrap around
		if (nextPointIndex == currentWaypoint) {
			currentState = Const.aiState.Idle;
			return;  // Out of waypoints
		}
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		//anim.SetBool("Walk",true);
		nav.SetDestination(walkWaypoints[nextPointIndex].transform.position);
	}

	void Run() {
		//if (anim.GetBool("Dying")) { currentState = aiState.Dying; return;}
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		if (inSight) {
			nav.angularSpeed = yawspeed;
			if (rangeToEnemy < meleeRange) {
				if (hasMelee && !backTurned) {
					nav.speed = meleeSpeed; 
					timeBetweenMeleeFinished = Time.time + timeBetweenMelee;
					currentState = Const.aiState.Attack1;
					return;
				}
			} else {
				if (rangeToEnemy < proj1Range) {
					if (hasProj1 && !backTurned) {
						nav.speed = proj1Speed;
						currentState = Const.aiState.Attack2;
						return;
					}
				} else {
					if (rangeToEnemy < proj2Range) {
						if (hasProj2 && !backTurned) {
							nav.speed = proj2Speed;
							currentState = Const.aiState.Attack3;
							return;
						}
					}
				}
			}
			nav.isStopped = false;
			nav.speed = runSpeed;
			nav.SetDestination(enemy.transform.position);
		} else {
			currentState = Const.aiState.Idle;
			return;
		}
	}

	void Attack1() {
		// Typically used for melee
		if (attack1SoundTime < Time.time && SFXAttack1) {
			SFX.PlayOneShot(SFXAttack1);
			attack1SoundTime = Time.time + timeBetweenMelee;
		}

		if (inSight && infront) {
			for (int i=0;i<meleeDamageColliders.Length;i++) {
				meleeDamageColliders[i].SetActive(true);
				meleeDamageColliders[i].GetComponent<AIMeleeDamageCollider>().MeleeColliderSetup(index,meleeDamageColliders.Length,impactMelee,gameObject);
			}
		}

		/*if (inSight) {
			if ((timeTillMeleeDamageFinished < Time.time) && !attackDamageDone) {
				if (rangeToEnemy < meleeRange) {
					DamageData ddNPC = Const.SetNPCDamageData(index, Const.aiState.Attack1);
					ddNPC.other = gameObject;
					ddNPC.attacknormal = Vector3.Normalize(enemy.transform.position - transform.position);
					ddNPC.impactVelocity = impactMelee;
					float take = Const.a.GetDamageTakeAmount(ddNPC);
					if (twoMeleeHits) take *= 0.5f; //halve it for double tap melee attacks
					ddNPC.damage = take;
					enemy.GetComponent<HealthManager>().TakeDamage(ddNPC);
					attackDamageDone = true;
				}
			}
			if (twoMeleeHits && (timeTillMeleeDamage2Finished < Time.time) && !attackDamage2Done) {
				if (rangeToEnemy < meleeRange) {
					DamageData ddNPC = Const.SetNPCDamageData(index, Const.aiState.Attack1);
					float take = Const.a.GetDamageTakeAmount(ddNPC);
					ddNPC.other = gameObject;
					ddNPC.attacknormal = Vector3.Normalize(enemy.transform.position - transform.position);
					ddNPC.impactVelocity = impactMelee2;
					take *= 0.5f; //take other half of melee damage on the 2nd swipe
					ddNPC.damage = take;
					enemy.GetComponent<HealthManager>().TakeDamage(ddNPC);
					attackDamage2Done = true;
				}
			}
		}*/

		if (timeBetweenMeleeFinished < Time.time) {
			goIntoPain = false; //prevent going into pain after attack
			currentState = Const.aiState.Run;
			return; // Done with attack
		}
	}

	void Attack2() {
		// Typically used for normal projectile attack
		if (attack2SoundTime < Time.time && SFXAttack2) {
			SFX.PlayOneShot(SFXAttack2);
			attack2SoundTime = Time.time + timeBetweenProj1;
		}
	}

	void Attack3() {
		// Typically used for secondary projectile or grenade attack
		if (attack3SoundTime < Time.time && SFXAttack3) {
			SFX.PlayOneShot(SFXAttack3);
			attack3SoundTime = Time.time + timeBetweenProj2;
		}

		if (explodeOnAttack3) {
			ExplosionForce ef = GetComponent<ExplosionForce>();
			DamageData ddNPC = Const.SetNPCDamageData(index, Const.aiState.Attack3,gameObject);
			float take = Const.a.GetDamageTakeAmount(ddNPC);
			ddNPC.other = gameObject;
			ddNPC.damage = take;
			//enemy.GetComponent<HealthManager>().TakeDamage(ddNPC); Handled by ExplodeInner
			if (ef != null) ef.ExplodeInner(transform.position+explosionOffset, attack3Force, attack3Radius, ddNPC);
			healthManager.ObjectDeath(SFXDeathClip);
			return;
		}
	}

	void Pain() {
		if (timeTillPainFinished < Time.time) {
			currentState = Const.aiState.Run; // go into run after we get hurt
			goIntoPain = false;
			timeTillPainFinished = Time.time + timeBetweenPain;
			return;
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
			gameObject.tag = "Searchable"; // Enable searching

			nav.speed = nav.speed * 0.5f; // half the speed while collapsing or whatever
			timeTillDeadFinished = Time.time + timeTillDead; // wait for death animation to finish before going into Dead()
		}
			
		if (timeTillDeadFinished < Time.time) {
			ai_dead = true;
			ai_dying = false;
			currentState = Const.aiState.Dead;
		}
	}

	void Dead() {
		nav.isStopped = true; // Stop moving
		//anim.speed = 0f; // Stop animation
		ai_dead = true;
		ai_dying = false;
		rbody.isKinematic = true;
		currentState = Const.aiState.Dead;
		firstSighting = false;
		if (healthManager.gibOnDeath) {
			ExplosionForce ef = GetComponent<ExplosionForce>();
			DamageData ddNPC = Const.SetNPCDamageData(index, Const.aiState.Attack3,gameObject);
			float take = Const.a.GetDamageTakeAmount(ddNPC);
			ddNPC.other = gameObject;
			ddNPC.damage = take;
			//enemy.GetComponent<HealthManager>().TakeDamage(ddNPC); Handled by ExplodeInner
			if (ef != null) ef.ExplodeInner(transform.position+explosionOffset, attack3Force, attack3Radius, ddNPC);
			healthManager.ObjectDeath(SFXDeathClip);
		}
	}

	void Inspect() {
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
	}
		
	void Interacting() {
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
	}

	void ResetEnemy() {
		firstSighting = true; // Reset playing sight sound
		enemy = null; // 
	}

	/*
	bool CheckIfEnemyInSight () {
		if (enemy == null) {
			ResetEnemy();
			return false; // We don't have a known enemy to check sight of
		}

		Vector3 checkline = enemy.transform.position - transform.position; // Get vector line made from enemy to found player

		// Check for line of sight
		RaycastHit hit;
		if(Physics.Raycast(transform.position + transform.up, checkline.normalized, out hit, sightRange)) {
			if (hit.collider.gameObject == enemy)
				return true; // Still can see enemy
		}
			
		ResetEnemy();
		return false; // Can't see enemy
	}
	*/

	/*bool CheckIfBackIsTurned() {
		if (enemy == null) return true;

		//Vector3 checkline = enemy.transform.position - transform.position; // Get vector line made from enemy to found player
		Vector2 org = new Vector2(transform.position.x,transform.position.z);
		Vector2 enemorg = new Vector2(enemy.transform.position.x,enemy.transform.position.z);
		Vector2 checkline = enemorg - org;
		Vector2 forwardXZ = new Vector2(transform.forward.x, transform.forward.z);
		//float angle = Vector3.Angle(checkline,transform.forward);
		float angle = Vector2.Angle(checkline,forwardXZ);
		if (angle < (fieldOfViewAttack * 0.5f)) return false;
		return true;
	}*/

	bool CheckIfEnemyInSight() {
		Vector3 checkline = enemy.transform.position - transform.position; // Get vector line made from enemy to found player
		RaycastHit hit;
		if(Physics.Raycast(transform.position + transform.up, checkline.normalized, out hit, sightRange)) {
			if (hit.collider.gameObject == enemy)
				return true;
		}
		enemy = null;
		return false;
	}

	bool CheckIfPlayerInSight () {
		if (enemy != null) return CheckIfEnemyInSight();

		GameObject playr1 = Const.a.player1;
		GameObject playr2 = Const.a.player2;
		GameObject playr3 = Const.a.player3;
		GameObject playr4 = Const.a.player4;

		if (playr1 == null) { Debug.Log("WARNING: NPC sight check - no host player 1."); return false; }  // No host player
		if (playr1 != null) {playr1 = playr1.GetComponent<PlayerReferenceManager>().playerCapsule;}
		if (playr2 != null) {playr2 = playr2.GetComponent<PlayerReferenceManager>().playerCapsule;}
		if (playr3 != null) {playr3 = playr3.GetComponent<PlayerReferenceManager>().playerCapsule;}
		if (playr4 != null) {playr4 = playr4.GetComponent<PlayerReferenceManager>().playerCapsule;}

		GameObject tempent = null;
		bool LOSpossible = false;

		for (int i=0;i<4;i++) {
			tempent = null;
			// Cycle through all the players to see if we can see anybody.  Defaults to earlier joined players. TODO: Add randomization if multiple players are visible.
			if (playr1 != null && i == 0) tempent = playr1;
			if (playr2 != null && i == 1) tempent = playr2;
			if (playr3 != null && i == 2) tempent = playr3;
			if (playr4 != null && i == 4) tempent = playr4;
			if (tempent == null) continue;

			Vector3 checkline = tempent.transform.position - transform.position; // Get vector line made from enemy to found player

			// Check for line of sight
			RaycastHit hit;
			if(Physics.Raycast(transform.position + transform.up, checkline.normalized, out hit, sightRange)) {
				if (hit.collider.gameObject == tempent)
					LOSpossible = true;  // Clear path from enemy to found player
			}

			float dist = Vector3.Distance(tempent.transform.position,transform.position);  // Get distance between enemy and found player
			float angle = Vector3.Angle(checkline,transform.forward);
			//float angle = Const.AngleInDeg(checkline,transform.forward);  // Get angle between enemy view forward ray and a line made to found player

			// If clear path to found player, and either within view angle or right behind the enemy
			if (LOSpossible) {
				if (angle < (fieldOfViewAngle * 0.5f)) {
					enemy = tempent;
					//backTurned = false;
					if (firstSighting) {
						firstSighting = false;
						if (hasSFX) SFX.PlayOneShot(SFXSightSound);
					}
					return true;
				} else {
					if (dist < distToSeeWhenBehind) {
						enemy = tempent;
						//backTurned = true;
						if (firstSighting) {
							firstSighting = false;
							if (hasSFX) SFX.PlayOneShot(SFXSightSound);
						}
						return true;
					}
				}
			}
			//if (tempent.GetComponent<PlayerHealth>().makingNoise && dist < distToHear) {
			//	lastKnownPosition = tempent.transform.position;
			//}
		}
		ResetEnemy();
		//if (lastKnownPosition == resetPosition) chasing = false;
		return false;
	}
	
    // QUAKE based AI functions
    // =============================================================================================================
    
    // Return range type based on distance to the target
    // Melee = Use melee attack
    // Near = Use close range projectile attack (if applicable)
    // Mid = Use projectile attack or grenades (if applicable)
    // Far = Can't see, too far away
    enemyRangeType enemyRange (GameObject target) {
        float r = Vector3.Distance(transform.position, target.transform.position);
        if (r < 3.6) return enemyRangeType.Melee;
        if(r < 15) return enemyRangeType.Near;
        if(r < 30) return enemyRangeType.Mid;
        return enemyRangeType.Far;
    }

    bool enemyVisible (GameObject target) {
        Vector3 checkline = target.transform.position - transform.position; // Get vector line made from enemy to found player
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up, checkline.normalized, out hit, sightRange)) {
            if (hit.collider.gameObject == target) {
                return true;
            }
                
        }
        return false;
    }

    bool enemyInFront (GameObject target) {
        Vector3 vec = Vector3.Normalize(target.transform.position - transform.position);
        float dot = Vector3.Dot(vec,transform.forward);
        if (dot > 0.300) return true; // enemy is within 27 degrees of forward facing vector
        return false;
    }

	void HuntTarget () {
		goalEntity = enemy;
		currentState = Const.aiState.Run;
		idealTransformForward = Vector3.Normalize(enemy.transform.position - transform.position);
		attackFinished = Time.time + 1.0f;
	}

	void SightSound () { SFX.PlayOneShot(SFXSightSound,1.0f); }
	void FoundTarget () {
		if (enemy.tag == "Player") {
			sightEntity = gameObject;
			sightEntityTime = Time.time;
		}
		showHostileTime = Time.time + 1.0f;
		SightSound();
		HuntTarget();
	}
}
