using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedSpawn : MonoBehaviour {
    public float delay = 0.5f;
	public GameObject[] objectsToSpawn;
	public bool despawnInstead = false;
	public bool doSelfAfterList = false;
	public bool destroyAfterListInsteadOfDeactivate = false;

	void OnEnable() {
        StartCoroutine(EnableObjects());
    }

    IEnumerator EnableObjects() {
        yield return new WaitForSeconds(delay);
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
