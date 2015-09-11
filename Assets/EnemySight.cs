using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour {
	public float fieldOfViewAngle = 110f;
	public bool playerInSight;
	public Vector3 lastKnownPosition;
	public Vector3 resetPosition;
	private NavMeshAgent nav;
	private GameObject player;
	private AudioSource playerSound;
	private SphereCollider col;
	private PlayerHealth playerHealth;
	private Vector3 previousFrameSighting;

	void Awake () {
		nav = GetComponent<NavMeshAgent>();
		col = GetComponent<SphereCollider>();
		player = GameObject.FindGameObjectWithTag("Player");
		playerSound = player.GetComponent<AudioSource>();
		playerHealth = player.GetComponent<PlayerHealth>();
		resetPosition = new Vector3(0f,-100000f,0f);
		previousFrameSighting = resetPosition;
		lastKnownPosition = resetPosition;
	}

	void OnTriggerStay (Collider other) {
		if (other.gameObject == player) {
			playerInSight = false;
			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction,transform.forward);
			if (angle < fieldOfViewAngle * 0.5f) {
				RaycastHit hit;
				if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius)) {
					if (hit.collider.gameObject == player) {
						playerInSight = true;
						lastKnownPosition = player.transform.position;
					}
				}
			}
			if (direction.magnitude < 3f) {
				playerInSight = true;
				lastKnownPosition = player.transform.position;
			}
			if (playerSound.isPlaying) {
				lastKnownPosition = player.transform.position;
			}
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.gameObject == player) {
			playerInSight = false;
		}
	}
}
