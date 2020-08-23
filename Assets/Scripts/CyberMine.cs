using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberMine : MonoBehaviour {

    void Start() {
        if (Const.a.difficultyCyber < 3) {
			if (UnityEngine.Random.Range(0,1f) < 0.2f) gameObject.SetActive(false); // 20% chance of not spawning on normal
		}

        if (Const.a.difficultyCyber < 2) {
			if (UnityEngine.Random.Range(0,1f) < 0.33f) gameObject.SetActive(false); // 33% chance of not spawning on easy
		}

        if (Const.a.difficultyCyber < 1) {
			if (UnityEngine.Random.Range(0,1f) < 0.50f) gameObject.SetActive(false); // 50% chance of not spawning on grandma
			return;
		}
    }

}
