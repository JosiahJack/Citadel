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
	[HideInInspector] public string wrongItemMessage;
	public int wrongItemMessageLingdex;
	[HideInInspector] public string installedMessage;
	public int installedMessageLingdex;
	[HideInInspector] public string alreadyInstalledMessage;
	public int alreadyInstalledMessageLingdex;
	public AudioClip SFXAlreadyInstalled;
	[HideInInspector] public string openMessage;
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
		//Debug.Log("Used InteractablePanel");
		if (open) {
			if (installed && ud.mainIndex == -1) {
				Const.sprintByIndexOrOverride (alreadyInstalledMessageLingdex, alreadyInstalledMessage,ud.owner);
				return; // do nothing already done here
			}

			// was player holding correct item in their hand when they used us?
			if (ud.mainIndex == requiredIndex) {
				if (installed) {
					Utils.PlayOneShotSavable(SFX,SFXAlreadyInstalled);
					return; // do nothing already done here
				}
				installed = true;
				installationItem.SetActive(true);
				Utils.PlayOneShotSavable(SFX,SFXInstallation);
				Const.sprintByIndexOrOverride (installedMessageLingdex, installedMessage,ud.owner);
				// any extra effect objects?  activate them here...good for sparks or turning on any extra bits and bobs
				if (effects.Length > 0) {
					for(int i=0;i<effects.Length;i++) {
						if (effects[i] != null) effects[i].SetActive(true);
					}
				}

				MouseLookScript.a.ResetHeldItem();
				MouseLookScript.a.ResetCursor();

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
				Utils.PlayOneShotSavable(SFX,SFXFail); // aaaahhh!! Try again
				Const.sprintByIndexOrOverride (wrongItemMessageLingdex, wrongItemMessage,ud.owner);
			}
		} else {
			open = true;
			anim.Play("Open");
			Utils.PlayOneShotSavable(SFX,SFXOpen);
			Const.sprintByIndexOrOverride (openMessageLingdex, openMessage,ud.owner);
		}
	}

	public static string Save(GameObject go) {
		InteractablePanel ip = go.GetComponent<InteractablePanel>();
		if (ip == null) {
			Debug.Log("InteractablePanel missing on savetype of InteractablePanel!  GameObject.name: " + go.name);
			return "0|0";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(ip.open); // bool - is the panel opened
		line += Utils.splitChar + Utils.BoolToString(ip.installed); // bool - is the item installed
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		InteractablePanel ip = go.GetComponent<InteractablePanel>(); // ip man!
		if (ip == null || index < 0 || entries == null) return index + 2;

		ip.open = Utils.GetBoolFromString(entries[index]); index++; // bool - is the panel opened
		ip.installed = Utils.GetBoolFromString(entries[index]); index++; // bool - is the item installed
		if (ip.installed && ip.installationItem != null){
			ip.installationItem.SetActive(true);
			// I already set up the effects[] objects with SaveObject of
			// Transform so no need to set them active here as they will be set
			// active by normal load loop that called this.
		}
		return index;
	}
}
