using System;

namespace ToolBuddy.FrameRateBooster.Optimizations
{
    struct Vector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        //TODO some implicit methods are slower than original

        public static Vector4 op_Implicit(Vector3 v)
        {
            //TODO remove assignation to 0 here and other implicit methods
            Vector4 result;
            result.x = v.x;
            result.y = v.y;
            result.z = v.z;
            result.w = 0.0f;
            return result;
        }

        public static Vector3 op_Implicit(Vector4 v)
        {
            Vector3 result;
            result.x = v.x;
            result.y = v.y;
            result.z = v.z;
            return result;
        }

        public static Vector4 op_Implicit(Vector2 v)
        {
            Vector4 result;
            result.x = v.x;
            result.y = v.y;
            result.z = 0.0f;
            result.w = 0.0f;
            return result;
        }

        //TODO Vector2 op_Implicit(UnityEngine.Vector4 v)
        //public static Vector2 op_Implicit(UnityEngine.Vector4 v)
        //{
        //    Vector2 result;
        //    result.x = v.x;
        //    result.y = v.y;
        //    return result;
        //}


        public static Vector4 op_Addition(Vector4 a, Vector4 b)
        {
            a.x += b.x;
            a.y += b.y;
            a.z += b.z;
            a.w += b.w;
            return a;
        }

        public static Vector4 op_UnaryNegation(Vector4 a)
        {
            Vector4 resutl;
            resutl.x = -a.x;
            resutl.y = -a.y;
            resutl.z = -a.z;
            resutl.w = -a.w;
            return resutl;
        }

        public static Vector4 op_Subtraction(Vector4 a, Vector4 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            a.z -= b.z;
            a.w -= b.w;
            return a;

        }

        public static Vector4 op_Multiply(Vector4 a, float d)
        {
            a.x *= d;
            a.y *= d;
            a.z *= d;
            a.w *= d;
            return a;
        }

        public static Vector4 op_Multiply(float d, Vector4 a)
        {
            a.x *= d;
            a.y *= d;
            a.z *= d;
            a.w *= d;
            return a;
        }

        public static Vector4 op_Division(Vector4 a, float d)
        {
            float inversed = 1 / d;
            a.x *= inversed;
            a.y *= inversed;
            a.z *= inversed;
            a.w *= inversed;
            return a;
        }

        public static float Magnitude(Vector4 vector)
        {
            return (float)Math.Sqrt(vector.x * (double)vector.x + vector.y * (double)vector.y + vector.z * (double)vector.z + vector.w * (double)vector.w);
        }

        public float get_magnitude()
        {
            return (float)Math.Sqrt(this.x * (double)this.x + this.y * (double)this.y + this.z * (double)this.z + this.w * (double)this.w);
        }

        public void Normalize()
        {
            float num = (float)Math.Sqrt(this.x * (double)this.x + this.y * (double)this.y + this.z * (double)this.z + this.w * (double)this.w);
            if (num > 9.99999974737875E-06)
            {
                float inversed = 1 / num;
                this.x *= inversed;
                this.y *= inversed;
                this.z *= inversed;
                this.w *= inversed;
            }
            else
            {
                this.x = 0;
                this.y = 0;
                this.z = 0;
                this.w = 0;
            }
        }

        public static Vector4 Normalize(Vector4 value)
        {
            Vector4 resutl;
            float num = (float)Math.Sqrt(value.x * (double)value.x + value.y * (double)value.y + value.z * (double)value.z + value.w * (double)value.w);
            if (num > 9.99999974737875E-06)
            {
                float inversed = 1 / num;
                resutl.x = value.x * inversed;
                resutl.y = value.y * inversed;
                resutl.z = value.z * inversed;
                resutl.w = value.w * inversed;
            }
            else
            {
                resutl.x = 0;
                resutl.y = 0;
                resutl.z = 0;
                resutl.w = 0;
            }
            return resutl;
        }


        public Vector4 get_normalized()
        {
            Vector4 resutl;
            float num = (float)Math.Sqrt(this.x * (double)this.x + this.y * (double)this.y + this.z * (double)this.z + this.w * (double)this.w);
            if (num > 9.99999974737875E-06)
            {
                float inversed = 1 / num;
                resutl.x = this.x * inversed;
                resutl.y = this.y * inversed;
                resutl.z = this.z * inversed;
                resutl.w = this.w * inversed;
            }
            else
            {
                resutl.x = 0;
                resutl.y = 0;
                resutl.z = 0;
                resutl.w = 0;
            }
            return resutl;
        }

        public static float Distance(Vector4 a, Vector4 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            a.z -= b.z;
            a.w -= b.w;
            return (float)Math.Sqrt(a.x * (double)a.x + a.y * (double)a.y + a.z * (double)a.z + a.w * (double)a.w);
        }

        public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (t < 0.0f)
                t = 0.0f;
            else if (t > 1.0f)
                t = 1f;

            a.x += (b.x - a.x) * t;
            a.y += (b.y - a.y) * t;
            a.z += (b.z - a.z) * t;
            a.w += (b.w - a.w) * t;
            return a;
        }

        public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t)
        {
            a.x += (b.x - a.x) * t;
            a.y += (b.y - a.y) * t;
            a.z += (b.z - a.z) * t;
            a.w += (b.w - a.w) * t;
            return a;
        }

        public static Vector4 Scale(Vector4 a, Vector4 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            a.z *= b.z;
            a.w *= b.w;
            return a;
        }

        public static Vector4 Min(Vector4 lhs, Vector4 rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (lhs.x >= rhs.x)
                lhs.x = rhs.x;
            if (lhs.y >= rhs.y)
                lhs.y = rhs.y;
            if (lhs.z >= rhs.z)
                lhs.z = rhs.z;
            if (lhs.w >= rhs.w)
                lhs.w = rhs.w;
            return lhs;
        }

        public static Vector4 Max(Vector4 lhs, Vector4 rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (lhs.x <= rhs.x)
                lhs.x = rhs.x;
            if (lhs.y <= rhs.y)
                lhs.y = rhs.y;
            if (lhs.z <= rhs.z)
                lhs.z = rhs.z;
            if (lhs.w <= rhs.w)
                lhs.w = rhs.w;
            return lhs;
        }
    }
}
