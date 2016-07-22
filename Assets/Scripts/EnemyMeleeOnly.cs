using UnityEngine;
using System.Collections;

public class EnemyMeleeOnly : MonoBehaviour {
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
	private NavMeshAgent nav;
	private EnemyHealth enemyHealth;
	private EnemySight enemySight;
	private Animator anim;
	private bool firstSighting;
	private AnimatorStateInfo currentBaseState;
	private static int deadState = Animator.StringToHash("Dead");

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

	void MeleeAttack() {
		playerHealth.TakeDamage(meleeDamageAmount * (Random.Range(0.7f,1.0f)));  // 10.5 to 15 damage
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
	void drawMyLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		StartCoroutine( drawLine(start, end, color, duration));
	}

	IEnumerator drawLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		GameObject myLine = new GameObject ();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer> ();
		LineRenderer lr = myLine.GetComponent<LineRenderer> ();
		lr.material = new Material (Shader.Find ("Particles/Additive"));
		lr.SetColors (color,color);
		lr.SetWidth (0.1f,0.1f);
		lr.SetPosition (0, start);
		lr.SetPosition (1, end);
		yield return new WaitForSeconds(duration);
		GameObject.Destroy (myLine);
	}
}
