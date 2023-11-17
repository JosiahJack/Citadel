using System;

namespace ToolBuddy.FrameRateBooster.Optimizations
{
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public static Vector3 op_Addition(Vector3 a, Vector3 b)
        {
            a.x += b.x;
            a.y += b.y;
            a.z += b.z;
            return a;
        }

        public static Vector3 op_UnaryNegation(Vector3 a)
        {
            Vector3 resutl;
            resutl.x = -a.x;
            resutl.y = -a.y;
            resutl.z = -a.z;
            return resutl;
        }

        public static Vector3 op_Subtraction(Vector3 a, Vector3 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            a.z -= b.z;
            return a;

        }

        public static Vector3 op_Multiply(Vector3 a, float d)
        {
            a.x *= d;
            a.y *= d;
            a.z *= d;
            return a;
        }

        public static Vector3 op_Multiply(float d, Vector3 a)
        {
            a.x *= d;
            a.y *= d;
            a.z *= d;
            return a;
        }

        public static Vector3 op_Division(Vector3 a, float d)
        {
            float inversed = 1 / d;
            a.x *= inversed;
            a.y *= inversed;
            a.z *= inversed;
            return a;
        }

        public static float Magnitude(Vector3 vector)
        {
            return (float)Math.Sqrt(vector.x * (double)vector.x + vector.y * (double)vector.y + vector.z * (double)vector.z);
        }

        public float get_magnitude()
        {
            return (float)Math.Sqrt(this.x * (double)this.x + this.y * (double)this.y + this.z * (double)this.z);
        }

        public void Normalize()
        {
            float num = (float)Math.Sqrt(this.x * (double)this.x + this.y * (double)this.y + this.z * (double)this.z);
            if (num > 9.99999974737875E-06)
            {
                float inversed = 1 / num;
                this.x *= inversed;
                this.y *= inversed;
                this.z *= inversed;
            }
            else
            {
                this.x = 0;
                this.y = 0;
                this.z = 0;
            }
        }

        public static Vector3 Normalize(Vector3 value)
        {
            Vector3 resutl;
            float num = (float)Math.Sqrt(value.x * (double)value.x + value.y * (double)value.y + value.z * (double)value.z);
            if (num > 9.99999974737875E-06)
            {
                float inversed = 1 / num;
                resutl.x = value.x * inversed;
                resutl.y = value.y * inversed;
                resutl.z = value.z * inversed;
            }
            else
            {
                resutl.x = 0;
                resutl.y = 0;
                resutl.z = 0;
            }
            return resutl;
        }


        public Vector3 get_normalized()
        {
            Vector3 resutl;
            float num = (float)Math.Sqrt(this.x * (double)this.x + this.y * (double)this.y + this.z * (double)this.z);
            if (num > 9.99999974737875E-06)
            {
                float inversed = 1 / num;
                resutl.x = this.x * inversed;
                resutl.y = this.y * inversed;
                resutl.z = this.z * inversed;
            }
            else
            {
                resutl.x = 0;
                resutl.y = 0;
                resutl.z = 0;
            }
            return resutl;
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            a.z -= b.z;
            return (float)Math.Sqrt(a.x * (double)a.x + a.y * (double)a.y + a.z * (double)a.z);
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (t < 0.0f)
                t = 0.0f;
            else if (t > 1.0f)
                t = 1f;

            a.x += (b.x - a.x) * t;
            a.y += (b.y - a.y) * t;
            a.z += (b.z - a.z) * t;
            return a;
        }

        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
        {
            a.x += (b.x - a.x) * t;
            a.y += (b.y - a.y) * t;
            a.z += (b.z - a.z) * t;
            return a;
        }

        public static Vector3 Scale(Vector3 a, Vector3 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            a.z *= b.z;
            return a;
        }

        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            Vector3 result;
            result.x = lhs.y * rhs.z - lhs.z * rhs.y;
            result.y = lhs.z * rhs.x - lhs.x * rhs.z;
            result.z = lhs.x * rhs.y - lhs.y * rhs.x;
            return result;
        }


        public static Vector3 Min(Vector3 lhs, Vector3 rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (lhs.x >= rhs.x)
                lhs.x = rhs.x;
            if (lhs.y >= rhs.y)
                lhs.y = rhs.y;
            if (lhs.z >= rhs.z)
                lhs.z = rhs.z;
            return lhs;
        }

        public static Vector3 Max(Vector3 lhs, Vector3 rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (lhs.x <= rhs.x)
                lhs.x = rhs.x;
            if (lhs.y <= rhs.y)
                lhs.y = rhs.y;
            if (lhs.z <= rhs.z)
                lhs.z = rhs.z;
            return lhs;
        }

        //TODO https://forum.unity.com/threads/c-performance-tips.533831/  ?
    }
}
