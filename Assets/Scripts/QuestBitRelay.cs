using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBitRelay : MonoBehaviour {
	[Header("Bits to enable or disable when this GameObject is Targetted, or To Test Against")]
	public int lev1SecCode;
	public int lev2SecCode;
	public int lev3SecCode;
	public int lev4SecCode;
	public int lev5SecCode;
	public int lev6SecCode;
	public bool RobotSpawnDeactivated;
	public bool IsotopeInstalled;
	public bool ShieldActivated;
	public bool LaserSafetyOverriden;
	public bool LaserDestroyed;
	public bool BetaGroveCyberUnlocked;
	public bool GroveAlphaJettisonEnabled;
	public bool GroveBetaJettisonEnabled;
	public bool GroveDeltaJettisonEnabled;
	public bool MasterJettisonBroken;
	public bool Relay428Fixed;
	public bool MasterJettisonEnabled;
	public bool BetaGroveJettisoned;
	public bool AntennaNorthDestroyed;
	public bool AntennaSouthDestroyed;
	public bool AntennaEastDestroyed;
	public bool AntennaWestDestroyed;
	public bool SelfDestructActivated;
	public bool BridgeSeparated;
	public bool IsolinearChipsetInstalled;
	public string target;
	public string targetIfFalse;
	public string argvalue;
	public string argvalueIfFalse;
	
    public void EnableBits() {
		if (RobotSpawnDeactivated) {
			Const.a.questData.RobotSpawnDeactivated = true;
			Debug.Log("Bit set RobotSpawnDeactivated: "
					  + Const.a.questData.RobotSpawnDeactivated.ToString());
		}

		if (IsotopeInstalled) {
			Const.a.questData.IsotopeInstalled = true;
			Debug.Log("Bit set IsotopeInstalled: "
					  + Const.a.questData.IsotopeInstalled.ToString());
		}

		if (ShieldActivated) {
			Const.a.questData.ShieldActivated = true;
			LevelManager.a.exterior_shield.SetActive(true);
			Debug.Log("Bit set ShieldActivated: "
					  + Const.a.questData.ShieldActivated.ToString());

			QuestLogNotesManager.a.notes[8].SetActive(true);
			QuestLogNotesManager.a.checkBoxes[8].isOn = 
				Const.a.questData.ShieldActivated;

			QuestLogNotesManager.a.labels[8].text = Const.a.stringTable[560];
		}

		if (LaserSafetyOverriden) {
			Const.a.questData.LaserSafetyOverriden = true;
			Debug.Log("Bit set LaserSafetyOverriden: "
					  + Const.a.questData.LaserSafetyOverriden.ToString());

			QuestLogNotesManager.a.notes[7].SetActive(true);
			QuestLogNotesManager.a.checkBoxes[7].isOn =
				Const.a.questData.LaserSafetyOverriden;

			QuestLogNotesManager.a.labels[7].text = Const.a.stringTable[559];
		}
		if (LaserDestroyed) { 
			Const.a.questData.LaserDestroyed = true;
			Debug.Log("Bit set LaserDestroyed: "
					  + Const.a.questData.LaserDestroyed.ToString());

			if (AutoSplitterData.missionSplitID == 1) {
				AutoSplitterData.missionSplitID++;
			}

			QuestLogNotesManager.a.notes[9].SetActive(true);
			QuestLogNotesManager.a.checkBoxes[9].isOn =
				Const.a.questData.LaserDestroyed;

			QuestLogNotesManager.a.labels[9].text = Const.a.stringTable[561];
		}

		if (BetaGroveCyberUnlocked) {
			Const.a.questData.BetaGroveCyberUnlocked = true;
			Debug.Log("Bit set BetaGroveCyberUnlocked: "
					  + Const.a.questData.BetaGroveCyberUnlocked.ToString());
		}

		if (GroveAlphaJettisonEnabled) {
			Const.a.questData.GroveAlphaJettisonEnabled = true;
			Debug.Log("Bit set GroveAlphaJettisonEnabled: "
					  + Const.a.questData.GroveAlphaJettisonEnabled.ToString());
		}

		if (GroveBetaJettisonEnabled) {
			Const.a.questData.GroveBetaJettisonEnabled = true;
			Debug.Log("Bit set GroveBetaJettisonEnabled: "
					  + Const.a.questData.GroveBetaJettisonEnabled.ToString());
		}

		if (GroveDeltaJettisonEnabled) {
			Const.a.questData.GroveDeltaJettisonEnabled = true;
			Debug.Log("Bit set GroveDeltaJettisonEnabled: "
					  + Const.a.questData.GroveDeltaJettisonEnabled.ToString());
		}

		if (MasterJettisonBroken) {
			Const.a.questData.MasterJettisonBroken = true;
			Debug.Log("Bit set MasterJettisonBroken: "
					  + Const.a.questData.MasterJettisonBroken.ToString());

			if (AutoSplitterData.missionSplitID == 2) {
				AutoSplitterData.missionSplitID++;
			}

			QuestLogNotesManager.a.notes[11].SetActive(true);
			QuestLogNotesManager.a.labels[11].text =
				Const.a.stringTable[563];// Set:Diagnose and repair broken relay
		}
		if (Relay428Fixed) {
			Const.a.questData.Relay428Fixed = true;
			Debug.Log("Bit set Relay428Fixed: "
					  + Const.a.questData.Relay428Fixed.ToString());

			QuestLogNotesManager.a.notes[11].SetActive(true);
			QuestLogNotesManager.a.checkBoxes[11].isOn =
				Const.a.questData.Relay428Fixed;

			QuestLogNotesManager.a.labels[11].text =
				Const.a.stringTable[563]; // Set:Diagnose and repair broken relay

			// Add:: 428.
			QuestLogNotesManager.a.labels[11].text += Const.a.stringTable[564];
		}
		if (MasterJettisonEnabled) {
			Const.a.questData.MasterJettisonEnabled = true;
			Debug.Log("Bit set MasterJettisonEnabled: "
					  + Const.a.questData.MasterJettisonEnabled.ToString());

			if (AutoSplitterData.missionSplitID == 3) {
				AutoSplitterData.missionSplitID++;
			}

			QuestLogNotesManager.a.notes[10].SetActive(true);
			QuestLogNotesManager.a.checkBoxes[10].isOn =
				Const.a.questData.MasterJettisonEnabled;

			QuestLogNotesManager.a.labels[10].text = Const.a.stringTable[562];
		}
		if (BetaGroveJettisoned) {
			Const.a.questData.BetaGroveJettisoned = true;
			Debug.Log("Bit set BetaGroveJettisoned: "
					  + Const.a.questData.BetaGroveJettisoned.ToString());

			if (AutoSplitterData.missionSplitID == 4) { 
				AutoSplitterData.missionSplitID++;
			}

			QuestLogNotesManager.a.notes[12].SetActive(true);
			QuestLogNotesManager.a.checkBoxes[12].isOn =
				Const.a.questData.BetaGroveJettisoned;

			QuestLogNotesManager.a.labels[12].text = Const.a.stringTable[565];
			QuestLogNotesManager.a.notes[13].SetActive(true);
			QuestLogNotesManager.a.labels[13].text = Const.a.stringTable[566];
		}
		if (AntennaNorthDestroyed) {
			Const.a.questData.AntennaNorthDestroyed = true;
			Debug.Log("Bit set AntennaNorthDestroyed: "
					  + Const.a.questData.AntennaNorthDestroyed.ToString());
		}

		if (AntennaSouthDestroyed) {
			Const.a.questData.AntennaSouthDestroyed = true;
			Debug.Log("Bit set AntennaSouthDestroyed: "
					  + Const.a.questData.AntennaSouthDestroyed.ToString());
		}

		if (AntennaEastDestroyed) {
			Const.a.questData.AntennaEastDestroyed = true;
			Debug.Log("Bit set AntennaEastDestroyed: "
					  + Const.a.questData.AntennaEastDestroyed.ToString());
		}

		if (AntennaWestDestroyed) {
			Const.a.questData.AntennaWestDestroyed = true;
			Debug.Log("Bit set AntennaWestDestroyed: "
					  + Const.a.questData.AntennaWestDestroyed.ToString());
		}

		if (SelfDestructActivated) {
			Const.a.questData.SelfDestructActivated = true;
			Debug.Log("Bit set SelfDestructActivated: "
					  + Const.a.questData.SelfDestructActivated.ToString());

			QuestLogNotesManager.a.notes[14].SetActive(true); // Self destruct
			QuestLogNotesManager.a.notes[15].SetActive(true); // Escape pod
			QuestLogNotesManager.a.checkBoxes[14].isOn =
				Const.a.questData.SelfDestructActivated;

			// Set:Engage reactor self-destruct.
			QuestLogNotesManager.a.labels[14].text = Const.a.stringTable[567];

			// Set:Escape on escape pod.
			QuestLogNotesManager.a.labels[15].text = Const.a.stringTable[568];
		}

		if (BridgeSeparated) {
			Const.a.questData.BridgeSeparated = true;
			Debug.Log("Bit set BridgeSeparated: "
					  + Const.a.questData.BridgeSeparated.ToString());

			QuestLogNotesManager.a.notes[14].SetActive(true); // Self destruct
			QuestLogNotesManager.a.checkBoxes[14].isOn =
				Const.a.questData.SelfDestructActivated;

			// Set:Engage reactor self-destruct.
			QuestLogNotesManager.a.labels[14].text = Const.a.stringTable[567];
			QuestLogNotesManager.a.notes[16].SetActive(true);
			QuestLogNotesManager.a.notes[17].SetActive(true);
			QuestLogNotesManager.a.checkBoxes[16].isOn = true;

			// Set:Access the bridge.
			QuestLogNotesManager.a.labels[16].text = Const.a.stringTable[569];

			// Set:Destroy SHODAN.
			QuestLogNotesManager.a.labels[17].text = Const.a.stringTable[570];
		}
		if (IsolinearChipsetInstalled) {
			Const.a.questData.IsolinearChipsetInstalled = true;
			Debug.Log("Bit set IsolinearChipsetInstalled: "
					  + Const.a.questData.IsolinearChipsetInstalled.ToString());
		}
	}

	public void DisableBits() {
		if (RobotSpawnDeactivated) {
			Const.a.questData.RobotSpawnDeactivated = false;
		}

		if (IsotopeInstalled) Const.a.questData.IsotopeInstalled = false;
		if (ShieldActivated) {
			Const.a.questData.ShieldActivated = false;
			LevelManager.a.exterior_shield.SetActive(false);
			Debug.Log("Bit unset ShieldActivated: "
					  + Const.a.questData.ShieldActivated.ToString());

			QuestLogNotesManager.a.checkBoxes[8].isOn =
				Const.a.questData.ShieldActivated;
		}
		if (LaserSafetyOverriden) {
			Const.a.questData.LaserSafetyOverriden = false;
			QuestLogNotesManager.a.checkBoxes[7].isOn = Const.a.questData.LaserSafetyOverriden;
		}
		if (LaserDestroyed) {
			Const.a.questData.LaserDestroyed = false;
			QuestLogNotesManager.a.checkBoxes[9].isOn = Const.a.questData.LaserDestroyed;
		}
		if (BetaGroveCyberUnlocked) Const.a.questData.BetaGroveCyberUnlocked = false;
		if (GroveAlphaJettisonEnabled) Const.a.questData.GroveAlphaJettisonEnabled = false;
		if (GroveBetaJettisonEnabled) Const.a.questData.GroveBetaJettisonEnabled = false;
		if (GroveDeltaJettisonEnabled) Const.a.questData.GroveDeltaJettisonEnabled = false;
		if (MasterJettisonBroken) Const.a.questData.MasterJettisonBroken = false;
		if (Relay428Fixed) {
			Const.a.questData.Relay428Fixed = false;
			QuestLogNotesManager.a.checkBoxes[11].isOn = Const.a.questData.Relay428Fixed;
		}
		if (MasterJettisonEnabled) {
			Const.a.questData.MasterJettisonEnabled = false;
			QuestLogNotesManager.a.checkBoxes[10].isOn = Const.a.questData.MasterJettisonEnabled;
		}
		if (BetaGroveJettisoned) {
			Const.a.questData.BetaGroveJettisoned = false;
			QuestLogNotesManager.a.checkBoxes[12].isOn = Const.a.questData.BetaGroveJettisoned;
		}
		if (AntennaNorthDestroyed) Const.a.questData.AntennaNorthDestroyed = false;
		if (AntennaSouthDestroyed) Const.a.questData.AntennaSouthDestroyed = false;
		if (AntennaEastDestroyed) Const.a.questData.AntennaEastDestroyed = false;
		if (AntennaWestDestroyed) Const.a.questData.AntennaWestDestroyed = false;
		if (SelfDestructActivated) {
			Const.a.questData.SelfDestructActivated = false;
			QuestLogNotesManager.a.checkBoxes[14].isOn = Const.a.questData.SelfDestructActivated;
		}
		if (BridgeSeparated) Const.a.questData.BridgeSeparated = false;
		if (IsolinearChipsetInstalled) Const.a.questData.IsolinearChipsetInstalled = false;
	}

    public void ToggleBits() {
		if (RobotSpawnDeactivated) Const.a.questData.RobotSpawnDeactivated = !Const.a.questData.RobotSpawnDeactivated;
		if (IsotopeInstalled) Const.a.questData.IsotopeInstalled = !Const.a.questData.IsotopeInstalled;
		if (ShieldActivated) {
			Const.a.questData.ShieldActivated = !Const.a.questData.ShieldActivated;
			LevelManager.a.exterior_shield.SetActive(Const.a.questData.ShieldActivated);
			QuestLogNotesManager.a.checkBoxes[8].isOn = Const.a.questData.ShieldActivated;
			if (Const.a.questData.ShieldActivated) {
				QuestLogNotesManager.a.notes[8].SetActive(true);
				QuestLogNotesManager.a.labels[8].text = Const.a.stringTable[560];
			}
		}
		if (LaserSafetyOverriden) {
			Const.a.questData.LaserSafetyOverriden = !Const.a.questData.LaserSafetyOverriden;
			QuestLogNotesManager.a.checkBoxes[7].isOn = Const.a.questData.LaserSafetyOverriden;
			if (Const.a.questData.LaserSafetyOverriden) {
				QuestLogNotesManager.a.notes[7].SetActive(true);
				QuestLogNotesManager.a.labels[7].text = Const.a.stringTable[559];
			}
		}
		if (LaserDestroyed) {
			Const.a.questData.LaserDestroyed = !Const.a.questData.LaserDestroyed;
			if (AutoSplitterData.missionSplitID == 1) { AutoSplitterData.missionSplitID++; }
			QuestLogNotesManager.a.checkBoxes[9].isOn = Const.a.questData.LaserDestroyed;
			if (Const.a.questData.LaserDestroyed) {
				QuestLogNotesManager.a.notes[9].SetActive(true);
				QuestLogNotesManager.a.labels[9].text = Const.a.stringTable[561];
			}
		}
		if (BetaGroveCyberUnlocked) Const.a.questData.BetaGroveCyberUnlocked = !Const.a.questData.BetaGroveCyberUnlocked;
		if (GroveAlphaJettisonEnabled) Const.a.questData.GroveAlphaJettisonEnabled = !Const.a.questData.GroveAlphaJettisonEnabled;
		if (GroveBetaJettisonEnabled) Const.a.questData.GroveBetaJettisonEnabled = !Const.a.questData.GroveBetaJettisonEnabled;
		if (GroveDeltaJettisonEnabled) Const.a.questData.GroveDeltaJettisonEnabled = !Const.a.questData.GroveDeltaJettisonEnabled;
		if (MasterJettisonBroken) {
			Const.a.questData.MasterJettisonBroken = !Const.a.questData.MasterJettisonBroken;
			if (Const.a.questData.MasterJettisonBroken) {
				QuestLogNotesManager.a.notes[11].SetActive(true); // Diagnose and repair broken relay
				QuestLogNotesManager.a.labels[11].text = Const.a.stringTable[563];// Set:Diagnose and repair broken relay
			}
		}
		if (Relay428Fixed) {
			Const.a.questData.Relay428Fixed = !Const.a.questData.Relay428Fixed;
			QuestLogNotesManager.a.checkBoxes[11].isOn = Const.a.questData.Relay428Fixed;
			if (Const.a.questData.Relay428Fixed) {
				QuestLogNotesManager.a.notes[11].SetActive(true);
				QuestLogNotesManager.a.labels[11].text = Const.a.stringTable[563]; // Set:Diagnose and repair broken relay
				QuestLogNotesManager.a.labels[11].text += Const.a.stringTable[564]; // Add:: 428.
			}
		}
		if (MasterJettisonEnabled) {
			Const.a.questData.MasterJettisonEnabled = !Const.a.questData.MasterJettisonEnabled;
			QuestLogNotesManager.a.checkBoxes[10].isOn = Const.a.questData.MasterJettisonEnabled;
			if (Const.a.questData.MasterJettisonEnabled) {
				QuestLogNotesManager.a.notes[10].SetActive(true);
				QuestLogNotesManager.a.labels[10].text = Const.a.stringTable[562];
			}
		}
		if (BetaGroveJettisoned) {
			Const.a.questData.BetaGroveJettisoned = !Const.a.questData.BetaGroveJettisoned;
			QuestLogNotesManager.a.checkBoxes[12].isOn = Const.a.questData.BetaGroveJettisoned;
			if (Const.a.questData.BetaGroveJettisoned ) {
				QuestLogNotesManager.a.notes[12].SetActive(true);
				QuestLogNotesManager.a.labels[12].text = Const.a.stringTable[565];
				QuestLogNotesManager.a.notes[13].SetActive(true);
				QuestLogNotesManager.a.labels[13].text = Const.a.stringTable[566];
			}
		}
		if (AntennaNorthDestroyed) Const.a.questData.AntennaNorthDestroyed = !Const.a.questData.AntennaNorthDestroyed;
		if (AntennaSouthDestroyed) Const.a.questData.AntennaSouthDestroyed = !Const.a.questData.AntennaSouthDestroyed;
		if (AntennaEastDestroyed) Const.a.questData.AntennaEastDestroyed = !Const.a.questData.AntennaEastDestroyed;
		if (AntennaWestDestroyed) Const.a.questData.AntennaWestDestroyed = !Const.a.questData.AntennaWestDestroyed;
		if (SelfDestructActivated) {
			Const.a.questData.SelfDestructActivated = !Const.a.questData.SelfDestructActivated;
			if (Const.a.questData.SelfDestructActivated) {
				QuestLogNotesManager.a.notes[14].SetActive(true);
				QuestLogNotesManager.a.notes[15].SetActive(true); // Escape pod
				QuestLogNotesManager.a.labels[14].text = Const.a.stringTable[567];// Set:Engage reactor self-destruct.
				QuestLogNotesManager.a.labels[15].text = Const.a.stringTable[568];// Set:Escape on escape pod.
			}
		}
		if (BridgeSeparated) {
			Const.a.questData.BridgeSeparated = !Const.a.questData.BridgeSeparated;
			if (Const.a.questData.BridgeSeparated) {
				QuestLogNotesManager.a.notes[16].SetActive(true);
				QuestLogNotesManager.a.notes[17].SetActive(true);
				QuestLogNotesManager.a.checkBoxes[16].isOn = true;
				QuestLogNotesManager.a.labels[16].text = Const.a.stringTable[569]; // Set:Access the bridge.
				QuestLogNotesManager.a.labels[17].text = Const.a.stringTable[570]; // Set:Destroy SHODAN.
			}
		}
		if (IsolinearChipsetInstalled) Const.a.questData.IsolinearChipsetInstalled = !Const.a.questData.IsolinearChipsetInstalled;
	}

	public void TestBits(bool testIfTrue, UseData ud, TargetIO tio) {
		if (RobotSpawnDeactivated && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.RobotSpawnDeactivated, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (IsotopeInstalled && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.IsotopeInstalled, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (ShieldActivated && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.ShieldActivated, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (LaserSafetyOverriden && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.LaserSafetyOverriden, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (LaserDestroyed && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.LaserDestroyed, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (BetaGroveCyberUnlocked && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.BetaGroveCyberUnlocked, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (GroveAlphaJettisonEnabled && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.GroveAlphaJettisonEnabled, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (GroveBetaJettisonEnabled && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.GroveBetaJettisonEnabled, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (GroveDeltaJettisonEnabled && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.GroveDeltaJettisonEnabled, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (MasterJettisonBroken && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.MasterJettisonBroken, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (Relay428Fixed && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.Relay428Fixed, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (MasterJettisonEnabled && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.MasterJettisonEnabled, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (BetaGroveJettisoned && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.BetaGroveJettisoned, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (AntennaNorthDestroyed && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.AntennaNorthDestroyed, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (AntennaSouthDestroyed && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.AntennaSouthDestroyed, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (AntennaEastDestroyed && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.AntennaEastDestroyed, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (AntennaWestDestroyed && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.AntennaWestDestroyed, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (SelfDestructActivated && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.SelfDestructActivated, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (BridgeSeparated && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.BridgeSeparated, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
		if (IsolinearChipsetInstalled && (!string.IsNullOrWhiteSpace(target) || !string.IsNullOrWhiteSpace(targetIfFalse))) Const.a.questData.TargetOnGatePassed(Const.a.questData.IsolinearChipsetInstalled, testIfTrue, ud, tio, target, argvalue, targetIfFalse, argvalueIfFalse);
	}
}
