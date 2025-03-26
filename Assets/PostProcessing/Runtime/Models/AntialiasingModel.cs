using System;

namespace UnityEngine.PostProcessing {
    [Serializable]
    public class AntialiasingModel : PostProcessingModel {
        // Most settings aren't exposed to the user anymore, presets are enough. Still, I'm leaving
        // the tooltip attributes in case an user wants to customize each preset.

        #region FXAA Settings
        public enum FxaaPreset
        {
            ExtremePerformance,
            Performance,
            Default,
            Quality,
            ExtremeQuality
        }

        [Serializable]
        public struct FxaaQualitySettings {
            [Tooltip("The amount of desired sub-pixel aliasing removal. Effects the sharpeness of the output.")]
            [Range(0f, 1f)]
            public float subpixelAliasingRemovalAmount;

            [Tooltip("The minimum amount of local contrast required to qualify a region as containing an edge.")]
            [Range(0.063f, 0.333f)]
            public float edgeDetectionThreshold;

            [Tooltip("Local contrast adaptation value to disallow the algorithm from executing on the darker regions.")]
            [Range(0f, 0.0833f)]
            public float minimumRequiredLuminance;

            public static FxaaQualitySettings[] presets =
            {
                // ExtremePerformance
                new FxaaQualitySettings
                {
                    subpixelAliasingRemovalAmount = 0f,
                    edgeDetectionThreshold = 0.333f,
                    minimumRequiredLuminance = 0.0833f
                },

                // Performance
                new FxaaQualitySettings
                {
                    subpixelAliasingRemovalAmount = 0.25f,
                    edgeDetectionThreshold = 0.25f,
                    minimumRequiredLuminance = 0.0833f
                },

                // Default
                new FxaaQualitySettings
                {
                    subpixelAliasingRemovalAmount = 0.75f,
                    edgeDetectionThreshold = 0.166f,
                    minimumRequiredLuminance = 0.0833f
                },

                // Quality
                new FxaaQualitySettings
                {
                    subpixelAliasingRemovalAmount = 1f,
                    edgeDetectionThreshold = 0.125f,
                    minimumRequiredLuminance = 0.0625f
                },

                // ExtremeQuality
                new FxaaQualitySettings
                {
                    subpixelAliasingRemovalAmount = 1f,
                    edgeDetectionThreshold = 0.063f,
                    minimumRequiredLuminance = 0.0312f
                }
            };
        }

        [Serializable]
        public struct FxaaConsoleSettings
        {
            [Tooltip("The amount of spread applied to the sampling coordinates while sampling for subpixel information.")]
            [Range(0.33f, 0.5f)]
            public float subpixelSpreadAmount;

            [Tooltip("This value dictates how sharp the edges in the image are kept; a higher value implies sharper edges.")]
            [Range(2f, 8f)]
            public float edgeSharpnessAmount;

            [Tooltip("The minimum amount of local contrast required to qualify a region as containing an edge.")]
            [Range(0.125f, 0.25f)]
            public float edgeDetectionThreshold;

            [Tooltip("Local contrast adaptation value to disallow the algorithm from executing on the darker regions.")]
            [Range(0.04f, 0.06f)]
            public float minimumRequiredLuminance;

            public static FxaaConsoleSettings[] presets =
            {
                // ExtremePerformance
                new FxaaConsoleSettings
                {
                    subpixelSpreadAmount = 0.33f,
                    edgeSharpnessAmount = 8f,
                    edgeDetectionThreshold = 0.25f,
                    minimumRequiredLuminance = 0.06f
                },

                // Performance
                new FxaaConsoleSettings
                {
                    subpixelSpreadAmount = 0.33f,
                    edgeSharpnessAmount = 8f,
                    edgeDetectionThreshold = 0.125f,
                    minimumRequiredLuminance = 0.06f
                },

                // Default
                new FxaaConsoleSettings
                {
                    subpixelSpreadAmount = 0.5f,
                    edgeSharpnessAmount = 8f,
                    edgeDetectionThreshold = 0.125f,
                    minimumRequiredLuminance = 0.05f
                },

                // Quality
                new FxaaConsoleSettings
                {
                    subpixelSpreadAmount = 0.5f,
                    edgeSharpnessAmount = 4f,
                    edgeDetectionThreshold = 0.125f,
                    minimumRequiredLuminance = 0.04f
                },

                // ExtremeQuality
                new FxaaConsoleSettings
                {
                    subpixelSpreadAmount = 0.5f,
                    edgeSharpnessAmount = 2f,
                    edgeDetectionThreshold = 0.125f,
                    minimumRequiredLuminance = 0.04f
                }
            };
        }

        [Serializable]
        public struct FxaaSettings
        {
            public FxaaPreset preset;

            public static FxaaSettings defaultSettings
            {
                get
                {
                    return new FxaaSettings
                    {
                        preset = FxaaPreset.Default
                    };
                }
            }
        }
        #endregion

        [Serializable]
        public struct Settings {
            public FxaaSettings fxaaSettings;
            public static Settings defaultSettings {
                get {
                    return new Settings {
                        fxaaSettings = FxaaSettings.defaultSettings
                    };
                }
            }
        }

        [SerializeField]
        Settings m_Settings = Settings.defaultSettings;
        public Settings settings {
            get { return m_Settings; }
            set { m_Settings = value; }
        }

        public override void Reset()
        {
            m_Settings = Settings.defaultSettings;
        }
    }
}
