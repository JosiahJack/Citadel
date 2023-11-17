using System;

namespace ToolBuddy.FrameRateBooster.Optimizations
{
    public struct Vector2
    {
        public float x;
        public float y;

        public static Vector2 op_Addition(Vector2 a, Vector2 b)
        {
            a.x += b.x;
            a.y += b.y;
            return a;
        }

        public static Vector2 op_UnaryNegation(Vector2 a)
        {
            a.x = -a.x;
            a.y = -a.y;
            return a;
        }

        public static Vector2 op_Subtraction(Vector2 a, Vector2 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            return a;

        }

        public static Vector2 op_Multiply(Vector2 a, float d)
        {
            a.x *= d;
            a.y *= d;
            return a;
        }

        public static Vector2 op_Multiply(float d, Vector2 a)
        {
            a.x *= d;
            a.y *= d;
            return a;
        }

#if UNITY_2018_1_OR_NEWER
        public static Vector2 op_Multiply(Vector2 a, Vector2 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            return a;
        }


        public static Vector2 op_Division(Vector2 a, Vector2 b)
        {
            a.x /= b.x;
            a.y /= b.y;
            return a;
        }
#endif



        public static Vector2 op_Division(Vector2 a, float d)
        {
            float inversed = 1 / d;
            a.x *= inversed;
            a.y *= inversed;
            return a;
        }


        public static Vector2 op_Implicit(Vector3 v)
        {
            Vector2 result;
            result.x = v.x;
            result.y = v.y;
            return result;
        }

        public static Vector3 op_Implicit(Vector2 v)
        {
            Vector3 result;
            result.x = v.x;
            result.y = v.y;
            result.z = 0.0f;
            return result;
        }



        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (t < 0.0f)
                t = 0.0f;
            else if (t > 1.0f)
                t = 1f;

            a.x += (b.x - a.x) * t;
            a.y += (b.y - a.y) * t;
            return a;
        }

        public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
        {
            a.x += (b.x - a.x) * t;
            a.y += (b.y - a.y) * t;
            return a;
        }

        public static Vector2 Scale(Vector2 a, Vector2 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            return a;
        }

#if UNITY_2018_1_OR_NEWER
        public static Vector2 Perpendicular(Vector2 inDirection)
        {
            Vector2 result;
            result.x = -inDirection.y;
            result.y = inDirection.x;
            return result;
        }
#endif


        public static Vector2 Min(Vector2 lhs, Vector2 rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (lhs.x >= rhs.x)
                lhs.x = rhs.x;
            if (lhs.y >= rhs.y)
                lhs.y = rhs.y;
            return lhs;
        }


        public static Vector2 Max(Vector2 lhs, Vector2 rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (lhs.x <= rhs.x)
                lhs.x = rhs.x;
            if (lhs.y <= rhs.y)
                lhs.y = rhs.y;
            return lhs;
        }

        public float get_magnitude()
        {
            return (float)Math.Sqrt(this.x * (double)this.x + this.y * (double)this.y);
        }

        public void Normalize()
        {
            float num = (float)Math.Sqrt(this.x * (double)this.x + this.y * (double)this.y);
            if (num > 9.99999974737875E-06)
            {
                float inversed = 1 / num;
                this.x *= inversed;
                this.y *= inversed;
            }
            else
            {
                this.x = 0;
                this.y = 0;
            }
        }

        public Vector2 get_normalized()
        {
            Vector2 resutl;
            float num = (float)Math.Sqrt(this.x * (double)this.x + this.y * (double)this.y);
            if (num > 9.99999974737875E-06)
            {
                float inversed = 1 / num;
                resutl.x = this.x * inversed;
                resutl.y = this.y * inversed;
            }
            else
            {
                resutl.x = 0;
                resutl.y = 0;
            }
            return resutl;
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            return (float)Math.Sqrt(a.x * (double)a.x + a.y * (double)a.y);
        }
    }
}