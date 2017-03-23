using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radiation : MonoBehaviour {
	public float intervalTime = 1f;
	public float radiationAmount = 11f;
	public float radFinished = 0f;
	public int numPlayers = 0;

	void OnTriggerEnter (Collider col) {
		if ((col.gameObject.tag == "Player") && (col.gameObject.GetComponent<PlayerHealth>().health > 0f)) {
			numPlayers++;
			col.gameObject.GetComponent<PlayerHealth>().radiationArea = true;
			col.gameObject.SendMessage("GiveRadiation",radiationAmount,SendMessageOptions.DontRequireReceiver);
			radFinished = Time.time + (intervalTime);
		}
	}

	void  OnTriggerStay (Collider col) {
		if ((col.gameObject.tag == "Player") && (col.gameObject.GetComponent<PlayerHealth>().health > 0f) && (radFinished < Time.time)) {
			col.gameObject.GetComponent<PlayerHealth>().radiationArea = true;
			col.gameObject.SendMessage("GiveRadiation",radiationAmount,SendMessageOptions.DontRequireReceiver);
			radFinished = Time.time + (intervalTime);
		}
	}

	void OnTriggerExit (Collider col) {
		if ((col.gameObject.tag == "Player") && (col.gameObject.GetComponent<PlayerHealth>().health > 0f)) {
			col.gameObject.GetComponent<PlayerHealth>().radiationArea = false;
			numPlayers--;
			if (numPlayers == 0) radFinished = Time.time;  // reset so re-triggering is instant
		}
	}
}
