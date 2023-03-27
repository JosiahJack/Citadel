using UnityEngine;
using System.Collections;

public class PauseAnimation : MonoBehaviour {
	private Animation anim;

	void Awake () {
		Initialize();
	}

	void Initialize() {
		anim = GetComponent<Animation>();
		if (!Const.a.panimsList.Contains(this)) Const.a.panimsList.Add(this);
	}

	void OnEnable () {
		if (anim == null) Initialize();
	}
		
	public void Pause () {
		if (anim != null && this.enabled && gameObject.activeInHierarchy) {
            anim.Stop(); // Pause the animation.
        }
	}

	public void UnPause () {
		if (anim != null && this.enabled && gameObject.activeInHierarchy) {
            anim.Play(); // Resume the animation.
        }
	}
}
