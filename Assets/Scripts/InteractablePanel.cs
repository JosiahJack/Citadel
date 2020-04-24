using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePanel : MonoBehaviour {
	public int requiredIndex;
	public GameObject installationItem; // item that will be activated to make it look like the player installed something, e.g. an isotope or plastique
	public GameObject[] effects; // any effect items to turn on
	public bool open = false;
	public bool installed = false;
	private AudioSource SFX;
	public AudioClip SFXInstallation;
	public AudioClip SFXOpen;
	public AudioClip SFXFail;
	private Animator anim;
	[HideInInspector]
	public string wrongItemMessage;
	public int wrongItemMessageLingdex;
	[HideInInspector]
	public string installedMessage;
	public int installedMessageLingdex;
	[HideInInspector]
	public string alreadyInstalledMessage;
	public int alreadyInstalledMessageLingdex;
	public AudioClip SFXAlreadyInstalled;
	[HideInInspector]
	public string openMessage;
	public int openMessageLingdex;
	public string target;
	public string argvalue;

	void Start() {
		anim = GetComponent<Animator>();
		SFX = GetComponent<AudioSource>();
		if (SFX == null) Debug.Log("BUG: Missing AudioSource on Interactable Panel");
		if (string.IsNullOrWhiteSpace(openMessage)) {
			if (openMessageLingdex >= 0) {
				if (openMessageLingdex < Const.a.stringTable.Length && openMessageLingdex != -1)
					openMessage = Const.a.stringTable[openMessageLingdex];
			}
		}
	}

	public void Use(UseData ud) {
		Debug.Log("Used InteractablePanel");
		if (open) {
			if (installed && ud.mainIndex == -1) {
				Const.sprintByIndexOrOverride (alreadyInstalledMessageLingdex, alreadyInstalledMessage,ud.owner);
				return; // do nothing already done here
			}

			// was player holding correct item in their hand when they used us?
			if (ud.mainIndex == requiredIndex) {
				if (installed) {
					if (SFXAlreadyInstalled != null && SFX != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXAlreadyInstalled);
					return; // do nothing already done here
				}
				installed = true;
				installationItem.SetActive(true);
				if (SFXInstallation != null && SFX != null) SFX.PlayOneShot(SFXInstallation);
				Const.sprintByIndexOrOverride (installedMessageLingdex, installedMessage,ud.owner);
				// any extra effect objects?  activate them here...good for sparks or turning on any extra bits and bobs
				if (effects.Length > 0) {
					for(int i=0;i<effects.Length;i++) {
						if (effects[i] != null) effects[i].SetActive(true);
					}
				}
				PlayerReferenceManager prm = ud.owner.GetComponent<PlayerReferenceManager>();
				if (prm != null) {
					MouseLookScript mls = prm.playerCapsuleMainCamera.GetComponent<MouseLookScript>();
					if (mls != null) {
						mls.ResetHeldItem();
						mls.ResetCursor();
					}
				}
				// use the target now that we are active
				ud.argvalue = argvalue;
				TargetIO tio = GetComponent<TargetIO>();
				if (tio != null) {
					ud.SetBits(tio);
				} else {
					Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
				}
				Const.a.UseTargets(ud,target);
			} else {
				if (SFXFail != null && SFX != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXFail); // aaaahhh!! Try again
				Const.sprintByIndexOrOverride (wrongItemMessageLingdex, wrongItemMessage,ud.owner);
			}
		} else {
			open = true;
			anim.Play("Open");
			if (SFXOpen != null && SFX != null && gameObject.activeInHierarchy) SFX.PlayOneShot(SFXOpen);
			Const.sprintByIndexOrOverride (openMessageLingdex, openMessage,ud.owner);
		}
	}
}
