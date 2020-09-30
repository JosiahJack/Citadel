using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoftwareInventory : MonoBehaviour {
	public bool isPulserNotDrill = true;
	public GameObject[] softs;
	public int[] softVersions;
	public bool[] hasSoft;
	public enum SoftwareType{None,Drill,Pulser,CShield,Decoy,Recall,Turbo,Game,Data,Integrity,Keycard};
	public AudioSource SFX;
	public AudioClip SFXChangeWeapon;
	public AudioClip SFXAcquireCyberItem;
	public AudioClip SFXAcquireCyberData;
	public HealthManager hm;
	public SoftwareInvText pulserButtonText;
	public SoftwareInvText drillButtonText;
	public Text drillVersionText;
	public Text pulserVersionText;
	public Text cshieldVersionText;
	public Text turboCountText;
	public Text decoyCountText;
	public Text recallCountText;
	public AccessCardInventory accessCardInventory;
	private Door.accessCardType doorAccessTypeAcquired;
	public PlayerMovement pm;
	public MouseLookScript mls;
	public GameObject decoyPrefab;
	public GameObject CyberSpaceStaticContainer;
	public int currentCyberItem = -1;

	public void UseCyberspaceItem() {
		if (currentCyberItem <= 0) {
			currentCyberItem = GetExistingCyberItemIndex(); // try one more time to be sure
			if (currentCyberItem <= 0) {
				Const.sprint(Const.a.stringTable[473],Const.a.player1); // Out of expendable softwares.
				return;
			}
			// oh it was good, ok then moving on down...
		}

		switch(currentCyberItem) {
			case 0:
				// Turbo
				if (softVersions[3] <= 0) {
					currentCyberItem = GetExistingCyberItemIndex();
					return; // out of turbos
				}
				UseTurbo();
				break;
			case 1:
				// Decoy
				if (softVersions[4] <= 0) {
					currentCyberItem = GetExistingCyberItemIndex();
					return; // out of decoys
				}
				UseDecoy();
				break;
			case 2:
				// Recall
				if (softVersions[5] <= 0) {
					currentCyberItem = GetExistingCyberItemIndex();
					return; // out of recalls
				}
				UseRecall();
				break;
		}
	}

	int GetExistingCyberItemIndex() {
		if (softVersions[3] > 0) return 0; // Turbo
		if (softVersions[4] > 0) return 1; // Decoy
		if (softVersions[5] > 0) return 2; // Recall
		return -1;
	}

	public void CycleCyberSpaceItemUp() {
		int nextIndex = currentCyberItem + 1; // add 1 to get slot above this
		if (nextIndex > 2) nextIndex = 0; // wraparound to bottom
		int countCheck = 0;
		bool buttonNotValid = hasSoft[nextIndex];
		while (buttonNotValid) {
			countCheck++;
			if (countCheck > 7) {
				currentCyberItem = -1;
				return; // no weapons!  don't runaway loop
			}
			nextIndex++;
			if (nextIndex > 2) nextIndex = 0;
			buttonNotValid = hasSoft[nextIndex];
		}
		currentCyberItem = nextIndex;
	}

	public void CycleCyberSpaceItemDn() {
		int nextIndex = currentCyberItem - 1; // add 1 to get slot above this
		if (nextIndex < 0) nextIndex = 2; // wraparound to top
		int countCheck = 0;
		bool buttonNotValid = (hasSoft[nextIndex] == false);
		while (buttonNotValid) {
			countCheck++;
			if (countCheck > 7) {
				currentCyberItem = -1;
				return; // no weapons!  don't runaway loop
			}
			nextIndex--;
			if (nextIndex < 0) nextIndex = 2;
			buttonNotValid = hasSoft[nextIndex];
		}
		currentCyberItem = nextIndex;
	}

	public void UseTurbo() {
		if (softVersions[3] <= 0) {
			softs[3].SetActive(false); // turn the button off now that we are out
			return; // out of turbos
		}
		softVersions[3]--; // reduce number of turbos we have left to use
		if (softVersions[3] <= 0) {
			hasSoft[3] = false;
			softs[3].SetActive(false); // turn the button off now that we are out
		}
		if (pm.turboFinished > PauseScript.a.relativeTime) {
			pm.turboFinished += pm.turboCyberTime; // effect stacks
		} else {
			pm.turboFinished = pm.turboCyberTime + PauseScript.a.relativeTime;
		}
	}

	public void UseDecoy() {
		if (softVersions[4] <= 0) {
			softs[4].SetActive(false); // turn the button off now that we are out
			return; // out of decoys
		}
		softVersions[4]--; // reduce number of decoys we have left to use
		if (softVersions[4] <= 0) {
			hasSoft[4] = false;
			softs[4].SetActive(false); // turn the button off now that we are out
		}
		GameObject decoyObj = Instantiate(decoyPrefab,pm.transform.position,mls.transform.rotation) as GameObject;
		if (decoyObj != null) {
			decoyObj.transform.SetParent(CyberSpaceStaticContainer.transform,true);
		}
	}

	public void UseRecall() {
		if (softVersions[5] <= 0) {
			softs[5].SetActive(false); // turn the button off now that we are out
			return; // out of recalls
		}
		softVersions[5]--; // reduce number of recalls we have left to use
		if (softVersions[5] <= 0) {
			hasSoft[5] = false;
			softs[5].SetActive(false); // turn the button off now that we are out
		}
		pm.transform.position = mls.cyberspaceRecallPoint; // pop back to cyber section start
	}

	public bool AddItem(SoftwareType type, int vers) {
		switch(type) {
			case SoftwareType.Drill:	
				softs[0].SetActive(true);
				if (isPulserNotDrill && !hasSoft[1]) {
					pulserButtonText.Select(false);
					drillButtonText.Select(true);
					isPulserNotDrill = false; // equip drill if we don't already have pulser
				}
				if (vers > softVersions[0]) softVersions[0] = vers;
				else Const.sprint(Const.a.stringTable[46],Const.a.player1);
				drillVersionText.text = softVersions[0].ToString();
				hasSoft[0] = true;
				if (SFX != null && SFXAcquireCyberItem != null) SFX.PlayOneShot(SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[444] + softVersions[0].ToString() + Const.a.stringTable[458],Const.a.player1);
				return true;
			case SoftwareType.Pulser:	
				softs[1].SetActive(true);
				if (!isPulserNotDrill && !hasSoft[1]) {
					pulserButtonText.Select(true);
					drillButtonText.Select(false);
					isPulserNotDrill = true; // always equip pulser when first picking it up
				}
				if (vers > softVersions[1]) softVersions[1] = vers;
				else Const.sprint(Const.a.stringTable[46],Const.a.player1);
				pulserVersionText.text = softVersions[1].ToString();
				hasSoft[1] = true;
				if (SFX != null && SFXAcquireCyberItem != null) SFX.PlayOneShot(SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[445] + softVersions[1].ToString() + Const.a.stringTable[458],Const.a.player1);
				return true;
			case SoftwareType.CShield:	
				softs[2].SetActive(true);
				if (vers > softVersions[2]) softVersions[2] = vers;
				else Const.sprint(Const.a.stringTable[46],Const.a.player1);
				cshieldVersionText.text = softVersions[2].ToString();
				hasSoft[2] = true;
				if (SFX != null && SFXAcquireCyberItem != null) SFX.PlayOneShot(SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[446] + softVersions[2].ToString() + Const.a.stringTable[458],Const.a.player1);
				return true;
			case SoftwareType.Turbo:
				if (currentCyberItem == -1f) currentCyberItem = 0;
				softs[3].SetActive(true);
				softVersions[3]++;
				turboCountText.text = softVersions[3].ToString();
				hasSoft[3] = true;
				if (SFX != null && SFXAcquireCyberItem != null) SFX.PlayOneShot(SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[447],Const.a.player1);
				return true;
			case SoftwareType.Decoy:	
				if (currentCyberItem == -1f) currentCyberItem = 1;
				softs[4].SetActive(true);
				softVersions[4]++;
				decoyCountText.text = softVersions[4].ToString();
				hasSoft[4] = true;
				if (SFX != null && SFXAcquireCyberItem != null) SFX.PlayOneShot(SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[448],Const.a.player1);
				return true;
			case SoftwareType.Recall:	
				if (currentCyberItem == -1f) currentCyberItem = 2;
				softs[5].SetActive(true);
				softVersions[5]++;
				recallCountText.text = softVersions[5].ToString();
				hasSoft[5] = true;
				if (SFX != null && SFXAcquireCyberItem != null) SFX.PlayOneShot(SFXAcquireCyberItem);
				Const.sprint(Const.a.stringTable[449],Const.a.player1);
				return true;
			case SoftwareType.Game:		
				softs[6].SetActive(true);
				switch(vers) {
					case 0: // Ping
							// UPDATE: Create minigame Ping
							Const.sprint(Const.a.stringTable[450],Const.a.player1);
							break;
					case 1: // 15
							// UPDATE: Create minigame 15
							Const.sprint(Const.a.stringTable[451],Const.a.player1);
							break;
					case 2: // Wing 0
							// UPDATE: Create minigame Wing 0
							Const.sprint(Const.a.stringTable[452],Const.a.player1);
							break;
					case 3: // Botbounce
							// UPDATE: Create minigame Botbounce
							Const.sprint(Const.a.stringTable[453],Const.a.player1);
							break;
					case 4: // Eel Zapper
							// UPDATE: Create minigame Eel Zapper
							Const.sprint(Const.a.stringTable[454],Const.a.player1);
							break;
					case 5: // Road
							// UPDATE: Create minigame Road
							Const.sprint(Const.a.stringTable[455],Const.a.player1);
							break;
					case 6: // TriopToe
							// UPDATE: Create minigame TriopToe
							Const.sprint(Const.a.stringTable[456],Const.a.player1);
							break;
				}
				if (SFX != null && SFXAcquireCyberItem != null) SFX.PlayOneShot(SFXAcquireCyberItem);
				
				return true;
			case SoftwareType.Data:		
				if (SFX != null && SFXAcquireCyberData != null) SFX.PlayOneShot(SFXAcquireCyberData);
				Const.sprint(Const.a.stringTable[457],Const.a.player1);
				LogInventory.a.hasLog[vers] = true;
				return true;
			case SoftwareType.Integrity:
				if (hm.cyberHealth >=255) return false;
				if (SFX != null && SFXAcquireCyberItem != null) SFX.PlayOneShot(SFXAcquireCyberItem);
				hm.cyberHealth += 77f;
				if (hm.cyberHealth > 255f) hm.cyberHealth = 255f;
				hm.ph.playerCyberHealthTicks.DrawTicks();
				Const.sprint(Const.a.stringTable[459],Const.a.player1);
				return true;
			case SoftwareType.Keycard:	
				bool alreadyHave = false;
				bool accessAdded = false;

				switch (vers) {
					case 81: doorAccessTypeAcquired = Door.accessCardType.Standard; break; //CHECKED! Good here
					case 82: doorAccessTypeAcquired = Door.accessCardType.Per1; break; //CHECKED! Good here
					case 83: doorAccessTypeAcquired = Door.accessCardType.Group1; break; //CHECKED! Good here
					case 84: doorAccessTypeAcquired = Door.accessCardType.Science; break; //CHECKED! Good here
					case 85: doorAccessTypeAcquired = Door.accessCardType.Engineering; break;  //CHECKED! Good here
					case 86: doorAccessTypeAcquired = Door.accessCardType.GroupB; break; //CHECKED! Good here
					case 87: doorAccessTypeAcquired = Door.accessCardType.Security; break; //CHECKED! Good here
					case 88: doorAccessTypeAcquired = Door.accessCardType.Per5; break;
					case 89: doorAccessTypeAcquired = Door.accessCardType.Medical; break;
					case 90: doorAccessTypeAcquired = Door.accessCardType.Standard; break;
					case 91: doorAccessTypeAcquired = Door.accessCardType.Group4; break;
					case 114: doorAccessTypeAcquired = Door.accessCardType.Admin; break;
				}

				for (int j = 0; j < accessCardInventory.accessCardsOwned.Length; j++) {
					if (accessCardInventory.accessCardsOwned [j] == doorAccessTypeAcquired) alreadyHave = true; // check if we already have this card
				}

				for (int i = 0; i < accessCardInventory.accessCardsOwned.Length; i++) {
					if (accessCardInventory.accessCardsOwned [i] == Door.accessCardType.None) {
						if (!alreadyHave) {
							accessCardInventory.accessCardsOwned [i] = doorAccessTypeAcquired;
							accessAdded = true;
							break;
						}
					}
				}

				if (alreadyHave) {
					Const.sprint (Const.a.stringTable[44] + doorAccessTypeAcquired.ToString(), Const.a.player1); // Already have access: ##
					return false;
				}

				if (accessAdded && !alreadyHave) {
					Const.sprint (Const.a.stringTable[45] + doorAccessTypeAcquired.ToString(), Const.a.player1); // New accesses gained ##
				}
				return true;
		}
		return false;
	}
}
