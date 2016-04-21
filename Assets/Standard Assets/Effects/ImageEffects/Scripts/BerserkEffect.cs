using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects {
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Color Adjustments/Berserk Effect")]
    public class BerserkEffect : ImageEffectBase {
		public Texture2D swapTexture;
		public float effectStrength = 0.3f;
		public float lothreshold = 0.0f;
		public float hithreshold = 1.0f;

        void OnRenderImage (RenderTexture source, RenderTexture destination) {
			material.SetTexture("_SwapTex", swapTexture);
			material.SetFloat("_EffectStrength", effectStrength);
			if (lothreshold < 0f)
				lothreshold = 0f;

			if (hithreshold > 1f)
				hithreshold = 1f;

			if (hithreshold < 0)
				hithreshold = 0;
			
			if (lothreshold > hithreshold || hithreshold < lothreshold)
				lothreshold = hithreshold;
			
			material.SetFloat("_LowThreshold", lothreshold);
			material.SetFloat("_HighThreshold", hithreshold);
            Graphics.Blit (source, destination, material);
        }
    }
}
