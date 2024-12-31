using CSM.Runtime.MonoBehaviours;
using UnityEngine;

namespace CSM.Runtime.Utilities {
  /// <summary>
  /// Utilities for computing bounds
  /// </summary>
  public static class BoundsUtility {

    const float _default_inflation = 1.2f;

    /// <summary>
    /// Compute bounding sphere of light
    /// </summary>
    /// <param name="a_light"></param>
    /// <param name="inflation"></param>
    /// <returns></returns>
    public static BoundingSphere ComputeLightBoundingSphere(CachedShadowMapLight a_light,float inflation=_default_inflation) {
      if (a_light.LightComponent) {
        switch (a_light.LightComponent.type) {
          case LightType.Spot: {
            return ComputeSpotLightBoundingSphere(light : a_light,inflation : inflation);
          }
          case LightType.Point: {
            return new BoundingSphere(pos : a_light.CachedPosition, rad : a_light.LightComponent.range*inflation);
          }
        }
      }

      return new BoundingSphere();
    }

    /// <summary>
    /// Compute bounding sphere of a spot light
    /// See illustration at https://www.mathalino.com/sites/default/files/users/Mathalino/differential-calculus/063-cone-inscribed-in-sphere.jpg
    /// </summary>
    /// <param name="light"></param>
    /// <param name="inflation"></param>
    /// <returns></returns>
    public static BoundingSphere ComputeSpotLightBoundingSphere(CachedShadowMapLight light,float inflation=_default_inflation) {
      if (light.LightComponent.spotAngle < 90.0f) {
        var r = Mathf.Tan(f : light.LightComponent.spotAngle * 0.5f * Mathf.Deg2Rad) * light.LightComponent.range;
        var h = light.LightComponent.range;
        var a = (r * r + h * h) / (2.0f * h);

        return new BoundingSphere(pos : light.CachedPosition + light.CachedForward * a, rad : a*inflation);
      } else {
        var r = Mathf.Tan(f : light.LightComponent.spotAngle * 0.5f * Mathf.Deg2Rad) * light.LightComponent.range;

        return new BoundingSphere(pos : light.CachedPosition + light.CachedForward * light.LightComponent.range, rad : r*inflation);
      }
    }

    /// <summary>
    /// Compute bounds of light
    /// </summary>
    /// <param name="light"></param>
    /// <param name="inflation"></param>
    /// <returns></returns>
    public static Bounds ComputeLightBounds(CachedShadowMapLight light,float inflation=_default_inflation) {
      if (light) {
        switch (light.LightComponent.type) {
          case LightType.Spot: {
            return ComputeSpotLightBounds(light : light,inflation : inflation);
          }
          case LightType.Point: {
            return ComputePointLightBounds(light : light,inflation : inflation);
          }
        }
      }

      return new Bounds();
    }

    /// <summary>
    /// Compute bounds of a spot light
    /// </summary>
    /// <param name="light"></param>
    /// <param name="inflation"></param>
    /// <returns></returns>
    public static Bounds ComputeSpotLightBounds(CachedShadowMapLight light,float inflation=_default_inflation) {
      var transform = light.CachedTransform;
      var forward1 = transform.forward;
      var pos = light.CachedPosition + -forward1 * (inflation-1.0f);
      var forward = forward1*inflation;
      var right = transform.right*inflation;
      var up = transform.up*inflation;

      var far_center = pos + forward * light.LightComponent.range;

      var far_height = Mathf.Tan(f : light.LightComponent.spotAngle * 0.5f * Mathf.Deg2Rad) * light.LightComponent.range;

      var far_top_left = far_center + up * (far_height) - right * (far_height);
      var far_top_right = far_center + up * (far_height) + right * (far_height);
      var far_bottom_left = far_center - up * (far_height) - right * (far_height);
      var far_bottom_right = far_center - up * (far_height) + right * (far_height);

      var min_bounds =
          new Vector3(x : Mathf.Min(a : far_top_left.x,
                                    b : Mathf.Min(a : far_top_right.x,
                                                  b : Mathf.Min(a : far_bottom_left.x,
                                                                b : Mathf.Min(a : far_bottom_right.x, b : pos.x)))),
                      y : Mathf.Min(a : far_top_left.y,
                                    b : Mathf.Min(a : far_top_right.y,
                                                  b : Mathf.Min(a : far_bottom_left.y,
                                                                b : Mathf.Min(a : far_bottom_right.y, b : pos.y)))),
                      z : Mathf.Min(a : far_top_left.z,
                                    b : Mathf.Min(a : far_top_right.z,
                                                  b : Mathf.Min(a : far_bottom_left.z,
                                                                b : Mathf.Min(a : far_bottom_right.z, b : pos.z)))));
      var max_bounds =
          new Vector3(x : Mathf.Max(a : far_top_left.x,
                                    b : Mathf.Max(a : far_top_right.x,
                                                  b : Mathf.Max(a : far_bottom_left.x,
                                                                b : Mathf.Max(a : far_bottom_right.x, b : pos.x)))),
                      y : Mathf.Max(a : far_top_left.y,
                                    b : Mathf.Max(a : far_top_right.y,
                                                  b : Mathf.Max(a : far_bottom_left.y,
                                                                b : Mathf.Max(a : far_bottom_right.y, b : pos.y)))),
                      z : Mathf.Max(a : far_top_left.z,
                                    b : Mathf.Max(a : far_top_right.z,
                                                  b : Mathf.Max(a : far_bottom_left.z,
                                                                b : Mathf.Max(a : far_bottom_right.z, b : pos.z)))));

      var bounds = new Bounds();
      bounds.SetMinMax(min : min_bounds, max : max_bounds);
      return bounds;
    }

    /// <summary>
    /// Compute bounds of a point light, add the same(inflation) percentage to the range as when creating the light geometry.
    /// </summary>
    /// <param name="light"></param>
    /// <param name="inflation"></param>
    /// <returns></returns>
    public static Bounds ComputePointLightBounds(CachedShadowMapLight light, float inflation=_default_inflation) {
      var range = light.LightComponent.range;
      var dif = new Vector3(x : range, y : range, z : range) * inflation;

      var min_bounds = light.CachedPosition - dif;
      var max_bounds = light.CachedPosition + dif;

      var bounds = new Bounds();
      bounds.SetMinMax(min : min_bounds, max : max_bounds);
      return bounds;
    }
  }
}