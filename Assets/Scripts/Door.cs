﻿using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	public string target;
	[Tooltip("Delay after full open before door closes")]
	public float delay;
	[Tooltip("Whether door is locked, unuseable until unlocked")]
	public bool locked;
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
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
	[Tooltip("Message to display when door requires a keycard, e.g.'Standard Access card required'")]
	public string cardMessage = " access card required";
	[Tooltip("Message to display when door is opened using a keycard, e.g.'Standard Access card used'")]
	public string cardUsedMessage = " access granted";
	public string butdoorStillLockedMessage = " but door is locked.";
	[HideInInspector]
	public bool blocked = false;
	private float useFinished;
	float waitBeforeClose;
	private Animator anim;
	private AudioSource SFX = null; // assign in the editor
	[Tooltip("Door sound when opening or closing")]
	public AudioClip SFXClip = null; // assign in the editor
	public enum doorState {Closed, Open, Closing, Opening};
	public enum accessCardType {None,Standard,Medical,Science,Admin,Group1,Group2,Group3,Group4,GroupA,GroupB,Storage,Engineering,Maintenance,Security,Per1,Per2,Per3,Per4,Per5};
	public accessCardType requiredAccessCard = accessCardType.None;
	public bool accessCardUsedByPlayer = false;
	public doorState doorOpen;

	public float timeBeforeLasersOn;
	public float lasersFinished;
	public GameObject[] laserLines;
	public bool toggleLasers = false;
	public bool targettingOnlyUnlocks = false;
	public bool debugging = false;

	private int defIndex = 0;
	private float topTime = 1.00f;
	private float defaultSpeed = 1.00f;
	private float speedZero = 0.00f;
	//private string idleOpenClipName = "IdleOpen";
	private string idleClosedClipName = "IdleClosed";
	private string openClipName = "DoorOpen";
	private string closeClipName = "DoorClose";
	private GameObject dynamicObjectsContainer;
	private int i = 0;
	private UnityEngine.AI.NavMeshObstacle nmo;

	void Start () {
		anim = GetComponent<Animator>();

		if (requiredAccessCard == accessCardType.None)
			accessCardUsedByPlayer = true;
		
		SFX = GetComponent<AudioSource>();
		useFinished = Time.time;
		nmo = GetComponent<UnityEngine.AI.NavMeshObstacle>();
		if (nmo == null) Const.sprint("BUG: Missing NavMeshObstacle on Door at " + transform.position.ToString(),Const.a.allPlayers);
		if (nmo != null) nmo.carving = true; // creates a "hole" in the NavMesh forcing enemies to find an alternate route

		if (startOpen) {
			stayOpen = true;
			OpenDoor();
		} else {
			doorOpen = doorState.Closed;
			anim.Play(idleClosedClipName);
		}
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position);
			return;
		}

		if (LevelManager.a.superoverride) {
			// SHODAN can go anywhere!  Full security override!
			locked = false;
			requiredAccessCard = accessCardType.None;
			accessCardUsedByPlayer = true;
		}

		ajar = false;
		if (useFinished < Time.time) {
			if (requiredAccessCard == accessCardType.None || ud.owner.GetComponent<PlayerReferenceManager>().playerInventory.GetComponent<AccessCardInventory>().HasAccessCard(requiredAccessCard) || accessCardUsedByPlayer) {
				useFinished = Time.time + useTimeDelay;
				AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(defIndex);
				float playbackTime = asi.normalizedTime;
				//AnimatorClipInfo[] aci = anim.GetCurrentAnimatorClipInfo(defIndex);

				if (!locked) {
					if (requiredAccessCard != accessCardType.None) {
						Const.sprint(requiredAccessCard.ToString() + cardUsedMessage,ud.owner); // tell the owner of the Use command that we are locked
						accessCardUsedByPlayer = true;
					}

					if (doorOpen == doorState.Open && playbackTime > 0.95f) {
						//Debug.Log("Was Open, Now Closing");
						doorOpen = doorState.Closing;
						CloseDoor();
						return;
					}

					if (doorOpen == doorState.Closed  && playbackTime > 0.95f){
						//Debug.Log("Was Close, Now Opening");
						doorOpen = doorState.Opening;
						OpenDoor();
						return;
					}

					if (doorOpen == doorState.Opening) {
						doorOpen = doorState.Closing;
						anim.Play(closeClipName,defIndex, topTime-playbackTime);
						//Debug.Log("Reversing from Opening to Closing.  playbackTime = " + playbackTime.ToString() + ", topTime-playbackTime = " + (topTime-playbackTime).ToString());
						SFX.PlayOneShot(SFXClip);
						return;
					}

					if (doorOpen == doorState.Closing) {
						doorOpen = doorState.Opening;
						waitBeforeClose = Time.time + delay;
						anim.Play(openClipName,defIndex, topTime-playbackTime);
						//Debug.Log("Reversing from Closing to Opening.  playbackTime = " + playbackTime.ToString() + ", topTime-playbackTime = " + (topTime-playbackTime).ToString());
						SFX.PlayOneShot(SFXClip);
						return;
					}
				} else {
					if (requiredAccessCard != accessCardType.None) {
						Const.sprint (requiredAccessCard.ToString() + cardUsedMessage + butdoorStillLockedMessage,ud.owner);
						accessCardUsedByPlayer = true;
					} else {
						Const.sprint(lockedMessage,ud.owner); // tell the owner of the Use command that we are locked
					}
				}
			} else {
				Const.sprint(requiredAccessCard.ToString() + cardMessage,ud.owner); // tell the owner of the Use command that we are locked
			}
		}
	}

	void Targetted (UseData ud) {
		if (locked) 
			locked = false;
	
		if (!targettingOnlyUnlocks) Use(ud);
	}

	public void ForceOpen() {
		if (doorOpen == doorState.Open) return;
		OpenDoor();
	}

	public void ForceClose() {
		if (doorOpen == doorState.Closed) return;
		CloseDoor();
	}

	public void Lock(string arg) {
		locked = true;
		if (arg == "" || arg == " " || arg == "  ") arg = "Door is locked"; // default
		lockedMessage = arg;
	}

	public void Unlock() {
		locked = false;
	}

	public void ToggleLocked(string arg) {
		if (locked) {
			Unlock();
		} else {
			Lock(arg);
		}
	}

	void OpenDoor() {
		anim.speed = defaultSpeed;
		doorOpen = doorState.Opening;
		waitBeforeClose = Time.time + delay;
		anim.Play(openClipName);
		SFX.PlayOneShot(SFXClip);
		//lightsFinished1 = Time.time + timeBeforeLightsOut1;
		if (toggleLasers) {
			for (int i=defIndex;i<laserLines.Length;i++) {
				laserLines[i].SetActive(false);
			}
			lasersFinished = Mathf.Infinity;
		}

	}

	void CloseDoor() {
		anim.speed = defaultSpeed;
		doorOpen = doorState.Closing;
		anim.Play(closeClipName);
		SFX.PlayOneShot(SFXClip);
		if (toggleLasers) {
			lasersFinished = Time.time + timeBeforeLasersOn;
		}
		dynamicObjectsContainer = LevelManager.a.GetCurrentLevelDynamicContainer();
        if (dynamicObjectsContainer == null) return; //didn't find current level, go ahead and ghost through objects
		// horrible hack to keep objects that have their physics sleeping ghosting through the door as it closes
		for (i=defIndex;i<dynamicObjectsContainer.transform.childCount;i++) {
			if (Vector3.Distance(transform.position,dynamicObjectsContainer.transform.GetChild(i).transform.position) < 5) {
				Rigidbody changeThis = dynamicObjectsContainer.transform.GetChild(i).GetComponent<Rigidbody>();
				if (changeThis != null) changeThis.WakeUp();
			}
		}
	}

	void Update () {
		if (ajar) {
			doorOpen = doorState.Closing;
			anim.Play(openClipName,defIndex, ajarPercentage);
			anim.speed = speedZero;
		}
			
		if (blocked || (PauseScript.a != null && PauseScript.a.Paused())) {
			Blocked();
		} else {
			if (PauseScript.a != null && !PauseScript.a.Paused()) {
				Unblocked();
			}
		}

		if (debugging) Debug.Log("doorOpen state = " + doorOpen.ToString());

		AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(defIndex);
		float playbackTime = asi.normalizedTime;
		//if (anim.GetCurrentAnimatorStateInfo(defIndex).IsName(idleClosedClipName))
		if (doorOpen == doorState.Closing && playbackTime > 0.95f)
			doorOpen = doorState.Closed;  // Door is closed

		//if (anim.GetCurrentAnimatorStateInfo(defIndex).IsName(idleOpenClipName))
		if (doorOpen == doorState.Opening && playbackTime > 0.95f)
			doorOpen = doorState.Open; // Door is open

		if (Time.time > waitBeforeClose) {
			if ((doorOpen == doorState.Open) && (!stayOpen) && (!startOpen))
				CloseDoor();
		}

		if (lasersFinished < Time.time) {
			for (int i=defIndex;i<laserLines.Length;i++) {
				laserLines[i].SetActive(true);
			}
		}

		if (doorOpen == doorState.Open) {
			nmo.enabled = false; // clear path, door is open
		} else {
			nmo.enabled = true; // door is not opened fully, treat as a closed path
		}
	}

	void Blocked () { anim.speed = speedZero; }
	void Unblocked () { anim.speed = defaultSpeed; }
}
