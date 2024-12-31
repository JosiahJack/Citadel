#define AUTO_REMOVE_NON_INTERSECTION_DYNAMIC_OBJECTS
#define LIGHT_CHANGED_TEST
//#define DELAYED_RETRY

using System;
using System.Collections.Generic;
using CSM.Runtime.Structs;
using CSM.Runtime.Utilities;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CSM.Runtime.MonoBehaviours {
  /// <inheritdoc />
  ///  <summary>
  /// The component should be attached to any spot or point light that should be cached
  ///  </summary>
  [RequireComponent(requiredComponent : typeof(Light))]
  [DisallowMultipleComponent]
  public class CachedShadowMapLight : TransformTracker {
    /// <summary>
    ///
    /// </summary>
    public enum BitDepth {
      Sixteen_bits_ = 16,
      Twentyfour_bits_ = 24,
      Thirtytwo_bits_ = 32
    }

    public static FilterMode _MFilterMode = FilterMode.Bilinear;
    public static int _MAnisoLevel = 1;
    public static BitDepth MShadowMapBitDepth { get; set; } = BitDepth.Sixteen_bits_;


    /// <summary>
    /// The dynamicObjects of this light source immediate context
    /// </summary>
    [SerializeField]
    List<DynamicObject> dynamicObjects = new List<DynamicObject>(32);

    [SerializeField] [ReadOnly] protected int currentResolution = 256;

    internal virtual void UpdateShadowMapResolution(int resolution = 1048)
    {
      if(this.LightComponent.shadowCustomResolution == resolution && !this.isActiveAndEnabled) {
        return;
      }

      resolution = 512;//;Mathf.Max(a : Mathf.ClosestPowerOfTwo(value : resolution),256);

      this.LightComponent.shadowCustomResolution = resolution;
      this.gameObject.SetActive(false);
      this.gameObject.SetActive(true);
    }

    void Set32Bit()
    {
        // 32-bit not used anymore - had some performance issues -G, 6/1/2020
        if (Graphics.activeTier == GraphicsTier.Tier3) {
          MShadowMapBitDepth = BitDepth.Thirtytwo_bits_;
        } else {
          MShadowMapBitDepth = BitDepth.Sixteen_bits_;
        }
    }

    /// <summary>
    /// Adds a dynamicObject to the light source context, it
    /// </summary>
    /// <param name="dob"></param>
    /// <param name="assume_dirty"></param>
    public void AddDynamicObject(DynamicObject dob, bool assume_dirty = true) {
      if (!this.dynamicObjects.Contains(item : dob)) {
        this.dynamicObjects.Add(item : dob);
        if (assume_dirty) {
          //Debug.Log("AddDynamicObject",this);
          ShadowMapCacheSystem.Instance.RevertToDynamicLighting(a_light : this);
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public Light LightComponent { get; private set; }



    [SerializeField] LightParameters lp;
    [SerializeField] bool lightChanged = false;
    [SerializeField] bool doNotCache = false;
#if DELAYED_RETRY
    [SerializeField] private int penalty_i = 0;
    [SerializeField] private int penalty_delay = 300;
#endif

    /// <summary>
    /// Set this flag if the light should not be cached
    /// </summary>
    public bool DoNotCache {
      get { return this.doNotCache; }
      set {
        this.doNotCache = value;
        if (value) {
          //Debug.Log("DoNotCache", this);
          ShadowMapCacheSystem.Instance.RevertToDynamicLighting(a_light : this);
        } else {
          this.MaybeRecomputeCache();
        }
      }
    }

    void MaybeRecomputeCache()
    {


      if (!this.LightComponent || !this.isActiveAndEnabled) {
        return;
      }

      if (!this.LightComponent.enabled){
        //if (ShadowMapCacheSystem.Instance.ActivelyCachedLights.Contains(this.LightComponent)){
          // Check if cached light needs to be updated immediately.
          // It needs to be updated if there are any dynamic objects moving inside the range.
          // It also needs to be updated for one frame after all dynamic objects left the range.

          if (this.DoNotCache
              || this.lightChanged
              || this.Moved
              || IntersectionUtility.DidDynamicObjectsChange(a_light : this, dynamic_objects : ref this.dynamicObjects)){
            //Debug.Log("MaybeRecomputeCheck1",this);
            ShadowMapCacheSystem.Instance.RevertToDynamicLighting(a_light : this);
          }
        //}
      }
      else if (!this.DoNotCache &&
               !this.lightChanged &&
               !this.Moved) {

#if DELAYED_RETRY
        if(penalty_i>0) return;
#endif

        if (!IntersectionUtility.DidDynamicObjectsChange(a_light : this, dynamic_objects : ref this.dynamicObjects)) {
          ShadowMapCacheSystem.Instance.EnqueueDirtyCandidate(a_light : this);
#if DELAYED_RETRY
          penalty_i = penalty_delay;
          #endif
        }
      }
    }

    void LateUpdate()
    {
      #if DELAYED_RETRY
      if (penalty_i > 0) penalty_i--;
      #endif
      this.Tick();
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnMove()
    {
      #if CACHED_SHADOW_MAP_DEBUG
            Debug.Log("Make light dynamic because it moved", this);
      #endif
      ShadowMapCacheSystem.Instance.RevertToDynamicLighting(a_light : this);
    }

    void Awake() {
      this.LightComponent = this.GetComponent<Light>();

            if (this.LightComponent == null)
            {
#if UNITY_EDITOR && CACHED_SHADOW_MAP_DEBUG
              if (Application.isEditor)
                Debug.LogError($" {this.GetInstanceID()} does not have a LightComponent, disabling CachedShadowMapLight component");
#endif
              this.enabled = false;
                return;
            }

            if (this.LightComponent.shadows == LightShadows.None)
            {
              #if UNITY_EDITOR && CACHED_SHADOW_MAP_DEBUG
                if (Application.isEditor)
                    Debug.LogError($" {this.GetInstanceID()} does not cast shadows, disabling CachedShadowMapLight component");
              #endif
              this.enabled = false;
                return;
            }

      if (this.isActiveAndEnabled) {
        this.UpdateShadowMapResolution();
      }
    }

    /// <inheritdoc />
    ///  <summary>
    ///  </summary>
    protected override void Setup() {
      if (this.LightComponent.shadows == LightShadows.None
          || (this.LightComponent.type != LightType.Spot && this.LightComponent.type != LightType.Point)
          || !this.isActiveAndEnabled) {
        return;
      }

      ShadowMapCacheSystem.Instance.AddLightToSystem(a_light : this);

      this.lp = new LightParameters {
                                        _SpotAngle = this.LightComponent.spotAngle,
                                        /*
                                        type = LightComponent.type,
                                        color = LightComponent.color,
                                        intensity = LightComponent.intensity,
                                        shadowRadius = LightComponent.shadowRadius,
                                        range = LightComponent.range,*/
                                    };

      this.OnNowMovingEvent += this.OnMove;
      this.OnNowRestingEvent += this.MaybeRecomputeCache;
      this.OnRestingEvent += this.MaybeRecomputeCache;
      this.OnMovingEvent += this.OnMove;
      this.OnTickEvent += this.DidLightChange;
    }

    void DidLightChange() {

      // Most these checks are not necessary apart from spotlight angle
      this.lightChanged = false;


      if (this.LightComponent.enabled
          && ShadowMapCacheSystem.Instance._ActivelyCachedLights.Contains(item : this.LightComponent)) {
        this.lightChanged = true;
        ShadowMapCacheSystem.Instance.RevertToDynamicLighting(a_light : this);
      }

#if LIGHT_CHANGED_TEST
      if (this.LightComponent.type == LightType.Spot) {
        if (Math.Abs(value : this.LightComponent.spotAngle - this.lp._SpotAngle) > float.Epsilon) {
          this.lp._SpotAngle = this.LightComponent.spotAngle;
          this.lightChanged = true;
          ShadowMapCacheSystem.Instance.RevertToDynamicLighting(a_light : this);
        }
      }

      /*
      if (LightComponent.type != lp.type)
      {
          // If this happens some weird side effects happens, suspected to be caused by something internally in unity
          lp.type = LightComponent.type;
          light_changed = true;
          //ShadowMapCacheSystem.Instance.RemoveLight(this);
          //ShadowMapCacheSystem.Instance.AddLight(this);
      }


      if (LightComponent.color != lp.color)
      {
          lp.color = LightComponent.color;
          light_changed = true;
      }

      if (LightComponent.intensity != lp.intensity)
      {
          lp.intensity = LightComponent.intensity;
          light_changed = true;
      }

      if (LightComponent.shadowRadius != lp.shadowRadius)
      {
          lp.shadowRadius = LightComponent.shadowRadius;
          light_changed = true;
      }

      if (LightComponent.range != lp.range)
      {
          lp.range = LightComponent.range;
          light_changed = true;
      }
*/
#endif


      IntersectionUtility.RemoveNonOverlapping(a_light : this, dynamic_objects : ref this.dynamicObjects);
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    protected override void Teardown() {
      this.OnNowMovingEvent -= this.OnMove;
      this.OnNowRestingEvent -= this.MaybeRecomputeCache;
      this.OnRestingEvent -= this.MaybeRecomputeCache;
      this.OnMovingEvent -= this.OnMove;
      this.OnTickEvent -= this.DidLightChange;

      ShadowMapCacheSystem.Instance.RemoveLightFromSystem(a_light : this);
    }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected() {
      var sphere = BoundsUtility.ComputeLightBoundingSphere(a_light : this);
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(center : sphere.position, radius : sphere.radius);
    }
    #endif
  }
}
