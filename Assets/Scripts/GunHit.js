#pragma strict

var maxDist : float = 100000000000;
var decalHitWall : GameObject;
var floatInFrontOfWall : float = 0.02;

function Update () {
	var hit : RaycastHit;
	if (Physics.Raycast(transform.position, transform.forward, hit, maxDist)) {
		if (decalHitWall && hit.transform.tag == "Geometry") {
			Instantiate(decalHitWall, hit.point + (hit.normal * floatInFrontOfWall), Quaternion.LookRotation(hit.normal));
		}
	}
	Destroy(gameObject);
}