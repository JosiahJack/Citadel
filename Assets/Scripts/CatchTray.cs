using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used in conjunction with the ObjectContainmentSystem, recovers fallen
// objects and returns them to nearest floor's center point which is the
// same as the center of that valid open grid cell.  Nearness is x,y in
// the flat plane of the world grid, ignoring height z (Unity's y).
public class CatchTray : MonoBehaviour {
	void  OnCollisionEnter (Collision col){
		Debug.Log("WARNING: Item of name "
				  + col.gameObject.name
				  + " fell out of level at "
				  + col.gameObject.transform.position.ToString());
		Rigidbody rbody = col.gameObject.GetComponent<Rigidbody>();
		rbody.velocity = Vector3.zero;
		float xpos = col.gameObject.transform.position.x;
		float ypos = col.gameObject.transform.position.z;
		col.gameObject.transform.position = ObjectContainmentSystem.FindNearestFloor(xpos,ypos,col.gameObject.transform.position.y);
	}
}