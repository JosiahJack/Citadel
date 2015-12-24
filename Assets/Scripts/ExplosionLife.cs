using UnityEngine;
using System.Collections;

public class ExplosionLife : MonoBehaviour {
	public float lightLife = 0.15f;
	public float brightness = 8f;
	public float delayBeforeDestroy = 0.8f;
	public float thinkInterval = 0.05f;
	private Light lite;

	void Awake () {
		StartCoroutine(LifeTime(GetComponent<Light>(), 0f, brightness, lightLife));
		StartCoroutine(DelayedDestroy());
	}

	IEnumerator LifeTime (Light l, float fadeStart, float fadeEnd, float fadeTime) {
		float t = 0.0f;
		
		while (t < fadeTime) {
			t += Time.deltaTime;
			
			l.intensity = Mathf.Lerp(fadeStart, fadeEnd, t / fadeTime);
			yield return new WaitForSeconds(thinkInterval);
		}
		l.intensity = 0f;
	}

	IEnumerator DelayedDestroy () {
		yield return new WaitForSeconds (delayBeforeDestroy);
		Destroy(this.gameObject);
	}
}
