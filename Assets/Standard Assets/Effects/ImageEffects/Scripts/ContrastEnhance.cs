using System;
using UnityEngine;

// Modified by Josiah Jack to reduce Garbage GC pressure by persisting RenderTextures.
namespace UnityStandardAssets.ImageEffects {
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Color Adjustments/Contrast Enhance (Unsharp Mask)")]
    class ContrastEnhance : PostEffectsBase {
        public float intensity = 0.5f;
        public float threshold = 0.0f;
        public float blurSpread = 1.0f;
        public Shader separableBlurShader = null;
        public Shader contrastCompositeShader = null;
        
        private RenderTexture color2;
        private RenderTexture color4a;
        private RenderTexture color4b;
        private Material separableBlurMaterial;
        private Material contrastCompositeMaterial;
        private int rtW = -1;
        private int rtH = -1;
        private int lastRtW = -1;
        private int lastRtH = -1;
        private Vector4 verticalOffset = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        private Vector4 horizontalOffset = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

        void Awake() {
            CheckSupport(false);
        }
        
        public override bool CheckResources () {
            //CheckSupport(false);
            contrastCompositeMaterial = CheckShaderAndCreateMaterial(contrastCompositeShader, contrastCompositeMaterial);
            separableBlurMaterial = CheckShaderAndCreateMaterial(separableBlurShader, separableBlurMaterial);
            if (!isSupported) ReportAutoDisable();
            return isSupported;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (CheckResources()==false) {
                Graphics.Blit(source, destination);
                return;
            }

            rtW = source.width;
            rtH = source.height;
            if (rtW != lastRtW || rtH != lastRtH || color2 == null || color4a == null || color4b == null) {
                CleanupRenderTextures(); // Release old textures if they exist

                // Allocate new textures
                color2 = new RenderTexture(rtW / 2, rtH / 2, 0);
                color4a = new RenderTexture(rtW / 4, rtH / 4, 0);
                color4b = new RenderTexture(rtW / 4, rtH / 4, 0);

                // Mark them as persistent
                color2.Create();
                color4a.Create();
                color4b.Create();

                lastRtW = rtW;
                lastRtH = rtH;
            }

            // downsample
            Graphics.Blit (source, color2);
            Graphics.Blit (color2, color4a);

            // blur
            verticalOffset.y = (blurSpread * 1.0f) / color4a.height;
            separableBlurMaterial.SetVector("offsets", verticalOffset);
            Graphics.Blit(color4a, color4b, separableBlurMaterial);

            horizontalOffset.x = (blurSpread * 1.0f) / color4a.width;
            separableBlurMaterial.SetVector("offsets", horizontalOffset);
            Graphics.Blit(color4b, color4a, separableBlurMaterial);

            // composite
            contrastCompositeMaterial.SetTexture("_MainTexBlurred", color4a);
            contrastCompositeMaterial.SetFloat("intensity", intensity);
            contrastCompositeMaterial.SetFloat("threshhold", threshold);
            Graphics.Blit(source, destination, contrastCompositeMaterial);
        }
        
        private void CleanupRenderTextures() {
            if (color2 != null)  {  color2.Release();  color2 = null; }
            if (color4a != null) { color4a.Release(); color4a = null; }
            if (color4b != null) { color4b.Release(); color4b = null; }
        }

        void OnDisable() {
            CleanupRenderTextures();
            if (separableBlurMaterial != null) DestroyImmediate(separableBlurMaterial);
            if (contrastCompositeMaterial != null) DestroyImmediate(contrastCompositeMaterial);
        }
    }
}
