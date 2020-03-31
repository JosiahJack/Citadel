using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorDelayedStop : MonoBehaviour {
    public float delay = 0.5f;
	private Animator anim;

	void Start() {
		anim = GetComponent<Animator>();
	}

	void OnEnable() {
		if (anim == null) anim = GetComponent<Animator>();
        StartCoroutine(EnableObjects());
    }

    IEnumerator EnableObjects() {
        yield return new WaitForSeconds(delay);
        anim.speed = 0f;
    }
}
