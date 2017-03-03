using UnityEngine;
using System.Collections;

public class KeypadKeycode : MonoBehaviour {
	public DataTab dataTabResetter;
	public GameObject keypadControl;
	public GameObject playerCapsule;
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

	void Start () {
		padInUse = false;
		disconnectDist = Const.a.frobDistance;
		SFXSource = GetComponent<AudioSource>();
	}

	void Use (GameObject owner) {
		padInUse = true;
		SFXSource.PlayOneShot(SFX);
		dataTabResetter.Reset();
		keypadControl.SetActive(true);
		keypadControl.GetComponent<KeypadKeycodeButtons>().keycode = keycode;
		keypadControl.GetComponent<KeypadKeycodeButtons>().keypad = this;
		playerCamera = owner;
		if (playerCamera.GetComponent<MouseLookScript>().inventoryMode == false)
			playerCamera.GetComponent<MouseLookScript>().ToggleInventoryMode();
		MFDManager.a.OpenTab(4,true,MFDManager.TabMSG.Keypad,0);
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
				keypadControl.SetActive(false);
			}
		}
	}
}
