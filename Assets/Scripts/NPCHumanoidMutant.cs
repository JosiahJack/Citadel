using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCHumanoidMutant : MonoBehaviour {
	public float fireSpeed = 1.5f;
	public float meleeDamageAmount = 15f;
	public float sightrange = 20f;
	public float range = 2f;
	public float roamingSpeed = 1.5f;
	public float chaseSpeed = 1.5f;
	public float searchSpeed = 1.5f;
	public float attackSpeed = 0.5f;
	public float roamMinWaitTime = 1f;
	public float stopAtPointTime = 1f;
	public float percentageOfTimeToShoot = 1.0f;
	public float timeBeforeAttack = 1f;
	public float fieldOfViewAngle = 110f;
	public float startingHealth = 2f;
	public float health;
	public float deathTime = 0.1f;
	public float delayBeforeHit = 0.2f;
	public float distToSeeWhenBehind = 2.5f;
	public bool inRange = false;
	public bool playerInSight = false;
	public bool playerInSightOnLastFrame = false;
	public bool chasing = false;
	public bool attacking = false;
	public bool searching = false;
	public bool roaming = false;
	public bool idle = false;
	public bool inspecting = false;
	public int index = 0;
	public enum AIActBusyType {AB_Idle,AB_Roaming,AB_Patrolling,AB_Monitoring,AB_Inspecting};
	public AIActBusyType currentActBusy = AIActBusyType.AB_Idle;
	public Vector3 lastKnownPosition;
	public Vector3 resetPosition;
	public Transform[] roamingWaypoints; // point(s) for NPC to walk to when roaming or patrolling
	public Transform[] inspectionPoints; // point(s) for NPC to inspect when monitoring an area, uses point [0] if inspecting
	public AudioClip SFXDeathClip;
	public AudioClip SFXIdleClip;
	public AudioClip SFXPainClip;
	public AudioClip SFXSightSound;
	private int wayPointIndex;
	private int inspectionIndex;
	//private static int deadState = Animator.StringToHash("Dead");
	//private static int idleState = Animator.StringToHash("Idle");
	private float waitTime = 0f;
	private float waitTilNextFire = 0f;
	private float idleTime;
	private float painTime;
	private float dyingTime;
	private bool isDead;
	private bool isDying;
	private bool firstSighting = true;
	//private bool firstStateFrame = true;
	private PlayerHealth playerHealth;
	private UnityEngine.AI.NavMeshAgent nav;
	private Animator anim;
	private AnimatorStateInfo currentBaseState;
	private GameObject player;
	private AudioSource playerSound;
	private SphereCollider col;
	private AudioSource SFX;
	private Rigidbody rbody;
	private CapsuleCollider capCol;

	void Awake () {
		nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
		waitTilNextFire = 0;
		waitTime = Time.time;
		anim = GetComponent<Animator>();
		wayPointIndex = roamingWaypoints.Length - 1;
		inspectionIndex = inspectionPoints.Length;
		firstSighting = true;
		col = GetComponent<SphereCollider>();
		capCol = GetComponent<CapsuleCollider>();
		player = GameObject.FindGameObjectWithTag("Player");
		playerSound = player.GetComponent<AudioSource>();
		playerHealth = player.GetComponent<PlayerHealth>();
		resetPosition = new Vector3(0f,-100000f,0f);
		lastKnownPosition = resetPosition;
		SFX = GetComponent<AudioSource>();
		rbody = GetComponent<Rigidbody>();
		health = startingHealth;
		dyingTime = Time.time;
		chasing = false;
		attacking = false;
		searching = false;
		roaming = false;
		idle = false;
		inspecting = false;
	}

	public void TakeDamage (DamageData dd) {
		if (health <= 0f)
			return;

		dd.defense = Const.a.defenseForNPC[index];
		dd.armorvalue = Const.a.armorvalueForNPC[index];
		dd.other = gameObject;
		float take = Const.a.GetDamageTakeAmount(dd);
		health -= take;
		if (health <= 0f) {
			health = 0f;
			deathTime = Time.time + deathTime;
			isDying = true;
			Dying();
		} else {
			if (painTime < Time.time && SFXPainClip != null) {
				SFX.PlayOneShot(SFXPainClip);
				painTime = Time.time + Random.Range(1f,3f);
			}
		}
	}

	void Update () {
		//AnimatorStateInfo nextState = anim.GetNextAnimatorStateInfo(0);
		//if (nextState.fullPathHash != deadState) {
		//	anim.SetBool("Dead",false);
		//}
		if (PauseScript.a != null && PauseScript.a.paused) {
			anim.speed = 0f;
			nav.Stop();
			return;
		} else {
			anim.speed = 1f;
			if (!idle && !inspecting)
				nav.Resume();
		}

		attacking = false;
		searching = false;
		roaming = false;
		idle = false;
		inspecting = false;

		if (health <= 0f) {
			isDying = true;
			dyingTime = Time.time + deathTime;
		}
		if (isDead) {
			Dead();
			return;
		}
		if (isDying) {
			Dying();
			return;
		}

		CheckIfPlayerInSight();

		if (chasing) {
			Chasing();
			return;
		}
			
		if (lastKnownPosition != resetPosition) {
			Searching();
			return;
		}

		AI_ActBusy();
	}

	void AI_ActBusy () {
		if (idleTime < Time.time && SFXIdleClip != null) {
				SFX.PlayOneShot(SFXIdleClip);
				idleTime = Time.time + Random.Range(3f,10f);
		}

		switch (currentActBusy) {
		case AIActBusyType.AB_Idle:
			Idle();
			break;
		case AIActBusyType.AB_Inspecting:
			Inspecting();
			break;
		case AIActBusyType.AB_Monitoring:
			Monitoring();
			break;
		case AIActBusyType.AB_Patrolling:
			Patrolling();
			break;
		case AIActBusyType.AB_Roaming:
			Roaming();
			break;
		}
	}

	void Idle () {
		// hands are of the devil, or SHODAN
		// NPC is standing around
		nav.Stop();
		anim.Play("Idle");
		idle = true;
		//if (firstStateFrame) {
		//	firstStateFrame = false;
		//	anim.Play("Idle");
		//}
	}

	void Inspecting () {
		// NPC is fiddling with or staring intently at item of interest
		if (inspectionPoints[0] != null) {
			inspectionIndex++;
			if (inspectionIndex > inspectionPoints.Length)
				inspectionIndex = 0;

			if (!(CheckIfInRangeOfWaypoint(inspectionPoints[inspectionIndex]))) {
				nav.SetDestination(inspectionPoints[inspectionIndex].position);
				nav.Resume();
			} else {
				nav.Stop(); // Time to look at the inspection point
				// TODO rotate to look at item of interest
				anim.Play("Inspecting");
				inspecting = true;
			}
		} else {
			currentActBusy = AIActBusyType.AB_Idle;
		}
	}

	void Monitoring () {
		// NPC is walking from point of interest to point of interest
		if (inspectionPoints.Length < 1) {
			currentActBusy = AIActBusyType.AB_Idle;
			return; // Nothing to monitor, default to idle
		}

		nav.speed = roamingSpeed;
		if (inspectionPoints.Length == 1) {
			currentActBusy = AIActBusyType.AB_Inspecting;
			Inspecting(); // Only one point of interest to monitor, technically inspecting
		}

		// Walk to next point
		// Inspect point for time + random factor, play one of NPC's inspection animations
		// Repeat
		anim.Play("Inspecting");
		inspecting = true;
	}

	void Patrolling() {
		// NPC is patrolling from waypoint to waypoint
		if (roamingWaypoints.Length < 1) {
			currentActBusy = AIActBusyType.AB_Idle;
			return; // Nothing to walk to, default to idle
		}

		nav.speed = roamingSpeed;
		if (roamingWaypoints.Length == 1) {
			// walk to roamingWaypoint[0]
			if (!(CheckIfInRangeOfWaypoint(roamingWaypoints[0]))) {
				nav.SetDestination(roamingWaypoints[0].position);
				nav.Resume();
				anim.Play("Walk");
				roaming = true;
			} else {
				currentActBusy = AIActBusyType.AB_Idle;
				return;
			}
		}

		if (roamingWaypoints.Length > 1) {
			wayPointIndex++;
			if (wayPointIndex >= roamingWaypoints.Length)
				wayPointIndex = 0;

			if (!(CheckIfInRangeOfWaypoint(roamingWaypoints[wayPointIndex]))) {
				nav.SetDestination(roamingWaypoints[wayPointIndex].position);
				nav.Resume();
			}
		}
	}

	void Roaming () {
		nav.speed = roamingSpeed;
		if (wayPointIndex >= roamingWaypoints.Length)
			wayPointIndex = 0;
		
		if (waitTime < Time.time) {
			if (!(CheckIfInRangeOfWaypoint(roamingWaypoints[wayPointIndex]))) {
				float dist = 2f;
				Vector3 randomDirection = Random.insideUnitSphere * dist;
				randomDirection += transform.position;

				NavMeshHit navHit;
				if (NavMesh.SamplePosition (randomDirection,out navHit, dist,NavMesh.AllAreas)) {
					//nav.SetDestination(roamingWaypoints[wayPointIndex].position);
					nav.SetDestination(navHit.position);
					nav.Resume();
					anim.Play("Walk");
					roaming = true;
				} else {
					nav.Stop();
					anim.Play("Idle");
					waitTime = Time.time + stopAtPointTime + Random.Range(0f, 1f);
					wayPointIndex++;
					roaming = false;
				}
			} else {
				nav.Stop();
				anim.Play("Idle");
				waitTime = Time.time + stopAtPointTime + Random.Range(0f, 1f);
				wayPointIndex++;
			}
		}
	}

	void Chasing () {
		if (playerHealth.health <= 0) {
			playerInSight = false;
			return;
		}

		if (!playerInSight) {
			lastKnownPosition = player.transform.position;
			return;
		}

		if (GetRange() < 2f) {
			inRange = true;
		} else {
			inRange = false;
		}

		if (inRange && playerInSight) {
			MeleeAttack();
			return;
		}
		
		nav.speed = chaseSpeed;
		nav.SetDestination(player.transform.position);
		nav.Resume();
		anim.Play("Run");
		chasing = true;
		if (firstSighting) {
			firstSighting = false;
			SFX.PlayOneShot(SFXSightSound);
			waitTilNextFire = Time.time + timeBeforeAttack; // delay before attack is added
		}

	}

	DamageData SetDamageData (int attackIndex) {
		if (index < 0 || index > 23) {
			Debug.Log("BUG: index set incorrectly on NPC EnemyMelee.  Not 0 to 23. Disabled.");
			gameObject.SetActive(false);
		}
		DamageData dd = new DamageData(); 
		// Attacker (self [a]) data
		dd.owner = gameObject;
		switch (attackIndex) {
		case 0:
			dd.damage = Const.a.damageForNPC[index];
			break;
		case 1:
			dd.damage = Const.a.damageForNPC2[index];
			break;
		case 2:
			dd.damage = Const.a.damageForNPC3[index];
			break;
		}
		dd.penetration = 0;
		dd.offense = 0;
		return dd;
	}

	void MeleeAttack() {
		if (waitTilNextFire <= Time.time) {
			DamageData dd = new DamageData();
			dd = SetDamageData(0); // set damage data for primary attack
			dd.attackType = Const.AttackType.Melee;
			dd.damage *= Random.Range(Const.a.randomMinimumDamageModifierForNPC[index],1.0f); // Add randomization for damage of minimum% to 100%
			playerHealth.TakeDamage(dd);
			waitTilNextFire = Time.time + fireSpeed;
		}
		//anim.SetBool("PlayerInRange",true);
		attacking = true;
		anim.Play("Attack");
		nav.Resume();
		nav.speed = attackSpeed;
		nav.SetDestination(player.transform.position);
	}

	void Searching () {
		nav.speed = searchSpeed;
		nav.SetDestination(lastKnownPosition);
		anim.Play("Run");
		nav.Resume();
		if (CheckIfInRangeOfPoint(lastKnownPosition)) {
			lastKnownPosition = resetPosition;
			currentActBusy = AIActBusyType.AB_Roaming;
		}
	}

	void Dying() {
		if (dyingTime < Time.time) {
			isDead = true;
			isDying = false;
			SFX.PlayOneShot(SFXDeathClip);
			gameObject.tag = "Searchable";
			gameObject.layer = 13;
			rbody.isKinematic = true;
			capCol.enabled = false;
			col.enabled = true;
			col.isTrigger = false;
			col.radius = 0.3f;
			col.transform.position = transform.position;
			anim.Play("Death");
			nav.speed = nav.speed * 0.5f;
		}
		//anim.SetBool("Dead",true);
	}

	void Dead () {
		// NPC is dead
		isDead = true;
		isDying = false;
		nav.Stop();
		gameObject.tag = "Searchable";
	}

	void CheckIfPlayerInSight () {
		Vector3 checkline = player.transform.position - transform.position;
		float dist = Vector3.Distance(player.transform.position,transform.position);
		float angle = Vector3.Angle(checkline,transform.forward);
		if (angle < fieldOfViewAngle * 0.5f) {
			RaycastHit hit;
			if(Physics.Raycast(transform.position + transform.up, checkline.normalized, out hit, sightrange)) {
				if (hit.collider.gameObject == player) {
					playerInSight = true;
					lastKnownPosition = player.transform.position;
					return;
				}
			}
			if (dist < distToSeeWhenBehind) {
				playerInSight = true;
				lastKnownPosition = player.transform.position;
				return;
			}
			return;
		}



		if (playerSound.isPlaying && dist < 8f) {
			lastKnownPosition = player.transform.position;
			return;
		}

		playerInSight = false;
	}

	float GetRange () { return (Vector3.Distance(player.transform.position,transform.position)); }

	bool CheckIfInRangeOfPoint (Vector3 dest) {
		if (Vector3.Distance(transform.position,dest) < 1f) {
			return true;
		} else {
			return false;
		}
	}

	bool CheckIfInRangeOfWaypoint (Transform waypoint) {
		if (waypoint != null) {
			if (Vector3.Distance(transform.position,waypoint.position) < 1f) {
				return true;
			} else {
				return false;
			}
		}
		return false;
	}
}
