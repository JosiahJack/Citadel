using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeypadElevator : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public DataTab dataTabResetter;
	public GameObject elevatorControl;
	public GameObject[] elevatorButtonHandlers;
	public GameObject[] targetDestination; // set by ElevatorKeypad.cs in Use(), which actually gets it from ElevatorButton.cs
	public Door linkedDoor;
	public bool[] buttonsEnabled;
	public bool[] buttonsDarkened;
	public string[] buttonText;
	public int currentFloor;
	public AudioClip SFX;
	private float disconnectDist;
	public float useDist = 2f;
	private AudioSource SFXSource;
	private bool padInUse;
	private GameObject playerCapsule;
	private GameObject playerCamera;
	private float distanceCheck;
	private int defIndex = 0;
	private int maxIndex = 8;
	private int four = 4;
	private float tickFinished;
	public float tickSecs = 0.1f;
	public bool locked = false;
	public string lockedTarget;
	public string argvalue;
	public string lockedMessage;
	public string blockedBySecurityText = "Blocked by SHODAN level Security.";

	void Awake () {
		if (targetDestination == null)
			Const.sprint("Warning: Could not find target destination for elevator keypad" + gameObject.name,playerCapsule.transform.parent.gameObject);
	}

	void Start () {
		padInUse = false;
		disconnectDist = Const.a.frobDistance;
		SFXSource = GetComponent<AudioSource>();
		for (int i=defIndex;i<maxIndex;i++) {
			elevatorButtonHandlers[i].GetComponent<ElevatorButton>().SetTooFarFalse();
		}

		tickFinished = Time.time + tickSecs + Random.value; // Random start to prevent tick calculations from bunching up in one frame
	}

	public void Use (UseData ud) {
		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			Const.sprint(blockedBySecurityText,ud.owner);
			MFDManager.a.BlockedBySecurity(transform.position);
			return;
		}

		if (LevelManager.a.superoverride) {
			// SHODAN can go anywhere!  Full security override!
			locked = false;
		}

		if (locked) {
			Const.sprint(lockedMessage,ud.owner);
			if (!string.IsNullOrWhiteSpace(lockedTarget)) {
				ud.argvalue = argvalue;
				TargetIO tio = GetComponent<TargetIO>();
				if (tio != null) {
					ud.SetBits(tio);
				} else {
					Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
				}
				Const.a.UseTargets(ud,lockedTarget); //target something because we are locked like a Vox message to say hey we are locked, e.g. "Non emergency life pods disabled."
			}
			return;
		}

		padInUse = true;
		SFXSource.PlayOneShot(SFX);
		dataTabResetter.Reset();
		elevatorControl.SetActive(true);
		playerCapsule = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsule; // Get player capsule of player using this pad
		playerCamera = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera;
		for (int i=defIndex;i<maxIndex;i++) {
			elevatorControl.GetComponent<ElevatorKeypad>().buttonsEnabled[i] = buttonsEnabled[i];
			elevatorControl.GetComponent<ElevatorKeypad>().buttonsDarkened[i] = buttonsDarkened[i];
			elevatorControl.GetComponent<ElevatorKeypad>().buttonText[i] = buttonText[i];
			elevatorControl.GetComponent<ElevatorKeypad>().targetDestination[i] = targetDestination[i];
		}
		elevatorControl.GetComponent<ElevatorKeypad>().currentFloor = currentFloor;
		elevatorControl.GetComponent<ElevatorKeypad>().activeKeypad = gameObject;
		if (playerCamera.GetComponent<MouseLookScript>().inventoryMode == false)
			playerCamera.GetComponent<MouseLookScript>().ToggleInventoryMode();

		MFDManager.a.OpenTab(four,true,MFDManager.TabMSG.Elevator,defIndex,MFDManager.handedness.LeftHand);
	}

	void Update () {
		// limit checks to only once every tickSecs to prevent CPU overload
		if (tickFinished < Time.time) {
			if (padInUse) {
				distanceCheck = Vector3.Distance (playerCapsule.transform.position, gameObject.transform.position);
				if (distanceCheck > disconnectDist) {
					padInUse = false;
					MFDManager.a.TurnOffElevatorPad ();
				} else {
					for (int i = defIndex; i < maxIndex; i++) {
						if (distanceCheck < useDist) {
							if (MFDManager.a.GetElevatorControlActiveState ())
								elevatorButtonHandlers [i].GetComponent<ElevatorButton> ().SetTooFarFalse ();
						} else {
							if (MFDManager.a.GetElevatorControlActiveState ())
								elevatorButtonHandlers [i].GetComponent<ElevatorButton> ().SetTooFarTrue ();
						}

						if (linkedDoor.doorOpen == Door.doorState.Closed) {
							if (MFDManager.a.GetElevatorControlActiveState ())
								elevatorButtonHandlers [i].GetComponent<ElevatorButton> ().doorOpen = false;
						} else {
							if (MFDManager.a.GetElevatorControlActiveState ())
								elevatorButtonHandlers [i].GetComponent<ElevatorButton> ().doorOpen = true;
						}
					}
				}
			}
		}
	}
}
