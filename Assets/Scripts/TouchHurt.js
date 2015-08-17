#pragma strict

var damage : float = 1; // assign in the editor

function OnCollisionEnter(col : Collision) {
	if (col.gameObject.tag == "Player") {
		col.gameObject.GetComponent(PlayerHealth).TakeDamage(damage);
	}
}