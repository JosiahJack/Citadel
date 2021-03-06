﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DigitalRuby.LightningBolt;

public class AIController : MonoBehaviour {
	public int index = 0; // NPC reference index for looking up constants in tables in Const.cs // save
	[HideInInspector]
	public string targetID;
	[HideInInspector]
	public bool hasTargetIDAttached = false;
	public Const.aiState currentState; // save (referenced by int index 0 thru 10)
    public Const.aiMoveType moveType;
	public Const.npcType npcType;
	public GameObject enemy; // save (referenced by int index enemIDRead)
    public GameObject searchColliderGO;
	public float yawspeed = 180f;
	public float fieldOfViewAngle = 180f;
    public float fieldOfViewAttack = 80f;
    public float fieldOfViewStartMovement = 45f;
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
	public float timeTillActualAttack1 = 0.5f;
    public float timeTillActualAttack2 = 0.5f;
	public float timeTillActualAttack3 = 0.2f;
    [HideInInspector]
    public float gracePeriodFinished; // save
    [HideInInspector]
    public float meleeDamageFinished; // save
	public float timeBetweenAttack1 = 1.2f;
	public float timeBetweenAttack2 = 1.5f;
	public float timeBetweenAttack3 = 3f;
	public float changeEnemyTime = 3f; // Time before enemy will switch to different attacker
    public float idleSFXTimeMin = 5f;
    public float idleSFXTimeMax = 12f;
	public float attack1MinRandomWait = 0.5f;
	public float attack1MaxRandomWait = 1f;
	public float attack1RandomWaitChance = 0.1f; //10% chance of waiting a random amount of time before attacking since most melees keep moving anyways
    public float attack2MinRandomWait = 1f;
    public float attack2MaxRandomWait = 2f;
    public float attack2RandomWaitChance = 0.5f; //50% chance of waiting a random amount of time before attacking to allow for movement
    public float attack3MinRandomWait = 1f;
    public float attack3MaxRandomWait = 2f;
    public float attack3RandomWaitChance = 0.5f; //50% chance of waiting a random amount of time before attacking to allow for movement
	public float impactMelee = 10f;
	public float impactMelee2 = 10f;
	public float impactMelee3 = 10f;
    public Const.AttackType attack1Type = Const.AttackType.Melee;
    public Const.AttackType attack2Type = Const.AttackType.Projectile;
    public Const.AttackType attack3Type = Const.AttackType.Projectile;
	public Const.PoolType attack1ProjectileLaunchedType = Const.PoolType.None;
	public Const.PoolType attack2ProjectileLaunchedType = Const.PoolType.None;
	public Const.PoolType attack3ProjectileLaunchedType = Const.PoolType.None;
	public float attack1projectilespeed = 2f;
	public float attack2projectilespeed = 2f;
	public float attack3projectilespeed = 2f;
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
    public bool dontLoopWaypoints = false;
	public bool visitWaypointsRandomly = false;
	public bool hasMelee = true;
	public bool hasProj1 = false;
	public bool hasProj2 = false;
	public bool hasLaserForProj2 = false;
	public LightningBoltScript laserLightning;
	public Transform[] walkWaypoints; // point(s) for NPC to walk to when roaming or patrolling
	public bool inSight = false; // save
    public bool infront; // save
    public bool inProjFOV; // save
    public bool LOSpossible; // save
    public bool goIntoPain = false; // save
	public bool explodeOnAttack3 = false;
    public bool ignoreEnemy = false;
	[HideInInspector]
	public float rangeToEnemy = 999f; // save
	public GameObject[] meleeDamageColliders;
	public bool preActivateMeleeColliders = false;
    public GameObject muzzleBurst;
    public GameObject muzzleBurst2;
    public GameObject rrCheckPoint;
	[HideInInspector]
	public GameObject attacker;
	[HideInInspector]
	public bool firstSighting; // save
	[HideInInspector]
	public bool dyingSetup; // save
	[HideInInspector]
	public bool ai_dying; // save
	[HideInInspector]
	public bool ai_dead; // save
	[HideInInspector]
	public int currentWaypoint; // save
	[HideInInspector]
	public Vector3 currentDestination; // save
	private float idleTime;
	private float attack1SoundTime;
	private float attack2SoundTime;
	private float attack3SoundTime;
	[HideInInspector]
	public float timeTillEnemyChangeFinished; // save
	[HideInInspector]
	public float timeTillDeadFinished; // save
	[HideInInspector]
	public float timeTillPainFinished; // save
	[HideInInspector]
	public AudioSource SFX;
	[HideInInspector]
	public Rigidbody rbody;
	public HealthManager healthManager;
	[HideInInspector]
	public BoxCollider boxCollider;
	[HideInInspector]
	public CapsuleCollider capsuleCollider;
	[HideInInspector]
	public SphereCollider sphereCollider;
	[HideInInspector]
	public MeshCollider meshCollider;
	private float tick;
	private float raycastingTick;
	[HideInInspector]
	public float tickFinished; // save
	[HideInInspector]
	public float raycastingTickFinished; // save
    public float huntTime = 5f;
	[HideInInspector]
    public float huntFinished; // save
	[HideInInspector]
	public bool hadEnemy; // save
	[HideInInspector]
    public Vector3 lastKnownEnemyPos; // save
	[HideInInspector]
    public Vector3 tempVec; // save
	private Vector3 tempVec2;
	[HideInInspector]
    public bool shotFired = false; // save
    private DamageData damageData;
    private RaycastHit tempHit;
    private bool useBlood;
    private HealthManager tempHM;
	[HideInInspector]
    public float randomWaitForNextAttack1Finished; // save
	[HideInInspector]
    public float randomWaitForNextAttack2Finished; // save
	[HideInInspector]
    public float randomWaitForNextAttack3Finished; // save
    public GameObject visibleMeshEntity;
    public GameObject gunPoint;
    public GameObject gunPoint2;
	[HideInInspector]
	public Vector3 idealTransformForward; // save
	[HideInInspector]
    public Vector3 idealPos; // used by flyers to establish correct height // save
	public float flightDesiredHeight = 0.75f;
	public bool flightHeightIsPercentage = true;
	[HideInInspector]
	public float attackFinished; // save
	[HideInInspector]
    public float attack2Finished; // save
	[HideInInspector]
    public float attack3Finished; // save
	[HideInInspector]
    public Vector3 targettingPosition; // used to give the player a chance to dodge attacks by moving after start of an attack, enemy attacks along same starting line // save
	public Material deathMaterial;
	public bool switchMaterialOnDeath;
	public SkinnedMeshRenderer actualSMR;
	public GameObject deathBurst;
	public float deathBurstTimer = 0.1f;
	[HideInInspector]
	public float deathBurstFinished; // save
	[HideInInspector]
	public bool deathBurstDone; // save
	public GameObject sightPoint;
	private NavMeshPath searchPath;
	public float rangeToHear = 10f;
	public bool asleep = false; // check if enemy starts out asleep, e.g. sleeping sec-2 bots on level 8 in the maintenance chargers // save
	public bool useGravityOnDeath = true;
	public bool useGravityOnDying = true;
	public float tranquilizeTime = 3f;
	[HideInInspector]
	public float tranquilizeFinished; // save
	public bool attack1UsesLaser = false;
	public bool hopOnRun = false;
	public float hopForce = 500f;
	public Animator hopAnimator;
	[HideInInspector]
	public bool hopDone; // save
	public bool wandering; // save
	[HideInInspector]
	public float wanderFinished; // save
	private float dotProjResult = -1f;
	private float dotResult = -1f;
	private Vector3 infrontVec;
	public bool actAsTurret = false;
	public GameObject sleepingCables;
	private PlayerMovement pm;
	public RectTransform npcAutomapOverlay;
	public Image npcAutomapOverlayImage;
	[HideInInspector]
	public bool startInitialized = false;
	private HealthManager enemyHM;

	public void Tranquilize() {
		// Check against percent chance (disruptability) of getting tranq'ed
		//if (UnityEngine.Random.Range(0,1f) < Const.a.disruptabilityForNPC[index])
		tranquilizeFinished = PauseScript.a.relativeTime + tranquilizeTime;
	}

	// Initialization and find components
	public void Start () {
        rbody = GetComponent<Rigidbody>();
		if (rbody.isKinematic) rbody.isKinematic = false;

		if (moveType == Const.aiMoveType.Fly || moveType == Const.aiMoveType.Cyber) {
			//rbody.isKinematic = true;
			rbody.useGravity = false;
		}
		healthManager = GetComponent<HealthManager>();
		if (healthManager == null) Debug.Log("Null health manager on npc with index " + index.ToString());
	    boxCollider = GetComponent<BoxCollider>();
		sphereCollider = GetComponent<SphereCollider>();
		meshCollider = GetComponent<MeshCollider>();
		capsuleCollider = GetComponent<CapsuleCollider>();
        if (searchColliderGO != null) searchColliderGO.SetActive(false);
		currentDestination = sightPoint.transform.position;
		if (meleeDamageColliders.Length > 0) {
			for (int i = 0; i < meleeDamageColliders.Length; i++) {
				if (meleeDamageColliders[i] != null) {
					if (meleeDamageColliders[i].activeSelf) {
						meleeDamageColliders[i].SetActive(false);
					}
				}
			}
		}

        currentState = Const.aiState.Idle;
		currentWaypoint = 0;
		enemy = null;
		enemyHM = null;
		firstSighting = true;
		inSight = false;
		goIntoPain = false;
		dyingSetup = false;
		ai_dead = false;
		ai_dying = false;
		attacker = null;
        shotFired = false;
		hopDone = false;
		idleTime = PauseScript.a.relativeTime + Random.Range(idleSFXTimeMin,idleSFXTimeMax);
		attack1SoundTime = PauseScript.a.relativeTime;
		attack2SoundTime = PauseScript.a.relativeTime;
		attack3SoundTime = PauseScript.a.relativeTime;
		timeTillEnemyChangeFinished = PauseScript.a.relativeTime;
        huntFinished = PauseScript.a.relativeTime;
		attackFinished = PauseScript.a.relativeTime;
		attack2Finished = PauseScript.a.relativeTime;
        attack3Finished = PauseScript.a.relativeTime;
        timeTillPainFinished = PauseScript.a.relativeTime;
		timeTillDeadFinished = PauseScript.a.relativeTime;
        meleeDamageFinished = PauseScript.a.relativeTime;
        gracePeriodFinished = PauseScript.a.relativeTime;
        randomWaitForNextAttack1Finished = PauseScript.a.relativeTime;
        randomWaitForNextAttack2Finished = PauseScript.a.relativeTime;
        randomWaitForNextAttack3Finished = PauseScript.a.relativeTime;
		tranquilizeFinished = PauseScript.a.relativeTime;
		deathBurstFinished = PauseScript.a.relativeTime;
		wanderFinished = PauseScript.a.relativeTime;
        damageData = new DamageData();
		damageData.ownerIsNPC = true;
        tempHit = new RaycastHit();
        tempVec = new Vector3(0f, 0f, 0f);
        SFX = GetComponent<AudioSource>();
		if (SFX == null) Debug.Log("WARNING: No audio source for npc at: " + transform.position.ToString());
		if (SFX != null) SFX.playOnAwake = false;
		if (walkWaypoints.Length > 0 && walkWaypoints[currentWaypoint] != null && walkPathOnStart && !asleep) {
            currentDestination = walkWaypoints[currentWaypoint].transform.position;
            currentState = Const.aiState.Walk; // If waypoints are set, start walking them from the get go
		} else {
            currentState = Const.aiState.Idle; // No waypoints, stay put
        }

		if (wandering) {
			currentState = Const.aiState.Walk;
			if (asleep) currentState = Const.aiState.Idle;
		}
		tick = 0.1f;
		raycastingTick = 0.2f;
		tickFinished = PauseScript.a.relativeTime + tick + Random.value;
		raycastingTickFinished = PauseScript.a.relativeTime + tick + Random.value;
		attackFinished = PauseScript.a.relativeTime + 1f;
		idealTransformForward = sightPoint.transform.forward;
		deathBurstDone = false;
		searchPath = new NavMeshPath();
		if (moveType != Const.aiMoveType.Cyber) {
			targetID = Const.GetTargetID(index);
		} else {
			targetID = Const.GetCyberTargetID(index);
		}

		if (Const.a.difficultyCombat == 1) {
			huntTime *= 0.75f; // more forgetfull on 1
		}

		if (Const.a.difficultyCombat >= 3) {
			huntTime *= 2f; // better memory on hardest Combat difficulty
		}
		startInitialized = true;
	}

	void AI_Face(Vector3 goalLocation) {
		if (asleep) return;
		Vector3 dir = (goalLocation - transform.position).normalized;
		dir.y = 0f;
		if (dir.magnitude > 0 && Vector3.up.magnitude > 0) {
			Quaternion lookRot = Quaternion.LookRotation(dir,Vector3.up);
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRot, tick * yawspeed * Time.deltaTime); // rotate as fast as we can towards goal position
		}
	}

	void OnEnable() {
		if (PauseScript.a != null) {
			idleTime = PauseScript.a.relativeTime + Random.Range(idleSFXTimeMin,idleSFXTimeMax);
			if (npcAutomapOverlay == null) {
				Const.PoolType pt = Const.PoolType.AutomapBotOverlays;
				switch (npcType) {
					case Const.npcType.Mutant: pt = Const.PoolType.AutomapMutantOverlays; break;
					case Const.npcType.Supermutant: pt = Const.PoolType.AutomapMutantOverlays; break;
					case Const.npcType.Robot: pt = Const.PoolType.AutomapBotOverlays; break;
					case Const.npcType.Cyborg: pt = Const.PoolType.AutomapCyborgOverlays; break;
					case Const.npcType.Supercyborg: pt = Const.PoolType.AutomapCyborgOverlays; break;
					case Const.npcType.MutantCyborg: pt = Const.PoolType.AutomapCyborgOverlays; break;
				}

				GameObject overlay = Const.a.GetObjectFromPool(pt);
				if (overlay != null) {
					overlay.SetActive(true);
					npcAutomapOverlay = overlay.GetComponent<RectTransform>();
					npcAutomapOverlayImage = overlay.GetComponent<Image>();
					//npcAutomapOverlayImage.enabled = false;
				}
			}
		}
	}

	void FixedUpdate() {
		if (PauseScript.a.Paused() || PauseScript.a.mainMenu.activeInHierarchy) {
			return; // don't do any checks or anything else...we're paused!
		}

        // Only think every tick seconds to save on CPU and prevent race conditions
        if (tickFinished < PauseScript.a.relativeTime) {
			tickFinished = PauseScript.a.relativeTime + tick;
			Think();
			if (npcAutomapOverlay != null && moveType != Const.aiMoveType.Cyber) {
				// 34.16488, -45.08, 0.4855735
				// x = ((0.6384575295) * 1008f) + 8;
				// x = 651
				tempVec2.y = (((((transform.position.z - Const.a.mapWorldMaxE)/(Const.a.mapWorldMaxW - Const.a.mapWorldMaxE)) * 1008f) + Const.a.mapTileMinX));
				tempVec2.x = (((((transform.position.x - Const.a.mapWorldMaxS)/(Const.a.mapWorldMaxN - Const.a.mapWorldMaxS)) * 1008f) + Const.a.mapTileMinY));
				tempVec2.z = -0.3f;
				npcAutomapOverlay.localPosition = tempVec2;
			}
		}

		if (raycastingTickFinished < PauseScript.a.relativeTime) {
			raycastingTickFinished = PauseScript.a.relativeTime + raycastingTick;
			inSight = CheckIfPlayerInSight();
			if (enemy != null) {
				// Check if enemy health drops to 0
				if (enemyHM == null) enemyHM = GetComponent<HealthManager>();
				if (enemyHM != null) {
					if (moveType == Const.aiMoveType.Cyber) {
						if (enemyHM.cyberHealth <= 0) {
							currentState = Const.aiState.Idle;
							enemy = null;
							enemyHM = null;
						}
					} else {
						if (enemyHM.health <= 0) {
							wandering = true; // enemy is dead, let's wander around aimlessly now
							currentState = Const.aiState.Walk;
							enemy = null;
							enemyHM = null;
						}
					}
				}

				// Enemy still has health
				enemyInFrontChecks(enemy);
				rangeToEnemy = Vector3.Distance(enemy.transform.position, sightPoint.transform.position);
			} else {
				infront = false;
				inProjFOV = false;
				rangeToEnemy = sightRange;
			}
		}

        // Rotation and Special movement that must be done every FixedUpdate
        if (currentState != Const.aiState.Dead) {
            if (currentState != Const.aiState.Idle) {
                idealTransformForward = currentDestination - sightPoint.transform.position;
                if (moveType != Const.aiMoveType.Cyber) idealTransformForward.y = 0;
				idealTransformForward = Vector3.Normalize(idealTransformForward);
				if (idealTransformForward.magnitude > Mathf.Epsilon) {
					AI_Face(currentDestination);
				}
            }
        }
	}

	void Think () {
		if (dyingSetup && deathBurstFinished < PauseScript.a.relativeTime && !deathBurstDone) {
			if (deathBurst != null) deathBurst.SetActive(true); // activate any death effects
			deathBurstDone = true;
		}

		if (moveType != Const.aiMoveType.Cyber) {
			if (healthManager.health <= 0) {
				// If we haven't gone into dying and we aren't dead, going into dying
				if (!ai_dying && !ai_dead) {
					ai_dying = true; //no going back
					currentState = Const.aiState.Dying; //start to collapse in a heap, melt, explode, etc.
				}
			}
		} else {
			if (healthManager.cyberHealth <= 0) {
				// If we haven't gone into dying and we aren't dead, going into dying
				if (!ai_dying && !ai_dead) {
					ai_dying = true; //no going back
					currentState = Const.aiState.Dying; //start to collapse in a heap, melt, explode, etc.
					Dying();
				}
			}
		}

		//Debug.DrawRay(sightPoint.transform.position,idealTransformForward,Color.red,0.1f); // RED where we are trying to turn to
		//Debug.DrawRay(sightPoint.transform.position,sightPoint.transform.forward,Color.magenta,0.1f); // MAGENTA where we are currently turned facing
		//if (gunPoint != null && enemy != null) Debug.DrawRay(gunPoint.transform.position,(enemy.transform.position-sightPoint.transform.position),Color.green,0.1f); // Where the gun to enemy ray goes
		switch (currentState) {
			case Const.aiState.Idle: 			Idle(); 		break;
			case Const.aiState.Walk:	 		Walk(); 		break;
			case Const.aiState.Run: 			Run(); 			break;
			case Const.aiState.Attack1: 		Attack1(); 		break;
			case Const.aiState.Attack2: 		Attack2(); 		break;
			case Const.aiState.Attack3: 		Attack3(); 		break;
			case Const.aiState.Pain: 			Pain();			break;
			case Const.aiState.Dying: 		    Dying(); 		break;
			case Const.aiState.Dead: 			Dead(); 		break;
			case Const.aiState.Inspect: 		Inspect(); 		break;
			case Const.aiState.Interacting: 	Interacting();	break;
			default: 					Idle(); 		break;
		}

		if (currentState == Const.aiState.Dead || currentState == Const.aiState.Dying) return; // Don't do any checks, we're dead

		if (asleep) return; // don't check for an enemy, we are sleeping! shh!!

		if (moveType == Const.aiMoveType.Fly && tranquilizeFinished < PauseScript.a.relativeTime) {
			float distUp = 0;
			float distDn = 0;
			Vector3 floorPoint = new Vector3();
			floorPoint = Vector3.zero;
			int layMask = LayerMask.GetMask("Default","Geometry","Bullets","Corpse","Door","InterDebris","PhysObjects","Player","Player2","Player3","Player4");
			if (Physics.Raycast(sightPoint.transform.position, sightPoint.transform.up * -1, out tempHit, sightRange,layMask)) {
				//drawMyLine(sightPoint.transform.position, tempHit.point, Color.green, 2f);
				distDn = Vector3.Distance(sightPoint.transform.position, tempHit.point);
				floorPoint = tempHit.point;
			}

			if (Physics.Raycast(sightPoint.transform.position, sightPoint.transform.up, out tempHit, sightRange,layMask)) {
				//drawMyLine(sightPoint.transform.position, tempHit.point, Color.green, 2f);
				distUp = Vector3.Distance(sightPoint.transform.position, tempHit.point);
			}
			float distT = (distUp + distDn);
			if (flightHeightIsPercentage) {
				idealPos = floorPoint + new Vector3(0,distT * flightDesiredHeight,0);
			} else {
				idealPos = floorPoint + new Vector3(0,flightDesiredHeight, 0);
			}

			if (enemy != null) idealPos.y = enemy.transform.position.y + 0.24f;
			//if (visibleMeshEntity != null) visibleMeshEntity.transform.position = Vector3.MoveTowards(visibleMeshEntity.transform.position, idealPos, runSpeed * Time.deltaTime);
			transform.position = Vector3.MoveTowards(transform.position, idealPos, runSpeed * Time.deltaTime);
		}
	}

	public bool CheckPain() {
		if (asleep) return false;

		if (goIntoPain && timeTillPainFinished < PauseScript.a.relativeTime) {
			if (moveType != Const.aiMoveType.Cyber) currentState = Const.aiState.Pain;
			if (attacker != null) {
				if (timeTillEnemyChangeFinished < PauseScript.a.relativeTime) {
					timeTillEnemyChangeFinished = PauseScript.a.relativeTime + changeEnemyTime;
					enemy = attacker; // Switch to whoever just attacked us
					if (enemy != null) {
						enemyHM = enemy.GetComponent<HealthManager>();
						lastKnownEnemyPos = enemy.transform.position;
						currentDestination = enemy.transform.position;
					}
				}
			}
			goIntoPain = false;
			timeTillPainFinished = PauseScript.a.relativeTime + timeToPain;
			return true;
		}
		return false;
	}

	void Idle() {
		if (enemy != null) {
			currentState = Const.aiState.Run;
			return;
		}

		if (idleTime < PauseScript.a.relativeTime && SFXIdle) {
			float rand = UnityEngine.Random.Range(0,1f); // for calculating 50% chance of idle
			if (rand < 0.5f) {
				if (SFX != null && SFXIdle != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXIdle);
			}
			idleTime = PauseScript.a.relativeTime + Random.Range(idleSFXTimeMin, idleSFXTimeMax);
		}

		if (asleep) {
			rbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		}
			
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		if (!asleep) CheckIfPlayerInSight();
	}

	void Walk() {
        if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		if (asleep) return;
        if (inSight || enemy != null) {
            currentState = Const.aiState.Run;
            return;
        }

        if (moveType == Const.aiMoveType.None) return;

		if (wandering) {
			if (wanderFinished < PauseScript.a.relativeTime || (Vector3.Distance(transform.position,currentDestination) < 0.64f)) {
				wanderFinished = PauseScript.a.relativeTime + UnityEngine.Random.Range(3f,8f);
				currentDestination = new Vector3(UnityEngine.Random.Range(-79f,79f),0,UnityEngine.Random.Range(-79f,79f));
			}
		}

		// destination still far away and turned to within angle to move, then move
		if (Vector3.Distance(sightPoint.transform.position, currentDestination) > 1.28 && tranquilizeFinished < PauseScript.a.relativeTime) {
			//if (WithinAngleToTarget() && !actAsTurret) rbody.AddForce(sightPoint.transform.forward.x * walkSpeed,(sightPoint.transform.forward.y * walkSpeed)+0.5f,sightPoint.transform.forward.z * walkSpeed);
			if (WithinAngleToTarget()) {
				if (hopOnRun) {
					// move it move it
					AnimatorStateInfo asi = hopAnimator.GetCurrentAnimatorStateInfo(0);
					float playbackTime = asi.normalizedTime;
					if (playbackTime > 0.1395f) {
						if (!hopDone) {
							hopDone = true;
							if (!actAsTurret) rbody.AddForce(sightPoint.transform.forward * hopForce);
						}
					} else {
						hopDone = false;
					}
				} else {
					tempVec = (sightPoint.transform.forward * runSpeed);
					tempVec.y = rbody.velocity.y; // carry across gravity
					rbody.velocity = tempVec;
				}
			}
		} else {
            if (visitWaypointsRandomly) {
                currentWaypoint = Random.Range(0, walkWaypoints.Length);
            } else {
                currentWaypoint++;
				if ((currentWaypoint >= walkWaypoints.Length) || (walkWaypoints[currentWaypoint] == null)) {
                    if (dontLoopWaypoints) {
                        currentState = Const.aiState.Idle; // Reached end of waypoints, just stop
                        return;
                    } else {
                        currentWaypoint = 0; // Wrap around
                        if (walkWaypoints[currentWaypoint] == null) {
                            currentState = Const.aiState.Idle;
                            return; // stop walking, out of waypoints
                        }
                    }
                }
			}  
            currentDestination = walkWaypoints[currentWaypoint].transform.position;
        }
	}

	void Run() {
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		if (asleep) return;
		if (enemy == null) {
			currentState = Const.aiState.Idle;
			return;
		}
        if (inSight) {
			//if (enemy == null) {
				//inSight = false;
				//return;
			//}
			if (enemy != null) targettingPosition = enemy.transform.position;
			if (enemy != null) currentDestination = enemy.transform.position;
			if (moveType == Const.aiMoveType.Cyber && enemy != null) {
				pm = enemy.GetComponent<PlayerMovement>();
				if (pm != null) targettingPosition = pm.cameraObject.transform.position;
			}
            huntFinished = PauseScript.a.relativeTime + huntTime;
            if (rangeToEnemy < meleeRange) {
                if (hasMelee && infront && (randomWaitForNextAttack1Finished < PauseScript.a.relativeTime) && tranquilizeFinished < PauseScript.a.relativeTime) {
                    //nav.speed = meleeSpeed;
                    attackFinished = PauseScript.a.relativeTime + timeBetweenAttack1 + timeTillActualAttack1;
                    currentState = Const.aiState.Attack1;
					if (preActivateMeleeColliders) {
						for (int i = 0; i < meleeDamageColliders.Length; i++) {
							//Debug.Log("Melee colliders...activate!");
							if (meleeDamageColliders[i] != null) meleeDamageColliders[i].SetActive(true);
							if (meleeDamageColliders[i] != null) {
								AIMeleeDamageCollider mcs = meleeDamageColliders[i].GetComponent<AIMeleeDamageCollider>();
								if (mcs != null) mcs.MeleeColliderSetup(index, meleeDamageColliders.Length, impactMelee, gameObject);
							}
						}
					}
                    return;
                }
            }
			if (rangeToEnemy < proj1Range) {
				if (hasProj1 && infront && inProjFOV && (randomWaitForNextAttack2Finished < PauseScript.a.relativeTime) && tranquilizeFinished < PauseScript.a.relativeTime) {
					//nav.speed = proj1Speed;
					shotFired = false;
					attackFinished = PauseScript.a.relativeTime + timeBetweenAttack2 + timeTillActualAttack2;
					gracePeriodFinished = PauseScript.a.relativeTime + timeTillActualAttack2;
					currentState = Const.aiState.Attack2;
					return;
				}
			}
			if (rangeToEnemy < proj2Range) {
				if (hasProj2 && infront && inProjFOV && (randomWaitForNextAttack3Finished < PauseScript.a.relativeTime) && tranquilizeFinished < PauseScript.a.relativeTime) {
					//nav.speed = proj2Speed;
					shotFired = false;
					attackFinished = PauseScript.a.relativeTime + timeBetweenAttack3 + timeTillActualAttack3;
					gracePeriodFinished = PauseScript.a.relativeTime + timeTillActualAttack3;
					currentState = Const.aiState.Attack3;
					return;
				}
			}

			// enemy still far away and turned to within angle to move, then move
			if ((moveType != Const.aiMoveType.None) && (rangeToEnemy > 1.28) && tranquilizeFinished < PauseScript.a.relativeTime) {
				//if (WithinAngleToTarget()) rbody.AddForce(transform.forward * runSpeed);
				if (WithinAngleToTarget()) {
					if (hopOnRun) {
						// move it move it
						AnimatorStateInfo asi = hopAnimator.GetCurrentAnimatorStateInfo(0);
						float playbackTime = asi.normalizedTime;
						if (playbackTime > 0.1395f) {
							if (!hopDone) {
								hopDone = true;
								if (!actAsTurret) rbody.AddForce(sightPoint.transform.forward * hopForce);
							}
						} else {
							hopDone = false;
						}
					} else {
						tempVec = (sightPoint.transform.forward * runSpeed);
						if (rbody.useGravity) tempVec.y = rbody.velocity.y; // carry across gravity
						rbody.velocity = tempVec;
					}
				}
				
			}

            if (enemy != null) lastKnownEnemyPos = enemy.transform.position;
        } else {
            if (huntFinished > PauseScript.a.relativeTime) {
                Hunt();
            } else {
                enemy = null;
				enemyHM = null;
				wandering = true; // magically look like we are still searching maybe?  Sometimes!
				wanderFinished = PauseScript.a.relativeTime - 1f;
                currentState = Const.aiState.Walk;
                return;
            }
		}
	}

    void Hunt() {
		if (NavMesh.CalculatePath(sightPoint.transform.position,enemy.transform.position,0,searchPath)) {
			currentDestination = searchPath.corners[0];
		} else {
			currentDestination = lastKnownEnemyPos;
		}

		// dest still far away and turned to within angle to move, then move
		if ((moveType != Const.aiMoveType.None) && (Vector3.Distance(sightPoint.transform.position, currentDestination) > 1.28)) {
			if (WithinAngleToTarget() && !actAsTurret) rbody.velocity = (sightPoint.transform.forward * runSpeed);
		}
    }

	void Attack1() {
		// Used for melee
		if (attack1SoundTime < PauseScript.a.relativeTime) {
			if (SFX != null && SFXAttack1 != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXAttack1);
			attack1SoundTime = PauseScript.a.relativeTime + timeBetweenAttack1;
			int layMask = 10;
			layMask = -layMask;
			if (Physics.Raycast(sightPoint.transform.position,sightPoint.transform.forward,out tempHit, meleeRange, layMask)) {
				tempHM = tempHit.transform.gameObject.GetComponent<HealthManager>();
				if (tempHit.transform.gameObject.GetComponent<HealthManager>() != null) {
					useBlood = true;
					CreateStandardImpactEffects(false);
					damageData.other = tempHit.transform.gameObject;
					if (tempHit.transform.gameObject.CompareTag("NPC")) {
						damageData.isOtherNPC = true;
					} else {
						damageData.isOtherNPC = false;
					}
					damageData.hit = tempHit;
					damageData.attacknormal = transform.forward;
					damageData.owner = gameObject;
					damageData.attackType = Const.AttackType.Projectile;
					damageData.damage = Const.a.damageForNPC[index];
					damageData.armorvalue = Const.a.armorvalueForNPC[index];
					damageData.damage = Const.a.GetDamageTakeAmount(damageData); // run it through the system and perform all checks
					HealthManager hm = tempHit.transform.gameObject.GetComponent<HealthManager>();
					if (hm == null) return;
					hm.TakeDamage(damageData);
				}
			}
            //for (int i = 0; i < meleeDamageColliders.Length; i++) {
				//Debug.Log("Melee colliders...activate!");
                //meleeDamageColliders[i].SetActive(true);
                //meleeDamageColliders[i].GetComponent<AIMeleeDamageCollider>().MeleeColliderSetup(index, meleeDamageColliders.Length, impactMelee, gameObject);
            //}
			//Debug.Log("Did...did we do it?  Did the melee colliders activate?");
        }

		if (Vector3.Distance(sightPoint.transform.position, currentDestination) > 1f && tranquilizeFinished < PauseScript.a.relativeTime) {
			if (WithinAngleToTarget() && !actAsTurret) rbody.AddForce(transform.forward * meleeSpeed);
		}
        currentDestination = enemy.transform.position;

        if (attackFinished < PauseScript.a.relativeTime) {
            //for (int i = 0; i < meleeDamageColliders.Length; i++) {
                //meleeDamageColliders[i].SetActive(false); // turn off melee colliders
            //}

            if (Random.Range(0f,1f) < attack1RandomWaitChance) {
                randomWaitForNextAttack1Finished = PauseScript.a.relativeTime + Random.Range(attack1MinRandomWait, attack1MaxRandomWait);
            } else {
                randomWaitForNextAttack1Finished = PauseScript.a.relativeTime;
            }
            goIntoPain = false; //prevent going into pain after attack
			currentState = Const.aiState.Run;
			return; // Done with attack
		}
	}

    bool WithinAngleToTarget () {
		if (idealTransformForward.magnitude > Mathf.Epsilon) {
			if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(idealTransformForward)) < fieldOfViewStartMovement) {
				return true;
			}
		}
        return false;
    }

    bool DidRayHit(Vector3 targPos, float dist, bool useGunPoint2) {
		if (useGunPoint2 && gunPoint2 != null) {
			tempVec = targPos - gunPoint2.transform.position;
		} else {
			tempVec = targPos - gunPoint.transform.position;
		}
		tempVec = tempVec.normalized;
		int layMask = LayerMask.GetMask("Default","Geometry","Bullets","Corpse","Door","InterDebris","PhysObjects","Player","Player2","Player3","Player4");

		if (useGunPoint2 && gunPoint2 != null) {
			if (Physics.Raycast(gunPoint2.transform.position, tempVec, out tempHit, sightRange, layMask)) {
				tempHM = tempHit.transform.gameObject.GetComponent<HealthManager>();
				if (tempHit.transform.gameObject.GetComponent<HealthManager>() != null) {
					useBlood = true;
				}
				//drawMyLine(gunPoint2.transform.position, tempHit.point, Color.green, 2f);
				return true;
			}
		} else {
			if (Physics.Raycast(gunPoint.transform.position, tempVec, out tempHit, sightRange, layMask)) {
				tempHM = tempHit.transform.gameObject.GetComponent<HealthManager>();
				if (tempHit.transform.gameObject.GetComponent<HealthManager>() != null) {
					useBlood = true;
				}
				//drawMyLine(gunPoint.transform.position, tempHit.point, Color.green, 2f);
				return true;
			}
		}
        return false;
    }

    GameObject GetImpactType(HealthManager hm) {
        if (hm == null) return Const.a.GetObjectFromPool(Const.PoolType.SparksSmall);
        switch (hm.bloodType) {
            case HealthManager.BloodType.None: return Const.a.GetObjectFromPool(Const.PoolType.SparksSmall);
            case HealthManager.BloodType.Red: return Const.a.GetObjectFromPool(Const.PoolType.BloodSpurtSmall);
            case HealthManager.BloodType.Yellow: return Const.a.GetObjectFromPool(Const.PoolType.BloodSpurtSmallYellow);
            case HealthManager.BloodType.Green: return Const.a.GetObjectFromPool(Const.PoolType.BloodSpurtSmallGreen);
            case HealthManager.BloodType.Robot: return Const.a.GetObjectFromPool(Const.PoolType.SparksSmallBlue);
			case HealthManager.BloodType.Leaf: return Const.a.GetObjectFromPool(Const.PoolType.LeafBurst);
			case HealthManager.BloodType.Mutation: return Const.a.GetObjectFromPool(Const.PoolType.MutationBurst);
			case HealthManager.BloodType.GrayMutation: return Const.a.GetObjectFromPool(Const.PoolType.GraytationBurst);
        }

        return Const.a.GetObjectFromPool(Const.PoolType.SparksSmall);
    }

    void CreateStandardImpactEffects(bool onlyBloodIfHitHasHM) {
        // Determine blood type of hit target and spawn corresponding blood particle effect from the Const.Pool
        if (useBlood) {
            GameObject impact = GetImpactType(tempHM);
            if (impact != null) {
                tempVec = tempHit.normal;
                impact.transform.position = tempHit.point + (tempVec*0.08f);
                impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, tempHit.normal);
                impact.SetActive(true);
            }
        } else {
            // Allow for skipping adding sparks after special override impact effects per attack functions below
            if (!onlyBloodIfHitHasHM) {
                GameObject impact = Const.a.GetObjectFromPool(Const.PoolType.SparksSmall); //Didn't hit an object with a HealthManager script, use sparks
                if (impact != null) {
                    tempVec = tempHit.normal;
                    impact.transform.position = tempHit.point + tempVec;
                    impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, tempHit.normal);
                    impact.SetActive(true);
                }
            }
        }
    }

    void Attack2() {
		if (Vector3.Distance(sightPoint.transform.position, currentDestination) > 1.28) {
			if (WithinAngleToTarget() && !actAsTurret) rbody.AddForce(transform.forward * proj1Speed);
		}
        currentDestination = targettingPosition;

        if (gracePeriodFinished < PauseScript.a.relativeTime) {
            if (!shotFired) {
                shotFired = true;
                // Typically used for normal projectile attack
                if (attack2SoundTime < PauseScript.a.relativeTime && SFXAttack2) {
					if (SFX != null && SFXAttack2 != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXAttack2);
                    attack2SoundTime = PauseScript.a.relativeTime + timeBetweenAttack2;
                }

                if (attack2Type == Const.AttackType.Projectile) {
                    if (muzzleBurst != null) muzzleBurst.SetActive(true);
                    if (DidRayHit(targettingPosition, proj1Range,false)) {
                        damageData.other = tempHit.transform.gameObject;
                        if (tempHit.transform.gameObject.CompareTag("NPC")) {
                            damageData.isOtherNPC = true;
                        } else {
                            damageData.isOtherNPC = false;
                        }
                        damageData.hit = tempHit;
                        tempVec = gunPoint.transform.position;
                        tempVec = (enemy.transform.position - tempVec);
                        damageData.attacknormal = tempVec;
                        damageData.damage = Const.a.damageForNPC2[index];
                        damageData.damage = Const.a.GetDamageTakeAmount(damageData);
                        damageData.owner = gameObject;
                        damageData.attackType = Const.AttackType.Projectile;
						if (attack1UsesLaser) {
							GameObject dynamicObjectsContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
							if (dynamicObjectsContainer == null) return; //didn't find current level
							GameObject lasertracer = Instantiate(Const.a.useableItems[105],transform.position,Quaternion.identity) as GameObject;
							if (lasertracer != null) {
								lasertracer.transform.SetParent(dynamicObjectsContainer.transform,true);
								lasertracer.GetComponent<LaserDrawing>().startPoint = sightPoint.transform.position;
								lasertracer.GetComponent<LaserDrawing>().endPoint = tempHit.point;
								lasertracer.SetActive(true);
							}
						}
                        HealthManager hm = tempHit.transform.gameObject.GetComponent<HealthManager>();
                        if (hm == null) {
							CreateStandardImpactEffects(false);
							return;
						}
                        hm.TakeDamage(damageData);
                    }
                } else {
					if (attack2Type == Const.AttackType.ProjectileLaunched) {
						if (muzzleBurst != null) muzzleBurst.SetActive(true);
						if (moveType != Const.aiMoveType.Cyber) {
							tempVec = (enemy.transform.position - gunPoint.transform.position);
						} else {
							tempVec = enemy.transform.position;
							tempVec.y += 0.84f;
							tempVec = (enemy.transform.position - gunPoint.transform.position);
						}
                        damageData.attacknormal = tempVec;
                        damageData.damage = Const.a.damageForNPC2[index];
                        damageData.damage = Const.a.GetDamageTakeAmount(damageData);
                        damageData.owner = gameObject;
                        damageData.attackType = Const.AttackType.ProjectileLaunched;
						// Create and hurl a beachball-like object.  On the developer commentary they said that the projectiles act
						// like a beachball for collisions with enemies, but act like a baseball for walls/floor to prevent hitting corners
						GameObject beachball = Const.a.GetObjectFromPool(attack2ProjectileLaunchedType);
						if (beachball != null) {
							beachball.GetComponent<ProjectileEffectImpact>().dd = damageData;
							beachball.GetComponent<ProjectileEffectImpact>().host = gameObject;
							beachball.GetComponent<ProjectileEffectImpact>().impactType = Const.a.GetPoolImpactFromPoolProjectileType(attack2ProjectileLaunchedType);
							beachball.transform.position = gunPoint.transform.position;
							beachball.transform.forward = tempVec.normalized;
							beachball.SetActive(true);
							GrenadeActivate ga = beachball.GetComponent<GrenadeActivate>();
							if (ga != null) {
								int typ = 7; // frag
								if (attack2ProjectileLaunchedType == Const.PoolType.GrenadeFragLive) typ = 7; // Fragmentation Grenade
								if (attack2ProjectileLaunchedType == Const.PoolType.ConcussionLive) typ = 8; // Concussion Grenade
								if (attack2ProjectileLaunchedType == Const.PoolType.EMPLive) typ = 9; // EMP Grenade
								if (attack2ProjectileLaunchedType == Const.PoolType.GasLive) typ = 13; // Gas Grenade
								ga.Activate(typ,null);
							}
							Vector3 shove = (beachball.transform.forward * attack2projectilespeed);
							shove += rbody.velocity; // add in the enemy's velocity to the projectile (in case they are riding on a moving platform or something - wait I don't have those!
							beachball.GetComponent<Rigidbody>().velocity = Vector3.zero; // prevent random variation from the last shot's velocity
							beachball.GetComponent<Rigidbody>().AddForce(shove, ForceMode.Impulse);
						}
					}
				}
            }
        }

        if (attackFinished < PauseScript.a.relativeTime) {
            if (Random.Range(0f,1f) < attack2RandomWaitChance) {
                randomWaitForNextAttack2Finished = PauseScript.a.relativeTime + Random.Range(attack2MinRandomWait, attack2MaxRandomWait);
            } else {
                randomWaitForNextAttack2Finished = PauseScript.a.relativeTime;
            }
            if (muzzleBurst != null) muzzleBurst.SetActive(false);
            goIntoPain = false; //prevent going into pain after attack
            currentState = Const.aiState.Run;
			//Debug.Log("Return from Attack2");
            return;
        }
	}

	void Attack3() {
		// Typically used for secondary projectile or grenade attack
		if (attack3SoundTime < PauseScript.a.relativeTime && SFXAttack3 || explodeOnAttack3) {
			if (SFX != null && SFXAttack3 != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXAttack3);
			attack3SoundTime = PauseScript.a.relativeTime + timeBetweenAttack3;
		}

		if (Vector3.Distance(sightPoint.transform.position, currentDestination) > 1.28) {
			if (WithinAngleToTarget() && !actAsTurret) rbody.AddForce(transform.forward * proj2Speed);
		}
        currentDestination = targettingPosition;

		if (explodeOnAttack3) {
			DamageData dd = Const.SetNPCDamageData(index, Const.aiState.Attack3,gameObject);
			float take = Const.a.GetDamageTakeAmount(dd);
			dd.other = gameObject;
			dd.damage = take;
			HealthManager hm = null;
			if (dd == null) return;
			float damageOriginal = dd.damage;
			Collider[] colliders = Physics.OverlapSphere(sightPoint.transform.position, attack3Radius);
			int i = 0;
			while (i < colliders.Length) {
				if (colliders[i] != null) {
					if (colliders[i].GetComponent<Rigidbody>() != null) {
						colliders[i].GetComponent<Rigidbody>().AddExplosionForce(attack3Force, sightPoint.transform.position, attack3Radius, 1.0f);
					}
					hm = colliders[i].GetComponent<HealthManager>();
					if (hm != null) {
						DamageData dnew = dd;
						dnew.damage = dd.damage;// * ((Vector3.Distance(colliders[i].gameObject.transform.position,sightPoint.transform.position))/attack3Radius);
						hm.TakeDamage(dd);
					}
				}
				
				i++;
			}
			healthManager.TakeDamage(dd);
			return;
		}

        if (gracePeriodFinished < PauseScript.a.relativeTime) {
            if (!shotFired) {
                shotFired = true;
                // Typically used for normal projectile attack
                if (attack3Type == Const.AttackType.Projectile) {
                    if (muzzleBurst2 != null) muzzleBurst2.SetActive(true);
					if (hasLaserForProj2) {
						if (laserLightning != null) {
							laserLightning.EndObject = null; // force lightning to use world space
							laserLightning.EndPosition = targettingPosition;
						}
					}
                    if (DidRayHit(targettingPosition, proj1Range,true)) {
                        damageData.other = tempHit.transform.gameObject;
                        if (tempHit.transform.gameObject.CompareTag("NPC")) {
                            damageData.isOtherNPC = true;
                        } else {
                            damageData.isOtherNPC = false;
                        }
                        damageData.hit = tempHit;
                        tempVec = gunPoint2.transform.position;
                        tempVec = (enemy.transform.position - tempVec);
                        damageData.attacknormal = tempVec;
                        damageData.damage = Const.a.damageForNPC3[index];
                        damageData.damage = Const.a.GetDamageTakeAmount(damageData);
                        damageData.owner = gameObject;
                        damageData.attackType = Const.AttackType.Projectile;
                        HealthManager hm = tempHit.transform.gameObject.GetComponent<HealthManager>();
                        if (hm == null) {
							CreateStandardImpactEffects(false);
							return;
						}
                        hm.TakeDamage(damageData);
                    }
                } else {
					if (attack3Type == Const.AttackType.ProjectileLaunched) {
                        tempVec = gunPoint2.transform.position;
                        tempVec = (enemy.transform.position - tempVec);
                        damageData.attacknormal = tempVec;
                       	damageData.damage = Const.a.damageForNPC3[index];
                        damageData.damage = Const.a.GetDamageTakeAmount(damageData);
                        damageData.owner = gameObject;
                        damageData.attackType = Const.AttackType.ProjectileLaunched;
						// Create and hurl a beachball-like object.  On the developer commentary they said that the projectiles act
						// like a beachball for collisions with enemies, but act like a baseball for walls/floor to prevent hitting corners
						GameObject beachball = Const.a.GetObjectFromPool(attack3ProjectileLaunchedType);
						if (beachball != null) {
							ProjectileEffectImpact pei = beachball.GetComponent<ProjectileEffectImpact>();
							if (pei != null) {
								pei.dd = damageData;
								pei.host = gameObject;
								pei.impactType = Const.a.GetPoolImpactFromPoolProjectileType(attack3ProjectileLaunchedType);
							}
							beachball.transform.position = gunPoint2.transform.position;
							beachball.transform.forward = tempVec.normalized;
							beachball.SetActive(true);
							GrenadeActivate ga = beachball.GetComponent<GrenadeActivate>();
							if (ga != null) {
								int typ = 7; // frag
								if (attack2ProjectileLaunchedType == Const.PoolType.GrenadeFragLive) typ = 7; // Fragmentation Grenade
								if (attack2ProjectileLaunchedType == Const.PoolType.ConcussionLive) typ = 8; // Concussion Grenade
								if (attack2ProjectileLaunchedType == Const.PoolType.EMPLive) typ = 9; // EMP Grenade
								if (attack2ProjectileLaunchedType == Const.PoolType.GasLive) typ = 13; // Gas Grenade
								ga.Activate(typ,null);
							}
							Vector3 shove = beachball.transform.forward * attack3projectilespeed;
							shove += rbody.velocity; // add in the enemy's velocity to the projectile (in case they are riding on a moving platform or something - wait I don't have those!
							beachball.GetComponent<Rigidbody>().velocity = Vector3.zero; // prevent random variation from the last shot's velocity
							beachball.GetComponent<Rigidbody>().AddForce(shove, ForceMode.Impulse);
						}
					}
				}
            }
        }

        if (attackFinished < PauseScript.a.relativeTime) {
            if (Random.Range(0f,1f) < attack3RandomWaitChance) {
                randomWaitForNextAttack3Finished = PauseScript.a.relativeTime + Random.Range(attack3MinRandomWait, attack3MaxRandomWait);
            } else {
                randomWaitForNextAttack3Finished = PauseScript.a.relativeTime;
            }
            if (muzzleBurst2 != null) muzzleBurst2.SetActive(false);
            goIntoPain = false; //prevent going into pain after attack
            currentState = Const.aiState.Run;
            return;
        }
	}

	void Pain() {
		if (timeTillPainFinished < PauseScript.a.relativeTime) {
			currentState = Const.aiState.Run; // go into run after we get hurt
			goIntoPain = false;
			timeTillPainFinished = PauseScript.a.relativeTime + timeBetweenPain;
		}
	}

	void Dying() {
		if (!dyingSetup) {
			if (sleepingCables != null) sleepingCables.SetActive(false);
			if (npcAutomapOverlay != null) {
				npcAutomapOverlay.gameObject.SetActive(false);
				npcAutomapOverlay = null;
				npcAutomapOverlayImage = null;
			}
			dyingSetup = true;
			if (deathBurstTimer > 0) {
				deathBurstFinished = PauseScript.a.relativeTime + deathBurstTimer;
			} else {
				if (!deathBurstDone) {
					if (deathBurst != null) deathBurst.SetActive(true); // activate any death effects
					deathBurstDone = true;
				}
			}

			for (int i = 0; i < meleeDamageColliders.Length; i++) {
				//Debug.Log("Melee colliders deactivated");
                if (meleeDamageColliders[i] != null) meleeDamageColliders[i].SetActive(false);
            }

			if (!healthManager.actAsCorpseOnly && SFX != null && SFXDeathClip != null) SFX.PlayOneShot(SFXDeathClip);
			//if (useGravityOnDying) {
			if (moveType == Const.aiMoveType.Fly && !healthManager.gibOnDeath) {
				rbody.useGravity = true; // for avian mutant and zero-g mutant
			} else {
				rbody.useGravity = true;
				rbody.isKinematic = true;
				//RaycastHit hit;
				//if (Physics.Raycast(sightPoint.transform.position, Vector3.down, out hit, sightRange)) {
					//if (hit.collider.gameObject.CompareTag("Geometry"))
						//transform.parent = hit.collider.gameObject.transform;
				//}
				//rbody.velocity = new Vector3(rbody.velocity.x,0,rbody.velocity.z);
				//rbody.Sleep();
			}
			asleep = false;
			rbody.constraints = RigidbodyConstraints.None;
			if (!rbody.freezeRotation) rbody.freezeRotation = true;
			gameObject.layer = 13; // Change to Corpse layer
			transform.position = new Vector3(transform.position.x, transform.position.y + 0.04f, transform.position.z); // bump it up a hair to prevent corpse falling through the floor
			firstSighting = true;
			timeTillDeadFinished = PauseScript.a.relativeTime + timeTillDead; // wait for death animation to finish before going into Dead()
			if (switchMaterialOnDeath) actualSMR.material = deathMaterial;
		}
			
		if (timeTillDeadFinished < PauseScript.a.relativeTime) {
			ai_dead = true;
			ai_dying = false;
			currentState = Const.aiState.Dead;
		}
	}

	void Dead() {
		asleep = false;
		ai_dead = true;
		ai_dying = false;
        if (boxCollider != null) { if (boxCollider.enabled) boxCollider.enabled = false; }
        if (sphereCollider != null) { if (sphereCollider.enabled) sphereCollider.enabled = false; }
        if (meshCollider != null) { if (meshCollider.enabled) meshCollider.enabled = false; }
		if (capsuleCollider != null) { if (capsuleCollider.enabled) capsuleCollider.enabled = false; }

		//if (useGravityOnDeath) {
			rbody.useGravity = true;
		//} else {
		//	rbody.useGravity = false;
		//}

		if (searchColliderGO != null) {
			searchColliderGO.SetActive(true);
			rbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
		}
		if (!rbody.freezeRotation) rbody.freezeRotation = true;
		currentState = Const.aiState.Dead;
		if (npcAutomapOverlayImage != null) npcAutomapOverlayImage.enabled = false;
		if (healthManager.gibOnDeath || healthManager.teleportOnDeath || healthManager.inCyberSpace) {
			if (visibleMeshEntity != null && visibleMeshEntity.activeInHierarchy) visibleMeshEntity.SetActive(false); // normally just turn off the main model, then...
			if (healthManager.gibOnDeath) healthManager.Gib(); // ... turn on the lovely gibs
			if (healthManager.teleportOnDeath && !healthManager.teleportDone) healthManager.TeleportAway();
		}
	}

	void Inspect() {
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
	}
		
	void Interacting() {
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
	}

	bool CheckIfEnemyInSight() {
		if (moveType == Const.aiMoveType.Cyber && Const.a.decoyActive) {
			LOSpossible = false;
			return false;
		}
		float dist = Vector3.Distance(enemy.transform.position,sightPoint.transform.position);  // Get distance between enemy and found player	
		//if (dist < distToSeeWhenBehind && !enemy.GetComponent<PlayerMovement>().Notarget) {
        //    LOSpossible = true;
		//	return true;
		//}
		if (npcAutomapOverlayImage != null) {
			//if (dist < 50f) {
				npcAutomapOverlayImage.enabled = true;
			//} else {
			//	npcAutomapOverlayImage.enabled = false;
			//}
		}
		if (dist > sightRange) return false;

		Vector3 checkline = enemy.transform.position - sightPoint.transform.position; // Get vector line made from enemy to found player
		int layMask = LayerMask.GetMask("Default","Geometry","Corpse","Door","InterDebris","PhysObjects","Player","Player2","Player3","Player4");
        RaycastHit hit;
        if (Physics.Raycast(sightPoint.transform.position, checkline.normalized, out hit, sightRange, layMask)) {
			//Debug.DrawRay(sightPoint.transform.position + transform.up,checkline.normalized, Color.green, 0.1f,false);
			//Debug.DrawLine(sightPoint.transform.position,hit.point,Color.green,0.1f,false);
            if (hit.collider.gameObject == enemy) {
				LOSpossible = true;
                return true;
			}
        }
        LOSpossible = false;
        return false;
	}

	bool CheckIfPlayerInSight () {
        if (ignoreEnemy || Const.a.difficultyCombat == 0) return false;
		if (enemy != null) return CheckIfEnemyInSight();
		if (moveType == Const.aiMoveType.Cyber && Const.a.decoyActive) return false;
		LOSpossible = false;

		if (Const.a.player1Capsule == null) return false; // no found player
		// found player
		if (Const.a.player1PlayerMovementScript.Notarget) return false; // can't see him, he's on notarget. skip to next available player to check against
		tempVec = Const.a.player1Capsule.transform.position;
		Vector3 checkline = tempVec - sightPoint.transform.position; // Get vector line made from enemy to found player
		float dist = Vector3.Distance(tempVec,sightPoint.transform.position);  // Get distance between enemy and found player
		if (npcAutomapOverlayImage != null) {
			//if (dist < 50f) {
				if (healthManager.health > 0 ) npcAutomapOverlayImage.enabled = true;
			//} else {
			//	npcAutomapOverlayImage.enabled = false;
			//}
		}
		if (dist > sightRange) return false; // don't waste time doing raycasts if we won't be able to see them anyway

		float angle = Vector3.Angle(checkline,sightPoint.transform.forward);
		if (angle < (fieldOfViewAngle * 0.5f)) {
			// Check for line of sight
			RaycastHit hit;
			int layMask = LayerMask.GetMask("Default","Geometry","Bullets","Corpse","Door","InterDebris","PhysObjects","Player","Player2","Player3","Player4");
			// changed from using sightRange to dist to minimize checkdistance
			if (Physics.Raycast(sightPoint.transform.position, checkline.normalized, out hit, (dist + 0.1f),layMask)) {
				//Debug.Log("Sight raycast successful");
				//Debug.DrawRay(sightPoint.transform.position,checkline.normalized, Color.green, 0.1f,false);
				//Debug.DrawLine(sightPoint.transform.position,hit.point,Color.green,0.1f,false);
				//Debug.Log(hit.collider.gameObject.name);
				if (hit.collider.gameObject == Const.a.player1Capsule) {
					//Debug.Log("Hit collider was player");
					LOSpossible = true;  // Clear path from enemy to found player
					SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
					PlaySightSound();
					return true;
				}
			} else {
				// if (dist < distToSeeWhenBehind) {
					// SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
					// PlaySightSound();
					// return true;
				// }
				if (Const.a.player1PlayerHealthScript != null) {
					if (Const.a.player1PlayerHealthScript.makingNoise) {
						if (dist < rangeToHear) {
							SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
							PlaySightSound();
							return true;
						}
					}
				}
			}
		} else {
			if (dist < distToSeeWhenBehind) {
				// Still check for line of sight, some locations there could be walls in the ways still due to the angles
				RaycastHit hit;
				int layMask = LayerMask.GetMask("Default","Geometry","Bullets","Corpse","Door","InterDebris","PhysObjects","Player","Player2","Player3","Player4");
				// changed from using sightRange to dist to minimize checkdistance
				if (Physics.Raycast(sightPoint.transform.position, checkline.normalized, out hit, (dist + 0.1f),layMask)) {
					//Debug.Log("Sight raycast successful");
					//Debug.DrawRay(sightPoint.transform.position,checkline.normalized, Color.green, 0.1f,false);
					//Debug.DrawLine(sightPoint.transform.position,hit.point,Color.green,0.1f,false);
					//Debug.Log(hit.collider.gameObject.name);
					if (hit.collider.gameObject == Const.a.player1Capsule) {
						//Debug.Log("Hit collider was player");
						LOSpossible = true;  // Clear path from enemy to found player
						SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
						PlaySightSound();
						return true;
					}
				}
				// SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
				// PlaySightSound();
				// return true;
			}
			if (Const.a.player1PlayerHealthScript != null) {
				if (Const.a.player1PlayerHealthScript.makingNoise) {
					if (dist < rangeToHear) {
						SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
						PlaySightSound();
						return true;
					}
				}
			}
		}

		return false;
	}

	void SetEnemy(GameObject enemSent,Transform targettingPosSent) {
		if (enemSent == null) return;
		enemy = enemSent;
		enemyHM = enemSent.GetComponent<HealthManager>();
		lastKnownEnemyPos = enemy.transform.position;
		targettingPosition = targettingPosSent.position;
	}

	void PlaySightSound() {
		if (firstSighting) {
			firstSighting = false;
			if (!healthManager.actAsCorpseOnly && SFXSightSound != null && SFX != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXSightSound);
		}
	}
	
	void enemyInFrontChecks(GameObject target) {
        infrontVec = target.transform.position;
		infrontVec.y = sightPoint.transform.position.y; // ignore height difference
		infrontVec = Vector3.Normalize(infrontVec - sightPoint.transform.position);
		inProjFOV = false;
		infront = false;
        dotResult = Vector3.Dot(infrontVec,sightPoint.transform.forward);
		dotProjResult = dotResult;
        if (dotProjResult > 0.800) inProjFOV = true; // enemy is within ±18° of forward facing vector
        if (dotResult > 0.300) infront = true; // enemy is within ±63° of forward facing vector
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
			if (ud.owner == tempent) {
				enemy = tempent;
				if (enemy != null) enemyHM = enemy.GetComponent<HealthManager>();
			} else {
				if (tempent != enemy) enemy = tempent;
				if (enemy != null) enemyHM = enemy.GetComponent<HealthManager>();
			}
		}
	}

	public void AwakeFromSleep(UseData ud) {
		if (sleepingCables != null) sleepingCables.SetActive(false);
		asleep = false;
		Alert(ud);
	}

	void drawMyLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		StartCoroutine( drawLine(start, end, color, duration));
	}

	IEnumerator drawLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		GameObject myLine = new GameObject ();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer> ();
		LineRenderer lr = myLine.GetComponent<LineRenderer> ();
		lr.material = new Material (Shader.Find ("Legacy Shaders/Particles/Additive"));
		lr.startColor = color;
		lr.endColor = color;
		lr.startWidth = 0.1f;
		lr.endWidth = 0.1f;
		lr.SetPosition (0, start);
		lr.SetPosition (1, end);
		yield return new WaitForSeconds(duration);
		GameObject.Destroy (myLine);
	}
}
