using UnityEngine;
using System.Collections;

public class Door1 : MonoBehaviour {
	Animator anim;
	int doorOpen;
	float waitBeforeClose;
	public float delay;
	private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	
	void Start () {
		doorOpen = 0;
		anim = GetComponent<Animator>();
		SFX = GetComponent<AudioSource>();
	}

	void Use () {
		if (doorOpen == 1) {
			doorOpen = 2; //Closing state
			anim.Play("DoorClose");
			SFX.PlayOneShot(SFXClip);
		}

		if (doorOpen == 0) {
			doorOpen = 3; //Opening state
			waitBeforeClose = Time.time + delay;
			anim.Play("DoorOpen");
			SFX.PlayOneShot(SFXClip);
		}
	}

	void CloseDoor() {
		if (doorOpen == 1) {
			doorOpen = 2;
			anim.Play("DoorClose");
			SFX.PlayOneShot(SFXClip);
		}
	}

	void Update () {
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("IdleClosed")) {
			doorOpen = 0;  // Door is closed
		}

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("IdleOpen")) {
			doorOpen = 1; // Door is open
		}

		if (Time.time > waitBeforeClose)
			CloseDoor();
	}
}
