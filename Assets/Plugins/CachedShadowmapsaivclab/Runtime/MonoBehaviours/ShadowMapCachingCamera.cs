#define ON_FIXED_UPDATE
//#define DELAYED_RESTART

using System.Collections;
using UnityEngine;

namespace CSM.Runtime.MonoBehaviours
{
    /// <inheritdoc cref="MonoBehaviour" />
    ///  <summary>
    /// This camera ensures that everything inside the area of affection by a light source is not culled away, and calls render to compute the shadow map in the shadow caching command buffer on the light source
    ///  </summary>
    [RequireComponent(requiredComponent : typeof(Camera))]
    [DisallowMultipleComponent]
    public class ShadowMapCachingCamera : MonoBehaviour
    {
        Camera _camera;
#if CACHED_SHADOW_MAP_DEBUG
            private bool Debugging = true;
#endif
#if DELAYED_RESTART
        [SerializeField] [Range(0, 999)] private int cachingDelay = 5*60; // Approximately 5 sec on ~60 fps
        [SerializeField] [ReadOnly] private int iDelay;
#endif

RenderTexture _render_target;

void Awake()
        {
            this._camera = this.GetComponent<Camera>();
            if (!this._camera) {
                this._camera = this.gameObject.AddComponent<Camera>();
            }

            this._camera.enabled = false;
            this._camera.depth = 0;
            this._render_target = RenderTexture.GetTemporary(1, 1);
            this._camera.targetTexture = this._render_target;

            this.AllowCaching();
        }


        [field: SerializeField] bool CachingAllowed { get; set; } = false;

        internal void DisallowCaching()
        {
            if (this._reallow_coroutine != null) {
                this.StopCoroutine(routine : this._reallow_coroutine);
            }

            this._reallow_coroutine = null;

            this.CachingAllowed = false;
        }

        IEnumerator _reallow_coroutine;


        IEnumerator PostPonedReactivation()
        {
            yield return new WaitForEndOfFrame();

#if DELAYED_RESTART
            iDelay = 0;
            while (iDelay < cachingDelay)
            {
                iDelay++;
                yield return new WaitForEndOfFrame();
            }
#endif

#if CACHED_SHADOW_MAP_DEBUG
            if (Debugging)
{
  Debug.Log("ShadowMapCachingCamera is now allowed to cache shadowmaps again");
}
#endif

            this.CachingAllowed = true;
            this._reallow_coroutine = null;
        }

        internal void AllowCaching()
        {
            if (this._reallow_coroutine != null || this.CachingAllowed) {
                return;
            }

            this._reallow_coroutine = this.PostPonedReactivation();

            this.StartCoroutine(routine : this._reallow_coroutine);
        }


#if ON_FIXED_UPDATE
        void FixedUpdate()
#else
        void LateUpdate()
#endif
        {
            if (this.CachingAllowed) {
                ShadowMapCacheSystem.Instance.CacheNext();
            }
        }

        public void CacheShadowMapOfLight(CachedShadowMapLight a_light, float margin = 1f)
        {
            this._camera.renderingPath = RenderingPath.DeferredShading;
            // Set rendering path to deferred to avoid issues with number of pixel lights.
            this._camera.useOcclusionCulling = false;
            this._camera.aspect = 1.0f;

            var culling_camera_transform = this._camera.transform;
            switch (a_light.LightComponent.type)
            {
                case LightType.Spot:

                    culling_camera_transform.position = a_light.CachedPosition;
                    culling_camera_transform.rotation = a_light.CachedRotation;

                    this._camera.fieldOfView = a_light.LightComponent.spotAngle + margin;
                    this._camera.nearClipPlane = Mathf.Max(a : a_light.LightComponent.shadowNearPlane - margin, 0.01f);
                    this._camera.farClipPlane = a_light.LightComponent.range + margin;

                    break;
                case LightType.Point:

                    // Cannot use orthographic camera here as deferred rendering path is not supported for it.

                    //m_cullingCamera.transform.position = light.transform.position - Vector3.forward * (light.range + 0.1f);
                    //m_cullingCamera.transform.rotation = Quaternion.identity;
                    //m_cullingCamera.orthographic = true;
                    //m_cullingCamera.orthographicSize = light.range;
                    //m_cullingCamera.aspect = 1.0f;
                    //m_cullingCamera.farClipPlane = 2.0f * light.range + 0.2f;
                    //m_cullingCamera.nearClipPlane = 0.1f;

                    // Instead, fit the frustum to the sphere of the light.
                    // We choose a fixed fov and then use c = a / sin A of a right triangle, where A is half the fov and a is the light's range.
                    // The assumption of a (far,narrow field of view) is that it should result in fewer collision view unrelated geometry than a (near,wide).
                    const float field_of_view_to_use = 35.0f;
                    var range = a_light.LightComponent.range;
                    var distance = range / Mathf.Sin(f : 0.5f * field_of_view_to_use * Mathf.Deg2Rad);

                    culling_camera_transform.position = a_light.CachedPosition - Vector3.forward * distance;
                    culling_camera_transform.rotation = Quaternion.identity;

                    this._camera.fieldOfView = field_of_view_to_use + margin;
                    this._camera.nearClipPlane = Mathf.Max(a : distance - range - margin, 0.01f);
                    this._camera.farClipPlane = distance + range + margin;

                    break;
                case LightType.Directional:
                case LightType.Area:
                case LightType.Disc:
                    Debug.Log("NotSupported");
                    break;
                default:
                    Debug.Log("NotSupported");
                    break;
            }


            // Store current shadow distance.
            var current_shadow_distance = QualitySettings.shadowDistance;

            // Set shadow distance to light range to prevent culling.
            QualitySettings.shadowDistance = this._camera.farClipPlane;

            // Render scene from light.
            this._camera.Render();

            // Restore shadow distance.
            QualitySettings.shadowDistance = current_shadow_distance;
        }
    }
}