using UnityEngine;
using System.Collections;

public class ExplosionLife : MonoBehaviour {
	public float lightLife = 0.15f;
	public float brightness = 8f;
	public float delayBeforeDestroy = 0.8f;
	public float thinkInterval = 0.05f;
	public bool dontDestroy = false;
	private Light lite;

	void Awake () {
		if (lightLife > 0f) StartCoroutine(LifeTime(GetComponent<Light>(), 0f, brightness, lightLife));
		StartCoroutine(DelayedDestroy());
	}

	IEnumerator LifeTime (Light l, float fadeStart, float fadeEnd, float fadeTime) {
		float t = 0.0f;
		
		while (t < fadeTime) {
			t += Time.deltaTime;
			
			if (l != null) l.intensity = Mathf.Lerp(fadeStart, fadeEnd, t / fadeTime);
			yield return new WaitForSeconds(thinkInterval);
		}
		if (l != null) l.intensity = 0f;
	}

	IEnumerator DelayedDestroy () {
		yield return new WaitForSeconds (delayBeforeDestroy);
		if (dontDestroy) {
			gameObject.SetActive(false);
		} else {
			Utils.SafeDestroy(this.gameObject);
		}
	}
}
