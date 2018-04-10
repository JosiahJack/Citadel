using UnityEngine;
using System.Collections;

public class SearchFXReset : MonoBehaviour {
	public float itemLifeTime = 3.00f;

	void OnEnable () {
		StartCoroutine(DisableMe());
	}

	IEnumerator DisableMe () {
		yield return new WaitForSeconds(itemLifeTime);
		gameObject.SetActive(false);
	}
}
