using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Berserk Effect")]
public class BerserkEffect : UnityStandardAssets.ImageEffects.ImageEffectBase {
	public Texture2D swapTexture;
	public float effectStrength = 3f;
	public float lothreshold = 0.0f;
	public float hithreshold = 0.25f; // save

	private float oldeffectStrength;
	private float oldlothreshold;
	private float oldhithreshold;

	void Awake () {
		oldeffectStrength = effectStrength;
		oldlothreshold = lothreshold;
		oldhithreshold = hithreshold;
	}

	public void Reset() {
		effectStrength = oldeffectStrength;
		lothreshold = oldlothreshold;
		hithreshold = oldhithreshold;
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		material.SetTexture("_SwapTex", swapTexture);
		material.SetFloat("_EffectStrength", effectStrength);
		if (lothreshold < 0f) lothreshold = 0f;
		if (hithreshold > 1f) hithreshold = 1f;
		if (hithreshold < 0) hithreshold = 0;
		if (lothreshold > hithreshold || hithreshold < lothreshold) {
			lothreshold = hithreshold;
		}
		material.SetFloat("_LowThreshold", lothreshold);
		material.SetFloat("_HighThreshold", hithreshold);
		Graphics.Blit (source, destination, material);
	}

	public static string Save(GameObject go) {
		BerserkEffect bzk = go.GetComponent<BerserkEffect>();
		if (bzk == null) {
			return Utils.DTypeWordToSaveString("bf"); // No warn, only placed on GunCamera to avoid duplicate application.

		}

		string line = System.String.Empty;
		line = Utils.BoolToString(bzk.enabled);
		line += Utils.splitChar + Utils.FloatToString(bzk.hithreshold);
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		BerserkEffect bzk = go.GetComponent<BerserkEffect>();
		if (bzk == null || index < 0 || entries == null) return index + 2;

		bzk.enabled = Utils.GetBoolFromString(entries[index]); index++;
		bzk.hithreshold = Utils.GetFloatFromString(entries[index]); index++;
		return index;
	}
}
