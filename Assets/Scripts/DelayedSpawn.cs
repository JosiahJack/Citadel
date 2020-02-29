using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedSpawn : MonoBehaviour {
    public float delay = 0.5f;
	public GameObject[] objectsToSpawn;
	public bool despawnInstead = false;

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
    }
}
