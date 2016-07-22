using UnityEngine;
using System.Collections;

public class Door1 : MonoBehaviour {
	[Tooltip("Delay after full open before door closes")]
	public float delay;
	[Tooltip("Whether door is locked, unuseable until unlocked")]
	public bool locked;
	[Tooltip("If yes, door never closes automatically")]
	public bool stayOpen;
	[Tooltip("Should door start partially open")]
	public bool ajar = false;
	[Tooltip("If partially open, by what percentage")]
	public float ajarPercentage = 0.5f;
	[Tooltip("Delay after use before door can be re-used")]
	public float useTimeDelay = 0.15f;
	[Tooltip("Message to display when door is locked, e.g.'door is broken beyond repair'")]
	public string lockedMessage = "Door is locked";
	public int doorOpen;
	[HideInInspector]
	public bool blocked = false;
	private float useFinished;
	float waitBeforeClose;
	private Animator anim;
	private AudioSource SFX = null; // assign in the editor
	[Tooltip("Door sound when opening or closing")]
	public AudioClip SFXClip = null; // assign in the editor
	enum doorState {Closed, Open, Closing, Opening};

	void Start () {
		doorOpen = (int) doorState.Closed;
		anim = GetComponent<Animator>();
		SFX = GetComponent<AudioSource>();
	}

	void Use (GameObject owner) {
		ajar = false;
		if (useFinished < Time.time) {
			useFinished = Time.time + useTimeDelay;
			if (!locked) {
				if (doorOpen == (int) doorState.Open) {
					CloseDoor();
					return;
				}

				if (doorOpen == (int) doorState.Closed){
					OpenDoor();
					return;
				}

				if (doorOpen == (int) doorState.Opening) {
					AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);
					float playbackTime = asi.normalizedTime;
					if (playbackTime > 0.15f && playbackTime < 0.85f) {
						doorOpen = (int) doorState.Closing;
						anim.Play("DoorClose",0, 1f-playbackTime);
						SFX.PlayOneShot(SFXClip);
					}
					return;
				}

				if (doorOpen == (int) doorState.Closing) {
					AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);
					float playbackTime = asi.normalizedTime;
					if (playbackTime > 0.15f && playbackTime < 0.85f) {
						doorOpen = (int) doorState.Opening;
						waitBeforeClose = Time.time + delay;
						anim.Play("DoorOpen",0, 1f-playbackTime);
						SFX.PlayOneShot(SFXClip);
					}
					return;
				}
			} else {
				Const.sprint(lockedMessage);
			}
		}
	}

	void Targetted (GameObject owner) {
		if (locked)
			locked = false;
	
		Use(owner);
	}

	void OpenDoor() {
		anim.speed = 1f;
		doorOpen = (int) doorState.Opening;
		waitBeforeClose = Time.time + delay;
		anim.Play("DoorOpen");
		SFX.PlayOneShot(SFXClip);
	}

	void CloseDoor() {
		anim.speed = 1f;
		doorOpen = (int) doorState.Closing;
		anim.Play("DoorClose");
		SFX.PlayOneShot(SFXClip);
	}

	void Update () {
		if (ajar) {
			doorOpen = (int) doorState.Opening;
			anim.Play("DoorOpen",0, ajarPercentage);
			anim.speed = 0f;
		}

		if (blocked || PauseScript.a.paused) {
			Blocked();
		} else {
			if (!PauseScript.a.paused) {
				Unblocked();
			}
		}

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("IdleClosed"))
			doorOpen = (int) doorState.Closed;  // Door is closed

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("IdleOpen"))
			doorOpen = (int) doorState.Open; // Door is open

		if (Time.time > waitBeforeClose) {
			if ((doorOpen == (int) doorState.Open) && (!stayOpen))
				CloseDoor();
		}
	}

	void Blocked () { anim.speed = 0f; }
	void Unblocked () { anim.speed = 1f; }
}
