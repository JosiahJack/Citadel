using UnityEngine;
using System.Collections;

public class Door1 : MonoBehaviour {
	Animator anim;
	int doorOpen;
	float waitBeforeClose;
	public float delay;
	public bool locked;
	public bool stayOpen;
	public bool ajar = false;
	public float ajarPercentage = 0.5f;
	private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	
	void Start () {
		doorOpen = 0;
		anim = GetComponent<Animator>();
		SFX = GetComponent<AudioSource>();
	}

	void Use (GameObject owner) {
		ajar = false;

		if (!locked) {
			if (doorOpen == 3) {
				AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);
				float playbackTime = asi.normalizedTime%1;
				doorOpen = 2;
				anim.Play("DoorClose",0, 1f-playbackTime);
				SFX.PlayOneShot(SFXClip);
				return;
			}

			if (doorOpen == 2) {
				AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);
				float playbackTime = asi.normalizedTime%1;
				doorOpen = 3;
				anim.Play("DoorOpen",0, 1f-playbackTime);
				SFX.PlayOneShot(SFXClip);
				return;
			}

			if (doorOpen == 1) {
				CloseDoor();
				return;
			}

			if (doorOpen == 0){
				OpenDoor();
				return;
			}
		} else {
			Const.sprint("Door is locked");
		}
	}

	void Targetted (GameObject owner) {
		if (locked)
			locked = false;
	
		Use(owner);
	}

	void OpenDoor() {
		anim.speed = 1f;
		doorOpen = 3; //Opening state
		waitBeforeClose = Time.time + delay;
		anim.Play("DoorOpen");
		SFX.PlayOneShot(SFXClip);
	}

	void CloseDoor() {
		anim.speed = 1f;
		doorOpen = 2;
		anim.Play("DoorClose");
		SFX.PlayOneShot(SFXClip);
	}

	void Update () {
		if (ajar) {
			anim.Play("DoorOpen",0, ajarPercentage);
			anim.speed = 0f;
		}

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("IdleClosed"))
			doorOpen = 0;  // Door is closed

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("IdleOpen"))
			doorOpen = 1; // Door is open

		if (Time.time > waitBeforeClose) {
			if ((doorOpen == 1) && (!stayOpen))
				CloseDoor();
		}
	}
}
