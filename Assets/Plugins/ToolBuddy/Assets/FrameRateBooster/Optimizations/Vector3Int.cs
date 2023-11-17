#if UNITY_2017_2_OR_NEWER
using System;

namespace ToolBuddy.FrameRateBooster.Optimizations
{
    public struct Vector3Int
    {
        private int m_X;
        private int m_Y;
        private int m_Z;

        public static Vector3Int op_Addition(Vector3Int a, Vector3Int b)
        {
            a.m_X += b.m_X;
            a.m_Y += b.m_Y;
            a.m_Z += b.m_Z;
            return a;
        }

        public static Vector3Int op_Subtraction(Vector3Int a, Vector3Int b)
        {
            a.m_X -= b.m_X;
            a.m_Y -= b.m_Y;
            a.m_Z -= b.m_Z;
            return a;

        }

        public static Vector3Int op_Multiply(Vector3Int a, int d)
        {
            a.m_X *= d;
            a.m_Y *= d;
            a.m_Z *= d;
            return a;
        }

        public static Vector3Int op_Multiply(Vector3Int a, Vector3Int b)
        {
            a.m_X *= b.m_X;
            a.m_Y *= b.m_Y;
            a.m_Z *= b.m_Z;
            return a;
        }

        public float get_magnitude()
        {
            return (float)Math.Sqrt(this.m_X * this.m_X + this.m_Y * this.m_Y + this.m_Z * this.m_Z);

        }


        public static float Distance(Vector3Int a, Vector3Int b)
        {
            a.m_X -= b.m_X;
            a.m_Y -= b.m_Y;
            a.m_Z -= b.m_Z;
            return (float)Math.Sqrt(a.m_X * a.m_X + a.m_Y * a.m_Y + a.m_Z * a.m_Z);
        }

        public static Vector3Int Scale(Vector3Int a, Vector3Int b)
        {
            a.m_X *= b.m_X;
            a.m_Y *= b.m_Y;
            a.m_Z *= b.m_Z;
            return a;
        }

        public static Vector3Int Min(Vector3Int lhs, Vector3Int rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (lhs.m_X >= rhs.m_X)
                lhs.m_X = rhs.m_X;
            if (lhs.m_Y >= rhs.m_Y)
                lhs.m_Y = rhs.m_Y;
            if (lhs.m_Z >= rhs.m_Z)
                lhs.m_Z = rhs.m_Z;
            return lhs;
        }

        public static Vector3Int Max(Vector3Int lhs, Vector3Int rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (lhs.m_X <= rhs.m_X)
                lhs.m_X = rhs.m_X;
            if (lhs.m_Y <= rhs.m_Y)
                lhs.m_Y = rhs.m_Y;
            if (lhs.m_Z <= rhs.m_Z)
                lhs.m_Z = rhs.m_Z;
            return lhs;
        }

        public static Vector3Int FloorToInt(Vector3 v)
        {
            Vector3Int result;
            result.m_X = (int)Math.Floor(v.x);
            result.m_Y = (int)Math.Floor(v.y);
            result.m_Z = (int)Math.Floor(v.z);
            return result;
        }

        public static Vector3Int CeilToInt(Vector3 v)
        {
            Vector3Int result;
            result.m_X = (int)Math.Ceiling(v.x);
            result.m_Y = (int)Math.Ceiling(v.y);
            result.m_Z = (int)Math.Ceiling(v.z);
            return result;
        }

        public static Vector3Int RoundToInt(Vector3 v)
        {
            Vector3Int result;
            result.m_X = (int)Math.Round(v.x);
            result.m_Y = (int)Math.Round(v.y);
            result.m_Z = (int)Math.Round(v.z);
            return result;
        }

        public static Vector3 op_Implicit(Vector3Int v)
        {
            Vector3 result;
            result.x = (float)v.m_X;
            result.y = (float)v.m_Y;
            result.z = (float)v.m_Z;
            return result;
        }
    }
}

#endif
