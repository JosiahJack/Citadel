var floatAbove : float = 0.04;
var playerCapsuleTransform : Transform;
//@HideInInspector
var collisionDetected : boolean = false;

function Update () {
	transform.position.x = playerCapsuleTransform.position.x;
	transform.position.y = playerCapsuleTransform.position.y + ((playerCapsuleTransform.GetComponent(Collider).height * playerCapsuleTransform.localScale.y) / 2) + (transform.localScale.y/2) + floatAbove;
	transform.position.z = playerCapsuleTransform.position.z;
}

function OnCollisionStay (collisionInfo : Collision) {
	if (collisionInfo.gameObject.tag == "Geometry") {
		collisionDetected = true;
	}
}

function OnCollisionExit () {
	collisionDetected = false;
}