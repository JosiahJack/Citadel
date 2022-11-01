using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radiation : MonoBehaviour {
	public float intervalTime = 1f;
	public float radiationAmount = 11f;
	public float radFinished = 0f;
	public int numPlayers = 0;
	public bool isEnabled = true;

	void OnTriggerEnter (Collider col) {
		if (!isEnabled) return;
		if (col.gameObject.CompareTag("Player")) {
			if (PlayerHealth.a.hm.health > 0f) {
				numPlayers++;
				PlayerHealth.a.radiationArea = true;
				PlayerHealth.a.GiveRadiation(radiationAmount);
				radFinished = PauseScript.a.relativeTime + (intervalTime*Random.Range(1f,1.5f));
			}
		}
	}

	void  OnTriggerStay (Collider col) {
		if (!isEnabled) return;
		if (col.gameObject.CompareTag("Player")) {
			if (PlayerHealth.a.hm.health > 0f && (radFinished < PauseScript.a.relativeTime)) {
				PlayerHealth.a.radiationArea = true;
				PlayerHealth.a.GiveRadiation(radiationAmount);
				radFinished = PauseScript.a.relativeTime + (intervalTime*Random.Range(1f,1.5f));
			}
		}
	}

	void OnTriggerExit (Collider col) {
		if (!isEnabled) return;
		if (col.gameObject.CompareTag("Player")) { 
			if (PlayerHealth.a.hm.health > 0f) {
				PlayerHealth.a.radiationArea = false;
				numPlayers--;
				if (numPlayers == 0) radFinished = PauseScript.a.relativeTime;  // reset so re-triggering is instant
			}
		}
	}

	public static string Save(GameObject go) {
		Radiation rad = go.GetComponent<Radiation>();
		if (rad == null) {
			UnityEngine.Debug.Log("Radiation missing on savetype of Radiation!  GameObject.name: " + go.name);
			return "1|0";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(rad.isEnabled); // bool - hey is this on? hello?
		line += Utils.splitChar + rad.numPlayers.ToString(); // int - how many players we are affecting
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		Radiation rad = go.GetComponent<Radiation>(); // ... ado about nothing
		if (rad == null || index < 0 || entries == null) return index + 2;

		rad.isEnabled = Utils.GetBoolFromString(entries[index]); index++; // bool - hey is this on? hello?
		rad.numPlayers = Utils.GetIntFromString(entries[index]); index++; // int - how many players we are affecting
		return index;
	}
}
