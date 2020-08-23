using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	public string target;
	public string argvalue;
	public bool onlyTargetOnce;
	[HideInInspector]
	public bool targetAlreadyDone = false; // save
	[Tooltip("Delay after full open before door closes")]
	public float delay;
	[Tooltip("Whether door is locked, unuseable until unlocked")]
	public bool locked; // saved
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	[Tooltip("If yes, door never closes automatically")]
	public bool stayOpen;
	[Tooltip("Should door start open (you should set stayOpen too!)")]
	public bool startOpen;
	[Tooltip("Should door start partially open")]
	public bool ajar = false; // save
	[Tooltip("If partially open, by what percentage")]
	public float ajarPercentage = 0.5f;
	[Tooltip("Delay after use before door can be re-used")]
	public float useTimeDelay = 0.15f;
	[Tooltip("Message to display when door is locked, e.g.'door is broken beyond repair'")]
	public string lockedMessage;
	public int lockedMessageLingdex = 3;
	private string cardMessage; // set in start to hardcoded lingdex 2
	private string cardUsedMessage; // diddo with lingdex 4
	private string butdoorStillLockedMessage;  // lingdex 5
	public bool blocked = false; // save
	[HideInInspector]
	public float useFinished; // save
	[HideInInspector]
	public float waitBeforeClose; // save
	[HideInInspector]
	public Animator anim;
	private AudioSource SFX = null;
	[Tooltip("Door sound when opening or closing")]
	public AudioClip SFXClip; // assign in the editor
	public enum doorState {Closed, Open, Closing, Opening};
	public enum accessCardType {None,Standard,Medical,Science,Admin,Group1,Group2,Group3,Group4,GroupA,GroupB,Storage,Engineering,Maintenance,Security,Per1,Per2,Per3,Per4,Per5,Command};
	public accessCardType requiredAccessCard = accessCardType.None;
	public bool accessCardUsedByPlayer = false; // save
	public doorState doorOpen; // save

	public float timeBeforeLasersOn;
	[HideInInspector]
	public float lasersFinished; // save
	public GameObject[] laserLines;
	public GameObject[] collidersList;
	public bool toggleLasers = false;
	public bool targettingOnlyUnlocks = false;
	public float animatorPlaybackTime; // save
	public bool changeLayerOnOpenClose = false;

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
	private bool firstUpdateAfterLoad = false;
	private string loadedClipName;
	private int loadedClipIndex;
	private float loadedAnimatorPlaybackTime;

	void Start () {
		anim = GetComponent<Animator>();
		animatorPlaybackTime = 0;
		if (requiredAccessCard == accessCardType.None)
			accessCardUsedByPlayer = true;
		
		SFX = GetComponent<AudioSource>();
		if (SFX == null) Debug.Log("BUG: No AudioSource on Door!");
		if (SFXClip == null) Debug.Log("BUG: No audio clip SFXClip on Door!");
		
		useFinished = PauseScript.a.relativeTime;
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

		cardMessage = Const.a.stringTable[2];
		if (string.IsNullOrEmpty(lockedMessage)) {
			if (lockedMessageLingdex >= 0) {
				if (lockedMessageLingdex < Const.a.stringTable.Length)
				lockedMessage = Const.a.stringTable[lockedMessageLingdex];
			}
		}
		cardUsedMessage = Const.a.stringTable[4];
		butdoorStillLockedMessage = Const.a.stringTable[5];
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position,ud);
			return;
		}

		if (LevelManager.a.superoverride || Const.a.difficultyMission == 0) {
			// SHODAN can go anywhere!  Full security override!
			locked = false;
			requiredAccessCard = accessCardType.None;
			accessCardUsedByPlayer = true;
		}

		if (Const.a.difficultyMission <= 1) {
			requiredAccessCard = accessCardType.None;
			accessCardUsedByPlayer = true;
		}

		AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(defIndex);
		animatorPlaybackTime = asi.normalizedTime;

		if (useFinished < PauseScript.a.relativeTime) {
			if (requiredAccessCard == accessCardType.None || ud.owner.GetComponent<PlayerReferenceManager>().playerInventory.GetComponent<AccessCardInventory>().HasAccessCard(requiredAccessCard) || accessCardUsedByPlayer) {
				useFinished = PauseScript.a.relativeTime + useTimeDelay;	
				//AnimatorClipInfo[] aci = anim.GetCurrentAnimatorClipInfo(defIndex);

				if (!locked) {
					if (requiredAccessCard != accessCardType.None) {
						Const.sprint(requiredAccessCard.ToString() + cardUsedMessage,ud.owner); // state that we just used a keycard and access was granted
						accessCardUsedByPlayer = true;
					}

					if (!string.IsNullOrWhiteSpace(target)) {
						if (onlyTargetOnce && !targetAlreadyDone) {
							targetAlreadyDone = true;
							ud.argvalue = argvalue;
							TargetIO tio = GetComponent<TargetIO>();
							if (tio != null) {
								ud.SetBits(tio);
							} else {
								Debug.Log("BUG: no TargetIO.cs found on an object with a Door.cs script!  Trying to call UseTargets without parameters!");
							}
							Const.a.UseTargets(ud,target);
						} else {
							targetAlreadyDone = true;
							ud.argvalue = argvalue;
							TargetIO tio = GetComponent<TargetIO>();
							if (tio != null) {
								ud.SetBits(tio);
							} else {
								Debug.Log("BUG: no TargetIO.cs found on an object with a Door.cs script!  Trying to call UseTargets without parameters!");
							}
							Const.a.UseTargets(ud,target);
						}
					}

					if (ajar) {
						ajar = false;
						animatorPlaybackTime = topTime * ajarPercentage;
					}

					if (doorOpen == doorState.Open && animatorPlaybackTime > 0.95f) {
						//Debug.Log("Was Open, Now Closing");
						doorOpen = doorState.Closing;
						CloseDoor();
						return;
					}

					if (doorOpen == doorState.Closed  && animatorPlaybackTime > 0.95f){
						//Debug.Log("Was Close, Now Opening");
						doorOpen = doorState.Opening;
						OpenDoor();
						return;
					}

					if (doorOpen == doorState.Opening) {
						doorOpen = doorState.Closing;
						anim.Play(closeClipName,defIndex, topTime-animatorPlaybackTime);
						//Debug.Log("Reversing from Opening to Closing. animatorPlaybackTime = " +animatorPlaybackTime.ToString() + ", topTime-playbackTime = " + (topTime-playbackTime).ToString());
						if (SFXClip != null && SFX != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXClip);
						return;
					}

					if (doorOpen == doorState.Closing) {
						doorOpen = doorState.Opening;
						waitBeforeClose = PauseScript.a.relativeTime + delay;
						anim.Play(openClipName,defIndex, topTime-animatorPlaybackTime);
						//Debug.Log("Reversing from Closing to Opening. animatorPlaybackTime = " +animatorPlaybackTime.ToString() + ", topTime-playbackTime = " + (topTime-playbackTime).ToString());
						if (SFXClip != null && SFX != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXClip);
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
		waitBeforeClose = PauseScript.a.relativeTime + delay;
		anim.Play(openClipName);
		if (SFX != null && SFXClip != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXClip);
		if (toggleLasers) {
			for (int i=defIndex;i<laserLines.Length;i++) {
				if (laserLines[i].activeSelf) laserLines[i].SetActive(false);
			}
			lasersFinished = Mathf.Infinity;
		}

		if (changeLayerOnOpenClose) {
			SetCollisionLayerOpen();
		}
	}

	void CloseDoor() {
		anim.speed = defaultSpeed;
		doorOpen = doorState.Closing;
		anim.Play(closeClipName);
		if (SFX != null && SFXClip != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXClip);
		if (toggleLasers) {
			lasersFinished = PauseScript.a.relativeTime + timeBeforeLasersOn;
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

		if (changeLayerOnOpenClose) {
			SetCollisionLayerClosed();
		}
	}

	public void SetAnimFromLoad(string n, int i, float t) {
		firstUpdateAfterLoad = true;
		loadedClipName = n;
		loadedClipIndex = i;
		loadedAnimatorPlaybackTime = t;
	}

	void Update () {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			if (firstUpdateAfterLoad) {
				firstUpdateAfterLoad = false;
				anim.Play(loadedClipName,loadedClipIndex,loadedAnimatorPlaybackTime);
				switch(loadedClipName) {
					case "IdleOpen": doorOpen = doorState.Open; break;
					case "IdleClosed": doorOpen = doorState.Closed; break;
					case "DoorOpen": doorOpen = doorState.Opening; break;
					case "DoorClose": doorOpen = doorState.Closing; break;
				}
			}

			if (ajar) {
				doorOpen = doorState.Opening;
				if (anim.speed != speedZero) {
					anim.Play(openClipName,defIndex, ajarPercentage);
					anim.speed = speedZero;
				}
				return;
			}
				
			if (blocked) {
				Blocked();
			} else {
				Unblocked();
			}

			if (doorOpen == doorState.Closing || doorOpen == doorState.Opening) {
				AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(defIndex);
				animatorPlaybackTime = asi.normalizedTime;
				//if (anim.GetCurrentAnimatorStateInfo(defIndex).IsName(idleClosedClipName))
				if (doorOpen == doorState.Closing && animatorPlaybackTime > 0.95f) {
					doorOpen = doorState.Closed;  // Door is closed
				}

				//if (anim.GetCurrentAnimatorStateInfo(defIndex).IsName(idleOpenClipName))
				if (doorOpen == doorState.Opening && animatorPlaybackTime > 0.95f)
					doorOpen = doorState.Open; // Door is open
			}

			if (PauseScript.a.relativeTime > waitBeforeClose) {
				if ((doorOpen == doorState.Open) && (!stayOpen) && (!startOpen))
					CloseDoor();
			}

			if (toggleLasers) {
				if (lasersFinished < PauseScript.a.relativeTime) {
					for (int i=defIndex;i<laserLines.Length;i++) {
						if (!laserLines[i].activeSelf) laserLines[i].SetActive(true);
					}
				}
			}

			if (doorOpen == doorState.Open) {
				if (nmo.enabled) nmo.enabled = false; // clear path, door is open
			} else {
				if (!nmo.enabled) nmo.enabled = true; // door is not opened fully, treat as a closed path
			}
		}
	}

	void SetCollisionLayerOpen() {
		for (int i=0;i<collidersList.Length;i++) {
			collidersList[i].layer = 19; // InterDebris
		}
	}

	void SetCollisionLayerClosed() {
		for (int i=0;i<collidersList.Length;i++) {
			collidersList[i].layer = 18; // Door
		}
	}

	void Blocked () {
		if (anim.speed != speedZero) anim.speed = speedZero;
	}

	void Unblocked () {
		if (anim.speed != defaultSpeed) anim.speed = defaultSpeed;
	}
}
