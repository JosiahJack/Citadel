using System.Text;
using UnityEngine;

public class QuestBits {
	public int lev1SecCode = -1;
	public int lev2SecCode = -1;
	public int lev3SecCode = -1;
	public int lev4SecCode = -1;
	public int lev5SecCode = -1;
	public int lev6SecCode = -1;
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

	public void ResetQuestData(QuestBits qD) {
		qD.lev1SecCode = Random.Range(0,10); // Must do repeatedly to prevent
											 // these all being the same number.
		qD.lev2SecCode = Random.Range(0,10);
		qD.lev3SecCode = Random.Range(0,10);
		qD.lev4SecCode = Random.Range(0,10);
		qD.lev5SecCode = Random.Range(0,10);
		qD.lev6SecCode = Random.Range(0,10);
		qD.RobotSpawnDeactivated = false;
		qD.IsotopeInstalled = false;
		qD.ShieldActivated = false;
		qD.LaserSafetyOverriden = false;
		qD.LaserDestroyed = false;
		qD.BetaGroveCyberUnlocked = false;
		qD.GroveAlphaJettisonEnabled = false;
		qD.GroveBetaJettisonEnabled = false;
		qD.GroveDeltaJettisonEnabled = false;
		qD.MasterJettisonBroken = false;
		qD.Relay428Fixed = false;
		qD.MasterJettisonEnabled = false;
		qD.BetaGroveJettisoned = false;
		qD.AntennaNorthDestroyed = false;
		qD.AntennaSouthDestroyed = false;
		qD.AntennaEastDestroyed = false;
		qD.AntennaWestDestroyed = false;
		qD.SelfDestructActivated = false;
		qD.BridgeSeparated = false;
		qD.IsolinearChipsetInstalled = false;
	}

	public void TargetOnGatePassed(bool bitToCheck, bool passIfTrue,
								   UseData ud, TargetIO tio,
								   string targ, string arg,
								   string targOnFalse, string argOnFalse) {
		if (passIfTrue) {
			if (!bitToCheck) { RunTargets(ud,tio,targOnFalse,argOnFalse); return; }
		} else {
			if (bitToCheck) { RunTargets(ud,tio,targOnFalse,argOnFalse); return; }
		}

		RunTargets(ud,tio,targ,arg);
	}

	public string Save() {
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(lev1SecCode.ToString()); s1.Append(Utils.splitChar);
		s1.Append(lev2SecCode.ToString()); s1.Append(Utils.splitChar);
		s1.Append(lev3SecCode.ToString()); s1.Append(Utils.splitChar);
		s1.Append(lev4SecCode.ToString()); s1.Append(Utils.splitChar);
		s1.Append(lev5SecCode.ToString()); s1.Append(Utils.splitChar);
		s1.Append(lev6SecCode.ToString()); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(RobotSpawnDeactivated)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(IsotopeInstalled)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ShieldActivated)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(LaserSafetyOverriden)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(LaserDestroyed)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(BetaGroveCyberUnlocked)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(GroveAlphaJettisonEnabled)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(GroveBetaJettisonEnabled)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(GroveDeltaJettisonEnabled)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(MasterJettisonBroken)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(Relay428Fixed)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(MasterJettisonEnabled)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(BetaGroveJettisoned)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(AntennaNorthDestroyed)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(AntennaSouthDestroyed)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(AntennaEastDestroyed)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(AntennaWestDestroyed)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(SelfDestructActivated)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(BridgeSeparated)); s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(IsolinearChipsetInstalled));
		return s1.ToString();
	}

	public int Load(ref string[] entries, int index) {
		lev1SecCode = Utils.GetIntFromString(entries[index]); index++;
		lev2SecCode = Utils.GetIntFromString(entries[index]); index++;
		lev3SecCode = Utils.GetIntFromString(entries[index]); index++;
		lev4SecCode = Utils.GetIntFromString(entries[index]); index++;
		lev5SecCode = Utils.GetIntFromString(entries[index]); index++;
		lev6SecCode = Utils.GetIntFromString(entries[index]); index++;
		RobotSpawnDeactivated = Utils.GetBoolFromString(entries[index]); index++;
		IsotopeInstalled = Utils.GetBoolFromString(entries[index]); index++;
		ShieldActivated = Utils.GetBoolFromString(entries[index]); index++;
		LaserSafetyOverriden = Utils.GetBoolFromString(entries[index]); index++;
		LaserDestroyed = Utils.GetBoolFromString(entries[index]); index++;
		BetaGroveCyberUnlocked = Utils.GetBoolFromString(entries[index]); index++;
		GroveAlphaJettisonEnabled = Utils.GetBoolFromString(entries[index]); index++;
		GroveBetaJettisonEnabled = Utils.GetBoolFromString(entries[index]); index++;
		GroveDeltaJettisonEnabled = Utils.GetBoolFromString(entries[index]); index++;
		MasterJettisonBroken = Utils.GetBoolFromString(entries[index]); index++;
		Relay428Fixed = Utils.GetBoolFromString(entries[index]); index++;
		MasterJettisonEnabled = Utils.GetBoolFromString(entries[index]); index++;
		BetaGroveJettisoned = Utils.GetBoolFromString(entries[index]); index++;
		AntennaNorthDestroyed = Utils.GetBoolFromString(entries[index]); index++;
		AntennaSouthDestroyed = Utils.GetBoolFromString(entries[index]); index++;
		AntennaEastDestroyed = Utils.GetBoolFromString(entries[index]); index++;
		AntennaWestDestroyed = Utils.GetBoolFromString(entries[index]); index++;
		SelfDestructActivated = Utils.GetBoolFromString(entries[index]); index++;
		BridgeSeparated = Utils.GetBoolFromString(entries[index]); index++;
		IsolinearChipsetInstalled = Utils.GetBoolFromString(entries[index]); index++;
		return index;
	}

	private void RunTargets(UseData ud, TargetIO tio, string target, string argvalue) {
		ud.argvalue = argvalue; // grr, arg! (Mutant Enemy reference alert)
		ud.SetBits(tio);
		Const.a.UseTargets(ud,target);
	}
}
