using UnityEngine;
using System.Collections;

public class KeypadKeycode : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public DataTab dataTabResetter;
	public GameObject keypadControl;
	public int keycode; // the access code
	public GameObject target;
	public GameObject target1;
	public GameObject target2;
	public GameObject target3;
	public AudioClip SFX;
	private float disconnectDist;
	private AudioSource SFXSource;
	private bool padInUse = false;
	private GameObject playerCamera;
	private GameObject playerCapsule;

	void Start () {
		padInUse = false;
		disconnectDist = Const.a.frobDistance;
		SFXSource = GetComponent<AudioSource>();
	}

	void Use (UseData ud) {
		if (LevelManager.a.levelSecurity[LevelManager.a.currentLevel] > securityThreshhold) {
			Const.sprint("Blocked by SHODAN level Security.",ud.owner);
			MFDManager.a.BlockedBySecurity();
			return;
		}

		padInUse = true;
		SFXSource.PlayOneShot(SFX);
		dataTabResetter.Reset();
		keypadControl.SetActive(true);
		keypadControl.GetComponent<KeypadKeycodeButtons>().keycode = keycode;
		keypadControl.GetComponent<KeypadKeycodeButtons>().keypad = this;
		playerCamera = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera;
		playerCapsule = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsule; // Get player capsule of player using this pad
		playerCamera.GetComponent<MouseLookScript>().ForceInventoryMode();
		MFDManager.a.OpenTab(4,true,MFDManager.TabMSG.Keypad,0,MFDManager.handedness.LeftHand);
	}

	public void UseTargets () {
		if (target != null) {
			target.SendMessageUpwards("Targetted", playerCamera);
		}
		if (target1 != null) {
			target.SendMessageUpwards("Targetted", playerCamera);
		}
		if (target2 != null) {
			target.SendMessageUpwards("Targetted", playerCamera);
		}
		if (target3 != null) {
			target.SendMessageUpwards("Targetted", playerCamera);
		}
	}

	void Update () {
		if (padInUse) {
			if (Vector3.Distance(playerCapsule.transform.position, gameObject.transform.position) > disconnectDist) {
				padInUse = false;
				MFDManager.a.TurnOffKeypad();
				//keypadControl.SetActive(false);
			}
		}
	}
}
