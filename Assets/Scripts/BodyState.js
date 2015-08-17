var floatAbove : float = 0.08;
var playerCapsuleTransform : Transform;
//@HideInInspector
var collisionDetected : boolean = false;
var collisionDebugAll : boolean = false;
//@HideInInspector
var colliderHeight : float;

function Start () {
	colliderHeight = playerCapsuleTransform.GetComponent(Collider).height;
}

function FixedUpdate () {
	//transform.position.x = playerCapsuleTransform.position.x;
	transform.position.y = playerCapsuleTransform.position.y + ((colliderHeight * playerCapsuleTransform.localScale.y)/2) + (transform.localScale.y/2) + floatAbove;
	//transform.position.z = playerCapsuleTransform.position.z;
}

function OnCollisionStay (collisionInfo : Collision) {
	collisionDebugAll = true;
	if (collisionInfo.gameObject.tag == "Geometry") {
		collisionDetected = true;
	}
}

function OnCollisionExit () {
	collisionDetected = false;
}