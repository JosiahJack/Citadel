using System;
using System.Collections.Generic;
using CSM.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Rendering;
using Object = System.Object;

namespace CSM.Runtime.MonoBehaviours {
  /// <inheritdoc />
  /// <summary>
  /// Should be attached to the main camera for displaying the cached shadow maps
  /// </summary>
  [RequireComponent(requiredComponent : typeof(Camera))]
  [DisallowMultipleComponent]
  public class CachedShadowMapReceiverCamera : MonoBehaviour {
    [SerializeField] ShadowMapCacheSystem lightCachingSystem;
    [SerializeField] internal Camera _camera;

    const int _near_pass_idx = 2;
    const int _non_near_pass_idx = 0;
    const string _lighting_shader_name = "Hidden/Internal-DeferredShading";

    Shader _cached_shadow_map_lighting_shader = null;
    Material _cached_shadow_map_lighting_material = null;
    Mesh _spot_light_mesh = null;
    Mesh _point_light_mesh = null;
    Mesh _quad_mesh = null;
    CullingGroup _culling_group = null;
    internal readonly Dictionary<Light, int> _BoundingSphereIndices = new Dictionary<Light, int>();

    static readonly int _shadow_map_texture_prop_id = Shader.PropertyToID("_ShadowMapTexture");
    static readonly int _world_to_shadow_prop_id = Shader.PropertyToID("unity_WorldToShadow");
    static readonly int _world_to_light_prop_id = Shader.PropertyToID("unity_WorldToLight");

    static readonly int _m_light_pos_prop_id = Shader.PropertyToID("_LightPos");

    //Hx Properties
    static readonly int _m_light_projection_params_prop_id = Shader.PropertyToID("_LightProjectionParams");
    static readonly int _m_light_shadow_data_prop_id = Shader.PropertyToID("_LightShadowData");

    static readonly int _m_light_color_prop_id = Shader.PropertyToID("_LightColor");
    static readonly int _m_light_cookie_prop_id = Shader.PropertyToID("_LightTexture0");
    static readonly int _m_light_as_quad_prop_id = Shader.PropertyToID("_LightAsQuad");
    readonly Plane[] _frustum_planes = new Plane[6];
    readonly Plane[] _near_plane = new Plane[1];
    readonly Plane[] _far_plane = new Plane[1];


    # if CACHED_SHADOW_MAP_DEBUG
        /// <summary>
        ///
        /// </summary>
        public bool Debugging
        {
            get { return this.debugging; }
            set
            {
                if (value)
                {
                    this.SetupDebugging();
                }

                this.debugging = value;
            }
        }

        [SerializeField] bool debugging = true;
        [SerializeField] bool drawWireFrames = true;
        [SerializeField] bool drawAtSource = true;
        [SerializeField] [Range(0.001f, 0.2f)] float debug_texture_view_size_frac = 0.06f;

        static readonly int _max_tri_size = Shader.PropertyToID("_MaxTriSize");
        static readonly int _wire_thickness = Shader.PropertyToID("_WireThickness");

        CommandBuffer _debug_command_buffer;

        Material _debug_wireframe_material;
        [SerializeField] Material _debug_quad_blit_material;
        [SerializeField] Material _debug_cube_blit_material;

    #endif

    # if CACHED_SHADOW_MAP_DEBUG
        void SetupDebugging()
        {
            if (!this._debug_wireframe_material)
            {
                this._debug_wireframe_material =
 new Material(Shader.Find("Hidden/Wireframe-Transparent-Culled"));
                this._debug_wireframe_material.SetFloat(_wire_thickness, 300);
                this._debug_wireframe_material.SetFloat(_max_tri_size, 200);
                this._debug_wireframe_material.hideFlags = HideFlags.HideAndDontSave;
            }

            if (!this._debug_quad_blit_material)
            {
                this._debug_quad_blit_material =
                    new Material(Shader.Find("Hidden/DebugDepthMap")) {hideFlags = HideFlags.HideAndDontSave};
            }

            if (!this._debug_cube_blit_material)
            {
                this._debug_cube_blit_material =
                    new Material(Shader.Find("Hidden/DebugDepthCubeMap")) {hideFlags =
 HideFlags.HideAndDontSave};
            }

            EnsureDebugCmdBufferExist();
        }

        void EnsureDebugCmdBufferExist()
        {
            if (this._debug_command_buffer == null)
            {
                _debug_command_buffer = new CommandBuffer
                    {name = "Shadow Map Debug Blit"};
                //unchecked wraps integer instead of throwing exception

                this._camera.AddCommandBuffer(CameraEvent.AfterEverything, _debug_command_buffer);
            }

            this._camera.RemoveCommandBuffer(CameraEvent.AfterEverything, _debug_command_buffer);
            this._camera.AddCommandBuffer(CameraEvent.AfterEverything, _debug_command_buffer);
        }

    #endif

    void RebuildLightCommandBuffer(CommandBuffer lighting_buffer,
                                   CachedShadowMapLight a_light,
                                   bool intersects_near,
                                   bool intersects_far,
                                   RenderTexture cached_shadow_map) {
      switch (a_light.LightComponent.type) {
        case LightType.Spot:
          this.BuildSpotLightBuffer(lighting_buffer : lighting_buffer,
                                    a_light : a_light,
                                    intersects_near : intersects_near,
                                    intersects_far : intersects_far,
                                    cached_shadow_map : cached_shadow_map);
          break;
        case LightType.Point:
          this.BuildPointLightBuffer(lighting_buffer : lighting_buffer,
                                     a_light : a_light,
                                     intersects_near : intersects_near,
                                     intersects_far : intersects_far,
                                     cached_shadow_map : cached_shadow_map);
          break;
      }
    }

    void BuildSpotLightBuffer(CommandBuffer lighting_buffer,
                              CachedShadowMapLight a_light,
                              bool intersects_near,
                              bool intersects_far,
                              RenderTexture cached_shadow_map) {
      var spot_width = Mathf.Tan(f : a_light.LightComponent.spotAngle * 0.5f * Mathf.Deg2Rad)
                       * a_light.LightComponent.range;

      // Ignore scale when computing view transform.
      var view = Matrix4x4.TRS(pos : a_light.CachedPosition, q : a_light.CachedRotation, s : Vector3.one);
      view = Matrix4x4.Inverse(m : view);

      var shadow_projection = Matrix4x4.Perspective(fov : a_light.LightComponent.spotAngle,
                                                    1.0f,
                                                    zNear : a_light.LightComponent.shadowNearPlane,
                                                    zFar : a_light.LightComponent.range);
      var light_projection = Matrix4x4.Perspective(fov : -a_light.LightComponent.spotAngle,
                                                   1.0f,
                                                   0.0f,
                                                   zFar : a_light.LightComponent.range);

      var negate_z = Matrix4x4.Scale(vector : Vector3.one);
      negate_z.m22 = -1;

      var scale_offset = Matrix4x4.identity;
      scale_offset.m00 = scale_offset.m11 = scale_offset.m22 = 0.5f;
      scale_offset.m03 = scale_offset.m13 = scale_offset.m23 = 0.5f;

      // Adjust for DirectX
      if (SystemInfo.usesReversedZBuffer) {
        shadow_projection.m20 = -shadow_projection.m20;
        shadow_projection.m21 = -shadow_projection.m21;
        shadow_projection.m22 = -shadow_projection.m22;
        shadow_projection.m23 = -shadow_projection.m23;
      }

      lighting_buffer.SetGlobalTexture(nameID : _shadow_map_texture_prop_id, value : cached_shadow_map);
      lighting_buffer.SetGlobalMatrix(nameID : _world_to_shadow_prop_id,
                                      value : scale_offset * shadow_projection * negate_z * view);
      lighting_buffer.SetGlobalMatrix(nameID : _world_to_light_prop_id,
                                      value : scale_offset * light_projection * view);

      var position = a_light.CachedPosition;
      //lightingBuffer.SetGlobalVector(_CustomLightPositionPID, position); //For hx
      lighting_buffer.SetGlobalVector(nameID : _m_light_pos_prop_id,
                                      value : new Vector4(x : position.x,
                                                          y : position.y,
                                                          z : position.z,
                                                          w : 1.0f
                                                              / Mathf.Pow(f : a_light.LightComponent.range,
                                                                          2)));

      if (QualitySettings.activeColorSpace == ColorSpace.Gamma) {
        lighting_buffer.SetGlobalColor(nameID : _m_light_color_prop_id,
                                       value : (a_light.LightComponent.color
                                                * a_light.LightComponent.intensity));
      } else {
        lighting_buffer.SetGlobalColor(nameID : _m_light_color_prop_id,
                                       value : (a_light.LightComponent.color
                                                * a_light.LightComponent.intensity)
                                       .linear); // Get color in linear space
      }

      lighting_buffer.SetGlobalTexture(nameID : _m_light_cookie_prop_id,
                                       value : a_light.LightComponent.cookie);
      // y, z and w are complete guesses based on observations
      lighting_buffer.SetGlobalVector(nameID : _m_light_shadow_data_prop_id,
                                      value : new Vector4(x : 1.0f - a_light.LightComponent.shadowStrength,
                                                          y : Mathf.Max(1.0f,
                                                                        b : this._camera.farClipPlane
                                                                            / QualitySettings.shadowDistance),
                                                          z : 5.0f
                                                              / QualitySettings.shadowDistance
                                                              / Mathf.Min(1.0f,
                                                                          b : this._camera.farClipPlane
                                                                              / QualitySettings
                                                                                  .shadowDistance),
                                                          w : QualitySettings.shadowProjection
                                                              == ShadowProjection.CloseFit
                                                                  ? -4.0f
                                                                  : -2.0f
                                                                    - 2.0f
                                                                    * this._camera.fieldOfView
                                                                    / 180.0f));

      lighting_buffer.EnableShaderKeyword("SPOT");
      lighting_buffer.EnableShaderKeyword("SHADOWS_DEPTH");

      if (intersects_near && intersects_far) {
        // Draw full-screen quad.
        // SetViewProjectionMatrices apparently handles platform differences in projection.
        lighting_buffer.SetGlobalFloat(nameID : _m_light_as_quad_prop_id, 1);
        lighting_buffer.SetViewProjectionMatrices(view : Matrix4x4.identity,
                                                  proj : Matrix4x4.Ortho(0.0f,
                                                                         1.0f,
                                                                         0.0f,
                                                                         1.0f,
                                                                         0.0f,
                                                                         zFar : this._camera.farClipPlane));
        lighting_buffer.DrawMesh(mesh : this._quad_mesh,
                                 matrix : Matrix4x4.identity,
                                 material : this._cached_shadow_map_lighting_material,
                                 0,
                                 0);

        #if CACHED_SHADOW_MAP_DEBUG
                if (this.Debugging && this.drawWireFrames && _debug_wireframe_material)
                {
                    lighting_buffer.DrawMesh(this._quad_mesh,
                        Matrix4x4.identity,
                        this._debug_wireframe_material,
                        0,
                        0);
                }
        #endif
      } else {
        // Does not intersect both.
        var pass = intersects_near
                       ? _near_pass_idx
                       : _non_near_pass_idx; // If intersecting near, we need to draw backside of mesh with reversed z-test.

        // Only draw transformed pyramid to avoid activating fragments outside spotlight cone.
        lighting_buffer.SetGlobalFloat(nameID : _m_light_as_quad_prop_id, 0);
        lighting_buffer.DrawMesh(mesh : this._spot_light_mesh,
                                 matrix : a_light.CachedTransform.localToWorldMatrix
                                          * Matrix4x4.Scale(vector : new Vector3(x : spot_width,
                                                                                 y : spot_width,
                                                                                 z : a_light
                                                                                     .LightComponent.range)),
                                 material : this._cached_shadow_map_lighting_material,
                                 0,
                                 shaderPass : pass);

        #if CACHED_SHADOW_MAP_DEBUG
                if (this.Debugging && this.drawWireFrames && _debug_wireframe_material)
                {
                    lighting_buffer.DrawMesh(this._spot_light_mesh,
                        a_light.transform.localToWorldMatrix
                        * Matrix4x4.Scale(new Vector3(spot_width, spot_width, a_light.range)),
                        this._debug_wireframe_material,
                        0,
                        0);
                }
        #endif
      }

      lighting_buffer.DisableShaderKeyword("SPOT");
      lighting_buffer.DisableShaderKeyword("SHADOWS_DEPTH");
    }

    void BuildPointLightBuffer(CommandBuffer lighting_buffer,
                               CachedShadowMapLight a_light,
                               bool intersects_near,
                               bool intersects_far,
                               RenderTexture cached_shadow_map) {
      lighting_buffer.SetGlobalTexture(nameID : _shadow_map_texture_prop_id, value : cached_shadow_map);
      var light_range = a_light.LightComponent.range;
      var shadow_near_plane = a_light.LightComponent.shadowNearPlane;
      var position = a_light.CachedPosition;
      lighting_buffer.SetGlobalVector(nameID : _m_light_pos_prop_id,
                                      value : new Vector4(x : position.x,
                                                          y : position.y,
                                                          z : position.z,
                                                          w : 1.0f / (light_range * light_range)));

      //lightingBuffer.SetGlobalVector(_CustomLightPositionPID, position); //For hx
      // w is a complete guess. Could not find any parameters that change the value.
      var a = (shadow_near_plane - light_range);
      lighting_buffer.SetGlobalVector(nameID : _m_light_projection_params_prop_id,
                                      value : new Vector4(x : light_range / a,
                                                          y : (shadow_near_plane * light_range) / a,
                                                          z : a_light.LightComponent.shadowBias,
                                                          0.97f));
      if (QualitySettings.activeColorSpace == ColorSpace.Gamma) {
        lighting_buffer.SetGlobalColor(nameID : _m_light_color_prop_id,
                                       value : (a_light.LightComponent.color
                                                * a_light.LightComponent.intensity));
      } else {
        lighting_buffer.SetGlobalColor(nameID : _m_light_color_prop_id,
                                       value : (a_light.LightComponent.color
                                                * a_light.LightComponent.intensity)
                                       .linear); // Get color in linear space
      }

      lighting_buffer.SetGlobalTexture(nameID : _m_light_cookie_prop_id,
                                       value : a_light.LightComponent.cookie);
      // y, z and w are complete guesses based on observations.
      lighting_buffer.SetGlobalVector(nameID : _m_light_shadow_data_prop_id,
                                      value : new Vector4(x : 1.0f - a_light.LightComponent.shadowStrength,
                                                          y : Mathf.Max(1.0f,
                                                                        b : this._camera.farClipPlane
                                                                            / QualitySettings.shadowDistance),
                                                          z : 5.0f
                                                              / QualitySettings.shadowDistance
                                                              / Mathf.Min(1.0f,
                                                                          b : this._camera.farClipPlane
                                                                              / QualitySettings
                                                                                  .shadowDistance),
                                                          w : QualitySettings.shadowProjection
                                                              == ShadowProjection.CloseFit
                                                                  ? -4.0f
                                                                  : -2.0f
                                                                    - 2.0f
                                                                    * this._camera.fieldOfView
                                                                    / 180.0f));

      lighting_buffer.EnableShaderKeyword("POINT");
      lighting_buffer.EnableShaderKeyword("SHADOWS_CUBE");

      if (intersects_near && intersects_far) {
        // Draw full-screen quad.
        // SetViewProjectionMatrices apparently handles platform differences in projection.
        lighting_buffer.SetGlobalFloat(nameID : _m_light_as_quad_prop_id, 1);
        lighting_buffer.SetViewProjectionMatrices(view : Matrix4x4.identity,
                                                  proj : Matrix4x4.Ortho(0.0f,
                                                                         1.0f,
                                                                         0.0f,
                                                                         1.0f,
                                                                         0.0f,
                                                                         zFar : this._camera.farClipPlane));
        lighting_buffer.DrawMesh(mesh : this._quad_mesh,
                                 matrix : Matrix4x4.identity,
                                 material : this._cached_shadow_map_lighting_material,
                                 0,
                                 0);

        #if CACHED_SHADOW_MAP_DEBUG
                if (this.Debugging && this.drawWireFrames && _debug_wireframe_material)
                {
                    lighting_buffer.DrawMesh(this._quad_mesh,
                        Matrix4x4.identity,
                        this._debug_wireframe_material,
                        0,
                        0);
                }
        #endif
      } else {
        // Does not intersect both.
        // If intersecting near, we need to draw backside of mesh with reversed z-test.

        var pass = intersects_near ? _near_pass_idx : _non_near_pass_idx;

        // Only draw transformed sphere to avoid activating fragments outside point light sphere.
        lighting_buffer.SetGlobalFloat(nameID : _m_light_as_quad_prop_id, 0);
        lighting_buffer.DrawMesh(mesh : this._point_light_mesh,
                                 matrix : a_light.CachedTransform.localToWorldMatrix
                                          * Matrix4x4.Scale(vector : new Vector3(x : 2.0f * light_range,
                                                                                 y : 2.0f * light_range,
                                                                                 z : 2.0f * light_range)),
                                 material : this._cached_shadow_map_lighting_material,
                                 0,
                                 shaderPass : pass);
        #if CACHED_SHADOW_MAP_DEBUG
                if (this.Debugging && this.drawWireFrames && _debug_wireframe_material)
                {
                    lighting_buffer.DrawMesh(this._point_light_mesh,
                        a_light.transform.localToWorldMatrix
                        * Matrix4x4.Scale(new Vector3(2.0f * light_range,
                            2.0f * light_range,
                            2.0f * light_range)),
                        this._debug_wireframe_material,
                        0,
                        0);
                }
        #endif
      }

      lighting_buffer.DisableShaderKeyword("POINT");
      lighting_buffer.DisableShaderKeyword("SHADOWS_CUBE");
    }
/*
    private void OnPostRender()
    {
      Debug.unityLogger.filterLogType = LogType.Log;
    }

*/

    void OnPreRender() {
      # if CACHED_SHADOW_MAP_DEBUG
            if (_debug_command_buffer != null)
            {
                this._debug_command_buffer.Clear();
                if (this.Debugging && this._debug_quad_blit_material)
                {
                    //Debug.unityLogger.filterLogType = LogType.Exception;
                    // Display the shadow maps in the corner.
                    var debug_x = 0;
                    var debug_y = 0;
                    var zero_one_rect = new Rect(0,
                        0,
                        1,
                        1);
                    var a = Screen.width * this.debug_texture_view_size_frac;

                    foreach (var a_light in this.lightCachingSystem.AllCachedLightComponents)
                    {
                        if (!this.drawAtSource)
                        {
                            this._debug_command_buffer.SetViewport(new Rect(a * debug_x++,
                                a * debug_y,
                                a,
                                a));
                            if (debug_x * a >= Screen.width - a)
                            {
                                debug_x = 0;
                                ++debug_y;
                            }
                        }
                        else
                        {
                            var pos = _camera.WorldToViewportPoint(a_light.transform.position);
                            if (!zero_one_rect.Contains(pos)
                                || pos.z <= 0
                                || pos.z >= _camera.farClipPlane) continue;
                            var size = (1.0f - Mathf.Pow(pos.z / _camera.farClipPlane, 2))
                                       * this.debug_texture_view_size_frac
                                       * Mathf.Min(Screen.height, Screen.width);
                            if (size < 1) continue;
                            this._debug_command_buffer.SetViewport(new Rect(Screen.width * pos.x - size / 2,
                                Screen.height * pos.y - size / 2,
                                size,
                                size));
                        }


                        this._debug_command_buffer.SetShadowSamplingMode(this.lightCachingSystem.CachedShadowMapTextures
                                [a_light.LightComponent],
                            ShadowSamplingMode.RawDepth);

                        this._debug_command_buffer.SetGlobalColor("_DebugColor",
                            this.lightCachingSystem.ActivelyCachedLights
                                .Contains(a_light.LightComponent)
                                ? Color.green
                                : Color.red);
                        if (a_light.LightComponent.type != LightType.Point)
                        {
                            this._debug_command_buffer.Blit(
                                this.lightCachingSystem.CachedShadowMapTextures[a_light.LightComponent],
                                BuiltinRenderTextureType.CurrentActive,
                                this._debug_quad_blit_material);
                        }
                        else
                        {
                            this._debug_command_buffer.Blit(
                                this.lightCachingSystem.CachedShadowMapTextures[a_light.LightComponent],
                                BuiltinRenderTextureType.CurrentActive,
                                this._debug_cube_blit_material);
                        }

                        this._debug_command_buffer.SetShadowSamplingMode(this.lightCachingSystem.CachedShadowMapTextures
                                [a_light.LightComponent],
                            ShadowSamplingMode.CompareDepths);
                    }
                }
            }
      #endif

      // Set up near and far planes for intersection tests.
      GeometryUtility.CalculateFrustumPlanes(camera : this._camera, planes : this._frustum_planes);
      this._frustum_planes[4].Flip();
      this._frustum_planes[5].Flip();
      this._near_plane[0] = this._frustum_planes[4];
      this._far_plane[0] = this._frustum_planes[5];

      for (var index = 0; index < this.lightCachingSystem._ActivelyCachedShadowMaps.Count; index++) {
        var a_light = this.lightCachingSystem._ActivelyCachedShadowMaps[index : index];
        if (!this.lightCachingSystem._BlitCmdBuffers.ContainsKey(key : a_light.LightComponent)) {
          //                  #if CACHED_SHADOW_MAP_DEBUG
          Debug.LogError(message :
                         $"Expected a lighting buffer to exist for light {a_light.GetInstanceID()}, reverting light to dynamic",
                         context : a_light);
//#endif
          this.lightCachingSystem.RevertToDynamicLighting(a_light : a_light);
          continue;
        }

        if (!this.lightCachingSystem._CachedShadowMapTextures[key : a_light.LightComponent].IsCreated()) {
//#if CACHED_SHADOW_MAP_DEBUG
          Debug.LogError(message :
                         $"RenderTexture for light {a_light.GetInstanceID()} is not created anymore, reverting light to dynamic",
                         context : a_light);
//#endif
          this.lightCachingSystem.RevertToDynamicLighting(a_light : a_light);
          continue;
        }

        var blit_buffer = this.lightCachingSystem._BlitCmdBuffers[key : a_light.LightComponent];
        blit_buffer.Clear();

        if (this.Visible(a_light : a_light.LightComponent)) {
          // Cull lights that are not visible.

          // Test if intersects near or far clip planes.
          var intersects_near = GeometryUtility.TestPlanesAABB(planes : this._near_plane,
                                                               bounds : this
                                                                        .lightCachingSystem
                                                                        ._LightBounds[key : a_light
                                                                                          .LightComponent]);
          var intersects_far = GeometryUtility.TestPlanesAABB(planes : this._far_plane,
                                                              bounds : this
                                                                       .lightCachingSystem
                                                                       ._LightBounds[key : a_light
                                                                                         .LightComponent]);

          //Should be rebuild every frame
          this.RebuildLightCommandBuffer(lighting_buffer : blit_buffer,
                                         a_light : a_light,
                                         intersects_near : intersects_near,
                                         intersects_far : intersects_far,
                                         cached_shadow_map :
                                         this.lightCachingSystem._CachedShadowMapTextures
                                             [key : a_light.LightComponent]);
        }
      }
    }

    void Awake() {
      this.lightCachingSystem = ShadowMapCacheSystem.Instance;
      this._camera = this.GetComponent<Camera>();

      #if CACHED_SHADOW_MAP_DEBUG
            if (Debugging)
            {
                this.SetupDebugging();
            }
      #endif

      if (!this._cached_shadow_map_lighting_shader) {
        this._cached_shadow_map_lighting_shader = Shader.Find(name : _lighting_shader_name);
      }

      // Create material used to render lights.
      if (!this._cached_shadow_map_lighting_material) {
        this._cached_shadow_map_lighting_material =
            new Material(shader : this._cached_shadow_map_lighting_shader) {
                                                                               hideFlags =
                                                                                   HideFlags.HideAndDontSave
                                                                           };
      }

      // Create mesh used to render lights.
      if (!this._spot_light_mesh) {
        this._spot_light_mesh = MeshUtility.CreateSpotlightPyramid();
        //this._spot_light_mesh = MeshUtility.CreateSpotlightCone();
        //this._spot_light_mesh = MeshUtility.GenerateSpotlightCircleMesh(_camera);
      }

      if (!this._point_light_mesh) {
        this._point_light_mesh = MeshUtility.CreateIcoSphere(1, 0.56f);
      }

      if (!this._quad_mesh) {
        this._quad_mesh = MeshUtility.CreateScreenSpaceQuad(main : this._camera);
      }
    }

    void OnDestroy() {
      ShadowMapCacheSystem.Dispose();
      this._culling_group?.Dispose();
      this._culling_group = null;
    }

    void OnDisable() {
      #if CACHED_SHADOW_MAP_DEBUG
            if (this._debug_command_buffer != null)
            {
                this._debug_command_buffer.Dispose();
                this._debug_command_buffer = null;
            }

            DestroyImmediate(this._debug_wireframe_material);
            DestroyImmediate(this._debug_quad_blit_material);
            DestroyImmediate(this._debug_cube_blit_material);
      #endif

      DestroyImmediate(obj : this._cached_shadow_map_lighting_material);
      DestroyImmediate(obj : this._spot_light_mesh);
      DestroyImmediate(obj : this._point_light_mesh);
      DestroyImmediate(obj : this._quad_mesh);
    }

    /// <summary>
    /// Is light visible to the main camera?
    /// </summary>
    /// <param name="a_light"></param>
    /// <returns></returns>
    public bool Visible(Light a_light) {
      if (_BoundingSphereIndices.ContainsKey(key : a_light)) {
        return this._culling_group.IsVisible(index : this._BoundingSphereIndices[key : a_light]);
      } else {
        return true;
      }
    }

    internal void RefreshCullingGroup() {
      if (this.lightCachingSystem._LightBoundingSpheres.Count > 0) {
        if (this._culling_group == null) {
          this._culling_group = new CullingGroup {targetCamera = this._camera};
        }

        this._BoundingSphereIndices.Clear();

        var bounding_sphere_array = new BoundingSphere[this.lightCachingSystem._LightBoundingSpheres.Count];
        var index = 0;
        foreach (var entry in this.lightCachingSystem._LightBoundingSpheres) {
          bounding_sphere_array[index] = entry.Value;
          this._BoundingSphereIndices.Add(key : entry.Key, value : index++);
        }

        this._culling_group.SetBoundingSpheres(array : bounding_sphere_array);
        this._culling_group.SetBoundingSphereCount(count : this
                                                           .lightCachingSystem._LightBoundingSpheres.Count);
      } else {
        this._culling_group?.Dispose();
        this._culling_group = null;
      }
    }
  }
}
