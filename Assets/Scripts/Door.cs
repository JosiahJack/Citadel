using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	[Tooltip("Delay after full open before door closes")]
	public float delay;
	[Tooltip("Whether door is locked, unuseable until unlocked")]
	public bool locked;
	[Tooltip("If yes, door never closes automatically")]
	public bool stayOpen;
	[Tooltip("Should door start open (you should set stayOpen too!)")]
	public bool startOpen;
	[Tooltip("Should door start partially open")]
	public bool ajar = false;
	[Tooltip("If partially open, by what percentage")]
	public float ajarPercentage = 0.5f;
	[Tooltip("Delay after use before door can be re-used")]
	public float useTimeDelay = 0.15f;
	[Tooltip("Message to display when door is locked, e.g.'door is broken beyond repair'")]
	public string lockedMessage = "Door is locked";
	[HideInInspector]
	public bool blocked = false;
	private float useFinished;
	float waitBeforeClose;
	private Animator anim;
	private AudioSource SFX = null; // assign in the editor
	[Tooltip("Door sound when opening or closing")]
	public AudioClip SFXClip = null; // assign in the editor
	public enum doorState {Closed, Open, Closing, Opening};
	public doorState doorOpen;

	public float timeBeforeLasersOn;
	public float lasersFinished;
	public GameObject[] laserLines;
	public bool toggleLasers = false;

	void Start () {
		anim = GetComponent<Animator>();
		if (startOpen) {
			doorOpen = doorState.Open;
			anim.Play("IdleOpen");
		} else {
			doorOpen = doorState.Closed;
			anim.Play("IdleClosed");
		}
		
		SFX = GetComponent<AudioSource>();
		useFinished = Time.time;
	}

	void Use (GameObject owner) {
		ajar = false;
		if (useFinished < Time.time) {
			useFinished = Time.time + useTimeDelay;
			AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);
			float playbackTime = asi.normalizedTime;
			//AnimatorClipInfo[] aci = anim.GetCurrentAnimatorClipInfo(0);

			if (!locked) {
				if (doorOpen == doorState.Open) {
					CloseDoor();
					return;
				}

				if (doorOpen == doorState.Closed){
					OpenDoor();
					return;
				}

				if (doorOpen == doorState.Opening) {
					if (playbackTime > 0.15f && playbackTime < 0.85f) {
						doorOpen = doorState.Closing;
						anim.Play("DoorClose",0, 1f-playbackTime);
						SFX.PlayOneShot(SFXClip);
					}
					return;
				}

				if (doorOpen == doorState.Closing) {
					if (playbackTime > 0.15f && playbackTime < 0.85f) {
						doorOpen = doorState.Opening;
						waitBeforeClose = Time.time + delay;
						anim.Play("DoorOpen",0, 1f-playbackTime);
						SFX.PlayOneShot(SFXClip);
					}
					return;
				}
			} else {
				Const.sprint(lockedMessage,owner); // tell the owner of the Use command that we are locked
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
		doorOpen = doorState.Opening;
		waitBeforeClose = Time.time + delay;
		anim.Play("DoorOpen");
		SFX.PlayOneShot(SFXClip);
		//lightsFinished1 = Time.time + timeBeforeLightsOut1;
		if (toggleLasers) {
			for (int i=0;i<laserLines.Length;i++) {
				laserLines[i].SetActive(false);
			}
			lasersFinished = Mathf.Infinity;
		}

	}

	void CloseDoor() {
		anim.speed = 1f;
		doorOpen = doorState.Closing;
		anim.Play("DoorClose");
		SFX.PlayOneShot(SFXClip);
		lasersFinished = Time.time + timeBeforeLasersOn;
	}

	void Update () {
		if (ajar) {
			doorOpen = doorState.Closing;
			anim.Play("DoorOpen",0, ajarPercentage);
			anim.speed = 0f;
		}
			
		if (blocked || (PauseScript.a != null && PauseScript.a.paused)) {
			Blocked();
		} else {
			if (PauseScript.a != null && !PauseScript.a.paused) {
				Unblocked();
			}
		}

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("IdleClosed"))
			doorOpen = doorState.Closed;  // Door is closed

		if (anim.GetCurrentAnimatorStateInfo(0).IsName("IdleOpen"))
			doorOpen = doorState.Open; // Door is open

		if (Time.time > waitBeforeClose) {
			if ((doorOpen == doorState.Open) && (!stayOpen))
				CloseDoor();
		}

		if (lasersFinished < Time.time) {
			for (int i=0;i<laserLines.Length;i++) {
				laserLines[i].SetActive(true);
			}
		}
	}

	void Blocked () { anim.speed = 0f; }
	void Unblocked () { anim.speed = 1f; }
}
