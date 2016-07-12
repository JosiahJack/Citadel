using UnityEngine;
using System.Collections;

public class PooledItemDestroy : MonoBehaviour {
	public float itemLifeTime = 3.00f;

	void OnEnable () {
		StartCoroutine(DestroyBackToPool());
	}

	IEnumerator DestroyBackToPool () {
		yield return new WaitForSeconds(itemLifeTime);
		gameObject.SetActive(false);
	}
}
