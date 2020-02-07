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
	public string wrongItemMessage;
	public string installedMessage;
	public string alreadyInstalledMessage;
	public AudioClip SFXAlreadyInstalled;
	public string openMessage;
	public string target;
	public string argvalue;

	void Start() {
		anim = GetComponent<Animator>();
		SFX = GetComponent<AudioSource>();
	}

	public void Use(UseData ud) {
		if (open) {
			if (installed && ud.mainIndex == -1) {
				Const.sprint(alreadyInstalledMessage,ud.owner);
				return; // do nothing already done here
			}

			// was player holding correct item in their hand when they used us?
			if (ud.mainIndex == requiredIndex) {
				if (installed) {
					if (SFXAlreadyInstalled != null) SFX.PlayOneShot(SFXAlreadyInstalled);
					return; // do nothing already done here
				}
				installed = true;
				installationItem.SetActive(true);
				if (SFXInstallation != null) SFX.PlayOneShot(SFXInstallation);
				Const.sprint(installedMessage,ud.owner);

				// use the target now that we are active
				ud.argvalue = argvalue;
				TargetIO tio = GetComponent<TargetIO>();
				if (tio != null) {
					ud.SetBits(tio);
				} else {
					Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
				}
				Const.a.UseTargets(ud,target);

				// any extra effect objects?  activate them here...good for sparks or turning on any extra bits and bobs
				if (effects.Length > 0) {
					for(int i=0;i<effects.Length;i++) {
						if (effects[i] != null) effects[i].SetActive(true);
					}
				}
			} else {
				if (SFXFail != null) SFX.PlayOneShot(SFXFail); // aaaahhh!! Try again
				Const.sprint(wrongItemMessage,ud.owner);
			}
		} else {
			open = true;
			anim.Play("Open");
			if (SFXOpen != null) SFX.PlayOneShot(SFXOpen);
			Const.sprint(openMessage,ud.owner);
		}
	}
}
