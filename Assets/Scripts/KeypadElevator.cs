using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeypadElevator : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public DataTab dataTabResetter;
	public GameObject elevatorControl;
	public GameObject[] elevatorButtonHandlers;
	public GameObject[] targetDestination;
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
	private GameObject levelManager;
	private GameObject playerCapsule;
	private GameObject playerCamera;
	private float distanceCheck;
	private int defIndex = 0;
	private int maxIndex = 8;
	private int four = 4;
	private float tickFinished;
	public float tickSecs = 0.1f;
	public string blockedBySecurityText = "Blocked by SHODAN level Security.";

	void Awake () {
		levelManager = GameObject.Find("LevelManager");
		if (levelManager == null)
			Const.sprint("Warning: Could not find object 'LevelManager' in scene",playerCapsule.transform.parent.gameObject);

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

		tickFinished = Time.time + tickSecs + Random.Range(0.1f,0.5f); // Random start to prevent tick calculations from bunching up in one frame
	}

	void Use (UseData ud) {
		if (LevelManager.a.levelSecurity[LevelManager.a.currentLevel] > securityThreshhold) {
			Const.sprint(blockedBySecurityText,ud.owner);
			MFDManager.a.BlockedBySecurity();
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
