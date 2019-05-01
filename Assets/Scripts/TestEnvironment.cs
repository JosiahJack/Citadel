using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnvironment : MonoBehaviour {
    public GameObject[] hideStuff;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < hideStuff.Length; i++)
            hideStuff[i].SetActive(false);
	}
}
