using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Color Adjustments/Grayscale")]
public class Grayscale : UnityStandardAssets.ImageEffects.ImageEffectBase {
    public Texture  textureRamp;
    public float    rampOffset;

    // Called by camera to apply image effect
    void OnRenderImage (RenderTexture source, RenderTexture destination) {
        material.SetTexture("_RampTex", textureRamp);
        material.SetFloat("_RampOffset", rampOffset);
        Graphics.Blit (source, destination, material);
    }
}
