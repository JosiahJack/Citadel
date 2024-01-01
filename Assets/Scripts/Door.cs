using UnityEngine;
using System.Collections;
using System.Text;

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
	public int securityThreshhold = 100; // If security level is not below this
										 // level, this is unusable.

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
	private bool initialized = false;

	void Start () {
		if (initialized) return;

		anim = GetComponent<Animator>();
		animatorPlaybackTime = 0;
		if (requiredAccessCard == AccessCardType.None) {
			accessCardUsedByPlayer = true;
		}
		
		SFX = GetComponent<AudioSource>();
		if (SFX == null) Debug.Log("BUG: No AudioSource on Door!");
		if (SFXClip == null) Debug.Log("BUG: No audio clip SFXClip on Door!");
		
		useFinished = PauseScript.a.relativeTime;
		nmo = GetComponent<UnityEngine.AI.NavMeshObstacle>();
		if (nmo == null) {
			Const.sprint("BUG: Missing NavMeshObstacle on Door at "
						 + transform.position.ToString());
		} else nmo.carving = true; // Creates a "hole" in the NavMesh forcing 
								   // enemies to find an alternate route.
		if (startOpen) {
			stayOpen = true;
			OpenDoor();
		} else {
			if (!ajar) SetCollisionLayer(18); // Door
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
		initialized = true;
	}

	public void Use (UseData ud) {
		if (ud == null) return;
		if (ud.owner == null) return;
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position,ud);
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

		AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(defIndex);
		animatorPlaybackTime = asi.normalizedTime;
		if (useFinished >= PauseScript.a.relativeTime) return;

		useFinished = PauseScript.a.relativeTime + useTimeDelay;	
		if (requiredAccessCard == AccessCardType.None
			|| Inventory.a.HasAccessCard(requiredAccessCard)
			|| accessCardUsedByPlayer) {

			if (!locked) {
				if (requiredAccessCard != AccessCardType.None) {
					// State that we just used a keycard and access was granted
					Const.sprint(Inventory.AccessCardCodeForType(requiredAccessCard)
									+ cardUsedMessage,ud.owner);
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

				if (doorOpen == DoorState.Open
					&& animatorPlaybackTime > 0.95f) {

					doorOpen = DoorState.Closing;
					CloseDoor();
				} else if (doorOpen == DoorState.Closed
						   && animatorPlaybackTime > 0.95f){

					doorOpen = DoorState.Opening;
					OpenDoor();
				} else if (doorOpen == DoorState.Opening) {
					doorOpen = DoorState.Closing;
					anim.Play(closeClipName,defIndex,
								topTime - animatorPlaybackTime);

					Utils.PlayOneShotSavable(SFX,SFXClip);
				} else if (doorOpen == DoorState.Closing) {
					doorOpen = DoorState.Opening;
					waitBeforeClose = PauseScript.a.relativeTime + delay;
					anim.Play(openClipName,defIndex,
								topTime - animatorPlaybackTime);

					Utils.PlayOneShotSavable(SFX,SFXClip);
				}
			} else {
				// Use access card
				if (requiredAccessCard != AccessCardType.None) {
					Const.sprint(requiredAccessCard.ToString()
									+ cardUsedMessage
									+ butdoorStillLockedMessage,ud.owner);

					accessCardUsedByPlayer = true;
				} else {
					
					Const.sprint(lockedMessage,ud.owner); 
					if (QuestLogNotesManager.a != null) {
						QuestLogNotesManager.a.NotifyLockedDoorAttempt(this);
					}
				}
			}
		} else {
			// Tell owner of the Use command that an access card is needed.
			Const.sprint(requiredAccessCard.ToString()
							+ cardMessage,ud.owner);
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
	}

	public void ForceClose() {
		if (doorOpen == DoorState.Closed) return;

		CloseDoor();
	}

	public void Lock(string arg) {
		locked = true;
		if (string.IsNullOrWhiteSpace(arg)) arg = "Door is locked"; // default
		lockedMessage = arg;
	}

	public void Unlock() {
		locked = false;
		if (QuestLogNotesManager.a != null) {
			QuestLogNotesManager.a.NotifyDoorUnlock(this);
		}
	}

	public void ToggleLocked(string arg) {
		if (locked) {
			Unlock();
		} else {
			Lock(arg);
		}
	}

	void DeactivateLasers() {
		for (int i=defIndex;i<laserLines.Length;i++) {
			if (laserLines[i].activeSelf) laserLines[i].SetActive(false);
		}
	}

	void OpenDoor() {
		anim.speed = defaultSpeed;
		doorOpen = DoorState.Opening;
		waitBeforeClose = PauseScript.a.relativeTime + delay;
		anim.Play(openClipName);
		Utils.PlayOneShotSavable(SFX,SFXClip);
		if (toggleLasers) {
			DeactivateLasers();
			lasersFinished = 0; // Checked explicitly for non-zero elsewhere.
		}

		SetCollisionLayer(19); // InterDebris
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

		// Horrible hack to keep objects that have their physics sleeping from
		// ghosting through the door as it closes.  Unity physics sucks.
		Vector3 objPos;
		GameObject childGO;
		for (i=defIndex;i<dynamicObjectsContainer.transform.childCount;i++) {
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
		anim.Play(loadedClipName,loadedClipIndex,loadedAnimatorPlaybackTime);
		switch(loadedClipName) {
			case "IdleOpen": doorOpen = DoorState.Open; break;
			case "IdleClosed": doorOpen = DoorState.Closed; break;
			case "DoorOpen": doorOpen = DoorState.Opening; break;
			case "DoorClose": doorOpen = DoorState.Closing; break;
		}
	}

	void SetAjar() {
		doorOpen = DoorState.Opening;
		anim.Play(openClipName,defIndex, ajarPercentage);
		anim.speed = speedZero;
	}

	void ActivateLasers() {
		for (int i=defIndex;i<laserLines.Length;i++) {
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
			AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(defIndex);
			animatorPlaybackTime = asi.normalizedTime;
			if (doorOpen == DoorState.Closing && animatorPlaybackTime > 0.95f) {
				doorOpen = DoorState.Closed; // Door is closed
				ActivateLasers();
			}

			if (doorOpen == DoorState.Opening && animatorPlaybackTime > 0.95f) {
				doorOpen = DoorState.Open; // Door is open
				DeactivateLasers();
			}
		}

		if (PauseScript.a.relativeTime > waitBeforeClose) {
			if ((doorOpen == DoorState.Open) && (!stayOpen) && (!startOpen)) {
				CloseDoor();
			}
		}

		if (toggleLasers && lasersFinished > 0 
			&& lasersFinished < PauseScript.a.relativeTime) {

			ActivateLasers();
			lasersFinished = 0;
		}

		if (doorOpen == DoorState.Open) Utils.DisableNavMeshObstacle(nmo); 
		else Utils.EnableNavMeshObstacle(nmo); // Only open if fully open.
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
		string clipName = "IdleClosed";
		switch (doorOpen) {
			case DoorState.Closed: clipName = "IdleClosed"; break;
			case DoorState.Open: clipName = "IdleOpen"; break;
			case DoorState.Closing: clipName = "DoorClose"; break;
			case DoorState.Opening: clipName = "DoorOpen"; break;
		}
		return clipName;
	}

	void OnDisable() {
		AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(defIndex);
		loadedClipName = GetClipName();
		loadedClipIndex = 0;
		loadedAnimatorPlaybackTime = asi.normalizedTime;
		firstUpdateAfterLoad = true;
	}

	public static string Save(GameObject go, PrefabIdentifier prefID) {
		Door dr = go.GetComponent<Door>();
		if (dr == null) {
			Debug.Log("SearchableItem missing on savetype of SearchableItem!  "
					  + "GameObject.name: " + go.name);
			return "0|0|0|0000.00000|0000.00000|0000.00000|0|0|2|0000.00000";
		}

		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(dr.target);
		s1.Append(Utils.splitChar);
		s1.Append(dr.argvalue);
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.onlyTargetOnce));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.targetAlreadyDone));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(dr.delay));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.locked));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(dr.securityThreshhold));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.stayOpen));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.startOpen));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.ajar));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(dr.ajarPercentage));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(dr.useTimeDelay));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(dr.lockedMessageLingdex));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.blocked));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(dr.useFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(dr.waitBeforeClose));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.AccessCardTypeToInt(dr.requiredAccessCard));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(dr.timeBeforeLasersOn));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(dr.lasersFinished));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.accessCardUsedByPlayer));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.toggleLasers));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.targettingOnlyUnlocks));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(dr.changeLayerOnOpenClose));
		s1.Append(Utils.splitChar);
		s1.Append(DoorStateToInt(dr.doorOpen));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(dr.animatorPlaybackTime));
		if (prefID.constIndex == 500) { // doorE
			s1.Append(Utils.splitChar);
			s1.Append(Utils.SaveTransform(go.transform.parent.transform));
		}

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

		dr.target = entries[index]; index++;
		dr.argvalue = entries[index]; index++;
		dr.onlyTargetOnce = Utils.GetBoolFromString(entries[index]); index++;
		dr.targetAlreadyDone = Utils.GetBoolFromString(entries[index]); index++;
		dr.delay = Utils.GetFloatFromString(entries[index]); index++;
		dr.locked = Utils.GetBoolFromString(entries[index]); index++;
		dr.securityThreshhold = Utils.GetIntFromString(entries[index]); index++;
		dr.stayOpen = Utils.GetBoolFromString(entries[index]); index++;
		dr.startOpen = Utils.GetBoolFromString(entries[index]); index++;
		dr.ajar = Utils.GetBoolFromString(entries[index]); index++;
		dr.ajarPercentage = Utils.GetFloatFromString(entries[index]); index++;
		dr.useTimeDelay = Utils.GetFloatFromString(entries[index]); index++;
		dr.lockedMessageLingdex = Utils.GetIntFromString(entries[index]); index++;
		dr.blocked = Utils.GetBoolFromString(entries[index]); index++;
		dr.useFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		dr.waitBeforeClose = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		dr.requiredAccessCard = Utils.IntToAccessCardType(Utils.GetIntFromString(entries[index])); index++;
		dr.timeBeforeLasersOn = Utils.GetFloatFromString(entries[index]); index++;
		dr.lasersFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		dr.accessCardUsedByPlayer = Utils.GetBoolFromString(entries[index]); index++;
		dr.toggleLasers = Utils.GetBoolFromString(entries[index]); index++;
		dr.targettingOnlyUnlocks = Utils.GetBoolFromString(entries[index]); index++;
		dr.changeLayerOnOpenClose = Utils.GetBoolFromString(entries[index]); index++;
		dr.doorOpen = IntToDoorState(Utils.GetIntFromString(entries[index])); index++;
		dr.animatorPlaybackTime = Utils.GetFloatFromString(entries[index]); index++;
		dr.SetAnimFromLoad(dr.GetClipName(),0,dr.animatorPlaybackTime);
		if (prefID.constIndex == 500) { // doorE
			Transform parentTR = go.transform.parent.transform;
			index = Utils.LoadTransform(parentTR,ref entries,index);
		}
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
