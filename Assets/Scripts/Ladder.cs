using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {
	PlayerMovement pmov;
	GameObject other;
	
	void  OnTriggerEnter (Collider other){
		if (other.tag == "Player") {
			pmov = other.GetComponent<PlayerMovement>();
			if (pmov != null) {	pmov.ladderState = true; }
		}
	}
	
	void  OnTriggerExit (Collider other){
		if (other.tag == "Player") {
			pmov = other.GetComponent<PlayerMovement>();
			if (pmov != null) {	pmov.ladderState = false; }
		}
	}
}