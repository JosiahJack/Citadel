using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BioMonitor : MonoBehaviour {
	// External references, required
	public Text heartRate;
	public Text patchEffects;
	public Text heartRateText;
	public Text header;
	public Text patchesActiveText;
	public Text bpmText;
	public Text fatigueDetailText;
	public Text fatigue;

	// Internal references
	private float beatTick = 0.5f;
	private StringBuilder tempStr;
	private float beatFinished; // Visual only, Time.time controlled

	void Start() {
		beatFinished = Time.time + beatTick;
		tempStr = new StringBuilder();
		tempStr.Clear();
	}

    void Update() {
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) return;
		if (!Inventory.a.hardwareIsActive[6]) return;
		if (beatFinished >= Time.time) return;

		beatFinished = Time.time + beatTick;
		header.text = Const.a.stringTable[526];
		heartRateText.text = Const.a.stringTable[527];
		bpmText.text = Const.a.stringTable[529];
		fatigueDetailText.text = Const.a.stringTable[531];
		tempStr.Clear();
		if (PlayerMovement.a.fatigue < 80f) tempStr.Append(Const.a.stringTable[533]); // Moderate
		else if (PlayerMovement.a.fatigue < 30f) tempStr.Append(Const.a.stringTable[534]); // Low
		else tempStr.Append(Const.a.stringTable[532]); // High!

		fatigue.text = tempStr.ToString();
		tempStr.Clear();
		heartRate.text = ((70 + ((PlayerMovement.a.fatigue/100f) * 110)) * Random.Range(0.9f,1.1f)).ToString("000");
		if (Inventory.a.BioMonitorVersion() > 1
			&& Utils.CheckFlags(PlayerPatch.a.patchActive, 127)) {
			patchesActiveText.text = Const.a.stringTable[528];
			if (Utils.CheckFlags(PlayerPatch.a.patchActive, PlayerPatch.a.PATCH_MEDI)) tempStr.Append(Const.a.stringTable[520]);
			if (Utils.CheckFlags(PlayerPatch.a.patchActive, PlayerPatch.a.PATCH_STAMINUP)) tempStr.Append(Const.a.stringTable[521]);
			if (Utils.CheckFlags(PlayerPatch.a.patchActive, PlayerPatch.a.PATCH_SIGHT)) tempStr.Append(Const.a.stringTable[522]);
			if (Utils.CheckFlags(PlayerPatch.a.patchActive, PlayerPatch.a.PATCH_GENIUS)) tempStr.Append(Const.a.stringTable[523]);
			if (Utils.CheckFlags(PlayerPatch.a.patchActive, PlayerPatch.a.PATCH_BERSERK)) tempStr.Append(Const.a.stringTable[524]);
			if (Utils.CheckFlags(PlayerPatch.a.patchActive, PlayerPatch.a.PATCH_REFLEX)) tempStr.Append(Const.a.stringTable[525]);
			if (Utils.CheckFlags(PlayerPatch.a.patchActive, PlayerPatch.a.PATCH_DETOX)) tempStr.Append(Const.a.stringTable[530]);
			patchEffects.text = tempStr.ToString();
		} else {
			patchesActiveText.text = System.String.Empty;
			patchEffects.text = System.String.Empty;
		}
    }
}
