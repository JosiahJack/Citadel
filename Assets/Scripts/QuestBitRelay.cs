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
		if (RobotSpawnDeactivated) Const.a.questData.RobotSpawnDeactivated = true;
		if (IsotopeInstalled) Const.a.questData.IsotopeInstalled = true;
		if (ShieldActivated) Const.a.questData.ShieldActivated = true;
		if (LaserSafetyOverriden) Const.a.questData.LaserSafetyOverriden = true;
		if (LaserDestroyed) Const.a.questData.LaserDestroyed = true;
		if (BetaGroveCyberUnlocked) Const.a.questData.BetaGroveCyberUnlocked = true;
		if (GroveAlphaJettisonEnabled) Const.a.questData.GroveAlphaJettisonEnabled = true;
		if (GroveBetaJettisonEnabled) Const.a.questData.GroveBetaJettisonEnabled = true;
		if (GroveDeltaJettisonEnabled) Const.a.questData.GroveDeltaJettisonEnabled = true;
		if (MasterJettisonBroken) Const.a.questData.MasterJettisonBroken = true;
		if (Relay428Fixed) Const.a.questData.Relay428Fixed = true;
		if (MasterJettisonEnabled) Const.a.questData.MasterJettisonEnabled = true;
		if (BetaGroveJettisoned) Const.a.questData.BetaGroveJettisoned = true;
		if (AntennaNorthDestroyed) Const.a.questData.AntennaNorthDestroyed = true;
		if (AntennaSouthDestroyed) Const.a.questData.AntennaSouthDestroyed = true;
		if (AntennaEastDestroyed) Const.a.questData.AntennaEastDestroyed = true;
		if (AntennaWestDestroyed) Const.a.questData.AntennaWestDestroyed = true;
		if (SelfDestructActivated) Const.a.questData.SelfDestructActivated = true;
		if (BridgeSeparated) Const.a.questData.BridgeSeparated = true;
		if (IsolinearChipsetInstalled) Const.a.questData.IsolinearChipsetInstalled = true;

		Const.a.DebugQuestBitShoutOut();
	}

	public void DisableBits() {
		if (RobotSpawnDeactivated) Const.a.questData.RobotSpawnDeactivated = false;
		if (IsotopeInstalled) Const.a.questData.IsotopeInstalled = false;
		if (ShieldActivated) Const.a.questData.ShieldActivated = false;
		if (LaserSafetyOverriden) Const.a.questData.LaserSafetyOverriden = false;
		if (LaserDestroyed) Const.a.questData.LaserDestroyed = false;
		if (BetaGroveCyberUnlocked) Const.a.questData.BetaGroveCyberUnlocked = false;
		if (GroveAlphaJettisonEnabled) Const.a.questData.GroveAlphaJettisonEnabled = false;
		if (GroveBetaJettisonEnabled) Const.a.questData.GroveBetaJettisonEnabled = false;
		if (GroveDeltaJettisonEnabled) Const.a.questData.GroveDeltaJettisonEnabled = false;
		if (MasterJettisonBroken) Const.a.questData.MasterJettisonBroken = false;
		if (Relay428Fixed) Const.a.questData.Relay428Fixed = false;
		if (MasterJettisonEnabled) Const.a.questData.MasterJettisonEnabled = false;
		if (BetaGroveJettisoned) Const.a.questData.BetaGroveJettisoned = false;
		if (AntennaNorthDestroyed) Const.a.questData.AntennaNorthDestroyed = false;
		if (AntennaSouthDestroyed) Const.a.questData.AntennaSouthDestroyed = false;
		if (AntennaEastDestroyed) Const.a.questData.AntennaEastDestroyed = false;
		if (AntennaWestDestroyed) Const.a.questData.AntennaWestDestroyed = false;
		if (SelfDestructActivated) Const.a.questData.SelfDestructActivated = false;
		if (BridgeSeparated) Const.a.questData.BridgeSeparated = false;
		if (IsolinearChipsetInstalled) Const.a.questData.IsolinearChipsetInstalled = false;
		Const.a.DebugQuestBitShoutOut();
	}

    public void ToggleBits() {
		if (RobotSpawnDeactivated) Const.a.questData.RobotSpawnDeactivated = !Const.a.questData.RobotSpawnDeactivated;
		if (IsotopeInstalled) Const.a.questData.IsotopeInstalled = !Const.a.questData.IsotopeInstalled;
		if (ShieldActivated) Const.a.questData.ShieldActivated = !Const.a.questData.ShieldActivated;
		if (LaserSafetyOverriden) Const.a.questData.LaserSafetyOverriden = !Const.a.questData.LaserSafetyOverriden;
		if (LaserDestroyed) Const.a.questData.LaserDestroyed = !Const.a.questData.LaserDestroyed;
		if (BetaGroveCyberUnlocked) Const.a.questData.BetaGroveCyberUnlocked = !Const.a.questData.BetaGroveCyberUnlocked;
		if (GroveAlphaJettisonEnabled) Const.a.questData.GroveAlphaJettisonEnabled = !Const.a.questData.GroveAlphaJettisonEnabled;
		if (GroveBetaJettisonEnabled) Const.a.questData.GroveBetaJettisonEnabled = !Const.a.questData.GroveBetaJettisonEnabled;
		if (GroveDeltaJettisonEnabled) Const.a.questData.GroveDeltaJettisonEnabled = !Const.a.questData.GroveDeltaJettisonEnabled;
		if (MasterJettisonBroken) Const.a.questData.MasterJettisonBroken = !Const.a.questData.MasterJettisonBroken;
		if (Relay428Fixed) Const.a.questData.Relay428Fixed = !Const.a.questData.Relay428Fixed;
		if (MasterJettisonEnabled) Const.a.questData.MasterJettisonEnabled = !Const.a.questData.MasterJettisonEnabled;
		if (BetaGroveJettisoned) Const.a.questData.BetaGroveJettisoned = !Const.a.questData.BetaGroveJettisoned;
		if (AntennaNorthDestroyed) Const.a.questData.AntennaNorthDestroyed = !Const.a.questData.AntennaNorthDestroyed;
		if (AntennaSouthDestroyed) Const.a.questData.AntennaSouthDestroyed = !Const.a.questData.AntennaSouthDestroyed;
		if (AntennaEastDestroyed) Const.a.questData.AntennaEastDestroyed = !Const.a.questData.AntennaEastDestroyed;
		if (AntennaWestDestroyed) Const.a.questData.AntennaWestDestroyed = !Const.a.questData.AntennaWestDestroyed;
		if (SelfDestructActivated) Const.a.questData.SelfDestructActivated = !Const.a.questData.SelfDestructActivated;
		if (BridgeSeparated) Const.a.questData.BridgeSeparated = !Const.a.questData.BridgeSeparated;
		if (IsolinearChipsetInstalled) Const.a.questData.IsolinearChipsetInstalled = !Const.a.questData.IsolinearChipsetInstalled;

		Const.a.DebugQuestBitShoutOut();
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
