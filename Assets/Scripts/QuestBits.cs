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
		qD.lev1SecCode = Random.Range(0,10); // Must do repeatedly to prevent these all being the same number.
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

	public void TargetOnGatePassed(bool bitToCheck, bool passIfTrue, UseData ud, TargetIO tio, string targ, string arg, string targOnFalse, string argOnFalse) {
		if (passIfTrue) {
			if (!bitToCheck) { RunTargets(ud,tio,targOnFalse,argOnFalse); return; }
		} else {
			if (bitToCheck) { RunTargets(ud,tio,targOnFalse,argOnFalse); return; }
		}

		RunTargets(ud,tio,targ,arg);
	}

	private void RunTargets(UseData ud, TargetIO tio, string target, string argvalue) {
		ud.argvalue = argvalue; // grr, arg! (Mutant Enemy reference alert)
		ud.SetBits(tio);
		Const.a.UseTargets(ud,target);
	}
}
