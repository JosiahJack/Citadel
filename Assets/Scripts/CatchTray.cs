using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchTray : MonoBehaviour {
	void  OnCollisionEnter (Collision col){
		Debug.Log("WARNING: Item of name " + col.gameObject.name + " fell out of level at " + col.gameObject.transform.position.ToString());
	}
}