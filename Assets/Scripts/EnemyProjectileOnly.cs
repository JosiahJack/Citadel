using UnityEngine;
using System.Collections;

public class EnemyProjectileOnly : MonoBehaviour {
	public GameObject bullet1;
	public float fireOffset = 0.32f;
	public float fireSpeed = 1f;
	public float shotSpeed = 50f;
	public float roamingSpeed = 1.5f;
	public float chaseSpeed = 1.5f;
	public float searchSpeed = 1.5f;
	public float roamMinWaitTime = 1f;
	public float percentageOfTimeToShoot = 0.4f;
	public float verticalShotOffset = 0.16f;
	public bool stopsToShoot = true;
	public Transform player;
	public Transform[] roamingWaypoints;
	public float timeBeforeAttack = 1f;
	private bool sighted;
	private int wayPointIndex;
	private float waitTime = 0f;
	private float waitTilNextFire = 0f;
	private PlayerHealth playerHealth;
	private NavMeshAgent nav;
	private EnemyHealth enemyHealth;
	private EnemySight enemySight;
	private Animator anim;
	private bool firstSighting;

	void Awake () {
		enemySight = GetComponent<EnemySight>();
		playerHealth = player.GetComponent<PlayerHealth>();
		nav = GetComponent<NavMeshAgent>();
		enemyHealth = GetComponent<EnemyHealth>();
		waitTilNextFire = 0;
		anim = GetComponent<Animator>();
		wayPointIndex = roamingWaypoints.Length;
		firstSighting = true;
	}

	void Update () {
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
			}
		} else {
			nav.Stop();
		}
	}

	void ProjectileAttack (bool stopForShot) {
		Vector3 verticalOffset = new Vector3(0f,verticalShotOffset,0f);  
		GameObject shot = (GameObject)Instantiate(bullet1,gameObject.transform.position + verticalOffset,Quaternion.identity);  //effect  //+(direction.normalized*fireOffset)
		Vector3 dir = Camera.main.transform.forward;
		shot.transform.rotation = Quaternion.LookRotation(-dir);
		shot.GetComponent<ProjectileEffectImpact>().host = gameObject;
		shot.GetComponent<Rigidbody>().AddForce(transform.forward * shotSpeed);
		waitTilNextFire = Time.time + fireSpeed + Random.Range(0f,2f);

		//if (stopForShot)
		//	nav.Stop();
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

		if (waitTilNextFire <= Time.time && Random.Range(0f, 1f) < percentageOfTimeToShoot && !firstSighting) {
			anim.SetBool("ShouldAttack", true);
			ProjectileAttack(stopsToShoot);
		} else {
			anim.SetBool("ShouldAttack", false);
		}
	}

	void Roaming () {
		nav.speed = roamingSpeed;
		if (waitTime < Time.time) {
			if (wayPointIndex != 0)
				nav.SetDestination(roamingWaypoints[wayPointIndex].position);
		}
	}
}
