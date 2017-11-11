using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {
	public int index = 0; // NPC reference index for looking up constants in tables in Const.cs
	public enum aiState{Idle,Walk,Run,Attack1,Attack2,Attack3,Pain,Dying,Dead,Inspect,Interacting};
	public aiState currentState;
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
	public float rangeToEnemy = 0f;
	public GameObject idleIndicator;
	public GameObject attackIndicator;
	public GameObject huntIndicator;
	public GameObject painIndicator;
	[HideInInspector]
	public GameObject attacker;

	private bool hasSFX;
	private bool firstSighting;
	private bool attackDamageDone;
	private bool attackDamage2Done;
	private bool dyingSetup;
	private bool ai_dying;
	private bool ai_dead;
	private int currentWaypoint;
	private float idleTime;
	private float timeTillMeleeDamageFinished;
	private float timeTillMeleeDamage2Finished;
	private float timeBetweenMeleeFinished;
	//private float timeBetweenProj1Finished;
	//private float timeBetweenProj2Finished;
	private float timeTillEnemyChangeFinished;
	private float timeTillDeadFinished;
	private float timeTillPainFinished;
	//private Vector3 resetPosition;
	private AudioSource SFX;
	private NavMeshAgent nav;
	//private Animator anim;
	private Rigidbody rbody;
	private HealthManager healthManager;
	private BoxCollider searchableCollider;
	private CapsuleCollider collisionCapsule;

	private float tick;
	private float tickFinished;

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
		searchableCollider = GetComponent<BoxCollider>();
		searchableCollider.enabled = false;
		collisionCapsule = GetComponent<CapsuleCollider>();
		collisionCapsule.enabled = true;
		currentState = aiState.Idle;
		currentWaypoint = 0;
		enemy = null;
		firstSighting = true;
		inSight = false;
		hasSFX = false;
		attackDamageDone = false;
		goIntoPain = false;
		dyingSetup = false;
		ai_dead = false;
		ai_dying = false;
		attacker = null;
		idleTime = Time.time + Random.Range(3f,10f);
		timeTillMeleeDamageFinished = Time.time;
		timeTillMeleeDamage2Finished = Time.time;
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
			currentState = aiState.Idle; // No waypoints, stay put
		} else {
			currentState = aiState.Walk; // If waypoints are set, start walking them from the get go
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

		if (tickFinished < Time.time) {
			Think();
			tickFinished = Time.time + tick;
		}
	}

	void Think () {
		if (healthManager.health <= 0 || healthManager.dead) {
			// If we haven't gone into dying and we aren't dead, going into dying
			if (!ai_dying && !ai_dead) {
				ai_dying = true; //no going back
				currentState = aiState.Dying; //start to collapse in a heap, melt, explode, etc.
			}
		}

		//check enemy health here

		switch (currentState) {
			case aiState.Idle: 			Idle(); 		break;
			case aiState.Walk:	 		Walk(); 		break;
			case aiState.Run: 			Run(); 			break;
			case aiState.Attack1: 		Attack1(); 		break;
			case aiState.Attack2: 		Attack2(); 		break;
			case aiState.Attack3: 		Attack3(); 		break;
			case aiState.Pain: 			Pain();			break;
			case aiState.Dying: 		Dying(); 		break;
			case aiState.Dead: 			Dead(); 		break;
			case aiState.Inspect: 		Inspect(); 		break;
			case aiState.Interacting: 	Interacting();	break;
			default: 					Idle(); 		break;
		}

		if (currentState == aiState.Dead || currentState == aiState.Dying) return; // Don't do any checks, we're dead

		inSight = CheckIfPlayerInSight();
		if (inSight) backTurned = CheckIfBackIsTurned();
		if (enemy != null) rangeToEnemy = Vector3.Distance(enemy.transform.position,transform.position);
	}

	bool CheckPain() {
		if (goIntoPain) {
			currentState = aiState.Pain;
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
			currentState = aiState.Run;
			return;
		}
		nav.isStopped = true;
		//anim.SetBool("Walk",false);
		//anim.SetBool("Pain",false);
		if (idleTime < Time.time) {
			SFX.PlayOneShot(SFXIdle);
			idleTime = Time.time + Random.Range(3f,10f);
		}
			
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		CheckIfPlayerInSight();
		huntIndicator.SetActive(false);
		attackIndicator.SetActive(false);
		idleIndicator.SetActive(true);
		painIndicator.SetActive(false);
	}

	void Walk() {
		nav.isStopped = false;
		nav.speed = walkSpeed;
		int nextPointIndex = currentWaypoint++;
		if ((nextPointIndex == walkWaypoints.Length) || (walkWaypoints[nextPointIndex] == null)) nextPointIndex = 0; // Wrap around
		if (nextPointIndex == currentWaypoint) {
			currentState = aiState.Idle;
			return;  // Out of waypoints
		}
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		//anim.SetBool("Walk",true);
		nav.SetDestination(walkWaypoints[nextPointIndex].transform.position);
		huntIndicator.SetActive(false);
		attackIndicator.SetActive(false);
		idleIndicator.SetActive(false);
		painIndicator.SetActive(false);
	}

	void Run() {
		//if (anim.GetBool("Dying")) { currentState = aiState.Dying; return;}
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		if (inSight) {
			nav.angularSpeed = yawspeed;
			if (rangeToEnemy < meleeRange) {
				if (hasMelee && !backTurned) {
					nav.speed = meleeSpeed; 
					attackDamageDone = false;
					attackDamage2Done = false;
					timeTillMeleeDamageFinished = Time.time + timeTillMeleeDamage;
					timeTillMeleeDamage2Finished = Time.time + timeTillMeleeDamage2;
					timeBetweenMeleeFinished = Time.time + timeBetweenMelee;
					currentState = aiState.Attack1;
					return;
				}
			} else {
				if (rangeToEnemy < proj1Range) {
					if (hasProj1 && !backTurned) {
						nav.speed = proj1Speed;
						currentState = aiState.Attack2;
						return;
					}
				} else {
					if (rangeToEnemy < proj2Range) {
						if (hasProj2 && !backTurned) {
							nav.speed = proj2Speed;
							currentState = aiState.Attack3;
							return;
						}
					}
				}
			}
			nav.isStopped = false;
			nav.speed = runSpeed;
			nav.SetDestination(enemy.transform.position);
			huntIndicator.SetActive(true);
			attackIndicator.SetActive(false);
			idleIndicator.SetActive(false);
			painIndicator.SetActive(false);
		} else {
			currentState = aiState.Idle;
			return;
		}
	}

	void Attack1() {
		// Typically used for melee
		huntIndicator.SetActive(false);
		attackIndicator.SetActive(true);
		idleIndicator.SetActive(false);
		painIndicator.SetActive(false);

		if (inSight) {
			if ((timeTillMeleeDamageFinished < Time.time) && !attackDamageDone) {
				if (rangeToEnemy < meleeRange) {
					DamageData ddNPC = SetNPCDamageData(index, aiState.Attack1);
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
					DamageData ddNPC = SetNPCDamageData(index, aiState.Attack1);
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
		}

		if (timeBetweenMeleeFinished < Time.time) {
			goIntoPain = false; //prevent going into pain after attack
			currentState = aiState.Run;
			return; // Done with attack
		}
	}

	void Attack2() {
		// Typically used for normal projectile attack
		huntIndicator.SetActive(false);
		attackIndicator.SetActive(true);
		idleIndicator.SetActive(false);
		painIndicator.SetActive(false);

	}

	void Attack3() {
		// Typically used for secondary projectile or grenade attack
		huntIndicator.SetActive(false);
		attackIndicator.SetActive(true);
		idleIndicator.SetActive(false);
		painIndicator.SetActive(false);

	}

	void Pain() {
		huntIndicator.SetActive(false);
		attackIndicator.SetActive(false);
		idleIndicator.SetActive(false);
		painIndicator.SetActive(true);
		if (timeTillPainFinished < Time.time) {
			currentState = aiState.Run; // go into run after we get hurt
			goIntoPain = false;
			timeTillPainFinished = Time.time + timeBetweenPain;
			return;
		}
	}

	void Dying() {
		if (!dyingSetup) {
			dyingSetup = true;
			SFX.PlayOneShot(SFXDeathClip);
			//anim.SetBool("Dying", true); // Set death animation going
			collisionCapsule.enabled = false; // Disable normal collision
			gameObject.tag = "Searchable"; // Enable searching
			searchableCollider.enabled = true; // Enable search collision box
			searchableCollider.isTrigger = false;
			nav.speed = nav.speed * 0.5f; // half the speed while collapsing or whatever
			timeTillDeadFinished = Time.time + timeTillDead; // wait for death animation to finish before going into Dead()
		}

		huntIndicator.SetActive(false);
		attackIndicator.SetActive(false);
		idleIndicator.SetActive(false);
		painIndicator.SetActive(false);

		if (timeTillDeadFinished < Time.time) {
			ai_dead = true;
			ai_dying = false;
			currentState = aiState.Dead;
		}
	}

	void Dead() {
		huntIndicator.SetActive(false);
		attackIndicator.SetActive(false);
		idleIndicator.SetActive(false);
		painIndicator.SetActive(false);
		nav.isStopped = true; // Stop moving
		//anim.speed = 0f; // Stop animation
		ai_dead = true;
		ai_dying = false;
		rbody.isKinematic = true;
		currentState = aiState.Dead;
		firstSighting = false;
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

	bool CheckIfBackIsTurned() {
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
	}

	bool CheckIfEnemyInSight() {
		Vector3 checkline = enemy.transform.position - transform.position; // Get vector line made from enemy to found player
		RaycastHit hit;
		if(Physics.Raycast(transform.position + transform.up, checkline.normalized, out hit, sightRange)) {
			if (hit.collider.gameObject == enemy)
				return true;
		}
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

	DamageData SetNPCDamageData (int NPCindex, aiState attackIndex) {
		if (NPCindex < 0 || NPCindex > 23) {
			Debug.Log("BUG: NPCindex set incorrectly on NPC.  Not 0 to 23. Disabled.");
			gameObject.SetActive(false);
		}
		DamageData dd = new DamageData(); 
		// Attacker (self [a]) data
		dd.owner = gameObject;
		switch (attackIndex) {
		case aiState.Attack1:
			dd.damage = Const.a.damageForNPC[NPCindex];
			break;
		case aiState.Attack2:
			dd.damage = Const.a.damageForNPC2[NPCindex];
			break;
		case aiState.Attack3:
			dd.damage = Const.a.damageForNPC3[NPCindex];
			break;
		default: Debug.Log("BUG: attackIndex not 0,1, or 2 on NPC! Damage set to 1."); dd.damage = 1f; break;
		}
		dd.penetration = 0;
		dd.offense = 0;
		return dd;
	}
}
