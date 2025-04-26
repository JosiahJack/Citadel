using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing {
    [RequireComponent(typeof(Camera)), DisallowMultipleComponent]
    [AddComponentMenu("Effects/Post-Processing Behaviour", -1)]
    public class PostProcessingBehaviour : MonoBehaviour {
        // Inspector fields
        public PostProcessingProfile profile;

        // Internal helpers
        Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>> m_CommandBuffers;
        List<PostProcessingComponentBase> m_Components;
        Dictionary<PostProcessingComponentBase, bool> m_ComponentStates;

        MaterialFactory m_MaterialFactory;
        RenderTextureFactory m_RenderTextureFactory;
        PostProcessingContext m_Context;
        Camera m_Camera;
        PostProcessingProfile m_PreviousProfile;

        // Effect components
        ScreenSpaceReflectionComponent m_ScreenSpaceReflection;
        BloomComponent m_Bloom;
        ChromaticAberrationComponent m_ChromaticAberration;
        ColorGradingComponent m_ColorGrading;
        FxaaComponent m_Fxaa;

        void OnEnable() {
            m_CommandBuffers = new Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>>();
            m_MaterialFactory = new MaterialFactory();
            m_RenderTextureFactory = new RenderTextureFactory();
            m_Context = new PostProcessingContext();

            // Keep a list of all post-fx for automation purposes
            m_Components = new List<PostProcessingComponentBase>();

            // Component list
            m_ScreenSpaceReflection = AddComponent(new ScreenSpaceReflectionComponent());
            m_Bloom = AddComponent(new BloomComponent());
            m_ChromaticAberration = AddComponent(new ChromaticAberrationComponent());
            m_ColorGrading = AddComponent(new ColorGradingComponent());
            m_Fxaa = AddComponent(new FxaaComponent());

            // Prepare state observers
            m_ComponentStates = new Dictionary<PostProcessingComponentBase, bool>();

            foreach (var component in m_Components)
                m_ComponentStates.Add(component, false);

            useGUILayout = false;
        }

        void OnPreCull() {
            m_Camera = GetComponent<Camera>();

            if (profile == null || m_Camera == null) return;

            // Prepare context
            var context = m_Context.Reset();
            context.profile = profile;
            context.renderTextureFactory = m_RenderTextureFactory;
            context.materialFactory = m_MaterialFactory;
            context.camera = m_Camera;

            // Prepare components
            m_ScreenSpaceReflection.Init(context, profile.screenSpaceReflection);
            m_Bloom.Init(context, profile.bloom);
            m_ChromaticAberration.Init(context, profile.chromaticAberration);
            m_ColorGrading.Init(context, profile.colorGrading);
            m_Fxaa.Init(context, profile.antialiasing);

            // Handles profile change and 'enable' state observers
            if (m_PreviousProfile != profile) {
                DisableComponents();
                m_PreviousProfile = profile;
            }

            CheckObservers();

            // Find out which camera flags are needed before rendering begins
            var flags = context.camera.depthTextureMode;
            foreach (var component in m_Components) {
                if (component.active) flags |= component.GetCameraFlags();
            }

            context.camera.depthTextureMode = flags;
        }

        void OnPreRender() {
            if (profile == null) return;

            // Command buffer-based effects should be set-up here
            TryExecuteCommandBuffer(m_ScreenSpaceReflection);
        }

        void OnPostRender() {
            // No action needed for the remaining effects
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (profile == null || m_Camera == null) {
                Graphics.Blit(source, destination);
                return;
            }

            // Uber shader setup
            bool uberActive = false;
            bool fxaaActive = m_Fxaa.active;

            var uberMaterial = m_MaterialFactory.Get("Hidden/Post FX/Uber Shader");
            uberMaterial.shaderKeywords = null;

            var src = source;
            var dst = destination;

            if (m_Bloom.active) {
                uberActive = true;
                m_Bloom.Prepare(src, uberMaterial, GraphicsUtils.whiteTexture);
            }

            uberActive |= TryPrepareUberImageEffect(m_ChromaticAberration, uberMaterial);
            uberActive |= TryPrepareUberImageEffect(m_ColorGrading, uberMaterial);

            var fxaaMaterial = fxaaActive ? m_MaterialFactory.Get("Hidden/Post FX/FXAA") : null;

            if (fxaaActive) {
                fxaaMaterial.shaderKeywords = null;
                if (uberActive) {
                    var output = m_RenderTextureFactory.Get(src);
                    Graphics.Blit(src, output, uberMaterial, 0);
                    src = output;
                }

                m_Fxaa.Render(src, dst);
            } else {
                if (uberActive) {
                    if (!GraphicsUtils.isLinearColorSpace) {
                        uberMaterial.EnableKeyword("UNITY_COLORSPACE_GAMMA");
                    }

                    Graphics.Blit(src, dst, uberMaterial, 0);
                } else {
                    Graphics.Blit(src, dst);
                }
            }

            m_RenderTextureFactory.ReleaseAll();
        }

        void OnDisable() {
            // Clear command buffers
            foreach (var cb in m_CommandBuffers.Values) {
                m_Camera.RemoveCommandBuffer(cb.Key, cb.Value);
                cb.Value.Dispose();
            }

            m_CommandBuffers.Clear();

            // Clear components
            if (profile != null) DisableComponents();
            m_Components.Clear();

            // Factories
            m_MaterialFactory.Dispose();
            m_RenderTextureFactory.Dispose();
            GraphicsUtils.Dispose();
        }

        #region State management

        List<PostProcessingComponentBase> m_ComponentsToEnable = new List<PostProcessingComponentBase>();
        List<PostProcessingComponentBase> m_ComponentsToDisable = new List<PostProcessingComponentBase>();

        void CheckObservers() {
            foreach (var cs in m_ComponentStates) {
                var component = cs.Key;
                var state = component.GetModel().enabled;
                if (state != cs.Value) {
                    if (state) m_ComponentsToEnable.Add(component);
                    else m_ComponentsToDisable.Add(component);
                }
            }

            for (int i = 0; i < m_ComponentsToDisable.Count; i++) {
                var c = m_ComponentsToDisable[i];
                m_ComponentStates[c] = false;
                c.OnDisable();
            }

            for (int i = 0; i < m_ComponentsToEnable.Count; i++) {
                var c = m_ComponentsToEnable[i];
                m_ComponentStates[c] = true;
                c.OnEnable();
            }

            m_ComponentsToDisable.Clear();
            m_ComponentsToEnable.Clear();
        }

        void DisableComponents() {
            foreach (var component in m_Components) {
                var model = component.GetModel();
                if (model != null && model.enabled) component.OnDisable();
            }
        }
        #endregion

        #region Command buffer handling & rendering helpers
        CommandBuffer AddCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel {
            var cb = new CommandBuffer { name = name };
            var kvp = new KeyValuePair<CameraEvent, CommandBuffer>(evt, cb);
            m_CommandBuffers.Add(typeof(T), kvp);
            m_Camera.AddCommandBuffer(evt, kvp.Value);
            return kvp.Value;
        }

        void RemoveCommandBuffer<T>() where T : PostProcessingModel {
            KeyValuePair<CameraEvent, CommandBuffer> kvp;
            var type = typeof(T);
            if (!m_CommandBuffers.TryGetValue(type, out kvp)) return;

            m_Camera.RemoveCommandBuffer(kvp.Key, kvp.Value);
            m_CommandBuffers.Remove(type);
            kvp.Value.Dispose();
        }

        CommandBuffer GetCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel {
            CommandBuffer cb;
            KeyValuePair<CameraEvent, CommandBuffer> kvp;
            if (!m_CommandBuffers.TryGetValue(typeof(T), out kvp)) {
                cb = AddCommandBuffer<T>(evt, name);
            } else if (kvp.Key != evt) {
                RemoveCommandBuffer<T>();
                cb = AddCommandBuffer<T>(evt, name);
            } else cb = kvp.Value;

            return cb;
        }

        void TryExecuteCommandBuffer<T>(PostProcessingComponentCommandBuffer<T> component) where T : PostProcessingModel {
            if (component.active) {
                var cb = GetCommandBuffer<T>(component.GetCameraEvent(), component.GetName());
                cb.Clear();
                component.PopulateCommandBuffer(cb);
            } else RemoveCommandBuffer<T>();
        }

        bool TryPrepareUberImageEffect<T>(PostProcessingComponentRenderTexture<T> component, Material material) where T : PostProcessingModel {
            if (!component.active) return false;

            component.Prepare(material);
            return true;
        }

        T AddComponent<T>(T component) where T : PostProcessingComponentBase {
            m_Components.Add(component);
            return component;
        }
        #endregion
    }
}
