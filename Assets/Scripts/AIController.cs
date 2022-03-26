using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DigitalRuby.LightningBolt;

public class AIController : MonoBehaviour {
	// External manually assigned references, required
	public int index = 0; // NPC reference index for looking up constants in tables in Const.cs // save
	public HealthManager healthManager;

	// External manually assigned references, but ok if not assigned
    /*[DTValidator.Optional] */public GameObject searchColliderGO;
	/*[DTValidator.Optional] */public AudioClip SFXIdle;
	/*[DTValidator.Optional] */public AudioClip SFXSightSound;
	/*[DTValidator.Optional] */public AudioClip SFXAttack1;
	/*[DTValidator.Optional] */public AudioClip SFXAttack2;
	/*[DTValidator.Optional] */public AudioClip SFXAttack3;
	/*[DTValidator.Optional] */public AudioClip SFXDeathClip;
	/*[DTValidator.Optional] */public LightningBoltScript laserLightning;
	/*[DTValidator.Optional] */public Transform[] walkWaypoints; // point(s) for NPC to walk to when roaming or patrolling
	/*[DTValidator.Optional] */public GameObject[] meleeDamageColliders; // Used by avian mutant lunge
    /*[DTValidator.Optional] */public GameObject muzzleBurst;
    /*[DTValidator.Optional] */public GameObject muzzleBurst2;
    /*[DTValidator.Optional] */public GameObject visibleMeshEntity;
    /*[DTValidator.Optional] */public GameObject gunPoint;
    /*[DTValidator.Optional] */public GameObject gunPoint2;
	/*[DTValidator.Optional] */public Material deathMaterial;
	/*[DTValidator.Optional] */public SkinnedMeshRenderer actualSMR;
	/*[DTValidator.Optional] */public GameObject deathBurst;
	/*[DTValidator.Optional] */public GameObject sightPoint;
	/*[DTValidator.Optional] */public Animator hopAnimator;
	/*[DTValidator.Optional] */public GameObject sleepingCables;
	/*[DTValidator.Optional] */public RectTransform npcAutomapOverlay;
	/*[DTValidator.Optional] */public Image npcAutomapOverlayImage;
	public Const.aiState currentState; // save (referenced by int index 0 thru 10)
	public Const.npcType npcType;
    public Const.AttackType attack1Type = Const.AttackType.Melee;
    public Const.AttackType attack2Type = Const.AttackType.Projectile;
    public Const.AttackType attack3Type = Const.AttackType.Projectile;
	public Const.PoolType attack1ProjectileLaunchedType = Const.PoolType.None;
	public Const.PoolType attack2ProjectileLaunchedType = Const.PoolType.None;
	public Const.PoolType attack3ProjectileLaunchedType = Const.PoolType.None;
	public bool walkPathOnStart = false;
    public bool dontLoopWaypoints = false;
	public bool visitWaypointsRandomly = false;
	public float deathBurstTimer = 0.1f;
	public bool asleep = false; // check if enemy starts out asleep, e.g. sleeping sec-2 bots on level 8 in the maintenance chargers // save
	public bool wandering; // save
	public bool actAsTurret = false;

	// Internal, keeping exposed in inspector for troubleshooting.
	/*[DTValidator.Optional] */public GameObject enemy; // save (referenced by int index enemIDRead)

	// Internal
	[HideInInspector] public string targetID;
	[HideInInspector] public bool hasTargetIDAttached = false;
    [HideInInspector] public float gracePeriodFinished; // save
    [HideInInspector] public float meleeDamageFinished; // save
	[HideInInspector] public bool inSight = false; // save
    [HideInInspector] public bool infront; // save
    [HideInInspector] public bool inProjFOV; // save
    [HideInInspector] public bool LOSpossible; // save
    [HideInInspector] public bool goIntoPain = false; // save
	[HideInInspector] public float rangeToEnemy = 999f; // save
	[HideInInspector] public GameObject attacker;
	[HideInInspector] public bool firstSighting; // save
	[HideInInspector] public bool dyingSetup; // save
	[HideInInspector] public bool ai_dying; // save
	[HideInInspector] public bool ai_dead; // save
	[HideInInspector] public int currentWaypoint; // save
	[HideInInspector] public Vector3 currentDestination; // save
	private float idleTime;
	private float attack1SoundTime;
	private float attack2SoundTime;
	private float attack3SoundTime;
	[HideInInspector] public float timeTillEnemyChangeFinished; // save
	[HideInInspector] public float timeTillDeadFinished; // save
	[HideInInspector] public float timeTillPainFinished; // save
	[HideInInspector] public AudioSource SFX;
	[HideInInspector] public Rigidbody rbody;
	[HideInInspector] public BoxCollider boxCollider;
	[HideInInspector] public CapsuleCollider capsuleCollider;
	[HideInInspector] public SphereCollider sphereCollider;
	[HideInInspector] public MeshCollider meshCollider;
	private float tick;
	[HideInInspector] public float tickFinished; // save
	[HideInInspector] public float raycastingTickFinished; // save
	[HideInInspector] public float huntFinished; // save
	[HideInInspector] public bool hadEnemy; // save
	[HideInInspector] public Vector3 lastKnownEnemyPos; // save
	[HideInInspector] public Vector3 tempVec; // save
	private Vector3 tempVec2;
	[HideInInspector] public bool shotFired = false; // save
    private DamageData damageData;
    private RaycastHit tempHit;
    private bool useBlood;
    private HealthManager tempHM;
	[HideInInspector] public float randomWaitForNextAttack1Finished; // save
	[HideInInspector] public float randomWaitForNextAttack2Finished; // save
	[HideInInspector] public float randomWaitForNextAttack3Finished; // save
	[HideInInspector] public Vector3 idealTransformForward; // save
	[HideInInspector] public Vector3 idealPos; // used by flyers to establish correct height // save
	[HideInInspector] public float attackFinished; // save
	[HideInInspector] public float attack2Finished; // save
	[HideInInspector] public float attack3Finished; // save
	[HideInInspector] public Vector3 targettingPosition; // used to give the player a chance to dodge attacks by moving after start of an attack, enemy attacks along same starting line // save
	[HideInInspector] public float deathBurstFinished; // save
	[HideInInspector] public bool deathBurstDone; // save
	private NavMeshPath searchPath;
	[HideInInspector] public float tranquilizeFinished; // save
	[HideInInspector] public bool hopDone; // save
	[HideInInspector] public float wanderFinished; // save
	private float dotResult = -1f;
	private Vector3 infrontVec;
	private PlayerMovement pm;
	[HideInInspector] public bool startInitialized = false;
	private HealthManager enemyHM;
	private float stopDistance = 1.28f;
	private Vector3 faceVec;
	private Quaternion lookRot;

	public void Tranquilize() { tranquilizeFinished = PauseScript.a.relativeTime + Const.a.timeForTranquilizationForNPC[index]; }

	// Initialization and find components
	public void Start () {
        rbody = GetComponent<Rigidbody>();
		rbody.isKinematic = false;
		if (index < 29 && index >= 0) {
			if (Const.a.moveTypeForNPC.Length > 1) {
				if (Const.a.moveTypeForNPC[index] == Const.aiMoveType.Fly ||
				    Const.a.moveTypeForNPC[index] == Const.aiMoveType.Cyber) rbody.useGravity = false;
				else rbody.useGravity = true;
			} else { rbody.useGravity = true; Debug.Log("Const.a.moveTypeForNPC had no length!"); }
		} else { rbody.useGravity = true; Debug.Log("Index was out of range with value of " + index.ToString() + " on " + gameObject.name);}
		healthManager = GetComponent<HealthManager>();
		boxCollider = GetComponent<BoxCollider>();
		sphereCollider = GetComponent<SphereCollider>();
		meshCollider = GetComponent<MeshCollider>();
		capsuleCollider = GetComponent<CapsuleCollider>();
        if (searchColliderGO != null) searchColliderGO.SetActive(false);
		if (sightPoint == null) sightPoint = gameObject;
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
		idleTime = PauseScript.a.relativeTime + Random.Range(Const.a.timeIdleSFXMinForNPC[index],Const.a.timeIdleSFXMaxForNPC[index]);
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
		tickFinished = PauseScript.a.relativeTime + Const.a.aiTickTime + Random.value;
		raycastingTickFinished = PauseScript.a.relativeTime + Const.a.aiTickTime + Random.value;
		attackFinished = PauseScript.a.relativeTime + 1f;
		idealTransformForward = sightPoint.transform.forward;
		deathBurstDone = false;
		searchPath = new NavMeshPath();
		if (Const.a.moveTypeForNPC[index] != Const.aiMoveType.Cyber) {
			targetID = Const.GetTargetID(index);
		} else {
			targetID = Const.GetCyberTargetID(index);
		}
		startInitialized = true;

		if (npcAutomapOverlay == null && !healthManager.actAsCorpseOnly) {
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
				if (npcAutomapOverlayImage != null) npcAutomapOverlayImage.enabled = true;
			} else {
				Debug.Log("ERROR: Couldn't find automap icon for " + gameObject.name + " of type " + pt.ToString());
			}
		}
	}

	void AI_Face(Vector3 goalLocation) {
		if (asleep) return;
		faceVec = (goalLocation - transform.position).normalized;
		faceVec.y = 0f;
		if (faceVec.sqrMagnitude > 0 && faceVec != Vector3.up) {
			lookRot = Quaternion.LookRotation(faceVec,Vector3.up);
			transform.rotation = Quaternion.Slerp(transform.rotation,lookRot,Const.a.aiTickTime * Const.a.yawSpeedForNPC[index] * Time.deltaTime); // rotate as fast as we can towards goal position
		}
	}

	void OnEnable() {
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
				if (npcAutomapOverlayImage != null) npcAutomapOverlayImage.enabled = true;
			}
		}
	}

	void LateUpdate() {
		Const.a.numberOfRaycastsThisFrame = 0;
	}

	void Update() {
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive() || !startInitialized) return;

		if (raycastingTickFinished < PauseScript.a.relativeTime) {
			raycastingTickFinished = PauseScript.a.relativeTime + Const.a.raycastTick;
			inSight = CheckIfPlayerInSight();
			if (enemy != null && healthManager.health > 0) {
				// Check if enemy health drops to 0
				if (enemyHM == null) enemyHM = enemy.GetComponent<HealthManager>();
				if (enemyHM != null) {
					if (Const.a.moveTypeForNPC[index] == Const.aiMoveType.Cyber) {
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
				if (enemy != null) {
					enemyInFrontChecks(enemy);
					// rangeToEnemy = Vector3.Distance(enemy.transform.position, sightPoint.transform.position);
					rangeToEnemy = (enemy.transform.position - sightPoint.transform.position).sqrMagnitude;
				}
			} else {
				infront = false;
				inProjFOV = false;
				rangeToEnemy = Const.a.sightRangeForNPC[index] * Const.a.sightRangeForNPC[index];
			}
		}
	}

	void FixedUpdate() {
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive() || !startInitialized) return; // Don't do any checks or anything else...we're paused!

		if (!rbody.useGravity && Const.a.moveTypeForNPC[index] != Const.aiMoveType.Cyber && Const.a.moveTypeForNPC[index] != Const.aiMoveType.Fly) rbody.useGravity = true; //Debug.Log(gameObject.name + " has rbody.useGravity set to false!");

        // Only think every tick seconds to save on CPU and prevent race conditions
        if (tickFinished < PauseScript.a.relativeTime) {
			tickFinished = PauseScript.a.relativeTime + Const.a.aiTickTime;
			Think();
			if (npcAutomapOverlay != null && Const.a.moveTypeForNPC[index] != Const.aiMoveType.Cyber && healthManager.health > 0 && Inventory.a.hasHardware[1] && Inventory.a.hardwareVersion[1] > 1) {
				// 34.16488, -45.08, 0.4855735
				// x = ((0.6384575295) * 1008f) + 8;
				// x = 651
				tempVec2.y = (((((transform.position.z - Const.a.mapWorldMaxE)/(Const.a.mapWorldMaxW - Const.a.mapWorldMaxE)) * 1008f) + Const.a.mapTileMinX));
				tempVec2.x = (((((transform.position.x - Const.a.mapWorldMaxS)/(Const.a.mapWorldMaxN - Const.a.mapWorldMaxS)) * 1008f) + Const.a.mapTileMinY));
				tempVec2.z = -0.3f;
				npcAutomapOverlay.localPosition = tempVec2;
			} else {
				if (npcAutomapOverlayImage != null) {
					if (npcAutomapOverlayImage.enabled == true) npcAutomapOverlayImage.enabled = false;
				}
			}
		}

        // Rotation and Special movement that must be done every FixedUpdate
        if (currentState != Const.aiState.Dead) {
            if (currentState != Const.aiState.Idle) {
                idealTransformForward = currentDestination - sightPoint.transform.position;
                if (Const.a.moveTypeForNPC[index] != Const.aiMoveType.Cyber) idealTransformForward.y = 0;
				idealTransformForward = Vector3.Normalize(idealTransformForward);
				if (idealTransformForward.sqrMagnitude > Mathf.Epsilon) {
					AI_Face(currentDestination);
				}
            }
        }
	}

	void Think() {
		if (dyingSetup && deathBurstFinished < PauseScript.a.relativeTime && !deathBurstDone) {
			if (deathBurst != null) deathBurst.SetActive(true); // activate any death effects
			deathBurstDone = true;
		}

		if (Const.a.moveTypeForNPC[index] != Const.aiMoveType.Cyber ||  npcType != Const.npcType.Cyber) {
			if (healthManager.health <= 0) {
				// If we haven't gone into dying and we aren't dead, going into dying
				if (!ai_dying && !ai_dead) {
					ai_dying = true; //no going back
					currentState = Const.aiState.Dying; //start to collapse in a heap, melt, explode, etc.
				}
			}
		} else {
			if (healthManager.cyberHealth <= 0 && npcType == Const.npcType.Cyber) {
				// If we haven't gone into dying and we aren't dead, going into dying
				if (!ai_dying && !ai_dead) {
					ai_dying = true; //no going back
					currentState = Const.aiState.Dying; //start to collapse in a heap, melt, explode, etc.
					Dying();
				}
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
			default: 							Idle(); 		break;
		}

		if (currentState == Const.aiState.Dead || currentState == Const.aiState.Dying) return; // Don't do any checks, we're dead.
		if (asleep) return; // Don't check for an enemy, we are sleeping! shh!!

		if (Const.a.moveTypeForNPC[index] == Const.aiMoveType.Fly && tranquilizeFinished < PauseScript.a.relativeTime) {
			float distUp = 0;
			float distDn = 0;
			Vector3 floorPoint = new Vector3();
			floorPoint = Const.a.vectorZero;
			if (Const.a.numberOfRaycastsThisFrame > Const.a.maxRaycastsPerFrame) idealPos = transform.position;
			else {
				if (Physics.Raycast(sightPoint.transform.position, sightPoint.transform.up * -1, out tempHit, Const.a.sightRangeForNPC[index],Const.a.layerMaskNPCSight)) {
					Const.a.numberOfRaycastsThisFrame++;
					distDn = Vector3.Distance(sightPoint.transform.position, tempHit.point);
					floorPoint = tempHit.point;
				}

				if (Physics.Raycast(sightPoint.transform.position, sightPoint.transform.up, out tempHit, Const.a.sightRangeForNPC[index],Const.a.layerMaskNPCSight)) {
					Const.a.numberOfRaycastsThisFrame++;
					distUp = Vector3.Distance(sightPoint.transform.position, tempHit.point);
				}
				float distT = (distUp + distDn);
				if (Const.a.flightHeightIsPercentageForNPC[index]) {
					idealPos = floorPoint + new Vector3(0,distT * Const.a.flightHeightForNPC[index],0);
				} else {
					idealPos = floorPoint + new Vector3(0,Const.a.flightHeightForNPC[index], 0);
				}
			}

			if (enemy != null) idealPos.y = enemy.transform.position.y + 0.24f;
			if (Const.a.runSpeedForNPC[index] > 0) transform.position = Vector3.MoveTowards(transform.position, idealPos, Const.a.runSpeedForNPC[index] * Time.deltaTime);
		}
	}

	public bool CheckPain() {
		if (asleep) return false;

		if (goIntoPain && timeTillPainFinished < PauseScript.a.relativeTime) {
			if (Const.a.moveTypeForNPC[index] != Const.aiMoveType.Cyber) currentState = Const.aiState.Pain;
			if (attacker != null) {
				if (timeTillEnemyChangeFinished < PauseScript.a.relativeTime) {
					timeTillEnemyChangeFinished = PauseScript.a.relativeTime + Const.a.timeToChangeEnemyForNPC[index];
					enemy = attacker; // Switch to whoever just attacked us
					if (enemy != null) {
						enemyHM = enemy.GetComponent<HealthManager>();
						lastKnownEnemyPos = enemy.transform.position;
						currentDestination = enemy.transform.position;
					}
				}
			}
			goIntoPain = false;
			timeTillPainFinished = PauseScript.a.relativeTime + Const.a.timeToPainForNPC[index];
			return true;
		}
		return false;
	}

	void Idle() {
		if (enemy != null) { currentState = Const.aiState.Run; return; }

		if (idleTime < PauseScript.a.relativeTime && SFXIdle != null) {
			float rand = UnityEngine.Random.Range(0,1f); // for calculating 50% chance of idle
			if (rand < 0.5f) {
				if (SFX != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXIdle);
			}
			idleTime = PauseScript.a.relativeTime + Random.Range(Const.a.timeIdleSFXMinForNPC[index],Const.a.timeIdleSFXMaxForNPC[index]);
		}
		if (asleep) rbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		CheckPain(); // Go into pain if we just got hurt, data is sent by the HealthManager
	}

	void Walk() {
        if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		if (asleep) return;
        if (inSight || enemy != null) { currentState = Const.aiState.Run; return; }
        if (Const.a.moveTypeForNPC[index] == Const.aiMoveType.None) return;

		if (wandering) {
			if (wanderFinished < PauseScript.a.relativeTime || (Vector3.Distance(transform.position,currentDestination) < 0.64f)) {
				wanderFinished = PauseScript.a.relativeTime + UnityEngine.Random.Range(3f,8f);
				currentDestination = new Vector3(UnityEngine.Random.Range(-79f,79f),0,UnityEngine.Random.Range(-79f,79f));
			}
		}

		// Destination still far away and turned to within angle to move, then move
		if (Vector3.Distance(sightPoint.transform.position, currentDestination) > stopDistance && tranquilizeFinished < PauseScript.a.relativeTime) {
			if (WithinAngleToTarget()) {
				if (Const.a.hopsOnMoveForNPC[index]) {
					// Move it move it.
					float playbackTime = 1f;
					if (hopAnimator != null) {
						AnimatorStateInfo asi = hopAnimator.GetCurrentAnimatorStateInfo(0);
						playbackTime = asi.normalizedTime;
					}

					if (playbackTime > 0.1395f) {
						if (!hopDone) {
							hopDone = true;
							if (!actAsTurret) rbody.AddForce(sightPoint.transform.forward * 500f);
						}
					} else {
						hopDone = false;
					}
				} else {
					tempVec = (sightPoint.transform.forward * Const.a.walkSpeedForNPC[index]);
					tempVec.y = rbody.velocity.y; // Carry across gravity.
					rbody.velocity = tempVec;
				}
			}
		} else {
            if (visitWaypointsRandomly) {
                currentWaypoint = Random.Range(0, walkWaypoints.Length); // Max is exclusive for the integer overload; no need to do (walkWaypoints.Length - 1).
            } else {
                currentWaypoint++;
				if ((currentWaypoint >= walkWaypoints.Length) || (walkWaypoints[currentWaypoint] == null)) {
                    if (dontLoopWaypoints) {
                        currentState = Const.aiState.Idle; // Reached end of waypoints, just stop.
                        return;
                    } else {
                        currentWaypoint = 0; // Wrap around.
                        if (walkWaypoints[currentWaypoint] == null) {
                            currentState = Const.aiState.Idle;
                            return; // Stop walking, out of waypoints.
                        }
                    }
                }
			} 
			if (currentWaypoint < walkWaypoints.Length && currentWaypoint >= 0) {
				if (walkWaypoints[currentWaypoint] != null) currentDestination = walkWaypoints[currentWaypoint].transform.position;
			}
        }
	}

	void Run() {
		if (CheckPain()) return; // Go into pain if we just got hurt, data is sent by the HealthManager
		if (asleep) return;
		if (enemy == null) { currentState = Const.aiState.Idle; return; }

        if (inSight) {
			if (enemy != null) {
				targettingPosition = enemy.transform.position;
				currentDestination = enemy.transform.position;
				if (Const.a.moveTypeForNPC[index] == Const.aiMoveType.Cyber) {
					pm = enemy.GetComponent<PlayerMovement>();
					if (pm != null) targettingPosition = pm.cameraObject.transform.position;
				}
			}
            huntFinished = PauseScript.a.relativeTime + Const.a.huntTimeForNPC[index];
			if (Const.a.difficultyCombat == 1) huntFinished = PauseScript.a.relativeTime + (Const.a.huntTimeForNPC[index] * 0.75f); // more forgetfull on 1
			if (Const.a.difficultyCombat >= 3) huntFinished = PauseScript.a.relativeTime + (Const.a.huntTimeForNPC[index] * 2.00f); // better memory on hardest Combat difficulty
            if (rangeToEnemy < (Const.a.rangeForNPC[index] * Const.a.rangeForNPC[index])) {
                if ((attack1Type != Const.AttackType.None) && infront && (randomWaitForNextAttack1Finished < PauseScript.a.relativeTime) && tranquilizeFinished < PauseScript.a.relativeTime) {
                    attackFinished = PauseScript.a.relativeTime + Const.a.timeBetweenAttack1ForNPC[index] + Const.a.timeToActualAttack1ForNPC[index];
					gracePeriodFinished = PauseScript.a.relativeTime + Const.a.timeToActualAttack1ForNPC[index];
                    currentState = Const.aiState.Attack1;
					if (Const.a.preactivateMeleeCollidersForNPC[index]) {
						for (int i = 0; i < meleeDamageColliders.Length; i++) {
							if (meleeDamageColliders[i] != null) meleeDamageColliders[i].SetActive(true);
							if (meleeDamageColliders[i] != null) {
								AIMeleeDamageCollider mcs = meleeDamageColliders[i].GetComponent<AIMeleeDamageCollider>();
								if (mcs != null) mcs.MeleeColliderSetup(index, meleeDamageColliders.Length, 10f, gameObject);
							}
						}
					}
                    return;
                }
            }
			if (rangeToEnemy < (Const.a.rangeForNPC2[index] * Const.a.rangeForNPC2[index])) {
				if ((attack2Type != Const.AttackType.None) && infront && inProjFOV && (randomWaitForNextAttack2Finished < PauseScript.a.relativeTime) && tranquilizeFinished < PauseScript.a.relativeTime) {
					shotFired = false;
					attackFinished = PauseScript.a.relativeTime + Const.a.timeBetweenAttack2ForNPC[index] + Const.a.timeToActualAttack2ForNPC[index];
					gracePeriodFinished = PauseScript.a.relativeTime + Const.a.timeToActualAttack2ForNPC[index];
					currentState = Const.aiState.Attack2;
					return;
				}
			}
			if (rangeToEnemy < (Const.a.rangeForNPC3[index] * Const.a.rangeForNPC3[index])) {
				if ((attack3Type != Const.AttackType.None) && infront && inProjFOV && (randomWaitForNextAttack3Finished < PauseScript.a.relativeTime) && tranquilizeFinished < PauseScript.a.relativeTime) {
					shotFired = false;
					attackFinished = PauseScript.a.relativeTime + Const.a.timeBetweenAttack3ForNPC[index] + Const.a.timeToActualAttack3ForNPC[index];
					gracePeriodFinished = PauseScript.a.relativeTime + Const.a.timeToActualAttack3ForNPC[index];
					currentState = Const.aiState.Attack3;
					return;
				}
			}

			// Enemy still far away and turned to within angle to move, then move
			if ((Const.a.moveTypeForNPC[index] != Const.aiMoveType.None) && (rangeToEnemy > (stopDistance * stopDistance)) && tranquilizeFinished < PauseScript.a.relativeTime) {
				if (WithinAngleToTarget()) {
					if (Const.a.hopsOnMoveForNPC[index]) {
						// Move it move it.
						AnimatorStateInfo asi = hopAnimator.GetCurrentAnimatorStateInfo(0);
						float playbackTime = asi.normalizedTime;
						if (playbackTime > 0.1395f) {
							if (!hopDone) {
								hopDone = true;
								if (!actAsTurret) rbody.AddForce(sightPoint.transform.forward * 500f);
							}
						} else {
							hopDone = false;
						}
					} else {
						tempVec = (sightPoint.transform.forward * Const.a.runSpeedForNPC[index]);
						if (rbody.useGravity) tempVec.y = rbody.velocity.y; // Carry across gravity.
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
				wandering = true; // Magically look like we are still searching maybe?  Sometimes!
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

		// The destination is still far enough away and within angle to move, then move.
		// if ((Const.a.moveTypeForNPC[index] != Const.aiMoveType.None) && (Vector3.Distance(sightPoint.transform.position, currentDestination) > stopDistance)) {
		if ((Const.a.moveTypeForNPC[index] != Const.aiMoveType.None) && ((sightPoint.transform.position - currentDestination).sqrMagnitude > (stopDistance * stopDistance))) {
			if (WithinAngleToTarget() && !actAsTurret && Const.a.runSpeedForNPC[index] > 0) rbody.velocity = (sightPoint.transform.forward * Const.a.runSpeedForNPC[index]);
		}
    }

	// Commonized function to remove previous boilerplate code from all 3 attack functions below.
	// Applies movement towards the enemy while attacking, assumes that we were already facing enemy within the attack angle.
	void ApplyAttackMovement(float speedToApply) {
		if (speedToApply <= 0) return;

		if (Vector3.Distance(sightPoint.transform.position, currentDestination) > stopDistance && tranquilizeFinished < PauseScript.a.relativeTime) {
			if (WithinAngleToTarget() && !actAsTurret) rbody.AddForce(transform.forward * speedToApply);
		}
        currentDestination = enemy.transform.position; // Attack3 used targettingPosition but it is so rare I decided to use the known working method from Attack1 and Attack2.
	}

	// attackNum corresponds to the attack used so correct lookup tables can be used.
	// attackNum of 1 = Attack1, 2 = Attack2, 3 = Attack3
	void Transition_AttackToRun(int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 1;
		switch (attackNum) {
			case 1: // Attack1
				if (Random.Range(0f,1f) < Const.a.timeAttack1WaitChanceForNPC[index]) {
					randomWaitForNextAttack1Finished = PauseScript.a.relativeTime + Random.Range(Const.a.timeAttack1WaitMinForNPC[index],Const.a.timeAttack1WaitMaxForNPC[index]);
				} else {
					randomWaitForNextAttack1Finished = PauseScript.a.relativeTime;
				}
				break;
			case 2: // Attack2
				if (Random.Range(0f,1f) < Const.a.timeAttack2WaitChanceForNPC[index]) {
					randomWaitForNextAttack2Finished = PauseScript.a.relativeTime + Random.Range(Const.a.timeAttack2WaitMinForNPC[index],Const.a.timeAttack2WaitMaxForNPC[index]);
				} else {
					randomWaitForNextAttack2Finished = PauseScript.a.relativeTime;
				}
				break;
			case 3: // Attack3
				if (Random.Range(0f,1f) < Const.a.timeAttack3WaitChanceForNPC[index]) {
					randomWaitForNextAttack3Finished = PauseScript.a.relativeTime + Random.Range(Const.a.timeAttack3WaitMinForNPC[index],Const.a.timeAttack3WaitMaxForNPC[index]);
				} else {
					randomWaitForNextAttack3Finished = PauseScript.a.relativeTime;
				}
				break;
			// Any other values are Unknown, do nothing with the timers.
		}

		goIntoPain = false; // Prevent going into pain immediately after an attack.
		currentState = Const.aiState.Run;
		return; // Done with attack.
	}

    bool WithinAngleToTarget () {
		if (idealTransformForward.sqrMagnitude > Mathf.Epsilon) {
			if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(idealTransformForward)) < Const.a.fovStartMovementForNPC[index]) return true;
		}
        return false;
    }

	Vector3 GetAttackStartPoint(int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 1;
		Vector3 startPos = sightPoint.transform.position;
		switch (attackNum) {
			case 2:
				if (gunPoint != null) startPos = gunPoint.transform.position;
				else if (gunPoint2 != null) startPos = gunPoint2.transform.position;
				break;
			case 3:
				if (gunPoint2 != null) startPos = gunPoint2.transform.position;
				else if (gunPoint != null) startPos = gunPoint.transform.position;
				break;
		}
		return startPos;
	}

	// Returns unit vector pointing from starting point of the attack towards the enemy.
	Vector3 GetDirectionRayToEnemy(Vector3 targPos, int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 1;
		switch (attackNum) {
			case 1:
				return sightPoint.transform.forward;
			case 2:
				return (targPos - GetAttackStartPoint(attackNum)).normalized;
			case 3:
				return (targPos - GetAttackStartPoint(attackNum)).normalized;
		}
		return Const.a.vectorZero;
	}

	float GetRangeForAttack(int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 1;
		float range = Const.a.rangeForNPC[index];
		switch (attackNum) {
			case 1:
				range = Const.a.rangeForNPC[index];
				break;
			case 2:
				range = Const.a.rangeForNPC2[index];
				break;
			case 3:
				range = Const.a.rangeForNPC3[index];
				break;
		}
		return range;
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
                impact.transform.position = tempHit.point + (tempHit.normal*0.08f);
                impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, tempHit.normal);
                impact.SetActive(true);
            }
        } else {
            // Allow for skipping adding sparks after special override impact effects per attack functions below
            if (!onlyBloodIfHitHasHM) {
                GameObject impact = Const.a.GetObjectFromPool(Const.PoolType.SparksSmall); //Didn't hit an object with a HealthManager script, use sparks
                if (impact != null) {
                    impact.transform.position = tempHit.point + tempHit.normal;
                    impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, tempHit.normal);
                    impact.SetActive(true);
                }
            }
        }
    }

	// Activates a GameObject that has automatically playing particle effects, lights, etc.
	// The muzzle bursts are all set up to deactivate on their own; no need to check them later.
	// attackNum corresponds to the attack used so correct lookup tables can be used.
	// attackNum of 1 = Attack1, 2 = Attack2, 3 = Attack3
	void MuzzleBurst(int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 1;
		switch (attackNum) {
			case 2:
				if (muzzleBurst != null) muzzleBurst.SetActive(true);
				break;
			case 3:
				if (muzzleBurst2 != null) muzzleBurst2.SetActive(true);
				break;
		}
	}

	// Does the raycast and sets tempHit for the hit data and tempHM for the hit object's HealthManager.
	// Returns true if it actually hit something.
    bool DidRayHit(int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 1;
		tempVec = GetDirectionRayToEnemy(targettingPosition,attackNum);
		if (Physics.Raycast(GetAttackStartPoint(attackNum), tempVec, out tempHit, GetRangeForAttack(attackNum),Const.a.layerMaskNPCAttack)) {
			Const.a.numberOfRaycastsThisFrame++;
			tempHM = tempHit.transform.gameObject.GetComponent<HealthManager>();
			return true; // True indicates we hit something, not that we hit something that can be hurt.  We need to know to apply spark impact effects still on walls and such.
		}
        return false;
    }

	// Used for attack type of Const.AttackType.Projectile.
	// Does a raycast and then applies attack instantly.
	// Also turns on laser effect if used.
	// attackNum corresponds to the attack used so correct lookup tables can be used.
	// attackNum of 1 = Attack1, 2 = Attack2, 3 = Attack3
	// Attack1 is typically Melee, Attack2 is typically a gun from gunPoint, Attack2 could be a gun or grenade from gunPoint2 (2 as in 2nd gun attack, NOT Attack2).
	void ProjectileRaycast(int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 1;
		MuzzleBurst(attackNum);
		if (DidRayHit(attackNum)) {
			if ((attackNum == 1 && Const.a.hasLaserOnAttack1ForNPC[index]) || (attackNum == 2 && Const.a.hasLaserOnAttack2ForNPC[index]) || (attackNum == 3 && Const.a.hasLaserOnAttack3ForNPC[index])) {
				GameObject dynamicObjectsContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
				if (dynamicObjectsContainer == null) return; // Didn't find current level.
				GameObject lasertracer = Instantiate(Const.a.useableItems[101],transform.position,Const.a.quaternionIdentity) as GameObject;
				if (lasertracer != null) {
					lasertracer.transform.SetParent(dynamicObjectsContainer.transform,true);
					lasertracer.GetComponent<LaserDrawing>().startPoint = sightPoint.transform.position;
					lasertracer.GetComponent<LaserDrawing>().endPoint = tempHit.point;
					lasertracer.SetActive(true);
				}
			}

			if (tempHM != null) {
				useBlood = true;
				// SetNPCDamageData sets :
				//   owner
				//   damage
				//   penetration
				//   offense
				damageData = Const.SetNPCDamageData(index,attackNum,gameObject);
				damageData.other = tempHit.transform.gameObject;
				if (tempHit.transform.gameObject.CompareTag("NPC")) {
					damageData.isOtherNPC = true;
				} else {
					damageData.isOtherNPC = false;
				}
				damageData.hit = tempHit;
				damageData.attacknormal = tempVec;
				damageData.attackType = Const.AttackType.Projectile;
				// GetDamageTakeAmount expects damageData to already have the following set:
				//   damage
				//   offense
				//   penetration
				//   attackType
				//   berserkActive
				//   isOtherNPC
				//   armorvalue
				//   defense
				damageData.impactVelocity = damageData.damage * 1.5f;
				damageData.damage = Const.a.GetDamageTakeAmount(damageData);
				Const.a.ApplyImpactForce(tempHit.transform.gameObject, damageData.impactVelocity,damageData.attacknormal,damageData.hit.point);
				tempHM.TakeDamage(damageData);
			} else {
				CreateStandardImpactEffects(false);
			}
		}
	}

	// Used for attack type of Const.AttackType.ProjectileLaunched.
	// Hurls a beachball-like object that will apply damage later if it hits something that can be hurt.
	// attackNum corresponds to the attack used so correct lookup tables can be used.
	// attackNum of 1 = Attack1, 2 = Attack2, 3 = Attack3
	void ProjectileLaunched(int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 2;
		MuzzleBurst(attackNum);
		tempVec = GetDirectionRayToEnemy(targettingPosition, attackNum);
		Vector3 startPos = GetAttackStartPoint(attackNum);
		float launchSpeed = Const.a.projectileSpeedAttack1ForNPC[index];
		damageData = Const.SetNPCDamageData(index,attackNum,gameObject); // SetNPCDamageData sets : owner, damage, penetration, offense
		damageData.attacknormal = tempVec;
		damageData.attackType = Const.AttackType.ProjectileLaunched; // Can't call Const.a.GetDamageTakeAmount here since we haven't hit anyone yet so we don't know if isOtherNPC is true or not, called in ProjectileEffectImpact
		Const.PoolType projectileType = Const.PoolType.GrenadeFragLive; // Default to Fragmentation Grenade
		switch (attackNum) {
			case 1:
				projectileType = attack1ProjectileLaunchedType;
				launchSpeed = Const.a.projectileSpeedAttack1ForNPC[index];
				break;
			case 2:
				projectileType = attack2ProjectileLaunchedType;
				launchSpeed = Const.a.projectileSpeedAttack2ForNPC[index];
				break;
			case 3:
				projectileType = attack3ProjectileLaunchedType;
				launchSpeed = Const.a.projectileSpeedAttack3ForNPC[index];
				break;
		}
		// Create and hurl a beachball-like object.  On the developer commentary they said that the projectiles act
		// like a beachball for collisions with enemies, but act like a baseball for walls/floor to prevent hitting corners
		// Calling it a beachball for fun.
		GameObject beachball = Const.a.GetObjectFromPool(projectileType);
		if (beachball != null) {
			ProjectileEffectImpact pei = beachball.GetComponent<ProjectileEffectImpact>();
			if (pei != null) {
				pei.dd = damageData;
				pei.host = gameObject;
				pei.impactType = Const.a.GetPoolImpactFromPoolProjectileType(projectileType);
			}
			beachball.transform.position = startPos;
			beachball.transform.forward = tempVec.normalized;
			beachball.SetActive(true);
			GrenadeActivate ga = beachball.GetComponent<GrenadeActivate>();
			if (ga != null) {
				int typ = 7; // Default to Fragmentation Grenade
				switch (projectileType) {
					case Const.PoolType.GrenadeFragLive: typ = 7; break; // Fragmentation Grenade
					case Const.PoolType.ConcussionLive: typ = 8; break; // Concussion Grenade
					case Const.PoolType.EMPLive: typ = 9; break; // EMP Grenade
					case Const.PoolType.GasLive: typ = 13; break; // Gas Grenade
				}
				ga.Activate(typ);
			}
			Vector3 shove = (beachball.transform.forward * launchSpeed);
			shove += rbody.velocity; // add in the enemy's velocity to the projectile (in case they are riding on a moving platform or something - wait I don't have those!
			beachball.GetComponent<Rigidbody>().velocity = Const.a.vectorZero; // prevent random variation from the last shot's velocity
			beachball.GetComponent<Rigidbody>().AddForce(shove, ForceMode.Impulse);
		}
	}

	// Die in a fiery explosion BOOM!
	// attackNum corresponds to the attack used so correct lookup tables can be used. 1 = Attack1, 2 = Attack2, 3 = Attack3
	void ExplodeAttack(int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 3;
		DamageData dd = Const.SetNPCDamageData(index,attackNum,gameObject);
		if (dd == null) return;

		float take = Const.a.GetDamageTakeAmount(dd);
		dd.other = gameObject;
		dd.damage = take;
		HealthManager hm = null;
		float damageOriginal = dd.damage;
		Collider[] colliders = Physics.OverlapSphere(sightPoint.transform.position, Const.a.attack3RadiusForNPC[index]);
		int i = 0;
		while (i < colliders.Length) {
			if (colliders[i] != null) {
				if (colliders[i].GetComponent<Rigidbody>() != null) colliders[i].GetComponent<Rigidbody>().AddExplosionForce(Const.a.attack3ForceForNPC[index], sightPoint.transform.position, Const.a.attack3RadiusForNPC[index], 1.0f);
				hm = colliders[i].GetComponent<HealthManager>();
				dd.impactVelocity = dd.damage * 1.5f;
				Const.a.ApplyImpactForce(colliders[i].gameObject, dd.impactVelocity,dd.attacknormal,dd.hit.point);
				if (hm != null) hm.TakeDamage(dd);
			}
			i++;
		}
		healthManager.TakeDamage(dd);
	}

	// Typically used for melee.
	void Attack1() {
		ApplyAttackMovement(Const.a.attack1SpeedForNPC[index]);
		if (gracePeriodFinished < PauseScript.a.relativeTime) {
			if (!shotFired) {
				shotFired = true;
				if (attack1SoundTime < PauseScript.a.relativeTime) {
					if (SFX != null && SFXAttack1 != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXAttack1);
					attack1SoundTime = PauseScript.a.relativeTime + Const.a.timeBetweenAttack1ForNPC[index];
				}
				switch (attack1Type) {
					case Const.AttackType.Melee:				ProjectileRaycast(1); break; // Melee
					case Const.AttackType.Projectile:			ProjectileRaycast(1); break;
					case Const.AttackType.ProjectileLaunched:	ProjectileLaunched(1); break;
				}
			}
        }

        if (attackFinished < PauseScript.a.relativeTime) Transition_AttackToRun(1);  // Handle exiting this state.
	}

	// Typically used for normal projectile attack
    void Attack2() {
		ApplyAttackMovement(Const.a.attack2SpeedForNPC[index]);
        if (gracePeriodFinished < PauseScript.a.relativeTime) {
            if (!shotFired) {
                shotFired = true; 
                if (attack2SoundTime < PauseScript.a.relativeTime && SFXAttack2 != null) {
					if (SFX != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXAttack2);
                    attack2SoundTime = PauseScript.a.relativeTime + Const.a.timeBetweenAttack2ForNPC[index];
                }
				switch (attack2Type) {
					case Const.AttackType.Melee:				ProjectileRaycast(2); break; // Melee
					case Const.AttackType.Projectile:			ProjectileRaycast(2); break;
					case Const.AttackType.ProjectileLaunched:	ProjectileLaunched(2); break;
				}
            }
        }

        if (attackFinished < PauseScript.a.relativeTime) Transition_AttackToRun(2); // Handle exiting this state.
	}

	// Typically used for secondary projectile or grenade attack
	void Attack3() {
		if (Const.a.explodeOnAttack3ForNPC[index]) { ExplodeAttack(3); return; } // No time check, this is only done once without delay.  We are dead now so exit on out.

		ApplyAttackMovement(Const.a.attack3SpeedForNPC[index]);
        if (gracePeriodFinished < PauseScript.a.relativeTime) {
            if (!shotFired) {
                shotFired = true;
				if (attack3SoundTime < PauseScript.a.relativeTime && SFXAttack3 != null) {
					if (SFX != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXAttack3);
					attack3SoundTime = PauseScript.a.relativeTime + Const.a.timeBetweenAttack3ForNPC[index];
				}
				switch (attack1Type) {
					case Const.AttackType.Melee:				ProjectileRaycast(3); break; // Melee
					case Const.AttackType.Projectile:			ProjectileRaycast(3); break;
					case Const.AttackType.ProjectileLaunched:	ProjectileLaunched(3); break;
				}
            }
        }
        if (attackFinished < PauseScript.a.relativeTime) Transition_AttackToRun(3); // Handle exiting this state.
	}

	void Pain() {
		if (timeTillPainFinished < PauseScript.a.relativeTime) {
			currentState = Const.aiState.Run; // go into run after we get hurt
			goIntoPain = false;
			timeTillPainFinished = PauseScript.a.relativeTime + Const.a.timeBetweenPainForNPC[index];
		}
	}

	void Dying() {
		if (!dyingSetup) {
			enemy = null; // reset for loading from saves
			if (sleepingCables != null) sleepingCables.SetActive(false);
			if (npcAutomapOverlay != null) npcAutomapOverlay.gameObject.SetActive(false);
			if (deathBurstTimer > 0) {
				deathBurstFinished = PauseScript.a.relativeTime + deathBurstTimer;
			} else {
				if (!deathBurstDone) {
					if (deathBurst != null) deathBurst.SetActive(true); // activate any death effects
					deathBurstDone = true;
				}
			}

			for (int i = 0; i < meleeDamageColliders.Length; i++) {
                if (meleeDamageColliders[i] != null) meleeDamageColliders[i].SetActive(false);
            }

			if (!healthManager.actAsCorpseOnly && SFX != null && SFXDeathClip != null) SFX.PlayOneShot(SFXDeathClip);
			if (Const.a.moveTypeForNPC[index] == Const.aiMoveType.Fly && !healthManager.gibOnDeath) {
				rbody.useGravity = true; // for avian mutant and zero-g mutant
			} else {
				rbody.useGravity = true;
				rbody.isKinematic = true;
			}
			asleep = false;
			rbody.constraints = RigidbodyConstraints.None;
			if (!rbody.freezeRotation) rbody.freezeRotation = true;
			gameObject.layer = 13; // Change to Corpse layer
			transform.position = new Vector3(transform.position.x, transform.position.y + 0.04f, transform.position.z); // bump it up a hair to prevent corpse falling through the floor
			firstSighting = true;
			timeTillDeadFinished = PauseScript.a.relativeTime + Const.a.timeTillDeadForNPC[index]; // wait for death animation to finish before going into Dead()
			if (Const.a.switchMaterialOnDeathForNPC[index] && deathMaterial != null && actualSMR != null) actualSMR.material = deathMaterial;
			dyingSetup = true;
		}

		if (timeTillDeadFinished < PauseScript.a.relativeTime) {
			ai_dead = true;
			ai_dying = false;
			currentState = Const.aiState.Dead;
		}
	}
	private bool deadChecksDone = false;
	void Dead() {
		asleep = false;
		ai_dead = true;
		ai_dying = false;
		if (!deadChecksDone) {
			deadChecksDone = true;
			if (boxCollider != null) { if (boxCollider.enabled) boxCollider.enabled = false; }
			if (sphereCollider != null) { if (sphereCollider.enabled) sphereCollider.enabled = false; }
			if (meshCollider != null) { if (meshCollider.enabled) meshCollider.enabled = false; }
			if (capsuleCollider != null) { if (capsuleCollider.enabled) capsuleCollider.enabled = false; }
			if (searchColliderGO != null) {
				searchColliderGO.SetActive(true);
				rbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
			}
			rbody.useGravity = true;
			if (!rbody.freezeRotation) rbody.freezeRotation = true;
			currentState = Const.aiState.Dead;
			if (npcAutomapOverlayImage != null) npcAutomapOverlayImage.enabled = false;
			if (healthManager.gibOnDeath || healthManager.teleportOnDeath || healthManager.inCyberSpace) {
				if (visibleMeshEntity != null) {
					if (visibleMeshEntity.activeInHierarchy) visibleMeshEntity.SetActive(false); // Normally just turn off the main model, then...
				}
				if (healthManager.gibOnDeath) healthManager.Gib(); // ... turn on the lovely gibs.
				if (healthManager.teleportOnDeath && !healthManager.teleportDone) healthManager.TeleportAway();
			}
		}
	}

	bool CheckIfEnemyInSight() {
		if (Const.a.moveTypeForNPC[index] == Const.aiMoveType.Cyber && Const.a.decoyActive) { LOSpossible = false; return false; }
		if (enemy.GetComponent<PlayerMovement>().Notarget) { enemy = null; LOSpossible = false; return false; } // Force forget when using Notarget cheat

		float dist = Vector3.Distance(enemy.transform.position,sightPoint.transform.position);  // Get distance between enemy and found player	
		if (npcAutomapOverlayImage != null) {
			if (healthManager.health > 0 && !npcAutomapOverlayImage.enabled) npcAutomapOverlayImage.enabled = true;
		}
		if (dist > Const.a.sightRangeForNPC[index]) return false;

		Vector3 checkline = enemy.transform.position - sightPoint.transform.position; // Get vector line made from enemy to found player
		if (Const.a.numberOfRaycastsThisFrame > Const.a.maxRaycastsPerFrame) return inSight; 

        if (Physics.Raycast(sightPoint.transform.position, checkline.normalized, out tempHit, Const.a.sightRangeForNPC[index], Const.a.layerMaskNPCSight)) {
			Const.a.numberOfRaycastsThisFrame++;
            if (tempHit.collider.gameObject == enemy) {
				LOSpossible = true;
                return true;
			}
        }
        LOSpossible = false;
        return false;
	}

	bool CheckIfPlayerInSight() {
        if (Const.a.difficultyCombat == 0) return false;
		if (enemy != null) return CheckIfEnemyInSight();

		LOSpossible = false; // Reset line of sight value. Doing this after CheckIfEnemyInSight so it doesn't break it.

		if (Const.a.moveTypeForNPC[index] == Const.aiMoveType.Cyber && Const.a.decoyActive) return false;
		if (Const.a.player1Capsule == null) return false; // No found player

		// Found player
		if (Const.a.player1PlayerMovementScript.Notarget) return false; // can't see him, he's on notarget. skip to next available player to check against
		tempVec = Const.a.player1Capsule.transform.position;
		Vector3 checkline = tempVec - sightPoint.transform.position; // Get vector line made from enemy to found player
		float dist = Vector3.Distance(tempVec,sightPoint.transform.position);  // Get distance between enemy and found player
		if (npcAutomapOverlayImage != null) {
			if (healthManager.health > 0 && !npcAutomapOverlayImage.enabled) npcAutomapOverlayImage.enabled = true;
		}
		if (dist > Const.a.sightRangeForNPC[index]) return false; // don't waste time doing raycasts if we won't be able to see them anyway

		float angle = Vector3.Angle(checkline,sightPoint.transform.forward);
		if (angle < (Const.a.fovForNPC[index] * 0.5f)) {
			// Check for line of sight
			if (Const.a.numberOfRaycastsThisFrame > Const.a.maxRaycastsPerFrame) return inSight; // Don't change it.

			if (Physics.Raycast(sightPoint.transform.position, checkline.normalized, out tempHit, (dist + 0.1f),Const.a.layerMaskNPCSight)) { // Changed from using sight range to dist to minimize checkdistance.
				Const.a.numberOfRaycastsThisFrame++;
				if (tempHit.collider.gameObject == Const.a.player1Capsule) {
					LOSpossible = true;  // Clear path from enemy to found player
					SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
					PlaySightSound();
					return true;
				}
			} else {
				if (Const.a.player1PlayerHealthScript != null) {
					if (Const.a.player1PlayerHealthScript.makingNoise) {
						if (dist < Const.a.hearingRangeForNPC[index]) {
							SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
							PlaySightSound();
							return true;
						}
					}
				}
			}
		} else {
			if (dist < Const.a.distToSeeBehindForNPC[index]) {
				// Still check for line of sight, some locations there could be walls in the ways still due to the angles
				// Changed from using sight range to dist to minimize checkdistance; added slight amount to it though to avoid quantization inaccuracies.
				if (Const.a.numberOfRaycastsThisFrame > Const.a.maxRaycastsPerFrame) return inSight; // Don't change it.

				if (Physics.Raycast(sightPoint.transform.position, checkline.normalized, out tempHit, (dist + 0.1f),Const.a.layerMaskNPCSight)) {
					Const.a.numberOfRaycastsThisFrame++;
					if (tempHit.collider.gameObject == Const.a.player1Capsule) {
						LOSpossible = true;  // Clear path from enemy to found player
						SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
						PlaySightSound();
						return true;
					}
				}
			}
			if (Const.a.player1PlayerHealthScript != null) {
				if (Const.a.player1PlayerHealthScript.makingNoise) {
					if (dist < Const.a.hearingRangeForNPC[index]) {
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
		if (firstSighting && healthManager.health > 0) {
			firstSighting = false;
			if (!healthManager.actAsCorpseOnly && SFXSightSound != null && SFX != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXSightSound);
		}
	}
	
	void enemyInFrontChecks(GameObject target) {
		if (target == null) { infront = false; inProjFOV = false; return; }

        infrontVec = target.transform.position;
		infrontVec.y = sightPoint.transform.position.y; // ignore height difference
		infrontVec = Vector3.Normalize(infrontVec - sightPoint.transform.position);
		inProjFOV = false;
		infront = false;
        dotResult = Vector3.Dot(infrontVec,sightPoint.transform.forward);
        if (dotResult > 0.800) inProjFOV = true; // enemy is within ±18° of forward facing vector
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

			if (ud != null) {
				if (ud.owner != null) {
					if (ud.owner == tempent) {
						enemy = tempent;
					} else {
						if (tempent != enemy) enemy = tempent;	
					}
				} else {
					if (tempent != enemy) enemy = tempent;	
				}
			} else {
				if (tempent != enemy) enemy = tempent;
			}
			if (enemy != null) enemyHM = enemy.GetComponent<HealthManager>();
		}
	}

	public void AwakeFromSleep(UseData ud) {
		if (sleepingCables != null) sleepingCables.SetActive(false);
		asleep = false;
		if (ud != null) Alert(ud);
	}
}
