using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestLogNotesManager : MonoBehaviour {
	public GameObject[] notes;
	public Text[] labels;
	public Toggle[] checkBoxes;
	public GameObject label15_StrikeThru;
	public Door neuroSurgeryDoor;
	public Door level6elevatorDoorTo7;

	// 0 - Destroy level 1 nodes.					...Later add:  Code is #.		DONE, CHECK DONE
	// 1 - Destroy level 2 nodes.					...Later add:  Code is #.		DONE, CHECK DONE
	// 2 - Destroy level 3 nodes.					...Later add:  Code is #.		DONE, CHECK DONE
	// 3 - Destroy level 4 nodes.					...Later add:  Code is #.		DONE, CHECK DONE
	// 4 - Destroy level 5 nodes.					...Later add:  Code is #.		DONE, CHECK DONE
	// 5 - Destroy level 6 nodes.					...Later add:  Code is #.		DONE, CHECK DONE
	// 6 - Escape neurosurgery suite.				...Later add:  Keycode is 451.	DONE, CHECK DONE
	// 7 - Disengage laser safety override.											DONE, CHECK DONE
	// 8 - Activate the station energy shield.										DONE, CHECK DONE
	// 9 - Destroy the mining laser.												DONE, CHECK DONE
	// 10- Enable master jettison.													DONE, CHECK DONE
	// 11- Diagnose and repair broken relay			...Later add:: 428.				DONE, CHECK DONE
	// 12- Jettison Beta Grove.														DONE, CHECK DONE
	// 13- Destroy the four relay antennae.											DONE, CHECK DONE
	// 14- Engage reactor self-destruct.											DONE, CHECK DONE
	// 15- Escape on escape pod.													DONE, STRIKE THRU DONE
	// 16- Access the bridge.														DONE, CHECK DONE
	// 17- Destroy SHODAN. 															DONE, N/A CAN'T SEE DURING CREDITS

	public static QuestLogNotesManager a;

	void Awake() {
		a = this;
		for (int i=0;i<=17;i++) {
			a.notes[i].SetActive(false);
		}

		if (Const.a.difficultyMission == 0) {
			gameObject.SetActive(false);
			return;
		}
		a.labels[6].text = Const.a.stringTable[554]; // Set:Escape neurosurgery suite.
		a.notes[6].SetActive(true);
		if (Inventory.a != null) Inventory.a.hasNewNotes = true;
	}

	public void NotifyDoorUnlock(Door d) {
		if (d == neuroSurgeryDoor) {
			checkBoxes[6].isOn = true; // Escape neurosurgery suite.
			Inventory.a.hasNewNotes = true;
		}
	}

	public void NotifyLockedDoorAttempt(Door d) {
		if (d == level6elevatorDoorTo7) {
			notes[12].SetActive(true); // Jettison Beta Grove.
			labels[12].text = Const.a.stringTable[565];
			Inventory.a.hasNewNotes = true;
		}
	}

	public void NotifyLevelChange(int levelIndex) {
		if (levelIndex == 9) {
			checkBoxes[16].isOn = true; // Access the bridge.
			Inventory.a.hasNewNotes = true;
		}
	}

	public void LogAdded(int logCustomIndex) {
		if (Const.a.difficultyMission == 0) return;
		// Not checking for eReader present here...best to assume so we don't
		// have to do all this later and remember it and save it in save file.

		switch (logCustomIndex) {
			case 6: // #2-4601-06.MAY.72, just rewards
				notes[6].SetActive(true); // Make sure this was on.
				Inventory.a.hasNewNotes = true;
				labels[6].text += Const.a.stringTable[555]; // Add:  Keycode is 451.
				break;
			case 8: // Honig-11.OCT.72, Medical CPU's
				notes[0].SetActive(true);
				Inventory.a.hasNewNotes = true;
				labels[0].text = Const.a.stringTable[556] + "1" + Const.a.stringTable[557]; // Set:Destroy level 1 nodes.
				break;
			case 10: // Stack-15.OCT.72, Shodan's Presence
				notes[0].SetActive(true);
				Inventory.a.hasNewNotes = true;
				labels[0].text = Const.a.stringTable[556] + "1" + Const.a.stringTable[557]; // Set:Destroy level 1 nodes.
				break;
			case 15: // D'Arcy-23.OCT.72, destroying the laser
				notes[7].SetActive(true);
				notes[8].SetActive(true);
				notes[9].SetActive(true);
				Inventory.a.hasNewNotes = true;
				labels[7].text = Const.a.stringTable[559]; // Set:Disengage laser safety override.
				labels[8].text = Const.a.stringTable[560]; // Set:Activate the station energy shield.
				labels[9].text = Const.a.stringTable[561]; // Set:Destroy the mining laser.
				break;
			case 29: // D'Arcy-21.OCT.72, block the laser with the shields?
				notes[7].SetActive(true);
				notes[8].SetActive(true);
				Inventory.a.hasNewNotes = true;
				labels[7].text = Const.a.stringTable[559]; // Set:Disengage laser safety override.
				labels[8].text = Const.a.stringTable[560]; // Set:Activate the station energy shield.
				break;
			case 42: // diagnostic-06.NOV.72, repair diagnostic
				notes[10].SetActive(true); // Enable the enable Master Jettison task, just in case it wasn't
				notes[11].SetActive(true);
				Inventory.a.hasNewNotes = true;
				labels[10].text = Const.a.stringTable[562]; // Set:Enable master jettison.
				labels[11].text = Const.a.stringTable[563]; // Set:Diagnose and repair broken relay
				labels[11].text += Const.a.stringTable[564]; // Add:: 428
				break;
			case 66: // SHODAN-07.OCT.72, new jettison procedure
				notes[10].SetActive(true); // Enable the enable Master Jettison task, just in case it wasn't
				notes[12].SetActive(true);
				Inventory.a.hasNewNotes = true;
				labels[10].text = Const.a.stringTable[562]; // Set:Enable master jettison.
				labels[12].text = Const.a.stringTable[565]; // Set:Diagnose and repair broken relay
				break;
			case 67: // Aaron-12.OCT.72, virus experiment
				notes[10].SetActive(true); // Enable the enable Master Jettison task, just in case it wasn't
				notes[12].SetActive(true);
				Inventory.a.hasNewNotes = true;
				labels[10].text = Const.a.stringTable[562]; // Set:Enable master jettison.
				labels[12].text = Const.a.stringTable[565]; // Set:Diagnose and repair broken relay
				break;
			case 84: // Rebecca-1, Citadel Station
				notes[9].SetActive(true);
				Inventory.a.hasNewNotes = true;
				labels[9].text = Const.a.stringTable[561]; // Set:Destroy the mining laser.
				break;
			case 90: // Rebecca-3, SHODAN downloading
				notes[13].SetActive(true); // Destroy the four relay antennae.
				Inventory.a.hasNewNotes = true;
				labels[13].text = Const.a.stringTable[566]; // Set:Destroy the four relay antennae.
				break;
			case 93: // Rebecca-4, SHODAN destroying Citadel Station
				notes[14].SetActive(true);
				Inventory.a.hasNewNotes = true;
				labels[14].text = Const.a.stringTable[567]; // Set:Destroy the mining laser.
				break;
			case 97: // Rebecca-5, get to the bridge!
				notes[16].SetActive(true);
				notes[17].SetActive(true);
				Inventory.a.hasNewNotes = true;
				labels[16].text = Const.a.stringTable[569]; // Set:Access the bridge.
				labels[17].text = Const.a.stringTable[570]; // Set:Destroy SHODAN.
				label15_StrikeThru.SetActive(true);
				break;
			case 120: // VMailStatus, Shield Activated
				notes[8].SetActive(true);
				Inventory.a.hasNewNotes = true;
				labels[8].text = Const.a.stringTable[569]; // Set:Activate the station energy shield.
				checkBoxes[8].isOn = true;
				break;
		}
	}

	public void NodesDestroyed(int levelIndex) {
		if (Const.a.difficultyMission == 0) return;

		switch (levelIndex) {
			case 1:
				notes[0].SetActive(true);
				labels[0].text = Const.a.stringTable[556] + "1" + Const.a.stringTable[557]; // Set:Destroy level 1 nodes. // Make sure this was on.
				notes[1].SetActive(true);
				labels[1].text = Const.a.stringTable[556] + "2" + Const.a.stringTable[557]; // Set:Destroy level 2 nodes.
				checkBoxes[0].isOn = true;
				Inventory.a.hasNewNotes = true;
				labels[0].text += Const.a.stringTable[558] + Const.a.questData.lev1SecCode.ToString() + "."; // Add:  Code is , Add:#, Add:.
				break;
			case 2:
				notes[1].SetActive(true);
				labels[1].text = Const.a.stringTable[556] + "2" + Const.a.stringTable[557]; // Set:Destroy level 2 nodes. // Make sure this was on.
				notes[2].SetActive(true);
				labels[2].text = Const.a.stringTable[556] + "3" + Const.a.stringTable[557]; // Set:Destroy level 3 nodes.
				checkBoxes[1].isOn = true;
				Inventory.a.hasNewNotes = true;
				labels[1].text += Const.a.stringTable[558] + Const.a.questData.lev1SecCode.ToString() + "."; // Add:  Code is , Add:#, Add:.
				break;
			case 3:
				notes[2].SetActive(true);
				labels[2].text = Const.a.stringTable[556] + "3" + Const.a.stringTable[557]; // Set:Destroy level 3 nodes. // Make sure this was on.
				notes[3].SetActive(true);
				labels[3].text = Const.a.stringTable[556] + "4" + Const.a.stringTable[557]; // Set:Destroy level 4 nodes.
				checkBoxes[2].isOn = true;
				Inventory.a.hasNewNotes = true;
				labels[2].text += Const.a.stringTable[558] + Const.a.questData.lev1SecCode.ToString() + "."; // Add:  Code is , Add:#, Add:.
				break;
			case 4:
				notes[3].SetActive(true);
				labels[3].text = Const.a.stringTable[556] + "4" + Const.a.stringTable[557]; // Set:Destroy level 4 nodes. // Make sure this was on.
				notes[4].SetActive(true);
				labels[4].text = Const.a.stringTable[556] + "5" + Const.a.stringTable[557]; // Set:Destroy level 5 nodes.
				checkBoxes[3].isOn = true;
				Inventory.a.hasNewNotes = true;
				labels[3].text += Const.a.stringTable[558] + Const.a.questData.lev1SecCode.ToString() + "."; // Add:  Code is , Add:#, Add:.
				break;
			case 5:
				notes[4].SetActive(true);
				labels[4].text = Const.a.stringTable[556] + "5" + Const.a.stringTable[557]; // Set:Destroy level 5 nodes. // Make sure this was on.
				notes[5].SetActive(true);
				labels[5].text = Const.a.stringTable[556] + "6" + Const.a.stringTable[557]; // Set:Destroy level 6 nodes.
				checkBoxes[4].isOn = true;
				Inventory.a.hasNewNotes = true;
				labels[4].text += Const.a.stringTable[558] + Const.a.questData.lev1SecCode.ToString() + "."; // Add:  Code is , Add:#, Add:.
				break;
			case 6:
				notes[5].SetActive(true);
				labels[5].text = Const.a.stringTable[556] + "6" + Const.a.stringTable[557]; // Set:Destroy level 6 nodes. // Make sure this was on.
				checkBoxes[5].isOn = true;
				Inventory.a.hasNewNotes = true;
				labels[5].text += Const.a.stringTable[558] + Const.a.questData.lev1SecCode.ToString() + "."; // Add:  Code is , Add:#, Add:.
				break;
		}
	}

	public void UpdateToNextMission (int missionIndex) {
		switch (missionIndex) {
			case 1: checkBoxes[9].isOn = true; break; // Laser destroyed
			case 2: checkBoxes[13].isOn = true; break; // Antennae destroyed
			case 3: checkBoxes[14].isOn = true; break; // Self destruct activated.
		}
	}
}
