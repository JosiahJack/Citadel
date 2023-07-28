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
		s1.Append(Utils.IntToString(lev1SecCode,"lev1SecCode"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(lev2SecCode,"lev2SecCode");
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(lev3SecCode,"lev3SecCode");
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(lev4SecCode,"lev4SecCode")a;
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(lev5SecCode,"lev5SecCode"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(lev6SecCode,"lev6SecCode"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(RobotSpawnDeactivated,
		                             "RobotSpawnDeactivated"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(IsotopeInstalled,"IsotopeInstalled"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(ShieldActivated,"ShieldActivated"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(LaserSafetyOverriden,
		                             "LaserSafetyOverriden"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(LaserDestroyed,"LaserDestroyed"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(BetaGroveCyberUnlocked,
		                             "BetaGroveCyberUnlocked"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(GroveAlphaJettisonEnabled,
		                             "GroveAlphaJettisonEnabled"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(GroveBetaJettisonEnabled,
		                             "GroveBetaJettisonEnabled"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(GroveDeltaJettisonEnabled,
		                             "GroveDeltaJettisonEnabled"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(MasterJettisonBroken,
		                             "MasterJettisonBroken"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(Relay428Fixed,"Relay428Fixed"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(MasterJettisonEnabled,
		                             "MasterJettisonEnabled"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(BetaGroveJettisoned,
		                             "BetaGroveJettisoned"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(AntennaNorthDestroyed,
		                             "AntennaNorthDestroyed"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(AntennaSouthDestroyed,
		                             "AntennaSouthDestroyed"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(AntennaEastDestroyed,
		                             "AntennaEastDestroyed"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(AntennaWestDestroyed,
		                             "AntennaWestDestroyed"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(SelfDestructActivated,
		                             "SelfDestructActivated"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(BridgeSeparated,"BridgeSeparated"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(IsolinearChipsetInstalled,
		                             "IsolinearChipsetInstalled"));
		return s1.ToString();
	}

	public int Load(ref string[] entries, int index) {
		lev1SecCode = Utils.GetIntFromString(entries[index],"lev1SecCode");
		index++;

		lev2SecCode = Utils.GetIntFromString(entries[index],"lev2SecCode");
		index++;

		lev3SecCode = Utils.GetIntFromString(entries[index],"lev3SecCode");
		index++;

		lev4SecCode = Utils.GetIntFromString(entries[index],"lev4SecCode");
		index++;

		lev5SecCode = Utils.GetIntFromString(entries[index],"lev5SecCode");
		index++;

		lev6SecCode = Utils.GetIntFromString(entries[index],"lev6SecCode");
		index++;

		RobotSpawnDeactivated = Utils.GetBoolFromString(entries[index],
		                                                "RobotSpawnDeactivated");
		index++;

		IsotopeInstalled = Utils.GetBoolFromString(entries[index],
		                                           "IsotopeInstalled");
		index++;

		ShieldActivated = Utils.GetBoolFromString(entries[index],
		                                          "ShieldActivated");
		index++;
		LevelManager.a.exterior_shield.SetActive(ShieldActivated);

		LaserSafetyOverriden = Utils.GetBoolFromString(entries[index],
		                                               "LaserSafetyOverriden");
		index++;

		LaserDestroyed = Utils.GetBoolFromString(entries[index],
		                                         "LaserDestroyed");
		index++;

		BetaGroveCyberUnlocked = Utils.GetBoolFromString(entries[index],
		                                                 "BetaGroveCyberUnlocked");
		index++;

		GroveAlphaJettisonEnabled = Utils.GetBoolFromString(entries[index],
		                                          "GroveAlphaJettisonEnabled");
		index++;

		GroveBetaJettisonEnabled = Utils.GetBoolFromString(entries[index],
		                                          "GroveBetaJettisonEnabled");
		index++;

		GroveDeltaJettisonEnabled = Utils.GetBoolFromString(entries[index],
		                                          "GroveDeltaJettisonEnabled");
		index++;

		MasterJettisonBroken = Utils.GetBoolFromString(entries[index],
		                                               "MasterJettisonBroken");
		index++;

		Relay428Fixed = Utils.GetBoolFromString(entries[index],
		                                        "Relay428Fixed");
		index++;

		MasterJettisonEnabled = Utils.GetBoolFromString(entries[index],
		                                              "MasterJettisonEnabled");
		index++;

		BetaGroveJettisoned = Utils.GetBoolFromString(entries[index],
		                                              "BetaGroveJettisoned");
		index++;

		AntennaNorthDestroyed = Utils.GetBoolFromString(entries[index],
		                                              "AntennaNorthDestroyed");
		index++;

		AntennaSouthDestroyed = Utils.GetBoolFromString(entries[index],
		                                              "AntennaSouthDestroyed");
		index++;

		AntennaEastDestroyed = Utils.GetBoolFromString(entries[index],
		                                               "AntennaEastDestroyed");
		index++;

		AntennaWestDestroyed = Utils.GetBoolFromString(entries[index],
		                                               "AntennaWestDestroyed");
		index++;

		SelfDestructActivated = Utils.GetBoolFromString(entries[index],
		                                              "SelfDestructActivated");
		index++;

		BridgeSeparated = Utils.GetBoolFromString(entries[index],
		                                          "BridgeSeparated");
		index++;

		IsolinearChipsetInstalled = Utils.GetBoolFromString(entries[index],
		                                          "IsolinearChipsetInstalled");
		index++;

		return index;
	}

	private void RunTargets(UseData ud, TargetIO tio, string target, string argvalue) {
		ud.argvalue = argvalue; // grr, arg! (Mutant Enemy reference alert)
		ud.SetBits(tio);
		Const.a.UseTargets(ud,target);
	}
}
