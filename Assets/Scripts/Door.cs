using UnityEngine;
using System.Collections;
using System.Text;

public class Door : MonoBehaviour {
	public string target;
	public string argvalue;
	public bool onlyTargetOnce;
	[Tooltip("Delay after full open before door closes")] public float delay;
	[Tooltip("Whether door is locked, unuseable until unlocked")] public bool locked; // saved
	public int securityThreshhold = 100; // If security level is not below this level, this is unusable.
	[Tooltip("If yes, door never closes automatically")] public bool stayOpen;
	[Tooltip("Should door start open (you should set stayOpen too!)")] public bool startOpen;
	[Tooltip("Should door start partially open")] public bool ajar = false; // save
	[Tooltip("If partially open, by what percentage")] public float ajarPercentage = 0.5f;
	[Tooltip("Delay after use before door can be re-used")] public float useTimeDelay = 0.15f;
	public int lockedMessageLingdex = 3;
	public bool blocked = false; // save
	[Tooltip("Door sound when opening or closing")] public int SFXIndex = 75;
	public AccessCardType requiredAccessCard = AccessCardType.None;
	public bool accessCardUsedByPlayer = false; // save
	public DoorState doorOpen; // save
	public float timeBeforeLasersOn;
	public GameObject[] laserLines;
	public GameObject[] collidersList;
	public bool toggleLasers = false;
	public bool targettingOnlyUnlocks = false;
	public float animatorPlaybackTime; // save
	public bool changeLayerOnOpenClose = false;

	[HideInInspector] public bool targetAlreadyDone = false; // save
	[HideInInspector] public float lasersFinished; // save
	[HideInInspector] public float useFinished; // save
	[HideInInspector] public float waitBeforeClose; // save
	[HideInInspector] public Animator anim;
	private AudioSource SFX = null;
	private GameObject dynamicObjectsContainer;
	private const float topTime = 1.00f;
	private const float defaultSpeed = 1.00f;
	private const float speedZero = 0.00f;
	private const string idleOpenClipName = "IdleOpen";
	private const string idleClosedClipName = "IdleClosed";
	private const string openClipName = "DoorOpen";
	private const string closeClipName = "DoorClose";
	private int i = 0;
	private bool firstUpdateAfterLoad = false;
	private string loadedClipName;
	private int loadedClipIndex;
	private float loadedAnimatorPlaybackTime;
	private bool initialized = false;
	private AnimatorStateInfo asi;
	private bool delayFrame = false;
	private static StringBuilder s1 = new StringBuilder();

	void Start () {
		if (initialized) return;

		anim = GetComponent<Animator>();
		animatorPlaybackTime = 0;
		if (requiredAccessCard == AccessCardType.None) {
			accessCardUsedByPlayer = true;
		}
		
		SFX = GetComponent<AudioSource>();		
		useFinished = PauseScript.a.relativeTime;
		if (startOpen) {
			stayOpen = true;
			OpenDoor();
		} else {
			if (!ajar) SetCollisionLayer(18); // Door
			doorOpen = DoorState.Closed;
			anim.Play(idleClosedClipName,0,0f);
		}

		initialized = true;
		asi = anim.GetCurrentAnimatorStateInfo(0);
		delayFrame = false;
	}

	public void Use (UseData ud) {
		if (ud == null) return;
		if (ud.owner == null) return;
		
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position);
			return;
		}

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

		asi = anim.GetCurrentAnimatorStateInfo(0);
		animatorPlaybackTime = asi.normalizedTime;
		if (useFinished >= PauseScript.a.relativeTime) return;

		useFinished = PauseScript.a.relativeTime + useTimeDelay;	
		if (requiredAccessCard == AccessCardType.None
			|| Inventory.a.HasAccessCard(requiredAccessCard)
			|| accessCardUsedByPlayer) {

			if (!locked) {
				if (requiredAccessCard != AccessCardType.None) {
					// State that we just used a keycard and access was granted
					Const.sprint(Inventory.AccessCardCodeForType(requiredAccessCard) + Const.a.stringTable[4]);
					accessCardUsedByPlayer = true;
				}

				if ((onlyTargetOnce && !targetAlreadyDone) || !onlyTargetOnce) {
					targetAlreadyDone = true;
					ud.argvalue = argvalue;
					Const.a.UseTargets(gameObject,ud,target);
				}

				if (ajar) {
					ajar = false;
					animatorPlaybackTime = topTime * ajarPercentage;
				}

				DoorActuate();
			} else {
				// Use access card
				if (requiredAccessCard != AccessCardType.None) {
					Const.sprint(requiredAccessCard.ToString() + Const.a.stringTable[4] + Const.a.stringTable[5]);
					accessCardUsedByPlayer = true;
				} else {
					Const.sprint(lockedMessageLingdex); 
					Utils.PlayOneShotSavable(SFX,Const.a.sounds[467],0.55f);
					if (QuestLogNotesManager.a != null) {
						QuestLogNotesManager.a.NotifyLockedDoorAttempt(this);
					}
				}
			}
		} else {
			// Tell owner of the Use command that an access card is needed.
			Const.sprint(requiredAccessCard.ToString() + Const.a.stringTable[2]);
			Utils.PlayOneShotSavable(SFX,Const.a.sounds[466],0.7f);
		}
	}
	
	public void DoorActuate() {
		asi = anim.GetCurrentAnimatorStateInfo(0);
		animatorPlaybackTime = asi.normalizedTime;
		if (doorOpen == DoorState.Open && animatorPlaybackTime > 0.95f) {
			doorOpen = DoorState.Closing;
			CloseDoor();
			delayFrame = true;
		} else if (doorOpen == DoorState.Closed && animatorPlaybackTime > 0.95f){
			doorOpen = DoorState.Opening;
			OpenDoor();
			delayFrame = true;
		} else if (doorOpen == DoorState.Opening) {
			doorOpen = DoorState.Closing;
			anim.Play(closeClipName,0,topTime - animatorPlaybackTime);
			Utils.PlayOneShotSavable(SFX,Const.a.sounds[SFXIndex]);
			delayFrame = true;
		} else if (doorOpen == DoorState.Closing) {
			doorOpen = DoorState.Opening;
			waitBeforeClose = PauseScript.a.relativeTime + delay;
			anim.Play(openClipName,0,topTime - animatorPlaybackTime);
			Utils.PlayOneShotSavable(SFX,Const.a.sounds[SFXIndex]);
			delayFrame = true;
		}
	}

	void Targetted (UseData ud) {
		if (locked) {
			locked = false;
			if (QuestLogNotesManager.a != null) {
				QuestLogNotesManager.a.NotifyDoorUnlock(this);
			}
		}

		if (!targettingOnlyUnlocks) Use(ud);
	}

	public void ForceOpen() {
		if (doorOpen == DoorState.Open) return;

		OpenDoor();
		delayFrame = true;
	}

	public void ForceClose() {
		if (doorOpen == DoorState.Closed) return;

		CloseDoor();
		delayFrame = true;
	}

	public void Lock() {
		locked = true;
	}

	public void Unlock() {
		locked = false;
		if (QuestLogNotesManager.a != null) {
			QuestLogNotesManager.a.NotifyDoorUnlock(this);
		}
	}

	public void ToggleLocked() {
		if (locked) {
			Unlock();
		} else {
			Lock();
		}
	}

	void DeactivateLasers() {
		for (int i=0;i<laserLines.Length;i++) {
			if (laserLines[i].activeSelf) laserLines[i].SetActive(false);
		}
	}

	void OpenDoor() {
		if (anim == null) anim = GetComponent<Animator>();
		if (anim != null) anim.speed = defaultSpeed;
		doorOpen = DoorState.Opening;
		waitBeforeClose = PauseScript.a.relativeTime + delay;
		if (anim != null) anim.Play(openClipName,0,0f);
		Utils.PlayOneShotSavable(SFX,Const.a.sounds[SFXIndex]);
		SetCollisionLayer(19); // InterDebris
	}

	void CloseDoor() {
		if (anim == null) anim = GetComponent<Animator>();
		if (anim != null) anim.speed = defaultSpeed;
		doorOpen = DoorState.Closing;
		if (anim != null) anim.Play(closeClipName,0,0f);
		Utils.PlayOneShotSavable(SFX,Const.a.sounds[SFXIndex]);
		dynamicObjectsContainer = LevelManager.a.GetCurrentDynamicContainer();

		// Horrible hack to keep objects that have their physics sleeping from
		// ghosting through the door as it closes.  Unity physics sucks.
		Vector3 objPos;
		GameObject childGO;
		for (i=0;i<dynamicObjectsContainer.transform.childCount;i++) {
			childGO = dynamicObjectsContainer.transform.GetChild(i).gameObject;
			objPos = childGO.transform.position;
			if (Vector3.Distance(transform.position,objPos) < 5) {
				Rigidbody childRbody = childGO.GetComponent<Rigidbody>();
				if (childRbody != null) childRbody.WakeUp(); // No ghosting!
			}
		}

		SetCollisionLayer(18); // Door
	}

	void SetAnimAfterLoad() {
		firstUpdateAfterLoad = false;
		if (anim == null) anim = GetComponent<Animator>();
		if (anim != null) anim.Play(loadedClipName,loadedClipIndex,loadedAnimatorPlaybackTime);
		delayFrame = true;
		switch(loadedClipName) {
			case idleOpenClipName: doorOpen = DoorState.Open; break;
			case idleClosedClipName: doorOpen = DoorState.Closed; break;
			case openClipName: doorOpen = DoorState.Opening; break;
			case closeClipName: doorOpen = DoorState.Closing; break;
		}
	}

	void SetAjar() {
		doorOpen = DoorState.Opening;
		if (toggleLasers) DeactivateLasers();
		if (anim == null) anim = GetComponent<Animator>();
		if (anim != null) anim.Play(openClipName,0,ajarPercentage);
		if (anim != null) anim.speed = speedZero;
	}

	void ActivateLasers() {
		for (int i=0;i<laserLines.Length;i++) {
			if (!laserLines[i].activeSelf) laserLines[i].SetActive(true);
		}
	}

	void Update() {
		if (PauseScript.a.Paused()) { anim.speed = speedZero; return; }
		if (PauseScript.a.MenuActive()) { anim.speed = speedZero; return; }
		if (firstUpdateAfterLoad) { SetAnimAfterLoad(); return; }
		if (ajar) { SetAjar(); return; }
			
		if (blocked) Blocked();
		else Unblocked();

		if (doorOpen == DoorState.Closing || doorOpen == DoorState.Opening) {
			AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);
			animatorPlaybackTime = asi.normalizedTime;
			if (doorOpen == DoorState.Closing && animatorPlaybackTime > 0.95f && !delayFrame) {
				doorOpen = DoorState.Closed; // Door is closed
			}
			
			if (doorOpen == DoorState.Opening && animatorPlaybackTime > 0.95f && !delayFrame) {
				doorOpen = DoorState.Open; // Door is open
			}
		}

		if (PauseScript.a.relativeTime > waitBeforeClose) {
			if ((doorOpen == DoorState.Open) && (!stayOpen) && (!startOpen) && !delayFrame) {
				Debug.Log("Close Door, stayOpen: " + stayOpen.ToString());
				CloseDoor();
			}
		}
		
		if (toggleLasers) {
			if (doorOpen == DoorState.Closed) {
				ActivateLasers();
			} else {
				DeactivateLasers();
			} 
		}
		
		delayFrame = false; // Handle race condition when setting anim prior to Update() running.
	}

	void SetCollisionLayer(int layerNum) {
		if (!changeLayerOnOpenClose) return;

		for (int i=0;i<collidersList.Length;i++) {
			collidersList[i].layer = layerNum; // InterDebris
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

	public string GetClipName() {
		string clipName = idleClosedClipName;
		switch (doorOpen) {
			case DoorState.Closed: clipName = idleClosedClipName; break;
			case DoorState.Open: clipName = idleOpenClipName; break;
			case DoorState.Closing: clipName = closeClipName; break;
			case DoorState.Opening: clipName = openClipName; break;
		}
		return clipName;
	}

	void OnDisable() {
		AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);
		loadedClipName = GetClipName();
		loadedClipIndex = 0;
		loadedAnimatorPlaybackTime = asi.normalizedTime;
		firstUpdateAfterLoad = true;
	}

	public static string Save(GameObject go, PrefabIdentifier prefID) {
		Door dr = go.GetComponent<Door>();
		s1.Clear();
		s1.Append(Utils.SaveString(dr.target,"target"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveString(dr.argvalue,"argvalue"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.onlyTargetOnce,"onlyTargetOnce"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.targetAlreadyDone,"targetAlreadyDone"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(dr.delay,"delay"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.locked,"locked"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(dr.securityThreshhold,"securityThreshhold"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.stayOpen,"stayOpen"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.startOpen,"startOpen"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.ajar,"ajar"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(dr.ajarPercentage,"ajarPercentage"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(dr.useTimeDelay,"useTimeDelay"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(dr.lockedMessageLingdex,"lockedMessageLingdex"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.blocked,"blocked"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(dr.useFinished,"useFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(dr.waitBeforeClose,"waitBeforeClose"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(Utils.AccessCardTypeToInt(dr.requiredAccessCard),"requiredAccessCard"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(dr.timeBeforeLasersOn,"timeBeforeLasersOn"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(dr.lasersFinished,"lasersFinished"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.accessCardUsedByPlayer,"accessCardUsedByPlayer"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.toggleLasers,"toggleLasers"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.targettingOnlyUnlocks,"targettingOnlyUnlocks"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.changeLayerOnOpenClose,"changeLayerOnOpenClose"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(DoorStateToInt(dr.doorOpen),"doorOpenState"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(dr.animatorPlaybackTime,"animatorPlaybackTime"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index,
						   PrefabIdentifier prefID) {
		Door dr = go.GetComponent<Door>();
		if (dr == null) {
			Debug.Log("Door.Load failure, dr == null on " + go.name);
			return index + 10;
		}

		if (index < 0) {
			Debug.Log("Door.Load failure, index < 0");
			return index + 10;
		}

		if (entries == null) {
			Debug.Log("Door.Load failure, entries == null");
			return index + 10;
		}

		dr.target = Utils.LoadString(entries[index],"target"); index++;
		dr.argvalue = Utils.LoadString(entries[index],"argvalue"); index++;
		dr.onlyTargetOnce = Utils.GetBoolFromString(entries[index],"onlyTargetOnce"); index++;
		dr.targetAlreadyDone = Utils.GetBoolFromString(entries[index],"targetAlreadyDone"); index++;
		dr.delay = Utils.GetFloatFromString(entries[index],"delay"); index++;
		dr.locked = Utils.GetBoolFromString(entries[index],"locked"); index++;
		dr.securityThreshhold = Utils.GetIntFromString(entries[index],"securityThreshhold"); index++;
		dr.stayOpen = Utils.GetBoolFromString(entries[index],"stayOpen"); index++;
		dr.startOpen = Utils.GetBoolFromString(entries[index],"startOpen"); index++;
		dr.ajar = Utils.GetBoolFromString(entries[index],"ajar"); index++;
		dr.ajarPercentage = Utils.GetFloatFromString(entries[index],"ajarPercentage"); index++;
		dr.useTimeDelay = Utils.GetFloatFromString(entries[index],"useTimeDelay"); index++;
		dr.lockedMessageLingdex = Utils.GetIntFromString(entries[index],"lockedMessageLingdex"); index++;
		dr.blocked = Utils.GetBoolFromString(entries[index],"blocked"); index++;
		dr.useFinished = Utils.LoadRelativeTimeDifferential(entries[index],"useFinished"); index++;
		dr.waitBeforeClose = Utils.LoadRelativeTimeDifferential(entries[index],"waitBeforeClose"); index++;
		dr.requiredAccessCard = Utils.IntToAccessCardType(Utils.GetIntFromString(entries[index],"requiredAccessCard")); index++;
		dr.timeBeforeLasersOn = Utils.GetFloatFromString(entries[index],"timeBeforeLasersOn"); index++;
		dr.lasersFinished = Utils.LoadRelativeTimeDifferential(entries[index],"lasersFinished"); index++;
		dr.accessCardUsedByPlayer = Utils.GetBoolFromString(entries[index],"accessCardUsedByPlayer"); index++;
		dr.toggleLasers = Utils.GetBoolFromString(entries[index],"toggleLasers"); index++;
		dr.targettingOnlyUnlocks = Utils.GetBoolFromString(entries[index],"targettingOnlyUnlocks"); index++;
		dr.changeLayerOnOpenClose = Utils.GetBoolFromString(entries[index],"changeLayerOnOpenClose"); index++;
		dr.doorOpen = IntToDoorState(Utils.GetIntFromString(entries[index],"doorOpenState")); index++;
		dr.animatorPlaybackTime = Utils.GetFloatFromString(entries[index],"animatorPlaybackTime"); index++;
		dr.SetAnimFromLoad(dr.GetClipName(),0,dr.animatorPlaybackTime);
		return index;
	}

	public static int DoorStateToInt(DoorState state) {
		switch (state) {
			case DoorState.Closed:  return 0;
			case DoorState.Open:    return 1;
			case DoorState.Closing: return 2;
			case DoorState.Opening: return 3;
		}
		return 0;
	}

	public static DoorState IntToDoorState(int state) {
		switch (state) {
			case 0: return DoorState.Closed;
			case 1: return DoorState.Open;
			case 2: return DoorState.Closing;
			case 3: return DoorState.Opening;
		}
		return DoorState.Closed;
	}
}
