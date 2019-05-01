using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDecay : MonoBehaviour {
	public float lifetime = 0.1f;

    void OnEnable() {
        StartCoroutine(Decay(lifetime));
    }

	private IEnumerator Decay(float LT) {
		yield return new WaitForSeconds(LT);
		gameObject.SetActive(false);
	}
}
