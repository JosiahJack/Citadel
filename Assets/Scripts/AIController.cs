using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DigitalRuby.LightningBolt;

public class AIController : MonoBehaviour {
	// External manually assigned references, required
	public int index = 0; // NPC reference index for looking up constants in
						  // tables in Const.cs // save
	public HealthManager healthManager;

	// External manually assigned references, but ok if not assigned
    public GameObject searchColliderGO;
	public LightningBoltScript laserLightning;
	public Transform[] walkWaypoints; // point(s) for NPC to walk to when
									  // roaming or patrolling
	public GameObject[] meleeDamageColliders; // Used by avian mutant lunge
    public GameObject muzzleBurst;
    public GameObject muzzleBurst2;
    public GameObject visibleMeshEntity;
    public GameObject gunPoint;
    public GameObject gunPoint2;
	public Material deathMaterial;
	public SkinnedMeshRenderer actualSMR;
	public GameObject deathBurst;
	public GameObject sightPoint;
	public AIState currentState; // save (referenced by int index 0 thru 10)
	public bool walkPathOnStart = false; // save
    public bool dontLoopWaypoints = false; // save
	public bool visitWaypointsRandomly = false; // save
	public bool asleep = false; // save, Check if enemy starts out asleep such
								// as the sleeping sec-2 bots on level 8 in the
								// maintenance and recharge bays.
	public bool wandering; // save
	public bool actAsTurret = false; // save

	// Internal, keeping exposed in inspector for troubleshooting.
	public GameObject enemy; // save (referenced by int index enemIDRead)

	// Internal
	[HideInInspector] public string targetID; // save
	[HideInInspector] public bool hasTargetIDAttached = false; // save
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
	[HideInInspector] public float idleTime; // save
	[HideInInspector] public float attack1SoundTime; // save
	[HideInInspector] public float attack2SoundTime; // save
	[HideInInspector] public float attack3SoundTime; // save
	[HideInInspector] public int SFXIndex = -1; // save
	[HideInInspector] public float timeTillEnemyChangeFinished; // save
	[HideInInspector] public float timeTillDeadFinished; // save
	[HideInInspector] public float timeTillPainFinished; // save
	[HideInInspector] public AudioSource SFX;
	[HideInInspector] public Rigidbody rbody;
	[HideInInspector] public float tickFinished; // save
	[HideInInspector] public float raycastingTickFinished; // save
	[HideInInspector] public float huntFinished; // save
	[HideInInspector] public bool hadEnemy; // save
	[HideInInspector] public Vector3 lastKnownEnemyPos; // save
	[HideInInspector] public Vector3 tempVec; // save
	private Vector3 tempVec2; // Only ever used right away, nosave
	[HideInInspector] public bool shotFired = false; // save
    private DamageData damageData;
    private RaycastHit tempHit;
    private HealthManager tempHM;
	[HideInInspector] public float randomWaitForNextAttack1Finished; // save
	[HideInInspector] public float randomWaitForNextAttack2Finished; // save
	[HideInInspector] public float randomWaitForNextAttack3Finished; // save
	[HideInInspector] public Vector3 idealTransformForward; // save
	[HideInInspector] public Vector3 idealPos; // used by flyers to establish
											   // correct height // save
	[HideInInspector] public float attackFinished; // save
	[HideInInspector] public float attack2Finished; // save
	[HideInInspector] public float attack3Finished; // save
	[HideInInspector] public Vector3 targettingPosition; // Used to give the
														 // player a chance to
														 // dodge attacks by
														 // moving after start
														 // of an attack, enemy
														 // attacks along same
														 // starting line, save
	[HideInInspector] public float deathBurstFinished; // save
	[HideInInspector] public bool deathBurstDone; // save
	private NavMeshPath searchPath;
	[HideInInspector] public float tranquilizeFinished; // save
	[HideInInspector] public bool hopDone; // save
	[HideInInspector] public float wanderFinished; // save
	private float dotResult = -1f; // Only ever used right away, nosave
	private Vector3 infrontVec; // Only ever used right away, nosave
	[HideInInspector] public bool startInitialized = false; // nosave
	private HealthManager enemyHM;
	private float stopDistance = 1.28f; // Constant
	private Vector3 faceVec; // Only ever used right away, nosave
	private Quaternion lookRot; // Only ever used right away, nosave
	private Animator hopAnimator;
	private bool deadChecksDone = false;
	private float near;
	private float mid;
	private float far;

	public void Tranquilize() {
		tranquilizeFinished = PauseScript.a.relativeTime
							  + Const.a.timeForTranquilizationForNPC[index];
	}

	private void DeactivateMeleeColliders() {
		if (meleeDamageColliders.Length < 1) return;

		for (int i = 0; i < meleeDamageColliders.Length; i++) {
			GameObject mdc = meleeDamageColliders[i];
			if (mdc == null) continue;

			if (mdc.activeSelf) mdc.SetActive(false);
		}
	}

	private void PreActivateMeleeColliders() {
		for (int i = 0; i < meleeDamageColliders.Length; i++) {
			GameObject mdc = meleeDamageColliders[i];
			if (mdc == null) continue;

			if (!mdc.activeSelf) mdc.SetActive(true);
			AIMeleeDamageCollider mcs =
				mdc.GetComponent<AIMeleeDamageCollider>();
			if (mcs == null) continue;

			mcs.MeleeColliderSetup(index,meleeDamageColliders.Length,10f,
								   gameObject);
		}
	}

	bool IsCyberNPC() {
		if (healthManager != null) {
			if (healthManager.inCyberSpace) return true;
		}

		if (Const.a.moveTypeForNPC[index] == AIMoveType.Cyber) return true;
		if (Const.a.typeForNPC[index] == NPCType.Cyber) return true;
		return false;
	}

	// Initialization and find components
	public void Start() {
		gameObject.layer = 10; // NPC Layer.
        rbody = GetComponent<Rigidbody>();
		rbody.isKinematic = false;
		if (index >= 29 || index < 0) {
			index = 0;
			Debug.Log("Index was out of range with value of "
					  + index.ToString() + " on " + gameObject.name
					  + ", set to index 0.");
		}

		if (Const.a.moveTypeForNPC[index] == AIMoveType.Fly || IsCyberNPC()) {
			rbody.useGravity = false;
		} else {
			rbody.useGravity = true;
		}

		healthManager = GetComponent<HealthManager>();
		if (visibleMeshEntity != null) {
			hopAnimator = visibleMeshEntity.GetComponent<Animator>();
		}

		if ((!healthManager.gibOnDeath && !healthManager.vaporizeCorpse) 
			|| index == 2) { // Avian Mutant
			Utils.Deactivate(searchColliderGO);
		}

		if (sightPoint == null) sightPoint = gameObject;
		DeactivateMeleeColliders();
		currentDestination = sightPoint.transform.position;
        currentState = AIState.Idle;
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
		idleTime = PauseScript.a.relativeTime
				   + Random.Range(Const.a.timeIdleSFXMinForNPC[index],
								  Const.a.timeIdleSFXMaxForNPC[index]);
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
		if (SFX == null) {
			Debug.Log("WARNING: No audio source for npc at: "
					  + transform.position.ToString());
		} else {
			SFX.playOnAwake = false;
		}

		if (walkWaypoints.Length > 0 && walkWaypoints[currentWaypoint] != null
			&& walkPathOnStart && !asleep) {
            currentDestination = 
				walkWaypoints[currentWaypoint].transform.position;
            currentState = AIState.Walk; // If waypoints are set, start walking
		} else {
            currentState = AIState.Idle; // No waypoints, stay put
        }

		if (wandering) currentState = AIState.Walk;
		if (asleep) currentState = AIState.Idle;
		tickFinished = PauseScript.a.relativeTime + Const.a.aiTickTime
					   + Random.value;

		raycastingTickFinished = tickFinished + Random.value; // Separate rand.
		attackFinished = PauseScript.a.relativeTime + 1f;
		idealTransformForward = sightPoint.transform.forward;
		deathBurstDone = false;
		searchPath = new NavMeshPath();
		if (!IsCyberNPC()) targetID = Const.GetTargetID(index);
		else             targetID = Const.GetCyberTargetID(index);
		
		startInitialized = true;
	}

	void AI_Face(Vector3 goalLocation) {
		if (asleep) return;

		faceVec = (goalLocation - transform.position).normalized;
		faceVec.y = 0f;
		if (faceVec.sqrMagnitude <= 0) return; // Avoid zero quat error.
		if (faceVec == Vector3.up) return; // Up results in no Y rotation.

		// Rotate as fast as we can towards facing the goal location.
		lookRot = Quaternion.LookRotation(faceVec,Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation,lookRot,
		  Const.a.aiTickTime * Const.a.yawSpeedForNPC[index] * Time.deltaTime); 
	}

	void LateUpdate() {
		Const.a.numberOfRaycastsThisFrame = 0;
	}

	bool HasHealth(HealthManager hm) {
		if (hm == null) return false;
		if (IsCyberNPC()) return (hm.cyberHealth > 0);
		return (hm.health > 0);
	}

	void EnableAutomapOverlay() {
		if (healthManager.linkedOverlay == null) return;

		if (healthManager.health > 0 && gameObject.activeInHierarchy) {
			Utils.EnableImage(healthManager.linkedOverlay);
		} else {
			Utils.DisableImage(healthManager.linkedOverlay);
		}
	}

	void Update() {
		if (!startInitialized) return;

		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) {
			rbody.isKinematic = true; // Prevent movement when paused.
			return;
		}

		rbody.isKinematic = false;
		if (raycastingTickFinished >= PauseScript.a.relativeTime) return;

		raycastingTickFinished = PauseScript.a.relativeTime
								 + Const.a.raycastTick;

		EnableAutomapOverlay();
		inSight = CheckIfPlayerInSight();
		if (enemy != null && HasHealth(healthManager)) {
			// Check if enemy health drops to 0
			if (enemyHM == null) enemyHM = enemy.GetComponent<HealthManager>();
			if (enemyHM != null) {
				if (!HasHealth(enemyHM)) {
					if (IsCyberNPC()) {
						currentState = AIState.Idle;
					} else {
						// Enemy is dead, let's wander around aimlessly now
						wandering = true;
						currentState = AIState.Walk;
					}
					enemy = null;
					enemyHM = null;
				}
			}

			// Enemy still has health
			if (enemy != null) {
				enemyInFrontChecks(enemy);
				rangeToEnemy = (enemy.transform.position
							- sightPoint.transform.position).sqrMagnitude;
			}
		} else {
			infront = false;
			inProjFOV = false;
			rangeToEnemy = Const.a.sightRangeForNPC[index]
						   * Const.a.sightRangeForNPC[index];
		}
	}

	void FixedUpdate() {
		if (PauseScript.a.Paused()) return; // Don't do any checks or anything
		if (PauseScript.a.MenuActive()) return; // else...we're paused!
		if (!startInitialized) return;

		if ((!rbody.useGravity && !IsCyberNPC()
			&& Const.a.moveTypeForNPC[index] != AIMoveType.Fly)
			&& !(currentState == AIState.Dead
				 || currentState == AIState.Dying)) {
			rbody.useGravity = true;
		}

        // Think every tick seconds to save on CPU and prevent race conditions.
        if (tickFinished < PauseScript.a.relativeTime) {
			tickFinished = PauseScript.a.relativeTime + Const.a.aiTickTime;
			Think();
			if (healthManager.linkedOverlay != null) {
				if (!IsCyberNPC()
					//&& healthManager.health > 0 // Only health, not cyber.
					&& Inventory.a.hasHardware[1]
					&& Inventory.a.NavUnitVersion() > 1) {

					//Utils.EnableImage(healthManager.linkedOverlay);
					//tempVec2 = Automap.a.GetMapPos(transform.position);
					//healthManager.linkedOverlay.rectTransform.localPosition = tempVec2;
					healthManager.UpdateLinkedOverlay();
				} else {
					Utils.DisableImage(healthManager.linkedOverlay);
				}
			}
		}

        // Rotation and Special movement that must be done every FixedUpdate
        if (currentState != AIState.Dead) {
            if (currentState != AIState.Idle) {
				if (actAsTurret && enemy != null) {
					currentDestination = enemy.transform.position;
					currentDestination.y = enemy.transform.position.y + 0.24f;
				}

                idealTransformForward = currentDestination
										- sightPoint.transform.position;

                if (!IsCyberNPC()) idealTransformForward.y = 0;
				idealTransformForward = idealTransformForward.normalized;
				if (idealTransformForward.sqrMagnitude > Mathf.Epsilon) {
					AI_Face(currentDestination);
				}
            }
        }
	}

	void Think() {
		if (dyingSetup && deathBurstFinished < PauseScript.a.relativeTime
			&& !deathBurstDone) {
			// Activate any death effects
			if (deathBurst != null) deathBurst.SetActive(true);
			deathBurstDone = true;
		}

		if (!HasHealth(healthManager)) {
			// If we haven't gone into dying and we aren't dead, do dying.
			if (!ai_dying && !ai_dead) {
				ai_dying = true; // No going back!
				currentState = AIState.Dying; // Start to collapse in a heap,
											  // melt, explode, etc.
			}
		}

		switch (currentState) {
			case AIState.Idle: 	  Idle(); 	 break;
			case AIState.Walk:	  Walk(); 	 break;
			case AIState.Run: 	  Run(); 	 break;
			case AIState.Attack1: Attack1(); break;
			case AIState.Attack2: Attack2(); break;
			case AIState.Attack3: Attack3(); break;
			case AIState.Pain: 	  Pain();	 break;
			case AIState.Dying:   Dying(); 	 break;
			case AIState.Dead: 	  Dead(); 	 break;
			default: 			  Idle(); 	 break;
		}

		if (currentState == AIState.Dead || currentState == AIState.Dying) {
			return; // Don't do any checks, we're dead.
		}

		if (asleep) return; // Don't check for an enemy, we are sleeping! shh!!

		if (Const.a.moveTypeForNPC[index] == AIMoveType.Fly
			&& tranquilizeFinished < PauseScript.a.relativeTime) {
			FlierMoveToHoverHeight();
		}	
	}

	void FlierMoveToHoverHeight() {
		float distUp = 0;
		float distDn = 0;
		Vector3 floorPoint = new Vector3();
		floorPoint = Const.a.vectorZero;
		if (!Const.a.RaycastBudgetExceeded()) {
			if (Physics.Raycast(sightPoint.transform.position,
								sightPoint.transform.up * -1,out tempHit,
								Const.a.sightRangeForNPC[index],
								Const.a.layerMaskNPCSight)) {
				Const.a.numberOfRaycastsThisFrame++;
				distDn = Vector3.Distance(sightPoint.transform.position,
										  tempHit.point);
				floorPoint = tempHit.point;
			}

			if (Physics.Raycast(sightPoint.transform.position,
								sightPoint.transform.up,out tempHit,
								Const.a.sightRangeForNPC[index],
								Const.a.layerMaskNPCSight)) {
				Const.a.numberOfRaycastsThisFrame++;
				distUp = Vector3.Distance(sightPoint.transform.position,
										  tempHit.point);
			}
			float distT = (distUp + distDn);
			float yHeight = Const.a.flightHeightForNPC[index];
			if (Const.a.flightHeightIsPercentageForNPC[index]) yHeight *= distT;
			idealPos = floorPoint + new Vector3(0,yHeight, 0);
		}

		if (enemy != null) idealPos.y = enemy.transform.position.y + 0.24f;
		if (idealPos != transform.position) {
			if (Const.a.runSpeedForNPC[index] > 0) {
				float spd = Const.a.runSpeedForNPC[index] * Time.deltaTime;
				transform.position = Vector3.MoveTowards(transform.position,
														 idealPos,spd);
			}
		}
	}

	public bool CheckPain() {
		if (asleep) return false;

		if (goIntoPain && timeTillPainFinished < PauseScript.a.relativeTime) {
			if (!IsCyberNPC()) currentState = AIState.Pain;
			if (attacker != null) {
				if (timeTillEnemyChangeFinished < PauseScript.a.relativeTime) {
					timeTillEnemyChangeFinished = PauseScript.a.relativeTime
						+ Const.a.timeToChangeEnemyForNPC[index];
					enemy = attacker; // Switch to whoever just attacked us
					if (enemy != null) {
						enemyHM = enemy.GetComponent<HealthManager>();
						lastKnownEnemyPos = enemy.transform.position;
						currentDestination = enemy.transform.position;
					}
				}
			}
			goIntoPain = false;
			timeTillPainFinished = PauseScript.a.relativeTime
								   + Const.a.timeToPainForNPC[index];
			return true;
		}
		return false;
	}

	void Idle() {
		if (enemy != null) { currentState = AIState.Run; return; }

		if (idleTime < PauseScript.a.relativeTime) {
			if (UnityEngine.Random.Range(0,1f) < 0.5f) { // 50% Chance of idle.
				SFXIndex = Const.a.sfxIdleForNPC[index];
				Utils.PlayOneShotSavable(SFX,SFXIndex);
			}
			idleTime = PauseScript.a.relativeTime
					   + Random.Range(Const.a.timeIdleSFXMinForNPC[index],
									  Const.a.timeIdleSFXMaxForNPC[index]);
		}

		if (asleep) {
			rbody.constraints =   RigidbodyConstraints.FreezePositionX
								| RigidbodyConstraints.FreezePositionY
								| RigidbodyConstraints.FreezePositionZ
								| RigidbodyConstraints.FreezeRotationX
								| RigidbodyConstraints.FreezeRotationY
								| RigidbodyConstraints.FreezeRotationZ;
		}

		CheckPain(); // Go into pain if just hurt, data sent by HealthManager.
	}

	void Walk() {
        if (CheckPain()) return; // Go into pain if just hurt
		if (asleep) return;
        if (inSight || enemy != null) { currentState = AIState.Run; return; }
        if (actAsTurret) { currentState = AIState.Idle; return; }
        if (Const.a.moveTypeForNPC[index] == AIMoveType.None) return;
		if (tranquilizeFinished >= PauseScript.a.relativeTime) return;

		float dist = Vector3.Distance(sightPoint.transform.position,
									  currentDestination);

		if (wandering) {
			if (wanderFinished < PauseScript.a.relativeTime || (dist < 0.64f)) {
				wanderFinished = PauseScript.a.relativeTime
								 + UnityEngine.Random.Range(3f,8f);

				float newX = transform.position.x
							 + UnityEngine.Random.Range(-79f,79f);

				float newZ = transform.position.z
							 + UnityEngine.Random.Range(-79f,79f);

				currentDestination = new Vector3(newX,0f,newZ);
			}
		}

		// Destination still far away and turned to within angle to move, move
		if (dist > stopDistance) {
			if (WithinAngleToTarget()) {
				if (Const.a.hopsOnMoveForNPC[index]) {
					// Move it move it.
					float playbackTime = 1f;
					if (hopAnimator != null) {
						AnimatorStateInfo asi =
							hopAnimator.GetCurrentAnimatorStateInfo(0);

						playbackTime = asi.normalizedTime;
					}

					if (playbackTime > 0.1395f) { // Hopper only!
						if (!hopDone) {
							hopDone = true;
							Vector3 force = sightPoint.transform.forward * 500f;
							rbody.AddForce(force);
						}
					} else {
						hopDone = false;
					}
				} else {
					tempVec = (sightPoint.transform.forward
							   * Const.a.walkSpeedForNPC[index]);

					tempVec.y = rbody.velocity.y; // Carry across gravity.
					rbody.velocity = tempVec;
				}
			}
			return; // Still moving to same target destination.
		}

		if (walkWaypoints.Length < 1) {
			if (!wandering) {
				currentState = AIState.Idle; // No wandering, just go to Idle.
			}
			return; // No waypoint to visit, wait for wandering timer.
		}

		// Need new spot to move to
		if (visitWaypointsRandomly) {
			// Max is exclusive for the integer overload, no need to do
			// (walkWaypoints.Length - 1).
			currentWaypoint = Random.Range(0, walkWaypoints.Length);
		} else {
			currentWaypoint++;
		}

		if (currentWaypoint < 0) currentWaypoint = 0;
		if ((currentWaypoint >= (walkWaypoints.Length - 1))) {
			currentWaypoint = 0; // Wrap around.

			// Stop when reached end of list; out of waypoints.
			if (dontLoopWaypoints) {
				currentState = AIState.Idle;
				return;
			}
		}

		if (walkWaypoints[currentWaypoint] == null) return; // No gaps allowed.

		currentDestination = walkWaypoints[currentWaypoint].transform.position;
	}

	bool CanAttack1(float dist) {
    	if (rangeToEnemy >= dist) return false;
		if (Const.a.attackTypeForNPC[index] == AttackType.None) return false;
		if (!infront) return false;
		if (randomWaitForNextAttack1Finished >= PauseScript.a.relativeTime) {
			return false;
		}

		return true;
	}

	bool CanAttack2(float dist) {
    	if (rangeToEnemy >= dist) return false;
		if (Const.a.attackTypeForNPC2[index] == AttackType.None) return false;
		if (!infront) return false;
		if (!inProjFOV) return false;
		if (randomWaitForNextAttack2Finished >= PauseScript.a.relativeTime) {
			return false;
		}

		return true;
	}

	bool CanAttack3(float dist) {
    	if (rangeToEnemy >= dist) return false;
		if (Const.a.attackTypeForNPC3[index] == AttackType.None) return false;
		if (!infront) return false;
		if (!inProjFOV) return false;
		if (randomWaitForNextAttack3Finished >= PauseScript.a.relativeTime) {
			return false;
		}

		return true;
	}

	void StartAttack1() {
		attackFinished = PauseScript.a.relativeTime
						 + Const.a.timeBetweenAttack1ForNPC[index]
						 + Const.a.timeToActualAttack1ForNPC[index];

		gracePeriodFinished = PauseScript.a.relativeTime
							  + Const.a.timeToActualAttack1ForNPC[index];

		currentState = AIState.Attack1;
		if (Const.a.preactivateMeleeCollidersForNPC[index]) {
			PreActivateMeleeColliders();
		}
	}

	void StartAttack2() {
		attackFinished = PauseScript.a.relativeTime
						 + Const.a.timeBetweenAttack2ForNPC[index]
						 + Const.a.timeToActualAttack2ForNPC[index];

		gracePeriodFinished = PauseScript.a.relativeTime
							  + Const.a.timeToActualAttack2ForNPC[index];

		currentState = AIState.Attack2;
	}

	void StartAttack3() {
		attackFinished = PauseScript.a.relativeTime
						 + Const.a.timeBetweenAttack3ForNPC[index]
						 + Const.a.timeToActualAttack3ForNPC[index];

		gracePeriodFinished = PauseScript.a.relativeTime
							  + Const.a.timeToActualAttack3ForNPC[index];

		currentState = AIState.Attack3;
	}

	void HopMove() {
		if (actAsTurret) return;

		// Move it move it.
		float playbackTime = 0.0f;
		if (hopAnimator != null) {
			AnimatorStateInfo asi = hopAnimator.GetCurrentAnimatorStateInfo(0);
			playbackTime = asi.normalizedTime;
		}

		if (playbackTime > 0.1395f) {
			if (!hopDone) {
				hopDone = true;
				rbody.AddForce(sightPoint.transform.forward * 500f); // Huh!
			}
		} else {
			hopDone = false;
		}
	}

	void RunMove() {
		if (actAsTurret) return;

		tempVec = (sightPoint.transform.forward * Const.a.runSpeedForNPC[index]);
		if (rbody.useGravity) tempVec.y = rbody.velocity.y; // Preserve gravity.
		rbody.velocity = tempVec;
	}

	void Run() {
		if (CheckPain()) return; // Go into pain just hurt
		if (asleep) return;
		if (enemy == null) { currentState = AIState.Idle; return; }
		if (tranquilizeFinished >= PauseScript.a.relativeTime) return;

        if (inSight) {
			if (enemy != null) {
				targettingPosition = enemy.transform.position;
				currentDestination = enemy.transform.position;
				lastKnownEnemyPos = enemy.transform.position;
				if (IsCyberNPC()) {
					targettingPosition =
						PlayerMovement.a.cameraObject.transform.position;
				}
			}

            huntFinished = PauseScript.a.relativeTime
						   + Const.a.huntTimeForNPC[index];

			shotFired = false;
			huntFinished = PauseScript.a.relativeTime;
			if (Const.a.difficultyCombat <= 1) { // More forgetful on easy.
				huntFinished += (Const.a.huntTimeForNPC[index] * 0.75f);
			} else if (Const.a.difficultyCombat >= 3) { // Good memory on hard.
				huntFinished += (Const.a.huntTimeForNPC[index] * 2.00f); 
			}

			near = Const.a.rangeForNPC[index]  * Const.a.rangeForNPC[index];
			mid  = Const.a.rangeForNPC2[index] * Const.a.rangeForNPC2[index];
			far  = Const.a.rangeForNPC3[index] * Const.a.rangeForNPC3[index];
            if (CanAttack1(near)) {
				StartAttack1();
				return;
            } else if (CanAttack2(mid)) {
				StartAttack2();
				return;
			} else if (CanAttack3(far)) {
				StartAttack3();
				return;
			}

			// Enemy still far away and turned to within angle, then move
			if ((Const.a.moveTypeForNPC[index] != AIMoveType.None)
				&& (rangeToEnemy > (stopDistance * stopDistance))) {
				if (WithinAngleToTarget()) {
					if (Const.a.hopsOnMoveForNPC[index]) HopMove();
					else                                 RunMove(); // <<<<<
				}
				
			}
        } else {
            if (huntFinished > PauseScript.a.relativeTime) {
                Hunt();
            } else {
                enemy = null;
				enemyHM = null;
				wandering = true; // Magically look like we are still searching
								  // maybe?  Sometimes!
				wanderFinished = PauseScript.a.relativeTime - 1f;
                currentState = AIState.Walk;
                return;
            }
		}
	}

    void Hunt() {
		if (NavMesh.CalculatePath(sightPoint.transform.position,
								  enemy.transform.position,0,searchPath)) {
			currentDestination = searchPath.corners[0];
		} else {
			currentDestination = lastKnownEnemyPos;
		}

		// Destination is still far enough away and within angle, then move.
		if ((Const.a.moveTypeForNPC[index] != AIMoveType.None)
			&& ((sightPoint.transform.position - currentDestination).sqrMagnitude > (stopDistance * stopDistance))) {
			if (WithinAngleToTarget() && !actAsTurret && Const.a.runSpeedForNPC[index] > 0) rbody.velocity = (sightPoint.transform.forward * Const.a.runSpeedForNPC[index]);
		}
    }

	// Commonized function to remove previous boilerplate code from all 3 attack functions below.
	// Applies movement towards the enemy while attacking, assumes that we were already facing enemy within the attack angle.
	void ApplyAttackMovement(float speedToApply) {
		if (actAsTurret) {
			currentDestination = sightPoint.transform.position;
			return;
		}
		if (speedToApply <= 0) return;

		if (Vector3.Distance(sightPoint.transform.position, currentDestination) > stopDistance && tranquilizeFinished < PauseScript.a.relativeTime) {
			if (WithinAngleToTarget()) rbody.AddForce(transform.forward * speedToApply);
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

		DeactivateMeleeColliders();
		goIntoPain = false; // Prevent going into pain immediately after an attack.
		currentState = AIState.Run;
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

    void CreateStandardImpactEffects(bool useBlood) {
        // Determine blood type of hit target and spawn corresponding blood particle effect from the Const.Pool
        if (useBlood) {
            GameObject impact = Const.a.GetImpactType(tempHM);
            if (impact != null) {
                impact.transform.position = tempHit.point + (tempHit.normal*0.08f);
                impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, tempHit.normal);
                impact.SetActive(true);
            }
        } else {
			GameObject impact = Const.a.GetObjectFromPool(PoolType.SparksSmall); //Didn't hit an object with a HealthManager script, use sparks
			if (impact != null) {
				impact.transform.position = tempHit.point + tempHit.normal;
				impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, tempHit.normal);
				impact.SetActive(true);
			}
        }
    }

	// Activates a GameObject that has automatically playing particle effects, lights, etc.
	// The muzzle bursts are all set up to deactivate on their own; no need to check them later.
	// attackNum corresponds to the attack used so correct lookup tables can be used.
	// attackNum of 1 = Attack1, 2 = Attack2, 3 = Attack3
	void MuzzleBurst(int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 1;
		if (index == 18) {
			Utils.Activate(muzzleBurst); // Also activate this one too
		}

		switch (attackNum) {
			case 2:
				Utils.Activate(muzzleBurst);
				break;
			case 3:
				Utils.Activate(muzzleBurst2);
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

			tempHM = tempHit.collider.transform.gameObject.GetComponent<HealthManager>(); // Thanks andeeeeeee!!
			if (tempHM == null) tempHM = tempHit.transform.gameObject.GetComponent<HealthManager>();

			return true; // True indicates we hit something, not that we hit something that can be hurt.  We need to know to apply spark impact effects still on walls and such.
		}
        return false;
    }

	// Used for attack type of AttackType.Projectile.
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
				GameObject dynamicObjectsContainer = LevelManager.a.GetCurrentDynamicContainer();
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
				// DamageData.SetNPCData sets:
				//   owner
				//   damage
				//   penetration
				//   offense
				damageData = DamageData.SetNPCData(index,attackNum,gameObject);
				damageData.other = tempHit.transform.gameObject; // Using tempHit.transform instead of tempHit.collider.transform to get overall parent of another NPC or of the player
				if (tempHit.transform.gameObject.CompareTag("NPC")) {
					damageData.isOtherNPC = true;
				} else {
					damageData.isOtherNPC = false;
				}
				damageData.hit = tempHit;
				damageData.attacknormal = tempVec;
				damageData.attackType = AttackType.Projectile;
				// GetDamageTakeAmount expects damageData to already have the following set:
				//   damage
				//   offense
				//   penetration
				//   attackType
				//   berserkActive
				//   isOtherNPC
				//   armorvalue
				//   defense
				damageData.impactVelocity = damageData.damage;
				if (tempHit.collider.transform.gameObject.CompareTag("Player")) {
					damageData.impactVelocity *= 0.5f;
				}

				damageData.damage = DamageData.GetDamageTakeAmount(damageData);
				//Utils.ApplyImpactForce(tempHit.collider.transform.gameObject, damageData.impactVelocity,damageData.attacknormal,damageData.hit.point);
				CreateStandardImpactEffects(true);
				tempHM.TakeDamage(damageData);
			} else {
				CreateStandardImpactEffects(false);
			}
		}
	}

	// Used for attack type of AttackType.ProjectileLaunched.
	// Hurls a beachball-like object that will apply damage later if it hits
	// something that can be hurt.
	//   attackNum corresponds to the correct lookup table
	//   attackNum of 1 = Attack1, 2 = Attack2, 3 = Attack3
	void ProjectileLaunched(int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 3;
		MuzzleBurst(attackNum);
		tempVec = GetDirectionRayToEnemy(targettingPosition, attackNum);
		Vector3 startPos = GetAttackStartPoint(attackNum);
		// DamageData.SetNPCData sets:
		//	 owner
		//   damage
		//   penetration
		//   offense
		damageData = DamageData.SetNPCData(index,attackNum,gameObject);
		damageData.attacknormal = tempVec;
		damageData.attackType = AttackType.ProjectileLaunched;
		// Can't call DamageData.GetDamageTakeAmount here since we haven't hit
		// anyone yet so we don't know if isOtherNPC is true or not, called in
		// ProjectileEffectImpact.

		// Create and hurl a beachball-like object.  On the developer
		// commentary they said that the projectiles act like a beachball for
		// collisions with enemies, but act like a baseball for walls/floor to
		// prevent hitting corners.  Calling it a beachball for fun.
		GameObject beachball = null;
		float launchSpeed = 10f;
		int masterIndex = 370; // Default frag.
		switch (attackNum) {
			case 1:
				masterIndex = Const.a.projectile1PrefabForNPC[index];
				launchSpeed = Const.a.projectileSpeedAttack1ForNPC[index];
				break;
			case 2:
				masterIndex = Const.a.projectile2PrefabForNPC[index];
				launchSpeed = Const.a.projectileSpeedAttack2ForNPC[index];
				break;
			case 3:
				masterIndex = Const.a.projectile3PrefabForNPC[index];
				launchSpeed = Const.a.projectileSpeedAttack3ForNPC[index];
				break;
		}

		beachball = ConsoleEmulator.SpawnDynamicObject(masterIndex,-1);
		if (beachball == null) beachball = Const.a.useableItems[63]; // Frag
		beachball.tag = "NPC";
		beachball.layer = 24; // NPCBullet
		ProjectileEffectImpact pei = 
			beachball.GetComponent<ProjectileEffectImpact>();
		if (pei != null) {
			pei.dd = damageData;
			pei.host = gameObject;
		}
		beachball.transform.position = startPos;
		beachball.transform.forward = tempVec.normalized;
		beachball.SetActive(true);
		GrenadeActivate ga = beachball.GetComponent<GrenadeActivate>();
		if (ga != null) ga.Activate();
		Vector3 shove = (beachball.transform.forward * launchSpeed);
		if (IsCyberNPC()) {
			if (enemy != null) {
				Rigidbody rbodyEnemy = enemy.GetComponent<Rigidbody>();
				if (rbodyEnemy != null) {
					shove = shove + (rbodyEnemy.velocity * 0.5f);
				}
			}
		}

		// Add in the enemy's velocity to the projectile (in case they are
		// riding on a moving platform or something - wait I don't have those!)
		shove += rbody.velocity;

		// Ensure no velocity to start with.
		beachball.GetComponent<Rigidbody>().velocity = Const.a.vectorZero;
		beachball.GetComponent<Rigidbody>().AddForce(shove, ForceMode.Impulse);
	}

	// Die in a fiery explosion BOOM!
	//   attackNum of 1 = Attack1, 2 = Attack2, 3 = Attack3
	void ExplodeAttack(int attackNum) {
		if (attackNum < 1 || attackNum > 3) attackNum = 3;
		DamageData dd = DamageData.SetNPCData(index,attackNum,gameObject);
		if (dd == null) return;

		float take = DamageData.GetDamageTakeAmount(dd);
		dd.other = gameObject;
		dd.damage = take;
		Utils.ApplyImpactForceSphere(dd,sightPoint.transform.position,
									 Const.a.attack3RadiusForNPC[index],1.5f,
									 Const.a.layerMaskNPCAttack);
		healthManager.TakeDamage(dd);
	}

	// Attack type determines raycast or launched object.  Checked for None
	//   elsewhere.
	//
	// attackIndex for Attack 1, 2, or 3 lookup tables.
	void AIAttack(AttackType att_type, int ind) {
		if (ind < 1 || ind > 3) ind = 1; // Melee hitscan by default.

		switch (att_type) {
			case AttackType.Melee:				ProjectileRaycast(ind); break;
			case AttackType.Projectile:			ProjectileRaycast(ind); break;
			case AttackType.ProjectileLaunched:	ProjectileLaunched(ind); break;
		}
	}

	// Typically used for melee.
	void Attack1() {
		ApplyAttackMovement(Const.a.attack1SpeedForNPC[index]);
		if (gracePeriodFinished < PauseScript.a.relativeTime) {
			if (!shotFired) {
				shotFired = true;
				if (attack1SoundTime < PauseScript.a.relativeTime) {
					SFXIndex = Const.a.sfxAttack1ForNPC[index];
					Utils.PlayOneShotSavable(SFX,SFXIndex);
					attack1SoundTime = PauseScript.a.relativeTime
						+ Const.a.timeBetweenAttack1ForNPC[index];
				}
				AIAttack(Const.a.attackTypeForNPC[index],1);
			}
        }

        if (attackFinished < PauseScript.a.relativeTime) {
			Transition_AttackToRun(1);  // Handle exiting this state.
		}
	}

	// Typically used for normal projectile attack
    void Attack2() {
		ApplyAttackMovement(Const.a.attack2SpeedForNPC[index]);
        if (gracePeriodFinished < PauseScript.a.relativeTime) {
            if (!shotFired) {
                shotFired = true; 
                if (attack2SoundTime < PauseScript.a.relativeTime) {
					SFXIndex = Const.a.sfxAttack2ForNPC[index];
					Utils.PlayOneShotSavable(SFX,SFXIndex);
                    attack2SoundTime = PauseScript.a.relativeTime
						+ Const.a.timeBetweenAttack2ForNPC[index];
                }
				AIAttack(Const.a.attackTypeForNPC2[index],2);
            }
        }

        if (attackFinished < PauseScript.a.relativeTime) {
			Transition_AttackToRun(2); // Handle exiting this state.
		}
	}

	// Typically used for secondary projectile or grenade attack
	void Attack3() {
		if (Const.a.explodeOnAttack3ForNPC[index]) {
			ExplodeAttack(3);
			return;  // No time check, this is only done once without delay.
					 // We are dead now so exit on out.
		}

		ApplyAttackMovement(Const.a.attack3SpeedForNPC[index]);
        if (gracePeriodFinished < PauseScript.a.relativeTime) {
            if (!shotFired) {
                shotFired = true;
				if (attack3SoundTime < PauseScript.a.relativeTime) {
					SFXIndex = Const.a.sfxAttack3ForNPC[index];
					Utils.PlayOneShotSavable(SFX,SFXIndex);
					attack3SoundTime = PauseScript.a.relativeTime
						+ Const.a.timeBetweenAttack3ForNPC[index];
				}
				AIAttack(Const.a.attackTypeForNPC3[index],3);
            }
        }
        if (attackFinished < PauseScript.a.relativeTime) {
			Transition_AttackToRun(3); // Handle exiting this state.
		}
	}

	void Pain() {
		if (timeTillPainFinished < PauseScript.a.relativeTime) {
			currentState = AIState.Run; // Go into run after we get hurt
			goIntoPain = false;
			timeTillPainFinished = PauseScript.a.relativeTime
				+ Const.a.timeBetweenPainForNPC[index];
		}
	}

	void DyingSetup() {
		enemy = null; // Reset for loading from saves

		if (Const.a.deathBurstTimerForNPC[index] > 0) {
			deathBurstFinished = PauseScript.a.relativeTime
				+ Const.a.deathBurstTimerForNPC[index];
		} else {
			if (!deathBurstDone) {
				Utils.Activate(deathBurst); // Activate death effects
				deathBurstDone = true;
			}
		}

		DeactivateMeleeColliders();
		if (healthManager != null) {
			if (!healthManager.actAsCorpseOnly) {
				Utils.Deactivate(healthManager.linkedOverlay.gameObject);
				SFXIndex = Const.a.sfxDeathForNPC[index];
				Utils.PlayOneShotSavable(SFX,SFXIndex);
			}

			if (Const.a.moveTypeForNPC[index] == AIMoveType.Fly
				&& (!healthManager.gibOnDeath || index == 2)) { // Avian Mutant
				if (healthManager.gibOnDeath) rbody.useGravity = false;
				else rbody.useGravity = true; // Avian Mutant and Zero-G Mutant
			} else {
				if (healthManager.gibOnDeath) {
					rbody.useGravity = false;
				} else {
					rbody.useGravity = true;
					rbody.isKinematic = true;
				}
			}
		}

		if (IsCyberNPC()) rbody.useGravity = false;
		asleep = false;
		rbody.constraints = RigidbodyConstraints.None;
		if (!rbody.freezeRotation) rbody.freezeRotation = true;
		//gameObject.layer = 13; // Change to Corpse layer

		// Bump it up a hair to prevent corpse falling through the floor
		//transform.position = new Vector3(transform.position.x,
		//								 transform.position.y + 0.04f,
		//								 transform.position.z);

		firstSighting = true;

		// Timer for wait until death animation finishes before Dead().
		timeTillDeadFinished = PauseScript.a.relativeTime;
		timeTillDeadFinished += Const.a.timeTillDeadForNPC[index];
		if (Const.a.switchMaterialOnDeathForNPC[index]
			&& deathMaterial != null && actualSMR != null) {
			actualSMR.material = deathMaterial;
		}

		dyingSetup = true;
	}

	void Dying() {
		if (!dyingSetup) DyingSetup();

		// Check if timer for dying animation is finished letting it play.
		if (timeTillDeadFinished < PauseScript.a.relativeTime) {
			ai_dead = true;
			ai_dying = false;
			currentState = AIState.Dead;
		}
		if (index == 0 || index == 14) Utils.Deactivate(visibleMeshEntity); // Autobomb
	}

	void Dead() {
		asleep = false;
		ai_dead = true;
		ai_dying = false;
		if (deadChecksDone) return;

		currentState = AIState.Dead;
		gameObject.layer = 13; // Corpse layer
		if (searchColliderGO != null && (!healthManager.gibOnDeath
											|| index == 2)) { // Avian Mutant
			searchColliderGO.SetActive(true);
			rbody.constraints = RigidbodyConstraints.FreezePositionX
								| RigidbodyConstraints.FreezePositionZ;
		}

		Utils.DisableImage(healthManager.linkedOverlay);
		if (!rbody.freezeRotation) rbody.freezeRotation = true;
		if (healthManager.gibOnDeath || healthManager.teleportOnDeath
			|| IsCyberNPC()) {
			rbody.useGravity = false;
			if (healthManager.teleportOnDeath) rbody.useGravity = true;

			// Normally just turn off the main model, then turn on lovely gibs.
			Utils.Deactivate(visibleMeshEntity);
			if (healthManager.gibOnDeath) healthManager.Gib();
			if (healthManager.teleportOnDeath && !healthManager.teleportDone) {
				healthManager.TeleportAway();
			}
		} else {
			if (index != 14) { // Hopper turns itself off.
				rbody.useGravity = true;
			}
		}

		if (index == 0) Utils.Deactivate(visibleMeshEntity); // Autobomb
		deadChecksDone = true;
	}

	bool CheckIfEnemyInSight() {
        if (Const.a.difficultyCombat == 0) return false;
		if (PlayerMovement.a.Notarget) {
			enemy = null; // Force forget when using Notarget cheat.
			LOSpossible = false;
			return false;
		}

		if (IsCyberNPC() && Const.a.decoyActive) {
			LOSpossible = false; // Silly decoy hack to prevent seeing player.
			return false;
		}

		// Get distance between enemy and found player
		float dist = Vector3.Distance(enemy.transform.position,
									  sightPoint.transform.position);

		if (dist > Const.a.sightRangeForNPC[index]) return false;
		if (Const.a.numberOfRaycastsThisFrame > Const.a.maxRaycastsPerFrame) {
			return inSight; // Zero order hold last until next actual update.
		}

		// Get vector line made from enemy to found player
		Vector3 line = enemy.transform.position - sightPoint.transform.position;
        if (Physics.Raycast(sightPoint.transform.position,line.normalized,
							out tempHit, Const.a.sightRangeForNPC[index],
							Const.a.layerMaskNPCSight)) {
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

		LOSpossible = false; // Reset line of sight value. Doing this after 
							 // CheckIfEnemyInSight so it doesn't break it.

		if (IsCyberNPC() && Const.a.decoyActive) return false;
		if (Const.a.player1Capsule == null) return false; // No found player

		// Can't see him, he's on notarget.
		if (PlayerMovement.a.Notarget) return false;

		tempVec = Const.a.player1Capsule.transform.position;

		// Get vector line made from enemy to found player
		Vector3 checkline = tempVec - sightPoint.transform.position;

		// Get distance between enemy and found player
		float dist = Vector3.Distance(tempVec,sightPoint.transform.position);

		// Don't waste time raycasting if we won't be able to see them anyway.
		if (dist > Const.a.sightRangeForNPC[index]) return false;

		float angle = Vector3.Angle(checkline,sightPoint.transform.forward);
		if (angle < (Const.a.fovForNPC[index] * 0.5f)) {
			// Check for line of sight
			if (Const.a.RaycastBudgetExceeded()) return inSight; // No change.

			// Changed from using sight range to dist to minimize checkdistance.
			if (Physics.Raycast(sightPoint.transform.position,
								checkline.normalized,out tempHit,
								(dist + 0.1f),Const.a.layerMaskNPCSight)) {
				Const.a.numberOfRaycastsThisFrame++;
				if (tempHit.collider.gameObject == Const.a.player1Capsule) {
					//if (IsCyberNPC()) {
					//	Debug.Log("Drawing ray from " + sightPoint.transform.position.ToString());
					//	Debug.DrawRay(sightPoint.transform.position,
					//				  checkline.normalized * 5f,Color.green,1f,
					//				  true);
					//}
					LOSpossible = true;  // Clear path from enemy to found player
					SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
					PlaySightSound();
					return true;
				}
			} else {
				if (PlayerHealth.a.makingNoise) {
					if (dist < Const.a.hearingRangeForNPC[index]) {
						SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
						PlaySightSound();
						return true;
					}
				}
			}
		} else {
			if (dist < Const.a.distToSeeBehindForNPC[index]) {
				// Still check for line of sight, some locations there could be
				// walls in the ways still due to the angles.  Changed from
				// using sight range to dist to minimize checkdistance; added
				// slight amount to it though to avoid quantization inaccuracies.

				if (Const.a.numberOfRaycastsThisFrame >
					Const.a.maxRaycastsPerFrame) {
					return inSight; // Don't change it, zero order hold.
				}

				if (Physics.Raycast(sightPoint.transform.position,
									checkline.normalized, out tempHit,
									(dist + 0.1f),Const.a.layerMaskNPCSight)) {
					Const.a.numberOfRaycastsThisFrame++;
					if (tempHit.collider.gameObject == Const.a.player1Capsule) {
						LOSpossible = true; // Clear path from enemy to player.
						SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
						PlaySightSound();
						return true;
					}
				}
			}
			if (PlayerHealth.a.makingNoise) {
				if (dist < Const.a.hearingRangeForNPC[index]) {
					SetEnemy(Const.a.player1Capsule,Const.a.player1TargettingPos);
					PlaySightSound();
					return true;
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
		if (firstSighting && HasHealth(healthManager)) {
			firstSighting = false;
			if (!healthManager.actAsCorpseOnly) {
				SFXIndex = Const.a.sfxSightSoundForNPC[index];
				Utils.PlayOneShotSavable(SFX,SFXIndex);	
			}
		}
	}
	
	void enemyInFrontChecks(GameObject target) {
		if (target == null) {
			infront = false;
			inProjFOV = false;
			return;
		}

        infrontVec = target.transform.position;
		infrontVec.y = sightPoint.transform.position.y; // Ignore height delta.
		infrontVec = Vector3.Normalize(infrontVec - sightPoint.transform.position);
		inProjFOV = false;
		infront = false;
        dotResult = Vector3.Dot(infrontVec,sightPoint.transform.forward);
        if (dotResult > 0.800) {
			inProjFOV = true; // Enemy is within ±18° of forward facing vector.
		}

        if (dotResult > 0.300) {
			infront = true;   // Enemy is within ±63° of forward facing vector.
		}
    }

	public void Alert(UseData ud) {
		enemy = Const.a.player1Capsule;
		if (enemy != null) enemyHM = enemy.GetComponent<HealthManager>();
	}

	public void AwakeFromSleep(UseData ud) {
		asleep = false;
		if (ud != null) Alert(ud);
	}

	public static string Save(GameObject go, PrefabIdentifier prefID) {
		AIController aic = go.GetComponent<AIController>();
		if (aic == null) {
			Debug.Log("AIController missing on savetype of NPC!  GameObject.name: " + go.name);
			return "";
		}

		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		if (!aic.startInitialized) aic.Start();
		s1.Append(Utils.UintToString(aic.index,"AIController.index"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(Utils.AIStateToInt(aic.currentState),
														"currentState"));
		s1.Append(Utils.splitChar);

		//UPDATE: For coop, pick one of 1,2,3,4
		if (aic.enemy != null) s1.Append("enemID:1"); // enemID (savefile variable only)
		else s1.Append("enemID:-1");

		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.walkPathOnStart,"walkPathOnStart"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.dontLoopWaypoints,"dontLoopWaypoints"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.visitWaypointsRandomly,"visitWaypointsRandomly"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.actAsTurret,"actAsTurret"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveString(aic.targetID,"targetID"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.hasTargetIDAttached));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.idleTime));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.attack1SoundTime));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.attack2SoundTime));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.attack3SoundTime));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.gracePeriodFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.meleeDamageFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.inSight));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.infront));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.inProjFOV));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.LOSpossible)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.goIntoPain)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.rangeToEnemy));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.firstSighting)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.dyingSetup)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.ai_dying)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.ai_dead)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(aic.currentWaypoint)); // int
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.currentDestination.x));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.currentDestination.y));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.currentDestination.z));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.timeTillEnemyChangeFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.timeTillDeadFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.timeTillPainFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.tickFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.raycastingTickFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.huntFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.hadEnemy)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.lastKnownEnemyPos.x));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.lastKnownEnemyPos.y));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.lastKnownEnemyPos.z));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.tempVec.x));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.tempVec.y));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.tempVec.z));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.shotFired)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.randomWaitForNextAttack1Finished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.randomWaitForNextAttack2Finished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.randomWaitForNextAttack3Finished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.idealTransformForward.x));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.idealTransformForward.y));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.idealTransformForward.z));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.idealPos.x));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.idealPos.y));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.idealPos.z));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.attackFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.attack2Finished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.attack3Finished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.targettingPosition.x));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.targettingPosition.y));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(aic.targettingPosition.z));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.deathBurstFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.deathBurstDone)); // bool
		if (aic.deathBurst != null) {
			s1.Append(Utils.splitChar);
			s1.Append(Utils.BoolToString(aic.deathBurst.activeSelf));
			s1.Append(Utils.splitChar);
			s1.Append(Utils.UintToString(aic.deathBurst.transform.childCount));
			for (int i=0;i<aic.deathBurst.transform.childCount;i++) {
				Transform childTR = aic.deathBurst.transform.GetChild(i);
				s1.Append(Utils.splitChar);
				s1.Append(Utils.BoolToString(childTR.gameObject.activeSelf));
				for (int j=0;j<childTR.childCount;j++) {
					s1.Append(Utils.splitChar);
					s1.Append(Utils.BoolToString(childTR.transform.GetChild(j).gameObject.activeSelf));
				}
			}
		}

		s1.Append(Utils.splitChar);
		if (aic.visibleMeshEntity != null) {
			s1.Append(Utils.BoolToString(aic.visibleMeshEntity.activeSelf,
										 "visibleMeshEntity.activeSelf"));
		} else {
			s1.Append(Utils.BoolToString(false,
										 "visibleMeshEntity.activeSelf"));
		}

		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.asleep)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.tranquilizeFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.hopDone)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(aic.wandering)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(aic.wanderFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(aic.SFXIndex));
		if (aic.searchColliderGO != null) {
			s1.Append(Utils.splitChar);
			s1.Append(Utils.BoolToString(aic.searchColliderGO.activeSelf,
										 "searchColliderGO.activeSelf"));
			s1.Append(Utils.splitChar);
			s1.Append(SearchableItem.Save(aic.searchColliderGO,prefID));

			// Weird exclusion for hopper to not check 
			// && !healthManager.vaporizeCorpse
            if (!aic.healthManager.gibOnDeath
				|| prefID.constIndex == 421 /* avian mutant */) {

				s1.Append(Utils.splitChar);
				s1.Append(HealthManager.Save(aic.searchColliderGO,prefID));
			}
		}
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index,
						   PrefabIdentifier prefID) {
		AIController aic = go.GetComponent<AIController>();
		if (aic == null) {
			Debug.Log("AIController.Load failure, aic == null");
			return index + 55;
		}

		if (index < 0) {
			Debug.Log("AIController.Load failure, index < 0");
			return index + 55;
		}

		if (entries == null) {
			Debug.Log("AIController.Load failure, entries == null");
			return index + 55;
		}

		aic.Start();
		float readFloatx, readFloaty, readFloatz;
		aic.index = Utils.GetIntFromString(entries[index],"AIController.index");
		index++;

		int state = Utils.GetIntFromString(entries[index],"currentState"); index++;
		aic.currentState = Utils.GetAIStateFromInt(state);
		int enemIDRead = Utils.GetIntFromString(entries[index],"enemID"); index++;
		if (enemIDRead >= 0) aic.enemy = Const.a.player1Capsule;
		else aic.enemy = null;

		aic.walkPathOnStart = Utils.GetBoolFromString(entries[index],"walkPathOnStart"); index++;
		aic.dontLoopWaypoints = Utils.GetBoolFromString(entries[index],"dontLoopWaypoints"); index++;
		aic.visitWaypointsRandomly = Utils.GetBoolFromString(entries[index],"visitWaypointsRandomly"); index++;
		aic.actAsTurret = Utils.GetBoolFromString(entries[index],"actAsTurret"); index++;
		aic.targetID = Utils.LoadString(entries[index],"targetID"); index++;
		aic.hasTargetIDAttached = Utils.GetBoolFromString(entries[index]); index++;
		aic.idleTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		aic.attack1SoundTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		aic.attack2SoundTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		aic.attack3SoundTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		aic.gracePeriodFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before applying pain damage on attack2
		aic.meleeDamageFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before applying pain damage on attack2
		aic.inSight = Utils.GetBoolFromString(entries[index]); index++; // bool
		aic.infront = Utils.GetBoolFromString(entries[index]); index++; // bool
		aic.inProjFOV = Utils.GetBoolFromString(entries[index]); index++; // bool
		aic.LOSpossible = Utils.GetBoolFromString(entries[index]); index++; // bool
		aic.goIntoPain = Utils.GetBoolFromString(entries[index]); index++; // bool
		aic.rangeToEnemy = Utils.GetFloatFromString(entries[index]); index++; // float
		aic.firstSighting = Utils.GetBoolFromString(entries[index]); index++; // bool - or are we dead?
		aic.dyingSetup = Utils.GetBoolFromString(entries[index]); index++; // bool - or are we dead?
		aic.ai_dying = Utils.GetBoolFromString(entries[index]); index++; // bool - are we dying the slow painful death
		aic.ai_dead = Utils.GetBoolFromString(entries[index]); index++; // bool - or are we dead?
		aic.currentWaypoint = Utils.GetIntFromString(entries[index]); index++; // int
		readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
		aic.currentDestination = new Vector3(readFloatx,readFloaty,readFloatz);
		aic.timeTillEnemyChangeFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.timeTillDeadFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.timeTillPainFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.raycastingTickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.huntFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.hadEnemy = Utils.GetBoolFromString(entries[index]); index++; // bool
		readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
		aic.lastKnownEnemyPos = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
		aic.tempVec = new Vector3(readFloatx,readFloaty,readFloatz);
		aic.shotFired = Utils.GetBoolFromString(entries[index]); index++; // bool
		aic.randomWaitForNextAttack1Finished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.randomWaitForNextAttack2Finished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.randomWaitForNextAttack3Finished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
		aic.idealTransformForward = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
		aic.idealPos = new Vector3(readFloatx,readFloaty,readFloatz);
		aic.attackFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.attack2Finished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.attack3Finished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
		readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
		aic.targettingPosition = new Vector3(readFloatx,readFloaty,readFloatz);
		aic.deathBurstFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.deathBurstDone = Utils.GetBoolFromString(entries[index]); index++; // bool
		if (aic.deathBurst != null) {
			aic.deathBurst.SetActive(Utils.GetBoolFromString(entries[index])); index++;
			int numChildrenFromSave = Utils.GetIntFromString(entries[index]); index++;
			if (numChildrenFromSave != aic.deathBurst.transform.childCount) Debug.Log("BUG: Number of deathBurst children in save does not match prefab on " + aic.gameObject.name);
			for (int i=0;i<aic.deathBurst.transform.childCount;i++) {
				Transform childTR = aic.deathBurst.transform.GetChild(i);
				childTR.gameObject.SetActive(Utils.GetBoolFromString(entries[index])); index++;
				for (int j=0;j<childTR.childCount;j++) {
					childTR.GetChild(j).gameObject.SetActive(Utils.GetBoolFromString(entries[index])); index++;	
				}
			}
		}

		if (aic.visibleMeshEntity != null) {
			aic.visibleMeshEntity.SetActive(Utils.GetBoolFromString(entries[index],
											"visibleMeshEntity.activeSelf"));
		}
		index++;

		aic.asleep = Utils.GetBoolFromString(entries[index]); index++; // bool - are we sleepnir? vague reference alert
		aic.tranquilizeFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.hopDone = Utils.GetBoolFromString(entries[index]); index++; // bool
		aic.wandering = Utils.GetBoolFromString(entries[index]); index++; // bool
		aic.wanderFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
		aic.SFXIndex = Utils.GetIntFromString(entries[index]); index++;
		if (aic.healthManager != null) {
			if (aic.healthManager.health > 0) {
				Utils.Activate(aic.visibleMeshEntity);
				Utils.EnableCollision(aic.gameObject);
				aic.gameObject.layer = 10; // NPC Layer.
				if (Const.a.moveTypeForNPC[aic.index] != AIMoveType.Fly) {
					aic.rbody.useGravity = true;
					aic.rbody.isKinematic = false;
				}
			} else {
				// Sky layer only collides with Geometry. This prevents the NPC
				// falling out of the world.
				aic.gameObject.layer = 15;
				if (Const.a.moveTypeForNPC[aic.index] != AIMoveType.Fly) {
					aic.rbody.useGravity = true;
					aic.rbody.isKinematic = false;
				}

				if (!aic.rbody.freezeRotation) aic.rbody.freezeRotation = true;
				if (aic.healthManager.gibOnDeath
					|| aic.healthManager.teleportOnDeath || aic.IsCyberNPC()) {
					aic.rbody.useGravity = false;
					if (aic.healthManager.teleportOnDeath) {
						aic.rbody.useGravity = true;
					}

					Utils.Deactivate(aic.visibleMeshEntity);
				} else aic.rbody.useGravity = true;
			}
		}

		if (aic.searchColliderGO != null) {
			aic.searchColliderGO.SetActive(Utils.GetBoolFromString(entries[index],
										   "searchColliderGO.activeSelf"));
			index++;
			index = SearchableItem.Load(aic.searchColliderGO, ref entries,
										index,prefID);

			if (!aic.healthManager.gibOnDeath
				|| prefID.constIndex == 421 /* avian mutant */) {

				index = HealthManager.Load(aic.searchColliderGO, ref entries,
										   index,prefID);
			}
		}

		if (aic.currentState == AIState.Attack1) {
			if (Const.a.preactivateMeleeCollidersForNPC[aic.index]) {
				aic.PreActivateMeleeColliders();
			} else {
				aic.DeactivateMeleeColliders();
			}
		} else {
			aic.DeactivateMeleeColliders();
		}
		return index;
	}
}
