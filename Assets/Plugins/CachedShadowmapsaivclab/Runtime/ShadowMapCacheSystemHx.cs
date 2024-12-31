//#define HX

using System;

#if HX
namespace CachedShadowMap.Runtime
{
    [Serializable]
    public class ShadowMapCacheSystemHx : ShadowMapCacheSystem
    {
        private protected override void ComputeCachedShadowMap(CachedShadowMapLight light, float margin = 2)
        {
            base.ComputeCachedShadowMap(light,margin);
            HxVolumetricCamera.SetCachedShadowMaps(ActivelyCachedLights, ActivelyCachedTextures);
        }

        protected internal override void RevertToDynamicLighting(CachedShadowMapLight aLight)
        {
            base.RevertToDynamicLighting(aLight);
            HxVolumetricCamera.SetCachedShadowMaps(ActivelyCachedLights, ActivelyCachedTextures);
        }
    }
}
#endif
