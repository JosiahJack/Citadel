using UnityEngine;
using System.Collections;

public class EnemyProjectileOnly : MonoBehaviour {
	public GameObject bullet1;
	public float fireOffset = 0.32f;
	public float fireSpeed = 1f;
	public float shotSpeed = 50f;
	public float verticalShotOffset = 0.16f;
	private bool sighted;
	private float waitTilNextFire = 0f;
	private Vector3 lastKnownPosition;
	public Transform player;
	private PlayerHealth playerHealth;
	private NavMeshAgent nav;
	private EnemyHealth enemyHealth;

	void Awake () {
		playerHealth = player.GetComponent<PlayerHealth>();
		nav = GetComponent<NavMeshAgent>();
		enemyHealth = GetComponent<EnemyHealth>();
		lastKnownPosition = transform.position;
		waitTilNextFire = 0;
	}

	void Update () {
		if((playerHealth.health > 0) && (enemyHealth.health > 0)) {
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(gameObject.transform.position, player.position - gameObject.transform.position, out hit, 200f)) {
				Debug.DrawLine (gameObject.transform.position, hit.point, Color.green);
				if (hit.transform.gameObject.tag == "Player") {
					Vector3 direction = transform.position - hit.point;
					lastKnownPosition = player.position;
					nav.SetDestination(player.position);
					if (waitTilNextFire <= Time.time) {
						Vector3 verticalOffset = new Vector3(0f,verticalShotOffset,0f);  
						GameObject shot = (GameObject)Instantiate(bullet1,gameObject.transform.position + verticalOffset,Quaternion.identity);  //effect  //+(direction.normalized*fireOffset)
						Vector3 dir = Camera.main.transform.forward;
						shot.transform.rotation = Quaternion.LookRotation(-dir);
						shot.GetComponent<ProjectileEffectImpact>().host = gameObject;
						shot.GetComponent<Rigidbody>().AddForce(shot.transform.forward * shotSpeed);
						waitTilNextFire = Time.time + fireSpeed + Random.Range(0f,2f);
					}
				} else {
					nav.SetDestination(lastKnownPosition);
				}
			}
		} else {
			nav.enabled = false;
		}
	}
}
