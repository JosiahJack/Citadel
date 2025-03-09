using System;
using System.Text;
using UnityEngine;

// Applies the color distortion effect from berserk patch.
//
// This picks various textures to use for a color replacement effect.  The look
// appears random but is series of fixed lookup tables that switch in sequence.
//
// This is an image effect that modifies a rendered image as rendered by the
// MainCamera.
//
// This is required to be paired with the BerserkEffect.shader and its shader
// properties must match:
// - _EffectStrength - The strength of the color modification.
// - _SwapTex - The colors lookup table texture used for color swapping.
// - _LowThreshold - The lower threshold below which no affect is applied.
// - _HighThreshold - The upper threshold above which no affect is applied.
//
// As berserk progresses, the effect gets "worse" by applying the affect to a
// greater percentage of the screen by raising the high threshold and gets more
// intense by raising the strength until finally the effect wears off rapidly.
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Berserk Effect")]
public class BerserkEffect : UnityStandardAssets.ImageEffects.ImageEffectBase {
	public Texture2D swapTexture;

	[HideInInspector] public float effectStrength = 3f; // save
	[HideInInspector] public float hithreshold = 0.25f; // save

	void Awake() {
		effectStrength = 3.0f;
		hithreshold = 0.25f;
	}

	public void Reset() {
		effectStrength = 3.0f;
		hithreshold = 0.25f;
	}

	public void IncrementStats() {
		effectStrength += 1.0f;
		hithreshold += 0.25f;
		if (hithreshold > 1f) hithreshold = 1f;
		if (hithreshold < 0.26f) hithreshold = 0.26f;
	}

	public void IncrementStrength() {
		effectStrength += 1.0f;
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		material.SetTexture("_SwapTex", swapTexture);
		material.SetFloat("_EffectStrength", effectStrength);
		material.SetFloat("_LowThreshold", 0.0f);
		material.SetFloat("_HighThreshold", hithreshold);
		Graphics.Blit (source, destination, material);
	}

	public static string Save(GameObject go) {
		BerserkEffect bzk = go.GetComponent<BerserkEffect>();
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.BoolToString(bzk.enabled,"BerserkEffect.enabled"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(bzk.hithreshold,"BerserkEffect.hithreshold"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(bzk.effectStrength,"BerserkEffect.effectStrength"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		BerserkEffect bzk = go.GetComponent<BerserkEffect>();
		bzk.enabled = Utils.GetBoolFromString(entries[index],"BerserkEffect.enabled"); index++;
		bzk.hithreshold = Utils.GetFloatFromString(entries[index],"BerserkEffect.hithreshold"); index++;
		bzk.effectStrength = Utils.GetFloatFromString(entries[index],"BerserkEffect.effectStrength"); index++;
		return index;
	}
}
