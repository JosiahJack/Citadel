using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {
	public int index = 0; // NPC reference index for looking up constants in tables in Const.cs
	public Const.aiState currentState;
    public Const.aiMoveType moveType;
	public GameObject enemy;
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
    public float gracePeriodFinished;
    [HideInInspector]
    public float meleeDamageFinished;
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
    public bool dontLoopWaypoints = false;
	public bool visitWaypointsRandomly = false;
	public bool hasMelee = true;
	public bool hasProj1 = false;
	public bool hasProj2 = false;
	public Transform[] walkWaypoints; // point(s) for NPC to walk to when roaming or patrolling
	public bool inSight = false;
    public bool infront;
    public bool inProjFOV;
    public bool LOSpossible;
    public bool goIntoPain = false;
	public bool explodeOnAttack3 = false;
    public bool ignoreEnemy = false;
	public float rangeToEnemy = 0f;
	public GameObject[] meleeDamageColliders;
    public GameObject muzzleBurst;
    public GameObject muzzleBurst2;
    public GameObject rrCheckPoint;
	[HideInInspector]
	public GameObject attacker;
	//private bool hasSFX;
	public bool firstSighting;
	private bool dyingSetup;
	private bool ai_dying;
	private bool ai_dead;
	private int currentWaypoint;
	private Vector3 currentDestination;
	private float idleTime;
	private float attack1SoundTime;
	private float attack2SoundTime;
	private float attack3SoundTime;
	private float timeTillEnemyChangeFinished;
	private float timeTillDeadFinished;
	private float timeTillPainFinished;
	private AudioSource SFX;
	private Rigidbody rbody;
	private HealthManager healthManager;
	private BoxCollider boxCollider;
	private CapsuleCollider capsuleCollider;
	private SphereCollider sphereCollider;
	private MeshCollider meshCollider;
	private float tick;
	private float tickFinished;
    public float huntTime = 5f;
    public float huntFinished;
	private bool hadEnemy;
    private Vector3 lastKnownEnemyPos;
    private Vector3 tempVec;
    //private bool randSpin = false;
    private bool shotFired = false;
    private DamageData damageData;
    private RaycastHit tempHit;
    private bool useBlood;
    private HealthManager tempHM;
    private float randomWaitForNextAttack1Finished;
    private float randomWaitForNextAttack2Finished;
    private float randomWaitForNextAttack3Finished;
    public GameObject visibleMeshEntity;
    public GameObject gunPoint;
    public GameObject gunPoint2;
	public Vector3 idealTransformForward;
    public Vector3 idealPos; // used by flyers to establish correct height
	public float flightDesiredHeight = 0.75f;
	public bool flightHeightIsPercentage = true;
	[HideInInspector]
	public float attackFinished;
	[HideInInspector]
    public float attack2Finished;
	[HideInInspector]
    public float attack3Finished;
    public Vector3 targettingPosition; // used to give the player a chance to dodge attacks by moving after start of an attack, enemy attacks along same starting line
	public GameObject explosionObject;
	private ExplosionForce explosion;
	public Material deathMaterial;
	public bool switchMaterialOnDeath;
	public SkinnedMeshRenderer actualSMR;
	public BoxCollider flyerCollider;
	public CapsuleCollider flyerColliderAlternate1;
	public bool specialCaseGibContainered;
	public GameObject visibleMeshSubObject;
	public GameObject deathBurst;
	public float deathBurstTimer = 0.1f;
	private float deathBurstFinished;
	private bool deathBurstDone;
	public GameObject sightPoint;
	private NavMeshPath searchPath;

	// Initialization and find components
	void Awake () {
        rbody = GetComponent<Rigidbody>();
		//rbody.isKinematic = true;
		if (moveType != Const.aiMoveType.Fly) rbody.isKinematic = false;
		healthManager = GetComponent<HealthManager>();
	    boxCollider = GetComponent<BoxCollider>();
		sphereCollider = GetComponent<SphereCollider>();
		meshCollider = GetComponent<MeshCollider>();
		capsuleCollider = GetComponent<CapsuleCollider>();
        if (searchColliderGO != null) searchColliderGO.SetActive(false);
		currentDestination = sightPoint.transform.position;
		if (meleeDamageColliders.Length > 0) {
			for (int i = 0; i < meleeDamageColliders.Length; i++) {
				meleeDamageColliders[i].SetActive(false); // turn off melee colliders
			}
		}

        currentState = Const.aiState.Idle;
		currentWaypoint = 0;
		enemy = null;
		firstSighting = true;
		inSight = false;
		goIntoPain = false;
		dyingSetup = false;
		ai_dead = false;
		ai_dying = false;
		attacker = null;
        shotFired = false;
		idleTime = Time.time + Random.Range(idleSFXTimeMin,idleSFXTimeMax);
		attack1SoundTime = Time.time;
		attack2SoundTime = Time.time;
		attack3SoundTime = Time.time;
		timeTillEnemyChangeFinished = Time.time;
        huntFinished = Time.time;
		attackFinished = Time.time;
		attack2Finished = Time.time;
        attack3Finished = Time.time;
        timeTillPainFinished = Time.time;
		timeTillDeadFinished = Time.time;
        meleeDamageFinished = Time.time;
        gracePeriodFinished = Time.time;
        randomWaitForNextAttack1Finished = Time.time;
        randomWaitForNextAttack2Finished = Time.time;
        randomWaitForNextAttack3Finished = Time.time;
		deathBurstFinished = Time.time;
        damageData = new DamageData();
		damageData.ownerIsNPC = true;
        tempHit = new RaycastHit();
        tempVec = new Vector3(0f, 0f, 0f);
        SFX = GetComponent<AudioSource>();
		if (explosionObject != null) explosion = explosionObject.GetComponent<ExplosionForce>();
		if (SFX == null) Debug.Log("WARNING: No audio source for npc at: " + transform.position.x.ToString() + ", " + transform.position.y.ToString() + ", " + transform.position.z + ".");
		if (walkWaypoints.Length > 0 && walkWaypoints[currentWaypoint] != null && walkPathOnStart) {
            currentDestination = walkWaypoints[currentWaypoint].transform.position;
            currentState = Const.aiState.Walk; // If waypoints are set, start walking them from the get go
		} else {
            currentState = Const.aiState.Idle; // No waypoints, stay put
        }
		//randSpin = true;
		tick = 0.05f;
		tickFinished = Time.time + tick;
		attackFinished = Time.time + 1f;
		idealTransformForward = sightPoint.transform.forward;
		deathBurstDone = false;
		searchPath = new NavMeshPath();
	}

	void FixedUpdate () {
		if (PauseScript.a != null && PauseScript.a.Paused()) {
			return; // don't do any checks or anything else...we're paused!
		}

        // Only think every tick seconds to save on CPU and prevent race conditions
        if (tickFinished < Time.time) {
			Think();
			tickFinished = Time.time + tick;
		}

        // Rotation and Special movement that must be done every FixedUpdate
        if (currentState != Const.aiState.Dead) {
            if (currentState != Const.aiState.Idle) {
                idealTransformForward = currentDestination - sightPoint.transform.position;
                idealTransformForward.y = 0;
				idealTransformForward = Vector3.Normalize(idealTransformForward);
				if (idealTransformForward.magnitude > Mathf.Epsilon) {
					Quaternion rot = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(idealTransformForward), yawspeed * Time.deltaTime);
					transform.rotation = rot;
				}
            }

            if (moveType == Const.aiMoveType.Fly) {
                float distUp = 0;
                float distDn = 0;
                Vector3 floorPoint = new Vector3();
                floorPoint = Vector3.zero;

                if (Physics.Raycast(sightPoint.transform.position, visibleMeshEntity.transform.up * -1, out tempHit, sightRange)) {
                    //drawMyLine(sightPoint.transform.position, tempHit.point, Color.green, 2f);
                    distDn = Vector3.Distance(sightPoint.transform.position, tempHit.point);
                    floorPoint = tempHit.point;
                }
                if (Physics.Raycast(sightPoint.transform.position, visibleMeshEntity.transform.up, out tempHit, sightRange)) {
                    //drawMyLine(sightPoint.transform.position, tempHit.point, Color.green, 2f);
                    distUp = Vector3.Distance(sightPoint.transform.position, tempHit.point);
                }
                float distT = (distUp + distDn);
                if (flightHeightIsPercentage) {
					idealPos = floorPoint + new Vector3(0,distT * flightDesiredHeight,0);
				} else {
					idealPos = floorPoint + new Vector3(0,flightDesiredHeight, 0);
				}

				if (enemy != null && flightHeightIsPercentage) idealPos.y = enemy.transform.position.y + 0.24f;
                visibleMeshEntity.transform.position = Vector3.MoveTowards(visibleMeshEntity.transform.position, idealPos, runSpeed * Time.deltaTime);
            }
        }
	}

	void Think () {
		if (dyingSetup && deathBurstFinished < Time.time && !deathBurstDone) {
			if (deathBurst != null) deathBurst.SetActive(true); // activate any death effects
			deathBurstDone = true;
		}

		if (healthManager.health <= 0) {
			// If we haven't gone into dying and we aren't dead, going into dying
			if (!ai_dying && !ai_dead) {
				ai_dying = true; //no going back
				currentState = Const.aiState.Dying; //start to collapse in a heap, melt, explode, etc.
			}
		}

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

		inSight = CheckIfPlayerInSight();
        //if (inSight) backTurned = CheckIfBackIsTurned();
        if (enemy != null) {
            infront = enemyInFront(enemy);
            inProjFOV = enemyInProjFOV(enemy);
            rangeToEnemy = Vector3.Distance(enemy.transform.position, sightPoint.transform.position);
        } else {
            infront = false;
            rangeToEnemy = sightRange;
        }
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

		if (idleTime < Time.time && SFXIdle) {
			if (SFX != null && SFXIdle != null) SFX.PlayOneShot(SFXIdle);
			idleTime = Time.time + Random.Range(idleSFXTimeMin, idleSFXTimeMax);
		}
			
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		CheckIfPlayerInSight();
	}

	void Walk() {
        if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
        if (inSight || enemy != null) {
            currentState = Const.aiState.Run;
            return;
        }

        if (moveType == Const.aiMoveType.None) return;

		// destination still far away and turned to within angle to move, then move
		if (Vector3.Distance(sightPoint.transform.position, currentDestination) > 1.28) {
			//if (WithinAngleToTarget()) rbody.AddForce(sightPoint.transform.forward.x * walkSpeed,(sightPoint.transform.forward.y * walkSpeed)+0.5f,sightPoint.transform.forward.z * walkSpeed);
			if (WithinAngleToTarget()) rbody.velocity = (sightPoint.transform.forward * runSpeed);
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
        if (inSight) {
            huntFinished = Time.time + huntTime;
            if (rangeToEnemy < meleeRange) {
                if (hasMelee && infront && (randomWaitForNextAttack1Finished < Time.time)) {
                    //nav.speed = meleeSpeed;
                    attackFinished = Time.time + timeBetweenAttack1 + timeTillActualAttack1;
                    currentState = Const.aiState.Attack1;
                    return;
                }
            } else {
                if (rangeToEnemy < proj1Range) {
                    if (hasProj1 && infront && inProjFOV && (randomWaitForNextAttack2Finished < Time.time)) {
                        //nav.speed = proj1Speed;
                        shotFired = false;
                        attackFinished = Time.time + timeBetweenAttack2 + timeTillActualAttack2;
                        gracePeriodFinished = Time.time + timeTillActualAttack2;
                        targettingPosition = enemy.transform.position;
                        currentState = Const.aiState.Attack2;
                        return;
                    }
                } else {
                    if (rangeToEnemy < proj2Range) {
                        if (hasProj2 && infront && inProjFOV && (randomWaitForNextAttack3Finished < Time.time)) {
                            //nav.speed = proj2Speed;
							shotFired = false;
							attackFinished = Time.time + timeBetweenAttack3 + timeTillActualAttack3;
							gracePeriodFinished = Time.time + timeTillActualAttack3;
							targettingPosition = enemy.transform.position;
							currentState = Const.aiState.Attack3;
                            return;
                        }
                    }
                }
            }

            currentDestination = enemy.transform.position;
			// enemy still far away and turned to within angle to move, then move
			if ((moveType != Const.aiMoveType.None) && (Vector3.Distance(sightPoint.transform.position, enemy.transform.position) > 1.28)) {
				//if (WithinAngleToTarget()) rbody.AddForce(transform.forward * runSpeed);
				if (WithinAngleToTarget()) rbody.velocity = (sightPoint.transform.forward * runSpeed);
			}

            lastKnownEnemyPos = enemy.transform.position;
            //randSpin = false;
        } else {
            if (huntFinished > Time.time) {
                Hunt();
            } else {
                enemy = null;
                currentState = Const.aiState.Idle;
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
			//if (WithinAngleToTarget()) rbody.AddForce(transform.forward * runSpeed);
			if (WithinAngleToTarget()) rbody.velocity = (sightPoint.transform.forward * runSpeed);
		}


		// using transform.position instead of sightPoint.transform.position for the distance because it deals with the overall enemy's transform rather than view
        //if (!randSpin && Vector3.Distance(transform.position, lastKnownEnemyPos) < nav.stoppingDistance) {
        //    randSpin = true; // only set destination point once so we aren't chasing our tail spinning in circles
        //    if (nav.enabled == true) nav.SetDestination(rrCheckPoint.transform.position);
        //} else {
        //    if (nav.enabled == true) nav.SetDestination(lastKnownEnemyPos);
        //}
    }

	void Attack1() {
		// Used for melee
		if (attack1SoundTime < Time.time) {
			if (SFX != null && SFXAttack1 != null) SFX.PlayOneShot(SFXAttack1);
			attack1SoundTime = Time.time + timeBetweenAttack1;
            for (int i = 0; i < meleeDamageColliders.Length; i++) {
				//Debug.Log("Melee colliders...activate!");
                meleeDamageColliders[i].SetActive(true);
                meleeDamageColliders[i].GetComponent<AIMeleeDamageCollider>().MeleeColliderSetup(index, meleeDamageColliders.Length, impactMelee, gameObject);
            }
			//Debug.Log("Did...did we do it?  Did the melee colliders activate?");
        }

		if (Vector3.Distance(sightPoint.transform.position, currentDestination) > 1.28) {
			if (WithinAngleToTarget()) rbody.AddForce(transform.forward * meleeSpeed);
		}
        currentDestination = enemy.transform.position;

        if (attackFinished < Time.time) {
            for (int i = 0; i < meleeDamageColliders.Length; i++) {
                meleeDamageColliders[i].SetActive(false); // turn off melee colliders
            }

            if (Random.Range(0f,1f) < attack1RandomWaitChance) {
                randomWaitForNextAttack1Finished = Time.time + Random.Range(attack1MinRandomWait, attack1MaxRandomWait);
            } else {
                randomWaitForNextAttack1Finished = Time.time;
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
        tempVec = targPos;
        tempVec.y += 0.24f;  //TODO get actual player camera position.y
		if (useGunPoint2 && gunPoint2 != null) {
			tempVec = tempVec - gunPoint2.transform.position;
		} else {
			tempVec = tempVec - gunPoint.transform.position;
		}
        int layMask = 10;
        layMask = -layMask;

		if (useGunPoint2 && gunPoint2 != null) {
			if (Physics.Raycast(gunPoint2.transform.position, tempVec.normalized, out tempHit, sightRange, layMask)) {
				tempHM = tempHit.transform.gameObject.GetComponent<HealthManager>();
				if (tempHit.transform.gameObject.GetComponent<HealthManager>() != null) {
					useBlood = true;
				}
				return true;
			}
		} else {
			if (Physics.Raycast(gunPoint.transform.position, tempVec.normalized, out tempHit, sightRange, layMask)) {
				tempHM = tempHit.transform.gameObject.GetComponent<HealthManager>();
				if (tempHit.transform.gameObject.GetComponent<HealthManager>() != null) {
					useBlood = true;
				}
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
        }

        return Const.a.GetObjectFromPool(Const.PoolType.SparksSmall);
    }

    void CreateStandardImpactEffects(bool onlyBloodIfHitHasHM) {
        // Determine blood type of hit target and spawn corresponding blood particle effect from the Const.Pool
        if (useBlood) {
            GameObject impact = GetImpactType(tempHM);
            if (impact != null) {
                tempVec = tempHit.normal;
                impact.transform.position = tempHit.point + tempVec;
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
			if (WithinAngleToTarget()) rbody.AddForce(transform.forward * proj1Speed);
		}
        currentDestination = targettingPosition;

        if (gracePeriodFinished < Time.time) {
            if (!shotFired) {
                shotFired = true;
                // Typically used for normal projectile attack
                if (attack2SoundTime < Time.time && SFXAttack2) {
					if (SFX != null && SFXAttack2 != null) SFX.PlayOneShot(SFXAttack2);
                    attack2SoundTime = Time.time + timeBetweenAttack2;
                }

                if (attack2Type == Const.AttackType.Projectile) {
                    if (muzzleBurst != null) muzzleBurst.SetActive(true);
                    if (DidRayHit(targettingPosition, proj1Range,false)) {
                        CreateStandardImpactEffects(false);
                        damageData.other = tempHit.transform.gameObject;
                        if (tempHit.transform.gameObject.tag == "NPC") {
                            damageData.isOtherNPC = true;
                        } else {
                            damageData.isOtherNPC = false;
                        }
                        damageData.hit = tempHit;
                        tempVec = gunPoint.transform.position;
                        tempVec = (enemy.transform.position - tempVec);
                        damageData.attacknormal = tempVec;
                        damageData.damage = 15f;
                        damageData.damage = Const.a.GetDamageTakeAmount(damageData);
                        damageData.owner = gameObject;
                        damageData.attackType = Const.AttackType.Projectile;
                        HealthManager hm = tempHit.transform.gameObject.GetComponent<HealthManager>();
                        if (hm == null) return;
                        hm.TakeDamage(damageData);
                    }
                } else {
					if (attack2Type == Const.AttackType.ProjectileLaunched) {
						if (muzzleBurst != null) muzzleBurst.SetActive(true);
                        tempVec = gunPoint.transform.position;
                        tempVec = (enemy.transform.position - tempVec);
                        damageData.attacknormal = tempVec;
                        damageData.damage = 30f;
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
							Vector3 shove = beachball.transform.forward * attack2projectilespeed;
							beachball.GetComponent<Rigidbody>().velocity = Vector3.zero; // prevent random variation from the last shot's velocity
							beachball.GetComponent<Rigidbody>().AddForce(shove, ForceMode.Impulse);
						}
					}
				}
            }
        }

        if (attackFinished < Time.time) {
            if (Random.Range(0f,1f) < attack2RandomWaitChance) {
                randomWaitForNextAttack2Finished = Time.time + Random.Range(attack2MinRandomWait, attack2MaxRandomWait);
            } else {
                randomWaitForNextAttack2Finished = Time.time;
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
		if (attack3SoundTime < Time.time && SFXAttack3 || explodeOnAttack3) {
			if (SFXAttack3 != null)
				SFX.PlayOneShot(SFXAttack3);
			attack3SoundTime = Time.time + timeBetweenAttack3;
		}

		if (Vector3.Distance(sightPoint.transform.position, currentDestination) > 1.28) {
			if (WithinAngleToTarget()) rbody.AddForce(transform.forward * proj2Speed);
		}
        currentDestination = targettingPosition;

		if (explodeOnAttack3) {
			//ExplosionForce ef = GetComponent<ExplosionForce>();
			DamageData ddNPC = Const.SetNPCDamageData(index, Const.aiState.Attack3,gameObject);
			float take = Const.a.GetDamageTakeAmount(ddNPC);
			ddNPC.other = gameObject;
			ddNPC.damage = take;
			if (explosion != null) explosion.ExplodeInner(explosionObject.transform.position+explosionOffset, attack3Force, attack3Radius, ddNPC);
			//healthManager.health = 0;
			healthManager.TakeDamage(ddNPC);
			return;
		}

        if (gracePeriodFinished < Time.time) {
            if (!shotFired) {
                shotFired = true;
                // Typically used for normal projectile attack
                if (attack3Type == Const.AttackType.Projectile) {
                    if (muzzleBurst2 != null) muzzleBurst2.SetActive(true);
                    if (DidRayHit(targettingPosition, proj1Range,true)) {
                        CreateStandardImpactEffects(false);
                        damageData.other = tempHit.transform.gameObject;
                        if (tempHit.transform.gameObject.tag == "NPC") {
                            damageData.isOtherNPC = true;
                        } else {
                            damageData.isOtherNPC = false;
                        }
                        damageData.hit = tempHit;
                        tempVec = gunPoint2.transform.position;
                        tempVec = (enemy.transform.position - tempVec);
                        damageData.attacknormal = tempVec;
                        damageData.damage = 15f;
                        damageData.damage = Const.a.GetDamageTakeAmount(damageData);
                        damageData.owner = gameObject;
                        damageData.attackType = Const.AttackType.Projectile;
                        HealthManager hm = tempHit.transform.gameObject.GetComponent<HealthManager>();
                        if (hm == null) return;
                        hm.TakeDamage(damageData);
                    }
                } else {
					if (attack3Type == Const.AttackType.ProjectileLaunched) {
                        tempVec = gunPoint2.transform.position;
                        tempVec = (enemy.transform.position - tempVec);
                        damageData.attacknormal = tempVec;
                        damageData.damage = 30f;
                        damageData.damage = Const.a.GetDamageTakeAmount(damageData);
                        damageData.owner = gameObject;
                        damageData.attackType = Const.AttackType.ProjectileLaunched;
						// Create and hurl a beachball-like object.  On the developer commentary they said that the projectiles act
						// like a beachball for collisions with enemies, but act like a baseball for walls/floor to prevent hitting corners
						GameObject beachball = Const.a.GetObjectFromPool(attack3ProjectileLaunchedType);
						if (beachball != null) {
							beachball.GetComponent<ProjectileEffectImpact>().dd = damageData;
							beachball.GetComponent<ProjectileEffectImpact>().host = gameObject;
							beachball.GetComponent<ProjectileEffectImpact>().impactType = Const.a.GetPoolImpactFromPoolProjectileType(attack3ProjectileLaunchedType);
							beachball.transform.position = gunPoint2.transform.position;
							beachball.transform.forward = tempVec.normalized;
							beachball.SetActive(true);
							Vector3 shove = beachball.transform.forward * attack3projectilespeed;
							beachball.GetComponent<Rigidbody>().velocity = Vector3.zero; // prevent random variation from the last shot's velocity
							beachball.GetComponent<Rigidbody>().AddForce(shove, ForceMode.Impulse);
						}
					}
				}
            }
        }

        if (attackFinished < Time.time) {
            if (Random.Range(0f,1f) < attack3RandomWaitChance) {
                randomWaitForNextAttack3Finished = Time.time + Random.Range(attack3MinRandomWait, attack3MaxRandomWait);
            } else {
                randomWaitForNextAttack3Finished = Time.time;
            }
            if (muzzleBurst2 != null) muzzleBurst2.SetActive(false);
            goIntoPain = false; //prevent going into pain after attack
            currentState = Const.aiState.Run;
            return;
        }
	}

	void Pain() {
		//if (enemy == null) {
		//	
		//}
		
		if (timeTillPainFinished < Time.time) {
			currentState = Const.aiState.Run; // go into run after we get hurt
			goIntoPain = false;
			timeTillPainFinished = Time.time + timeBetweenPain;
		}
	}

	void Dying() {
		if (!dyingSetup) {
			dyingSetup = true;
			if (deathBurstTimer > 0) {
				deathBurstFinished = Time.time + deathBurstTimer;
			} else {
				if (!deathBurstDone) {
					if (deathBurst != null) deathBurst.SetActive(true); // activate any death effects
					deathBurstDone = true;
				}
				
			}

			for (int i = 0; i < meleeDamageColliders.Length; i++) {
				//Debug.Log("Melee colliders deactivated");
                meleeDamageColliders[i].SetActive(false);
            }

			if (!healthManager.actAsCorpseOnly && SFX != null && SFXDeathClip != null) SFX.PlayOneShot(SFXDeathClip);

            // Turn off normal NPC collider and enable corpse collider for searching
            //if (boxCollider != null) boxCollider.enabled = false;
            //if (sphereCollider != null) sphereCollider.enabled = false;
            //if (meshCollider != null) meshCollider.enabled = false;
            //if (capsuleCollider != null) capsuleCollider.enabled = false;
			if (moveType == Const.aiMoveType.Fly) {
				if (flyerCollider != null) flyerCollider.enabled = false;
				if (flyerColliderAlternate1 != null) flyerColliderAlternate1.enabled = false;
				if (!healthManager.gibOnDeath) {
					rbody.useGravity = true;
					//rbody.isKinematic = false;
				}
			}
			firstSighting = true;
			timeTillDeadFinished = Time.time + timeTillDead; // wait for death animation to finish before going into Dead()
			if (switchMaterialOnDeath) actualSMR.material = deathMaterial;
		}
			
		if (timeTillDeadFinished < Time.time) {
			ai_dead = true;
			ai_dying = false;
			currentState = Const.aiState.Dead;
		}
	}

	void Dead() {
		ai_dead = true;
		ai_dying = false;
        if (boxCollider != null) boxCollider.enabled = false;
        if (sphereCollider != null) sphereCollider.enabled = false;
        if (meshCollider != null) meshCollider.enabled = false;
		if (capsuleCollider != null) capsuleCollider.enabled = false;
		if (searchColliderGO != null) searchColliderGO.SetActive(true);
		if (moveType == Const.aiMoveType.Fly) {
			rbody.isKinematic = false;
		} else {
			//rbody.isKinematic = true;
			if (searchColliderGO != null) {
				rbody.useGravity = true;
				//rbody.isKinematic = false;
				rbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // prevent corpse from flipping over but let it get moved around by func_walls and grenades and such...if it doesn't get gibbed first
			}
		}
		currentState = Const.aiState.Dead;
		if (healthManager.gibOnDeath) {
			// Special case for gibing on death for the avian mutant to allow it's corpse to rain from the sky, otherwise it would get disabled along with
			// the container that held the visibleMesh.  Needed because we want the corpse to fall from the exact same place as the model and model is in
			// the air within the visibleMeshContainer.  Hence, the special case gib containered flag.  Bit silly ya.
			if (specialCaseGibContainered) {
				rbody.useGravity = false; // prevent the invisible part of the enemy from falling indefinitely
				visibleMeshSubObject.SetActive(false); // turn off the visual model in case of Avian Mutant
			} else {
				visibleMeshEntity.SetActive(false); // normally just turn off the main model, then...
			}
			healthManager.Gib(); // ... turn on the lovely gibs
			if (explosion != null) explosion.ExplodeOuter(explosionObject.transform.position); // blast the gibs away from the center TODO this isn't working on the repair bot??
			//if (explosion != null) explosion.ExplodeInner(explodeObject.transform.position, nearforce, nearradius, null);
		}
	}
	
	public void SendEnemy (GameObject enemCandidate) {
		if (ignoreEnemy) return;
		if (enemy != null) return; //already have enemy TODO: use time to switch to switch to alternate attacker

		GameObject playr1 = Const.a.player1;
		GameObject playr2 = Const.a.player2;
		GameObject playr3 = Const.a.player3;
		GameObject playr4 = Const.a.player4;

		if (playr1 == null) { Debug.Log("WARNING: NPC sight check - no host player 1."); return; }  // No host player
		if (playr1 != null) {playr1 = playr1.GetComponent<PlayerReferenceManager>().playerCapsule;}
		if (playr2 != null) {playr2 = playr2.GetComponent<PlayerReferenceManager>().playerCapsule;}
		if (playr3 != null) {playr3 = playr3.GetComponent<PlayerReferenceManager>().playerCapsule;}
		if (playr4 != null) {playr4 = playr4.GetComponent<PlayerReferenceManager>().playerCapsule;}
		
		if (enemCandidate == playr1) {
		enemy = playr1;
			if (firstSighting) {
				firstSighting = false;
				if (SFX != null && SFXSightSound != null) SFX.PlayOneShot(SFXSightSound);
			}
		} else {
			if (enemCandidate == playr2) {
				enemy = playr1;
				if (firstSighting) {
					firstSighting = false;
					if (SFXSightSound != null) SFX.PlayOneShot(SFXSightSound);
				}
			} else {
				if (enemCandidate == playr3) {
					enemy = playr1;
					if (firstSighting) {
						firstSighting = false;
						if (SFXSightSound != null) SFX.PlayOneShot(SFXSightSound);
					}
				} else {
					if (enemCandidate == playr4) {
						enemy = playr1;
						if (firstSighting) {
							firstSighting = false;
							if (SFXSightSound != null) SFX.PlayOneShot(SFXSightSound);
						}
					}
				}
			}
		}
	}

	void Inspect() {
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
	}
		
	void Interacting() {
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
	}

	bool CheckIfEnemyInSight() {
		Vector3 checkline = enemy.transform.position - sightPoint.transform.position; // Get vector line made from enemy to found player
        int layMask = 10;
        layMask = -layMask;

        RaycastHit hit;
        //if (Physics.Raycast(sightPoint.transform.position + transform.up, checkline.normalized, out hit, sightRange, layMask)) {
        if (Physics.Raycast(sightPoint.transform.position                 , checkline.normalized, out hit, sightRange, layMask)) {
            LOSpossible = true;
			//Debug.DrawRay(sightPoint.transform.position + transform.up,checkline.normalized, Color.green, 0.1f,false);
			//Debug.DrawLine(sightPoint.transform.position,hit.point,Color.green,0.1f,false);
            if (hit.collider.gameObject == enemy)
                return true;
        }
        LOSpossible = false;

		// Testing to see that dist check still works
		float dist = Vector3.Distance(enemy.transform.position,sightPoint.transform.position);  // Get distance between enemy and found player	
		if (dist < distToSeeWhenBehind) {
			return true;
		}
		// works!  what??
		// end test block

        return false;
	}

	bool CheckIfPlayerInSight () {
        if (ignoreEnemy) return false;
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
		LOSpossible = false;

		for (int i=0;i<4;i++) {
			tempent = null;
			// Cycle through all the players to see if we can see anybody.  Defaults to earlier joined players. TODO: Add randomization if multiple players are visible.
			if (playr1 != null && i == 0) tempent = playr1;
			if (playr2 != null && i == 1) tempent = playr2;
			if (playr3 != null && i == 2) tempent = playr3;
			if (playr4 != null && i == 4) tempent = playr4;
			if (tempent == null) continue; // no found player
			// found player

			if (tempent.GetComponent<PlayerMovement>().Notarget) continue; // can't see him, he's on notarget. skip to next available player to check against
			//Debug.Log("Found a player with sight check");
            tempVec = tempent.transform.position;
			Vector3 checkline = tempVec - sightPoint.transform.position; // Get vector line made from enemy to found player
            int layMask = 10;
            layMask = -layMask;

			// Check for line of sight
			RaycastHit hit;
            if (Physics.Raycast(sightPoint.transform.position, checkline.normalized, out hit, sightRange,layMask)) {
				//Debug.Log("Sight raycast successful");
				//Debug.DrawRay(sightPoint.transform.position,checkline.normalized, Color.green, 0.1f,false);
				//Debug.DrawLine(sightPoint.transform.position,hit.point,Color.green,0.1f,false);
				//Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.gameObject == tempent) {
					//Debug.Log("Hit collider was same as playr#");
					LOSpossible = true;  // Clear path from enemy to found player
				}
			}

			float dist = Vector3.Distance(tempent.transform.position,sightPoint.transform.position);  // Get distance between enemy and found player
			float angle = Vector3.Angle(checkline,sightPoint.transform.forward);
			
			// If clear path to found player, and either within view angle or right behind the enemy
			if (LOSpossible) {
				//Debug.Log("LOS was possible");
				if (angle < (fieldOfViewAngle * 0.5f)) {
					enemy = tempent;
					if (firstSighting) {
						firstSighting = false;
						if (SFXSightSound != null) SFX.PlayOneShot(SFXSightSound);
					}
					return true;
				} else {
					if (dist < distToSeeWhenBehind) {
						enemy = tempent;
						if (firstSighting) {
							firstSighting = false;
							if (SFXSightSound != null) SFX.PlayOneShot(SFXSightSound);
						}
						return true;
					}
				}
			} else {
				if (dist < distToSeeWhenBehind) {
					enemy = tempent;
					if (firstSighting) {
						firstSighting = false;
						if (SFXSightSound != null) SFX.PlayOneShot(SFXSightSound);
					}
					return true;
				}
			}
		}
		return false;
	}
	
    bool enemyInFront (GameObject target) {
        Vector3 vec = Vector3.Normalize(target.transform.position - sightPoint.transform.position);
        float dot = Vector3.Dot(vec,transform.forward);
        if (dot > 0.300) return true; // enemy is within 27 degrees of forward facing vector
        return false;
    }

    bool enemyInProjFOV(GameObject target) {
        Vector3 vec = Vector3.Normalize(target.transform.position - sightPoint.transform.position);
        float dot = Vector3.Dot(vec, transform.forward);
        if (dot > 0.800) return true; // enemy is within 27 degrees of forward facing vector
        return false;
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
			// Cycle through all the players to see if we can see anybody.  Defaults to earlier joined players. TODO: Add randomization if multiple players are visible.
			if (playr1 != null && i == 0) tempent = playr1;
			if (playr2 != null && i == 1) tempent = playr2;
			if (playr3 != null && i == 2) tempent = playr3;
			if (playr4 != null && i == 4) tempent = playr4;
			if (ud.owner == tempent) { enemy = tempent; } else { if (tempent != enemy) enemy = tempent;}
		}
	}
}
