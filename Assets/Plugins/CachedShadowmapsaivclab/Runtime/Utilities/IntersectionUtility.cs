using System;
using System.Collections.Generic;
using CSM.Runtime.MonoBehaviours;
using UnityEngine;

namespace CSM.Runtime.Utilities
{
    /// <summary>
    /// Utilities for intersection tests
    /// </summary>
    public static class IntersectionUtility
    {
        internal static bool DidIntersectingDynamicsObjectChange(CachedShadowMapLight a_light,
            ref List<DynamicObject> dynamic_objects,
            bool only_when_intersecting = true)
        {
            var changed = false;
            if (a_light)
            {
                for (var index = 0; index < dynamic_objects.Count; index++)
                {
                    var dynamic_object = dynamic_objects[index : index];
                    var intersects = false;
                    if (dynamic_object)
                    {
                        switch (a_light.LightComponent.type)
                        {
                            case LightType.Spot:
                                intersects = ConeSphereIntersection(spot_light : a_light, dynamic_object : dynamic_object);
                                break;
                            case LightType.Point:
                                intersects = SphereSphereIntersection(point_light : a_light, dynamic_object : dynamic_object);
                                break;
                            case LightType.Directional:
                            case LightType.Area:
                            case LightType.Disc:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        if (intersects || !only_when_intersecting)
                        {
                            if (dynamic_object.ChangeEnableState
                                || (dynamic_object.Moved
                                    && dynamic_object.isActiveAndEnabled
                                    && dynamic_object.gameObject.activeInHierarchy))
                            {
                                changed = true;
                            }
                        }
                    }
                }
            }

            return changed;
        }

        static bool NotIntersecting(CachedShadowMapLight a_light, DynamicObject dynamic_object)
        {
            if (dynamic_object && dynamic_object.isActiveAndEnabled && dynamic_object.gameObject.activeInHierarchy)
            {
                switch (a_light.LightComponent.type)
                {
                    case LightType.Spot:
                        return !ConeSphereIntersection(spot_light : a_light, dynamic_object : dynamic_object);
                    case LightType.Point:
                        return !SphereSphereIntersection(point_light : a_light, dynamic_object : dynamic_object);
                    case LightType.Directional:
                    case LightType.Area:
                    case LightType.Disc:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }

        internal static void RemoveNonOverlapping(CachedShadowMapLight a_light, ref List<DynamicObject> dynamic_objects)
        {
            if (dynamic_objects.Count < 1 || !a_light) {
                return;
            }

            for (var i = dynamic_objects.Count - 1; i >= 0; i--)
            {
                if (NotIntersecting(a_light : a_light, dynamic_object : dynamic_objects[index : i])) {
                    dynamic_objects.RemoveAt(index : i);
                }
            }

            // dynamic_objects.RemoveAll(o=>NotIntersecting(aLight,o)); // GENERATES GARBAGE!
        }

        internal static bool DidDynamicObjectsChange(CachedShadowMapLight a_light, ref List<DynamicObject> dynamic_objects)
        {
            var changed = false;
            if (a_light && dynamic_objects.Count > 0)
            {
                for (var index = 0; index < dynamic_objects.Count; index++)
                {
                    var o = dynamic_objects[index : index];
                    var dynamic_object = o;
                    if (dynamic_object == null) {
                        continue;
                    }

                    if (dynamic_object.ChangeEnableState
                        || (dynamic_object.Moved
                            && dynamic_object.isActiveAndEnabled
                            && dynamic_object.gameObject.activeInHierarchy))
                    {
                        changed = true;
                    }
                }
            }

            return changed;
        }

        /// <summary>
        /// see https://www.geometrictools.com/Documentation/IntersectionSphereCone.pdf
        /// </summary>
        /// <param name="spot_light"></param>
        /// <param name="dynamic_object"></param>
        /// <returns></returns>
        public static bool ConeSphereIntersection(CachedShadowMapLight spot_light,
            DynamicObject dynamic_object)
        {
            // FIXME Optimize computations of boundingSphereRadius^2, 1.0f / Mathf.Sin(angle * 0.5f), Mathf.Sin(angle * 0.5f)^2 and  Mathf.Cos(angle * 0.5f)^2
            var sin = Mathf.Sin(f : spot_light.LightComponent.spotAngle * 0.5f * Mathf.Deg2Rad);
            var cos = Mathf.Cos(f : spot_light.LightComponent.spotAngle * 0.5f * Mathf.Deg2Rad);
            var offset = (dynamic_object.boundingSphereRadius / sin);
            var u = spot_light.CachedPosition - offset * spot_light.CachedForward; // Assumes unit length forward.
            var d = dynamic_object.CachedPosition + dynamic_object.offset - u;

            var distance = Vector3.Dot(lhs : spot_light.CachedForward, rhs : d); // Assumes unit length forward.

            return distance >= d.magnitude * cos
                   && distance <= offset + spot_light.LightComponent.range + dynamic_object.boundingSphereRadius;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="point_light"></param>
        /// <param name="dynamic_object"></param>
        /// <returns></returns>
        public static bool SphereSphereIntersection(CachedShadowMapLight point_light, DynamicObject dynamic_object)
        {
            var diff = dynamic_object.CachedPosition + dynamic_object.offset - point_light.CachedPosition;
            return diff.magnitude - dynamic_object.boundingSphereRadius <= point_light.LightComponent.range;
        }
    }
}