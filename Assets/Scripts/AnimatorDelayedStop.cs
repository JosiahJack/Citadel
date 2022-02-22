using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorDelayedStop : MonoBehaviour {
    public float delay = 0.5f;
	private Animator anim;

	void Start() {
		anim = GetComponent<Animator>();
		if (anim == null) Debug.Log("BUG: AnimatorDelayedStop missing component for anim");
	}

	void OnEnable() {
		if (anim == null) anim = GetComponent<Animator>();
		if (anim == null) Debug.Log("BUG: AnimatorDelayedStop missing component for anim");
        StartCoroutine(EnableObjects());
    }

    IEnumerator EnableObjects() {
        yield return new WaitForSeconds(delay);
        anim.speed = 0f; // The whole point of this script, stop the animation from playing.
    }
}
