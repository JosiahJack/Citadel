using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedSpawn : MonoBehaviour {
    public float delay = 0.5f;
	public GameObject[] objectsToSpawn;
	public bool despawnInstead = false;
	public bool doSelfAfterList = false;
	public bool destroyAfterListInsteadOfDeactivate = false;
	[HideInInspector] public float timerFinished; // save
	[HideInInspector] public bool active; // save

	void OnEnable() {
		timerFinished = PauseScript.a.relativeTime + delay;
        active = true;
    }

	void Update() {
		if (active) {
			if (timerFinished < PauseScript.a.relativeTime) {
				active = false; // Once only, unless we do self after the list.
				for (int i=0;i<objectsToSpawn.Length;i++) {
					if (despawnInstead) {
						if (objectsToSpawn[i] != null) objectsToSpawn[i].SetActive(false);
					} else {
						if (objectsToSpawn[i] != null) objectsToSpawn[i].SetActive(true);
					}
				}
				if (doSelfAfterList) {
					if (despawnInstead) {
						if (destroyAfterListInsteadOfDeactivate) {
							Destroy(gameObject);
						} else {
							gameObject.SetActive(false);
						}
					} else {
						gameObject.SetActive(true);
					}
				}
			}
		}
    }

	public static string Save(GameObject go) {
		DelayedSpawn ds = go.GetComponent<DelayedSpawn>();
		if (ds == null) {
			Debug.Log("DelayedSpawn missing on savetype of DelayedSpawn!  GameObject.name: " + go.name);
			return "0000.00000|0";
		}

		string line = System.String.Empty;
		line = Utils.SaveRelativeTimeDifferential(ds.timerFinished);
		line += Utils.splitChar + Utils.BoolToString(ds.active);
		return line;
	}
}
