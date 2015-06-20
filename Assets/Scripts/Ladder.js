#pragma strict
var other : GameObject;

function OnTriggerEnter(other : Collider) {
	if (other.tag == "Player") {
		other.GetComponent(PlayerMovement).ladderState = true;
	}
}

function OnTriggerExit(other : Collider) {
	if (other.tag == "Player") {
		other.GetComponent(PlayerMovement).ladderState = false;
	}
}