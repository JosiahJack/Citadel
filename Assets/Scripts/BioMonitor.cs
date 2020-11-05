using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BioMonitor : MonoBehaviour {
	public Text heartRate;
	public Text patchEffects;
	public Text heartRateText;
	public Text header;
	public Text patchesActiveText;
	public Text bpmText;
	public Text fatigueDetailText;
	public Text fatigue;
	public float beatTick = 0.5f;

	public PlayerMovement pm;
	public PlayerPatch ph;
	public HardwareInvCurrent hwc;
	public HardwareInventory hwi;

	private string tempStr;
	private float beatFinished; // Visual only, Time.time controlled

	void Start() {
		beatFinished = Time.time + beatTick;
		tempStr = "";
	}

    void Update() {
		if (!PauseScript.a.Paused() && beatFinished < Time.time) {
			if (hwc.hardwareIsActive[6]) {
				header.text = Const.a.stringTable[526];
				heartRateText.text = Const.a.stringTable[527];
				bpmText.text = Const.a.stringTable[529];

				fatigueDetailText.text = Const.a.stringTable[531];
				tempStr = Const.a.stringTable[532]; // High!
				if (pm.fatigue < 80f) tempStr = Const.a.stringTable[533]; // Moderate
				if (pm.fatigue < 30f) tempStr = Const.a.stringTable[534]; // Low

				fatigue.text = tempStr;
				tempStr = "";
				heartRate.text = ((70 + ((pm.fatigue/100f) * 110)) * Random.Range(0.9f,1.1f)).ToString("000");
				if (hwi.hardwareVersion[6] > 1) {
					patchesActiveText.text = Const.a.stringTable[528];
					if (Const.a.CheckFlags(ph.patchActive, ph.PATCH_MEDI)) tempStr += Const.a.stringTable[520];
					if (Const.a.CheckFlags(ph.patchActive, ph.PATCH_STAMINUP)) tempStr += Const.a.stringTable[521];
					if (Const.a.CheckFlags(ph.patchActive, ph.PATCH_SIGHT)) tempStr += Const.a.stringTable[522];
					if (Const.a.CheckFlags(ph.patchActive, ph.PATCH_GENIUS)) tempStr += Const.a.stringTable[523];
					if (Const.a.CheckFlags(ph.patchActive, ph.PATCH_BERSERK)) tempStr += Const.a.stringTable[524];
					if (Const.a.CheckFlags(ph.patchActive, ph.PATCH_REFLEX)) tempStr += Const.a.stringTable[525];
					if (Const.a.CheckFlags(ph.patchActive, ph.PATCH_DETOX)) tempStr = Const.a.stringTable[530];
					patchEffects.text = tempStr;
				} else {
					patchesActiveText.text = System.String.Empty;
					patchEffects.text = System.String.Empty;
				}
			}
			beatFinished = Time.time + beatTick;
		}
    }
}
