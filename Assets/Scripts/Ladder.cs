using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {	
	void  OnTriggerEnter (Collider other){
		if (other.CompareTag("Player")) {
			PlayerMovement.a.ladderState++;
			if (PlayerMovement.a.ladderState < 1) PlayerMovement.a.ladderState = 1;
		}
	}
	
	void  OnTriggerExit (Collider other){
		if (other.CompareTag("Player")) {
			PlayerMovement.a.ladderState--;
			if (PlayerMovement.a.ladderState < 0) PlayerMovement.a.ladderState = 0;
		}
	}
}
