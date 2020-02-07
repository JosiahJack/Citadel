using UnityEngine;
using System.Collections;

public class PooledItemDestroy : MonoBehaviour {
	public float itemLifeTime = 3.00f;
	public bool onlyOnce = false;
	private bool doneYet = false;

	void OnEnable () {
		StartCoroutine(DestroyBackToPool());
	}

	IEnumerator DestroyBackToPool () {
		if (!doneYet) {
			yield return new WaitForSeconds(itemLifeTime);
			if (onlyOnce) doneYet = true;
			gameObject.SetActive(false);
		}
	}
}
