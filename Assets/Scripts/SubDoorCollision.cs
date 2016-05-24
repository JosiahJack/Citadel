using UnityEngine;
using System.Collections;

public class SubDoorCollision : MonoBehaviour {
	private GameObject parent;
	private Door1 parentDoorScript;
	enum doorState {Closed, Open, Closing, Opening};

	void Awake () {
		parent = transform.parent.gameObject;
		parentDoorScript = parent.GetComponent<Door1>();
	}

	void OnCollisionEnter () {
		if (parentDoorScript.doorOpen == (int) doorState.Closing)
			parentDoorScript.blocked = true; //only block the door while closing
	}

	void OnCollisionExit () {
		parentDoorScript.blocked = false;
	}
}
