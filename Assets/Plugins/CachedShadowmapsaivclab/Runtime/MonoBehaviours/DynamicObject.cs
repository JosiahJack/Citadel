#define AUTO_ADD_OVERLAPPING_LIGHTS

using System;
using CSM.Runtime.Utilities;
using UnityEngine;

namespace CSM.Runtime.MonoBehaviours
{
    /// <inheritdoc />
    /// <summary>
    /// The component should be attached to anything the affect the shadows of a cached shadow map, whenever the transform of the gameObject which this monoBehaviour is attached to is moving the CachedShadowMapsLight's will be dirtied and therefore disabled until entire "context"(this DynamicObject, other DynamicObjects and the lightSource itself) is resting again and the cachedShadowMap will be recomputed.
    /// </summary>
    //[ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class DynamicObject : TransformTracker
    {
        /// <summary>
        /// the bounding sphere center offset
        /// </summary>
        public Vector3 offset = Vector3.zero;

        /// <summary>
        /// the radius of the bounding sphere
        /// </summary>
        public float boundingSphereRadius = 1.0f;

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(center : this.transform.position + this.offset, radius : this
            .boundingSphereRadius);
        }
#endif

#if AUTO_ADD_OVERLAPPING_LIGHTS
        /// <inheritdoc />
        ///  <summary>
        ///  </summary>
        protected override void Setup()
        {
            base.Setup();
            this.AddToOverlappingLight();
            this.OnMovingEvent += this.AddToOverlappingLight;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        protected override void Teardown()
        {
            base.Teardown();
            this.OnMovingEvent -= this.AddToOverlappingLight;
        }

        void Update()
        {
            this.Tick();
        }

        void AddToOverlappingLight()
        {
            foreach (var cached_light in ShadowMapCacheSystem.Instance._AllCachedLightComponents)
            {
                if (!cached_light || !cached_light.isActiveAndEnabled)
                {
                    continue;
                }

                var intersects = false;
                switch (cached_light.LightComponent.type)
                {
                    case LightType.Spot:

                        intersects = IntersectionUtility.ConeSphereIntersection(spot_light : cached_light,
                            dynamic_object : this);
                        break;
                    case LightType.Point:
                        intersects =
                            IntersectionUtility.SphereSphereIntersection(point_light : cached_light,
                                dynamic_object : this);
                        break;
                    case LightType.Directional:
                    case LightType.Area:
                    case LightType.Disc:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (intersects)
                {
                    //Debug.Log("Im a this the one destroying everything",this);
                    cached_light.AddDynamicObject(dob : this);
                }
            }
        }
#endif
    }
}
