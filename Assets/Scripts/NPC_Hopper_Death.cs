using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Hopper_Death : MonoBehaviour {
	private float blendAmount1 = 0;
	private float blendAmount2 = 0;
	private int blendStep1 = -1;
	private int blendStep2 = 0;
	private float blendAmountPerTick = 18f;
	private float redBlendAmountPerTick = 0.5f;
	private float rimPowerShiftPerTick = 15f;
	private float blendShapeTickSecs = 0.05f;
	private float blendRimColorTickSecs = 0.05f;
	private float redTint = 0f;
	private float rimPower = 255f;
	private float tick2Finished;
	private float tickFinished;
	private SkinnedMeshRenderer smr;

	void Awake() {
		smr = GetComponent<SkinnedMeshRenderer>();
		redTint = 0f;
		rimPower = 255f;
		tick2Finished = PauseScript.a.relativeTime + blendRimColorTickSecs;
		tickFinished = PauseScript.a.relativeTime + blendShapeTickSecs;
		blendStep1 = -1;
		blendStep2 = 0;
		blendAmountPerTick = 18f;
		blendAmount1 = 0;
		blendAmount2 = 0;
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (tick2Finished < PauseScript.a.relativeTime) {
				rimPower = smr.material.GetColor("_RimColor").r;
				rimPower -= rimPowerShiftPerTick;
				if (rimPower < 0) rimPower = 0;
				smr.material.SetColor("_RimColor",new Color(rimPower,0,(rimPower*0.75f),0));
				tick2Finished = PauseScript.a.relativeTime + blendRimColorTickSecs;
			}

			if (tickFinished < PauseScript.a.relativeTime && blendStep2 < 6) {
				blendAmount1 -= blendAmountPerTick;
				blendAmount2 += blendAmountPerTick;
				if (blendStep2 >= 3) {
					redTint -= redBlendAmountPerTick;
				} else {
					redTint += redBlendAmountPerTick;
				}

				if (blendAmount1 < 0) blendAmount1 = 0;
				if (blendStep1 >= 0 && blendStep1 < 5) smr.SetBlendShapeWeight(blendStep1,blendAmount1);
				if (blendStep2 >= 0 && blendStep2 < 5) smr.SetBlendShapeWeight(blendStep2,blendAmount2);
				if (blendStep2 < 5) {
					if (smr.GetBlendShapeWeight(blendStep2) >= 100) {
						blendStep1++;
						blendStep2++;
						blendAmount1 = 100;
						blendAmount2 = 0;
					}
				} else {
					blendStep2 = 6;
					smr.SetBlendShapeWeight(0,0);
					smr.SetBlendShapeWeight(1,0);
					smr.SetBlendShapeWeight(2,0);
					smr.SetBlendShapeWeight(3,0);
					smr.SetBlendShapeWeight(4,100);
					redTint = 0f;
				}

				if (redTint < 0) redTint = 0; //floor
				if (redTint > 10) redTint = 10; // ceil
				smr.material.SetColor("_HSVAAdjust",new Color(redTint,0,0,0));
				tickFinished = PauseScript.a.relativeTime + blendShapeTickSecs;
			}
		}
	}
}
