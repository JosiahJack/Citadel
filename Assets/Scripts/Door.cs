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

	public AccessCardType requiredAccessCard = AccessCardType.None;
	public bool accessCardUsedByPlayer = false; // save
	public DoorState doorOpen; // save

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
		if (requiredAccessCard == AccessCardType.None)
			accessCardUsedByPlayer = true;
		
		SFX = GetComponent<AudioSource>();
		if (SFX == null) Debug.Log("BUG: No AudioSource on Door!");
		if (SFXClip == null) Debug.Log("BUG: No audio clip SFXClip on Door!");
		
		useFinished = PauseScript.a.relativeTime;
		nmo = GetComponent<UnityEngine.AI.NavMeshObstacle>();
		if (nmo == null) Const.sprint("BUG: Missing NavMeshObstacle on Door at " + transform.position.ToString());
		if (nmo != null) nmo.carving = true; // creates a "hole" in the NavMesh forcing enemies to find an alternate route

		if (startOpen) {
			stayOpen = true;
			OpenDoor();
		} else {
			if (changeLayerOnOpenClose && !ajar) {
				SetCollisionLayerClosed();
			}
			doorOpen = DoorState.Closed;
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
		if (ud == null) return;
		if (ud.owner == null) return;
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) { MFDManager.a.BlockedBySecurity(transform.position,ud); return; }

		// SHODAN can go anywhere!  Full security override!
		if (LevelManager.a.superoverride || Const.a.difficultyMission <= 0) {
			locked = false;
			requiredAccessCard = AccessCardType.None;
			accessCardUsedByPlayer = true;
		}

		if (Const.a.difficultyMission <= 1) {
			requiredAccessCard = AccessCardType.None;
			accessCardUsedByPlayer = true;
		}

		AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(defIndex);
		animatorPlaybackTime = asi.normalizedTime;
		if (useFinished < PauseScript.a.relativeTime) {
			if (requiredAccessCard == AccessCardType.None || Inventory.a.HasAccessCard(requiredAccessCard) || accessCardUsedByPlayer) {
				useFinished = PauseScript.a.relativeTime + useTimeDelay;	
				if (!locked) {
					if (requiredAccessCard != AccessCardType.None) {
						Const.sprint(Inventory.AccessCardCodeForType(requiredAccessCard) + cardUsedMessage,ud.owner); // state that we just used a keycard and access was granted
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

					if (doorOpen == DoorState.Open && animatorPlaybackTime > 0.95f) {
						doorOpen = DoorState.Closing;
						CloseDoor();
						return;
					}

					if (doorOpen == DoorState.Closed && animatorPlaybackTime > 0.95f){
						doorOpen = DoorState.Opening;
						OpenDoor();
						return;
					}

					if (doorOpen == DoorState.Opening) {
						doorOpen = DoorState.Closing;
						anim.Play(closeClipName,defIndex, topTime-animatorPlaybackTime);
						Utils.PlayOneShotSavable(SFX,SFXClip);
						return;
					}

					if (doorOpen == DoorState.Closing) {
						doorOpen = DoorState.Opening;
						waitBeforeClose = PauseScript.a.relativeTime + delay;
						anim.Play(openClipName,defIndex, topTime-animatorPlaybackTime);
						Utils.PlayOneShotSavable(SFX,SFXClip);
						return;
					}
				} else {
					if (requiredAccessCard != AccessCardType.None) {
						Const.sprint (requiredAccessCard.ToString() + cardUsedMessage + butdoorStillLockedMessage,ud.owner);
						accessCardUsedByPlayer = true;
					} else {
						Const.sprint(lockedMessage,ud.owner); // tell the owner of the Use command that we are locked
						if (QuestLogNotesManager.a != null) QuestLogNotesManager.a.NotifyLockedDoorAttempt(this);
					}
				}
			} else {
				Const.sprint(requiredAccessCard.ToString() + cardMessage,ud.owner); // tell the owner of the Use command that we are locked
			}
		}
	}

	void Targetted (UseData ud) {
		if (locked) {
			locked = false;
			if (QuestLogNotesManager.a != null) QuestLogNotesManager.a.NotifyDoorUnlock(this);
		}

		if (!targettingOnlyUnlocks) Use(ud);
	}

	public void ForceOpen() {
		if (doorOpen == DoorState.Open) return;
		OpenDoor();
	}

	public void ForceClose() {
		if (doorOpen == DoorState.Closed) return;
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
		doorOpen = DoorState.Opening;
		waitBeforeClose = PauseScript.a.relativeTime + delay;
		anim.Play(openClipName);
		Utils.PlayOneShotSavable(SFX,SFXClip);
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
		doorOpen = DoorState.Closing;
		anim.Play(closeClipName);
		Utils.PlayOneShotSavable(SFX,SFXClip);
		if (toggleLasers) {
			lasersFinished = PauseScript.a.relativeTime + timeBeforeLasersOn;
		}
		dynamicObjectsContainer = LevelManager.a.GetCurrentDynamicContainer();
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

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (firstUpdateAfterLoad) {
				firstUpdateAfterLoad = false;
				anim.Play(loadedClipName,loadedClipIndex,loadedAnimatorPlaybackTime);
				switch(loadedClipName) {
					case "IdleOpen": doorOpen = DoorState.Open; break;
					case "IdleClosed": doorOpen = DoorState.Closed; break;
					case "DoorOpen": doorOpen = DoorState.Opening; break;
					case "DoorClose": doorOpen = DoorState.Closing; break;
				}
				return;
			}

			if (ajar) {
				doorOpen = DoorState.Opening;
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

			if (doorOpen == DoorState.Closing || doorOpen == DoorState.Opening) {
				AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(defIndex);
				animatorPlaybackTime = asi.normalizedTime;
				if (doorOpen == DoorState.Closing && animatorPlaybackTime > 0.95f) doorOpen = DoorState.Closed; // Door is closed
				if (doorOpen == DoorState.Opening && animatorPlaybackTime > 0.95f) doorOpen = DoorState.Open; // Door is open
			}

			if (PauseScript.a.relativeTime > waitBeforeClose) {
				if ((doorOpen == DoorState.Open) && (!stayOpen) && (!startOpen)) CloseDoor();
			}

			if (toggleLasers) {
				if (lasersFinished < PauseScript.a.relativeTime && lasersFinished > 0) {
					for (int i=defIndex;i<laserLines.Length;i++) {
						if (!laserLines[i].activeSelf) laserLines[i].SetActive(true);
					}
					lasersFinished = 0;
				}
			}

			if (doorOpen == DoorState.Open) {
				if (nmo.enabled) nmo.enabled = false; // clear path, door is open
			} else {
				if (!nmo.enabled) nmo.enabled = true; // door is not opened fully, treat as a closed path
			}
		} else anim.speed = speedZero;
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

	public void SetAnimFromLoad(string n, int i, float t) {
		firstUpdateAfterLoad = true;
		loadedClipName = n;
		loadedClipIndex = i;
		loadedAnimatorPlaybackTime = t;
	}

	public static string Save(GameObject go, PrefabIdentifier prefID) {
		Door dr = go.GetComponent<Door>();
		if (dr == null) {
			Debug.Log("SearchableItem missing on savetype of SearchableItem!  GameObject.name: " + go.name);
			return "0|0|0|0000.00000|0000.00000|0000.00000|0|0|2|0000.00000";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(dr.targetAlreadyDone); // bool - have we already ran targets
		line += Utils.splitChar + Utils.BoolToString(dr.locked); // bool - is this locked?
		line += Utils.splitChar + Utils.BoolToString(dr.ajar); // bool - is this locked?
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(dr.useFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(dr.waitBeforeClose); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(dr.lasersFinished); // float
		line += Utils.splitChar + Utils.BoolToString(dr.blocked); // bool - is the door blocked currently?
		line += Utils.splitChar + Utils.BoolToString(dr.accessCardUsedByPlayer); // bool - is the door blocked currently?
		switch (dr.doorOpen) {
			case DoorState.Closed: line += Utils.splitChar + "0"; break;
			case DoorState.Open: line += Utils.splitChar + "1"; break;
			case DoorState.Closing: line += Utils.splitChar + "2"; break;
			case DoorState.Opening: line += Utils.splitChar + "3"; break;
		}
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(dr.animatorPlaybackTime); // float - current animation time
		if (prefID.constIndex == 500) { // doorE
			line += Utils.splitChar + Utils.SaveTransform(go.transform.parent.transform);
		}
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index,
						   PrefabIdentifier prefID) {
		Door dr = go.GetComponent<Door>();
		if (dr == null || index < 0 || entries == null) return index + 10;

		dr.targetAlreadyDone = Utils.GetBoolFromString(entries[index]); index++; // bool - have we already ran targets
		dr.locked = Utils.GetBoolFromString(entries[index]); index++; // bool - is this locked?
		dr.ajar = Utils.GetBoolFromString(entries[index]); index++; // bool - is this ajar?
		dr.useFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		dr.waitBeforeClose = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		dr.lasersFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		dr.blocked = Utils.GetBoolFromString(entries[index]); index++; // bool - is the door blocked currently?
		dr.accessCardUsedByPlayer = Utils.GetBoolFromString(entries[index]); index++; // bool - is the door blocked currently?
		int state = Utils.GetIntFromString(entries[index]); index++;
		string clipName = "IdleClosed";
		switch (state) {
			case 0: dr.doorOpen = DoorState.Closed; clipName = "IdleClosed"; break;
			case 1: dr.doorOpen = DoorState.Open; clipName = "IdleOpen"; break;
			case 2: dr.doorOpen = DoorState.Closing; clipName = "DoorClose"; break;
			case 3: dr.doorOpen = DoorState.Opening; clipName = "DoorOpen"; break;
		}
		dr.animatorPlaybackTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		dr.SetAnimFromLoad(clipName,0,dr.animatorPlaybackTime); index++;
		if (prefID.constIndex == 500) { // doorE
			Transform parentTR = go.transform.parent.transform;
			index = Utils.LoadTransform(parentTR,ref entries,index);
		}
		return index;
	}
}
