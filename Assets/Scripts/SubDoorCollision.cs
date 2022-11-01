using UnityEngine;
using System.Collections;

public class SubDoorCollision : MonoBehaviour {
	private GameObject parent;
	private Door parentDoorScript;

	void Awake () {
		parent = transform.parent.gameObject;
		parentDoorScript = parent.GetComponent<Door>();
	}

	void OnCollisionEnter () {
		if (parentDoorScript.doorOpen == DoorState.Closing)
			parentDoorScript.blocked = true; //only block the door while closing
	}

	void OnCollisionExit () {
		parentDoorScript.blocked = false;
	}
}
