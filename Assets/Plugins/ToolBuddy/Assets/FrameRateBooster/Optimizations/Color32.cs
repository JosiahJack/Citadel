
namespace ToolBuddy.FrameRateBooster.Optimizations
{
    struct Color32
    {

        public byte r;
        public byte g;
        public byte b;
        public byte a;

        static public Color32 op_Implicit(Color c)
        {
            //TODO better branching code?
            //TODO histoire habituel du double transformé en float
            Color32 result;
            float maxValue = byte.MaxValue;
            if (c.r < 0.0f)
                c.r = 0.0f;
            else if (c.r > 1.0f)
                c.r = 1f;
            result.r = (byte)(c.r * maxValue);
            if (c.g < 0.0f)
                c.g = 0.0f;
            else if (c.g > 1.0f)
                c.g = 1f;
            result.g = (byte)(c.g * maxValue);
            if (c.b < 0.0f)
                c.b = 0.0f;
            else if (c.b > 1.0f)
                c.b = 1f;
            result.b = (byte)(c.b * maxValue);
            if (c.a < 0.0f)
                c.a = 0.0f;
            else if (c.a > 1.0f)
                c.a = 1f;
            result.a = (byte)(c.a * maxValue);
            return result;
        }
        public static Color op_Implicit(Color32 c)
        {
            Color result;
            float inverseMaxValue = 1f / (float)byte.MaxValue;
            result.r = (float)c.r * inverseMaxValue;
            result.g = (float)c.g * inverseMaxValue;
            result.b = (float)c.b * inverseMaxValue;
            result.a = (float)c.a * inverseMaxValue;
            return result;
        }

        public static Color32 Lerp(Color32 a, Color32 b, float t)
        {
            if (t < 0.0f)
                t = 0.0f;
            else if (t > 1.0f)
                t = 1f;
            //TODO same comment about why code had conversion to double here
            a.r = (byte)((float)a.r + (float)((int)b.r - (int)a.r) * t);
            a.g = (byte)((float)a.g + (float)((int)b.g - (int)a.g) * t);
            a.b = (byte)((float)a.b + (float)((int)b.b - (int)a.b) * t);
            a.a = (byte)((float)a.a + (float)((int)b.a - (int)a.a) * t);
            return a;
        }
        public static Color32 LerpUnclamped(Color32 a, Color32 b, float t)
        {

            //TODO same comment about why code had conversion to double here
            a.r = (byte)((float)a.r + (float)((int)b.r - (int)a.r) * t);
            a.g = (byte)((float)a.g + (float)((int)b.g - (int)a.g) * t);
            a.b = (byte)((float)a.b + (float)((int)b.b - (int)a.b) * t);
            a.a = (byte)((float)a.a + (float)((int)b.a - (int)a.a) * t);

            return a;
        }
    }
}
