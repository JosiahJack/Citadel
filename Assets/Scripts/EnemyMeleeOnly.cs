using UnityEngine;
using System.Collections;

public class EnemyMeleeOnly : MonoBehaviour {
	public int index; // enemy reference index (0 through 23)
	public float fireSpeed = 1.5f;
	public float meleeDamageAmount = 15f;
	public float range = 2f;
	public bool inRange = false;
	public float roamingSpeed = 1.5f;
	public float chaseSpeed = 1.5f;
	public float searchSpeed = 1.5f;
	public float roamMinWaitTime = 1f;
	public float percentageOfTimeToShoot = 1.0f;
	public Transform player;
	public Transform[] roamingWaypoints;
	public float timeBeforeAttack = 1f;
	private bool sighted;
	private int wayPointIndex;
	private float waitTime = 0f;
	private float waitTilNextFire = 0f;
	private PlayerHealth playerHealth;
	private UnityEngine.AI.NavMeshAgent nav;
	private EnemyHealth enemyHealth;
	private EnemySight enemySight;
	private Animator anim;
	private bool firstSighting;
	private AnimatorStateInfo currentBaseState;
	private static int deadState = Animator.StringToHash("Dead");

	void Awake () {
		enemySight = GetComponent<EnemySight>();
		playerHealth = player.GetComponent<PlayerHealth>();
		nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
		enemyHealth = GetComponent<EnemyHealth>();
		waitTilNextFire = 0;
		anim = GetComponent<Animator>();
		wayPointIndex = roamingWaypoints.Length;
		firstSighting = true;
	}

	void Update () {
		AnimatorStateInfo nextState = anim.GetNextAnimatorStateInfo(0);
		if (nextState.fullPathHash != deadState) {
			anim.SetBool("Dead",false);
		}

		if (enemyHealth.health > 0f) {
			if(playerHealth.health > 0) {
				if (enemySight.playerInSight) {
					Chasing();
				} else {
					if (enemySight.lastKnownPosition != enemySight.resetPosition) {
						Searching();
					} else {
						Roaming();
					}
				}
			} else {
				Roaming();
			}
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
		DamageData dd = new DamageData();
		dd = SetDamageData(0); // set damage data for primary attack
		dd.attackType = Const.AttackType.Melee;
		dd.damage *= Random.Range(Const.a.randomMinimumDamageModifierForNPC[index],1.0f); // Add randomization for damage of minimum% to 100%
		playerHealth.TakeDamage(dd);
		anim.SetBool("PlayerInRange",true);
		waitTilNextFire = Time.time + fireSpeed + Random.Range(0f,2f);
	}

	void Searching () {
		nav.speed = searchSpeed;
		nav.SetDestination(enemySight.lastKnownPosition);
		Vector3 deltaPos = enemySight.lastKnownPosition - transform.position;
		if (deltaPos.magnitude <= nav.stoppingDistance)
			enemySight.lastKnownPosition = enemySight.resetPosition;
	}

	void Chasing () {
		nav.speed = chaseSpeed;
		nav.SetDestination(player.position);
		if (firstSighting) {
			firstSighting = false;
			waitTilNextFire = Time.time + timeBeforeAttack;
		}
			
		if (GetRange() < 1) {
			inRange = true;
		} else {
			inRange = false;
		}
		
		if (waitTilNextFire <= Time.time && Random.Range(0f, 1f) < percentageOfTimeToShoot && !firstSighting && inRange)
			MeleeAttack();
	}

	void Roaming () {
		nav.speed = roamingSpeed;
		if (waitTime < Time.time) {
			if (wayPointIndex != 0)
				nav.SetDestination(roamingWaypoints[wayPointIndex].position);
		}
	}
	
	float GetRange () {
		float retval = Vector3.Distance(player.transform.position,transform.position);
		return retval;
	}
}
