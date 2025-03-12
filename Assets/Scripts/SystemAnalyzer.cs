using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemAnalyzer : MonoBehaviour {
	public Text descSecurity;
	public Text security;
	public Text descLaser;
	public Text laser;
	public Text descLifepod;
	public Text lifepod;
	public Text descShield;
	public Text shield;
	public Text descReactor;
	public Text reactor;
	public Text descProcessor;
	public Text processor;
	public Text descProgram;
	public Text program;
	public Text descAlpha;
	public Text alpha;
	public Text descBeta;
	public Text beta;
	public Text descGamma;
	public Text gamma;
	public Text descDelta;
	public Text delta;

	public void Close() {
		MFDManager.a.sysAnalyzerLH.SetActive(false);
		MFDManager.a.sysAnalyzerRH.SetActive(false);
		MFDManager.a.mouseClickHeldOverGUI = true;
		GUIState.a.ClearOverButton();
	}
    // Start is called before the first frame update
    void Update() {
		descSecurity.text = Const.a.stringTable[474];
		security.text = LevelManager.a.levelSecurity[LevelManager.a.currentLevel] + Const.a.stringTable[307];
		descLaser.text = Const.a.stringTable[475];
		laser.text = Const.a.questData.LaserDestroyed ? Const.a.stringTable[486] : Const.a.stringTable[485];
		descLifepod.text = Const.a.stringTable[476];
		lifepod.text = Const.a.questData.SelfDestructActivated ? Const.a.stringTable[488] : Const.a.stringTable[487];
		descShield.text = Const.a.stringTable[477];
		shield.text = Const.a.questData.ShieldActivated ? Const.a.stringTable[490] : Const.a.stringTable[489];
		descReactor.text = Const.a.stringTable[478];
		reactor.text = Const.a.questData.SelfDestructActivated ? Const.a.stringTable[491] : Const.a.stringTable[492];
		descProcessor.text = Const.a.stringTable[479];
		int nodeCount = 0;
		for (int i=0;i<14;i++) {
			nodeCount += LevelManager.a.levelSmallNodeCount[i];
			nodeCount += LevelManager.a.levelLargeNodeCount[i];
			nodeCount -= LevelManager.a.levelSmallNodeDestroyedCount[i];
			nodeCount -= LevelManager.a.levelLargeNodeDestroyedCount[i];
		}
		processor.text = nodeCount.ToString();
		descProgram.text = Const.a.stringTable[480];
		if (!Const.a.questData.LaserDestroyed) {
			program.text = Const.a.stringTable[494];
		} else {
			if (!Const.a.questData.BetaGroveJettisoned) {
				program.text = Const.a.stringTable[495];
			} else {
				if (!(Const.a.questData.AntennaNorthDestroyed && Const.a.questData.AntennaSouthDestroyed && Const.a.questData.AntennaWestDestroyed && Const.a.questData.AntennaEastDestroyed)) {
					program.text = Const.a.stringTable[496];
				} else {
					if (!Const.a.questData.BridgeSeparated) {
						program.text = Const.a.stringTable[497];
					} else {
						program.text = Const.a.stringTable[498];
					}
				}
			}
		}
		descAlpha.text = Const.a.stringTable[481];
		alpha.text = Const.a.stringTable[492];
		descBeta.text = Const.a.stringTable[482];
		beta.text = Const.a.questData.BetaGroveJettisoned ? Const.a.stringTable[493] : Const.a.stringTable[492];
		descGamma.text = Const.a.stringTable[483];
		gamma.text = Const.a.stringTable[493];
		descDelta.text = Const.a.stringTable[484];
		delta.text = Const.a.stringTable[492];
	}
}
