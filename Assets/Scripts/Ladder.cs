using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {
	
	GameObject other;
	
	void  OnTriggerEnter (Collider other){
		if (other.tag == "Player") {
			other.GetComponent<PlayerMovement>().ladderState = true;
		}
	}
	
	void  OnTriggerExit (Collider other){
		if (other.tag == "Player") {
			other.GetComponent<PlayerMovement>().ladderState = false;
		}
	}
}