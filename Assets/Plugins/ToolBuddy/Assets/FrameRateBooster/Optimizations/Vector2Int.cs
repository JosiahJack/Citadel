#if UNITY_2017_2_OR_NEWER
using System;

namespace ToolBuddy.FrameRateBooster.Optimizations
{
    public struct Vector2Int
    {
        public int m_X;
        public int m_Y;

        public static Vector2Int op_Addition(Vector2Int a, Vector2Int b)
        {
            a.m_X += b.m_X;
            a.m_Y += b.m_Y;
            return a;
        }

        public static Vector2Int op_Subtraction(Vector2Int a, Vector2Int b)
        {
            a.m_X -= b.m_X;
            a.m_Y -= b.m_Y;
            return a;

        }

        public static Vector2Int op_Multiply(Vector2Int a, int d)
        {
            a.m_X *= d;
            a.m_Y *= d;
            return a;
        }

        public static Vector2Int op_Multiply(Vector2Int a, Vector2Int b)
        {
            a.m_X *= b.m_X;
            a.m_Y *= b.m_Y;
            return a;
        }

        public float get_magnitude()
        {
            return (float)Math.Sqrt(this.m_X * this.m_X + this.m_Y * this.m_Y);

        }


        public static float Distance(Vector2Int a, Vector2Int b)
        {
            a.m_X -= b.m_X;
            a.m_Y -= b.m_Y;
            return (float)Math.Sqrt(a.m_X * a.m_X + a.m_Y * a.m_Y);
        }

        public static Vector2Int Scale(Vector2Int a, Vector2Int b)
        {
            a.m_X *= b.m_X;
            a.m_Y *= b.m_Y;
            return a;
        }

        public static Vector2Int Min(Vector2Int lhs, Vector2Int rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (lhs.m_X >= rhs.m_X)
                lhs.m_X = rhs.m_X;
            if (lhs.m_Y >= rhs.m_Y)
                lhs.m_Y = rhs.m_Y;
            return lhs;
        }

        public static Vector2Int Max(Vector2Int lhs, Vector2Int rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            if (lhs.m_X <= rhs.m_X)
                lhs.m_X = rhs.m_X;
            if (lhs.m_Y <= rhs.m_Y)
                lhs.m_Y = rhs.m_Y;
            return lhs;
        }

        public static Vector2Int FloorToInt(Vector2 v)
        {
            Vector2Int result;
            result.m_X = (int)Math.Floor(v.x);
            result.m_Y = (int)Math.Floor(v.y);
            return result;
        }

        public static Vector2Int CeilToInt(Vector2 v)
        {
            Vector2Int result;
            result.m_X = (int)Math.Ceiling(v.x);
            result.m_Y = (int)Math.Ceiling(v.y);
            return result;
        }

        public static Vector2Int RoundToInt(Vector2 v)
        {
            Vector2Int result;
            result.m_X = (int)Math.Round(v.x);
            result.m_Y = (int)Math.Round(v.y);
            return result;
        }

        public static Vector2 op_Implicit(Vector2Int v)
        {
            Vector2 result;
            result.x = (float)v.m_X;
            result.y = (float)v.m_Y;
            return result;
        }
    }
}

#endif
