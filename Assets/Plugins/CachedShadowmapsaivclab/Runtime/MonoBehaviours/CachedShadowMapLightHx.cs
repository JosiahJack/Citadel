#define AUTO_REMOVE_NON_INTERSECTION_DYNAMIC_OBJECTS
//#define HX

#if HX
using UnityEngine;

namespace CachedShadowMap.Runtime.MonoBehaviours
{
    /// <inheritdoc />
    ///  <summary>
    /// The component should be attached to any spot or point light that should be cached, with hx volumetric light support
    ///  </summary>
    [RequireComponent(typeof(HxVolumetricLight))]
    [DisallowMultipleComponent]
    public class CachedShadowMapLightHx : CachedShadowMapLight
    {
        public readonly static int[] SHADOW_MAP_RESOLUTION_VALUES = {256, 512, 1024, 2048};

        // extra high resolution when taking screenshots
        public static bool USE_4K_FOR_SCREENSHOTS = false;
        public static bool USE_8K_FOR_SCREENSHOTS = false;
        private readonly static int SCREENSHOT_4K = 4096;
        private readonly static int SCREENSHOT_8K = 8192;

        internal override void UpdateShadowMapResolution(int resolution = 1048)
        {
            // shadowmap resolutions: https://docs.unity3d.com/Manual/LightPerformance.html

            if (Player_Manager.Instance == null) // default
                currentResolution =
                    SHADOW_MAP_RESOLUTION_VALUES[GameSettingsData.SHADOW_MAP_RESOLUTION_DEFAULT];
            else if (Player_Manager.Instance != null)
            {
                // loaded from file
                var a = Player_Manager.Instance.Common != null
                    ? Player_Manager.Instance.Common.MenuManager.GameSaveManager.SettingsData.ShadowMapResolution
                    : GameSettingsData.SHADOW_MAP_RESOLUTION_DEFAULT;
                if (a < SHADOW_MAP_RESOLUTION_VALUES.Length)
                    currentResolution = SHADOW_MAP_RESOLUTION_VALUES[a];
                else
                {
                    Debug.LogWarning("Outside range setting to default");
                    currentResolution =
                        SHADOW_MAP_RESOLUTION_VALUES[GameSettingsData.SHADOW_MAP_RESOLUTION_DEFAULT];
                }
            }
            else // default if no file found
                currentResolution =
                    SHADOW_MAP_RESOLUTION_VALUES[GameSettingsData.SHADOW_MAP_RESOLUTION_DEFAULT];

            // extra high resolution when taking screenshots (unstable)
            if (USE_4K_FOR_SCREENSHOTS)
                currentResolution = SCREENSHOT_4K;
            else if (USE_8K_FOR_SCREENSHOTS)
                currentResolution = SCREENSHOT_8K;

            base.UpdateShadowMapResolution(currentResolution);
        }

        private HxVolumetricLight _hx;

        protected override void Setup()
        {
            base.Setup();

            _hx = GetComponent<HxVolumetricLight>();
        }

        protected override void OnMove()
        {
            base.OnMove();

            if (_hx) // && !LightComponent.enabled)
                _hx.UpdatePosition(false, true);
        }
    }
}
#endif
