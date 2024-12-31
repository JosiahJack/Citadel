//#define HX

//#define PERIODIC_RECACHING
//#define REMOVE_COMMANDBUFFERS_WHILE_NOT_ACTIVE
//#define PRERENDER_CONTEXT
//#define PRECLEAR_SHADOW_MAP
//#define SHADOW_MAP_SANITY_CHECK
#define SHADOW_MAP_UPDATE_CHECK

using System;
using System.Collections.Generic;
using System.Linq;
using CSM.Runtime.MonoBehaviours;
using CSM.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace CSM.Runtime {
  /// <summary>
  /// Singleton system for maintaining handles to mainCamera, cullingCamera, commandBuffers and RenderTextures
  /// </summary>
  [Serializable]
  public class ShadowMapCacheSystem {
    static ShadowMapCacheSystem _instance;
    static bool _disposed = false;

    internal readonly Dictionary<Light, CommandBuffer> _CaptureCmdBuffers =
        new Dictionary<Light, CommandBuffer>();

    internal readonly Dictionary<Light, CommandBuffer> _BlitCmdBuffers =
        new Dictionary<Light, CommandBuffer>();

    internal readonly Dictionary<Light, Bounds> _LightBounds = new Dictionary<Light, Bounds>();

    internal readonly Dictionary<Light, BoundingSphere> _LightBoundingSpheres =
        new Dictionary<Light, BoundingSphere>();

    internal readonly Dictionary<Light, RenderTexture> _CachedShadowMapTextures =
        new Dictionary<Light, RenderTexture>();

    internal readonly Dictionary<Light, RenderTexture> _ActivelyCachedTextures =
        new Dictionary<Light, RenderTexture>();

    internal readonly HashSet<Light> _ActivelyCachedLights = new HashSet<Light>();

    internal readonly HashSet<CachedShadowMapLight> _AllCachedLightComponents =
        new HashSet<CachedShadowMapLight>();

    internal readonly List<CachedShadowMapLight> _ActivelyCachedShadowMaps = new List<CachedShadowMapLight>();

    Queue<CachedShadowMapLight> DirtyCachedLights { get; } = new Queue<CachedShadowMapLight>();

    int _blit_id = 0;
    int _capture_id = 0;

    CameraEvent _blit_event = CameraEvent.AfterLighting;
    LightEvent _capture_event = LightEvent.AfterShadowMap;
    [SerializeField] CachedShadowMapReceiverCamera _ReceiverCamera;

    /// <summary>
    /// The culling camera of the system, responsible for ensuring that lights and meshRenderers will not be culled when caching
    /// </summary>
    [SerializeField]
    ShadowMapCachingCamera cachingCamera;

    #if UNITY_EDITOR && CACHED_SHADOW_MAP_DEBUG
    // ReSharper disable once NotAccessedField.Local
    [SerializeField] Light[] activeCachedLightsList;

    // ReSharper disable once NotAccessedField.Local
    [SerializeField] RenderTexture[] activeCachedShadowMaps;

    // ReSharper disable once NotAccessedField.Local
    [SerializeField] int[] boundingSphereInd;
    #endif

    #if UNITY_EDITOR && CACHED_SHADOW_MAP_DEBUG
    void UpdateEditorDebugInfo() {
        //this.activeCachedLightsList = this.ActivelyCachedLights.ToArray();
      //this.activeCachedShadowMaps = this.ActivelyCachedTextures.Values.ToArray();
      //this.boundingSphereInd = this.BoundingSphereIndices.Values.ToArray();
    }
    #endif

    #if PERIODIC_RECACHING
    int _standby_counter = 0;
    int _standby_num = 20; // (1/60) * 20 = ~ every 0.33 seconds
    int _recaching_pointer = 0;
    #endif

    /// <summary>
    /// Singleton Instance
    /// </summary>
    public static ShadowMapCacheSystem Instance {
      get {
        if (_instance != null) {
          return _instance;
        }

        #if HX
                _instance = new ShadowMapCacheSystemHx();
                if (!_disposed)
                {
                    _instance.CreateCullingCamera();
                }

                if (_instance != null && _instance.cachingCamera != null)
                    StaticHelperClass.SetDontDestroyOnLoad(_instance.cachingCamera.gameObject);
        #else
        _instance = new ShadowMapCacheSystem();
        if (!_disposed) {
          _instance.CreateCullingCamera();
        }
        #endif
        SceneManager.sceneLoaded += _instance.NewSceneLoaded;

        return _instance;
      }
    }

    void NewSceneLoaded(Scene arg0, LoadSceneMode arg1) {
      #if CACHED_SHADOW_MAP_DEBUG
                Debug.LogWarning("A new scene was loaded!");
      #endif
      this.MakeAllLightsDynamic();
    }

    /// <summary>
    /// The main camera of the system, responsible for displaying the cachedShadowMaps
    /// </summary>

    CachedShadowMapReceiverCamera ReceiverCamera {
      get {
        if (this._ReceiverCamera != null) {
          return this._ReceiverCamera;
        }

        Camera main_cam = null;

        //if (!UnityEngine.Application.IsPlaying()) // Check is quitting
        main_cam = Camera.main;
        //main_cam = GameObject.FindObjectOfType<Camera>();

        var a = main_cam?.GetComponent<CachedShadowMapReceiverCamera>();

        if (!a) {
          this._ReceiverCamera = main_cam?.gameObject.AddComponent<CachedShadowMapReceiverCamera>();
        } else {
          this._ReceiverCamera = a;
        }

        return this._ReceiverCamera;
      }
    }

    enum AntiAliasingEnum {
      None_ = 1,
      Two_ = 2,
      Four_ = 4,
      Eight_ = 8,
    }

    void CreateCullingCamera() {
      if (this.cachingCamera == null) {
        var camera_object = new GameObject("[ShadowCachingCullingCamera]");
        this.cachingCamera = camera_object.AddComponent<ShadowMapCachingCamera>();
      }
    }

    /// <summary>
    /// Adds a cachedShadowMapLight to the system, sets up a renderTexture to be rendered into by the light
    /// </summary>
    /// <param name="a_light"></param>
    public void AddLightToSystem(CachedShadowMapLight a_light) {
      this._AllCachedLightComponents.Add(item : a_light);
      this.EnsureCachedTextureExist(a_light : a_light);
    }

    /// <summary>
    /// Removes a cachedShadowMapLight from the system, removes, disposes, releases CommandBuffers and RenderTextures for the light
    /// </summary>
    /// <param name="a_light"></param>
    public void RemoveLightFromSystem(CachedShadowMapLight a_light) {
      #if !REMOVE_COMMANDBUFFERS_WHILE_NOT_ACTIVE
      if (this._CaptureCmdBuffers.ContainsKey(key : a_light.LightComponent)) {
        a_light.LightComponent.RemoveCommandBuffer(evt : this._capture_event,
                                                   buffer : this._CaptureCmdBuffers[key : a_light
                                                                                        .LightComponent]);
      }

      if (this._BlitCmdBuffers.ContainsKey(key : a_light.LightComponent)) {
        this.ReceiverCamera?._camera.RemoveCommandBuffer(evt : this._blit_event,
                                                         buffer : this._BlitCmdBuffers[key : a_light
                                                                                           .LightComponent]);
      }
      #endif

      this.RevertToDynamicLighting(a_light : a_light);

      this.RemoveBounds(a_light : a_light);

      this._AllCachedLightComponents.Remove(item : a_light);

      // Remove and dispose command buffers.
      if (this._CaptureCmdBuffers.ContainsKey(key : a_light.LightComponent)) {
        var shadow_map_cache_buffer = this._CaptureCmdBuffers[key : a_light.LightComponent];
        this._CaptureCmdBuffers.Remove(key : a_light.LightComponent);
        shadow_map_cache_buffer.Dispose();
      }

      if (this._BlitCmdBuffers.ContainsKey(key : a_light.LightComponent)) {
        var lighting_buffer = this._BlitCmdBuffers[key : a_light.LightComponent];
        this._BlitCmdBuffers.Remove(key : a_light.LightComponent);
        lighting_buffer.Dispose();
      }

      // Remove and release render texture.
      if (this._CachedShadowMapTextures.ContainsKey(key : a_light.LightComponent)) {
        this._CachedShadowMapTextures[key : a_light.LightComponent].Release();
        RenderTexture.DestroyImmediate(obj : this._CachedShadowMapTextures[key : a_light.LightComponent]);
        this._CachedShadowMapTextures.Remove(key : a_light.LightComponent);
      }
    }

    /// <summary>
    ///
    /// </summary>
    public void UpdateResolutionAll() {
      if (this._AllCachedLightComponents == null) {
        return;
      }

      var s = this._AllCachedLightComponents.ToArray();
      for (var i = 0; i < s.Length; i++) {
        s[i].UpdateShadowMapResolution();
      }

      this.MakeAllLightsDynamic();
    }

    /// <summary>
    /// Cache the next dirty enqueued light if any
    /// </summary>
    public void CacheNext() {
      if (this.DirtyCachedLights.Count > 0) {
        this.ComputeCachedShadowMap(a_light : this.DirtyCachedLights.Dequeue());
      }
      #if PERIODIC_RECACHING
      else {
        if (this._ActivelyCachedShadowMaps.Count > 0 && this._standby_counter++ > this._standby_num) {
          this._standby_counter = 0;
          var a =
              this._ActivelyCachedShadowMaps
                  [index : ArrayUtilities.PositiveMod(x : unchecked(this._recaching_pointer++),
                                                      m : this._ActivelyCachedShadowMaps.Count)];
          this.RevertToDynamicLighting(a_light : a);
          this.ComputeCachedShadowMap(a_light : a);
        }
      }
      #endif
    }

    private protected virtual void ComputeCachedShadowMap(CachedShadowMapLight a_light, float margin = 2f) {
      if (!a_light || !a_light.isActiveAndEnabled || !a_light.LightComponent) {
        #if CACHED_SHADOW_MAP_DEBUG
                    Debug.LogWarning($"light was null!!!", aLight);
        #endif
        this.RevertToDynamicLighting(a_light : a_light);
        return;
      }

      //Debug.Log("Updating CSM " + aLight.name, aLight);
      this.EnsureCaptureCmdBufferExist(a_light : a_light);
      this.EnsureCachedTextureExist(a_light : a_light);

      var capture_cmd_buffer = this._CaptureCmdBuffers[key : a_light.LightComponent];

      a_light.LightComponent.enabled =
          true; // Make sure the light source is on while we render using the caching camera

      var cull_d = a_light.LightComponent.layerShadowCullDistances;
      a_light.LightComponent.layerShadowCullDistances = null;

      var lrm = a_light.LightComponent.renderMode;
      a_light.LightComponent.renderMode = LightRenderMode.ForcePixel;

      var lscm = a_light.LightComponent.lightShadowCasterMode;
      a_light.LightComponent.lightShadowCasterMode = LightShadowCasterMode.Everything;

      #if PRERENDER_CONTEXT
            #if PRECLEAR_SHADOW_MAP
                captureCmdBuffer.SetRenderTarget(this.CachedShadowMapTextures[aLight.LightComponent]);
                captureCmdBuffer.ClearRenderTarget(true,false,Color.black);
            #endif

            this.camera.CacheShadowMapOfLight(aLight, margin);
                        captureCmdBuffer.Clear();
      #endif
      #if SHADOW_MAP_UPDATE_CHECK
      var last_update_count = this._CachedShadowMapTextures[key : a_light.LightComponent].updateCount;
      capture_cmd_buffer.IncrementUpdateCount(dest : this._CachedShadowMapTextures
                                                  [key : a_light.LightComponent]);
      #endif

      capture_cmd_buffer.CopyTexture(src : BuiltinRenderTextureType.CurrentActive,
                                     dst : this._CachedShadowMapTextures[key : a_light.LightComponent]);

      this.cachingCamera.CacheShadowMapOfLight(a_light : a_light,
                                               margin :
                                               margin); // Reset camera and set light specific parameters.

      capture_cmd_buffer.Clear();
      a_light.LightComponent.renderMode = lrm;
      a_light.LightComponent.layerShadowCullDistances = cull_d;
      a_light.LightComponent.lightShadowCasterMode = lscm;

      #if SHADOW_MAP_UPDATE_CHECK
      if (last_update_count == this._CachedShadowMapTextures[key : a_light.LightComponent].updateCount) {
        Debug.LogError("CachedShadowMap was not updated!");
        this.RevertToDynamicLighting(a_light : a_light);
        this.EnqueueDirtyCandidate(a_light : a_light);
        return;
      }
      #endif

      #if SHADOW_MAP_SANITY_CHECK
      if (ShadowMapChecking.SanityCheck(CachedShadowMapTextures[aLight.LightComponent]) &&
          CachedShadowMapTextures[aLight.LightComponent].IsCreated())
      {
          RevertToDynamicLighting(aLight);
        EnqueueDirtyCandidate(aLight);
        return;
      }
      #endif

      this.AddBounds(a_light : a_light);

      this.EnsureBlitCmdBufferExist(a_light : a_light);

      if (!this._ActivelyCachedLights.Contains(item : a_light.LightComponent)) {
        this._ActivelyCachedLights.Add(item : a_light.LightComponent);
      }

      if (!this._ActivelyCachedShadowMaps.Contains(item : a_light)) {
        this._ActivelyCachedShadowMaps.Add(item : a_light);
      }

      if (!this._ActivelyCachedTextures.ContainsKey(key : a_light.LightComponent)) {
        this._ActivelyCachedTextures.Add(key : a_light.LightComponent,
                                         value : this._CachedShadowMapTextures[key : a_light.LightComponent]);
      }

      a_light.LightComponent.enabled = false;

      #if UNITY_EDITOR && CACHED_SHADOW_MAP_DEBUG
      this.UpdateEditorDebugInfo();
      #endif
    }

    void AddBounds(CachedShadowMapLight a_light) {
      var bounding_sphere = BoundsUtility.ComputeLightBoundingSphere(a_light : a_light);
      if (this._LightBoundingSpheres.ContainsKey(key : a_light.LightComponent)) {
        this._LightBoundingSpheres[key : a_light.LightComponent] = bounding_sphere;
      } else {
        this._LightBoundingSpheres.Add(key : a_light.LightComponent, value : bounding_sphere);
      }

      var light_bounds = BoundsUtility.ComputeLightBounds(light : a_light);
      if (this._LightBounds.ContainsKey(key : a_light.LightComponent)) {
        this._LightBounds[key : a_light.LightComponent] = light_bounds;
      } else {
        this._LightBounds.Add(key : a_light.LightComponent, value : light_bounds);
      }

      this.ReceiverCamera?.RefreshCullingGroup();
    }

    void RemoveBounds(CachedShadowMapLight a_light) {
      if (this._LightBoundingSpheres.ContainsKey(key : a_light.LightComponent)) {
        this._LightBoundingSpheres.Remove(key : a_light.LightComponent);
      }

      if (this._LightBounds.ContainsKey(key : a_light.LightComponent)) {
        this._LightBounds.Remove(key : a_light.LightComponent);
      }

      this.ReceiverCamera?.RefreshCullingGroup();
    }

    /// <summary>
    /// Enqueue a CachedShadowMapLight to be cached
    /// </summary>
    /// <param name="a_light"></param>
    public void EnqueueDirtyCandidate(CachedShadowMapLight a_light) {
      if (this.DirtyCachedLights.Contains(item : a_light)) {
        return;
      }

      if (a_light && a_light.LightComponent) {
        #if CACHED_SHADOW_MAP_DEBUG
                if (!aLight.isActiveAndEnabled)
                {
                    Debug.LogError("Light was not isActiveAndEnabled", aLight);
                }

                if (this.DirtyCachedLights.Contains(aLight))
                {
                    Debug.LogError("Light was already enqueued", aLight);
                }
        #endif

        this.DirtyCachedLights.Enqueue(item : a_light);
      }
    }

    void EnsureBlitCmdBufferExist(CachedShadowMapLight a_light) {
      if (!this._BlitCmdBuffers.ContainsKey(key : a_light.LightComponent)) {
        var lighting_buffer = new CommandBuffer {
                                                    name =
                                                        $"Cached Shadow Map #{unchecked(this._blit_id++)} #CID{a_light.LightComponent.GetInstanceID()}"
                                                };
        //unchecked wraps integer instead of throwing exception

        this._BlitCmdBuffers.Add(key : a_light.LightComponent, value : lighting_buffer);
      }

      this.ReceiverCamera?._camera.RemoveCommandBuffer(evt : this._blit_event,
                                                       buffer : this._BlitCmdBuffers[key : a_light
                                                                                         .LightComponent]);
      this.ReceiverCamera?._camera.AddCommandBuffer(evt : this._blit_event,
                                                    buffer : this._BlitCmdBuffers
                                                        [key : a_light.LightComponent]);
    }

    bool TextureTypeFits(CachedShadowMapLight a_light) {
      if (a_light.LightComponent.type == LightType.Point) {
        return this._CachedShadowMapTextures[key : a_light.LightComponent].dimension == TextureDimension.Cube;
      }

      return this._CachedShadowMapTextures[key : a_light.LightComponent].dimension == TextureDimension.Tex2D;
    }

    void EnsureCachedTextureExist(CachedShadowMapLight a_light) {
      if (this._CachedShadowMapTextures.ContainsKey(key : a_light.LightComponent)) {
        if (this._CachedShadowMapTextures[key : a_light.LightComponent].width
            == a_light.LightComponent.shadowCustomResolution
            && this._CachedShadowMapTextures[key : a_light.LightComponent].height
            == a_light.LightComponent.shadowCustomResolution
            && this.TextureTypeFits(a_light : a_light)
            && this._CachedShadowMapTextures[key : a_light.LightComponent].depth
            == (int)CachedShadowMapLight.MShadowMapBitDepth
            && this._CachedShadowMapTextures[key : a_light.LightComponent].IsCreated()) {
          return;
        }

        this._CachedShadowMapTextures[key : a_light.LightComponent].DiscardContents();
        this._CachedShadowMapTextures[key : a_light.LightComponent].Release();
        RenderTexture.DestroyImmediate(obj : this._CachedShadowMapTextures[key : a_light.LightComponent]);
        this._CachedShadowMapTextures.Remove(key : a_light.LightComponent);

        //this.CachedShadowMapTextures[aLight.LightComponent].MarkRestoreExpected();
      }

      ;

      var shadow_custom_resolution = a_light.LightComponent.shadowCustomResolution;
      var cached_shadow_map =
          new RenderTexture(width : shadow_custom_resolution,
                            height : shadow_custom_resolution,
                            depth : (int)CachedShadowMapLight.MShadowMapBitDepth,
                            format : RenderTextureFormat.Shadowmap) {
                                                                        antiAliasing =
                                                                            (int)AntiAliasingEnum.None_,
                                                                        filterMode =
                                                                            CachedShadowMapLight._MFilterMode,
                                                                        useMipMap = false,
                                                                        autoGenerateMips = false,
                                                                        wrapMode = TextureWrapMode.Clamp,
                                                                        memorylessMode =
                                                                            RenderTextureMemoryless.None,
                                                                        vrUsage = VRTextureUsage.None,
                                                                        useDynamicScale = false,
                                                                        name =
                                                                            $"Cached Shadow Map for #{a_light.GetInstanceID()}",
                                                                        anisoLevel = CachedShadowMapLight
                                                                            ._MAnisoLevel
                                                                    };

      if (a_light.LightComponent.type == LightType.Point) {
        cached_shadow_map.dimension = TextureDimension.Cube;
      }

      cached_shadow_map.Create();
      this._CachedShadowMapTextures[key : a_light.LightComponent] = cached_shadow_map;
    }

    void EnsureCaptureCmdBufferExist(CachedShadowMapLight a_light) {
      if (!this._CaptureCmdBuffers.ContainsKey(key : a_light.LightComponent)) {
        var s = new CommandBuffer {
                                      name =
                                          $"Shadow Map Cache #{unchecked(this._capture_id++)} #CID{a_light.GetInstanceID()}"
                                  };
        //unchecked wraps integer instead of throwing exception
        a_light.LightComponent.AddCommandBuffer(evt : this._capture_event, buffer : s);
        this._CaptureCmdBuffers.Add(key : a_light.LightComponent, value : s);
      }

      a_light.LightComponent.RemoveCommandBuffer(evt : this._capture_event,
                                                 buffer : this._CaptureCmdBuffers
                                                     [key : a_light.LightComponent]);
      a_light.LightComponent.AddCommandBuffer(evt : this._capture_event,
                                              buffer : this._CaptureCmdBuffers[key : a_light.LightComponent]);
    }

    /// <summary>
    /// Disables the cached shadow map functionality for a single CachedShadowMapLight in system
    /// </summary>
    /// <param name="a_light"></param>
    protected internal virtual void RevertToDynamicLighting(CachedShadowMapLight a_light) {
      if (a_light && a_light.LightComponent) {
        a_light.LightComponent.enabled = true;
      }

      #if REMOVE_COMMANDBUFFERS_WHILE_NOT_ACTIVE
            if (this._captureCmdBuffers.ContainsKey(aLight.LightComponent))
            {
                aLight.LightComponent.RemoveCommandBuffer(captureEvent, this._captureCmdBuffers[a_light.LightComponent]);
                this._captureCmdBuffers[a_light.LightComponent].Clear();
            }

            if (this.BlitCmdBuffers.ContainsKey(aLight.LightComponent))
            {
                if(mainCamera?._camera)
                    this.mainCamera._camera.RemoveCommandBuffer(blitEvent, this.BlitCmdBuffers[a_light.LightComponent]);
                    this.BlitCmdBuffers[a_light.LightComponent].Clear();
            }
      #endif

      if (this._CachedShadowMapTextures.ContainsKey(key : a_light.LightComponent)) {
        this._CachedShadowMapTextures[key : a_light.LightComponent].DiscardContents();
        this._CachedShadowMapTextures[key : a_light.LightComponent].Release();
        //this.CachedShadowMapTextures[aLight.LightComponent].MarkRestoreExpected();
      }

      if (this._ActivelyCachedTextures.ContainsKey(key : a_light.LightComponent)) {
        this._ActivelyCachedTextures.Remove(key : a_light.LightComponent);
      }

      if (this._ActivelyCachedLights.Contains(item : a_light.LightComponent)) {
        this._ActivelyCachedLights.Remove(item : a_light.LightComponent);
      }

      if (this._ActivelyCachedShadowMaps.Contains(item : a_light)) {
        this._ActivelyCachedShadowMaps.Remove(item : a_light);
      }

      #if UNITY_EDITOR && CACHED_SHADOW_MAP_DEBUG
        this.UpdateEditorDebugInfo();
      #endif
    }

    /// <summary>
    /// Disables the cached shadow maps for all CachedShadowMapLight in system
    /// </summary>
    public void MakeAllLightsDynamic() {
      #if CACHED_SHADOW_MAP_DEBUG
            Debug.Log("Making all lights dynamic");
      #endif
      foreach (var a_light in this._AllCachedLightComponents) {
        this.RevertToDynamicLighting(a_light : a_light);
      }
    }

    internal void DoNotCacheLights(string a) {
      this.cachingCamera.DisallowCaching();

      foreach (var c in ShadowMapCacheSystem.Instance._AllCachedLightComponents) {
        if (c) {
          c.DoNotCache = true;
        }
      }
    }

    internal void AllowingCachingAgain(string a) {
      this.cachingCamera.AllowCaching();

      foreach (var c in ShadowMapCacheSystem.Instance._AllCachedLightComponents) {
        if (c) {
          c.DoNotCache = false;
        }
      }
    }

    /// <summary>
    /// Disposing of the systems, for clean destruction
    /// </summary>
    public static void Dispose() {
      if (_instance == null) {
        //CustomDebug.Log("ShadowMapCacheSystem instance is null", DebugType.SettingMenus);
        return;
      }

      //CustomDebug.Log("Disposing ShadowMapCacheSystem", DebugType.SettingMenus);

      _disposed = true;
      SceneManager.sceneLoaded -= _instance.NewSceneLoaded;
      GC.Collect();
    }
  }
}
