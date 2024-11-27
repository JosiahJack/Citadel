using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePanel : MonoBehaviour {
	public int requiredIndex;
	public GameObject installationItem; // item that will be activated to make it look like the player installed something, e.g. an isotope or plastique
	public GameObject[] effects; // any effect items to turn on
	public bool open = false;
	public bool installed = false;
	public int wrongItemMessageLingdex;
	public int SFXInstallationIndex;
	public int SFXOpenIndex;
	public int installedMessageLingdex;
	public int alreadyInstalledMessageLingdex;
	public int SFXAlreadyInstalledIndex;
	public int openMessageLingdex;
	public string target;
	public string argvalue;
	
	private AudioSource SFX;
	private Animator anim;

	void Start() {
		anim = GetComponent<Animator>();
		SFX = GetComponent<AudioSource>();
	}

	public void Use(UseData ud) {
		if (open) {
			if (installed && ud.mainIndex == -1) {
				Const.sprint(alreadyInstalledMessageLingdex);
				return; // do nothing already done here
			}

			// Was player holding correct item in their hand when they used us?
			if (ud.mainIndex == requiredIndex && (requiredIndex != 92
												  || (requiredIndex == 92
												  && ud.customIndex == 1))) {
												     // Abe Ghiran's head.
				if (installed) { 					 // ... is big
					Utils.PlayOneShotSavable(SFX,Const.a.sounds[SFXAlreadyInstalledIndex]);
					return; // do nothing already done here
				}
				installed = true;
				if (installationItem != null) installationItem.SetActive(true);
				Utils.PlayOneShotSavable(SFX,Const.a.sounds[SFXInstallationIndex]);
				Const.sprint(installedMessageLingdex);
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
				Const.a.UseTargets(gameObject,ud,target);
			} else {
				Utils.PlayOneShotSavable(SFX,Const.a.sounds[43]); // button_deny, aaaahhh!! Try again
				Const.sprint(wrongItemMessageLingdex);
			}
		} else {
			open = true;
			anim.Play("Open");
			Utils.PlayOneShotSavable(SFX,Const.a.sounds[SFXOpenIndex]);
			Const.sprint(openMessageLingdex);
		}
	}

	public static string Save(GameObject go) {
		InteractablePanel ip = go.GetComponent<InteractablePanel>();
		string line = System.String.Empty;
		line = Utils.BoolToString(ip.open,"open");
		line += Utils.splitChar + Utils.BoolToString(ip.installed,"installed");
		line += Utils.splitChar + Utils.SaveSubActivatedGOState(ip.installationItem);
		for (int i=0;i<ip.effects.Length;i++) { line += Utils.splitChar + Utils.SaveSubActivatedGOState(ip.effects[i]); }
		if (ip.installationItem != null) {
			DelayedSpawn despawner = ip.installationItem.GetComponent<DelayedSpawn>();
			if (despawner != null) { // plastique
				line += Utils.splitChar + DelayedSpawn.Save(ip.installationItem);
				Transform childTr = ip.installationItem.transform.GetChild(0);
				if (childTr != null) {
					DelayedSpawn despawner2 = childTr.gameObject.GetComponent<DelayedSpawn>();
					if (despawner2 != null) { // plastique
						line += Utils.splitChar + DelayedSpawn.Save(childTr.gameObject);
					}
				}
			}
		}

		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		InteractablePanel ip = go.GetComponent<InteractablePanel>(); // ip man!
		ip.open = Utils.GetBoolFromString(entries[index],"open"); index++;
		ip.installed = Utils.GetBoolFromString(entries[index],"installed"); index++;
		index = Utils.LoadSubActivatedGOState(ip.installationItem,ref entries,index);
		for (int i=0; i<ip.effects.Length; i++) { index = Utils.LoadSubActivatedGOState(ip.effects[i],ref entries,index); }
		if (ip.installationItem != null) {
			DelayedSpawn despawner = ip.installationItem.GetComponent<DelayedSpawn>();
			if (despawner != null) { // plastique
				index = DelayedSpawn.Load(ip.installationItem,ref entries,index);
				Transform childTr = ip.installationItem.transform.GetChild(0);
				if (childTr != null) {
					DelayedSpawn despawner2 = childTr.gameObject.GetComponent<DelayedSpawn>();
					if (despawner2 != null) { // plastique
						index = DelayedSpawn.Load(childTr.gameObject,ref entries,index);
					}
				}
			}
		}

		return index;
	}
}
