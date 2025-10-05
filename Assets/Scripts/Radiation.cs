using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Radiation : MonoBehaviour {
	public float radiationAmount = 11f;
	public float intervalTime = 1f;
	public float radFinished = 0f;
	private static StringBuilder s1 = new StringBuilder();

	void Start() {
		radFinished = PauseScript.a.relativeTime + (intervalTime * 2);
	}

	void OnTriggerEnter (Collider col) {
		if (col.gameObject.CompareTag("Player")) {
			if (PlayerHealth.a.hm.health > 0f) {
				PlayerHealth.a.radiationArea = true;
				PlayerHealth.a.GiveRadiation(radiationAmount);
				radFinished = PauseScript.a.relativeTime + (intervalTime*Random.Range(1f,1.5f));
			}
		}
	}

	void  OnTriggerStay (Collider col) {
		if (col.gameObject.CompareTag("Player")) {
			if (PlayerHealth.a.hm.health > 0f && (radFinished < PauseScript.a.relativeTime)) {
				PlayerHealth.a.radiationArea = true;
				PlayerHealth.a.GiveRadiation(radiationAmount);
				radFinished = PauseScript.a.relativeTime + (intervalTime*Random.Range(1f,1.5f));
			}
		}
	}

	void OnTriggerExit (Collider col) {
		if (col.gameObject.CompareTag("Player")) { 
			if (PlayerHealth.a.hm.health > 0f) {
				PlayerHealth.a.radiationArea = false;
				radFinished = PauseScript.a.relativeTime;  // reset so re-triggering is instant
			}
		}
	}
	
	void OnDisable() {
		PlayerHealth.a.radiationArea = false;
	}
	
	public static string Save(GameObject go) {
		Radiation rad = go.GetComponent<Radiation>();
		s1.Clear();
		s1.Append(Utils.FloatToString(rad.radiationAmount,"radiationAmount"));
		s1.Append(Utils.splitChar);	
		s1.Append(Utils.FloatToString(rad.intervalTime,"intervalTime"));
		s1.Append(Utils.splitChar);	
		s1.Append(Utils.SaveRelativeTimeDifferential(rad.radFinished,"radFinished"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		Radiation rad = go.GetComponent<Radiation>();
		rad.radiationAmount = Utils.GetFloatFromString(entries[index],"radiationAmount"); index++;
		rad.intervalTime = Utils.GetFloatFromString(entries[index],"intervalTime"); index++;
		rad.radFinished = Utils.LoadRelativeTimeDifferential(entries[index],"radFinished"); index++;
		return index;
	}
}
