using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedSpawn : MonoBehaviour {
    public float delay = 0.5f;
	public GameObject[] objectsToSpawn;

	void OnEnable() {
        StartCoroutine(EnableObjects());
    }

    IEnumerator EnableObjects() {
        yield return new WaitForSeconds(delay);
        for (int i=0;i<objectsToSpawn.Length;i++) {
			if (objectsToSpawn[i] != null) objectsToSpawn[i].SetActive(true);
		}
    }
}
