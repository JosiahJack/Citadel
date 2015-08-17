#pragma strict

var health : float = 255f;
var resetAfterDeathTime : float = 1f;
var timer : float;
static var playerDead : boolean;
var PainSFX : AudioSource;
var PainSFXClip : AudioClip;
var cameraObject : GameObject;

function Update () {
	if (health <= 0) {
		if (!playerDead) {
			PlayerDying();
		} else {
			PlayerDead();
		}
	}
}
	
function PlayerDying () {
		timer += Time.deltaTime;

		if (timer >= resetAfterDeathTime) {
			playerDead = true;
		}
}

function PlayerDead () {
	//gameObject.GetComponent(PlayerMovement).enabled = false;
	//cameraObject.SetActive(false);
	cameraObject.GetComponent(Camera).enabled = false;
	Cursor.lockState = CursorLockMode.None;
}

function TakeDamage(take : float) {
	health -= take;
	PainSFX.PlayOneShot(PainSFXClip);
	print("Player Health: " + health.ToString());
}